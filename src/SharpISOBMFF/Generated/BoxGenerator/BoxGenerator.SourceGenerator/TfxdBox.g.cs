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
aligned(8) class TfxdBox() extends FullBox('uuid 6d1d9b0542d544e680e2141daff757b2') {
	if(version == 0x1) {
		unsigned int(64) fragmentAbsoluteTime;
		unsigned int(64) fragmentAbsoluteDuration;
	} else {
		unsigned int(32) fragmentAbsoluteTime;
		unsigned int(32) fragmentAbsoluteDuration;
} }
*/
public partial class TfxdBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TfxdBox() extends FullBox('uuid 6d1d9b0542d544e680e2141daff757b2') {
	if(version == 0x1) {
		unsigned int(64) fragmentAbsoluteTime;
		unsigned int(64) fragmentAbsoluteDuration;
	} else {
		unsigned int(32) fragmentAbsoluteTime;
		unsigned int(32) fragmentAbsoluteDuration;
} }"; } }
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "TfxdBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	protected ulong fragmentAbsoluteTime; 
	public ulong FragmentAbsoluteTime { get { return this.fragmentAbsoluteTime; } set { this.fragmentAbsoluteTime = value; } }

	protected ulong fragmentAbsoluteDuration; 
	public ulong FragmentAbsoluteDuration { get { return this.fragmentAbsoluteDuration; } set { this.fragmentAbsoluteDuration = value; } }

	public TfxdBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("6d1d9b0542d544e680e2141daff757b2"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0x1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0x1)
		{
			boxSize += stream.WriteUInt64( this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.WriteUInt64( this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.WriteUInt32( this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0x1)
		{
			boxSize += 64; // fragmentAbsoluteTime
			boxSize += 64; // fragmentAbsoluteDuration
		}

		else 
		{
			boxSize += 32; // fragmentAbsoluteTime
			boxSize += 32; // fragmentAbsoluteDuration
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		if (version == 0x1)
		{
			boxSize += tracer.Field("fragmentAbsoluteTime", 64, this.fragmentAbsoluteTime); 
			boxSize += tracer.Field("fragmentAbsoluteDuration", 64, this.fragmentAbsoluteDuration); 
		}

		else 
		{
			boxSize += tracer.Field("fragmentAbsoluteTime", 32, this.fragmentAbsoluteTime); 
			boxSize += tracer.Field("fragmentAbsoluteDuration", 32, this.fragmentAbsoluteDuration); 
		}
		return boxSize;
	}
}

}
