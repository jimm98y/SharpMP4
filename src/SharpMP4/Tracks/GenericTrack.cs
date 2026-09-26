using System;
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

        /// <summary>
        /// The sample goes to the file as it arrives, so this hands back exactly what it was
        /// given, without copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
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