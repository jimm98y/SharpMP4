using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SLConfigDescriptor extends BaseDescriptor : bit(8) tag=SLConfigDescrTag {
 bit(8) predefined;
 if (predefined==0) {
 bit(1) useAccessUnitStartFlag;
 bit(1) useAccessUnitEndFlag;
 bit(1) useRandomAccessPointFlag;
 bit(1) hasRandomAccessUnitsOnlyFlag;
 bit(1) usePaddingFlag;
 bit(1) useTimeStampsFlag;
 bit(1) useIdleFlag;
 bit(1) durationFlag;
 bit(32) timeStampResolution;
 bit(32) OCRResolution;
 bit(8) timeStampLength; // must be <= 64
 bit(8) OCRLength; // must be <= 64
 bit(8) AU_Length; // must be <= 32
 bit(8) instantBitrateLength;
 bit(4) degradationPriorityLength;
 bit(5) AU_seqNumLength; // must be <= 16
 bit(5) packetSeqNumLength; // must be <= 16
 bit(2) reserved=0b11;
 }
 if (durationFlag) {
 bit(32) timeScale;
 bit(16) accessUnitDuration;
 bit(16) compositionUnitDuration;
 }
 if (!useTimeStampsFlag) {
 bit(timeStampLength) startDecodingTimeStamp;
 bit(timeStampLength) startCompositionTimeStamp;
 }
 bit(8) ocr[]; // OCR stream flag, reserved, OCR_ES_id 
 }
