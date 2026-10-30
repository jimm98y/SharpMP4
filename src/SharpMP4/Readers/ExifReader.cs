using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpMP4.Readers
{
    /// <summary>The names of the tags of Exif, of their IFDs, as <see cref="ContainerExtensions.ReadMetadata"/> keys them.</summary>
    public static class ExifTags
    {
        /// <summary>
        /// The name of a tag of an IFD - IFD0, IFD1, Exif, GPS, Interop, or a maker's: Apple, Canon, Nikon - of Exif 3.0,
        /// TIFF 6.0 and ExifTool's of the makers' notes; of one not known, its number, 0x010F.
        /// </summary>
        public static string NameOf(string ifd, ushort tag) => ExifReader.NameOf(ifd, tag);
    }

    /// <summary>
    /// A text of Exif of a character code (Exif 4.6.5 - UserComment, GPSProcessingMethod, GPSAreaInformation): 8 bytes
    /// that say which - "ASCII\0\0\0", "UNICODE\0", "JIS\0\0\0\0\0", or zeros or spaces of none said - then the text in it.
    /// Kept as the bytes it was read of, code and all, which write back as they were; the text told of them.
    /// </summary>
    public readonly struct CharacterCodedString
    {
        public CharacterCodedString(byte[] bytes, bool littleEndian)
        {
            Bytes = bytes ?? Array.Empty<byte>();
            IsLittleEndian = littleEndian;
        }

        public byte[] Bytes { get; }

        /// <summary>Of UNICODE, whether the text is little endian: of the byte order of the TIFF header, as cameras write it.</summary>
        public bool IsLittleEndian { get; }

        /// <summary>The 8 bytes of the code, as they are.</summary>
        public string Code => Encoding.ASCII.GetString(Bytes, 0, Math.Min(8, Bytes.Length));

        /// <summary>
        /// The text after the code, all of it: of UNICODE, UCS-2; of JIS, not decoded, its bytes in hex; of ASCII and of
        /// none said, as UTF-8, which ASCII is.
        /// </summary>
        public string Text
        {
            get
            {
                if (Bytes.Length <= 8)
                    return "";
                return Code switch
                {
                    "UNICODE\0" => (IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode).GetString(Bytes, 8, Bytes.Length - 8),
                    "JIS\0\0\0\0\0" => BitConverter.ToString(Bytes, 8).Replace("-", ""),
                    _ => Encoding.UTF8.GetString(Bytes, 8, Bytes.Length - 8),
                };
            }
        }

        /// <summary>The code, as it is, then the text.</summary>
        public override string ToString() => Code + Text;
    }

    /// <summary>
    /// The tags of Exif (CIPA DC-008, Exif 3.0) - of an Exif item of a HEIF image (ISO/IEC 23008-12 A.2.1) or the 'Exif'
    /// box of a JPEG XL file (ISO/IEC 18181-2): an offset to the TIFF header, then TIFF (TIFF 6.0). Its 0th IFD, the Exif,
    /// GPS and interoperability IFDs it points to, and the 1st IFD - of the thumbnail, which is a tag of its own. Each tag
    /// keyed by its name, of its IFD; its value as it is - ASCII of its terminating zero, a rational as numerator/denominator
    /// - and where it is in the file; what its value means, beside it.
    /// </summary>
    internal static class ExifReader
    {
        /// <param name="data">The item's bytes, or the box's payload: the offset of the TIFF header, then the Exif - or the TIFF alone.</param>
        /// <param name="fileOffsetOf">Where a byte of <paramref name="data"/> is in the file; -1 where that is not known.</param>
        /// <param name="firstIfd">
        /// What the TIFF's first IFD is: IFD0, the 1st after it - or, of Canon's CMT2 and CMT4 of a CR3, the Exif or GPS IFD
        /// alone.
        /// </param>
        /// <param name="readFile">The bytes of the file at an offset, of a length: of a thumbnail past the item's bytes.</param>
        public static void Read(byte[] data, Box box, List<MetadataTag> tags, Func<long, long> fileOffsetOf, string firstIfd = "IFD0",
            Func<long, long, byte[]> readFile = null)
        {
            if (data == null || data.Length < 8)
                return;
            // exif_tiff_header_offset: from after it, where the TIFF header is - after "Exif\0\0", as JPEG's APP1 has it.
            // Of files made before HEIF had the field - Nokia's conformance files - the TIFF header first, or "Exif\0\0".
            long tiff = 4 + ((long)data[0] << 24 | (long)data[1] << 16 | (long)data[2] << 8 | data[3]);
            if (IsTiffHeader(data, 0))
                tiff = 0;
            else if (data[0] == 'E' && data[1] == 'x' && data[2] == 'i' && data[3] == 'f' && data[4] == 0 && data[5] == 0 && IsTiffHeader(data, 6))
                tiff = 6;
            if (tiff + 8 > data.Length)
                return;
            bool little;
            if (data[tiff] == 'I' && data[tiff + 1] == 'I')
                little = true;
            else if (data[tiff] == 'M' && data[tiff + 1] == 'M')
                little = false;
            else
                return;

            var reader = new Tiff(data, (int)tiff, little, box, tags, fileOffsetOf, readFile);
            if (reader.UInt16(2) != 42)
                return;

            // each IFD once, whatever points to it: a loop of offsets reads no IFD twice
            uint next = reader.ReadIfd(reader.UInt32(4), firstIfd);
            if (next != 0 && firstIfd == "IFD0")
                reader.ReadIfd(next, "IFD1");
        }

        // "II*\0" or "MM\0*": a TIFF header, of either byte order
        private static bool IsTiffHeader(byte[] data, int at) =>
            data.Length >= at + 8 && (data[at] == 'I' && data[at + 1] == 'I' && data[at + 2] == 42 && data[at + 3] == 0
                || data[at] == 'M' && data[at + 1] == 'M' && data[at + 2] == 0 && data[at + 3] == 42);

        private sealed class Tiff
        {
            private readonly byte[] _bytes;
            private readonly int _start;
            private readonly bool _little;
            private readonly Box _box;
            private readonly List<MetadataTag> _tags;
            private readonly Func<long, long> _fileOffsetOf;
            private readonly Func<long, long, byte[]> _readFile;
            private readonly HashSet<long> _visited = new HashSet<long>();

            public Tiff(byte[] bytes, int start, bool little, Box box, List<MetadataTag> tags, Func<long, long> fileOffsetOf, Func<long, long, byte[]> readFile)
            {
                _bytes = bytes;
                _start = start;
                _little = little;
                _box = box;
                _tags = tags;
                _fileOffsetOf = fileOffsetOf;
                _readFile = readFile;
            }

            private bool Has(long offset, long length) => offset >= 0 && length >= 0 && _start + offset + length <= _bytes.Length;

            public ushort UInt16(long offset)
            {
                int at = (int)(_start + offset);
                return _little ? (ushort)(_bytes[at] | _bytes[at + 1] << 8) : (ushort)(_bytes[at] << 8 | _bytes[at + 1]);
            }

            public uint UInt32(long offset)
            {
                int at = (int)(_start + offset);
                return _little
                    ? (uint)(_bytes[at] | _bytes[at + 1] << 8 | _bytes[at + 2] << 16 | _bytes[at + 3] << 24)
                    : (uint)(_bytes[at] << 24 | _bytes[at + 1] << 16 | _bytes[at + 2] << 8 | _bytes[at + 3]);
            }

            private byte[] Bytes(long offset, long length)
            {
                var bytes = new byte[length];
                Buffer.BlockCopy(_bytes, (int)(_start + offset), bytes, 0, (int)length);
                return bytes;
            }

            // where an offset of the TIFF header is in the file: of the item's bytes where it is in them, else - a thumbnail
            // past the item, as Panasonic's are - of where the header is; -1 where neither is known
            private long FileOffsetOfTiff(long offset)
            {
                long inItem = _fileOffsetOf(_start + offset);
                if (inItem >= 0)
                    return inItem;
                long header = _fileOffsetOf(_start);
                return header >= 0 ? header + offset : -1;
            }

            private void Add(string ifd, ushort tag, string key, object value, long offset, long length, long fileOffset = -2)
            {
                string description = value == null ? null : Describe(ifd, key, value);
                // offsets of the TIFF header - of strips, tiles, a thumbnail - and where they are in the file
                if (key is "StripOffsets" or "TileOffsets" or "JPEGInterchangeFormat" && value is long or long[])
                {
                    long[] offsets = value is long one ? new[] { one } : (long[])value;
                    var inFile = offsets.Select(FileOffsetOfTiff).ToArray();
                    if (inFile.All(o => o >= 0))
                        description = "in the file at " + string.Join(" ", inFile);
                }
                _tags.Add(new MetadataTag
                {
                    Family = MetadataFamily.Exif,
                    Namespace = ifd,
                    Id = tag,
                    Key = key,
                    Value = value,
                    Description = description,
                    Box = _box,
                    Offset = value == null ? -1 : fileOffset != -2 ? fileOffset : _fileOffsetOf(_start + offset),
                    Length = value == null ? 0 : length,
                });
            }

            /// <summary>The tags of an IFD, and of the IFDs it points to; the offset of the IFD after it, 0 of none.</summary>
            public uint ReadIfd(uint offset, string ifd)
            {
                if (offset == 0 || !_visited.Add(offset) || !Has(offset, 2))
                    return 0;
                int count = UInt16(offset);
                if (!Has(offset + 2, count * 12L + 4))
                    return 0;
                long thumbnail = -1, thumbnailLength = -1;
                for (int i = 0; i < count; i++)
                {
                    long entry = offset + 2 + i * 12L;
                    ushort tag = UInt16(entry);
                    ushort type = UInt16(entry + 2);
                    uint values = UInt32(entry + 4);

                    string sub = ifd is "IFD0" or "IFD1" or "Exif" ? tag switch { 0x8769 => "Exif", 0x8825 => "GPS", 0xA005 => "Interop", _ => null } : null;
                    if (sub != null)
                    {
                        ReadIfd(UInt32(entry + 8), sub);
                        continue;
                    }

                    int size = SizeOf(type);
                    long length = size * (long)values;
                    // of 4 bytes or fewer, the value itself; else where it is
                    long at = length <= 4 ? entry + 8 : UInt32(entry + 8);
                    object value = size == 0 || !Has(at, length) ? null : Value(ifd, tag, type, values, at);
                    Add(ifd, tag, NameOf(ifd, tag), value, at, length);

                    // the maker's notes: kept as their bytes, and their tags of the maker's IFD, where it is one known
                    if (ifd == "Exif" && tag == 0x927C && value is byte[] note)
                        ReadMakerNote(at, note);

                    if (ifd == "IFD1" && tag == 0x0201 && value is long start)
                        thumbnail = start;
                    else if (ifd == "IFD1" && tag == 0x0202 && value is long size2)
                        thumbnailLength = size2;
                }

                // the thumbnail: a JPEG of the offset and length IFD1 says - of the file where it is past the item's bytes,
                // as Panasonic's are
                if (thumbnail >= 0 && thumbnailLength > 0 && Has(thumbnail, thumbnailLength))
                {
                    Add(ifd, 0x0201, "ThumbnailImage", Bytes(thumbnail, thumbnailLength), thumbnail, thumbnailLength);
                }
                else if (thumbnail >= 0 && thumbnailLength > 0 && _readFile != null && FileOffsetOfTiff(thumbnail) is long at && at >= 0)
                {
                    byte[] image = _readFile(at, thumbnailLength);
                    if (image != null && image.Length == thumbnailLength)
                        Add(ifd, 0x0201, "ThumbnailImage", image, thumbnail, thumbnailLength, at);
                }

                return UInt32(offset + 2 + count * 12L);
            }

            /// <summary>
            /// The tags of a MakerNote, of the maker's IFD (ExifTool's MakerNotes.pm): Apple's - "Apple iOS\0", a version,
            /// "MM", then the IFD, its offsets of the note; Nikon's of type 3 - "Nikon\0", a version, then a TIFF header of
            /// its own; Canon's - the IFD alone, its offsets of the TIFF header, as the Exif's are. Of another maker's,
            /// nothing but its bytes.
            /// </summary>
            private void ReadMakerNote(long at, byte[] note)
            {
                bool StartsWith(string text) =>
                    note.Length >= text.Length && Encoding.ASCII.GetString(note, 0, text.Length) == text;

                try
                {
                    if (StartsWith("Apple iOS\0") && note.Length >= 16 && note[12] == 'M' && note[13] == 'M')
                    {
                        var apple = new Tiff(_bytes, (int)(_start + at), little: false, _box, _tags, _fileOffsetOf, _readFile);
                        apple.ReadIfd(14, "Apple");
                    }
                    else if (StartsWith("Nikon\0") && note.Length >= 18 && IsTiffHeader(note, 10))
                    {
                        var nikon = new Tiff(_bytes, (int)(_start + at + 10), little: note[10] == 'I', _box, _tags, _fileOffsetOf, _readFile);
                        nikon.ReadIfd(nikon.UInt32(4), "Nikon");
                    }
                    else if (MakeOf()?.StartsWith("Canon", StringComparison.Ordinal) == true)
                    {
                        ReadIfd((uint)at, "Canon");
                    }
                }
                catch (Exception)
                {
                    // notes of a layout other than their maker's: their bytes, the tag before
                }
            }

            // the Make of the 0th IFD read last: the camera of the notes
            private string MakeOf() =>
                _tags.LastOrDefault(t => t.Family == MetadataFamily.Exif && t.Namespace == "IFD0" && t.Key == "Make")?.Value?.ToString();

            private static int SizeOf(ushort type) => type switch
            {
                1 or 2 or 6 or 7 or 129 => 1,  // BYTE, ASCII, SBYTE, UNDEFINED, UTF-8 (Exif 3.0)
                3 or 8 => 2,                   // SHORT, SSHORT
                4 or 9 or 11 => 4,             // LONG, SLONG, FLOAT
                5 or 10 or 12 => 8,            // RATIONAL, SRATIONAL, DOUBLE
                _ => 0,
            };

            private object Value(string ifd, ushort tag, ushort type, uint count, long at)
            {
                // Text is kept as its bytes, as the strings of boxes are: of a terminator where it is not, of an encoding not
                // expected, they write back as they were. Windows' XPTitle, XPComment, XPAuthor, XPKeywords, XPSubject:
                // UTF-16LE of bytes
                if (ifd is "IFD0" or "IFD1" && tag >= 0x9C9B && tag <= 0x9C9F && type == 1)
                    return new BinaryUTF16String(Bytes(at, count), littleEndian: true);

                switch (type)
                {
                    // ASCII, its terminating zero too - told as UTF-8, which ASCII is, and which writers put in it
                    case 2:
                    case 129:
                        return new BinaryUTF8String(Bytes(at, count)) { IsZeroTerminated = false };
                    case 7:
                    {
                        var bytes = Bytes(at, count);
                        // UserComment, GPSProcessingMethod, GPSAreaInformation: 8 bytes of the code of the text, then the text
                        if ((ifd == "Exif" && tag == 0x9286 || ifd == "GPS" && tag is 0x001B or 0x001C) && count >= 8)
                            return new CharacterCodedString(bytes, _little);
                        // of printable ASCII - ExifVersion's "0232" - as text
                        bool text = count > 0 && Array.TrueForAll(bytes, b => b >= 0x20 && b < 0x7F);
                        return text ? new BinaryUTF8String(bytes) { IsZeroTerminated = false } : bytes;
                    }
                    case 5:
                    case 10:
                    {
                        var rationals = new string[count];
                        for (int i = 0; i < count; i++)
                        {
                            uint numerator = UInt32(at + 8L * i), denominator = UInt32(at + 8L * i + 4);
                            rationals[i] = type == 5 ? $"{numerator}/{denominator}" : $"{(int)numerator}/{(int)denominator}";
                        }
                        return count == 1 ? rationals[0] : rationals;
                    }
                    case 11:
                    case 12:
                        return Floats(type, count, at);
                    default:
                    {
                        var numbers = new long[count];
                        for (int i = 0; i < count; i++)
                        {
                            numbers[i] = type switch
                            {
                                1 => _bytes[_start + at + i],
                                6 => (sbyte)_bytes[_start + at + i],
                                3 => UInt16(at + 2L * i),
                                8 => (short)UInt16(at + 2L * i),
                                4 => UInt32(at + 4L * i),
                                9 => (int)UInt32(at + 4L * i),
                                _ => 0,
                            };
                        }
                        return count == 1 ? numbers[0] : numbers;
                    }
                }
            }


            private object Floats(ushort type, uint count, long at)
            {
                var values = new double[count];
                for (int i = 0; i < count; i++)
                {
                    if (type == 11)
                    {
                        values[i] = BitConverter.ToSingle(BitConverter.GetBytes(UInt32(at + 4L * i)), 0);
                    }
                    else
                    {
                        ulong high = UInt32(at + 8L * i + (_little ? 4 : 0)), low = UInt32(at + 8L * i + (_little ? 0 : 4));
                        values[i] = BitConverter.Int64BitsToDouble((long)(high << 32 | low));
                    }
                }
                return count == 1 ? values[0] : string.Join(" ", values.Select(v => v.ToString(CultureInfo.InvariantCulture)));
            }
        }

        #region What values mean

        /// <summary>What a value means: of a tag of codes, the code's meaning; of a rational, its number; else nothing.</summary>
        private static string Describe(string ifd, string key, object value)
        {
            if (ifd == "GPS" && key is "GPSLatitude" or "GPSLongitude" or "GPSDestLatitude" or "GPSDestLongitude" && value is string[] dms && dms.Length == 3)
            {
                // degrees, minutes and seconds, as degrees
                double? d = Number(dms[0]), m = Number(dms[1]), s = Number(dms[2]);
                return d != null && m != null && s != null ? $"{d + m / 60 + s / 3600:0.######}°" : null;
            }

            string meaning = value is long code ? Meaning(ifd, key, code) : null;
            if (meaning != null)
                return meaning;
            if (key == "FileSource" && value is byte[] { Length: 1 } source)
                return source[0] == 3 ? "digital still camera" : null;
            if (key == "SceneType" && value is byte[] { Length: 1 } scene)
                return scene[0] == 1 ? "directly photographed" : null;
            if (value is string rational && rational.Contains("/") && Number(rational) is double number)
                return key == "ExposureTime" && number > 0 && number < 1 ? $"{number:0.######} s (1/{1 / number:0.#})" : number.ToString("0.######", CultureInfo.InvariantCulture);
            return null;
        }

        private static double? Number(string rational)
        {
            int slash = rational.IndexOf('/');
            if (slash < 0 || !double.TryParse(rational.Substring(0, slash), NumberStyles.Integer, CultureInfo.InvariantCulture, out double numerator)
                || !double.TryParse(rational.Substring(slash + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out double denominator)
                || denominator == 0)
                return null;
            return numerator / denominator;
        }

        private static string Meaning(string ifd, string key, long code)
        {
            switch (key)
            {
                case "Orientation":
                    return code switch
                    {
                        1 => "top-left: as it is",
                        2 => "top-right: mirrored horizontally",
                        3 => "bottom-right: turned 180°",
                        4 => "bottom-left: mirrored vertically",
                        5 => "left-top: mirrored horizontally, turned 270° clockwise",
                        6 => "right-top: turned 90° clockwise",
                        7 => "right-bottom: mirrored horizontally, turned 90° clockwise",
                        8 => "left-bottom: turned 270° clockwise",
                        _ => null,
                    };
                case "ResolutionUnit":
                case "FocalPlaneResolutionUnit":
                    return code switch { 1 => "none", 2 => "inches", 3 => "centimetres", _ => null };
                case "Compression":
                    return code switch { 1 => "uncompressed", 6 => "JPEG (old style)", 7 => "JPEG", _ => null };
                case "PhotometricInterpretation":
                    return code switch { 0 => "white is zero", 1 => "black is zero", 2 => "RGB", 6 => "YCbCr", _ => null };
                case "PlanarConfiguration":
                    return code switch { 1 => "chunky", 2 => "planar", _ => null };
                case "YCbCrPositioning":
                    return code switch { 1 => "centred", 2 => "co-sited", _ => null };
                case "ExposureProgram":
                    return code switch
                    {
                        0 => "not defined", 1 => "manual", 2 => "normal program", 3 => "aperture priority", 4 => "shutter priority",
                        5 => "creative program", 6 => "action program", 7 => "portrait mode", 8 => "landscape mode", _ => null,
                    };
                case "SensitivityType":
                    return code switch
                    {
                        0 => "unknown", 1 => "standard output sensitivity", 2 => "recommended exposure index", 3 => "ISO speed",
                        4 => "standard output sensitivity and recommended exposure index", 5 => "standard output sensitivity and ISO speed",
                        6 => "recommended exposure index and ISO speed", 7 => "standard output sensitivity, recommended exposure index and ISO speed",
                        _ => null,
                    };
                case "MeteringMode":
                    return code switch
                    {
                        0 => "unknown", 1 => "average", 2 => "centre-weighted average", 3 => "spot", 4 => "multi-spot", 5 => "pattern",
                        6 => "partial", 255 => "other", _ => null,
                    };
                case "LightSource":
                    return code switch
                    {
                        0 => "unknown", 1 => "daylight", 2 => "fluorescent", 3 => "tungsten", 4 => "flash", 9 => "fine weather",
                        10 => "cloudy weather", 11 => "shade", 12 => "daylight fluorescent", 13 => "day white fluorescent",
                        14 => "cool white fluorescent", 15 => "white fluorescent", 16 => "warm white fluorescent",
                        17 => "standard light A", 18 => "standard light B", 19 => "standard light C", 20 => "D55", 21 => "D65",
                        22 => "D75", 23 => "D50", 24 => "ISO studio tungsten", 255 => "other", _ => null,
                    };
                case "Flash":
                    return FlashMeaning(code);
                case "ColorSpace":
                    return code switch { 1 => "sRGB", 0xFFFF => "uncalibrated", _ => null };
                case "SensingMethod":
                    return code switch
                    {
                        1 => "not defined", 2 => "one-chip colour area", 3 => "two-chip colour area", 4 => "three-chip colour area",
                        5 => "colour sequential area", 7 => "trilinear", 8 => "colour sequential linear", _ => null,
                    };
                case "CustomRendered":
                    return code switch
                    {
                        0 => "normal", 1 => "custom", 2 => "HDR, its original not kept", 3 => "HDR, its original kept",
                        4 => "original, of an HDR", 6 => "panorama", 7 => "portrait HDR", 8 => "portrait", _ => null,
                    };
                case "ExposureMode":
                    return code switch { 0 => "auto", 1 => "manual", 2 => "auto bracket", _ => null };
                case "WhiteBalance":
                    return code switch { 0 => "auto", 1 => "manual", _ => null };
                case "SceneCaptureType":
                    return code switch { 0 => "standard", 1 => "landscape", 2 => "portrait", 3 => "night scene", _ => null };
                case "GainControl":
                    return code switch { 0 => "none", 1 => "low gain up", 2 => "high gain up", 3 => "low gain down", 4 => "high gain down", _ => null };
                case "Contrast":
                case "Sharpness":
                    return code switch { 0 => "normal", 1 => "soft", 2 => "hard", _ => null };
                case "Saturation":
                    return code switch { 0 => "normal", 1 => "low", 2 => "high", _ => null };
                case "SubjectDistanceRange":
                    return code switch { 0 => "unknown", 1 => "macro", 2 => "close view", 3 => "distant view", _ => null };
                case "CompositeImage":
                    return code switch { 0 => "unknown", 1 => "not composite", 2 => "composite", 3 => "composite, captured as shot", _ => null };
                case "GPSAltitudeRef":
                    return code switch { 0 => "above sea level", 1 => "below sea level", _ => null };
                case "GPSDifferential":
                    return code switch { 0 => "without correction", 1 => "differential correction", _ => null };
            }
            return null;
        }

        private static string FlashMeaning(long code)
        {
            if ((code & 0x20) != 0)
                return "no flash function";
            var parts = new List<string> { (code & 1) != 0 ? "fired" : "did not fire" };
            switch ((code >> 3) & 3)
            {
                case 1: parts.Add("compulsory firing"); break;
                case 2: parts.Add("compulsory suppression"); break;
                case 3: parts.Add("auto"); break;
            }
            switch ((code >> 1) & 3)
            {
                case 2: parts.Add("return not detected"); break;
                case 3: parts.Add("return detected"); break;
            }
            if ((code & 0x40) != 0)
                parts.Add("red-eye reduction");
            return string.Join(", ", parts);
        }

        #endregion

        #region Names

        /// <summary>The name of a tag, of its IFD (Exif 3.0, TIFF 6.0, and Windows' and Adobe's of TIFF); of one not known, its number.</summary>
        internal static string NameOf(string ifd, ushort tag)
        {
            var names = ifd switch
            {
                "GPS" => GpsNames,
                "Interop" => InteropNames,
                "Apple" => AppleNames,
                "Canon" => CanonNames,
                "Nikon" => NikonNames,
                _ => TiffAndExifNames,
            };
            return names.TryGetValue(tag, out string name) ? name : $"0x{tag:X4}";
        }

        // of the makers' notes, as ExifTool names them (Apple.pm, Canon.pm, Nikon.pm): those of most cameras
        private static readonly Dictionary<ushort, string> AppleNames = new Dictionary<ushort, string>
        {
            [0x0001] = "MakerNoteVersion", [0x0002] = "AEMatrix", [0x0003] = "RunTime", [0x0004] = "AEStable",
            [0x0005] = "AETarget", [0x0006] = "AEAverage", [0x0007] = "AFStable", [0x0008] = "AccelerationVector",
            [0x000A] = "HDRImageType", [0x000B] = "BurstUUID", [0x000C] = "FocusDistanceRange", [0x000F] = "OISMode",
            [0x0011] = "ContentIdentifier", [0x0014] = "ImageCaptureType", [0x0015] = "ImageUniqueID",
            [0x0017] = "LivePhotoVideoIndex", [0x0019] = "ImageProcessingFlags", [0x001A] = "QualityHint",
            [0x001D] = "LuminanceNoiseAmplitude", [0x001F] = "PhotosAppFeatureFlags", [0x0020] = "ImageCaptureRequestID",
            [0x0021] = "HDRHeadroom", [0x0023] = "AFPerformance", [0x0025] = "SceneFlags", [0x0026] = "SignalToNoiseRatioType",
            [0x0027] = "SignalToNoiseRatio", [0x002B] = "PhotoIdentifier", [0x002D] = "ColorTemperature", [0x002E] = "CameraType",
            [0x002F] = "FocusPosition", [0x0030] = "HDRGain", [0x0038] = "AFMeasuredDepth", [0x003D] = "AFConfidence",
            [0x0040] = "SemanticStyle", [0x0041] = "SemanticStyleRenderingVer", [0x0042] = "SemanticStylePreset",
        };

        private static readonly Dictionary<ushort, string> CanonNames = new Dictionary<ushort, string>
        {
            [0x0001] = "CanonCameraSettings", [0x0002] = "CanonFocalLength", [0x0003] = "CanonFlashInfo",
            [0x0004] = "CanonShotInfo", [0x0005] = "CanonPanorama", [0x0006] = "CanonImageType",
            [0x0007] = "CanonFirmwareVersion", [0x0008] = "FileNumber", [0x0009] = "OwnerName", [0x000C] = "SerialNumber",
            [0x000D] = "CanonCameraInfo", [0x000E] = "CanonFileLength", [0x000F] = "CustomFunctions", [0x0010] = "CanonModelID",
            [0x0012] = "CanonAFInfo", [0x0013] = "ThumbnailImageValidArea", [0x0015] = "SerialNumberFormat",
            [0x001A] = "SuperMacro", [0x001C] = "DateStampMode", [0x001D] = "MyColors", [0x001E] = "FirmwareRevision",
            [0x0023] = "Categories", [0x0024] = "FaceDetect1", [0x0025] = "FaceDetect2", [0x0026] = "CanonAFInfo2",
            [0x0028] = "ImageUniqueID", [0x0035] = "TimeInfo", [0x0038] = "BatteryType", [0x003C] = "AFInfo3",
            [0x0093] = "CanonFileInfo", [0x0095] = "LensModel", [0x0096] = "InternalSerialNumber", [0x0097] = "DustRemovalData",
            [0x0098] = "CropInfo", [0x0099] = "CustomFunctions2", [0x009A] = "AspectInfo", [0x00A0] = "ProcessingInfo",
            [0x00AA] = "MeasuredColor", [0x00B4] = "ColorSpace", [0x00D0] = "VRDOffset", [0x00E0] = "SensorInfo",
            [0x4001] = "ColorData", [0x4008] = "PictureStyleUserDef", [0x4009] = "PictureStylePC", [0x4010] = "CustomPictureStyleFileName",
            [0x4013] = "AFMicroAdj", [0x4015] = "VignettingCorr", [0x4016] = "VignettingCorr2", [0x4018] = "LightingOpt",
            [0x4019] = "LensInfo", [0x4020] = "AmbienceInfo", [0x4021] = "MultiExp", [0x4024] = "FilterInfo",
            [0x4025] = "HDRInfo", [0x4028] = "AFConfig",
        };

        private static readonly Dictionary<ushort, string> NikonNames = new Dictionary<ushort, string>
        {
            [0x0001] = "MakerNoteVersion", [0x0002] = "ISO", [0x0003] = "ColorMode", [0x0004] = "Quality",
            [0x0005] = "WhiteBalance", [0x0006] = "Sharpness", [0x0007] = "FocusMode", [0x0008] = "FlashSetting",
            [0x0009] = "FlashType", [0x000B] = "WhiteBalanceFineTune", [0x000C] = "WB_RBLevels", [0x000D] = "ProgramShift",
            [0x000E] = "ExposureDifference", [0x0011] = "PreviewIFD", [0x0012] = "FlashExposureComp", [0x0013] = "ISOSetting",
            [0x0017] = "ExternalFlashExposureComp", [0x0018] = "FlashExposureBracketValue", [0x0019] = "ExposureBracketValue",
            [0x001B] = "CropHiSpeed", [0x001D] = "SerialNumber", [0x001E] = "ColorSpace", [0x001F] = "VRInfo",
            [0x0022] = "ActiveD-Lighting", [0x0023] = "PictureControlData", [0x0024] = "WorldTime", [0x0025] = "ISOInfo",
            [0x002A] = "VignetteControl", [0x0083] = "LensType", [0x0084] = "Lens", [0x0087] = "FlashMode",
            [0x0089] = "ShootingMode", [0x008B] = "LensFStops", [0x0091] = "ShotInfo", [0x0093] = "NEFCompression",
            [0x0095] = "NoiseReduction", [0x0097] = "ColorBalance", [0x0098] = "LensData", [0x00A7] = "ShutterCount",
            [0x00A8] = "FlashInfo", [0x00AB] = "VariProgram", [0x00B1] = "HighISONoiseReduction", [0x00B7] = "AFInfo2",
            [0x00B8] = "FileInfo", [0x00BB] = "RetouchInfo",
        };

        private static readonly Dictionary<ushort, string> TiffAndExifNames = new Dictionary<ushort, string>
        {
            // TIFF 6.0, of the 0th and 1st IFDs
            [0x00FE] = "NewSubfileType", [0x00FF] = "SubfileType", [0x0100] = "ImageWidth", [0x0101] = "ImageLength",
            [0x0102] = "BitsPerSample", [0x0103] = "Compression", [0x0106] = "PhotometricInterpretation", [0x010A] = "FillOrder",
            [0x010D] = "DocumentName", [0x010E] = "ImageDescription", [0x010F] = "Make", [0x0110] = "Model",
            [0x0111] = "StripOffsets", [0x0112] = "Orientation", [0x0115] = "SamplesPerPixel", [0x0116] = "RowsPerStrip",
            [0x0117] = "StripByteCounts", [0x011A] = "XResolution", [0x011B] = "YResolution", [0x011C] = "PlanarConfiguration",
            [0x011D] = "PageName", [0x0128] = "ResolutionUnit", [0x0129] = "PageNumber", [0x012D] = "TransferFunction",
            [0x0131] = "Software", [0x0132] = "DateTime", [0x013B] = "Artist", [0x013C] = "HostComputer", [0x013D] = "Predictor",
            [0x013E] = "WhitePoint", [0x013F] = "PrimaryChromaticities", [0x0142] = "TileWidth", [0x0143] = "TileLength",
            [0x0144] = "TileOffsets", [0x0145] = "TileByteCounts", [0x014A] = "SubIFDs", [0x0152] = "ExtraSamples",
            [0x0153] = "SampleFormat", [0x0201] = "JPEGInterchangeFormat", [0x0202] = "JPEGInterchangeFormatLength",
            [0x0211] = "YCbCrCoefficients", [0x0212] = "YCbCrSubSampling", [0x0213] = "YCbCrPositioning",
            [0x0214] = "ReferenceBlackWhite", [0x02BC] = "ApplicationNotes", [0x4746] = "Rating", [0x4749] = "RatingPercent",
            [0x8298] = "Copyright", [0x83BB] = "IPTC-NAA", [0x8649] = "ImageResources", [0x8773] = "InterColorProfile",
            [0x9C9B] = "XPTitle", [0x9C9C] = "XPComment", [0x9C9D] = "XPAuthor", [0x9C9E] = "XPKeywords", [0x9C9F] = "XPSubject",
            [0xC4A5] = "PrintImageMatching", [0xEA1C] = "Padding", [0xEA1D] = "OffsetSchema",
            // Exif 3.0, of the Exif IFD
            [0x829A] = "ExposureTime", [0x829D] = "FNumber", [0x8822] = "ExposureProgram", [0x8824] = "SpectralSensitivity",
            [0x8827] = "PhotographicSensitivity", [0x8828] = "OECF", [0x8830] = "SensitivityType",
            [0x8831] = "StandardOutputSensitivity", [0x8832] = "RecommendedExposureIndex", [0x8833] = "ISOSpeed",
            [0x8834] = "ISOSpeedLatitudeyyy", [0x8835] = "ISOSpeedLatitudezzz", [0x9000] = "ExifVersion",
            [0x9003] = "DateTimeOriginal", [0x9004] = "DateTimeDigitized", [0x9010] = "OffsetTime",
            [0x9011] = "OffsetTimeOriginal", [0x9012] = "OffsetTimeDigitized", [0x9101] = "ComponentsConfiguration",
            [0x9102] = "CompressedBitsPerPixel", [0x9201] = "ShutterSpeedValue", [0x9202] = "ApertureValue",
            [0x9203] = "BrightnessValue", [0x9204] = "ExposureBiasValue", [0x9205] = "MaxApertureValue",
            [0x9206] = "SubjectDistance", [0x9207] = "MeteringMode", [0x9208] = "LightSource", [0x9209] = "Flash",
            [0x920A] = "FocalLength", [0x9214] = "SubjectArea", [0x927C] = "MakerNote", [0x9286] = "UserComment",
            [0x9290] = "SubSecTime", [0x9291] = "SubSecTimeOriginal", [0x9292] = "SubSecTimeDigitized",
            [0x9400] = "Temperature", [0x9401] = "Humidity", [0x9402] = "Pressure", [0x9403] = "WaterDepth",
            [0x9404] = "Acceleration", [0x9405] = "CameraElevationAngle", [0xA000] = "FlashpixVersion",
            [0xA001] = "ColorSpace", [0xA002] = "PixelXDimension", [0xA003] = "PixelYDimension",
            [0xA004] = "RelatedSoundFile", [0xA20B] = "FlashEnergy", [0xA20C] = "SpatialFrequencyResponse",
            [0xA20E] = "FocalPlaneXResolution", [0xA20F] = "FocalPlaneYResolution", [0xA210] = "FocalPlaneResolutionUnit",
            [0xA214] = "SubjectLocation", [0xA215] = "ExposureIndex", [0xA217] = "SensingMethod", [0xA300] = "FileSource",
            [0xA301] = "SceneType", [0xA302] = "CFAPattern", [0xA401] = "CustomRendered", [0xA402] = "ExposureMode",
            [0xA403] = "WhiteBalance", [0xA404] = "DigitalZoomRatio", [0xA405] = "FocalLengthIn35mmFilm",
            [0xA406] = "SceneCaptureType", [0xA407] = "GainControl", [0xA408] = "Contrast", [0xA409] = "Saturation",
            [0xA40A] = "Sharpness", [0xA40B] = "DeviceSettingDescription", [0xA40C] = "SubjectDistanceRange",
            [0xA420] = "ImageUniqueID", [0xA430] = "CameraOwnerName", [0xA431] = "BodySerialNumber",
            [0xA432] = "LensSpecification", [0xA433] = "LensMake", [0xA434] = "LensModel", [0xA435] = "LensSerialNumber",
            [0xA436] = "ImageTitle", [0xA437] = "Photographer", [0xA438] = "ImageEditor", [0xA439] = "CameraFirmware",
            [0xA43A] = "RAWDevelopingSoftware", [0xA43B] = "ImageEditingSoftware", [0xA43C] = "MetadataEditingSoftware",
            [0xA460] = "CompositeImage", [0xA461] = "SourceImageNumberOfCompositeImage",
            [0xA462] = "SourceExposureTimesOfCompositeImage", [0xA500] = "Gamma",
        };

        private static readonly Dictionary<ushort, string> GpsNames = new Dictionary<ushort, string>
        {
            [0x0000] = "GPSVersionID", [0x0001] = "GPSLatitudeRef", [0x0002] = "GPSLatitude", [0x0003] = "GPSLongitudeRef",
            [0x0004] = "GPSLongitude", [0x0005] = "GPSAltitudeRef", [0x0006] = "GPSAltitude", [0x0007] = "GPSTimeStamp",
            [0x0008] = "GPSSatellites", [0x0009] = "GPSStatus", [0x000A] = "GPSMeasureMode", [0x000B] = "GPSDOP",
            [0x000C] = "GPSSpeedRef", [0x000D] = "GPSSpeed", [0x000E] = "GPSTrackRef", [0x000F] = "GPSTrack",
            [0x0010] = "GPSImgDirectionRef", [0x0011] = "GPSImgDirection", [0x0012] = "GPSMapDatum",
            [0x0013] = "GPSDestLatitudeRef", [0x0014] = "GPSDestLatitude", [0x0015] = "GPSDestLongitudeRef",
            [0x0016] = "GPSDestLongitude", [0x0017] = "GPSDestBearingRef", [0x0018] = "GPSDestBearing",
            [0x0019] = "GPSDestDistanceRef", [0x001A] = "GPSDestDistance", [0x001B] = "GPSProcessingMethod",
            [0x001C] = "GPSAreaInformation", [0x001D] = "GPSDateStamp", [0x001E] = "GPSDifferential",
            [0x001F] = "GPSHPositioningError",
        };

        private static readonly Dictionary<ushort, string> InteropNames = new Dictionary<ushort, string>
        {
            [0x0001] = "InteroperabilityIndex", [0x0002] = "InteroperabilityVersion", [0x1000] = "RelatedImageFileFormat",
            [0x1001] = "RelatedImageWidth", [0x1002] = "RelatedImageLength",
        };

        #endregion
    }
}
