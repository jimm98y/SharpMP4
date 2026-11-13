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
aligned(8) class ICCCicpType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(8) colour_primaries;
	unsigned int(8) transfer_characteristics;
	unsigned int(8) matrix_coefficients;
	unsigned int(8) video_full_range_flag;
}
*/
public partial class ICCCicpType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCCicpType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(8) colour_primaries;
	unsigned int(8) transfer_characteristics;
	unsigned int(8) matrix_coefficients;
	unsigned int(8) video_full_range_flag;
}"; } }
	public virtual string DisplayName { get { return "ICCCicpType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte colour_primaries; 
	public byte ColourPrimaries { get { return this.colour_primaries; } set { this.colour_primaries = value; } }

	protected byte transfer_characteristics; 
	public byte TransferCharacteristics { get { return this.transfer_characteristics; } set { this.transfer_characteristics = value; } }

	protected byte matrix_coefficients; 
	public byte MatrixCoefficients { get { return this.matrix_coefficients; } set { this.matrix_coefficients = value; } }

	protected byte video_full_range_flag; 
	public byte VideoFullRangeFlag { get { return this.video_full_range_flag; } set { this.video_full_range_flag = value; } }

	public ICCCicpType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.colour_primaries, "colour_primaries"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.transfer_characteristics, "transfer_characteristics"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.matrix_coefficients, "matrix_coefficients"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.video_full_range_flag, "video_full_range_flag"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt8( this.colour_primaries, "colour_primaries"); 
		boxSize += stream.WriteUInt8( this.transfer_characteristics, "transfer_characteristics"); 
		boxSize += stream.WriteUInt8( this.matrix_coefficients, "matrix_coefficients"); 
		boxSize += stream.WriteUInt8( this.video_full_range_flag, "video_full_range_flag"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 8; // colour_primaries
		boxSize += 8; // transfer_characteristics
		boxSize += 8; // matrix_coefficients
		boxSize += 8; // video_full_range_flag
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("colour_primaries", 8, this.colour_primaries); 
		boxSize += tracer.Field("transfer_characteristics", 8, this.transfer_characteristics); 
		boxSize += tracer.Field("matrix_coefficients", 8, this.matrix_coefficients); 
		boxSize += tracer.Field("video_full_range_flag", 8, this.video_full_range_flag); 
		return boxSize;
	}
}

}
