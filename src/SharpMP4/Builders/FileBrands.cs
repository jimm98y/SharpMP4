using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// The 'ftyp' of a file a builder writes: the brand of its format (<see cref="Mp4FileFormat"/>), brands of the features
    /// the file has, as ISO/IEC 14496-12 Annex E defines them, and of the codecs its tracks are of
    /// (<see cref="ITrack.CompatibleBrand"/>); and what of the format's constraints the tracks are held to.
    /// </summary>
    internal static class FileBrands
    {
        /// <summary>
        /// Whether the tracks of a file - those it has and the one being added - fit its format: a track the format cannot
        /// hold throws, of a codec it does not list is warned of. Each track is checked as it is added, and all of them
        /// again as the 'ftyp' is made, the format being settable between.
        /// </summary>
        public static void Validate(Mp4FileFormat format, IReadOnlyCollection<ITrack> tracks, IMp4Logger logger)
        {
            // a sample entry only QuickTime has makes a QuickTime file: of an MP4 the builder makes it one
            if (format != Mp4FileFormat.Mp4 && format != Mp4FileFormat.QuickTime &&
                tracks.FirstOrDefault(track => track.CompatibleBrand == TrackBase.QuickTimeBrand) is ITrack quickTime)
                throw new InvalidOperationException($"A {quickTime.GetType().Name} is of a sample entry only QuickTime has: it cannot be in a file of {format}.");

            switch (format)
            {
                case Mp4FileFormat.M4A:
                case Mp4FileFormat.M4B:
                    if (tracks.FirstOrDefault(track => track.HandlerType != HandlerTypes.Sound && track.HandlerType != HandlerTypes.Text) is ITrack notAudio)
                        throw new InvalidOperationException($"A file of {format} holds audio, and text of chapters: not a track of the handler '{notAudio.HandlerType}'.");
                    break;

                case Mp4FileFormat.ThreeGpp:
                    // the Basic profile (TS 26.244 5.4.3): of video, audio and text one track each at most, no other
                    foreach (var group in tracks.GroupBy(track => track.HandlerType))
                    {
                        if (group.Key != HandlerTypes.Video && group.Key != HandlerTypes.Sound && group.Key != HandlerTypes.Text)
                            throw new InvalidOperationException($"A 3GP file of the Basic profile holds video, audio and text: not a track of the handler '{group.Key}'.");
                        if (group.Count() > 1)
                            throw new InvalidOperationException($"A 3GP file of the Basic profile holds one track of each media type: not {group.Count()} of the handler '{group.Key}'.");
                    }
                    foreach (var track in tracks.Where(track => !IsThreeGppCodec(track)))
                    {
                        if (logger?.IsWarningEnabled == true)
                            logger.LogWarning($"A {track.GetType().Name} is of a codec 3GP files do not list: a 3GP player may not play it.");
                    }
                    break;
            }
        }

        // The codecs of 3GP (TS 26.244 6): H.263, MPEG-4 Visual, AVC and HEVC video; AAC audio - AMR has no track here -;
        // 3GPP timed text
        private static bool IsThreeGppCodec(ITrack track) =>
            track is H263Track || track is MPEG4Track || track is H264Track || track is H265Track || track is AACTrack || track is TimedTextTrack;

        /// <summary>The 'ftyp' of the tracks of a file of a format, fragmented or not, and the features it has.</summary>
        /// <param name="isProtected">Whether a track is protected: its 'sinf' (iso2), its 'saiz' and 'saio' (iso6).</param>
        /// <param name="hasSubtitleMediaHeader">Whether a track of subtitles has an 'sthd' (iso8).</param>
        public static FileTypeBox Create(Mp4FileFormat format, IReadOnlyCollection<ITrack> tracks, bool fragmented, bool isProtected, bool hasSubtitleMediaHeader, IMp4Logger logger)
        {
            Validate(format, tracks, logger);

            var codecBrands = tracks.Select(track => track.CompatibleBrand).Where(brand => !string.IsNullOrEmpty(brand)).Distinct().ToList();

            // a QuickTime file says so alone; a track of a sample entry only QuickTime has - 'H261', 'xd5c' - makes an MP4 one
            if (format == Mp4FileFormat.QuickTime || codecBrands.Contains(TrackBase.QuickTimeBrand))
                return FileType(TrackBase.QuickTimeBrand, 0x200, new List<string> { TrackBase.QuickTimeBrand });

            // the format's own brands, first
            var brands = new List<string>();
            uint minor = 512;
            switch (format)
            {
                case Mp4FileFormat.M4V:
                    brands.Add("M4V ");
                    break;
                case Mp4FileFormat.M4A:
                    brands.Add("M4A ");
                    break;
                case Mp4FileFormat.M4B:
                    brands.Add("M4B ");
                    brands.Add("M4A ");
                    break;
                case Mp4FileFormat.ThreeGpp:
                    // Releases 4 and 5 do not allow movie fragments: a fragmented file is not marked as theirs (TS 26.244 5.5)
                    brands.Add("3gp6");
                    if (!fragmented)
                    {
                        brands.Add("3gp5");
                        brands.Add("3gp4");
                    }
                    break;
                case Mp4FileFormat.ThreeGpp2:
                    brands.Add("3g2a");
                    minor = 0x10000;
                    break;
            }

            if (fragmented)
            {
                // the default-base-is-moof flag of every 'tfhd' (iso5), 'tfdt' and 'trun' of version 1 (iso6): of a file
                // marked 'isom', 'avc1' or 'iso2' to 'iso4' the flag cannot be set (14496-12 Annex E)
                brands.Add("iso6");
                codecBrands.Remove(H264Track.BRAND);
            }
            else
            {
                brands.Add("isom");
                if (isProtected)
                    brands.Add("iso6");
            }

            if (hasSubtitleMediaHeader)
                brands.Add("iso8");
            brands.Add("mp41");
            brands.AddRange(codecBrands);
            return FileType(brands[0], minor, brands);
        }

        private static FileTypeBox FileType(string major, uint minor, List<string> brands) => new FileTypeBox
        {
            MajorBrand = IsoStream.FromFourCC(major),
            MinorVersion = minor,
            CompatibleBrands = brands.Distinct().Select(IsoStream.FromFourCC).ToArray(),
        };
    }
}
