using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    public class GenericTrack : TrackBase
    {
        public override string HandlerName { get; }
        public override string HandlerType { get; }
        public override string Language { get; set; } = "und";

        public Box SampleEntry { get; }

        public GenericTrack(uint timescale, int defaultSampleDuration, uint handlerType, string handlerName) : base()
        {
            Timescale = timescale;
            DefaultSampleDuration = defaultSampleDuration;
            HandlerType = IsoStream.ToFourCC(handlerType);
            HandlerName = handlerName;
        }

        public GenericTrack(Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName) : this(timescale, sampleDuration, handlerType, handlerName)  
        {
            SampleEntry = sampleEntry;
        }

        public override Box CreateSampleEntryBox()
        {
            return SampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // nothing to do
        }

        public override ITrack Clone()
        {
            return new GenericTrack(Timescale, DefaultSampleDuration, IsoStream.FromFourCC(HandlerType), HandlerName);
        }
    }
} 