using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SubsegmentIndexBox extends FullBox('ssix', 0, 0) { 
 unsigned int(32) subsegment_count; 
 for( i=1; i <= subsegment_count; i++) 
 { 
  unsigned int(32) range_count; 
  for ( j=1; j <= range_count; j++) { 
   unsigned int(8) level; 
   unsigned int(24) range_size; 
  }  
 } 
} 
*/
public partial class SubsegmentIndexBox : FullBox
{
	public const string TYPE = "ssix";
	public override string DisplayName { get { return "SubsegmentIndexBox"; } }

	protected uint subsegment_count; 
	public uint SubsegmentCount { get { return this.subsegment_count; } set { this.subsegment_count = value; } }

	protected uint[] range_count; 
	public uint[] RangeCount { get { return this.range_count; } set { this.range_count = value; } }

	protected byte[][] level; 
	public byte[][] Level { get { return this.level; } set { this.level = value; } }

	protected uint[][] range_size; 
	public uint[][] RangeSize { get { return this.range_size; } set { this.range_size = value; } }

	public SubsegmentIndexBox(): base(IsoStream.FromFourCC("ssix"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.subsegment_count, "subsegment_count"); 

		this.range_count = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( subsegment_count), "range_count");
		this.level = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( subsegment_count), "level");
		this.range_size = stream.SafeAllocate<uint[]>(boxSize, readSize, IsoStream.GetInt( subsegment_count), "range_size");
		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.range_count[i], "range_count"); 

			this.level[i] = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( range_count[i]), "level[i]");
			this.range_size[i] = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( range_count[i]), "range_size[i]");
			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level[i][j], "level"); 
				boxSize += stream.ReadUInt24(boxSize, readSize,  out this.range_size[i][j], "range_size"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.subsegment_count, "subsegment_count"); 

		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += stream.WriteUInt32( this.range_count[i], "range_count"); 

			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += stream.WriteUInt8( this.level[i][j], "level"); 
				boxSize += stream.WriteUInt24( this.range_size[i][j], "range_size"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // subsegment_count

		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += 32; // range_count

			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += 8; // level
				boxSize += 24; // range_size
			}
		}
		return boxSize;
	}
}

}
