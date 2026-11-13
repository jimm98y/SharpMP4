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
class AppleQTVRTrackBox() extends Box ('qtvr'){
	unsigned int(32) reserved1;
	unsigned int(16) reserved2;
	unsigned int(16) dataRefIndex;
	unsigned int(32) data;
}
*/
public partial class AppleQTVRTrackBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleQTVRTrackBox() extends Box ('qtvr'){
	unsigned int(32) reserved1;
	unsigned int(16) reserved2;
	unsigned int(16) dataRefIndex;
	unsigned int(32) data;
}"; } }
	public const string TYPE = "qtvr";
	public override string DisplayName { get { return "AppleQTVRTrackBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


	protected uint reserved1; 
	public uint Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort reserved2; 
	public ushort Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort dataRefIndex; 
	public ushort DataRefIndex { get { return this.dataRefIndex; } set { this.dataRefIndex = value; } }

	protected uint data; 
	public uint Data { get { return this.data; } set { this.data = value; } }

	public AppleQTVRTrackBox(): base(IsoStream.FromFourCC("qtvr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataRefIndex, "dataRefIndex"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt16( this.dataRefIndex, "dataRefIndex"); 
		boxSize += stream.WriteUInt32( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reserved1
		boxSize += 16; // reserved2
		boxSize += 16; // dataRefIndex
		boxSize += 32; // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved1", 32, this.reserved1); 
		boxSize += tracer.Field("reserved2", 16, this.reserved2); 
		boxSize += tracer.Field("dataRefIndex", 16, this.dataRefIndex); 
		boxSize += tracer.Field("data", 32, this.data); 
		return boxSize;
	}
}

}
