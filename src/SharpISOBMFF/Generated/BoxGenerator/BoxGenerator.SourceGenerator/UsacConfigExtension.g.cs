using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class UsacConfigExtension()
{
  EscapedValue(2, 4, 8) numConfigExtensionsMinusOne;
  for (i = 0; i < numConfigExtensionsMinusOne + 1; i++) {
    EscapedValue(4, 8, 16) usacConfigExtType;
    EscapedValue(4, 8, 16) usacConfigExtLength;
    UsacConfigExtensionData(usacConfigExtLength) usacConfigExtensionData;
  }
}

// the bytes of a config extension - loudness info, a stream id, fill - usacConfigExtLength of them

*/
public partial class UsacConfigExtension : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "UsacConfigExtension"; } }

	protected EscapedValue numConfigExtensionsMinusOne; 
	public EscapedValue NumConfigExtensionsMinusOne { get { return this.numConfigExtensionsMinusOne; } set { this.numConfigExtensionsMinusOne = value; } }

	protected EscapedValue[] usacConfigExtType; 
	public EscapedValue[] UsacConfigExtType { get { return this.usacConfigExtType; } set { this.usacConfigExtType = value; } }

	protected EscapedValue[] usacConfigExtLength; 
	public EscapedValue[] UsacConfigExtLength { get { return this.usacConfigExtLength; } set { this.usacConfigExtLength = value; } }

	protected UsacConfigExtensionData[] usacConfigExtensionData; 
	public UsacConfigExtensionData[] UsacConfigExtensionData { get { return this.usacConfigExtensionData; } set { this.usacConfigExtensionData = value; } }

	public UsacConfigExtension(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(2, 4, 8),  out this.numConfigExtensionsMinusOne, "numConfigExtensionsMinusOne"); 

		this.usacConfigExtType = stream.SafeAllocate<EscapedValue>(boxSize, readSize, IsoStream.GetInt( numConfigExtensionsMinusOne + 1), "usacConfigExtType");
		this.usacConfigExtLength = stream.SafeAllocate<EscapedValue>(boxSize, readSize, IsoStream.GetInt( numConfigExtensionsMinusOne + 1), "usacConfigExtLength");
		this.usacConfigExtensionData = stream.SafeAllocate<UsacConfigExtensionData>(boxSize, readSize, IsoStream.GetInt( numConfigExtensionsMinusOne + 1), "usacConfigExtensionData");
		for (int i = 0; i < numConfigExtensionsMinusOne + 1; i++)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(4, 8, 16),  out this.usacConfigExtType[i], "usacConfigExtType"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(4, 8, 16),  out this.usacConfigExtLength[i], "usacConfigExtLength"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new UsacConfigExtensionData(usacConfigExtLength[i]),  out this.usacConfigExtensionData[i], "usacConfigExtensionData"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteClass( this.numConfigExtensionsMinusOne, "numConfigExtensionsMinusOne"); 

		for (int i = 0; i < numConfigExtensionsMinusOne + 1; i++)
		{
			boxSize += stream.WriteClass( this.usacConfigExtType[i], "usacConfigExtType"); 
			boxSize += stream.WriteClass( this.usacConfigExtLength[i], "usacConfigExtLength"); 
			boxSize += stream.WriteClass( this.usacConfigExtensionData[i], "usacConfigExtensionData"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += IsoStream.CalculateClassSize(numConfigExtensionsMinusOne); // numConfigExtensionsMinusOne

		for (int i = 0; i < numConfigExtensionsMinusOne + 1; i++)
		{
			boxSize += IsoStream.CalculateClassSize(usacConfigExtType[i]); // usacConfigExtType
			boxSize += IsoStream.CalculateClassSize(usacConfigExtLength[i]); // usacConfigExtLength
			boxSize += IsoStream.CalculateClassSize(usacConfigExtensionData[i]); // usacConfigExtensionData
		}
		return boxSize;
	}
}

}
