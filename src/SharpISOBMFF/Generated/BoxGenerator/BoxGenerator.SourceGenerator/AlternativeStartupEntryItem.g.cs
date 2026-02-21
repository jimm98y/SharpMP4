using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AlternativeStartupEntryItem() {
		unsigned int(16) num_output_samples;
		unsigned int(16) num_total_samples;
}

*/
public partial class AlternativeStartupEntryItem : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AlternativeStartupEntryItem"; } }

	protected ushort num_output_samples; 
	public ushort NumOutputSamples { get { return this.num_output_samples; } set { this.num_output_samples = value; } }

	protected ushort num_total_samples; 
	public ushort NumTotalSamples { get { return this.num_total_samples; } set { this.num_total_samples = value; } }

	public AlternativeStartupEntryItem(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_output_samples, "num_output_samples"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_total_samples, "num_total_samples"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.num_output_samples, "num_output_samples"); 
		boxSize += stream.WriteUInt16( this.num_total_samples, "num_total_samples"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // num_output_samples
		boxSize += 16; // num_total_samples
		return boxSize;
	}
}

}
