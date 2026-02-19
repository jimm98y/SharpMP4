using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ShadowSyncSampleBox
	extends FullBox('stsh', version = 0, 0) {
	unsigned int(32)	entry_count;
	int i;
	for (i=0; i < entry_count; i++) {
		unsigned int(32)	shadowed_sample_number;
		unsigned int(32)	sync_sample_number;
	}
}
*/
public partial class ShadowSyncSampleBox : FullBox
{
	public const string TYPE = "stsh";
	public override string DisplayName { get { return "ShadowSyncSampleBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] shadowed_sample_number; 
	public uint[] ShadowedSampleNumber { get { return this.shadowed_sample_number; } set { this.shadowed_sample_number = value; } }

	protected uint[] sync_sample_number; 
	public uint[] SyncSampleNumber { get { return this.sync_sample_number; } set { this.sync_sample_number = value; } }

	public ShadowSyncSampleBox(): base(IsoStream.FromFourCC("stsh"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		this.shadowed_sample_number = new uint[IsoStream.GetInt( entry_count)];
		this.sync_sample_number = new uint[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.shadowed_sample_number[i], "shadowed_sample_number"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sync_sample_number[i], "sync_sample_number"); 
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
			boxSize += stream.WriteUInt32( this.shadowed_sample_number[i], "shadowed_sample_number"); 
			boxSize += stream.WriteUInt32( this.sync_sample_number[i], "sync_sample_number"); 
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
			boxSize += 32; // shadowed_sample_number
			boxSize += 32; // sync_sample_number
		}
		return boxSize;
	}
}

}
