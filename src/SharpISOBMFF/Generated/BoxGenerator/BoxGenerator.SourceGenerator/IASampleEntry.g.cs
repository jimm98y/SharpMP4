using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// 6.2.3
class IASampleEntry extends AudioSampleEntry('iamf') {
 IAConfigurationBox ia_configuration_box;
}
*/
public partial class IASampleEntry : AudioSampleEntry
{
	public const string TYPE = "iamf";
	public override string DisplayName { get { return "IASampleEntry"; } }
	public IAConfigurationBox IaConfigurationBox { get { return this.children.OfType<IAConfigurationBox>().FirstOrDefault(); } }

	public IASampleEntry(): base(IsoStream.FromFourCC("iamf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ia_configuration_box, "ia_configuration_box"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.ia_configuration_box, "ia_configuration_box"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(ia_configuration_box); // ia_configuration_box
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
