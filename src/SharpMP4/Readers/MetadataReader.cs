using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpMP4.Readers
{
    /// <summary>The kind of metadata a tag is.</summary>
    public enum MetadataFamily
    {
        /// <summary>An iTunes item of 'ilst', keyed by its box type (©nam, trkn, covr).</summary>
        ItemList,

        /// <summary>An iTunes '----' item, keyed by its name, in the namespace of its mean (com.apple.iTunes).</summary>
        Freeform,

        /// <summary>A QuickTime keyed item: 'ilst' with a 'keys' table, keyed by the key (com.apple.quicktime.make).</summary>
        Keys,

        /// <summary>An item of 'udta': QuickTime's (©nam, ©day, WLOC), 3GPP's (titl, perf, cprt) or a vendor's, keyed by its box type.</summary>
        UserData,

        /// <summary>
        /// An XMP packet (ISO 16684-1), as text: QuickTime's 'XMP_' in 'udta', or the 'uuid' box of the XMP
        /// Specification Part 3; keyed by its box type.
        /// </summary>
        Xmp,
    }

    /// <summary>A metadata tag as the file has it: its key, language and value.</summary>
    public sealed class MetadataTag
    {
        public MetadataFamily Family { get; set; }

        /// <summary>The track it is in; 0 for the movie's.</summary>
        public uint TrackID { get; set; }

        /// <summary>The box type (ItemList, UserData), the name (Freeform) or the key (Keys).</summary>
        public string Key { get; set; }

        /// <summary>A freeform item's mean, a key's namespace (mdta); null for the others.</summary>
        public string Namespace { get; set; }

        /// <summary>
        /// ISO 639-2/T, with an ISO 3166 country where the value has one (deu-DE); a Macintosh language code
        /// as "mac:n" (QuickTime strings); null where there is none.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The value: string for text; long for integers; double for floats; int[] for the number and total of
        /// trkn and disk, and a window's location; string[] for a kind's scheme and value; MetadataChapter[] for
        /// a chapter list; byte[] for images and what has no well-known type.
        /// </summary>
        public object Value { get; set; }

        /// <summary>The well-known data type of its 'data' box (Apple, QuickTime File Format); -1 where it has none.</summary>
        public int DataType { get; set; } = -1;

        /// <summary>The box the value is in.</summary>
        public Box Box { get; set; }

        public override string ToString() => $"{Family} {(TrackID != 0 ? $"track {TrackID} " : "")}{Key}{(Language != null ? "-" + Language : "")} = {ValueText(Value)}";

        internal static string ValueText(object value) => value switch
        {
            null => "",
            byte[] bytes => $"byte[{bytes.Length}]",
            int[] numbers => string.Join(" ", numbers),
            _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    /// <summary>A chapter of a chapter list: where it starts, and its title.</summary>
    public sealed class MetadataChapter
    {
        public TimeSpan Start { get; set; }

        public string Title { get; set; }

        public override string ToString() => $"{Start} {Title}";
    }

    /// <summary>
    /// Reads the metadata tags of a file: the iTunes item list, QuickTime keyed metadata and the strings of
    /// 'udta', of the movie and of each track; and XMP, in a box or an item. An item's bytes are read from
    /// the file, which is then to be open still.
    /// </summary>
    public static class MetadataReader
    {
        public static List<MetadataTag> Read(Container file)
        {
            var tags = new List<MetadataTag>();
            foreach (var xmp in file.Children.OfType<XMPCustomBox>())
                tags.Add(Xmp(xmp, 0, xmp.Data.Bytes));
            foreach (var meta in file.Children.OfType<MetaBox>())
                ReadItems(file, meta, tags);
            foreach (var moov in file.Children.OfType<MovieBox>())
            {
                ReadScope(moov.Children, 0, tags);
                foreach (var trak in moov.Children.OfType<TrackBox>())
                {
                    uint trackID = trak.Children.OfType<TrackHeaderBox>().FirstOrDefault()?.TrackID ?? 0;
                    ReadScope(trak.Children, trackID, tags);
                }
            }
            return tags;
        }

        // The 'meta' and 'udta' of the movie or of a track
        private static void ReadScope(IEnumerable<Box> children, uint trackID, List<MetadataTag> tags)
        {
            foreach (var box in children)
            {
                if (box is MetaBox meta)
                {
                    ReadMeta(meta, trackID, tags);
                }
                else if (box is XMPCustomBox xmp)
                {
                    tags.Add(Xmp(xmp, trackID, xmp.Data.Bytes));
                }
                else if (box is UserDataBox udta && udta.Children != null)
                {
                    foreach (var item in udta.Children)
                    {
                        if (item is MetaBox udtaMeta)
                            ReadMeta(udtaMeta, trackID, tags);
                        else
                            ReadUserData(item, trackID, tags);
                    }
                }
            }
        }

        private static void ReadMeta(MetaBox meta, uint trackID, List<MetadataTag> tags)
        {
            if (meta.Children == null)
                return;

            // With a 'keys' table, an item's type is the index of its key, from 1
            var keys = meta.Children.OfType<MetaDataKeyTableBox>().FirstOrDefault();
            var keyNames = new List<(string Namespace, string Key)>();
            if (keys?.Children != null)
            {
                foreach (var key in keys.Children)
                {
                    string ns = IsoStream.ToFourCC(key.FourCC);
                    string name = key is MdtaBox mdta ? UpToZero(Encoding.UTF8.GetString(mdta.Data ?? Array.Empty<byte>())) : null;
                    keyNames.Add((ns, name));
                }
            }

            foreach (var ilst in meta.Children.OfType<AppleItemListBox>())
            {
                if (ilst.Children == null)
                    continue;
                foreach (var item in ilst.Children)
                {
                    if (keys != null)
                    {
                        uint index = item.FourCC;
                        if (index >= 1 && index <= keyNames.Count)
                        {
                            var (ns, key) = keyNames[(int)index - 1];
                            AddItems(item, MetadataFamily.Keys, key, ns, trackID, tags, keyed: true);
                        }
                    }
                    else if (item is CustomBox freeform)
                    {
                        ReadFreeform(freeform, trackID, tags);
                    }
                    else
                    {
                        AddItems(item, MetadataFamily.ItemList, IsoStream.ToFourCC(item.FourCC), null, trackID, tags, keyed: false);
                    }
                }
            }
        }

        // An item's 'data' boxes, each a value of its own
        private static void AddItems(Box item, MetadataFamily family, string key, string ns, uint trackID, List<MetadataTag> tags, bool keyed)
        {
            if (item.Children == null)
                return;
            foreach (var data in item.Children.OfType<DataBox>())
                tags.Add(Tag(family, key, ns, trackID, data.DataType, data.DataLang, data.Data, data, keyed));
        }

        private static MetadataTag Tag(MetadataFamily family, string key, string ns, uint trackID, uint type, uint locale, byte[] bytes, Box box, bool keyed)
        {
            return new MetadataTag
            {
                Family = family,
                Key = key,
                Namespace = ns,
                TrackID = trackID,
                DataType = (int)(type & 0xFFFFFF),
                Language = keyed ? Locale(locale) : null,
                Value = DataValue(key, (int)(type & 0xFFFFFF), bytes ?? Array.Empty<byte>()),
                Box = box,
            };
        }

        /// <summary>'----': a 'mean', a 'name' and 'data' boxes; its name is the key, in the namespace of its mean.</summary>
        private static void ReadFreeform(CustomBox freeform, uint trackID, List<MetadataTag> tags)
        {
            if (freeform.Children == null)
                return;
            string mean = freeform.Children.OfType<ITunesMetadataMeanBox>().FirstOrDefault()?.Domain.Text;
            string name = freeform.Children.OfType<ITunesMetadataNameBox>().FirstOrDefault()?.Name.Text;
            foreach (var data in freeform.Children.OfType<DataBox>())
                tags.Add(Tag(MetadataFamily.Freeform, name, mean, trackID, data.DataType, data.DataLang, data.Data, data, keyed: false));
        }

        /// <summary>
        /// An item of 'udta': QuickTime's strings, each in the languages it has; 3GPP's strings and rating;
        /// the settings QuickTime keeps there; Nero's chapters; and the payload of whatever else it holds.
        /// </summary>
        private static void ReadUserData(Box item, uint trackID, List<MetadataTag> tags)
        {
            string key = IsoStream.ToFourCC(item.FourCC);
            switch (item)
            {
                case FreeSpaceBox:
                case FreeSpaceBoxskipDup:
                    // room left, not a tag
                    break;
                case Box when key is "hnti" or "hinf":
                    // a hint track's information (ISO/IEC 14496-12 9.1.4, 9.1.5): structure, not a tag
                    break;
                case XMPBox b:
                    tags.Add(Xmp(b, trackID, b.Data));
                    break;
                case QuickTimeTextBox text when text.Value != null:
                    AddQuickTime(text.Value);
                    break;
                case AppleTrackTypeBox trackType when trackType.Value != null:
                    AddQuickTime(trackType.Value);
                    break;
                case AdzcBox b when b.Value != null: AddQuickTime(b.Value); break;
                case AdzeBox b when b.Value != null: AddQuickTime(b.Value); break;
                case AdzmBox b when b.Value != null: AddQuickTime(b.Value); break;
                case CollectionNameBox b: Add3gpp(b.Language, b.Value); break;
                // cameras' strings (ExifTool, QuickTime UserData: Format string)
                case CameraAngleBox b: Add(UpToZero(b.Value.Text)); break;
                case CameraIDBox b: Add(UpToZero(b.Value.Text)); break;
                case CanonFirmwareVersionBox b: Add(UpToZero(b.Value.Text)); break;
                case ClfnBox b: Add(UpToZero(b.Value.Text)); break;
                case ClipIDBox b: Add(UpToZero(b.Value.Text)); break;
                case FirmwareVersionInfoBox b: Add(UpToZero(b.Value.Text)); break;
                case ReelNameBox b: Add(UpToZero(b.Value.Text)); break;
                case SceneBox b: Add(UpToZero(b.Value.Text)); break;
                case SerialNumberBox b: Add(UpToZero(b.Value.Text)); break;
                case ShotNameBox b: Add(UpToZero(b.Value.Text)); break;
                // cameras' rationals (ExifTool: Format rational64s), as the number they are
                case CxBox b: AddRational(b.Numerator, b.Denominator); break;
                case CyBox b: AddRational(b.Numerator, b.Denominator); break;
                case LevelMeterBox b: AddRational(b.Numerator, b.Denominator); break;
                case LevelMeter2Box b: AddRational(b.Numerator, b.Denominator); break;
                case PitchBox b: AddRational(b.Numerator, b.Denominator); break;
                case RadsBox b: AddRational(b.Numerator, b.Denominator); break;
                case YawBox b: AddRational(b.Numerator, b.Denominator); break;
                case ThreeGPPTitleBox b: Add3gpp(b.Language, b.Value); break;
                case ThreeGPPAlbumBox b: Add3gpp(b.Language, b.Value); break;
                case ThreeGPPGenreBox b: Add3gpp(b.Language, b.Value); break;
                case ThreeGPPPerformerBox b: Add3gpp(b.Language, b.Value); break;
                case ThreeGPPAuthorBox b: Add3gpp(b.Language, b.Value); break;
                case ThreeGPPDescriptionBox b: Add3gpp(b.Language, b.Value); break;
                case CopyrightBox b: Add3gpp(b.Language, b.Notice); break;
                case ThreeGPPRatingBox b: Add3gpp(b.Language, b.RatingInfo); break;
                case ThreeGPPRecordingYearBox b: Add((long)b.Year); break;
                // QTFF, User Data Atoms: the name of the movie or track, and the settings of QuickTime Player
                case AppleName2Box b: Add(UpToZero(b.Name.Text)); break;
                case AppleWindowLocationBox b: Add(new int[] { b.LocationX, b.LocationY }); break;
                case AppleLoopingBox b: Add((long)b.Data); break;
                case AppleSelectionOnlyBox b: Add((long)b.Data); break;
                case PlayAllFramesBox b: Add((long)b.Data); break;
                case AppleHintVersionBox b: Add(UpToZero(b.Version.Text)); break;
                case AppleApertureModeBox b: Add(UpToZero(b.Mode.Text)); break;
                // ISO/IEC 14496-12 8.10.4: a role, by its scheme
                case KindBox b: Add(new[] { UpToZero(b.SchemeURI.Text), UpToZero(b.Value.Text) }); break;
                case AdobeChapterBox b:
                    Add((b.Chapters ?? Array.Empty<AdobeChapterRecord>())
                        .Select(c => new MetadataChapter { Start = TimeSpan.FromTicks((long)c.Timestamp), Title = Encoding.UTF8.GetString(c.Title ?? Array.Empty<byte>()) })
                        .ToArray());
                    break;
                default:
                    // A vendor's, or a structure no one reads as a tag: its payload as it is
                    Add(Payload(item));
                    break;
            }

            void Add(object value) =>
                tags.Add(new MetadataTag { Family = MetadataFamily.UserData, Key = key, TrackID = trackID, Value = value, Box = item });

            void AddRational(int numerator, int denominator) =>
                Add(denominator != 0 ? (double)numerator / denominator : (object)new[] { numerator, denominator });

            void AddQuickTime(MultiLanguageString[] strings)
            {
                foreach (var s in strings)
                    tags.Add(new MetadataTag { Family = MetadataFamily.UserData, Key = key, TrackID = trackID, Language = QuickTimeLanguage(s.Language), Value = QuickTimeText(s.Language, s.Value.Bytes), Box = item });
            }

            // a packed language of 0 ("```") is none
            void Add3gpp(string language, BinaryUTF8String value) =>
                tags.Add(new MetadataTag { Family = MetadataFamily.UserData, Key = key, TrackID = trackID, Language = language == "```" ? null : language, Value = String3gpp(value.Bytes), Box = item });
        }

        /// <summary>
        /// The XMP items of a 'meta' (HEIF, ISO/IEC 23008-12 A.2): items of type 'mime' and content type
        /// application/rdf+xml.
        /// </summary>
        private static void ReadItems(Container file, MetaBox meta, List<MetadataTag> tags)
        {
            var iinf = meta.Children?.OfType<ItemInfoBox>().FirstOrDefault();
            if (iinf == null)
                return;
            foreach (var infe in iinf.ItemInfos)
            {
                if (infe.ItemType == IsoStream.FromFourCC("mime") && UpToZero(infe.ContentType.Text) == "application/rdf+xml")
                {
                    byte[] packet = ItemData(file, meta, infe.ItemID);
                    if (packet != null)
                        tags.Add(Xmp(infe, 0, packet));
                }
            }
        }

        /// <summary>
        /// An item's bytes (ISO/IEC 14496-12 8.11.3): its extents, in the file (construction method 0) or in
        /// the 'idat' (1); null for an item elsewhere.
        /// </summary>
        private static byte[] ItemData(Container file, MetaBox meta, uint itemID)
        {
            var iloc = meta.Children.OfType<ItemLocationBox>().FirstOrDefault();
            if (iloc?.ItemID == null)
                return null;
            int index = Array.IndexOf(iloc.ItemID, itemID);
            if (index < 0)
                return null;
            int method = iloc.ConstructionMethod != null ? iloc.ConstructionMethod[index] : 0;
            if (method > 1 || iloc.DataReferenceIndex != null && iloc.DataReferenceIndex[index] != 0)
                return null;

            var idat = meta.Children.OfType<ItemDataBox>().FirstOrDefault();
            StreamMarker marker = method == 1 ? idat?.Data : file.Children.OfType<MediaDataBox>().FirstOrDefault(m => m.Data != null)?.Data ?? idat?.Data;
            if (marker?.Stream == null)
                return null;
            long origin = method == 1 ? marker.Position : 0;
            long baseOffset = (long)UnsignedInteger(iloc.BaseOffset[index]);

            var data = new List<byte>();
            IsoStream stream = marker.Stream;
            long position = stream.GetCurrentOffset();
            try
            {
                for (int i = 0; i < iloc.ExtentCount[index]; i++)
                {
                    long offset = origin + baseOffset + (long)UnsignedInteger(iloc.ExtentOffset[index][i]);
                    long length = (long)UnsignedInteger(iloc.ExtentLength[index][i]);
                    // an extent of length 0 is the rest of its source
                    if (length == 0)
                        length = (method == 1 ? marker.Position + marker.Length : stream.GetStreamLength()) - offset;
                    stream.SeekFromBeginning(offset);
                    stream.ReadBytes((ulong)length, out byte[] extent);
                    data.AddRange(extent);
                }
            }
            finally
            {
                stream.SeekFromBeginning(position);
            }
            return data.ToArray();
        }

        private static MetadataTag Xmp(Box box, uint trackID, byte[] packet) =>
            new MetadataTag { Family = MetadataFamily.Xmp, Key = IsoStream.ToFourCC(box.FourCC), TrackID = trackID, Value = UpToZero(Encoding.UTF8.GetString(packet ?? Array.Empty<byte>())), Box = box };

        /// <summary>A box's bytes after its size and type: what a box read as bytes holds, or what the box writes.</summary>
        public static byte[] Payload(Box box)
        {
            using var memory = new MemoryStream();
            new IsoStream(new StreamWrapper(memory)).WriteBox(box, "");
            byte[] bytes = memory.ToArray();
            int header = bytes.Length >= 8 && bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 1 ? 16 : 8;
            return bytes.Skip(Math.Min(header, bytes.Length)).ToArray();
        }

        private static string UpToZero(string text)
        {
            int zero = text.IndexOf('\0');
            return zero < 0 ? text : text.Substring(0, zero);
        }

        /// <summary>
        /// A QuickTime string (QuickTime File Format, User data text): in a Macintosh language, the Macintosh
        /// text encoding - but English (0) that is UTF-8, as writers put it; in an ISO 639-2/T language, UTF-8,
        /// or UTF-16 after its byte order mark. Without the zeros after it.
        /// </summary>
        private static string QuickTimeText(ushort language, byte[] bytes)
        {
            bytes ??= Array.Empty<byte>();
            string text;
            bool utf16 = bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF;
            if ((language < 0x400 || language == 0x7fff) && !utf16)
                text = language == 0 && IsUtf8(bytes) ? Encoding.UTF8.GetString(bytes) : MacRoman(bytes);
            else if (utf16)
                text = Encoding.BigEndianUnicode.GetString(bytes, 2, (bytes.Length - 2) & ~1);
            else
                text = Encoding.UTF8.GetString(bytes);
            return text.TrimEnd('\0');
        }

        /// <summary>Whether bytes are UTF-8 with something past ASCII in them.</summary>
        private static bool IsUtf8(byte[] bytes)
        {
            bool beyondAscii = false;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                if (b < 0x80)
                    continue;
                int follow = b >= 0xF0 && b < 0xF8 ? 3 : b >= 0xE0 ? 2 : b >= 0xC2 && b < 0xE0 ? 1 : -1;
                if (b >= 0xF8 || follow < 0 || i + follow >= bytes.Length)
                    return false;
                for (int j = 1; j <= follow; j++)
                {
                    if ((bytes[i + j] & 0xC0) != 0x80)
                        return false;
                }
                i += follow;
                beyondAscii = true;
            }
            return beyondAscii;
        }

        // Mac OS Roman, 0x80 to 0xFF (Unicode's mapping, MAPPINGS/VENDORS/APPLE/ROMAN.TXT)
        private const string MacRomanHigh =
            "\u00C4\u00C5\u00C7\u00C9\u00D1\u00D6\u00DC\u00E1\u00E0\u00E2\u00E4\u00E3\u00E5\u00E7\u00E9\u00E8" +
            "\u00EA\u00EB\u00ED\u00EC\u00EE\u00EF\u00F1\u00F3\u00F2\u00F4\u00F6\u00F5\u00FA\u00F9\u00FB\u00FC" +
            "\u2020\u00B0\u00A2\u00A3\u00A7\u2022\u00B6\u00DF\u00AE\u00A9\u2122\u00B4\u00A8\u2260\u00C6\u00D8" +
            "\u221E\u00B1\u2264\u2265\u00A5\u00B5\u2202\u2211\u220F\u03C0\u222B\u00AA\u00BA\u03A9\u00E6\u00F8" +
            "\u00BF\u00A1\u00AC\u221A\u0192\u2248\u2206\u00AB\u00BB\u2026\u00A0\u00C0\u00C3\u00D5\u0152\u0153" +
            "\u2013\u2014\u201C\u201D\u2018\u2019\u00F7\u25CA\u00FF\u0178\u2044\u20AC\u2039\u203A\uFB01\uFB02" +
            "\u2021\u00B7\u201A\u201E\u2030\u00C2\u00CA\u00C1\u00CB\u00C8\u00CD\u00CE\u00CF\u00CC\u00D3\u00D4" +
            "\uF8FF\u00D2\u00DA\u00DB\u00D9\u0131\u02C6\u02DC\u00AF\u02D8\u02D9\u02DA\u00B8\u02DD\u02DB\u02C7";

        private static string MacRoman(byte[] bytes)
        {
            var text = new StringBuilder(bytes.Length);
            foreach (byte b in bytes)
                text.Append(b < 0x80 ? (char)b : MacRomanHigh[b - 0x80]);
            return text.ToString();
        }

        /// <summary>A 3GPP string (3GPP TS 26.244 8.x): UTF-8, or UTF-16 after a byte order mark; ending at a zero.</summary>
        private static string String3gpp(byte[] bytes)
        {
            if (bytes == null)
                return "";
            if (bytes.Length >= 2 && (bytes[0] == 0xFE && bytes[1] == 0xFF || bytes[0] == 0xFF && bytes[1] == 0xFE))
            {
                var encoding = bytes[0] == 0xFE ? Encoding.BigEndianUnicode : Encoding.Unicode;
                string text = encoding.GetString(bytes, 2, (bytes.Length - 2) & ~1);
                int zero = text.IndexOf('\0');
                return zero < 0 ? text : text.Substring(0, zero);
            }
            return new BinaryUTF8String(bytes).Text;
        }

        /// <summary>
        /// A 'data' value by its well-known type (QuickTime File Format, Well-known types); binary (0) as the
        /// layouts iTunes gives trkn, disk and gnre.
        /// </summary>
        private static object DataValue(string key, int type, byte[] bytes)
        {
            switch (type)
            {
                case 1: // UTF-8
                case 4: // UTF-8, sort
                    return Encoding.UTF8.GetString(bytes);
                case 2: // UTF-16
                case 5: // UTF-16, sort
                    return Encoding.BigEndianUnicode.GetString(bytes);
                case 21: // big-endian signed integer, 1 to 8 bytes
                case 65: case 66: case 67: case 74:
                    return bytes.Length is 1 or 2 or 3 or 4 or 8 ? SignedInteger(bytes) : (object)bytes;
                case 22: // big-endian unsigned integer
                case 75: case 76: case 77: case 78:
                    return bytes.Length is 1 or 2 or 3 or 4 or 8 ? (long)UnsignedInteger(bytes) : (object)bytes;
                case 23: // big-endian float32
                    return bytes.Length == 4 ? (double)BitConverter.ToSingle(BigEndian(bytes), 0) : (object)bytes;
                case 24: // big-endian float64
                    return bytes.Length == 8 ? BitConverter.ToDouble(BigEndian(bytes), 0) : (object)bytes;
                case 0:
                    // trkn and disk: reserved(16) number(16) total(16) [reserved(16)]; gnre: the ID3v1 genre, from 1
                    if ((key == "trkn" || key == "disk") && bytes.Length >= 6)
                        return new[] { (bytes[2] << 8) | bytes[3], (bytes[4] << 8) | bytes[5] };
                    if (key == "gnre" && bytes.Length == 2)
                        return (long)((bytes[0] << 8) | bytes[1]);
                    return bytes;
                default:
                    // images (13 JPEG, 14 PNG, 27 BMP) and the rest
                    return bytes;
            }
        }

        private static long SignedInteger(byte[] bytes)
        {
            long value = (sbyte)bytes[0];
            for (int i = 1; i < bytes.Length; i++)
                value = (value << 8) | bytes[i];
            return value;
        }

        private static ulong UnsignedInteger(byte[] bytes)
        {
            ulong value = 0;
            foreach (byte b in bytes ?? Array.Empty<byte>())
                value = (value << 8) | b;
            return value;
        }

        private static byte[] BigEndian(byte[] bytes)
        {
            var copy = (byte[])bytes.Clone();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(copy);
            return copy;
        }

        /// <summary>
        /// A 'data' box's locale: its country (16 bits, ISO 3166) and language (16 bits: a Macintosh code below
        /// 0x400, packed ISO 639-2/T above); null where both are the default.
        /// </summary>
        private static string Locale(uint locale)
        {
            int country = (int)(locale >> 16), language = (int)(locale & 0xFFFF);
            string lang = language == 0 ? null : QuickTimeLanguage((ushort)language);
            string ctry = country is 0 or 0x7fff ? null : new string(new[] { (char)(country >> 8), (char)(country & 0xFF) });
            if (lang == null && ctry == null)
                return null;
            return ctry == null ? lang : $"{lang ?? "und"}-{ctry}";
        }

        /// <summary>A QuickTime language code: packed ISO 639-2/T from 0x400, a Macintosh code below and "unspecified" (0x7fff) as "mac:n".</summary>
        private static string QuickTimeLanguage(ushort code)
        {
            if (code < 0x400 || code == 0x7fff)
                return "mac:" + code;
            return new string(new[] { (char)(((code >> 10) & 0x1f) + 0x60), (char)(((code >> 5) & 0x1f) + 0x60), (char)((code & 0x1f) + 0x60) });
        }
    }
}
