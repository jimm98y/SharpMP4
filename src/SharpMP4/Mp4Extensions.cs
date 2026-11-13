// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
