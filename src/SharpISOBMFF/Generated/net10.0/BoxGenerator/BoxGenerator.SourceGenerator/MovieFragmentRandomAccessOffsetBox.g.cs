using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MovieFragmentRandomAccessOffsetBox
 extends FullBox('mfro', version, 0) {
	unsigned int(32)	parent_size;
}
*/
public partial class MovieFragmentRandomAccessOffsetBox : FullBox
{
	public const string TYPE = "mfro";
	public override string DisplayName { get { return "MovieFragmentRandomAccessOffsetBox"; } }

	protected uint parent_size; 
	public uint ParentSize { get { return this.parent_size; } set { this.parent_size = value; } }

	public MovieFragmentRandomAccessOffsetBox(byte version = 0): base(IsoStream.FromFourCC("mfro"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.parent_size, "parent_size"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.parent_size, "parent_size"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // parent_size
		return boxSize;
	}
}

}
