using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PrimaryItemBox
		extends FullBox('pitm', version, 0) {
	if (version == 0) {
		unsigned int(16)	item_ID;
	} else {
		unsigned int(32)	item_ID;
	}
}
*/
public partial class PrimaryItemBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class PrimaryItemBox
		extends FullBox('pitm', version, 0) {
	if (version == 0) {
		unsigned int(16)	item_ID;
	} else {
		unsigned int(32)	item_ID;
	}
}"; } }
	public const string TYPE = "pitm";
	public override string DisplayName { get { return "PrimaryItemBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }


	protected uint item_ID; 
	public uint ItemID { get { return this.item_ID; } set { this.item_ID = value; } }

	public PrimaryItemBox(byte version = 0): base(IsoStream.FromFourCC("pitm"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_ID, "item_ID"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.item_ID, "item_ID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0)
		{
			boxSize += stream.WriteUInt16( this.item_ID, "item_ID"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.item_ID, "item_ID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0)
		{
			boxSize += 16; // item_ID
		}

		else 
		{
			boxSize += 32; // item_ID
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		if (version == 0)
		{
			boxSize += tracer.Field("item_ID", 16, this.item_ID); 
		}

		else 
		{
			boxSize += tracer.Field("item_ID", 32, this.item_ID); 
		}
		return boxSize;
	}
}

}
