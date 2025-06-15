using System;
using System.Collections.Generic;
using System.Linq;
using SharpH264;
using SharpH265;
using SharpH266;
using SharpH26X;
using SharpISOBMFF;

namespace SharpMP4
{
    public enum VideoFormat
    {
        Unknown,
        H264,
        H265,
        H266,
    }

    public static class Mp4Extensions
    {
        public static IEnumerable<TrackBox> FindVideoTrack(this Mp4 mp4)
        {
            return mp4.GetMovieBox().GetTracks()
               .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Video));
        }

        public static IEnumerable<TrackBox> FindAudioTrack(this Mp4 mp4)
        {
            return mp4.GetMovieBox().GetTracks()
                .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Sound));
        }

        public static IEnumerable<TrackBox> FindHintTrack(this Mp4 mp4)
        {
            return mp4.GetMovieBox().GetTracks()
                .Where(x =>
                    x.Children.OfType<MediaBox>().Single()
                     .Children.OfType<HandlerBox>().Single()
                     .HandlerType == IsoStream.FromFourCC(HandlerTypes.Hint));
        }

        public static MovieBox GetMovieBox(this Mp4 mp4)
        {
            return mp4.Children.OfType<MovieBox>().Single();
        }

        public static IEnumerable<TrackBox> GetTracks(this MovieBox moov)
        {
            return moov.Children.OfType<TrackBox>();
        }

        public static AudioSampleEntry GetAudioSampleEntryBox(this TrackBox track)
        {
            return track
                .Children.OfType<MediaBox>().Single()
                .Children.OfType<MediaInformationBox>().Single()
                .Children.OfType<SampleTableBox>().Single()
                .Children.OfType<SampleDescriptionBox>().Single()
                .Children.OfType<AudioSampleEntry>().Single();
        }

        public static List<IList<IList<byte[]>>> ParseMdat(this Mp4 inputMp4)
        {
            List<IList<IList<byte[]>>> ret = new List<IList<IList<byte[]>>>();

            int nalLengthSize = 4;
            IItuContext context = null;
            VideoFormat format = VideoFormat.Unknown;
            uint videoTrackId = 0;
            uint audioTrackId = 0;

            AVCConfigurationBox avcC = null;
            HEVCConfigurationBox hvcC = null;
            VvcConfigurationBox vvcC = null;

            if (inputMp4.Children.Count == 0)
                return null;

            var ftypBox = inputMp4
                .Children.OfType<FileTypeBox>().SingleOrDefault();
            var movieBox = inputMp4
                .Children.OfType<MovieBox>().SingleOrDefault();
            var tracks = movieBox
                .Children.OfType<TrackBox>();

            long[] dts = new long[tracks.Count()];
            long[] pts = new long[tracks.Count()];

            var videoTrack = inputMp4.FindVideoTrack().First();
            var audioTrack = inputMp4.FindAudioTrack().FirstOrDefault();
            videoTrackId = videoTrack.Children.OfType<TrackHeaderBox>().Single().TrackID;
            if(audioTrack != null)
                audioTrackId = audioTrack.Children.OfType<TrackHeaderBox>().Single().TrackID;

            var visualSample = videoTrack
                .Children.OfType<MediaBox>().Single()
                .Children.OfType<MediaInformationBox>().Single()
                .Children.OfType<SampleTableBox>().Single()
                .Children.OfType<SampleDescriptionBox>().Single()
                .Children.OfType<VisualSampleEntry>().Single();

            avcC = visualSample.Children.OfType<AVCConfigurationBox>().SingleOrDefault();
            hvcC = visualSample.Children.OfType<HEVCConfigurationBox>().SingleOrDefault();
            vvcC = visualSample.Children.OfType<VvcConfigurationBox>().SingleOrDefault();            

            foreach(var track in tracks)
                ret.Add(new List<IList<byte[]>>() { new List<byte[]>() });

            if (avcC != null)
            {
                context = new H264Context();
                format = VideoFormat.H264;

                nalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // usually 4 bytes

                foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
                {
                    try
                    {
                        //ParseNALU(context, format, spsBinary);
                        ret[(int)(videoTrackId - 1)][0].Add(spsBinary);
                    }
                    catch (Exception ex)
                    {
                        SharpISOBMFF.Log.Error($"---Error (1) reading file, exception: {ex.Message}");
                        throw;
                    }
                }

                foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
                {
                    try
                    {
                        //ParseNALU(context, format, ppsBinary);
                        ret[(int)(videoTrackId - 1)][0].Add(ppsBinary);
                    }
                    catch (Exception ex)
                    {
                        SharpISOBMFF.Log.Error($"---Error (2) reading file, exception: {ex.Message}");
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
                            //ParseNALU(context, format, nalu);
                            ret[(int)(videoTrackId - 1)][0].Add(nalu);
                        }
                        catch (Exception ex)
                        {
                            SharpISOBMFF.Log.Error($"---Error (3) reading file, exception: {ex.Message}");
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
                            //ParseNALU(context, format, nalu);
                            ret[(int)(videoTrackId - 1)][0].Add(nalu);
                        }
                        catch (Exception ex)
                        {
                            SharpISOBMFF.Log.Error($"---Error (4) reading file, exception: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            else
            {
                //throw new NotSupportedException();
                SharpISOBMFF.Log.Error($"Error reading file");
                return null;
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
                    var currentMdat = (MediaDataBox)inputMp4.Children[i];

                    if (currentMdat.Size > 8)
                    {
                        mdat = currentMdat;
                    }
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
                            if (tfdt != null)
                            {
                                dts[trackId - 1] = (long)tfdt.BaseMediaDecodeTime;
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
                                if (k == 1)
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

                                pts[trackId - 1] = dts[trackId - 1] + sampleCompositionTime;

                                if (isVideo)
                                {
                                    try
                                    {
                                        var au = ReadAU(nalLengthSize, context, format, mdat.Data, sampleSize, dts[(int)(trackId - 1)], pts[(int)(trackId - 1)], sampleDuration);
                                        size += au.size;
                                        ret[(int)(trackId - 1)].Add(au.naluList);
                                    }
                                    catch (Exception ex)
                                    {
                                        SharpISOBMFF.Log.Error($"---Error (5) reading file, exception: {ex.Message}");
                                        throw;
                                    }

                                }
                                else
                                {
                                    size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, sampleSize, out byte[] sampleData);
                                    ret[(int)(trackId - 1)][0].Add(sampleData);
                                }

                                dts[trackId - 1] += sampleDuration;
                            }
                        }

                        // start looking for next moof/mdat pair
                        moof = null;
                        mdat = null;
                    }
                    else
                    {
                        foreach (var track in tracks)
                        {
                            // mp4
                            uint trackId = track.Children.OfType<TrackHeaderBox>().Single().TrackID;

                            SampleTableBox sample_table_box = track
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

                            bool isVideo = trackId == videoTrackId;

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
                                    s2c_index += 1;
                                    s2c_next_run = (s2c_index < sample_to_chunks.FirstChunk.Length) ? sample_to_chunks.FirstChunk[s2c_index] : (uint)(chunkOffsets.Length + 1);
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

                                    pts[trackId - 1] = dts[trackId - 1] + co_sample_delta;

                                    if (isVideo)
                                    {
                                        try
                                        {
                                            var au = ReadAU(nalLengthSize, context, format, mdat.Data, sampleSize, dts[trackId - 1], pts[trackId - 1], t2s_sample_delta);
                                            size += au.size;
                                            ret[(int)(trackId - 1)].Add(au.naluList);
                                        }
                                        catch (Exception ex)
                                        {
                                            SharpISOBMFF.Log.Error($"---Error (6) reading file, exception: {ex.Message}");
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, sampleSize, out byte[] sampleData);
                                        ret[(int)(trackId - 1)][0].Add(sampleData);
                                    }

                                    dts[trackId - 1] += t2s_sample_delta;
                                }
                            }
                        }
                    }

                    mdat = null;
                }
            }

            return ret;
        }

        static (ulong size, List<byte[]> naluList) ReadAU(int nalLengthSize, IItuContext context, VideoFormat format, StreamMarker marker, uint sampleSizeInBytes, long dts, long pts, uint duration)
        {
            ulong size = 0;
            long offsetInBytes = 0;

            SharpISOBMFF.Log.Debug($"AU begin {sampleSizeInBytes}, PTS: {pts}, DTS: {dts}, duration: {duration}");

            List<byte[]> naluList = new List<byte[]>();

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
                //ParseNALU(context, format, sampleData);
                naluList.Add(sampleData);
            } while (offsetInBytes < sampleSizeInBytes);

            if (offsetInBytes != sampleSizeInBytes)
                throw new Exception("Mismatch!");

            SharpISOBMFF.Log.Debug("AU end");

            return (size, naluList);
        }
    }
}
