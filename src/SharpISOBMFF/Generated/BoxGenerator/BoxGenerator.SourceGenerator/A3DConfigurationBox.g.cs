using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class A3DConfigurationBox extends Box('a3dC') {
	MVDDecoderConfigurationRecord MVDConfig;
	MVDDepthResolutionBox mvdDepthRes;	//Optional
}
*/
public partial class A3DConfigurationBox : Box
{
	public const string TYPE = "a3dC";
	public override string DisplayName { get { return "A3DConfigurationBox"; } }

	protected MVDDecoderConfigurationRecord MVDConfig; 
	public MVDDecoderConfigurationRecord _MVDConfig { get { return this.MVDConfig; } set { this.MVDConfig = value; } }
	public MVDDepthResolutionBox MvdDepthRes { get { return this.children.OfType<MVDDepthResolutionBox>().FirstOrDefault(); } }

	public A3DConfigurationBox(): base(IsoStream.FromFourCC("a3dC"))
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
