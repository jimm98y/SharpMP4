using SharpAV1;
using SharpH264;
using SharpH265;
using SharpH266;
using SharpH26X;
using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// .\ffmpeg.exe - i "frag_bunny.mp4" - c:v copy -bsf:v trace_headers -f null -  > log.txt 2>&1

var logger = new FileLogger("C:\\Temp\\video_debug2.txt");

//Log.SinkDebug = (o, e) => { Console.WriteLine(o); };
SharpISOBMFF.Log.SinkInfo = (o, e) => 
{ 
    Console.WriteLine($"{DateTime.UtcNow} {o}");
    File.AppendAllText("C:\\Temp\\_decoding_progress2.txt", o + "\r\n");
};
SharpISOBMFF.Log.SinkError = (o, e) => 
{ 
    Debug.WriteLine(o);
    File.AppendAllText("C:\\Temp\\_decoding_errors2.txt", o + "\r\n");
};

SharpISOBMFF.Log.SinkDebug = (o, e) =>
{
    Debug.WriteLine(o);
    //logger.Log(o + "\r\n");
};
SharpH26X.Log.SinkInfo = (o, e) =>
{
    Debug.WriteLine(o);
    //logger.Log(o + "\r\n");
};

//var files = File.ReadAllLines("C:\\Temp\\testFiles.txt");
//var files = File.ReadAllLines("C:\\Temp\\_h265.txt");
//var files = new string[] { "C:\\Git\\heif_howto\\test_images\\nokia\\winter_1440x960.heic" };
//var files = new string[] { "\\\\192.168.1.250\\photo2\\Santiago3\\0_IMG_1060.HEIC" };
//var files = new string[] { "C:\\Users\\lukasvolf\\Downloads\\NovosobornayaSquare_3840x2160.mp4" };
//var files = new string[] { "C:\\Git\\SharpMP4\\src\\FragmentedMp4Recorder\\frag_bunny.mp4" };
var files = new string[] { "C:\\Temp\\002.mp4" };

