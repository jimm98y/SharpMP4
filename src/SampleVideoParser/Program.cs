using SharpH264;
using SharpH265;
using SharpH26X;
using SharpMP4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// .\ffmpeg.exe - i "frag_bunny.mp4" - c:v copy -bsf:v trace_headers -f null -  > log.txt 2>&1

//Log.SinkDebug = (o, e) => { Console.WriteLine(o); };
Log.SinkInfo = (o, e) => { Console.WriteLine(o); };
Log.SinkError = (o, e) => { 
    Debug.WriteLine(o);
    try
    {
        File.AppendAllText("C:\\Temp\\_decoding_errors.txt", o + "\r\n");
    }
    catch
    {

    }
};

//var files = File.ReadAllLines("C:\\Temp\\_h265.txt");
var files = new string[] { "\\\\192.168.1.250\\photo2\\Santiago3\\0_IMG_1060.HEIC" };

foreach (var file in files)
{
    Log.Info($"----Reading: {file}");

    try
    {
        using (Stream inputFileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var inputMp4 = new Mp4();
            try
            {
                inputMp4.Read(new IsoStream(inputFileStream));
            }
            catch (SharpMP4.IsoEndOfStreamException)
            {
                Log.Error($"Invalid file: {file}");
                continue;
            }

            int nalLengthSize = 4;
            IItuContext context = null;
            VideoFormat format = VideoFormat.Unknown;
            uint videoTrackId = 0;

            HEVCConfigurationBox hvcC = null;
            AVCConfigurationBox avcC = null;

            var movieBox = inputMp4
                .Children.OfType<MovieBox>().SingleOrDefault();

            if (movieBox == null)
            {
                // HEIC
                var metaBox = inputMp4.Children.OfType<MetaBox>().SingleOrDefault();
                var iprpBox = metaBox.Children.OfType<ItemPropertiesBox>().SingleOrDefault();
                var ipcoBox = iprpBox.Children.OfType<ItemPropertyContainerBox>().SingleOrDefault();

                hvcC = ipcoBox.Children.OfType<HEVCConfigurationBox>().SingleOrDefault();
            }
            else
            {
                var tracks = movieBox
                    .Children.OfType<TrackBox>();

                var videoTrack = tracks.FirstOrDefault(x => x
                    .Children.OfType<MediaBox>().Single()
                    .Children.OfType<MediaInformationBox>().Single()
                    .Children.OfType<VideoMediaHeaderBox>().FirstOrDefault() != null);
                videoTrackId = videoTrack.Children.OfType<TrackHeaderBox>().Single().TrackID;

                var visualSample = videoTrack
                    .Children.OfType<MediaBox>().Single()
                    .Children.OfType<MediaInformationBox>().Single()
                    .Children.OfType<SampleTableBox>().Single()
                    .Children.OfType<SampleDescriptionBox>().Single()
                    .Children.OfType<VisualSampleEntry>().Single();

                avcC = visualSample.Children.OfType<AVCConfigurationBox>().SingleOrDefault();
                hvcC = visualSample.Children.OfType<HEVCConfigurationBox>().SingleOrDefault();
            }

            if (avcC != null)
            {
                context = new H264Context();
                format = VideoFormat.H264;

                nalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
                {
                    ParseNALU(context, format, spsBinary);
                }

                foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
                {
                    ParseNALU(context, format, ppsBinary);
                }
            }
            else if (hvcC != null)
            {
                context = new H265Context();
                format = VideoFormat.H265;

                nalLengthSize = hvcC._HEVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                foreach (var nalus in hvcC._HEVCConfig.NalUnit)
                {
                    foreach (var nalu in nalus)
                    {
                        ParseNALU(context, format, nalu);
                    }
                }
            }
            else
            {
                //throw new NotSupportedException();
                Log.Error($"{file}");
                continue;
            }

            MovieBox moov = null;
            MovieFragmentBox moof = null;
            MediaDataBox mdat = null;
                
            MetaBox meta = null;
            ulong size = 0;
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
                else if (inputMp4.Children[i] is MetaBox)
                {
                    meta = (MetaBox)inputMp4.Children[i];
                }
                
                if (moov != null && mdat != null)
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

                            for (int j = 0; j < trun._TrunEntry.Length; j++)
                            {
                                var entry = trun._TrunEntry[j];
                                uint sampleSize = entry.SampleSize;

                                if (isVideo)
                                {
                                    try
                                    {
                                        size += ReadAU(nalLengthSize, context, format, mdat, sampleSize);
                                    }
                                    catch (Exception)
                                    {
                                        Log.Error($"---Error reading {file}");
                                    }
                                }
                                else
                                {
                                    // audio
                                    size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, sampleSize, out byte[] sampleData);
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
                        for (int k = 1; k <= chunkOffsets.Length; k++)
                        {
                            if (k >= next_run)
                            {
                                samples_per_chunk = sample_to_chunks.SamplesPerChunk[s2c_index];
                                s2c_index += 1;
                                next_run = (s2c_index < sample_to_chunks.FirstChunk.Length) ? sample_to_chunks.FirstChunk[s2c_index] : (uint)(chunkOffsets.Length + 1);
                            }

                            long chunkOffset = (long)chunkOffsets[k - 1];

                            // seek to the chunk offset
                            mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                            // read samples in this chunk
                            for (int l = 0; l < samples_per_chunk; l++)
                            {
                                uint sampleSize = sampleSizes[sample_idx++];

                                try
                                {
                                    size += ReadAU(nalLengthSize, context, format, mdat, sampleSize);
                                }
                                catch (Exception)
                                {
                                    Log.Error($"---Error reading {file}");
                                }
                            }
                        }
                    }

                    mdat = null;
                }
                else if(meta != null)
                {
                    // HEIC
                    var primaryItem = meta.Children.OfType<PrimaryItemBox>().SingleOrDefault();
                    uint item_id = primaryItem.ItemID;

                    var ilocBox = meta.Children.OfType<ItemLocationBox>().SingleOrDefault();
                    for (int l = 0; l < ilocBox.BaseOffset.Length; l++)
                    {
                        int baseOffset = ilocBox.BaseOffset[l].Length == 0 ? 0 : ReadVariableInt(ilocBox.BaseOffsetSize, ilocBox.BaseOffset[l]);

                        for (int j = 0; j < ilocBox.ExtentCount[l]; j++)
                        {
                            long position = baseOffset + ReadVariableInt(ilocBox.OffsetSize, ilocBox.ExtentOffset[l][j]);
                            uint length = (uint)ReadVariableInt(ilocBox.LengthSize, ilocBox.ExtentLength[l][j]);
                            mdat.Data.Stream.SeekFromBeginning(position);

                            try
                            {
                                size += ReadAU(nalLengthSize, context, format, mdat, length);
                            }
                            catch (Exception)
                            {
                                Log.Error($"---Error reading {file}");
                            }
                        }
                    }

                }
            }
        }
    }
    catch(Exception ex)
    {

    }
}

