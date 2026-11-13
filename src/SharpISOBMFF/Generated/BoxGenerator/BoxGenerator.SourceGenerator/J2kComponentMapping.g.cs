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
aligned(8) class J2kComponentMapping() {
	unsigned int(16) component;
	unsigned int(8) mapping_type;
	unsigned int(8) palette_column;
}
*/
public partial class J2kComponentMapping : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class J2kComponentMapping() {
	unsigned int(16) component;
	unsigned int(8) mapping_type;
	unsigned int(8) palette_column;
}"; } }
	public virtual string DisplayName { get { return "J2kComponentMapping"; } }

	protected ushort component; 
	public ushort Component { get { return this.component; } set { this.component = value; } }

	protected byte mapping_type; 
	public byte MappingType { get { return this.mapping_type; } set { this.mapping_type = value; } }

	protected byte palette_column; 
	public byte PaletteColumn { get { return this.palette_column; } set { this.palette_column = value; } }

	public J2kComponentMapping(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.component, "component"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.mapping_type, "mapping_type"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.palette_column, "palette_column"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.component, "component"); 
		boxSize += stream.WriteUInt8( this.mapping_type, "mapping_type"); 
		boxSize += stream.WriteUInt8( this.palette_column, "palette_column"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // component
		boxSize += 8; // mapping_type
		boxSize += 8; // palette_column
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("component", 16, this.component); 
		boxSize += tracer.Field("mapping_type", 8, this.mapping_type); 
		boxSize += tracer.Field("palette_column", 8, this.palette_column); 
		return boxSize;
	}
}

}
