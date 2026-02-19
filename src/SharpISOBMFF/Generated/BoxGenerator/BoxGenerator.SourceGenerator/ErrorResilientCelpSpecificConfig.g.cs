using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class ErrorResilientCelpSpecificConfig(samplingFrequencyIndex)
{
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    ER_SC_CelpHeader(samplingFrequencyIndex);
  }
  else {
    uimsbf(1) isBWSLayer;
    if (isBWSLayer) {
      CelpBWSenhHeader();
    }
    else {
      uimsbf(2) CELPBRSid;
    }
  }
}


*/
public partial class ErrorResilientCelpSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ErrorResilientCelpSpecificConfig"; } }

	protected bool isBaseLayer; 
	public bool IsBaseLayer { get { return this.isBaseLayer; } set { this.isBaseLayer = value; } }

	protected ER_SC_CelpHeader ER_SC_CelpHeader; 
	public ER_SC_CelpHeader ERSCCelpHeader { get { return this.ER_SC_CelpHeader; } set { this.ER_SC_CelpHeader = value; } }

	protected bool isBWSLayer; 
	public bool IsBWSLayer { get { return this.isBWSLayer; } set { this.isBWSLayer = value; } }

	protected CelpBWSenhHeader CelpBWSenhHeader; 
	public CelpBWSenhHeader _CelpBWSenhHeader { get { return this.CelpBWSenhHeader; } set { this.CelpBWSenhHeader = value; } }

	protected byte CELPBRSid; 
	public byte _CELPBRSid { get { return this.CELPBRSid; } set { this.CELPBRSid = value; } }

	protected int samplingFrequencyIndex; 
	public int SamplingFrequencyIndex { get { return this.samplingFrequencyIndex; } set { this.samplingFrequencyIndex = value; } }

	public ErrorResilientCelpSpecificConfig(int samplingFrequencyIndex): base()
	{
		this.samplingFrequencyIndex = samplingFrequencyIndex;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ER_SC_CelpHeader(samplingFrequencyIndex),  out this.ER_SC_CelpHeader, "ER_SC_CelpHeader"); 
		}

		else 
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.isBWSLayer, "isBWSLayer"); 

			if (isBWSLayer)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new CelpBWSenhHeader(),  out this.CelpBWSenhHeader, "CelpBWSenhHeader"); 
			}

			else 
			{
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.CELPBRSid, "CELPBRSid"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.WriteClass( this.ER_SC_CelpHeader, "ER_SC_CelpHeader"); 
		}

		else 
		{
			boxSize += stream.WriteBit( this.isBWSLayer, "isBWSLayer"); 

			if (isBWSLayer)
			{
				boxSize += stream.WriteClass( this.CelpBWSenhHeader, "CelpBWSenhHeader"); 
			}

			else 
			{
				boxSize += stream.WriteBits(2,  this.CELPBRSid, "CELPBRSid"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // isBaseLayer

		if (isBaseLayer)
		{
			boxSize += IsoStream.CalculateClassSize(ER_SC_CelpHeader); // ER_SC_CelpHeader
		}

		else 
		{
			boxSize += 1; // isBWSLayer

			if (isBWSLayer)
			{
				boxSize += IsoStream.CalculateClassSize(CelpBWSenhHeader); // CelpBWSenhHeader
			}

			else 
			{
				boxSize += 2; // CELPBRSid
			}
		}
		return boxSize;
	}
}

}
