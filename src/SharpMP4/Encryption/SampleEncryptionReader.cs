using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// Each sample's protection, of a track fragment or of a whole track (ISO/IEC 23001-7, 7): its key, pattern and
    /// whether it is protected at all from the 'seig' group it belongs to by 'sbgp', else from the track's 'tenc';
    /// its IV and subsamples from 'senc' (or PIFF's), else from the sample auxiliary information 'saiz' and 'saio'
    /// point to.
    /// </summary>
    public static class SampleEncryptionReader
    {
        private static readonly uint Seig = IsoStream.FromFourCC("seig");
        private static readonly uint Cenc = IsoStream.FromFourCC("cenc");

        /// <summary>
        /// The protection of each sample of a track fragment.
        /// </summary>
        /// <param name="protection">The track's protection.</param>
        /// <param name="traf">The track fragment.</param>
        /// <param name="stbl">The track's sample table, for the 'seig' groups a fragment may refer to.</param>
        /// <param name="sampleCount">How many samples the fragment has.</param>
        /// <param name="auxiliaryBase">Where the offsets of a 'saio' of the fragment count from: its base data offset.</param>
        /// <param name="read">Reads so many bytes at an offset of the file.</param>
        public static SampleEncryption[] ForFragment(TrackProtection protection, TrackFragmentBox traf, SampleTableBox stbl, int sampleCount, long auxiliaryBase, Func<long, int, byte[]> read)
        {
            var groups = GroupsOf(traf.Children, stbl?.Children, sampleCount, fragment: true);
            var samples = Defaults(protection, groups, sampleCount, out int[] ivSizes);

            var senc = traf.Children.OfType<SampleEncryptionBox>().FirstOrDefault();
            var piff = traf.Children.OfType<PiffSampleEncryptionBox>().FirstOrDefault();
            if (senc != null)
                ApplySampleEncryptionBox(samples, senc.Samples, senc.Flags);
            else if (piff != null)
                ApplySampleEncryptionBox(samples, piff.Samples, piff.Flags);
            else
                ApplyAuxiliaryInformation(samples, ivSizes, traf.Children, sampleCount, read, offsetOfEntry: _ => auxiliaryBase, chunkOfSample: null);

            return samples;
        }

        /// <summary>
        /// The protection of each sample of a track that is not fragmented: from its 'stbl', whose 'saio' offsets are
        /// the file's, one for all the samples or one a chunk.
        /// </summary>
        public static SampleEncryption[] ForTrack(TrackProtection protection, SampleTableBox stbl, uint[] samplesInChunk, Func<long, int, byte[]> read)
        {
            int sampleCount = samplesInChunk.Sum(n => (int)n);
            var groups = GroupsOf(stbl.Children, null, sampleCount, fragment: false);
            var samples = Defaults(protection, groups, sampleCount, out int[] ivSizes);

            // a 'senc' of the whole track is in its 'trak' (23001-7, 7.2.1) - or, as some write it, in the 'stbl'
            var trak = TrackOf(stbl);
            var boxes = stbl.Children.Concat(trak?.Children ?? Enumerable.Empty<Box>()).ToList();
            var senc = boxes.OfType<SampleEncryptionBox>().FirstOrDefault();
            var piff = boxes.OfType<PiffSampleEncryptionBox>().FirstOrDefault();
            if (senc != null)
            {
                ApplySampleEncryptionBox(samples, senc.Samples, senc.Flags);
                return samples;
            }
            if (piff != null)
            {
                ApplySampleEncryptionBox(samples, piff.Samples, piff.Flags);
                return samples;
            }

            // the chunk each sample is in, and the first sample of each chunk
            var chunkOf = new int[sampleCount];
            var firstOfChunk = new int[samplesInChunk.Length];
            for (int chunk = 0, sample = 0; chunk < samplesInChunk.Length; chunk++)
            {
                firstOfChunk[chunk] = sample;
                for (int k = 0; k < samplesInChunk[chunk] && sample < sampleCount; k++)
                    chunkOf[sample++] = chunk;
            }

            ApplyAuxiliaryInformation(samples, ivSizes, stbl.Children, sampleCount, read, offsetOfEntry: _ => 0, chunkOfSample: (chunkOf, firstOfChunk));
            return samples;
        }

        private static TrackBox TrackOf(Box box)
        {
            for (var parent = box?.GetParent(); parent != null; parent = (parent as Box)?.GetParent())
            {
                if (parent is TrackBox trak)
                    return trak;
            }
            return null;
        }

        /// <summary>The 'seig' entry each sample belongs to, or null where it is in none and the track's defaults hold.</summary>
        private static CencSampleEncryptionInformationGroupEntry[] GroupsOf(IList<Box> boxes, IList<Box> trackBoxes, int sampleCount, bool fragment)
        {
            var groups = new CencSampleEncryptionInformationGroupEntry[sampleCount];
            var sbgp = boxes.OfType<SampleToGroupBox>().FirstOrDefault(b => b.GroupingType == Seig);
            if (sbgp == null)
                return groups;

            var local = Entries(boxes);
            var global = Entries(trackBoxes);

            int sample = 0;
            for (int i = 0; i < sbgp.EntryCount && sample < sampleCount; i++)
            {
                uint index = sbgp.GroupDescriptionIndex[i];
                CencSampleEncryptionInformationGroupEntry entry = null;
                // in a fragment, indices above 0x10000 are of its own 'sgpd', the others of the track's (14496-12 8.9.4)
                if (fragment && index > 0x10000)
                    entry = At(local, index - 0x10001);
                else if (index > 0)
                    entry = fragment ? At(global, index - 1) : At(local, index - 1);

                for (uint k = 0; k < sbgp.SampleCount[i] && sample < sampleCount; k++)
                    groups[sample++] = entry;
            }
            return groups;
        }

        private static CencSampleEncryptionInformationGroupEntry[] Entries(IList<Box> boxes) =>
            boxes?.OfType<SampleGroupDescriptionBox>().FirstOrDefault(b => b.GroupingType == Seig)?
                ._SampleGroupDescriptionEntry?.Select(e => e as CencSampleEncryptionInformationGroupEntry).ToArray()
            ?? Array.Empty<CencSampleEncryptionInformationGroupEntry>();

        private static CencSampleEncryptionInformationGroupEntry At(CencSampleEncryptionInformationGroupEntry[] entries, uint index) =>
            index < entries.Length ? entries[index] : null;

        /// <summary>Each sample's protection as its group or track declares it, and the size of the IV each has of its own.</summary>
        private static SampleEncryption[] Defaults(TrackProtection protection, CencSampleEncryptionInformationGroupEntry[] groups, int sampleCount, out int[] ivSizes)
        {
            var samples = new SampleEncryption[sampleCount];
            ivSizes = new int[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                var group = groups[i];
                ivSizes[i] = group == null ? protection.DefaultPerSampleIVSize : group.PerSampleIVSize;
                if (group != null && group.MultiKeyFlag)
                {
                    // several keys at once (23001-7:2016/Amd 1): not one this model describes - no key ID, so it is not decrypted
                    samples[i] = new SampleEncryption { IsProtected = group.IsProtected != 0, CryptByteBlock = group.CryptByteBlock, SkipByteBlock = group.SkipByteBlock };
                    ivSizes[i] = -1;
                    continue;
                }
                samples[i] = group == null ? protection.DefaultSampleEncryption() : new SampleEncryption
                {
                    IsProtected = group.IsProtected != 0,
                    KeyId = group._KID,
                    IV = group.PerSampleIVSize == 0 ? group.ConstantIV : null,
                    CryptByteBlock = group.CryptByteBlock,
                    SkipByteBlock = group.SkipByteBlock,
                };
            }
            return samples;
        }

        /// <summary>Each sample's IV, where it has one of its own, and its subsamples, from a 'senc'.</summary>
        private static void ApplySampleEncryptionBox(SampleEncryption[] samples, SampleEncryptionSample[] entries, uint flags)
        {
            if (entries == null)
                return;

            for (int i = 0; i < samples.Length && i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry._InitializationVector != null && entry._InitializationVector.Length > 0)
                    samples[i].IV = entry._InitializationVector;
                if ((flags & 0x2) != 0 && entry.Subsamples != null)
                    samples[i].Subsamples = entry.Subsamples.Select(s => new EncryptionSubsample(s._BytesOfClearData, s._BytesOfProtectedData)).ToArray();
            }
        }

        /// <summary>
        /// Each sample's IV and subsamples from its auxiliary information: 'saiz' gives the size of each sample's, 'saio'
        /// where they are - all one after another from one offset, or from an offset for each chunk.
        /// </summary>
        private static void ApplyAuxiliaryInformation(SampleEncryption[] samples, int[] ivSizes, IList<Box> boxes, int sampleCount, Func<long, int, byte[]> read,
            Func<int, long> offsetOfEntry, (int[] ChunkOf, int[] FirstOfChunk)? chunkOfSample)
        {
            // of the Common Encryption's type: 'cenc', or no type, which is the scheme's
            // of aux_info_type_parameter 0: 1 is the multiple IVs of several keys (23001-7:2016/Amd 1, 7.1), not read here
            var saiz = boxes.OfType<SampleAuxiliaryInformationSizesBox>().FirstOrDefault(b => (b.Flags & 1) == 0 || ((b.AuxInfoType == Cenc || IsCommonEncryption(b.AuxInfoType)) && b.AuxInfoTypeParameter == 0));
            var saio = boxes.OfType<SampleAuxiliaryInformationOffsetsBox>().FirstOrDefault(b => (b.Flags & 1) == 0 || ((b.AuxInfoType == Cenc || IsCommonEncryption(b.AuxInfoType)) && b.AuxInfoTypeParameter == 0));
            if (saiz == null || saio == null || saio.EntryCount == 0 || read == null)
                return;

            int count = Math.Min(sampleCount, (int)saiz.SampleCount);
            int SizeOf(int sample) => saiz.DefaultSampleInfoSize != 0 ? saiz.DefaultSampleInfoSize : saiz.SampleInfoSize[sample];

            long offset = 0;
            for (int i = 0; i < count; i++)
            {
                if (i == 0 || (chunkOfSample != null && saio.EntryCount > 1 && chunkOfSample.Value.FirstOfChunk[chunkOfSample.Value.ChunkOf[i]] == i))
                {
                    int entry = saio.EntryCount == 1 ? 0 : chunkOfSample?.ChunkOf[i] ?? 0;
                    if (entry >= saio.Offset.Length)
                        return;
                    offset = offsetOfEntry(entry) + (long)saio.Offset[entry];
                }

                int size = SizeOf(i);
                if (size > 0 && ivSizes[i] >= 0)
                    ApplyAuxiliaryInformation(samples[i], ivSizes[i], read(offset, size));
                offset += size;
            }
        }

        private static bool IsCommonEncryption(uint type)
        {
            string scheme = IsoStream.ToFourCC(type);
            return scheme == ProtectionSchemes.Cbc1 || scheme == ProtectionSchemes.Cens || scheme == ProtectionSchemes.Cbcs;
        }

        /// <summary>One sample's auxiliary information: its IV, of the size its track or group says, then its subsamples, if it has any.</summary>
        private static void ApplyAuxiliaryInformation(SampleEncryption sample, int declaredIvSize, byte[] info)
        {
            if (info == null)
                return;

            // the IV is as long as declared; where that does not fit, what is not the subsamples is the IV: a count of
            // subsamples and 6 bytes each fill the rest exactly
            int ivSize = info.Length;
            foreach (int candidate in new[] { declaredIvSize, 16, 8, 0 })
            {
                if (info.Length == candidate)
                {
                    ivSize = candidate;
                    break;
                }
                if (info.Length >= candidate + 2)
                {
                    int subsamples = info[candidate] << 8 | info[candidate + 1];
                    if (candidate + 2 + 6 * subsamples == info.Length)
                    {
                        ivSize = candidate;
                        break;
                    }
                }
            }

            if (ivSize > 0)
                sample.IV = info.Take(ivSize).ToArray();

            if (info.Length >= ivSize + 2)
            {
                int count = info[ivSize] << 8 | info[ivSize + 1];
                var subsamples = new EncryptionSubsample[count];
                for (int k = 0, p = ivSize + 2; k < count && p + 6 <= info.Length; k++, p += 6)
                    subsamples[k] = new EncryptionSubsample(info[p] << 8 | info[p + 1], (uint)(info[p + 2] << 24 | info[p + 3] << 16 | info[p + 4] << 8 | info[p + 5]));
                sample.Subsamples = subsamples;
            }
        }
    }
}
