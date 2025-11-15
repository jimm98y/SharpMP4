using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MpodBox() extends Box('mpod') {
 unsigned int(32) unknown1; unsigned int(32) unknown2;
 } 
*/
public partial class MpodBox : Box
{
	public const string TYPE = "mpod";
	public override string DisplayName { get { return "MpodBox"; } }

	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected uint unknown2; 
	public uint Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }

	public MpodBox(): base(IsoStream.FromFourCC("mpod"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown2, "unknown2"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt32( this.unknown2, "unknown2"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 32; // unknown2
		return boxSize;
	}
}

}
