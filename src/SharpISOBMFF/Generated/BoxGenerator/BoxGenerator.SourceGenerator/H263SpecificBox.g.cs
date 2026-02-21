using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class H263SpecificBox() extends Box('d263') {
	 unsigned int(32) vendor;
	 unsigned int(8) decoderVersion;
 unsigned int(8) level;
 unsigned int(8) profile;
 } 
*/
public partial class H263SpecificBox : Box
{
	public const string TYPE = "d263";
	public override string DisplayName { get { return "H263SpecificBox"; } }

	protected uint vendor; 
	public uint Vendor { get { return this.vendor; } set { this.vendor = value; } }

	protected byte decoderVersion; 
	public byte DecoderVersion { get { return this.decoderVersion; } set { this.decoderVersion = value; } }

	protected byte level; 
	public byte Level { get { return this.level; } set { this.level = value; } }

	protected byte profile; 
	public byte Profile { get { return this.profile; } set { this.profile = value; } }

	public H263SpecificBox(): base(IsoStream.FromFourCC("d263"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vendor, "vendor"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.decoderVersion, "decoderVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level, "level"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile, "profile"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.vendor, "vendor"); 
		boxSize += stream.WriteUInt8( this.decoderVersion, "decoderVersion"); 
		boxSize += stream.WriteUInt8( this.level, "level"); 
		boxSize += stream.WriteUInt8( this.profile, "profile"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // vendor
		boxSize += 8; // decoderVersion
		boxSize += 8; // level
		boxSize += 8; // profile
		return boxSize;
	}
}

}
