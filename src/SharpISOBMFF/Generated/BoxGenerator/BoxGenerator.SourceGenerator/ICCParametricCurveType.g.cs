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
aligned(8) class ICCParametricCurveType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(16) function_type;
	unsigned int(16) reserved = 0;
	signed int(32) parameters[];	// s15Fixed16Number: g; g a b; g a b c; g a b c d; g a b c d e f
}
*/
public partial class ICCParametricCurveType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCParametricCurveType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(16) function_type;
	unsigned int(16) reserved = 0;
	signed int(32) parameters[];	// s15Fixed16Number: g; g a b; g a b c; g a b c d; g a b c d e f
}"; } }
	public virtual string DisplayName { get { return "ICCParametricCurveType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort function_type; 
	public ushort FunctionType { get { return this.function_type; } set { this.function_type = value; } }

	protected ushort reserved0 = 0; 
	public ushort Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected int[] parameters = [];  //  s15Fixed16Number: g; g a b; g a b c; g a b c d; g a b c d e f
	public int[] Parameters { get { return this.parameters; } set { this.parameters = value; } }

	public ICCParametricCurveType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.function_type, "function_type"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadInt32ArrayTillEnd(boxSize, readSize,  out this.parameters, "parameters"); // s15Fixed16Number: g; g a b; g a b c; g a b c d; g a b c d e f
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.function_type, "function_type"); 
		boxSize += stream.WriteUInt16( this.reserved0, "reserved0"); 
		boxSize += stream.WriteInt32ArrayTillEnd( this.parameters, "parameters"); // s15Fixed16Number: g; g a b; g a b c; g a b c d; g a b c d e f
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 16; // function_type
		boxSize += 16; // reserved0
		boxSize += ((ulong)parameters.Length * 32); // parameters
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("function_type", 16, this.function_type); 
		boxSize += tracer.Field("reserved0", 16, this.reserved0); 
		boxSize += tracer.Field("parameters", ((ulong)parameters.Length * 32), this.parameters); // s15Fixed16Number: g; g a b; g a b c; g a b c d; g a b c d e f
		return boxSize;
	}
}

}
