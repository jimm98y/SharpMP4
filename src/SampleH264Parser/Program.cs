using SharpH264;
using SharpMP4;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using (Stream inputFileStream = new FileStream("C:\\Git\\SharpMP4\\src\\FragmentedMp4Recorder\\frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read))
{
    var inputMp4 = new Mp4();
    inputMp4.Read(new IsoStream(inputFileStream));

    var tracks = inputMp4
        .Children.OfType<MovieBox>().Single()
        .Children.OfType<TrackBox>();

    var videoTrack = tracks.FirstOrDefault(x => x
        .Children.OfType<MediaBox>().Single()
        .Children.OfType<MediaInformationBox>().Single()
        .Children.OfType<VideoMediaHeaderBox>().FirstOrDefault() != null);
    uint videoTrackId = videoTrack.Children.OfType<TrackHeaderBox>().Single().TrackID;
    
    var h264VisualSample = videoTrack
        .Children.OfType<MediaBox>().Single()
        .Children.OfType<MediaInformationBox>().Single()
        .Children.OfType<SampleTableBox>().Single()
        .Children.OfType<SampleDescriptionBox>().Single()
        .Children.OfType<VisualSampleEntry>().Single();

    H264Context context = new H264Context();

    var avcC = h264VisualSample.Children.OfType<AVCConfigurationBox>().SingleOrDefault();
    if (avcC != null)
    {
        int nalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // 4 bytes

        foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
        {
            ReadH264NALU(context, spsBinary);
        }

        foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
        {
            ReadH264NALU(context, ppsBinary);
        }
    }
    else
    {
        throw new NotSupportedException();
    }

    MovieBox moov = null;
    MovieFragmentBox moof = null;
    MediaDataBox mdat = null;
    for (int i = 0; i < inputMp4.Children.Count; i++)
    {
        if (inputMp4.Children[i] is MovieBox)
        {
            moov = (MovieBox)inputMp4.Children[i];
        }
        else if (inputMp4.Children[i] is MovieFragmentBox)
        {
            moof = (MovieFragmentBox)inputMp4.Children[i];
        }
        else if (inputMp4.Children[i] is MediaDataBox)
        {
            mdat = (MediaDataBox)inputMp4.Children[i];
        }

        if(moov != null && mdat != null)
        {
            mdat.Data.Stream.SeekFromBeginning(mdat.Data.Position);

            if (moof != null)
            {
                IOrderedEnumerable<TrackRunBox> plan = moof.Children.OfType<TrackFragmentBox>().SelectMany(x => x.Children.OfType<TrackRunBox>()).OrderBy(x => x.DataOffset);

                foreach (var trun in plan)
                {
                    uint trackId = (trun.GetParent() as TrackFragmentBox).Children.OfType<TrackFragmentHeaderBox>().First().TrackID;
                    bool isVideo = trackId == videoTrackId;
                    if (Log.DebugEnabled) Log.Debug($"--TRUN: {(isVideo ? "video" : "audio")}");
                    ulong size = 0;
                    for (int j = 0; j < trun._TrunEntry.Length; j++)
                    {
                        var entry = trun._TrunEntry[j];
                        int sampleSize = (int)entry.SampleSize;

                        if (isVideo)
                        {
                            uint nalUnitLength = 0;
                            long offset = 0;

                            Debug.WriteLine("AU begin");

                            do
                            {
                                size += mdat.Data.Stream.ReadUInt32(size, (ulong)mdat.Data.Length, out nalUnitLength);
                                offset += 4;
                                size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, nalUnitLength, out byte[] sampleData);
                                offset += sampleData.Length;
                                ReadH264NALU(context, sampleData);
                            } while (offset < sampleSize);

                            Debug.WriteLine("AU end");
                        }
                        else
                        {
                            // audio
                            size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, (uint)sampleSize, out byte[] sampleData);
                        }
                    }
                }
            }
            else
            {
                SampleTableBox sample_table_box = moov
                    .Children.OfType<TrackBox>().First(x => x.Children.OfType<TrackHeaderBox>().Single().TrackID == videoTrackId)
                    .Children.OfType<MediaBox>().Single()
                    .Children.OfType<MediaInformationBox>().Single()
                    .Children.OfType<SampleTableBox>().Single();
                ChunkOffsetBox chunk_offsets_box = sample_table_box.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                ChunkLargeOffsetBox chunk_offsets_large_box = sample_table_box.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                SampleToChunkBox sample_to_chunks = sample_table_box.Children.OfType<SampleToChunkBox>().Single();
                SampleSizeBox sample_size_box = sample_table_box.Children.OfType<SampleSizeBox>().Single();
                uint[] sampleSizes = sample_size_box.SampleSize > 0 ? Enumerable.Repeat(sample_size_box.SampleSize, (int)sample_size_box.SampleCount).ToArray() : sample_size_box.EntrySize;
                ulong[] chunkOffsets = chunk_offsets_box != null ? chunk_offsets_box.ChunkOffset.Select(x => (ulong)x).ToArray() : chunk_offsets_large_box.ChunkOffset;

                // https://developer.apple.com/documentation/quicktime-file-format/sample-to-chunk_atom/sample-to-chunk_table
                int s2c_index = 0;
                uint next_run = 0;
                int sample_idx = 0;
                uint samples_per_chunk = 0;
                for (int k = 1; k < chunkOffsets.Length; k++)
                {
                    if (k >= next_run)
                    {
                        samples_per_chunk = sample_to_chunks.SamplesPerChunk[s2c_index];
                        s2c_index += 1;
                        next_run = (s2c_index < sample_to_chunks.FirstChunk.Length) ? sample_to_chunks.FirstChunk[s2c_index] : (uint)(chunkOffsets.Length + 1);
                    }
                    
                    ulong size = 0;
                    long chunkOffset = (long)chunkOffsets[k - 1];

                    // seek to the chunk offset
                    mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                    // read samples in this chunk
                    for (int l = 0; l < samples_per_chunk; l++)
                    {
                        uint sampleSize = sampleSizes[sample_idx++];

                        Debug.WriteLine("AU begin");

                        long offset = 0;
                        do
                        {
                            uint nalUnitLength = 0;
                            size += mdat.Data.Stream.ReadUInt32(size, (ulong)mdat.Data.Length, out nalUnitLength);
                            offset += 4;
                            size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, nalUnitLength, out byte[] sampleData);
                            offset += sampleData.Length;
                            ReadH264NALU(context, sampleData);
                        } while (offset < sampleSize);

                        Debug.WriteLine("AU end");
                    }
                }
            }

            mdat = null;
        }
    }
}

