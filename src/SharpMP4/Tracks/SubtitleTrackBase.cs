using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>What subtitle tracks share: their samples go to the file as they are, each a random access point.</summary>
    public abstract class SubtitleTrackBase : TrackBase, ISubtitleTrack
    {
        public override string Language { get; set; } = "und";

        /// <summary>The scheme of DASH's roles, as a 'kind' box names it (ISO/IEC 23009-1 5.8.5.5).</summary>
        public const string DashRoleScheme = "urn:mpeg:dash:role:2011";

        /// <summary>DASH's role of a forced subtitle track.</summary>
        public const string ForcedSubtitleRole = "forced-subtitle";

        public bool Forced { get; set; }

        /// <summary>
        /// The 'udta' a track's 'trak' holds, with what it says of the track: of a forced subtitle track, a 'kind' box of the
        /// DASH role 'forced-subtitle', which ffmpeg reads as its forced disposition. Null where there is nothing to say.
        /// </summary>
        public static UserDataBox CreateUserDataBox(ITrack track)
        {
            if (track is not ISubtitleTrack subtitles || !subtitles.Forced)
                return null;

            var udta = new UserDataBox { Children = new List<Box>() };
            var kind = new KindBox { SchemeURI = Terminated(DashRoleScheme), Value = Terminated(ForcedSubtitleRole) };
            kind.SetParent(udta);
            udta.Children.Add(kind);
            return udta;
        }

        /// <summary>Whether a 'trak' says its track is forced: a 'kind' box in its 'udta' of the DASH role 'forced-subtitle'.</summary>
        public static bool IsForced(TrackBox trak) =>
            trak?.Children?.OfType<UserDataBox>().SelectMany(udta => udta.Children ?? new List<Box>()).OfType<KindBox>()
                .Any(kind => kind.SchemeURI.Text == DashRoleScheme && kind.Value.Text == ForcedSubtitleRole) ?? false;

        /// <summary>The sample entry the track was read with, which it writes back as it was; null of a track made to be written.</summary>
        public Box SampleEntry { get; protected set; }

        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // no picture: no width and height
        }

        public abstract IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration);

        public abstract byte[] CreateSample(IReadOnlyList<SubtitleCue> cues);

        /// <summary>The boxes a sample is made of, as WebVTT's are.</summary>
        protected static IList<Box> ReadBoxes(byte[] buffer, int offset, int length)
        {
            var container = new Container();
            container.Read(new IsoStream(new StreamWrapper(new MemoryStream(buffer, offset, length, writable: false))));
            return container.Children;
        }

        protected static byte[] WriteBoxes(IEnumerable<Box> boxes)
        {
            var container = new Container();
            foreach (var box in boxes)
            {
                box.SetParent(container);
                container.Children.Add(box);
            }
            using var output = new MemoryStream();
            container.Write(new IsoStream(new StreamWrapper(output)));
            return output.ToArray();
        }

        /// <summary>A string of a sample entry, ended with a zero: the bytes of one hold it, as they are read.</summary>
        protected static BinaryUTF8String Terminated(string text) => new BinaryUTF8String(text + "\0");

        protected static SubtitleCue[] Cue(string text, long start, long duration) =>
            new[] { new SubtitleCue { Start = start, End = start + duration, Text = text } };
    }
}
