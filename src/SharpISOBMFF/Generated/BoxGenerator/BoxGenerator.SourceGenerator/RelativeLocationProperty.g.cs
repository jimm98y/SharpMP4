using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class RelativeLocationProperty
extends ItemFullProperty('rloc', version = 0, flags = 0)
{
	unsigned int(32) horizontal_offset;
	unsigned int(32) vertical_offset;
}
*/
public partial class RelativeLocationProperty : ItemFullProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class RelativeLocationProperty
extends ItemFullProperty('rloc', version = 0, flags = 0)
{
	unsigned int(32) horizontal_offset;
	unsigned int(32) vertical_offset;
}"; } }
	public const string TYPE = "rloc";
	public override string DisplayName { get { return "RelativeLocationProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected uint horizontal_offset; 
	public uint HorizontalOffset { get { return this.horizontal_offset; } set { this.horizontal_offset = value; } }

	protected uint vertical_offset; 
	public uint VerticalOffset { get { return this.vertical_offset; } set { this.vertical_offset = value; } }

	public RelativeLocationProperty(): base(IsoStream.FromFourCC("rloc"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.horizontal_offset, "horizontal_offset"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vertical_offset, "vertical_offset"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.horizontal_offset, "horizontal_offset"); 
		boxSize += stream.WriteUInt32( this.vertical_offset, "vertical_offset"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // horizontal_offset
		boxSize += 32; // vertical_offset
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("horizontal_offset", 32, this.horizontal_offset); 
		boxSize += tracer.Field("vertical_offset", 32, this.vertical_offset); 
		return boxSize;
	}
}

}
