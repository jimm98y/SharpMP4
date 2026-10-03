using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMP4.Tracks
{
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
            // every sample forced, as the entry's display flags say
            Forced = entry is TextSampleEntrytx3gDup tx3g && (tx3g.DisplayFlags & AllSamplesForced) != 0;
        }

        // Of the display flags of a 3GPP timed text entry: every sample forced, as Apple's and VLC's players read them. VLC
        // selects a forced track by default, where it selects no other subtitles unless asked; it reads no 'kind' box.
        private const uint AllSamplesForced = 0x80000000;

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
            // the entry the track was read with, a copy of it: the writer puts what it is given in its own 'stsd'
            if (SampleEntry != null)
                return CopyOfSampleEntry();

            // as ffmpeg's mov_text writes it: at the bottom, centred, white on transparent, the font of ID 1 'Serif', 18
            var entry = new TextSampleEntrytx3gDup
            {
                DataReferenceIndex = 1,
                ReservedSampleEntry = new byte[6],
                DisplayFlags = Forced ? AllSamplesForced : 0u,
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

        public override ITrack Clone() => CopySettingsTo(WithSampleEntryOf(new TimedTextTrack(Timescale, _handlerType) { Forced = Forced }));
    }
}
