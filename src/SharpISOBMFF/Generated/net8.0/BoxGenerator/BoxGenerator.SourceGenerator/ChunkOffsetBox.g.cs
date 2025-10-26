using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ChunkOffsetBox
	extends FullBox('stco', version = 0, 0) {
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++) {
		unsigned int(32)	chunk_offset;
	}
}
*/
public partial class ChunkOffsetBox : FullBox
{
	public const string TYPE = "stco";
	public override string DisplayName { get { return "ChunkOffsetBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] chunk_offset; 
	public uint[] ChunkOffset { get { return this.chunk_offset; } set { this.chunk_offset = value; } }

	public ChunkOffsetBox(): base(IsoStream.FromFourCC("stco"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.chunk_offset = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.chunk_offset[i], "chunk_offset"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt32( this.chunk_offset[i], "chunk_offset"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 32; // chunk_offset
		}
		return boxSize;
	}
}

}
