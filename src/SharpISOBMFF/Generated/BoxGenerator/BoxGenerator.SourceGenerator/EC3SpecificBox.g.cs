using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EC3SpecificBox() extends Box('dec3') {
 bit(13) dataRate;
 bit(3) numIndSub;
 EC3SpecificEntry entries[numIndSub + 1];
 }
 
*/
public partial class EC3SpecificBox : Box
{
	public const string TYPE = "dec3";
	public override string DisplayName { get { return "EC3SpecificBox"; } }

	protected ushort dataRate; 
	public ushort DataRate { get { return this.dataRate; } set { this.dataRate = value; } }

	protected byte numIndSub; 
	public byte NumIndSub { get { return this.numIndSub; } set { this.numIndSub = value; } }

	protected EC3SpecificEntry[] entries; 
	public EC3SpecificEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public EC3SpecificBox(): base(IsoStream.FromFourCC("dec3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 13,  out this.dataRate, "dataRate"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.numIndSub, "numIndSub"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(numIndSub + 1), () => new EC3SpecificEntry(),  out this.entries, "entries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(13,  this.dataRate, "dataRate"); 
		boxSize += stream.WriteBits(3,  this.numIndSub, "numIndSub"); 
		boxSize += stream.WriteClass( this.entries, "entries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 13; // dataRate
		boxSize += 3; // numIndSub
		boxSize += IsoStream.CalculateClassSize(entries); // entries
		return boxSize;
	}
}

}
