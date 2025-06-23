using System.Collections.Generic;
using System.Linq;
using SharpISOBMFF;

namespace SharpMP4
{
    public static class Mp4Extensions
    {
        public static IEnumerable<TrackBox> FindVideoTracks(this Container mp4)
        {
            return mp4.GetMovieBox().GetTracks()
               .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Video));
        }

        public static IEnumerable<TrackBox> FindAudioTracks(this Container mp4)
        {
            return mp4.GetMovieBox().GetTracks()
                .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Sound));
        }

        public static IEnumerable<TrackBox> FindHintTracks(this Container mp4)
        {
            return mp4.GetMovieBox().GetTracks()
                .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Hint));
        }

        public static MovieBox GetMovieBox(this Container mp4)
        {
            return mp4.Children.OfType<MovieBox>().Single();
        }

        public static IEnumerable<TrackBox> GetTracks(this MovieBox moov)
        {
            return moov.Children.OfType<TrackBox>();
        }

        public static AudioSampleEntry GetAudioSampleEntryBox(this TrackBox track)
        {
            return track
                .Children.OfType<MediaBox>().Single()
                .Children.OfType<MediaInformationBox>().Single()
                .Children.OfType<SampleTableBox>().Single()
                .Children.OfType<SampleDescriptionBox>().Single()
                .Children.OfType<AudioSampleEntry>().Single();
        }
    }
}
