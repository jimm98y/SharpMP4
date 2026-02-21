using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class GetAudioObjectType()
{
  uimsbf(5) audioObjectType;
  if (audioObjectType == 31) {
    uimsbf(6) audioObjectTypeExt;
    audioObjectType = 32 + audioObjectTypeExt;
  }
  return audioObjectType;
}


*/
public partial class GetAudioObjectType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "GetAudioObjectType"; } }

	protected byte audioObjectType; 
	public byte AudioObjectType { get { return this.audioObjectType; } set { this.audioObjectType = value; } }

	protected byte audioObjectTypeExt; 
	public byte AudioObjectTypeExt { get { return this.audioObjectTypeExt; } set { this.audioObjectTypeExt = value; } }

	public GetAudioObjectType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.audioObjectType, "audioObjectType"); 

		if (audioObjectType == 31)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.audioObjectTypeExt, "audioObjectTypeExt"); 
			audioObjectType = (byte)(32 + audioObjectTypeExt);
		}
		// return audioObjectType;
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(5,  this.audioObjectType, "audioObjectType"); 

		if (audioObjectType == 31)
		{
			boxSize += stream.WriteBits(6,  this.audioObjectTypeExt, "audioObjectTypeExt"); 
			audioObjectType = (byte)(32 + audioObjectTypeExt);
		}
		// return audioObjectType;
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 5; // audioObjectType

		if (audioObjectType == 31)
		{
			boxSize += 6; // audioObjectTypeExt
			audioObjectType = (byte)(32 + audioObjectTypeExt);
		}
		// return audioObjectType;
		return boxSize;
	}
}

}
