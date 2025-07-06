using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    public class GenericTrack : TrackBase
    {
        public override string HandlerName { get; }
        public override string HandlerType { get; }
        public override string Language { get; set; } = "und";

        public Box SampleEntry { get; }

        public GenericTrack(Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            SampleEntry = sampleEntry;
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
            HandlerType = IsoStream.ToFourCC(handlerType);
            HandlerName = handlerName;
        }

        public override Box CreateSampleEntryBox()
        {
            return SampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // nothing to do
        }
    }
} 