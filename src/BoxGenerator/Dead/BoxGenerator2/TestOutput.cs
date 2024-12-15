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
        public uint SSRC { get; set; }

        public ReceivedSsrcBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.SSRC = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.SSRC);
            return size;
        }
    }


    public class timestampsynchrony : Box
    {
        public override string FourCC { get { return "tssy"; } }
        public byte reserved { get; set; }
        public byte timestamp_sync { get; set; }

        public timestampsynchrony()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.timestamp_sync = IsoReaderWriter.ReadBits(stream, 2);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 2, this.timestamp_sync);
            return size;
        }
    }


    public class timescaleentry : Box
    {
        public override string FourCC { get { return "tims"; } }
        public uint timescale { get; set; }

        public timescaleentry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.timescale = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
            return size;
        }
    }


    public class timeoffset : Box
    {
        public override string FourCC { get { return "tims"; } }
        public int offset { get; set; }

        public timeoffset()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.offset);
            return size;
        }
    }


    public class sequenceoffset : Box
    {
        public override string FourCC { get { return "tims"; } }
        public int offset { get; set; }

        public sequenceoffset()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.offset);
            return size;
        }
    }


    public class timescaleentry1 : Box
    {
        public override string FourCC { get { return "tsro"; } }
        public uint timescale { get; set; }

        public timescaleentry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.timescale = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
            return size;
        }
    }


    public class timeoffset1 : Box
    {
        public override string FourCC { get { return "tsro"; } }
        public int offset { get; set; }

        public timeoffset1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.offset);
            return size;
        }
    }


    public class sequenceoffset1 : Box
    {
        public override string FourCC { get { return "tsro"; } }
        public int offset { get; set; }

        public sequenceoffset1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.offset);
            return size;
        }
    }


    public class timescaleentry2 : Box
    {
        public override string FourCC { get { return "snro"; } }
        public uint timescale { get; set; }

        public timescaleentry2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.timescale = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
            return size;
        }
    }


    public class timeoffset2 : Box
    {
        public override string FourCC { get { return "snro"; } }
        public int offset { get; set; }

        public timeoffset2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.offset);
            return size;
        }
    }


    public class sequenceoffset2 : Box
    {
        public override string FourCC { get { return "snro"; } }
        public int offset { get; set; }

        public sequenceoffset2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.offset);
            return size;
        }
    }


    public class hintBytesSent : Box
    {
        public override string FourCC { get { return "trpy"; } }
        public ulong bytessent { get; set; }

        public hintBytesSent()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintPacketsSent : Box
    {
        public override string FourCC { get { return "trpy"; } }
        public ulong packetssent { get; set; }

        public hintPacketsSent()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.packetssent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.packetssent);
            return size;
        }
    }


    public class hintBytesSent1 : Box
    {
        public override string FourCC { get { return "trpy"; } }
        public ulong bytessent { get; set; }

        public hintBytesSent1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintBytesSent2 : Box
    {
        public override string FourCC { get { return "nump"; } }
        public ulong bytessent { get; set; }

        public hintBytesSent2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintPacketsSent1 : Box
    {
        public override string FourCC { get { return "nump"; } }
        public ulong packetssent { get; set; }

        public hintPacketsSent1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.packetssent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.packetssent);
            return size;
        }
    }


    public class hintBytesSent3 : Box
    {
        public override string FourCC { get { return "nump"; } }
        public ulong bytessent { get; set; }

        public hintBytesSent3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintBytesSent4 : Box
    {
        public override string FourCC { get { return "tpyl"; } }
        public ulong bytessent { get; set; }

        public hintBytesSent4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintPacketsSent2 : Box
    {
        public override string FourCC { get { return "tpyl"; } }
        public ulong packetssent { get; set; }

        public hintPacketsSent2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.packetssent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.packetssent);
            return size;
        }
    }


    public class hintBytesSent5 : Box
    {
        public override string FourCC { get { return "tpyl"; } }
        public ulong bytessent { get; set; }

        public hintBytesSent5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintBytesSent6 : Box
    {
        public override string FourCC { get { return "totl"; } }
        public uint bytessent { get; set; }

        public hintBytesSent6()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return size;
        }
    }


    public class hintPacketsSent3 : Box
    {
        public override string FourCC { get { return "totl"; } }
        public uint packetssent { get; set; }

        public hintPacketsSent3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.packetssent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.packetssent);
            return size;
        }
    }


    public class hintBytesSent7 : Box
    {
        public override string FourCC { get { return "totl"; } }
        public uint bytessent { get; set; }

        public hintBytesSent7()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return size;
        }
    }


    public class hintBytesSent8 : Box
    {
        public override string FourCC { get { return "npck"; } }
        public uint bytessent { get; set; }

        public hintBytesSent8()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return size;
        }
    }


    public class hintPacketsSent4 : Box
    {
        public override string FourCC { get { return "npck"; } }
        public uint packetssent { get; set; }

        public hintPacketsSent4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.packetssent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.packetssent);
            return size;
        }
    }


    public class hintBytesSent9 : Box
    {
        public override string FourCC { get { return "npck"; } }
        public uint bytessent { get; set; }

        public hintBytesSent9()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return size;
        }
    }


    public class hintBytesSent10 : Box
    {
        public override string FourCC { get { return "tpay"; } }
        public uint bytessent { get; set; }

        public hintBytesSent10()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return size;
        }
    }


    public class hintPacketsSent5 : Box
    {
        public override string FourCC { get { return "tpay"; } }
        public uint packetssent { get; set; }

        public hintPacketsSent5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.packetssent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.packetssent);
            return size;
        }
    }


    public class hintBytesSent11 : Box
    {
        public override string FourCC { get { return "tpay"; } }
        public uint bytessent { get; set; }

        public hintBytesSent11()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytessent);
            return size;
        }
    }


    public class hintmaxrate : Box
    {
        public override string FourCC { get { return "maxr"; } }
        public uint period { get; set; }  //  in milliseconds
        public uint bytes { get; set; }

        public hintmaxrate()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            /*  maximum data rate */
            this.period = IsoReaderWriter.ReadUInt32(stream);
            this.bytes = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            /*  maximum data rate */
            size += IsoReaderWriter.WriteUInt32(stream, this.period);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytes);
            return size;
        }
    }


    public class hintmediaBytesSent : Box
    {
        public override string FourCC { get { return "dmed"; } }
        public ulong bytessent { get; set; }

        public hintmediaBytesSent()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintimmediateBytesSent : Box
    {
        public override string FourCC { get { return "dmed"; } }
        public ulong bytessent { get; set; }

        public hintimmediateBytesSent()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintrepeatedBytesSent : Box
    {
        public override string FourCC { get { return "dmed"; } }
        public ulong bytessent { get; set; }

        public hintrepeatedBytesSent()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintmediaBytesSent1 : Box
    {
        public override string FourCC { get { return "dimm"; } }
        public ulong bytessent { get; set; }

        public hintmediaBytesSent1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintimmediateBytesSent1 : Box
    {
        public override string FourCC { get { return "dimm"; } }
        public ulong bytessent { get; set; }

        public hintimmediateBytesSent1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintrepeatedBytesSent1 : Box
    {
        public override string FourCC { get { return "dimm"; } }
        public ulong bytessent { get; set; }

        public hintrepeatedBytesSent1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintmediaBytesSent2 : Box
    {
        public override string FourCC { get { return "drep"; } }
        public ulong bytessent { get; set; }

        public hintmediaBytesSent2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintimmediateBytesSent2 : Box
    {
        public override string FourCC { get { return "drep"; } }
        public ulong bytessent { get; set; }

        public hintimmediateBytesSent2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintrepeatedBytesSent2 : Box
    {
        public override string FourCC { get { return "drep"; } }
        public ulong bytessent { get; set; }

        public hintrepeatedBytesSent2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytessent = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.bytessent);
            return size;
        }
    }


    public class hintminrelativetime : Box
    {
        public override string FourCC { get { return "tmin"; } }
        public int time { get; set; }

        public hintminrelativetime()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.time);
            return size;
        }
    }


    public class hintmaxrelativetime : Box
    {
        public override string FourCC { get { return "tmin"; } }
        public int time { get; set; }

        public hintmaxrelativetime()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.time);
            return size;
        }
    }


    public class hintminrelativetime1 : Box
    {
        public override string FourCC { get { return "tmax"; } }
        public int time { get; set; }

        public hintminrelativetime1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.time);
            return size;
        }
    }


    public class hintmaxrelativetime1 : Box
    {
        public override string FourCC { get { return "tmax"; } }
        public int time { get; set; }

        public hintmaxrelativetime1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.time);
            return size;
        }
    }


    public class hintlargestpacket : Box
    {
        public override string FourCC { get { return "pmax"; } }
        public uint bytes { get; set; }

        public hintlargestpacket()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytes = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytes);
            return size;
        }
    }


    public class hintlongestpacket : Box
    {
        public override string FourCC { get { return "pmax"; } }
        public uint time { get; set; }

        public hintlongestpacket()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.time);
            return size;
        }
    }


    public class hintlargestpacket1 : Box
    {
        public override string FourCC { get { return "dmax"; } }
        public uint bytes { get; set; }

        public hintlargestpacket1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bytes = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bytes);
            return size;
        }
    }


    public class hintlongestpacket1 : Box
    {
        public override string FourCC { get { return "dmax"; } }
        public uint time { get; set; }

        public hintlongestpacket1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.time);
            return size;
        }
    }


    public class hintpayloadID : Box
    {
        public override string FourCC { get { return "payt"; } }
        public uint payloadID { get; set; }  //  payload ID used in RTP packets
        public byte count { get; set; }
        public sbyte[] rtpmap_string { get; set; }

        public hintpayloadID()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.payloadID = IsoReaderWriter.ReadUInt32(stream);
            this.count = IsoReaderWriter.ReadUInt8(stream);
            this.rtpmap_string = IsoReaderWriter.ReadInt8Array(stream, count);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.payloadID);
            size += IsoReaderWriter.WriteUInt8(stream, this.count);
            size += IsoReaderWriter.WriteInt8Array(stream, count, this.rtpmap_string);
            return size;
        }
    }


    public class StereoVideoBox : FullBox
    {
        public override string FourCC { get { return "stvi"; } }
        public uint reserved { get; set; } = 0;
        public byte single_view_allowed { get; set; }
        public uint stereo_scheme { get; set; }
        public uint length { get; set; }
        public byte[] stereo_indication_type { get; set; }
        public Box[] any_box { get; set; }  //  optional

        public StereoVideoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 30);
            this.single_view_allowed = IsoReaderWriter.ReadBits(stream, 2);
            this.stereo_scheme = IsoReaderWriter.ReadUInt32(stream);
            this.length = IsoReaderWriter.ReadUInt32(stream);
            this.stereo_indication_type = IsoReaderWriter.ReadBytes(stream, length);
            this.any_box = IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 30, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 2, this.single_view_allowed);
            size += IsoReaderWriter.WriteUInt32(stream, this.stereo_scheme);
            size += IsoReaderWriter.WriteUInt32(stream, this.length);
            size += IsoReaderWriter.WriteBytes(stream, length, this.stereo_indication_type);
            size += IsoReaderWriter.WriteBoxes(stream, this.any_box);
            return size;
        }
    }


    public class ExtendedLanguageBox : FullBox
    {
        public override string FourCC { get { return "elng"; } }
        public string extended_language { get; set; }

        public ExtendedLanguageBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.extended_language = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.extended_language);
            return size;
        }
    }


    public class BitRateBox : Box
    {
        public override string FourCC { get { return "btrt"; } }
        public uint bufferSizeDB { get; set; }
        public uint maxBitrate { get; set; }
        public uint avgBitrate { get; set; }

        public BitRateBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bufferSizeDB = IsoReaderWriter.ReadUInt32(stream);
            this.maxBitrate = IsoReaderWriter.ReadUInt32(stream);
            this.avgBitrate = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.bufferSizeDB);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxBitrate);
            size += IsoReaderWriter.WriteUInt32(stream, this.avgBitrate);
            return size;
        }
    }


    public class PixelAspectRatioBox : Box
    {
        public override string FourCC { get { return "pasp"; } }
        public uint hSpacing { get; set; }
        public uint vSpacing { get; set; }

        public PixelAspectRatioBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hSpacing = IsoReaderWriter.ReadUInt32(stream);
            this.vSpacing = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.hSpacing);
            size += IsoReaderWriter.WriteUInt32(stream, this.vSpacing);
            return size;
        }
    }


    public class CleanApertureBox : Box
    {
        public override string FourCC { get { return "clap"; } }
        public uint cleanApertureWidthN { get; set; }
        public uint cleanApertureWidthD { get; set; }
        public uint cleanApertureHeightN { get; set; }
        public uint cleanApertureHeightD { get; set; }
        public uint horizOffN { get; set; }
        public uint horizOffD { get; set; }
        public uint vertOffN { get; set; }
        public uint vertOffD { get; set; }

        public CleanApertureBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.cleanApertureWidthN = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureWidthD = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureHeightN = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureHeightD = IsoReaderWriter.ReadUInt32(stream);
            this.horizOffN = IsoReaderWriter.ReadUInt32(stream);
            this.horizOffD = IsoReaderWriter.ReadUInt32(stream);
            this.vertOffN = IsoReaderWriter.ReadUInt32(stream);
            this.vertOffD = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthN);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthD);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightN);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightD);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizOffN);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizOffD);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertOffN);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertOffD);
            return size;
        }
    }


    public class ContentColourVolumeBox : Box
    {
        public override string FourCC { get { return "cclv"; } }
        public bool reserved1 { get; set; } = false;  //  ccv_cancel_flag
        public bool reserved2 { get; set; } = false;  //  ccv_persistence_flag
        public bool ccv_primaries_present_flag { get; set; }
        public bool ccv_min_luminance_value_present_flag { get; set; }
        public bool ccv_max_luminance_value_present_flag { get; set; }
        public bool ccv_avg_luminance_value_present_flag { get; set; }
        public byte ccv_reserved_zero_2bits { get; set; } = 0;
        public int[] ccv_primaries_x { get; set; }
        public int[] ccv_primaries_y { get; set; }
        public uint ccv_min_luminance_value { get; set; }
        public uint ccv_max_luminance_value { get; set; }
        public uint ccv_avg_luminance_value { get; set; }

        public ContentColourVolumeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved1 = IsoReaderWriter.ReadBit(stream);
            this.reserved2 = IsoReaderWriter.ReadBit(stream);
            this.ccv_primaries_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_min_luminance_value_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_max_luminance_value_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_avg_luminance_value_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_reserved_zero_2bits = IsoReaderWriter.ReadBits(stream, 2);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    this.ccv_primaries_x[c] = IsoReaderWriter.ReadInt32(stream);
                    this.ccv_primaries_y[c] = IsoReaderWriter.ReadInt32(stream);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                this.ccv_min_luminance_value = IsoReaderWriter.ReadUInt32(stream);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                this.ccv_max_luminance_value = IsoReaderWriter.ReadUInt32(stream);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                this.ccv_avg_luminance_value = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.reserved1);
            size += IsoReaderWriter.WriteBit(stream, this.reserved2);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_primaries_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_min_luminance_value_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_max_luminance_value_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_avg_luminance_value_present_flag);
            size += IsoReaderWriter.WriteBits(stream, 2, this.ccv_reserved_zero_2bits);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    size += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_x[c]);
                    size += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_y[c]);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.ccv_min_luminance_value);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.ccv_max_luminance_value);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.ccv_avg_luminance_value);
            }
            return size;
        }
    }


    public class ColourInformationBox : Box
    {
        public override string FourCC { get { return "colr"; } }
        public uint colour_type { get; set; }
        public ushort colour_primaries { get; set; }
        public ushort transfer_characteristics { get; set; }
        public ushort matrix_coefficients { get; set; }
        public bool full_range_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public ICC_profile ICC_profile { get; set; }  //  restricted ICC profile

        public ColourInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.colour_type = IsoReaderWriter.ReadUInt32(stream);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                this.colour_primaries = IsoReaderWriter.ReadUInt16(stream);
                this.transfer_characteristics = IsoReaderWriter.ReadUInt16(stream);
                this.matrix_coefficients = IsoReaderWriter.ReadUInt16(stream);
                this.full_range_flag = IsoReaderWriter.ReadBit(stream);
                this.reserved = IsoReaderWriter.ReadBits(stream, 7);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                this.ICC_profile = (ICC_profile)IsoReaderWriter.ReadClass(stream);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                this.ICC_profile = (ICC_profile)IsoReaderWriter.ReadClass(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.colour_type);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.colour_primaries);
                size += IsoReaderWriter.WriteUInt16(stream, this.transfer_characteristics);
                size += IsoReaderWriter.WriteUInt16(stream, this.matrix_coefficients);
                size += IsoReaderWriter.WriteBit(stream, this.full_range_flag);
                size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                size += IsoReaderWriter.WriteClass(stream, this.ICC_profile);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                size += IsoReaderWriter.WriteClass(stream, this.ICC_profile);
            }
            return size;
        }
    }


    public class ContentLightLevelBox : Box
    {
        public override string FourCC { get { return "clli"; } }
        public ushort max_content_light_level { get; set; }
        public ushort max_pic_average_light_level { get; set; }

        public ContentLightLevelBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.max_content_light_level = IsoReaderWriter.ReadUInt16(stream);
            this.max_pic_average_light_level = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.max_content_light_level);
            size += IsoReaderWriter.WriteUInt16(stream, this.max_pic_average_light_level);
            return size;
        }
    }


    public class MasteringDisplayColourVolumeBox : Box
    {
        public override string FourCC { get { return "mdcv"; } }
        public ushort display_primaries_x { get; set; }
        public ushort display_primaries_y { get; set; }
        public ushort white_point_x { get; set; }
        public ushort white_point_y { get; set; }
        public uint max_display_mastering_luminance { get; set; }
        public uint min_display_mastering_luminance { get; set; }

        public MasteringDisplayColourVolumeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                this.display_primaries_x = IsoReaderWriter.ReadUInt16(stream);
                this.display_primaries_y = IsoReaderWriter.ReadUInt16(stream);
            }
            this.white_point_x = IsoReaderWriter.ReadUInt16(stream);
            this.white_point_y = IsoReaderWriter.ReadUInt16(stream);
            this.max_display_mastering_luminance = IsoReaderWriter.ReadUInt32(stream);
            this.min_display_mastering_luminance = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_x);
                size += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_y);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.white_point_x);
            size += IsoReaderWriter.WriteUInt16(stream, this.white_point_y);
            size += IsoReaderWriter.WriteUInt32(stream, this.max_display_mastering_luminance);
            size += IsoReaderWriter.WriteUInt32(stream, this.min_display_mastering_luminance);
            return size;
        }
    }


    public class ScrambleSchemeInfoBox : Box
    {
        public override string FourCC { get { return "scrb"; } }
        public SchemeTypeBox scheme_type_box { get; set; }
        public SchemeInformationBox info { get; set; }  //  optional

        public ScrambleSchemeInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scheme_type_box = (SchemeTypeBox)IsoReaderWriter.ReadBox(stream);
            this.info = (SchemeInformationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            size += IsoReaderWriter.WriteBox(stream, this.info);
            return size;
        }
    }


    public class ChannelLayout : FullBox
    {
        public override string FourCC { get { return "chnl"; } }
        public byte stream_structure { get; set; }
        public byte definedLayout { get; set; }
        public byte speaker_position { get; set; }
        public short azimuth { get; set; }
        public sbyte elevation { get; set; }
        public ulong omittedChannelsMap { get; set; }  //  a ‘1’ bit indicates ‘not in this track’
        public byte object_count { get; set; }
        public byte format_ordering { get; set; }
        public byte baseChannelCount { get; set; }
        public byte layout_channel_count { get; set; }
        public byte reserved { get; set; } = 0;
        public byte channel_order_definition { get; set; }
        public bool omitted_channels_present { get; set; }

        public ChannelLayout()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.stream_structure = IsoReaderWriter.ReadUInt8(stream);

                if ((stream_structure & channelStructured) == channelStructured)
                {
                    this.definedLayout = IsoReaderWriter.ReadUInt8(stream);

                    if (definedLayout == 0)
                    {

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            /*   layout_channel_count comes from the sample entry */
                            this.speaker_position = IsoReaderWriter.ReadUInt8(stream);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                this.azimuth = IsoReaderWriter.ReadInt16(stream);
                                this.elevation = IsoReaderWriter.ReadInt8(stream);
                            }
                        }
                    }

                    else
                    {
                        this.omittedChannelsMap = IsoReaderWriter.ReadUInt64(stream);
                    }
                }

                if ((stream_structure & objectStructured) == objectStructured)
                {
                    this.object_count = IsoReaderWriter.ReadUInt8(stream);
                }
            }

            else
            {
                this.stream_structure = IsoReaderWriter.ReadBits(stream, 4);
                this.format_ordering = IsoReaderWriter.ReadBits(stream, 4);
                this.baseChannelCount = IsoReaderWriter.ReadUInt8(stream);

                if ((stream_structure & channelStructured) == channelStructured)
                {
                    this.definedLayout = IsoReaderWriter.ReadUInt8(stream);

                    if (definedLayout == 0)
                    {
                        this.layout_channel_count = IsoReaderWriter.ReadUInt8(stream);

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            this.speaker_position = IsoReaderWriter.ReadUInt8(stream);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                this.azimuth = IsoReaderWriter.ReadInt16(stream);
                                this.elevation = IsoReaderWriter.ReadInt8(stream);
                            }
                        }
                    }

                    else
                    {
                        this.reserved = IsoReaderWriter.ReadBits(stream, 4);
                        this.channel_order_definition = IsoReaderWriter.ReadBits(stream, 3);
                        this.omitted_channels_present = IsoReaderWriter.ReadBit(stream);

                        if (omitted_channels_present == 1)
                        {
                            this.omittedChannelsMap = IsoReaderWriter.ReadUInt64(stream);
                        }
                    }
                }

                if ((stream_structure & objectStructured) == objectStructured)
                {
                    /*  object_count is derived from baseChannelCount */
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.stream_structure);

                if ((stream_structure & channelStructured) == channelStructured)
                {
                    size += IsoReaderWriter.WriteUInt8(stream, this.definedLayout);

                    if (definedLayout == 0)
                    {

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            /*   layout_channel_count comes from the sample entry */
                            size += IsoReaderWriter.WriteUInt8(stream, this.speaker_position);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                size += IsoReaderWriter.WriteInt16(stream, this.azimuth);
                                size += IsoReaderWriter.WriteInt8(stream, this.elevation);
                            }
                        }
                    }

                    else
                    {
                        size += IsoReaderWriter.WriteUInt64(stream, this.omittedChannelsMap);
                    }
                }

                if ((stream_structure & objectStructured) == objectStructured)
                {
                    size += IsoReaderWriter.WriteUInt8(stream, this.object_count);
                }
            }

            else
            {
                size += IsoReaderWriter.WriteBits(stream, 4, this.stream_structure);
                size += IsoReaderWriter.WriteBits(stream, 4, this.format_ordering);
                size += IsoReaderWriter.WriteUInt8(stream, this.baseChannelCount);

                if ((stream_structure & channelStructured) == channelStructured)
                {
                    size += IsoReaderWriter.WriteUInt8(stream, this.definedLayout);

                    if (definedLayout == 0)
                    {
                        size += IsoReaderWriter.WriteUInt8(stream, this.layout_channel_count);

                        for (int i = 1; i <= layout_channel_count; i++)
                        {
                            size += IsoReaderWriter.WriteUInt8(stream, this.speaker_position);

                            if (speaker_position == 126)
                            {
                                /*  explicit position */
                                size += IsoReaderWriter.WriteInt16(stream, this.azimuth);
                                size += IsoReaderWriter.WriteInt8(stream, this.elevation);
                            }
                        }
                    }

                    else
                    {
                        size += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
                        size += IsoReaderWriter.WriteBits(stream, 3, this.channel_order_definition);
                        size += IsoReaderWriter.WriteBit(stream, this.omitted_channels_present);

                        if (omitted_channels_present == 1)
                        {
                            size += IsoReaderWriter.WriteUInt64(stream, this.omittedChannelsMap);
                        }
                    }
                }

                if ((stream_structure & objectStructured) == objectStructured)
                {
                    /*  object_count is derived from baseChannelCount */
                }
            }
            return size;
        }
    }


    public class DownMixInstructions : FullBox
    {
        public override string FourCC { get { return "dmix"; } }
        public byte[] reserved { get; set; } = [];  //  byte align
        public int downmix_instructions_count { get; set; } = 1;
        public byte targetLayout { get; set; }
        public byte targetChannelCount { get; set; }
        public bool in_stream { get; set; }
        public byte downmix_ID { get; set; }
        public byte bs_downmix_offset { get; set; }
        public int size { get; set; } = 4;
        public byte bs_downmix_coefficient_v1 { get; set; }
        public byte bs_downmix_coefficient { get; set; }

        public DownMixInstructions()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version >= 1)
            {
                this.reserved = IsoReaderWriter.ReadBit(stream);
                this.downmix_instructions_count = IsoReaderWriter.ReadBits(stream, 7);
            }

            else
            {
                this.downmix_instructions_count = IsoReaderWriter.ReadInt32(stream);
            }

            for (int a = 1; a <= downmix_instructions_count; a++)
            {
                this.targetLayout = IsoReaderWriter.ReadUInt8(stream);
                this.reserved = IsoReaderWriter.ReadBit(stream);
                this.targetChannelCount = IsoReaderWriter.ReadBits(stream, 7);
                this.in_stream = IsoReaderWriter.ReadBit(stream);
                this.downmix_ID = IsoReaderWriter.ReadBits(stream, 7);

                if (in_stream == 0)
                {
                    /*  downmix coefficients are out of stream and supplied here */


                    if (version >= 1)
                    {
                        this.bs_downmix_offset = IsoReaderWriter.ReadBits(stream, 4);
                        this.size = IsoReaderWriter.ReadInt32(stream);

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                this.bs_downmix_coefficient_v1 = IsoReaderWriter.ReadBits(stream, 5);
                                size += 5;
                            }
                        }
                        this.reserved = IsoReaderWriter.ReadBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size));
                    }

                    else
                    {

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                this.bs_downmix_coefficient = IsoReaderWriter.ReadBits(stream, 4);
                            }
                        }
                    }
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version >= 1)
            {
                size += IsoReaderWriter.WriteBit(stream, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 7, this.downmix_instructions_count);
            }

            else
            {
                size += IsoReaderWriter.WriteInt32(stream, this.downmix_instructions_count);
            }

            for (int a = 1; a <= downmix_instructions_count; a++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.targetLayout);
                size += IsoReaderWriter.WriteBit(stream, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 7, this.targetChannelCount);
                size += IsoReaderWriter.WriteBit(stream, this.in_stream);
                size += IsoReaderWriter.WriteBits(stream, 7, this.downmix_ID);

                if (in_stream == 0)
                {
                    /*  downmix coefficients are out of stream and supplied here */


                    if (version >= 1)
                    {
                        size += IsoReaderWriter.WriteBits(stream, 4, this.bs_downmix_offset);
                        size += IsoReaderWriter.WriteInt32(stream, this.size);

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                size += IsoReaderWriter.WriteBits(stream, 5, this.bs_downmix_coefficient_v1);
                                size += 5;
                            }
                        }
                        size += IsoReaderWriter.WriteBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size), this.reserved);
                    }

                    else
                    {

                        for (int i = 1; i <= targetChannelCount; i++)
                        {

                            for (int j = 1; j <= baseChannelCount; j++)
                            {
                                size += IsoReaderWriter.WriteBits(stream, 4, this.bs_downmix_coefficient);
                            }
                        }
                    }
                }
            }
            return size;
        }
    }


    public class SamplingRateBox : FullBox
    {
        public override string FourCC { get { return "srat"; } }
        public uint sampling_rate { get; set; }

        public SamplingRateBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sampling_rate = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.sampling_rate);
            return size;
        }
    }


    public class TextConfigBox : FullBox
    {
        public override string FourCC { get { return "txtC"; } }
        public string text_config { get; set; }

        public TextConfigBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.text_config = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.text_config);
            return size;
        }
    }


    public class URIInitBox : FullBox
    {
        public override string FourCC { get { return "uriI"; } }
        public byte[] uri_initialization_data { get; set; }

        public URIInitBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.uri_initialization_data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.uri_initialization_data);
            return size;
        }
    }


    public class CopyrightBox : FullBox
    {
        public override string FourCC { get { return "cprt"; } }
        public bool pad { get; set; } = false;
        public byte[] language { get; set; }  //  ISO-639-2/T language code
        public string notice { get; set; }

        public CopyrightBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.pad = IsoReaderWriter.ReadBit(stream);
            this.language = IsoReaderWriter.ReadBitsArray(stream, 5, 3);
            this.notice = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.pad);
            size += IsoReaderWriter.WriteBitsArray(stream, 5, 3, this.language);
            size += IsoReaderWriter.WriteString(stream, this.notice);
            return size;
        }
    }


    public class KindBox : FullBox
    {
        public override string FourCC { get { return "kind"; } }
        public string schemeURI { get; set; }
        public string value { get; set; }

        public KindBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.schemeURI = IsoReaderWriter.ReadString(stream);
            this.value = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.schemeURI);
            size += IsoReaderWriter.WriteString(stream, this.value);
            return size;
        }
    }


    public class TrackSelectionBox : FullBox
    {
        public override string FourCC { get { return "tsel"; } }
        public int switch_group { get; set; } = 0;
        public uint[] attribute_list { get; set; }  //  to end of the box

        public TrackSelectionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.switch_group = IsoReaderWriter.ReadInt32(stream);
            this.attribute_list = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.switch_group);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.attribute_list);
            return size;
        }
    }


    public class SubTrackBox : Box
    {
        public override string FourCC { get { return "strk"; } }

        public SubTrackBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class trackhintinformation : Box
    {
        public override string FourCC { get { return "hnti"; } }

        public trackhintinformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class rtptracksdphintinformation : Box
    {
        public override string FourCC { get { return "hnti"; } }
        public sbyte[] sdptext { get; set; }

        public rtptracksdphintinformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sdptext = IsoReaderWriter.ReadInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return size;
        }
    }


    public class trackhintinformation1 : Box
    {
        public override string FourCC { get { return "sdp "; } }

        public trackhintinformation1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class rtptracksdphintinformation1 : Box
    {
        public override string FourCC { get { return "sdp "; } }
        public sbyte[] sdptext { get; set; }

        public rtptracksdphintinformation1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sdptext = IsoReaderWriter.ReadInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return size;
        }
    }


    public class moviehintinformation : Box
    {
        public override string FourCC { get { return "rtp "; } }

        public moviehintinformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class rtpmoviehintinformation : Box
    {
        public override string FourCC { get { return "rtp "; } }
        public uint descriptionformat { get; set; } = "sdp ";
        public sbyte[] sdptext { get; set; }

        public rtpmoviehintinformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.descriptionformat = IsoReaderWriter.ReadUInt32(stream);
            this.sdptext = IsoReaderWriter.ReadInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.descriptionformat);
            size += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return size;
        }
    }


    public class hintstatisticsbox : Box
    {
        public override string FourCC { get { return "hinf"; } }

        public hintstatisticsbox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class LoudnessBox : Box
    {
        public override string FourCC { get { return "ludt"; } }
        public int[] TrackLoudnessInfo { get; set; }  //  not more than one AlbumLoudnessInfo box with version>=1 is allowed	albumLoudness	AlbumLoudnessInfo[];

        public LoudnessBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            /*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
            this.TrackLoudnessInfo = IsoReaderWriter.ReadInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            /*  not more than one TrackLoudnessInfo box with version>=1 is allowed */
            size += IsoReaderWriter.WriteInt32Array(stream, this.TrackLoudnessInfo);
            return size;
        }
    }


    public class TrackLoudnessInfo : LoudnessBaseBox
    {
        public override string FourCC { get { return "tlou"; } }

        public TrackLoudnessInfo()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class AlbumLoudnessInfo : LoudnessBaseBox
    {
        public override string FourCC { get { return "alou"; } }

        public AlbumLoudnessInfo()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class DataEntryUrlBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "url "; } }
        public string location { get; set; }

        public DataEntryUrlBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.location = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.location);
            return size;
        }
    }


    public class DataEntryUrnBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "urn "; } }
        public string name { get; set; }
        public string location { get; set; }

        public DataEntryUrnBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.name = IsoReaderWriter.ReadString(stream);
            this.location = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.name);
            size += IsoReaderWriter.WriteString(stream, this.location);
            return size;
        }
    }


    public class DataEntryImdaBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "imdt"; } }
        public uint imda_ref_identifier { get; set; }

        public DataEntryImdaBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.imda_ref_identifier = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.imda_ref_identifier);
            return size;
        }
    }


    public class DataEntrySeqNumImdaBox : DataEntryBaseBox
    {
        public override string FourCC { get { return "snim"; } }

        public DataEntrySeqNumImdaBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class ItemPropertyContainerBox : Box
    {
        public override string FourCC { get { return "ipco"; } }
        public Box[] properties { get; set; }  //  boxes derived from

        public ItemPropertyContainerBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.properties = IsoReaderWriter.ReadBoxes(stream);
            /*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
            /*  to fill the box */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.properties);
            /*  ItemProperty or ItemFullProperty, or FreeSpaceBox(es) */
            /*  to fill the box */
            return size;
        }
    }


    public class ItemPropertyAssociationBox : FullBox
    {
        public override string FourCC { get { return "ipma"; } }
        public uint entry_count { get; set; }
        public uint item_ID { get; set; }
        public byte association_count { get; set; }
        public bool essential { get; set; }
        public ushort property_index { get; set; }

        public ItemPropertyAssociationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < entry_count; i++)
            {

                if (version < 1)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt16(stream);
                }

                else
                {
                    this.item_ID = IsoReaderWriter.ReadUInt32(stream);
                }
                this.association_count = IsoReaderWriter.ReadUInt8(stream);

                for (int j = 0; j < association_count; j++)
                {
                    this.essential = IsoReaderWriter.ReadBit(stream);

                    if ((flags & 1) == 1)
                    {
                        this.property_index = IsoReaderWriter.ReadBits(stream, 15);
                    }

                    else
                    {
                        this.property_index = IsoReaderWriter.ReadBits(stream, 7);
                    }
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 0; i < entry_count; i++)
            {

                if (version < 1)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
                }
                size += IsoReaderWriter.WriteUInt8(stream, this.association_count);

                for (int j = 0; j < association_count; j++)
                {
                    size += IsoReaderWriter.WriteBit(stream, this.essential);

                    if ((flags & 1) == 1)
                    {
                        size += IsoReaderWriter.WriteBits(stream, 15, this.property_index);
                    }

                    else
                    {
                        size += IsoReaderWriter.WriteBits(stream, 7, this.property_index);
                    }
                }
            }
            return size;
        }
    }


    public class ItemPropertiesBox : Box
    {
        public override string FourCC { get { return "iprp"; } }
        public ItemPropertyContainerBox property_container { get; set; }
        public ItemPropertyAssociationBox[] association { get; set; }

        public ItemPropertiesBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.property_container = (ItemPropertyContainerBox)IsoReaderWriter.ReadBox(stream);
            this.association = (ItemPropertyAssociationBox[])IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.property_container);
            size += IsoReaderWriter.WriteBoxes(stream, this.association);
            return size;
        }
    }


    public class AlternativeStartupSequencePropertiesBox : FullBox
    {
        public override string FourCC { get { return "assp"; } }
        public int min_initial_alt_startup_offset { get; set; }
        public uint num_entries { get; set; }
        public uint grouping_type_parameter { get; set; }

        public AlternativeStartupSequencePropertiesBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.min_initial_alt_startup_offset = IsoReaderWriter.ReadInt32(stream);
            }

            else if (version == 1)
            {
                this.num_entries = IsoReaderWriter.ReadUInt32(stream);

                for (int j = 1; j <= num_entries; j++)
                {
                    this.grouping_type_parameter = IsoReaderWriter.ReadUInt32(stream);
                    this.min_initial_alt_startup_offset = IsoReaderWriter.ReadInt32(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteInt32(stream, this.min_initial_alt_startup_offset);
            }

            else if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

                for (int j = 1; j <= num_entries; j++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
                    size += IsoReaderWriter.WriteInt32(stream, this.min_initial_alt_startup_offset);
                }
            }
            return size;
        }
    }


    public class BinaryXMLBox : FullBox
    {
        public override string FourCC { get { return "bxml"; } }
        public byte[] data { get; set; }  //  to end of box

        public BinaryXMLBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return size;
        }
    }


    public class CompleteTrackInfoBox : Box
    {
        public override string FourCC { get { return "cinf"; } }
        public OriginalFormatBox original_format { get; set; }

        public CompleteTrackInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.original_format = (OriginalFormatBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.original_format);
            return size;
        }
    }


    public class ChunkLargeOffsetBox : FullBox
    {
        public override string FourCC { get { return "co64"; } }
        public uint entry_count { get; set; }
        public ulong chunk_offset { get; set; }

        public ChunkLargeOffsetBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.chunk_offset = IsoReaderWriter.ReadUInt64(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.chunk_offset);
            }
            return size;
        }
    }


    public class CompactSampleToGroupBox : FullBox
    {
        public override string FourCC { get { return "csgp"; } }
        public uint grouping_type { get; set; }
        public uint grouping_type_parameter { get; set; }
        public uint pattern_count { get; set; }
        public byte[] pattern_length { get; set; }
        public byte[] sample_count { get; set; }
        public byte[] sample_group_description_index { get; set; }  //  whose msb might indicate fragment_local or global

        public CompactSampleToGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.grouping_type = IsoReaderWriter.ReadUInt32(stream);

            if (grouping_type_parameter_present == 1)
            {
                this.grouping_type_parameter = IsoReaderWriter.ReadUInt32(stream);
            }
            this.pattern_count = IsoReaderWriter.ReadUInt32(stream);
            totalPatternLength = 0;

            for (int i = 1; i <= pattern_count; i++)
            {
                this.pattern_length[i] = IsoReaderWriter.ReadBits(stream, pattern_size_code);
                this.sample_count[i] = IsoReaderWriter.ReadBits(stream, count_size_code);
            }

            for (int j = 1; j <= pattern_count; j++)
            {

                for (int k = 1; k <= pattern_length[j]; k++)
                {
                    this.sample_group_description_index[j][k] = IsoReaderWriter.ReadBits(stream, index_size_code);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

            if (grouping_type_parameter_present == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
            }
            size += IsoReaderWriter.WriteUInt32(stream, this.pattern_count);
            totalPatternLength = 0;

            for (int i = 1; i <= pattern_count; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, pattern_size_code, this.pattern_length[i]);
                size += IsoReaderWriter.WriteBits(stream, count_size_code, this.sample_count[i]);
            }

            for (int j = 1; j <= pattern_count; j++)
            {

                for (int k = 1; k <= pattern_length[j]; k++)
                {
                    size += IsoReaderWriter.WriteBits(stream, index_size_code, this.sample_group_description_index[j][k]);
                }
            }
            return size;
        }
    }


    public class CompositionToDecodeBox : FullBox
    {
        public override string FourCC { get { return "cslg"; } }
        public long compositionToDTSShift { get; set; }
        public long leastDecodeToDisplayDelta { get; set; }
        public long greatestDecodeToDisplayDelta { get; set; }
        public long compositionStartTime { get; set; }
        public long compositionEndTime { get; set; }

        public CompositionToDecodeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.compositionToDTSShift = IsoReaderWriter.ReadInt32(stream);
                this.leastDecodeToDisplayDelta = IsoReaderWriter.ReadInt32(stream);
                this.greatestDecodeToDisplayDelta = IsoReaderWriter.ReadInt32(stream);
                this.compositionStartTime = IsoReaderWriter.ReadInt32(stream);
                this.compositionEndTime = IsoReaderWriter.ReadInt32(stream);
            }

            else
            {
                this.compositionToDTSShift = IsoReaderWriter.ReadInt64(stream);
                this.leastDecodeToDisplayDelta = IsoReaderWriter.ReadInt64(stream);
                this.greatestDecodeToDisplayDelta = IsoReaderWriter.ReadInt64(stream);
                this.compositionStartTime = IsoReaderWriter.ReadInt64(stream);
                this.compositionEndTime = IsoReaderWriter.ReadInt64(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteInt32(stream, this.compositionToDTSShift);
                size += IsoReaderWriter.WriteInt32(stream, this.leastDecodeToDisplayDelta);
                size += IsoReaderWriter.WriteInt32(stream, this.greatestDecodeToDisplayDelta);
                size += IsoReaderWriter.WriteInt32(stream, this.compositionStartTime);
                size += IsoReaderWriter.WriteInt32(stream, this.compositionEndTime);
            }

            else
            {
                size += IsoReaderWriter.WriteInt64(stream, this.compositionToDTSShift);
                size += IsoReaderWriter.WriteInt64(stream, this.leastDecodeToDisplayDelta);
                size += IsoReaderWriter.WriteInt64(stream, this.greatestDecodeToDisplayDelta);
                size += IsoReaderWriter.WriteInt64(stream, this.compositionStartTime);
                size += IsoReaderWriter.WriteInt64(stream, this.compositionEndTime);
            }
            return size;
        }
    }


    public class CompositionOffsetBox : FullBox
    {
        public override string FourCC { get { return "ctts"; } }
        public uint entry_count { get; set; }
        public uint sample_count { get; set; }
        public uint sample_offset { get; set; }

        public CompositionOffsetBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            if (version == 0)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    this.sample_count = IsoReaderWriter.ReadUInt32(stream);
                    this.sample_offset = IsoReaderWriter.ReadUInt32(stream);
                }
            }

            else if (version == 1)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    this.sample_count = IsoReaderWriter.ReadUInt32(stream);
                    this.sample_offset = IsoReaderWriter.ReadInt32(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            if (version == 0)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                    size += IsoReaderWriter.WriteUInt32(stream, this.sample_offset);
                }
            }

            else if (version == 1)
            {

                for (int i = 0; i < entry_count; i++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                    size += IsoReaderWriter.WriteInt32(stream, this.sample_offset);
                }
            }
            return size;
        }
    }


    public class DataInformationBox : Box
    {
        public override string FourCC { get { return "dinf"; } }

        public DataInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class DataReferenceBox : FullBox
    {
        public override string FourCC { get { return "dref"; } }
        public uint entry_count { get; set; }
        public DataEntryBaseBox data_entry { get; set; }

        public DataReferenceBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.data_entry = (DataEntryBaseBox)IsoReaderWriter.ReadBox(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteBox(stream, this.data_entry);
            }
            return size;
        }
    }


    public class EditBox : Box
    {
        public override string FourCC { get { return "edts"; } }

        public EditBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class EditListBox : FullBox
    {
        public override string FourCC { get { return "elst"; } }
        public uint entry_count { get; set; }
        public ulong edit_duration { get; set; }
        public long media_time { get; set; }
        public short media_rate_integer { get; set; }
        public short media_rate_fraction { get; set; }

        public EditListBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 1)
                {
                    this.edit_duration = IsoReaderWriter.ReadUInt64(stream);
                    this.media_time = IsoReaderWriter.ReadInt64(stream);
                }

                else
                {
                    /*  version==0 */
                    this.edit_duration = IsoReaderWriter.ReadUInt32(stream);
                    this.media_time = IsoReaderWriter.ReadInt32(stream);
                }
                this.media_rate_integer = IsoReaderWriter.ReadInt16(stream);
                this.media_rate_fraction = IsoReaderWriter.ReadInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 1)
                {
                    size += IsoReaderWriter.WriteUInt64(stream, this.edit_duration);
                    size += IsoReaderWriter.WriteInt64(stream, this.media_time);
                }

                else
                {
                    /*  version==0 */
                    size += IsoReaderWriter.WriteUInt32(stream, this.edit_duration);
                    size += IsoReaderWriter.WriteInt32(stream, this.media_time);
                }
                size += IsoReaderWriter.WriteInt16(stream, this.media_rate_integer);
                size += IsoReaderWriter.WriteInt16(stream, this.media_rate_fraction);
            }
            return size;
        }
    }


    public class ExtendedTypeBox : Box
    {
        public override string FourCC { get { return "etyp"; } }
        public TypeCombinationBox[] compatible_combinations { get; set; }  //  to end of the box

        public ExtendedTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.compatible_combinations = (TypeCombinationBox[])IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.compatible_combinations);
            return size;
        }
    }


    public class FDItemInfoExtension : ItemInfoExtension
    {
        public override string FourCC { get { return "fdel"; } }
        public string content_location { get; set; }
        public string content_MD5 { get; set; }
        public ulong content_length { get; set; }
        public ulong transfer_length { get; set; }
        public byte entry_count { get; set; }
        public uint group_id { get; set; }

        public FDItemInfoExtension()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.content_location = IsoReaderWriter.ReadString(stream);
            this.content_MD5 = IsoReaderWriter.ReadString(stream);
            this.content_length = IsoReaderWriter.ReadUInt64(stream);
            this.transfer_length = IsoReaderWriter.ReadUInt64(stream);
            this.entry_count = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.group_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.content_location);
            size += IsoReaderWriter.WriteString(stream, this.content_MD5);
            size += IsoReaderWriter.WriteUInt64(stream, this.content_length);
            size += IsoReaderWriter.WriteUInt64(stream, this.transfer_length);
            size += IsoReaderWriter.WriteUInt8(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            }
            return size;
        }
    }


    public class FECReservoirBox : FullBox
    {
        public override string FourCC { get { return "fecr"; } }
        public uint entry_count { get; set; }
        public uint item_ID { get; set; }
        public uint symbol_count { get; set; }

        public FECReservoirBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.entry_count = IsoReaderWriter.ReadUInt32(stream);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt16(stream);
                }

                else
                {
                    this.item_ID = IsoReaderWriter.ReadUInt32(stream);
                }
                this.symbol_count = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
                }
                size += IsoReaderWriter.WriteUInt32(stream, this.symbol_count);
            }
            return size;
        }
    }


    public class PartitionEntry : Box
    {
        public override string FourCC { get { return "fiin"; } }
        public FilePartitionBox blocks_and_symbols { get; set; }
        public FECReservoirBox FEC_symbol_locations { get; set; }  // optional
        public FileReservoirBox File_symbol_locations { get; set; }  // optional

        public PartitionEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.blocks_and_symbols = (FilePartitionBox)IsoReaderWriter.ReadBox(stream);
            this.FEC_symbol_locations = (FECReservoirBox)IsoReaderWriter.ReadBox(stream);
            this.File_symbol_locations = (FileReservoirBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.blocks_and_symbols);
            size += IsoReaderWriter.WriteBox(stream, this.FEC_symbol_locations);
            size += IsoReaderWriter.WriteBox(stream, this.File_symbol_locations);
            return size;
        }
    }


    public class FDItemInformationBox : FullBox
    {
        public override string FourCC { get { return "fiin"; } }
        public ushort entry_count { get; set; }
        public PartitionEntry[] partition_entries { get; set; }
        public FDSessionGroupBox session_info { get; set; }  // optional
        public GroupIdToNameBox group_id_to_name { get; set; }  // optional

        public FDItemInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            this.partition_entries = (PartitionEntry[])IsoReaderWriter.ReadClasses(stream, entry_count);
            this.session_info = (FDSessionGroupBox)IsoReaderWriter.ReadBox(stream);
            this.group_id_to_name = (GroupIdToNameBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            size += IsoReaderWriter.WriteClasses(stream, entry_count, this.partition_entries);
            size += IsoReaderWriter.WriteBox(stream, this.session_info);
            size += IsoReaderWriter.WriteBox(stream, this.group_id_to_name);
            return size;
        }
    }


    public class FileReservoirBox : FullBox
    {
        public override string FourCC { get { return "fire"; } }
        public uint entry_count { get; set; }
        public uint item_ID { get; set; }
        public uint symbol_count { get; set; }

        public FileReservoirBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.entry_count = IsoReaderWriter.ReadUInt32(stream);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt16(stream);
                }

                else
                {
                    this.item_ID = IsoReaderWriter.ReadUInt32(stream);
                }
                this.symbol_count = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (version == 0)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
                }
                size += IsoReaderWriter.WriteUInt32(stream, this.symbol_count);
            }
            return size;
        }
    }


    public class FilePartitionBox : FullBox
    {
        public override string FourCC { get { return "fpar"; } }
        public uint item_ID { get; set; }
        public ushort packet_payload_size { get; set; }
        public byte reserved { get; set; } = 0;
        public byte FEC_encoding_ID { get; set; }
        public ushort FEC_instance_ID { get; set; }
        public ushort max_source_block_length { get; set; }
        public ushort encoding_symbol_length { get; set; }
        public ushort max_number_of_encoding_symbols { get; set; }
        public string scheme_specific_info { get; set; }
        public uint entry_count { get; set; }
        public ushort block_count { get; set; }
        public uint block_size { get; set; }

        public FilePartitionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.item_ID = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
            this.packet_payload_size = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt8(stream);
            this.FEC_encoding_ID = IsoReaderWriter.ReadUInt8(stream);
            this.FEC_instance_ID = IsoReaderWriter.ReadUInt16(stream);
            this.max_source_block_length = IsoReaderWriter.ReadUInt16(stream);
            this.encoding_symbol_length = IsoReaderWriter.ReadUInt16(stream);
            this.max_number_of_encoding_symbols = IsoReaderWriter.ReadUInt16(stream);
            this.scheme_specific_info = IsoReaderWriter.ReadString(stream);

            if (version == 0)
            {
                this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.entry_count = IsoReaderWriter.ReadUInt32(stream);
            }

            for (int i = 1; i <= entry_count; i++)
            {
                this.block_count = IsoReaderWriter.ReadUInt16(stream);
                this.block_size = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.packet_payload_size);
            size += IsoReaderWriter.WriteUInt8(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt8(stream, this.FEC_encoding_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.FEC_instance_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.max_source_block_length);
            size += IsoReaderWriter.WriteUInt16(stream, this.encoding_symbol_length);
            size += IsoReaderWriter.WriteUInt16(stream, this.max_number_of_encoding_symbols);
            size += IsoReaderWriter.WriteString(stream, this.scheme_specific_info);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);
            }

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.block_count);
                size += IsoReaderWriter.WriteUInt32(stream, this.block_size);
            }
            return size;
        }
    }


    public class FreeSpaceBox : Box
    {
        public override string FourCC { get { return "free"; } }
        public byte[] data { get; set; }

        public FreeSpaceBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return size;
        }
    }


    public class OriginalFormatBox : Box
    {
        public override string FourCC { get { return "frma"; } }
        public uint data_format { get; set; } // = codingname

        public OriginalFormatBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.data_format = IsoReaderWriter.ReadUInt32(stream);
            /*  or un-transformed sample entry (in case of restriction */
            /*  and complete track information) */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.data_format);
            /*  or un-transformed sample entry (in case of restriction */
            /*  and complete track information) */
            return size;
        }
    }


    public class FileTypeBox : GeneralTypeBox
    {
        public override string FourCC { get { return "ftyp"; } }

        public FileTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class GroupIdToNameBox : FullBox
    {
        public override string FourCC { get { return "gitn"; } }
        public ushort entry_count { get; set; }
        public uint group_ID { get; set; }
        public string group_name { get; set; }

        public GroupIdToNameBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.group_ID = IsoReaderWriter.ReadUInt32(stream);
                this.group_name = IsoReaderWriter.ReadString(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.group_ID);
                size += IsoReaderWriter.WriteString(stream, this.group_name);
            }
            return size;
        }
    }


    public class GroupsListBox : Box
    {
        public override string FourCC { get { return "grpl"; } }

        public GroupsListBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class HandlerBox : FullBox
    {
        public override string FourCC { get { return "hdlr"; } }
        public uint pre_defined { get; set; } = 0;
        public uint handler_type { get; set; }
        public uint[] reserved { get; set; } = [];
        public string name { get; set; }

        public HandlerBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt32(stream);
            this.handler_type = IsoReaderWriter.ReadUInt32(stream);
            this.reserved = IsoReaderWriter.ReadUInt32Array(stream, 3);
            this.name = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt32(stream, this.handler_type);
            size += IsoReaderWriter.WriteUInt32Array(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteString(stream, this.name);
            return size;
        }
    }


    public class HintMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "hmhd"; } }
        public ushort maxPDUsize { get; set; }
        public ushort avgPDUsize { get; set; }
        public uint maxbitrate { get; set; }
        public uint avgbitrate { get; set; }
        public uint reserved { get; set; } = 0;

        public HintMediaHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.maxPDUsize = IsoReaderWriter.ReadUInt16(stream);
            this.avgPDUsize = IsoReaderWriter.ReadUInt16(stream);
            this.maxbitrate = IsoReaderWriter.ReadUInt32(stream);
            this.avgbitrate = IsoReaderWriter.ReadUInt32(stream);
            this.reserved = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.maxPDUsize);
            size += IsoReaderWriter.WriteUInt16(stream, this.avgPDUsize);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxbitrate);
            size += IsoReaderWriter.WriteUInt32(stream, this.avgbitrate);
            size += IsoReaderWriter.WriteUInt32(stream, this.reserved);
            return size;
        }
    }


    public class ItemDataBox : Box
    {
        public override string FourCC { get { return "idat"; } }
        public byte[] data { get; set; }

        public ItemDataBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return size;
        }
    }


    public class ItemInfoBox : FullBox
    {
        public override string FourCC { get { return "iinf"; } }
        public uint entry_count { get; set; }
        public ItemInfoEntry[] item_infos { get; set; }

        public ItemInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.entry_count = IsoReaderWriter.ReadUInt32(stream);
            }
            this.item_infos = (ItemInfoEntry[])IsoReaderWriter.ReadClasses(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);
            }
            size += IsoReaderWriter.WriteClasses(stream, entry_count, this.item_infos);
            return size;
        }
    }


    public class ItemLocationBox : FullBox
    {
        public override string FourCC { get { return "iloc"; } }
        public byte offset_size { get; set; }
        public byte length_size { get; set; }
        public byte base_offset_size { get; set; }
        public byte index_size { get; set; }
        public ushort reserved { get; set; } = 0;
        public uint item_count { get; set; }
        public uint item_ID { get; set; }
        public byte construction_method { get; set; }
        public ushort data_reference_index { get; set; }
        public byte[] base_offset { get; set; }
        public ushort extent_count { get; set; }
        public byte[] item_reference_index { get; set; }
        public byte[] extent_offset { get; set; }
        public byte[] extent_length { get; set; }

        public ItemLocationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.offset_size = IsoReaderWriter.ReadBits(stream, 4);
            this.length_size = IsoReaderWriter.ReadBits(stream, 4);
            this.base_offset_size = IsoReaderWriter.ReadBits(stream, 4);

            if ((version == 1) || (version == 2))
            {
                this.index_size = IsoReaderWriter.ReadBits(stream, 4);
            }

            else
            {
                this.reserved = IsoReaderWriter.ReadBits(stream, 4);
            }

            if (version < 2)
            {
                this.item_count = IsoReaderWriter.ReadUInt16(stream);
            }

            else if (version == 2)
            {
                this.item_count = IsoReaderWriter.ReadUInt32(stream);
            }

            for (int i = 0; i < item_count; i++)
            {

                if (version < 2)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt16(stream);
                }

                else if (version == 2)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt32(stream);
                }

                if ((version == 1) || (version == 2))
                {
                    this.reserved = IsoReaderWriter.ReadBits(stream, 12);
                    this.construction_method = IsoReaderWriter.ReadBits(stream, 4);
                }
                this.data_reference_index = IsoReaderWriter.ReadUInt16(stream);
                this.base_offset = IsoReaderWriter.ReadBytes(stream, base_offset_size);
                this.extent_count = IsoReaderWriter.ReadUInt16(stream);

                for (int j = 0; j < extent_count; j++)
                {

                    if (((version == 1) || (version == 2)) && (index_size > 0))
                    {
                        this.item_reference_index = IsoReaderWriter.ReadBytes(stream, index_size);
                    }
                    this.extent_offset = IsoReaderWriter.ReadBytes(stream, offset_size);
                    this.extent_length = IsoReaderWriter.ReadBytes(stream, length_size);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 4, this.offset_size);
            size += IsoReaderWriter.WriteBits(stream, 4, this.length_size);
            size += IsoReaderWriter.WriteBits(stream, 4, this.base_offset_size);

            if ((version == 1) || (version == 2))
            {
                size += IsoReaderWriter.WriteBits(stream, 4, this.index_size);
            }

            else
            {
                size += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            }

            if (version < 2)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.item_count);
            }

            else if (version == 2)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.item_count);
            }

            for (int i = 0; i < item_count; i++)
            {

                if (version < 2)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else if (version == 2)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
                }

                if ((version == 1) || (version == 2))
                {
                    size += IsoReaderWriter.WriteBits(stream, 12, this.reserved);
                    size += IsoReaderWriter.WriteBits(stream, 4, this.construction_method);
                }
                size += IsoReaderWriter.WriteUInt16(stream, this.data_reference_index);
                size += IsoReaderWriter.WriteBytes(stream, base_offset_size, this.base_offset);
                size += IsoReaderWriter.WriteUInt16(stream, this.extent_count);

                for (int j = 0; j < extent_count; j++)
                {

                    if (((version == 1) || (version == 2)) && (index_size > 0))
                    {
                        size += IsoReaderWriter.WriteBytes(stream, index_size, this.item_reference_index);
                    }
                    size += IsoReaderWriter.WriteBytes(stream, offset_size, this.extent_offset);
                    size += IsoReaderWriter.WriteBytes(stream, length_size, this.extent_length);
                }
            }
            return size;
        }
    }


    public class IdentifiedMediaDataBox : Box
    {
        public override string FourCC { get { return "imda"; } }
        public uint imda_identifier { get; set; }
        public byte[] data { get; set; }  //  until the end of the box

        public IdentifiedMediaDataBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.imda_identifier = IsoReaderWriter.ReadUInt32(stream);
            this.data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.imda_identifier);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return size;
        }
    }


    public class ItemInfoEntry : FullBox
    {
        public override string FourCC { get { return "infe"; } }
        public uint item_ID { get; set; }
        public ushort item_protection_index { get; set; }
        public string item_name { get; set; }
        public string content_type { get; set; }
        public string content_encoding { get; set; }  // optional
        public uint extension_type { get; set; }  // optional
        public uint item_type { get; set; }
        public string item_uri_type { get; set; }

        public ItemInfoEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if ((version == 0) || (version == 1))
            {
                this.item_ID = IsoReaderWriter.ReadUInt16(stream);
                this.item_protection_index = IsoReaderWriter.ReadUInt16(stream);
                this.item_name = IsoReaderWriter.ReadString(stream);
                this.content_type = IsoReaderWriter.ReadString(stream);
                this.content_encoding = IsoReaderWriter.ReadString(stream);
            }

            if (version == 1)
            {
                this.extension_type = IsoReaderWriter.ReadUInt32(stream);
                // TODO: This should likely be a FullBox: ItemInfoExtensionextension_type; //optional

            }

            if (version >= 2)
            {

                if (version == 2)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt16(stream);
                }

                else if (version == 3)
                {
                    this.item_ID = IsoReaderWriter.ReadUInt32(stream);
                }
                this.item_protection_index = IsoReaderWriter.ReadUInt16(stream);
                this.item_type = IsoReaderWriter.ReadUInt32(stream);
                this.item_name = IsoReaderWriter.ReadString(stream);

                if (item_type == IsoReaderWriter.FromFourCC("mime"))
                {
                    this.content_type = IsoReaderWriter.ReadString(stream);
                    this.content_encoding = IsoReaderWriter.ReadString(stream);
                }

                else if (item_type == IsoReaderWriter.FromFourCC("uri "))
                {
                    this.item_uri_type = IsoReaderWriter.ReadString(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if ((version == 0) || (version == 1))
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                size += IsoReaderWriter.WriteUInt16(stream, this.item_protection_index);
                size += IsoReaderWriter.WriteString(stream, this.item_name);
                size += IsoReaderWriter.WriteString(stream, this.content_type);
                size += IsoReaderWriter.WriteString(stream, this.content_encoding);
            }

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.extension_type);
                // TODO: This should likely be a FullBox: ItemInfoExtensionextension_type; //optional

            }

            if (version >= 2)
            {

                if (version == 2)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
                }

                else if (version == 3)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
                }
                size += IsoReaderWriter.WriteUInt16(stream, this.item_protection_index);
                size += IsoReaderWriter.WriteUInt32(stream, this.item_type);
                size += IsoReaderWriter.WriteString(stream, this.item_name);

                if (item_type == IsoReaderWriter.FromFourCC("mime"))
                {
                    size += IsoReaderWriter.WriteString(stream, this.content_type);
                    size += IsoReaderWriter.WriteString(stream, this.content_encoding);
                }

                else if (item_type == IsoReaderWriter.FromFourCC("uri "))
                {
                    size += IsoReaderWriter.WriteString(stream, this.item_uri_type);
                }
            }
            return size;
        }
    }


    public class ItemProtectionBox : FullBox
    {
        public override string FourCC { get { return "ipro"; } }
        public ushort protection_count { get; set; }
        public ProtectionSchemeInfoBox protection_information { get; set; }

        public ItemProtectionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.protection_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 1; i <= protection_count; i++)
            {
                this.protection_information = (ProtectionSchemeInfoBox)IsoReaderWriter.ReadBox(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.protection_count);

            for (int i = 1; i <= protection_count; i++)
            {
                size += IsoReaderWriter.WriteBox(stream, this.protection_information);
            }
            return size;
        }
    }


    public class ItemReferenceBox : FullBox
    {
        public override string FourCC { get { return "iref"; } }
        public SingleItemTypeReferenceBoxLarge[] references { get; set; }

        public ItemReferenceBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.references = (SingleItemTypeReferenceBox)IsoReaderWriter.ReadBox(stream);
            }

            else if (version == 1)
            {
                this.references = (SingleItemTypeReferenceBoxLarge[])IsoReaderWriter.ReadBoxes(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteBox(stream, this.references);
            }

            else if (version == 1)
            {
                size += IsoReaderWriter.WriteBoxes(stream, this.references);
            }
            return size;
        }
    }


    public class LevelAssignmentBox : FullBox
    {
        public override string FourCC { get { return "leva"; } }
        public byte level_count { get; set; }
        public uint track_ID { get; set; }
        public bool padding_flag { get; set; }
        public byte assignment_type { get; set; }
        public uint grouping_type { get; set; }
        public uint grouping_type_parameter { get; set; }
        public uint sub_track_ID { get; set; }

        public LevelAssignmentBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.level_count = IsoReaderWriter.ReadUInt8(stream);

            for (int j = 1; j <= level_count; j++)
            {
                this.track_ID = IsoReaderWriter.ReadUInt32(stream);
                this.padding_flag = IsoReaderWriter.ReadBit(stream);
                this.assignment_type = IsoReaderWriter.ReadBits(stream, 7);

                if (assignment_type == 0)
                {
                    this.grouping_type = IsoReaderWriter.ReadUInt32(stream);
                }

                else if (assignment_type == 1)
                {
                    this.grouping_type = IsoReaderWriter.ReadUInt32(stream);
                    this.grouping_type_parameter = IsoReaderWriter.ReadUInt32(stream);
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
                    this.sub_track_ID = IsoReaderWriter.ReadUInt32(stream);
                }
                /*  other assignment_type values are reserved */
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.level_count);

            for (int j = 1; j <= level_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
                size += IsoReaderWriter.WriteBit(stream, this.padding_flag);
                size += IsoReaderWriter.WriteBits(stream, 7, this.assignment_type);

                if (assignment_type == 0)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);
                }

                else if (assignment_type == 1)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);
                    size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
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
                    size += IsoReaderWriter.WriteUInt32(stream, this.sub_track_ID);
                }
                /*  other assignment_type values are reserved */
            }
            return size;
        }
    }


    public class MediaDataBox : Box
    {
        public override string FourCC { get { return "mdat"; } }
        public byte[] data { get; set; }

        public MediaDataBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return size;
        }
    }


    public class MediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "mdhd"; } }
        public ulong creation_time { get; set; }
        public ulong modification_time { get; set; }
        public uint timescale { get; set; }
        public ulong duration { get; set; }
        public bool pad { get; set; } = false;
        public byte[] language { get; set; }  //  ISO-639-2/T language code
        public ushort pre_defined { get; set; } = 0;

        public MediaHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 1)
            {
                this.creation_time = IsoReaderWriter.ReadUInt64(stream);
                this.modification_time = IsoReaderWriter.ReadUInt64(stream);
                this.timescale = IsoReaderWriter.ReadUInt32(stream);
                this.duration = IsoReaderWriter.ReadUInt64(stream);
            }

            else
            {
                /*  version==0 */
                this.creation_time = IsoReaderWriter.ReadUInt32(stream);
                this.modification_time = IsoReaderWriter.ReadUInt32(stream);
                this.timescale = IsoReaderWriter.ReadUInt32(stream);
                this.duration = IsoReaderWriter.ReadUInt32(stream);
            }
            this.pad = IsoReaderWriter.ReadBit(stream);
            this.language = IsoReaderWriter.ReadBitsArray(stream, 5, 3);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
                size += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
                size += IsoReaderWriter.WriteUInt64(stream, this.duration);
            }

            else
            {
                /*  version==0 */
                size += IsoReaderWriter.WriteUInt32(stream, this.creation_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.modification_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
                size += IsoReaderWriter.WriteUInt32(stream, this.duration);
            }
            size += IsoReaderWriter.WriteBit(stream, this.pad);
            size += IsoReaderWriter.WriteBitsArray(stream, 5, 3, this.language);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            return size;
        }
    }


    public class MediaBox : Box
    {
        public override string FourCC { get { return "mdia"; } }

        public MediaBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class MovieExtendsHeaderBox : FullBox
    {
        public override string FourCC { get { return "mehd"; } }
        public ulong fragment_duration { get; set; }

        public MovieExtendsHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 1)
            {
                this.fragment_duration = IsoReaderWriter.ReadUInt64(stream);
            }

            else
            {
                /*  version==0 */
                this.fragment_duration = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.fragment_duration);
            }

            else
            {
                /*  version==0 */
                size += IsoReaderWriter.WriteUInt32(stream, this.fragment_duration);
            }
            return size;
        }
    }


    public class MetaBox : FullBox
    {
        public override string FourCC { get { return "meta"; } }
        public HandlerBox theHandler { get; set; }
        public PrimaryItemBox primary_resource { get; set; }  //  optional
        public DataInformationBox file_locations { get; set; }  //  optional
        public ItemLocationBox item_locations { get; set; }  //  optional
        public ItemProtectionBox protections { get; set; }  //  optional
        public ItemInfoBox item_infos { get; set; }  //  optional
        public IPMPControlBox IPMP_control { get; set; }  //  optional
        public ItemReferenceBox item_refs { get; set; }  //  optional
        public ItemDataBox item_data { get; set; }  //  optional
        public Box[] other_boxes { get; set; }  //  optional

        public MetaBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.theHandler = (HandlerBox)IsoReaderWriter.ReadBox(stream);
            this.primary_resource = (PrimaryItemBox)IsoReaderWriter.ReadBox(stream);
            this.file_locations = (DataInformationBox)IsoReaderWriter.ReadBox(stream);
            this.item_locations = (ItemLocationBox)IsoReaderWriter.ReadBox(stream);
            this.protections = (ItemProtectionBox)IsoReaderWriter.ReadBox(stream);
            this.item_infos = (ItemInfoBox)IsoReaderWriter.ReadBox(stream);
            this.IPMP_control = (IPMPControlBox)IsoReaderWriter.ReadBox(stream);
            this.item_refs = (ItemReferenceBox)IsoReaderWriter.ReadBox(stream);
            this.item_data = (ItemDataBox)IsoReaderWriter.ReadBox(stream);
            this.other_boxes = IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.theHandler);
            size += IsoReaderWriter.WriteBox(stream, this.primary_resource);
            size += IsoReaderWriter.WriteBox(stream, this.file_locations);
            size += IsoReaderWriter.WriteBox(stream, this.item_locations);
            size += IsoReaderWriter.WriteBox(stream, this.protections);
            size += IsoReaderWriter.WriteBox(stream, this.item_infos);
            size += IsoReaderWriter.WriteBox(stream, this.IPMP_control);
            size += IsoReaderWriter.WriteBox(stream, this.item_refs);
            size += IsoReaderWriter.WriteBox(stream, this.item_data);
            size += IsoReaderWriter.WriteBoxes(stream, this.other_boxes);
            return size;
        }
    }


    public class MovieFragmentHeaderBox : FullBox
    {
        public override string FourCC { get { return "mfhd"; } }
        public uint sequence_number { get; set; }

        public MovieFragmentHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sequence_number = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.sequence_number);
            return size;
        }
    }


    public class MovieFragmentRandomAccessBox : Box
    {
        public override string FourCC { get { return "mfra"; } }

        public MovieFragmentRandomAccessBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class MovieFragmentRandomAccessOffsetBox : FullBox
    {
        public override string FourCC { get { return "mfro"; } }
        public uint parent_size { get; set; }

        public MovieFragmentRandomAccessOffsetBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.parent_size = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.parent_size);
            return size;
        }
    }


    public class MediaInformationBox : Box
    {
        public override string FourCC { get { return "minf"; } }

        public MediaInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class CompressedMovieFragmentBox : CompressedBox
    {
        public override string FourCC { get { return "moof"; } }

        public CompressedMovieFragmentBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class CompressedMovieBox : CompressedBox
    {
        public override string FourCC { get { return "moov"; } }

        public CompressedMovieBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class MovieExtendsBox : Box
    {
        public override string FourCC { get { return "mvex"; } }

        public MovieExtendsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class MovieHeaderBox : FullBox
    {
        public override string FourCC { get { return "mvhd"; } }
        public ulong creation_time { get; set; }
        public ulong modification_time { get; set; }
        public uint timescale { get; set; }
        public ulong duration { get; set; }
        public int rate { get; set; } = 0x00010000;  //  typically 1.0
        public short volume { get; set; } = 0x0100;  //  typically, full volume
        public uint[] reserved { get; set; } = [];
        public uint[] matrix { get; set; } =
            { 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };  //  Unity matrix
        public uint[] pre_defined { get; set; } = [];
        public uint next_track_ID { get; set; }

        public MovieHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 1)
            {
                this.creation_time = IsoReaderWriter.ReadUInt64(stream);
                this.modification_time = IsoReaderWriter.ReadUInt64(stream);
                this.timescale = IsoReaderWriter.ReadUInt32(stream);
                this.duration = IsoReaderWriter.ReadUInt64(stream);
            }

            else
            {
                /*  version==0 */
                this.creation_time = IsoReaderWriter.ReadUInt32(stream);
                this.modification_time = IsoReaderWriter.ReadUInt32(stream);
                this.timescale = IsoReaderWriter.ReadUInt32(stream);
                this.duration = IsoReaderWriter.ReadUInt32(stream);
            }
            this.rate = IsoReaderWriter.ReadInt32(stream);
            this.volume = IsoReaderWriter.ReadInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt32Array(stream, 2);
            this.matrix = IsoReaderWriter.ReadUInt32Array(stream, 9);
            this.pre_defined = IsoReaderWriter.ReadUInt32Array(stream, 6);
            this.next_track_ID = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
                size += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
                size += IsoReaderWriter.WriteUInt64(stream, this.duration);
            }

            else
            {
                /*  version==0 */
                size += IsoReaderWriter.WriteUInt32(stream, this.creation_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.modification_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.timescale);
                size += IsoReaderWriter.WriteUInt32(stream, this.duration);
            }
            size += IsoReaderWriter.WriteInt32(stream, this.rate);
            size += IsoReaderWriter.WriteInt16(stream, this.volume);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteUInt32Array(stream, 9, this.matrix);
            size += IsoReaderWriter.WriteUInt32Array(stream, 6, this.pre_defined);
            size += IsoReaderWriter.WriteUInt32(stream, this.next_track_ID);
            return size;
        }
    }


    public class NullMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "nmhd"; } }

        public NullMediaHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class OriginalFileTypeBox : Box
    {
        public override string FourCC { get { return "otyp"; } }

        public OriginalFileTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class PaddingBitsBox : FullBox
    {
        public override string FourCC { get { return "padb"; } }
        public uint sample_count { get; set; }
        public bool reserved { get; set; } = false;
        public byte pad1 { get; set; }
        public byte pad2 { get; set; }

        public PaddingBitsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sample_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 0; i < ((sample_count + 1) / 2); i++)
            {
                this.reserved = IsoReaderWriter.ReadBit(stream);
                this.pad1 = IsoReaderWriter.ReadBits(stream, 3);
                this.reserved = IsoReaderWriter.ReadBit(stream);
                this.pad2 = IsoReaderWriter.ReadBits(stream, 3);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);


            for (int i = 0; i < ((sample_count + 1) / 2); i++)
            {
                size += IsoReaderWriter.WriteBit(stream, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 3, this.pad1);
                size += IsoReaderWriter.WriteBit(stream, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 3, this.pad2);
            }
            return size;
        }
    }


    public class PartitionEntry1 : Box
    {
        public override string FourCC { get { return "paen"; } }
        public FilePartitionBox blocks_and_symbols { get; set; }
        public FECReservoirBox FEC_symbol_locations { get; set; }  // optional
        public FileReservoirBox File_symbol_locations { get; set; }  // optional

        public PartitionEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.blocks_and_symbols = (FilePartitionBox)IsoReaderWriter.ReadBox(stream);
            this.FEC_symbol_locations = (FECReservoirBox)IsoReaderWriter.ReadBox(stream);
            this.File_symbol_locations = (FileReservoirBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.blocks_and_symbols);
            size += IsoReaderWriter.WriteBox(stream, this.FEC_symbol_locations);
            size += IsoReaderWriter.WriteBox(stream, this.File_symbol_locations);
            return size;
        }
    }


    public class FDItemInformationBox1 : FullBox
    {
        public override string FourCC { get { return "paen"; } }
        public ushort entry_count { get; set; }
        public PartitionEntry[] partition_entries { get; set; }
        public FDSessionGroupBox session_info { get; set; }  // optional
        public GroupIdToNameBox group_id_to_name { get; set; }  // optional

        public FDItemInformationBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            this.partition_entries = (PartitionEntry[])IsoReaderWriter.ReadClasses(stream, entry_count);
            this.session_info = (FDSessionGroupBox)IsoReaderWriter.ReadBox(stream);
            this.group_id_to_name = (GroupIdToNameBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            size += IsoReaderWriter.WriteClasses(stream, entry_count, this.partition_entries);
            size += IsoReaderWriter.WriteBox(stream, this.session_info);
            size += IsoReaderWriter.WriteBox(stream, this.group_id_to_name);
            return size;
        }
    }


    public class ProgressiveDownloadInfoBox : FullBox
    {
        public override string FourCC { get { return "pdin"; } }
        public uint rate { get; set; }
        public uint initial_delay { get; set; }

        public ProgressiveDownloadInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            for (int i = 0; ; i++)
            {
                /*  to end of box */
                this.rate = IsoReaderWriter.ReadUInt32(stream);
                this.initial_delay = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            for (int i = 0; ; i++)
            {
                /*  to end of box */
                size += IsoReaderWriter.WriteUInt32(stream, this.rate);
                size += IsoReaderWriter.WriteUInt32(stream, this.initial_delay);
            }
            return size;
        }
    }


    public class PrimaryItemBox : FullBox
    {
        public override string FourCC { get { return "pitm"; } }
        public uint item_ID { get; set; }

        public PrimaryItemBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 0)
            {
                this.item_ID = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.item_ID);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.item_ID);
            }
            return size;
        }
    }


    public class ProducerReferenceTimeBox : FullBox
    {
        public override string FourCC { get { return "prft"; } }
        public uint reference_track_ID { get; set; }
        public ulong ntp_timestamp { get; set; }
        public ulong media_time { get; set; }

        public ProducerReferenceTimeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reference_track_ID = IsoReaderWriter.ReadUInt32(stream);
            this.ntp_timestamp = IsoReaderWriter.ReadUInt64(stream);

            if (version == 0)
            {
                this.media_time = IsoReaderWriter.ReadUInt32(stream);
            }

            else
            {
                this.media_time = IsoReaderWriter.ReadUInt64(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.reference_track_ID);
            size += IsoReaderWriter.WriteUInt64(stream, this.ntp_timestamp);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.media_time);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.media_time);
            }
            return size;
        }
    }


    public class RestrictedSchemeInfoBox : Box
    {
        public override string FourCC { get { return "rinf"; } }
        public OriginalFormatBox original_format { get; set; }
        public SchemeTypeBox scheme_type_box { get; set; }
        public SchemeInformationBox info { get; set; }  //  optional

        public RestrictedSchemeInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.original_format = (OriginalFormatBox)IsoReaderWriter.ReadBox(stream);
            this.scheme_type_box = (SchemeTypeBox)IsoReaderWriter.ReadBox(stream);
            this.info = (SchemeInformationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.original_format);
            size += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            size += IsoReaderWriter.WriteBox(stream, this.info);
            return size;
        }
    }


    public class SampleAuxiliaryInformationOffsetsBox : FullBox
    {
        public override string FourCC { get { return "saio"; } }
        public uint aux_info_type { get; set; }
        public uint aux_info_type_parameter { get; set; }
        public uint entry_count { get; set; }
        public ulong[] offset { get; set; }

        public SampleAuxiliaryInformationOffsetsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if ((flags & 1) == 1)
            {
                this.aux_info_type = IsoReaderWriter.ReadUInt32(stream);
                this.aux_info_type_parameter = IsoReaderWriter.ReadUInt32(stream);
            }
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            if (version == 0)
            {
                this.offset = IsoReaderWriter.ReadUInt32(stream);
            }

            else
            {
                this.offset = IsoReaderWriter.ReadUInt64Array(stream, entry_count);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if ((flags & 1) == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type);
                size += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type_parameter);
            }
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            if (version == 0)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.offset);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt64Array(stream, entry_count, this.offset);
            }
            return size;
        }
    }


    public class SampleAuxiliaryInformationSizesBox : FullBox
    {
        public override string FourCC { get { return "saiz"; } }
        public uint aux_info_type { get; set; }
        public uint aux_info_type_parameter { get; set; }
        public byte default_sample_info_size { get; set; }
        public uint sample_count { get; set; }
        public byte[] sample_info_size { get; set; }

        public SampleAuxiliaryInformationSizesBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if ((flags & 1) == 1)
            {
                this.aux_info_type = IsoReaderWriter.ReadUInt32(stream);
                this.aux_info_type_parameter = IsoReaderWriter.ReadUInt32(stream);
            }
            this.default_sample_info_size = IsoReaderWriter.ReadUInt8(stream);
            this.sample_count = IsoReaderWriter.ReadUInt32(stream);

            if (default_sample_info_size == 0)
            {
                this.sample_info_size[sample_count] = IsoReaderWriter.ReadBytes(stream, sample_count);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if ((flags & 1) == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type);
                size += IsoReaderWriter.WriteUInt32(stream, this.aux_info_type_parameter);
            }
            size += IsoReaderWriter.WriteUInt8(stream, this.default_sample_info_size);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);

            if (default_sample_info_size == 0)
            {
                size += IsoReaderWriter.WriteBytes(stream, sample_count, this.sample_info_size[sample_count]);
            }
            return size;
        }
    }


    public class SampleToGroupBox : FullBox
    {
        public override string FourCC { get { return "sbgp"; } }
        public uint grouping_type { get; set; }
        public uint grouping_type_parameter { get; set; }
        public uint entry_count { get; set; }
        public uint sample_count { get; set; }
        public uint group_description_index { get; set; }

        public SampleToGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.grouping_type = IsoReaderWriter.ReadUInt32(stream);

            if (version == 1)
            {
                this.grouping_type_parameter = IsoReaderWriter.ReadUInt32(stream);
            }
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.sample_count = IsoReaderWriter.ReadUInt32(stream);
                this.group_description_index = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
            }
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                size += IsoReaderWriter.WriteUInt32(stream, this.group_description_index);
            }
            return size;
        }
    }


    public class SchemeInformationBox : Box
    {
        public override string FourCC { get { return "schi"; } }
        public Box[] scheme_specific_data { get; set; }

        public SchemeInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scheme_specific_data = IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.scheme_specific_data);
            return size;
        }
    }


    public class SchemeTypeBox : FullBox
    {
        public override string FourCC { get { return "schm"; } }
        public uint scheme_type { get; set; }  //  4CC identifying the scheme
        public uint scheme_version { get; set; }  //  scheme version
        public string scheme_uri { get; set; }  //  browser uri

        public SchemeTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scheme_type = IsoReaderWriter.ReadUInt32(stream);
            this.scheme_version = IsoReaderWriter.ReadUInt32(stream);

            if ((flags & 0x000001) == 0x000001)
            {
                this.scheme_uri = IsoReaderWriter.ReadString(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.scheme_type);
            size += IsoReaderWriter.WriteUInt32(stream, this.scheme_version);

            if ((flags & 0x000001) == 0x000001)
            {
                size += IsoReaderWriter.WriteString(stream, this.scheme_uri);
            }
            return size;
        }
    }


    public class CompatibleSchemeTypeBox : FullBox
    {
        public override string FourCC { get { return "csch"; } }
        public uint scheme_type { get; set; }  //  4CC identifying the scheme
        public uint scheme_version { get; set; }  //  scheme version 
        public string scheme_uri { get; set; }  //  browser uri

        public CompatibleSchemeTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            /*  identical syntax to SchemeTypeBox */
            this.scheme_type = IsoReaderWriter.ReadUInt32(stream);
            this.scheme_version = IsoReaderWriter.ReadUInt32(stream);

            if ((flags & 0x000001) == 0x000001)
            {
                this.scheme_uri = IsoReaderWriter.ReadString(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            /*  identical syntax to SchemeTypeBox */
            size += IsoReaderWriter.WriteUInt32(stream, this.scheme_type);
            size += IsoReaderWriter.WriteUInt32(stream, this.scheme_version);

            if ((flags & 0x000001) == 0x000001)
            {
                size += IsoReaderWriter.WriteString(stream, this.scheme_uri);
            }
            return size;
        }
    }


    public class SampleDependencyTypeBox : FullBox
    {
        public override string FourCC { get { return "sdtp"; } }
        public byte is_leading { get; set; }
        public byte sample_depends_on { get; set; }
        public byte sample_is_depended_on { get; set; }
        public byte sample_has_redundancy { get; set; }

        public SampleDependencyTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            for (int i = 0; i < sample_count; i++)
            {
                this.is_leading = IsoReaderWriter.ReadBits(stream, 2);
                this.sample_depends_on = IsoReaderWriter.ReadBits(stream, 2);
                this.sample_is_depended_on = IsoReaderWriter.ReadBits(stream, 2);
                this.sample_has_redundancy = IsoReaderWriter.ReadBits(stream, 2);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            for (int i = 0; i < sample_count; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 2, this.is_leading);
                size += IsoReaderWriter.WriteBits(stream, 2, this.sample_depends_on);
                size += IsoReaderWriter.WriteBits(stream, 2, this.sample_is_depended_on);
                size += IsoReaderWriter.WriteBits(stream, 2, this.sample_has_redundancy);
            }
            return size;
        }
    }


    public class FDSessionGroupBox : Box
    {
        public override string FourCC { get { return "segr"; } }
        public ushort num_session_groups { get; set; }
        public byte entry_count { get; set; }
        public uint group_ID { get; set; }
        public ushort num_channels_in_session_group { get; set; }
        public uint hint_track_ID { get; set; }

        public FDSessionGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_session_groups = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_session_groups; i++)
            {
                this.entry_count = IsoReaderWriter.ReadUInt8(stream);

                for (int j = 0; j < entry_count; j++)
                {
                    this.group_ID = IsoReaderWriter.ReadUInt32(stream);
                }
                this.num_channels_in_session_group = IsoReaderWriter.ReadUInt16(stream);

                for (int k = 0; k < num_channels_in_session_group; k++)
                {
                    this.hint_track_ID = IsoReaderWriter.ReadUInt32(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_session_groups);

            for (int i = 0; i < num_session_groups; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.entry_count);

                for (int j = 0; j < entry_count; j++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.group_ID);
                }
                size += IsoReaderWriter.WriteUInt16(stream, this.num_channels_in_session_group);

                for (int k = 0; k < num_channels_in_session_group; k++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.hint_track_ID);
                }
            }
            return size;
        }
    }


    public class SampleGroupDescriptionBox : FullBox
    {
        public override string FourCC { get { return "sgpd"; } }
        public uint grouping_type { get; set; }
        public uint default_length { get; set; }
        public uint default_group_description_index { get; set; }
        public uint entry_count { get; set; }
        public uint description_length { get; set; }

        public SampleGroupDescriptionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.grouping_type = IsoReaderWriter.ReadUInt32(stream);

            if (version >= 1)
            {
                this.default_length = IsoReaderWriter.ReadUInt32(stream);
            }

            if (version >= 2)
            {
                this.default_group_description_index = IsoReaderWriter.ReadUInt32(stream);
            }
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 1; i <= entry_count; i++)
            {

                if (version >= 1)
                {

                    if (default_length == 0)
                    {
                        this.description_length = IsoReaderWriter.ReadUInt32(stream);
                    }
                }
                // TODO: This should likely be a FullBox: SampleGroupDescriptionEntrygrouping_type; // an instance of a class derived from SampleGroupDescriptionEntry

                /*   that is appropriate and permitted for the media type */
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

            if (version >= 1)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.default_length);
            }

            if (version >= 2)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.default_group_description_index);
            }
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 1; i <= entry_count; i++)
            {

                if (version >= 1)
                {

                    if (default_length == 0)
                    {
                        size += IsoReaderWriter.WriteUInt32(stream, this.description_length);
                    }
                }
                // TODO: This should likely be a FullBox: SampleGroupDescriptionEntrygrouping_type; // an instance of a class derived from SampleGroupDescriptionEntry

                /*   that is appropriate and permitted for the media type */
            }
            return size;
        }
    }


    public class CompressedSegmentIndexBox : CompressedBox
    {
        public override string FourCC { get { return "sidx"; } }

        public CompressedSegmentIndexBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class ProtectionSchemeInfoBox : Box
    {
        public override string FourCC { get { return "sinf"; } }
        public OriginalFormatBox original_format { get; set; }
        public SchemeTypeBox scheme_type_box { get; set; }  //  optional
        public SchemeInformationBox info { get; set; }  //  optional

        public ProtectionSchemeInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.original_format = (OriginalFormatBox)IsoReaderWriter.ReadBox(stream);
            this.scheme_type_box = (SchemeTypeBox)IsoReaderWriter.ReadBox(stream);
            this.info = (SchemeInformationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.original_format);
            size += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            size += IsoReaderWriter.WriteBox(stream, this.info);
            return size;
        }
    }


    public class FreeSpaceBox1 : Box
    {
        public override string FourCC { get { return "skip"; } }
        public byte[] data { get; set; }

        public FreeSpaceBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.data = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.data);
            return size;
        }
    }


    public class SoundMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "smhd"; } }
        public short balance { get; set; } = 0;
        public ushort reserved { get; set; } = 0;

        public SoundMediaHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.balance = IsoReaderWriter.ReadInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt16(stream, this.balance);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            return size;
        }
    }


    public class SRTPProcessBox : FullBox
    {
        public override string FourCC { get { return "srpp"; } }
        public uint encryption_algorithm_rtp { get; set; }
        public uint encryption_algorithm_rtcp { get; set; }
        public uint integrity_algorithm_rtp { get; set; }
        public uint integrity_algorithm_rtcp { get; set; }
        public SchemeTypeBox scheme_type_box { get; set; }
        public SchemeInformationBox info { get; set; }

        public SRTPProcessBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.encryption_algorithm_rtp = IsoReaderWriter.ReadUInt32(stream);
            this.encryption_algorithm_rtcp = IsoReaderWriter.ReadUInt32(stream);
            this.integrity_algorithm_rtp = IsoReaderWriter.ReadUInt32(stream);
            this.integrity_algorithm_rtcp = IsoReaderWriter.ReadUInt32(stream);
            this.scheme_type_box = (SchemeTypeBox)IsoReaderWriter.ReadBox(stream);
            this.info = (SchemeInformationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.encryption_algorithm_rtp);
            size += IsoReaderWriter.WriteUInt32(stream, this.encryption_algorithm_rtcp);
            size += IsoReaderWriter.WriteUInt32(stream, this.integrity_algorithm_rtp);
            size += IsoReaderWriter.WriteUInt32(stream, this.integrity_algorithm_rtcp);
            size += IsoReaderWriter.WriteBox(stream, this.scheme_type_box);
            size += IsoReaderWriter.WriteBox(stream, this.info);
            return size;
        }
    }


    public class CompressedSubsegmentIndexBox : CompressedBox
    {
        public override string FourCC { get { return "ssix"; } }

        public CompressedSubsegmentIndexBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class SampleTableBox : Box
    {
        public override string FourCC { get { return "stbl"; } }

        public SampleTableBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class ChunkOffsetBox : FullBox
    {
        public override string FourCC { get { return "stco"; } }
        public uint entry_count { get; set; }
        public uint chunk_offset { get; set; }

        public ChunkOffsetBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.chunk_offset = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.chunk_offset);
            }
            return size;
        }
    }


    public class DegradationPriorityBox : FullBox
    {
        public override string FourCC { get { return "stdp"; } }
        public ushort priority { get; set; }

        public DegradationPriorityBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);


            for (int i = 0; i < sample_count; i++)
            {
                this.priority = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);


            for (int i = 0; i < sample_count; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.priority);
            }
            return size;
        }
    }


    public class SubtitleMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "sthd"; } }

        public SubtitleMediaHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class SubTrackDefinitionBox : Box
    {
        public override string FourCC { get { return "strd"; } }

        public SubTrackDefinitionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class SubTrackInformationBox : FullBox
    {
        public override string FourCC { get { return "stri"; } }
        public short switch_group { get; set; } = 0;
        public short alternate_group { get; set; } = 0;
        public uint sub_track_ID { get; set; } = 0;
        public uint[] attribute_list { get; set; }  //  to the end of the box

        public SubTrackInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.switch_group = IsoReaderWriter.ReadInt16(stream);
            this.alternate_group = IsoReaderWriter.ReadInt16(stream);
            this.sub_track_ID = IsoReaderWriter.ReadUInt32(stream);
            this.attribute_list = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt16(stream, this.switch_group);
            size += IsoReaderWriter.WriteInt16(stream, this.alternate_group);
            size += IsoReaderWriter.WriteUInt32(stream, this.sub_track_ID);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.attribute_list);
            return size;
        }
    }


    public class SampleToChunkBox : FullBox
    {
        public override string FourCC { get { return "stsc"; } }
        public uint entry_count { get; set; }
        public uint first_chunk { get; set; }
        public uint samples_per_chunk { get; set; }
        public uint sample_description_index { get; set; }

        public SampleToChunkBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.first_chunk = IsoReaderWriter.ReadUInt32(stream);
                this.samples_per_chunk = IsoReaderWriter.ReadUInt32(stream);
                this.sample_description_index = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.first_chunk);
                size += IsoReaderWriter.WriteUInt32(stream, this.samples_per_chunk);
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_description_index);
            }
            return size;
        }
    }


    public class SampleDescriptionBox : FullBox
    {
        public override string FourCC { get { return "stsd"; } }
        public uint entry_count { get; set; }

        public SampleDescriptionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            this.entry_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                // TODO: This should likely be a FullBox: SampleEntry; // an instance of a class derived from SampleEntry

            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                // TODO: This should likely be a FullBox: SampleEntry; // an instance of a class derived from SampleEntry

            }
            return size;
        }
    }


    public class SubTrackSampleGroupBox : FullBox
    {
        public override string FourCC { get { return "stsg"; } }
        public uint grouping_type { get; set; }
        public ushort item_count { get; set; }
        public uint group_description_index { get; set; }

        public SubTrackSampleGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.grouping_type = IsoReaderWriter.ReadUInt32(stream);
            this.item_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < item_count; i++)
            {
                this.group_description_index = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);
            size += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.group_description_index);
            }
            return size;
        }
    }


    public class ShadowSyncSampleBox : FullBox
    {
        public override string FourCC { get { return "stsh"; } }
        public uint entry_count { get; set; }
        public uint shadowed_sample_number { get; set; }
        public uint sync_sample_number { get; set; }

        public ShadowSyncSampleBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 0; i < entry_count; i++)
            {
                this.shadowed_sample_number = IsoReaderWriter.ReadUInt32(stream);
                this.sync_sample_number = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.shadowed_sample_number);
                size += IsoReaderWriter.WriteUInt32(stream, this.sync_sample_number);
            }
            return size;
        }
    }


    public class SyncSampleBox : FullBox
    {
        public override string FourCC { get { return "stss"; } }
        public uint entry_count { get; set; }
        public uint sample_number { get; set; }

        public SyncSampleBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 0; i < entry_count; i++)
            {
                this.sample_number = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_number);
            }
            return size;
        }
    }


    public class SampleSizeBox : FullBox
    {
        public override string FourCC { get { return "stsz"; } }
        public uint sample_size { get; set; }
        public uint sample_count { get; set; }
        public uint entry_size { get; set; }

        public SampleSizeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sample_size = IsoReaderWriter.ReadUInt32(stream);
            this.sample_count = IsoReaderWriter.ReadUInt32(stream);

            if (sample_size == 0)
            {

                for (int i = 1; i <= sample_count; i++)
                {
                    this.entry_size = IsoReaderWriter.ReadUInt32(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_size);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);

            if (sample_size == 0)
            {

                for (int i = 1; i <= sample_count; i++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.entry_size);
                }
            }
            return size;
        }
    }


    public class TimeToSampleBox : FullBox
    {
        public override string FourCC { get { return "stts"; } }
        public uint entry_count { get; set; }
        public uint sample_count { get; set; }
        public uint sample_delta { get; set; }

        public TimeToSampleBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 0; i < entry_count; i++)
            {
                this.sample_count = IsoReaderWriter.ReadUInt32(stream);
                this.sample_delta = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_delta);
            }
            return size;
        }
    }


    public class SegmentTypeBox : GeneralTypeBox
    {
        public override string FourCC { get { return "styp"; } }

        public SegmentTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class CompactSampleSizeBox : FullBox
    {
        public override string FourCC { get { return "stz2"; } }
        public uint reserved { get; set; } = 0;
        public byte field_size { get; set; }
        public uint sample_count { get; set; }
        public byte[] entry_size { get; set; }

        public CompactSampleSizeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadUInt24(stream);
            this.field_size = IsoReaderWriter.ReadUInt8(stream);
            this.sample_count = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= sample_count; i++)
            {
                this.entry_size = IsoReaderWriter.ReadBytes(stream, field_size);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt24(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt8(stream, this.field_size);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);

            for (int i = 1; i <= sample_count; i++)
            {
                size += IsoReaderWriter.WriteBytes(stream, field_size, this.entry_size);
            }
            return size;
        }
    }


    public class SubSampleInformationBox : FullBox
    {
        public override string FourCC { get { return "subs"; } }
        public uint entry_count { get; set; }
        public uint sample_delta { get; set; }
        public ushort subsample_count { get; set; }
        public uint subsample_size { get; set; }
        public byte subsample_priority { get; set; }
        public byte discardable { get; set; }
        public uint codec_specific_parameters { get; set; }

        public SubSampleInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 0; i < entry_count; i++)
            {
                this.sample_delta = IsoReaderWriter.ReadUInt32(stream);
                this.subsample_count = IsoReaderWriter.ReadUInt16(stream);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            this.subsample_size = IsoReaderWriter.ReadUInt32(stream);
                        }

                        else
                        {
                            this.subsample_size = IsoReaderWriter.ReadUInt16(stream);
                        }
                        this.subsample_priority = IsoReaderWriter.ReadUInt8(stream);
                        this.discardable = IsoReaderWriter.ReadUInt8(stream);
                        this.codec_specific_parameters = IsoReaderWriter.ReadUInt32(stream);
                    }
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_delta);
                size += IsoReaderWriter.WriteUInt16(stream, this.subsample_count);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            size += IsoReaderWriter.WriteUInt32(stream, this.subsample_size);
                        }

                        else
                        {
                            size += IsoReaderWriter.WriteUInt16(stream, this.subsample_size);
                        }
                        size += IsoReaderWriter.WriteUInt8(stream, this.subsample_priority);
                        size += IsoReaderWriter.WriteUInt8(stream, this.discardable);
                        size += IsoReaderWriter.WriteUInt32(stream, this.codec_specific_parameters);
                    }
                }
            }
            return size;
        }
    }


    public class TrackFragmentBaseMediaDecodeTimeBox : FullBox
    {
        public override string FourCC { get { return "tfdt"; } }
        public ulong baseMediaDecodeTime { get; set; }

        public TrackFragmentBaseMediaDecodeTimeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 1)
            {
                this.baseMediaDecodeTime = IsoReaderWriter.ReadUInt64(stream);
            }

            else
            {
                /*  version==0 */
                this.baseMediaDecodeTime = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.baseMediaDecodeTime);
            }

            else
            {
                /*  version==0 */
                size += IsoReaderWriter.WriteUInt32(stream, this.baseMediaDecodeTime);
            }
            return size;
        }
    }


    public class TrackFragmentHeaderBox : FullBox
    {
        public override string FourCC { get { return "tfhd"; } }
        public uint track_ID { get; set; }  //  all the following are optional fields
        public ulong base_data_offset { get; set; }
        public uint sample_description_index { get; set; }
        public uint default_sample_duration { get; set; }
        public uint default_sample_size { get; set; }
        public uint default_sample_flags { get; set; }

        public TrackFragmentHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_ID = IsoReaderWriter.ReadUInt32(stream);
            /*  their presence is indicated by bits in the tf_flags */
            this.base_data_offset = IsoReaderWriter.ReadUInt64(stream);
            this.sample_description_index = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_duration = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_size = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_flags = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
            /*  their presence is indicated by bits in the tf_flags */
            size += IsoReaderWriter.WriteUInt64(stream, this.base_data_offset);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_description_index);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_duration);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_size);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_flags);
            return size;
        }
    }


    public class TrackFragmentRandomAccessBox : FullBox
    {
        public override string FourCC { get { return "tfra"; } }
        public uint track_ID { get; set; }
        public uint reserved { get; set; } = 0;
        public byte length_size_of_traf_num { get; set; }
        public byte length_size_of_trun_num { get; set; }
        public byte length_size_of_sample_num { get; set; }
        public uint number_of_entry { get; set; }
        public ulong time { get; set; }
        public ulong moof_offset { get; set; }
        public byte[] traf_number { get; set; }
        public byte[] trun_number { get; set; }
        public byte[] sample_delta { get; set; }

        public TrackFragmentRandomAccessBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 26);
            this.length_size_of_traf_num = IsoReaderWriter.ReadBits(stream, 2);
            this.length_size_of_trun_num = IsoReaderWriter.ReadBits(stream, 2);
            this.length_size_of_sample_num = IsoReaderWriter.ReadBits(stream, 2);
            this.number_of_entry = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= number_of_entry; i++)
            {

                if (version == 1)
                {
                    this.time = IsoReaderWriter.ReadUInt64(stream);
                    this.moof_offset = IsoReaderWriter.ReadUInt64(stream);
                }

                else
                {
                    this.time = IsoReaderWriter.ReadUInt32(stream);
                    this.moof_offset = IsoReaderWriter.ReadUInt32(stream);
                }
                this.traf_number = IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_traf_num + 1));
                this.trun_number = IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_trun_num + 1));
                this.sample_delta = IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_sample_num + 1));
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
            size += IsoReaderWriter.WriteBits(stream, 26, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 2, this.length_size_of_traf_num);
            size += IsoReaderWriter.WriteBits(stream, 2, this.length_size_of_trun_num);
            size += IsoReaderWriter.WriteBits(stream, 2, this.length_size_of_sample_num);
            size += IsoReaderWriter.WriteUInt32(stream, this.number_of_entry);

            for (int i = 1; i <= number_of_entry; i++)
            {

                if (version == 1)
                {
                    size += IsoReaderWriter.WriteUInt64(stream, this.time);
                    size += IsoReaderWriter.WriteUInt64(stream, this.moof_offset);
                }

                else
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.time);
                    size += IsoReaderWriter.WriteUInt32(stream, this.moof_offset);
                }
                size += IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_traf_num + 1), this.traf_number);
                size += IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_trun_num + 1), this.trun_number);
                size += IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_sample_num + 1), this.sample_delta);
            }
            return size;
        }
    }


    public class TrackHeaderBox : FullBox
    {
        public override string FourCC { get { return "tkhd"; } }
        public ulong creation_time { get; set; }
        public ulong modification_time { get; set; }
        public uint track_ID { get; set; }
        public uint[] reserved { get; set; } = [];
        public ulong duration { get; set; }
        public short layer { get; set; } = 0;
        public short alternate_group { get; set; } = 0;
        public short volume { get; set; } = 0; // = { default samplerate of media}<<16;
        public uint[] matrix { get; set; } =
            { 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };  //  unity matrix
        public uint width { get; set; }
        public uint height { get; set; }

        public TrackHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            if (version == 1)
            {
                this.creation_time = IsoReaderWriter.ReadUInt64(stream);
                this.modification_time = IsoReaderWriter.ReadUInt64(stream);
                this.track_ID = IsoReaderWriter.ReadUInt32(stream);
                this.reserved = IsoReaderWriter.ReadUInt32(stream);
                this.duration = IsoReaderWriter.ReadUInt64(stream);
            }

            else
            {
                /*  version==0 */
                this.creation_time = IsoReaderWriter.ReadUInt32(stream);
                this.modification_time = IsoReaderWriter.ReadUInt32(stream);
                this.track_ID = IsoReaderWriter.ReadUInt32(stream);
                this.reserved = IsoReaderWriter.ReadUInt32(stream);
                this.duration = IsoReaderWriter.ReadUInt32(stream);
            }
            this.reserved = IsoReaderWriter.ReadUInt32Array(stream, 2);
            this.layer = IsoReaderWriter.ReadInt16(stream);
            this.alternate_group = IsoReaderWriter.ReadInt16(stream);
            this.volume = IsoReaderWriter.ReadInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.matrix = IsoReaderWriter.ReadUInt32Array(stream, 9);
            this.width = IsoReaderWriter.ReadUInt32(stream);
            this.height = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            if (version == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
                size += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
                size += IsoReaderWriter.WriteUInt32(stream, this.reserved);
                size += IsoReaderWriter.WriteUInt64(stream, this.duration);
            }

            else
            {
                /*  version==0 */
                size += IsoReaderWriter.WriteUInt32(stream, this.creation_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.modification_time);
                size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
                size += IsoReaderWriter.WriteUInt32(stream, this.reserved);
                size += IsoReaderWriter.WriteUInt32(stream, this.duration);
            }
            size += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteInt16(stream, this.layer);
            size += IsoReaderWriter.WriteInt16(stream, this.alternate_group);
            size += IsoReaderWriter.WriteInt16(stream, this.volume);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32Array(stream, 9, this.matrix);
            size += IsoReaderWriter.WriteUInt32(stream, this.width);
            size += IsoReaderWriter.WriteUInt32(stream, this.height);
            return size;
        }
    }


    public class TrackFragmentBox : Box
    {
        public override string FourCC { get { return "traf"; } }

        public TrackFragmentBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class TrackBox : Box
    {
        public override string FourCC { get { return "trak"; } }

        public TrackBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class TrackReferenceBox : Box
    {
        public override string FourCC { get { return "tref"; } }
        public TrackReferenceTypeBox[] TrackReferenceTypeBox { get; set; }

        public TrackReferenceBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.TrackReferenceTypeBox = (TrackReferenceTypeBox[])IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.TrackReferenceTypeBox);
            return size;
        }
    }


    public class TrackExtensionPropertiesBox : FullBox
    {
        public override string FourCC { get { return "trep"; } }
        public uint track_ID { get; set; }  //  Any number of boxes may follow

        public TrackExtensionPropertiesBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_ID = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
            return size;
        }
    }


    public class TrackExtendsBox : FullBox
    {
        public override string FourCC { get { return "trex"; } }
        public uint track_ID { get; set; }
        public uint default_sample_description_index { get; set; }
        public uint default_sample_duration { get; set; }
        public uint default_sample_size { get; set; }
        public uint default_sample_flags { get; set; }

        public TrackExtendsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_ID = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_description_index = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_duration = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_size = IsoReaderWriter.ReadUInt32(stream);
            this.default_sample_flags = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_ID);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_description_index);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_duration);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_size);
            size += IsoReaderWriter.WriteUInt32(stream, this.default_sample_flags);
            return size;
        }
    }


    public class TrackGroupBox : Box
    {
        public override string FourCC { get { return "trgr"; } }

        public TrackGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class TrackRunBox : FullBox
    {
        public override string FourCC { get { return "trun"; } }
        public uint sample_count { get; set; }  //  the following are optional fields
        public int data_offset { get; set; }
        public uint first_sample_flags { get; set; }  //  all fields in the following array are optional

        public TrackRunBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sample_count = IsoReaderWriter.ReadUInt32(stream);
            this.data_offset = IsoReaderWriter.ReadInt32(stream);
            this.first_sample_flags = IsoReaderWriter.ReadUInt32(stream);
            /*  as indicated by bits set in the tr_flags */

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_count);
            size += IsoReaderWriter.WriteInt32(stream, this.data_offset);
            size += IsoReaderWriter.WriteUInt32(stream, this.first_sample_flags);
            /*  as indicated by bits set in the tr_flags */

            return size;
        }
    }


    public class TrackTypeBox : GeneralTypeBox
    {
        public override string FourCC { get { return "ttyp"; } }

        public TrackTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class TypeCombinationBox : Box
    {
        public override string FourCC { get { return "tyco"; } }
        public uint[] compatible_brands { get; set; }  //  to end of the box

        public TypeCombinationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.compatible_brands = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.compatible_brands);
            return size;
        }
    }


    public class UserDataBox : Box
    {
        public override string FourCC { get { return "udta"; } }

        public UserDataBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class BoxHeader
    {

        public uint size { get; set; }
        public uint type { get; set; } // = boxtype
        public ulong largesize { get; set; }
        public byte[] usertype { get; set; } // = extended_type

        public BoxHeader()
        { }

        public async Task ReadAsync(Stream stream)
        {
            this.size = IsoReaderWriter.ReadUInt32(stream);
            this.type = IsoReaderWriter.ReadUInt32(stream);

            if (size == 1)
            {
                this.largesize = IsoReaderWriter.ReadUInt64(stream);
            }

            else if (size == 0)
            {
                /*  box extends to end of file */
            }

            if (type == IsoReaderWriter.FromFourCC("uuid"))
            {
                this.usertype = IsoReaderWriter.ReadBytes(stream, 16);
            }
        }

        public async Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += IsoReaderWriter.WriteUInt32(stream, this.size);
            size += IsoReaderWriter.WriteUInt32(stream, this.type);

            if (size == 1)
            {
                size += IsoReaderWriter.WriteUInt64(stream, this.largesize);
            }

            else if (size == 0)
            {
                /*  box extends to end of file */
            }

            if (type == IsoReaderWriter.FromFourCC("uuid"))
            {
                size += IsoReaderWriter.WriteBytes(stream, 16, this.usertype);
            }
            return size;
        }
    }


    public class VideoMediaHeaderBox : FullBox
    {
        public override string FourCC { get { return "vmhd"; } }
        public ushort graphicsmode { get; set; } = 0;  //  copy, see below
        public ushort[] opcolor { get; set; } = { 0, 0, 0 };

        public VideoMediaHeaderBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.graphicsmode = IsoReaderWriter.ReadUInt16(stream);
            this.opcolor = IsoReaderWriter.ReadUInt16Array(stream, 3);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.graphicsmode);
            size += IsoReaderWriter.WriteUInt16Array(stream, 3, this.opcolor);
            return size;
        }
    }


    public class XMLBox : FullBox
    {
        public override string FourCC { get { return "xml "; } }
        public string xml { get; set; }

        public XMLBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.xml = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.xml);
            return size;
        }
    }


    public class CompressedMovieFragmentBox1 : CompressedBox
    {
        public override string FourCC { get { return "!mof"; } }

        public CompressedMovieFragmentBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class CompressedMovieBox1 : CompressedBox
    {
        public override string FourCC { get { return "!mov"; } }

        public CompressedMovieBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class CompressedSegmentIndexBox1 : CompressedBox
    {
        public override string FourCC { get { return "!six"; } }

        public CompressedSegmentIndexBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class CompressedSubsegmentIndexBox1 : CompressedBox
    {
        public override string FourCC { get { return "!ssx"; } }

        public CompressedSubsegmentIndexBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class AmbientViewingEnvironmentBox : Box
    {
        public override string FourCC { get { return "amve"; } }
        public uint ambient_illuminance { get; set; }
        public ushort ambient_light_x { get; set; }
        public ushort ambient_light_y { get; set; }

        public AmbientViewingEnvironmentBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.ambient_illuminance = IsoReaderWriter.ReadUInt32(stream);
            this.ambient_light_x = IsoReaderWriter.ReadUInt16(stream);
            this.ambient_light_y = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.ambient_illuminance);
            size += IsoReaderWriter.WriteUInt16(stream, this.ambient_light_x);
            size += IsoReaderWriter.WriteUInt16(stream, this.ambient_light_y);
            return size;
        }
    }


    public class MetadataKeyTableBox : Box
    {
        public override string FourCC { get { return "keys"; } }
        public MetadataKeyBox[] MetadataKeyBox { get; set; }

        public MetadataKeyTableBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.MetadataKeyBox = (MetadataKeyBox[])IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.MetadataKeyBox);
            return size;
        }
    }


    public class URIBox : FullBox
    {
        public override string FourCC { get { return "uri "; } }
        public string theURI { get; set; }

        public URIBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.theURI = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.theURI);
            return size;
        }
    }


    public class IroiInfoBox : Box
    {
        public override string FourCC { get { return "iroi"; } }
        public byte iroi_type { get; set; }
        public byte reserved { get; set; } = 0;
        public byte grid_roi_mb_width { get; set; }
        public byte grid_roi_mb_height { get; set; }
        public uint num_roi { get; set; }
        public uint top_left_mb { get; set; }
        public byte roi_mb_width { get; set; }
        public byte roi_mb_height { get; set; }

        public IroiInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.iroi_type = IsoReaderWriter.ReadBits(stream, 2);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);

            if (iroi_type == 0)
            {
                this.grid_roi_mb_width = IsoReaderWriter.ReadUInt8(stream);
                this.grid_roi_mb_height = IsoReaderWriter.ReadUInt8(stream);
            }

            else if (iroi_type == 1)
            {
                this.num_roi = IsoReaderWriter.ReadUInt24(stream);

                for (int i = 1; i <= num_roi; i++)
                {
                    this.top_left_mb = IsoReaderWriter.ReadUInt32(stream);
                    this.roi_mb_width = IsoReaderWriter.ReadUInt8(stream);
                    this.roi_mb_height = IsoReaderWriter.ReadUInt8(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 2, this.iroi_type);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);

            if (iroi_type == 0)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.grid_roi_mb_width);
                size += IsoReaderWriter.WriteUInt8(stream, this.grid_roi_mb_height);
            }

            else if (iroi_type == 1)
            {
                size += IsoReaderWriter.WriteUInt24(stream, this.num_roi);

                for (int i = 1; i <= num_roi; i++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.top_left_mb);
                    size += IsoReaderWriter.WriteUInt8(stream, this.roi_mb_width);
                    size += IsoReaderWriter.WriteUInt8(stream, this.roi_mb_height);
                }
            }
            return size;
        }
    }


    public class TierDependencyBox : Box
    {
        public override string FourCC { get { return "ldep"; } }
        public ushort entry_count { get; set; }
        public ushort dependencyTierId { get; set; }

        public TierDependencyBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < entry_count; i++)
            {
                this.dependencyTierId = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);

            for (int i = 0; i < entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.dependencyTierId);
            }
            return size;
        }
    }


    public class SVCDependencyRangeBox : Box
    {
        public override string FourCC { get { return "svdr"; } }
        public byte min_dependency_id { get; set; }
        public byte min_temporal_id { get; set; }
        public byte reserved { get; set; } = 0;
        public byte min_quality_id { get; set; }
        public byte max_dependency_id { get; set; }
        public byte max_temporal_id { get; set; }
        public byte max_quality_id { get; set; }

        public SVCDependencyRangeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.min_dependency_id = IsoReaderWriter.ReadBits(stream, 3);
            this.min_temporal_id = IsoReaderWriter.ReadBits(stream, 3);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.min_quality_id = IsoReaderWriter.ReadBits(stream, 4);
            this.max_dependency_id = IsoReaderWriter.ReadBits(stream, 3);
            this.max_temporal_id = IsoReaderWriter.ReadBits(stream, 3);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.max_quality_id = IsoReaderWriter.ReadBits(stream, 4);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 3, this.min_dependency_id);
            size += IsoReaderWriter.WriteBits(stream, 3, this.min_temporal_id);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 4, this.min_quality_id);
            size += IsoReaderWriter.WriteBits(stream, 3, this.max_dependency_id);
            size += IsoReaderWriter.WriteBits(stream, 3, this.max_temporal_id);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 4, this.max_quality_id);
            return size;
        }
    }


    public class InitialParameterSetBox : Box
    {
        public override string FourCC { get { return "svip"; } }
        public byte sps_id_count { get; set; }
        public byte SPS_index { get; set; }
        public byte pps_id_count { get; set; }
        public byte PPS_index { get; set; }

        public InitialParameterSetBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sps_id_count = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i < sps_id_count; i++)
            {
                this.SPS_index = IsoReaderWriter.ReadUInt8(stream);
            }
            this.pps_id_count = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i < pps_id_count; i++)
            {
                this.PPS_index = IsoReaderWriter.ReadUInt8(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.sps_id_count);

            for (int i = 0; i < sps_id_count; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.SPS_index);
            }
            size += IsoReaderWriter.WriteUInt8(stream, this.pps_id_count);

            for (int i = 0; i < pps_id_count; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.PPS_index);
            }
            return size;
        }
    }


    public class PriorityRangeBox : Box
    {
        public override string FourCC { get { return "svpr"; } }
        public byte reserved1 { get; set; } = 0;
        public byte min_priorityId { get; set; }
        public byte reserved2 { get; set; } = 0;
        public byte max_priorityId { get; set; }

        public PriorityRangeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved1 = IsoReaderWriter.ReadBits(stream, 2);
            this.min_priorityId = IsoReaderWriter.ReadBits(stream, 6);
            this.reserved2 = IsoReaderWriter.ReadBits(stream, 2);
            this.max_priorityId = IsoReaderWriter.ReadBits(stream, 6);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 2, this.reserved1);
            size += IsoReaderWriter.WriteBits(stream, 6, this.min_priorityId);
            size += IsoReaderWriter.WriteBits(stream, 2, this.reserved2);
            size += IsoReaderWriter.WriteBits(stream, 6, this.max_priorityId);
            return size;
        }
    }


    public class TranscodingInfoBox : Box
    {
        public override string FourCC { get { return "tran"; } }
        public byte reserved { get; set; } = 0;
        public byte conversion_idc { get; set; }
        public bool cavlc_info_present_flag { get; set; }
        public bool cabac_info_present_flag { get; set; }
        public uint cavlc_profile_level_idc { get; set; }
        public uint cavlc_max_bitrate { get; set; }
        public uint cavlc_avg_bitrate { get; set; }
        public uint cabac_profile_level_idc { get; set; }
        public uint cabac_max_bitrate { get; set; }
        public uint cabac_avg_bitrate { get; set; }

        public TranscodingInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 4);
            this.conversion_idc = IsoReaderWriter.ReadBits(stream, 2);
            this.cavlc_info_present_flag = IsoReaderWriter.ReadBit(stream);
            this.cabac_info_present_flag = IsoReaderWriter.ReadBit(stream);

            if (cavlc_info_present_flag)
            {
                this.cavlc_profile_level_idc = IsoReaderWriter.ReadUInt24(stream);
                this.cavlc_max_bitrate = IsoReaderWriter.ReadUInt32(stream);
                this.cavlc_avg_bitrate = IsoReaderWriter.ReadUInt32(stream);
            }

            if (cabac_info_present_flag)
            {
                this.cabac_profile_level_idc = IsoReaderWriter.ReadUInt24(stream);
                this.cabac_max_bitrate = IsoReaderWriter.ReadUInt32(stream);
                this.cabac_avg_bitrate = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 2, this.conversion_idc);
            size += IsoReaderWriter.WriteBit(stream, this.cavlc_info_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.cabac_info_present_flag);

            if (cavlc_info_present_flag)
            {
                size += IsoReaderWriter.WriteUInt24(stream, this.cavlc_profile_level_idc);
                size += IsoReaderWriter.WriteUInt32(stream, this.cavlc_max_bitrate);
                size += IsoReaderWriter.WriteUInt32(stream, this.cavlc_avg_bitrate);
            }

            if (cabac_info_present_flag)
            {
                size += IsoReaderWriter.WriteUInt24(stream, this.cabac_profile_level_idc);
                size += IsoReaderWriter.WriteUInt32(stream, this.cabac_max_bitrate);
                size += IsoReaderWriter.WriteUInt32(stream, this.cabac_avg_bitrate);
            }
            return size;
        }
    }


    public class RectRegionBox : Box
    {
        public override string FourCC { get { return "rrgn"; } }
        public ushort base_region_tierID { get; set; }
        public bool dynamic_rect { get; set; }
        public byte reserved { get; set; } = 0;
        public ushort horizontal_offset { get; set; }
        public ushort vertical_offset { get; set; }
        public ushort region_width { get; set; }
        public ushort region_height { get; set; }

        public RectRegionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.base_region_tierID = IsoReaderWriter.ReadUInt16(stream);
            this.dynamic_rect = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 7);

            if (dynamic_rect == 0)
            {
                this.horizontal_offset = IsoReaderWriter.ReadUInt16(stream);
                this.vertical_offset = IsoReaderWriter.ReadUInt16(stream);
                this.region_width = IsoReaderWriter.ReadUInt16(stream);
                this.region_height = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.base_region_tierID);
            size += IsoReaderWriter.WriteBit(stream, this.dynamic_rect);
            size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);

            if (dynamic_rect == 0)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.horizontal_offset);
                size += IsoReaderWriter.WriteUInt16(stream, this.vertical_offset);
                size += IsoReaderWriter.WriteUInt16(stream, this.region_width);
                size += IsoReaderWriter.WriteUInt16(stream, this.region_height);
            }
            return size;
        }
    }


    public class BufferingBox : Box
    {
        public override string FourCC { get { return "buff"; } }
        public ushort operating_point_count { get; set; }
        public uint byte_rate { get; set; }
        public uint cpb_size { get; set; }
        public uint dpb_size { get; set; }
        public uint init_cpb_delay { get; set; }
        public uint init_dpb_delay { get; set; }

        public BufferingBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.operating_point_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < operating_point_count; i++)
            {
                this.byte_rate = IsoReaderWriter.ReadUInt32(stream);
                this.cpb_size = IsoReaderWriter.ReadUInt32(stream);
                this.dpb_size = IsoReaderWriter.ReadUInt32(stream);
                this.init_cpb_delay = IsoReaderWriter.ReadUInt32(stream);
                this.init_dpb_delay = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.operating_point_count);

            for (int i = 0; i < operating_point_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.byte_rate);
                size += IsoReaderWriter.WriteUInt32(stream, this.cpb_size);
                size += IsoReaderWriter.WriteUInt32(stream, this.dpb_size);
                size += IsoReaderWriter.WriteUInt32(stream, this.init_cpb_delay);
                size += IsoReaderWriter.WriteUInt32(stream, this.init_dpb_delay);
            }
            return size;
        }
    }


    public class MVCSubTrackViewBox : FullBox
    {
        public override string FourCC { get { return "mstv"; } }
        public ushort item_count { get; set; }
        public ushort view_id { get; set; }
        public byte temporal_id { get; set; }
        public byte reserved { get; set; }

        public MVCSubTrackViewBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.item_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < item_count; i++)
            {
                this.view_id = IsoReaderWriter.ReadBits(stream, 10);
                this.temporal_id = IsoReaderWriter.ReadBits(stream, 4);
                this.reserved = IsoReaderWriter.ReadBits(stream, 2);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 10, this.view_id);
                size += IsoReaderWriter.WriteBits(stream, 4, this.temporal_id);
                size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            }
            return size;
        }
    }


    public class MultiviewGroupBox : FullBox
    {
        public override string FourCC { get { return "mvcg"; } }
        public uint multiview_group_id { get; set; }
        public ushort num_entries { get; set; }
        public byte reserved { get; set; } = 0;
        public byte entry_type { get; set; }
        public uint track_id { get; set; }
        public ushort tier_id { get; set; }
        public byte reserved1 { get; set; } = 0;
        public ushort output_view_id { get; set; }
        public byte reserved2 { get; set; } = 0;
        public ushort start_view_id { get; set; }
        public ushort view_count { get; set; }
        public TierInfoBox subset_stream_info { get; set; }  //  optional
        public MultiviewRelationAttributeBox relation_attributes { get; set; }  //  optional
        public TierBitRateBox subset_stream_bit_rate { get; set; }  //  optional
        public BufferingBox subset_stream_buffering { get; set; }  //  optional
        public MultiviewSceneInfoBox multiview_scene_info { get; set; }  //  optional

        public MultiviewGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.multiview_group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entries = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i < num_entries; i++)
            {
                this.entry_type = IsoReaderWriter.ReadUInt8(stream);

                if (entry_type == 0)
                {
                    this.track_id = IsoReaderWriter.ReadUInt32(stream);
                }

                else if (entry_type == 1)
                {
                    this.track_id = IsoReaderWriter.ReadUInt32(stream);
                    this.tier_id = IsoReaderWriter.ReadUInt16(stream);
                }

                else if (entry_type == 2)
                {
                    this.reserved1 = IsoReaderWriter.ReadBits(stream, 6);
                    this.output_view_id = IsoReaderWriter.ReadBits(stream, 10);
                }

                else if (entry_type == 3)
                {
                    this.reserved2 = IsoReaderWriter.ReadBits(stream, 6);
                    this.start_view_id = IsoReaderWriter.ReadBits(stream, 10);
                    this.view_count = IsoReaderWriter.ReadUInt16(stream);
                }
            }
            this.subset_stream_info = (TierInfoBox)IsoReaderWriter.ReadBox(stream);
            this.relation_attributes = (MultiviewRelationAttributeBox)IsoReaderWriter.ReadBox(stream);
            this.subset_stream_bit_rate = (TierBitRateBox)IsoReaderWriter.ReadBox(stream);
            this.subset_stream_buffering = (BufferingBox)IsoReaderWriter.ReadBox(stream);
            this.multiview_scene_info = (MultiviewSceneInfoBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.multiview_group_id);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_entries);
            size += IsoReaderWriter.WriteUInt8(stream, this.reserved);

            for (int i = 0; i < num_entries; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.entry_type);

                if (entry_type == 0)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.track_id);
                }

                else if (entry_type == 1)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.track_id);
                    size += IsoReaderWriter.WriteUInt16(stream, this.tier_id);
                }

                else if (entry_type == 2)
                {
                    size += IsoReaderWriter.WriteBits(stream, 6, this.reserved1);
                    size += IsoReaderWriter.WriteBits(stream, 10, this.output_view_id);
                }

                else if (entry_type == 3)
                {
                    size += IsoReaderWriter.WriteBits(stream, 6, this.reserved2);
                    size += IsoReaderWriter.WriteBits(stream, 10, this.start_view_id);
                    size += IsoReaderWriter.WriteUInt16(stream, this.view_count);
                }
            }
            size += IsoReaderWriter.WriteBox(stream, this.subset_stream_info);
            size += IsoReaderWriter.WriteBox(stream, this.relation_attributes);
            size += IsoReaderWriter.WriteBox(stream, this.subset_stream_bit_rate);
            size += IsoReaderWriter.WriteBox(stream, this.subset_stream_buffering);
            size += IsoReaderWriter.WriteBox(stream, this.multiview_scene_info);
            return size;
        }
    }


    public class MultiviewInformationBox : FullBox
    {
        public override string FourCC { get { return "mvci"; } }

        public MultiviewInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class MVDDepthResolutionBox : Box
    {
        public override string FourCC { get { return "3dpr"; } }
        public ushort depth_width { get; set; }
        public ushort depth_height { get; set; }
        public ushort depth_hor_mult_minus1 { get; set; }  //  optional
        public ushort depth_ver_mult_minus1 { get; set; }  //  optional
        public byte depth_hor_rsh { get; set; }  //  optional
        public byte depth_ver_rsh { get; set; }  //  optional
        public ushort grid_pos_num_views { get; set; }  //  optional
        public byte reserved { get; set; } = 0;
        public ushort[] grid_pos_view_id { get; set; }
        public short[] grid_pos_x { get; set; }
        public short[] grid_pos_y { get; set; }

        public MVDDepthResolutionBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.depth_width = IsoReaderWriter.ReadUInt16(stream);
            this.depth_height = IsoReaderWriter.ReadUInt16(stream);
            /*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
            this.depth_hor_mult_minus1 = IsoReaderWriter.ReadUInt16(stream);
            this.depth_ver_mult_minus1 = IsoReaderWriter.ReadUInt16(stream);
            this.depth_hor_rsh = IsoReaderWriter.ReadBits(stream, 4);
            this.depth_ver_rsh = IsoReaderWriter.ReadBits(stream, 4);
            this.grid_pos_num_views = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < grid_pos_num_views; i++)
            {
                this.reserved = IsoReaderWriter.ReadBits(stream, 6);
                this.grid_pos_view_id[i] = IsoReaderWriter.ReadBits(stream, 10);
                this.grid_pos_x[grid_pos_view_id[i]] = IsoReaderWriter.ReadInt16(stream);
                this.grid_pos_y[grid_pos_view_id[i]] = IsoReaderWriter.ReadInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.depth_width);
            size += IsoReaderWriter.WriteUInt16(stream, this.depth_height);
            /*  The following 5 fields are collectively optional; they are either all present or all absent. When grid_pos_num_views is not present, the for loop is not present, equivalent to grid_pos_num_views equal to 0. These fields may be present or absent whenever the box is present (e.g., in MVCDConfigurationBox or A3DConfigurationBox).  */
            size += IsoReaderWriter.WriteUInt16(stream, this.depth_hor_mult_minus1);
            size += IsoReaderWriter.WriteUInt16(stream, this.depth_ver_mult_minus1);
            size += IsoReaderWriter.WriteBits(stream, 4, this.depth_hor_rsh);
            size += IsoReaderWriter.WriteBits(stream, 4, this.depth_ver_rsh);
            size += IsoReaderWriter.WriteUInt16(stream, this.grid_pos_num_views);

            for (int i = 0; i < grid_pos_num_views; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 10, this.grid_pos_view_id[i]);
                size += IsoReaderWriter.WriteInt16Array(stream, grid_pos_view_id[i], this.grid_pos_x[grid_pos_view_id[i]]);
                size += IsoReaderWriter.WriteInt16Array(stream, grid_pos_view_id[i], this.grid_pos_y[grid_pos_view_id[i]]);
            }
            return size;
        }
    }


    public class MultiviewRelationAttributeBox : FullBox
    {
        public override string FourCC { get { return "mvra"; } }
        public ushort reserved1 { get; set; } = 0;
        public ushort num_common_attributes { get; set; }
        public uint common_attribute { get; set; }
        public uint common_value { get; set; }
        public ushort reserved2 { get; set; } = 0;
        public ushort num_differentiating_attributes { get; set; }
        public uint differentiating_attribute { get; set; }

        public MultiviewRelationAttributeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved1 = IsoReaderWriter.ReadUInt16(stream);
            this.num_common_attributes = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_common_attributes; i++)
            {
                this.common_attribute = IsoReaderWriter.ReadUInt32(stream);
                this.common_value = IsoReaderWriter.ReadUInt32(stream);
            }
            this.reserved2 = IsoReaderWriter.ReadUInt16(stream);
            this.num_differentiating_attributes = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_differentiating_attributes; i++)
            {
                this.differentiating_attribute = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved1);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_common_attributes);

            for (int i = 0; i < num_common_attributes; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.common_attribute);
                size += IsoReaderWriter.WriteUInt32(stream, this.common_value);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved2);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_differentiating_attributes);

            for (int i = 0; i < num_differentiating_attributes; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.differentiating_attribute);
            }
            return size;
        }
    }


    public class SampleDependencyBox : FullBox
    {
        public override string FourCC { get { return "sdep"; } }
        public ushort dependency_count { get; set; }
        public short relative_sample_number { get; set; }

        public SampleDependencyBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            for (int i = 0; i < sample_count; i++)
            {
                this.dependency_count = IsoReaderWriter.ReadUInt16(stream);

                for (int k = 0; k < dependency_count; k++)
                {
                    this.relative_sample_number = IsoReaderWriter.ReadInt16(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            for (int i = 0; i < sample_count; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.dependency_count);

                for (int k = 0; k < dependency_count; k++)
                {
                    size += IsoReaderWriter.WriteInt16(stream, this.relative_sample_number);
                }
            }
            return size;
        }
    }


    public class SeiInformationBox : Box
    {
        public override string FourCC { get { return "seii"; } }
        public ushort numRequiredSEIs { get; set; }
        public ushort requiredSEI_ID { get; set; }
        public ushort numNotRequiredSEIs { get; set; }
        public ushort notrequiredSEI_ID { get; set; }

        public SeiInformationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.numRequiredSEIs = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < numRequiredSEIs; i++)
            {
                this.requiredSEI_ID = IsoReaderWriter.ReadUInt16(stream);
            }
            this.numNotRequiredSEIs = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < numNotRequiredSEIs; i++)
            {
                this.notrequiredSEI_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.numRequiredSEIs);

            for (int i = 0; i < numRequiredSEIs; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.requiredSEI_ID);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.numNotRequiredSEIs);

            for (int i = 0; i < numNotRequiredSEIs; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.notrequiredSEI_ID);
            }
            return size;
        }
    }


    public class SVCSubTrackLayerBox : FullBox
    {
        public override string FourCC { get { return "sstl"; } }
        public ushort item_count { get; set; }
        public byte dependency_id { get; set; }
        public byte quality_id { get; set; }
        public byte temporal_id { get; set; }
        public byte priority_id { get; set; }
        public byte dependency_id_range { get; set; }
        public byte quality_id_range { get; set; }
        public byte temporal_id_range { get; set; }
        public byte priority_id_range { get; set; }

        public SVCSubTrackLayerBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.item_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < item_count; i++)
            {
                this.dependency_id = IsoReaderWriter.ReadBits(stream, 3);
                this.quality_id = IsoReaderWriter.ReadBits(stream, 4);
                this.temporal_id = IsoReaderWriter.ReadBits(stream, 3);
                this.priority_id = IsoReaderWriter.ReadBits(stream, 6);
                this.dependency_id_range = IsoReaderWriter.ReadBits(stream, 2);
                this.quality_id_range = IsoReaderWriter.ReadBits(stream, 2);
                this.temporal_id_range = IsoReaderWriter.ReadBits(stream, 2);
                this.priority_id_range = IsoReaderWriter.ReadBits(stream, 2);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 3, this.dependency_id);
                size += IsoReaderWriter.WriteBits(stream, 4, this.quality_id);
                size += IsoReaderWriter.WriteBits(stream, 3, this.temporal_id);
                size += IsoReaderWriter.WriteBits(stream, 6, this.priority_id);
                size += IsoReaderWriter.WriteBits(stream, 2, this.dependency_id_range);
                size += IsoReaderWriter.WriteBits(stream, 2, this.quality_id_range);
                size += IsoReaderWriter.WriteBits(stream, 2, this.temporal_id_range);
                size += IsoReaderWriter.WriteBits(stream, 2, this.priority_id_range);
            }
            return size;
        }
    }


    public class MVCSubTrackMultiviewGroupBox : FullBox
    {
        public override string FourCC { get { return "stmg"; } }
        public ushort item_count { get; set; }
        public uint MultiviewGroupId { get; set; }

        public MVCSubTrackMultiviewGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.item_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < item_count; i++)
            {
                this.MultiviewGroupId = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.MultiviewGroupId);
            }
            return size;
        }
    }


    public class SubTrackTierBox : FullBox
    {
        public override string FourCC { get { return "stti"; } }
        public ushort item_count { get; set; }
        public ushort tierID { get; set; }

        public SubTrackTierBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.item_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < item_count; i++)
            {
                this.tierID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.tierID);
            }
            return size;
        }
    }


    public class MultiviewGroupRelationBox : FullBox
    {
        public override string FourCC { get { return "swtc"; } }
        public uint num_entries { get; set; }
        public uint multiview_group_id { get; set; }
        public MultiviewRelationAttributeBox relation_attributes { get; set; }

        public MultiviewGroupRelationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_entries = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entries; i++)
            {
                this.multiview_group_id = IsoReaderWriter.ReadUInt32(stream);
            }
            this.relation_attributes = (MultiviewRelationAttributeBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

            for (int i = 0; i < num_entries; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.multiview_group_id);
            }
            size += IsoReaderWriter.WriteBox(stream, this.relation_attributes);
            return size;
        }
    }


    public class TierBitRateBox : Box
    {
        public override string FourCC { get { return "tibr"; } }
        public uint baseBitRate { get; set; }
        public uint maxBitRate { get; set; }
        public uint avgBitRate { get; set; }
        public uint tierBaseBitRate { get; set; }
        public uint tierMaxBitRate { get; set; }
        public uint tierAvgBitRate { get; set; }

        public TierBitRateBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.baseBitRate = IsoReaderWriter.ReadUInt32(stream);
            this.maxBitRate = IsoReaderWriter.ReadUInt32(stream);
            this.avgBitRate = IsoReaderWriter.ReadUInt32(stream);
            this.tierBaseBitRate = IsoReaderWriter.ReadUInt32(stream);
            this.tierMaxBitRate = IsoReaderWriter.ReadUInt32(stream);
            this.tierAvgBitRate = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.baseBitRate);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxBitRate);
            size += IsoReaderWriter.WriteUInt32(stream, this.avgBitRate);
            size += IsoReaderWriter.WriteUInt32(stream, this.tierBaseBitRate);
            size += IsoReaderWriter.WriteUInt32(stream, this.tierMaxBitRate);
            size += IsoReaderWriter.WriteUInt32(stream, this.tierAvgBitRate);
            return size;
        }
    }


    public class TierInfoBox : Box
    {
        public override string FourCC { get { return "tiri"; } }
        public ushort tierID { get; set; }
        public byte profileIndication { get; set; }
        public byte profile_compatibility { get; set; }
        public byte levelIndication { get; set; }
        public byte reserved { get; set; } = 0;
        public ushort visualWidth { get; set; }
        public ushort visualHeight { get; set; }
        public byte discardable { get; set; }
        public byte constantFrameRate { get; set; }
        public ushort frameRate { get; set; }

        public TierInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            /* Mandatory Box */
            this.tierID = IsoReaderWriter.ReadUInt16(stream);
            this.profileIndication = IsoReaderWriter.ReadUInt8(stream);
            this.profile_compatibility = IsoReaderWriter.ReadUInt8(stream);
            this.levelIndication = IsoReaderWriter.ReadUInt8(stream);
            this.reserved = IsoReaderWriter.ReadUInt8(stream);
            this.visualWidth = IsoReaderWriter.ReadUInt16(stream);
            this.visualHeight = IsoReaderWriter.ReadUInt16(stream);
            this.discardable = IsoReaderWriter.ReadBits(stream, 2);
            this.constantFrameRate = IsoReaderWriter.ReadBits(stream, 2);
            this.reserved = IsoReaderWriter.ReadBits(stream, 4);
            this.frameRate = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            /* Mandatory Box */
            size += IsoReaderWriter.WriteUInt16(stream, this.tierID);
            size += IsoReaderWriter.WriteUInt8(stream, this.profileIndication);
            size += IsoReaderWriter.WriteUInt8(stream, this.profile_compatibility);
            size += IsoReaderWriter.WriteUInt8(stream, this.levelIndication);
            size += IsoReaderWriter.WriteUInt8(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.visualWidth);
            size += IsoReaderWriter.WriteUInt16(stream, this.visualHeight);
            size += IsoReaderWriter.WriteBits(stream, 2, this.discardable);
            size += IsoReaderWriter.WriteBits(stream, 2, this.constantFrameRate);
            size += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.frameRate);
            return size;
        }
    }


    public class TileSubTrackGroupBox : FullBox
    {
        public override string FourCC { get { return "tstb"; } }
        public ushort item_count { get; set; }
        public ushort tileGroupID { get; set; }

        public TileSubTrackGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.item_count = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < item_count; i++)
            {
                this.tileGroupID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.item_count);

            for (int i = 0; i < item_count; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.tileGroupID);
            }
            return size;
        }
    }


    public class MultiviewSceneInfoBox : Box
    {
        public override string FourCC { get { return "vwdi"; } }
        public byte max_disparity { get; set; }

        public MultiviewSceneInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.max_disparity = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.max_disparity);
            return size;
        }
    }


    public class MVCDConfigurationBox : Box
    {
        public override string FourCC { get { return "mvdC"; } }
        public MVDDecoderConfigurationRecord MVDConfig { get; set; }
        public MVDDepthResolutionBox mvdDepthRes { get; set; }  // Optional

        public MVCDConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.MVDConfig = (MVDDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
            this.mvdDepthRes = (MVDDepthResolutionBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.MVDConfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdDepthRes);
            return size;
        }
    }


    public class A3DConfigurationBox : Box
    {
        public override string FourCC { get { return "a3dC"; } }
        public MVDDecoderConfigurationRecord MVDConfig { get; set; }
        public MVDDepthResolutionBox mvdDepthRes { get; set; }  // Optional

        public A3DConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.MVDConfig = (MVDDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
            this.mvdDepthRes = (MVDDepthResolutionBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.MVDConfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdDepthRes);
            return size;
        }
    }


    public class ViewIdentifierBox : FullBox
    {
        public override string FourCC { get { return "vwid"; } }
        public byte reserved6 { get; set; } = 0;
        public byte min_temporal_id { get; set; }
        public byte max_temporal_id { get; set; }
        public ushort num_views { get; set; }
        public byte reserved1 { get; set; } = 0;
        public ushort[] view_id { get; set; }
        public byte reserved2 { get; set; } = 0;
        public ushort view_order_index { get; set; }
        public bool[] texture_in_stream { get; set; }
        public bool[] texture_in_track { get; set; }
        public bool[] depth_in_stream { get; set; }
        public bool[] depth_in_track { get; set; }
        public byte base_view_type { get; set; }
        public ushort num_ref_views { get; set; }
        public byte reserved5 { get; set; } = 0;
        public byte[][] dependent_component_idc { get; set; }
        public ushort[][] ref_view_id { get; set; }

        public ViewIdentifierBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved6 = IsoReaderWriter.ReadBits(stream, 2);
            this.min_temporal_id = IsoReaderWriter.ReadBits(stream, 3);
            this.max_temporal_id = IsoReaderWriter.ReadBits(stream, 3);
            this.num_views = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_views; i++)
            {
                this.reserved1 = IsoReaderWriter.ReadBits(stream, 6);
                this.view_id[i] = IsoReaderWriter.ReadBits(stream, 10);
                this.reserved2 = IsoReaderWriter.ReadBits(stream, 6);
                this.view_order_index = IsoReaderWriter.ReadBits(stream, 10);
                this.texture_in_stream[i] = IsoReaderWriter.ReadBit(stream);
                this.texture_in_track[i] = IsoReaderWriter.ReadBit(stream);
                this.depth_in_stream[i] = IsoReaderWriter.ReadBit(stream);
                this.depth_in_track[i] = IsoReaderWriter.ReadBit(stream);
                this.base_view_type = IsoReaderWriter.ReadBits(stream, 2);
                this.num_ref_views = IsoReaderWriter.ReadBits(stream, 10);

                for (int j = 0; j < num_ref_views; j++)
                {
                    this.reserved5 = IsoReaderWriter.ReadBits(stream, 4);
                    this.dependent_component_idc[i][j] = IsoReaderWriter.ReadBits(stream, 2);
                    this.ref_view_id[i][j] = IsoReaderWriter.ReadBits(stream, 10);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 2, this.reserved6);
            size += IsoReaderWriter.WriteBits(stream, 3, this.min_temporal_id);
            size += IsoReaderWriter.WriteBits(stream, 3, this.max_temporal_id);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_views);

            for (int i = 0; i < num_views; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 6, this.reserved1);
                size += IsoReaderWriter.WriteBits(stream, 10, this.view_id[i]);
                size += IsoReaderWriter.WriteBits(stream, 6, this.reserved2);
                size += IsoReaderWriter.WriteBits(stream, 10, this.view_order_index);
                size += IsoReaderWriter.WriteBit(stream, this.texture_in_stream[i]);
                size += IsoReaderWriter.WriteBit(stream, this.texture_in_track[i]);
                size += IsoReaderWriter.WriteBit(stream, this.depth_in_stream[i]);
                size += IsoReaderWriter.WriteBit(stream, this.depth_in_track[i]);
                size += IsoReaderWriter.WriteBits(stream, 2, this.base_view_type);
                size += IsoReaderWriter.WriteBits(stream, 10, this.num_ref_views);

                for (int j = 0; j < num_ref_views; j++)
                {
                    size += IsoReaderWriter.WriteBits(stream, 4, this.reserved5);
                    size += IsoReaderWriter.WriteBits(stream, 2, this.dependent_component_idc[i][j]);
                    size += IsoReaderWriter.WriteBits(stream, 10, this.ref_view_id[i][j]);
                }
            }
            return size;
        }
    }


    public class MVCConfigurationBox : Box
    {
        public override string FourCC { get { return "mvcC"; } }
        public MVCDecoderConfigurationRecord MVCConfig { get; set; }

        public MVCConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.MVCConfig = (MVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.MVCConfig);
            return size;
        }
    }


    public class AVCConfigurationBox : Box
    {
        public override string FourCC { get { return "avcC"; } }
        public AVCDecoderConfigurationRecord AVCConfig { get; set; }

        public AVCConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.AVCConfig = (AVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.AVCConfig);
            return size;
        }
    }


    public class HEVCConfigurationBox : Box
    {
        public override string FourCC { get { return "hvcC"; } }
        public HEVCDecoderConfigurationRecord HEVCConfig { get; set; }

        public HEVCConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.HEVCConfig = (HEVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.HEVCConfig);
            return size;
        }
    }


    public class LHEVCConfigurationBox : Box
    {
        public override string FourCC { get { return "lhvC"; } }
        public LHEVCDecoderConfigurationRecord LHEVCConfig { get; set; }

        public LHEVCConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.LHEVCConfig = (LHEVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.LHEVCConfig);
            return size;
        }
    }


    public class MPEG4ExtensionDescriptorsBox : Box
    {
        public override string FourCC { get { return "m4ds"; } }
        public Descriptor[] Descr { get; set; }

        public MPEG4ExtensionDescriptorsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.Descr = (Descriptor[])IsoReaderWriter.ReadClasses(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClasses(stream, this.Descr);
            return size;
        }
    }


    public class SVCConfigurationBox : Box
    {
        public override string FourCC { get { return "svcC"; } }
        public SVCDecoderConfigurationRecord SVCConfig { get; set; }

        public SVCConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.SVCConfig = (SVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.SVCConfig);
            return size;
        }
    }


    public class ScalabilityInformationSEIBox : Box
    {
        public override string FourCC { get { return "seib"; } }
        public byte[] scalinfosei { get; set; }

        public ScalabilityInformationSEIBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scalinfosei = IsoReaderWriter.ReadBytes(stream, size - 64);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBytes(stream, size - 64, this.scalinfosei);
            return size;
        }
    }


    public class SVCPriorityAssignmentBox : Box
    {
        public override string FourCC { get { return "svcP"; } }
        public byte method_count { get; set; }
        public string[] PriorityAssignmentURI { get; set; }

        public SVCPriorityAssignmentBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.method_count = IsoReaderWriter.ReadUInt8(stream);
            this.PriorityAssignmentURI[method_count] = IsoReaderWriter.ReadStringArray(stream, method_count);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.method_count);
            size += IsoReaderWriter.WriteStringArray(stream, method_count, this.PriorityAssignmentURI[method_count]);
            return size;
        }
    }


    public class ViewScalabilityInformationSEIBox : Box
    {
        public override string FourCC { get { return "vsib"; } }
        public byte[] mvcscalinfosei { get; set; }

        public ViewScalabilityInformationSEIBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcscalinfosei = IsoReaderWriter.ReadBytes(stream, size - 64);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBytes(stream, size - 64, this.mvcscalinfosei);
            return size;
        }
    }


    public class MVDScalabilityInformationSEIBox : Box
    {
        public override string FourCC { get { return "3sib"; } }
        public byte[] mvdscalinfosei { get; set; }

        public MVDScalabilityInformationSEIBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvdscalinfosei = IsoReaderWriter.ReadBytes(stream, size - 64);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBytes(stream, size - 64, this.mvdscalinfosei);
            return size;
        }
    }


    public class MVCViewPriorityAssignmentBox : Box
    {
        public override string FourCC { get { return "mvcP"; } }
        public byte method_count { get; set; }
        public string[] PriorityAssignmentURI { get; set; }

        public MVCViewPriorityAssignmentBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.method_count = IsoReaderWriter.ReadUInt8(stream);
            this.PriorityAssignmentURI[method_count] = IsoReaderWriter.ReadStringArray(stream, method_count);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.method_count);
            size += IsoReaderWriter.WriteStringArray(stream, method_count, this.PriorityAssignmentURI[method_count]);
            return size;
        }
    }


    public class HEVCTileConfigurationBox : Box
    {
        public override string FourCC { get { return "hvtC"; } }
        public HEVCTileTierLevelConfigurationRecord HEVCTileTierLevelConfig { get; set; }

        public HEVCTileConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.HEVCTileTierLevelConfig = (HEVCTileTierLevelConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.HEVCTileTierLevelConfig);
            return size;
        }
    }


    public class EVCConfigurationBox : Box
    {
        public override string FourCC { get { return "evcC"; } }
        public EVCDecoderConfigurationRecord EVCConfig { get; set; }

        public EVCConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.EVCConfig = (EVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.EVCConfig);
            return size;
        }
    }


    public class SVCPriorityLayerInfoBox : Box
    {
        public override string FourCC { get { return "qlif"; } }
        public byte pr_layer_num { get; set; }
        public byte pr_layer { get; set; }
        public uint profile_level_idc { get; set; }
        public uint max_bitrate { get; set; }
        public uint avg_bitrate { get; set; }

        public SVCPriorityLayerInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.pr_layer_num = IsoReaderWriter.ReadUInt8(stream);

            for (int j = 0; j < pr_layer_num; j++)
            {
                this.pr_layer = IsoReaderWriter.ReadUInt8(stream);
                this.profile_level_idc = IsoReaderWriter.ReadUInt24(stream);
                this.max_bitrate = IsoReaderWriter.ReadUInt32(stream);
                this.avg_bitrate = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.pr_layer_num);

            for (int j = 0; j < pr_layer_num; j++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.pr_layer);
                size += IsoReaderWriter.WriteUInt24(stream, this.profile_level_idc);
                size += IsoReaderWriter.WriteUInt32(stream, this.max_bitrate);
                size += IsoReaderWriter.WriteUInt32(stream, this.avg_bitrate);
            }
            return size;
        }
    }


    public class VvcConfigurationBox : FullBox
    {
        public override string FourCC { get { return "vvcC"; } }
        public VvcDecoderConfigurationRecord VvcConfig { get; set; }

        public VvcConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.VvcConfig = (VvcDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.VvcConfig);
            return size;
        }
    }


    public class VvcNALUConfigBox : FullBox
    {
        public override string FourCC { get { return "vvnC"; } }
        public byte reserved { get; set; } = 0;
        public byte LengthSizeMinusOne { get; set; }

        public VvcNALUConfigBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.LengthSizeMinusOne = IsoReaderWriter.ReadBits(stream, 2);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 2, this.LengthSizeMinusOne);
            return size;
        }
    }


    public class DefaultHevcExtractorConstructorBox : FullBox
    {
        public override string FourCC { get { return "dhec"; } }
        public uint num_entries { get; set; }
        public byte constructor_type { get; set; }
        public byte flags { get; set; }

        public DefaultHevcExtractorConstructorBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_entries = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 1; i <= num_entries; i++)
            {
                this.constructor_type = IsoReaderWriter.ReadUInt8(stream);
                this.flags = IsoReaderWriter.ReadUInt8(stream);

                if (constructor_type == 0)
                {
                    // TODO: This should likely be a FullBox: SampleConstructor;

                }

                else if (constructor_type == 2)
                {
                    // TODO: This should likely be a FullBox: InlineConstructor;

                }

                else if (constructor_type == 3)
                {
                    // TODO: This should likely be a FullBox: SampleConstructorFromTrackGroup;

                }

                else if (constructor_type == 6)
                {
                    // TODO: This should likely be a FullBox: NALUStartInlineConstructor;

                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

            for (int i = 1; i <= num_entries; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.constructor_type);
                size += IsoReaderWriter.WriteUInt8(stream, this.flags);

                if (constructor_type == 0)
                {
                    // TODO: This should likely be a FullBox: SampleConstructor;

                }

                else if (constructor_type == 2)
                {
                    // TODO: This should likely be a FullBox: InlineConstructor;

                }

                else if (constructor_type == 3)
                {
                    // TODO: This should likely be a FullBox: SampleConstructorFromTrackGroup;

                }

                else if (constructor_type == 6)
                {
                    // TODO: This should likely be a FullBox: NALUStartInlineConstructor;

                }
            }
            return size;
        }
    }


    public class SVCMetadataSampleConfigBox : FullBox
    {
        public override string FourCC { get { return "svmC"; } }
        public byte sample_statement_type { get; set; }
        public byte default_statement_type { get; set; }
        public byte default_statement_length { get; set; }
        public byte entry_count { get; set; }
        public byte statement_type { get; set; }  //  from the user extension ranges
        public string statement_namespace { get; set; }

        public SVCMetadataSampleConfigBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            this.sample_statement_type = IsoReaderWriter.ReadUInt8(stream);
            /*  normally group, or seq  */
            this.default_statement_type = IsoReaderWriter.ReadUInt8(stream);
            this.default_statement_length = IsoReaderWriter.ReadUInt8(stream);
            this.entry_count = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 1; i <= entry_count; i++)
            {
                this.statement_type = IsoReaderWriter.ReadUInt8(stream);
                this.statement_namespace = IsoReaderWriter.ReadString(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            size += IsoReaderWriter.WriteUInt8(stream, this.sample_statement_type);
            /*  normally group, or seq  */
            size += IsoReaderWriter.WriteUInt8(stream, this.default_statement_type);
            size += IsoReaderWriter.WriteUInt8(stream, this.default_statement_length);
            size += IsoReaderWriter.WriteUInt8(stream, this.entry_count);

            for (int i = 1; i <= entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.statement_type);
                size += IsoReaderWriter.WriteString(stream, this.statement_namespace);
            }
            return size;
        }
    }


    public class EVCSliceComponentTrackConfigurationBox : Box
    {
        public override string FourCC { get { return "evsC"; } }
        public EVCSliceComponentTrackConfigurationRecord config { get; set; }

        public EVCSliceComponentTrackConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (EVCSliceComponentTrackConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.config);
            return size;
        }
    }


    public class WebVTTConfigurationBox : Box
    {
        public override string FourCC { get { return "vttC"; } }
        public string config { get; set; }

        public WebVTTConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.config);
            return size;
        }
    }


    public class WebVTTSourceLabelBox : Box
    {
        public override string FourCC { get { return "vlab"; } }
        public string source_label { get; set; }

        public WebVTTSourceLabelBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.source_label = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.source_label);
            return size;
        }
    }


    public class WVTTSampleEntry : PlainTextSampleEntry
    {
        public override string FourCC { get { return "wvtt"; } }
        public WebVTTConfigurationBox config { get; set; }
        public WebVTTSourceLabelBox label { get; set; }  //  recommended

        public WVTTSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (WebVTTConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.label = (WebVTTSourceLabelBox)IsoReaderWriter.ReadBox(stream);
            // TODO: This should likely be a FullBox: MPEG4BitRateBox; // optional

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.label);
            // TODO: This should likely be a FullBox: MPEG4BitRateBox; // optional

            return size;
        }
    }


    public class AuxiliaryTypeInfoBox : FullBox
    {
        public override string FourCC { get { return "auxi"; } }
        public string aux_track_type { get; set; }

        public AuxiliaryTypeInfoBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.aux_track_type = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.aux_track_type);
            return size;
        }
    }


    public class CodingConstraintsBox : FullBox
    {
        public override string FourCC { get { return "ccst"; } }
        public bool all_ref_pics_intra { get; set; }
        public bool intra_pred_used { get; set; }
        public byte max_ref_per_pic { get; set; }
        public uint reserved { get; set; }

        public CodingConstraintsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.all_ref_pics_intra = IsoReaderWriter.ReadBit(stream);
            this.intra_pred_used = IsoReaderWriter.ReadBit(stream);
            this.max_ref_per_pic = IsoReaderWriter.ReadBits(stream, 4);
            this.reserved = IsoReaderWriter.ReadBits(stream, 26);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.all_ref_pics_intra);
            size += IsoReaderWriter.WriteBit(stream, this.intra_pred_used);
            size += IsoReaderWriter.WriteBits(stream, 4, this.max_ref_per_pic);
            size += IsoReaderWriter.WriteBits(stream, 26, this.reserved);
            return size;
        }
    }


    public class MD5IntegrityBox : FullBox
    {
        public override string FourCC { get { return "md5i"; } }
        public byte[] input_MD5 { get; set; }
        public uint input_4cc { get; set; }
        public uint grouping_type { get; set; }
        public uint grouping_type_parameter { get; set; }
        public uint num_entries { get; set; }
        public uint[] group_description_index { get; set; }

        public MD5IntegrityBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.input_MD5 = IsoReaderWriter.ReadBytes(stream, 16);
            this.input_4cc = IsoReaderWriter.ReadUInt32(stream);

            if (input_4cc == IsoReaderWriter.FromFourCC("sgpd"))
            {
                this.grouping_type = IsoReaderWriter.ReadUInt32(stream);

                if ((flags & 1) == 1)
                {
                    this.grouping_type_parameter = IsoReaderWriter.ReadUInt32(stream);
                }
                this.num_entries = IsoReaderWriter.ReadUInt32(stream);

                for (int i = 0; i < num_entries; i++)
                {
                    this.group_description_index[i] = IsoReaderWriter.ReadUInt32(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBytes(stream, 16, this.input_MD5);
            size += IsoReaderWriter.WriteUInt32(stream, this.input_4cc);

            if (input_4cc == IsoReaderWriter.FromFourCC("sgpd"))
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type);

                if ((flags & 1) == 1)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.grouping_type_parameter);
                }
                size += IsoReaderWriter.WriteUInt32(stream, this.num_entries);

                for (int i = 0; i < num_entries; i++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.group_description_index[i]);
                }
            }
            return size;
        }
    }


    public class AudioSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "enca"; } }
        public uint[] reserved { get; set; } = [];
        public ushort channelcount { get; set; }
        public ushort samplesize { get; set; } = 16;
        public ushort pre_defined { get; set; } = 0;
        public uint samplerate { get; set; } = 0; // = {if track_is_audio 0x0100 else 0}; //  optional boxes follow
        public DownMixInstructions[] DownMixInstructions { get; set; }
        public DRCCoefficientsBasic[] DRCCoefficientsBasic { get; set; }
        public DRCInstructionsBasic[] DRCInstructionsBasic { get; set; }
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC { get; set; }
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC { get; set; }  //  we permit only one DRC Extension box:

        public AudioSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadUInt32Array(stream, 2);
            this.channelcount = IsoReaderWriter.ReadUInt16(stream);
            this.samplesize = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.samplerate = IsoReaderWriter.ReadUInt32(stream);
            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            this.DownMixInstructions = (DownMixInstructions[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsBasic = (DRCCoefficientsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsBasic = (DRCInstructionsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsUniDRC = (DRCCoefficientsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsUniDRC = (DRCInstructionsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: ChannelLayout;

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            size += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32(stream, this.samplerate);
            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            size += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: ChannelLayout;

            return size;
        }
    }


    public class AudioSampleEntryV1 : SampleEntry
    {
        public override string FourCC { get { return "enca"; } }
        public ushort entry_version { get; set; }  //  shall be 1, 
        public ushort[] reserved { get; set; } = [];
        public ushort channelcount { get; set; }  //  shall be correct
        public ushort samplesize { get; set; } = 16;
        public ushort pre_defined { get; set; } = 0;
        public uint samplerate { get; set; } = 1 << 16;  //  optional boxes follow
        public DownMixInstructions[] DownMixInstructions { get; set; }
        public DRCCoefficientsBasic[] DRCCoefficientsBasic { get; set; }
        public DRCInstructionsBasic[] DRCInstructionsBasic { get; set; }
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC { get; set; }
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC { get; set; }  //  we permit only one DRC Extension box:

        public AudioSampleEntryV1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_version = IsoReaderWriter.ReadUInt16(stream);
            /*  and shall be in an stsd with version ==1 */
            this.reserved = IsoReaderWriter.ReadUInt16Array(stream, 3);
            this.channelcount = IsoReaderWriter.ReadUInt16(stream);
            this.samplesize = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.samplerate = IsoReaderWriter.ReadUInt32(stream);
            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            this.DownMixInstructions = (DownMixInstructions[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsBasic = (DRCCoefficientsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsBasic = (DRCInstructionsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsUniDRC = (DRCCoefficientsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsUniDRC = (DRCInstructionsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: ChannelLayout;

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_version);
            /*  and shall be in an stsd with version ==1 */
            size += IsoReaderWriter.WriteUInt16Array(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            size += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32(stream, this.samplerate);
            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            size += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: ChannelLayout;

            return size;
        }
    }


    public class AudioSampleEntryV11 : SampleEntry
    {
        public override string FourCC { get { return "enca"; } }
        public ushort entry_version { get; set; }  //  shall be 1, 
        public ushort[] reserved { get; set; } = [];
        public ushort channelcount { get; set; }  //  shall be correct
        public ushort samplesize { get; set; } = 16;
        public ushort pre_defined { get; set; } = 0;
        public uint samplerate { get; set; } = 1 << 16;  //  optional boxes follow
        public DownMixInstructions[] DownMixInstructions { get; set; }
        public DRCCoefficientsBasic[] DRCCoefficientsBasic { get; set; }
        public DRCInstructionsBasic[] DRCInstructionsBasic { get; set; }
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC { get; set; }
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC { get; set; }  //  we permit only one DRC Extension box:

        public AudioSampleEntryV11()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_version = IsoReaderWriter.ReadUInt16(stream);
            /*  and shall be in an stsd with version ==1 */
            this.reserved = IsoReaderWriter.ReadUInt16Array(stream, 3);
            this.channelcount = IsoReaderWriter.ReadUInt16(stream);
            this.samplesize = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.samplerate = IsoReaderWriter.ReadUInt32(stream);
            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            this.DownMixInstructions = (DownMixInstructions[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsBasic = (DRCCoefficientsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsBasic = (DRCInstructionsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsUniDRC = (DRCCoefficientsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsUniDRC = (DRCInstructionsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: ChannelLayout;

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_version);
            /*  and shall be in an stsd with version ==1 */
            size += IsoReaderWriter.WriteUInt16Array(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            size += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32(stream, this.samplerate);
            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            size += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: ChannelLayout;

            return size;
        }
    }


    public class FontSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "encf"; } }

        public FontSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            /* other boxes from derived specifications */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            /* other boxes from derived specifications */
            return size;
        }
    }


    public class MetaDataSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "encm"; } }

        public MetaDataSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class XMLMetaDataSampleEntry : MetaDataSampleEntry
    {
        public override string FourCC { get { return "metx"; } }
        public string content_encoding { get; set; }  //  optional
        public string ns { get; set; }
        public string schema_location { get; set; }  //  optional

        public XMLMetaDataSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.content_encoding = IsoReaderWriter.ReadString(stream);
            this.ns = IsoReaderWriter.ReadString(stream);
            this.schema_location = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.content_encoding);
            size += IsoReaderWriter.WriteString(stream, this.ns);
            size += IsoReaderWriter.WriteString(stream, this.schema_location);
            return size;
        }
    }


    public class TextMetaDataSampleEntry : MetaDataSampleEntry
    {
        public override string FourCC { get { return "mett"; } }
        public string content_encoding { get; set; }  //  optional
        public string mime_format { get; set; }

        public TextMetaDataSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.content_encoding = IsoReaderWriter.ReadString(stream);
            this.mime_format = IsoReaderWriter.ReadString(stream);
            // TODO: This should likely be a FullBox: TextConfigBox; // optional

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.content_encoding);
            size += IsoReaderWriter.WriteString(stream, this.mime_format);
            // TODO: This should likely be a FullBox: TextConfigBox; // optional

            return size;
        }
    }


    public class URIMetaSampleEntry : MetaDataSampleEntry
    {
        public override string FourCC { get { return "urim"; } }
        public URIBox the_label { get; set; }
        public URIInitBox init { get; set; }  //  optional

        public URIMetaSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.the_label = (URIBox)IsoReaderWriter.ReadBox(stream);
            this.init = (URIInitBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.the_label);
            size += IsoReaderWriter.WriteBox(stream, this.init);
            return size;
        }
    }


    public class BoxedMetadataSampleEntry : MetadataSampleEntry
    {
        public override string FourCC { get { return "mebx"; } }

        public BoxedMetadataSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            // TODO: This should likely be a FullBox: MetadataKeyTableBox; // mandatory

            // TODO: This should likely be a FullBox: BitRateBox; // optional

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            // TODO: This should likely be a FullBox: MetadataKeyTableBox; // mandatory

            // TODO: This should likely be a FullBox: BitRateBox; // optional

            return size;
        }
    }


    public class FDHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "fdp "; } }
        public ushort hinttrackversion { get; set; } = 1;
        public ushort highestcompatibleversion { get; set; } = 1;
        public ushort partition_entry_ID { get; set; }
        public ushort FEC_overhead { get; set; }

        public FDHintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hinttrackversion = IsoReaderWriter.ReadUInt16(stream);
            this.highestcompatibleversion = IsoReaderWriter.ReadUInt16(stream);
            this.partition_entry_ID = IsoReaderWriter.ReadUInt16(stream);
            this.FEC_overhead = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            size += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            size += IsoReaderWriter.WriteUInt16(stream, this.partition_entry_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.FEC_overhead);
            return size;
        }
    }


    public class IncompleteAVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "icpv"; } }
        public AVCConfigurationBox config { get; set; }

        public IncompleteAVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            // TODO: This should likely be a FullBox: CompleteTrackInfoBox;

            this.config = (AVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            // TODO: This should likely be a FullBox: CompleteTrackInfoBox;

            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class ProtectedMPEG2TransportStreamSampleEntry : MPEG2TSSampleEntry
    {
        public override string FourCC { get { return "pm2t"; } }
        public ProtectionSchemeInfoBox SchemeInformation { get; set; }

        public ProtectedMPEG2TransportStreamSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.SchemeInformation = (ProtectionSchemeInfoBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.SchemeInformation);
            return size;
        }
    }


    public class ProtectedRtpReceptionHintSampleEntry : RtpReceptionHintSampleEntry
    {
        public override string FourCC { get { return "prtp"; } }
        public ProtectionSchemeInfoBox SchemeInformation { get; set; }

        public ProtectedRtpReceptionHintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.SchemeInformation = (ProtectionSchemeInfoBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.SchemeInformation);
            return size;
        }
    }


    public class MPEG2TSReceptionSampleEntry : MPEG2TSSampleEntry
    {
        public override string FourCC { get { return "rm2t"; } }

        public MPEG2TSReceptionSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class ReceivedRtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "rrtp"; } }
        public ushort hinttrackversion { get; set; } = 1;
        public ushort highestcompatibleversion { get; set; } = 1;
        public uint maxpacketsize { get; set; }

        public ReceivedRtpHintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hinttrackversion = IsoReaderWriter.ReadUInt16(stream);
            this.highestcompatibleversion = IsoReaderWriter.ReadUInt16(stream);
            this.maxpacketsize = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            size += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return size;
        }
    }


    public class ReceivedSrtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "rsrp"; } }
        public ushort hinttrackversion { get; set; } = 1;
        public ushort highestcompatibleversion { get; set; } = 1;
        public uint maxpacketsize { get; set; }

        public ReceivedSrtpHintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hinttrackversion = IsoReaderWriter.ReadUInt16(stream);
            this.highestcompatibleversion = IsoReaderWriter.ReadUInt16(stream);
            this.maxpacketsize = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            size += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return size;
        }
    }


    public class HintSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "rtcp"; } }

        public HintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class moviehintinformation1 : Box
    {
        public override string FourCC { get { return "rtp "; } }

        public moviehintinformation1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class rtpmoviehintinformation1 : Box
    {
        public override string FourCC { get { return "rtp "; } }
        public uint descriptionformat { get; set; } = "sdp ";
        public sbyte[] sdptext { get; set; }

        public rtpmoviehintinformation1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.descriptionformat = IsoReaderWriter.ReadUInt32(stream);
            this.sdptext = IsoReaderWriter.ReadInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.descriptionformat);
            size += IsoReaderWriter.WriteInt8Array(stream, this.sdptext);
            return size;
        }
    }


    public class TextSubtitleSampleEntry : SubtitleSampleEntry
    {
        public override string FourCC { get { return "sbtt"; } }
        public string content_encoding { get; set; }  //  optional
        public string mime_format { get; set; }

        public TextSubtitleSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.content_encoding = IsoReaderWriter.ReadString(stream);
            this.mime_format = IsoReaderWriter.ReadString(stream);
            // TODO: This should likely be a FullBox: TextConfigBox; // optional

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.content_encoding);
            size += IsoReaderWriter.WriteString(stream, this.mime_format);
            // TODO: This should likely be a FullBox: TextConfigBox; // optional

            return size;
        }
    }


    public class MPEG2TSServerSampleEntry : MPEG2TSSampleEntry
    {
        public override string FourCC { get { return "sm2t"; } }

        public MPEG2TSServerSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class SrtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "srtp"; } }
        public ushort hinttrackversion { get; set; } = 1;
        public ushort highestcompatibleversion { get; set; } = 1;
        public uint maxpacketsize { get; set; }

        public SrtpHintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hinttrackversion = IsoReaderWriter.ReadUInt16(stream);
            this.highestcompatibleversion = IsoReaderWriter.ReadUInt16(stream);
            this.maxpacketsize = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            size += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return size;
        }
    }


    public class XMLSubtitleSampleEntry : SubtitleSampleEntry
    {
        public override string FourCC { get { return "stpp"; } }
        public string ns { get; set; }
        public string schema_location { get; set; }  //  optional
        public string auxiliary_mime_types { get; set; }  //  optional, required if auxiliary resources are present

        public XMLSubtitleSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.ns = IsoReaderWriter.ReadString(stream);
            this.schema_location = IsoReaderWriter.ReadString(stream);
            this.auxiliary_mime_types = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.ns);
            size += IsoReaderWriter.WriteString(stream, this.schema_location);
            size += IsoReaderWriter.WriteString(stream, this.auxiliary_mime_types);
            return size;
        }
    }


    public class SimpleTextSampleEntry : PlainTextSampleEntry
    {
        public override string FourCC { get { return "stxt"; } }
        public string content_encoding { get; set; }  //  optional
        public string mime_format { get; set; }

        public SimpleTextSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.content_encoding = IsoReaderWriter.ReadString(stream);
            this.mime_format = IsoReaderWriter.ReadString(stream);
            // TODO: This should likely be a FullBox: TextConfigBox; // optional

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.content_encoding);
            size += IsoReaderWriter.WriteString(stream, this.mime_format);
            // TODO: This should likely be a FullBox: TextConfigBox; // optional

            return size;
        }
    }


    public class HapticSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "encp"; } }
        public Box[] otherboxes { get; set; }

        public HapticSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.otherboxes = IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.otherboxes);
            return size;
        }
    }


    public class VolumetricVisualSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "enc3"; } }
        public byte[] compressorname { get; set; }  //  other boxes from derived specifications

        public VolumetricVisualSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.compressorname = IsoReaderWriter.ReadBytes(stream, 32);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBytes(stream, 32, this.compressorname);
            return size;
        }
    }


    public class VisualSampleEntry : SampleEntry
    {
        public override string FourCC { get { return "resv"; } }
        public uint[] pre_defined { get; set; } = [];
        public uint reserved { get; set; } = 0;
        public ushort width { get; set; }
        public ushort height { get; set; }
        public uint horizresolution { get; set; } = 0x00480000;  //  72 dpi
        public uint vertresolution { get; set; } = 0x00480000;  //  72 dpi
        public ushort frame_count { get; set; } = 1;
        public byte[] compressorname { get; set; }
        public ushort depth { get; set; } = 0x0018;
        public CleanApertureBox clap { get; set; }  //  optional
        public PixelAspectRatioBox pasp { get; set; }  //  optional

        public VisualSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt32Array(stream, 3);
            this.width = IsoReaderWriter.ReadUInt16(stream);
            this.height = IsoReaderWriter.ReadUInt16(stream);
            this.horizresolution = IsoReaderWriter.ReadUInt32(stream);
            this.vertresolution = IsoReaderWriter.ReadUInt32(stream);
            this.reserved = IsoReaderWriter.ReadUInt32(stream);
            this.frame_count = IsoReaderWriter.ReadUInt16(stream);
            this.compressorname = IsoReaderWriter.ReadBytes(stream, 32);
            this.depth = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadInt16(stream);
            this.clap = (CleanApertureBox)IsoReaderWriter.ReadBox(stream);
            this.pasp = (PixelAspectRatioBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32Array(stream, 3, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.width);
            size += IsoReaderWriter.WriteUInt16(stream, this.height);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizresolution);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertresolution);
            size += IsoReaderWriter.WriteUInt32(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.frame_count);
            size += IsoReaderWriter.WriteBytes(stream, 32, this.compressorname);
            size += IsoReaderWriter.WriteUInt16(stream, this.depth);
            size += IsoReaderWriter.WriteInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteBox(stream, this.clap);
            size += IsoReaderWriter.WriteBox(stream, this.pasp);
            return size;
        }
    }


    public class AudioSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resa"; } }
        public uint[] reserved { get; set; } = [];
        public ushort channelcount { get; set; }
        public ushort samplesize { get; set; } = 16;
        public ushort pre_defined { get; set; } = 0;
        public uint samplerate { get; set; } = 0; // = {if track_is_audio 0x0100 else 0}; //  optional boxes follow
        public DownMixInstructions[] DownMixInstructions { get; set; }
        public DRCCoefficientsBasic[] DRCCoefficientsBasic { get; set; }
        public DRCInstructionsBasic[] DRCInstructionsBasic { get; set; }
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC { get; set; }
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC { get; set; }  //  we permit only one DRC Extension box:

        public AudioSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadUInt32Array(stream, 2);
            this.channelcount = IsoReaderWriter.ReadUInt16(stream);
            this.samplesize = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.samplerate = IsoReaderWriter.ReadUInt32(stream);
            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            this.DownMixInstructions = (DownMixInstructions[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsBasic = (DRCCoefficientsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsBasic = (DRCInstructionsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsUniDRC = (DRCCoefficientsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsUniDRC = (DRCInstructionsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: ChannelLayout;

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            size += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32(stream, this.samplerate);
            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            size += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: ChannelLayout;

            return size;
        }
    }


    public class AudioSampleEntryV12 : SampleEntry
    {
        public override string FourCC { get { return "resa"; } }
        public ushort entry_version { get; set; }  //  shall be 1, 
        public ushort[] reserved { get; set; } = [];
        public ushort channelcount { get; set; }  //  shall be correct
        public ushort samplesize { get; set; } = 16;
        public ushort pre_defined { get; set; } = 0;
        public uint samplerate { get; set; } = 1 << 16;  //  optional boxes follow
        public DownMixInstructions[] DownMixInstructions { get; set; }
        public DRCCoefficientsBasic[] DRCCoefficientsBasic { get; set; }
        public DRCInstructionsBasic[] DRCInstructionsBasic { get; set; }
        public DRCCoefficientsUniDRC[] DRCCoefficientsUniDRC { get; set; }
        public DRCInstructionsUniDRC[] DRCInstructionsUniDRC { get; set; }  //  we permit only one DRC Extension box:

        public AudioSampleEntryV12()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_version = IsoReaderWriter.ReadUInt16(stream);
            /*  and shall be in an stsd with version ==1 */
            this.reserved = IsoReaderWriter.ReadUInt16Array(stream, 3);
            this.channelcount = IsoReaderWriter.ReadUInt16(stream);
            this.samplesize = IsoReaderWriter.ReadUInt16(stream);
            this.pre_defined = IsoReaderWriter.ReadUInt16(stream);
            this.reserved = IsoReaderWriter.ReadUInt16(stream);
            this.samplerate = IsoReaderWriter.ReadUInt32(stream);
            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            this.DownMixInstructions = (DownMixInstructions[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsBasic = (DRCCoefficientsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsBasic = (DRCInstructionsBasic[])IsoReaderWriter.ReadClasses(stream);
            this.DRCCoefficientsUniDRC = (DRCCoefficientsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            this.DRCInstructionsUniDRC = (DRCInstructionsUniDRC[])IsoReaderWriter.ReadClasses(stream);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: ChannelLayout;

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_version);
            /*  and shall be in an stsd with version ==1 */
            size += IsoReaderWriter.WriteUInt16Array(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.channelcount);
            size += IsoReaderWriter.WriteUInt16(stream, this.samplesize);
            size += IsoReaderWriter.WriteUInt16(stream, this.pre_defined);
            size += IsoReaderWriter.WriteUInt16(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt32(stream, this.samplerate);
            // TODO: This should likely be a FullBox: SamplingRateBox;

            // TODO: This should likely be a FullBox: Box; // further boxes as needed

            // TODO: This should likely be a FullBox: ChannelLayout;

            size += IsoReaderWriter.WriteClasses(stream, this.DownMixInstructions);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsBasic);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCCoefficientsUniDRC);
            size += IsoReaderWriter.WriteClasses(stream, this.DRCInstructionsUniDRC);
            // TODO: This should likely be a FullBox: UniDrcConfigExtension; // optional boxes follow

            // TODO: This should likely be a FullBox: ChannelLayout;

            return size;
        }
    }


    public class MetaDataSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resm"; } }

        public MetaDataSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class FontSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resf"; } }

        public FontSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            /* other boxes from derived specifications */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            /* other boxes from derived specifications */
            return size;
        }
    }


    public class HapticSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "resp"; } }
        public Box[] otherboxes { get; set; }

        public HapticSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.otherboxes = IsoReaderWriter.ReadBoxes(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBoxes(stream, this.otherboxes);
            return size;
        }
    }


    public class VolumetricVisualSampleEntry1 : SampleEntry
    {
        public override string FourCC { get { return "res3"; } }
        public byte[] compressorname { get; set; }  //  other boxes from derived specifications

        public VolumetricVisualSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.compressorname = IsoReaderWriter.ReadBytes(stream, 32);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBytes(stream, 32, this.compressorname);
            return size;
        }
    }


    public class RtpHintSampleEntry : HintSampleEntry
    {
        public override string FourCC { get { return "rtp "; } }
        public ushort hinttrackversion { get; set; } = 1;
        public ushort highestcompatibleversion { get; set; } = 1;
        public uint maxpacketsize { get; set; }

        public RtpHintSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hinttrackversion = IsoReaderWriter.ReadUInt16(stream);
            this.highestcompatibleversion = IsoReaderWriter.ReadUInt16(stream);
            this.maxpacketsize = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.hinttrackversion);
            size += IsoReaderWriter.WriteUInt16(stream, this.highestcompatibleversion);
            size += IsoReaderWriter.WriteUInt32(stream, this.maxpacketsize);
            return size;
        }
    }


    public class EntityToGroupBox : FullBox
    {
        public override string FourCC { get { return "altr"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class BrandProperty : GeneralTypeBox
    {
        public override string FourCC { get { return "brnd"; } }

        public BrandProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class SingleItemTypeReferenceBox : Box
    {
        public override string FourCC { get { return "fdel"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge : Box
    {
        public override string FourCC { get { return "fdel"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox1 : Box
    {
        public override string FourCC { get { return "iloc"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge1 : Box
    {
        public override string FourCC { get { return "iloc"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class AlternativeStartupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "alst"; } }
        public ushort roll_count { get; set; }
        public ushort first_output_sample { get; set; }
        public uint[] sample_offset { get; set; }
        public ushort[] num_output_samples { get; set; }
        public ushort[] num_total_samples { get; set; }

        public AlternativeStartupEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.roll_count = IsoReaderWriter.ReadUInt16(stream);
            this.first_output_sample = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 1; i <= roll_count; i++)
            {
                this.sample_offset[i] = IsoReaderWriter.ReadUInt32(stream);
            }
            int j = 1;

            while (true)
            {
                /*  optional, until the end of the structure */
                this.num_output_samples[j] = IsoReaderWriter.ReadUInt16(stream);
                this.num_total_samples[j] = IsoReaderWriter.ReadUInt16(stream);
                j++;
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.roll_count);
            size += IsoReaderWriter.WriteUInt16(stream, this.first_output_sample);

            for (int i = 1; i <= roll_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_offset[i]);
            }
            int j = 1;

            while (true)
            {
                /*  optional, until the end of the structure */
                size += IsoReaderWriter.WriteUInt16(stream, this.num_output_samples[j]);
                size += IsoReaderWriter.WriteUInt16(stream, this.num_total_samples[j]);
                j++;
            }
            return size;
        }
    }


    public class VisualDRAPEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "drap"; } }
        public byte DRAP_type { get; set; }
        public uint reserved { get; set; } = 0;

        public VisualDRAPEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.DRAP_type = IsoReaderWriter.ReadBits(stream, 3);
            this.reserved = IsoReaderWriter.ReadBits(stream, 29);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 3, this.DRAP_type);
            size += IsoReaderWriter.WriteBits(stream, 29, this.reserved);
            return size;
        }
    }


    public class AudioPreRollEntry : AudioSampleGroupEntry
    {
        public override string FourCC { get { return "prol"; } }
        public short roll_distance { get; set; }

        public AudioPreRollEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.roll_distance = IsoReaderWriter.ReadInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt16(stream, this.roll_distance);
            return size;
        }
    }


    public class VisualRandomAccessEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "rap "; } }
        public bool num_leading_samples_known { get; set; }
        public byte num_leading_samples { get; set; }

        public VisualRandomAccessEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_leading_samples_known = IsoReaderWriter.ReadBit(stream);
            this.num_leading_samples = IsoReaderWriter.ReadBits(stream, 7);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.num_leading_samples_known);
            size += IsoReaderWriter.WriteBits(stream, 7, this.num_leading_samples);
            return size;
        }
    }


    public class RateShareEntry : SampleGroupDescriptionEntry
    {
        public override string FourCC { get { return "rash"; } }
        public ushort operation_point_count { get; set; }
        public ushort target_rate_share { get; set; }
        public uint available_bitrate { get; set; }
        public uint maximum_bitrate { get; set; }
        public uint minimum_bitrate { get; set; }
        public byte discard_priority { get; set; }

        public RateShareEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.operation_point_count = IsoReaderWriter.ReadUInt16(stream);

            if (operation_point_count == 1)
            {
                this.target_rate_share = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {

                for (int i = 0; i < operation_point_count; i++)
                {
                    this.available_bitrate = IsoReaderWriter.ReadUInt32(stream);
                    this.target_rate_share = IsoReaderWriter.ReadUInt16(stream);
                }
            }
            this.maximum_bitrate = IsoReaderWriter.ReadUInt32(stream);
            this.minimum_bitrate = IsoReaderWriter.ReadUInt32(stream);
            this.discard_priority = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.operation_point_count);

            if (operation_point_count == 1)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.target_rate_share);
            }

            else
            {

                for (int i = 0; i < operation_point_count; i++)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.available_bitrate);
                    size += IsoReaderWriter.WriteUInt16(stream, this.target_rate_share);
                }
            }
            size += IsoReaderWriter.WriteUInt32(stream, this.maximum_bitrate);
            size += IsoReaderWriter.WriteUInt32(stream, this.minimum_bitrate);
            size += IsoReaderWriter.WriteUInt8(stream, this.discard_priority);
            return size;
        }
    }


    public class AudioRollRecoveryEntry : AudioSampleGroupEntry
    {
        public override string FourCC { get { return "roll"; } }
        public short roll_distance { get; set; }

        public AudioRollRecoveryEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.roll_distance = IsoReaderWriter.ReadInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt16(stream, this.roll_distance);
            return size;
        }
    }


    public class SAPEntry : SampleGroupDescriptionEntry
    {
        public override string FourCC { get { return "sap "; } }
        public bool dependent_flag { get; set; }
        public byte reserved { get; set; }
        public byte SAP_type { get; set; }

        public SAPEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.dependent_flag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 3);
            this.SAP_type = IsoReaderWriter.ReadBits(stream, 4);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.dependent_flag);
            size += IsoReaderWriter.WriteBits(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 4, this.SAP_type);
            return size;
        }
    }


    public class SampleToMetadataItemEntry : SampleGroupDescriptionEntry
    {
        public override string FourCC { get { return "stmi"; } }
        public uint meta_box_handler_type { get; set; }
        public uint num_items { get; set; }
        public uint[] item_id { get; set; }

        public SampleToMetadataItemEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.meta_box_handler_type = IsoReaderWriter.ReadUInt32(stream);
            this.num_items = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_items; i++)
            {
                this.item_id[i] = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.meta_box_handler_type);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_items);

            for (int i = 0; i < num_items; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.item_id[i]);
            }
            return size;
        }
    }


    public class TemporalLevelEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "tele"; } }
        public bool level_independently_decodable { get; set; }
        public byte reserved { get; set; } = 0;

        public TemporalLevelEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.level_independently_decodable = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 7);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.level_independently_decodable);
            size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            return size;
        }
    }


    public class PixelAspectRatioEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pasr"; } }
        public uint hSpacing { get; set; }
        public uint vSpacing { get; set; }

        public PixelAspectRatioEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hSpacing = IsoReaderWriter.ReadUInt32(stream);
            this.vSpacing = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.hSpacing);
            size += IsoReaderWriter.WriteUInt32(stream, this.vSpacing);
            return size;
        }
    }


    public class CleanApertureEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "casg"; } }
        public uint cleanApertureWidthN { get; set; }
        public uint cleanApertureWidthD { get; set; }
        public uint cleanApertureHeightN { get; set; }
        public uint cleanApertureHeightD { get; set; }
        public uint horizOffN { get; set; }
        public uint horizOffD { get; set; }
        public uint vertOffN { get; set; }
        public uint vertOffD { get; set; }

        public CleanApertureEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.cleanApertureWidthN = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureWidthD = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureHeightN = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureHeightD = IsoReaderWriter.ReadUInt32(stream);
            this.horizOffN = IsoReaderWriter.ReadUInt32(stream);
            this.horizOffD = IsoReaderWriter.ReadUInt32(stream);
            this.vertOffN = IsoReaderWriter.ReadUInt32(stream);
            this.vertOffD = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthN);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthD);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightN);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightD);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizOffN);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizOffD);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertOffN);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertOffD);
            return size;
        }
    }


    public class TrackGroupTypeBox : FullBox
    {
        public override string FourCC { get { return "msrc"; } }
        public uint track_group_id { get; set; }  //  the remaining data may be specified 

        public TrackGroupTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_group_id = IsoReaderWriter.ReadUInt32(stream);
            /*   for a particular track_group_type */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_group_id);
            /*   for a particular track_group_type */
            return size;
        }
    }


    public class StereoVideoGroupBox : TrackGroupTypeBox
    {
        public override string FourCC { get { return "ster"; } }
        public bool left_view_flag { get; set; }
        public uint reserved { get; set; }

        public StereoVideoGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.left_view_flag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 31);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.left_view_flag);
            size += IsoReaderWriter.WriteBits(stream, 31, this.reserved);
            return size;
        }
    }


    public class TrackReferenceTypeBox : Box
    {
        public override string FourCC { get { return "auxl"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox1 : Box
    {
        public override string FourCC { get { return "font"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox2 : Box
    {
        public override string FourCC { get { return "hind"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox3 : Box
    {
        public override string FourCC { get { return "hint"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox4 : Box
    {
        public override string FourCC { get { return "subt"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox5 : Box
    {
        public override string FourCC { get { return "thmb"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox6 : Box
    {
        public override string FourCC { get { return "vdep"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox6()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox7 : Box
    {
        public override string FourCC { get { return "vplx"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox7()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox8 : Box
    {
        public override string FourCC { get { return "cdsc"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox8()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox9 : Box
    {
        public override string FourCC { get { return "adda"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox9()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox10 : Box
    {
        public override string FourCC { get { return "adrc"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox10()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class HEVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvc1"; } }
        public HEVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public HEVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCLHVCSampleEntry : HEVCSampleEntry
    {
        public override string FourCC { get { return "hvc1"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }

        public HEVCLHVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return size;
        }
    }


    public class HEVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "hvc2"; } }
        public HEVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public HEVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCLHVCSampleEntry1 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hvc2"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }

        public HEVCLHVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return size;
        }
    }


    public class HEVCSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "hvc3"; } }
        public HEVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public HEVCSampleEntry2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCLHVCSampleEntry2 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hvc3"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }

        public HEVCLHVCSampleEntry2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return size;
        }
    }


    public class LHEVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "lhv1"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public LHEVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class LHEVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "lhe1"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public LHEVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "hev1"; } }
        public HEVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public HEVCSampleEntry3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCLHVCSampleEntry3 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hev1"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }

        public HEVCLHVCSampleEntry3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return size;
        }
    }


    public class HEVCSampleEntry4 : VisualSampleEntry
    {
        public override string FourCC { get { return "hev2"; } }
        public HEVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public HEVCSampleEntry4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCLHVCSampleEntry4 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hev2"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }

        public HEVCLHVCSampleEntry4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return size;
        }
    }


    public class HEVCSampleEntry5 : VisualSampleEntry
    {
        public override string FourCC { get { return "hev3"; } }
        public HEVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public HEVCSampleEntry5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class HEVCLHVCSampleEntry5 : HEVCSampleEntry
    {
        public override string FourCC { get { return "hev3"; } }
        public LHEVCConfigurationBox lhvcconfig { get; set; }

        public HEVCLHVCSampleEntry5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lhvcconfig = (LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.lhvcconfig);
            return size;
        }
    }


    public class AVCParameterSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "avcp"; } }
        public AVCConfigurationBox config { get; set; }

        public AVCParameterSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (AVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class AVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "avc1"; } }
        public AVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public AVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (AVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class AVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "avc3"; } }
        public AVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public AVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (AVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class AVCMVCSampleEntry : AVCSampleEntry
    {
        public override string FourCC { get { return "avc1"; } }
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  optional
        public MVCConfigurationBox mvcconfig { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public AVCMVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class AVCMVCSampleEntry1 : AVCSampleEntry
    {
        public override string FourCC { get { return "avc3"; } }
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  optional
        public MVCConfigurationBox mvcconfig { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public AVCMVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class AVC2SampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "avc2"; } }
        public AVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public AVC2SampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (AVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class AVC2SampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "avc4"; } }
        public AVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public AVC2SampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (AVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class AVC2MVCSampleEntry : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc2"; } }
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  optional
        public MVCConfigurationBox mvcconfig { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public AVC2MVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class AVC2MVCSampleEntry1 : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc4"; } }
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  optional
        public MVCConfigurationBox mvcconfig { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public AVC2MVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc1"; } }
        public MVCConfigurationBox mvcconfig { get; set; }  //  mandatory
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc2"; } }
        public MVCConfigurationBox mvcconfig { get; set; }  //  mandatory
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc3"; } }
        public MVCConfigurationBox mvcconfig { get; set; }  //  mandatory
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCSampleEntry2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvc4"; } }
        public MVCConfigurationBox mvcconfig { get; set; }  //  mandatory
        public ViewScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public MVCViewPriorityAssignmentBox view_priority_method { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  optional
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCSampleEntry3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcconfig = (MVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.view_priority_method = (MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.view_priority_method);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCDSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd1"; } }
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCDSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCDSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd2"; } }
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCDSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCDSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd3"; } }
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCDSampleEntry2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class MVCDSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "mvd4"; } }
        public MVCDConfigurationBox mvcdconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional
        public A3DConfigurationBox a3dconfig { get; set; }  //  optional

        public MVCDSampleEntry3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.mvcdconfig = (MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.mvcdconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            return size;
        }
    }


    public class A3DSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d1"; } }
        public A3DConfigurationBox a3dconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional

        public A3DSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            return size;
        }
    }


    public class A3DSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d2"; } }
        public A3DConfigurationBox a3dconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional

        public A3DSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            return size;
        }
    }


    public class A3DSampleEntry2 : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d3"; } }
        public A3DConfigurationBox a3dconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional

        public A3DSampleEntry2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            return size;
        }
    }


    public class A3DSampleEntry3 : VisualSampleEntry
    {
        public override string FourCC { get { return "a3d4"; } }
        public A3DConfigurationBox a3dconfig { get; set; }  //  mandatory
        public MVDScalabilityInformationSEIBox mvdscalinfosei { get; set; }  //  optional
        public ViewIdentifierBox view_identifiers { get; set; }  //  mandatory
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public IntrinsicCameraParametersBox intrinsic_camera_params { get; set; }  //  optional
        public ExtrinsicCameraParametersBox extrinsic_camera_params { get; set; }  //  optional

        public A3DSampleEntry3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.a3dconfig = (A3DConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.mvdscalinfosei = (MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.view_identifiers = (ViewIdentifierBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.intrinsic_camera_params = (IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
            this.extrinsic_camera_params = (ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.a3dconfig);
            size += IsoReaderWriter.WriteBox(stream, this.mvdscalinfosei);
            size += IsoReaderWriter.WriteBox(stream, this.view_identifiers);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.intrinsic_camera_params);
            size += IsoReaderWriter.WriteBox(stream, this.extrinsic_camera_params);
            return size;
        }
    }


    public class AVCSVCSampleEntry : AVCSampleEntry
    {
        public override string FourCC { get { return "avc1"; } }
        public SVCConfigurationBox svcconfig { get; set; }  //  optional
        public ScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public SVCPriorityAssignmentBox method { get; set; }  //  optional

        public AVCSVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.svcconfig = (SVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.method = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.method);
            return size;
        }
    }


    public class AVCSVCSampleEntry1 : AVCSampleEntry
    {
        public override string FourCC { get { return "avc3"; } }
        public SVCConfigurationBox svcconfig { get; set; }  //  optional
        public ScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public SVCPriorityAssignmentBox method { get; set; }  //  optional

        public AVCSVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.svcconfig = (SVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.method = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.method);
            return size;
        }
    }


    public class AVC2SVCSampleEntry : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc2"; } }
        public SVCConfigurationBox svcconfig { get; set; }  //  optional
        public ScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public SVCPriorityAssignmentBox method { get; set; }  //  optional

        public AVC2SVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.svcconfig = (SVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.method = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.method);
            return size;
        }
    }


    public class AVC2SVCSampleEntry1 : AVC2SampleEntry
    {
        public override string FourCC { get { return "avc4"; } }
        public SVCConfigurationBox svcconfig { get; set; }  //  optional
        public ScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public SVCPriorityAssignmentBox method { get; set; }  //  optional

        public AVC2SVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.svcconfig = (SVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.method = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.method);
            return size;
        }
    }


    public class SVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "svc1"; } }
        public SVCConfigurationBox svcconfig { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public ScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public SVCPriorityAssignmentBox method { get; set; }  //  optional

        public SVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.svcconfig = (SVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.method = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.method);
            return size;
        }
    }


    public class SVCSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "svc2"; } }
        public SVCConfigurationBox svcconfig { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional
        public ScalabilityInformationSEIBox scalability { get; set; }  //  optional
        public SVCPriorityAssignmentBox method { get; set; }  //  optional

        public SVCSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.svcconfig = (SVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
            this.scalability = (ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream);
            this.method = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.svcconfig);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            size += IsoReaderWriter.WriteBox(stream, this.scalability);
            size += IsoReaderWriter.WriteBox(stream, this.method);
            return size;
        }
    }


    public class HEVCTileSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvt1"; } }
        public HEVCTileConfigurationBox config { get; set; }  //  optional

        public HEVCTileSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCTileConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class LHEVCTileSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "lht1"; } }

        public LHEVCTileSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class HEVCTileSSHInfoSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvt3"; } }
        public HEVCTileConfigurationBox config { get; set; }  //  optional 

        public HEVCTileSSHInfoSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCTileConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class HEVCSliceSegmentDataSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "hvt2"; } }
        public HEVCTileConfigurationBox config { get; set; }  //  optional

        public HEVCSliceSegmentDataSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (HEVCTileConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class VvcSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "vvc1"; } }
        public VvcConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public VvcSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (VvcConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class VvcSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "vvi1"; } }
        public VvcConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public VvcSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (VvcConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class VvcSubpicSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "vvs1"; } }
        public VvcNALUConfigBox config { get; set; }

        public VvcSubpicSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (VvcNALUConfigBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class VvcNonVCLSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "vvcN"; } }
        public VvcNALUConfigBox config { get; set; }

        public VvcNonVCLSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (VvcNALUConfigBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class EVCSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "evc1"; } }
        public EVCConfigurationBox config { get; set; }
        public MPEG4ExtensionDescriptorsBox descr { get; set; }  //  optional

        public EVCSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (EVCConfigurationBox)IsoReaderWriter.ReadBox(stream);
            this.descr = (MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.descr);
            return size;
        }
    }


    public class SVCMetadataSampleEntry : MetadataSampleEntry
    {
        public override string FourCC { get { return "svcM"; } }
        public SVCMetadataSampleConfigBox config { get; set; }
        public SVCPriorityAssignmentBox methods { get; set; }  //  optional
        public SVCPriorityLayerInfoBox priorities { get; set; }  //  optional

        public SVCMetadataSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (SVCMetadataSampleConfigBox)IsoReaderWriter.ReadBox(stream);
            this.methods = (SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream);
            this.priorities = (SVCPriorityLayerInfoBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            size += IsoReaderWriter.WriteBox(stream, this.methods);
            size += IsoReaderWriter.WriteBox(stream, this.priorities);
            return size;
        }
    }


    public class EVCSliceComponentTrackSampleEntry : VisualSampleEntry
    {
        public override string FourCC { get { return "evs1"; } }
        public EVCSliceComponentTrackConfigurationBox config { get; set; }

        public EVCSliceComponentTrackSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (EVCSliceComponentTrackConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class EVCSliceComponentTrackSampleEntry1 : VisualSampleEntry
    {
        public override string FourCC { get { return "evs2"; } }
        public EVCSliceComponentTrackConfigurationBox config { get; set; }

        public EVCSliceComponentTrackSampleEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.config = (EVCSliceComponentTrackConfigurationBox)IsoReaderWriter.ReadBox(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBox(stream, this.config);
            return size;
        }
    }


    public class SubpicCommonGroupBox : EntityToGroupBox
    {
        public override string FourCC { get { return "acgl"; } }
        public bool level_is_present_flag { get; set; }
        public bool level_is_static_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public byte level_idc { get; set; }
        public uint level_info_entity_idx { get; set; }
        public ushort num_active_tracks { get; set; }

        public SubpicCommonGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.level_is_present_flag = IsoReaderWriter.ReadBit(stream);
            this.level_is_static_flag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);

            if (level_is_present_flag)
            {
                this.level_idc = IsoReaderWriter.ReadUInt8(stream);
            }

            if (level_is_static_flag == 0)
            {
                this.level_info_entity_idx = IsoReaderWriter.ReadUInt32(stream);
            }
            this.num_active_tracks = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.level_is_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.level_is_static_flag);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);

            if (level_is_present_flag)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.level_idc);
            }

            if (level_is_static_flag == 0)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.level_info_entity_idx);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.num_active_tracks);
            return size;
        }
    }


    public class SubpicMultipleGroupsBox : EntityToGroupBox
    {
        public override string FourCC { get { return "amgl"; } }
        public bool level_is_present_flag { get; set; }
        public bool level_is_static_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public byte level_idc { get; set; }
        public uint level_info_entity_idx { get; set; }
        public ushort num_subgroup_ids { get; set; }
        public uint[] track_subgroup_id { get; set; }
        public ushort[] num_active_tracks { get; set; }

        public SubpicMultipleGroupsBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.level_is_present_flag = IsoReaderWriter.ReadBit(stream);
            this.level_is_static_flag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 7);

            if (level_is_present_flag)
            {
                this.level_idc = IsoReaderWriter.ReadUInt8(stream);
            }

            if (level_is_static_flag == 0)
            {
                this.level_info_entity_idx = IsoReaderWriter.ReadUInt32(stream);
            }
            this.num_subgroup_ids = IsoReaderWriter.ReadUInt16(stream);
            ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.track_subgroup_id[i] = IsoReaderWriter.ReadBytes(stream, subgroupIdLen);
            }

            for (int i = 0; i < num_subgroup_ids; i++)
            {
                this.num_active_tracks[i] = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.level_is_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.level_is_static_flag);
            size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);

            if (level_is_present_flag)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.level_idc);
            }

            if (level_is_static_flag == 0)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.level_info_entity_idx);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.num_subgroup_ids);
            ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteBytes(stream, subgroupIdLen, this.track_subgroup_id[i]);
            }

            for (int i = 0; i < num_subgroup_ids; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.num_active_tracks[i]);
            }
            return size;
        }
    }


    public class OperatingPointGroupBox : EntityToGroupBox
    {
        public override string FourCC { get { return "opeg"; } }
        public byte num_profile_tier_level_minus1 { get; set; }
        public VvcPTLRecord[] opeg_ptl { get; set; }
        public byte reserved { get; set; } = 0;
        public bool incomplete_operating_points_flag { get; set; }
        public ushort num_olss { get; set; }
        public byte[] ptl_idx { get; set; }
        public ushort[] ols_idx { get; set; }
        public byte[] layer_count { get; set; }
        public bool[] layer_info_present_flag { get; set; }
        public byte[][] layer_id { get; set; }
        public byte[][] is_output_layer { get; set; }
        public ushort num_operating_points { get; set; }
        public ushort ols_loop_entry_idx { get; set; }
        public byte max_temporal_id { get; set; }
        public bool frame_rate_info_flag { get; set; }
        public bool bit_rate_info_flag { get; set; }
        public byte op_availability_idc { get; set; }
        public byte chroma_format_idc { get; set; }
        public byte bit_depth_minus8 { get; set; }
        public ushort max_picture_width { get; set; }
        public ushort max_picture_height { get; set; }
        public ushort avg_frame_rate { get; set; }
        public byte constant_frame_rate { get; set; }
        public uint max_bit_rate { get; set; }
        public uint avg_bit_rate { get; set; }
        public byte entity_count { get; set; }
        public byte entity_idx { get; set; }

        public OperatingPointGroupBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_profile_tier_level_minus1 = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i <= num_profile_tier_level_minus1; i++)
            {
                this.opeg_ptl[i] = (VvcPTLRecord)IsoReaderWriter.ReadClass(stream);
            }
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.incomplete_operating_points_flag = IsoReaderWriter.ReadBit(stream);
            this.num_olss = IsoReaderWriter.ReadBits(stream, 9);

            for (int i = 0; i < num_olss; i++)
            {
                this.ptl_idx[i] = IsoReaderWriter.ReadUInt8(stream);
                this.ols_idx[i] = IsoReaderWriter.ReadBits(stream, 9);
                this.layer_count[i] = IsoReaderWriter.ReadBits(stream, 6);
                this.reserved = IsoReaderWriter.ReadBit(stream);
                this.layer_info_present_flag[i] = IsoReaderWriter.ReadBit(stream);

                if (layer_info_present_flag)
                {

                    for (int j = 0; j < layer_count; j++)
                    {
                        this.layer_id[i][j] = IsoReaderWriter.ReadBits(stream, 6);
                        this.is_output_layer[i][j] = IsoReaderWriter.ReadBits(stream, 1);
                        this.reserved = IsoReaderWriter.ReadBit(stream);
                    }
                }
            }
            this.reserved = IsoReaderWriter.ReadBits(stream, 4);
            this.num_operating_points = IsoReaderWriter.ReadBits(stream, 12);

            for (int i = 0; i < num_operating_points; i++)
            {
                this.ols_loop_entry_idx = IsoReaderWriter.ReadBits(stream, 9);
                this.max_temporal_id = IsoReaderWriter.ReadBits(stream, 3);
                this.frame_rate_info_flag = IsoReaderWriter.ReadBit(stream);
                this.bit_rate_info_flag = IsoReaderWriter.ReadBit(stream);

                if (incomplete_operating_points_flag)
                {
                    this.op_availability_idc = IsoReaderWriter.ReadBits(stream, 2);
                }

                else
                {
                    this.reserved = IsoReaderWriter.ReadBits(stream, 2);
                }
                this.reserved = IsoReaderWriter.ReadBits(stream, 3);
                this.chroma_format_idc = IsoReaderWriter.ReadBits(stream, 2);
                this.bit_depth_minus8 = IsoReaderWriter.ReadBits(stream, 3);
                this.max_picture_width = IsoReaderWriter.ReadUInt16(stream);
                this.max_picture_height = IsoReaderWriter.ReadUInt16(stream);

                if (frame_rate_info_flag)
                {
                    this.avg_frame_rate = IsoReaderWriter.ReadUInt16(stream);
                    this.reserved = IsoReaderWriter.ReadBits(stream, 6);
                    this.constant_frame_rate = IsoReaderWriter.ReadBits(stream, 2);
                }

                if (bit_rate_info_flag)
                {
                    this.max_bit_rate = IsoReaderWriter.ReadUInt32(stream);
                    this.avg_bit_rate = IsoReaderWriter.ReadUInt32(stream);
                }
                this.entity_count = IsoReaderWriter.ReadUInt8(stream);

                for (int j = 0; j < entity_count; j++)
                {
                    this.entity_idx = IsoReaderWriter.ReadUInt8(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.num_profile_tier_level_minus1);

            for (int i = 0; i <= num_profile_tier_level_minus1; i++)
            {
                size += IsoReaderWriter.WriteClass(stream, this.opeg_ptl[i]);
            }
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.incomplete_operating_points_flag);
            size += IsoReaderWriter.WriteBits(stream, 9, this.num_olss);

            for (int i = 0; i < num_olss; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.ptl_idx[i]);
                size += IsoReaderWriter.WriteBits(stream, 9, this.ols_idx[i]);
                size += IsoReaderWriter.WriteBits(stream, 6, this.layer_count[i]);
                size += IsoReaderWriter.WriteBit(stream, this.reserved);
                size += IsoReaderWriter.WriteBit(stream, this.layer_info_present_flag[i]);

                if (layer_info_present_flag)
                {

                    for (int j = 0; j < layer_count; j++)
                    {
                        size += IsoReaderWriter.WriteBits(stream, 6, this.layer_id[i][j]);
                        size += IsoReaderWriter.WriteBits(stream, 1, this.is_output_layer[i][j]);
                        size += IsoReaderWriter.WriteBit(stream, this.reserved);
                    }
                }
            }
            size += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 12, this.num_operating_points);

            for (int i = 0; i < num_operating_points; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 9, this.ols_loop_entry_idx);
                size += IsoReaderWriter.WriteBits(stream, 3, this.max_temporal_id);
                size += IsoReaderWriter.WriteBit(stream, this.frame_rate_info_flag);
                size += IsoReaderWriter.WriteBit(stream, this.bit_rate_info_flag);

                if (incomplete_operating_points_flag)
                {
                    size += IsoReaderWriter.WriteBits(stream, 2, this.op_availability_idc);
                }

                else
                {
                    size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
                }
                size += IsoReaderWriter.WriteBits(stream, 3, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 2, this.chroma_format_idc);
                size += IsoReaderWriter.WriteBits(stream, 3, this.bit_depth_minus8);
                size += IsoReaderWriter.WriteUInt16(stream, this.max_picture_width);
                size += IsoReaderWriter.WriteUInt16(stream, this.max_picture_height);

                if (frame_rate_info_flag)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.avg_frame_rate);
                    size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
                    size += IsoReaderWriter.WriteBits(stream, 2, this.constant_frame_rate);
                }

                if (bit_rate_info_flag)
                {
                    size += IsoReaderWriter.WriteUInt32(stream, this.max_bit_rate);
                    size += IsoReaderWriter.WriteUInt32(stream, this.avg_bit_rate);
                }
                size += IsoReaderWriter.WriteUInt8(stream, this.entity_count);

                for (int j = 0; j < entity_count; j++)
                {
                    size += IsoReaderWriter.WriteUInt8(stream, this.entity_idx);
                }
            }
            return size;
        }
    }


    public class SwitchableTracks : EntityToGroupBox
    {
        public override string FourCC { get { return "swtk"; } }
        public ushort[] track_switch_hierarchy_id { get; set; }

        public SwitchableTracks()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.track_switch_hierarchy_id[i] = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.track_switch_hierarchy_id[i]);
            }
            return size;
        }
    }


    public class EntityToGroupBox1 : FullBox
    {
        public override string FourCC { get { return "vvcb"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class AUDSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "aud "; } }
        public uint audNalUnit { get; set; }

        public AUDSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.audNalUnit = IsoReaderWriter.ReadBits(stream, 24);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 24, this.audNalUnit);
            return size;
        }
    }


    public class AVCLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "avll"; } }
        public byte layerNumber { get; set; }
        public byte reserved { get; set; } = 0;
        public bool accurateStatisticsFlag { get; set; }
        public ushort avgBitRate { get; set; }
        public ushort avgFrameRate { get; set; }

        public AVCLayerEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.layerNumber = IsoReaderWriter.ReadUInt8(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 7);
            this.accurateStatisticsFlag = IsoReaderWriter.ReadBit(stream);
            this.avgBitRate = IsoReaderWriter.ReadUInt16(stream);
            this.avgFrameRate = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.layerNumber);
            size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.accurateStatisticsFlag);
            size += IsoReaderWriter.WriteUInt16(stream, this.avgBitRate);
            size += IsoReaderWriter.WriteUInt16(stream, this.avgFrameRate);
            return size;
        }
    }


    public class AVCSubSequenceEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "avss"; } }
        public ushort subSequenceIdentifer { get; set; }
        public byte layerNumber { get; set; }
        public bool durationFlag { get; set; }
        public bool avgRateFlag { get; set; }
        public byte reserved { get; set; } = 0;
        public uint duration { get; set; }
        public bool accurateStatisticsFlag { get; set; }
        public ushort avgBitRate { get; set; }
        public ushort avgFrameRate { get; set; }
        public byte numReferences { get; set; }
        public DependencyInfo[] dependency { get; set; }

        public AVCSubSequenceEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.subSequenceIdentifer = IsoReaderWriter.ReadUInt16(stream);
            this.layerNumber = IsoReaderWriter.ReadUInt8(stream);
            this.durationFlag = IsoReaderWriter.ReadBit(stream);
            this.avgRateFlag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);

            if (durationFlag)
            {
                this.duration = IsoReaderWriter.ReadUInt32(stream);
            }

            if (avgRateFlag)
            {
                this.reserved = IsoReaderWriter.ReadBits(stream, 7);
                this.accurateStatisticsFlag = IsoReaderWriter.ReadBit(stream);
                this.avgBitRate = IsoReaderWriter.ReadUInt16(stream);
                this.avgFrameRate = IsoReaderWriter.ReadUInt16(stream);
            }
            this.numReferences = IsoReaderWriter.ReadUInt8(stream);
            this.dependency = (DependencyInfo[])IsoReaderWriter.ReadClasses(stream, numReferences);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.subSequenceIdentifer);
            size += IsoReaderWriter.WriteUInt8(stream, this.layerNumber);
            size += IsoReaderWriter.WriteBit(stream, this.durationFlag);
            size += IsoReaderWriter.WriteBit(stream, this.avgRateFlag);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);

            if (durationFlag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.duration);
            }

            if (avgRateFlag)
            {
                size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
                size += IsoReaderWriter.WriteBit(stream, this.accurateStatisticsFlag);
                size += IsoReaderWriter.WriteUInt16(stream, this.avgBitRate);
                size += IsoReaderWriter.WriteUInt16(stream, this.avgFrameRate);
            }
            size += IsoReaderWriter.WriteUInt8(stream, this.numReferences);
            size += IsoReaderWriter.WriteClasses(stream, numReferences, this.dependency);
            return size;
        }
    }


    public class DecodingCapabilityInformation : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dcfi"; } }
        public ushort dci_nal_unit_length { get; set; }
        public byte[] dci_nal_unit { get; set; }

        public DecodingCapabilityInformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.dci_nal_unit_length = IsoReaderWriter.ReadUInt16(stream);
            this.dci_nal_unit = IsoReaderWriter.ReadBytes(stream, dci_nal_unit_length);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.dci_nal_unit_length);
            size += IsoReaderWriter.WriteBytes(stream, dci_nal_unit_length, this.dci_nal_unit);
            return size;
        }
    }


    public class DecodeRetimingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dtrt"; } }
        public byte tierCount { get; set; }
        public ushort tierID { get; set; }
        public short delta { get; set; }

        public DecodeRetimingEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.tierCount = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 1; i <= tierCount; i++)
            {
                this.tierID = IsoReaderWriter.ReadUInt16(stream);
                this.delta = IsoReaderWriter.ReadInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.tierCount);

            for (int i = 1; i <= tierCount; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.tierID);
                size += IsoReaderWriter.WriteInt16(stream, this.delta);
            }
            return size;
        }
    }


    public class EndOfBitstreamSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "eob "; } }
        public ushort eobNalUnit { get; set; }

        public EndOfBitstreamSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.eobNalUnit = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.eobNalUnit);
            return size;
        }
    }


    public class EndOfSequenceSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "eos "; } }
        public byte num_eos_nal_unit_minus1 { get; set; }
        public ushort[] eosNalUnit { get; set; }

        public EndOfSequenceSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_eos_nal_unit_minus1 = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i <= num_eos_nal_unit_minus1; i++)
            {
                this.eosNalUnit[i] = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.num_eos_nal_unit_minus1);

            for (int i = 0; i <= num_eos_nal_unit_minus1; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.eosNalUnit[i]);
            }
            return size;
        }
    }


    public class LhvcExternalBaseLayerInfo : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "lbli"; } }
        public bool reserved { get; set; } = true;
        public bool bl_irap_pic_flag { get; set; }
        public byte bl_irap_nal_unit_type { get; set; }
        public sbyte sample_offset { get; set; }

        public LhvcExternalBaseLayerInfo()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBit(stream);
            this.bl_irap_pic_flag = IsoReaderWriter.ReadBit(stream);
            this.bl_irap_nal_unit_type = IsoReaderWriter.ReadBits(stream, 6);
            this.sample_offset = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.bl_irap_pic_flag);
            size += IsoReaderWriter.WriteBits(stream, 6, this.bl_irap_nal_unit_type);
            size += IsoReaderWriter.WriteInt8(stream, this.sample_offset);
            return size;
        }
    }


    public class LayerInfoGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "linf"; } }
        public byte reserved { get; set; } = 0;
        public byte num_layers_in_track { get; set; }
        public bool irap_gdr_pics_in_layer_only_flag { get; set; }
        public bool completeness_flag { get; set; }
        public byte layer_id { get; set; }
        public byte min_TemporalId { get; set; }
        public byte max_TemporalId { get; set; }
        public byte sub_layer_presence_flags { get; set; }

        public LayerInfoGroupEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 2);
            this.num_layers_in_track = IsoReaderWriter.ReadBits(stream, 6);

            for (int i = 0; i < num_layers_in_track; i++)
            {
                this.reserved = IsoReaderWriter.ReadBits(stream, 2);
                this.irap_gdr_pics_in_layer_only_flag = IsoReaderWriter.ReadBit(stream);
                this.completeness_flag = IsoReaderWriter.ReadBit(stream);
                this.layer_id = IsoReaderWriter.ReadBits(stream, 6);
                this.min_TemporalId = IsoReaderWriter.ReadBits(stream, 3);
                this.max_TemporalId = IsoReaderWriter.ReadBits(stream, 3);
                this.reserved = IsoReaderWriter.ReadBit(stream);
                this.sub_layer_presence_flags = IsoReaderWriter.ReadBits(stream, 7);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 6, this.num_layers_in_track);

            for (int i = 0; i < num_layers_in_track; i++)
            {
                size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
                size += IsoReaderWriter.WriteBit(stream, this.irap_gdr_pics_in_layer_only_flag);
                size += IsoReaderWriter.WriteBit(stream, this.completeness_flag);
                size += IsoReaderWriter.WriteBits(stream, 6, this.layer_id);
                size += IsoReaderWriter.WriteBits(stream, 3, this.min_TemporalId);
                size += IsoReaderWriter.WriteBits(stream, 3, this.max_TemporalId);
                size += IsoReaderWriter.WriteBit(stream, this.reserved);
                size += IsoReaderWriter.WriteBits(stream, 7, this.sub_layer_presence_flags);
            }
            return size;
        }
    }


    public class VvcMixedNALUnitTypePicEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "minp"; } }
        public ushort num_mix_nalu_pic_idx { get; set; }
        public ushort[] mix_subp_track_idx1 { get; set; }
        public ushort[] mix_subp_track_idx2 { get; set; }
        public ushort pps_mix_nalu_types_in_pic_bit_pos { get; set; }
        public byte pps_id { get; set; }

        public VvcMixedNALUnitTypePicEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_mix_nalu_pic_idx = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_mix_nalu_pic_idx; i++)
            {
                this.mix_subp_track_idx1[i] = IsoReaderWriter.ReadUInt16(stream);
                this.mix_subp_track_idx2[i] = IsoReaderWriter.ReadUInt16(stream);
            }
            this.pps_mix_nalu_types_in_pic_bit_pos = IsoReaderWriter.ReadBits(stream, 10);
            this.pps_id = IsoReaderWriter.ReadBits(stream, 6);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_mix_nalu_pic_idx);

            for (int i = 0; i < num_mix_nalu_pic_idx; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.mix_subp_track_idx1[i]);
                size += IsoReaderWriter.WriteUInt16(stream, this.mix_subp_track_idx2[i]);
            }
            size += IsoReaderWriter.WriteBits(stream, 10, this.pps_mix_nalu_types_in_pic_bit_pos);
            size += IsoReaderWriter.WriteBits(stream, 6, this.pps_id);
            return size;
        }
    }


    public class MultiviewGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "mvif"; } }
        public byte groupID { get; set; }
        public byte primary_groupID { get; set; }
        public byte reserved { get; set; } = 0;
        public bool is_tl_switching_point { get; set; }
        public byte tl_switching_distance { get; set; }

        public MultiviewGroupEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.groupID = IsoReaderWriter.ReadUInt8(stream);
            this.primary_groupID = IsoReaderWriter.ReadUInt8(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 4);
            this.is_tl_switching_point = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 3);
            this.tl_switching_distance = IsoReaderWriter.ReadUInt8(stream);

            if (groupID == primary_groupID)
            {
                // TODO: This should likely be a FullBox: ViewIdentifierBox; // Mandatory

                // TODO: This should likely be a FullBox: TierInfoBox; // Mandatory

                // TODO: This should likely be a FullBox: TierDependencyBox; // Mandatory

                // TODO: This should likely be a FullBox: PriorityRangeBox; // Mandatory

                /* Optional Boxes or fields may follow when defined later */
                // TODO: This should likely be a FullBox: TierBitRateBox; // optional

                // TODO: This should likely be a FullBox: BufferingBox; // optional

                // TODO: This should likely be a FullBox: InitialParameterSetBox; // optional

                // TODO: This should likely be a FullBox: ProtectionSchemeInfoBox; // optional

                // TODO: This should likely be a FullBox: ViewPriorityBox; // optional

            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.groupID);
            size += IsoReaderWriter.WriteUInt8(stream, this.primary_groupID);
            size += IsoReaderWriter.WriteBits(stream, 4, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.is_tl_switching_point);
            size += IsoReaderWriter.WriteBits(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteUInt8(stream, this.tl_switching_distance);

            if (groupID == primary_groupID)
            {
                // TODO: This should likely be a FullBox: ViewIdentifierBox; // Mandatory

                // TODO: This should likely be a FullBox: TierInfoBox; // Mandatory

                // TODO: This should likely be a FullBox: TierDependencyBox; // Mandatory

                // TODO: This should likely be a FullBox: PriorityRangeBox; // Mandatory

                /* Optional Boxes or fields may follow when defined later */
                // TODO: This should likely be a FullBox: TierBitRateBox; // optional

                // TODO: This should likely be a FullBox: BufferingBox; // optional

                // TODO: This should likely be a FullBox: InitialParameterSetBox; // optional

                // TODO: This should likely be a FullBox: ProtectionSchemeInfoBox; // optional

                // TODO: This should likely be a FullBox: ViewPriorityBox; // optional

            }
            return size;
        }
    }


    public class NALUMapEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "nalm"; } }
        public byte reserved { get; set; } = 0;
        public bool large_size { get; set; }
        public bool rle { get; set; }
        public ushort entry_count { get; set; }
        public ushort NALU_start_number { get; set; }
        public ushort groupID { get; set; }

        public NALUMapEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.large_size = IsoReaderWriter.ReadBit(stream);
            this.rle = IsoReaderWriter.ReadBit(stream);

            if (large_size)
            {
                this.entry_count = IsoReaderWriter.ReadUInt16(stream);
            }

            else
            {
                this.entry_count = IsoReaderWriter.ReadUInt8(stream);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (rle)
                {

                    if (large_size)
                    {
                        this.NALU_start_number = IsoReaderWriter.ReadUInt16(stream);
                    }

                    else
                    {
                        this.NALU_start_number = IsoReaderWriter.ReadUInt8(stream);
                    }
                }
                this.groupID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.large_size);
            size += IsoReaderWriter.WriteBit(stream, this.rle);

            if (large_size)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.entry_count);
            }

            else
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.entry_count);
            }

            for (int i = 1; i <= entry_count; i++)
            {

                if (rle)
                {

                    if (large_size)
                    {
                        size += IsoReaderWriter.WriteUInt16(stream, this.NALU_start_number);
                    }

                    else
                    {
                        size += IsoReaderWriter.WriteUInt8(stream, this.NALU_start_number);
                    }
                }
                size += IsoReaderWriter.WriteUInt16(stream, this.groupID);
            }
            return size;
        }
    }


    public class OperatingPointsInformation : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "oinf"; } }
        public OperatingPointsRecord oinf { get; set; }

        public OperatingPointsInformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.oinf = (OperatingPointsRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.oinf);
            return size;
        }
    }


    public class OperatingPointDecodeTimeHint : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "opth"; } }
        public int delta_time { get; set; }

        public OperatingPointDecodeTimeHint()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.delta_time = IsoReaderWriter.ReadInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt32(stream, this.delta_time);
            return size;
        }
    }


    public class ParameterSetNALUEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pase"; } }
        public ushort ps_nalu_length { get; set; }
        public byte[] ps_nal_unit { get; set; }

        public ParameterSetNALUEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.ps_nalu_length = IsoReaderWriter.ReadUInt16(stream);
            this.ps_nal_unit = IsoReaderWriter.ReadBytes(stream, ps_nalu_length);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.ps_nalu_length);
            size += IsoReaderWriter.WriteBytes(stream, ps_nalu_length, this.ps_nal_unit);
            return size;
        }
    }


    public class PSSampleGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pss1"; } }
        public bool sps_present { get; set; }
        public bool pps_present { get; set; }
        public bool aps_present { get; set; }
        public byte reserved { get; set; } = 0;

        public PSSampleGroupEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sps_present = IsoReaderWriter.ReadBit(stream);
            this.pps_present = IsoReaderWriter.ReadBit(stream);
            this.aps_present = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 5);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.sps_present);
            size += IsoReaderWriter.WriteBit(stream, this.pps_present);
            size += IsoReaderWriter.WriteBit(stream, this.aps_present);
            size += IsoReaderWriter.WriteBits(stream, 5, this.reserved);
            return size;
        }
    }


    public class VvcRectRegionOrderEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "rror"; } }
        public bool subpic_id_info_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public ushort num_alternate_region_set { get; set; }
        public ushort[] num_regions_in_set { get; set; }
        public ushort[] alternate_region_set_id { get; set; }
        public ushort[][] groupID { get; set; }
        public ushort num_regions_minus1 { get; set; }
        public ushort[] region_id { get; set; }
        public VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info { get; set; }

        public VvcRectRegionOrderEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.subpic_id_info_flag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 7);
            this.num_alternate_region_set = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_alternate_region_set; i++)
            {
                this.num_regions_in_set[i] = IsoReaderWriter.ReadUInt16(stream);
                this.alternate_region_set_id[i] = IsoReaderWriter.ReadUInt16(stream);

                for (int j = 0; j < num_regions_in_set[i]; j++)
                {
                    this.groupID[i][j] = IsoReaderWriter.ReadUInt16(stream);
                }
            }
            this.num_regions_minus1 = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i < num_regions_minus1; i++)
            {
                this.region_id[i] = IsoReaderWriter.ReadUInt16(stream);
            }

            if (subpic_id_info_flag)
            {
                this.subpic_id_rewriting_info = (VVCSubpicIDRewritingInfomationStruct)IsoReaderWriter.ReadClass(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.subpic_id_info_flag);
            size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            size += IsoReaderWriter.WriteUInt16(stream, this.num_alternate_region_set);

            for (int i = 0; i < num_alternate_region_set; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.num_regions_in_set[i]);
                size += IsoReaderWriter.WriteUInt16(stream, this.alternate_region_set_id[i]);

                for (int j = 0; j < num_regions_in_set[i]; j++)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.groupID[i][j]);
                }
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.num_regions_minus1);

            for (int i = 0; i < num_regions_minus1; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.region_id[i]);
            }

            if (subpic_id_info_flag)
            {
                size += IsoReaderWriter.WriteClass(stream, this.subpic_id_rewriting_info);
            }
            return size;
        }
    }


    public class ScalableGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "scif"; } }
        public byte groupID { get; set; }
        public byte primary_groupID { get; set; }
        public bool is_tier_IDR { get; set; }
        public bool noInterLayerPredFlag { get; set; }
        public bool useRefBasePicFlag { get; set; }
        public bool storeBaseRepFlag { get; set; }
        public bool is_tl_switching_point { get; set; }
        public byte reserved { get; set; } = 0;
        public byte tl_switching_distance { get; set; }

        public ScalableGroupEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.groupID = IsoReaderWriter.ReadUInt8(stream);
            this.primary_groupID = IsoReaderWriter.ReadUInt8(stream);
            this.is_tier_IDR = IsoReaderWriter.ReadBit(stream);
            this.noInterLayerPredFlag = IsoReaderWriter.ReadBit(stream);
            this.useRefBasePicFlag = IsoReaderWriter.ReadBit(stream);
            this.storeBaseRepFlag = IsoReaderWriter.ReadBit(stream);
            this.is_tl_switching_point = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 3);
            this.tl_switching_distance = IsoReaderWriter.ReadUInt8(stream);

            if (groupID == primary_groupID)
            {
                // TODO: This should likely be a FullBox: TierInfoBox; // Mandatory

                // TODO: This should likely be a FullBox: SVCDependencyRangeBox; // Mandatory

                // TODO: This should likely be a FullBox: PriorityRangeBox; // Mandatory

                /* Optional Boxes or fields may follow when defined later */
                // TODO: This should likely be a FullBox: TierBitRateBox; // optional

                // TODO: This should likely be a FullBox: RectRegionBox; // optional

                // TODO: This should likely be a FullBox: BufferingBox; // optional

                // TODO: This should likely be a FullBox: TierDependencyBox; // optional

                // TODO: This should likely be a FullBox: InitialParameterSetBox; // optional

                // TODO: This should likely be a FullBox: IroiInfoBox; // optional

                // TODO: This should likely be a FullBox: ProtectionSchemeInfoBox; // optional

                // TODO: This should likely be a FullBox: TranscodingInfoBox; // optional

            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.groupID);
            size += IsoReaderWriter.WriteUInt8(stream, this.primary_groupID);
            size += IsoReaderWriter.WriteBit(stream, this.is_tier_IDR);
            size += IsoReaderWriter.WriteBit(stream, this.noInterLayerPredFlag);
            size += IsoReaderWriter.WriteBit(stream, this.useRefBasePicFlag);
            size += IsoReaderWriter.WriteBit(stream, this.storeBaseRepFlag);
            size += IsoReaderWriter.WriteBit(stream, this.is_tl_switching_point);
            size += IsoReaderWriter.WriteBits(stream, 3, this.reserved);
            size += IsoReaderWriter.WriteUInt8(stream, this.tl_switching_distance);

            if (groupID == primary_groupID)
            {
                // TODO: This should likely be a FullBox: TierInfoBox; // Mandatory

                // TODO: This should likely be a FullBox: SVCDependencyRangeBox; // Mandatory

                // TODO: This should likely be a FullBox: PriorityRangeBox; // Mandatory

                /* Optional Boxes or fields may follow when defined later */
                // TODO: This should likely be a FullBox: TierBitRateBox; // optional

                // TODO: This should likely be a FullBox: RectRegionBox; // optional

                // TODO: This should likely be a FullBox: BufferingBox; // optional

                // TODO: This should likely be a FullBox: TierDependencyBox; // optional

                // TODO: This should likely be a FullBox: InitialParameterSetBox; // optional

                // TODO: This should likely be a FullBox: IroiInfoBox; // optional

                // TODO: This should likely be a FullBox: ProtectionSchemeInfoBox; // optional

                // TODO: This should likely be a FullBox: TranscodingInfoBox; // optional

            }
            return size;
        }
    }


    public class ScalableNALUMapEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "scnm"; } }
        public byte reserved { get; set; } = 0;
        public byte NALU_count { get; set; }
        public byte groupID { get; set; }

        public ScalableNALUMapEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadUInt8(stream);
            this.NALU_count = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 1; i <= NALU_count; i++)
            {
                this.groupID = IsoReaderWriter.ReadUInt8(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.reserved);
            size += IsoReaderWriter.WriteUInt8(stream, this.NALU_count);

            for (int i = 1; i <= NALU_count; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.groupID);
            }
            return size;
        }
    }


    public class VvcSubpicIDEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "spid"; } }
        public bool rect_region_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public bool continuous_id_flag { get; set; }
        public ushort num_subpics_minus1 { get; set; }
        public ushort[] subpic_id { get; set; }
        public ushort[] groupID { get; set; }

        public VvcSubpicIDEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.rect_region_flag = IsoReaderWriter.ReadBit(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 2);
            this.continuous_id_flag = IsoReaderWriter.ReadBit(stream);
            this.num_subpics_minus1 = IsoReaderWriter.ReadBits(stream, 12);

            for (int i = 0; i <= num_subpics_minus1; i++)
            {

                if ((continuous_id_flag && i == 0) || !continuous_id_flag)
                {
                    this.subpic_id[i] = IsoReaderWriter.ReadUInt16(stream);
                }

                if (rect_region_flag)
                {
                    this.groupID[i] = IsoReaderWriter.ReadUInt16(stream);
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.rect_region_flag);
            size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.continuous_id_flag);
            size += IsoReaderWriter.WriteBits(stream, 12, this.num_subpics_minus1);

            for (int i = 0; i <= num_subpics_minus1; i++)
            {

                if ((continuous_id_flag && i == 0) || !continuous_id_flag)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.subpic_id[i]);
                }

                if (rect_region_flag)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.groupID[i]);
                }
            }
            return size;
        }
    }


    public class SubpicLevelInfoEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "spli"; } }
        public byte level_idc { get; set; }

        public SubpicLevelInfoEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.level_idc = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.level_idc);
            return size;
        }
    }


    public class VvcSubpicOrderEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "spor"; } }
        public bool subpic_id_info_flag { get; set; }
        public ushort num_subpic_ref_idx { get; set; }
        public ushort[] subp_track_ref_idx { get; set; }
        public VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info { get; set; }

        public VvcSubpicOrderEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.subpic_id_info_flag = IsoReaderWriter.ReadBit(stream);
            this.num_subpic_ref_idx = IsoReaderWriter.ReadBits(stream, 15);

            for (int i = 0; i < num_subpic_ref_idx; i++)
            {
                this.subp_track_ref_idx[i] = IsoReaderWriter.ReadUInt16(stream);
            }

            if (subpic_id_info_flag)
            {
                this.subpic_id_rewriting_info = (VVCSubpicIDRewritingInfomationStruct)IsoReaderWriter.ReadClass(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.subpic_id_info_flag);
            size += IsoReaderWriter.WriteBits(stream, 15, this.num_subpic_ref_idx);

            for (int i = 0; i < num_subpic_ref_idx; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.subp_track_ref_idx[i]);
            }

            if (subpic_id_info_flag)
            {
                size += IsoReaderWriter.WriteClass(stream, this.subpic_id_rewriting_info);
            }
            return size;
        }
    }


    public class StepwiseTemporalLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "stsa"; } }

        public StepwiseTemporalLayerEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class VvcSubpicLayoutMapEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "sulm"; } }
        public uint groupID_info_4cc { get; set; }
        public ushort entry_count_minus1 { get; set; }
        public ushort groupID { get; set; }

        public VvcSubpicLayoutMapEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.groupID_info_4cc = IsoReaderWriter.ReadUInt32(stream);
            this.entry_count_minus1 = IsoReaderWriter.ReadUInt16(stream);

            for (int i = 0; i <= entry_count_minus1; i++)
            {
                this.groupID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.groupID_info_4cc);
            size += IsoReaderWriter.WriteUInt16(stream, this.entry_count_minus1);

            for (int i = 0; i <= entry_count_minus1; i++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.groupID);
            }
            return size;
        }
    }


    public class SyncSampleEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "sync"; } }
        public byte reserved { get; set; } = 0;
        public byte NAL_unit_type { get; set; }

        public SyncSampleEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 2);
            this.NAL_unit_type = IsoReaderWriter.ReadBits(stream, 6);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 6, this.NAL_unit_type);
            return size;
        }
    }


    public class RectangularRegionGroupEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "trif"; } }
        public ushort groupID { get; set; }
        public bool rect_region_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public byte independent_idc { get; set; }
        public bool full_picture { get; set; }
        public bool filtering_disabled { get; set; }
        public bool has_dependency_list { get; set; }
        public ushort horizontal_offset { get; set; }
        public ushort vertical_offset { get; set; }
        public ushort region_width { get; set; }
        public ushort region_height { get; set; }
        public ushort dependency_rect_region_count { get; set; }
        public ushort dependencyRectRegionGroupID { get; set; }

        public RectangularRegionGroupEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.groupID = IsoReaderWriter.ReadUInt16(stream);
            this.rect_region_flag = IsoReaderWriter.ReadBit(stream);

            if (!rect_region_flag)
            {
                this.reserved = IsoReaderWriter.ReadBits(stream, 7);
            }

            else
            {
                this.independent_idc = IsoReaderWriter.ReadBits(stream, 2);
                this.full_picture = IsoReaderWriter.ReadBit(stream);
                this.filtering_disabled = IsoReaderWriter.ReadBit(stream);
                this.has_dependency_list = IsoReaderWriter.ReadBit(stream);
                this.reserved = IsoReaderWriter.ReadBits(stream, 2);

                if (!full_picture)
                {
                    this.horizontal_offset = IsoReaderWriter.ReadUInt16(stream);
                    this.vertical_offset = IsoReaderWriter.ReadUInt16(stream);
                }
                this.region_width = IsoReaderWriter.ReadUInt16(stream);
                this.region_height = IsoReaderWriter.ReadUInt16(stream);

                if (has_dependency_list)
                {
                    this.dependency_rect_region_count = IsoReaderWriter.ReadUInt16(stream);

                    for (int i = 1; i <= dependency_rect_region_count; i++)
                    {
                        this.dependencyRectRegionGroupID = IsoReaderWriter.ReadUInt16(stream);
                    }
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.groupID);
            size += IsoReaderWriter.WriteBit(stream, this.rect_region_flag);

            if (!rect_region_flag)
            {
                size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            }

            else
            {
                size += IsoReaderWriter.WriteBits(stream, 2, this.independent_idc);
                size += IsoReaderWriter.WriteBit(stream, this.full_picture);
                size += IsoReaderWriter.WriteBit(stream, this.filtering_disabled);
                size += IsoReaderWriter.WriteBit(stream, this.has_dependency_list);
                size += IsoReaderWriter.WriteBits(stream, 2, this.reserved);

                if (!full_picture)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.horizontal_offset);
                    size += IsoReaderWriter.WriteUInt16(stream, this.vertical_offset);
                }
                size += IsoReaderWriter.WriteUInt16(stream, this.region_width);
                size += IsoReaderWriter.WriteUInt16(stream, this.region_height);

                if (has_dependency_list)
                {
                    size += IsoReaderWriter.WriteUInt16(stream, this.dependency_rect_region_count);

                    for (int i = 1; i <= dependency_rect_region_count; i++)
                    {
                        size += IsoReaderWriter.WriteUInt16(stream, this.dependencyRectRegionGroupID);
                    }
                }
            }
            return size;
        }
    }


    public class TemporalSubLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "tsas"; } }

        public TemporalSubLayerEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            return size;
        }
    }


    public class TemporalLayerEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "tscl"; } }
        public byte temporalLayerId { get; set; }
        public byte tlprofile_space { get; set; }
        public bool tltier_flag { get; set; }
        public byte tlprofile_idc { get; set; }
        public uint tlprofile_compatibility_flags { get; set; }
        public ulong tlconstraint_indicator_flags { get; set; }
        public byte tllevel_idc { get; set; }
        public ushort tlMaxBitRate { get; set; }
        public ushort tlAvgBitRate { get; set; }
        public byte tlConstantFrameRate { get; set; }
        public ushort tlAvgFrameRate { get; set; }

        public TemporalLayerEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.temporalLayerId = IsoReaderWriter.ReadUInt8(stream);
            this.tlprofile_space = IsoReaderWriter.ReadBits(stream, 2);
            this.tltier_flag = IsoReaderWriter.ReadBit(stream);
            this.tlprofile_idc = IsoReaderWriter.ReadBits(stream, 5);
            this.tlprofile_compatibility_flags = IsoReaderWriter.ReadUInt32(stream);
            this.tlconstraint_indicator_flags = IsoReaderWriter.ReadUInt48(stream);
            this.tllevel_idc = IsoReaderWriter.ReadUInt8(stream);
            this.tlMaxBitRate = IsoReaderWriter.ReadUInt16(stream);
            this.tlAvgBitRate = IsoReaderWriter.ReadUInt16(stream);
            this.tlConstantFrameRate = IsoReaderWriter.ReadUInt8(stream);
            this.tlAvgFrameRate = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.temporalLayerId);
            size += IsoReaderWriter.WriteBits(stream, 2, this.tlprofile_space);
            size += IsoReaderWriter.WriteBit(stream, this.tltier_flag);
            size += IsoReaderWriter.WriteBits(stream, 5, this.tlprofile_idc);
            size += IsoReaderWriter.WriteUInt32(stream, this.tlprofile_compatibility_flags);
            size += IsoReaderWriter.WriteUInt48(stream, this.tlconstraint_indicator_flags);
            size += IsoReaderWriter.WriteUInt8(stream, this.tllevel_idc);
            size += IsoReaderWriter.WriteUInt16(stream, this.tlMaxBitRate);
            size += IsoReaderWriter.WriteUInt16(stream, this.tlAvgBitRate);
            size += IsoReaderWriter.WriteUInt8(stream, this.tlConstantFrameRate);
            size += IsoReaderWriter.WriteUInt16(stream, this.tlAvgFrameRate);
            return size;
        }
    }


    public class ViewPriorityEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "vipr"; } }

        public ViewPriorityEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            // TODO: This should likely be a FullBox: ViewPriorityBox;

        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            // TODO: This should likely be a FullBox: ViewPriorityBox;

            return size;
        }
    }


    public class VvcOperatingPointsInformation : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "vopi"; } }
        public VvcOperatingPointsRecord oinf { get; set; }

        public VvcOperatingPointsInformation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.oinf = (VvcOperatingPointsRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.oinf);
            return size;
        }
    }


    public class TrackGroupTypeBox1 : FullBox
    {
        public override string FourCC { get { return "alte"; } }
        public uint track_group_id { get; set; }  //  the remaining data may be specified 

        public TrackGroupTypeBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_group_id = IsoReaderWriter.ReadUInt32(stream);
            /*   for a particular track_group_type */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_group_id);
            /*   for a particular track_group_type */
            return size;
        }
    }


    public class TrackGroupTypeBox2 : FullBox
    {
        public override string FourCC { get { return "cstg"; } }
        public uint track_group_id { get; set; }  //  the remaining data may be specified 

        public TrackGroupTypeBox2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_group_id = IsoReaderWriter.ReadUInt32(stream);
            /*   for a particular track_group_type */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_group_id);
            /*   for a particular track_group_type */
            return size;
        }
    }


    public class TrackGroupTypeBox3 : FullBox
    {
        public override string FourCC { get { return "snut"; } }
        public uint track_group_id { get; set; }  //  the remaining data may be specified 

        public TrackGroupTypeBox3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_group_id = IsoReaderWriter.ReadUInt32(stream);
            /*   for a particular track_group_type */
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.track_group_id);
            /*   for a particular track_group_type */
            return size;
        }
    }


    public class TrackReferenceTypeBox11 : Box
    {
        public override string FourCC { get { return "avcp"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox11()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox12 : Box
    {
        public override string FourCC { get { return "deps"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox12()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox13 : Box
    {
        public override string FourCC { get { return "evcr"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox13()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox14 : Box
    {
        public override string FourCC { get { return "mixn"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox14()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox15 : Box
    {
        public override string FourCC { get { return "oref"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox15()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox16 : Box
    {
        public override string FourCC { get { return "recr"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox16()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox17 : Box
    {
        public override string FourCC { get { return "sabt"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox17()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox18 : Box
    {
        public override string FourCC { get { return "sbas"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox18()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox19 : Box
    {
        public override string FourCC { get { return "scal"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox19()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox20 : Box
    {
        public override string FourCC { get { return "subp"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox20()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox21 : Box
    {
        public override string FourCC { get { return "swfr"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox21()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox22 : Box
    {
        public override string FourCC { get { return "swto"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox22()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox23 : Box
    {
        public override string FourCC { get { return "tbas"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox23()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox24 : Box
    {
        public override string FourCC { get { return "vref"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox24()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox25 : Box
    {
        public override string FourCC { get { return "vreg"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox25()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class TrackReferenceTypeBox26 : Box
    {
        public override string FourCC { get { return "vvcN"; } }
        public uint[] track_IDs { get; set; }

        public TrackReferenceTypeBox26()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.track_IDs = IsoReaderWriter.ReadUInt32Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32Array(stream, this.track_IDs);
            return size;
        }
    }


    public class EntityToGroupBox2 : FullBox
    {
        public override string FourCC { get { return "eqiv"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox3 : FullBox
    {
        public override string FourCC { get { return "ster"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox4 : FullBox
    {
        public override string FourCC { get { return "aebr"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox5 : FullBox
    {
        public override string FourCC { get { return "afbr"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox6 : FullBox
    {
        public override string FourCC { get { return "albc"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox6()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox7 : FullBox
    {
        public override string FourCC { get { return "brst"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox7()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox8 : FullBox
    {
        public override string FourCC { get { return "iaug"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox8()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox9 : FullBox
    {
        public override string FourCC { get { return "tsyn"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox9()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox10 : FullBox
    {
        public override string FourCC { get { return "dobr"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox10()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox11 : FullBox
    {
        public override string FourCC { get { return "favc"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox11()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox12 : FullBox
    {
        public override string FourCC { get { return "fobr"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox12()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox13 : FullBox
    {
        public override string FourCC { get { return "pano"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox13()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class EntityToGroupBox14 : FullBox
    {
        public override string FourCC { get { return "wbbr"; } }
        public uint group_id { get; set; }
        public uint num_entities_in_group { get; set; }
        public uint entity_id { get; set; }  //  the remaining data may be specified for a particular grouping_type

        public EntityToGroupBox14()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.group_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_entities_in_group = IsoReaderWriter.ReadUInt32(stream);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                this.entity_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.group_id);
            size += IsoReaderWriter.WriteUInt32(stream, this.num_entities_in_group);

            for (int i = 0; i < num_entities_in_group; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.entity_id);
            }
            return size;
        }
    }


    public class AuxiliaryTypeProperty : ItemFullProperty
    {
        public override string FourCC { get { return "auxC"; } }
        public string aux_type { get; set; }
        public byte[] aux_subtype { get; set; }  //  until the end of the box, the semantics depend on the aux_type value

        public AuxiliaryTypeProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.aux_type = IsoReaderWriter.ReadString(stream);
            this.aux_subtype = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.aux_type);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.aux_subtype);
            return size;
        }
    }


    public class AVCConfigurationBox1 : Box
    {
        public override string FourCC { get { return "avcC"; } }
        public AVCDecoderConfigurationRecord AVCConfig { get; set; }

        public AVCConfigurationBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.AVCConfig = (AVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.AVCConfig);
            return size;
        }
    }


    public class CleanApertureBox1 : Box
    {
        public override string FourCC { get { return "clap"; } }
        public uint cleanApertureWidthN { get; set; }
        public uint cleanApertureWidthD { get; set; }
        public uint cleanApertureHeightN { get; set; }
        public uint cleanApertureHeightD { get; set; }
        public uint horizOffN { get; set; }
        public uint horizOffD { get; set; }
        public uint vertOffN { get; set; }
        public uint vertOffD { get; set; }

        public CleanApertureBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.cleanApertureWidthN = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureWidthD = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureHeightN = IsoReaderWriter.ReadUInt32(stream);
            this.cleanApertureHeightD = IsoReaderWriter.ReadUInt32(stream);
            this.horizOffN = IsoReaderWriter.ReadUInt32(stream);
            this.horizOffD = IsoReaderWriter.ReadUInt32(stream);
            this.vertOffN = IsoReaderWriter.ReadUInt32(stream);
            this.vertOffD = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthN);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureWidthD);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightN);
            size += IsoReaderWriter.WriteUInt32(stream, this.cleanApertureHeightD);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizOffN);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizOffD);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertOffN);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertOffD);
            return size;
        }
    }


    public class ColourInformationBox1 : Box
    {
        public override string FourCC { get { return "colr"; } }
        public uint colour_type { get; set; }
        public ushort colour_primaries { get; set; }
        public ushort transfer_characteristics { get; set; }
        public ushort matrix_coefficients { get; set; }
        public bool full_range_flag { get; set; }
        public byte reserved { get; set; } = 0;
        public ICC_profile ICC_profile { get; set; }  //  restricted ICC profile

        public ColourInformationBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.colour_type = IsoReaderWriter.ReadUInt32(stream);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                this.colour_primaries = IsoReaderWriter.ReadUInt16(stream);
                this.transfer_characteristics = IsoReaderWriter.ReadUInt16(stream);
                this.matrix_coefficients = IsoReaderWriter.ReadUInt16(stream);
                this.full_range_flag = IsoReaderWriter.ReadBit(stream);
                this.reserved = IsoReaderWriter.ReadBits(stream, 7);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                this.ICC_profile = (ICC_profile)IsoReaderWriter.ReadClass(stream);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                this.ICC_profile = (ICC_profile)IsoReaderWriter.ReadClass(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.colour_type);

            if (colour_type == IsoReaderWriter.FromFourCC("nclx"))
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.colour_primaries);
                size += IsoReaderWriter.WriteUInt16(stream, this.transfer_characteristics);
                size += IsoReaderWriter.WriteUInt16(stream, this.matrix_coefficients);
                size += IsoReaderWriter.WriteBit(stream, this.full_range_flag);
                size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("rICC"))
            {
                size += IsoReaderWriter.WriteClass(stream, this.ICC_profile);
            }

            else if (colour_type == IsoReaderWriter.FromFourCC("prof"))
            {
                size += IsoReaderWriter.WriteClass(stream, this.ICC_profile);
            }
            return size;
        }
    }


    public class HEVCConfigurationBox1 : Box
    {
        public override string FourCC { get { return "hvcC"; } }
        public HEVCDecoderConfigurationRecord HEVCConfig { get; set; }

        public HEVCConfigurationBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.HEVCConfig = (HEVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.HEVCConfig);
            return size;
        }
    }


    public class ImageMirror : ItemProperty
    {
        public override string FourCC { get { return "imir"; } }
        public byte reserved { get; set; } = 0;
        public bool axis { get; set; }

        public ImageMirror()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 7);
            this.axis = IsoReaderWriter.ReadBit(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 7, this.reserved);
            size += IsoReaderWriter.WriteBit(stream, this.axis);
            return size;
        }
    }


    public class ImageRotation : ItemProperty
    {
        public override string FourCC { get { return "irot"; } }
        public byte reserved { get; set; } = 0;
        public byte angle { get; set; }

        public ImageRotation()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved = IsoReaderWriter.ReadBits(stream, 6);
            this.angle = IsoReaderWriter.ReadBits(stream, 2);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBits(stream, 6, this.reserved);
            size += IsoReaderWriter.WriteBits(stream, 2, this.angle);
            return size;
        }
    }


    public class ImageSpatialExtentsProperty : ItemFullProperty
    {
        public override string FourCC { get { return "ispe"; } }
        public uint image_width { get; set; }
        public uint image_height { get; set; }

        public ImageSpatialExtentsProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.image_width = IsoReaderWriter.ReadUInt32(stream);
            this.image_height = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.image_width);
            size += IsoReaderWriter.WriteUInt32(stream, this.image_height);
            return size;
        }
    }


    public class JPEGConfigurationBox : Box
    {
        public override string FourCC { get { return "jpgC"; } }
        public byte[] JPEGprefix { get; set; }

        public JPEGConfigurationBox()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.JPEGprefix = IsoReaderWriter.ReadUInt8Array(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8Array(stream, this.JPEGprefix);
            return size;
        }
    }


    public class LHEVCConfigurationBox1 : Box
    {
        public override string FourCC { get { return "lhvC"; } }
        public LHEVCDecoderConfigurationRecord LHEVCConfig { get; set; }

        public LHEVCConfigurationBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.LHEVCConfig = (LHEVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.LHEVCConfig);
            return size;
        }
    }


    public class LayerSelectorProperty : ItemProperty
    {
        public override string FourCC { get { return "lsel"; } }
        public ushort layer_id { get; set; }

        public LayerSelectorProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.layer_id = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.layer_id);
            return size;
        }
    }


    public class OperatingPointsInformationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "oinf"; } }
        public OperatingPointsRecord op_info { get; set; }  //  specified in ISO/IEC 14496-15

        public OperatingPointsInformationProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.op_info = (OperatingPointsRecord)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.op_info);
            return size;
        }
    }


    public class PixelAspectRatioBox1 : Box
    {
        public override string FourCC { get { return "pasp"; } }
        public uint hSpacing { get; set; }
        public uint vSpacing { get; set; }

        public PixelAspectRatioBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.hSpacing = IsoReaderWriter.ReadUInt32(stream);
            this.vSpacing = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.hSpacing);
            size += IsoReaderWriter.WriteUInt32(stream, this.vSpacing);
            return size;
        }
    }


    public class PixelInformationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "pixi"; } }
        public byte num_channels { get; set; }
        public byte bits_per_channel { get; set; }

        public PixelInformationProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.num_channels = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i < num_channels; i++)
            {
                this.bits_per_channel = IsoReaderWriter.ReadUInt8(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.num_channels);

            for (int i = 0; i < num_channels; i++)
            {
                size += IsoReaderWriter.WriteUInt8(stream, this.bits_per_channel);
            }
            return size;
        }
    }


    public class RelativeLocationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "rloc"; } }
        public uint horizontal_offset { get; set; }
        public uint vertical_offset { get; set; }

        public RelativeLocationProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.horizontal_offset = IsoReaderWriter.ReadUInt32(stream);
            this.vertical_offset = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.horizontal_offset);
            size += IsoReaderWriter.WriteUInt32(stream, this.vertical_offset);
            return size;
        }
    }


    public class SubSampleInformationBox1 : FullBox
    {
        public override string FourCC { get { return "subs"; } }
        public uint entry_count { get; set; }
        public uint sample_delta { get; set; }
        public ushort subsample_count { get; set; }
        public uint subsample_size { get; set; }
        public byte subsample_priority { get; set; }
        public byte discardable { get; set; }
        public uint codec_specific_parameters { get; set; }

        public SubSampleInformationBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.entry_count = IsoReaderWriter.ReadUInt32(stream);


            for (int i = 0; i < entry_count; i++)
            {
                this.sample_delta = IsoReaderWriter.ReadUInt32(stream);
                this.subsample_count = IsoReaderWriter.ReadUInt16(stream);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            this.subsample_size = IsoReaderWriter.ReadUInt32(stream);
                        }

                        else
                        {
                            this.subsample_size = IsoReaderWriter.ReadUInt16(stream);
                        }
                        this.subsample_priority = IsoReaderWriter.ReadUInt8(stream);
                        this.discardable = IsoReaderWriter.ReadUInt8(stream);
                        this.codec_specific_parameters = IsoReaderWriter.ReadUInt32(stream);
                    }
                }
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.entry_count);


            for (int i = 0; i < entry_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.sample_delta);
                size += IsoReaderWriter.WriteUInt16(stream, this.subsample_count);

                if (subsample_count > 0)
                {

                    for (int j = 0; j < subsample_count; j++)
                    {

                        if (version == 1)
                        {
                            size += IsoReaderWriter.WriteUInt32(stream, this.subsample_size);
                        }

                        else
                        {
                            size += IsoReaderWriter.WriteUInt16(stream, this.subsample_size);
                        }
                        size += IsoReaderWriter.WriteUInt8(stream, this.subsample_priority);
                        size += IsoReaderWriter.WriteUInt8(stream, this.discardable);
                        size += IsoReaderWriter.WriteUInt32(stream, this.codec_specific_parameters);
                    }
                }
            }
            return size;
        }
    }


    public class TargetOlsProperty : ItemFullProperty
    {
        public override string FourCC { get { return "tols"; } }
        public ushort target_ols_idx { get; set; }

        public TargetOlsProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.target_ols_idx = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.target_ols_idx);
            return size;
        }
    }


    public class AutoExposureBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "aebr"; } }
        public sbyte exposure_step { get; set; }
        public sbyte exposure_numerator { get; set; }

        public AutoExposureBracketingEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.exposure_step = IsoReaderWriter.ReadInt8(stream);
            this.exposure_numerator = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8(stream, this.exposure_step);
            size += IsoReaderWriter.WriteInt8(stream, this.exposure_numerator);
            return size;
        }
    }


    public class FlashExposureBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "afbr"; } }
        public sbyte flash_exposure_numerator { get; set; }
        public sbyte flash_exposure_denominator { get; set; }

        public FlashExposureBracketingEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.flash_exposure_numerator = IsoReaderWriter.ReadInt8(stream);
            this.flash_exposure_denominator = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_numerator);
            size += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_denominator);
            return size;
        }
    }


    public class AccessibilityTextProperty : ItemFullProperty
    {
        public override string FourCC { get { return "altt"; } }
        public string alt_text { get; set; }
        public string alt_lang { get; set; }

        public AccessibilityTextProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.alt_text = IsoReaderWriter.ReadString(stream);
            this.alt_lang = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.alt_text);
            size += IsoReaderWriter.WriteString(stream, this.alt_lang);
            return size;
        }
    }


    public class CreationTimeProperty : ItemFullProperty
    {
        public override string FourCC { get { return "crtt"; } }
        public ulong creation_time { get; set; }

        public CreationTimeProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.creation_time = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.creation_time);
            return size;
        }
    }


    public class DepthOfFieldBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dobr"; } }
        public sbyte f_stop_numerator { get; set; }
        public sbyte f_stop_denominator { get; set; }

        public DepthOfFieldBracketingEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.f_stop_numerator = IsoReaderWriter.ReadInt8(stream);
            this.f_stop_denominator = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8(stream, this.f_stop_numerator);
            size += IsoReaderWriter.WriteInt8(stream, this.f_stop_denominator);
            return size;
        }
    }


    public class FocusBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "fobr"; } }
        public ushort focus_distance_numerator { get; set; }
        public ushort focus_distance_denominator { get; set; }

        public FocusBracketingEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.focus_distance_numerator = IsoReaderWriter.ReadUInt16(stream);
            this.focus_distance_denominator = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_numerator);
            size += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_denominator);
            return size;
        }
    }


    public class ImageScaling : ItemFullProperty
    {
        public override string FourCC { get { return "iscl"; } }
        public ushort target_width_numerator { get; set; }
        public ushort target_width_denominator { get; set; }
        public ushort target_height_numerator { get; set; }
        public ushort target_height_denominator { get; set; }

        public ImageScaling()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.target_width_numerator = IsoReaderWriter.ReadUInt16(stream);
            this.target_width_denominator = IsoReaderWriter.ReadUInt16(stream);
            this.target_height_numerator = IsoReaderWriter.ReadUInt16(stream);
            this.target_height_denominator = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.target_width_numerator);
            size += IsoReaderWriter.WriteUInt16(stream, this.target_width_denominator);
            size += IsoReaderWriter.WriteUInt16(stream, this.target_height_numerator);
            size += IsoReaderWriter.WriteUInt16(stream, this.target_height_denominator);
            return size;
        }
    }


    public class ModificationTimeProperty : ItemFullProperty
    {
        public override string FourCC { get { return "mdft"; } }
        public ulong modification_time { get; set; }

        public ModificationTimeProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.modification_time = IsoReaderWriter.ReadUInt64(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt64(stream, this.modification_time);
            return size;
        }
    }


    public class PanoramaEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pano"; } }
        public ushort frame_number { get; set; }

        public PanoramaEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.frame_number = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.frame_number);
            return size;
        }
    }


    public class RequiredReferenceTypesProperty : ItemFullProperty
    {
        public override string FourCC { get { return "rref"; } }
        public byte reference_type_count { get; set; }
        public uint[] reference_type { get; set; }

        public RequiredReferenceTypesProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reference_type_count = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i < reference_type_count; i++)
            {
                this.reference_type[i] = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.reference_type_count);

            for (int i = 0; i < reference_type_count; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.reference_type[i]);
            }
            return size;
        }
    }


    public class UserDescriptionProperty : ItemFullProperty
    {
        public override string FourCC { get { return "udes"; } }
        public string lang { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string tags { get; set; }

        public UserDescriptionProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.lang = IsoReaderWriter.ReadString(stream);
            this.name = IsoReaderWriter.ReadString(stream);
            this.description = IsoReaderWriter.ReadString(stream);
            this.tags = IsoReaderWriter.ReadString(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteString(stream, this.lang);
            size += IsoReaderWriter.WriteString(stream, this.name);
            size += IsoReaderWriter.WriteString(stream, this.description);
            size += IsoReaderWriter.WriteString(stream, this.tags);
            return size;
        }
    }


    public class WhiteBalanceBracketingEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "wbbr"; } }
        public ushort blue_amber { get; set; }
        public sbyte green_magenta { get; set; }

        public WhiteBalanceBracketingEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.blue_amber = IsoReaderWriter.ReadUInt16(stream);
            this.green_magenta = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.blue_amber);
            size += IsoReaderWriter.WriteInt8(stream, this.green_magenta);
            return size;
        }
    }


    public class ContentColourVolumeBox1 : Box
    {
        public override string FourCC { get { return "cclv"; } }
        public bool reserved1 { get; set; } = false;  //  ccv_cancel_flag
        public bool reserved2 { get; set; } = false;  //  ccv_persistence_flag
        public bool ccv_primaries_present_flag { get; set; }
        public bool ccv_min_luminance_value_present_flag { get; set; }
        public bool ccv_max_luminance_value_present_flag { get; set; }
        public bool ccv_avg_luminance_value_present_flag { get; set; }
        public byte ccv_reserved_zero_2bits { get; set; } = 0;
        public int[] ccv_primaries_x { get; set; }
        public int[] ccv_primaries_y { get; set; }
        public uint ccv_min_luminance_value { get; set; }
        public uint ccv_max_luminance_value { get; set; }
        public uint ccv_avg_luminance_value { get; set; }

        public ContentColourVolumeBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.reserved1 = IsoReaderWriter.ReadBit(stream);
            this.reserved2 = IsoReaderWriter.ReadBit(stream);
            this.ccv_primaries_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_min_luminance_value_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_max_luminance_value_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_avg_luminance_value_present_flag = IsoReaderWriter.ReadBit(stream);
            this.ccv_reserved_zero_2bits = IsoReaderWriter.ReadBits(stream, 2);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    this.ccv_primaries_x[c] = IsoReaderWriter.ReadInt32(stream);
                    this.ccv_primaries_y[c] = IsoReaderWriter.ReadInt32(stream);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                this.ccv_min_luminance_value = IsoReaderWriter.ReadUInt32(stream);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                this.ccv_max_luminance_value = IsoReaderWriter.ReadUInt32(stream);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                this.ccv_avg_luminance_value = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.reserved1);
            size += IsoReaderWriter.WriteBit(stream, this.reserved2);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_primaries_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_min_luminance_value_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_max_luminance_value_present_flag);
            size += IsoReaderWriter.WriteBit(stream, this.ccv_avg_luminance_value_present_flag);
            size += IsoReaderWriter.WriteBits(stream, 2, this.ccv_reserved_zero_2bits);

            if (ccv_primaries_present_flag)
            {

                for (int c = 0; c < 3; c++)
                {
                    size += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_x[c]);
                    size += IsoReaderWriter.WriteInt32(stream, this.ccv_primaries_y[c]);
                }
            }

            if (ccv_min_luminance_value_present_flag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.ccv_min_luminance_value);
            }

            if (ccv_max_luminance_value_present_flag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.ccv_max_luminance_value);
            }

            if (ccv_avg_luminance_value_present_flag)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.ccv_avg_luminance_value);
            }
            return size;
        }
    }


    public class MasteringDisplayColourVolumeBox1 : Box
    {
        public override string FourCC { get { return "mdcv"; } }
        public ushort display_primaries_x { get; set; }
        public ushort display_primaries_y { get; set; }
        public ushort white_point_x { get; set; }
        public ushort white_point_y { get; set; }
        public uint max_display_mastering_luminance { get; set; }
        public uint min_display_mastering_luminance { get; set; }

        public MasteringDisplayColourVolumeBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                this.display_primaries_x = IsoReaderWriter.ReadUInt16(stream);
                this.display_primaries_y = IsoReaderWriter.ReadUInt16(stream);
            }
            this.white_point_x = IsoReaderWriter.ReadUInt16(stream);
            this.white_point_y = IsoReaderWriter.ReadUInt16(stream);
            this.max_display_mastering_luminance = IsoReaderWriter.ReadUInt32(stream);
            this.min_display_mastering_luminance = IsoReaderWriter.ReadUInt32(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);

            for (int c = 0; c < 3; c++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_x);
                size += IsoReaderWriter.WriteUInt16(stream, this.display_primaries_y);
            }
            size += IsoReaderWriter.WriteUInt16(stream, this.white_point_x);
            size += IsoReaderWriter.WriteUInt16(stream, this.white_point_y);
            size += IsoReaderWriter.WriteUInt32(stream, this.max_display_mastering_luminance);
            size += IsoReaderWriter.WriteUInt32(stream, this.min_display_mastering_luminance);
            return size;
        }
    }


    public class ContentLightLevelBox1 : Box
    {
        public override string FourCC { get { return "clli"; } }
        public ushort max_content_light_level { get; set; }
        public ushort max_pic_average_light_level { get; set; }

        public ContentLightLevelBox1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.max_content_light_level = IsoReaderWriter.ReadUInt16(stream);
            this.max_pic_average_light_level = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.max_content_light_level);
            size += IsoReaderWriter.WriteUInt16(stream, this.max_pic_average_light_level);
            return size;
        }
    }


    public class WipeTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "wipe"; } }
        public byte transition_direction { get; set; }

        public WipeTransitionEffectProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.transition_direction = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.transition_direction);
            return size;
        }
    }


    public class ZoomTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "zoom"; } }
        public bool transition_direction { get; set; }
        public byte transition_shape { get; set; }

        public ZoomTransitionEffectProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.transition_direction = IsoReaderWriter.ReadBit(stream);
            this.transition_shape = IsoReaderWriter.ReadBits(stream, 7);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteBit(stream, this.transition_direction);
            size += IsoReaderWriter.WriteBits(stream, 7, this.transition_shape);
            return size;
        }
    }


    public class FadeTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "fade"; } }
        public byte transition_direction { get; set; }

        public FadeTransitionEffectProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.transition_direction = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.transition_direction);
            return size;
        }
    }


    public class SplitTransitionEffectProperty : ItemFullProperty
    {
        public override string FourCC { get { return "splt"; } }
        public byte transition_direction { get; set; }

        public SplitTransitionEffectProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.transition_direction = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.transition_direction);
            return size;
        }
    }


    public class SuggestedTransitionPeriodProperty : ItemFullProperty
    {
        public override string FourCC { get { return "stpe"; } }
        public byte transition_period { get; set; }

        public SuggestedTransitionPeriodProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.transition_period = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.transition_period);
            return size;
        }
    }


    public class SuggestedTimeDisplayDurationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "ssld"; } }
        public ushort duration { get; set; }

        public SuggestedTimeDisplayDurationProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.duration = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.duration);
            return size;
        }
    }


    public class MaskConfigurationProperty : ItemFullProperty
    {
        public override string FourCC { get { return "mskC"; } }
        public byte bits_per_pixel { get; set; }

        public MaskConfigurationProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.bits_per_pixel = IsoReaderWriter.ReadUInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt8(stream, this.bits_per_pixel);
            return size;
        }
    }


    public class VvcSubpicIDProperty : ItemFullProperty
    {
        public override string FourCC { get { return "spid"; } }
        public VvcSubpicIDEntry sid_info { get; set; }  //  specified in ISO/IEC 14496-15

        public VvcSubpicIDProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sid_info = (VvcSubpicIDEntry)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.sid_info);
            return size;
        }
    }


    public class VvcSubpicOrderProperty : ItemFullProperty
    {
        public override string FourCC { get { return "spor"; } }
        public VvcSubpicOrderEntry sor_info { get; set; }  //  specified in ISO/IEC 14496-15

        public VvcSubpicOrderProperty()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sor_info = (VvcSubpicOrderEntry)IsoReaderWriter.ReadClass(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteClass(stream, this.sor_info);
            return size;
        }
    }


    public class SingleItemTypeReferenceBox2 : Box
    {
        public override string FourCC { get { return "auxl"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge2 : Box
    {
        public override string FourCC { get { return "auxl"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge2()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox3 : Box
    {
        public override string FourCC { get { return "base"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge3 : Box
    {
        public override string FourCC { get { return "base"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge3()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox4 : Box
    {
        public override string FourCC { get { return "dimg"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge4 : Box
    {
        public override string FourCC { get { return "dimg"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge4()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox5 : Box
    {
        public override string FourCC { get { return "dpnd"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge5 : Box
    {
        public override string FourCC { get { return "dpnd"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge5()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox6 : Box
    {
        public override string FourCC { get { return "exbl"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox6()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge6 : Box
    {
        public override string FourCC { get { return "exbl"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge6()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox7 : Box
    {
        public override string FourCC { get { return "grid"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox7()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge7 : Box
    {
        public override string FourCC { get { return "grid"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge7()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox8 : Box
    {
        public override string FourCC { get { return "thmb"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox8()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge8 : Box
    {
        public override string FourCC { get { return "thmb"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge8()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox9 : Box
    {
        public override string FourCC { get { return "pred"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox9()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge9 : Box
    {
        public override string FourCC { get { return "pred"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge9()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBox10 : Box
    {
        public override string FourCC { get { return "tbas"; } }
        public ushort from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public ushort to_item_ID { get; set; }

        public SingleItemTypeReferenceBox10()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt16(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt16(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt16(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class SingleItemTypeReferenceBoxLarge10 : Box
    {
        public override string FourCC { get { return "tbas"; } }
        public uint from_item_ID { get; set; }
        public ushort reference_count { get; set; }
        public uint to_item_ID { get; set; }

        public SingleItemTypeReferenceBoxLarge10()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.from_item_ID = IsoReaderWriter.ReadUInt32(stream);
            this.reference_count = IsoReaderWriter.ReadUInt16(stream);

            for (int j = 0; j < reference_count; j++)
            {
                this.to_item_ID = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.from_item_ID);
            size += IsoReaderWriter.WriteUInt16(stream, this.reference_count);

            for (int j = 0; j < reference_count; j++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.to_item_ID);
            }
            return size;
        }
    }


    public class VisualEquivalenceEntry : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "eqiv"; } }
        public short time_offset { get; set; }
        public ushort timescale_multiplier { get; set; }

        public VisualEquivalenceEntry()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.time_offset = IsoReaderWriter.ReadInt16(stream);
            this.timescale_multiplier = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt16(stream, this.time_offset);
            size += IsoReaderWriter.WriteUInt16(stream, this.timescale_multiplier);
            return size;
        }
    }


    public class DirectReferenceSamplesList : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "refs"; } }
        public uint sample_id { get; set; }
        public byte num_direct_reference_samples { get; set; }
        public uint direct_reference_sample_id { get; set; }

        public DirectReferenceSamplesList()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.sample_id = IsoReaderWriter.ReadUInt32(stream);
            this.num_direct_reference_samples = IsoReaderWriter.ReadUInt8(stream);

            for (int i = 0; i < num_direct_reference_samples; i++)
            {
                this.direct_reference_sample_id = IsoReaderWriter.ReadUInt32(stream);
            }
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt32(stream, this.sample_id);
            size += IsoReaderWriter.WriteUInt8(stream, this.num_direct_reference_samples);

            for (int i = 0; i < num_direct_reference_samples; i++)
            {
                size += IsoReaderWriter.WriteUInt32(stream, this.direct_reference_sample_id);
            }
            return size;
        }
    }


    public class AutoExposureBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "aebr"; } }
        public sbyte exposure_step { get; set; }
        public sbyte exposure_numerator { get; set; }

        public AutoExposureBracketingEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.exposure_step = IsoReaderWriter.ReadInt8(stream);
            this.exposure_numerator = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8(stream, this.exposure_step);
            size += IsoReaderWriter.WriteInt8(stream, this.exposure_numerator);
            return size;
        }
    }


    public class FlashExposureBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "afbr"; } }
        public sbyte flash_exposure_numerator { get; set; }
        public sbyte flash_exposure_denominator { get; set; }

        public FlashExposureBracketingEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.flash_exposure_numerator = IsoReaderWriter.ReadInt8(stream);
            this.flash_exposure_denominator = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_numerator);
            size += IsoReaderWriter.WriteInt8(stream, this.flash_exposure_denominator);
            return size;
        }
    }


    public class DepthOfFieldBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "dobr"; } }
        public sbyte f_stop_numerator { get; set; }
        public sbyte f_stop_denominator { get; set; }

        public DepthOfFieldBracketingEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.f_stop_numerator = IsoReaderWriter.ReadInt8(stream);
            this.f_stop_denominator = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteInt8(stream, this.f_stop_numerator);
            size += IsoReaderWriter.WriteInt8(stream, this.f_stop_denominator);
            return size;
        }
    }


    public class FocusBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "fobr"; } }
        public ushort focus_distance_numerator { get; set; }
        public ushort focus_distance_denominator { get; set; }

        public FocusBracketingEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.focus_distance_numerator = IsoReaderWriter.ReadUInt16(stream);
            this.focus_distance_denominator = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_numerator);
            size += IsoReaderWriter.WriteUInt16(stream, this.focus_distance_denominator);
            return size;
        }
    }


    public class PanoramaEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "pano"; } }
        public ushort frame_number { get; set; }

        public PanoramaEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.frame_number = IsoReaderWriter.ReadUInt16(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.frame_number);
            return size;
        }
    }


    public class WhiteBalanceBracketingEntry1 : VisualSampleGroupEntry
    {
        public override string FourCC { get { return "wbbr"; } }
        public ushort blue_amber { get; set; }
        public sbyte green_magenta { get; set; }

        public WhiteBalanceBracketingEntry1()
        { }

        public async override Task ReadAsync(Stream stream)
        {
            await base.ReadAsync(stream);
            this.blue_amber = IsoReaderWriter.ReadUInt16(stream);
            this.green_magenta = IsoReaderWriter.ReadInt8(stream);
        }

        public async override Task<ulong> WriteAsync(Stream stream)
        {
            ulong size = 0;
            size += await base.WriteAsync(stream);
            size += IsoReaderWriter.WriteUInt16(stream, this.blue_amber);
            size += IsoReaderWriter.WriteInt8(stream, this.green_magenta);
            return size;
        }
    }



}