static void ReadH264NALU(H264Context context, byte[] sampleData)
{
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        ulong ituSize = 0;
        NalUnit nu = new NalUnit((uint)sampleData.Length);
        ituSize += nu.Read(context, stream);
        context.NalHeader = nu;

        var ms = new MemoryStream();
        using (ItuStream wstream = new ItuStream(ms))
        {
            nu.Write(context, wstream);

            if(nu.NalUnitType == H264NALTypes.UNSPECIFIED0) // 0
            {
                Debug.WriteLine($"NALU: Unspecified {nu.NalUnitType}");
                throw new InvalidOperationException();
            }
            else if (nu.NalUnitType == H264NALTypes.SLICE) // 1
            {
                Debug.WriteLine("NALU: 1, slice of non-IDR picture");
                context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.DPA) // 2
            {
                Debug.WriteLine("NALU: 2, coded slice data partition A");
                context.SliceDataPartitionaLayerRbsp = new SliceDataPartitionaLayerRbsp();
                context.SliceDataPartitionaLayerRbsp.Read(context, stream);
                context.SliceDataPartitionaLayerRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.DPB) // 3
            {
                Debug.WriteLine("NALU: 3, coded slice data partition B");
                context.SliceDataPartitionbLayerRbsp = new SliceDataPartitionbLayerRbsp();
                context.SliceDataPartitionbLayerRbsp.Read(context, stream);
                context.SliceDataPartitionbLayerRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.DPB) // 4
            {
                Debug.WriteLine("NALU: 4, coded slice data partition C");
                context.SliceDataPartitioncLayerRbsp = new SliceDataPartitioncLayerRbsp();
                context.SliceDataPartitioncLayerRbsp.Read(context, stream);
                context.SliceDataPartitioncLayerRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.IDR_SLICE) // 5
            {
                Debug.WriteLine("NALU: 5, slice of non-IDR picture");
                context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.SEI) // 6
            {
                Debug.WriteLine("NALU: 6, SEI");
                context.SeiRbsp = new SeiRbsp();
                context.SeiRbsp.Read(context, stream);
                context.SeiRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.SPS) // 7
            {
                Debug.WriteLine("NALU: 7, SPS");
                context.SeqParameterSetRbsp = new SeqParameterSetRbsp();
                context.SeqParameterSetRbsp.Read(context, stream);
                context.SeqParameterSetRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.PPS) // 8
            {
                Debug.WriteLine("NALU: 8, PPS");
                context.PicParameterSetRbsp = new PicParameterSetRbsp();
                context.PicParameterSetRbsp.Read(context, stream);
                context.PicParameterSetRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.AUD) // 9
            {
                Debug.WriteLine($"NALU: 9, AU Delimiter");
                context.AccessUnitDelimiterRbsp = new AccessUnitDelimiterRbsp();
                context.AccessUnitDelimiterRbsp.Read(context, stream);
                context.AccessUnitDelimiterRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.END_OF_SEQUENCE) // 10
            {
                Debug.WriteLine($"NALU: 10, End of Sequence");
                context.EndOfSeqRbsp = new EndOfSeqRbsp();
                context.EndOfSeqRbsp.Read(context, stream);
                context.EndOfSeqRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.END_OF_STREAM) // 11
            {
                Debug.WriteLine($"NALU: 11, End of Stream");
                context.EndOfStreamRbsp = new EndOfStreamRbsp();
                context.EndOfStreamRbsp.Read(context, stream);
                context.EndOfStreamRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.FILLER_DATA) // 12
            {
                Debug.WriteLine($"NALU: 12, Filler Data");
                context.FillerDataRbsp = new FillerDataRbsp();
                context.FillerDataRbsp.Read(context, stream);
                context.FillerDataRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.SPS_EXT) // 13
            {
                Debug.WriteLine($"NALU: 13, SPS Extension");
                context.SeqParameterSetExtensionRbsp = new SeqParameterSetExtensionRbsp();
                context.SeqParameterSetExtensionRbsp.Read(context, stream);
                context.SeqParameterSetExtensionRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.PREFIX_NAL) // 14
            {
                Debug.WriteLine($"NALU: 14, Prefix NAL");
                context.PrefixNalUnitRbsp = new PrefixNalUnitRbsp();
                context.PrefixNalUnitRbsp.Read(context, stream);
                context.PrefixNalUnitRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.SUBSET_SPS) // 15
            {
                Debug.WriteLine($"NALU: 15, Subset SPS");
                context.SubsetSeqParameterSetRbsp = new SubsetSeqParameterSetRbsp();
                context.SubsetSeqParameterSetRbsp.Read(context, stream);
                context.SubsetSeqParameterSetRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.DPS) // 16
            {
                Debug.WriteLine($"NALU: 16, DPS");
                context.DepthParameterSetRbsp = new DepthParameterSetRbsp();
                context.DepthParameterSetRbsp.Read(context, stream);
                context.DepthParameterSetRbsp.Write(context, wstream);
                if (!ms.ToArray().SequenceEqual(sampleData))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if(nu.NalUnitType == H264NALTypes.RESERVED0) // 17
            {
                Debug.WriteLine($"NALU: Reserved {nu.NalUnitType}");
                throw new InvalidOperationException(); 
            }
            else if (nu.NalUnitType == H264NALTypes.RESERVED1) // 18
            {
                Debug.WriteLine($"NALU: Reserved {nu.NalUnitType}");
                throw new InvalidOperationException();
            }
            else if (nu.NalUnitType == H264NALTypes.SLICE_NOPARTITIONING) // 19
            {
                Debug.WriteLine($"NALU: 19, Auxiliary coded picture without partitioning");
                context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.SLICE_EXT) // 20
            {
                Debug.WriteLine($"NALU: Slice Layer Extension");
                context.SliceLayerExtensionRbsp = new SliceLayerExtensionRbsp();
                context.SliceLayerExtensionRbsp.Read(context, stream);
                context.SliceLayerExtensionRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.SLICE_EXT_VIEW_COMPONENT) // 21
            {
                Debug.WriteLine($"NALU: Slice Extension for Depth View or a 3D AVC texture view component");
                context.SliceLayerExtensionRbsp = new SliceLayerExtensionRbsp();
                context.SliceLayerExtensionRbsp.Read(context, stream);
                context.SliceLayerExtensionRbsp.Write(context, wstream);
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                    throw new Exception($"Failed to write NALu {nu.NalUnitType}");
            }
            else if (nu.NalUnitType == H264NALTypes.RESERVED2) // 22
            {
                Debug.WriteLine($"NALU: Reserved {nu.NalUnitType}");
                throw new InvalidOperationException();
            }
            else if (nu.NalUnitType == H264NALTypes.RESERVED3) // 23
            {
                Debug.WriteLine($"NALU: Reserved {nu.NalUnitType}");
                throw new InvalidOperationException();
            }
            else
            {
                Debug.WriteLine($"NALU: Unspecified {nu.NalUnitType}");
                throw new InvalidOperationException();
            }
        }
    }
}