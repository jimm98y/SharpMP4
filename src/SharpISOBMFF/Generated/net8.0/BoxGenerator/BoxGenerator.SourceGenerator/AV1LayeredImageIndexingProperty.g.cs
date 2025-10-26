using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AV1LayeredImageIndexingProperty extends ItemProperty('a1lx') {
    unsigned int(7) reserved = 0;
    unsigned int(1) large_size;
    if(large_size) {
       unsigned int(32) layer_size[3];
    }
    else {
       unsigned int(16) layer_size[3];
    }
}
*/
public partial class AV1LayeredImageIndexingProperty : ItemProperty
{
	public const string TYPE = "a1lx";
	public override string DisplayName { get { return "AV1LayeredImageIndexingProperty"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool large_size; 
	public bool LargeSize { get { return this.large_size; } set { this.large_size = value; } }

	protected uint[] layer_size; 
	public uint[] LayerSize { get { return this.layer_size; } set { this.layer_size = value; } }

	public AV1LayeredImageIndexingProperty(): base(IsoStream.FromFourCC("a1lx"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.large_size, "large_size"); 

		if (large_size)
		{
			boxSize += stream.ReadUInt32Array(boxSize, readSize, 3,  out this.layer_size, "layer_size"); 
		}

		else 
		{
			boxSize += stream.ReadUInt16Array(boxSize, readSize, 3,  out this.layer_size, "layer_size"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.large_size, "large_size"); 

		if (large_size)
		{
			boxSize += stream.WriteUInt32Array(3,  this.layer_size, "layer_size"); 
		}

		else 
		{
			boxSize += stream.WriteUInt16Array(3,  this.layer_size, "layer_size"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 7; // reserved
		boxSize += 1; // large_size

		if (large_size)
		{
			boxSize += 3 * 32; // layer_size
		}

		else 
		{
			boxSize += 3 * 16; // layer_size
		}
		return boxSize;
	}
}

}
