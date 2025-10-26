using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ColourInformationBox extends Box('colr'){
	unsigned int(32) colour_type;
	if (colour_type == 'nclx' || colour_type == 'nclc')	/* on-screen colours *//*
	{
		unsigned int(16) colour_primaries;
		unsigned int(16) transfer_characteristics;
		unsigned int(16) matrix_coefficients;
 		if(colour_type == 'nclx') {
 			unsigned int(1)  full_range_flag;
		unsigned int(7)  reserved = 0;
		}
	}
	else if (colour_type == 'rICC')
	{
		ICC_profile;	// restricted ICC profile
	}
	else if (colour_type == 'prof')
	{
		ICC_profile;	// unrestricted ICC profile
	}
}
*/
public partial class ColourInformationBox : Box
{
	public const string TYPE = "colr";
	public override string DisplayName { get { return "ColourInformationBox"; } }

	protected uint colour_type; 
	public uint ColourType { get { return this.colour_type; } set { this.colour_type = value; } }

	protected ushort colour_primaries; 
	public ushort ColourPrimaries { get { return this.colour_primaries; } set { this.colour_primaries = value; } }

	protected ushort transfer_characteristics; 
	public ushort TransferCharacteristics { get { return this.transfer_characteristics; } set { this.transfer_characteristics = value; } }

	protected ushort matrix_coefficients; 
	public ushort MatrixCoefficients { get { return this.matrix_coefficients; } set { this.matrix_coefficients = value; } }

	protected bool full_range_flag; 
	public bool FullRangeFlag { get { return this.full_range_flag; } set { this.full_range_flag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ICC_profile ICC_profile;  //  restricted ICC profile
	public ICC_profile ICCProfile { get { return this.ICC_profile; } set { this.ICC_profile = value; } }

	public ColourInformationBox(): base(IsoStream.FromFourCC("colr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.colour_type, "colour_type"); 

		if (colour_type == IsoStream.FromFourCC("nclx") || colour_type == IsoStream.FromFourCC("nclc"))
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.colour_primaries, "colour_primaries"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.transfer_characteristics, "transfer_characteristics"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.matrix_coefficients, "matrix_coefficients"); 

			if (colour_type == IsoStream.FromFourCC("nclx"))
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.full_range_flag, "full_range_flag"); 
				boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
			}
		}

		else if (colour_type == IsoStream.FromFourCC("rICC"))
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ICC_profile(),  out this.ICC_profile, "ICC_profile"); // restricted ICC profile
		}

		else if (colour_type == IsoStream.FromFourCC("prof"))
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ICC_profile(),  out this.ICC_profile, "ICC_profile"); // unrestricted ICC profile
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.colour_type, "colour_type"); 

		if (colour_type == IsoStream.FromFourCC("nclx") || colour_type == IsoStream.FromFourCC("nclc"))
		{
			boxSize += stream.WriteUInt16( this.colour_primaries, "colour_primaries"); 
			boxSize += stream.WriteUInt16( this.transfer_characteristics, "transfer_characteristics"); 
			boxSize += stream.WriteUInt16( this.matrix_coefficients, "matrix_coefficients"); 

			if (colour_type == IsoStream.FromFourCC("nclx"))
			{
				boxSize += stream.WriteBit( this.full_range_flag, "full_range_flag"); 
				boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
			}
		}

		else if (colour_type == IsoStream.FromFourCC("rICC"))
		{
			boxSize += stream.WriteClass( this.ICC_profile, "ICC_profile"); // restricted ICC profile
		}

		else if (colour_type == IsoStream.FromFourCC("prof"))
		{
			boxSize += stream.WriteClass( this.ICC_profile, "ICC_profile"); // unrestricted ICC profile
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // colour_type

		if (colour_type == IsoStream.FromFourCC("nclx") || colour_type == IsoStream.FromFourCC("nclc"))
		{
			boxSize += 16; // colour_primaries
			boxSize += 16; // transfer_characteristics
			boxSize += 16; // matrix_coefficients

			if (colour_type == IsoStream.FromFourCC("nclx"))
			{
				boxSize += 1; // full_range_flag
				boxSize += 7; // reserved
			}
		}

		else if (colour_type == IsoStream.FromFourCC("rICC"))
		{
			boxSize += IsoStream.CalculateClassSize(ICC_profile); // ICC_profile
		}

		else if (colour_type == IsoStream.FromFourCC("prof"))
		{
			boxSize += IsoStream.CalculateClassSize(ICC_profile); // ICC_profile
		}
		return boxSize;
	}
}

}