int ReadVariableInt(byte size, byte[] bytes)
{
    switch(size)
    {
        case 1:
            return bytes[0];

        case 2:
            return (bytes[0] << 8) + bytes[1];

        case 3:
            return (bytes[0] << 16) + (bytes[1] << 8) + bytes[2];

        case 4:
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];

        default:
            throw new NotSupportedException($"Size {size} not supported");
    }
}

static ulong ReadAU(int nalLengthSize, IItuContext context, VideoFormat format, MediaDataBox mdat, uint sampleSizeInBytes)
{
    ulong size = 0;
    long offsetInBytes = 0;

    Log.Debug($"AU begin {sampleSizeInBytes}");

    do
    {
        uint nalUnitLength = 0;
        switch (nalLengthSize)
        {
            case 1:
                size += mdat.Data.Stream.ReadUInt8(size, (ulong)mdat.Data.Length, out nalUnitLength);
                break;
            case 2:
                size += mdat.Data.Stream.ReadUInt16(size, (ulong)mdat.Data.Length, out nalUnitLength);
                break;
            case 3:
                size += mdat.Data.Stream.ReadUInt24(size, (ulong)mdat.Data.Length, out nalUnitLength);
                break;
            case 4:
                size += mdat.Data.Stream.ReadUInt32(size, (ulong)mdat.Data.Length, out nalUnitLength);
                break;

            default:
                throw new Exception($"NAL unit length {nalLengthSize} not supported!");
        }
        offsetInBytes += nalLengthSize;

        if (nalUnitLength > (sampleSizeInBytes - offsetInBytes))
        {
            Log.Error($"Invalid NALU size: {nalUnitLength}");
            nalUnitLength = (uint)(sampleSizeInBytes - offsetInBytes);
            size += nalUnitLength;
            offsetInBytes += nalUnitLength;
            break;
        }

        size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, nalUnitLength, out byte[] sampleData);
        offsetInBytes += sampleData.Length;
        ParseNALU(context, format, sampleData);
    } while (offsetInBytes < sampleSizeInBytes);

    if (offsetInBytes != sampleSizeInBytes)
        throw new Exception("Mismatch!");

    Log.Debug("AU end");

    return size;
}