*/
public partial class SLConfigDescriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.SLConfigDescrTag;
	public override string DisplayName { get { return "SLConfigDescriptor"; } }

	protected byte predefined; 
	public byte Predefined { get { return this.predefined; } set { this.predefined = value; } }

	protected bool useAccessUnitStartFlag; 
	public bool UseAccessUnitStartFlag { get { return this.useAccessUnitStartFlag; } set { this.useAccessUnitStartFlag = value; } }

	protected bool useAccessUnitEndFlag; 
	public bool UseAccessUnitEndFlag { get { return this.useAccessUnitEndFlag; } set { this.useAccessUnitEndFlag = value; } }

	protected bool useRandomAccessPointFlag; 
	public bool UseRandomAccessPointFlag { get { return this.useRandomAccessPointFlag; } set { this.useRandomAccessPointFlag = value; } }

	protected bool hasRandomAccessUnitsOnlyFlag; 
	public bool HasRandomAccessUnitsOnlyFlag { get { return this.hasRandomAccessUnitsOnlyFlag; } set { this.hasRandomAccessUnitsOnlyFlag = value; } }

	protected bool usePaddingFlag; 
	public bool UsePaddingFlag { get { return this.usePaddingFlag; } set { this.usePaddingFlag = value; } }

	protected bool useTimeStampsFlag; 
	public bool UseTimeStampsFlag { get { return this.useTimeStampsFlag; } set { this.useTimeStampsFlag = value; } }

	protected bool useIdleFlag; 
	public bool UseIdleFlag { get { return this.useIdleFlag; } set { this.useIdleFlag = value; } }

	protected bool durationFlag; 
	public bool DurationFlag { get { return this.durationFlag; } set { this.durationFlag = value; } }

	protected uint timeStampResolution; 
	public uint TimeStampResolution { get { return this.timeStampResolution; } set { this.timeStampResolution = value; } }

	protected uint OCRResolution; 
	public uint _OCRResolution { get { return this.OCRResolution; } set { this.OCRResolution = value; } }

	protected byte timeStampLength;  //  must be <= 64
	public byte TimeStampLength { get { return this.timeStampLength; } set { this.timeStampLength = value; } }

	protected byte OCRLength;  //  must be <= 64
	public byte _OCRLength { get { return this.OCRLength; } set { this.OCRLength = value; } }

	protected byte AU_Length;  //  must be <= 32
	public byte AULength { get { return this.AU_Length; } set { this.AU_Length = value; } }

	protected byte instantBitrateLength; 
	public byte InstantBitrateLength { get { return this.instantBitrateLength; } set { this.instantBitrateLength = value; } }

	protected byte degradationPriorityLength; 
	public byte DegradationPriorityLength { get { return this.degradationPriorityLength; } set { this.degradationPriorityLength = value; } }

	protected byte AU_seqNumLength;  //  must be <= 16
	public byte AUSeqNumLength { get { return this.AU_seqNumLength; } set { this.AU_seqNumLength = value; } }

	protected byte packetSeqNumLength;  //  must be <= 16
	public byte PacketSeqNumLength { get { return this.packetSeqNumLength; } set { this.packetSeqNumLength = value; } }

	protected byte reserved =0b11; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint timeScale; 
	public uint TimeScale { get { return this.timeScale; } set { this.timeScale = value; } }

	protected ushort accessUnitDuration; 
	public ushort AccessUnitDuration { get { return this.accessUnitDuration; } set { this.accessUnitDuration = value; } }

	protected ushort compositionUnitDuration; 
	public ushort CompositionUnitDuration { get { return this.compositionUnitDuration; } set { this.compositionUnitDuration = value; } }

	protected byte[] startDecodingTimeStamp; 
	public byte[] StartDecodingTimeStamp { get { return this.startDecodingTimeStamp; } set { this.startDecodingTimeStamp = value; } }

	protected byte[] startCompositionTimeStamp; 
	public byte[] StartCompositionTimeStamp { get { return this.startCompositionTimeStamp; } set { this.startCompositionTimeStamp = value; } }

	protected byte[] ocr;  //  OCR stream flag, reserved, OCR_ES_id 
	public byte[] Ocr { get { return this.ocr; } set { this.ocr = value; } }

	public SLConfigDescriptor(): base(DescriptorTags.SLConfigDescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.predefined, "predefined"); 

		if (predefined==0)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.useAccessUnitStartFlag, "useAccessUnitStartFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.useAccessUnitEndFlag, "useAccessUnitEndFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.useRandomAccessPointFlag, "useRandomAccessPointFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.hasRandomAccessUnitsOnlyFlag, "hasRandomAccessUnitsOnlyFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.usePaddingFlag, "usePaddingFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.useTimeStampsFlag, "useTimeStampsFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.useIdleFlag, "useIdleFlag"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.durationFlag, "durationFlag"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timeStampResolution, "timeStampResolution"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.OCRResolution, "OCRResolution"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.timeStampLength, "timeStampLength"); // must be <= 64
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.OCRLength, "OCRLength"); // must be <= 64
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.AU_Length, "AU_Length"); // must be <= 32
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.instantBitrateLength, "instantBitrateLength"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.degradationPriorityLength, "degradationPriorityLength"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.AU_seqNumLength, "AU_seqNumLength"); // must be <= 16
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.packetSeqNumLength, "packetSeqNumLength"); // must be <= 16
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.reserved, "reserved"); 
		}

		if (durationFlag)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timeScale, "timeScale"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.accessUnitDuration, "accessUnitDuration"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.compositionUnitDuration, "compositionUnitDuration"); 
		}

		if (!useTimeStampsFlag)
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(timeStampLength ),  out this.startDecodingTimeStamp, "startDecodingTimeStamp"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(timeStampLength ),  out this.startCompositionTimeStamp, "startCompositionTimeStamp"); 
		}
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.ocr, "ocr"); // OCR stream flag, reserved, OCR_ES_id 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.predefined, "predefined"); 

		if (predefined==0)
		{
			boxSize += stream.WriteBit( this.useAccessUnitStartFlag, "useAccessUnitStartFlag"); 
			boxSize += stream.WriteBit( this.useAccessUnitEndFlag, "useAccessUnitEndFlag"); 
			boxSize += stream.WriteBit( this.useRandomAccessPointFlag, "useRandomAccessPointFlag"); 
			boxSize += stream.WriteBit( this.hasRandomAccessUnitsOnlyFlag, "hasRandomAccessUnitsOnlyFlag"); 
			boxSize += stream.WriteBit( this.usePaddingFlag, "usePaddingFlag"); 
			boxSize += stream.WriteBit( this.useTimeStampsFlag, "useTimeStampsFlag"); 
			boxSize += stream.WriteBit( this.useIdleFlag, "useIdleFlag"); 
			boxSize += stream.WriteBit( this.durationFlag, "durationFlag"); 
			boxSize += stream.WriteUInt32( this.timeStampResolution, "timeStampResolution"); 
			boxSize += stream.WriteUInt32( this.OCRResolution, "OCRResolution"); 
			boxSize += stream.WriteUInt8( this.timeStampLength, "timeStampLength"); // must be <= 64
			boxSize += stream.WriteUInt8( this.OCRLength, "OCRLength"); // must be <= 64
			boxSize += stream.WriteUInt8( this.AU_Length, "AU_Length"); // must be <= 32
			boxSize += stream.WriteUInt8( this.instantBitrateLength, "instantBitrateLength"); 
			boxSize += stream.WriteBits(4,  this.degradationPriorityLength, "degradationPriorityLength"); 
			boxSize += stream.WriteBits(5,  this.AU_seqNumLength, "AU_seqNumLength"); // must be <= 16
			boxSize += stream.WriteBits(5,  this.packetSeqNumLength, "packetSeqNumLength"); // must be <= 16
			boxSize += stream.WriteBits(2,  this.reserved, "reserved"); 
		}

		if (durationFlag)
		{
			boxSize += stream.WriteUInt32( this.timeScale, "timeScale"); 
			boxSize += stream.WriteUInt16( this.accessUnitDuration, "accessUnitDuration"); 
			boxSize += stream.WriteUInt16( this.compositionUnitDuration, "compositionUnitDuration"); 
		}

		if (!useTimeStampsFlag)
		{
			boxSize += stream.WriteBits((uint)(timeStampLength ),  this.startDecodingTimeStamp, "startDecodingTimeStamp"); 
			boxSize += stream.WriteBits((uint)(timeStampLength ),  this.startCompositionTimeStamp, "startCompositionTimeStamp"); 
		}
		boxSize += stream.WriteUInt8ArrayTillEnd( this.ocr, "ocr"); // OCR stream flag, reserved, OCR_ES_id 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // predefined

		if (predefined==0)
		{
			boxSize += 1; // useAccessUnitStartFlag
			boxSize += 1; // useAccessUnitEndFlag
			boxSize += 1; // useRandomAccessPointFlag
			boxSize += 1; // hasRandomAccessUnitsOnlyFlag
			boxSize += 1; // usePaddingFlag
			boxSize += 1; // useTimeStampsFlag
			boxSize += 1; // useIdleFlag
			boxSize += 1; // durationFlag
			boxSize += 32; // timeStampResolution
			boxSize += 32; // OCRResolution
			boxSize += 8; // timeStampLength
			boxSize += 8; // OCRLength
			boxSize += 8; // AU_Length
			boxSize += 8; // instantBitrateLength
			boxSize += 4; // degradationPriorityLength
			boxSize += 5; // AU_seqNumLength
			boxSize += 5; // packetSeqNumLength
			boxSize += 2; // reserved
		}

		if (durationFlag)
		{
			boxSize += 32; // timeScale
			boxSize += 16; // accessUnitDuration
			boxSize += 16; // compositionUnitDuration
		}

		if (!useTimeStampsFlag)
		{
			boxSize += (ulong)(timeStampLength ); // startDecodingTimeStamp
			boxSize += (ulong)(timeStampLength ); // startCompositionTimeStamp
		}
		boxSize += ((ulong)ocr.Length * 8); // ocr
		return boxSize;
	}
}

}
