using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AVCConfigurationBox extends Box('avcC') {
	AVCDecoderConfigurationRecord() AVCConfig;
}
*/
public partial class AVCConfigurationBox : Box
{
	public const string TYPE = "avcC";
	public override string DisplayName { get { return "AVCConfigurationBox"; } }

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
}

}
