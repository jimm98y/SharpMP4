using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PcmCBox() extends FullBox('pcmC') {
	 unsigned int(8) formatFlags;
	 unsigned int(8) pcmSampleSize;
 } 
*/
public partial class PcmCBox : FullBox
{
	public const string TYPE = "pcmC";
	public override string DisplayName { get { return "PcmCBox"; } }

	protected byte formatFlags; 
	public byte FormatFlags { get { return this.formatFlags; } set { this.formatFlags = value; } }

	protected byte pcmSampleSize; 
	public byte PcmSampleSize { get { return this.pcmSampleSize; } set { this.pcmSampleSize = value; } }

	public PcmCBox(): base(IsoStream.FromFourCC("pcmC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.formatFlags, "formatFlags"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pcmSampleSize, "pcmSampleSize"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.formatFlags, "formatFlags"); 
		boxSize += stream.WriteUInt8( this.pcmSampleSize, "pcmSampleSize"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // formatFlags
		boxSize += 8; // pcmSampleSize
		return boxSize;
	}
}

}
