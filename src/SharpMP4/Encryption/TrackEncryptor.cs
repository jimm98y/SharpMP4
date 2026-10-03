using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// Protects a track as a builder writes it (ISO/IEC 23001-7): each sample with the key, its IV and, of H.264, H.265
    /// and H.266 video, its NAL units' slice headers left clear; its sample entry made 'encv', 'enca' or 'enct'; and the
    /// boxes that say how each sample is protected - 'senc', 'saiz' and 'saio' - for the builder to put where it writes
    /// them: in each 'traf', or in the 'trak' and its 'stbl'.
    /// </summary>
    internal sealed class TrackEncryptor
    {
        private SubsampleSplitter _splitter;
        private bool _splitterMade;
        private byte[] _nextIV;

        public TrackProtection Protection { get; }
        public byte[] Key { get; }
        public IMp4Logger Logger { get; }

        private TrackEncryptor(TrackProtection protection, byte[] key, IMp4Logger logger)
        {
            Protection = protection;
            Key = key;
            Logger = logger;
            if (protection.DefaultPerSampleIVSize > 0)
                _nextIV = TrackProtection.RandomBytes(protection.DefaultPerSampleIVSize);
        }

        /// <summary>
        /// The encryptor of a track, its protection and key checked; the protection says what the key is.
        /// </summary>
        public static TrackEncryptor Create(ITrack track, TrackProtection protection, byte[] key, IMp4Logger logger)
        {
            if (protection == null)
                throw new ArgumentNullException(nameof(protection));
            if (key == null || (key.Length != 16 && key.Length != 32))
                throw new ArgumentException("The key is 16 bytes, or 32 for AES-256.", nameof(key));
            if (protection.DefaultKeyId == null || protection.DefaultKeyId.Length != 16)
                throw new ArgumentException("The key ID is 16 bytes.", nameof(protection));
            if (protection.Scheme != ProtectionSchemes.Cenc && protection.Scheme != ProtectionSchemes.Cbc1 &&
                protection.Scheme != ProtectionSchemes.Cens && protection.Scheme != ProtectionSchemes.Cbcs)
                throw new NotSupportedException($"Unsupported protection scheme: {protection.Scheme}");
            if (protection.DefaultPerSampleIVSize == 0 ? protection.DefaultConstantIV?.Length != 16 && protection.DefaultConstantIV?.Length != 8
                                                       : protection.DefaultPerSampleIVSize != 8 && protection.DefaultPerSampleIVSize != 16)
                throw new ArgumentException("The IVs are 8 or 16 bytes: each sample's, or the constant IV.", nameof(protection));

            // the sample entry each kind of track is protected in, which a track of any other kind has none of
            ProtectedSampleEntryType(track.HandlerType);

            // what the key is, the 'tenc' says (23001-7:2023 DAM 1)
            protection.UseAes256 = key.Length == 32;
            return new TrackEncryptor(protection, key, logger);
        }

        /// <summary>
        /// The type a protected sample entry takes, by the kind of its track (14496-12 8.12.1): 'encv' of video, 'enca' of
        /// audio, 'enct' of text and subtitles. Their samples are protected whole but for NAL unit video's (23001-7, 9.4).
        /// </summary>
        public static string ProtectedSampleEntryType(string handlerType)
        {
            switch (handlerType)
            {
                case "vide": return "encv";
                case "soun": return "enca";
                case "text":
                case "sbtl":
                case "subt": return "enct";
                default: throw new NotSupportedException($"A track of handler type '{handlerType}' cannot be protected: 14496-12 gives no protected sample entry for it.");
            }
        }

        /// <summary>
        /// A sample protected, in a copy of its own - the caller's is not written over: its IV, its subsamples - of NAL unit
        /// video, one a NAL unit, its slice header clear - and its bytes encrypted with the key.
        /// </summary>
        public ArraySegment<byte> Protect(ITrack track, ArraySegment<byte> sample, out SampleEncryption encryption)
        {
            byte[] data = new byte[sample.Count];
            Buffer.BlockCopy(sample.Array, sample.Offset, data, 0, sample.Count);

            if (!_splitterMade)
            {
                _splitterMade = true;
                if (track.HandlerType == HandlerTypes.Video)
                {
                    _splitter = SubsampleSplitter.For(track.CreateSampleEntryBox(), Logger);
                    if (_splitter == null)
                        throw new NotSupportedException("Of video, only H.264, H.265, H.266, AV1, AV2 and VP9 can be protected: the subsamples of the others' samples are not known.");
                    if (!_splitter.Supports(Protection.Scheme))
                        throw new NotSupportedException($"The binding of the track's codec does not allow the scheme '{Protection.Scheme}'.");
                }
            }

            encryption = new SampleEncryption
            {
                KeyId = Protection.DefaultKeyId,
                CryptByteBlock = Protection.DefaultCryptByteBlock,
                SkipByteBlock = Protection.DefaultSkipByteBlock,
                IV = Protection.DefaultPerSampleIVSize == 0 ? Protection.DefaultConstantIV : TakeIV(data.Length),
                // every protected range whole blocks but for 'cbcs', whose ranges start on the first byte of slice data (10.4.1)
                Subsamples = _splitter?.Split(data, 0, data.Length, wholeBlocks: Protection.Scheme != ProtectionSchemes.Cbcs),
            };

            CommonEncryption.Encrypt(Protection.Scheme, Key, encryption, data, 0, data.Length);
            return new ArraySegment<byte>(data);
        }

        /// <summary>
        /// The IV of a sample, and the next one's ready: for AES-CTR the one counted on, so that no two samples share a
        /// counter under the key (9.2, 10.1, 10.3) - by one where the IV is 8 bytes, the block counter below it, and by
        /// the sample's blocks where it is 16, as a number of 128 bits; for 'cbc1' a random one.
        /// </summary>
        private byte[] TakeIV(int sampleLength)
        {
            byte[] iv = _nextIV;
            if (Protection.Scheme == ProtectionSchemes.Cbc1)
            {
                _nextIV = TrackProtection.RandomBytes(iv.Length);
                return iv;
            }

            byte[] next = (byte[])iv.Clone();
            ulong step = next.Length == 8 ? 1 : (ulong)(sampleLength + 15) / 16 + 1;
            for (int i = next.Length - 1; i >= 0 && step != 0; i--)
            {
                ulong sum = next[i] + (step & 0xFF);
                next[i] = (byte)sum;
                step = (step >> 8) + (sum >> 8);
            }
            _nextIV = next;
            return iv;
        }

        /// <summary>
        /// A sample entry protected (14496-12 8.12): of type 'encv', 'enca' or 'enct', with a 'sinf' holding what it was and
        /// how its samples are protected.
        /// </summary>
        public void ProtectSampleEntry(ITrack track, Box sampleEntry)
        {
            uint original = sampleEntry.FourCC;
            Protection.OriginalFormat = IsoStream.ToFourCC(original);
            sampleEntry.FourCC = IsoStream.FromFourCC(ProtectedSampleEntryType(track.HandlerType));

            var sinf = Protection.CreateProtectionSchemeInfoBox(original);
            if (sampleEntry.Children == null)
                sampleEntry.Children = new List<Box>();
            sinf.SetParent(sampleEntry);
            sampleEntry.Children.Add(sinf);
        }

        /// <summary>The protection systems' headers of the tracks, each once, as 'pssh' boxes.</summary>
        public static List<ProtectionSystemSpecificHeaderBox> ProtectionSystemHeaders(IEnumerable<TrackProtection> protections)
        {
            var seen = new HashSet<string>();
            var boxes = new List<ProtectionSystemSpecificHeaderBox>();
            foreach (var header in protections.SelectMany(p => p.Systems))
            {
                string key = $"{BitConverter.ToString(header.SystemId)}|{BitConverter.ToString(header.Data ?? Array.Empty<byte>())}|{string.Join(",", header.KeyIds?.Select(k => BitConverter.ToString(k)) ?? Enumerable.Empty<string>())}";
                if (seen.Add(key))
                    boxes.Add(TrackProtection.CreateProtectionSystemSpecificHeaderBox(header));
            }
            return boxes;
        }

        /// <summary>
        /// How the samples are protected: each sample's IV and subsamples in a 'senc', and the same as sample auxiliary
        /// information - their sizes in 'saiz', where they are in 'saio', for the builder to point at the 'senc's samples,
        /// which start 16 bytes into it - unless there is none, as of samples with a constant IV and no subsamples (7.1,
        /// 10.4.1); then those two are null. A sample's information of more than 255 bytes - of more than 40 subsamples,
        /// as of AV1's many tiles - takes 'saiz' version 1, of 16 bit sizes, or 2, of 32 (14496-12 9th edition).
        /// </summary>
        public (SampleEncryptionBox Senc, SampleAuxiliaryInformationSizesBox Saiz, SampleAuxiliaryInformationOffsetsBox Saio) CreateSampleEncryptionBoxes(IList<SampleEncryption> encryptions, IList<uint> sampleSizes)
        {
            byte ivSize = Protection.DefaultPerSampleIVSize;
            bool subsamples = encryptions.Any(e => e.Subsamples != null);
            uint flags = subsamples ? 0x2u : 0u;

            // each sample's IV, then its subsamples: a count, and of each its clear and protected bytes (7.2)
            var data = new List<byte>();
            var sizes = new uint[encryptions.Count];
            for (int i = 0; i < encryptions.Count; i++)
            {
                var encryption = encryptions[i];
                int start = data.Count;
                if (ivSize > 0)
                    data.AddRange(encryption.IV);
                if (subsamples)
                {
                    // a sample protected whole, among ones in subsamples, is one subsample of its own
                    var runs = encryption.Subsamples ?? new[] { new EncryptionSubsample(0, sampleSizes[i]) };
                    data.Add((byte)(runs.Length >> 8));
                    data.Add((byte)runs.Length);
                    foreach (var run in runs)
                    {
                        data.Add((byte)(run.ClearBytes >> 8));
                        data.Add((byte)run.ClearBytes);
                        data.Add((byte)(run.ProtectedBytes >> 24));
                        data.Add((byte)(run.ProtectedBytes >> 16));
                        data.Add((byte)(run.ProtectedBytes >> 8));
                        data.Add((byte)run.ProtectedBytes);
                    }
                }
                sizes[i] = (uint)(data.Count - start);
            }

            var senc = new SampleEncryptionBox(0, flags)
            {
                SampleCount = (uint)encryptions.Count,
                SampleData = data.ToArray(),
            };

            if (!sizes.Any(size => size > 0))
                return (senc, null, null);

            // version 0 where the sizes fit its 8 bits, as readers before the 9th edition read it
            uint largest = sizes.Max();
            byte version = largest <= byte.MaxValue ? (byte)0 : largest <= ushort.MaxValue ? (byte)1 : (byte)2;
            bool same = sizes.All(size => size == sizes[0]);
            var saiz = new SampleAuxiliaryInformationSizesBox(version)
            {
                DefaultSampleInfoSize = same ? sizes[0] : 0u,
                SampleCount = (uint)sizes.Length,
                SampleInfoSize = same ? null : sizes,
            };
            var saio = new SampleAuxiliaryInformationOffsetsBox(0, 0)
            {
                EntryCount = 1,
                Offset = new ulong[] { 0 },
            };
            return (senc, saiz, saio);
        }
    }
}
