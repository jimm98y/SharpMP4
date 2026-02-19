using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AVCParameterSampleEntry() extends VisualSampleEntry ('avcp'){
	AVCConfigurationBox	config;
}
*/
public partial class AVCParameterSampleEntry : VisualSampleEntry
{
	public const string TYPE = "avcp";
	public override string DisplayName { get { return "AVCParameterSampleEntry"; } }
	public AVCConfigurationBox Config { get { return this.children.OfType<AVCConfigurationBox>().FirstOrDefault(); } }

	public AVCParameterSampleEntry(): base(IsoStream.FromFourCC("avcp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
