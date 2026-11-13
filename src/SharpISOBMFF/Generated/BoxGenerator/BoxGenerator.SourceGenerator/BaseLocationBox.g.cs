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
aligned(8) class BaseLocationBox() extends FullBox('bloc') {
	unsigned int(8) baseLocation[256];
	unsigned int(8) purchaseLocation[256];
	unsigned int(8) reserved[512];
}
*/
public partial class BaseLocationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class BaseLocationBox() extends FullBox('bloc') {
	unsigned int(8) baseLocation[256];
	unsigned int(8) purchaseLocation[256];
	unsigned int(8) reserved[512];
}"; } }
	public const string TYPE = "bloc";
	public override string DisplayName { get { return "BaseLocationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected byte[] baseLocation = new byte[256]; 
	public byte[] BaseLocation { get { return this.baseLocation; } set { this.baseLocation = value; } }

	protected byte[] purchaseLocation = new byte[256]; 
	public byte[] PurchaseLocation { get { return this.purchaseLocation; } set { this.purchaseLocation = value; } }

	protected byte[] reserved = new byte[512]; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public BaseLocationBox(): base(IsoStream.FromFourCC("bloc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 256,  out this.baseLocation, "baseLocation"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 256,  out this.purchaseLocation, "purchaseLocation"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 512,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(256,  this.baseLocation, "baseLocation"); 
		boxSize += stream.WriteUInt8Array(256,  this.purchaseLocation, "purchaseLocation"); 
		boxSize += stream.WriteUInt8Array(512,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 256 * 8; // baseLocation
		boxSize += 256 * 8; // purchaseLocation
		boxSize += 512 * 8; // reserved
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("baseLocation", 256 * 8, this.baseLocation); 
		boxSize += tracer.Field("purchaseLocation", 256 * 8, this.purchaseLocation); 
		boxSize += tracer.Field("reserved", 512 * 8, this.reserved); 
		return boxSize;
	}
}

}
