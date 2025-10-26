using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ExtrinsicCameraParametersBox extends FullBox ('ecam', version=0, flags) { 
 unsigned int(6)  reserved=0; 
 unsigned int(10)  ref_view_id; 
 unsigned int(8) prec_rotation_param; 
 unsigned int(8) prec_translation_param; 
 for (j=1; j<=3; j++) { /* row *//*   
  for (k=1; k<=3; k++) { /* column *//* 
   unsigned int(8) exponent_r[j][k]; 
   signed   int(64) mantissa_r [j][k]; 
  } 
  unsigned int(8) exponent_t[j]; 
  signed   int(64) mantissa_t[j]; 
 } 
}
*/
public partial class ExtrinsicCameraParametersBox : FullBox
{
	public const string TYPE = "ecam";
	public override string DisplayName { get { return "ExtrinsicCameraParametersBox"; } }

	protected byte reserved =0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort ref_view_id; 
	public ushort RefViewId { get { return this.ref_view_id; } set { this.ref_view_id = value; } }

	protected byte prec_rotation_param; 
	public byte PrecRotationParam { get { return this.prec_rotation_param; } set { this.prec_rotation_param = value; } }

	protected byte prec_translation_param; 
	public byte PrecTranslationParam { get { return this.prec_translation_param; } set { this.prec_translation_param = value; } }

	protected byte[][] exponent_r; 
	public byte[][] Exponentr { get { return this.exponent_r; } set { this.exponent_r = value; } }

	protected long[][] mantissa_r; 
	public long[][] Mantissar { get { return this.mantissa_r; } set { this.mantissa_r = value; } }

	protected byte[] exponent_t; 
	public byte[] Exponentt { get { return this.exponent_t; } set { this.exponent_t = value; } }

	protected long[] mantissa_t; 
	public long[] Mantissat { get { return this.mantissa_t; } set { this.mantissa_t = value; } }

	public ExtrinsicCameraParametersBox(uint flags = 0): base(IsoStream.FromFourCC("ecam"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.ref_view_id, "ref_view_id"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.prec_rotation_param, "prec_rotation_param"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.prec_translation_param, "prec_translation_param"); 

		this.exponent_r = new byte[IsoStream.GetInt(3)][];
		this.mantissa_r = new long[IsoStream.GetInt(3)][];
		this.exponent_t = new byte[IsoStream.GetInt(3)];
		this.mantissa_t = new long[IsoStream.GetInt(3)];
		for (int j=0; j<3; j++)
		{
			/*  row  */

			this.exponent_r[j] = new byte[IsoStream.GetInt(3)];
			this.mantissa_r[j] = new long[IsoStream.GetInt(3)];
			for (int k=0; k<3; k++)
			{
				/*  column  */
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_r[j][k], "exponent_r"); 
				boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_r[j][k], "mantissa_r"); 
			}
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_t[j], "exponent_t"); 
			boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_t[j], "mantissa_t"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(10,  this.ref_view_id, "ref_view_id"); 
		boxSize += stream.WriteUInt8( this.prec_rotation_param, "prec_rotation_param"); 
		boxSize += stream.WriteUInt8( this.prec_translation_param, "prec_translation_param"); 

		for (int j=0; j<3; j++)
		{
			/*  row  */

			for (int k=0; k<3; k++)
			{
				/*  column  */
				boxSize += stream.WriteUInt8( this.exponent_r[j][k], "exponent_r"); 
				boxSize += stream.WriteInt64( this.mantissa_r[j][k], "mantissa_r"); 
			}
			boxSize += stream.WriteUInt8( this.exponent_t[j], "exponent_t"); 
			boxSize += stream.WriteInt64( this.mantissa_t[j], "mantissa_t"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 10; // ref_view_id
		boxSize += 8; // prec_rotation_param
		boxSize += 8; // prec_translation_param

		for (int j=0; j<3; j++)
		{
			/*  row  */

			for (int k=0; k<3; k++)
			{
				/*  column  */
				boxSize += 8; // exponent_r
				boxSize += 64; // mantissa_r
			}
			boxSize += 8; // exponent_t
			boxSize += 64; // mantissa_t
		}
		return boxSize;
	}
}

}
