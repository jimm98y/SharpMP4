using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxGenerator2
{
    public class ReceivedSsrcBox : Box
    {
        public override string FourCC { get { return "rssr"; } }
        public uint SSRC;

        public ReceivedSsrcBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.SSRC);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.SSRC);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // SSRC
            return boxSize;
        }
    }


    public class timestampsynchrony : Box
    {
        public override string FourCC { get { return "tssy"; } }
        public byte reserved;
        public byte timestamp_sync;

        public timestampsynchrony()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.timestamp_sync);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.timestamp_sync);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 6; // reserved
            boxSize += 2; // timestamp_sync
            return boxSize;
        }
    }


    public class timescaleentry : Box
    {
        public override string FourCC { get { return "tims"; } }
        public uint timescale;

        public timescaleentry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // timescale
            return boxSize;
        }
    }


    public class timeoffset : Box
    {
        public override string FourCC { get { return "tims"; } }
        public int offset;

        public timeoffset()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // offset
            return boxSize;
        }
    }


    public class sequenceoffset : Box
    {
        public override string FourCC { get { return "tims"; } }
        public int offset;

        public sequenceoffset()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // offset
            return boxSize;
        }
    }


    public class timescaleentry1 : Box
    {
        public override string FourCC { get { return "tsro"; } }
        public uint timescale;

        public timescaleentry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // timescale
            return boxSize;
        }
    }


    public class timeoffset1 : Box
    {
        public override string FourCC { get { return "tsro"; } }
        public int offset;

        public timeoffset1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // offset
            return boxSize;
        }
    }


    public class sequenceoffset1 : Box
    {
        public override string FourCC { get { return "tsro"; } }
        public int offset;

        public sequenceoffset1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // offset
            return boxSize;
        }
    }


    public class timescaleentry2 : Box
    {
        public override string FourCC { get { return "snro"; } }
        public uint timescale;

        public timescaleentry2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // timescale
            return boxSize;
        }
    }


    public class timeoffset2 : Box
    {
        public override string FourCC { get { return "snro"; } }
        public int offset;

        public timeoffset2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // offset
            return boxSize;
        }
    }


    public class sequenceoffset2 : Box
    {
        public override string FourCC { get { return "snro"; } }
        public int offset;

        public sequenceoffset2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // offset
            return boxSize;
        }
    }


    public class hintBytesSent : Box
    {
        public override string FourCC { get { return "trpy"; } }
        public ulong bytessent;

        public hintBytesSent()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintPacketsSent : Box
    {
        public override string FourCC { get { return "trpy"; } }
        public ulong packetssent;

        public hintPacketsSent()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.packetssent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.packetssent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // packetssent
            return boxSize;
        }
    }


    public class hintBytesSent1 : Box
    {
        public override string FourCC { get { return "trpy"; } }
        public ulong bytessent;

        public hintBytesSent1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintBytesSent2 : Box
    {
        public override string FourCC { get { return "nump"; } }
        public ulong bytessent;

        public hintBytesSent2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintPacketsSent1 : Box
    {
        public override string FourCC { get { return "nump"; } }
        public ulong packetssent;

        public hintPacketsSent1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.packetssent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.packetssent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // packetssent
            return boxSize;
        }
    }


    public class hintBytesSent3 : Box
    {
        public override string FourCC { get { return "nump"; } }
        public ulong bytessent;

        public hintBytesSent3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintBytesSent4 : Box
    {
        public override string FourCC { get { return "tpyl"; } }
        public ulong bytessent;

        public hintBytesSent4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintPacketsSent2 : Box
    {
        public override string FourCC { get { return "tpyl"; } }
        public ulong packetssent;

        public hintPacketsSent2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.packetssent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.packetssent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // packetssent
            return boxSize;
        }
    }


    public class hintBytesSent5 : Box
    {
        public override string FourCC { get { return "tpyl"; } }
        public ulong bytessent;

        public hintBytesSent5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintBytesSent6 : Box
    {
        public override string FourCC { get { return "totl"; } }
        public uint bytessent;

        public hintBytesSent6()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytessent
            return boxSize;
        }
    }


    public class hintPacketsSent3 : Box
    {
        public override string FourCC { get { return "totl"; } }
        public uint packetssent;

        public hintPacketsSent3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.packetssent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.packetssent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // packetssent
            return boxSize;
        }
    }


    public class hintBytesSent7 : Box
    {
        public override string FourCC { get { return "totl"; } }
        public uint bytessent;

        public hintBytesSent7()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytessent
            return boxSize;
        }
    }


    public class hintBytesSent8 : Box
    {
        public override string FourCC { get { return "npck"; } }
        public uint bytessent;

        public hintBytesSent8()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytessent
            return boxSize;
        }
    }


    public class hintPacketsSent4 : Box
    {
        public override string FourCC { get { return "npck"; } }
        public uint packetssent;

        public hintPacketsSent4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.packetssent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.packetssent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // packetssent
            return boxSize;
        }
    }


    public class hintBytesSent9 : Box
    {
        public override string FourCC { get { return "npck"; } }
        public uint bytessent;

        public hintBytesSent9()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytessent
            return boxSize;
        }
    }


    public class hintBytesSent10 : Box
    {
        public override string FourCC { get { return "tpay"; } }
        public uint bytessent;

        public hintBytesSent10()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytessent
            return boxSize;
        }
    }


    public class hintPacketsSent5 : Box
    {
        public override string FourCC { get { return "tpay"; } }
        public uint packetssent;

        public hintPacketsSent5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.packetssent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.packetssent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // packetssent
            return boxSize;
        }
    }


    public class hintBytesSent11 : Box
    {
        public override string FourCC { get { return "tpay"; } }
        public uint bytessent;

        public hintBytesSent11()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytessent
            return boxSize;
        }
    }


    public class hintmaxrate : Box
    {
        public override string FourCC { get { return "maxr"; } }
        public uint period;  //  in milliseconds
        public uint bytes;

        public hintmaxrate()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /*  maximum data rate */
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.period); // in milliseconds
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytes);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /*  maximum data rate */
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.period); // in milliseconds
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /*  maximum data rate */
            boxSize += 32; // period
            boxSize += 32; // bytes
            return boxSize;
        }
    }


    public class hintmediaBytesSent : Box
    {
        public override string FourCC { get { return "dmed"; } }
        public ulong bytessent;

        public hintmediaBytesSent()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintimmediateBytesSent : Box
    {
        public override string FourCC { get { return "dmed"; } }
        public ulong bytessent;

        public hintimmediateBytesSent()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintrepeatedBytesSent : Box
    {
        public override string FourCC { get { return "dmed"; } }
        public ulong bytessent;

        public hintrepeatedBytesSent()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintmediaBytesSent1 : Box
    {
        public override string FourCC { get { return "dimm"; } }
        public ulong bytessent;

        public hintmediaBytesSent1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintimmediateBytesSent1 : Box
    {
        public override string FourCC { get { return "dimm"; } }
        public ulong bytessent;

        public hintimmediateBytesSent1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintrepeatedBytesSent1 : Box
    {
        public override string FourCC { get { return "dimm"; } }
        public ulong bytessent;

        public hintrepeatedBytesSent1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintmediaBytesSent2 : Box
    {
        public override string FourCC { get { return "drep"; } }
        public ulong bytessent;

        public hintmediaBytesSent2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintimmediateBytesSent2 : Box
    {
        public override string FourCC { get { return "drep"; } }
        public ulong bytessent;

        public hintimmediateBytesSent2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintrepeatedBytesSent2 : Box
    {
        public override string FourCC { get { return "drep"; } }
        public ulong bytessent;

        public hintrepeatedBytesSent2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.bytessent);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // bytessent
            return boxSize;
        }
    }


    public class hintminrelativetime : Box
    {
        public override string FourCC { get { return "tmin"; } }
        public int time;

        public hintminrelativetime()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // time
            return boxSize;
        }
    }


    public class hintmaxrelativetime : Box
    {
        public override string FourCC { get { return "tmin"; } }
        public int time;

        public hintmaxrelativetime()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // time
            return boxSize;
        }
    }


    public class hintminrelativetime1 : Box
    {
        public override string FourCC { get { return "tmax"; } }
        public int time;

        public hintminrelativetime1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // time
            return boxSize;
        }
    }


    public class hintmaxrelativetime1 : Box
    {
        public override string FourCC { get { return "tmax"; } }
        public int time;

        public hintmaxrelativetime1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // time
            return boxSize;
        }
    }


    public class hintlargestpacket : Box
    {
        public override string FourCC { get { return "pmax"; } }
        public uint bytes;

        public hintlargestpacket()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytes);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytes
            return boxSize;
        }
    }


    public class hintlongestpacket : Box
    {
        public override string FourCC { get { return "pmax"; } }
        public uint time;

        public hintlongestpacket()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // time
            return boxSize;
        }
    }


    public class hintlargestpacket1 : Box
    {
        public override string FourCC { get { return "dmax"; } }
        public uint bytes;

        public hintlargestpacket1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bytes);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bytes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bytes
            return boxSize;
        }
    }


    public class hintlongestpacket1 : Box
    {
        public override string FourCC { get { return "dmax"; } }
        public uint time;

        public hintlongestpacket1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // time
            return boxSize;
        }
    }


    public class hintpayloadID : Box
    {
        public override string FourCC { get { return "payt"; } }
        public uint payloadID;  //  payload ID used in RTP packets
        public byte count;
        public sbyte[] rtpmap_string;

        public hintpayloadID()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.payloadID); // payload ID used in RTP packets
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.count);
            boxSize += IsoReaderWriter.ReadInt8Array(stream, count, out this.rtpmap_string);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.payloadID); // payload ID used in RTP packets
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.count);
            boxSize += IsoReaderWriter.WriteInt8Array(stream, count, this.rtpmap_string);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // payloadID
            boxSize += 8; // count
            boxSize += (ulong)count * 8; // rtpmap_string
            return boxSize;
        }
    }


    public class StereoVideoBox : FullBox
    {
        public override string FourCC { get { return "stvi"; } }
        public uint reserved = 0;
        public byte single_view_allowed;
        public uint stereo_scheme;
        public uint length;
        public byte[] stereo_indication_type;
        public Box[] any_box;  //  optional

        public StereoVideoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 30, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.single_view_allowed);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.stereo_scheme);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.length);
            boxSize += IsoReaderWriter.ReadBytes(stream, length, out this.stereo_indication_type);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBoxes(stream, out this.any_box); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 30, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.single_view_allowed);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.stereo_scheme);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.length);
            boxSize += IsoReaderWriter.WriteBytes(stream, length, this.stereo_indication_type);
            if (this.any_box != null) boxSize += IsoReaderWriter.WriteBoxes(stream, this.any_box); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 30; // reserved
            boxSize += 2; // single_view_allowed
            boxSize += 32; // stereo_scheme
            boxSize += 32; // length
            boxSize += (ulong)length * 8; // stereo_indication_type
            if (this.any_box != null) boxSize += IsoReaderWriter.CalculateSize(any_box); // any_box
            return boxSize;
        }
    }


    public class ExtendedLanguageBox : FullBox
    {
        public override string FourCC { get { return "elng"; } }
        public string extended_language;

        public ExtendedLanguageBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.extended_language);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.extended_language);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)extended_language.Length * 8; // extended_language
            return boxSize;
        }
    }


    public class BitRateBox : Box
    {
        public override string FourCC { get { return "btrt"; } }
        public uint bufferSizeDB;
        public uint maxBitrate;
        public uint avgBitrate;

        public BitRateBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.bufferSizeDB);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxBitrate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.avgBitrate);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.bufferSizeDB);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxBitrate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.avgBitrate);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // bufferSizeDB
            boxSize += 32; // maxBitrate
            boxSize += 32; // avgBitrate
            return boxSize;
        }
    }


    public class PixelAspectRatioBox : Box
    {
        public override string FourCC { get { return "pasp"; } }
        public uint hSpacing;
        public uint vSpacing;

        public PixelAspectRatioBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.hSpacing);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vSpacing);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.hSpacing);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vSpacing);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // hSpacing
            boxSize += 32; // vSpacing
            return boxSize;
        }
    }


    public class CleanApertureBox : Box
    {
        public override string FourCC { get { return "clap"; } }
        public uint cleanApertureWidthN;
        public uint cleanApertureWidthD;
        public uint cleanApertureHeightN;
        public uint cleanApertureHeightD;
        public uint horizOffN;
        public uint horizOffD;
        public uint vertOffN;
        public uint vertOffD;

        public CleanApertureBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureWidthN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureWidthD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureHeightN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureHeightD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizOffN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizOffD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertOffN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertOffD);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizOffN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizOffD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertOffN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertOffD);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // cleanApertureWidthN
            boxSize += 32; // cleanApertureWidthD
            boxSize += 32; // cleanApertureHeightN
            boxSize += 32; // cleanApertureHeightD
            boxSize += 32; // horizOffN
            boxSize += 32; // horizOffD
            boxSize += 32; // vertOffN
            boxSize += 32; // vertOffD
            return boxSize;
        }
    }


    public class ContentColourVolumeBox : Box
    {
        public override string FourCC { get { return "cclv"; } }
        public bool reserved1 = false;  //  ccv_cancel_flag
        public bool reserved2 = false;  //  ccv_persistence_flag
        public bool ccv_primaries_present_flag;
        public bool ccv_min_luminance_value_present_flag;
        public bool ccv_max_luminance_value_present_flag;
        public bool ccv_avg_luminance_value_present_flag;
        public byte ccv_reserved_zero_2bits = 0;
        public int[] ccv_primaries_x;
        public int[] ccv_primaries_y;
        public uint ccv_min_luminance_value;
        public uint ccv_max_luminance_value;
        public uint ccv_avg_luminance_value;

        public ContentColourVolumeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved1); // ccv_cancel_flag
            boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved2); // ccv_persistence_flag
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_primaries_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_min_luminance_value_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_max_luminance_value_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_avg_luminance_value_present_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.ccv_reserved_zero_2bits);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.ccv_primaries_x[c]);
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.ccv_primaries_y[c]);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ccv_min_luminance_value);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ccv_max_luminance_value);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ccv_avg_luminance_value);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.reserved1); // ccv_cancel_flag
            boxSize += IsoReaderWriter.WriteBit(stream, this.reserved2); // ccv_persistence_flag
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_primaries_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_min_luminance_value_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_max_luminance_value_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_avg_luminance_value_present_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.ccv_reserved_zero_2bits);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_x[c]);
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_y[c]);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.ccv_min_luminance_value);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.ccv_max_luminance_value);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.ccv_avg_luminance_value);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // reserved1
            boxSize += 1; // reserved2
            boxSize += 1; // ccv_primaries_present_flag
            boxSize += 1; // ccv_min_luminance_value_present_flag
            boxSize += 1; // ccv_max_luminance_value_present_flag
            boxSize += 1; // ccv_avg_luminance_value_present_flag
            boxSize += 2; // ccv_reserved_zero_2bits

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    boxSize += 32; // ccv_primaries_x
                    boxSize += 32; // ccv_primaries_y
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                boxSize += 32; // ccv_min_luminance_value
            }

            if (ccv_max_luminance_value_present_flag)
            {
                boxSize += 32; // ccv_max_luminance_value
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                boxSize += 32; // ccv_avg_luminance_value
            }
            return boxSize;
        }
    }


    public class ColourInformationBox : Box
    {
        public override string FourCC { get { return "colr"; } }
        public uint colour_type;
        public ushort colour_primaries;
        public ushort transfer_characteristics;
        public ushort matrix_coefficients;
        public bool full_range_flag;
        public byte reserved = 0;
        public ICC_profile ICC_profile;  //  restricted ICC profile
        public ICC_profile ICC_profile0;  //  unrestricted ICC profile

        public ColourInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.colour_type);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.colour_primaries);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.transfer_characteristics);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.matrix_coefficients);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.full_range_flag);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.ICC_profile); // restricted ICC profile
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.ICC_profile0); // unrestricted ICC profile
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.colour_type);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.colour_primaries);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.transfer_characteristics);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.matrix_coefficients);
                boxSize += IsoReaderWriter.WriteBit(stream, this.full_range_flag);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.ICC_profile); // restricted ICC profile
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.ICC_profile0); // unrestricted ICC profile
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // colour_type

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                boxSize += 16; // colour_primaries
                boxSize += 16; // transfer_characteristics
                boxSize += 16; // matrix_coefficients
                boxSize += 1; // full_range_flag
                boxSize += 7; // reserved
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                boxSize += IsoReaderWriter.CalculateClassSize(ICC_profile); // ICC_profile
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                boxSize += IsoReaderWriter.CalculateClassSize(ICC_profile0); // ICC_profile0
            }
            return boxSize;
        }
    }


    public class ContentLightLevelBox : Box
    {
        public override string FourCC { get { return "clli"; } }
        public ushort max_content_light_level;
        public ushort max_pic_average_light_level;

        public ContentLightLevelBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_content_light_level);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_pic_average_light_level);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_content_light_level);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_pic_average_light_level);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // max_content_light_level
            boxSize += 16; // max_pic_average_light_level
            return boxSize;
        }
    }


    public class MasteringDisplayColourVolumeBox : Box
    {
        public override string FourCC { get { return "mdcv"; } }
        public ushort display_primaries_x;
        public ushort display_primaries_y;
        public ushort white_point_x;
        public ushort white_point_y;
        public uint max_display_mastering_luminance;
        public uint min_display_mastering_luminance;

        public MasteringDisplayColourVolumeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.display_primaries_x);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.display_primaries_y);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.white_point_x);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.white_point_y);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.max_display_mastering_luminance);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.min_display_mastering_luminance);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_x);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_y);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.white_point_x);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.white_point_y);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.max_display_mastering_luminance);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.min_display_mastering_luminance);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            for (int c = 0; c < 3; c++)
            {
                boxSize += 16; // display_primaries_x
                boxSize += 16; // display_primaries_y
            }
            boxSize += 16; // white_point_x
            boxSize += 16; // white_point_y
            boxSize += 32; // max_display_mastering_luminance
            boxSize += 32; // min_display_mastering_luminance
            return boxSize;
        }
    }


    public class ScrambleSchemeInfoBox : Box
    {
        public override string FourCC { get { return "scrb"; } }
        public SchemeTypeBox scheme_type_box;
        public SchemeInformationBox info;  //  optional

        public ScrambleSchemeInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.scheme_type_box);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.info); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            if (this.info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.info); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(scheme_type_box); // scheme_type_box
            if (this.info != null) boxSize += IsoReaderWriter.CalculateSize(info); // info
            return boxSize;
        }
    }


    public class ChannelLayout : FullBox
    {
        public override string FourCC { get { return "chnl"; } }
        public byte stream_structure;
        public byte definedLayout;
        public byte speaker_position;
        public short azimuth;
        public sbyte elevation;
        public ulong omittedChannelsMap;  //  a ‘1’ bit indicates ‘not in this track’
        public byte object_count;
        public byte stream_structure0;
        public byte format_ordering;
        public byte baseChannelCount;
        public byte definedLayout0;
        public byte layout_channel_count;
        public byte speaker_position0;
        public short azimuth0;
        public sbyte elevation0;
        public byte reserved = 0;
        public byte channel_order_definition;
        public bool omitted_channels_present;
        public ulong omittedChannelsMap0;  //  a ‘1’ bit indicates ‘not in this track’

        public ChannelLayout()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.stream_structure);

                if ((stream_structure & 1) == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt8(stream, out this.definedLayout);

                    if (definedLayout == 0)
                    {

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            /*   layout_channel_count comes from the sample entry */
                            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.speaker_position);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                boxSize += IsoReaderWriter.ReadInt16(stream, out this.azimuth);
                                boxSize += IsoReaderWriter.ReadInt8(stream, out this.elevation);
                            }
                        }
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.ReadUInt64(stream, out this.omittedChannelsMap); // a ‘1’ bit indicates ‘not in this track’
                    }
                }

                if ((stream_structure & 2) == 2)
                {
                    boxSize += IsoReaderWriter.ReadUInt8(stream, out this.object_count);
                }
            }

            else
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.stream_structure0);
                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.format_ordering);
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.baseChannelCount);

                if ((stream_structure & 1) == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt8(stream, out this.definedLayout0);

                    if (definedLayout == 0)
                    {
                        boxSize += IsoReaderWriter.ReadUInt8(stream, out this.layout_channel_count);

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.speaker_position0);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                boxSize += IsoReaderWriter.ReadInt16(stream, out this.azimuth0);
                                boxSize += IsoReaderWriter.ReadInt8(stream, out this.elevation0);
                            }
                        }
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved);
                        boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.channel_order_definition);
                        boxSize += IsoReaderWriter.ReadBit(stream, out this.omitted_channels_present);

                        if (omitted_channels_present == true)
                        {
                            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.omittedChannelsMap0); // a ‘1’ bit indicates ‘not in this track’
                        }
                    }
                }

                if ((stream_structure & 2) == 2)
                {
                    /*  object_count is derived from baseChannelCount */
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.stream_structure);

                if ((stream_structure & 1) == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt8(stream, this.definedLayout);

                    if (definedLayout == 0)
                    {

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            /*   layout_channel_count comes from the sample entry */
                            boxSize += IsoReaderWriter.WriteUInt8(stream, this.speaker_position);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                boxSize += IsoReaderWriter.WriteInt16(stream, this.azimuth);
                                boxSize += IsoReaderWriter.WriteInt8(stream, this.elevation);
                            }
                        }
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.WriteUInt64(stream, this.omittedChannelsMap); // a ‘1’ bit indicates ‘not in this track’
                    }
                }

                if ((stream_structure & 2) == 2)
                {
                    boxSize += IsoReaderWriter.WriteUInt8(stream, this.object_count);
                }
            }

            else
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.stream_structure0);
                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.format_ordering);
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.baseChannelCount);

                if ((stream_structure & 1) == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt8(stream, this.definedLayout0);

                    if (definedLayout == 0)
                    {
                        boxSize += IsoReaderWriter.WriteUInt8(stream, this.layout_channel_count);

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            boxSize += IsoReaderWriter.WriteUInt8(stream, this.speaker_position0);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                boxSize += IsoReaderWriter.WriteInt16(stream, this.azimuth0);
                                boxSize += IsoReaderWriter.WriteInt8(stream, this.elevation0);
                            }
                        }
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
                        boxSize += IsoReaderWriter.WriteBits(stream, 3, this.channel_order_definition);
                        boxSize += IsoReaderWriter.WriteBit(stream, this.omitted_channels_present);

                        if (omitted_channels_present == true)
                        {
                            boxSize += IsoReaderWriter.WriteUInt64(stream, this.omittedChannelsMap0); // a ‘1’ bit indicates ‘not in this track’
                        }
                    }
                }

                if ((stream_structure & 2) == 2)
                {
                    /*  object_count is derived from baseChannelCount */
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 8; // stream_structure

                if ((stream_structure & 1) == 1)
                {
                    boxSize += 8; // definedLayout

                    if (definedLayout == 0)
                    {

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            /*   layout_channel_count comes from the sample entry */
                            boxSize += 8; // speaker_position

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                boxSize += 16; // azimuth
                                boxSize += 8; // elevation
                            }
                        }
                    }

                    else
                    {
                        boxSize += 64; // omittedChannelsMap
                    }
                }

                if ((stream_structure & 2) == 2)
                {
                    boxSize += 8; // object_count
                }
            }

            else
            {
                boxSize += 4; // stream_structure0
                boxSize += 4; // format_ordering
                boxSize += 8; // baseChannelCount

                if ((stream_structure & 1) == 1)
                {
                    boxSize += 8; // definedLayout0

                    if (definedLayout == 0)
                    {
                        boxSize += 8; // layout_channel_count

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            boxSize += 8; // speaker_position0

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                boxSize += 16; // azimuth0
                                boxSize += 8; // elevation0
                            }
                        }
                    }

                    else
                    {
                        boxSize += 4; // reserved
                        boxSize += 3; // channel_order_definition
                        boxSize += 1; // omitted_channels_present

                        if (omitted_channels_present == true)
                        {
                            boxSize += 64; // omittedChannelsMap0
                        }
                    }
                }

                if ((stream_structure & 2) == 2)
                {
                    /*  object_count is derived from baseChannelCount */
                }
            }
            return boxSize;
        }
    }


    public class DownMixInstructions : FullBox
    {
        public override string FourCC { get { return "dmix"; } }
        public bool reserved = false;
        public byte downmix_instructions_count;
        public int downmix_instructions_count0 = 1;
        public byte targetLayout;
        public bool reserved0 = false;
        public byte targetChannelCount;
        public bool in_stream;
        public byte downmix_ID;
        public byte bs_downmix_offset;
        public byte bs_downmix_coefficient_v1;
        public byte[] reserved00 = [];  //  byte align
        public byte bs_downmix_coefficient;

        public DownMixInstructions()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            int baseChannelCount = 0; // TODO: get somewhere

            if (version >= 1)
            {
                boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.downmix_instructions_count);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.downmix_instructions_count0);
            }

            for (int a = 1; a <= downmix_instructions_count; a++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.targetLayout);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved0);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.targetChannelCount);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.in_stream);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.downmix_ID);

                if (in_stream == false)
                {
                    /*  downmix coefficients are out of stream and supplied here */


                    if (version >= 1)
                    {
                        boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.bs_downmix_offset);
                        int size = 4;

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                boxSize += IsoReaderWriter.ReadBits(stream, 5, out this.bs_downmix_coefficient_v1);
                                size += 5;
                            }
                        }
                        boxSize += IsoReaderWriter.ReadBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size), out this.reserved00); // byte align
                    }

                    else
                    {

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.bs_downmix_coefficient);
                            }
                        }
                    }
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            int baseChannelCount = 0; // TODO: get somewhere

            if (version >= 1)
            {
                boxSize += IsoReaderWriter.WriteBit(stream, this.reserved);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.downmix_instructions_count);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteInt32(stream, this.downmix_instructions_count0);
            }

            for (int a = 1; a <= downmix_instructions_count; a++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.targetLayout);
                boxSize += IsoReaderWriter.WriteBit(stream, this.reserved0);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.targetChannelCount);
                boxSize += IsoReaderWriter.WriteBit(stream, this.in_stream);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.downmix_ID);

                if (in_stream == false)
                {
                    /*  downmix coefficients are out of stream and supplied here */


                    if (version >= 1)
                    {
                        boxSize += IsoReaderWriter.WriteBits(stream, 4, this.bs_downmix_offset);
                        int size = 4;

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                boxSize += IsoReaderWriter.WriteBits(stream, 5, this.bs_downmix_coefficient_v1);
                                size += 5;
                            }
                        }
                        boxSize += IsoReaderWriter.WriteBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size), this.reserved00); // byte align
                    }

                    else
                    {

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.bs_downmix_coefficient);
                            }
                        }
                    }
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            int baseChannelCount = 0; // TODO: get somewhere

            if (version >= 1)
            {
                boxSize += 1; // reserved
                boxSize += 7; // downmix_instructions_count
            }

            else
            {
                boxSize += 32; // downmix_instructions_count0
            }

            for (int a = 1; a <= downmix_instructions_count; a++)
            {
                boxSize += 8; // targetLayout
                boxSize += 1; // reserved0
                boxSize += 7; // targetChannelCount
                boxSize += 1; // in_stream
                boxSize += 7; // downmix_ID

                if (in_stream == false)
                {
                    /*  downmix coefficients are out of stream and supplied here */


                    if (version >= 1)
                    {
                        boxSize += 4; // bs_downmix_offset
                        int size = 4;

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                boxSize += 5; // bs_downmix_coefficient_v1
                                size += 5;
                            }
                        }
                        boxSize += (ulong)(Math.Ceiling(size / 8d) - size) * 8; // reserved00
                    }

                    else
                    {

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                boxSize += 4; // bs_downmix_coefficient
                            }
                        }
                    }
                }
            }
            return boxSize;
        }
    }


    public class SamplingRateBox : FullBox
    {
        public override string FourCC { get { return "srat"; } }
        public uint sampling_rate;

        public SamplingRateBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sampling_rate);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sampling_rate);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // sampling_rate
            return boxSize;
        }
    }


    public class TextConfigBox : FullBox
    {
        public override string FourCC { get { return "txtC"; } }
        public string text_config;

        public TextConfigBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.text_config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.text_config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)text_config.Length * 8; // text_config
            return boxSize;
        }
    }


    public class URIInitBox : FullBox
    {
        public override string FourCC { get { return "uriI"; } }
        public byte[] uri_initialization_data;

        public URIInitBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.uri_initialization_data);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.uri_initialization_data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)uri_initialization_data.Length * 8; // uri_initialization_data
            return boxSize;
        }
    }


    public class CopyrightBox : FullBox
    {
        public override string FourCC { get { return "cprt"; } }
        public bool pad = false;
        public byte[] language;  //  ISO-639-2/T language code
        public string notice;

        public CopyrightBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.pad);
            boxSize += IsoReaderWriter.ReadBitsArray(stream, 5, 3, out this.language); // ISO-639-2/T language code
            boxSize += IsoReaderWriter.ReadString(stream, out this.notice);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.pad);
            boxSize += IsoReaderWriter.WriteBitsArray(stream, 5, 3, this.language); // ISO-639-2/T language code
            boxSize += IsoReaderWriter.WriteString(stream, this.notice);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // pad
            boxSize += 3 * 5; // language
            boxSize += (ulong)notice.Length * 8; // notice
            return boxSize;
        }
    }


    public class KindBox : FullBox
    {
        public override string FourCC { get { return "kind"; } }
        public string schemeURI;
        public string value;

        public KindBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.schemeURI);
            boxSize += IsoReaderWriter.ReadString(stream, out this.value);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.schemeURI);
            boxSize += IsoReaderWriter.WriteString(stream, this.value);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)schemeURI.Length * 8; // schemeURI
            boxSize += (ulong)value.Length * 8; // value
            return boxSize;
        }
    }


    public class TrackSelectionBox : FullBox
    {
        public override string FourCC { get { return "tsel"; } }
        public int switch_group = 0;
        public uint[] attribute_list;  //  to end of the box

        public TrackSelectionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.switch_group);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.attribute_list); // to end of the box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.switch_group);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.attribute_list); // to end of the box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // switch_group
            boxSize += 32; // attribute_list
            return boxSize;
        }
    }


    public class SubTrackBox : Box
    {
        public override string FourCC { get { return "strk"; } }

        public SubTrackBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class trackhintinformation : Box
    {
        public override string FourCC { get { return "hnti"; } }

        public trackhintinformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class rtptracksdphintinformation : Box
    {
        public override string FourCC { get { return "hnti"; } }
        public sbyte[] sdptext;

        public rtptracksdphintinformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8Array(stream, out this.sdptext);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)sdptext.Length * 8; // sdptext
            return boxSize;
        }
    }


    public class trackhintinformation1 : Box
    {
        public override string FourCC { get { return "sdp "; } }

        public trackhintinformation1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class rtptracksdphintinformation1 : Box
    {
        public override string FourCC { get { return "sdp "; } }
        public sbyte[] sdptext;

        public rtptracksdphintinformation1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8Array(stream, out this.sdptext);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)sdptext.Length * 8; // sdptext
            return boxSize;
        }
    }


    public class moviehintinformation : Box
    {
        public override string FourCC { get { return "rtp "; } }

        public moviehintinformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class rtpmoviehintinformation : Box
    {
        public override string FourCC { get { return "rtp "; } }
        public uint descriptionformat = IsoReaderWriter.FromFourCC("sdp ");
        public sbyte[] sdptext;

        public rtpmoviehintinformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.descriptionformat);
            boxSize += IsoReaderWriter.ReadInt8Array(stream, out this.sdptext);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.descriptionformat);
            boxSize += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // descriptionformat
            boxSize += (ulong)sdptext.Length * 8; // sdptext
            return boxSize;
        }
    }


    public class hintstatisticsbox : Box
    {
        public override string FourCC { get { return "hinf"; } }

        public hintstatisticsbox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class LoudnessBox : Box
    {
        public override string FourCC { get { return "ludt"; } }
        public int[] TrackLoudnessInfo;  //  not more than one AlbumLoudnessInfo box with version>=1 is allowed	albumLoudness	AlbumLoudnessInfo[];

        public LoudnessBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
            boxSize += IsoReaderWriter.ReadInt32Array(stream, out this.TrackLoudnessInfo); // not more than one AlbumLoudnessInfo box with version>=1 is allowed	albumLoudness	AlbumLoudnessInfo[];
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
            boxSize += IsoReaderWriter.WriteInt32Array(stream, this.TrackLoudnessInfo); // not more than one AlbumLoudnessInfo box with version>=1 is allowed	albumLoudness	AlbumLoudnessInfo[];
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
            boxSize += (ulong)TrackLoudnessInfo.Length * 32; // TrackLoudnessInfo
            return boxSize;
        }
    }


    public class TrackLoudnessInfo : LoudnessBaseBox
    {
        public override string FourCC { get { return "tlou"; } }

        public TrackLoudnessInfo()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class AlbumLoudnessInfo : LoudnessBaseBox
    {
        public override string FourCC { get { return "alou"; } }

        public AlbumLoudnessInfo()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class DataEntryUrlBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "url "; } }
        public string location;

        public DataEntryUrlBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.location);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.location);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)location.Length * 8; // location
            return boxSize;
        }
    }


    public class DataEntryUrnBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "urn "; } }
        public string name;
        public string location;

        public DataEntryUrnBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.name);
            boxSize += IsoReaderWriter.ReadString(stream, out this.location);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.name);
            boxSize += IsoReaderWriter.WriteString(stream, this.location);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)name.Length * 8; // name
            boxSize += (ulong)location.Length * 8; // location
            return boxSize;
        }
    }


    public class DataEntryImdaBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "imdt"; } }
        public uint imda_ref_identifier;

        public DataEntryImdaBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.imda_ref_identifier);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.imda_ref_identifier);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // imda_ref_identifier
            return boxSize;
        }
    }


    public class DataEntrySeqNumImdaBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "snim"; } }

        public DataEntrySeqNumImdaBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class ItemPropertyContainerBox : Box
    {
        public override string FourCC { get { return "ipco"; } }
        public Box[] properties;  //  boxes derived from

        public ItemPropertyContainerBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.properties); // boxes derived from
            /*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
            /*  to fill the box */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.properties); // boxes derived from
            /*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
            /*  to fill the box */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(properties); // properties
            /*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
            /*  to fill the box */
            return boxSize;
        }
    }


    public class ItemPropertyAssociationBox : FullBox
    {
        public override string FourCC { get { return "ipma"; } }
        public uint entry_count;
        public ushort item_ID;
        public uint item_ID0;
        public byte association_count;
        public bool essential;
        public ushort property_index;
        public byte property_index0;

        public ItemPropertyAssociationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 0; i < entry_count; i++)
            {

                if (version < 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
                }

                else
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID0);
                }
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.association_count);

                for (int j = 0; j < association_count; j++)
                {
                    boxSize += IsoReaderWriter.ReadBit(stream, out this.essential);

                    if ((flags & 1) == 1)
                    {
                        boxSize += IsoReaderWriter.ReadBits(stream, 15, out this.property_index);
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.property_index0);
                    }
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 0; i < entry_count; i++)
            {

                if (version < 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID0);
                }
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.association_count);

                for (int j = 0; j < association_count; j++)
                {
                    boxSize += IsoReaderWriter.WriteBit(stream, this.essential);

                    if ((flags & 1) == 1)
                    {
                        boxSize += IsoReaderWriter.WriteBits(stream, 15, this.property_index);
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.WriteBits(stream, 7, this.property_index0);
                    }
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count

            for (int i = 0; i < entry_count; i++)
            {

                if (version < 1)
                {
                    boxSize += 16; // item_ID
                }

                else
                {
                    boxSize += 32; // item_ID0
                }
                boxSize += 8; // association_count

                for (int j = 0; j < association_count; j++)
                {
                    boxSize += 1; // essential

                    if ((flags & 1) == 1)
                    {
                        boxSize += 15; // property_index
                    }

                    else
                    {
                        boxSize += 7; // property_index0
                    }
                }
            }
            return boxSize;
        }
    }


    public class ItemPropertiesBox : Box
    {
        public override string FourCC { get { return "iprp"; } }
        public ItemPropertyContainerBox property_container;
        public ItemPropertyAssociationBox[] association;

        public ItemPropertiesBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.property_container);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.association);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.property_container);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.association);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(property_container); // property_container
            boxSize += IsoReaderWriter.CalculateSize(association); // association
            return boxSize;
        }
    }


    public class AlternativeStartupSequencePropertiesBox : FullBox
    {
        public override string FourCC { get { return "assp"; } }
        public int min_initial_alt_startup_offset;
        public uint num_entries;
        public uint grouping_type_parameter;
        public int min_initial_alt_startup_offset0;

        public AlternativeStartupSequencePropertiesBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.min_initial_alt_startup_offset);
            }

            else if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entries);

                for (int j = 1; j <= num_entries; j++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type_parameter);
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.min_initial_alt_startup_offset0);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteInt32(stream, this.min_initial_alt_startup_offset);
            }

            else if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

                for (int j = 1; j <= num_entries; j++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.min_initial_alt_startup_offset0);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 32; // min_initial_alt_startup_offset
            }

            else if (version == 1)
            {
                boxSize += 32; // num_entries

                for (int j = 1; j <= num_entries; j++)
                {
                    boxSize += 32; // grouping_type_parameter
                    boxSize += 32; // min_initial_alt_startup_offset0
                }
            }
            return boxSize;
        }
    }


    public class BinaryXMLBox : FullBox
    {
        public override string FourCC { get { return "bxml"; } }
        public byte[] data;  //  to end of box

        public BinaryXMLBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.data); // to end of box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.data); // to end of box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)data.Length * 8; // data
            return boxSize;
        }
    }


    public class CompleteTrackInfoBox : Box
    {
        public override string FourCC { get { return "cinf"; } }
        public OriginalFormatBox original_format;

        public CompleteTrackInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.original_format);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.original_format);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(original_format); // original_format
            return boxSize;
        }
    }


    public class ChunkLargeOffsetBox : FullBox
    {
        public override string FourCC { get { return "co64"; } }
        public uint entry_count;
        public ulong chunk_offset;

        public ChunkLargeOffsetBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.chunk_offset);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.chunk_offset);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 64; // chunk_offset
            }
            return boxSize;
        }
    }


    public class CompactSampleToGroupBox : FullBox
    {
        public override string FourCC { get { return "csgp"; } }
        public uint grouping_type;
        public uint grouping_type_parameter;
        public uint pattern_count;
        public byte[] pattern_length;
        public byte[] sample_count;
        public byte[][] sample_group_description_index;  //  whose msb might indicate fragment_local or global

        public CompactSampleToGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            bool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);
            int count_size_code = (flags >> 2) & 0x3;
            int pattern_size_code = (flags >> 4) & 0x3;
            int index_size_code = flags & 0x3;

            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type);

            if (grouping_type_parameter_present == true)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type_parameter);
            }
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.pattern_count);
            uint totalPatternLength = 0;

            for (int i = 1; i <= pattern_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, pattern_size_code, out this.pattern_length[i]);
                boxSize += IsoReaderWriter.ReadBits(stream, count_size_code, out this.sample_count[i]);
            }

            for (int j = 1; j <= pattern_count; j++)
            {

                for (int k = 1; k <= pattern_length[j]; k++)
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, index_size_code, out this.sample_group_description_index[j][k]); // whose msb might indicate fragment_local or global
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            bool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);
            int count_size_code = (flags >> 2) & 0x3;
            int pattern_size_code = (flags >> 4) & 0x3;
            int index_size_code = flags & 0x3;

            boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

            if (grouping_type_parameter_present == true)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
            }
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.pattern_count);
            uint totalPatternLength = 0;

            for (int i = 1; i <= pattern_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, pattern_size_code, this.pattern_length[i]);
                boxSize += IsoReaderWriter.WriteBits(stream, count_size_code, this.sample_count[i]);
            }

            for (int j = 1; j <= pattern_count; j++)
            {

                for (int k = 1; k <= pattern_length[j]; k++)
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, index_size_code, this.sample_group_description_index[j][k]); // whose msb might indicate fragment_local or global
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            bool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);
            int count_size_code = (flags >> 2) & 0x3;
            int pattern_size_code = (flags >> 4) & 0x3;
            int index_size_code = flags & 0x3;

            boxSize += 32; // grouping_type

            if (grouping_type_parameter_present == true)
            {
                boxSize += 32; // grouping_type_parameter
            }
            boxSize += 32; // pattern_count
            uint totalPatternLength = 0;

            for (int i = 1; i <= pattern_count; i++)
            {
                boxSize += (ulong)pattern_size_code; // pattern_length
                boxSize += (ulong)count_size_code; // sample_count
            }

            for (int j = 1; j <= pattern_count; j++)
            {

                for (int k = 1; k <= pattern_length[j]; k++)
                {
                    boxSize += (ulong)index_size_code; // sample_group_description_index
                }
            }
            return boxSize;
        }
    }


    public class CompositionToDecodeBox : FullBox
    {
        public override string FourCC { get { return "cslg"; } }
        public int compositionToDTSShift;
        public int leastDecodeToDisplayDelta;
        public int greatestDecodeToDisplayDelta;
        public int compositionStartTime;
        public int compositionEndTime;
        public long compositionToDTSShift0;
        public long leastDecodeToDisplayDelta0;
        public long greatestDecodeToDisplayDelta0;
        public long compositionStartTime0;
        public long compositionEndTime0;

        public CompositionToDecodeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.compositionToDTSShift);
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.leastDecodeToDisplayDelta);
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.greatestDecodeToDisplayDelta);
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.compositionStartTime);
                boxSize += IsoReaderWriter.ReadInt32(stream, out this.compositionEndTime);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadInt64(stream, out this.compositionToDTSShift0);
                boxSize += IsoReaderWriter.ReadInt64(stream, out this.leastDecodeToDisplayDelta0);
                boxSize += IsoReaderWriter.ReadInt64(stream, out this.greatestDecodeToDisplayDelta0);
                boxSize += IsoReaderWriter.ReadInt64(stream, out this.compositionStartTime0);
                boxSize += IsoReaderWriter.ReadInt64(stream, out this.compositionEndTime0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteInt32(stream, this.compositionToDTSShift);
                boxSize += IsoReaderWriter.WriteInt32(stream, this.leastDecodeToDisplayDelta);
                boxSize += IsoReaderWriter.WriteInt32(stream, this.greatestDecodeToDisplayDelta);
                boxSize += IsoReaderWriter.WriteInt32(stream, this.compositionStartTime);
                boxSize += IsoReaderWriter.WriteInt32(stream, this.compositionEndTime);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteInt64(stream, this.compositionToDTSShift0);
                boxSize += IsoReaderWriter.WriteInt64(stream, this.leastDecodeToDisplayDelta0);
                boxSize += IsoReaderWriter.WriteInt64(stream, this.greatestDecodeToDisplayDelta0);
                boxSize += IsoReaderWriter.WriteInt64(stream, this.compositionStartTime0);
                boxSize += IsoReaderWriter.WriteInt64(stream, this.compositionEndTime0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 32; // compositionToDTSShift
                boxSize += 32; // leastDecodeToDisplayDelta
                boxSize += 32; // greatestDecodeToDisplayDelta
                boxSize += 32; // compositionStartTime
                boxSize += 32; // compositionEndTime
            }

            else
            {
                boxSize += 64; // compositionToDTSShift0
                boxSize += 64; // leastDecodeToDisplayDelta0
                boxSize += 64; // greatestDecodeToDisplayDelta0
                boxSize += 64; // compositionStartTime0
                boxSize += 64; // compositionEndTime0
            }
            return boxSize;
        }
    }


    public class CompositionOffsetBox : FullBox
    {
        public override string FourCC { get { return "ctts"; } }
        public uint entry_count;
        public uint sample_count;
        public uint sample_offset;
        public uint sample_count0;
        public int sample_offset0;

        public CompositionOffsetBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            if (version == 0)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_offset);
                }
            }

            else if (version == 1)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count0);
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.sample_offset0);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            if (version == 0)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_offset);
                }
            }

            else if (version == 1)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count0);
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.sample_offset0);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count


            if (version == 0)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    boxSize += 32; // sample_count
                    boxSize += 32; // sample_offset
                }
            }

            else if (version == 1)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    boxSize += 32; // sample_count0
                    boxSize += 32; // sample_offset0
                }
            }
            return boxSize;
        }
    }


    public class DataInformationBox : Box
    {
        public override string FourCC { get { return "dinf"; } }

        public DataInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class DataReferenceBox : FullBox
    {
        public override string FourCC { get { return "dref"; } }
        public uint entry_count;
        public DataEntryBaseBox data_entry;

        public DataReferenceBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBox(stream, out this.data_entry);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBox(stream, this.data_entry);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.CalculateSize(data_entry); // data_entry
            }
            return boxSize;
        }
    }


    public class EditBox : Box
    {
        public override string FourCC { get { return "edts"; } }

        public EditBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class EditListBox : FullBox
    {
        public override string FourCC { get { return "elst"; } }
        public uint entry_count;
        public ulong edit_duration;
        public long media_time;
        public uint edit_duration0;
        public int media_time0;
        public short media_rate_integer;
        public short media_rate_fraction;

        public EditListBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt64(stream, out this.edit_duration);
                    boxSize += IsoReaderWriter.ReadInt64(stream, out this.media_time);
                }

                else
                {
                    /*  version==0 */
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.edit_duration0);
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.media_time0);
                }
                boxSize += IsoReaderWriter.ReadInt16(stream, out this.media_rate_integer);
                boxSize += IsoReaderWriter.ReadInt16(stream, out this.media_rate_fraction);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt64(stream, this.edit_duration);
                    boxSize += IsoReaderWriter.WriteInt64(stream, this.media_time);
                }

                else
                {
                    /*  version==0 */
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.edit_duration0);
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.media_time0);
                }
                boxSize += IsoReaderWriter.WriteInt16(stream, this.media_rate_integer);
                boxSize += IsoReaderWriter.WriteInt16(stream, this.media_rate_fraction);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 1)
                {
                    boxSize += 64; // edit_duration
                    boxSize += 64; // media_time
                }

                else
                {
                    /*  version==0 */
                    boxSize += 32; // edit_duration0
                    boxSize += 32; // media_time0
                }
                boxSize += 16; // media_rate_integer
                boxSize += 16; // media_rate_fraction
            }
            return boxSize;
        }
    }


    public class ExtendedTypeBox : Box
    {
        public override string FourCC { get { return "etyp"; } }
        public TypeCombinationBox[] compatible_combinations;  //  to end of the box

        public ExtendedTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.compatible_combinations); // to end of the box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.compatible_combinations); // to end of the box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(compatible_combinations); // compatible_combinations
            return boxSize;
        }
    }


    public class FDItemInfoExtension : ItemInfoExtension
    {
        public override string FourCC { get { return "fdel"; } }
        public string content_location;
        public string content_MD5;
        public ulong content_length;
        public ulong transfer_length;
        public byte entry_count;
        public uint group_id;

        public FDItemInfoExtension()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.content_location);
            boxSize += IsoReaderWriter.ReadString(stream, out this.content_MD5);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.content_length);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.transfer_length);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.content_location);
            boxSize += IsoReaderWriter.WriteString(stream, this.content_MD5);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.content_length);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.transfer_length);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)content_location.Length * 8; // content_location
            boxSize += (ulong)content_MD5.Length * 8; // content_MD5
            boxSize += 64; // content_length
            boxSize += 64; // transfer_length
            boxSize += 8; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 32; // group_id
            }
            return boxSize;
        }
    }


    public class FECReservoirBox : FullBox
    {
        public override string FourCC { get { return "fecr"; } }
        public ushort entry_count;
        public uint entry_count0;
        public ushort item_ID;
        public uint item_ID0;
        public uint symbol_count;

        public FECReservoirBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
                }

                else
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID0);
                }
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.symbol_count);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID0);
                }
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.symbol_count);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 16; // entry_count
            }

            else
            {
                boxSize += 32; // entry_count0
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    boxSize += 16; // item_ID
                }

                else
                {
                    boxSize += 32; // item_ID0
                }
                boxSize += 32; // symbol_count
            }
            return boxSize;
        }
    }


    public class PartitionEntry : Box
    {
        public override string FourCC { get { return "fiin"; } }
        public FilePartitionBox blocks_and_symbols;
        public FECReservoirBox FEC_symbol_locations;  // optional
        public FileReservoirBox File_symbol_locations;  // optional

        public PartitionEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.blocks_and_symbols);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.FEC_symbol_locations); //optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.File_symbol_locations); //optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.blocks_and_symbols);
            if (this.FEC_symbol_locations != null) boxSize += IsoReaderWriter.WriteBox(stream, this.FEC_symbol_locations); //optional
            if (this.File_symbol_locations != null) boxSize += IsoReaderWriter.WriteBox(stream, this.File_symbol_locations); //optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(blocks_and_symbols); // blocks_and_symbols
            if (this.FEC_symbol_locations != null) boxSize += IsoReaderWriter.CalculateSize(FEC_symbol_locations); // FEC_symbol_locations
            if (this.File_symbol_locations != null) boxSize += IsoReaderWriter.CalculateSize(File_symbol_locations); // File_symbol_locations
            return boxSize;
        }
    }


    public class FDItemInformationBox : FullBox
    {
        public override string FourCC { get { return "fiin"; } }
        public ushort entry_count;
        public PartitionEntry[] partition_entries;
        public FDSessionGroupBox session_info;  // optional
        public GroupIdToNameBox group_id_to_name;  // optional

        public FDItemInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            boxSize += IsoReaderWriter.ReadClasses(stream, entry_count, out this.partition_entries);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.session_info); //optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.group_id_to_name); //optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            boxSize += IsoReaderWriter.WriteClasses(stream, entry_count, this.partition_entries);
            if (this.session_info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.session_info); //optional
            if (this.group_id_to_name != null) boxSize += IsoReaderWriter.WriteBox(stream, this.group_id_to_name); //optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_count
            boxSize += IsoReaderWriter.CalculateSize(partition_entries); // partition_entries
            if (this.session_info != null) boxSize += IsoReaderWriter.CalculateSize(session_info); // session_info
            if (this.group_id_to_name != null) boxSize += IsoReaderWriter.CalculateSize(group_id_to_name); // group_id_to_name
            return boxSize;
        }
    }


    public class FileReservoirBox : FullBox
    {
        public override string FourCC { get { return "fire"; } }
        public ushort entry_count;
        public uint entry_count0;
        public ushort item_ID;
        public uint item_ID0;
        public uint symbol_count;

        public FileReservoirBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
                }

                else
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID0);
                }
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.symbol_count);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID0);
                }
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.symbol_count);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 16; // entry_count
            }

            else
            {
                boxSize += 32; // entry_count0
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    boxSize += 16; // item_ID
                }

                else
                {
                    boxSize += 32; // item_ID0
                }
                boxSize += 32; // symbol_count
            }
            return boxSize;
        }
    }


    public class FilePartitionBox : FullBox
    {
        public override string FourCC { get { return "fpar"; } }
        public ushort item_ID;
        public uint item_ID0;
        public ushort packet_payload_size;
        public byte reserved = 0;
        public byte FEC_encoding_ID;
        public ushort FEC_instance_ID;
        public ushort max_source_block_length;
        public ushort encoding_symbol_length;
        public ushort max_number_of_encoding_symbols;
        public string scheme_specific_info;
        public ushort entry_count;
        public uint entry_count0;
        public ushort block_count;
        public uint block_size;

        public FilePartitionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID0);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.packet_payload_size);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.FEC_encoding_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.FEC_instance_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_source_block_length);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.encoding_symbol_length);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_number_of_encoding_symbols);
            boxSize += IsoReaderWriter.ReadString(stream, out this.scheme_specific_info);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.block_count);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.block_size);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID0);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.packet_payload_size);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.FEC_encoding_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.FEC_instance_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_source_block_length);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.encoding_symbol_length);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_number_of_encoding_symbols);
            boxSize += IsoReaderWriter.WriteString(stream, this.scheme_specific_info);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.block_count);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.block_size);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 16; // item_ID
            }

            else
            {
                boxSize += 32; // item_ID0
            }
            boxSize += 16; // packet_payload_size
            boxSize += 8; // reserved
            boxSize += 8; // FEC_encoding_ID
            boxSize += 16; // FEC_instance_ID
            boxSize += 16; // max_source_block_length
            boxSize += 16; // encoding_symbol_length
            boxSize += 16; // max_number_of_encoding_symbols
            boxSize += (ulong)scheme_specific_info.Length * 8; // scheme_specific_info

            if (version == 0)
            {
                boxSize += 16; // entry_count
            }

            else
            {
                boxSize += 32; // entry_count0
            }

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 16; // block_count
                boxSize += 32; // block_size
            }
            return boxSize;
        }
    }


    public class FreeSpaceBox : Box
    {
        public override string FourCC { get { return "free"; } }
        public byte[] data;

        public FreeSpaceBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.data);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)data.Length * 8; // data
            return boxSize;
        }
    }


    public class OriginalFormatBox : Box
    {
        public override string FourCC { get { return "frma"; } }
        public uint data_format; // = codingname

        public OriginalFormatBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.data_format); // format of decrypted, encoded data (in case of protection)
            /*  or un-transformed sample entry (in case of restriction */
            /*  and complete track information) */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.data_format); // format of decrypted, encoded data (in case of protection)
            /*  or un-transformed sample entry (in case of restriction */
            /*  and complete track information) */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // data_format
            /*  or un-transformed sample entry (in case of restriction */
            /*  and complete track information) */
            return boxSize;
        }
    }


    public class FileTypeBox : Box
    {
        public override string FourCC { get { return "ftyp"; } }
        public uint major_brand;
        public uint minor_version;
        public uint[] compatible_brands;  //  to end of the box

        public FileTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.major_brand);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.minor_version);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.compatible_brands); // to end of the box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.major_brand);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.minor_version);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.compatible_brands); // to end of the box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // major_brand
            boxSize += 32; // minor_version
            boxSize += 32; // compatible_brands
            return boxSize;
        }
    }


    public class GroupIdToNameBox : FullBox
    {
        public override string FourCC { get { return "gitn"; } }
        public ushort entry_count;
        public uint group_ID;
        public string group_name;

        public GroupIdToNameBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_ID);
                boxSize += IsoReaderWriter.ReadString(stream, out this.group_name);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_ID);
                boxSize += IsoReaderWriter.WriteString(stream, this.group_name);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 32; // group_ID
                boxSize += (ulong)group_name.Length * 8; // group_name
            }
            return boxSize;
        }
    }


    public class GroupsListBox : Box
    {
        public override string FourCC { get { return "grpl"; } }

        public GroupsListBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class HandlerBox : FullBox
    {
        public override string FourCC { get { return "hdlr"; } }
        public uint pre_defined = 0;
        public uint handler_type;
        public uint[] reserved = [];
        public string name;

        public HandlerBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.handler_type);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 3, out this.reserved);
            boxSize += IsoReaderWriter.ReadString(stream, out this.name);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.handler_type);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 3, this.reserved);
            boxSize += IsoReaderWriter.WriteString(stream, this.name);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // pre_defined
            boxSize += 32; // handler_type
            boxSize += 3 * 32; // reserved
            boxSize += (ulong)name.Length * 8; // name
            return boxSize;
        }
    }


    public class HintMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "hmhd"; } }
        public ushort maxPDUsize;
        public ushort avgPDUsize;
        public uint maxbitrate;
        public uint avgbitrate;
        public uint reserved = 0;

        public HintMediaHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.maxPDUsize);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.avgPDUsize);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxbitrate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.avgbitrate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.maxPDUsize);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.avgPDUsize);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxbitrate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.avgbitrate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // maxPDUsize
            boxSize += 16; // avgPDUsize
            boxSize += 32; // maxbitrate
            boxSize += 32; // avgbitrate
            boxSize += 32; // reserved
            return boxSize;
        }
    }


    public class ItemDataBox : Box
    {
        public override string FourCC { get { return "idat"; } }
        public byte[] data;

        public ItemDataBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.data);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8 * (ulong)data.Length; // data
            return boxSize;
        }
    }


    public class ItemInfoBox : FullBox
    {
        public override string FourCC { get { return "iinf"; } }
        public ushort entry_count;
        public uint entry_count0;
        public ItemInfoEntry[] item_infos;

        public ItemInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count0);
            }
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.item_infos);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count0);
            }
            boxSize += IsoReaderWriter.WriteClasses(stream, entry_count, this.item_infos);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 16; // entry_count
            }

            else
            {
                boxSize += 32; // entry_count0
            }
            boxSize += IsoReaderWriter.CalculateClassSize(item_infos); // item_infos
            return boxSize;
        }
    }


    public class ItemLocationBox : FullBox
    {
        public override string FourCC { get { return "iloc"; } }
        public byte offset_size;
        public byte length_size;
        public byte base_offset_size;
        public byte index_size;
        public byte reserved;
        public ushort item_count;
        public uint item_count0;
        public ushort item_ID;
        public uint item_ID0;
        public ushort reserved0 = 0;
        public byte construction_method;
        public ushort data_reference_index;
        public byte[] base_offset;
        public ushort extent_count;
        public byte[] item_reference_index;
        public byte[] extent_offset;
        public byte[] extent_length;

        public ItemLocationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.offset_size);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.length_size);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.base_offset_size);

            if ((version == 1) || (version == 2))
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.index_size);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved);
            }

            if (version < 2)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);
            }

            else if (version == 2)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_count0);
            }

            for (int i = 0; i < item_count; i++)
            {

                if (version < 2)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
                }

                else if (version == 2)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID0);
                }

                if ((version == 1) || (version == 2))
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, 12, out this.reserved0);
                    boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.construction_method);
                }
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.data_reference_index);
                boxSize += IsoReaderWriter.ReadBytes(stream, base_offset_size, out this.base_offset);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.extent_count);

                for (int j = 0; j < extent_count; j++)
                {

                    if (((version == 1) || (version == 2)) && (index_size > 0))
                    {
                        boxSize += IsoReaderWriter.ReadBytes(stream, index_size, out this.item_reference_index);
                    }
                    boxSize += IsoReaderWriter.ReadBytes(stream, offset_size, out this.extent_offset);
                    boxSize += IsoReaderWriter.ReadBytes(stream, length_size, out this.extent_length);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.offset_size);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.length_size);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.base_offset_size);

            if ((version == 1) || (version == 2))
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.index_size);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            }

            if (version < 2)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);
            }

            else if (version == 2)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_count0);
            }

            for (int i = 0; i < item_count; i++)
            {

                if (version < 2)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else if (version == 2)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID0);
                }

                if ((version == 1) || (version == 2))
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, 12, this.reserved0);
                    boxSize += IsoReaderWriter.WriteBits(stream, 4, this.construction_method);
                }
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.data_reference_index);
                boxSize += IsoReaderWriter.WriteBytes(stream, base_offset_size, this.base_offset);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.extent_count);

                for (int j = 0; j < extent_count; j++)
                {

                    if (((version == 1) || (version == 2)) && (index_size > 0))
                    {
                        boxSize += IsoReaderWriter.WriteBytes(stream, index_size, this.item_reference_index);
                    }
                    boxSize += IsoReaderWriter.WriteBytes(stream, offset_size, this.extent_offset);
                    boxSize += IsoReaderWriter.WriteBytes(stream, length_size, this.extent_length);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 4; // offset_size
            boxSize += 4; // length_size
            boxSize += 4; // base_offset_size

            if ((version == 1) || (version == 2))
            {
                boxSize += 4; // index_size
            }

            else
            {
                boxSize += 4; // reserved
            }

            if (version < 2)
            {
                boxSize += 16; // item_count
            }

            else if (version == 2)
            {
                boxSize += 32; // item_count0
            }

            for (int i = 0; i < item_count; i++)
            {

                if (version < 2)
                {
                    boxSize += 16; // item_ID
                }

                else if (version == 2)
                {
                    boxSize += 32; // item_ID0
                }

                if ((version == 1) || (version == 2))
                {
                    boxSize += 12; // reserved0
                    boxSize += 4; // construction_method
                }
                boxSize += 16; // data_reference_index
                boxSize += (ulong)base_offset_size * 8; // base_offset
                boxSize += 16; // extent_count

                for (int j = 0; j < extent_count; j++)
                {

                    if (((version == 1) || (version == 2)) && (index_size > 0))
                    {
                        boxSize += (ulong)index_size * 8; // item_reference_index
                    }
                    boxSize += (ulong)offset_size * 8; // extent_offset
                    boxSize += (ulong)length_size * 8; // extent_length
                }
            }
            return boxSize;
        }
    }


    public class IdentifiedMediaDataBox : Box
    {
        public override string FourCC { get { return "imda"; } }
        public uint imda_identifier;
        public byte[] data;  //  until the end of the box

        public IdentifiedMediaDataBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.imda_identifier);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.data); // until the end of the box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.imda_identifier);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.data); // until the end of the box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // imda_identifier
            boxSize += 8 * (ulong)data.Length; // data
            return boxSize;
        }
    }


    public class ItemInfoEntry : FullBox
    {
        public override string FourCC { get { return "infe"; } }
        public ushort item_ID;
        public ushort item_protection_index;
        public string item_name;
        public string content_type;
        public string content_encoding;  // optional
        public uint extension_type;  // optional
        public ItemInfoExtension ItemInfoExtension;  // optional
        public ushort item_ID0;
        public uint item_ID00;
        public ushort item_protection_index0;
        public uint item_type;
        public string item_name0;
        public string content_type0;
        public string content_encoding0;  // optional
        public string item_uri_type;

        public ItemInfoEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if ((version == 0) || (version == 1))
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_protection_index);
                boxSize += IsoReaderWriter.ReadString(stream, out this.item_name);
                boxSize += IsoReaderWriter.ReadString(stream, out this.content_type);
                if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.content_encoding); //optional
            }

            if (version == 1)
            {
                if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.extension_type); //optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.ItemInfoExtension); //optional
            }

            if (version >= 2)
            {

                if (version == 2)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID0);
                }

                else if (version == 3)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID00);
                }
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_protection_index0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_type);
                boxSize += IsoReaderWriter.ReadString(stream, out this.item_name0);

                if (item_type == IsoReaderWriter.FromFourCC("mime"))
                {
                    boxSize += IsoReaderWriter.ReadString(stream, out this.content_type0);
                    if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.content_encoding0); //optional
                }

                else if (item_type == IsoReaderWriter.FromFourCC("uri "))
                {
                    boxSize += IsoReaderWriter.ReadString(stream, out this.item_uri_type);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if ((version == 0) || (version == 1))
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_protection_index);
                boxSize += IsoReaderWriter.WriteString(stream, this.item_name);
                boxSize += IsoReaderWriter.WriteString(stream, this.content_type);
                if (this.content_encoding != null) boxSize += IsoReaderWriter.WriteString(stream, this.content_encoding); //optional
            }

            if (version == 1)
            {
                if (this.extension_type != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.extension_type); //optional
                if (this.ItemInfoExtension != null) boxSize += IsoReaderWriter.WriteBox(stream, this.ItemInfoExtension); //optional
            }

            if (version >= 2)
            {

                if (version == 2)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID0);
                }

                else if (version == 3)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID00);
                }
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_protection_index0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_type);
                boxSize += IsoReaderWriter.WriteString(stream, this.item_name0);

                if (item_type == IsoReaderWriter.FromFourCC("mime"))
                {
                    boxSize += IsoReaderWriter.WriteString(stream, this.content_type0);
                    if (this.content_encoding0 != null) boxSize += IsoReaderWriter.WriteString(stream, this.content_encoding0); //optional
                }

                else if (item_type == IsoReaderWriter.FromFourCC("uri "))
                {
                    boxSize += IsoReaderWriter.WriteString(stream, this.item_uri_type);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if ((version == 0) || (version == 1))
            {
                boxSize += 16; // item_ID
                boxSize += 16; // item_protection_index
                boxSize += (ulong)item_name.Length * 8; // item_name
                boxSize += (ulong)content_type.Length * 8; // content_type
                if (this.content_encoding != null) boxSize += (ulong)content_encoding.Length * 8; // content_encoding
            }

            if (version == 1)
            {
                if (this.extension_type != null) boxSize += 32; // extension_type
                if (this.ItemInfoExtension != null) boxSize += IsoReaderWriter.CalculateSize(ItemInfoExtension); // ItemInfoExtension
            }

            if (version >= 2)
            {

                if (version == 2)
                {
                    boxSize += 16; // item_ID0
                }

                else if (version == 3)
                {
                    boxSize += 32; // item_ID00
                }
                boxSize += 16; // item_protection_index0
                boxSize += 32; // item_type
                boxSize += (ulong)item_name0.Length * 8; // item_name0

                if (item_type == IsoReaderWriter.FromFourCC("mime"))
                {
                    boxSize += (ulong)content_type0.Length * 8; // content_type0
                    if (this.content_encoding0 != null) boxSize += (ulong)content_encoding0.Length * 8; // content_encoding0
                }

                else if (item_type == IsoReaderWriter.FromFourCC("uri "))
                {
                    boxSize += (ulong)item_uri_type.Length * 8; // item_uri_type
                }
            }
            return boxSize;
        }
    }


    public class ItemProtectionBox : FullBox
    {
        public override string FourCC { get { return "ipro"; } }
        public ushort protection_count;
        public ProtectionSchemeInfoBox protection_information;

        public ItemProtectionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.protection_count);

            for (int i = 1; i <= protection_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBox(stream, out this.protection_information);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.protection_count);

            for (int i = 1; i <= protection_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBox(stream, this.protection_information);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // protection_count

            for (int i = 1; i <= protection_count; i++)
            {
                boxSize += IsoReaderWriter.CalculateSize(protection_information); // protection_information
            }
            return boxSize;
        }
    }


    public class ItemReferenceBox : FullBox
    {
        public override string FourCC { get { return "iref"; } }
        public SingleItemTypeReferenceBox[] references;
        public SingleItemTypeReferenceBoxLarge[] references0;

        public ItemReferenceBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadBoxes(stream, out this.references);
            }

            else if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadBoxes(stream, out this.references0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteBoxes(stream, this.references);
            }

            else if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteBoxes(stream, this.references0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += IsoReaderWriter.CalculateSize(references); // references
            }

            else if (version == 1)
            {
                boxSize += IsoReaderWriter.CalculateSize(references0); // references0
            }
            return boxSize;
        }
    }


    public class LevelAssignmentBox : FullBox
    {
        public override string FourCC { get { return "leva"; } }
        public byte level_count;
        public uint track_ID;
        public bool padding_flag;
        public byte assignment_type;
        public uint grouping_type;
        public uint grouping_type0;
        public uint grouping_type_parameter;
        public uint sub_track_ID;

        public LevelAssignmentBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.level_count);

            for (int j = 1; j <= level_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.padding_flag);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.assignment_type);

                if (assignment_type == 0)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type);
                }

                else if (assignment_type == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type0);
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type_parameter);
                }

                else if (assignment_type == 2)
                {
                }
                /*  no further syntax elements needed */

                else if (assignment_type == 3)
                {
                }
                /*  no further syntax elements needed */

                else if (assignment_type == 4)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sub_track_ID);
                }
                /*  other assignment_type values are reserved */
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.level_count);

            for (int j = 1; j <= level_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
                boxSize += IsoReaderWriter.WriteBit(stream, this.padding_flag);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.assignment_type);

                if (assignment_type == 0)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);
                }

                else if (assignment_type == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type0);
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
                }

                else if (assignment_type == 2)
                {
                }
                /*  no further syntax elements needed */

                else if (assignment_type == 3)
                {
                }
                /*  no further syntax elements needed */

                else if (assignment_type == 4)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.sub_track_ID);
                }
                /*  other assignment_type values are reserved */
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // level_count

            for (int j = 1; j <= level_count; j++)
            {
                boxSize += 32; // track_ID
                boxSize += 1; // padding_flag
                boxSize += 7; // assignment_type

                if (assignment_type == 0)
                {
                    boxSize += 32; // grouping_type
                }

                else if (assignment_type == 1)
                {
                    boxSize += 32; // grouping_type0
                    boxSize += 32; // grouping_type_parameter
                }

                else if (assignment_type == 2)
                {
                }
                /*  no further syntax elements needed */

                else if (assignment_type == 3)
                {
                }
                /*  no further syntax elements needed */

                else if (assignment_type == 4)
                {
                    boxSize += 32; // sub_track_ID
                }
                /*  other assignment_type values are reserved */
            }
            return boxSize;
        }
    }


    public class MediaDataBox : Box
    {
        public override string FourCC { get { return "mdat"; } }
        public byte[] data;

        public MediaDataBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.data);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8 * (ulong)data.Length; // data
            return boxSize;
        }
    }


    public class MediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "mdhd"; } }
        public ulong creation_time;
        public ulong modification_time;
        public uint timescale;
        public ulong duration;
        public uint creation_time0;
        public uint modification_time0;
        public uint timescale0;
        public uint duration0;
        public bool pad = false;
        public byte[] language;  //  ISO-639-2/T language code
        public ushort pre_defined = 0;

        public MediaHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.creation_time);
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.modification_time);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale);
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.creation_time0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.modification_time0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.duration0);
            }
            boxSize += IsoReaderWriter.ReadBit(stream, out this.pad);
            boxSize += IsoReaderWriter.ReadBitsArray(stream, 5, 3, out this.language); // ISO-639-2/T language code
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale);
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.creation_time0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.modification_time0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.duration0);
            }
            boxSize += IsoReaderWriter.WriteBit(stream, this.pad);
            boxSize += IsoReaderWriter.WriteBitsArray(stream, 5, 3, this.language); // ISO-639-2/T language code
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 1)
            {
                boxSize += 64; // creation_time
                boxSize += 64; // modification_time
                boxSize += 32; // timescale
                boxSize += 64; // duration
            }

            else
            {
                /*  version==0 */
                boxSize += 32; // creation_time0
                boxSize += 32; // modification_time0
                boxSize += 32; // timescale0
                boxSize += 32; // duration0
            }
            boxSize += 1; // pad
            boxSize += 3 * 5; // language
            boxSize += 16; // pre_defined
            return boxSize;
        }
    }


    public class MediaBox : Box
    {
        public override string FourCC { get { return "mdia"; } }

        public MediaBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class MovieExtendsHeaderBox : FullBox
    {
        public override string FourCC { get { return "mehd"; } }
        public ulong fragment_duration;
        public uint fragment_duration0;

        public MovieExtendsHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.fragment_duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.fragment_duration0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.fragment_duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.fragment_duration0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 1)
            {
                boxSize += 64; // fragment_duration
            }

            else
            {
                /*  version==0 */
                boxSize += 32; // fragment_duration0
            }
            return boxSize;
        }
    }


    public class MetaBox : FullBox
    {
        public override string FourCC { get { return "meta"; } }
        public HandlerBox theHandler;
        public PrimaryItemBox primary_resource;  //  optional
        public DataInformationBox file_locations;  //  optional
        public ItemLocationBox item_locations;  //  optional
        public ItemProtectionBox protections;  //  optional
        public ItemInfoBox item_infos;  //  optional
        public IPMPControlBox IPMP_control;  //  optional
        public ItemReferenceBox item_refs;  //  optional
        public ItemDataBox item_data;  //  optional
        public Box[] other_boxes;  //  optional

        public MetaBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.theHandler);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.primary_resource); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.file_locations); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.item_locations); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.protections); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.item_infos); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.IPMP_control); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.item_refs); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.item_data); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBoxes(stream, out this.other_boxes); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.theHandler);
            if (this.primary_resource != null) boxSize += IsoReaderWriter.WriteBox(stream, this.primary_resource); // optional
            if (this.file_locations != null) boxSize += IsoReaderWriter.WriteBox(stream, this.file_locations); // optional
            if (this.item_locations != null) boxSize += IsoReaderWriter.WriteBox(stream, this.item_locations); // optional
            if (this.protections != null) boxSize += IsoReaderWriter.WriteBox(stream, this.protections); // optional
            if (this.item_infos != null) boxSize += IsoReaderWriter.WriteBox(stream, this.item_infos); // optional
            if (this.IPMP_control != null) boxSize += IsoReaderWriter.WriteBox(stream, this.IPMP_control); // optional
            if (this.item_refs != null) boxSize += IsoReaderWriter.WriteBox(stream, this.item_refs); // optional
            if (this.item_data != null) boxSize += IsoReaderWriter.WriteBox(stream, this.item_data); // optional
            if (this.other_boxes != null) boxSize += IsoReaderWriter.WriteBoxes(stream, this.other_boxes); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(theHandler); // theHandler
            if (this.primary_resource != null) boxSize += IsoReaderWriter.CalculateSize(primary_resource); // primary_resource
            if (this.file_locations != null) boxSize += IsoReaderWriter.CalculateSize(file_locations); // file_locations
            if (this.item_locations != null) boxSize += IsoReaderWriter.CalculateSize(item_locations); // item_locations
            if (this.protections != null) boxSize += IsoReaderWriter.CalculateSize(protections); // protections
            if (this.item_infos != null) boxSize += IsoReaderWriter.CalculateSize(item_infos); // item_infos
            if (this.IPMP_control != null) boxSize += IsoReaderWriter.CalculateSize(IPMP_control); // IPMP_control
            if (this.item_refs != null) boxSize += IsoReaderWriter.CalculateSize(item_refs); // item_refs
            if (this.item_data != null) boxSize += IsoReaderWriter.CalculateSize(item_data); // item_data
            if (this.other_boxes != null) boxSize += IsoReaderWriter.CalculateSize(other_boxes); // other_boxes
            return boxSize;
        }
    }


    public class MovieFragmentHeaderBox : FullBox
    {
        public override string FourCC { get { return "mfhd"; } }
        public uint sequence_number;

        public MovieFragmentHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sequence_number);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sequence_number);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // sequence_number
            return boxSize;
        }
    }


    public class MovieFragmentRandomAccessBox : Box
    {
        public override string FourCC { get { return "mfra"; } }

        public MovieFragmentRandomAccessBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class MovieFragmentRandomAccessOffsetBox : FullBox
    {
        public override string FourCC { get { return "mfro"; } }
        public uint parent_size;

        public MovieFragmentRandomAccessOffsetBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.parent_size);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.parent_size);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // parent_size
            return boxSize;
        }
    }


    public class MediaInformationBox : Box
    {
        public override string FourCC { get { return "minf"; } }

        public MediaInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class CompressedMovieFragmentBox : CompressedBox
    {
        public override string FourCC { get { return "moof"; } }

        public CompressedMovieFragmentBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class CompressedMovieBox : CompressedBox
    {
        public override string FourCC { get { return "moov"; } }

        public CompressedMovieBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class MovieExtendsBox : Box
    {
        public override string FourCC { get { return "mvex"; } }

        public MovieExtendsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class MovieHeaderBox : FullBox
    {
        public override string FourCC { get { return "mvhd"; } }
        public ulong creation_time;
        public ulong modification_time;
        public uint timescale;
        public ulong duration;
        public uint creation_time0;
        public uint modification_time0;
        public uint timescale0;
        public uint duration0;
        public int rate = 0x00010000;  //  typically 1.0
        public short volume = 0x0100;  //  typically, full volume
        public ushort reserved = 0;
        public uint[] reserved0 = [];
        public uint[] matrix =
            { 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };  //  Unity matrix
        public uint[] pre_defined = [];
        public uint next_track_ID;

        public MovieHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.creation_time);
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.modification_time);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale);
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.creation_time0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.modification_time0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.timescale0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.duration0);
            }
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.rate); // typically 1.0
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.volume); // typically, full volume
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 2, out this.reserved0);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 9, out this.matrix); // Unity matrix
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 6, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.next_track_ID);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale);
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.creation_time0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.modification_time0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.timescale0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.duration0);
            }
            boxSize += IsoReaderWriter.WriteInt32(stream, this.rate); // typically 1.0
            boxSize += IsoReaderWriter.WriteInt16(stream, this.volume); // typically, full volume
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved0);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 9, this.matrix); // Unity matrix
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 6, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.next_track_ID);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 1)
            {
                boxSize += 64; // creation_time
                boxSize += 64; // modification_time
                boxSize += 32; // timescale
                boxSize += 64; // duration
            }

            else
            {
                /*  version==0 */
                boxSize += 32; // creation_time0
                boxSize += 32; // modification_time0
                boxSize += 32; // timescale0
                boxSize += 32; // duration0
            }
            boxSize += 32; // rate
            boxSize += 16; // volume
            boxSize += 16; // reserved
            boxSize += 2 * 32; // reserved0
            boxSize += 9 * 32; // matrix
            boxSize += 6 * 32; // pre_defined
            boxSize += 32; // next_track_ID
            return boxSize;
        }
    }


    public class NullMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "nmhd"; } }

        public NullMediaHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class OriginalFileTypeBox : Box
    {
        public override string FourCC { get { return "otyp"; } }

        public OriginalFileTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class PaddingBitsBox : FullBox
    {
        public override string FourCC { get { return "padb"; } }
        public uint sample_count;
        public bool reserved = false;
        public byte pad1;
        public bool reserved0 = false;
        public byte pad2;

        public PaddingBitsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);


            for (int i = 0; i < ((sample_count + 1) / 2); i++)
            {
                boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.pad1);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved0);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.pad2);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);


            for (int i = 0; i < ((sample_count + 1) / 2); i++)
            {
                boxSize += IsoReaderWriter.WriteBit(stream, this.reserved);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.pad1);
                boxSize += IsoReaderWriter.WriteBit(stream, this.reserved0);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.pad2);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // sample_count


            for (int i = 0; i < ((sample_count + 1) / 2); i++)
            {
                boxSize += 1; // reserved
                boxSize += 3; // pad1
                boxSize += 1; // reserved0
                boxSize += 3; // pad2
            }
            return boxSize;
        }
    }


    public class PartitionEntry1 : Box
    {
        public override string FourCC { get { return "paen"; } }
        public FilePartitionBox blocks_and_symbols;
        public FECReservoirBox FEC_symbol_locations;  // optional
        public FileReservoirBox File_symbol_locations;  // optional

        public PartitionEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.blocks_and_symbols);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.FEC_symbol_locations); //optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.File_symbol_locations); //optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.blocks_and_symbols);
            if (this.FEC_symbol_locations != null) boxSize += IsoReaderWriter.WriteBox(stream, this.FEC_symbol_locations); //optional
            if (this.File_symbol_locations != null) boxSize += IsoReaderWriter.WriteBox(stream, this.File_symbol_locations); //optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(blocks_and_symbols); // blocks_and_symbols
            if (this.FEC_symbol_locations != null) boxSize += IsoReaderWriter.CalculateSize(FEC_symbol_locations); // FEC_symbol_locations
            if (this.File_symbol_locations != null) boxSize += IsoReaderWriter.CalculateSize(File_symbol_locations); // File_symbol_locations
            return boxSize;
        }
    }


    public class FDItemInformationBox1 : FullBox
    {
        public override string FourCC { get { return "paen"; } }
        public ushort entry_count;
        public PartitionEntry[] partition_entries;
        public FDSessionGroupBox session_info;  // optional
        public GroupIdToNameBox group_id_to_name;  // optional

        public FDItemInformationBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            boxSize += IsoReaderWriter.ReadClasses(stream, entry_count, out this.partition_entries);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.session_info); //optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.group_id_to_name); //optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            boxSize += IsoReaderWriter.WriteClasses(stream, entry_count, this.partition_entries);
            if (this.session_info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.session_info); //optional
            if (this.group_id_to_name != null) boxSize += IsoReaderWriter.WriteBox(stream, this.group_id_to_name); //optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_count
            boxSize += IsoReaderWriter.CalculateSize(partition_entries); // partition_entries
            if (this.session_info != null) boxSize += IsoReaderWriter.CalculateSize(session_info); // session_info
            if (this.group_id_to_name != null) boxSize += IsoReaderWriter.CalculateSize(group_id_to_name); // group_id_to_name
            return boxSize;
        }
    }


    public class ProgressiveDownloadInfoBox : FullBox
    {
        public override string FourCC { get { return "pdin"; } }
        public uint rate;
        public uint initial_delay;

        public ProgressiveDownloadInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            for (int i = 0; ; i++)
            {
                /*  to end of box */
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.rate);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.initial_delay);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            for (int i = 0; ; i++)
            {
                /*  to end of box */
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.rate);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.initial_delay);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            for (int i = 0; ; i++)
            {
                /*  to end of box */
                boxSize += 32; // rate
                boxSize += 32; // initial_delay
            }
            return boxSize;
        }
    }


    public class PrimaryItemBox : FullBox
    {
        public override string FourCC { get { return "pitm"; } }
        public ushort item_ID;
        public uint item_ID0;

        public PrimaryItemBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_ID);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_ID0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_ID0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 0)
            {
                boxSize += 16; // item_ID
            }

            else
            {
                boxSize += 32; // item_ID0
            }
            return boxSize;
        }
    }


    public class ProducerReferenceTimeBox : FullBox
    {
        public override string FourCC { get { return "prft"; } }
        public uint reference_track_ID;
        public ulong ntp_timestamp;
        public uint media_time;
        public ulong media_time0;

        public ProducerReferenceTimeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.reference_track_ID);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.ntp_timestamp);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.media_time);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.media_time0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.reference_track_ID);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.ntp_timestamp);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.media_time);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.media_time0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // reference_track_ID
            boxSize += 64; // ntp_timestamp

            if (version == 0)
            {
                boxSize += 32; // media_time
            }

            else
            {
                boxSize += 64; // media_time0
            }
            return boxSize;
        }
    }


    public class RestrictedSchemeInfoBox : Box
    {
        public override string FourCC { get { return "rinf"; } }
        public OriginalFormatBox original_format;
        public SchemeTypeBox scheme_type_box;
        public SchemeInformationBox info;  //  optional

        public RestrictedSchemeInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.original_format);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.scheme_type_box);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.info); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.original_format);
            boxSize += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            if (this.info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.info); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(original_format); // original_format
            boxSize += IsoReaderWriter.CalculateSize(scheme_type_box); // scheme_type_box
            if (this.info != null) boxSize += IsoReaderWriter.CalculateSize(info); // info
            return boxSize;
        }
    }


    public class SampleAuxiliaryInformationOffsetsBox : FullBox
    {
        public override string FourCC { get { return "saio"; } }
        public uint aux_info_type;
        public uint aux_info_type_parameter;
        public uint entry_count;
        public uint[] offset;
        public ulong[] offset0;

        public SampleAuxiliaryInformationOffsetsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if ((flags & 1) == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.aux_info_type);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.aux_info_type_parameter);
            }
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt32Array(stream, entry_count, out this.offset);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt64Array(stream, entry_count, out this.offset0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if ((flags & 1) == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type_parameter);
            }
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            if (version == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt32Array(stream, entry_count, this.offset);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt64Array(stream, entry_count, this.offset0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if ((flags & 1) == 1)
            {
                boxSize += 32; // aux_info_type
                boxSize += 32; // aux_info_type_parameter
            }
            boxSize += 32; // entry_count

            if (version == 0)
            {
                boxSize += (ulong)entry_count * 32; // offset
            }

            else
            {
                boxSize += (ulong)entry_count * 64; // offset0
            }
            return boxSize;
        }
    }


    public class SampleAuxiliaryInformationSizesBox : FullBox
    {
        public override string FourCC { get { return "saiz"; } }
        public uint aux_info_type;
        public uint aux_info_type_parameter;
        public byte default_sample_info_size;
        public uint sample_count;
        public byte[] sample_info_size;

        public SampleAuxiliaryInformationSizesBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if ((flags & 1) == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.aux_info_type);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.aux_info_type_parameter);
            }
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.default_sample_info_size);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);

            if (default_sample_info_size == 0)
            {
                boxSize += IsoReaderWriter.ReadBytes(stream, sample_count, out this.sample_info_size);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if ((flags & 1) == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type_parameter);
            }
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.default_sample_info_size);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);

            if (default_sample_info_size == 0)
            {
                boxSize += IsoReaderWriter.WriteBytes(stream, sample_count, this.sample_info_size);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if ((flags & 1) == 1)
            {
                boxSize += 32; // aux_info_type
                boxSize += 32; // aux_info_type_parameter
            }
            boxSize += 8; // default_sample_info_size
            boxSize += 32; // sample_count

            if (default_sample_info_size == 0)
            {
                boxSize += (ulong)sample_count * 8; // sample_info_size
            }
            return boxSize;
        }
    }


    public class SampleToGroupBox : FullBox
    {
        public override string FourCC { get { return "sbgp"; } }
        public uint grouping_type;
        public uint grouping_type_parameter;
        public uint entry_count;
        public uint sample_count;
        public uint group_description_index;

        public SampleToGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type_parameter);
            }
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_description_index);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
            }
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_description_index);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // grouping_type

            if (version == 1)
            {
                boxSize += 32; // grouping_type_parameter
            }
            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 32; // sample_count
                boxSize += 32; // group_description_index
            }
            return boxSize;
        }
    }


    public class SchemeInformationBox : Box
    {
        public override string FourCC { get { return "schi"; } }
        public Box[] scheme_specific_data;

        public SchemeInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.scheme_specific_data);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.scheme_specific_data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(scheme_specific_data); // scheme_specific_data
            return boxSize;
        }
    }


    public class SchemeTypeBox : FullBox
    {
        public override string FourCC { get { return "schm"; } }
        public uint scheme_type;  //  4CC identifying the scheme
        public uint scheme_version;  //  scheme version
        public string scheme_uri;  //  browser uri

        public SchemeTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.scheme_type); // 4CC identifying the scheme
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.scheme_version); // scheme version

            if ((flags & 0x000001) == 0x000001)
            {
                boxSize += IsoReaderWriter.ReadString(stream, out this.scheme_uri); // browser uri
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.scheme_type); // 4CC identifying the scheme
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.scheme_version); // scheme version

            if ((flags & 0x000001) == 0x000001)
            {
                boxSize += IsoReaderWriter.WriteString(stream, this.scheme_uri); // browser uri
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // scheme_type
            boxSize += 32; // scheme_version

            if ((flags & 0x000001) == 0x000001)
            {
                boxSize += (ulong)scheme_uri.Length * 8; // scheme_uri
            }
            return boxSize;
        }
    }


    public class CompatibleSchemeTypeBox : FullBox
    {
        public override string FourCC { get { return "csch"; } }
        public uint scheme_type;  //  4CC identifying the scheme
        public uint scheme_version;  //  scheme version 
        public string scheme_uri;  //  browser uri

        public CompatibleSchemeTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /*  identical syntax to SchemeTypeBox */
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.scheme_type); // 4CC identifying the scheme
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.scheme_version); // scheme version 

            if ((flags & 0x000001) == 0x000001)
            {
                boxSize += IsoReaderWriter.ReadString(stream, out this.scheme_uri); // browser uri
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /*  identical syntax to SchemeTypeBox */
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.scheme_type); // 4CC identifying the scheme
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.scheme_version); // scheme version 

            if ((flags & 0x000001) == 0x000001)
            {
                boxSize += IsoReaderWriter.WriteString(stream, this.scheme_uri); // browser uri
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /*  identical syntax to SchemeTypeBox */
            boxSize += 32; // scheme_type
            boxSize += 32; // scheme_version

            if ((flags & 0x000001) == 0x000001)
            {
                boxSize += (ulong)scheme_uri.Length * 8; // scheme_uri
            }
            return boxSize;
        }
    }


    public class SampleDependencyTypeBox : FullBox
    {
        public override string FourCC { get { return "sdtp"; } }
        public byte is_leading;
        public byte sample_depends_on;
        public byte sample_is_depended_on;
        public byte sample_has_redundancy;

        public SampleDependencyTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            int sample_count = 0; // TODO: taken from the stsz sample_count


            for (int i = 0; i < sample_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.is_leading);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.sample_depends_on);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.sample_is_depended_on);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.sample_has_redundancy);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            int sample_count = 0; // TODO: taken from the stsz sample_count


            for (int i = 0; i < sample_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.is_leading);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.sample_depends_on);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.sample_is_depended_on);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.sample_has_redundancy);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            int sample_count = 0; // TODO: taken from the stsz sample_count


            for (int i = 0; i < sample_count; i++)
            {
                boxSize += 2; // is_leading
                boxSize += 2; // sample_depends_on
                boxSize += 2; // sample_is_depended_on
                boxSize += 2; // sample_has_redundancy
            }
            return boxSize;
        }
    }


    public class FDSessionGroupBox : Box
    {
        public override string FourCC { get { return "segr"; } }
        public ushort num_session_groups;
        public byte entry_count;
        public uint group_ID;
        public ushort num_channels_in_session_group;
        public uint hint_track_ID;

        public FDSessionGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_session_groups);

            for (int i = 0; i < num_session_groups; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entry_count);

                for (int j = 0; j < entry_count; j++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_ID);
                }
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_channels_in_session_group);

                for (int k = 0; k < num_channels_in_session_group; k++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.hint_track_ID);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_session_groups);

            for (int i = 0; i < num_session_groups; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.entry_count);

                for (int j = 0; j < entry_count; j++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_ID);
                }
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_channels_in_session_group);

                for (int k = 0; k < num_channels_in_session_group; k++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.hint_track_ID);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // num_session_groups

            for (int i = 0; i < num_session_groups; i++)
            {
                boxSize += 8; // entry_count

                for (int j = 0; j < entry_count; j++)
                {
                    boxSize += 32; // group_ID
                }
                boxSize += 16; // num_channels_in_session_group

                for (int k = 0; k < num_channels_in_session_group; k++)
                {
                    boxSize += 32; // hint_track_ID
                }
            }
            return boxSize;
        }
    }


    public class SampleGroupDescriptionBox : FullBox
    {
        public override string FourCC { get { return "sgpd"; } }
        public uint grouping_type;
        public uint default_length;
        public uint default_group_description_index;
        public uint entry_count;
        public uint description_length;
        public SampleGroupDescriptionEntry SampleGroupDescriptionEntry;  //  an instance of a class derived from SampleGroupDescriptionEntry

        public SampleGroupDescriptionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type);

            if (version >= 1)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_length);
            }

            if (version >= 2)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_group_description_index);
            }
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            for (int i = 1; i <= entry_count; i++)
            {

                if (version >= 1)
                {

                    if (default_length == 0)
                    {
                        boxSize += IsoReaderWriter.ReadUInt32(stream, out this.description_length);
                    }
                }
                boxSize += IsoReaderWriter.ReadBox(stream, out this.SampleGroupDescriptionEntry); // an instance of a class derived from SampleGroupDescriptionEntry
                /*   that is appropriate and permitted for the media type */
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

            if (version >= 1)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_length);
            }

            if (version >= 2)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_group_description_index);
            }
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 1; i <= entry_count; i++)
            {

                if (version >= 1)
                {

                    if (default_length == 0)
                    {
                        boxSize += IsoReaderWriter.WriteUInt32(stream, this.description_length);
                    }
                }
                boxSize += IsoReaderWriter.WriteBox(stream, this.SampleGroupDescriptionEntry); // an instance of a class derived from SampleGroupDescriptionEntry
                /*   that is appropriate and permitted for the media type */
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // grouping_type

            if (version >= 1)
            {
                boxSize += 32; // default_length
            }

            if (version >= 2)
            {
                boxSize += 32; // default_group_description_index
            }
            boxSize += 32; // entry_count


            for (int i = 1; i <= entry_count; i++)
            {

                if (version >= 1)
                {

                    if (default_length == 0)
                    {
                        boxSize += 32; // description_length
                    }
                }
                boxSize += IsoReaderWriter.CalculateSize(SampleGroupDescriptionEntry); // SampleGroupDescriptionEntry
                /*   that is appropriate and permitted for the media type */
            }
            return boxSize;
        }
    }


    public class CompressedSegmentIndexBox : CompressedBox
    {
        public override string FourCC { get { return "sidx"; } }

        public CompressedSegmentIndexBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class ProtectionSchemeInfoBox : Box
    {
        public override string FourCC { get { return "sinf"; } }
        public OriginalFormatBox original_format;
        public SchemeTypeBox scheme_type_box;  //  optional
        public SchemeInformationBox info;  //  optional

        public ProtectionSchemeInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.original_format);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scheme_type_box); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.info); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.original_format);
            if (this.scheme_type_box != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scheme_type_box); // optional
            if (this.info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.info); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(original_format); // original_format
            if (this.scheme_type_box != null) boxSize += IsoReaderWriter.CalculateSize(scheme_type_box); // scheme_type_box
            if (this.info != null) boxSize += IsoReaderWriter.CalculateSize(info); // info
            return boxSize;
        }
    }


    public class FreeSpaceBox1 : Box
    {
        public override string FourCC { get { return "skip"; } }
        public byte[] data;

        public FreeSpaceBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.data);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)data.Length * 8; // data
            return boxSize;
        }
    }


    public class SoundMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "smhd"; } }
        public short balance = 0;
        public ushort reserved = 0;

        public SoundMediaHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.balance);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.balance);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // balance
            boxSize += 16; // reserved
            return boxSize;
        }
    }


    public class SRTPProcessBox : FullBox
    {
        public override string FourCC { get { return "srpp"; } }
        public uint encryption_algorithm_rtp;
        public uint encryption_algorithm_rtcp;
        public uint integrity_algorithm_rtp;
        public uint integrity_algorithm_rtcp;
        public SchemeTypeBox scheme_type_box;
        public SchemeInformationBox info;

        public SRTPProcessBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.encryption_algorithm_rtp);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.encryption_algorithm_rtcp);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.integrity_algorithm_rtp);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.integrity_algorithm_rtcp);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.scheme_type_box);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.info);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.encryption_algorithm_rtp);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.encryption_algorithm_rtcp);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.integrity_algorithm_rtp);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.integrity_algorithm_rtcp);
            boxSize += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            boxSize += IsoReaderWriter.WriteBox(stream, this.info);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // encryption_algorithm_rtp
            boxSize += 32; // encryption_algorithm_rtcp
            boxSize += 32; // integrity_algorithm_rtp
            boxSize += 32; // integrity_algorithm_rtcp
            boxSize += IsoReaderWriter.CalculateSize(scheme_type_box); // scheme_type_box
            boxSize += IsoReaderWriter.CalculateSize(info); // info
            return boxSize;
        }
    }


    public class CompressedSubsegmentIndexBox : CompressedBox
    {
        public override string FourCC { get { return "ssix"; } }

        public CompressedSubsegmentIndexBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class SampleTableBox : Box
    {
        public override string FourCC { get { return "stbl"; } }

        public SampleTableBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class ChunkOffsetBox : FullBox
    {
        public override string FourCC { get { return "stco"; } }
        public uint entry_count;
        public uint chunk_offset;

        public ChunkOffsetBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.chunk_offset);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.chunk_offset);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 32; // chunk_offset
            }
            return boxSize;
        }
    }


    public class DegradationPriorityBox : FullBox
    {
        public override string FourCC { get { return "stdp"; } }
        public ushort priority;

        public DegradationPriorityBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            int sample_count = 0; // TODO: taken from the stsz sample_count



            for (int i = 0; i < sample_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.priority);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            int sample_count = 0; // TODO: taken from the stsz sample_count



            for (int i = 0; i < sample_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.priority);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            int sample_count = 0; // TODO: taken from the stsz sample_count



            for (int i = 0; i < sample_count; i++)
            {
                boxSize += 16; // priority
            }
            return boxSize;
        }
    }


    public class SubtitleMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "sthd"; } }

        public SubtitleMediaHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class SubTrackDefinitionBox : Box
    {
        public override string FourCC { get { return "strd"; } }

        public SubTrackDefinitionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class SubTrackInformationBox : FullBox
    {
        public override string FourCC { get { return "stri"; } }
        public short switch_group = 0;
        public short alternate_group = 0;
        public uint sub_track_ID = 0;
        public uint[] attribute_list;  //  to the end of the box

        public SubTrackInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.switch_group);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.alternate_group);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sub_track_ID);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.attribute_list); // to the end of the box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.switch_group);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.alternate_group);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sub_track_ID);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.attribute_list); // to the end of the box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // switch_group
            boxSize += 16; // alternate_group
            boxSize += 32; // sub_track_ID
            boxSize += 32; // attribute_list
            return boxSize;
        }
    }


    public class SampleToChunkBox : FullBox
    {
        public override string FourCC { get { return "stsc"; } }
        public uint entry_count;
        public uint first_chunk;
        public uint samples_per_chunk;
        public uint sample_description_index;

        public SampleToChunkBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.first_chunk);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.samples_per_chunk);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_description_index);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.first_chunk);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.samples_per_chunk);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_description_index);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 32; // first_chunk
                boxSize += 32; // samples_per_chunk
                boxSize += 32; // sample_description_index
            }
            return boxSize;
        }
    }


    public class SampleDescriptionBox : FullBox
    {
        public override string FourCC { get { return "stsd"; } }
        public uint entry_count;
        public SampleEntry SampleEntry;  //  an instance of a class derived from SampleEntry

        public SampleDescriptionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBox(stream, out this.SampleEntry); // an instance of a class derived from SampleEntry
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBox(stream, this.SampleEntry); // an instance of a class derived from SampleEntry
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            boxSize += 32; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.CalculateSize(SampleEntry); // SampleEntry
            }
            return boxSize;
        }
    }


    public class SubTrackSampleGroupBox : FullBox
    {
        public override string FourCC { get { return "stsg"; } }
        public uint grouping_type;
        public ushort item_count;
        public uint group_description_index;

        public SubTrackSampleGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_description_index);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_description_index);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // grouping_type
            boxSize += 16; // item_count

            for (int i = 0; i < item_count; i++)
            {
                boxSize += 32; // group_description_index
            }
            return boxSize;
        }
    }


    public class ShadowSyncSampleBox : FullBox
    {
        public override string FourCC { get { return "stsh"; } }
        public uint entry_count;
        public uint shadowed_sample_number;
        public uint sync_sample_number;

        public ShadowSyncSampleBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.shadowed_sample_number);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sync_sample_number);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.shadowed_sample_number);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sync_sample_number);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += 32; // shadowed_sample_number
                boxSize += 32; // sync_sample_number
            }
            return boxSize;
        }
    }


    public class SyncSampleBox : FullBox
    {
        public override string FourCC { get { return "stss"; } }
        public uint entry_count;
        public uint sample_number;

        public SyncSampleBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_number);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_number);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += 32; // sample_number
            }
            return boxSize;
        }
    }


    public class SampleSizeBox : FullBox
    {
        public override string FourCC { get { return "stsz"; } }
        public uint sample_size;
        public uint sample_count;
        public uint entry_size;

        public SampleSizeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_size);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);

            if (sample_size == 0)
            {

                for (int i = 1; i <= sample_count; i++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_size);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_size);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);

            if (sample_size == 0)
            {

                for (int i = 1; i <= sample_count; i++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_size);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // sample_size
            boxSize += 32; // sample_count

            if (sample_size == 0)
            {

                for (int i = 1; i <= sample_count; i++)
                {
                    boxSize += 32; // entry_size
                }
            }
            return boxSize;
        }
    }


    public class TimeToSampleBox : FullBox
    {
        public override string FourCC { get { return "stts"; } }
        public uint entry_count;
        public uint sample_count;
        public uint sample_delta;

        public TimeToSampleBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_delta);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_delta);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += 32; // sample_count
                boxSize += 32; // sample_delta
            }
            return boxSize;
        }
    }


    public class SegmentTypeBox : GeneralTypeBox
    {
        public override string FourCC { get { return "styp"; } }

        public SegmentTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class CompactSampleSizeBox : FullBox
    {
        public override string FourCC { get { return "stz2"; } }
        public uint reserved = 0;
        public byte field_size;
        public uint sample_count;
        public byte[] entry_size;

        public CompactSampleSizeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt24(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.field_size);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count);

            for (int i = 1; i <= sample_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBytes(stream, field_size, out this.entry_size);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt24(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.field_size);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count);

            for (int i = 1; i <= sample_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBytes(stream, field_size, this.entry_size);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 24; // reserved
            boxSize += 8; // field_size
            boxSize += 32; // sample_count

            for (int i = 1; i <= sample_count; i++)
            {
                boxSize += (ulong)field_size; // entry_size
            }
            return boxSize;
        }
    }


    public class SubSampleInformationBox : FullBox
    {
        public override string FourCC { get { return "subs"; } }
        public uint entry_count;
        public uint sample_delta;
        public ushort subsample_count;
        public uint subsample_size;
        public ushort subsample_size0;
        public byte subsample_priority;
        public byte discardable;
        public uint codec_specific_parameters;

        public SubSampleInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_delta);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subsample_count);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.subsample_size);
                        }

                        else
                        {
                            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subsample_size0);
                        }
                        boxSize += IsoReaderWriter.ReadUInt8(stream, out this.subsample_priority);
                        boxSize += IsoReaderWriter.ReadUInt8(stream, out this.discardable);
                        boxSize += IsoReaderWriter.ReadUInt32(stream, out this.codec_specific_parameters);
                    }
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_delta);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.subsample_count);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            boxSize += IsoReaderWriter.WriteUInt32(stream, this.subsample_size);
                        }

                        else
                        {
                            boxSize += IsoReaderWriter.WriteUInt16(stream, this.subsample_size0);
                        }
                        boxSize += IsoReaderWriter.WriteUInt8(stream, this.subsample_priority);
                        boxSize += IsoReaderWriter.WriteUInt8(stream, this.discardable);
                        boxSize += IsoReaderWriter.WriteUInt32(stream, this.codec_specific_parameters);
                    }
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += 32; // sample_delta
                boxSize += 16; // subsample_count

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            boxSize += 32; // subsample_size
                        }

                        else
                        {
                            boxSize += 16; // subsample_size0
                        }
                        boxSize += 8; // subsample_priority
                        boxSize += 8; // discardable
                        boxSize += 32; // codec_specific_parameters
                    }
                }
            }
            return boxSize;
        }
    }


    public class TrackFragmentBaseMediaDecodeTimeBox : FullBox
    {
        public override string FourCC { get { return "tfdt"; } }
        public ulong baseMediaDecodeTime;
        public uint baseMediaDecodeTime0;

        public TrackFragmentBaseMediaDecodeTimeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.baseMediaDecodeTime);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.baseMediaDecodeTime0);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.baseMediaDecodeTime);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.baseMediaDecodeTime0);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 1)
            {
                boxSize += 64; // baseMediaDecodeTime
            }

            else
            {
                /*  version==0 */
                boxSize += 32; // baseMediaDecodeTime0
            }
            return boxSize;
        }
    }


    public class TrackFragmentHeaderBox : FullBox
    {
        public override string FourCC { get { return "tfhd"; } }
        public uint track_ID;  //  all the following are optional fields
        public ulong base_data_offset;
        public uint sample_description_index;
        public uint default_sample_duration;
        public uint default_sample_size;
        public uint default_sample_flags;

        public TrackFragmentHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID); // all the following are optional fields
            /*  their presence is indicated by bits in the tf_flags */
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.base_data_offset);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_description_index);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_duration);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_size);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_flags);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.track_ID != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID); // all the following are optional fields
            /*  their presence is indicated by bits in the tf_flags */
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.base_data_offset);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_description_index);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_duration);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_size);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_flags);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.track_ID != null) boxSize += 32; // track_ID
            /*  their presence is indicated by bits in the tf_flags */
            boxSize += 64; // base_data_offset
            boxSize += 32; // sample_description_index
            boxSize += 32; // default_sample_duration
            boxSize += 32; // default_sample_size
            boxSize += 32; // default_sample_flags
            return boxSize;
        }
    }


    public class TrackFragmentRandomAccessBox : FullBox
    {
        public override string FourCC { get { return "tfra"; } }
        public uint track_ID;
        public uint reserved = 0;
        public byte length_size_of_traf_num;
        public byte length_size_of_trun_num;
        public byte length_size_of_sample_num;
        public uint number_of_entry;
        public ulong time;
        public ulong moof_offset;
        public uint time0;
        public uint moof_offset0;
        public byte[] traf_number;
        public byte[] trun_number;
        public byte[] sample_delta;

        public TrackFragmentRandomAccessBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID);
            boxSize += IsoReaderWriter.ReadBits(stream, 26, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.length_size_of_traf_num);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.length_size_of_trun_num);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.length_size_of_sample_num);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.number_of_entry);

            for (int i = 1; i <= number_of_entry; i++)
            {

                if (version == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt64(stream, out this.time);
                    boxSize += IsoReaderWriter.ReadUInt64(stream, out this.moof_offset);
                }

                else
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.time0);
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.moof_offset0);
                }
                boxSize += IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_traf_num + 1), out this.traf_number);
                boxSize += IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_trun_num + 1), out this.trun_number);
                boxSize += IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_sample_num + 1), out this.sample_delta);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
            boxSize += IsoReaderWriter.WriteBits(stream, 26, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.length_size_of_traf_num);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.length_size_of_trun_num);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.length_size_of_sample_num);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.number_of_entry);

            for (int i = 1; i <= number_of_entry; i++)
            {

                if (version == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt64(stream, this.time);
                    boxSize += IsoReaderWriter.WriteUInt64(stream, this.moof_offset);
                }

                else
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.time0);
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.moof_offset0);
                }
                boxSize += IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_traf_num + 1), this.traf_number);
                boxSize += IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_trun_num + 1), this.trun_number);
                boxSize += IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_sample_num + 1), this.sample_delta);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_ID
            boxSize += 26; // reserved
            boxSize += 2; // length_size_of_traf_num
            boxSize += 2; // length_size_of_trun_num
            boxSize += 2; // length_size_of_sample_num
            boxSize += 32; // number_of_entry

            for (int i = 1; i <= number_of_entry; i++)
            {

                if (version == 1)
                {
                    boxSize += 64; // time
                    boxSize += 64; // moof_offset
                }

                else
                {
                    boxSize += 32; // time0
                    boxSize += 32; // moof_offset0
                }
                boxSize += (ulong)(length_size_of_traf_num + 1) * 8; // traf_number
                boxSize += (ulong)(length_size_of_trun_num + 1) * 8; // trun_number
                boxSize += (ulong)(length_size_of_sample_num + 1) * 8; // sample_delta
            }
            return boxSize;
        }
    }


    public class TrackHeaderBox : FullBox
    {
        public override string FourCC { get { return "tkhd"; } }
        public ulong creation_time;
        public ulong modification_time;
        public uint track_ID;
        public uint reserved = 0;
        public ulong duration;
        public uint creation_time0;
        public uint modification_time0;
        public uint track_ID0;
        public uint reserved0 = 0;
        public uint duration0;
        public uint[] reserved1 = [];
        public short layer = 0;
        public short alternate_group = 0;
        public short volume = 0; // = { default samplerate of media}<<16;
        public ushort reserved2 = 0;
        public uint[] matrix =
            { 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };  //  unity matrix
        public uint width;
        public uint height;

        public TrackHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.creation_time);
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.modification_time);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.reserved);
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.creation_time0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.modification_time0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.reserved0);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.duration0);
            }
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 2, out this.reserved1);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.layer);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.alternate_group);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.volume);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved2);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 9, out this.matrix); // unity matrix
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.width);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.height);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            if (version == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.reserved);
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.duration);
            }

            else
            {
                /*  version==0 */
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.creation_time0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.modification_time0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.reserved0);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.duration0);
            }
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved1);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.layer);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.alternate_group);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.volume);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved2);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 9, this.matrix); // unity matrix
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.width);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.height);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            if (version == 1)
            {
                boxSize += 64; // creation_time
                boxSize += 64; // modification_time
                boxSize += 32; // track_ID
                boxSize += 32; // reserved
                boxSize += 64; // duration
            }

            else
            {
                /*  version==0 */
                boxSize += 32; // creation_time0
                boxSize += 32; // modification_time0
                boxSize += 32; // track_ID0
                boxSize += 32; // reserved0
                boxSize += 32; // duration0
            }
            boxSize += 2 * 32; // reserved1
            boxSize += 16; // layer
            boxSize += 16; // alternate_group
            boxSize += 16; // volume
            boxSize += 16; // reserved2
            boxSize += 9 * 32; // matrix
            boxSize += 32; // width
            boxSize += 32; // height
            return boxSize;
        }
    }


    public class TrackFragmentBox : Box
    {
        public override string FourCC { get { return "traf"; } }

        public TrackFragmentBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class TrackBox : Box
    {
        public override string FourCC { get { return "trak"; } }

        public TrackBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class TrackReferenceBox : Box
    {
        public override string FourCC { get { return "tref"; } }
        public TrackReferenceTypeBox[] TrackReferenceTypeBox;

        public TrackReferenceBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.TrackReferenceTypeBox);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.TrackReferenceTypeBox);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(TrackReferenceTypeBox); // TrackReferenceTypeBox
            return boxSize;
        }
    }


    public class TrackExtensionPropertiesBox : FullBox
    {
        public override string FourCC { get { return "trep"; } }
        public uint track_ID;  //  Any number of boxes may follow

        public TrackExtensionPropertiesBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID); // Any number of boxes may follow
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID); // Any number of boxes may follow
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_ID
            return boxSize;
        }
    }


    public class TrackExtendsBox : FullBox
    {
        public override string FourCC { get { return "trex"; } }
        public uint track_ID;
        public uint default_sample_description_index;
        public uint default_sample_duration;
        public uint default_sample_size;
        public uint default_sample_flags;

        public TrackExtendsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_ID);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_description_index);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_duration);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_size);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.default_sample_flags);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_description_index);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_duration);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_size);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.default_sample_flags);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_ID
            boxSize += 32; // default_sample_description_index
            boxSize += 32; // default_sample_duration
            boxSize += 32; // default_sample_size
            boxSize += 32; // default_sample_flags
            return boxSize;
        }
    }


    public class TrackGroupBox : Box
    {
        public override string FourCC { get { return "trgr"; } }

        public TrackGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class TrackRunBox : FullBox
    {
        public override string FourCC { get { return "trun"; } }
        public uint sample_count;  //  the following are optional fields
        public int data_offset;
        public uint first_sample_flags;  //  all fields in the following array are optional

        public TrackRunBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_count); // the following are optional fields
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.data_offset);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.first_sample_flags); // all fields in the following array are optional
            /*  as indicated by bits set in the tr_flags */

            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.sample_count != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_count); // the following are optional fields
            boxSize += IsoReaderWriter.WriteInt32(stream, this.data_offset);
            if (this.first_sample_flags != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.first_sample_flags); // all fields in the following array are optional
            /*  as indicated by bits set in the tr_flags */

            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.sample_count != null) boxSize += 32; // sample_count
            boxSize += 32; // data_offset
            if (this.first_sample_flags != null) boxSize += 32; // first_sample_flags
            /*  as indicated by bits set in the tr_flags */

            return boxSize;
        }
    }


    public class TrackTypeBox : GeneralTypeBox
    {
        public override string FourCC { get { return "ttyp"; } }

        public TrackTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class TypeCombinationBox : Box
    {
        public override string FourCC { get { return "tyco"; } }
        public uint[] compatible_brands;  //  to end of the box

        public TypeCombinationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.compatible_brands); // to end of the box
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.compatible_brands); // to end of the box
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // compatible_brands
            return boxSize;
        }
    }


    public class UserDataBox : Box
    {
        public override string FourCC { get { return "udta"; } }

        public UserDataBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class BoxHeader
    {

        public uint size;
        public uint type; // = boxtype
        public ulong largesize;
        public byte[] usertype; // = extended_type

        public BoxHeader()
        { }

        public async Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.size);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.type);

            if (size == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt64(stream, out this.largesize);
            }

            else if (size == 0)
            {
                /*  box extends to end of file */
            }

            if (type == IsoReaderWriter.FromFourCC("uuid"))
            {
                boxSize += IsoReaderWriter.ReadBytes(stream, 16, out this.usertype);
            }
            return boxSize;
        }

        public async Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.size);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.type);

            if (size == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt64(stream, this.largesize);
            }

            else if (size == 0)
            {
                /*  box extends to end of file */
            }

            if (type == IsoReaderWriter.FromFourCC("uuid"))
            {
                boxSize += IsoReaderWriter.WriteBytes(stream, 16, this.usertype);
            }
            return boxSize;
        }

        public ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += 32; // size
            boxSize += 32; // type

            if (size == 1)
            {
                boxSize += 64; // largesize
            }

            else if (size == 0)
            {
                /*  box extends to end of file */
            }

            if (type == IsoReaderWriter.FromFourCC("uuid"))
            {
                boxSize += 16 * 8; // usertype
            }
            return boxSize;
        }
    }


    public class VideoMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "vmhd"; } }
        public ushort graphicsmode = 0;  //  copy, see below
        public ushort[] opcolor = { 0, 0, 0 };

        public VideoMediaHeaderBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.graphicsmode); // copy, see below
            boxSize += IsoReaderWriter.ReadUInt16Array(stream, 3, out this.opcolor);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.graphicsmode); // copy, see below
            boxSize += IsoReaderWriter.WriteUInt16Array(stream, 3, this.opcolor);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // graphicsmode
            boxSize += 3 * 16; // opcolor
            return boxSize;
        }
    }


    public class XMLBox : FullBox
    {
        public override string FourCC { get { return "xml "; } }
        public string xml;

        public XMLBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.xml);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.xml);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)xml.Length * 8; // xml
            return boxSize;
        }
    }


    public class CompressedMovieFragmentBox1 : CompressedBox
    {
        public override string FourCC { get { return "!mof"; } }

        public CompressedMovieFragmentBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class CompressedMovieBox1 : CompressedBox
    {
        public override string FourCC { get { return "!mov"; } }

        public CompressedMovieBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class CompressedSegmentIndexBox1 : CompressedBox
    {
        public override string FourCC { get { return "!six"; } }

        public CompressedSegmentIndexBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class CompressedSubsegmentIndexBox1 : CompressedBox
    {
        public override string FourCC { get { return "!ssx"; } }

        public CompressedSubsegmentIndexBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class AmbientViewingEnvironmentBox : Box
    {
        public override string FourCC { get { return "amve"; } }
        public uint ambient_illuminance;
        public ushort ambient_light_x;
        public ushort ambient_light_y;

        public AmbientViewingEnvironmentBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ambient_illuminance);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.ambient_light_x);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.ambient_light_y);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.ambient_illuminance);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.ambient_light_x);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.ambient_light_y);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // ambient_illuminance
            boxSize += 16; // ambient_light_x
            boxSize += 16; // ambient_light_y
            return boxSize;
        }
    }


    public class MetadataKeyTableBox : Box
    {
        public override string FourCC { get { return "keys"; } }
        public MetadataKeyBox[] MetadataKeyBox;

        public MetadataKeyTableBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.MetadataKeyBox);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.MetadataKeyBox);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(MetadataKeyBox); // MetadataKeyBox
            return boxSize;
        }
    }


    public class URIBox : FullBox
    {
        public override string FourCC { get { return "uri "; } }
        public string theURI;

        public URIBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.theURI);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.theURI);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)theURI.Length * 8; // theURI
            return boxSize;
        }
    }


    public class IroiInfoBox : Box
    {
        public override string FourCC { get { return "iroi"; } }
        public byte iroi_type;
        public byte reserved = 0;
        public byte grid_roi_mb_width;
        public byte grid_roi_mb_height;
        public uint num_roi;
        public uint top_left_mb;
        public byte roi_mb_width;
        public byte roi_mb_height;

        public IroiInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.iroi_type);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);

            if (iroi_type == 0)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.grid_roi_mb_width);
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.grid_roi_mb_height);
            }

            else if (iroi_type == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt24(stream, out this.num_roi);

                for (int i = 1; i <= num_roi; i++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.top_left_mb);
                    boxSize += IsoReaderWriter.ReadUInt8(stream, out this.roi_mb_width);
                    boxSize += IsoReaderWriter.ReadUInt8(stream, out this.roi_mb_height);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.iroi_type);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);

            if (iroi_type == 0)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.grid_roi_mb_width);
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.grid_roi_mb_height);
            }

            else if (iroi_type == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt24(stream, this.num_roi);

                for (int i = 1; i <= num_roi; i++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.top_left_mb);
                    boxSize += IsoReaderWriter.WriteUInt8(stream, this.roi_mb_width);
                    boxSize += IsoReaderWriter.WriteUInt8(stream, this.roi_mb_height);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2; // iroi_type
            boxSize += 6; // reserved

            if (iroi_type == 0)
            {
                boxSize += 8; // grid_roi_mb_width
                boxSize += 8; // grid_roi_mb_height
            }

            else if (iroi_type == 1)
            {
                boxSize += 24; // num_roi

                for (int i = 1; i <= num_roi; i++)
                {
                    boxSize += 32; // top_left_mb
                    boxSize += 8; // roi_mb_width
                    boxSize += 8; // roi_mb_height
                }
            }
            return boxSize;
        }
    }


    public class TierDependencyBox : Box
    {
        public override string FourCC { get { return "ldep"; } }
        public ushort entry_count;
        public ushort dependencyTierId;

        public TierDependencyBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);

            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.dependencyTierId);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);

            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.dependencyTierId);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_count

            for (int i = 0; i < entry_count; i++)
            {
                boxSize += 16; // dependencyTierId
            }
            return boxSize;
        }
    }


    public class SVCDependencyRangeBox : Box
    {
        public override string FourCC { get { return "svdr"; } }
        public byte min_dependency_id;
        public byte min_temporal_id;
        public byte reserved = 0;
        public byte min_quality_id;
        public byte max_dependency_id;
        public byte max_temporal_id;
        public byte reserved0 = 0;
        public byte max_quality_id;

        public SVCDependencyRangeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.min_dependency_id);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.min_temporal_id);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.min_quality_id);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.max_dependency_id);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.max_temporal_id);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved0);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.max_quality_id);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.min_dependency_id);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.min_temporal_id);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.min_quality_id);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.max_dependency_id);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.max_temporal_id);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved0);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.max_quality_id);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 3; // min_dependency_id
            boxSize += 3; // min_temporal_id
            boxSize += 6; // reserved
            boxSize += 4; // min_quality_id
            boxSize += 3; // max_dependency_id
            boxSize += 3; // max_temporal_id
            boxSize += 6; // reserved0
            boxSize += 4; // max_quality_id
            return boxSize;
        }
    }


    public class InitialParameterSetBox : Box
    {
        public override string FourCC { get { return "svip"; } }
        public byte sps_id_count;
        public byte SPS_index;
        public byte pps_id_count;
        public byte PPS_index;

        public InitialParameterSetBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.sps_id_count);

            for (int i = 0; i < sps_id_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.SPS_index);
            }
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.pps_id_count);

            for (int i = 0; i < pps_id_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.PPS_index);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.sps_id_count);

            for (int i = 0; i < sps_id_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.SPS_index);
            }
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.pps_id_count);

            for (int i = 0; i < pps_id_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.PPS_index);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // sps_id_count

            for (int i = 0; i < sps_id_count; i++)
            {
                boxSize += 8; // SPS_index
            }
            boxSize += 8; // pps_id_count

            for (int i = 0; i < pps_id_count; i++)
            {
                boxSize += 8; // PPS_index
            }
            return boxSize;
        }
    }


    public class PriorityRangeBox : Box
    {
        public override string FourCC { get { return "svpr"; } }
        public byte reserved1 = 0;
        public byte min_priorityId;
        public byte reserved2 = 0;
        public byte max_priorityId;

        public PriorityRangeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved1);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.min_priorityId);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved2);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.max_priorityId);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved1);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.min_priorityId);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved2);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.max_priorityId);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2; // reserved1
            boxSize += 6; // min_priorityId
            boxSize += 2; // reserved2
            boxSize += 6; // max_priorityId
            return boxSize;
        }
    }


    public class TranscodingInfoBox : Box
    {
        public override string FourCC { get { return "tran"; } }
        public byte reserved = 0;
        public byte conversion_idc;
        public bool cavlc_info_present_flag;
        public bool cabac_info_present_flag;
        public uint cavlc_profile_level_idc;
        public uint cavlc_max_bitrate;
        public uint cavlc_avg_bitrate;
        public uint cabac_profile_level_idc;
        public uint cabac_max_bitrate;
        public uint cabac_avg_bitrate;

        public TranscodingInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.conversion_idc);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.cavlc_info_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.cabac_info_present_flag);

            if (cavlc_info_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt24(stream, out this.cavlc_profile_level_idc);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cavlc_max_bitrate);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cavlc_avg_bitrate);
            }

            if (cabac_info_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt24(stream, out this.cabac_profile_level_idc);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cabac_max_bitrate);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cabac_avg_bitrate);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.conversion_idc);
            boxSize += IsoReaderWriter.WriteBit(stream, this.cavlc_info_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.cabac_info_present_flag);

            if (cavlc_info_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt24(stream, this.cavlc_profile_level_idc);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.cavlc_max_bitrate);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.cavlc_avg_bitrate);
            }

            if (cabac_info_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt24(stream, this.cabac_profile_level_idc);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.cabac_max_bitrate);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.cabac_avg_bitrate);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 4; // reserved
            boxSize += 2; // conversion_idc
            boxSize += 1; // cavlc_info_present_flag
            boxSize += 1; // cabac_info_present_flag

            if (cavlc_info_present_flag)
            {
                boxSize += 24; // cavlc_profile_level_idc
                boxSize += 32; // cavlc_max_bitrate
                boxSize += 32; // cavlc_avg_bitrate
            }

            if (cabac_info_present_flag)
            {
                boxSize += 24; // cabac_profile_level_idc
                boxSize += 32; // cabac_max_bitrate
                boxSize += 32; // cabac_avg_bitrate
            }
            return boxSize;
        }
    }


    public class RectRegionBox : Box
    {
        public override string FourCC { get { return "rrgn"; } }
        public ushort base_region_tierID;
        public bool dynamic_rect;
        public byte reserved = 0;
        public ushort horizontal_offset;
        public ushort vertical_offset;
        public ushort region_width;
        public ushort region_height;

        public RectRegionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.base_region_tierID);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.dynamic_rect);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);

            if (dynamic_rect == false)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.horizontal_offset);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.vertical_offset);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.region_width);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.region_height);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.base_region_tierID);
            boxSize += IsoReaderWriter.WriteBit(stream, this.dynamic_rect);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);

            if (dynamic_rect == false)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.horizontal_offset);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.vertical_offset);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.region_width);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.region_height);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // base_region_tierID
            boxSize += 1; // dynamic_rect
            boxSize += 7; // reserved

            if (dynamic_rect == false)
            {
                boxSize += 16; // horizontal_offset
                boxSize += 16; // vertical_offset
                boxSize += 16; // region_width
                boxSize += 16; // region_height
            }
            return boxSize;
        }
    }


    public class BufferingBox : Box
    {
        public override string FourCC { get { return "buff"; } }
        public ushort operating_point_count;
        public uint byte_rate;
        public uint cpb_size;
        public uint dpb_size;
        public uint init_cpb_delay;
        public uint init_dpb_delay;

        public BufferingBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.operating_point_count);

            for (int i = 0; i < operating_point_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.byte_rate);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cpb_size);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.dpb_size);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.init_cpb_delay);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.init_dpb_delay);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.operating_point_count);

            for (int i = 0; i < operating_point_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.byte_rate);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.cpb_size);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.dpb_size);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.init_cpb_delay);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.init_dpb_delay);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // operating_point_count

            for (int i = 0; i < operating_point_count; i++)
            {
                boxSize += 32; // byte_rate
                boxSize += 32; // cpb_size
                boxSize += 32; // dpb_size
                boxSize += 32; // init_cpb_delay
                boxSize += 32; // init_dpb_delay
            }
            return boxSize;
        }
    }


    public class MVCSubTrackViewBox : FullBox
    {
        public override string FourCC { get { return "mstv"; } }
        public ushort item_count;
        public ushort view_id;
        public byte temporal_id;
        public byte reserved;

        public MVCSubTrackViewBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.view_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.temporal_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 10, this.view_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.temporal_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // item_count

            for (int i = 0; i < item_count; i++)
            {
                boxSize += 10; // view_id
                boxSize += 4; // temporal_id
                boxSize += 2; // reserved
            }
            return boxSize;
        }
    }


    public class MultiviewGroupBox : FullBox
    {
        public override string FourCC { get { return "mvcg"; } }
        public uint multiview_group_id;
        public ushort num_entries;
        public byte reserved = 0;
        public byte entry_type;
        public uint track_id;
        public uint track_id0;
        public ushort tier_id;
        public byte reserved1 = 0;
        public ushort output_view_id;
        public byte reserved2 = 0;
        public ushort start_view_id;
        public ushort view_count;
        public TierInfoBox subset_stream_info;  //  optional
        public MultiviewRelationAttributeBox relation_attributes;  //  optional
        public TierBitRateBox subset_stream_bit_rate;  //  optional
        public BufferingBox subset_stream_buffering;  //  optional
        public MultiviewSceneInfoBox multiview_scene_info;  //  optional

        public MultiviewGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.multiview_group_id);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_entries);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.reserved);

            for (int i = 0; i < num_entries; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entry_type);

                if (entry_type == 0)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_id);
                }

                else if (entry_type == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_id0);
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tier_id);
                }

                else if (entry_type == 2)
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved1);
                    boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.output_view_id);
                }

                else if (entry_type == 3)
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved2);
                    boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.start_view_id);
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.view_count);
                }
            }
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.subset_stream_info); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.relation_attributes); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.subset_stream_bit_rate); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.subset_stream_buffering); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.multiview_scene_info); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.multiview_group_id);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_entries);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.reserved);

            for (int i = 0; i < num_entries; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.entry_type);

                if (entry_type == 0)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_id);
                }

                else if (entry_type == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_id0);
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.tier_id);
                }

                else if (entry_type == 2)
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved1);
                    boxSize += IsoReaderWriter.WriteBits(stream, 10, this.output_view_id);
                }

                else if (entry_type == 3)
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved2);
                    boxSize += IsoReaderWriter.WriteBits(stream, 10, this.start_view_id);
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.view_count);
                }
            }
            if (this.subset_stream_info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.subset_stream_info); // optional
            if (this.relation_attributes != null) boxSize += IsoReaderWriter.WriteBox(stream, this.relation_attributes); // optional
            if (this.subset_stream_bit_rate != null) boxSize += IsoReaderWriter.WriteBox(stream, this.subset_stream_bit_rate); // optional
            if (this.subset_stream_buffering != null) boxSize += IsoReaderWriter.WriteBox(stream, this.subset_stream_buffering); // optional
            if (this.multiview_scene_info != null) boxSize += IsoReaderWriter.WriteBox(stream, this.multiview_scene_info); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // multiview_group_id
            boxSize += 16; // num_entries
            boxSize += 8; // reserved

            for (int i = 0; i < num_entries; i++)
            {
                boxSize += 8; // entry_type

                if (entry_type == 0)
                {
                    boxSize += 32; // track_id
                }

                else if (entry_type == 1)
                {
                    boxSize += 32; // track_id0
                    boxSize += 16; // tier_id
                }

                else if (entry_type == 2)
                {
                    boxSize += 6; // reserved1
                    boxSize += 10; // output_view_id
                }

                else if (entry_type == 3)
                {
                    boxSize += 6; // reserved2
                    boxSize += 10; // start_view_id
                    boxSize += 16; // view_count
                }
            }
            if (this.subset_stream_info != null) boxSize += IsoReaderWriter.CalculateSize(subset_stream_info); // subset_stream_info
            if (this.relation_attributes != null) boxSize += IsoReaderWriter.CalculateSize(relation_attributes); // relation_attributes
            if (this.subset_stream_bit_rate != null) boxSize += IsoReaderWriter.CalculateSize(subset_stream_bit_rate); // subset_stream_bit_rate
            if (this.subset_stream_buffering != null) boxSize += IsoReaderWriter.CalculateSize(subset_stream_buffering); // subset_stream_buffering
            if (this.multiview_scene_info != null) boxSize += IsoReaderWriter.CalculateSize(multiview_scene_info); // multiview_scene_info
            return boxSize;
        }
    }


    public class MultiviewInformationBox : FullBox
    {
        public override string FourCC { get { return "mvci"; } }

        public MultiviewInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class MVDDepthResolutionBox : Box
    {
        public override string FourCC { get { return "3dpr"; } }
        public ushort depth_width;
        public ushort depth_height;
        public ushort depth_hor_mult_minus1;  //  optional
        public ushort depth_ver_mult_minus1;  //  optional
        public byte depth_hor_rsh;  //  optional
        public byte depth_ver_rsh;  //  optional
        public ushort grid_pos_num_views;  //  optional
        public byte reserved = 0;
        public ushort[] grid_pos_view_id;
        public short[] grid_pos_x;
        public short[] grid_pos_y;

        public MVDDepthResolutionBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.depth_width);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.depth_height);
            /*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt16(stream, out this.depth_hor_mult_minus1); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt16(stream, out this.depth_ver_mult_minus1); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.depth_hor_rsh); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.depth_ver_rsh); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt16(stream, out this.grid_pos_num_views); // optional

            for (int i = 0; i < grid_pos_num_views; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
                boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.grid_pos_view_id[i]);
                boxSize += IsoReaderWriter.ReadInt16(stream, out this.grid_pos_x[grid_pos_view_id[i]]);
                boxSize += IsoReaderWriter.ReadInt16(stream, out this.grid_pos_y[grid_pos_view_id[i]]);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.depth_width);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.depth_height);
            /*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
            if (this.depth_hor_mult_minus1 != null) boxSize += IsoReaderWriter.WriteUInt16(stream, this.depth_hor_mult_minus1); // optional
            if (this.depth_ver_mult_minus1 != null) boxSize += IsoReaderWriter.WriteUInt16(stream, this.depth_ver_mult_minus1); // optional
            if (this.depth_hor_rsh != null) boxSize += IsoReaderWriter.WriteBits(stream, 4, this.depth_hor_rsh); // optional
            if (this.depth_ver_rsh != null) boxSize += IsoReaderWriter.WriteBits(stream, 4, this.depth_ver_rsh); // optional
            if (this.grid_pos_num_views != null) boxSize += IsoReaderWriter.WriteUInt16(stream, this.grid_pos_num_views); // optional

            for (int i = 0; i < grid_pos_num_views; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
                boxSize += IsoReaderWriter.WriteBits(stream, 10, this.grid_pos_view_id[i]);
                boxSize += IsoReaderWriter.WriteInt16(stream, this.grid_pos_x[grid_pos_view_id[i]]);
                boxSize += IsoReaderWriter.WriteInt16(stream, this.grid_pos_y[grid_pos_view_id[i]]);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // depth_width
            boxSize += 16; // depth_height
            /*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
            if (this.depth_hor_mult_minus1 != null) boxSize += 16; // depth_hor_mult_minus1
            if (this.depth_ver_mult_minus1 != null) boxSize += 16; // depth_ver_mult_minus1
            if (this.depth_hor_rsh != null) boxSize += 4; // depth_hor_rsh
            if (this.depth_ver_rsh != null) boxSize += 4; // depth_ver_rsh
            if (this.grid_pos_num_views != null) boxSize += 16; // grid_pos_num_views

            for (int i = 0; i < grid_pos_num_views; i++)
            {
                boxSize += 6; // reserved
                boxSize += 10; // grid_pos_view_id
                boxSize += 16; // grid_pos_x
                boxSize += 16; // grid_pos_y
            }
            return boxSize;
        }
    }


    public class MultiviewRelationAttributeBox : FullBox
    {
        public override string FourCC { get { return "mvra"; } }
        public ushort reserved1 = 0;
        public ushort num_common_attributes;
        public uint common_attribute;
        public uint common_value;
        public ushort reserved2 = 0;
        public ushort num_differentiating_attributes;
        public uint differentiating_attribute;

        public MultiviewRelationAttributeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved1);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_common_attributes);

            for (int i = 0; i < num_common_attributes; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.common_attribute);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.common_value);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved2);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_differentiating_attributes);

            for (int i = 0; i < num_differentiating_attributes; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.differentiating_attribute);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved1);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_common_attributes);

            for (int i = 0; i < num_common_attributes; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.common_attribute);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.common_value);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved2);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_differentiating_attributes);

            for (int i = 0; i < num_differentiating_attributes; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.differentiating_attribute);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // reserved1
            boxSize += 16; // num_common_attributes

            for (int i = 0; i < num_common_attributes; i++)
            {
                boxSize += 32; // common_attribute
                boxSize += 32; // common_value
            }
            boxSize += 16; // reserved2
            boxSize += 16; // num_differentiating_attributes

            for (int i = 0; i < num_differentiating_attributes; i++)
            {
                boxSize += 32; // differentiating_attribute
            }
            return boxSize;
        }
    }


    public class SampleDependencyBox : FullBox
    {
        public override string FourCC { get { return "sdep"; } }
        public ushort dependency_count;
        public short relative_sample_number;

        public SampleDependencyBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            int sample_count = 0; // TODO: taken from the stsz sample_count


            for (int i = 0; i < sample_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.dependency_count);

                for (int k = 0; k < dependency_count; k++)
                {
                    boxSize += IsoReaderWriter.ReadInt16(stream, out this.relative_sample_number);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            int sample_count = 0; // TODO: taken from the stsz sample_count


            for (int i = 0; i < sample_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.dependency_count);

                for (int k = 0; k < dependency_count; k++)
                {
                    boxSize += IsoReaderWriter.WriteInt16(stream, this.relative_sample_number);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            int sample_count = 0; // TODO: taken from the stsz sample_count


            for (int i = 0; i < sample_count; i++)
            {
                boxSize += 16; // dependency_count

                for (int k = 0; k < dependency_count; k++)
                {
                    boxSize += 16; // relative_sample_number
                }
            }
            return boxSize;
        }
    }


    public class SeiInformationBox : Box
    {
        public override string FourCC { get { return "seii"; } }
        public ushort numRequiredSEIs;
        public ushort requiredSEI_ID;
        public ushort numNotRequiredSEIs;
        public ushort notrequiredSEI_ID;

        public SeiInformationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.numRequiredSEIs);

            for (int i = 0; i < numRequiredSEIs; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.requiredSEI_ID);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.numNotRequiredSEIs);

            for (int i = 0; i < numNotRequiredSEIs; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.notrequiredSEI_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.numRequiredSEIs);

            for (int i = 0; i < numRequiredSEIs; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.requiredSEI_ID);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.numNotRequiredSEIs);

            for (int i = 0; i < numNotRequiredSEIs; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.notrequiredSEI_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // numRequiredSEIs

            for (int i = 0; i < numRequiredSEIs; i++)
            {
                boxSize += 16; // requiredSEI_ID
            }
            boxSize += 16; // numNotRequiredSEIs

            for (int i = 0; i < numNotRequiredSEIs; i++)
            {
                boxSize += 16; // notrequiredSEI_ID
            }
            return boxSize;
        }
    }


    public class SVCSubTrackLayerBox : FullBox
    {
        public override string FourCC { get { return "sstl"; } }
        public ushort item_count;
        public byte dependency_id;
        public byte quality_id;
        public byte temporal_id;
        public byte priority_id;
        public byte dependency_id_range;
        public byte quality_id_range;
        public byte temporal_id_range;
        public byte priority_id_range;

        public SVCSubTrackLayerBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.dependency_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.quality_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.temporal_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.priority_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.dependency_id_range);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.quality_id_range);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.temporal_id_range);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.priority_id_range);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.dependency_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 4, this.quality_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.temporal_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 6, this.priority_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.dependency_id_range);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.quality_id_range);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.temporal_id_range);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.priority_id_range);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // item_count

            for (int i = 0; i < item_count; i++)
            {
                boxSize += 3; // dependency_id
                boxSize += 4; // quality_id
                boxSize += 3; // temporal_id
                boxSize += 6; // priority_id
                boxSize += 2; // dependency_id_range
                boxSize += 2; // quality_id_range
                boxSize += 2; // temporal_id_range
                boxSize += 2; // priority_id_range
            }
            return boxSize;
        }
    }


    public class MVCSubTrackMultiviewGroupBox : FullBox
    {
        public override string FourCC { get { return "stmg"; } }
        public ushort item_count;
        public uint MultiviewGroupId;

        public MVCSubTrackMultiviewGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.MultiviewGroupId);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.MultiviewGroupId);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // item_count

            for (int i = 0; i < item_count; i++)
            {
                boxSize += 32; // MultiviewGroupId
            }
            return boxSize;
        }
    }


    public class SubTrackTierBox : FullBox
    {
        public override string FourCC { get { return "stti"; } }
        public ushort item_count;
        public ushort tierID;

        public SubTrackTierBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tierID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.tierID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // item_count

            for (int i = 0; i < item_count; i++)
            {
                boxSize += 16; // tierID
            }
            return boxSize;
        }
    }


    public class MultiviewGroupRelationBox : FullBox
    {
        public override string FourCC { get { return "swtc"; } }
        public uint num_entries;
        public uint multiview_group_id;
        public MultiviewRelationAttributeBox relation_attributes;

        public MultiviewGroupRelationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entries);

            for (int i = 0; i < num_entries; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.multiview_group_id);
            }
            boxSize += IsoReaderWriter.ReadBox(stream, out this.relation_attributes);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

            for (int i = 0; i < num_entries; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.multiview_group_id);
            }
            boxSize += IsoReaderWriter.WriteBox(stream, this.relation_attributes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // num_entries

            for (int i = 0; i < num_entries; i++)
            {
                boxSize += 32; // multiview_group_id
            }
            boxSize += IsoReaderWriter.CalculateSize(relation_attributes); // relation_attributes
            return boxSize;
        }
    }


    public class TierBitRateBox : Box
    {
        public override string FourCC { get { return "tibr"; } }
        public uint baseBitRate;
        public uint maxBitRate;
        public uint avgBitRate;
        public uint tierBaseBitRate;
        public uint tierMaxBitRate;
        public uint tierAvgBitRate;

        public TierBitRateBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.baseBitRate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxBitRate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.avgBitRate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.tierBaseBitRate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.tierMaxBitRate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.tierAvgBitRate);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.baseBitRate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxBitRate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.avgBitRate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.tierBaseBitRate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.tierMaxBitRate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.tierAvgBitRate);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // baseBitRate
            boxSize += 32; // maxBitRate
            boxSize += 32; // avgBitRate
            boxSize += 32; // tierBaseBitRate
            boxSize += 32; // tierMaxBitRate
            boxSize += 32; // tierAvgBitRate
            return boxSize;
        }
    }


    public class TierInfoBox : Box
    {
        public override string FourCC { get { return "tiri"; } }
        public ushort tierID;
        public byte profileIndication;
        public byte profile_compatibility;
        public byte levelIndication;
        public byte reserved = 0;
        public ushort visualWidth;
        public ushort visualHeight;
        public byte discardable;
        public byte constantFrameRate;
        public byte reserved0 = 0;
        public ushort frameRate;

        public TierInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /* Mandatory Box */
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tierID);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.profileIndication);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.profile_compatibility);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.levelIndication);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.visualWidth);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.visualHeight);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.discardable);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.constantFrameRate);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved0);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.frameRate);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /* Mandatory Box */
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.tierID);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.profileIndication);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.profile_compatibility);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.levelIndication);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.visualWidth);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.visualHeight);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.discardable);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.constantFrameRate);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved0);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.frameRate);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /* Mandatory Box */
            boxSize += 16; // tierID
            boxSize += 8; // profileIndication
            boxSize += 8; // profile_compatibility
            boxSize += 8; // levelIndication
            boxSize += 8; // reserved
            boxSize += 16; // visualWidth
            boxSize += 16; // visualHeight
            boxSize += 2; // discardable
            boxSize += 2; // constantFrameRate
            boxSize += 4; // reserved0
            boxSize += 16; // frameRate
            return boxSize;
        }
    }


    public class TileSubTrackGroupBox : FullBox
    {
        public override string FourCC { get { return "tstb"; } }
        public ushort item_count;
        public ushort tileGroupID;

        public TileSubTrackGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tileGroupID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.tileGroupID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // item_count

            for (int i = 0; i < item_count; i++)
            {
                boxSize += 16; // tileGroupID
            }
            return boxSize;
        }
    }


    public class MultiviewSceneInfoBox : Box
    {
        public override string FourCC { get { return "vwdi"; } }
        public byte max_disparity;

        public MultiviewSceneInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.max_disparity);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.max_disparity);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // max_disparity
            return boxSize;
        }
    }


    public class MVCDConfigurationBox : Box
    {
        public override string FourCC { get { return "mvdC"; } }
        public MVDDecoderConfigurationRecord MVDConfig;
        public MVDDepthResolutionBox mvdDepthRes;  // Optional

        public MVCDConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.MVDConfig);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdDepthRes); //Optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.MVDConfig);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvdDepthRes); //Optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(MVDConfig); // MVDConfig
            boxSize += IsoReaderWriter.CalculateSize(mvdDepthRes); // mvdDepthRes
            return boxSize;
        }
    }


    public class A3DConfigurationBox : Box
    {
        public override string FourCC { get { return "a3dC"; } }
        public MVDDecoderConfigurationRecord MVDConfig;
        public MVDDepthResolutionBox mvdDepthRes;  // Optional

        public A3DConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.MVDConfig);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdDepthRes); //Optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.MVDConfig);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvdDepthRes); //Optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(MVDConfig); // MVDConfig
            boxSize += IsoReaderWriter.CalculateSize(mvdDepthRes); // mvdDepthRes
            return boxSize;
        }
    }


    public class ViewIdentifierBox : FullBox
    {
        public override string FourCC { get { return "vwid"; } }
        public byte reserved6 = 0;
        public byte min_temporal_id;
        public byte max_temporal_id;
        public ushort num_views;
        public byte reserved1 = 0;
        public ushort[] view_id;
        public byte reserved2 = 0;
        public ushort view_order_index;
        public bool[] texture_in_stream;
        public bool[] texture_in_track;
        public bool[] depth_in_stream;
        public bool[] depth_in_track;
        public byte base_view_type;
        public ushort num_ref_views;
        public byte reserved5 = 0;
        public byte[][] dependent_component_idc;
        public ushort[][] ref_view_id;

        public ViewIdentifierBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved6);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.min_temporal_id);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.max_temporal_id);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_views);

            for (int i = 0; i < num_views; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved1);
                boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.view_id[i]);
                boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved2);
                boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.view_order_index);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.texture_in_stream[i]);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.texture_in_track[i]);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.depth_in_stream[i]);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.depth_in_track[i]);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.base_view_type);
                boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.num_ref_views);

                for (int j = 0; j < num_ref_views; j++)
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved5);
                    boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.dependent_component_idc[i][j]);
                    boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.ref_view_id[i][j]);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved6);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.min_temporal_id);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.max_temporal_id);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_views);

            for (int i = 0; i < num_views; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved1);
                boxSize += IsoReaderWriter.WriteBits(stream, 10, this.view_id[i]);
                boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved2);
                boxSize += IsoReaderWriter.WriteBits(stream, 10, this.view_order_index);
                boxSize += IsoReaderWriter.WriteBit(stream, this.texture_in_stream[i]);
                boxSize += IsoReaderWriter.WriteBit(stream, this.texture_in_track[i]);
                boxSize += IsoReaderWriter.WriteBit(stream, this.depth_in_stream[i]);
                boxSize += IsoReaderWriter.WriteBit(stream, this.depth_in_track[i]);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.base_view_type);
                boxSize += IsoReaderWriter.WriteBits(stream, 10, this.num_ref_views);

                for (int j = 0; j < num_ref_views; j++)
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved5);
                    boxSize += IsoReaderWriter.WriteBits(stream, 2, this.dependent_component_idc[i][j]);
                    boxSize += IsoReaderWriter.WriteBits(stream, 10, this.ref_view_id[i][j]);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2; // reserved6
            boxSize += 3; // min_temporal_id
            boxSize += 3; // max_temporal_id
            boxSize += 16; // num_views

            for (int i = 0; i < num_views; i++)
            {
                boxSize += 6; // reserved1
                boxSize += 10; // view_id
                boxSize += 6; // reserved2
                boxSize += 10; // view_order_index
                boxSize += 1; // texture_in_stream
                boxSize += 1; // texture_in_track
                boxSize += 1; // depth_in_stream
                boxSize += 1; // depth_in_track
                boxSize += 2; // base_view_type
                boxSize += 10; // num_ref_views

                for (int j = 0; j < num_ref_views; j++)
                {
                    boxSize += 4; // reserved5
                    boxSize += 2; // dependent_component_idc
                    boxSize += 10; // ref_view_id
                }
            }
            return boxSize;
        }
    }


    public class MVCConfigurationBox : Box
    {
        public override string FourCC { get { return "mvcC"; } }
        public MVCDecoderConfigurationRecord MVCConfig;

        public MVCConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.MVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.MVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(MVCConfig); // MVCConfig
            return boxSize;
        }
    }


    public class AVCConfigurationBox : Box
    {
        public override string FourCC { get { return "avcC"; } }
        public AVCDecoderConfigurationRecord AVCConfig;

        public AVCConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.AVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.AVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(AVCConfig); // AVCConfig
            return boxSize;
        }
    }


    public class HEVCConfigurationBox : Box
    {
        public override string FourCC { get { return "hvcC"; } }
        public HEVCDecoderConfigurationRecord HEVCConfig;

        public HEVCConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.HEVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.HEVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(HEVCConfig); // HEVCConfig
            return boxSize;
        }
    }


    public class LHEVCConfigurationBox : Box
    {
        public override string FourCC { get { return "lhvC"; } }
        public LHEVCDecoderConfigurationRecord LHEVCConfig;

        public LHEVCConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.LHEVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.LHEVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(LHEVCConfig); // LHEVCConfig
            return boxSize;
        }
    }


    public class MPEG4ExtensionDescriptorsBox : Box
    {
        public override string FourCC { get { return "m4ds"; } }
        public Descriptor[] Descr;

        public MPEG4ExtensionDescriptorsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.Descr);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.Descr);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(Descr); // Descr
            return boxSize;
        }
    }


    public class SVCConfigurationBox : Box
    {
        public override string FourCC { get { return "svcC"; } }
        public SVCDecoderConfigurationRecord SVCConfig;

        public SVCConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.SVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.SVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(SVCConfig); // SVCConfig
            return boxSize;
        }
    }


    public class ScalabilityInformationSEIBox : Box
    {
        public override string FourCC { get { return "seib"; } }
        public byte[] scalinfosei;

        public ScalabilityInformationSEIBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBytes(stream, size - 64, out this.scalinfosei);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBytes(stream, size - 64, this.scalinfosei);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)(size - 64) * 8; // scalinfosei
            return boxSize;
        }
    }


    public class SVCPriorityAssignmentBox : Box
    {
        public override string FourCC { get { return "svcP"; } }
        public byte method_count;
        public string[] PriorityAssignmentURI;

        public SVCPriorityAssignmentBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.method_count);
            boxSize += IsoReaderWriter.ReadStringArray(stream, method_count, out this.PriorityAssignmentURI);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.method_count);
            boxSize += IsoReaderWriter.WriteStringArray(stream, method_count, this.PriorityAssignmentURI);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // method_count
            boxSize += IsoReaderWriter.CalculateSize(PriorityAssignmentURI); // PriorityAssignmentURI
            return boxSize;
        }
    }


    public class ViewScalabilityInformationSEIBox : Box
    {
        public override string FourCC { get { return "vsib"; } }
        public byte[] mvcscalinfosei;

        public ViewScalabilityInformationSEIBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBytes(stream, size - 64, out this.mvcscalinfosei);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBytes(stream, size - 64, this.mvcscalinfosei);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)(size - 64) * 8; // mvcscalinfosei
            return boxSize;
        }
    }


    public class MVDScalabilityInformationSEIBox : Box
    {
        public override string FourCC { get { return "3sib"; } }
        public byte[] mvdscalinfosei;

        public MVDScalabilityInformationSEIBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBytes(stream, size - 64, out this.mvdscalinfosei);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBytes(stream, size - 64, this.mvdscalinfosei);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)(size - 64) * 8; // mvdscalinfosei
            return boxSize;
        }
    }


    public class MVCViewPriorityAssignmentBox : Box
    {
        public override string FourCC { get { return "mvcP"; } }
        public byte method_count;
        public string[] PriorityAssignmentURI;

        public MVCViewPriorityAssignmentBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.method_count);
            boxSize += IsoReaderWriter.ReadStringArray(stream, method_count, out this.PriorityAssignmentURI);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.method_count);
            boxSize += IsoReaderWriter.WriteStringArray(stream, method_count, this.PriorityAssignmentURI);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // method_count
            boxSize += IsoReaderWriter.CalculateSize(PriorityAssignmentURI); // PriorityAssignmentURI
            return boxSize;
        }
    }


    public class HEVCTileConfigurationBox : Box
    {
        public override string FourCC { get { return "hvtC"; } }
        public HEVCTileTierLevelConfigurationRecord HEVCTileTierLevelConfig;

        public HEVCTileConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.HEVCTileTierLevelConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.HEVCTileTierLevelConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(HEVCTileTierLevelConfig); // HEVCTileTierLevelConfig
            return boxSize;
        }
    }


    public class EVCConfigurationBox : Box
    {
        public override string FourCC { get { return "evcC"; } }
        public EVCDecoderConfigurationRecord EVCConfig;

        public EVCConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.EVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.EVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(EVCConfig); // EVCConfig
            return boxSize;
        }
    }


    public class SVCPriorityLayerInfoBox : Box
    {
        public override string FourCC { get { return "qlif"; } }
        public byte pr_layer_num;
        public byte pr_layer;
        public uint profile_level_idc;
        public uint max_bitrate;
        public uint avg_bitrate;

        public SVCPriorityLayerInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.pr_layer_num);

            for (int j = 0; j < pr_layer_num; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.pr_layer);
                boxSize += IsoReaderWriter.ReadUInt24(stream, out this.profile_level_idc);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.max_bitrate);
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.avg_bitrate);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.pr_layer_num);

            for (int j = 0; j < pr_layer_num; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.pr_layer);
                boxSize += IsoReaderWriter.WriteUInt24(stream, this.profile_level_idc);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.max_bitrate);
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.avg_bitrate);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // pr_layer_num

            for (int j = 0; j < pr_layer_num; j++)
            {
                boxSize += 8; // pr_layer
                boxSize += 24; // profile_level_idc
                boxSize += 32; // max_bitrate
                boxSize += 32; // avg_bitrate
            }
            return boxSize;
        }
    }


    public class VvcConfigurationBox : FullBox
    {
        public override string FourCC { get { return "vvcC"; } }
        public VvcDecoderConfigurationRecord VvcConfig;

        public VvcConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.VvcConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.VvcConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(VvcConfig); // VvcConfig
            return boxSize;
        }
    }


    public class VvcNALUConfigBox : FullBox
    {
        public override string FourCC { get { return "vvnC"; } }
        public byte reserved = 0;
        public byte LengthSizeMinusOne;

        public VvcNALUConfigBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.LengthSizeMinusOne);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.LengthSizeMinusOne);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 6; // reserved
            boxSize += 2; // LengthSizeMinusOne
            return boxSize;
        }
    }


    public class DefaultHevcExtractorConstructorBox : FullBox
    {
        public override string FourCC { get { return "dhec"; } }
        public uint num_entries;
        public byte constructor_type;
        public byte flags;
        public SampleConstructor SampleConstructor;
        public InlineConstructor InlineConstructor;
        public SampleConstructorFromTrackGroup SampleConstructorFromTrackGroup;
        public NALUStartInlineConstructor NALUStartInlineConstructor;

        public DefaultHevcExtractorConstructorBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entries);

            for (int i = 1; i <= num_entries; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.constructor_type);
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.flags);

                if (constructor_type == 0)
                {
                    boxSize += IsoReaderWriter.ReadBox(stream, out this.SampleConstructor);
                }

                else if (constructor_type == 2)
                {
                    boxSize += IsoReaderWriter.ReadBox(stream, out this.InlineConstructor);
                }

                else if (constructor_type == 3)
                {
                    boxSize += IsoReaderWriter.ReadBox(stream, out this.SampleConstructorFromTrackGroup);
                }

                else if (constructor_type == 6)
                {
                    boxSize += IsoReaderWriter.ReadBox(stream, out this.NALUStartInlineConstructor);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

            for (int i = 1; i <= num_entries; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.constructor_type);
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.flags);

                if (constructor_type == 0)
                {
                    boxSize += IsoReaderWriter.WriteBox(stream, this.SampleConstructor);
                }

                else if (constructor_type == 2)
                {
                    boxSize += IsoReaderWriter.WriteBox(stream, this.InlineConstructor);
                }

                else if (constructor_type == 3)
                {
                    boxSize += IsoReaderWriter.WriteBox(stream, this.SampleConstructorFromTrackGroup);
                }

                else if (constructor_type == 6)
                {
                    boxSize += IsoReaderWriter.WriteBox(stream, this.NALUStartInlineConstructor);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // num_entries

            for (int i = 1; i <= num_entries; i++)
            {
                boxSize += 8; // constructor_type
                boxSize += 8; // flags

                if (constructor_type == 0)
                {
                    boxSize += IsoReaderWriter.CalculateSize(SampleConstructor); // SampleConstructor
                }

                else if (constructor_type == 2)
                {
                    boxSize += IsoReaderWriter.CalculateSize(InlineConstructor); // InlineConstructor
                }

                else if (constructor_type == 3)
                {
                    boxSize += IsoReaderWriter.CalculateSize(SampleConstructorFromTrackGroup); // SampleConstructorFromTrackGroup
                }

                else if (constructor_type == 6)
                {
                    boxSize += IsoReaderWriter.CalculateSize(NALUStartInlineConstructor); // NALUStartInlineConstructor
                }
            }
            return boxSize;
        }
    }


    public class SVCMetadataSampleConfigBox : FullBox
    {
        public override string FourCC { get { return "svmC"; } }
        public byte sample_statement_type;
        public byte default_statement_type;
        public byte default_statement_length;
        public byte entry_count;
        public byte statement_type;  //  from the user extension ranges
        public string statement_namespace;

        public SVCMetadataSampleConfigBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.sample_statement_type);
            /*  normally group, or seq  */
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.default_statement_type);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.default_statement_length);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.statement_type); // from the user extension ranges
                boxSize += IsoReaderWriter.ReadString(stream, out this.statement_namespace);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            boxSize += IsoReaderWriter.WriteUInt8(stream, this.sample_statement_type);
            /*  normally group, or seq  */
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.default_statement_type);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.default_statement_length);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.statement_type); // from the user extension ranges
                boxSize += IsoReaderWriter.WriteString(stream, this.statement_namespace);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            boxSize += 8; // sample_statement_type
            /*  normally group, or seq  */
            boxSize += 8; // default_statement_type
            boxSize += 8; // default_statement_length
            boxSize += 8; // entry_count

            for (int i = 1; i <= entry_count; i++)
            {
                boxSize += 8; // statement_type
                boxSize += (ulong)statement_namespace.Length * 8; // statement_namespace
            }
            return boxSize;
        }
    }


    public class EVCSliceComponentTrackConfigurationBox : Box
    {
        public override string FourCC { get { return "evsC"; } }
        public EVCSliceComponentTrackConfigurationRecord config;

        public EVCSliceComponentTrackConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(config); // config
            return boxSize;
        }
    }


    public class WebVTTConfigurationBox : Box
    {
        public override string FourCC { get { return "vttC"; } }
        public string config;

        public WebVTTConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)config.Length * 8; // config
            return boxSize;
        }
    }


    public class WebVTTSourceLabelBox : Box
    {
        public override string FourCC { get { return "vlab"; } }
        public string source_label;

        public WebVTTSourceLabelBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.source_label);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.source_label);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)source_label.Length * 8; // source_label
            return boxSize;
        }
    }


    public class WVTTSampleEntry : PlainTextSampleEntry
    {
        public override string FourCC { get { return "wvtt"; } }
        public WebVTTConfigurationBox config;
        public WebVTTSourceLabelBox label;  //  recommended
        public MPEG4BitRateBox MPEG4BitRateBox;  //  optional

        public WVTTSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.label); // recommended
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.MPEG4BitRateBox); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            boxSize += IsoReaderWriter.WriteBox(stream, this.label); // recommended
            if (this.MPEG4BitRateBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.MPEG4BitRateBox); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            boxSize += IsoReaderWriter.CalculateSize(label); // label
            if (this.MPEG4BitRateBox != null) boxSize += IsoReaderWriter.CalculateSize(MPEG4BitRateBox); // MPEG4BitRateBox
            return boxSize;
        }
    }


    public class AuxiliaryTypeInfoBox : FullBox
    {
        public override string FourCC { get { return "auxi"; } }
        public string aux_track_type;

        public AuxiliaryTypeInfoBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.aux_track_type);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.aux_track_type);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)aux_track_type.Length * 8; // aux_track_type
            return boxSize;
        }
    }


    public class CodingConstraintsBox : FullBox
    {
        public override string FourCC { get { return "ccst"; } }
        public bool all_ref_pics_intra;
        public bool intra_pred_used;
        public byte max_ref_per_pic;
        public uint reserved;

        public CodingConstraintsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.all_ref_pics_intra);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.intra_pred_used);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.max_ref_per_pic);
            boxSize += IsoReaderWriter.ReadBits(stream, 26, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.all_ref_pics_intra);
            boxSize += IsoReaderWriter.WriteBit(stream, this.intra_pred_used);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.max_ref_per_pic);
            boxSize += IsoReaderWriter.WriteBits(stream, 26, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // all_ref_pics_intra
            boxSize += 1; // intra_pred_used
            boxSize += 4; // max_ref_per_pic
            boxSize += 26; // reserved
            return boxSize;
        }
    }


    public class MD5IntegrityBox : FullBox
    {
        public override string FourCC { get { return "md5i"; } }
        public byte[] input_MD5;
        public uint input_4cc;
        public uint grouping_type;
        public uint grouping_type_parameter;
        public uint num_entries;
        public uint[] group_description_index;

        public MD5IntegrityBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBytes(stream, 16, out this.input_MD5);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.input_4cc);

            if (input_4cc == IsoReaderWriter.FromFourCC("sgpd"))
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type);

                if ((flags & 1) == 1)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.grouping_type_parameter);
                }
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entries);

                for (int i = 0; i < num_entries; i++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_description_index[i]);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBytes(stream, 16, this.input_MD5);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.input_4cc);

            if (input_4cc == IsoReaderWriter.FromFourCC("sgpd"))
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

                if ((flags & 1) == 1)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
                }
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

                for (int i = 0; i < num_entries; i++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_description_index[i]);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16 * 8; // input_MD5
            boxSize += 32; // input_4cc

            if (input_4cc == IsoReaderWriter.FromFourCC("sgpd"))
            {
                boxSize += 32; // grouping_type

                if ((flags & 1) == 1)
                {
                    boxSize += 32; // grouping_type_parameter
                }
                boxSize += 32; // num_entries

                for (int i = 0; i < num_entries; i++)
                {
                    boxSize += 32; // group_description_index
                }
            }
            return boxSize;
        }
    }


    public class AudioSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "enca"; } }
        public uint[] reserved = [];
        public ushort channelcount;
        public ushort samplesize = 16;
        public ushort pre_defined = 0;
        public ushort reserved0 = 0;
        public uint samplerate = 0; // = {if track_is_audio 0x0100 else 0}; //  optional boxes follow
        public Box Box;  //  further boxes as needed
        public ChannelLayout ChannelLayout;
        public DownMixInstructions[] DownMixInstructions;
        public DRCCoefficientsBasic[] DRCCoefficientsBasic;
        public DRCInstructionsBasic[] DRCInstructionsBasic;
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC;
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC;  //  we permit only one DRC Extension box:
        public UniDrcConfigExtension UniDrcConfigExtension;  //  optional boxes follow
        public SamplingRateBox SamplingRateBox;
        public ChannelLayout ChannelLayout0;

        public AudioSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 2, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.channelcount);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.samplesize);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved0);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DownMixInstructions);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SamplingRateBox);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout0);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved0);
            if (this.samplerate != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.WriteBox(stream, this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.SamplingRateBox);
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout0);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2 * 32; // reserved
            boxSize += 16; // channelcount
            boxSize += 16; // samplesize
            boxSize += 16; // pre_defined
            boxSize += 16; // reserved0
            if (this.samplerate != null) boxSize += 32; // samplerate
            boxSize += IsoReaderWriter.CalculateSize(Box); // Box
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout); // ChannelLayout
            boxSize += IsoReaderWriter.CalculateClassSize(DownMixInstructions); // DownMixInstructions
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsBasic); // DRCInstructionsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.CalculateSize(UniDrcConfigExtension); // UniDrcConfigExtension
            boxSize += IsoReaderWriter.CalculateSize(SamplingRateBox); // SamplingRateBox
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout0); // ChannelLayout0
            return boxSize;
        }
    }


    public class AudioSampleEntryV1 : SampleEntry
    {
        public override string FourCC { get { return "enca"; } }
        public ushort entry_version;  //  shall be 1, 
        public ushort[] reserved = [];
        public ushort channelcount;  //  shall be correct
        public ushort samplesize = 16;
        public ushort pre_defined = 0;
        public ushort reserved0 = 0;
        public uint samplerate = 1 << 16;  //  optional boxes follow
        public SamplingRateBox SamplingRateBox;
        public Box Box;  //  further boxes as needed
        public ChannelLayout ChannelLayout;
        public DownMixInstructions[] DownMixInstructions;
        public DRCCoefficientsBasic[] DRCCoefficientsBasic;
        public DRCInstructionsBasic[] DRCInstructionsBasic;
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC;
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC;  //  we permit only one DRC Extension box:
        public UniDrcConfigExtension UniDrcConfigExtension;  //  optional boxes follow
        public ChannelLayout ChannelLayout0;

        public AudioSampleEntryV1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_version); // shall be 1, 
            /*  and shall be in an stsd with version ==1 */
            boxSize += IsoReaderWriter.ReadUInt16Array(stream, 3, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.channelcount); // shall be correct
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.samplesize);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved0);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SamplingRateBox);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DownMixInstructions);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout0);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_version); // shall be 1, 
            /*  and shall be in an stsd with version ==1 */
            boxSize += IsoReaderWriter.WriteUInt16Array(stream, 3, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.channelcount); // shall be correct
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved0);
            if (this.samplerate != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.SamplingRateBox);
            boxSize += IsoReaderWriter.WriteBox(stream, this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.WriteBox(stream, this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout0);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_version
            /*  and shall be in an stsd with version ==1 */
            boxSize += 3 * 16; // reserved
            boxSize += 16; // channelcount
            boxSize += 16; // samplesize
            boxSize += 16; // pre_defined
            boxSize += 16; // reserved0
            if (this.samplerate != null) boxSize += 32; // samplerate
            boxSize += IsoReaderWriter.CalculateSize(SamplingRateBox); // SamplingRateBox
            boxSize += IsoReaderWriter.CalculateSize(Box); // Box
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout); // ChannelLayout
            boxSize += IsoReaderWriter.CalculateClassSize(DownMixInstructions); // DownMixInstructions
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsBasic); // DRCInstructionsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.CalculateSize(UniDrcConfigExtension); // UniDrcConfigExtension
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout0); // ChannelLayout0
            return boxSize;
        }
    }


    public class AudioSampleEntryV11 : SampleEntry
    {
        public override string FourCC { get { return "enca"; } }
        public ushort entry_version;  //  shall be 1, 
        public ushort[] reserved = [];
        public ushort channelcount;  //  shall be correct
        public ushort samplesize = 16;
        public ushort pre_defined = 0;
        public ushort reserved0 = 0;
        public uint samplerate = 1 << 16;  //  optional boxes follow
        public SamplingRateBox SamplingRateBox;
        public Box Box;  //  further boxes as needed
        public ChannelLayout ChannelLayout;
        public DownMixInstructions[] DownMixInstructions;
        public DRCCoefficientsBasic[] DRCCoefficientsBasic;
        public DRCInstructionsBasic[] DRCInstructionsBasic;
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC;
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC;  //  we permit only one DRC Extension box:
        public UniDrcConfigExtension UniDrcConfigExtension;  //  optional boxes follow
        public ChannelLayout ChannelLayout0;

        public AudioSampleEntryV11()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_version); // shall be 1, 
            /*  and shall be in an stsd with version ==1 */
            boxSize += IsoReaderWriter.ReadUInt16Array(stream, 3, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.channelcount); // shall be correct
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.samplesize);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved0);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SamplingRateBox);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DownMixInstructions);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout0);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_version); // shall be 1, 
            /*  and shall be in an stsd with version ==1 */
            boxSize += IsoReaderWriter.WriteUInt16Array(stream, 3, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.channelcount); // shall be correct
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved0);
            if (this.samplerate != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.SamplingRateBox);
            boxSize += IsoReaderWriter.WriteBox(stream, this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.WriteBox(stream, this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout0);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_version
            /*  and shall be in an stsd with version ==1 */
            boxSize += 3 * 16; // reserved
            boxSize += 16; // channelcount
            boxSize += 16; // samplesize
            boxSize += 16; // pre_defined
            boxSize += 16; // reserved0
            if (this.samplerate != null) boxSize += 32; // samplerate
            boxSize += IsoReaderWriter.CalculateSize(SamplingRateBox); // SamplingRateBox
            boxSize += IsoReaderWriter.CalculateSize(Box); // Box
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout); // ChannelLayout
            boxSize += IsoReaderWriter.CalculateClassSize(DownMixInstructions); // DownMixInstructions
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsBasic); // DRCInstructionsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.CalculateSize(UniDrcConfigExtension); // UniDrcConfigExtension
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout0); // ChannelLayout0
            return boxSize;
        }
    }


    public class FontSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "encf"; } }

        public FontSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /* other boxes from derived specifications */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /* other boxes from derived specifications */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /* other boxes from derived specifications */
            return boxSize;
        }
    }


    public class MetaDataSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "encm"; } }

        public MetaDataSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class SampleEntry : Box
    {
        public override string FourCC { get { return "encv"; } }

        public SampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /*  ProtectionSchemeInfoBox { */
            /*  OriginalFormatBox;	// data_format is 'resv' */
            /*  SchemeTypeBox; */
            /*  SchemeInformationBox; */
            /*  } */
            /*  tRestrictedSchemeInfoBox { */
            /*  OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1' */
            /*  SchemeTypeBox; */
            /*  SchemeInformationBox; */
            /*  } */
            /*  Boxes specific to the untransformed sample entry type */
            /*  For 'avc1', these would include AVCConfigurationBox */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /*  ProtectionSchemeInfoBox { */
            /*  OriginalFormatBox;	// data_format is 'resv' */
            /*  SchemeTypeBox; */
            /*  SchemeInformationBox; */
            /*  } */
            /*  tRestrictedSchemeInfoBox { */
            /*  OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1' */
            /*  SchemeTypeBox; */
            /*  SchemeInformationBox; */
            /*  } */
            /*  Boxes specific to the untransformed sample entry type */
            /*  For 'avc1', these would include AVCConfigurationBox */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /*  ProtectionSchemeInfoBox { */
            /*  OriginalFormatBox;	// data_format is 'resv' */
            /*  SchemeTypeBox; */
            /*  SchemeInformationBox; */
            /*  } */
            /*  tRestrictedSchemeInfoBox { */
            /*  OriginalFormatBox; // data_format indicates a codec, e.g. 'avc1' */
            /*  SchemeTypeBox; */
            /*  SchemeInformationBox; */
            /*  } */
            /*  Boxes specific to the untransformed sample entry type */
            /*  For 'avc1', these would include AVCConfigurationBox */
            return boxSize;
        }
    }


    public class XMLMetaDataSampleEntry : MetaDataSampleEntry
    {
        public override string FourCC { get { return "metx"; } }
        public string content_encoding;  //  optional
        public string ns;
        public string schema_location;  //  optional

        public XMLMetaDataSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.content_encoding); // optional
            boxSize += IsoReaderWriter.ReadString(stream, out this.ns);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.schema_location); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.content_encoding != null) boxSize += IsoReaderWriter.WriteString(stream, this.content_encoding); // optional
            boxSize += IsoReaderWriter.WriteString(stream, this.ns);
            if (this.schema_location != null) boxSize += IsoReaderWriter.WriteString(stream, this.schema_location); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.content_encoding != null) boxSize += (ulong)content_encoding.Length * 8; // content_encoding
            boxSize += (ulong)ns.Length * 8; // ns
            if (this.schema_location != null) boxSize += (ulong)schema_location.Length * 8; // schema_location
            return boxSize;
        }
    }


    public class TextMetaDataSampleEntry : MetaDataSampleEntry
    {
        public override string FourCC { get { return "mett"; } }
        public string content_encoding;  //  optional
        public string mime_format;
        public TextConfigBox TextConfigBox;  //  optional

        public TextMetaDataSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.content_encoding); // optional
            boxSize += IsoReaderWriter.ReadString(stream, out this.mime_format);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TextConfigBox); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.content_encoding != null) boxSize += IsoReaderWriter.WriteString(stream, this.content_encoding); // optional
            boxSize += IsoReaderWriter.WriteString(stream, this.mime_format);
            if (this.TextConfigBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TextConfigBox); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.content_encoding != null) boxSize += (ulong)content_encoding.Length * 8; // content_encoding
            boxSize += (ulong)mime_format.Length * 8; // mime_format
            if (this.TextConfigBox != null) boxSize += IsoReaderWriter.CalculateSize(TextConfigBox); // TextConfigBox
            return boxSize;
        }
    }


    public class URIMetaSampleEntry : MetaDataSampleEntry
    {
        public override string FourCC { get { return "urim"; } }
        public URIBox the_label;
        public URIInitBox init;  //  optional

        public URIMetaSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.the_label);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.init); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.the_label);
            if (this.init != null) boxSize += IsoReaderWriter.WriteBox(stream, this.init); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(the_label); // the_label
            if (this.init != null) boxSize += IsoReaderWriter.CalculateSize(init); // init
            return boxSize;
        }
    }


    public class BoxedMetadataSampleEntry : MetadataSampleEntry
    {
        public override string FourCC { get { return "mebx"; } }
        public MetadataKeyTableBox MetadataKeyTableBox;  //  mandatory
        public BitRateBox BitRateBox;  //  optional

        public BoxedMetadataSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.MetadataKeyTableBox); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.BitRateBox); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.MetadataKeyTableBox); // mandatory
            if (this.BitRateBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.BitRateBox); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(MetadataKeyTableBox); // MetadataKeyTableBox
            if (this.BitRateBox != null) boxSize += IsoReaderWriter.CalculateSize(BitRateBox); // BitRateBox
            return boxSize;
        }
    }


    public class FDHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "fdp "; } }
        public ushort hinttrackversion = 1;
        public ushort highestcompatibleversion = 1;
        public ushort partition_entry_ID;
        public ushort FEC_overhead;

        public FDHintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.hinttrackversion);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.highestcompatibleversion);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.partition_entry_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.FEC_overhead);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.partition_entry_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.FEC_overhead);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // hinttrackversion
            boxSize += 16; // highestcompatibleversion
            boxSize += 16; // partition_entry_ID
            boxSize += 16; // FEC_overhead
            return boxSize;
        }
    }


    public class IncompleteAVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "icpv"; } }
        public CompleteTrackInfoBox CompleteTrackInfoBox;
        public AVCConfigurationBox config;

        public IncompleteAVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.CompleteTrackInfoBox);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.CompleteTrackInfoBox);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(CompleteTrackInfoBox); // CompleteTrackInfoBox
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class ProtectedMPEG2TransportStreamSampleEntry : MPEG2TSSampleEntry
    {
        public override string FourCC { get { return "pm2t"; } }
        public ProtectionSchemeInfoBox SchemeInformation;

        public ProtectedMPEG2TransportStreamSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SchemeInformation);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.SchemeInformation);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(SchemeInformation); // SchemeInformation
            return boxSize;
        }
    }


    public class ProtectedRtpReceptionHintSampleEntry : RtpReceptionHintSampleEntry
    {
        public override string FourCC { get { return "prtp"; } }
        public ProtectionSchemeInfoBox SchemeInformation;

        public ProtectedRtpReceptionHintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SchemeInformation);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.SchemeInformation);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(SchemeInformation); // SchemeInformation
            return boxSize;
        }
    }


    public class MPEG2TSReceptionSampleEntry : MPEG2TSSampleEntry
    {
        public override string FourCC { get { return "rm2t"; } }

        public MPEG2TSReceptionSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class ReceivedRtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "rrtp"; } }
        public ushort hinttrackversion = 1;
        public ushort highestcompatibleversion = 1;
        public uint maxpacketsize;

        public ReceivedRtpHintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.hinttrackversion);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.highestcompatibleversion);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxpacketsize);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // hinttrackversion
            boxSize += 16; // highestcompatibleversion
            boxSize += 32; // maxpacketsize
            return boxSize;
        }
    }


    public class ReceivedSrtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "rsrp"; } }
        public ushort hinttrackversion = 1;
        public ushort highestcompatibleversion = 1;
        public uint maxpacketsize;

        public ReceivedSrtpHintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.hinttrackversion);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.highestcompatibleversion);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxpacketsize);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // hinttrackversion
            boxSize += 16; // highestcompatibleversion
            boxSize += 32; // maxpacketsize
            return boxSize;
        }
    }


    public class HintSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "rtcp"; } }

        public HintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class moviehintinformation1 : Box
    {
        public override string FourCC { get { return "rtp "; } }

        public moviehintinformation1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class rtpmoviehintinformation1 : Box
    {
        public override string FourCC { get { return "rtp "; } }
        public uint descriptionformat = IsoReaderWriter.FromFourCC("sdp ");
        public sbyte[] sdptext;

        public rtpmoviehintinformation1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.descriptionformat);
            boxSize += IsoReaderWriter.ReadInt8Array(stream, out this.sdptext);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.descriptionformat);
            boxSize += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // descriptionformat
            boxSize += (ulong)sdptext.Length * 8; // sdptext
            return boxSize;
        }
    }


    public class TextSubtitleSampleEntry : SubtitleSampleEntry
    {
        public override string FourCC { get { return "sbtt"; } }
        public string content_encoding;  //  optional
        public string mime_format;
        public TextConfigBox TextConfigBox;  //  optional

        public TextSubtitleSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.content_encoding); // optional
            boxSize += IsoReaderWriter.ReadString(stream, out this.mime_format);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TextConfigBox); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.content_encoding != null) boxSize += IsoReaderWriter.WriteString(stream, this.content_encoding); // optional
            boxSize += IsoReaderWriter.WriteString(stream, this.mime_format);
            if (this.TextConfigBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TextConfigBox); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.content_encoding != null) boxSize += (ulong)content_encoding.Length * 8; // content_encoding
            boxSize += (ulong)mime_format.Length * 8; // mime_format
            if (this.TextConfigBox != null) boxSize += IsoReaderWriter.CalculateSize(TextConfigBox); // TextConfigBox
            return boxSize;
        }
    }


    public class MPEG2TSServerSampleEntry : MPEG2TSSampleEntry
    {
        public override string FourCC { get { return "sm2t"; } }

        public MPEG2TSServerSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class SrtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "srtp"; } }
        public ushort hinttrackversion = 1;
        public ushort highestcompatibleversion = 1;
        public uint maxpacketsize;

        public SrtpHintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.hinttrackversion);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.highestcompatibleversion);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxpacketsize);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // hinttrackversion
            boxSize += 16; // highestcompatibleversion
            boxSize += 32; // maxpacketsize
            return boxSize;
        }
    }


    public class XMLSubtitleSampleEntry : SubtitleSampleEntry
    {
        public override string FourCC { get { return "stpp"; } }
        public string ns;
        public string schema_location;  //  optional
        public string auxiliary_mime_types;  //  optional, required if auxiliary resources are present

        public XMLSubtitleSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.ns);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.schema_location); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.auxiliary_mime_types); // optional, required if auxiliary resources are present
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.ns);
            if (this.schema_location != null) boxSize += IsoReaderWriter.WriteString(stream, this.schema_location); // optional
            if (this.auxiliary_mime_types != null) boxSize += IsoReaderWriter.WriteString(stream, this.auxiliary_mime_types); // optional, required if auxiliary resources are present
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)ns.Length * 8; // ns
            if (this.schema_location != null) boxSize += (ulong)schema_location.Length * 8; // schema_location
            if (this.auxiliary_mime_types != null) boxSize += (ulong)auxiliary_mime_types.Length * 8; // auxiliary_mime_types
            return boxSize;
        }
    }


    public class SimpleTextSampleEntry : PlainTextSampleEntry
    {
        public override string FourCC { get { return "stxt"; } }
        public string content_encoding;  //  optional
        public string mime_format;
        public TextConfigBox TextConfigBox;  //  optional

        public SimpleTextSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadString(stream, out this.content_encoding); // optional
            boxSize += IsoReaderWriter.ReadString(stream, out this.mime_format);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TextConfigBox); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.content_encoding != null) boxSize += IsoReaderWriter.WriteString(stream, this.content_encoding); // optional
            boxSize += IsoReaderWriter.WriteString(stream, this.mime_format);
            if (this.TextConfigBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TextConfigBox); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.content_encoding != null) boxSize += (ulong)content_encoding.Length * 8; // content_encoding
            boxSize += (ulong)mime_format.Length * 8; // mime_format
            if (this.TextConfigBox != null) boxSize += IsoReaderWriter.CalculateSize(TextConfigBox); // TextConfigBox
            return boxSize;
        }
    }


    public class HapticSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "encp"; } }
        public Box[] otherboxes;

        public HapticSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.otherboxes);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.otherboxes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(otherboxes); // otherboxes
            return boxSize;
        }
    }


    public class VolumetricVisualSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "enc3"; } }
        public byte[] compressorname;  //  other boxes from derived specifications

        public VolumetricVisualSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBytes(stream, 32, out this.compressorname); // other boxes from derived specifications
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBytes(stream, 32, this.compressorname); // other boxes from derived specifications
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32 * 8; // compressorname
            return boxSize;
        }
    }


    public class VisualSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "resv"; } }
        public ushort pre_defined = 0;
        public ushort reserved = 0;
        public uint[] pre_defined0 = [];
        public ushort width;
        public ushort height;
        public uint horizresolution = 0x00480000;  //  72 dpi
        public uint vertresolution = 0x00480000;  //  72 dpi
        public uint reserved0 = 0;
        public ushort frame_count = 1;
        public byte[] compressorname;
        public ushort depth = 0x0018;
        public short pre_defined1 = -1;  //  other boxes from derived specifications
        public CleanApertureBox clap;  //  optional
        public PixelAspectRatioBox pasp;  //  optional

        public VisualSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 3, out this.pre_defined0);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.width);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.height);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizresolution); // 72 dpi
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertresolution); // 72 dpi
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.reserved0);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.frame_count);
            boxSize += IsoReaderWriter.ReadBytes(stream, 32, out this.compressorname);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.depth);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.pre_defined1); // other boxes from derived specifications
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.clap); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.pasp); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 3, this.pre_defined0);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.width);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.height);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizresolution); // 72 dpi
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertresolution); // 72 dpi
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.reserved0);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.frame_count);
            boxSize += IsoReaderWriter.WriteBytes(stream, 32, this.compressorname);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.depth);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.pre_defined1); // other boxes from derived specifications
            if (this.clap != null) boxSize += IsoReaderWriter.WriteBox(stream, this.clap); // optional
            if (this.pasp != null) boxSize += IsoReaderWriter.WriteBox(stream, this.pasp); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // pre_defined
            boxSize += 16; // reserved
            boxSize += 3 * 32; // pre_defined0
            boxSize += 16; // width
            boxSize += 16; // height
            boxSize += 32; // horizresolution
            boxSize += 32; // vertresolution
            boxSize += 32; // reserved0
            boxSize += 16; // frame_count
            boxSize += 32 * 8; // compressorname
            boxSize += 16; // depth
            boxSize += 16; // pre_defined1
            if (this.clap != null) boxSize += IsoReaderWriter.CalculateSize(clap); // clap
            if (this.pasp != null) boxSize += IsoReaderWriter.CalculateSize(pasp); // pasp
            return boxSize;
        }
    }


    public class AudioSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resa"; } }
        public uint[] reserved = [];
        public ushort channelcount;
        public ushort samplesize = 16;
        public ushort pre_defined = 0;
        public ushort reserved0 = 0;
        public uint samplerate = 0; // = {if track_is_audio 0x0100 else 0}; //  optional boxes follow
        public Box Box;  //  further boxes as needed
        public ChannelLayout ChannelLayout;
        public DownMixInstructions[] DownMixInstructions;
        public DRCCoefficientsBasic[] DRCCoefficientsBasic;
        public DRCInstructionsBasic[] DRCInstructionsBasic;
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC;
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC;  //  we permit only one DRC Extension box:
        public UniDrcConfigExtension UniDrcConfigExtension;  //  optional boxes follow
        public SamplingRateBox SamplingRateBox;
        public ChannelLayout ChannelLayout0;

        public AudioSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, 2, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.channelcount);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.samplesize);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved0);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DownMixInstructions);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SamplingRateBox);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout0);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved0);
            if (this.samplerate != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.WriteBox(stream, this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.SamplingRateBox);
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout0);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2 * 32; // reserved
            boxSize += 16; // channelcount
            boxSize += 16; // samplesize
            boxSize += 16; // pre_defined
            boxSize += 16; // reserved0
            if (this.samplerate != null) boxSize += 32; // samplerate
            boxSize += IsoReaderWriter.CalculateSize(Box); // Box
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout); // ChannelLayout
            boxSize += IsoReaderWriter.CalculateClassSize(DownMixInstructions); // DownMixInstructions
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsBasic); // DRCInstructionsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.CalculateSize(UniDrcConfigExtension); // UniDrcConfigExtension
            boxSize += IsoReaderWriter.CalculateSize(SamplingRateBox); // SamplingRateBox
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout0); // ChannelLayout0
            return boxSize;
        }
    }


    public class AudioSampleEntryV12 : SampleEntry
    {
        public override string FourCC { get { return "resa"; } }
        public ushort entry_version;  //  shall be 1, 
        public ushort[] reserved = [];
        public ushort channelcount;  //  shall be correct
        public ushort samplesize = 16;
        public ushort pre_defined = 0;
        public ushort reserved0 = 0;
        public uint samplerate = 1 << 16;  //  optional boxes follow
        public SamplingRateBox SamplingRateBox;
        public Box Box;  //  further boxes as needed
        public ChannelLayout ChannelLayout;
        public DownMixInstructions[] DownMixInstructions;
        public DRCCoefficientsBasic[] DRCCoefficientsBasic;
        public DRCInstructionsBasic[] DRCInstructionsBasic;
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC;
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC;  //  we permit only one DRC Extension box:
        public UniDrcConfigExtension UniDrcConfigExtension;  //  optional boxes follow
        public ChannelLayout ChannelLayout0;

        public AudioSampleEntryV12()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_version); // shall be 1, 
            /*  and shall be in an stsd with version ==1 */
            boxSize += IsoReaderWriter.ReadUInt16Array(stream, 3, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.channelcount); // shall be correct
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.samplesize);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.pre_defined);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reserved0);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadUInt32(stream, out this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.SamplingRateBox);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DownMixInstructions);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.ReadClasses(stream, out this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ChannelLayout0);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_version); // shall be 1, 
            /*  and shall be in an stsd with version ==1 */
            boxSize += IsoReaderWriter.WriteUInt16Array(stream, 3, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.channelcount); // shall be correct
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reserved0);
            if (this.samplerate != null) boxSize += IsoReaderWriter.WriteUInt32(stream, this.samplerate); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.SamplingRateBox);
            boxSize += IsoReaderWriter.WriteBox(stream, this.Box); // further boxes as needed
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            boxSize += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC); // we permit only one DRC Extension box:
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.WriteBox(stream, this.UniDrcConfigExtension); // optional boxes follow
            boxSize += IsoReaderWriter.WriteBox(stream, this.ChannelLayout0);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // entry_version
            /*  and shall be in an stsd with version ==1 */
            boxSize += 3 * 16; // reserved
            boxSize += 16; // channelcount
            boxSize += 16; // samplesize
            boxSize += 16; // pre_defined
            boxSize += 16; // reserved0
            if (this.samplerate != null) boxSize += 32; // samplerate
            boxSize += IsoReaderWriter.CalculateSize(SamplingRateBox); // SamplingRateBox
            boxSize += IsoReaderWriter.CalculateSize(Box); // Box
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout); // ChannelLayout
            boxSize += IsoReaderWriter.CalculateClassSize(DownMixInstructions); // DownMixInstructions
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsBasic); // DRCInstructionsBasic
            boxSize += IsoReaderWriter.CalculateClassSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
            boxSize += IsoReaderWriter.CalculateClassSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
            if (this.UniDrcConfigExtension != null) boxSize += IsoReaderWriter.CalculateSize(UniDrcConfigExtension); // UniDrcConfigExtension
            boxSize += IsoReaderWriter.CalculateSize(ChannelLayout0); // ChannelLayout0
            return boxSize;
        }
    }


    public class MetaDataSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resm"; } }

        public MetaDataSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class FontSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resf"; } }

        public FontSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            /* other boxes from derived specifications */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            /* other boxes from derived specifications */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            /* other boxes from derived specifications */
            return boxSize;
        }
    }


    public class HapticSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resp"; } }
        public Box[] otherboxes;

        public HapticSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBoxes(stream, out this.otherboxes);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBoxes(stream, this.otherboxes);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(otherboxes); // otherboxes
            return boxSize;
        }
    }


    public class VolumetricVisualSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "res3"; } }
        public byte[] compressorname;  //  other boxes from derived specifications

        public VolumetricVisualSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBytes(stream, 32, out this.compressorname); // other boxes from derived specifications
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBytes(stream, 32, this.compressorname); // other boxes from derived specifications
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32 * 8; // compressorname
            return boxSize;
        }
    }


    public class RtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "rtp "; } }
        public ushort hinttrackversion = 1;
        public ushort highestcompatibleversion = 1;
        public uint maxpacketsize;

        public RtpHintSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.hinttrackversion);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.highestcompatibleversion);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maxpacketsize);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // hinttrackversion
            boxSize += 16; // highestcompatibleversion
            boxSize += 32; // maxpacketsize
            return boxSize;
        }
    }


    public class EntityToGroupBox : FullBox
    {
        public override string FourCC { get { return "altr"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class BrandProperty : GeneralTypeBox
    {
        public override string FourCC { get { return "brnd"; } }

        public BrandProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox : Box
    {
        public override string FourCC { get { return "fdel"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge : Box
    {
        public override string FourCC { get { return "fdel"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox1 : Box
    {
        public override string FourCC { get { return "iloc"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge1 : Box
    {
        public override string FourCC { get { return "iloc"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class AlternativeStartupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "alst"; } }
        public ushort roll_count;
        public ushort first_output_sample;
        public uint[] sample_offset;
        public ushort[] num_output_samples;
        public ushort[] num_total_samples;

        public AlternativeStartupEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.roll_count);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.first_output_sample);

            for (int i = 1; i <= roll_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_offset[i]);
            }
            int j = 1;

            while (true)
            {
                /*  optional, until the end of the structure */
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_output_samples[j]);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_total_samples[j]);
                j++;
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.roll_count);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.first_output_sample);

            for (int i = 1; i <= roll_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_offset[i]);
            }
            int j = 1;

            while (true)
            {
                /*  optional, until the end of the structure */
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_output_samples[j]);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_total_samples[j]);
                j++;
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // roll_count
            boxSize += 16; // first_output_sample

            for (int i = 1; i <= roll_count; i++)
            {
                boxSize += 32; // sample_offset
            }
            int j = 1;

            while (true)
            {
                /*  optional, until the end of the structure */
                boxSize += 16; // num_output_samples
                boxSize += 16; // num_total_samples
                j++;
            }
            return boxSize;
        }
    }


    public class VisualDRAPEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "drap"; } }
        public byte DRAP_type;
        public uint reserved = 0;

        public VisualDRAPEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.DRAP_type);
            boxSize += IsoReaderWriter.ReadBits(stream, 29, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.DRAP_type);
            boxSize += IsoReaderWriter.WriteBits(stream, 29, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 3; // DRAP_type
            boxSize += 29; // reserved
            return boxSize;
        }
    }


    public class AudioPreRollEntry : AudioSampleGroupEntry
    {
        public override string FourCC { get { return "prol"; } }
        public short roll_distance;

        public AudioPreRollEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.roll_distance);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.roll_distance);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // roll_distance
            return boxSize;
        }
    }


    public class VisualRandomAccessEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "rap "; } }
        public bool num_leading_samples_known;
        public byte num_leading_samples;

        public VisualRandomAccessEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.num_leading_samples_known);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.num_leading_samples);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.num_leading_samples_known);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.num_leading_samples);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // num_leading_samples_known
            boxSize += 7; // num_leading_samples
            return boxSize;
        }
    }


    public class RateShareEntry : SampleGroupDescriptionEntry
    {
        public override string FourCC { get { return "rash"; } }
        public ushort operation_point_count;
        public ushort target_rate_share;
        public uint available_bitrate;
        public ushort target_rate_share0;
        public uint maximum_bitrate;
        public uint minimum_bitrate;
        public byte discard_priority;

        public RateShareEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.operation_point_count);

            if (operation_point_count == 1)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_rate_share);
            }

            else
            {

                for (int i = 0; i < operation_point_count; i++)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.available_bitrate);
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_rate_share0);
                }
            }
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.maximum_bitrate);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.minimum_bitrate);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.discard_priority);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.operation_point_count);

            if (operation_point_count == 1)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_rate_share);
            }

            else
            {

                for (int i = 0; i < operation_point_count; i++)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.available_bitrate);
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_rate_share0);
                }
            }
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.maximum_bitrate);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.minimum_bitrate);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.discard_priority);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // operation_point_count

            if (operation_point_count == 1)
            {
                boxSize += 16; // target_rate_share
            }

            else
            {

                for (int i = 0; i < operation_point_count; i++)
                {
                    boxSize += 32; // available_bitrate
                    boxSize += 16; // target_rate_share0
                }
            }
            boxSize += 32; // maximum_bitrate
            boxSize += 32; // minimum_bitrate
            boxSize += 8; // discard_priority
            return boxSize;
        }
    }


    public class AudioRollRecoveryEntry : AudioSampleGroupEntry
    {
        public override string FourCC { get { return "roll"; } }
        public short roll_distance;

        public AudioRollRecoveryEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.roll_distance);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.roll_distance);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // roll_distance
            return boxSize;
        }
    }


    public class SAPEntry : SampleGroupDescriptionEntry
    {
        public override string FourCC { get { return "sap "; } }
        public bool dependent_flag;
        public byte reserved;
        public byte SAP_type;

        public SAPEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.dependent_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.SAP_type);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.dependent_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.SAP_type);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // dependent_flag
            boxSize += 3; // reserved
            boxSize += 4; // SAP_type
            return boxSize;
        }
    }


    public class SampleToMetadataItemEntry : SampleGroupDescriptionEntry
    {
        public override string FourCC { get { return "stmi"; } }
        public uint meta_box_handler_type;
        public uint num_items;
        public uint[] item_id;

        public SampleToMetadataItemEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.meta_box_handler_type);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_items);

            for (int i = 0; i < num_items; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.item_id[i]);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.meta_box_handler_type);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_items);

            for (int i = 0; i < num_items; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.item_id[i]);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // meta_box_handler_type
            boxSize += 32; // num_items

            for (int i = 0; i < num_items; i++)
            {
                boxSize += 32; // item_id
            }
            return boxSize;
        }
    }


    public class TemporalLevelEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "tele"; } }
        public bool level_independently_decodable;
        public byte reserved = 0;

        public TemporalLevelEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.level_independently_decodable);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.level_independently_decodable);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // level_independently_decodable
            boxSize += 7; // reserved
            return boxSize;
        }
    }


    public class PixelAspectRatioEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pasr"; } }
        public uint hSpacing;
        public uint vSpacing;

        public PixelAspectRatioEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.hSpacing);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vSpacing);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.hSpacing);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vSpacing);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // hSpacing
            boxSize += 32; // vSpacing
            return boxSize;
        }
    }


    public class CleanApertureEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "casg"; } }
        public uint cleanApertureWidthN;
        public uint cleanApertureWidthD;
        public uint cleanApertureHeightN;
        public uint cleanApertureHeightD;
        public uint horizOffN;
        public uint horizOffD;
        public uint vertOffN;
        public uint vertOffD;

        public CleanApertureEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureWidthN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureWidthD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureHeightN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureHeightD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizOffN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizOffD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertOffN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertOffD);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizOffN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizOffD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertOffN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertOffD);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // cleanApertureWidthN
            boxSize += 32; // cleanApertureWidthD
            boxSize += 32; // cleanApertureHeightN
            boxSize += 32; // cleanApertureHeightD
            boxSize += 32; // horizOffN
            boxSize += 32; // horizOffD
            boxSize += 32; // vertOffN
            boxSize += 32; // vertOffD
            return boxSize;
        }
    }


    public class TrackGroupTypeBox : FullBox
    {
        public override string FourCC { get { return "msrc"; } }
        public uint track_group_id;  //  the remaining data may be specified 

        public TrackGroupTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_group_id
            /*   for a particular track_group_type */
            return boxSize;
        }
    }


    public class StereoVideoGroupBox : TrackGroupTypeBox
    {
        public override string FourCC { get { return "ster"; } }
        public bool left_view_flag;
        public uint reserved;

        public StereoVideoGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.left_view_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 31, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.left_view_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 31, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // left_view_flag
            boxSize += 31; // reserved
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox : Box
    {
        public override string FourCC { get { return "auxl"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox1 : Box
    {
        public override string FourCC { get { return "font"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox2 : Box
    {
        public override string FourCC { get { return "hind"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox3 : Box
    {
        public override string FourCC { get { return "hint"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox4 : Box
    {
        public override string FourCC { get { return "subt"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox5 : Box
    {
        public override string FourCC { get { return "thmb"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox6 : Box
    {
        public override string FourCC { get { return "vdep"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox6()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox7 : Box
    {
        public override string FourCC { get { return "vplx"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox7()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox8 : Box
    {
        public override string FourCC { get { return "cdsc"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox8()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox9 : Box
    {
        public override string FourCC { get { return "adda"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox9()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox10 : Box
    {
        public override string FourCC { get { return "adrc"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox10()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class HEVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvc1"; } }
        public HEVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public HEVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCLHVCSampleEntry : HEVCSampleEntry
    {
        public override string FourCC { get { return "hvc1"; } }
        public LHEVCConfigurationBox lhvcconfig;

        public HEVCLHVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            return boxSize;
        }
    }


    public class HEVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "hvc2"; } }
        public HEVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public HEVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCLHVCSampleEntry1 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hvc2"; } }
        public LHEVCConfigurationBox lhvcconfig;

        public HEVCLHVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            return boxSize;
        }
    }


    public class HEVCSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "hvc3"; } }
        public HEVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public HEVCSampleEntry2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCLHVCSampleEntry2 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hvc3"; } }
        public LHEVCConfigurationBox lhvcconfig;

        public HEVCLHVCSampleEntry2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            return boxSize;
        }
    }


    public class LHEVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "lhv1"; } }
        public LHEVCConfigurationBox lhvcconfig;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public LHEVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class LHEVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "lhe1"; } }
        public LHEVCConfigurationBox lhvcconfig;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public LHEVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "hev1"; } }
        public HEVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public HEVCSampleEntry3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCLHVCSampleEntry3 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hev1"; } }
        public LHEVCConfigurationBox lhvcconfig;

        public HEVCLHVCSampleEntry3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            return boxSize;
        }
    }


    public class HEVCSampleEntry4 : VisualSampleEntry
    {
        public override string FourCC { get { return "hev2"; } }
        public HEVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public HEVCSampleEntry4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCLHVCSampleEntry4 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hev2"; } }
        public LHEVCConfigurationBox lhvcconfig;

        public HEVCLHVCSampleEntry4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            return boxSize;
        }
    }


    public class HEVCSampleEntry5 : VisualSampleEntry
    {
        public override string FourCC { get { return "hev3"; } }
        public HEVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public HEVCSampleEntry5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class HEVCLHVCSampleEntry5 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hev3"; } }
        public LHEVCConfigurationBox lhvcconfig;

        public HEVCLHVCSampleEntry5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.lhvcconfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(lhvcconfig); // lhvcconfig
            return boxSize;
        }
    }


    public class AVCParameterSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "avcp"; } }
        public AVCConfigurationBox config;

        public AVCParameterSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class AVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "avc1"; } }
        public AVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public AVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class AVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "avc3"; } }
        public AVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public AVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class AVCMVCSampleEntry : AVCSampleEntry
    {
        public override string FourCC { get { return "avc1"; } }
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  optional
        public MVCConfigurationBox mvcconfig;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public AVCMVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // optional
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class AVCMVCSampleEntry1 : AVCSampleEntry
    {
        public override string FourCC { get { return "avc3"; } }
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  optional
        public MVCConfigurationBox mvcconfig;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public AVCMVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // optional
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class AVC2SampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "avc2"; } }
        public AVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public AVC2SampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class AVC2SampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "avc4"; } }
        public AVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public AVC2SampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class AVC2MVCSampleEntry : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc2"; } }
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  optional
        public MVCConfigurationBox mvcconfig;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public AVC2MVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // optional
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class AVC2MVCSampleEntry1 : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc4"; } }
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  optional
        public MVCConfigurationBox mvcconfig;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public AVC2MVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // optional
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.view_identifiers != null) boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.mvcconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc1"; } }
        public MVCConfigurationBox mvcconfig;  //  mandatory
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // mandatory
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc2"; } }
        public MVCConfigurationBox mvcconfig;  //  mandatory
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // mandatory
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc3"; } }
        public MVCConfigurationBox mvcconfig;  //  mandatory
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCSampleEntry2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // mandatory
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc4"; } }
        public MVCConfigurationBox mvcconfig;  //  mandatory
        public ViewScalabilityInformationSEIBox scalability;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public MVCDConfigurationBox mvcdconfig;  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCSampleEntry3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.view_priority_method); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcconfig); // mandatory
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.view_priority_method); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // optional
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcconfig); // mvcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.view_priority_method != null) boxSize += IsoReaderWriter.CalculateSize(view_priority_method); // view_priority_method
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.mvcdconfig != null) boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCDSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd1"; } }
        public MVCDConfigurationBox mvcdconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCDSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCDSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd2"; } }
        public MVCDConfigurationBox mvcdconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCDSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCDSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd3"; } }
        public MVCDConfigurationBox mvcdconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCDSampleEntry2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class MVCDSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd4"; } }
        public MVCDConfigurationBox mvcdconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional
        public A3DConfigurationBox a3dconfig;  //  optional

        public MVCDSampleEntry3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.mvcdconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.mvcdconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(mvcdconfig); // mvcdconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            if (this.a3dconfig != null) boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            return boxSize;
        }
    }


    public class A3DSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d1"; } }
        public A3DConfigurationBox a3dconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional

        public A3DSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            return boxSize;
        }
    }


    public class A3DSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d2"; } }
        public A3DConfigurationBox a3dconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional

        public A3DSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            return boxSize;
        }
    }


    public class A3DSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d3"; } }
        public A3DConfigurationBox a3dconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional

        public A3DSampleEntry2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            return boxSize;
        }
    }


    public class A3DSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d4"; } }
        public A3DConfigurationBox a3dconfig;  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei;  //  optional
        public ViewIdentifierBox view_identifiers;  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params;  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params;  //  optional

        public A3DSampleEntry3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.a3dconfig); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.ReadBox(stream, out this.view_identifiers); // mandatory
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.intrinsic_camera_params); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.a3dconfig); // mandatory
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei); // optional
            boxSize += IsoReaderWriter.WriteBox(stream, this.view_identifiers); // mandatory
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params); // optional
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(a3dconfig); // a3dconfig
            if (this.mvdscalinfosei != null) boxSize += IsoReaderWriter.CalculateSize(mvdscalinfosei); // mvdscalinfosei
            boxSize += IsoReaderWriter.CalculateSize(view_identifiers); // view_identifiers
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.intrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(intrinsic_camera_params); // intrinsic_camera_params
            if (this.extrinsic_camera_params != null) boxSize += IsoReaderWriter.CalculateSize(extrinsic_camera_params); // extrinsic_camera_params
            return boxSize;
        }
    }


    public class AVCSVCSampleEntry : AVCSampleEntry
    {
        public override string FourCC { get { return "avc1"; } }
        public SVCConfigurationBox svcconfig;  //  optional
        public ScalabilityInformationSEIBox scalability;  //  optional
        public SVCPriorityAssignmentBox method;  //  optional

        public AVCSVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.svcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.method); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.svcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.svcconfig); // optional
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.method); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.svcconfig != null) boxSize += IsoReaderWriter.CalculateSize(svcconfig); // svcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.method != null) boxSize += IsoReaderWriter.CalculateSize(method); // method
            return boxSize;
        }
    }


    public class AVCSVCSampleEntry1 : AVCSampleEntry
    {
        public override string FourCC { get { return "avc3"; } }
        public SVCConfigurationBox svcconfig;  //  optional
        public ScalabilityInformationSEIBox scalability;  //  optional
        public SVCPriorityAssignmentBox method;  //  optional

        public AVCSVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.svcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.method); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.svcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.svcconfig); // optional
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.method); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.svcconfig != null) boxSize += IsoReaderWriter.CalculateSize(svcconfig); // svcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.method != null) boxSize += IsoReaderWriter.CalculateSize(method); // method
            return boxSize;
        }
    }


    public class AVC2SVCSampleEntry : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc2"; } }
        public SVCConfigurationBox svcconfig;  //  optional
        public ScalabilityInformationSEIBox scalability;  //  optional
        public SVCPriorityAssignmentBox method;  //  optional

        public AVC2SVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.svcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.method); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.svcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.svcconfig); // optional
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.method); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.svcconfig != null) boxSize += IsoReaderWriter.CalculateSize(svcconfig); // svcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.method != null) boxSize += IsoReaderWriter.CalculateSize(method); // method
            return boxSize;
        }
    }


    public class AVC2SVCSampleEntry1 : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc4"; } }
        public SVCConfigurationBox svcconfig;  //  optional
        public ScalabilityInformationSEIBox scalability;  //  optional
        public SVCPriorityAssignmentBox method;  //  optional

        public AVC2SVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.svcconfig); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.method); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.svcconfig != null) boxSize += IsoReaderWriter.WriteBox(stream, this.svcconfig); // optional
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.method); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.svcconfig != null) boxSize += IsoReaderWriter.CalculateSize(svcconfig); // svcconfig
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.method != null) boxSize += IsoReaderWriter.CalculateSize(method); // method
            return boxSize;
        }
    }


    public class SVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "svc1"; } }
        public SVCConfigurationBox svcconfig;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public ScalabilityInformationSEIBox scalability;  //  optional
        public SVCPriorityAssignmentBox method;  //  optional

        public SVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.svcconfig);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.method); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.method); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(svcconfig); // svcconfig
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.method != null) boxSize += IsoReaderWriter.CalculateSize(method); // method
            return boxSize;
        }
    }


    public class SVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "svc2"; } }
        public SVCConfigurationBox svcconfig;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional
        public ScalabilityInformationSEIBox scalability;  //  optional
        public SVCPriorityAssignmentBox method;  //  optional

        public SVCSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.svcconfig);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.scalability); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.method); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            if (this.scalability != null) boxSize += IsoReaderWriter.WriteBox(stream, this.scalability); // optional
            if (this.method != null) boxSize += IsoReaderWriter.WriteBox(stream, this.method); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(svcconfig); // svcconfig
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            if (this.scalability != null) boxSize += IsoReaderWriter.CalculateSize(scalability); // scalability
            if (this.method != null) boxSize += IsoReaderWriter.CalculateSize(method); // method
            return boxSize;
        }
    }


    public class HEVCTileSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvt1"; } }
        public HEVCTileConfigurationBox config;  //  optional

        public HEVCTileSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.config); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.config != null) boxSize += IsoReaderWriter.WriteBox(stream, this.config); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.config != null) boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class LHEVCTileSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "lht1"; } }

        public LHEVCTileSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class HEVCTileSSHInfoSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvt3"; } }
        public HEVCTileConfigurationBox config;  //  optional 

        public HEVCTileSSHInfoSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.config); // optional 
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.config != null) boxSize += IsoReaderWriter.WriteBox(stream, this.config); // optional 
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.config != null) boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class HEVCSliceSegmentDataSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvt2"; } }
        public HEVCTileConfigurationBox config;  //  optional

        public HEVCSliceSegmentDataSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.config); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            if (this.config != null) boxSize += IsoReaderWriter.WriteBox(stream, this.config); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            if (this.config != null) boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class VvcSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "vvc1"; } }
        public VvcConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public VvcSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class VvcSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "vvi1"; } }
        public VvcConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public VvcSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class VvcSubpicSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "vvs1"; } }
        public VvcNALUConfigBox config;

        public VvcSubpicSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class VvcNonVCLSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "vvcN"; } }
        public VvcNALUConfigBox config;

        public VvcNonVCLSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class EVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "evc1"; } }
        public EVCConfigurationBox config;
        public MPEG4ExtensionDescriptorsBox descr;  //  optional

        public EVCSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.descr); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.descr != null) boxSize += IsoReaderWriter.WriteBox(stream, this.descr); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.descr != null) boxSize += IsoReaderWriter.CalculateSize(descr); // descr
            return boxSize;
        }
    }


    public class SVCMetadataSampleEntry : MetadataSampleEntry
    {
        public override string FourCC { get { return "svcM"; } }
        public SVCMetadataSampleConfigBox config;
        public SVCPriorityAssignmentBox methods;  //  optional
        public SVCPriorityLayerInfoBox priorities;  //  optional

        public SVCMetadataSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.methods); // optional
            if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.priorities); // optional
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            if (this.methods != null) boxSize += IsoReaderWriter.WriteBox(stream, this.methods); // optional
            if (this.priorities != null) boxSize += IsoReaderWriter.WriteBox(stream, this.priorities); // optional
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            if (this.methods != null) boxSize += IsoReaderWriter.CalculateSize(methods); // methods
            if (this.priorities != null) boxSize += IsoReaderWriter.CalculateSize(priorities); // priorities
            return boxSize;
        }
    }


    public class EVCSliceComponentTrackSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "evs1"; } }
        public EVCSliceComponentTrackConfigurationBox config;

        public EVCSliceComponentTrackSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class EVCSliceComponentTrackSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "evs2"; } }
        public EVCSliceComponentTrackConfigurationBox config;

        public EVCSliceComponentTrackSampleEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.config);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.config);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(config); // config
            return boxSize;
        }
    }


    public class SubpicCommonGroupBox : EntityToGroupBox
    {
        public override string FourCC { get { return "acgl"; } }
        public bool level_is_present_flag;
        public bool level_is_static_flag;
        public byte reserved = 0;
        public byte level_idc;
        public uint level_info_entity_idx;
        public ushort num_active_tracks;

        public SubpicCommonGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.level_is_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.level_is_static_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);

            if (level_is_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.level_idc);
            }

            if (level_is_static_flag == false)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.level_info_entity_idx);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_active_tracks);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.level_is_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.level_is_static_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);

            if (level_is_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.level_idc);
            }

            if (level_is_static_flag == false)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.level_info_entity_idx);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_active_tracks);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // level_is_present_flag
            boxSize += 1; // level_is_static_flag
            boxSize += 6; // reserved

            if (level_is_present_flag)
            {
                boxSize += 8; // level_idc
            }

            if (level_is_static_flag == false)
            {
                boxSize += 32; // level_info_entity_idx
            }
            boxSize += 16; // num_active_tracks
            return boxSize;
        }
    }


    public class SubpicMultipleGroupsBox : EntityToGroupBox
    {
        public override string FourCC { get { return "amgl"; } }
        public bool level_is_present_flag;
        public bool level_is_static_flag;
        public byte reserved = 0;
        public byte level_idc;
        public uint level_info_entity_idx;
        public ushort num_subgroup_ids;
        public uint[] track_subgroup_id;
        public ushort[] num_active_tracks;

        public SubpicMultipleGroupsBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.level_is_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.level_is_static_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);

            if (level_is_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.level_idc);
            }

            if (level_is_static_flag == false)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.level_info_entity_idx);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_subgroup_ids);
            ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_subgroup_id[i]);
            }

            for (int i = 0; i < num_subgroup_ids; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_active_tracks[i]);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.level_is_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.level_is_static_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);

            if (level_is_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.level_idc);
            }

            if (level_is_static_flag == false)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.level_info_entity_idx);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_subgroup_ids);
            ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_subgroup_id[i]);
            }

            for (int i = 0; i < num_subgroup_ids; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_active_tracks[i]);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // level_is_present_flag
            boxSize += 1; // level_is_static_flag
            boxSize += 7; // reserved

            if (level_is_present_flag)
            {
                boxSize += 8; // level_idc
            }

            if (level_is_static_flag == false)
            {
                boxSize += 32; // level_info_entity_idx
            }
            boxSize += 16; // num_subgroup_ids
            ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // track_subgroup_id
            }

            for (int i = 0; i < num_subgroup_ids; i++)
            {
                boxSize += 16; // num_active_tracks
            }
            return boxSize;
        }
    }


    public class OperatingPointGroupBox : EntityToGroupBox
    {
        public override string FourCC { get { return "opeg"; } }
        public byte num_profile_tier_level_minus1;
        public VvcPTLRecord[] opeg_ptl;
        public byte reserved = 0;
        public bool incomplete_operating_points_flag;
        public ushort num_olss;
        public byte[] ptl_idx;
        public ushort[] ols_idx;
        public byte[] layer_count;
        public bool reserved0 = false;
        public bool[] layer_info_present_flag;
        public byte[][] layer_id;
        public byte[][] is_output_layer;
        public bool reserved00 = false;
        public byte reserved1 = 0;
        public ushort num_operating_points;
        public ushort ols_loop_entry_idx;
        public byte max_temporal_id;
        public bool frame_rate_info_flag;
        public bool bit_rate_info_flag;
        public byte op_availability_idc;
        public byte reserved2 = 0;
        public byte reserved01 = 0;
        public byte chroma_format_idc;
        public byte bit_depth_minus8;
        public ushort max_picture_width;
        public ushort max_picture_height;
        public ushort avg_frame_rate;
        public byte reserved10 = 0;
        public byte constant_frame_rate;
        public uint max_bit_rate;
        public uint avg_bit_rate;
        public byte entity_count;
        public byte entity_idx;

        public OperatingPointGroupBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.num_profile_tier_level_minus1);

            for (int i = 0; i <= num_profile_tier_level_minus1; i++)
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.opeg_ptl[i]);
            }
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.incomplete_operating_points_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 9, out this.num_olss);

            for (int i = 0; i < num_olss; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.ptl_idx[i]);
                boxSize += IsoReaderWriter.ReadBits(stream, 9, out this.ols_idx[i]);
                boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.layer_count[i]);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved0);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.layer_info_present_flag[i]);

                if (layer_info_present_flag[i])
                {

                    for (int j = 0; j < layer_count[i]; j++)
                    {
                        boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.layer_id[i][j]);
                        boxSize += IsoReaderWriter.ReadBits(stream, 1, out this.is_output_layer[i][j]);
                        boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved00);
                    }
                }
            }
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved1);
            boxSize += IsoReaderWriter.ReadBits(stream, 12, out this.num_operating_points);

            for (int i = 0; i < num_operating_points; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 9, out this.ols_loop_entry_idx);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.max_temporal_id);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.frame_rate_info_flag);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.bit_rate_info_flag);

                if (incomplete_operating_points_flag)
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.op_availability_idc);
                }

                else
                {
                    boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved2);
                }
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.reserved01);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.chroma_format_idc);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.bit_depth_minus8);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_picture_width);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_picture_height);

                if (frame_rate_info_flag)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.avg_frame_rate);
                    boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved10);
                    boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.constant_frame_rate);
                }

                if (bit_rate_info_flag)
                {
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.max_bit_rate);
                    boxSize += IsoReaderWriter.ReadUInt32(stream, out this.avg_bit_rate);
                }
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entity_count);

                for (int j = 0; j < entity_count; j++)
                {
                    boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entity_idx);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.num_profile_tier_level_minus1);

            for (int i = 0; i <= num_profile_tier_level_minus1; i++)
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.opeg_ptl[i]);
            }
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.incomplete_operating_points_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 9, this.num_olss);

            for (int i = 0; i < num_olss; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.ptl_idx[i]);
                boxSize += IsoReaderWriter.WriteBits(stream, 9, this.ols_idx[i]);
                boxSize += IsoReaderWriter.WriteBits(stream, 6, this.layer_count[i]);
                boxSize += IsoReaderWriter.WriteBit(stream, this.reserved0);
                boxSize += IsoReaderWriter.WriteBit(stream, this.layer_info_present_flag[i]);

                if (layer_info_present_flag[i])
                {

                    for (int j = 0; j < layer_count[i]; j++)
                    {
                        boxSize += IsoReaderWriter.WriteBits(stream, 6, this.layer_id[i][j]);
                        boxSize += IsoReaderWriter.WriteBits(stream, 1, this.is_output_layer[i][j]);
                        boxSize += IsoReaderWriter.WriteBit(stream, this.reserved00);
                    }
                }
            }
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved1);
            boxSize += IsoReaderWriter.WriteBits(stream, 12, this.num_operating_points);

            for (int i = 0; i < num_operating_points; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 9, this.ols_loop_entry_idx);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.max_temporal_id);
                boxSize += IsoReaderWriter.WriteBit(stream, this.frame_rate_info_flag);
                boxSize += IsoReaderWriter.WriteBit(stream, this.bit_rate_info_flag);

                if (incomplete_operating_points_flag)
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, 2, this.op_availability_idc);
                }

                else
                {
                    boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved2);
                }
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.reserved01);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.chroma_format_idc);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.bit_depth_minus8);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_picture_width);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_picture_height);

                if (frame_rate_info_flag)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.avg_frame_rate);
                    boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved10);
                    boxSize += IsoReaderWriter.WriteBits(stream, 2, this.constant_frame_rate);
                }

                if (bit_rate_info_flag)
                {
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.max_bit_rate);
                    boxSize += IsoReaderWriter.WriteUInt32(stream, this.avg_bit_rate);
                }
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.entity_count);

                for (int j = 0; j < entity_count; j++)
                {
                    boxSize += IsoReaderWriter.WriteUInt8(stream, this.entity_idx);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // num_profile_tier_level_minus1

            for (int i = 0; i <= num_profile_tier_level_minus1; i++)
            {
                boxSize += IsoReaderWriter.CalculateClassSize(opeg_ptl); // opeg_ptl
            }
            boxSize += 6; // reserved
            boxSize += 1; // incomplete_operating_points_flag
            boxSize += 9; // num_olss

            for (int i = 0; i < num_olss; i++)
            {
                boxSize += 8; // ptl_idx
                boxSize += 9; // ols_idx
                boxSize += 6; // layer_count
                boxSize += 1; // reserved0
                boxSize += 1; // layer_info_present_flag

                if (layer_info_present_flag[i])
                {

                    for (int j = 0; j < layer_count[i]; j++)
                    {
                        boxSize += 6; // layer_id
                        boxSize += 1; // is_output_layer
                        boxSize += 1; // reserved00
                    }
                }
            }
            boxSize += 4; // reserved1
            boxSize += 12; // num_operating_points

            for (int i = 0; i < num_operating_points; i++)
            {
                boxSize += 9; // ols_loop_entry_idx
                boxSize += 3; // max_temporal_id
                boxSize += 1; // frame_rate_info_flag
                boxSize += 1; // bit_rate_info_flag

                if (incomplete_operating_points_flag)
                {
                    boxSize += 2; // op_availability_idc
                }

                else
                {
                    boxSize += 2; // reserved2
                }
                boxSize += 3; // reserved01
                boxSize += 2; // chroma_format_idc
                boxSize += 3; // bit_depth_minus8
                boxSize += 16; // max_picture_width
                boxSize += 16; // max_picture_height

                if (frame_rate_info_flag)
                {
                    boxSize += 16; // avg_frame_rate
                    boxSize += 6; // reserved10
                    boxSize += 2; // constant_frame_rate
                }

                if (bit_rate_info_flag)
                {
                    boxSize += 32; // max_bit_rate
                    boxSize += 32; // avg_bit_rate
                }
                boxSize += 8; // entity_count

                for (int j = 0; j < entity_count; j++)
                {
                    boxSize += 8; // entity_idx
                }
            }
            return boxSize;
        }
    }


    public class SwitchableTracks : EntityToGroupBox
    {
        public override string FourCC { get { return "swtk"; } }
        public ushort[] track_switch_hierarchy_id;

        public SwitchableTracks()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.track_switch_hierarchy_id[i]);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.track_switch_hierarchy_id[i]);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 16; // track_switch_hierarchy_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox1 : FullBox
    {
        public override string FourCC { get { return "vvcb"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class AUDSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "aud "; } }
        public uint audNalUnit;

        public AUDSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 24, out this.audNalUnit);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 24, this.audNalUnit);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 24; // audNalUnit
            return boxSize;
        }
    }


    public class AVCLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "avll"; } }
        public byte layerNumber;
        public byte reserved = 0;
        public bool accurateStatisticsFlag;
        public ushort avgBitRate;
        public ushort avgFrameRate;

        public AVCLayerEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.layerNumber);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.accurateStatisticsFlag);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.avgBitRate);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.avgFrameRate);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.layerNumber);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.accurateStatisticsFlag);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.avgBitRate);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.avgFrameRate);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // layerNumber
            boxSize += 7; // reserved
            boxSize += 1; // accurateStatisticsFlag
            boxSize += 16; // avgBitRate
            boxSize += 16; // avgFrameRate
            return boxSize;
        }
    }


    public class AVCSubSequenceEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "avss"; } }
        public ushort subSequenceIdentifer;
        public byte layerNumber;
        public bool durationFlag;
        public bool avgRateFlag;
        public byte reserved = 0;
        public uint duration;
        public byte reserved0 = 0;
        public bool accurateStatisticsFlag;
        public ushort avgBitRate;
        public ushort avgFrameRate;
        public byte numReferences;
        public DependencyInfo[] dependency;

        public AVCSubSequenceEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subSequenceIdentifer);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.layerNumber);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.durationFlag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.avgRateFlag);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);

            if (durationFlag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.duration);
            }

            if (avgRateFlag)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved0);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.accurateStatisticsFlag);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.avgBitRate);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.avgFrameRate);
            }
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.numReferences);
            boxSize += IsoReaderWriter.ReadClasses(stream, numReferences, out this.dependency);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.subSequenceIdentifer);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.layerNumber);
            boxSize += IsoReaderWriter.WriteBit(stream, this.durationFlag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.avgRateFlag);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);

            if (durationFlag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.duration);
            }

            if (avgRateFlag)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved0);
                boxSize += IsoReaderWriter.WriteBit(stream, this.accurateStatisticsFlag);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.avgBitRate);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.avgFrameRate);
            }
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.numReferences);
            boxSize += IsoReaderWriter.WriteClasses(stream, numReferences, this.dependency);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // subSequenceIdentifer
            boxSize += 8; // layerNumber
            boxSize += 1; // durationFlag
            boxSize += 1; // avgRateFlag
            boxSize += 6; // reserved

            if (durationFlag)
            {
                boxSize += 32; // duration
            }

            if (avgRateFlag)
            {
                boxSize += 7; // reserved0
                boxSize += 1; // accurateStatisticsFlag
                boxSize += 16; // avgBitRate
                boxSize += 16; // avgFrameRate
            }
            boxSize += 8; // numReferences
            boxSize += IsoReaderWriter.CalculateClassSize(dependency); // dependency
            return boxSize;
        }
    }


    public class DecodingCapabilityInformation : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dcfi"; } }
        public ushort dci_nal_unit_length;
        public byte[] dci_nal_unit;

        public DecodingCapabilityInformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.dci_nal_unit_length);
            boxSize += IsoReaderWriter.ReadBytes(stream, dci_nal_unit_length, out this.dci_nal_unit);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.dci_nal_unit_length);
            boxSize += IsoReaderWriter.WriteBytes(stream, dci_nal_unit_length, this.dci_nal_unit);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // dci_nal_unit_length
            boxSize += (ulong)dci_nal_unit_length * 8; // dci_nal_unit
            return boxSize;
        }
    }


    public class DecodeRetimingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dtrt"; } }
        public byte tierCount;
        public ushort tierID;
        public short delta;

        public DecodeRetimingEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.tierCount);

            for (int i = 1; i <= tierCount; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tierID);
                boxSize += IsoReaderWriter.ReadInt16(stream, out this.delta);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.tierCount);

            for (int i = 1; i <= tierCount; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.tierID);
                boxSize += IsoReaderWriter.WriteInt16(stream, this.delta);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // tierCount

            for (int i = 1; i <= tierCount; i++)
            {
                boxSize += 16; // tierID
                boxSize += 16; // delta
            }
            return boxSize;
        }
    }


    public class EndOfBitstreamSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "eob "; } }
        public ushort eobNalUnit;

        public EndOfBitstreamSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.eobNalUnit);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.eobNalUnit);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // eobNalUnit
            return boxSize;
        }
    }


    public class EndOfSequenceSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "eos "; } }
        public byte num_eos_nal_unit_minus1;
        public ushort[] eosNalUnit;

        public EndOfSequenceSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.num_eos_nal_unit_minus1);

            for (int i = 0; i <= num_eos_nal_unit_minus1; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.eosNalUnit[i]);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.num_eos_nal_unit_minus1);

            for (int i = 0; i <= num_eos_nal_unit_minus1; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.eosNalUnit[i]);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // num_eos_nal_unit_minus1

            for (int i = 0; i <= num_eos_nal_unit_minus1; i++)
            {
                boxSize += 16; // eosNalUnit
            }
            return boxSize;
        }
    }


    public class LhvcExternalBaseLayerInfo : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "lbli"; } }
        public bool reserved = true;
        public bool bl_irap_pic_flag;
        public byte bl_irap_nal_unit_type;
        public sbyte sample_offset;

        public LhvcExternalBaseLayerInfo()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.bl_irap_pic_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.bl_irap_nal_unit_type);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.sample_offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.bl_irap_pic_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.bl_irap_nal_unit_type);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.sample_offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // reserved
            boxSize += 1; // bl_irap_pic_flag
            boxSize += 6; // bl_irap_nal_unit_type
            boxSize += 8; // sample_offset
            return boxSize;
        }
    }


    public class LayerInfoGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "linf"; } }
        public byte reserved = 0;
        public byte num_layers_in_track;
        public byte reserved0 = 0;
        public bool irap_gdr_pics_in_layer_only_flag;
        public bool completeness_flag;
        public byte layer_id;
        public byte min_TemporalId;
        public byte max_TemporalId;
        public bool reserved00 = false;
        public byte sub_layer_presence_flags;

        public LayerInfoGroupEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.num_layers_in_track);

            for (int i = 0; i < num_layers_in_track; i++)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved0);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.irap_gdr_pics_in_layer_only_flag);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.completeness_flag);
                boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.layer_id);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.min_TemporalId);
                boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.max_TemporalId);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved00);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.sub_layer_presence_flags);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.num_layers_in_track);

            for (int i = 0; i < num_layers_in_track; i++)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved0);
                boxSize += IsoReaderWriter.WriteBit(stream, this.irap_gdr_pics_in_layer_only_flag);
                boxSize += IsoReaderWriter.WriteBit(stream, this.completeness_flag);
                boxSize += IsoReaderWriter.WriteBits(stream, 6, this.layer_id);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.min_TemporalId);
                boxSize += IsoReaderWriter.WriteBits(stream, 3, this.max_TemporalId);
                boxSize += IsoReaderWriter.WriteBit(stream, this.reserved00);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.sub_layer_presence_flags);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2; // reserved
            boxSize += 6; // num_layers_in_track

            for (int i = 0; i < num_layers_in_track; i++)
            {
                boxSize += 2; // reserved0
                boxSize += 1; // irap_gdr_pics_in_layer_only_flag
                boxSize += 1; // completeness_flag
                boxSize += 6; // layer_id
                boxSize += 3; // min_TemporalId
                boxSize += 3; // max_TemporalId
                boxSize += 1; // reserved00
                boxSize += 7; // sub_layer_presence_flags
            }
            return boxSize;
        }
    }


    public class VvcMixedNALUnitTypePicEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "minp"; } }
        public ushort num_mix_nalu_pic_idx;
        public ushort[] mix_subp_track_idx1;
        public ushort[] mix_subp_track_idx2;
        public ushort pps_mix_nalu_types_in_pic_bit_pos;
        public byte pps_id;

        public VvcMixedNALUnitTypePicEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_mix_nalu_pic_idx);

            for (int i = 0; i < num_mix_nalu_pic_idx; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.mix_subp_track_idx1[i]);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.mix_subp_track_idx2[i]);
            }
            boxSize += IsoReaderWriter.ReadBits(stream, 10, out this.pps_mix_nalu_types_in_pic_bit_pos);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.pps_id);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_mix_nalu_pic_idx);

            for (int i = 0; i < num_mix_nalu_pic_idx; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.mix_subp_track_idx1[i]);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.mix_subp_track_idx2[i]);
            }
            boxSize += IsoReaderWriter.WriteBits(stream, 10, this.pps_mix_nalu_types_in_pic_bit_pos);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.pps_id);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // num_mix_nalu_pic_idx

            for (int i = 0; i < num_mix_nalu_pic_idx; i++)
            {
                boxSize += 16; // mix_subp_track_idx1
                boxSize += 16; // mix_subp_track_idx2
            }
            boxSize += 10; // pps_mix_nalu_types_in_pic_bit_pos
            boxSize += 6; // pps_id
            return boxSize;
        }
    }


    public class MultiviewGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "mvif"; } }
        public byte groupID;
        public byte primary_groupID;
        public byte reserved = 0;
        public bool is_tl_switching_point;
        public byte reserved0 = 0;
        public byte tl_switching_distance;
        public ViewIdentifierBox ViewIdentifierBox;  //  Mandatory
        public TierInfoBox TierInfoBox;  //  Mandatory
        public TierDependencyBox TierDependencyBox;  //  Mandatory
        public PriorityRangeBox PriorityRangeBox;  //  Mandatory
        public TierBitRateBox TierBitRateBox;  //  optional
        public BufferingBox BufferingBox;  //  optional
        public InitialParameterSetBox InitialParameterSetBox;  //  optional
        public ProtectionSchemeInfoBox ProtectionSchemeInfoBox;  //  optional
        public ViewPriorityBox ViewPriorityBox;  //  optional

        public MultiviewGroupEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.groupID);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.primary_groupID);
            boxSize += IsoReaderWriter.ReadBits(stream, 4, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.is_tl_switching_point);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.reserved0);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.tl_switching_distance);

            if (groupID == primary_groupID)
            {
                boxSize += IsoReaderWriter.ReadBox(stream, out this.ViewIdentifierBox); // Mandatory
                boxSize += IsoReaderWriter.ReadBox(stream, out this.TierInfoBox); // Mandatory
                boxSize += IsoReaderWriter.ReadBox(stream, out this.TierDependencyBox); // Mandatory
                boxSize += IsoReaderWriter.ReadBox(stream, out this.PriorityRangeBox); // Mandatory
                /* Optional Boxes or fields may follow when defined later */
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TierBitRateBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.BufferingBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.InitialParameterSetBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.ProtectionSchemeInfoBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.ViewPriorityBox); // optional
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.groupID);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.primary_groupID);
            boxSize += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.is_tl_switching_point);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.reserved0);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.tl_switching_distance);

            if (groupID == primary_groupID)
            {
                boxSize += IsoReaderWriter.WriteBox(stream, this.ViewIdentifierBox); // Mandatory
                boxSize += IsoReaderWriter.WriteBox(stream, this.TierInfoBox); // Mandatory
                boxSize += IsoReaderWriter.WriteBox(stream, this.TierDependencyBox); // Mandatory
                boxSize += IsoReaderWriter.WriteBox(stream, this.PriorityRangeBox); // Mandatory
                /* Optional Boxes or fields may follow when defined later */
                if (this.TierBitRateBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TierBitRateBox); // optional
                if (this.BufferingBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.BufferingBox); // optional
                if (this.InitialParameterSetBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.InitialParameterSetBox); // optional
                if (this.ProtectionSchemeInfoBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.ProtectionSchemeInfoBox); // optional
                if (this.ViewPriorityBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.ViewPriorityBox); // optional
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // groupID
            boxSize += 8; // primary_groupID
            boxSize += 4; // reserved
            boxSize += 1; // is_tl_switching_point
            boxSize += 3; // reserved0
            boxSize += 8; // tl_switching_distance

            if (groupID == primary_groupID)
            {
                boxSize += IsoReaderWriter.CalculateSize(ViewIdentifierBox); // ViewIdentifierBox
                boxSize += IsoReaderWriter.CalculateSize(TierInfoBox); // TierInfoBox
                boxSize += IsoReaderWriter.CalculateSize(TierDependencyBox); // TierDependencyBox
                boxSize += IsoReaderWriter.CalculateSize(PriorityRangeBox); // PriorityRangeBox
                /* Optional Boxes or fields may follow when defined later */
                if (this.TierBitRateBox != null) boxSize += IsoReaderWriter.CalculateSize(TierBitRateBox); // TierBitRateBox
                if (this.BufferingBox != null) boxSize += IsoReaderWriter.CalculateSize(BufferingBox); // BufferingBox
                if (this.InitialParameterSetBox != null) boxSize += IsoReaderWriter.CalculateSize(InitialParameterSetBox); // InitialParameterSetBox
                if (this.ProtectionSchemeInfoBox != null) boxSize += IsoReaderWriter.CalculateSize(ProtectionSchemeInfoBox); // ProtectionSchemeInfoBox
                if (this.ViewPriorityBox != null) boxSize += IsoReaderWriter.CalculateSize(ViewPriorityBox); // ViewPriorityBox
            }
            return boxSize;
        }
    }


    public class NALUMapEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "nalm"; } }
        public byte reserved = 0;
        public bool large_size;
        public bool rle;
        public ushort entry_count;
        public byte entry_count0;
        public ushort NALU_start_number;
        public byte NALU_start_number0;
        public ushort groupID;

        public NALUMapEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.large_size);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.rle);

            if (large_size)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (rle)
                {

                    if (large_size)
                    {
                        boxSize += IsoReaderWriter.ReadUInt16(stream, out this.NALU_start_number);
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.ReadUInt8(stream, out this.NALU_start_number0);
                    }
                }
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.groupID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.large_size);
            boxSize += IsoReaderWriter.WriteBit(stream, this.rle);

            if (large_size)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.entry_count0);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (rle)
                {

                    if (large_size)
                    {
                        boxSize += IsoReaderWriter.WriteUInt16(stream, this.NALU_start_number);
                    }

                    else
                    {
                        boxSize += IsoReaderWriter.WriteUInt8(stream, this.NALU_start_number0);
                    }
                }
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.groupID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 6; // reserved
            boxSize += 1; // large_size
            boxSize += 1; // rle

            if (large_size)
            {
                boxSize += 16; // entry_count
            }

            else
            {
                boxSize += 8; // entry_count0
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (rle)
                {

                    if (large_size)
                    {
                        boxSize += 16; // NALU_start_number
                    }

                    else
                    {
                        boxSize += 8; // NALU_start_number0
                    }
                }
                boxSize += 16; // groupID
            }
            return boxSize;
        }
    }


    public class OperatingPointsInformation : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "oinf"; } }
        public OperatingPointsRecord oinf;

        public OperatingPointsInformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.oinf);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.oinf);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(oinf); // oinf
            return boxSize;
        }
    }


    public class OperatingPointDecodeTimeHint : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "opth"; } }
        public int delta_time;

        public OperatingPointDecodeTimeHint()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt32(stream, out this.delta_time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt32(stream, this.delta_time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // delta_time
            return boxSize;
        }
    }


    public class ParameterSetNALUEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pase"; } }
        public ushort ps_nalu_length;
        public byte[] ps_nal_unit;

        public ParameterSetNALUEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.ps_nalu_length);
            boxSize += IsoReaderWriter.ReadBytes(stream, ps_nalu_length, out this.ps_nal_unit);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.ps_nalu_length);
            boxSize += IsoReaderWriter.WriteBytes(stream, ps_nalu_length, this.ps_nal_unit);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // ps_nalu_length
            boxSize += (ulong)ps_nalu_length * 8; // ps_nal_unit
            return boxSize;
        }
    }


    public class PSSampleGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pss1"; } }
        public bool sps_present;
        public bool pps_present;
        public bool aps_present;
        public byte reserved = 0;

        public PSSampleGroupEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.sps_present);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.pps_present);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.aps_present);
            boxSize += IsoReaderWriter.ReadBits(stream, 5, out this.reserved);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.sps_present);
            boxSize += IsoReaderWriter.WriteBit(stream, this.pps_present);
            boxSize += IsoReaderWriter.WriteBit(stream, this.aps_present);
            boxSize += IsoReaderWriter.WriteBits(stream, 5, this.reserved);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // sps_present
            boxSize += 1; // pps_present
            boxSize += 1; // aps_present
            boxSize += 5; // reserved
            return boxSize;
        }
    }


    public class VvcRectRegionOrderEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "rror"; } }
        public bool subpic_id_info_flag;
        public byte reserved = 0;
        public ushort num_alternate_region_set;
        public ushort[] num_regions_in_set;
        public ushort[] alternate_region_set_id;
        public ushort[][] groupID;
        public ushort num_regions_minus1;
        public ushort[] region_id;
        public VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info;

        public VvcRectRegionOrderEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.subpic_id_info_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_alternate_region_set);

            for (int i = 0; i < num_alternate_region_set; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_regions_in_set[i]);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.alternate_region_set_id[i]);

                for (int j = 0; j < num_regions_in_set[i]; j++)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.groupID[i][j]);
                }
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.num_regions_minus1);

            for (int i = 0; i < num_regions_minus1; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.region_id[i]);
            }

            if (subpic_id_info_flag)
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.subpic_id_rewriting_info);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.subpic_id_info_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_alternate_region_set);

            for (int i = 0; i < num_alternate_region_set; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_regions_in_set[i]);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.alternate_region_set_id[i]);

                for (int j = 0; j < num_regions_in_set[i]; j++)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.groupID[i][j]);
                }
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.num_regions_minus1);

            for (int i = 0; i < num_regions_minus1; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.region_id[i]);
            }

            if (subpic_id_info_flag)
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.subpic_id_rewriting_info);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // subpic_id_info_flag
            boxSize += 7; // reserved
            boxSize += 16; // num_alternate_region_set

            for (int i = 0; i < num_alternate_region_set; i++)
            {
                boxSize += 16; // num_regions_in_set
                boxSize += 16; // alternate_region_set_id

                for (int j = 0; j < num_regions_in_set[i]; j++)
                {
                    boxSize += 16; // groupID
                }
            }
            boxSize += 16; // num_regions_minus1

            for (int i = 0; i < num_regions_minus1; i++)
            {
                boxSize += 16; // region_id
            }

            if (subpic_id_info_flag)
            {
                boxSize += IsoReaderWriter.CalculateClassSize(subpic_id_rewriting_info); // subpic_id_rewriting_info
            }
            return boxSize;
        }
    }


    public class ScalableGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "scif"; } }
        public byte groupID;
        public byte primary_groupID;
        public bool is_tier_IDR;
        public bool noInterLayerPredFlag;
        public bool useRefBasePicFlag;
        public bool storeBaseRepFlag;
        public bool is_tl_switching_point;
        public byte reserved = 0;
        public byte tl_switching_distance;
        public TierInfoBox TierInfoBox;  //  Mandatory
        public SVCDependencyRangeBox SVCDependencyRangeBox;  //  Mandatory
        public PriorityRangeBox PriorityRangeBox;  //  Mandatory
        public TierBitRateBox TierBitRateBox;  //  optional
        public RectRegionBox RectRegionBox;  //  optional
        public BufferingBox BufferingBox;  //  optional
        public TierDependencyBox TierDependencyBox;  //  optional
        public InitialParameterSetBox InitialParameterSetBox;  //  optional
        public IroiInfoBox IroiInfoBox;  //  optional
        public ProtectionSchemeInfoBox ProtectionSchemeInfoBox;  //  optional
        public TranscodingInfoBox TranscodingInfoBox;  //  optional

        public ScalableGroupEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.groupID);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.primary_groupID);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.is_tier_IDR);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.noInterLayerPredFlag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.useRefBasePicFlag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.storeBaseRepFlag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.is_tl_switching_point);
            boxSize += IsoReaderWriter.ReadBits(stream, 3, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.tl_switching_distance);

            if (groupID == primary_groupID)
            {
                boxSize += IsoReaderWriter.ReadBox(stream, out this.TierInfoBox); // Mandatory
                boxSize += IsoReaderWriter.ReadBox(stream, out this.SVCDependencyRangeBox); // Mandatory
                boxSize += IsoReaderWriter.ReadBox(stream, out this.PriorityRangeBox); // Mandatory
                /* Optional Boxes or fields may follow when defined later */
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TierBitRateBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.RectRegionBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.BufferingBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TierDependencyBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.InitialParameterSetBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.IroiInfoBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.ProtectionSchemeInfoBox); // optional
                if (boxSize < size) boxSize += IsoReaderWriter.ReadBox(stream, out this.TranscodingInfoBox); // optional
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.groupID);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.primary_groupID);
            boxSize += IsoReaderWriter.WriteBit(stream, this.is_tier_IDR);
            boxSize += IsoReaderWriter.WriteBit(stream, this.noInterLayerPredFlag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.useRefBasePicFlag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.storeBaseRepFlag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.is_tl_switching_point);
            boxSize += IsoReaderWriter.WriteBits(stream, 3, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.tl_switching_distance);

            if (groupID == primary_groupID)
            {
                boxSize += IsoReaderWriter.WriteBox(stream, this.TierInfoBox); // Mandatory
                boxSize += IsoReaderWriter.WriteBox(stream, this.SVCDependencyRangeBox); // Mandatory
                boxSize += IsoReaderWriter.WriteBox(stream, this.PriorityRangeBox); // Mandatory
                /* Optional Boxes or fields may follow when defined later */
                if (this.TierBitRateBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TierBitRateBox); // optional
                if (this.RectRegionBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.RectRegionBox); // optional
                if (this.BufferingBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.BufferingBox); // optional
                if (this.TierDependencyBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TierDependencyBox); // optional
                if (this.InitialParameterSetBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.InitialParameterSetBox); // optional
                if (this.IroiInfoBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.IroiInfoBox); // optional
                if (this.ProtectionSchemeInfoBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.ProtectionSchemeInfoBox); // optional
                if (this.TranscodingInfoBox != null) boxSize += IsoReaderWriter.WriteBox(stream, this.TranscodingInfoBox); // optional
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // groupID
            boxSize += 8; // primary_groupID
            boxSize += 1; // is_tier_IDR
            boxSize += 1; // noInterLayerPredFlag
            boxSize += 1; // useRefBasePicFlag
            boxSize += 1; // storeBaseRepFlag
            boxSize += 1; // is_tl_switching_point
            boxSize += 3; // reserved
            boxSize += 8; // tl_switching_distance

            if (groupID == primary_groupID)
            {
                boxSize += IsoReaderWriter.CalculateSize(TierInfoBox); // TierInfoBox
                boxSize += IsoReaderWriter.CalculateSize(SVCDependencyRangeBox); // SVCDependencyRangeBox
                boxSize += IsoReaderWriter.CalculateSize(PriorityRangeBox); // PriorityRangeBox
                /* Optional Boxes or fields may follow when defined later */
                if (this.TierBitRateBox != null) boxSize += IsoReaderWriter.CalculateSize(TierBitRateBox); // TierBitRateBox
                if (this.RectRegionBox != null) boxSize += IsoReaderWriter.CalculateSize(RectRegionBox); // RectRegionBox
                if (this.BufferingBox != null) boxSize += IsoReaderWriter.CalculateSize(BufferingBox); // BufferingBox
                if (this.TierDependencyBox != null) boxSize += IsoReaderWriter.CalculateSize(TierDependencyBox); // TierDependencyBox
                if (this.InitialParameterSetBox != null) boxSize += IsoReaderWriter.CalculateSize(InitialParameterSetBox); // InitialParameterSetBox
                if (this.IroiInfoBox != null) boxSize += IsoReaderWriter.CalculateSize(IroiInfoBox); // IroiInfoBox
                if (this.ProtectionSchemeInfoBox != null) boxSize += IsoReaderWriter.CalculateSize(ProtectionSchemeInfoBox); // ProtectionSchemeInfoBox
                if (this.TranscodingInfoBox != null) boxSize += IsoReaderWriter.CalculateSize(TranscodingInfoBox); // TranscodingInfoBox
            }
            return boxSize;
        }
    }


    public class ScalableNALUMapEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "scnm"; } }
        public byte reserved = 0;
        public byte NALU_count;
        public byte groupID;

        public ScalableNALUMapEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.reserved);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.NALU_count);

            for (int i = 1; i <= NALU_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.groupID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.reserved);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.NALU_count);

            for (int i = 1; i <= NALU_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.groupID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // reserved
            boxSize += 8; // NALU_count

            for (int i = 1; i <= NALU_count; i++)
            {
                boxSize += 8; // groupID
            }
            return boxSize;
        }
    }


    public class VvcSubpicIDEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "spid"; } }
        public bool rect_region_flag;
        public byte reserved = 0;
        public bool continuous_id_flag;
        public ushort num_subpics_minus1;
        public ushort[] subpic_id;
        public ushort[] groupID;

        public VvcSubpicIDEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.rect_region_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.continuous_id_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 12, out this.num_subpics_minus1);

            for (int i = 0; i <= num_subpics_minus1; i++)
            {

                if ((continuous_id_flag && i == 0) || !continuous_id_flag)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subpic_id[i]);
                }

                if (rect_region_flag)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.groupID[i]);
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.rect_region_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.continuous_id_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 12, this.num_subpics_minus1);

            for (int i = 0; i <= num_subpics_minus1; i++)
            {

                if ((continuous_id_flag && i == 0) || !continuous_id_flag)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.subpic_id[i]);
                }

                if (rect_region_flag)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.groupID[i]);
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // rect_region_flag
            boxSize += 2; // reserved
            boxSize += 1; // continuous_id_flag
            boxSize += 12; // num_subpics_minus1

            for (int i = 0; i <= num_subpics_minus1; i++)
            {

                if ((continuous_id_flag && i == 0) || !continuous_id_flag)
                {
                    boxSize += 16; // subpic_id
                }

                if (rect_region_flag)
                {
                    boxSize += 16; // groupID
                }
            }
            return boxSize;
        }
    }


    public class SubpicLevelInfoEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "spli"; } }
        public byte level_idc;

        public SubpicLevelInfoEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.level_idc);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.level_idc);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // level_idc
            return boxSize;
        }
    }


    public class VvcSubpicOrderEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "spor"; } }
        public bool subpic_id_info_flag;
        public ushort num_subpic_ref_idx;
        public ushort[] subp_track_ref_idx;
        public VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info;

        public VvcSubpicOrderEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.subpic_id_info_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 15, out this.num_subpic_ref_idx);

            for (int i = 0; i < num_subpic_ref_idx; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subp_track_ref_idx[i]);
            }

            if (subpic_id_info_flag)
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.subpic_id_rewriting_info);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.subpic_id_info_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 15, this.num_subpic_ref_idx);

            for (int i = 0; i < num_subpic_ref_idx; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.subp_track_ref_idx[i]);
            }

            if (subpic_id_info_flag)
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.subpic_id_rewriting_info);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // subpic_id_info_flag
            boxSize += 15; // num_subpic_ref_idx

            for (int i = 0; i < num_subpic_ref_idx; i++)
            {
                boxSize += 16; // subp_track_ref_idx
            }

            if (subpic_id_info_flag)
            {
                boxSize += IsoReaderWriter.CalculateClassSize(subpic_id_rewriting_info); // subpic_id_rewriting_info
            }
            return boxSize;
        }
    }


    public class StepwiseTemporalLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "stsa"; } }

        public StepwiseTemporalLayerEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class VvcSubpicLayoutMapEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "sulm"; } }
        public uint groupID_info_4cc;
        public ushort entry_count_minus1;
        public ushort groupID;

        public VvcSubpicLayoutMapEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.groupID_info_4cc);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.entry_count_minus1);

            for (int i = 0; i <= entry_count_minus1; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.groupID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.groupID_info_4cc);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.entry_count_minus1);

            for (int i = 0; i <= entry_count_minus1; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.groupID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // groupID_info_4cc
            boxSize += 16; // entry_count_minus1

            for (int i = 0; i <= entry_count_minus1; i++)
            {
                boxSize += 16; // groupID
            }
            return boxSize;
        }
    }


    public class SyncSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "sync"; } }
        public byte reserved = 0;
        public byte NAL_unit_type;

        public SyncSampleEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.NAL_unit_type);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.NAL_unit_type);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 2; // reserved
            boxSize += 6; // NAL_unit_type
            return boxSize;
        }
    }


    public class RectangularRegionGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "trif"; } }
        public ushort groupID;
        public bool rect_region_flag;
        public byte reserved = 0;
        public byte independent_idc;
        public bool full_picture;
        public bool filtering_disabled;
        public bool has_dependency_list;
        public byte reserved0 = 0;
        public ushort horizontal_offset;
        public ushort vertical_offset;
        public ushort region_width;
        public ushort region_height;
        public ushort dependency_rect_region_count;
        public ushort dependencyRectRegionGroupID;

        public RectangularRegionGroupEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.groupID);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.rect_region_flag);

            if (!rect_region_flag)
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            }

            else
            {
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.independent_idc);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.full_picture);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.filtering_disabled);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.has_dependency_list);
                boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.reserved0);

                if (!full_picture)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.horizontal_offset);
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.vertical_offset);
                }
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.region_width);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.region_height);

                if (has_dependency_list)
                {
                    boxSize += IsoReaderWriter.ReadUInt16(stream, out this.dependency_rect_region_count);

                    for (int i = 1; i <= dependency_rect_region_count; i++)
                    {
                        boxSize += IsoReaderWriter.ReadUInt16(stream, out this.dependencyRectRegionGroupID);
                    }
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.groupID);
            boxSize += IsoReaderWriter.WriteBit(stream, this.rect_region_flag);

            if (!rect_region_flag)
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            }

            else
            {
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.independent_idc);
                boxSize += IsoReaderWriter.WriteBit(stream, this.full_picture);
                boxSize += IsoReaderWriter.WriteBit(stream, this.filtering_disabled);
                boxSize += IsoReaderWriter.WriteBit(stream, this.has_dependency_list);
                boxSize += IsoReaderWriter.WriteBits(stream, 2, this.reserved0);

                if (!full_picture)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.horizontal_offset);
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.vertical_offset);
                }
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.region_width);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.region_height);

                if (has_dependency_list)
                {
                    boxSize += IsoReaderWriter.WriteUInt16(stream, this.dependency_rect_region_count);

                    for (int i = 1; i <= dependency_rect_region_count; i++)
                    {
                        boxSize += IsoReaderWriter.WriteUInt16(stream, this.dependencyRectRegionGroupID);
                    }
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // groupID
            boxSize += 1; // rect_region_flag

            if (!rect_region_flag)
            {
                boxSize += 7; // reserved
            }

            else
            {
                boxSize += 2; // independent_idc
                boxSize += 1; // full_picture
                boxSize += 1; // filtering_disabled
                boxSize += 1; // has_dependency_list
                boxSize += 2; // reserved0

                if (!full_picture)
                {
                    boxSize += 16; // horizontal_offset
                    boxSize += 16; // vertical_offset
                }
                boxSize += 16; // region_width
                boxSize += 16; // region_height

                if (has_dependency_list)
                {
                    boxSize += 16; // dependency_rect_region_count

                    for (int i = 1; i <= dependency_rect_region_count; i++)
                    {
                        boxSize += 16; // dependencyRectRegionGroupID
                    }
                }
            }
            return boxSize;
        }
    }


    public class TemporalSubLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "tsas"; } }

        public TemporalSubLayerEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            return boxSize;
        }
    }


    public class TemporalLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "tscl"; } }
        public byte temporalLayerId;
        public byte tlprofile_space;
        public bool tltier_flag;
        public byte tlprofile_idc;
        public uint tlprofile_compatibility_flags;
        public ulong tlconstraint_indicator_flags;
        public byte tllevel_idc;
        public ushort tlMaxBitRate;
        public ushort tlAvgBitRate;
        public byte tlConstantFrameRate;
        public ushort tlAvgFrameRate;

        public TemporalLayerEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.temporalLayerId);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.tlprofile_space);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.tltier_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 5, out this.tlprofile_idc);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.tlprofile_compatibility_flags);
            boxSize += IsoReaderWriter.ReadUInt48(stream, out this.tlconstraint_indicator_flags);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.tllevel_idc);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tlMaxBitRate);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tlAvgBitRate);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.tlConstantFrameRate);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.tlAvgFrameRate);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.temporalLayerId);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.tlprofile_space);
            boxSize += IsoReaderWriter.WriteBit(stream, this.tltier_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 5, this.tlprofile_idc);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.tlprofile_compatibility_flags);
            boxSize += IsoReaderWriter.WriteUInt48(stream, this.tlconstraint_indicator_flags);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.tllevel_idc);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.tlMaxBitRate);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.tlAvgBitRate);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.tlConstantFrameRate);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.tlAvgFrameRate);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // temporalLayerId
            boxSize += 2; // tlprofile_space
            boxSize += 1; // tltier_flag
            boxSize += 5; // tlprofile_idc
            boxSize += 32; // tlprofile_compatibility_flags
            boxSize += 48; // tlconstraint_indicator_flags
            boxSize += 8; // tllevel_idc
            boxSize += 16; // tlMaxBitRate
            boxSize += 16; // tlAvgBitRate
            boxSize += 8; // tlConstantFrameRate
            boxSize += 16; // tlAvgFrameRate
            return boxSize;
        }
    }


    public class ViewPriorityEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "vipr"; } }
        public ViewPriorityBox ViewPriorityBox;

        public ViewPriorityEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBox(stream, out this.ViewPriorityBox);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBox(stream, this.ViewPriorityBox);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateSize(ViewPriorityBox); // ViewPriorityBox
            return boxSize;
        }
    }


    public class VvcOperatingPointsInformation : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "vopi"; } }
        public VvcOperatingPointsRecord oinf;

        public VvcOperatingPointsInformation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.oinf);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.oinf);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(oinf); // oinf
            return boxSize;
        }
    }


    public class TrackGroupTypeBox1 : FullBox
    {
        public override string FourCC { get { return "alte"; } }
        public uint track_group_id;  //  the remaining data may be specified 

        public TrackGroupTypeBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_group_id
            /*   for a particular track_group_type */
            return boxSize;
        }
    }


    public class TrackGroupTypeBox2 : FullBox
    {
        public override string FourCC { get { return "cstg"; } }
        public uint track_group_id;  //  the remaining data may be specified 

        public TrackGroupTypeBox2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_group_id
            /*   for a particular track_group_type */
            return boxSize;
        }
    }


    public class TrackGroupTypeBox3 : FullBox
    {
        public override string FourCC { get { return "snut"; } }
        public uint track_group_id;  //  the remaining data may be specified 

        public TrackGroupTypeBox3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.track_group_id); // the remaining data may be specified 
            /*   for a particular track_group_type */
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_group_id
            /*   for a particular track_group_type */
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox11 : Box
    {
        public override string FourCC { get { return "avcp"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox11()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox12 : Box
    {
        public override string FourCC { get { return "deps"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox12()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox13 : Box
    {
        public override string FourCC { get { return "evcr"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox13()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox14 : Box
    {
        public override string FourCC { get { return "mixn"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox14()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox15 : Box
    {
        public override string FourCC { get { return "oref"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox15()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox16 : Box
    {
        public override string FourCC { get { return "recr"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox16()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox17 : Box
    {
        public override string FourCC { get { return "sabt"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox17()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox18 : Box
    {
        public override string FourCC { get { return "sbas"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox18()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox19 : Box
    {
        public override string FourCC { get { return "scal"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox19()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox20 : Box
    {
        public override string FourCC { get { return "subp"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox20()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox21 : Box
    {
        public override string FourCC { get { return "swfr"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox21()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox22 : Box
    {
        public override string FourCC { get { return "swto"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox22()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox23 : Box
    {
        public override string FourCC { get { return "tbas"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox23()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox24 : Box
    {
        public override string FourCC { get { return "vref"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox24()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox25 : Box
    {
        public override string FourCC { get { return "vreg"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox25()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class TrackReferenceTypeBox26 : Box
    {
        public override string FourCC { get { return "vvcN"; } }
        public uint[] track_IDs;

        public TrackReferenceTypeBox26()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32Array(stream, out this.track_IDs);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // track_IDs
            return boxSize;
        }
    }


    public class EntityToGroupBox2 : FullBox
    {
        public override string FourCC { get { return "eqiv"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox3 : FullBox
    {
        public override string FourCC { get { return "ster"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox4 : FullBox
    {
        public override string FourCC { get { return "aebr"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox5 : FullBox
    {
        public override string FourCC { get { return "afbr"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox6 : FullBox
    {
        public override string FourCC { get { return "albc"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox6()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox7 : FullBox
    {
        public override string FourCC { get { return "brst"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox7()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox8 : FullBox
    {
        public override string FourCC { get { return "iaug"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox8()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox9 : FullBox
    {
        public override string FourCC { get { return "tsyn"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox9()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox10 : FullBox
    {
        public override string FourCC { get { return "dobr"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox10()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox11 : FullBox
    {
        public override string FourCC { get { return "favc"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox11()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox12 : FullBox
    {
        public override string FourCC { get { return "fobr"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox12()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox13 : FullBox
    {
        public override string FourCC { get { return "pano"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox13()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class EntityToGroupBox14 : FullBox
    {
        public override string FourCC { get { return "wbbr"; } }
        public uint group_id;
        public uint num_entities_in_group;
        public uint entity_id;  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox14()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.group_id);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.entity_id); // the remaining data may be specified for a particular grouping_type
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // group_id
            boxSize += 32; // num_entities_in_group

            for (int i = 0; i < num_entities_in_group; i++)
            {
                boxSize += 32; // entity_id
            }
            return boxSize;
        }
    }


    public class AuxiliaryTypeProperty : ItemFullProperty
    {
        public override string FourCC { get { return "auxC"; } }
        public string aux_type;
        public byte[] aux_subtype;  //  until the end of the box, the semantics depend on the aux_type value

        public AuxiliaryTypeProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.aux_type);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.aux_subtype); // until the end of the box, the semantics depend on the aux_type value
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.aux_type);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.aux_subtype); // until the end of the box, the semantics depend on the aux_type value
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)aux_type.Length * 8; // aux_type
            boxSize += (ulong)aux_subtype.Length * 8; // aux_subtype
            return boxSize;
        }
    }


    public class AVCConfigurationBox1 : Box
    {
        public override string FourCC { get { return "avcC"; } }
        public AVCDecoderConfigurationRecord AVCConfig;

        public AVCConfigurationBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.AVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.AVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(AVCConfig); // AVCConfig
            return boxSize;
        }
    }


    public class CleanApertureBox1 : Box
    {
        public override string FourCC { get { return "clap"; } }
        public uint cleanApertureWidthN;
        public uint cleanApertureWidthD;
        public uint cleanApertureHeightN;
        public uint cleanApertureHeightD;
        public uint horizOffN;
        public uint horizOffD;
        public uint vertOffN;
        public uint vertOffD;

        public CleanApertureBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureWidthN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureWidthD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureHeightN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.cleanApertureHeightD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizOffN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizOffD);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertOffN);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertOffD);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizOffN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizOffD);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertOffN);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertOffD);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // cleanApertureWidthN
            boxSize += 32; // cleanApertureWidthD
            boxSize += 32; // cleanApertureHeightN
            boxSize += 32; // cleanApertureHeightD
            boxSize += 32; // horizOffN
            boxSize += 32; // horizOffD
            boxSize += 32; // vertOffN
            boxSize += 32; // vertOffD
            return boxSize;
        }
    }


    public class ColourInformationBox1 : Box
    {
        public override string FourCC { get { return "colr"; } }
        public uint colour_type;
        public ushort colour_primaries;
        public ushort transfer_characteristics;
        public ushort matrix_coefficients;
        public bool full_range_flag;
        public byte reserved = 0;
        public ICC_profile ICC_profile;  //  restricted ICC profile
        public ICC_profile ICC_profile0;  //  unrestricted ICC profile

        public ColourInformationBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.colour_type);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.colour_primaries);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.transfer_characteristics);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.matrix_coefficients);
                boxSize += IsoReaderWriter.ReadBit(stream, out this.full_range_flag);
                boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.ICC_profile); // restricted ICC profile
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                boxSize += IsoReaderWriter.ReadClass(stream, out this.ICC_profile0); // unrestricted ICC profile
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.colour_type);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.colour_primaries);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.transfer_characteristics);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.matrix_coefficients);
                boxSize += IsoReaderWriter.WriteBit(stream, this.full_range_flag);
                boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.ICC_profile); // restricted ICC profile
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                boxSize += IsoReaderWriter.WriteClass(stream, this.ICC_profile0); // unrestricted ICC profile
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // colour_type

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                boxSize += 16; // colour_primaries
                boxSize += 16; // transfer_characteristics
                boxSize += 16; // matrix_coefficients
                boxSize += 1; // full_range_flag
                boxSize += 7; // reserved
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                boxSize += IsoReaderWriter.CalculateClassSize(ICC_profile); // ICC_profile
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                boxSize += IsoReaderWriter.CalculateClassSize(ICC_profile0); // ICC_profile0
            }
            return boxSize;
        }
    }


    public class HEVCConfigurationBox1 : Box
    {
        public override string FourCC { get { return "hvcC"; } }
        public HEVCDecoderConfigurationRecord HEVCConfig;

        public HEVCConfigurationBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.HEVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.HEVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(HEVCConfig); // HEVCConfig
            return boxSize;
        }
    }


    public class ImageMirror : ItemProperty
    {
        public override string FourCC { get { return "imir"; } }
        public byte reserved = 0;
        public bool axis;

        public ImageMirror()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.reserved);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.axis);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            boxSize += IsoReaderWriter.WriteBit(stream, this.axis);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 7; // reserved
            boxSize += 1; // axis
            return boxSize;
        }
    }


    public class ImageRotation : ItemProperty
    {
        public override string FourCC { get { return "irot"; } }
        public byte reserved = 0;
        public byte angle;

        public ImageRotation()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBits(stream, 6, out this.reserved);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.angle);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.angle);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 6; // reserved
            boxSize += 2; // angle
            return boxSize;
        }
    }


    public class ImageSpatialExtentsProperty : ItemFullProperty
    {
        public override string FourCC { get { return "ispe"; } }
        public uint image_width;
        public uint image_height;

        public ImageSpatialExtentsProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.image_width);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.image_height);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.image_width);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.image_height);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // image_width
            boxSize += 32; // image_height
            return boxSize;
        }
    }


    public class JPEGConfigurationBox : Box
    {
        public override string FourCC { get { return "jpgC"; } }
        public byte[] JPEGprefix;

        public JPEGConfigurationBox()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8Array(stream, out this.JPEGprefix);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8Array(stream, this.JPEGprefix);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)JPEGprefix.Length * 8; // JPEGprefix
            return boxSize;
        }
    }


    public class LHEVCConfigurationBox1 : Box
    {
        public override string FourCC { get { return "lhvC"; } }
        public LHEVCDecoderConfigurationRecord LHEVCConfig;

        public LHEVCConfigurationBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.LHEVCConfig);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.LHEVCConfig);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(LHEVCConfig); // LHEVCConfig
            return boxSize;
        }
    }


    public class LayerSelectorProperty : ItemProperty
    {
        public override string FourCC { get { return "lsel"; } }
        public ushort layer_id;

        public LayerSelectorProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.layer_id);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.layer_id);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // layer_id
            return boxSize;
        }
    }


    public class OperatingPointsInformationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "oinf"; } }
        public OperatingPointsRecord op_info;  //  specified in ISO/IEC 14496-15

        public OperatingPointsInformationProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.op_info); // specified in ISO/IEC 14496-15
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.op_info); // specified in ISO/IEC 14496-15
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(op_info); // op_info
            return boxSize;
        }
    }


    public class PixelAspectRatioBox1 : Box
    {
        public override string FourCC { get { return "pasp"; } }
        public uint hSpacing;
        public uint vSpacing;

        public PixelAspectRatioBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.hSpacing);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vSpacing);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.hSpacing);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vSpacing);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // hSpacing
            boxSize += 32; // vSpacing
            return boxSize;
        }
    }


    public class PixelInformationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "pixi"; } }
        public byte num_channels;
        public byte bits_per_channel;

        public PixelInformationProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.num_channels);

            for (int i = 0; i < num_channels; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt8(stream, out this.bits_per_channel);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.num_channels);

            for (int i = 0; i < num_channels; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt8(stream, this.bits_per_channel);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // num_channels

            for (int i = 0; i < num_channels; i++)
            {
                boxSize += 8; // bits_per_channel
            }
            return boxSize;
        }
    }


    public class RelativeLocationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "rloc"; } }
        public uint horizontal_offset;
        public uint vertical_offset;

        public RelativeLocationProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.horizontal_offset);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.vertical_offset);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.horizontal_offset);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.vertical_offset);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // horizontal_offset
            boxSize += 32; // vertical_offset
            return boxSize;
        }
    }


    public class SubSampleInformationBox1 : FullBox
    {
        public override string FourCC { get { return "subs"; } }
        public uint entry_count;
        public uint sample_delta;
        public ushort subsample_count;
        public uint subsample_size;
        public ushort subsample_size0;
        public byte subsample_priority;
        public byte discardable;
        public uint codec_specific_parameters;

        public SubSampleInformationBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_delta);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subsample_count);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.subsample_size);
                        }

                        else
                        {
                            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.subsample_size0);
                        }
                        boxSize += IsoReaderWriter.ReadUInt8(stream, out this.subsample_priority);
                        boxSize += IsoReaderWriter.ReadUInt8(stream, out this.discardable);
                        boxSize += IsoReaderWriter.ReadUInt32(stream, out this.codec_specific_parameters);
                    }
                }
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_delta);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.subsample_count);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            boxSize += IsoReaderWriter.WriteUInt32(stream, this.subsample_size);
                        }

                        else
                        {
                            boxSize += IsoReaderWriter.WriteUInt16(stream, this.subsample_size0);
                        }
                        boxSize += IsoReaderWriter.WriteUInt8(stream, this.subsample_priority);
                        boxSize += IsoReaderWriter.WriteUInt8(stream, this.discardable);
                        boxSize += IsoReaderWriter.WriteUInt32(stream, this.codec_specific_parameters);
                    }
                }
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // entry_count


            for (int i = 0; i < entry_count; i++)
            {
                boxSize += 32; // sample_delta
                boxSize += 16; // subsample_count

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            boxSize += 32; // subsample_size
                        }

                        else
                        {
                            boxSize += 16; // subsample_size0
                        }
                        boxSize += 8; // subsample_priority
                        boxSize += 8; // discardable
                        boxSize += 32; // codec_specific_parameters
                    }
                }
            }
            return boxSize;
        }
    }


    public class TargetOlsProperty : ItemFullProperty
    {
        public override string FourCC { get { return "tols"; } }
        public ushort target_ols_idx;

        public TargetOlsProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_ols_idx);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_ols_idx);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // target_ols_idx
            return boxSize;
        }
    }


    public class AutoExposureBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "aebr"; } }
        public sbyte exposure_step;
        public sbyte exposure_numerator;

        public AutoExposureBracketingEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.exposure_step);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.exposure_numerator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.exposure_step);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.exposure_numerator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // exposure_step
            boxSize += 8; // exposure_numerator
            return boxSize;
        }
    }


    public class FlashExposureBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "afbr"; } }
        public sbyte flash_exposure_numerator;
        public sbyte flash_exposure_denominator;

        public FlashExposureBracketingEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.flash_exposure_numerator);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.flash_exposure_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_numerator);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // flash_exposure_numerator
            boxSize += 8; // flash_exposure_denominator
            return boxSize;
        }
    }


    public class AccessibilityTextProperty : ItemFullProperty
    {
        public override string FourCC { get { return "altt"; } }
        public string alt_text;
        public string alt_lang;

        public AccessibilityTextProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.alt_text);
            boxSize += IsoReaderWriter.ReadString(stream, out this.alt_lang);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.alt_text);
            boxSize += IsoReaderWriter.WriteString(stream, this.alt_lang);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)alt_text.Length * 8; // alt_text
            boxSize += (ulong)alt_lang.Length * 8; // alt_lang
            return boxSize;
        }
    }


    public class CreationTimeProperty : ItemFullProperty
    {
        public override string FourCC { get { return "crtt"; } }
        public ulong creation_time;

        public CreationTimeProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.creation_time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // creation_time
            return boxSize;
        }
    }


    public class DepthOfFieldBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dobr"; } }
        public sbyte f_stop_numerator;
        public sbyte f_stop_denominator;

        public DepthOfFieldBracketingEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.f_stop_numerator);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.f_stop_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.f_stop_numerator);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.f_stop_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // f_stop_numerator
            boxSize += 8; // f_stop_denominator
            return boxSize;
        }
    }


    public class FocusBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "fobr"; } }
        public ushort focus_distance_numerator;
        public ushort focus_distance_denominator;

        public FocusBracketingEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.focus_distance_numerator);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.focus_distance_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_numerator);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // focus_distance_numerator
            boxSize += 16; // focus_distance_denominator
            return boxSize;
        }
    }


    public class ImageScaling : ItemFullProperty
    {
        public override string FourCC { get { return "iscl"; } }
        public ushort target_width_numerator;
        public ushort target_width_denominator;
        public ushort target_height_numerator;
        public ushort target_height_denominator;

        public ImageScaling()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_width_numerator);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_width_denominator);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_height_numerator);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.target_height_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_width_numerator);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_width_denominator);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_height_numerator);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.target_height_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // target_width_numerator
            boxSize += 16; // target_width_denominator
            boxSize += 16; // target_height_numerator
            boxSize += 16; // target_height_denominator
            return boxSize;
        }
    }


    public class ModificationTimeProperty : ItemFullProperty
    {
        public override string FourCC { get { return "mdft"; } }
        public ulong modification_time;

        public ModificationTimeProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt64(stream, out this.modification_time);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 64; // modification_time
            return boxSize;
        }
    }


    public class PanoramaEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pano"; } }
        public ushort frame_number;

        public PanoramaEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.frame_number);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.frame_number);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // frame_number
            return boxSize;
        }
    }


    public class RequiredReferenceTypesProperty : ItemFullProperty
    {
        public override string FourCC { get { return "rref"; } }
        public byte reference_type_count;
        public uint[] reference_type;

        public RequiredReferenceTypesProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.reference_type_count);

            for (int i = 0; i < reference_type_count; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.reference_type[i]);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.reference_type_count);

            for (int i = 0; i < reference_type_count; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.reference_type[i]);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // reference_type_count

            for (int i = 0; i < reference_type_count; i++)
            {
                boxSize += 32; // reference_type
            }
            return boxSize;
        }
    }


    public class UserDescriptionProperty : ItemFullProperty
    {
        public override string FourCC { get { return "udes"; } }
        public string lang;
        public string name;
        public string description;
        public string tags;

        public UserDescriptionProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadString(stream, out this.lang);
            boxSize += IsoReaderWriter.ReadString(stream, out this.name);
            boxSize += IsoReaderWriter.ReadString(stream, out this.description);
            boxSize += IsoReaderWriter.ReadString(stream, out this.tags);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteString(stream, this.lang);
            boxSize += IsoReaderWriter.WriteString(stream, this.name);
            boxSize += IsoReaderWriter.WriteString(stream, this.description);
            boxSize += IsoReaderWriter.WriteString(stream, this.tags);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += (ulong)lang.Length * 8; // lang
            boxSize += (ulong)name.Length * 8; // name
            boxSize += (ulong)description.Length * 8; // description
            boxSize += (ulong)tags.Length * 8; // tags
            return boxSize;
        }
    }


    public class WhiteBalanceBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "wbbr"; } }
        public ushort blue_amber;
        public sbyte green_magenta;

        public WhiteBalanceBracketingEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.blue_amber);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.green_magenta);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.blue_amber);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.green_magenta);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // blue_amber
            boxSize += 8; // green_magenta
            return boxSize;
        }
    }


    public class ContentColourVolumeBox1 : Box
    {
        public override string FourCC { get { return "cclv"; } }
        public bool reserved1 = false;  //  ccv_cancel_flag
        public bool reserved2 = false;  //  ccv_persistence_flag
        public bool ccv_primaries_present_flag;
        public bool ccv_min_luminance_value_present_flag;
        public bool ccv_max_luminance_value_present_flag;
        public bool ccv_avg_luminance_value_present_flag;
        public byte ccv_reserved_zero_2bits = 0;
        public int[] ccv_primaries_x;
        public int[] ccv_primaries_y;
        public uint ccv_min_luminance_value;
        public uint ccv_max_luminance_value;
        public uint ccv_avg_luminance_value;

        public ContentColourVolumeBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved1); // ccv_cancel_flag
            boxSize += IsoReaderWriter.ReadBit(stream, out this.reserved2); // ccv_persistence_flag
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_primaries_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_min_luminance_value_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_max_luminance_value_present_flag);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.ccv_avg_luminance_value_present_flag);
            boxSize += IsoReaderWriter.ReadBits(stream, 2, out this.ccv_reserved_zero_2bits);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.ccv_primaries_x[c]);
                    boxSize += IsoReaderWriter.ReadInt32(stream, out this.ccv_primaries_y[c]);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ccv_min_luminance_value);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ccv_max_luminance_value);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.ccv_avg_luminance_value);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.reserved1); // ccv_cancel_flag
            boxSize += IsoReaderWriter.WriteBit(stream, this.reserved2); // ccv_persistence_flag
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_primaries_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_min_luminance_value_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_max_luminance_value_present_flag);
            boxSize += IsoReaderWriter.WriteBit(stream, this.ccv_avg_luminance_value_present_flag);
            boxSize += IsoReaderWriter.WriteBits(stream, 2, this.ccv_reserved_zero_2bits);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_x[c]);
                    boxSize += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_y[c]);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.ccv_min_luminance_value);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.ccv_max_luminance_value);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.ccv_avg_luminance_value);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // reserved1
            boxSize += 1; // reserved2
            boxSize += 1; // ccv_primaries_present_flag
            boxSize += 1; // ccv_min_luminance_value_present_flag
            boxSize += 1; // ccv_max_luminance_value_present_flag
            boxSize += 1; // ccv_avg_luminance_value_present_flag
            boxSize += 2; // ccv_reserved_zero_2bits

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    boxSize += 32; // ccv_primaries_x
                    boxSize += 32; // ccv_primaries_y
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                boxSize += 32; // ccv_min_luminance_value
            }

            if (ccv_max_luminance_value_present_flag)
            {
                boxSize += 32; // ccv_max_luminance_value
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                boxSize += 32; // ccv_avg_luminance_value
            }
            return boxSize;
        }
    }


    public class MasteringDisplayColourVolumeBox1 : Box
    {
        public override string FourCC { get { return "mdcv"; } }
        public ushort display_primaries_x;
        public ushort display_primaries_y;
        public ushort white_point_x;
        public ushort white_point_y;
        public uint max_display_mastering_luminance;
        public uint min_display_mastering_luminance;

        public MasteringDisplayColourVolumeBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.display_primaries_x);
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.display_primaries_y);
            }
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.white_point_x);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.white_point_y);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.max_display_mastering_luminance);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.min_display_mastering_luminance);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_x);
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_y);
            }
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.white_point_x);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.white_point_y);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.max_display_mastering_luminance);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.min_display_mastering_luminance);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();

            for (int c = 0; c < 3; c++)
            {
                boxSize += 16; // display_primaries_x
                boxSize += 16; // display_primaries_y
            }
            boxSize += 16; // white_point_x
            boxSize += 16; // white_point_y
            boxSize += 32; // max_display_mastering_luminance
            boxSize += 32; // min_display_mastering_luminance
            return boxSize;
        }
    }


    public class ContentLightLevelBox1 : Box
    {
        public override string FourCC { get { return "clli"; } }
        public ushort max_content_light_level;
        public ushort max_pic_average_light_level;

        public ContentLightLevelBox1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_content_light_level);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.max_pic_average_light_level);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_content_light_level);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.max_pic_average_light_level);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // max_content_light_level
            boxSize += 16; // max_pic_average_light_level
            return boxSize;
        }
    }


    public class WipeTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "wipe"; } }
        public byte transition_direction;

        public WipeTransitionEffectProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.transition_direction);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.transition_direction);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // transition_direction
            return boxSize;
        }
    }


    public class ZoomTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "zoom"; } }
        public bool transition_direction;
        public byte transition_shape;

        public ZoomTransitionEffectProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadBit(stream, out this.transition_direction);
            boxSize += IsoReaderWriter.ReadBits(stream, 7, out this.transition_shape);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteBit(stream, this.transition_direction);
            boxSize += IsoReaderWriter.WriteBits(stream, 7, this.transition_shape);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 1; // transition_direction
            boxSize += 7; // transition_shape
            return boxSize;
        }
    }


    public class FadeTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "fade"; } }
        public byte transition_direction;

        public FadeTransitionEffectProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.transition_direction);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.transition_direction);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // transition_direction
            return boxSize;
        }
    }


    public class SplitTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "splt"; } }
        public byte transition_direction;

        public SplitTransitionEffectProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.transition_direction);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.transition_direction);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // transition_direction
            return boxSize;
        }
    }


    public class SuggestedTransitionPeriodProperty : ItemFullProperty
    {
        public override string FourCC { get { return "stpe"; } }
        public byte transition_period;

        public SuggestedTransitionPeriodProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.transition_period);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.transition_period);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // transition_period
            return boxSize;
        }
    }


    public class SuggestedTimeDisplayDurationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "ssld"; } }
        public ushort duration;

        public SuggestedTimeDisplayDurationProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.duration);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.duration);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // duration
            return boxSize;
        }
    }


    public class MaskConfigurationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "mskC"; } }
        public byte bits_per_pixel;

        public MaskConfigurationProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.bits_per_pixel);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.bits_per_pixel);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // bits_per_pixel
            return boxSize;
        }
    }


    public class VvcSubpicIDProperty : ItemFullProperty
    {
        public override string FourCC { get { return "spid"; } }
        public VvcSubpicIDEntry sid_info;  //  specified in ISO/IEC 14496-15

        public VvcSubpicIDProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.sid_info); // specified in ISO/IEC 14496-15
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.sid_info); // specified in ISO/IEC 14496-15
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(sid_info); // sid_info
            return boxSize;
        }
    }


    public class VvcSubpicOrderProperty : ItemFullProperty
    {
        public override string FourCC { get { return "spor"; } }
        public VvcSubpicOrderEntry sor_info;  //  specified in ISO/IEC 14496-15

        public VvcSubpicOrderProperty()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadClass(stream, out this.sor_info); // specified in ISO/IEC 14496-15
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteClass(stream, this.sor_info); // specified in ISO/IEC 14496-15
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += IsoReaderWriter.CalculateClassSize(sor_info); // sor_info
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox2 : Box
    {
        public override string FourCC { get { return "auxl"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge2 : Box
    {
        public override string FourCC { get { return "auxl"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge2()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox3 : Box
    {
        public override string FourCC { get { return "base"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge3 : Box
    {
        public override string FourCC { get { return "base"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge3()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox4 : Box
    {
        public override string FourCC { get { return "dimg"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge4 : Box
    {
        public override string FourCC { get { return "dimg"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge4()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox5 : Box
    {
        public override string FourCC { get { return "dpnd"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge5 : Box
    {
        public override string FourCC { get { return "dpnd"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge5()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox6 : Box
    {
        public override string FourCC { get { return "exbl"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox6()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge6 : Box
    {
        public override string FourCC { get { return "exbl"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge6()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox7 : Box
    {
        public override string FourCC { get { return "grid"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox7()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge7 : Box
    {
        public override string FourCC { get { return "grid"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge7()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox8 : Box
    {
        public override string FourCC { get { return "thmb"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox8()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge8 : Box
    {
        public override string FourCC { get { return "thmb"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge8()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox9 : Box
    {
        public override string FourCC { get { return "pred"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox9()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge9 : Box
    {
        public override string FourCC { get { return "pred"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge9()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBox10 : Box
    {
        public override string FourCC { get { return "tbas"; } }
        public ushort from_item_ID;
        public ushort reference_count;
        public ushort to_item_ID;

        public SingleItemTypeReferenceBox10()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt16(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 16; // to_item_ID
            }
            return boxSize;
        }
    }


    public class SingleItemTypeReferenceBoxLarge10 : Box
    {
        public override string FourCC { get { return "tbas"; } }
        public uint from_item_ID;
        public ushort reference_count;
        public uint to_item_ID;

        public SingleItemTypeReferenceBoxLarge10()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.from_item_ID);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.to_item_ID);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // from_item_ID
            boxSize += 16; // reference_count

            for (int j = 0; j < reference_count; j++)
            {
                boxSize += 32; // to_item_ID
            }
            return boxSize;
        }
    }


    public class VisualEquivalenceEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "eqiv"; } }
        public short time_offset;
        public ushort timescale_multiplier;

        public VisualEquivalenceEntry()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt16(stream, out this.time_offset);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.timescale_multiplier);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt16(stream, this.time_offset);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.timescale_multiplier);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // time_offset
            boxSize += 16; // timescale_multiplier
            return boxSize;
        }
    }


    public class DirectReferenceSamplesList : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "refs"; } }
        public uint sample_id;
        public byte num_direct_reference_samples;
        public uint direct_reference_sample_id;

        public DirectReferenceSamplesList()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt32(stream, out this.sample_id);
            boxSize += IsoReaderWriter.ReadUInt8(stream, out this.num_direct_reference_samples);

            for (int i = 0; i < num_direct_reference_samples; i++)
            {
                boxSize += IsoReaderWriter.ReadUInt32(stream, out this.direct_reference_sample_id);
            }
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt32(stream, this.sample_id);
            boxSize += IsoReaderWriter.WriteUInt8(stream, this.num_direct_reference_samples);

            for (int i = 0; i < num_direct_reference_samples; i++)
            {
                boxSize += IsoReaderWriter.WriteUInt32(stream, this.direct_reference_sample_id);
            }
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 32; // sample_id
            boxSize += 8; // num_direct_reference_samples

            for (int i = 0; i < num_direct_reference_samples; i++)
            {
                boxSize += 32; // direct_reference_sample_id
            }
            return boxSize;
        }
    }


    public class AutoExposureBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "aebr"; } }
        public sbyte exposure_step;
        public sbyte exposure_numerator;

        public AutoExposureBracketingEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.exposure_step);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.exposure_numerator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.exposure_step);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.exposure_numerator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // exposure_step
            boxSize += 8; // exposure_numerator
            return boxSize;
        }
    }


    public class FlashExposureBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "afbr"; } }
        public sbyte flash_exposure_numerator;
        public sbyte flash_exposure_denominator;

        public FlashExposureBracketingEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.flash_exposure_numerator);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.flash_exposure_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_numerator);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // flash_exposure_numerator
            boxSize += 8; // flash_exposure_denominator
            return boxSize;
        }
    }


    public class DepthOfFieldBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dobr"; } }
        public sbyte f_stop_numerator;
        public sbyte f_stop_denominator;

        public DepthOfFieldBracketingEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.f_stop_numerator);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.f_stop_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.f_stop_numerator);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.f_stop_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 8; // f_stop_numerator
            boxSize += 8; // f_stop_denominator
            return boxSize;
        }
    }


    public class FocusBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "fobr"; } }
        public ushort focus_distance_numerator;
        public ushort focus_distance_denominator;

        public FocusBracketingEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.focus_distance_numerator);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.focus_distance_denominator);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_numerator);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_denominator);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // focus_distance_numerator
            boxSize += 16; // focus_distance_denominator
            return boxSize;
        }
    }


    public class PanoramaEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pano"; } }
        public ushort frame_number;

        public PanoramaEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.frame_number);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.frame_number);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // frame_number
            return boxSize;
        }
    }


    public class WhiteBalanceBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "wbbr"; } }
        public ushort blue_amber;
        public sbyte green_magenta;

        public WhiteBalanceBracketingEntry1()
        { }

        public async override Task<ulong> ReadAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.ReadAsync(stream);
            boxSize += IsoReaderWriter.ReadUInt16(stream, out this.blue_amber);
            boxSize += IsoReaderWriter.ReadInt8(stream, out this.green_magenta);
            return boxSize;
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong boxSize = 0;
            boxSize += await base.WriteAsync(stream);
            boxSize += IsoReaderWriter.WriteUInt16(stream, this.blue_amber);
            boxSize += IsoReaderWriter.WriteInt8(stream, this.green_magenta);
            return boxSize;
        }

        public override ulong CalculateSize()
        {
            ulong boxSize = 0;
            boxSize += base.CalculateSize();
            boxSize += 16; // blue_amber
            boxSize += 8; // green_magenta
            return boxSize;
        }
    }



}
