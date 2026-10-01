using SharpISOBMFF;
using SharpMP4.Common;
using System;
using System.Linq;

namespace SharpMP4.Tracks
{
    public delegate ITrack TrackCreator(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName, IMp4Logger logger);

    public class TrackFactory
    {
        public TrackCreator CreateTrack { get; set; } = DefaultCreateTrack;

        public static ITrack DefaultCreateTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName, IMp4Logger logger)
        {
            ITrack track = DefaultCreateTrackInternal(trackID, sampleEntry, timescale, sampleDuration, handlerType, handlerName);

            track.Logger = logger ?? DefaultMp4Logger.Instance;

            return track;
        }

        private static ITrack DefaultCreateTrackInternal(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            // subtitles and timed text are known by their sample entry, whichever handler - 'text', 'subt', 'sbtl' - they are of
            ITrack subtitles = CreateSubtitleTrack(trackID, sampleEntry, timescale, handlerType);
            if (subtitles != null)
            {
                return subtitles;
            }

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
            // QuickTime's entries of H.261, MPEG-1 and MPEG-2 video have no configuration: known by their own coding, whichever
            // of their boxes - 'fiel', 'colr', 'pasp' - is given
            if ((sampleEntry as VisualSampleEntry ?? sampleEntry.GetParent() as VisualSampleEntry) is VisualSampleEntry visualEntry)
            {
                string coding = Encryption.SubsampleSplitter.CodingOf(visualEntry);
                if (coding == "H261")
                    return new H261Track(visualEntry, timescale, sampleDuration) { TrackID = trackID };
                if (H262Track.QuickTimeEntries.ContainsKey(coding))
                    return new H262Track(visualEntry, coding, timescale, sampleDuration) { TrackID = trackID };
            }

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

                case "av2C":
                    return new AV2Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                // of an 'mp4v' entry of MPEG-2 or MPEG-1 video - MPEG-4 Visual's has one too
                case "esds" when sampleEntry is ESDBox esds && esds._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault() is DecoderConfigDescriptor config
                    && H262Track.IsH262(config.ObjectTypeIndication):
                    return new H262Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                // of an 'mp4v' entry of MPEG-4 Visual
                case "esds" when sampleEntry is ESDBox esds && esds._ES?.Children?.OfType<DecoderConfigDescriptor>().FirstOrDefault() is DecoderConfigDescriptor config
                    && MPEG4VisualTrack.IsMPEG4Visual(config.ObjectTypeIndication):
                    return new MPEG4VisualTrack(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                // of an 's263' or 'h263' entry (3GPP TS 26.244)
                case "d263":
                    return new H263Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

                // of a 'vp09' entry - VP8's 'vp08' has one too
                case "vpcC" when sampleEntry.GetParent() is Box entry && Encryption.SubsampleSplitter.CodingOf(entry) == "vp09":
                    return new VP9Track(sampleEntry, timescale, sampleDuration) { TrackID = trackID };

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

        /// <summary>
        /// The track of a sample entry of subtitles or timed text: WebVTT's 'wvtt', TTML's 'stpp', simple text's 'stxt' and
        /// 3GPP timed text's 'tx3g'. Null of any other.
        /// </summary>
        private static ITrack CreateSubtitleTrack(uint trackID, Box box, uint timescale, uint handlerType)
        {
            // given the sample entry's first child where it has one: its configuration, as of video and audio
            Box entry = box is SampleEntry ? box : box?.GetParent() as SampleEntry ?? box;
            string handler = IsoStream.ToFourCC(handlerType);
            switch (entry)
            {
                case WVTTSampleEntry wvtt:
                    return new WebVttTrack(wvtt, timescale) { TrackID = trackID };
                case XMLSubtitleSampleEntry stpp:
                    return new TtmlTrack(stpp, timescale, handler) { TrackID = trackID };
                case SimpleTextSampleEntry stxt:
                    return new SimpleTextTrack(stxt, timescale) { TrackID = trackID };
                case TextSampleEntrytx3gDup tx3g:
                    return new TimedTextTrack(tx3g, timescale, handler) { TrackID = trackID };
                default:
                    return null;
            }
        }

        public static ITrack CreateGenericTrack(uint trackID, Box sampleEntry, uint timescale, int sampleDuration, uint handlerType, string handlerName)
        {
            return new GenericTrack(sampleEntry, timescale, sampleDuration, handlerType, handlerName) { TrackID = trackID };
        }
    }
}
