using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class HILNconfig()
{
  uimsbf(1) HILNquantMode;
  uimsbf(8) HILNmaxNumLine;
  uimsbf(4) HILNsampleRateCode;
  uimsbf(12) HILNframeLength;
  uimsbf(2) HILNcontMode;
}


*/
public partial class HILNconfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "HILNconfig"; } }

	protected bool HILNquantMode; 
	public bool _HILNquantMode { get { return this.HILNquantMode; } set { this.HILNquantMode = value; } }

	protected byte HILNmaxNumLine; 
	public byte _HILNmaxNumLine { get { return this.HILNmaxNumLine; } set { this.HILNmaxNumLine = value; } }

	protected byte HILNsampleRateCode; 
	public byte _HILNsampleRateCode { get { return this.HILNsampleRateCode; } set { this.HILNsampleRateCode = value; } }

	protected ushort HILNframeLength; 
	public ushort _HILNframeLength { get { return this.HILNframeLength; } set { this.HILNframeLength = value; } }

	protected byte HILNcontMode; 
	public byte _HILNcontMode { get { return this.HILNcontMode; } set { this.HILNcontMode = value; } }

	public HILNconfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.HILNquantMode, "HILNquantMode"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.HILNmaxNumLine, "HILNmaxNumLine"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.HILNsampleRateCode, "HILNsampleRateCode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.HILNframeLength, "HILNframeLength"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.HILNcontMode, "HILNcontMode"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.HILNquantMode, "HILNquantMode"); 
		boxSize += stream.WriteUInt8( this.HILNmaxNumLine, "HILNmaxNumLine"); 
		boxSize += stream.WriteBits(4,  this.HILNsampleRateCode, "HILNsampleRateCode"); 
		boxSize += stream.WriteBits(12,  this.HILNframeLength, "HILNframeLength"); 
		boxSize += stream.WriteBits(2,  this.HILNcontMode, "HILNcontMode"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // HILNquantMode
		boxSize += 8; // HILNmaxNumLine
		boxSize += 4; // HILNsampleRateCode
		boxSize += 12; // HILNframeLength
		boxSize += 2; // HILNcontMode
		return boxSize;
	}
}

}
