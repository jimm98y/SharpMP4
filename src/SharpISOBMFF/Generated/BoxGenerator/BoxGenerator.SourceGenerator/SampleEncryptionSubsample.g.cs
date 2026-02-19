using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleEncryptionSubsample(version) {
  if (version==0) {
     unsigned int(16) BytesOfClearData;
     unsigned int(32) BytesOfProtectedData;
  }
  else if(version == 1) {
     unsigned int(16) multi_subindex;
     unsigned int(16) BytesOfClearData;
     unsigned int(32) BytesOfProtectedData;
  }
}
*/
public partial class SampleEncryptionSubsample : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SampleEncryptionSubsample"; } }

	protected ushort BytesOfClearData; 
	public ushort _BytesOfClearData { get { return this.BytesOfClearData; } set { this.BytesOfClearData = value; } }

	protected uint BytesOfProtectedData; 
	public uint _BytesOfProtectedData { get { return this.BytesOfProtectedData; } set { this.BytesOfProtectedData = value; } }

	protected ushort multi_subindex; 
	public ushort MultiSubindex { get { return this.multi_subindex; } set { this.multi_subindex = value; } }

	protected byte version; 
	public byte Version { get { return this.version; } set { this.version = value; } }

	public SampleEncryptionSubsample(byte version): base()
	{
		this.version = version;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;

		if (version==0)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.BytesOfClearData, "BytesOfClearData"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.BytesOfProtectedData, "BytesOfProtectedData"); 
		}

		else if (version == 1)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.multi_subindex, "multi_subindex"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.BytesOfClearData, "BytesOfClearData"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.BytesOfProtectedData, "BytesOfProtectedData"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;

		if (version==0)
		{
			boxSize += stream.WriteUInt16( this.BytesOfClearData, "BytesOfClearData"); 
			boxSize += stream.WriteUInt32( this.BytesOfProtectedData, "BytesOfProtectedData"); 
		}

		else if (version == 1)
		{
			boxSize += stream.WriteUInt16( this.multi_subindex, "multi_subindex"); 
			boxSize += stream.WriteUInt16( this.BytesOfClearData, "BytesOfClearData"); 
			boxSize += stream.WriteUInt32( this.BytesOfProtectedData, "BytesOfProtectedData"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;

		if (version==0)
		{
			boxSize += 16; // BytesOfClearData
			boxSize += 32; // BytesOfProtectedData
		}

		else if (version == 1)
		{
			boxSize += 16; // multi_subindex
			boxSize += 16; // BytesOfClearData
			boxSize += 32; // BytesOfProtectedData
		}
		return boxSize;
	}
}

}
