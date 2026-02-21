using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class IntrinsicCameraParametersBox extends FullBox ('icam', version=0, flags) { 
 unsigned int(6)  
 reserved=0;  
 unsigned int(10)  ref_view_id; 
 unsigned int(32) prec_focal_length; 
 unsigned int(32) prec_principal_point; 
 unsigned int(32) prec_skew_factor; 
 unsigned int(8) exponent_focal_length_x; 
 signed   int(64) mantissa_focal_length_x; 
 unsigned int(8) exponent_focal_length_y; 
 signed   int(64) mantissa_focal_length_y;  
 unsigned int(8) exponent_principal_point_x; 
 signed   int(64) mantissa_principal_point_x; 
 unsigned int(8) exponent_principal_point_y; 
 signed   int(64) mantissa_principal_point_y; 
 unsigned int(8) exponent_skew_factor; 
 signed   int(64) mantissa_skew_factor; 
} 
*/
public partial class IntrinsicCameraParametersBox : FullBox
{
	public const string TYPE = "icam";
	public override string DisplayName { get { return "IntrinsicCameraParametersBox"; } }

	protected byte reserved =0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort ref_view_id; 
	public ushort RefViewId { get { return this.ref_view_id; } set { this.ref_view_id = value; } }

	protected uint prec_focal_length; 
	public uint PrecFocalLength { get { return this.prec_focal_length; } set { this.prec_focal_length = value; } }

	protected uint prec_principal_point; 
	public uint PrecPrincipalPoint { get { return this.prec_principal_point; } set { this.prec_principal_point = value; } }

	protected uint prec_skew_factor; 
	public uint PrecSkewFactor { get { return this.prec_skew_factor; } set { this.prec_skew_factor = value; } }

	protected byte exponent_focal_length_x; 
	public byte ExponentFocalLengthx { get { return this.exponent_focal_length_x; } set { this.exponent_focal_length_x = value; } }

	protected long mantissa_focal_length_x; 
	public long MantissaFocalLengthx { get { return this.mantissa_focal_length_x; } set { this.mantissa_focal_length_x = value; } }

	protected byte exponent_focal_length_y; 
	public byte ExponentFocalLengthy { get { return this.exponent_focal_length_y; } set { this.exponent_focal_length_y = value; } }

	protected long mantissa_focal_length_y; 
	public long MantissaFocalLengthy { get { return this.mantissa_focal_length_y; } set { this.mantissa_focal_length_y = value; } }

	protected byte exponent_principal_point_x; 
	public byte ExponentPrincipalPointx { get { return this.exponent_principal_point_x; } set { this.exponent_principal_point_x = value; } }

	protected long mantissa_principal_point_x; 
	public long MantissaPrincipalPointx { get { return this.mantissa_principal_point_x; } set { this.mantissa_principal_point_x = value; } }

	protected byte exponent_principal_point_y; 
	public byte ExponentPrincipalPointy { get { return this.exponent_principal_point_y; } set { this.exponent_principal_point_y = value; } }

	protected long mantissa_principal_point_y; 
	public long MantissaPrincipalPointy { get { return this.mantissa_principal_point_y; } set { this.mantissa_principal_point_y = value; } }

	protected byte exponent_skew_factor; 
	public byte ExponentSkewFactor { get { return this.exponent_skew_factor; } set { this.exponent_skew_factor = value; } }

	protected long mantissa_skew_factor; 
	public long MantissaSkewFactor { get { return this.mantissa_skew_factor; } set { this.mantissa_skew_factor = value; } }

	public IntrinsicCameraParametersBox(uint flags = 0): base(IsoStream.FromFourCC("icam"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 10,  out this.ref_view_id, "ref_view_id"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.prec_focal_length, "prec_focal_length"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.prec_principal_point, "prec_principal_point"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.prec_skew_factor, "prec_skew_factor"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_focal_length_x, "exponent_focal_length_x"); 
		boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_focal_length_x, "mantissa_focal_length_x"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_focal_length_y, "exponent_focal_length_y"); 
		boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_focal_length_y, "mantissa_focal_length_y"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_principal_point_x, "exponent_principal_point_x"); 
		boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_principal_point_x, "mantissa_principal_point_x"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_principal_point_y, "exponent_principal_point_y"); 
		boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_principal_point_y, "mantissa_principal_point_y"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.exponent_skew_factor, "exponent_skew_factor"); 
		boxSize += stream.ReadInt64(boxSize, readSize,  out this.mantissa_skew_factor, "mantissa_skew_factor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(10,  this.ref_view_id, "ref_view_id"); 
		boxSize += stream.WriteUInt32( this.prec_focal_length, "prec_focal_length"); 
		boxSize += stream.WriteUInt32( this.prec_principal_point, "prec_principal_point"); 
		boxSize += stream.WriteUInt32( this.prec_skew_factor, "prec_skew_factor"); 
		boxSize += stream.WriteUInt8( this.exponent_focal_length_x, "exponent_focal_length_x"); 
		boxSize += stream.WriteInt64( this.mantissa_focal_length_x, "mantissa_focal_length_x"); 
		boxSize += stream.WriteUInt8( this.exponent_focal_length_y, "exponent_focal_length_y"); 
		boxSize += stream.WriteInt64( this.mantissa_focal_length_y, "mantissa_focal_length_y"); 
		boxSize += stream.WriteUInt8( this.exponent_principal_point_x, "exponent_principal_point_x"); 
		boxSize += stream.WriteInt64( this.mantissa_principal_point_x, "mantissa_principal_point_x"); 
		boxSize += stream.WriteUInt8( this.exponent_principal_point_y, "exponent_principal_point_y"); 
		boxSize += stream.WriteInt64( this.mantissa_principal_point_y, "mantissa_principal_point_y"); 
		boxSize += stream.WriteUInt8( this.exponent_skew_factor, "exponent_skew_factor"); 
		boxSize += stream.WriteInt64( this.mantissa_skew_factor, "mantissa_skew_factor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // reserved
		boxSize += 10; // ref_view_id
		boxSize += 32; // prec_focal_length
		boxSize += 32; // prec_principal_point
		boxSize += 32; // prec_skew_factor
		boxSize += 8; // exponent_focal_length_x
		boxSize += 64; // mantissa_focal_length_x
		boxSize += 8; // exponent_focal_length_y
		boxSize += 64; // mantissa_focal_length_y
		boxSize += 8; // exponent_principal_point_x
		boxSize += 64; // mantissa_principal_point_x
		boxSize += 8; // exponent_principal_point_y
		boxSize += 64; // mantissa_principal_point_y
		boxSize += 8; // exponent_skew_factor
		boxSize += 64; // mantissa_skew_factor
		return boxSize;
	}
}

}
