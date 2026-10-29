using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AvcNalUnitStorageBox() extends Box('avcn') {
	AVCDecoderConfigurationRecord() AVCConfig;
}
*/
public partial class AvcNalUnitStorageBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AvcNalUnitStorageBox() extends Box('avcn') {
	AVCDecoderConfigurationRecord() AVCConfig;
}"; } }
	public const string TYPE = "avcn";
	public override string DisplayName { get { return "AvcNalUnitStorageBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("traf"); }


	protected AVCDecoderConfigurationRecord AVCConfig; 
	public AVCDecoderConfigurationRecord _AVCConfig { get { return this.AVCConfig; } set { this.AVCConfig = value; } }

	public AvcNalUnitStorageBox(): base(IsoStream.FromFourCC("avcn"))
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