foreach (var file in files)
{
    SharpISOBMFF.Log.Info(file);

    try
    {
        using (Stream inputFileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var inputMp4 = new Container();
            try
            {
                inputMp4.Read(new IsoStream(inputFileStream));
            }
            catch (IsoEndOfStreamException)
            {
                SharpISOBMFF.Log.Error($"Invalid file: {file}");
                continue;
            }

            int nalLengthSize = 4;
            object context = null;
            VideoFormat format = VideoFormat.Unknown;
            uint videoTrackId = 0;
            uint primaryItemID = 0;

            long video_dts = 0;
            long video_pts = 0;

            long audio_dts = 0;
            long audio_pts = 0;

            AVCConfigurationBox avcC = null;
            HEVCConfigurationBox hvcC = null;
            VvcConfigurationBox vvcC = null;
            AV1CodecConfigurationBox av1C = null;

            if (inputMp4.Children.Count == 0)
                continue;

            var ftypBox = inputMp4
                .Children.OfType<FileTypeBox>().SingleOrDefault();
            var movieBox = inputMp4
                .Children.OfType<MovieBox>().SingleOrDefault();

            if (ftypBox.CompatibleBrands.Contains(IsoStream.FromFourCC("heic")))
            {
                // HEIC
                var metaBox = inputMp4.Children.OfType<MetaBox>().SingleOrDefault();
                var iprpBox = metaBox.Children.OfType<ItemPropertiesBox>().SingleOrDefault();
                var ipcoBox = iprpBox.Children.OfType<ItemPropertyContainerBox>().SingleOrDefault();
                var ipmaBox = iprpBox.Children.OfType<ItemPropertyAssociationBox>().SingleOrDefault();
                var irefBox = metaBox.Children.OfType<ItemReferenceBox>().SingleOrDefault();
                var iinfBox = metaBox.Children.OfType<ItemInfoBox>().SingleOrDefault();

                var primaryItem = metaBox.Children.OfType<PrimaryItemBox>().SingleOrDefault();
                primaryItemID = primaryItem.ItemID;

                int itemIndex = Array.IndexOf(ipmaBox.ItemID, primaryItemID);
                var indexes = ipmaBox.PropertyIndex[itemIndex];
                var propertyBoxes = ipcoBox.Children.Where((x, idx) => indexes.Contains((ushort)(idx + 1))).ToArray();

                if (iinfBox.ItemInfos.Single(x => x.ItemID == primaryItemID).ItemType == IsoStream.FromFourCC("grid"))
                {
                    // Apple stores HEIC files with a grid of images
                    foreach (var tile in irefBox.References.Single(x => x.FromItemID == primaryItemID).ToItemID)
                    {
                        indexes = ipmaBox.PropertyIndex[tile - 1];
                        propertyBoxes = ipcoBox.Children.Where((x, idx) => indexes.Contains((ushort)(idx + 1))).ToArray();
                        hvcC = propertyBoxes.OfType<HEVCConfigurationBox>().SingleOrDefault();
                        avcC = propertyBoxes.OfType<AVCConfigurationBox>().SingleOrDefault();
                        vvcC = propertyBoxes.OfType<VvcConfigurationBox>().SingleOrDefault();
                    }
                }
                else if (iinfBox.ItemInfos.Single(x => x.ItemID == primaryItemID).ItemType == IsoStream.FromFourCC("hvc1"))
                {
                    hvcC = propertyBoxes.OfType<HEVCConfigurationBox>().SingleOrDefault();
                    avcC = propertyBoxes.OfType<AVCConfigurationBox>().SingleOrDefault();
                    vvcC = propertyBoxes.OfType<VvcConfigurationBox>().SingleOrDefault();
                }
                else
                {
                    throw new NotSupportedException();
                }
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
                vvcC = visualSample.Children.OfType<VvcConfigurationBox>().SingleOrDefault();
                av1C = visualSample.Children.OfType<AV1CodecConfigurationBox>().SingleOrDefault();
            }

            if (avcC != null)
            {
                context = new H264Context();
                format = VideoFormat.H264;

                nalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
                {
                    try
                    {
                        ParseNALU((IItuContext)context, format, spsBinary);
                    }
                    catch (Exception ex)
                    {
                        SharpISOBMFF.Log.Error($"---Error (1) reading {file}, exception: {ex.Message}");
                        logger.Flush();
                        throw;
                    }
                }

                foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
                {
                    try
                    {
                        ParseNALU((IItuContext)context, format, ppsBinary);
                    }
                    catch (Exception ex)
                    {
                        SharpISOBMFF.Log.Error($"---Error (2) reading {file}, exception: {ex.Message}");
                        logger.Flush();
                        throw;
                    }
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
                        try 
                        { 
                            ParseNALU((IItuContext)context, format, nalu);
                        }
                        catch (Exception ex)
                        {
                            SharpISOBMFF.Log.Error($"---Error (3) reading {file}, exception: {ex.Message}");
                            logger.Flush();
                            throw;
                        }
                    }
                }
            }
            else if (vvcC != null)
            {
                context = new H266Context();
                format = VideoFormat.H266;

                nalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

                foreach (var nalus in vvcC._VvcConfig.NalUnit)
                {
                    foreach (var nalu in nalus)
                    {
                        try 
                        { 
                            ParseNALU((IItuContext)context, format, nalu);
                        }
                        catch (Exception ex)
                        {
                            SharpISOBMFF.Log.Error($"---Error (4) reading {file}, exception: {ex.Message}");
                            logger.Flush();
                            throw;
                        }
                    }
                }
            }
            else if(av1C != null)
            {
                context = new AV1Context();
                format = VideoFormat.AV1;

                ParseOBU((IAomContext)context, format, av1C.Av1Config.ConfigOBUs);
            }
            else
            {
                //throw new NotSupportedException();
                SharpISOBMFF.Log.Error($"{file}");
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
                        // fmp4
                        video_dts = 0;

                        IOrderedEnumerable<TrackRunBox> plan = moof.Children.OfType<TrackFragmentBox>().SelectMany(x => x.Children.OfType<TrackRunBox>()).OrderBy(x => x.DataOffset);
                        MovieExtendsBox mvex = moov.Children.OfType<MovieExtendsBox>().SingleOrDefault();
                        TrackExtendsBox trex = null;

                        if (mvex != null)
                        {
                            trex = mvex.Children.OfType<TrackExtendsBox>().SingleOrDefault(x => x.TrackID == videoTrackId);
                        }

                        foreach (var trun in plan)
                        {
                            var traf = trun.GetParent() as TrackFragmentBox;
                            var tfdt = traf.Children.OfType<TrackFragmentBaseMediaDecodeTimeBox>().SingleOrDefault();
                            var tfhd = traf.Children.OfType<TrackFragmentHeaderBox>().Single();
                            uint trackId = tfhd.TrackID;
                            bool isVideo = trackId == videoTrackId;
                            if(tfdt != null)
                            {
                                if(isVideo)
                                    video_dts = (long)tfdt.BaseMediaDecodeTime;
                                else
                                    audio_dts = (long)tfdt.BaseMediaDecodeTime;
                            }

                            if (SharpISOBMFF.Log.DebugEnabled) SharpISOBMFF.Log.Debug($"--TRUN: {(isVideo ? "video" : "audio")}");

                            var firstSampleFlags = trun.FirstSampleFlags;
                            if ((trun.Flags & 0x4) != 0x4)
                                firstSampleFlags = tfhd.DefaultSampleFlags;

                            for (int k = 1; k <= trun._TrunEntry.Length; k++)
                            {
                                var entry = trun._TrunEntry[k - 1];

                                uint sampleDuration = tfhd.DefaultSampleDuration;
                                if ((entry.Flags & 0x100) == 0x100)
                                    sampleDuration = entry.SampleDuration;
                                else if ((tfhd.Flags & 0x8) == 0x8)
                                    sampleDuration = tfhd.DefaultSampleDuration;
                                else if (trex != null)
                                    sampleDuration = trex.DefaultSampleDuration;
                                else
                                    throw new Exception("Cannot get sample duration");

                                uint sampleSize = tfhd.DefaultSampleSize;
                                if ((entry.Flags & 0x200) == 0x200)
                                    sampleSize = entry.SampleSize;
                                else if ((tfhd.Flags & 0x10) == 0x10)
                                    sampleSize = tfhd.DefaultSampleSize;
                                else if (trex != null)
                                    sampleSize = trex.DefaultSampleSize;
                                else
                                    throw new Exception("Cannot get sample size");

                                uint sampleFlags = tfhd.DefaultSampleFlags;
                                if(k == 1)
                                    sampleFlags = firstSampleFlags;
                                else if ((entry.Flags & 0x400) == 0x400)
                                    sampleFlags = entry.SampleFlags;

                                // CTS
                                int sampleCompositionTime = 0;
                                if ((entry.Flags & 0x800) == 0x800)
                                {
                                    if (entry.Version == 0)
                                        sampleCompositionTime = (int)entry.SampleCompositionTimeOffset;
                                    else
                                        sampleCompositionTime = entry.SampleCompositionTimeOffset0;
                                }

                                if (isVideo)
                                {
                                    video_pts = video_dts + sampleCompositionTime;

                                    try
                                    {
                                        size += ReadSample(nalLengthSize, (IItuContext)context, format, mdat.Data, sampleSize, video_dts, video_pts, sampleDuration);
                                    }
                                    catch (Exception ex)
                                    {
                                        SharpISOBMFF.Log.Error($"---Error (5) reading {file}, exception: {ex.Message}");
                                        logger.Flush();
                                        throw;
                                    }

                                    video_dts += sampleDuration;
                                }
                                else
                                {
                                    audio_pts = audio_dts + sampleCompositionTime;
                                    size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    audio_dts += sampleDuration;
                                }
                            }
                        }

                        // start looking for next moof/mdat pair
                        moof = null;
                        mdat = null; 
                    }
                    else
                    {
                        // mp4
                        video_dts = 0;
                        audio_dts = 0;

                        SampleTableBox sample_table_box = moov
                            .Children.OfType<TrackBox>().First(x => x.Children.OfType<TrackHeaderBox>().Single().TrackID == videoTrackId)
                            .Children.OfType<MediaBox>().Single()
                            .Children.OfType<MediaInformationBox>().Single()
                            .Children.OfType<SampleTableBox>().Single();
                        ChunkOffsetBox chunk_offsets_box = sample_table_box.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                        ChunkLargeOffsetBox chunk_offsets_large_box = sample_table_box.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();
                        SampleToChunkBox sample_to_chunks = sample_table_box.Children.OfType<SampleToChunkBox>().Single();
                        SampleSizeBox sample_size_box = sample_table_box.Children.OfType<SampleSizeBox>().Single();
                        TimeToSampleBox time_to_sample_box = sample_table_box.Children.OfType<TimeToSampleBox>().Single();
                        CompositionOffsetBox composition_offset_box = sample_table_box.Children.OfType<CompositionOffsetBox>().SingleOrDefault();
                        uint[] sampleSizes = sample_size_box.SampleSize > 0 ? Enumerable.Repeat(sample_size_box.SampleSize, (int)sample_size_box.SampleCount).ToArray() : sample_size_box.EntrySize;
                        ulong[] chunkOffsets = chunk_offsets_box != null ? chunk_offsets_box.ChunkOffset.Select(x => (ulong)x).ToArray() : chunk_offsets_large_box.ChunkOffset;

                        // https://developer.apple.com/documentation/quicktime-file-format/sample-to-chunk_atom/sample-to-chunk_table
                        int s2c_index = 0;
                        uint s2c_next_run = 0;
                        uint s2c_samples_per_chunk = 0;

                        int t2s_index = 0;
                        uint t2s_next_run = 0;
                        uint t2s_sample_delta = 0;

                        int co_index = 0;
                        uint co_next_run = 0;
                        int co_sample_delta = 0;

                        int sample_idx = 0;
                        for (int k = 1; k <= chunkOffsets.Length; k++)
                        {
                            if (k >= s2c_next_run)
                            {
                                s2c_samples_per_chunk = sample_to_chunks.SamplesPerChunk[s2c_index];
                                s2c_next_run = (s2c_index < sample_to_chunks.FirstChunk.Length) ? sample_to_chunks.FirstChunk[s2c_index] : (uint)(chunkOffsets.Length + 1);
                                s2c_index += 1;
                            }

                            long chunkOffset = (long)chunkOffsets[k - 1];

                            // seek to the chunk offset
                            mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                            // read samples in this chunk                            
                            for (int l = 1; l <= s2c_samples_per_chunk; l++)
                            {
                                if (sample_idx >= t2s_next_run)
                                {
                                    t2s_sample_delta = time_to_sample_box.SampleDelta[t2s_index];
                                    t2s_next_run += (t2s_index < time_to_sample_box.SampleCount.Length) ? time_to_sample_box.SampleCount[t2s_index] : (uint)(chunkOffsets.Length + 1);
                                    t2s_index += 1;
                                }

                                if (composition_offset_box != null && sample_idx >= co_next_run)
                                {
                                    co_sample_delta = composition_offset_box.Version == 0 ? (int)composition_offset_box.SampleOffset[co_index] : composition_offset_box.SampleOffset0[co_index];
                                    co_next_run += (co_index < composition_offset_box.SampleCount.Length) ? composition_offset_box.SampleCount[co_index] : (uint)(chunkOffsets.Length + 1);
                                    co_index += 1;
                                }

                                uint sampleSize = sampleSizes[sample_idx++];

                                video_pts = video_dts + co_sample_delta;

                                try
                                {
                                    size += ReadSample(nalLengthSize, context, format, mdat.Data, sampleSize, video_dts, video_pts, t2s_sample_delta);
                                }
                                catch (Exception ex)
                                {
                                    SharpISOBMFF.Log.Error($"---Error (6) reading {file}, exception: {ex.Message}");
                                    logger.Flush();
                                    throw;
                                }

                                video_dts += t2s_sample_delta;
                            }                            
                        }
                    }

                    mdat = null;
                }
                else if(meta != null && mdat != null)
                {
                    // heif
                    var ilocBox = meta.Children.OfType<ItemLocationBox>().SingleOrDefault();
                    var iinfBox = meta.Children.OfType<ItemInfoBox>().SingleOrDefault();

                    for (int l = 0; l < ilocBox.BaseOffset.Length; l++)
                    {
                        if(iinfBox.ItemInfos.ElementAt(l).ItemType != IsoStream.FromFourCC("hvc1"))
                           continue; // skip non-video items

                        int baseOffset = ilocBox.BaseOffset[l].Length == 0 ? 0 : GetVariableInt(ilocBox.BaseOffsetSize, ilocBox.BaseOffset[l]);

                        for (int j = 0; j < ilocBox.ExtentCount[l]; j++)
                        {
                            long position = baseOffset + GetVariableInt(ilocBox.OffsetSize, ilocBox.ExtentOffset[l][j]);
                            uint length = (uint)GetVariableInt(ilocBox.LengthSize, ilocBox.ExtentLength[l][j]);

                            mdat.Data.Stream.SeekFromBeginning(position);

                            try
                            {
                                size += ReadSample(nalLengthSize, (IItuContext)context, format, mdat.Data, length, video_dts, video_pts, 0);
                            }
                            catch (Exception ex)
                            {
                                SharpISOBMFF.Log.Error($"---Error (7) reading {file}, exception: {ex.Message}");
                                logger.Flush();
                                throw;
                            }
                        }
                    }
                }
            }
        }
    }
    catch(Exception ex)
    {
        SharpISOBMFF.Log.Error($"---Error (8) reading {file}, exception: {ex.Message}");
        logger.Flush();
    }
}

