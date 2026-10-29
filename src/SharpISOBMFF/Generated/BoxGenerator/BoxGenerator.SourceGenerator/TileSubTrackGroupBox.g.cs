using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TileSubTrackGroupBox extends FullBox('tstb', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(16) tileGroupID;
	}
}
*/
public partial class TileSubTrackGroupBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TileSubTrackGroupBox extends FullBox('tstb', 0, 0) {
	unsigned int(16) item_count;
	for(i = 0; i< item_count; i++) {
		unsigned int(16) tileGroupID;
	}
}"; } }
	public const string TYPE = "tstb";
	public override string DisplayName { get { return "TileSubTrackGroupBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("strd"); }


	protected ushort item_count; 
	public ushort ItemCount { get { return this.item_count; } set { this.item_count = value; } }

	protected ushort[] tileGroupID; 
	public ushort[] TileGroupID { get { return this.tileGroupID; } set { this.tileGroupID = value; } }

	public TileSubTrackGroupBox(): base(IsoStream.FromFourCC("tstb"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.item_count, "item_count"); 

		this.tileGroupID = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( item_count), "tileGroupID");
		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tileGroupID[i], "tileGroupID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.item_count, "item_count"); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += stream.WriteUInt16( this.tileGroupID[i], "tileGroupID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // item_count

		for (int i = 0; i< item_count; i++)
		{
			boxSize += 16; // tileGroupID
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("item_count", 16, this.item_count); 

		for (int i = 0; i< item_count; i++)
		{
			boxSize += tracer.Field($"tileGroupID[{i}]", 16, this.tileGroupID[i]); 
		}
		return boxSize;
	}
}

}
