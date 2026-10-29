using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AVCConfigurationBox extends Box('avcC') {
	AVCDecoderConfigurationRecord() AVCConfig;
}
*/
public partial class AVCConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AVCConfigurationBox extends Box('avcC') {
	AVCDecoderConfigurationRecord() AVCConfig;
}"; } }
	public const string TYPE = "avcC";
	public override string DisplayName { get { return "AVCConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco", "RestrictedSampleEntry(resv)", "AVCSampleEntry(avc1,avc3)", "AVC2SampleEntry(avc2,avc4)", "VisualSampleEntry(encv)", "AVCParameterSampleEntry(avcp)", "IncompleteAVCSampleEntry(icpv)", "VisualSampleEntry(dva1,dvav)"); }


	protected AVCDecoderConfigurationRecord AVCConfig; 
	public AVCDecoderConfigurationRecord _AVCConfig { get { return this.AVCConfig; } set { this.AVCConfig = value; } }

	public AVCConfigurationBox(): base(IsoStream.FromFourCC("avcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new AVCDecoderConfigurationRecord(),  out this.AVCConfig, "AVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.AVCConfig, "AVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(AVCConfig); // AVCConfig
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("AVCConfig", IsoStream.CalculateClassSize(AVCConfig), this.AVCConfig); 
		return boxSize;
	}
}

}
