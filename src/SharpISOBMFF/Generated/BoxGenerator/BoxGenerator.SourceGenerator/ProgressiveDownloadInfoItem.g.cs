using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProgressiveDownloadInfoItem {
 		unsigned int(32)  rate;
		unsigned int(32)  initial_delay;
 }

*/
public partial class ProgressiveDownloadInfoItem : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ProgressiveDownloadInfoItem"; } }

	protected uint rate; 
	public uint Rate { get { return this.rate; } set { this.rate = value; } }

	protected uint initial_delay; 
	public uint InitialDelay { get { return this.initial_delay; } set { this.initial_delay = value; } }

	public ProgressiveDownloadInfoItem(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.rate, "rate"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.initial_delay, "initial_delay"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.rate, "rate"); 
		boxSize += stream.WriteUInt32( this.initial_delay, "initial_delay"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // rate
		boxSize += 32; // initial_delay
		return boxSize;
	}
}

}
