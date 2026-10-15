using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ModificationTimeProperty
extends ItemFullProperty('mdft', version = 0, flags = 0) {
	unsigned int(64)  modification_time;
}

*/
public partial class ModificationTimeProperty : ItemFullProperty
{
	public const string TYPE = "mdft";
	public override string DisplayName { get { return "ModificationTimeProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected ulong modification_time; 
	public ulong ModificationTime { get { return this.modification_time; } set { this.modification_time = value; } }

	public ModificationTimeProperty(): base(IsoStream.FromFourCC("mdft"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.modification_time, "modification_time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.modification_time, "modification_time"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // modification_time
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("modification_time", 64, this.modification_time); 
		return boxSize;
	}
}

}
