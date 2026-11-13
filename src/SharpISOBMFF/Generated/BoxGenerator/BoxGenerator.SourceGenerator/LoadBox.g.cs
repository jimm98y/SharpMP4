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
aligned(8) class LoadBox() extends Box('load') {
	signed int(32) preloadStartTime;
	signed int(32) preloadDuration;
	unsigned int(32) preloadFlags;
	unsigned int(32) defaultHints;
}
*/
public partial class LoadBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class LoadBox() extends Box('load') {
	signed int(32) preloadStartTime;
	signed int(32) preloadDuration;
	unsigned int(32) preloadFlags;
	unsigned int(32) defaultHints;
}"; } }
	public const string TYPE = "load";
	public override string DisplayName { get { return "LoadBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("trak"); }


	protected int preloadStartTime; 
	public int PreloadStartTime { get { return this.preloadStartTime; } set { this.preloadStartTime = value; } }

	protected int preloadDuration; 
	public int PreloadDuration { get { return this.preloadDuration; } set { this.preloadDuration = value; } }

	protected uint preloadFlags; 
	public uint PreloadFlags { get { return this.preloadFlags; } set { this.preloadFlags = value; } }

	protected uint defaultHints; 
	public uint DefaultHints { get { return this.defaultHints; } set { this.defaultHints = value; } }

	public LoadBox(): base(IsoStream.FromFourCC("load"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.preloadStartTime, "preloadStartTime"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.preloadDuration, "preloadDuration"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.preloadFlags, "preloadFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.defaultHints, "defaultHints"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.preloadStartTime, "preloadStartTime"); 
		boxSize += stream.WriteInt32( this.preloadDuration, "preloadDuration"); 
		boxSize += stream.WriteUInt32( this.preloadFlags, "preloadFlags"); 
		boxSize += stream.WriteUInt32( this.defaultHints, "defaultHints"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // preloadStartTime
		boxSize += 32; // preloadDuration
		boxSize += 32; // preloadFlags
		boxSize += 32; // defaultHints
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("preloadStartTime", 32, this.preloadStartTime); 
		boxSize += tracer.Field("preloadDuration", 32, this.preloadDuration); 
		boxSize += tracer.Field("preloadFlags", 32, this.preloadFlags); 
		boxSize += tracer.Field("defaultHints", 32, this.defaultHints); 
		return boxSize;
	}
}

}
