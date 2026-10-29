using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HEVCSampleEntry() extends VisualSampleEntry ('hev1'){
	HEVCConfigurationBox	config;
	MPEG4ExtensionDescriptorsBox () descr;	// optional
}
*/
public partial class HEVCSampleEntryhev1Dup : VisualSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class HEVCSampleEntry() extends VisualSampleEntry ('hev1'){
	HEVCConfigurationBox	config;
	MPEG4ExtensionDescriptorsBox () descr;	// optional
}"; } }
	public const string TYPE = "hev1";
	public override string DisplayName { get { return "HEVCSampleEntryhev1Dup"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public HEVCConfigurationBox Config { get { return this.children.OfType<HEVCConfigurationBox>().FirstOrDefault(); } }
	public MPEG4ExtensionDescriptorsBox Descr { get { return this.children.OfType<MPEG4ExtensionDescriptorsBox>().FirstOrDefault(); } }

	public HEVCSampleEntryhev1Dup(): base(IsoStream.FromFourCC("hev1"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.descr, "descr"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
		// boxSize += stream.WriteBox( this.descr, "descr"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		// boxSize += IsoStream.CalculateBoxSize(descr); // descr
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("config", IsoStream.CalculateBoxSize(config), this.config); 
		// boxSize += tracer.Field("descr", IsoStream.CalculateBoxSize(descr), this.descr); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
