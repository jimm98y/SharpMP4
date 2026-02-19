using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class ProtectedRtpReceptionHintSampleEntry
	extends RtpReceptionHintSampleEntry ('prtp') {
	ProtectionSchemeInfoBox		SchemeInformation;
}
*/
public partial class ProtectedRtpReceptionHintSampleEntry : RtpReceptionHintSampleEntry
{
	public const string TYPE = "prtp";
	public override string DisplayName { get { return "ProtectedRtpReceptionHintSampleEntry"; } }
	public ProtectionSchemeInfoBox _SchemeInformation { get { return this.children.OfType<ProtectionSchemeInfoBox>().FirstOrDefault(); } }

	public ProtectedRtpReceptionHintSampleEntry(): base(IsoStream.FromFourCC("prtp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SchemeInformation, "SchemeInformation"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.SchemeInformation, "SchemeInformation"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(SchemeInformation); // SchemeInformation
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
