using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class VolumetricVisualSampleEntry(codingname) 
	extends SampleEntry (codingname){
	unsigned int(8)[32] compressorname;
	// other boxes from derived specifications
}
*/
public partial class VolumetricVisualSampleEntry : SampleEntry
{
	public override string DisplayName { get { return "VolumetricVisualSampleEntry"; } }

	protected byte[] compressorname;  //  other boxes from derived specifications
	public byte[] Compressorname { get { return this.compressorname; } set { this.compressorname = value; } }

	public VolumetricVisualSampleEntry(uint codingname = 0): base(codingname)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 32,  out this.compressorname, "compressorname"); // other boxes from derived specifications
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(32,  this.compressorname, "compressorname"); // other boxes from derived specifications
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32 * 8; // compressorname
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
