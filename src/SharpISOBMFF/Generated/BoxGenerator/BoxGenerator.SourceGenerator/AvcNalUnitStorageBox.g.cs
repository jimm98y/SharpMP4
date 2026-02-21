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
	public const string TYPE = "avcn";
	public override string DisplayName { get { return "AvcNalUnitStorageBox"; } }

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
}

}
