using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class VvcConfigurationBox extends FullBox('vvcC',version=0,flags) {
	VvcDecoderConfigurationRecord() VvcConfig;
}
*/
public partial class VvcConfigurationBox : FullBox
{
	public const string TYPE = "vvcC";
	public override string DisplayName { get { return "VvcConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VvcSampleEntry(vvc1,vvi1)", "VisualSampleEntry(encv)", "RestrictedSampleEntry(resv)", "ipco"); }


	protected VvcDecoderConfigurationRecord VvcConfig; 
	public VvcDecoderConfigurationRecord _VvcConfig { get { return this.VvcConfig; } set { this.VvcConfig = value; } }

	public VvcConfigurationBox(uint flags = 0): base(IsoStream.FromFourCC("vvcC"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcDecoderConfigurationRecord(),  out this.VvcConfig, "VvcConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.VvcConfig, "VvcConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(VvcConfig); // VvcConfig
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("VvcConfig", IsoStream.CalculateClassSize(VvcConfig), this.VvcConfig); 
		return boxSize;
	}
}

}
