using SharpISOBMFF;
using SharpMP4Common;
using System;

namespace SharpMP4.Tracks
{
    public delegate ITrack TrackCreator(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName, IMp4Logger logger);

    public class TrackFactory
    {
        public TrackCreator CreateTrack { get; set; } = DefaultCreateTrack;

        public static ITrack DefaultCreateTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName, IMp4Logger logger)
        {
            ITrack track = DefaultCreateTrackInternal(trackID, sampleEntry, timescale, sampleDuration, handlerType, handlerName);

            track.Logger = logger ?? new DefaultMp4Logger();

            return track;
        }

        private static ITrack DefaultCreateTrackInternal(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
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
                case "avcC":
                    return new H264Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "hvcC":
                    return new H265Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "vvcC":
                    return new H266Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "av1C":
                    return new AV1Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                default:
                    throw new NotSupportedException($"Unsupported video codec: {IsoStream.ToFourCC(sampleEntry.FourCC)}");
            }
        }

        private static ITrack CreateAudioTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration)
        {
            switch (IsoStream.ToFourCC(sampleEntry.FourCC))
            {
                case "esds": // mp4
                case "wave": // quicktime
                    return new AACTrack(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                case "dOps":
                    return new OpusTrack(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                default:
                    throw new NotSupportedException($"Unsupported audio codec: {IsoStream.ToFourCC(sampleEntry.FourCC)}");
            }
        }

        public static ITrack CreateGenericTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            return new GenericTrack(sampleEntry, timescale, sampleDuration, handlerType, handlerName) { TrackID = trackID };
        }
    }
}
