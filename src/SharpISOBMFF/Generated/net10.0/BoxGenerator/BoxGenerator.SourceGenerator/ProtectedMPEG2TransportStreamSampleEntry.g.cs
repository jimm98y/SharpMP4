using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ProtectedMPEG2TransportStreamSampleEntry
	extends MPEG2TSSampleEntry('pm2t') {
	ProtectionSchemeInfoBox		SchemeInformation;
}
*/
public partial class ProtectedMPEG2TransportStreamSampleEntry : MPEG2TSSampleEntry
{
	public const string TYPE = "pm2t";
	public override string DisplayName { get { return "ProtectedMPEG2TransportStreamSampleEntry"; } }
	public ProtectionSchemeInfoBox _SchemeInformation { get { return this.children.OfType<ProtectionSchemeInfoBox>().FirstOrDefault(); } }

	public ProtectedMPEG2TransportStreamSampleEntry(): base(IsoStream.FromFourCC("pm2t"))
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
