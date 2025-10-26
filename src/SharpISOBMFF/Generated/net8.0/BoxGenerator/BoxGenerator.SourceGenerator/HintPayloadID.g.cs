using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class HintPayloadID extends Box('payt') {
	uint(32)	payloadID;		// payload ID used in RTP packets
	uint(8)		count;
	char		rtpmap_string[count]; }
*/
public partial class HintPayloadID : Box
{
	public const string TYPE = "payt";
	public override string DisplayName { get { return "HintPayloadID"; } }

	protected uint payloadID;  //  payload ID used in RTP packets
	public uint PayloadID { get { return this.payloadID; } set { this.payloadID = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected byte[] rtpmap_string; 
	public byte[] RtpmapString { get { return this.rtpmap_string; } set { this.rtpmap_string = value; } }

	public HintPayloadID(): base(IsoStream.FromFourCC("payt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.payloadID, "payloadID"); // payload ID used in RTP packets
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.rtpmap_string, "rtpmap_string"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.payloadID, "payloadID"); // payload ID used in RTP packets
		boxSize += stream.WriteUInt8( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.rtpmap_string, "rtpmap_string"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // payloadID
		boxSize += 8; // count
		boxSize += ((ulong)(count) * 8); // rtpmap_string
		return boxSize;
	}
}

}
