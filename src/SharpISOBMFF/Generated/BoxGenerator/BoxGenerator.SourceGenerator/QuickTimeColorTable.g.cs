using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class QuickTimeColorTable() {
	signed int(32) color_table_seed;
	signed int(16) flags;
	signed int(16) color_table_size;
	AppleColor colors[color_table_size + 1];
}
*/
public partial class QuickTimeColorTable : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class QuickTimeColorTable() {
	signed int(32) color_table_seed;
	signed int(16) flags;
	signed int(16) color_table_size;
	AppleColor colors[color_table_size + 1];
}"; } }
	public virtual string DisplayName { get { return "QuickTimeColorTable"; } }

	protected int color_table_seed; 
	public int ColorTableSeed { get { return this.color_table_seed; } set { this.color_table_seed = value; } }

	protected short flags; 
	public short Flags { get { return this.flags; } set { this.flags = value; } }

	protected short color_table_size; 
	public short ColorTableSize { get { return this.color_table_size; } set { this.color_table_size = value; } }

	protected AppleColor[] colors = []; 
	public AppleColor[] Colors { get { return this.colors; } set { this.colors = value; } }

	public QuickTimeColorTable(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.color_table_seed, "color_table_seed"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.color_table_size, "color_table_size"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(color_table_size + 1), () => new AppleColor(),  out this.colors, "colors"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteInt32( this.color_table_seed, "color_table_seed"); 
		boxSize += stream.WriteInt16( this.flags, "flags"); 
		boxSize += stream.WriteInt16( this.color_table_size, "color_table_size"); 
		boxSize += stream.WriteClass( this.colors, "colors"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // color_table_seed
		boxSize += 16; // flags
		boxSize += 16; // color_table_size
		boxSize += IsoStream.CalculateClassSize(colors); // colors
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("color_table_seed", 32, this.color_table_seed); 
		boxSize += tracer.Field("flags", 16, this.flags); 
		boxSize += tracer.Field("color_table_size", 16, this.color_table_size); 
		boxSize += tracer.Field("colors", IsoStream.CalculateClassSize(colors), this.colors); 
		return boxSize;
	}
}

}
