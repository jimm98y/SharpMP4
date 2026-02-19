using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AVCLayerEntry() extends VisualSampleGroupEntry ('avll')
{
	unsigned int(8)  layerNumber;
	unsigned int(7)  reserved = 0;
	unsigned int(1)  accurateStatisticsFlag;
	unsigned int(16) avgBitRate;
	unsigned int(16) avgFrameRate;
}
*/
public partial class AVCLayerEntry : VisualSampleGroupEntry
{
	public const string TYPE = "avll";
	public override string DisplayName { get { return "AVCLayerEntry"; } }

	protected byte layerNumber; 
	public byte LayerNumber { get { return this.layerNumber; } set { this.layerNumber = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool accurateStatisticsFlag; 
	public bool AccurateStatisticsFlag { get { return this.accurateStatisticsFlag; } set { this.accurateStatisticsFlag = value; } }

	protected ushort avgBitRate; 
	public ushort AvgBitRate { get { return this.avgBitRate; } set { this.avgBitRate = value; } }

	protected ushort avgFrameRate; 
	public ushort AvgFrameRate { get { return this.avgFrameRate; } set { this.avgFrameRate = value; } }

	public AVCLayerEntry(): base(IsoStream.FromFourCC("avll"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layerNumber, "layerNumber"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.accurateStatisticsFlag, "accurateStatisticsFlag"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgBitRate, "avgBitRate"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgFrameRate, "avgFrameRate"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.layerNumber, "layerNumber"); 
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.accurateStatisticsFlag, "accurateStatisticsFlag"); 
		boxSize += stream.WriteUInt16( this.avgBitRate, "avgBitRate"); 
		boxSize += stream.WriteUInt16( this.avgFrameRate, "avgFrameRate"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // layerNumber
		boxSize += 7; // reserved
		boxSize += 1; // accurateStatisticsFlag
		boxSize += 16; // avgBitRate
		boxSize += 16; // avgFrameRate
		return boxSize;
	}
}

}
