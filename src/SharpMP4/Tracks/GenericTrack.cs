using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    public class GenericTrack : TrackBase
    {
        public override string HandlerName { get; }
        public override string HandlerType { get; }
        public override string Language { get; set; } = "und";

        public Box Config { get; }

        public GenericTrack(Box config, uint timescale, int sampleDuration, uint handlerType, string handlerName) : base()
        {
            Config = config;
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
            HandlerType = IsoStream.ToFourCC(handlerType);
            HandlerName = handlerName;
        }

        public override Box CreateSampleEntryBox()
        {
            return Config;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // nothing to do
        }

        public override ITrack Clone()
        {
            return new GenericTrack(Config, Timescale, DefaultSampleDuration, IsoStream.FromFourCC(HandlerType), HandlerName);
        }
    }
} 