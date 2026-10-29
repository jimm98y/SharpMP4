using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class LHEVCSampleEntry() extends VisualSampleEntry ('lhe1') {
	LHEVCConfigurationBox		lhvcconfig;
	MPEG4ExtensionDescriptorsBox () descr;	// optional
}
*/
public partial class LHEVCSampleEntrylhe1Dup : VisualSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class LHEVCSampleEntry() extends VisualSampleEntry ('lhe1') {
	LHEVCConfigurationBox		lhvcconfig;
	MPEG4ExtensionDescriptorsBox () descr;	// optional
}"; } }
	public const string TYPE = "lhe1";
	public override string DisplayName { get { return "LHEVCSampleEntrylhe1Dup"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public LHEVCConfigurationBox Lhvcconfig { get { return this.children.OfType<LHEVCConfigurationBox>().FirstOrDefault(); } }
	public MPEG4ExtensionDescriptorsBox Descr { get { return this.children.OfType<MPEG4ExtensionDescriptorsBox>().FirstOrDefault(); } }

	public LHEVCSampleEntrylhe1Dup(): base(IsoStream.FromFourCC("lhe1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.lhvcconfig, "lhvcconfig"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.descr, "descr"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.lhvcconfig, "lhvcconfig"); 
		// boxSize += stream.WriteBox( this.descr, "descr"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(lhvcconfig); // lhvcconfig
		// boxSize += IsoStream.CalculateBoxSize(descr); // descr
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("lhvcconfig", IsoStream.CalculateBoxSize(lhvcconfig), this.lhvcconfig); 
		// boxSize += tracer.Field("descr", IsoStream.CalculateBoxSize(descr), this.descr); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
