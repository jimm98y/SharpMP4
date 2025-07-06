using SharpISOBMFF;
using System;

namespace SharpMP4.Tracks
{
    public class TrackFactory
    {
        public static Func<Box, uint, int, uint, string, ITrack> CreateTrack = DefaultCreateTrack;

        public static ITrack DefaultCreateTrack(Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            if (handlerType == IsoStream.FromFourCC(HandlerTypes.Video))
            {
                return CreateVideoTrack(sampleEntry, timescale, sampleDuration);
            }
            else if (handlerType == IsoStream.FromFourCC(HandlerTypes.Sound))
            {
                return CreateAudioTrack(sampleEntry, timescale, sampleDuration);
            }
            else
            {
                return CreateGenericTrack(sampleEntry, timescale, sampleDuration, handlerType, handlerName);
            }
        }

        private static ITrack CreateVideoTrack(Box sampleEntry, uint timescale, int sampleDuration)
        {
            switch (IsoStream.ToFourCC(sampleEntry.FourCC))
            {
                case "avc1":
                case "avc2":
                case "avc3":
                case "avc4":
                    return new H264Track(sampleEntry, timescale, sampleDuration);

                case "hvc1":
                case "hvc2":
                case "hvc3":
                case "hev1":
                case "hev2":
                case "hev3":
                    return new H265Track(sampleEntry, timescale, sampleDuration);

                case "vvc1":
                case "vvcN":
                    return new H266Track(sampleEntry, timescale, sampleDuration);

                default:
                    return CreateGenericTrack(sampleEntry, timescale, sampleDuration, IsoStream.FromFourCC(HandlerTypes.Video), HandlerNames.Video);
            }
        }

        private static ITrack CreateAudioTrack(Box sampleEntry, uint timescale, int sampleDuration)
        {
            switch (IsoStream.ToFourCC(sampleEntry.FourCC))
            {
                case "mp4a":
                    return new AACTrack(sampleEntry, timescale, sampleDuration);

                case "Opus":
                    return new OpusTrack(sampleEntry, timescale, sampleDuration);

                default:
                    return CreateGenericTrack(sampleEntry, timescale, sampleDuration, IsoStream.FromFourCC(HandlerTypes.Sound), HandlerNames.Sound);
            }
        }

        private static ITrack CreateGenericTrack(Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            return new GenericTrack(sampleEntry, timescale, sampleDuration, handlerType, handlerName);
        }
    }
}
