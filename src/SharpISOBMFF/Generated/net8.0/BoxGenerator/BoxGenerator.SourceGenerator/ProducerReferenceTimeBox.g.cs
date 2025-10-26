using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ProducerReferenceTimeBox
	extends FullBox('prft', version, flags) {
	unsigned int(32) reference_track_ID;
	unsigned int(64) ntp_timestamp;
	if (version==0) {
		unsigned int(32) media_time;
	} else {
		unsigned int(64) media_time;
	}
}
*/
public partial class ProducerReferenceTimeBox : FullBox
{
	public const string TYPE = "prft";
	public override string DisplayName { get { return "ProducerReferenceTimeBox"; } }

	protected uint reference_track_ID; 
	public uint ReferenceTrackID { get { return this.reference_track_ID; } set { this.reference_track_ID = value; } }

	protected ulong ntp_timestamp; 
	public ulong NtpTimestamp { get { return this.ntp_timestamp; } set { this.ntp_timestamp = value; } }

	protected ulong media_time; 
	public ulong MediaTime { get { return this.media_time; } set { this.media_time = value; } }

	public ProducerReferenceTimeBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("prft"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reference_track_ID, "reference_track_ID"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.ntp_timestamp, "ntp_timestamp"); 

		if (version==0)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.media_time, "media_time"); 
		}

		else 
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.media_time, "media_time"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reference_track_ID, "reference_track_ID"); 
		boxSize += stream.WriteUInt64( this.ntp_timestamp, "ntp_timestamp"); 

		if (version==0)
		{
			boxSize += stream.WriteUInt32( this.media_time, "media_time"); 
		}

		else 
		{
			boxSize += stream.WriteUInt64( this.media_time, "media_time"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reference_track_ID
		boxSize += 64; // ntp_timestamp

		if (version==0)
		{
			boxSize += 32; // media_time
		}

		else 
		{
			boxSize += 64; // media_time
		}
		return boxSize;
	}
}

}
