using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MVCConfigurationBox extends Box('mvcC') {
	MVCDecoderConfigurationRecord() MVCConfig;
}
*/
public partial class MVCConfigurationBox : Box
{
	public const string TYPE = "mvcC";
	public override string DisplayName { get { return "MVCConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCMVCSampleEntry(avc1,avc3)", "AVC2MVCSampleEntry(avc2,avc4)", "MVCSampleEntry(mvc1,mvc2,mvc3,mvc4)"); }


	protected MVCDecoderConfigurationRecord MVCConfig; 
	public MVCDecoderConfigurationRecord _MVCConfig { get { return this.MVCConfig; } set { this.MVCConfig = value; } }

	public MVCConfigurationBox(): base(IsoStream.FromFourCC("mvcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new MVCDecoderConfigurationRecord(),  out this.MVCConfig, "MVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.MVCConfig, "MVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(MVCConfig); // MVCConfig
		return boxSize;
	}
}

}
