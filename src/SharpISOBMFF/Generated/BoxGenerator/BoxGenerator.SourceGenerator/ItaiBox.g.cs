using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ItaiBox() extends FullBox('itai') {
	 unsigned int(64) tai_timestamp;
 unsigned int(8) status_bits;
 } 
*/
public partial class ItaiBox : FullBox
{
	public const string TYPE = "itai";
	public override string DisplayName { get { return "ItaiBox"; } }

	protected ulong tai_timestamp; 
	public ulong TaiTimestamp { get { return this.tai_timestamp; } set { this.tai_timestamp = value; } }

	protected byte status_bits; 
	public byte StatusBits { get { return this.status_bits; } set { this.status_bits = value; } }

	public ItaiBox(): base(IsoStream.FromFourCC("itai"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.tai_timestamp, "tai_timestamp"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.status_bits, "status_bits"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.tai_timestamp, "tai_timestamp"); 
		boxSize += stream.WriteUInt8( this.status_bits, "status_bits"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // tai_timestamp
		boxSize += 8; // status_bits
		return boxSize;
	}
}

}
