using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoVideoGroupBox extends TrackGroupTypeBox('ster') 
{
	unsigned int(1) left_view_flag;
	bit(31) reserved;
}
*/
public partial class StereoVideoGroupBox : TrackGroupTypeBox
{
	public const string TYPE = "ster";
	public override string DisplayName { get { return "StereoVideoGroupBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("trgr"); }


	protected bool left_view_flag; 
	public bool LeftViewFlag { get { return this.left_view_flag; } set { this.left_view_flag = value; } }

	protected uint reserved; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public StereoVideoGroupBox(): base(IsoStream.FromFourCC("ster"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.left_view_flag, "left_view_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.left_view_flag, "left_view_flag"); 
		boxSize += stream.WriteBits(31,  this.reserved, "reserved"); 
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("left_view_flag", 1, this.left_view_flag); 
		boxSize += builder.Field("reserved", 31, this.reserved); 
		return boxSize;
	}
}

}
