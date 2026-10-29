using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PspProfExtensionBox() extends Box('uuid 50524f4621d24fcebb88695cfac9c740') {
	unsigned int(32) unknown1;
	unsigned int(32) entry_count;
	Box boxes[];
}
*/
public partial class PspProfExtensionBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class PspProfExtensionBox() extends Box('uuid 50524f4621d24fcebb88695cfac9c740') {
	unsigned int(32) unknown1;
	unsigned int(32) entry_count;
	Box boxes[];
}"; } }
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "PspProfExtensionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	protected uint unknown1; 
	public uint Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	public PspProfExtensionBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("50524f4621d24fcebb88695cfac9c740"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		// boxSize += stream.WriteBox( this.boxes, "boxes"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // unknown1
		boxSize += 32; // entry_count
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("unknown1", 32, this.unknown1); 
		boxSize += tracer.Field("entry_count", 32, this.entry_count); 
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
