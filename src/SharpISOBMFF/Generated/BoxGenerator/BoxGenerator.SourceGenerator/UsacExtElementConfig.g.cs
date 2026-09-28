using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacExtElementConfig()
{
  EscapedValue(4, 8, 16) usacExtElementType;
  EscapedValue(4, 8, 16) usacExtElementConfigLength;
  bslbf(1) usacExtElementDefaultLengthPresent;
  if (usacExtElementDefaultLengthPresent) {
    EscapedValue(8, 16, 0) usacExtElementDefaultLength;
  }
  bslbf(1) usacExtElementPayloadFrag;
  bslbf(8) usacExtElementConfigData[usacExtElementConfigLength];
}


*/
public partial class UsacExtElementConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacExtElementConfig"; } }

	protected EscapedValue usacExtElementType; 
	public EscapedValue UsacExtElementType { get { return this.usacExtElementType; } set { this.usacExtElementType = value; } }

	protected EscapedValue usacExtElementConfigLength; 
	public EscapedValue UsacExtElementConfigLength { get { return this.usacExtElementConfigLength; } set { this.usacExtElementConfigLength = value; } }

	protected bool usacExtElementDefaultLengthPresent; 
	public bool UsacExtElementDefaultLengthPresent { get { return this.usacExtElementDefaultLengthPresent; } set { this.usacExtElementDefaultLengthPresent = value; } }

	protected EscapedValue usacExtElementDefaultLength; 
	public EscapedValue UsacExtElementDefaultLength { get { return this.usacExtElementDefaultLength; } set { this.usacExtElementDefaultLength = value; } }

	protected bool usacExtElementPayloadFrag; 
	public bool UsacExtElementPayloadFrag { get { return this.usacExtElementPayloadFrag; } set { this.usacExtElementPayloadFrag = value; } }

	protected byte[] usacExtElementConfigData; 
	public byte[] UsacExtElementConfigData { get { return this.usacExtElementConfigData; } set { this.usacExtElementConfigData = value; } }

	public UsacExtElementConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(4, 8, 16),  out this.usacExtElementType, "usacExtElementType"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(4, 8, 16),  out this.usacExtElementConfigLength, "usacExtElementConfigLength"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.usacExtElementDefaultLengthPresent, "usacExtElementDefaultLengthPresent"); 

		if (usacExtElementDefaultLengthPresent)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(8, 16, 0),  out this.usacExtElementDefaultLength, "usacExtElementDefaultLength"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.usacExtElementPayloadFrag, "usacExtElementPayloadFrag"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(usacExtElementConfigLength),  out this.usacExtElementConfigData, "usacExtElementConfigData"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteClass( this.usacExtElementType, "usacExtElementType"); 
		boxSize += stream.WriteClass( this.usacExtElementConfigLength, "usacExtElementConfigLength"); 
		boxSize += stream.WriteBit( this.usacExtElementDefaultLengthPresent, "usacExtElementDefaultLengthPresent"); 

		if (usacExtElementDefaultLengthPresent)
		{
			boxSize += stream.WriteClass( this.usacExtElementDefaultLength, "usacExtElementDefaultLength"); 
		}
		boxSize += stream.WriteBit( this.usacExtElementPayloadFrag, "usacExtElementPayloadFrag"); 
		boxSize += stream.WriteUInt8Array((uint)(usacExtElementConfigLength),  this.usacExtElementConfigData, "usacExtElementConfigData"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += IsoStream.CalculateClassSize(usacExtElementType); // usacExtElementType
		boxSize += IsoStream.CalculateClassSize(usacExtElementConfigLength); // usacExtElementConfigLength
		boxSize += 1; // usacExtElementDefaultLengthPresent

		if (usacExtElementDefaultLengthPresent)
		{
			boxSize += IsoStream.CalculateClassSize(usacExtElementDefaultLength); // usacExtElementDefaultLength
		}
		boxSize += 1; // usacExtElementPayloadFrag
		boxSize += ((ulong)(usacExtElementConfigLength) * 8); // usacExtElementConfigData
		return boxSize;
	}
}

}
