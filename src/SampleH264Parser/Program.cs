
// try parse SPS
using SharpH264;
using SharpMP4;
using System;
using System.Collections.Generic;
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

    var avcC = h264VisualSample.Children.OfType<AVCConfigurationBox>().Single();
    int nalLengthSize = avcC._AVCConfig.LengthSizeMinusOne + 1; // 4 bytes

    SeqParameterSetRbsp sps;
    foreach (var spsBinary in avcC._AVCConfig.SequenceParameterSetNALUnit)
    {
        using (ItuStream stream = new ItuStream(new MemoryStream(spsBinary)))
        {
            NalUnit nu = new NalUnit((uint)spsBinary.Length);
            nu.Read(stream);
            H264Helpers.SetNalUnit(nu);

            sps = new SeqParameterSetRbsp();
            H264Helpers.SetSeqParameterSet(sps);

            sps.Read(stream);

            var ms = new MemoryStream();
            using (ItuStream wstream = new ItuStream(ms))
            {
                nu.Write(wstream);
                sps.Write(wstream);

                byte[] wbytes = ms.ToArray();
                if (!wbytes.SequenceEqual(spsBinary))
                {
                    throw new Exception("Failed to write SPS");
                }
            }
        }
    }

    PicParameterSetRbsp pps;
    foreach (var ppsBinary in avcC._AVCConfig.PictureParameterSetNALUnit)
    {
        using (ItuStream stream = new ItuStream(new MemoryStream(ppsBinary)))
        {
            NalUnit nu = new NalUnit((uint)ppsBinary.Length);
            nu.Read(stream);
            H264Helpers.SetNalUnit(nu);

            pps = new PicParameterSetRbsp();
            H264Helpers.SetPicParameterSet(pps);

            pps.Read(stream);

            var ms = new MemoryStream();
            using (ItuStream wstream = new ItuStream(ms))
            {
                nu.Write(wstream);
                pps.Write(wstream);

                byte[] wbytes = ms.ToArray();
                if (!wbytes.SequenceEqual(ppsBinary))
                {
                    throw new Exception("Failed to write PPS");
                }
            }
        }
    }

    MovieBox moov = null;
    MovieFragmentBox moof = null;
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
        else if (inputMp4.Children[i] is MediaDataBox mdat)
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
                            size += mdat.Data.Stream.ReadUInt32(size, (ulong)mdat.Data.Length, out nalUnitLength);
                            size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, nalUnitLength, out byte[] sampleData);
                            ReadNALu(sampleData);
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
                uint[] sampleSizes =  sample_size_box.SampleSize > 0 ? Enumerable.Repeat(sample_size_box.SampleSize, (int)sample_size_box.SampleCount).ToArray() : sample_size_box.EntrySize;
                ulong[] chunkOffsets = chunk_offsets_box != null ? chunk_offsets_box.ChunkOffset.Select(x => (ulong)x).ToArray() : chunk_offsets_large_box.ChunkOffset;

                ulong size = 0;
                for(int k = 0; k < chunkOffsets.Length; k++)
                {
                    long chunkOffset = (long)chunkOffsets[k];
                    uint sampleSize = sampleSizes[k];

                    uint nalUnitLength = 0;
                    mdat.Data.Stream.SeekFromBeginning(chunkOffset);

                    // AU begin
                    long offset = 0;
                    do
                    {
                        size += mdat.Data.Stream.ReadUInt32(size, (ulong)mdat.Data.Length, out nalUnitLength);
                        size += mdat.Data.Stream.ReadUInt8Array(size, (ulong)mdat.Data.Length, nalUnitLength, out byte[] sampleData);
                        offset += 4 + sampleData.Length;
                        ReadNALu(sampleData);
                    } while (offset < sampleSize);
                }
            }
        }
    }
}

static void ReadNALu(byte[] sampleData)
{
    using (ItuStream stream = new ItuStream(new MemoryStream(sampleData)))
    {
        ulong ituSize = 0;
        NalUnit nu = new NalUnit((uint)sampleData.Length);
        ituSize += nu.Read(stream);
        H264Helpers.SetNalUnit(nu);

        if (nu.NalUnitType == 6)
        {
            Debug.WriteLine("NALU: SEI");

            SeiRbsp sei = new SeiRbsp();
            H264Helpers.SetSei(sei);
            ituSize += sei.Read(stream);
        }
        else if (nu.NalUnitType == 9)
        {
            Debug.WriteLine($"NALU: AU Delimiter");
        }
        else
        {
            Debug.WriteLine($"NALU: {nu.NalUnitType}");
        }
    }
}