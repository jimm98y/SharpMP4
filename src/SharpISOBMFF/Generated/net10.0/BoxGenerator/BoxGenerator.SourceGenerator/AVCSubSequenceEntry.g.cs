using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AVCSubSequenceEntry () extends VisualSampleGroupEntry ('avss') 
{ 
  unsigned int(16) subSequenceIdentifer; 
  unsigned int(8)  layerNumber;  
  unsigned int(1)  durationFlag; 
  unsigned int(1)  avgRateFlag; 
  unsigned int(6)  reserved = 0;  
  if (durationFlag) 
   unsigned int(32) duration; 
  if (avgRateFlag) 
  {
   unsigned int(8)  accurateStatisticsFlag; 
   unsigned int(16) avgBitRate; 
   unsigned int(16) avgFrameRate; 
  }
  unsigned int(8) numReferences; 
  DependencyInfo dependency[numReferences]; 
 } 

*/
public partial class AVCSubSequenceEntry : VisualSampleGroupEntry
{
	public const string TYPE = "avss";
	public override string DisplayName { get { return "AVCSubSequenceEntry"; } }

	protected ushort subSequenceIdentifer; 
	public ushort SubSequenceIdentifer { get { return this.subSequenceIdentifer; } set { this.subSequenceIdentifer = value; } }

	protected byte layerNumber; 
	public byte LayerNumber { get { return this.layerNumber; } set { this.layerNumber = value; } }

	protected bool durationFlag; 
	public bool DurationFlag { get { return this.durationFlag; } set { this.durationFlag = value; } }

	protected bool avgRateFlag; 
	public bool AvgRateFlag { get { return this.avgRateFlag; } set { this.avgRateFlag = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint duration; 
	public uint Duration { get { return this.duration; } set { this.duration = value; } }

	protected byte accurateStatisticsFlag; 
	public byte AccurateStatisticsFlag { get { return this.accurateStatisticsFlag; } set { this.accurateStatisticsFlag = value; } }

	protected ushort avgBitRate; 
	public ushort AvgBitRate { get { return this.avgBitRate; } set { this.avgBitRate = value; } }

	protected ushort avgFrameRate; 
	public ushort AvgFrameRate { get { return this.avgFrameRate; } set { this.avgFrameRate = value; } }

	protected byte numReferences; 
	public byte NumReferences { get { return this.numReferences; } set { this.numReferences = value; } }

	protected DependencyInfo[] dependency; 
	public DependencyInfo[] Dependency { get { return this.dependency; } set { this.dependency = value; } }

	public AVCSubSequenceEntry(): base(IsoStream.FromFourCC("avss"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subSequenceIdentifer, "subSequenceIdentifer"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layerNumber, "layerNumber"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.durationFlag, "durationFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.avgRateFlag, "avgRateFlag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 

		if (durationFlag)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.duration, "duration"); 
		}

		if (avgRateFlag)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.accurateStatisticsFlag, "accurateStatisticsFlag"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgBitRate, "avgBitRate"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.avgFrameRate, "avgFrameRate"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numReferences, "numReferences"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(numReferences), () => new DependencyInfo(),  out this.dependency, "dependency"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.subSequenceIdentifer, "subSequenceIdentifer"); 
		boxSize += stream.WriteUInt8( this.layerNumber, "layerNumber"); 
		boxSize += stream.WriteBit( this.durationFlag, "durationFlag"); 
		boxSize += stream.WriteBit( this.avgRateFlag, "avgRateFlag"); 
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 

		if (durationFlag)
		{
			boxSize += stream.WriteUInt32( this.duration, "duration"); 
		}

		if (avgRateFlag)
		{
			boxSize += stream.WriteUInt8( this.accurateStatisticsFlag, "accurateStatisticsFlag"); 
			boxSize += stream.WriteUInt16( this.avgBitRate, "avgBitRate"); 
			boxSize += stream.WriteUInt16( this.avgFrameRate, "avgFrameRate"); 
		}
		boxSize += stream.WriteUInt8( this.numReferences, "numReferences"); 
		boxSize += stream.WriteClass( this.dependency, "dependency"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // subSequenceIdentifer
		boxSize += 8; // layerNumber
		boxSize += 1; // durationFlag
		boxSize += 1; // avgRateFlag
		boxSize += 6; // reserved

		if (durationFlag)
		{
			boxSize += 32; // duration
		}

		if (avgRateFlag)
		{
			boxSize += 8; // accurateStatisticsFlag
			boxSize += 16; // avgBitRate
			boxSize += 16; // avgFrameRate
		}
		boxSize += 8; // numReferences
		boxSize += IsoStream.CalculateClassSize(dependency); // dependency
		return boxSize;
	}
}

}
