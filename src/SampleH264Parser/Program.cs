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

    H264Context readContext = new H264Context();
    H264Context writeContext = new H264Context();

    var avcC = h264VisualSample.Children.OfType<AVCConfigurationBox>().SingleOrDefault();
    if (avcC != null)
    {
        int nalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // 4 bytes

        foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
        {
            ReadNALu(readContext, writeContext, spsBinary);
        }

        foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
        {
            ReadNALu(readContext, writeContext, ppsBinary);
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
                                ReadNALu(readContext, writeContext, sampleData);
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
                            ReadNALu(readContext, writeContext, sampleData);
                        } while (offset < sampleSize);

                        Debug.WriteLine("AU end");
                    }
                }
            }

            mdat = null;
        }
    }
}

static void ReadNALu(H264Context readContext, H264Context writeContext, byte[] sampleData)
{
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        ulong ituSize = 0;
        NalUnit nu = new NalUnit((uint)sampleData.Length);
        ituSize += nu.Read(readContext, stream);
        readContext.NalHeader = nu;

        if(nu.NalUnitType == 7)
        {
            SeqParameterSetRbsp sps = new SeqParameterSetRbsp();
            readContext.Sps = sps;

            sps.Read(readContext, stream);

            var ms = new MemoryStream();
            using (ItuStream wstream = new ItuStream(ms))
            {
                nu.Write(readContext, wstream);
                sps.Write(readContext, wstream);

                byte[] wbytes = ms.ToArray();
                if (!wbytes.SequenceEqual(sampleData))
                {
                    throw new Exception("Failed to write SPS");
                }
            }
        }
        else if(nu.NalUnitType == 8)
        {
            PicParameterSetRbsp pps = new PicParameterSetRbsp();
            readContext.Pps = pps;

            pps.Read(readContext, stream);

            var ms = new MemoryStream();
            using (ItuStream wstream = new ItuStream(ms))
            {
                nu.Write(readContext, wstream);
                pps.Write(readContext, wstream);

                byte[] wbytes = ms.ToArray();
                if (!wbytes.SequenceEqual(sampleData))
                {
                    throw new Exception("Failed to write PPS");
                }
            }
        }
        else if (nu.NalUnitType == 6)
        {
            Debug.WriteLine("NALU: SEI");

            SeiRbsp sei = new SeiRbsp();
            readContext.Sei = sei;
            ituSize += sei.Read(readContext, stream);

            var ms = new MemoryStream();
            using (ItuStream wstream = new ItuStream(ms))
            {
                nu.Write(readContext, wstream);
                sei.Write(readContext, wstream);

                byte[] wbytes = ms.ToArray();
                if (!wbytes.SequenceEqual(sampleData))
                {
                    Debug.WriteLine(Convert.ToHexString(wbytes));
                    Debug.WriteLine(Convert.ToHexString(sampleData));
                    throw new Exception("Failed to write SEI");
                }
            }
        }
        else if (nu.NalUnitType == 9)
        {
            Debug.WriteLine($"NALU: AU Delimiter");
        }
        else if(nu.NalUnitType == 1 || nu.NalUnitType == 5)
        {
            Debug.WriteLine($"NALU: {nu.NalUnitType}");

            SliceLayerWithoutPartitioningRbsp slice = new SliceLayerWithoutPartitioningRbsp();
            readContext.Slice = slice;

            slice.Read(readContext, stream);

            var ms = new MemoryStream();
            using (ItuStream wstream = new ItuStream(ms))
            {
                nu.Write(readContext, wstream);
                slice.Write(readContext, wstream);

                byte[] wbytes = ms.ToArray();
                if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(wbytes)))
                {
                    throw new Exception("Failed to write Slice");
                }
            }
        }
        else
        {
            Debug.WriteLine($"NALU: {nu.NalUnitType}");
        }
    }
}