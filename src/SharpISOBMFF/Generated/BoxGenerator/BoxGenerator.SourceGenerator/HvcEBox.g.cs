using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Dolby Vision's enhancement layer configuration: an hvcC's record, as GPAC and FFmpeg's mov_read_hvce read it
aligned(8) class HvcEBox() extends Box('hvcE') {
	HEVCDecoderConfigurationRecord() HEVCConfig;
}
*/
public partial class HvcEBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Dolby Vision's enhancement layer configuration: an hvcC's record, as GPAC and FFmpeg's mov_read_hvce read it
aligned(8) class HvcEBox() extends Box('hvcE') {
	HEVCDecoderConfigurationRecord() HEVCConfig;
}"; } }
	public const string TYPE = "hvcE";
	public override string DisplayName { get { return "HvcEBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry(dvhe,dvh1,hvc1,hev1,hvc2,hev2,encv,resv)"); }


	protected HEVCDecoderConfigurationRecord HEVCConfig; 
	public HEVCDecoderConfigurationRecord _HEVCConfig { get { return this.HEVCConfig; } set { this.HEVCConfig = value; } }

	public HvcEBox(): base(IsoStream.FromFourCC("hvcE"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new HEVCDecoderConfigurationRecord(),  out this.HEVCConfig, "HEVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.HEVCConfig, "HEVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(HEVCConfig); // HEVCConfig
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("HEVCConfig", IsoStream.CalculateClassSize(HEVCConfig), this.HEVCConfig); 
		return boxSize;
	}
}

}
