using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace SharpMP4.Encryption
{
    /// <summary>A protection system's header ('pssh'): the DRM's own data, for the keys it names.</summary>
    public class ProtectionSystemHeader
    {
        /// <summary>The 16 byte ID of the DRM system: Widevine's edef8ba9-79d6-4ace-a3c8-27dcd51d21ed, and so on.</summary>
        public byte[] SystemId { get; set; }

        /// <summary>The IDs of the keys the data is for, which a version 1 'pssh' lists; null for a version 0 one.</summary>
        public List<byte[]> KeyIds { get; set; }

        public byte[] Data { get; set; }

        public static ProtectionSystemHeader From(Box box)
        {
            switch (box)
            {
                case ProtectionSystemSpecificHeaderBox pssh:
                    return new ProtectionSystemHeader
                    {
                        SystemId = pssh.SystemId,
                        KeyIds = pssh.KeyIDs?.Select(k => k.Key).ToList(),
                        Data = pssh.Data,
                    };
                case UuidBasedProtectionSystemSpecificHeaderBox piff:
                    return new ProtectionSystemHeader { SystemId = piff.SystemID, Data = piff.Data };
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// How a track is protected, from its sample entry's 'sinf' (ISO/IEC 14496-12 8.12, ISO/IEC 23001-7): the scheme,
    /// what the sample entry was before it was protected, and the defaults of its samples, from its 'tenc'. Samples
    /// of a 'seig' sample group, and each sample's own IV and subsamples, are in <see cref="SampleEncryption"/>.
    /// </summary>
    public class TrackProtection
    {
        /// <summary>The scheme type: 'cenc', 'cbcs', 'cens', 'cbc1', 'piff', or another's.</summary>
        public string Scheme { get; set; } = ProtectionSchemes.Cenc;

        public uint SchemeVersion { get; set; } = 0x00010000;

        /// <summary>The sample entry's type before it was protected ('avc1', 'mp4a', ...), from 'frma'.</summary>
        public string OriginalFormat { get; set; }

        public bool DefaultIsProtected { get; set; } = true;

        public byte[] DefaultKeyId { get; set; }

        /// <summary>0 where the samples share <see cref="DefaultConstantIV"/>, 8 or 16 where each has an IV of its own.</summary>
        public byte DefaultPerSampleIVSize { get; set; } = 8;

        public byte[] DefaultConstantIV { get; set; }

        public byte DefaultCryptByteBlock { get; set; }

        public byte DefaultSkipByteBlock { get; set; }

        /// <summary>
        /// Whether the samples are protected with a key of 256 bits rather than 128: 'tenc' version 2's use_AES_256, of the
        /// draft of ISO/IEC 23001-7:2023 Amendment 1 (DAM 1, MPEG w25903).
        /// </summary>
        public bool UseAes256 { get; set; }

        /// <summary>The protection systems' headers, of the 'moov'.</summary>
        public List<ProtectionSystemHeader> Systems { get; set; } = new List<ProtectionSystemHeader>();

        /// <summary>The protection of a sample entry, or null where it has no 'sinf'.</summary>
        public static TrackProtection FromSampleEntry(Box sampleEntry)
        {
            var sinf = sampleEntry?.Children?.OfType<ProtectionSchemeInfoBox>().FirstOrDefault();
            if (sinf == null)
                return null;

            var frma = sinf.Children?.OfType<OriginalFormatBox>().FirstOrDefault();
            var schm = sinf.Children?.OfType<SchemeTypeBox>().FirstOrDefault();
            var schi = sinf.Children?.OfType<SchemeInformationBox>().FirstOrDefault();

            var protection = new TrackProtection
            {
                Scheme = schm != null ? IsoStream.ToFourCC(schm.SchemeType) : null,
                SchemeVersion = schm?.SchemeVersion ?? 0,
                OriginalFormat = frma != null ? IsoStream.ToFourCC(frma.DataFormat) : null,
            };

            var tenc = schi?.Children?.OfType<TrackEncryptionBox>().FirstOrDefault();
            var piff = schi?.Children?.OfType<PiffTrackEncryptionBox>().FirstOrDefault();
            if (tenc != null)
            {
                protection.DefaultIsProtected = tenc.DefaultIsProtected != 0;
                protection.DefaultKeyId = tenc.DefaultKID;
                protection.DefaultPerSampleIVSize = tenc.DefaultPerSampleIVSize;
                protection.DefaultConstantIV = tenc.DefaultPerSampleIVSize == 0 ? tenc.DefaultConstantIV : null;
                protection.DefaultCryptByteBlock = tenc.DefaultCryptByteBlock;
                protection.DefaultSkipByteBlock = tenc.DefaultSkipByteBlock;
                protection.UseAes256 = tenc.Version >= 2 && tenc.UseAES256;
            }
            else if (piff != null)
            {
                protection.DefaultIsProtected = piff.AlgorithmID != 0;
                protection.DefaultKeyId = piff.Kid;
                protection.DefaultPerSampleIVSize = piff.PerSampleIVSize;
            }

            return protection;
        }

        /// <summary>
        /// A track's protection for writing, with the defaults its scheme is written with (as Shaka Packager does): for
        /// 'cbcs' a constant IV of 16 bytes, random, and video in the pattern 1:9, audio whole; for 'cens' 8 byte IVs
        /// and video in the pattern 1:9; for 'cenc' 8 byte IVs; for 'cbc1' 16 byte ones.
        /// </summary>
        /// <param name="scheme">One of <see cref="ProtectionSchemes"/>: 'cenc', 'cbc1', 'cens' or 'cbcs'.</param>
        /// <param name="keyId">The 16 byte ID of the key the samples are protected with.</param>
        /// <param name="isVideo">Whether the track is video, which a pattern is for.</param>
        public static TrackProtection Create(string scheme, byte[] keyId, bool isVideo)
        {
            if (keyId == null || keyId.Length != 16)
                throw new ArgumentException("The key ID is 16 bytes.", nameof(keyId));

            var protection = new TrackProtection { Scheme = scheme, DefaultKeyId = keyId };
            switch (scheme)
            {
                case ProtectionSchemes.Cenc:
                    protection.DefaultPerSampleIVSize = 8;
                    break;
                case ProtectionSchemes.Cbc1:
                    protection.DefaultPerSampleIVSize = 16;
                    break;
                case ProtectionSchemes.Cens:
                    protection.DefaultPerSampleIVSize = 8;
                    if (isVideo)
                        (protection.DefaultCryptByteBlock, protection.DefaultSkipByteBlock) = (1, 9);
                    break;
                case ProtectionSchemes.Cbcs:
                    protection.DefaultPerSampleIVSize = 0;
                    protection.DefaultConstantIV = RandomBytes(16);
                    if (isVideo)
                        (protection.DefaultCryptByteBlock, protection.DefaultSkipByteBlock) = (1, 9);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported protection scheme: {scheme}");
            }
            return protection;
        }

        internal static byte[] RandomBytes(int count)
        {
            var bytes = new byte[count];
            using (var random = RandomNumberGenerator.Create())
                random.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// The 'sinf' of a protected sample entry, which becomes 'encv' or 'enca' (14496-12 8.12): what it was in 'frma',
        /// the scheme in 'schm', and the track's defaults in the 'tenc' of 'schi' - of version 1 where there is a pattern.
        /// </summary>
        public ProtectionSchemeInfoBox CreateProtectionSchemeInfoBox(uint originalFormat)
        {
            var sinf = new ProtectionSchemeInfoBox { Children = new List<Box>() };

            var frma = new OriginalFormatBox { DataFormat = originalFormat };
            var schm = new SchemeTypeBox { SchemeType = IsoStream.FromFourCC(Scheme), SchemeVersion = SchemeVersion };
            var schi = new SchemeInformationBox { Children = new List<Box>() };
            // version 1 for a pattern, 2 for a key of 256 bits (23001-7:2023 DAM 1), 0 otherwise
            bool pattern = ProtectionSchemes.IsPattern(Scheme);
            var tenc = new TrackEncryptionBox(UseAes256 ? (byte)2 : pattern ? (byte)1 : (byte)0)
            {
                UseAES256 = UseAes256,
                DefaultCryptByteBlock = pattern ? DefaultCryptByteBlock : (byte)0,
                DefaultSkipByteBlock = pattern ? DefaultSkipByteBlock : (byte)0,
                DefaultIsProtected = DefaultIsProtected ? (byte)1 : (byte)0,
                DefaultPerSampleIVSize = DefaultPerSampleIVSize,
                DefaultKID = DefaultKeyId,
            };
            if (DefaultIsProtected && DefaultPerSampleIVSize == 0)
            {
                tenc.DefaultConstantIVSize = (byte)DefaultConstantIV.Length;
                tenc.DefaultConstantIV = DefaultConstantIV;
            }

            foreach (var (box, parent) in new (Box, Box)[] { (frma, sinf), (schm, sinf), (schi, sinf), (tenc, schi) })
            {
                box.SetParent(parent);
                parent.Children.Add(box);
            }
            return sinf;
        }

        /// <summary>The 'pssh' of a protection system's header: of version 1 where it names the key IDs it is for.</summary>
        public static ProtectionSystemSpecificHeaderBox CreateProtectionSystemSpecificHeaderBox(ProtectionSystemHeader header)
        {
            bool keyIds = header.KeyIds != null && header.KeyIds.Count > 0;
            var pssh = new ProtectionSystemSpecificHeaderBox(keyIds ? (byte)1 : (byte)0)
            {
                SystemId = header.SystemId,
                Data = header.Data ?? Array.Empty<byte>(),
            };
            pssh.DataSize = (uint)pssh.Data.Length;
            if (keyIds)
            {
                pssh.Count = (uint)header.KeyIds.Count;
                pssh.KeyIDs = header.KeyIds.Select(k => new ProtectionSystemSpecificKeyID { Key = k }).ToArray();
            }
            return pssh;
        }

        /// <summary>A sample's protection where nothing more than the track's defaults is known of it.</summary>
        public SampleEncryption DefaultSampleEncryption() => new SampleEncryption
        {
            IsProtected = DefaultIsProtected,
            KeyId = DefaultKeyId,
            IV = DefaultConstantIV,
            CryptByteBlock = DefaultCryptByteBlock,
            SkipByteBlock = DefaultSkipByteBlock,
        };
    }
}
