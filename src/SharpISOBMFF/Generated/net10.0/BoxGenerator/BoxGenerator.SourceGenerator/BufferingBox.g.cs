using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class BufferingBox extends Box('buff'){
	unsigned int(16) 		operating_point_count
;	for (i = 0; i < operating_point_count; i++){
		unsigned int (32) 	byte_rate;
		unsigned int (32) 	cpb_size;
		unsigned int (32) 	dpb_size;
		unsigned int (32)		init_cpb_delay;
		unsigned int (32) 	init_dpb_delay;
	}
}
*/
public partial class BufferingBox : Box
{
	public const string TYPE = "buff";
	public override string DisplayName { get { return "BufferingBox"; } }

	protected ushort operating_point_count; 
	public ushort OperatingPointCount { get { return this.operating_point_count; } set { this.operating_point_count = value; } }

	protected uint[] byte_rate; 
	public uint[] ByteRate { get { return this.byte_rate; } set { this.byte_rate = value; } }

	protected uint[] cpb_size; 
	public uint[] CpbSize { get { return this.cpb_size; } set { this.cpb_size = value; } }

	protected uint[] dpb_size; 
	public uint[] DpbSize { get { return this.dpb_size; } set { this.dpb_size = value; } }

	protected uint[] init_cpb_delay; 
	public uint[] InitCpbDelay { get { return this.init_cpb_delay; } set { this.init_cpb_delay = value; } }

	protected uint[] init_dpb_delay; 
	public uint[] InitDpbDelay { get { return this.init_dpb_delay; } set { this.init_dpb_delay = value; } }

	public BufferingBox(): base(IsoStream.FromFourCC("buff"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.operating_point_count, "operating_point_count"); 

		this.byte_rate = new uint[IsoStream.GetInt( operating_point_count)];
		this.cpb_size = new uint[IsoStream.GetInt( operating_point_count)];
		this.dpb_size = new uint[IsoStream.GetInt( operating_point_count)];
		this.init_cpb_delay = new uint[IsoStream.GetInt( operating_point_count)];
		this.init_dpb_delay = new uint[IsoStream.GetInt( operating_point_count)];
		for (int i = 0; i < operating_point_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.byte_rate[i], "byte_rate"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cpb_size[i], "cpb_size"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dpb_size[i], "dpb_size"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.init_cpb_delay[i], "init_cpb_delay"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.init_dpb_delay[i], "init_dpb_delay"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.operating_point_count, "operating_point_count"); 

		for (int i = 0; i < operating_point_count; i++)
		{
			boxSize += stream.WriteUInt32( this.byte_rate[i], "byte_rate"); 
			boxSize += stream.WriteUInt32( this.cpb_size[i], "cpb_size"); 
			boxSize += stream.WriteUInt32( this.dpb_size[i], "dpb_size"); 
			boxSize += stream.WriteUInt32( this.init_cpb_delay[i], "init_cpb_delay"); 
			boxSize += stream.WriteUInt32( this.init_dpb_delay[i], "init_dpb_delay"); 
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

}
