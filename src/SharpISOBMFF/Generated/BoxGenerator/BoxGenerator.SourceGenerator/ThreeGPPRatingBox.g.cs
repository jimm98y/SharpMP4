using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ThreeGPPRatingBox() extends FullBox('rtng', version = 0, 0) {
	unsigned int(32) rating_entity;
	unsigned int(32) rating_criteria;
	bit(1) pad = 0;
 unsigned int(5)[3] language;
	string rating_info;
} 
*/
public partial class ThreeGPPRatingBox : FullBox
{
	public const string TYPE = "rtng";
	public override string DisplayName { get { return "ThreeGPPRatingBox"; } }

	protected uint rating_entity; 
	public uint RatingEntity { get { return this.rating_entity; } set { this.rating_entity = value; } }

	protected uint rating_criteria; 
	public uint RatingCriteria { get { return this.rating_criteria; } set { this.rating_criteria = value; } }

	protected bool pad = false; 
	public bool Pad { get { return this.pad; } set { this.pad = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected BinaryUTF8String rating_info; 
	public BinaryUTF8String RatingInfo { get { return this.rating_info; } set { this.rating_info = value; } }

	public ThreeGPPRatingBox(): base(IsoStream.FromFourCC("rtng"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.rating_entity, "rating_entity"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.rating_criteria, "rating_criteria"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pad, "pad"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.rating_info, "rating_info"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.rating_entity, "rating_entity"); 
		boxSize += stream.WriteUInt32( this.rating_criteria, "rating_criteria"); 
		boxSize += stream.WriteBit( this.pad, "pad"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteStringZeroTerminated( this.rating_info, "rating_info"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // rating_entity
		boxSize += 32; // rating_criteria
		boxSize += 1; // pad
		boxSize += 15; // language
		boxSize += IsoStream.CalculateStringSize(rating_info); // rating_info
		return boxSize;
	}
}

}