static void ParseNALU(IItuContext ctx, VideoFormat format, byte[] sampleData)
{
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        if(format == VideoFormat.H264)
        {
            ParseH264NALU((H264Context)ctx, sampleData);
        }
        else if(format == VideoFormat.H265)
        {
            ParseH265NALU((H265Context)ctx, sampleData);
        }
        else
        {
            throw new NotSupportedException();
        }
    }
}

static void ParseH264NALU(H264Context context, byte[] sampleData)
{
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        ulong ituSize = 0;
        var nu = new SharpH264.NalUnit((uint)sampleData.Length);
        context.NalHeader = nu;
        ituSize += nu.Read(context, stream);

        var ms = new MemoryStream();
        using (ItuStream wstream = new ItuStream(ms))
        {
            nu.Write(context, wstream);

            try
            {
                if (nu.NalUnitType == H264NALTypes.UNSPECIFIED0) // 0
                {
                    Log.Debug($"NALU: 0, Unspecified {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE) // 1
                {
                    Log.Debug($"NALU: 1, slice of non-IDR picture, {sampleData.Length} bytes");
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPA) // 2
                {
                    Log.Debug($"NALU: 2, coded slice data partition A, {sampleData.Length} bytes");
                    context.SliceDataPartitionaLayerRbsp = new SliceDataPartitionaLayerRbsp();
                    context.SliceDataPartitionaLayerRbsp.Read(context, stream);
                    context.SliceDataPartitionaLayerRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPB) // 3
                {
                    Log.Debug($"NALU: 3, coded slice data partition B, {sampleData.Length} bytes");
                    context.SliceDataPartitionbLayerRbsp = new SliceDataPartitionbLayerRbsp();
                    context.SliceDataPartitionbLayerRbsp.Read(context, stream);
                    context.SliceDataPartitionbLayerRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPB) // 4
                {
                    Log.Debug($"NALU: 4, coded slice data partition C, {sampleData.Length} bytes");
                    context.SliceDataPartitioncLayerRbsp = new SliceDataPartitioncLayerRbsp();
                    context.SliceDataPartitioncLayerRbsp.Read(context, stream);
                    context.SliceDataPartitioncLayerRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.IDR_SLICE) // 5
                {
                    Log.Debug($"NALU: 5, slice of non-IDR picture, {sampleData.Length} bytes");
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SEI) // 6
                {
                    Log.Debug($"NALU: 6, SEI, {sampleData.Length} bytes");
                    context.SeiRbsp = new SharpH264.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SPS) // 7
                {
                    Log.Debug($"NALU: 7, SPS, {sampleData.Length} bytes");
                    context.SeqParameterSetRbsp = new SharpH264.SeqParameterSetRbsp();
                    context.SeqParameterSetRbsp.Read(context, stream);
                    context.SeqParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.PPS) // 8
                {
                    Log.Debug($"NALU: 8, PPS, {sampleData.Length} bytes");
                    context.PicParameterSetRbsp = new SharpH264.PicParameterSetRbsp();
                    context.PicParameterSetRbsp.Read(context, stream);
                    context.PicParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.AUD) // 9
                {
                    Log.Debug($"NALU: 9, AU Delimiter, {sampleData.Length} bytes");
                    context.AccessUnitDelimiterRbsp = new SharpH264.AccessUnitDelimiterRbsp();
                    context.AccessUnitDelimiterRbsp.Read(context, stream);
                    context.AccessUnitDelimiterRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.END_OF_SEQUENCE) // 10
                {
                    Log.Debug($"NALU: 10, End of Sequence, {sampleData.Length} bytes");
                    context.EndOfSeqRbsp = new SharpH264.EndOfSeqRbsp();
                    context.EndOfSeqRbsp.Read(context, stream);
                    context.EndOfSeqRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.END_OF_STREAM) // 11
                {
                    Log.Debug($"NALU: 11, End of Stream, {sampleData.Length} bytes");
                    context.EndOfStreamRbsp = new EndOfStreamRbsp();
                    context.EndOfStreamRbsp.Read(context, stream);
                    context.EndOfStreamRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.FILLER_DATA) // 12
                {
                    Log.Debug($"NALU: 12, Filler Data, {sampleData.Length} bytes");
                    context.FillerDataRbsp = new SharpH264.FillerDataRbsp();
                    context.FillerDataRbsp.Read(context, stream);
                    context.FillerDataRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SPS_EXT) // 13
                {
                    Log.Debug($"NALU: 13, SPS Extension, {sampleData.Length} bytes");
                    context.SeqParameterSetExtensionRbsp = new SeqParameterSetExtensionRbsp();
                    context.SeqParameterSetExtensionRbsp.Read(context, stream);
                    context.SeqParameterSetExtensionRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.PREFIX_NAL) // 14
                {
                    Log.Debug($"NALU: 14, Prefix NAL, {sampleData.Length} bytes");
                    context.PrefixNalUnitRbsp = new PrefixNalUnitRbsp();
                    context.PrefixNalUnitRbsp.Read(context, stream);
                    context.PrefixNalUnitRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SUBSET_SPS) // 15
                {
                    Log.Debug($"NALU: 15, Subset SPS, {sampleData.Length} bytes");
                    context.SubsetSeqParameterSetRbsp = new SubsetSeqParameterSetRbsp();
                    context.SubsetSeqParameterSetRbsp.Read(context, stream);
                    context.SubsetSeqParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPS) // 16
                {
                    Log.Debug($"NALU: 16, DPS, {sampleData.Length} bytes");
                    context.DepthParameterSetRbsp = new DepthParameterSetRbsp();
                    context.DepthParameterSetRbsp.Read(context, stream);
                    context.DepthParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED0) // 17
                {
                    Log.Debug($"NALU: 17, Reserved {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED1) // 18
                {
                    Log.Debug($"NALU: 18, Reserved {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE_NOPARTITIONING) // 19
                {
                    Log.Debug($"NALU: 19, Auxiliary coded picture without partitioning, {sampleData.Length} bytes");
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE_EXT) // 20
                {
                    Log.Debug($"NALU: 20, Slice Layer Extension, {sampleData.Length} bytes");
                    context.SliceLayerExtensionRbsp = new SliceLayerExtensionRbsp();
                    context.SliceLayerExtensionRbsp.Read(context, stream);
                    context.SliceLayerExtensionRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE_EXT_VIEW_COMPONENT) // 21
                {
                    Log.Debug($"NALU: 21, Slice Extension for Depth View or a 3D AVC texture view component, {sampleData.Length} bytes");
                    context.SliceLayerExtensionRbsp = new SliceLayerExtensionRbsp();
                    context.SliceLayerExtensionRbsp.Read(context, stream);
                    context.SliceLayerExtensionRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED2) // 22
                {
                    Log.Debug($"NALU: 22, Reserved, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED3) // 23
                {
                    Log.Debug($"NALU: 23, Reserved, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else
                {
                    Log.Debug($"NALU: Unspecified {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex.Message}");
                Log.Error($"SampleData: {Convert.ToHexString(sampleData)}");
                Log.Error($"WriteData: {Convert.ToHexString(ms.ToArray())}");
                throw;
            }
        }
    }
}

static void ParseH265NALU(H265Context context, byte[] sampleData)
{
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        ulong ituSize = 0;
        var nu = new SharpH265.NalUnit((uint)sampleData.Length);
        context.NalHeader = nu;
        ituSize += nu.Read(context, stream);

        var ms = new MemoryStream();
        using (ItuStream wstream = new ItuStream(ms))
        {
            nu.Write(context, wstream);

            try
            {
                if (nu.NalUnitHeader.NalUnitType >= H265NALTypes.UNSPEC48) // unspecified
                {
                    Log.Debug($"NALU: 0, Unspecified {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.SPS_NUT) // 33
                {
                    Log.Debug($"NALU: 33, SPS, {sampleData.Length} bytes");
                    context.SeqParameterSetRbsp = new SharpH265.SeqParameterSetRbsp();
                    context.SeqParameterSetRbsp.Read(context, stream);
                    context.SeqParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PPS_NUT) // 34
                {
                    Log.Debug($"NALU: 34, PPS, {sampleData.Length} bytes");
                    context.PicParameterSetRbsp = new SharpH265.PicParameterSetRbsp();
                    context.PicParameterSetRbsp.Read(context, stream);
                    context.PicParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.VPS_NUT) // 32
                {
                    Log.Debug($"NALU: 32, VPS, {sampleData.Length} bytes");
                    context.VideoParameterSetRbsp = new SharpH265.VideoParameterSetRbsp();
                    context.VideoParameterSetRbsp.Read(context, stream);
                    context.VideoParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.AUD_NUT) // 35
                {
                    Log.Debug($"NALU: 35, AUD, {sampleData.Length} bytes");
                    context.AccessUnitDelimiterRbsp = new SharpH265.AccessUnitDelimiterRbsp();
                    context.AccessUnitDelimiterRbsp.Read(context, stream);
                    context.AccessUnitDelimiterRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.EOS_NUT) // 36
                {
                    Log.Debug($"NALU: 36, EOS, {sampleData.Length} bytes");
                    context.EndOfSeqRbsp = new SharpH265.EndOfSeqRbsp();
                    context.EndOfSeqRbsp.Read(context, stream);
                    context.EndOfSeqRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.EOB_NUT) // 37
                {
                    Log.Debug($"NALU: 37, EOB, {sampleData.Length} bytes");
                    context.EndOfBitstreamRbsp = new SharpH265.EndOfBitstreamRbsp();
                    context.EndOfBitstreamRbsp.Read(context, stream);
                    context.EndOfBitstreamRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.FD_NUT) // 38
                {
                    Log.Debug($"NALU: 38, FillerData, {sampleData.Length} bytes");
                    context.FillerDataRbsp = new SharpH265.FillerDataRbsp();
                    context.FillerDataRbsp.Read(context, stream);
                    context.FillerDataRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PREFIX_SEI_NUT) // 39
                {
                    Log.Debug($"NALU: 39, Prefix SEI, {sampleData.Length} bytes");
                    context.SeiRbsp = new SharpH265.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.SUFFIX_SEI_NUT) // 40
                {
                    Log.Debug($"NALU: 40, Suffix SEI, {sampleData.Length} bytes");
                    var oldSei = context.SeiRbsp;
                    context.SeiRbsp = new SharpH265.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.CRA_NUT ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.IDR_N_LP ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.IDR_W_RADL ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.BLA_N_LP ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_LP ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_RADL ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RASL_N ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RASL_R ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RADL_N ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RADL_R ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.STSA_N ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.STSA_R ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TSA_N ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TSA_R ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TRAIL_N ||
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TRAIL_R
                    ) 
                {
                    Log.Debug($"NALU: {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                }
                else
                {
                    // 10-15, 22-31, 41-47
                    Log.Debug($"NALU: Reserved {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error: {ex.Message}");
                Log.Error($"SampleData: {Convert.ToHexString(sampleData)}");
                Log.Error($"WriteData:  {Convert.ToHexString(ms.ToArray())}");
                throw;
            }
        }
    }
}

static string[] DirSearch(string sDir)
{
    List<string> files = new List<string>();
    try
    {
        foreach (string d in Directory.GetDirectories(sDir))
        {
            foreach (string f in Directory.GetFiles(d))
            {
                files.Add(f);
            }
            foreach (var ff in DirSearch(d))
            {
                files.Add(ff);
            }
        }

        foreach (string f in Directory.GetFiles(sDir))
        {
            files.Add(f);
        }
    }
    catch (System.Exception excpt)
    {
        Console.WriteLine(excpt.Message);
    }

    return files.ToArray();
}

public enum VideoFormat
{
    Unknown,
    H264,
    H265,
}
