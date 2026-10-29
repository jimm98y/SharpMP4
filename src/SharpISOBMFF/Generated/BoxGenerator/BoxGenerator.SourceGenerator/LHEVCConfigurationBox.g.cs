using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class LHEVCConfigurationBox extends Box('lhvC') {
	LHEVCDecoderConfigurationRecord() LHEVCConfig;
}
*/
public partial class LHEVCConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class LHEVCConfigurationBox extends Box('lhvC') {
	LHEVCDecoderConfigurationRecord() LHEVCConfig;
}"; } }
	public const string TYPE = "lhvC";
	public override string DisplayName { get { return "LHEVCConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco", "RestrictedSampleEntry(resv)", "VisualSampleEntry(encv)", "LHEVCSampleEntry(lhv1,lhe1)", "HEVCSampleEntry(hvc1,hvc2,hev1,hev2,hvc3,hev3)", "HEVCLHVCSampleEntry(hvc1,hvc2,hev1,hev2,hvc3,hev3)"); }


	protected LHEVCDecoderConfigurationRecord LHEVCConfig; 
	public LHEVCDecoderConfigurationRecord _LHEVCConfig { get { return this.LHEVCConfig; } set { this.LHEVCConfig = value; } }

	public LHEVCConfigurationBox(): base(IsoStream.FromFourCC("lhvC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new LHEVCDecoderConfigurationRecord(),  out this.LHEVCConfig, "LHEVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.LHEVCConfig, "LHEVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(LHEVCConfig); // LHEVCConfig
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("LHEVCConfig", IsoStream.CalculateClassSize(LHEVCConfig), this.LHEVCConfig); 
		return boxSize;
	}
}

}
