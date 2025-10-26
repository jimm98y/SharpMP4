using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class DependencyInfo  
{ 
unsigned int(8)   subSeqDirectionFlag; 
unsigned int(8)   layerNumber; 
unsigned int(16)  subSequenceIdentifier; 
} 

*/
public partial class DependencyInfo : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "DependencyInfo"; } }

	protected byte subSeqDirectionFlag; 
	public byte SubSeqDirectionFlag { get { return this.subSeqDirectionFlag; } set { this.subSeqDirectionFlag = value; } }

	protected byte layerNumber; 
	public byte LayerNumber { get { return this.layerNumber; } set { this.layerNumber = value; } }

	protected ushort subSequenceIdentifier; 
	public ushort SubSequenceIdentifier { get { return this.subSequenceIdentifier; } set { this.subSequenceIdentifier = value; } }

	public DependencyInfo(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.subSeqDirectionFlag, "subSeqDirectionFlag"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layerNumber, "layerNumber"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subSequenceIdentifier, "subSequenceIdentifier"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.subSeqDirectionFlag, "subSeqDirectionFlag"); 
		boxSize += stream.WriteUInt8( this.layerNumber, "layerNumber"); 
		boxSize += stream.WriteUInt16( this.subSequenceIdentifier, "subSequenceIdentifier"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // subSeqDirectionFlag
		boxSize += 8; // layerNumber
		boxSize += 16; // subSequenceIdentifier
		return boxSize;
	}
}

}
