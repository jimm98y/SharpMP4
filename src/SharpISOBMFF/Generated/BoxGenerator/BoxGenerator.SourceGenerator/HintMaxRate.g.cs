using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintMaxRate extends Box('maxr') {	// maximum data rate
	uint(32)	period;			// in milliseconds
	uint(32)	bytes; }			// max bytes sent in any period 'period' long including RTP headers
*/
public partial class HintMaxRate : Box
{
	public const string TYPE = "maxr";
	public override string DisplayName { get { return "HintMaxRate"; } }

	protected uint period;  //  in milliseconds
	public uint Period { get { return this.period; } set { this.period = value; } }

	protected uint bytes; 
	public uint Bytes { get { return this.bytes; } set { this.bytes = value; } }

	public HintMaxRate(): base(IsoStream.FromFourCC("maxr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  maximum data rate */
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.period, "period"); // in milliseconds
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  maximum data rate */
		boxSize += stream.WriteUInt32( this.period, "period"); // in milliseconds
		boxSize += stream.WriteUInt32( this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  maximum data rate */
		boxSize += 32; // period
		boxSize += 32; // bytes
		return boxSize;
	}
}

}
