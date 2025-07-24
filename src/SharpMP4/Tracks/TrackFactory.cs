using SharpISOBMFF;
using System;

namespace SharpMP4.Tracks
{
    public class TrackFactory
    {
        public static Func<uint, Box, uint, int, uint, string, ITrack> CreateTrack = DefaultCreateTrack;

        public static ITrack DefaultCreateTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            if (handlerType == IsoStream.FromFourCC(HandlerTypes.Video))
            {
                return CreateVideoTrack(trackID, sampleEntry, timescale, sampleDuration);
            }
            else if (handlerType == IsoStream.FromFourCC(HandlerTypes.Sound))
            {
                return CreateAudioTrack(trackID, sampleEntry, timescale, sampleDuration);
            }
            else
            {
                return CreateGenericTrack(trackID, sampleEntry, timescale, sampleDuration, handlerType, handlerName);
            }
        }

        private static ITrack CreateVideoTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration)
        {
            switch (IsoStream.ToFourCC(sampleEntry.FourCC))
            {
                case "avc1":
                case "avc2":
                case "avc3":
                case "avc4":
                    return new H264Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "hvc1":
                case "hvc2":
                case "hvc3":
                case "hev1":
                case "hev2":
                case "hev3":
                    return new H265Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "vvc1":
                case "vvcN":
                    return new H266Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "av01":
                    return new AV1Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                default:
                    return CreateGenericTrack(trackID, sampleEntry, timescale, sampleDuration, IsoStream.FromFourCC(HandlerTypes.Video), HandlerNames.Video);
            }
        }

        private static ITrack CreateAudioTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration)
        {
            switch (IsoStream.ToFourCC(sampleEntry.FourCC))
            {
                case "mp4a":
                    return new AACTrack(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "Opus":
                    return new OpusTrack(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                default:
                    return CreateGenericTrack(trackID, sampleEntry, timescale, sampleDuration, IsoStream.FromFourCC(HandlerTypes.Sound), HandlerNames.Sound);
            }
        }

        private static ITrack CreateGenericTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            return new GenericTrack(sampleEntry, timescale, sampleDuration, handlerType, handlerName) { TrackID = trackID };
        }
    }
}
