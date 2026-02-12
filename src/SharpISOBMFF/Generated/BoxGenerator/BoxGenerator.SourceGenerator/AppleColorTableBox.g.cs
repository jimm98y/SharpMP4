using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AppleColorTableBox() extends Box ('ctab'){
 signed int(32) color_table_seed;
 signed int(16) flags;
 signed int(16) color_table_size;
 AppleColor colors[];
 }
 
*/
public partial class AppleColorTableBox : Box
{
	public const string TYPE = "ctab";
	public override string DisplayName { get { return "AppleColorTableBox"; } }

	protected int color_table_seed; 
	public int ColorTableSeed { get { return this.color_table_seed; } set { this.color_table_seed = value; } }

	protected short flags; 
	public short Flags { get { return this.flags; } set { this.flags = value; } }

	protected short color_table_size; 
	public short ColorTableSize { get { return this.color_table_size; } set { this.color_table_size = value; } }

	protected AppleColor[] colors; 
	public AppleColor[] Colors { get { return this.colors; } set { this.colors = value; } }

	public AppleColorTableBox(): base(IsoStream.FromFourCC("ctab"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.color_table_seed, "color_table_seed"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.color_table_size, "color_table_size"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new AppleColor(),  out this.colors, "colors"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.color_table_seed, "color_table_seed"); 
		boxSize += stream.WriteInt16( this.flags, "flags"); 
		boxSize += stream.WriteInt16( this.color_table_size, "color_table_size"); 
		boxSize += stream.WriteClass( this.colors, "colors"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // color_table_seed
		boxSize += 16; // flags
		boxSize += 16; // color_table_size
		boxSize += IsoStream.CalculateClassSize(colors); // colors
		return boxSize;
	}
}

}
