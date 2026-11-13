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
class ComponentDescription() {
	unsigned int(32) componentType;
	unsigned int(32) componentSubType;
	unsigned int(32) componentManufacturer;
	unsigned int(32) componentFlags;
	unsigned int(32) componentFlagsMask;
}
*/
public partial class ComponentDescription : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class ComponentDescription() {
	unsigned int(32) componentType;
	unsigned int(32) componentSubType;
	unsigned int(32) componentManufacturer;
	unsigned int(32) componentFlags;
	unsigned int(32) componentFlagsMask;
}"; } }
	public virtual string DisplayName { get { return "ComponentDescription"; } }

	protected uint componentType; 
	public uint ComponentType { get { return this.componentType; } set { this.componentType = value; } }

	protected uint componentSubType; 
	public uint ComponentSubType { get { return this.componentSubType; } set { this.componentSubType = value; } }

	protected uint componentManufacturer; 
	public uint ComponentManufacturer { get { return this.componentManufacturer; } set { this.componentManufacturer = value; } }

	protected uint componentFlags; 
	public uint ComponentFlags { get { return this.componentFlags; } set { this.componentFlags = value; } }

	protected uint componentFlagsMask; 
	public uint ComponentFlagsMask { get { return this.componentFlagsMask; } set { this.componentFlagsMask = value; } }

	public ComponentDescription(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentType, "componentType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentSubType, "componentSubType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentManufacturer, "componentManufacturer"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentFlags, "componentFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentFlagsMask, "componentFlagsMask"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.componentType, "componentType"); 
		boxSize += stream.WriteUInt32( this.componentSubType, "componentSubType"); 
		boxSize += stream.WriteUInt32( this.componentManufacturer, "componentManufacturer"); 
		boxSize += stream.WriteUInt32( this.componentFlags, "componentFlags"); 
		boxSize += stream.WriteUInt32( this.componentFlagsMask, "componentFlagsMask"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // componentType
		boxSize += 32; // componentSubType
		boxSize += 32; // componentManufacturer
		boxSize += 32; // componentFlags
		boxSize += 32; // componentFlagsMask
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("componentType", 32, this.componentType); 
		boxSize += tracer.Field("componentSubType", 32, this.componentSubType); 
		boxSize += tracer.Field("componentManufacturer", 32, this.componentManufacturer); 
		boxSize += tracer.Field("componentFlags", 32, this.componentFlags); 
		boxSize += tracer.Field("componentFlagsMask", 32, this.componentFlagsMask); 
		return boxSize;
	}
}

}
