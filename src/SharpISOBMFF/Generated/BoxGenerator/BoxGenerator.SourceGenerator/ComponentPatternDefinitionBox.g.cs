// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// ISO/IEC 23001-17 component pattern, as of a Bayer filter: of each cell of the pattern, its component and gain
aligned(8) class ComponentPatternDefinitionBox() extends FullBox('cpat') {
	unsigned int(16) pattern_width;
	unsigned int(16) pattern_height;
	for (i = 0; i < pattern_width * pattern_height; i++) {
		unsigned int(32) component_index;
		float(32) component_gain;
	}
}"; } }
	public const string TYPE = "cpat";
	public override string DisplayName { get { return "ComponentPatternDefinitionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry(uncv)", "ipco"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("pattern_width", 16, this.pattern_width); 
		boxSize += tracer.Field("pattern_height", 16, this.pattern_height); 

		for (int i = 0; i < pattern_width * pattern_height; i++)
		{
			boxSize += tracer.Field($"component_index[{i}]", 32, this.component_index[i]); 
			boxSize += tracer.Field($"component_gain[{i}]", 32, this.component_gain[i]); 
		}
		return boxSize;
	}
}

}
