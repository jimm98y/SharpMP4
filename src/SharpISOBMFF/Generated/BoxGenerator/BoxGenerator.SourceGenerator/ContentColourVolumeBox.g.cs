using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ContentColourVolumeBox extends Box('cclv'){
	unsigned int(1) reserved1 = 0;	// ccv_cancel_flag
	unsigned int(1) reserved2 = 0;	// ccv_persistence_flag
	unsigned int(1) ccv_primaries_present_flag;
	unsigned int(1) ccv_min_luminance_value_present_flag;
	unsigned int(1) ccv_max_luminance_value_present_flag;
	unsigned int(1) ccv_avg_luminance_value_present_flag;
	unsigned int(2) ccv_reserved_zero_2bits = 0;
	if( ccv_primaries_present_flag ) {
		for( c = 0; c < 3; c++ ) {
			signed int(32) ccv_primaries_x[ c ];
			signed int(32) ccv_primaries_y[ c ];
		}
	}
	if( ccv_min_luminance_value_present_flag )
		unsigned int(32) ccv_min_luminance_value;
	if( ccv_max_luminance_value_present_flag )
 		unsigned int(32) ccv_max_luminance_value;
	if( ccv_avg_luminance_value_present_flag )
 		unsigned int(32) ccv_avg_luminance_value;
}
*/
public partial class ContentColourVolumeBox : Box
{
	public const string TYPE = "cclv";
	public override string DisplayName { get { return "ContentColourVolumeBox"; } }

	protected bool reserved1 = false;  //  ccv_cancel_flag
	public bool Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected bool reserved2 = false;  //  ccv_persistence_flag
	public bool Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected bool ccv_primaries_present_flag; 
	public bool CcvPrimariesPresentFlag { get { return this.ccv_primaries_present_flag; } set { this.ccv_primaries_present_flag = value; } }

	protected bool ccv_min_luminance_value_present_flag; 
	public bool CcvMinLuminanceValuePresentFlag { get { return this.ccv_min_luminance_value_present_flag; } set { this.ccv_min_luminance_value_present_flag = value; } }

	protected bool ccv_max_luminance_value_present_flag; 
	public bool CcvMaxLuminanceValuePresentFlag { get { return this.ccv_max_luminance_value_present_flag; } set { this.ccv_max_luminance_value_present_flag = value; } }

	protected bool ccv_avg_luminance_value_present_flag; 
	public bool CcvAvgLuminanceValuePresentFlag { get { return this.ccv_avg_luminance_value_present_flag; } set { this.ccv_avg_luminance_value_present_flag = value; } }

	protected byte ccv_reserved_zero_2bits = 0; 
	public byte CcvReservedZero2bits { get { return this.ccv_reserved_zero_2bits; } set { this.ccv_reserved_zero_2bits = value; } }

	protected int[] ccv_primaries_x; 
	public int[] CcvPrimariesx { get { return this.ccv_primaries_x; } set { this.ccv_primaries_x = value; } }

	protected int[] ccv_primaries_y; 
	public int[] CcvPrimariesy { get { return this.ccv_primaries_y; } set { this.ccv_primaries_y = value; } }

	protected uint ccv_min_luminance_value; 
	public uint CcvMinLuminanceValue { get { return this.ccv_min_luminance_value; } set { this.ccv_min_luminance_value = value; } }

	protected uint ccv_max_luminance_value; 
	public uint CcvMaxLuminanceValue { get { return this.ccv_max_luminance_value; } set { this.ccv_max_luminance_value = value; } }

	protected uint ccv_avg_luminance_value; 
	public uint CcvAvgLuminanceValue { get { return this.ccv_avg_luminance_value; } set { this.ccv_avg_luminance_value = value; } }

	public ContentColourVolumeBox(): base(IsoStream.FromFourCC("cclv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved1, "reserved1"); // ccv_cancel_flag
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved2, "reserved2"); // ccv_persistence_flag
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ccv_primaries_present_flag, "ccv_primaries_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ccv_min_luminance_value_present_flag, "ccv_min_luminance_value_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ccv_max_luminance_value_present_flag, "ccv_max_luminance_value_present_flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ccv_avg_luminance_value_present_flag, "ccv_avg_luminance_value_present_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.ccv_reserved_zero_2bits, "ccv_reserved_zero_2bits"); 

		if ( ccv_primaries_present_flag )
		{

			this.ccv_primaries_x = new int[IsoStream.GetInt( 3)];
			this.ccv_primaries_y = new int[IsoStream.GetInt( 3)];
			for (int  c = 0; c < 3; c++ )
			{
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.ccv_primaries_x[ c ], "ccv_primaries_x"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.ccv_primaries_y[ c ], "ccv_primaries_y"); 
			}
		}

		if ( ccv_min_luminance_value_present_flag )
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ccv_min_luminance_value, "ccv_min_luminance_value"); 
		}

		if ( ccv_max_luminance_value_present_flag )
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ccv_max_luminance_value, "ccv_max_luminance_value"); 
		}

		if ( ccv_avg_luminance_value_present_flag )
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ccv_avg_luminance_value, "ccv_avg_luminance_value"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved1, "reserved1"); // ccv_cancel_flag
		boxSize += stream.WriteBit( this.reserved2, "reserved2"); // ccv_persistence_flag
		boxSize += stream.WriteBit( this.ccv_primaries_present_flag, "ccv_primaries_present_flag"); 
		boxSize += stream.WriteBit( this.ccv_min_luminance_value_present_flag, "ccv_min_luminance_value_present_flag"); 
		boxSize += stream.WriteBit( this.ccv_max_luminance_value_present_flag, "ccv_max_luminance_value_present_flag"); 
		boxSize += stream.WriteBit( this.ccv_avg_luminance_value_present_flag, "ccv_avg_luminance_value_present_flag"); 
		boxSize += stream.WriteBits(2,  this.ccv_reserved_zero_2bits, "ccv_reserved_zero_2bits"); 

		if ( ccv_primaries_present_flag )
		{

			for (int  c = 0; c < 3; c++ )
			{
				boxSize += stream.WriteInt32( this.ccv_primaries_x[ c ], "ccv_primaries_x"); 
				boxSize += stream.WriteInt32( this.ccv_primaries_y[ c ], "ccv_primaries_y"); 
			}
		}

		if ( ccv_min_luminance_value_present_flag )
		{
			boxSize += stream.WriteUInt32( this.ccv_min_luminance_value, "ccv_min_luminance_value"); 
		}

		if ( ccv_max_luminance_value_present_flag )
		{
			boxSize += stream.WriteUInt32( this.ccv_max_luminance_value, "ccv_max_luminance_value"); 
		}

		if ( ccv_avg_luminance_value_present_flag )
		{
			boxSize += stream.WriteUInt32( this.ccv_avg_luminance_value, "ccv_avg_luminance_value"); 
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

		if ( ccv_primaries_present_flag )
		{

			for (int  c = 0; c < 3; c++ )
			{
				boxSize += 32; // ccv_primaries_x
				boxSize += 32; // ccv_primaries_y
			}
		}

		if ( ccv_min_luminance_value_present_flag )
		{
			boxSize += 32; // ccv_min_luminance_value
		}

		if ( ccv_max_luminance_value_present_flag )
		{
			boxSize += 32; // ccv_max_luminance_value
		}

		if ( ccv_avg_luminance_value_present_flag )
		{
			boxSize += 32; // ccv_avg_luminance_value
		}
		return boxSize;
	}
}

}