int GetVariableInt(byte size, byte[] bytes)
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

static ulong ReadSample(int length, object context, VideoFormat format, StreamMarker marker, uint sampleSizeInBytes, long dts, long pts, uint duration)
{
    if (format == VideoFormat.H264 || format == VideoFormat.H265 || format == VideoFormat.H266)
    {
        return ReadH26XSample(length, (IItuContext)context, format, marker, sampleSizeInBytes, dts, pts, duration);
    }
    else if (format == VideoFormat.AV1)
    {
        return ReadAVXSample(length, (IAomContext)context, format, marker, sampleSizeInBytes, dts, pts, duration);
    }
    else
    {
        throw new NotSupportedException();
    }
}

static ulong ReadH26XSample(int nalLengthSize, IItuContext context, VideoFormat format, StreamMarker marker, uint sampleSizeInBytes, long dts, long pts, uint duration)
{
    ulong size = 0;
    long offsetInBytes = 0;

    SharpISOBMFF.Log.Debug($"Sample begin {sampleSizeInBytes}, PTS: {pts}, DTS: {dts}, duration: {duration}");

    do
    {
        uint nalUnitLength = 0;
        switch (nalLengthSize)
        {
            case 1:
                size += marker.Stream.ReadUInt8(size, (ulong)marker.Length, out nalUnitLength);
                break;
            case 2:
                size += marker.Stream.ReadUInt16(size, (ulong)marker.Length, out nalUnitLength);
                break;
            case 3:
                size += marker.Stream.ReadUInt24(size, (ulong)marker.Length, out nalUnitLength);
                break;
            case 4:
                size += marker.Stream.ReadUInt32(size, (ulong)marker.Length, out nalUnitLength);
                break;
            default:
                throw new Exception($"NAL unit length {nalLengthSize} not supported!");
        }
        offsetInBytes += nalLengthSize;

        if (nalUnitLength > (sampleSizeInBytes - offsetInBytes))
        {
            SharpISOBMFF.Log.Error($"Invalid NALU size: {nalUnitLength}");
            nalUnitLength = (uint)(sampleSizeInBytes - offsetInBytes);
            size += nalUnitLength;
            offsetInBytes += nalUnitLength;
            break;
        }

        size += marker.Stream.ReadUInt8Array(size, (ulong)marker.Length, nalUnitLength, out byte[] sampleData);
        offsetInBytes += sampleData.Length;

        ParseNALU(context, format, sampleData);

    } while (offsetInBytes < sampleSizeInBytes);

    if (offsetInBytes != sampleSizeInBytes)
        throw new Exception("Mismatch!");

    SharpISOBMFF.Log.Debug("Sample end");

    return size;
}

