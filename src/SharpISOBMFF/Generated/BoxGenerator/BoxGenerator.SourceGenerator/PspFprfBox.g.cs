using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PspFprfBox() extends Box('FPRF') {
	unsigned int(32) unknown1;
	unsigned int(32) unknown2;
	unsigned int(32) unknown3;
}
*/
public partial class PspFprfBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class PspFprfBox() extends Box('FPRF') {
	unsigned int(32) unknown1;
	unsigned int(32) unknown2;
	unsigned int(32) unknown3;
}"; } }
	public const string TYPE = "FPRF";
	public override string DisplayName { get { return "PspFprfBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("uuid"); }


	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected uint unknown2; 
	public uint Unknown2 { get { return this.unknown2; } set { this.unknown2 = value; } }

	protected uint unknown3; 
	public uint Unknown3 { get { return this.unknown3; } set { this.unknown3 = value; } }

	public PspFprfBox(): base(IsoStream.FromFourCC("FPRF"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown2, "unknown2"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown3, "unknown3"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt32( this.unknown2, "unknown2"); 
		boxSize += stream.WriteUInt32( this.unknown3, "unknown3"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 32; // unknown2
		boxSize += 32; // unknown3
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("unknown1", 32, this.unknown1); 
		boxSize += tracer.Field("unknown2", 32, this.unknown2); 
		boxSize += tracer.Field("unknown3", 32, this.unknown3); 
		return boxSize;
	}
}

}
