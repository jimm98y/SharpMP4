using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SRTPProcessBox extends FullBox('srpp', version, 0) {
	unsigned int(32)		encryption_algorithm_rtp;
	unsigned int(32)		encryption_algorithm_rtcp;
	unsigned int(32)		integrity_algorithm_rtp;
	unsigned int(32)		integrity_algorithm_rtcp;
	SchemeTypeBox			scheme_type_box;
	SchemeInformationBox	info;
}
*/
public partial class SRTPProcessBox : FullBox
{
	public const string TYPE = "srpp";
	public override string DisplayName { get { return "SRTPProcessBox"; } }

	protected uint encryption_algorithm_rtp; 
	public uint EncryptionAlgorithmRtp { get { return this.encryption_algorithm_rtp; } set { this.encryption_algorithm_rtp = value; } }

	protected uint encryption_algorithm_rtcp; 
	public uint EncryptionAlgorithmRtcp { get { return this.encryption_algorithm_rtcp; } set { this.encryption_algorithm_rtcp = value; } }

	protected uint integrity_algorithm_rtp; 
	public uint IntegrityAlgorithmRtp { get { return this.integrity_algorithm_rtp; } set { this.integrity_algorithm_rtp = value; } }

	protected uint integrity_algorithm_rtcp; 
	public uint IntegrityAlgorithmRtcp { get { return this.integrity_algorithm_rtcp; } set { this.integrity_algorithm_rtcp = value; } }
	public SchemeTypeBox SchemeTypeBox { get { return this.children.OfType<SchemeTypeBox>().FirstOrDefault(); } }
	public SchemeInformationBox Info { get { return this.children.OfType<SchemeInformationBox>().FirstOrDefault(); } }

	public SRTPProcessBox(byte version = 0): base(IsoStream.FromFourCC("srpp"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.encryption_algorithm_rtp, "encryption_algorithm_rtp"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.encryption_algorithm_rtcp, "encryption_algorithm_rtcp"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.integrity_algorithm_rtp, "integrity_algorithm_rtp"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.integrity_algorithm_rtcp, "integrity_algorithm_rtcp"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.scheme_type_box, "scheme_type_box"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.info, "info"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.encryption_algorithm_rtp, "encryption_algorithm_rtp"); 
		boxSize += stream.WriteUInt32( this.encryption_algorithm_rtcp, "encryption_algorithm_rtcp"); 
		boxSize += stream.WriteUInt32( this.integrity_algorithm_rtp, "integrity_algorithm_rtp"); 
		boxSize += stream.WriteUInt32( this.integrity_algorithm_rtcp, "integrity_algorithm_rtcp"); 
		// boxSize += stream.WriteBox( this.scheme_type_box, "scheme_type_box"); 
		// boxSize += stream.WriteBox( this.info, "info"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // encryption_algorithm_rtp
		boxSize += 32; // encryption_algorithm_rtcp
		boxSize += 32; // integrity_algorithm_rtp
		boxSize += 32; // integrity_algorithm_rtcp
		// boxSize += IsoStream.CalculateBoxSize(scheme_type_box); // scheme_type_box
		// boxSize += IsoStream.CalculateBoxSize(info); // info
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
