using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-17 component pattern, as of a Bayer filter: of each cell of the pattern, its component and gain
aligned(8) class ComponentPatternDefinitionBox() extends FullBox('cpat') {
	 unsigned int(16) pattern_width;
 unsigned int(16) pattern_height;
 for (i = 0; i < pattern_width * pattern_height; i++) {
 unsigned int(32) component_index;
 float(32) component_gain;
 }
 } 
*/
public partial class ComponentPatternDefinitionBox : FullBox
{
	public const string TYPE = "cpat";
	public override string DisplayName { get { return "ComponentPatternDefinitionBox"; } }

	protected ushort pattern_width; 
	public ushort PatternWidth { get { return this.pattern_width; } set { this.pattern_width = value; } }

	protected ushort pattern_height; 
	public ushort PatternHeight { get { return this.pattern_height; } set { this.pattern_height = value; } }

	protected uint[] component_index; 
	public uint[] ComponentIndex { get { return this.component_index; } set { this.component_index = value; } }

	protected double[] component_gain; 
	public double[] ComponentGain { get { return this.component_gain; } set { this.component_gain = value; } }

	public ComponentPatternDefinitionBox(): base(IsoStream.FromFourCC("cpat"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pattern_width, "pattern_width"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pattern_height, "pattern_height"); 

		this.component_index = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( pattern_width * pattern_height), "component_index");
		this.component_gain = stream.SafeAllocate<double>(boxSize, readSize, IsoStream.GetInt( pattern_width * pattern_height), "component_gain");
		for (int i = 0; i < pattern_width * pattern_height; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.component_index[i], "component_index"); 
			boxSize += stream.ReadFloat32(boxSize, readSize,  out this.component_gain[i], "component_gain"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.pattern_width, "pattern_width"); 
		boxSize += stream.WriteUInt16( this.pattern_height, "pattern_height"); 

		for (int i = 0; i < pattern_width * pattern_height; i++)
		{
			boxSize += stream.WriteUInt32( this.component_index[i], "component_index"); 
			boxSize += stream.WriteFloat32( this.component_gain[i], "component_gain"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // pattern_width
		boxSize += 16; // pattern_height

		for (int i = 0; i < pattern_width * pattern_height; i++)
		{
			boxSize += 32; // component_index
			boxSize += 32; // component_gain
		}
		return boxSize;
	}
}

}
