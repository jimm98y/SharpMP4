using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MovieFragmentHeaderBox
			extends FullBox('mfhd', 0, 0){
	unsigned int(32)	sequence_number;
}
*/
public partial class MovieFragmentHeaderBox : FullBox
{
	public const string TYPE = "mfhd";
	public override string DisplayName { get { return "MovieFragmentHeaderBox"; } }

	protected uint sequence_number; 
	public uint SequenceNumber { get { return this.sequence_number; } set { this.sequence_number = value; } }

	public MovieFragmentHeaderBox(): base(IsoStream.FromFourCC("mfhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sequence_number, "sequence_number"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sequence_number, "sequence_number"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sequence_number
		return boxSize;
	}
}

}
