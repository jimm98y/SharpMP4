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
class AppleCompressedMoovDataBox() extends Box ('cmvd'){
	unsigned int(32) uncompressedSize;
	bit(8) compressedData[];
}
*/
public partial class AppleCompressedMoovDataBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleCompressedMoovDataBox() extends Box ('cmvd'){
	unsigned int(32) uncompressedSize;
	bit(8) compressedData[];
}"; } }
	public const string TYPE = "cmvd";
	public override string DisplayName { get { return "AppleCompressedMoovDataBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("cmov"); }


	protected uint uncompressedSize; 
	public uint UncompressedSize { get { return this.uncompressedSize; } set { this.uncompressedSize = value; } }

	protected byte[] compressedData = []; 
	public byte[] CompressedData { get { return this.compressedData; } set { this.compressedData = value; } }

	public AppleCompressedMoovDataBox(): base(IsoStream.FromFourCC("cmvd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.uncompressedSize, "uncompressedSize"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.compressedData, "compressedData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.uncompressedSize, "uncompressedSize"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.compressedData, "compressedData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // uncompressedSize
		boxSize += ((ulong)compressedData.Length * 8); // compressedData
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("uncompressedSize", 32, this.uncompressedSize); 
		boxSize += tracer.Field("compressedData", ((ulong)compressedData.Length * 8), this.compressedData); 
		return boxSize;
	}
}

}
