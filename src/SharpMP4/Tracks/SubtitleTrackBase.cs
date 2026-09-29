using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// A cue of a subtitle track: its text, shown from <see cref="Start"/> to <see cref="End"/>, in the track's timescale.
    /// </summary>
    public class SubtitleCue
    {
        public long Start { get; set; }
        public long End { get; set; }

        /// <summary>The text: of WebVTT the cue's payload, of TTML the document, of 3GPP timed text and simple text the sample's text.</summary>
        public string Text { get; set; }

        /// <summary>Of WebVTT: the cue's identifier, where it has one.</summary>
        public string Id { get; set; }

        /// <summary>Of WebVTT: the cue's settings - position, alignment, and so on - where it has any.</summary>
        public string Settings { get; set; }

        public override string ToString() => $"{Start}-{End}: {Text}";
    }

    /// <summary>A track of subtitles or timed text: its samples are cues.</summary>
    public interface ISubtitleTrack : ITrack
    {
        /// <summary>
        /// The cues of a sample shown from <paramref name="start"/> for <paramref name="duration"/>: none of a sample that
        /// shows nothing - an empty 3GPP timed text sample, a WebVTT 'vtte'.
        /// </summary>
        IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration);

        /// <summary>
        /// The sample of the cues shown together over one span of time, all of them from its start to its end - none for
        /// a gap between them.
        /// </summary>
        byte[] CreateSample(IReadOnlyList<SubtitleCue> cues);
    }

    /// <summary>What subtitle tracks share: their samples go to the file as they are, each a random access point.</summary>
    public abstract class SubtitleTrackBase : TrackBase, ISubtitleTrack
    {
        public override string Language { get; set; } = "und";

        /// <summary>The sample entry the track was read with, which it writes back as it was; null of a track made to be written.</summary>
        public Box SampleEntry { get; protected set; }

        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // no picture: no width and height
        }

        public abstract IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration);

        public abstract byte[] CreateSample(IReadOnlyList<SubtitleCue> cues);

        /// <summary>The boxes a sample is made of, as WebVTT's are.</summary>
        protected static IList<Box> ReadBoxes(byte[] buffer, int offset, int length)
        {
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(new MemoryStream(buffer, offset, length, writable: false))));
            return container.Children;
        }

        protected static byte[] WriteBoxes(IEnumerable<Box> boxes)
        {
            var container = new Container();
            foreach (var box in boxes)
            {
                box.SetParent(container);
                container.Children.Add(box);
            }
            using var output = new MemoryStream();
            container.Write(new IsoStream(new StreamWrapper(output)));
            return output.ToArray();
        }

        /// <summary>A string of a sample entry, ended with a zero: the bytes of one hold it, as they are read.</summary>
        protected static BinaryUTF8String Terminated(string text) => new BinaryUTF8String(text + "\0");

        protected static SubtitleCue[] Cue(string text, long start, long duration) =>
            new[] { new SubtitleCue { Start = start, End = start + duration, Text = text } };
    }

    /// <summary>
    /// WebVTT (ISO/IEC 14496-30, 7): a 'wvtt' sample entry with the file's header in 'vttC', and each sample the cues
    /// shown over its span - a 'vttc' each, its payload, ID and settings - or a 'vtte' where none is.
    /// </summary>
    public class WebVttTrack : SubtitleTrackBase
    {
        public override string HandlerName => HandlerNames.Text;
        public override string HandlerType => HandlerTypes.Text;

        /// <summary>The WebVTT file's header: "WEBVTT", and what follows it up to the first cue.</summary>
        public string Config { get; set; } = "WEBVTT";

        /// <summary>A label of where the cues came from, 'vlab'; null for none.</summary>
        public string SourceLabel { get; set; }

        public WebVttTrack(uint timescale = 1000)
        {
            Timescale = timescale;
        }

        public WebVttTrack(WVTTSampleEntry entry, uint timescale) : this(timescale)
        {
            SampleEntry = entry;
            Config = entry.Config?.Config.ToString() ?? Config;
            SourceLabel = entry.Label?.SourceLabel.ToString();
        }

        public override IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration)
        {
            var cues = new List<SubtitleCue>();
            foreach (var cue in ReadBoxes(buffer, offset, length).OfType<VTTCueBox>())
            {
                var children = cue.Children ?? new List<Box>();
                cues.Add(new SubtitleCue
                {
                    Start = start,
                    End = start + duration,
                    // the whole of each box: WebVTT's strings are neither counted nor ended with a zero
                    Text = children.OfType<CuePayloadBox>().FirstOrDefault()?.CueText.ToString() ?? "",
                    Id = children.OfType<CueIDBox>().FirstOrDefault()?.CueID.ToString(),
                    Settings = children.OfType<CueSettingsBox>().FirstOrDefault()?.Settings.ToString(),
                });
            }
            return cues;
        }

        public override byte[] CreateSample(IReadOnlyList<SubtitleCue> cues)
        {
            if (cues == null || cues.Count == 0)
                return WriteBoxes(new Box[] { new VTTEmptyBox() });

            return WriteBoxes(cues.Select(cue =>
            {
                var box = new VTTCueBox { Children = new List<Box>() };
                if (!string.IsNullOrEmpty(cue.Id))
                    box.Children.Add(new CueIDBox { CueID = new BinaryUTF8String(cue.Id) { IsZeroTerminated = false } });
                if (!string.IsNullOrEmpty(cue.Settings))
                    box.Children.Add(new CueSettingsBox { Settings = new BinaryUTF8String(cue.Settings) { IsZeroTerminated = false } });
                box.Children.Add(new CuePayloadBox { CueText = new BinaryUTF8String(cue.Text ?? "") { IsZeroTerminated = false } });
                foreach (var child in box.Children)
                    child.SetParent(box);
                return (Box)box;
            }));
        }

        public override Box CreateSampleEntryBox()
        {
            if (SampleEntry != null)
                return SampleEntry;

            var entry = new WVTTSampleEntry { DataReferenceIndex = 1, ReservedSampleEntry = new byte[6], Children = new List<Box>() };
            entry.Children.Add(new WebVTTConfigurationBox { Config = new BinaryUTF8String(Config) { IsZeroTerminated = false } });
            if (SourceLabel != null)
                entry.Children.Add(new WebVTTSourceLabelBox { SourceLabel = new BinaryUTF8String(SourceLabel) { IsZeroTerminated = false } });
            foreach (var child in entry.Children)
                child.SetParent(entry);
            return entry;
        }

        public override ITrack Clone() => new WebVttTrack(Timescale) { Config = Config, SourceLabel = SourceLabel, Language = Language };
    }

    /// <summary>
    /// TTML and its IMSC profiles (ISO/IEC 14496-30, 6): an 'stpp' sample entry with the documents' namespaces, and each
    /// sample a TTML document - its images, where it has any, after it.
    /// </summary>
    public class TtmlTrack : SubtitleTrackBase
    {
        public override string HandlerName => HandlerNames.Subtitle;

        /// <summary>The documents' namespaces, space separated.</summary>
        public string Namespace { get; set; } = "http://www.w3.org/ns/ttml";

        /// <summary>Where the schemas of the namespaces are, space separated; null for none.</summary>
        public string SchemaLocation { get; set; }

        /// <summary>The media types of the resources after each document - images, fonts - space separated; null for none.</summary>
        public string AuxiliaryMimeTypes { get; set; }

        public TtmlTrack(uint timescale = 1000)
        {
            Timescale = timescale;
        }

        public TtmlTrack(XMLSubtitleSampleEntry entry, uint timescale, string handlerType) : this(timescale)
        {
            SampleEntry = entry;
            _handlerType = handlerType;
            Namespace = entry.Ns.Length > 0 ? entry.Ns.Text : Namespace;
            SchemaLocation = entry.SchemaLocationPresent ? entry.SchemaLocation.Text : null;
            AuxiliaryMimeTypes = entry.AuxiliaryMimeTypesPresent ? entry.AuxiliaryMimeTypes.Text : null;
        }

        // CMAF's IMSC tracks are of the 'text' handler, 14496-30's of 'subt'
        private readonly string _handlerType;
        public override string HandlerType => _handlerType ?? HandlerTypes.Subtitle;

        public override IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration)
        {
            // the document, without the images after it: it ends with its root element's end tag, </tt> or </prefix:tt>
            string text = Encoding.UTF8.GetString(buffer, offset, length);
            var end = RootEnd.Match(text);
            return Cue(end.Success ? text.Substring(0, end.Index + end.Length) : text.TrimEnd('\0'), start, duration);
        }

        private static readonly Regex RootEnd = new Regex(@"</([A-Za-z_][\w.-]*:)?tt\s*>", RegexOptions.CultureInvariant);

        /// <summary>
        /// A TTML sample is one document: of one cue, its text the document; of none, a document with nothing in it, for
        /// a sample is a document even where nothing is shown.
        /// </summary>
        public override byte[] CreateSample(IReadOnlyList<SubtitleCue> cues)
        {
            if (cues == null || cues.Count == 0)
                return Encoding.UTF8.GetBytes($"<?xml version=\"1.0\" encoding=\"UTF-8\"?><tt xmlns=\"{Namespace.Split(' ')[0]}\"><body/></tt>");
            if (cues.Count != 1)
                throw new ArgumentException("A TTML sample is one document: one cue, its text the document.", nameof(cues));
            return Encoding.UTF8.GetBytes(cues[0].Text ?? "");
        }

        public override Box CreateSampleEntryBox()
        {
            if (SampleEntry != null)
                return SampleEntry;

            return new XMLSubtitleSampleEntry
            {
                DataReferenceIndex = 1,
                ReservedSampleEntry = new byte[6],
                Ns = Terminated(Namespace),
                SchemaLocation = Terminated(SchemaLocation ?? ""),
                SchemaLocationPresent = SchemaLocation != null || AuxiliaryMimeTypes != null,
                AuxiliaryMimeTypes = Terminated(AuxiliaryMimeTypes ?? ""),
                AuxiliaryMimeTypesPresent = AuxiliaryMimeTypes != null,
                Children = new List<Box>(),
            };
        }

        public override ITrack Clone() => new TtmlTrack(Timescale) { Namespace = Namespace, SchemaLocation = SchemaLocation, AuxiliaryMimeTypes = AuxiliaryMimeTypes, Language = Language };
    }

    /// <summary>
    /// 3GPP timed text (3GPP TS 26.245), which ffmpeg calls mov_text: a 'tx3g' sample entry with the text's default box,
    /// style and fonts, and each sample its text - its length in 16 bits, then UTF-8, or UTF-16 after a byte order mark -
    /// then boxes that style it. An empty sample, of length 0, shows nothing.
    /// </summary>
    public class TimedTextTrack : SubtitleTrackBase
    {
        private readonly string _handlerType;

        public override string HandlerName => HandlerNames.Text;

        /// <summary>'text', as 3GPP has it; 'sbtl', as QuickTime and Apple's players do.</summary>
        public override string HandlerType => _handlerType ?? HandlerTypes.Text;

        public TimedTextTrack(uint timescale = 1000, string handlerType = HandlerTypes.Text)
        {
            Timescale = timescale;
            _handlerType = handlerType;
        }

        public TimedTextTrack(Box entry, uint timescale, string handlerType) : this(timescale, handlerType)
        {
            SampleEntry = entry;
        }

        public override IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration)
        {
            if (length < 2)
                return Array.Empty<SubtitleCue>();

            int textLength = Math.Min(buffer[offset] << 8 | buffer[offset + 1], length - 2);
            if (textLength == 0)
                return Array.Empty<SubtitleCue>();

            // UTF-16, big endian, after its byte order mark; else UTF-8
            bool utf16 = textLength >= 2 && buffer[offset + 2] == 0xFE && buffer[offset + 3] == 0xFF;
            string text = utf16
                ? Encoding.BigEndianUnicode.GetString(buffer, offset + 4, textLength - 2)
                : Encoding.UTF8.GetString(buffer, offset + 2, textLength);
            return Cue(text, start, duration);
        }

        public override byte[] CreateSample(IReadOnlyList<SubtitleCue> cues)
        {
            if (cues == null || cues.Count == 0)
                return new byte[2];
            if (cues.Count > 1)
                throw new ArgumentException("A 3GPP timed text sample is one text: one cue, or none.", nameof(cues));

            byte[] text = Encoding.UTF8.GetBytes(cues[0].Text ?? "");
            if (text.Length > ushort.MaxValue)
                throw new ArgumentException("A 3GPP timed text sample's text is at most 65535 bytes.", nameof(cues));
            var sample = new byte[2 + text.Length];
            sample[0] = (byte)(text.Length >> 8);
            sample[1] = (byte)text.Length;
            Buffer.BlockCopy(text, 0, sample, 2, text.Length);
            return sample;
        }

        public override Box CreateSampleEntryBox()
        {
            if (SampleEntry != null)
                return SampleEntry;

            // as ffmpeg's mov_text writes it: at the bottom, centred, white on transparent, the font of ID 1 'Serif', 18
            var entry = new TextSampleEntrytx3gDup
            {
                DataReferenceIndex = 1,
                ReservedSampleEntry = new byte[6],
                DisplayFlags = 0,
                HorizontalJustification = 1,
                VerticalJustification = 0xFF,
                BackgroundColorRgba = new byte[4],
                RectRecord = new RectRecord(),
                StyleRecord = new StyleRecord { FontId = 1, FontSize = 18, TextColor = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF } },
                Children = new List<Box>(),
            };
            var ftab = new FontTableBox
            {
                EntryCount = 1,
                FontEntries = new[] { new FontRecord { FontId = 1, Count = 5, FontName = Encoding.ASCII.GetBytes("Serif") } },
            };
            ftab.SetParent(entry);
            entry.Children.Add(ftab);
            return entry;
        }

        public override ITrack Clone() => new TimedTextTrack(Timescale, _handlerType) { Language = Language };
    }

    /// <summary>
    /// Simple text (ISO/IEC 14496-30, 5): an 'stxt' sample entry with the text's media type, and each sample the text.
    /// </summary>
    public class SimpleTextTrack : SubtitleTrackBase
    {
        public override string HandlerName => HandlerNames.Text;
        public override string HandlerType => HandlerTypes.Text;

        /// <summary>The media type of the text: "text/plain", or another's.</summary>
        public string MimeFormat { get; set; } = "text/plain";

        /// <summary>How the text is encoded as a whole - "gzip", say - null for as it is.</summary>
        public string ContentEncoding { get; set; }

        /// <summary>What every sample's text begins with, in 'txtC': of an SVG track, the document the samples animate; null for nothing.</summary>
        public string TextConfig { get; set; }

        public SimpleTextTrack(uint timescale = 1000)
        {
            Timescale = timescale;
        }

        public SimpleTextTrack(SimpleTextSampleEntry entry, uint timescale) : this(timescale)
        {
            SampleEntry = entry;
            MimeFormat = entry.MimeFormat.Length > 0 ? entry.MimeFormat.Text : MimeFormat;
            ContentEncoding = entry.ContentEncodingPresent && entry.ContentEncoding.Length > 0 ? entry.ContentEncoding.Text : null;
            TextConfig = entry._TextConfigBox?.TextConfig.Text;
        }

        public override IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration) =>
            length == 0 ? Array.Empty<SubtitleCue>() : Cue(Encoding.UTF8.GetString(buffer, offset, length), start, duration);

        public override byte[] CreateSample(IReadOnlyList<SubtitleCue> cues) =>
            Encoding.UTF8.GetBytes(string.Concat((cues ?? Array.Empty<SubtitleCue>()).Select(cue => cue.Text)));

        public override Box CreateSampleEntryBox()
        {
            if (SampleEntry != null)
                return SampleEntry;

            var entry = new SimpleTextSampleEntry
            {
                DataReferenceIndex = 1,
                ReservedSampleEntry = new byte[6],
                ContentEncoding = Terminated(ContentEncoding ?? ""),
                ContentEncodingPresent = true,
                MimeFormat = Terminated(MimeFormat),
                Children = new List<Box>(),
            };
            if (TextConfig != null)
            {
                var txtC = new TextConfigBox { TextConfig = Terminated(TextConfig) };
                txtC.SetParent(entry);
                entry.Children.Add(txtC);
            }
            return entry;
        }

        public override ITrack Clone() => new SimpleTextTrack(Timescale) { MimeFormat = MimeFormat, ContentEncoding = ContentEncoding, TextConfig = TextConfig, Language = Language };
    }
}
