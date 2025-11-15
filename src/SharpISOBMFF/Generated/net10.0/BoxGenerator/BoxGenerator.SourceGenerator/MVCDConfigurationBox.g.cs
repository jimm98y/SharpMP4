using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MVCDConfigurationBox extends Box('mvdC') {
	MVDDecoderConfigurationRecord MVDConfig;
	MVDDepthResolutionBox mvdDepthRes;	//Optional
}
*/
public partial class MVCDConfigurationBox : Box
{
	public const string TYPE = "mvdC";
	public override string DisplayName { get { return "MVCDConfigurationBox"; } }

	protected MVDDecoderConfigurationRecord MVDConfig; 
	public MVDDecoderConfigurationRecord _MVDConfig { get { return this.MVDConfig; } set { this.MVDConfig = value; } }
	public MVDDepthResolutionBox MvdDepthRes { get { return this.children.OfType<MVDDepthResolutionBox>().FirstOrDefault(); } }

	public MVCDConfigurationBox(): base(IsoStream.FromFourCC("mvdC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new MVDDecoderConfigurationRecord(),  out this.MVDConfig, "MVDConfig"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.mvdDepthRes, "mvdDepthRes"); //Optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.MVDConfig, "MVDConfig"); 
		// boxSize += stream.WriteBox( this.mvdDepthRes, "mvdDepthRes"); //Optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(MVDConfig); // MVDConfig
		// boxSize += IsoStream.CalculateBoxSize(mvdDepthRes); // mvdDepthRes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
