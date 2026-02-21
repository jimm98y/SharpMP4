using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FullBox(unsigned int(32) boxtype, unsigned int(8) v, bit(24) f)
extends Box(boxtype) { 
unsigned int(8) version = v;
bit(24) flags = f;
 }
*/
public partial class FullBox : Box
{
	public override string DisplayName { get { return "FullBox"; } }

	protected byte version; // = v
	public byte Version { get { return this.version; } set { this.version = value; } }

	protected uint flags; // = f
	public uint Flags { get { return this.flags; } set { this.flags = value; } }
public FullBox(uint boxtype, byte[] uuid) : base(boxtype, uuid) { }


	public FullBox(uint boxtype, byte v = 0, uint f = 0): base(boxtype)
	{
		this.version = v;
		 this.flags = f;	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt24(boxSize, readSize,  out this.flags, "flags"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.version, "version"); 
		boxSize += stream.WriteUInt24( this.flags, "flags"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // version
		boxSize += 24; // flags
		return boxSize;
	}
}

}
