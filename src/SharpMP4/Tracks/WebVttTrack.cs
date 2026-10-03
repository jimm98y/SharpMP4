using SharpISOBMFF;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
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
                return CopyOfSampleEntry();

            var entry = new WVTTSampleEntry { DataReferenceIndex = 1, ReservedSampleEntry = new byte[6], Children = new List<Box>() };
            entry.Children.Add(new WebVTTConfigurationBox { Config = new BinaryUTF8String(Config) { IsZeroTerminated = false } });
            if (SourceLabel != null)
                entry.Children.Add(new WebVTTSourceLabelBox { SourceLabel = new BinaryUTF8String(SourceLabel) { IsZeroTerminated = false } });
            foreach (var child in entry.Children)
                child.SetParent(entry);
            return entry;
        }

        public override ITrack Clone() => CopySettingsTo(WithSampleEntryOf(new WebVttTrack(Timescale) { Config = Config, SourceLabel = SourceLabel, Forced = Forced }));
    }
}
