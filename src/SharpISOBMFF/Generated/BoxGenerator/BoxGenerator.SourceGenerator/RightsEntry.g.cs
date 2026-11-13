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
aligned(8) class RightsEntry() {
	unsigned int(32) key; // veID, plat, aver, tran, song, tool, medi, mode
	unsigned int(32) value;
}
*/
public partial class RightsEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class RightsEntry() {
	unsigned int(32) key; // veID, plat, aver, tran, song, tool, medi, mode
	unsigned int(32) value;
}"; } }
	public virtual string DisplayName { get { return "RightsEntry"; } }

	protected uint key;  //  veID, plat, aver, tran, song, tool, medi, mode
	public uint Key { get { return this.key; } set { this.key = value; } }

	protected uint value; 
	public uint Value { get { return this.value; } set { this.value = value; } }

	public RightsEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.key, "key"); // veID, plat, aver, tran, song, tool, medi, mode
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.key, "key"); // veID, plat, aver, tran, song, tool, medi, mode
		boxSize += stream.WriteUInt32( this.value, "value"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // key
		boxSize += 32; // value
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("key", 32, this.key); // veID, plat, aver, tran, song, tool, medi, mode
		boxSize += tracer.Field("value", 32, this.value); 
		return boxSize;
	}
}

}
