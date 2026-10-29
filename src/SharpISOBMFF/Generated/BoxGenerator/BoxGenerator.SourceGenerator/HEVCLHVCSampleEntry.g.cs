using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HEVCLHVCSampleEntry() extends HEVCSampleEntry() {
	LHEVCConfigurationBox		lhvcconfig;
}
*/
public partial class HEVCLHVCSampleEntry : HEVCSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class HEVCLHVCSampleEntry() extends HEVCSampleEntry() {
	LHEVCConfigurationBox		lhvcconfig;
}"; } }
	public override string DisplayName { get { return "HEVCLHVCSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "stsd" }); }

	public LHEVCConfigurationBox Lhvcconfig { get { return this.children.OfType<LHEVCConfigurationBox>().FirstOrDefault(); } }

	public HEVCLHVCSampleEntry(): base()
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.lhvcconfig, "lhvcconfig"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.lhvcconfig, "lhvcconfig"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(lhvcconfig); // lhvcconfig
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("lhvcconfig", IsoStream.CalculateBoxSize(lhvcconfig), this.lhvcconfig); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
