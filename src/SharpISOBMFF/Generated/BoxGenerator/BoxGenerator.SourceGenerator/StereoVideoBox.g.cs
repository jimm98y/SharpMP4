using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoVideoBox extends FullBox('stvi', version = 0, 0)
{
	template unsigned int(30) reserved = 0;
	unsigned int(2)	single_view_allowed;
	unsigned int(32)	stereo_scheme;
	unsigned int(32)	length;
	unsigned int(8)[length]	stereo_indication_type;
	Box[] any_box; // optional
}
*/
public partial class StereoVideoBox : FullBox
{
	public const string TYPE = "stvi";
	public override string DisplayName { get { return "StereoVideoBox"; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte single_view_allowed; 
	public byte SingleViewAllowed { get { return this.single_view_allowed; } set { this.single_view_allowed = value; } }

	protected uint stereo_scheme; 
	public uint StereoScheme { get { return this.stereo_scheme; } set { this.stereo_scheme = value; } }

	protected uint length; 
	public uint Length { get { return this.length; } set { this.length = value; } }

	protected byte[] stereo_indication_type; 
	public byte[] StereoIndicationType { get { return this.stereo_indication_type; } set { this.stereo_indication_type = value; } }
	public IEnumerable<Box> AnyBox { get { return this.children.OfType<Box>(); } }

	public StereoVideoBox(): base(IsoStream.FromFourCC("stvi"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 30,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.single_view_allowed, "single_view_allowed"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.stereo_scheme, "stereo_scheme"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.length, "length"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(length),  out this.stereo_indication_type, "stereo_indication_type"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.any_box, "any_box"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(30,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.single_view_allowed, "single_view_allowed"); 
		boxSize += stream.WriteUInt32( this.stereo_scheme, "stereo_scheme"); 
		boxSize += stream.WriteUInt32( this.length, "length"); 
		boxSize += stream.WriteUInt8Array((uint)(length),  this.stereo_indication_type, "stereo_indication_type"); 
		// boxSize += stream.WriteBox( this.any_box, "any_box"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
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
		boxSize += ((ulong)(length) * 8); // stereo_indication_type
		// boxSize += IsoStream.CalculateBoxSize(any_box); // any_box
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
