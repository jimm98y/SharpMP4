using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class EVCConfigurationBox extends Box('evcC') {
	EVCDecoderConfigurationRecord() EVCConfig;
}
*/
public partial class EVCConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class EVCConfigurationBox extends Box('evcC') {
	EVCDecoderConfigurationRecord() EVCConfig;
}"; } }
	public const string TYPE = "evcC";
	public override string DisplayName { get { return "EVCConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("EVCSampleEntry(evc1)", "VisualSampleEntry(encv)", "RestrictedSampleEntry(resv)", "ipco"); }


	protected EVCDecoderConfigurationRecord EVCConfig; 
	public EVCDecoderConfigurationRecord _EVCConfig { get { return this.EVCConfig; } set { this.EVCConfig = value; } }

	public EVCConfigurationBox(): base(IsoStream.FromFourCC("evcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EVCDecoderConfigurationRecord(),  out this.EVCConfig, "EVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.EVCConfig, "EVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(EVCConfig); // EVCConfig
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("EVCConfig", IsoStream.CalculateClassSize(EVCConfig), this.EVCConfig); 
		return boxSize;
	}
}

}
