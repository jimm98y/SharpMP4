using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// E.5, Pseudocode E.2: an ac4_dsi_v1() (E.6.1), to the end of the box
aligned(8) class AC4SpecificBox extends Box('dac4') {
 bit(8) ac4_dsi_v1[];
}
*/
public partial class AC4SpecificBox : Box
{
	public const string TYPE = "dac4";
	public override string DisplayName { get { return "AC4SpecificBox"; } }

	protected byte[] ac4_dsi_v1; 
	public byte[] Ac4DsiV1 { get { return this.ac4_dsi_v1; } set { this.ac4_dsi_v1 = value; } }

	public AC4SpecificBox(): base(IsoStream.FromFourCC("dac4"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.ac4_dsi_v1, "ac4_dsi_v1"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.ac4_dsi_v1, "ac4_dsi_v1"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)ac4_dsi_v1.Length * 8); // ac4_dsi_v1
		return boxSize;
	}
}

}
