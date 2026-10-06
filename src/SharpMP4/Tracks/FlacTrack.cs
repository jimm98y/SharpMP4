using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// FLAC Track: an 'fLaC' sample entry and its 'dfLa', of the stream's metadata blocks - STREAMINFO first.
    /// </summary>
    /// <remarks>https://github.com/xiph/flac/blob/master/doc/isoflac.txt</remarks>
    public class FlacTrack : TrackBase
    {
        /// <summary>The metadata block type of STREAMINFO, which a stream has first and once (RFC 9639 8.1).</summary>
        public const byte STREAMINFO = 0;

        /// <summary>The size of a STREAMINFO block's data.</summary>
        public const int STREAMINFO_SIZE = 34;

        public override string HandlerName => HandlerNames.Sound;
        public override string HandlerType => HandlerTypes.Sound;
        public override string Language { get; set; } = "und";

        public byte ChannelCount { get; private set; }
        public uint SamplingRate { get; private set; }
        public byte BitsPerSample { get; private set; }

        /// <summary>The samples of each channel a frame holds, at most: of a stream of a fixed block size, every frame's but the last.</summary>
        public ushort MaxBlockSize { get; private set; }

        /// <summary>The stream's metadata blocks, STREAMINFO first, each its type and its data - without the header of each.</summary>
        public IReadOnlyList<(byte Type, byte[] Data)> MetadataBlocks { get; }

        /// <summary>The STREAMINFO block's data: 34 bytes.</summary>
        public byte[] StreamInfo => MetadataBlocks[0].Data;

        /// <summary>
        /// Ctor, of a stream's metadata blocks, STREAMINFO first: what an encoder hands out of a stream's header, or 'fLaC'
        /// and the blocks of a FLAC file's start.
        /// </summary>
        /// <param name="metadataBlocks">The blocks' types and data, STREAMINFO first.</param>
        public FlacTrack(IEnumerable<(byte Type, byte[] Data)> metadataBlocks)
        {
            MetadataBlocks = metadataBlocks?.ToList() ?? throw new ArgumentNullException(nameof(metadataBlocks));
            ReadStreamInfo();
        }

        /// <summary>Ctor, of a stream's STREAMINFO block's data alone.</summary>
        public FlacTrack(byte[] streamInfo) : this(new[] { (STREAMINFO, streamInfo) })
        { }

        /// <summary>
        /// Ctor with initialization from the <see cref="SampleEntry"/>.
        /// </summary>
        /// <param name="config">The entry's <see cref="FLACSpecificBox"/>.</param>
        public FlacTrack(Box config, uint timescale = 0, int sampleDuration = -1)
        {
            if (config is not FLACSpecificBox dfLa)
                throw new ArgumentException($"Invalid FLACSpecificBox: {IsoStream.ToFourCC(config.FourCC)}");

            MetadataBlocks = (dfLa.Blocks ?? Array.Empty<FLACMetadataBlock>()).Select(b => (b._BlockType, b._BlockData)).ToList();
            ReadStreamInfo();

            // of the stream's own rate, which the entry cannot say above 65535
            Timescale = timescale == 0 ? SamplingRate : timescale;
            if (sampleDuration > 0)
                DefaultSampleDuration = sampleDuration;
        }

        /// <summary>
        /// The stream's parameters, of its STREAMINFO (RFC 9639 8.2): the block sizes in 16 bits each, the frame sizes in 24,
        /// then the sample rate in 20, the channels less one in 3 and the bits per sample less one in 5.
        /// </summary>
        private void ReadStreamInfo()
        {
            if (MetadataBlocks.Count == 0 || MetadataBlocks[0].Type != STREAMINFO || MetadataBlocks[0].Data == null || MetadataBlocks[0].Data.Length < STREAMINFO_SIZE)
                throw new ArgumentException("A FLAC stream's metadata begins with its STREAMINFO block, of 34 bytes.");

            byte[] info = MetadataBlocks[0].Data;
            MaxBlockSize = (ushort)((info[2] << 8) | info[3]);
            SamplingRate = (uint)((info[10] << 12) | (info[11] << 4) | (info[12] >> 4));
            ChannelCount = (byte)(((info[12] >> 1) & 0x7) + 1);
            BitsPerSample = (byte)((((info[12] & 0x1) << 4) | (info[13] >> 4)) + 1);

            Timescale = SamplingRate;
            DefaultSampleDuration = MaxBlockSize;
        }

        /// <summary>
        /// The stream's header as a FLAC file starts with it: 'fLaC', then each metadata block with its header, the last
        /// marked so - what a decoder of a FLAC stream is given first.
        /// </summary>
        public byte[] CreateStreamHeader()
        {
            var header = new List<byte> { (byte)'f', (byte)'L', (byte)'a', (byte)'C' };
            for (int i = 0; i < MetadataBlocks.Count; i++)
            {
                var (type, data) = MetadataBlocks[i];
                header.Add((byte)((i == MetadataBlocks.Count - 1 ? 0x80 : 0) | (type & 0x7F)));
                header.Add((byte)(data.Length >> 16));
                header.Add((byte)(data.Length >> 8));
                header.Add((byte)data.Length);
                header.AddRange(data);
            }
            return header.ToArray();
        }

        /// <summary>
        /// The metadata blocks of a FLAC stream's header - 'fLaC' and the blocks, each with its header - as
        /// <see cref="FlacTrack(IEnumerable{ValueTuple{byte, byte[]}})"/> takes them.
        /// </summary>
        public static List<(byte Type, byte[] Data)> ParseStreamHeader(byte[] header)
        {
            if (header == null || header.Length < 8 || header[0] != 'f' || header[1] != 'L' || header[2] != 'a' || header[3] != 'C')
                throw new ArgumentException("A FLAC stream's header begins with 'fLaC'.", nameof(header));

            var blocks = new List<(byte, byte[])>();
            int position = 4;
            while (position + 4 <= header.Length)
            {
                bool last = (header[position] & 0x80) != 0;
                byte type = (byte)(header[position] & 0x7F);
                int length = (header[position + 1] << 16) | (header[position + 2] << 8) | header[position + 3];
                position += 4;
                if (position + length > header.Length)
                    throw new ArgumentException("A FLAC metadata block runs past the header's end.", nameof(header));
                var data = new byte[length];
                Buffer.BlockCopy(header, position, data, 0, length);
                blocks.Add((type, data));
                position += length;
                if (last)
                    break;
            }
            return blocks;
        }

        /// <summary>
        /// The sample goes to the file as it arrives - a frame, each of which a decoder can start at - so this hands back
        /// exactly what it was given, without copying it anywhere.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            isRandomAccessPoint = true;
            output = buffer == null ? default : new ArraySegment<byte>(buffer, offset, length);
        }

        public override Box CreateSampleEntryBox()
        {
            AudioSampleEntryV1 audioSampleEntry = new AudioSampleEntryV1(IsoStream.FromFourCC("fLaC"));
            audioSampleEntry.Children = new List<Box>();
            audioSampleEntry.Channelcount = ChannelCount;
            // 16.16 fixed point, of a rate it can hold; of one above 65535, 0, the stream's own rate in its STREAMINFO
            audioSampleEntry.Samplerate = SamplingRate <= 0xFFFF ? SamplingRate << 16 : 0;
            audioSampleEntry.DataReferenceIndex = 1;
            audioSampleEntry.Samplesize = BitsPerSample;
            audioSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API
            audioSampleEntry.Reserved = new ushort[3]; // TODO simplify API

            FLACSpecificBox dfLa = new FLACSpecificBox();
            dfLa.SetParent(audioSampleEntry);
            audioSampleEntry.Children.Add(dfLa);

            var blocks = new FLACMetadataBlock[MetadataBlocks.Count];
            for (int i = 0; i < blocks.Length; i++)
            {
                var (type, data) = MetadataBlocks[i];
                blocks[i] = new FLACMetadataBlock
                {
                    _LastMetadataBlockFlag = i == blocks.Length - 1,
                    _BlockType = type,
                    _Length = (uint)data.Length,
                    _BlockData = data,
                };
                blocks[i].SetParent(dfLa);
            }
            dfLa.Blocks = blocks;

            return audioSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            tkhd.Volume = 256;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new FlacTrack(MetadataBlocks.Select(b => (b.Type, (byte[])b.Data.Clone()))));
        }
    }
}