static void ParseNALU(IItuContext ctx, VideoFormat format, byte[] sampleData)
{
    if(format == VideoFormat.H264)
    {
        ParseH264NALU((H264Context)ctx, sampleData);
    }
    else if(format == VideoFormat.H265)
    {
        ParseH265NALU((H265Context)ctx, sampleData);
    }
    else if (format == VideoFormat.H266)
    {
        ParseH266NALU((H266Context)ctx, sampleData);
    }
    else
    {
        throw new NotSupportedException();
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
                    SharpISOBMFF.Log.Debug($"NALU: 0, Unspecified {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE) // 1
                {
                    SharpISOBMFF.Log.Debug($"NALU: 1, slice of non-IDR picture, {sampleData.Length} bytes");
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPA) // 2
                {
                    SharpISOBMFF.Log.Debug($"NALU: 2, coded slice data partition A, {sampleData.Length} bytes");
                    context.SliceDataPartitionaLayerRbsp = new SliceDataPartitionaLayerRbsp();
                    context.SliceDataPartitionaLayerRbsp.Read(context, stream);
                    context.SliceDataPartitionaLayerRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPB) // 3
                {
                    SharpISOBMFF.Log.Debug($"NALU: 3, coded slice data partition B, {sampleData.Length} bytes");
                    context.SliceDataPartitionbLayerRbsp = new SliceDataPartitionbLayerRbsp();
                    context.SliceDataPartitionbLayerRbsp.Read(context, stream);
                    context.SliceDataPartitionbLayerRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPC) // 4
                {
                    SharpISOBMFF.Log.Debug($"NALU: 4, coded slice data partition C, {sampleData.Length} bytes");
                    context.SliceDataPartitioncLayerRbsp = new SliceDataPartitioncLayerRbsp();
                    context.SliceDataPartitioncLayerRbsp.Read(context, stream);
                    context.SliceDataPartitioncLayerRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.IDR_SLICE) // 5
                {
                    SharpISOBMFF.Log.Debug($"NALU: 5, slice of non-IDR picture, {sampleData.Length} bytes");
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SEI) // 6
                {
                    SharpISOBMFF.Log.Debug($"NALU: 6, SEI, {sampleData.Length} bytes");
                    context.SeiRbsp = new SharpH264.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SPS) // 7
                {
                    SharpISOBMFF.Log.Debug($"NALU: 7, SPS, {sampleData.Length} bytes");
                    context.SeqParameterSetRbsp = new SharpH264.SeqParameterSetRbsp();
                    context.SeqParameterSetRbsp.Read(context, stream);
                    context.SeqParameterSetRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.PPS) // 8
                {
                    SharpISOBMFF.Log.Debug($"NALU: 8, PPS, {sampleData.Length} bytes");
                    context.PicParameterSetRbsp = new SharpH264.PicParameterSetRbsp();
                    context.PicParameterSetRbsp.Read(context, stream);
                    context.PicParameterSetRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.AUD) // 9
                {
                    SharpISOBMFF.Log.Debug($"NALU: 9, AU Delimiter, {sampleData.Length} bytes");
                    context.AccessUnitDelimiterRbsp = new SharpH264.AccessUnitDelimiterRbsp();
                    context.AccessUnitDelimiterRbsp.Read(context, stream);
                    context.AccessUnitDelimiterRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.END_OF_SEQUENCE) // 10
                {
                    SharpISOBMFF.Log.Debug($"NALU: 10, End of Sequence, {sampleData.Length} bytes");
                    context.EndOfSeqRbsp = new SharpH264.EndOfSeqRbsp();
                    context.EndOfSeqRbsp.Read(context, stream);
                    context.EndOfSeqRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.END_OF_STREAM) // 11
                {
                    SharpISOBMFF.Log.Debug($"NALU: 11, End of Stream, {sampleData.Length} bytes");
                    context.EndOfStreamRbsp = new EndOfStreamRbsp();
                    context.EndOfStreamRbsp.Read(context, stream);
                    context.EndOfStreamRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.FILLER_DATA) // 12
                {
                    SharpISOBMFF.Log.Debug($"NALU: 12, Filler Data, {sampleData.Length} bytes");
                    context.FillerDataRbsp = new SharpH264.FillerDataRbsp();
                    context.FillerDataRbsp.Read(context, stream);
                    context.FillerDataRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SPS_EXT) // 13
                {
                    SharpISOBMFF.Log.Debug($"NALU: 13, SPS Extension, {sampleData.Length} bytes");
                    context.SeqParameterSetExtensionRbsp = new SeqParameterSetExtensionRbsp();
                    context.SeqParameterSetExtensionRbsp.Read(context, stream);
                    context.SeqParameterSetExtensionRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.PREFIX_NAL) // 14
                {
                    SharpISOBMFF.Log.Debug($"NALU: 14, Prefix NAL, {sampleData.Length} bytes");
                    context.PrefixNalUnitRbsp = new PrefixNalUnitRbsp();
                    context.PrefixNalUnitRbsp.Read(context, stream);
                    context.PrefixNalUnitRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SUBSET_SPS) // 15
                {
                    SharpISOBMFF.Log.Debug($"NALU: 15, Subset SPS, {sampleData.Length} bytes");
                    context.SubsetSeqParameterSetRbsp = new SubsetSeqParameterSetRbsp();
                    context.SubsetSeqParameterSetRbsp.Read(context, stream);
                    context.SubsetSeqParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.DPS) // 16
                {
                    SharpISOBMFF.Log.Debug($"NALU: 16, DPS, {sampleData.Length} bytes");
                    context.DepthParameterSetRbsp = new DepthParameterSetRbsp();
                    context.DepthParameterSetRbsp.Read(context, stream);
                    context.DepthParameterSetRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED0) // 17
                {
                    SharpISOBMFF.Log.Debug($"NALU: 17, Reserved {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED1) // 18
                {
                    SharpISOBMFF.Log.Debug($"NALU: 18, Reserved {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE_NOPARTITIONING) // 19
                {
                    SharpISOBMFF.Log.Debug($"NALU: 19, Auxiliary coded picture without partitioning, {sampleData.Length} bytes");
                    context.SliceLayerWithoutPartitioningRbsp = new SliceLayerWithoutPartitioningRbsp();
                    context.SliceLayerWithoutPartitioningRbsp.Read(context, stream);
                    context.SliceLayerWithoutPartitioningRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE_EXT) // 20
                {
                    SharpISOBMFF.Log.Debug($"NALU: 20, Slice Layer Extension, {sampleData.Length} bytes");
                    context.SliceLayerExtensionRbsp = new SliceLayerExtensionRbsp();
                    context.SliceLayerExtensionRbsp.Read(context, stream);
                    context.SliceLayerExtensionRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.SLICE_EXT_VIEW_COMPONENT) // 21
                {
                    SharpISOBMFF.Log.Debug($"NALU: 21, Slice Extension for Depth View or a 3D AVC texture view component, {sampleData.Length} bytes");
                    context.SliceLayerExtensionRbsp = new SliceLayerExtensionRbsp();
                    context.SliceLayerExtensionRbsp.Read(context, stream);
                    context.SliceLayerExtensionRbsp.Write(context, wstream);
                    if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                        throw new Exception($"Failed to write NALu {nu.NalUnitType}");
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED2) // 22
                {
                    SharpISOBMFF.Log.Debug($"NALU: 22, Reserved, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (nu.NalUnitType == H264NALTypes.RESERVED3) // 23
                {
                    SharpISOBMFF.Log.Debug($"NALU: 23, Reserved, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else
                {
                    SharpISOBMFF.Log.Debug($"NALU: Unspecified {nu.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                SharpISOBMFF.Log.Error($"Error: {ex.Message}");
                SharpISOBMFF.Log.Error($"SampleData: {Convert.ToHexString(sampleData)}");
                SharpISOBMFF.Log.Error($"WriteData:  {Convert.ToHexString(ms.ToArray())}");
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
                    SharpISOBMFF.Log.Debug($"NALU: Unspecified {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TRAIL_N ||      // 0
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TRAIL_R ||      // 1
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TSA_N ||        // 2
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.TSA_R ||        // 3
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.STSA_N ||       // 4
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.STSA_R ||       // 5
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RADL_N ||       // 6
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RADL_R ||       // 7
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RASL_N ||       // 8
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.RASL_R ||       // 9
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_LP ||     // 16
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.BLA_W_RADL ||   // 17
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.BLA_N_LP ||     // 18
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.IDR_W_RADL ||   // 19
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.IDR_N_LP ||     // 20
                    nu.NalUnitHeader.NalUnitType == H265NALTypes.CRA_NUT         // 21
                    )
                {
                    SharpISOBMFF.Log.Debug($"NALU: {nu.NalUnitHeader.NalUnitType}, Slice, {sampleData.Length} bytes");
                    context.SliceSegmentLayerRbsp = new SharpH265.SliceSegmentLayerRbsp();
                    context.SliceSegmentLayerRbsp.Read(context, stream);
                    context.SliceSegmentLayerRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray()))) // only partially parsed
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.VPS_NUT) // 32
                {
                    SharpISOBMFF.Log.Debug($"NALU: 32, VPS, {sampleData.Length} bytes");
                    context.VideoParameterSetRbsp = new SharpH265.VideoParameterSetRbsp();
                    context.VideoParameterSetRbsp.Read(context, stream);
                    context.VideoParameterSetRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.SPS_NUT) // 33
                {
                    SharpISOBMFF.Log.Debug($"NALU: 33, SPS, {sampleData.Length} bytes");
                    context.SeqParameterSetRbsp = new SharpH265.SeqParameterSetRbsp();
                    context.SeqParameterSetRbsp.Read(context, stream);
                    context.SeqParameterSetRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PPS_NUT) // 34
                {
                    SharpISOBMFF.Log.Debug($"NALU: 34, PPS, {sampleData.Length} bytes");
                    context.PicParameterSetRbsp = new SharpH265.PicParameterSetRbsp();
                    context.PicParameterSetRbsp.Read(context, stream);
                    context.PicParameterSetRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.AUD_NUT) // 35
                {
                    SharpISOBMFF.Log.Debug($"NALU: 35, AUD, {sampleData.Length} bytes");
                    context.AccessUnitDelimiterRbsp = new SharpH265.AccessUnitDelimiterRbsp();
                    context.AccessUnitDelimiterRbsp.Read(context, stream);
                    context.AccessUnitDelimiterRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.EOS_NUT) // 36
                {
                    SharpISOBMFF.Log.Debug($"NALU: 36, EOS, {sampleData.Length} bytes");
                    context.EndOfSeqRbsp = new SharpH265.EndOfSeqRbsp();
                    context.EndOfSeqRbsp.Read(context, stream);
                    context.EndOfSeqRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.EOB_NUT) // 37
                {
                    SharpISOBMFF.Log.Debug($"NALU: 37, EOB, {sampleData.Length} bytes");
                    context.EndOfBitstreamRbsp = new SharpH265.EndOfBitstreamRbsp();
                    context.EndOfBitstreamRbsp.Read(context, stream);
                    context.EndOfBitstreamRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.FD_NUT) // 38
                {
                    SharpISOBMFF.Log.Debug($"NALU: 38, FillerData, {sampleData.Length} bytes");
                    context.FillerDataRbsp = new SharpH265.FillerDataRbsp();
                    context.FillerDataRbsp.Read(context, stream);
                    context.FillerDataRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.PREFIX_SEI_NUT) // 39
                {
                    SharpISOBMFF.Log.Debug($"NALU: 39, Prefix SEI, {sampleData.Length} bytes");
                    context.SeiRbsp = new SharpH265.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H265NALTypes.SUFFIX_SEI_NUT) // 40
                {
                    SharpISOBMFF.Log.Debug($"NALU: 40, Suffix SEI, {sampleData.Length} bytes");
                    var oldSei = context.SeiRbsp;
                    context.SeiRbsp = new SharpH265.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!(ms.ToArray().SequenceEqual(sampleData) || ms.ToArray().Concat(new byte[] { 0 }).ToArray().SequenceEqual(sampleData))) // tolerate 1 zero byte padding
                        throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else
                {
                    // 10-15, 22-31, 41-47
                    SharpISOBMFF.Log.Debug($"NALU: Reserved {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                SharpISOBMFF.Log.Error($"Error: {ex.Message}");
                SharpISOBMFF.Log.Error($"SampleData: {Convert.ToHexString(sampleData)}");
                SharpISOBMFF.Log.Error($"WriteData:  {Convert.ToHexString(ms.ToArray())}");
                throw;
            }
        }
    }
}


static void ParseH266NALU(H266Context context, byte[] sampleData)
{
    NaluDebug.NaluCounter++;
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        ulong ituSize = 0;
        var nu = new SharpH266.NalUnit((uint)sampleData.Length);
        context.NalHeader = nu;
        ituSize += nu.Read(context, stream);

        var ms = new MemoryStream();
        using (ItuStream wstream = new ItuStream(ms))
        {
            nu.Write(context, wstream);

            try
            {
                if (nu.NalUnitHeader.NalUnitType >= H266NALTypes.UNSPEC_28) // unspecified
                {
                    SharpISOBMFF.Log.Debug($"NALU: Unspecified {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
                else if (
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.TRAIL_NUT ||    // 0
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.STSA_NUT ||     // 1
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.RADL_NUT ||     // 2
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.RASL_NUT ||     // 3
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.IDR_W_RADL ||   // 7
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.IDR_N_LP ||     // 8
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.CRA_NUT ||      // 9
                    nu.NalUnitHeader.NalUnitType == H266NALTypes.GDR_NUT         // 10
                    )
                {
                    SharpISOBMFF.Log.Debug($"NALU: {nu.NalUnitHeader.NalUnitType}, Slice, {sampleData.Length} bytes");
                    context.SliceLayerRbsp = new SharpH266.SliceLayerRbsp();
                    context.SliceLayerRbsp.Read(context, stream);
                    context.SliceLayerRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray()))) // only partially parsed
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.OPI_NUT) // 12
                {
                    SharpISOBMFF.Log.Debug($"NALU: 12, OPI, {sampleData.Length} bytes");
                    context.OperatingPointInformationRbsp = new SharpH266.OperatingPointInformationRbsp();
                    context.OperatingPointInformationRbsp.Read(context, stream);
                    context.OperatingPointInformationRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.DCI_NUT) // 13
                {
                    SharpISOBMFF.Log.Debug($"NALU: 13, DCI, {sampleData.Length} bytes");
                    context.DecodingCapabilityInformationRbsp = new SharpH266.DecodingCapabilityInformationRbsp();
                    context.DecodingCapabilityInformationRbsp.Read(context, stream);
                    context.DecodingCapabilityInformationRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.VPS_NUT) // 14
                {
                    SharpISOBMFF.Log.Debug($"NALU: 14, VPS, {sampleData.Length} bytes");
                    context.VideoParameterSetRbsp = new SharpH266.VideoParameterSetRbsp();
                    context.VideoParameterSetRbsp.Read(context, stream);
                    context.VideoParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.SPS_NUT) // 15
                {
                    SharpISOBMFF.Log.Debug($"NALU: 15, SPS, {sampleData.Length} bytes");
                    context.SeqParameterSetRbsp = new SharpH266.SeqParameterSetRbsp();
                    context.SeqParameterSetRbsp.Read(context, stream);
                    context.SeqParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PPS_NUT) // 16
                {
                    SharpISOBMFF.Log.Debug($"NALU: 16, PPS, {sampleData.Length} bytes");
                    context.PicParameterSetRbsp = new SharpH266.PicParameterSetRbsp();
                    context.PicParameterSetRbsp.Read(context, stream);
                    context.PicParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PREFIX_APS_NUT) // 17
                {
                    SharpISOBMFF.Log.Debug($"NALU: 17, Prefix APS, {sampleData.Length} bytes");
                    context.AdaptationParameterSetRbsp = new SharpH266.AdaptationParameterSetRbsp();
                    context.AdaptationParameterSetRbsp.Read(context, stream);
                    context.AdaptationParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.SUFFIX_APS_NUT) // 18
                {
                    SharpISOBMFF.Log.Debug($"NALU: 18, Suffix APS, {sampleData.Length} bytes");
                    context.AdaptationParameterSetRbsp = new SharpH266.AdaptationParameterSetRbsp();
                    context.AdaptationParameterSetRbsp.Read(context, stream);
                    context.AdaptationParameterSetRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PH_NUT) // 19
                {
                    SharpISOBMFF.Log.Debug($"NALU: 19, Picture Header, {sampleData.Length} bytes");
                    context.PictureHeaderRbsp = new SharpH266.PictureHeaderRbsp();
                    context.PictureHeaderRbsp.Read(context, stream);
                    context.PictureHeaderRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.AUD_NUT) // 20
                {
                    SharpISOBMFF.Log.Debug($"NALU: 20, AUD, {sampleData.Length} bytes");
                    context.AccessUnitDelimiterRbsp = new SharpH266.AccessUnitDelimiterRbsp();
                    context.AccessUnitDelimiterRbsp.Read(context, stream);
                    context.AccessUnitDelimiterRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.EOS_NUT) // 21
                {
                    SharpISOBMFF.Log.Debug($"NALU: 21, EOS, {sampleData.Length} bytes");
                    context.EndOfSeqRbsp = new SharpH266.EndOfSeqRbsp();
                    context.EndOfSeqRbsp.Read(context, stream);
                    context.EndOfSeqRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.EOB_NUT) // 22
                {
                    SharpISOBMFF.Log.Debug($"NALU: 22, EOB, {sampleData.Length} bytes");
                    context.EndOfBitstreamRbsp = new SharpH266.EndOfBitstreamRbsp();
                    context.EndOfBitstreamRbsp.Read(context, stream);
                    context.EndOfBitstreamRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.PREFIX_SEI_NUT) // 23
                {
                    SharpISOBMFF.Log.Debug($"NALU: 23, Prefix SEI, {sampleData.Length} bytes");
                    context.SeiRbsp = new SharpH266.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.SUFFIX_SEI_NUT) // 24
                {
                    SharpISOBMFF.Log.Debug($"NALU: 24, Suffix SEI, {sampleData.Length} bytes");
                    context.SeiRbsp = new SharpH266.SeiRbsp();
                    context.SeiRbsp.Read(context, stream);
                    context.SeiRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else if (nu.NalUnitHeader.NalUnitType == H266NALTypes.FD_NUT) // 25
                {
                    SharpISOBMFF.Log.Debug($"NALU: 25, Filler Data, {sampleData.Length} bytes");
                    context.FillerDataRbsp = new SharpH266.FillerDataRbsp();
                    context.FillerDataRbsp.Read(context, stream);
                    context.FillerDataRbsp.Write(context, wstream);
                    if (!ms.ToArray().SequenceEqual(sampleData))
                        //if (!Convert.ToHexString(sampleData).StartsWith(Convert.ToHexString(ms.ToArray())))
                            throw new Exception($"Failed to write NALu {nu.NalUnitHeader.NalUnitType}");
                }
                else
                {
                    // 4-6, 11, 26-27
                    SharpISOBMFF.Log.Debug($"NALU: Reserved {nu.NalUnitHeader.NalUnitType}, {sampleData.Length} bytes");
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                SharpISOBMFF.Log.Error($"NALU counter: {NaluDebug.NaluCounter}");
                SharpISOBMFF.Log.Error($"Error: {ex.Message}");
                SharpISOBMFF.Log.Error($"SampleData: {Convert.ToHexString(sampleData)}");
                SharpISOBMFF.Log.Error($"WriteData:  {Convert.ToHexString(ms.ToArray())}");
                throw;
            }
        }
    }
}

static ulong ReadAVXSample(int length, IAomContext context, VideoFormat format, StreamMarker marker, uint sampleSizeInBytes, long dts, long pts, uint duration)
{
    ulong size = 0;

    SharpISOBMFF.Log.Debug($"Sample begin {sampleSizeInBytes}, PTS: {pts}, DTS: {dts}, duration: {duration}");

    size += marker.Stream.ReadUInt8Array(size, (ulong)marker.Length, sampleSizeInBytes, out byte[] sampleData);
    using(AomStream stream = new AomStream(new MemoryStream(sampleData)))
    {
        int len = (int)sampleSizeInBytes;
        do
        {
            context.Read(stream, (int)sampleSizeInBytes);
            len -= context.ObuSize;
        } while (len > 0);
    }

    SharpISOBMFF.Log.Debug("Sample end");

    return size;
}

static void ParseOBU(IAomContext context, VideoFormat format, byte[] sampleData)
{
    using (AomStream stream = new AomStream(new MemoryStream(sampleData)))
    {
        
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
    H266,
    AV1,
}

public static class NaluDebug
{
    public static int NaluCounter = 0;
}

public class FileLogger : IDisposable
{
    private readonly string _path;
    private readonly FileStream _fs;
    private readonly BufferedStream _bs;

    public FileLogger(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
        _fs = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.Read);
        _bs = new BufferedStream(_fs, 4096);
    }

    public void Log(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        _bs.Write(bytes, 0, bytes.Length);
    }

    public void Flush()
    {
        _bs.Flush();
    }

    public void Dispose()
    {
        _bs.Flush();
        _bs.Dispose();
    }
}