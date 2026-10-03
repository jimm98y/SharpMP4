using SharpISOBMFF;
using SharpMP4.Tracks;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>
    /// The 'ftyp' of a file a builder writes: brands of the features the file has, as ISO/IEC 14496-12 Annex E defines
    /// them, and of the codecs its tracks are of (<see cref="ITrack.CompatibleBrand"/>).
    /// </summary>
    internal static class FileBrands
    {
        /// <summary>The 'ftyp' of the tracks of a file, fragmented or not, and the features it has.</summary>
        /// <param name="isProtected">Whether a track is protected: its 'sinf' (iso2), its 'saiz' and 'saio' (iso6).</param>
        /// <param name="hasSubtitleMediaHeader">Whether a track of subtitles has an 'sthd' (iso8).</param>
        public static FileTypeBox Create(IEnumerable<ITrack> tracks, bool fragmented, bool isProtected, bool hasSubtitleMediaHeader)
        {
            var codecBrands = tracks.Select(track => track.CompatibleBrand).Where(brand => !string.IsNullOrEmpty(brand)).Distinct().ToList();

            // a track of a sample entry only QuickTime has - 'H261', 'xd5c' - makes a QuickTime file, which says so alone
            if (codecBrands.Contains(TrackBase.QuickTimeBrand))
                return FileType(TrackBase.QuickTimeBrand, 0x200, new List<string> { TrackBase.QuickTimeBrand });

            string major;
            var brands = new List<string>();
            if (fragmented)
            {
                // the default-base-is-moof flag of every 'tfhd' (iso5), 'tfdt' and 'trun' of version 1 (iso6): of a file
                // marked 'isom', 'avc1' or 'iso2' to 'iso4' the flag cannot be set (14496-12 Annex E)
                major = "iso6";
                brands.Add("iso6");
                codecBrands.Remove(H264Track.BRAND);
            }
            else
            {
                major = "isom";
                brands.Add("isom");
                if (isProtected)
                    brands.Add("iso6");
            }

            if (hasSubtitleMediaHeader)
                brands.Add("iso8");
            brands.Add("mp41");
            brands.AddRange(codecBrands);
            return FileType(major, 512, brands);
        }

        private static FileTypeBox FileType(string major, uint minor, List<string> brands) => new FileTypeBox
        {
            MajorBrand = IsoStream.FromFourCC(major),
            MinorVersion = minor,
            CompatibleBrands = brands.Distinct().Select(IsoStream.FromFourCC).ToArray(),
        };
    }
}
