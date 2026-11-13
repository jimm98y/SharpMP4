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
aligned(8) class TrunEntry(version, flags) {
	if(flags & 0x100) {
		unsigned int(32) sample_duration;
	}
	if(flags & 0x200) {
		unsigned int(32) sample_size;
	}
	if(flags & 0x400) {
		unsigned int(32) sample_flags;
	}
	if(flags & 0x800)
	{
		if (version == 0)
		{
			unsigned int(32) sample_composition_time_offset;
		}
		else
		{
			signed int(32) sample_composition_time_offset;
		}
	}
}
*/
public partial class TrunEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class TrunEntry(version, flags) {
	if(flags & 0x100) {
		unsigned int(32) sample_duration;
	}
	if(flags & 0x200) {
		unsigned int(32) sample_size;
	}
	if(flags & 0x400) {
		unsigned int(32) sample_flags;
	}
	if(flags & 0x800)
	{
		if (version == 0)
		{
			unsigned int(32) sample_composition_time_offset;
		}
		else
		{
			signed int(32) sample_composition_time_offset;
		}
	}
}"; } }
	public virtual string DisplayName { get { return "TrunEntry"; } }

	protected uint sample_duration; 
	public uint SampleDuration { get { return this.sample_duration; } set { this.sample_duration = value; } }

	protected uint sample_size; 
	public uint SampleSize { get { return this.sample_size; } set { this.sample_size = value; } }

	protected uint sample_flags; 
	public uint SampleFlags { get { return this.sample_flags; } set { this.sample_flags = value; } }

	protected uint sample_composition_time_offset; 
	public uint SampleCompositionTimeOffset { get { return this.sample_composition_time_offset; } set { this.sample_composition_time_offset = value; } }

	protected int sample_composition_time_offset0; 
	public int SampleCompositionTimeOffset0 { get { return this.sample_composition_time_offset0; } set { this.sample_composition_time_offset0 = value; } }

	protected byte version; 
	public byte Version { get { return this.version; } set { this.version = value; } }

	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	public TrunEntry(byte version = 0, uint flags = 0): base()
	{
		this.version = version;
		 this.flags = flags;	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;

		if ((flags  &  0x100) ==  0x100)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_duration, "sample_duration"); 
		}

		if ((flags  &  0x200) ==  0x200)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_size, "sample_size"); 
		}

		if ((flags  &  0x400) ==  0x400)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_flags, "sample_flags"); 
		}

		if ((flags  &  0x800) ==  0x800)
		{

			if (version == 0)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sample_composition_time_offset, "sample_composition_time_offset"); 
			}

			else 
			{
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.sample_composition_time_offset0, "sample_composition_time_offset0"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;

		if ((flags  &  0x100) ==  0x100)
		{
			boxSize += stream.WriteUInt32( this.sample_duration, "sample_duration"); 
		}

		if ((flags  &  0x200) ==  0x200)
		{
			boxSize += stream.WriteUInt32( this.sample_size, "sample_size"); 
		}

		if ((flags  &  0x400) ==  0x400)
		{
			boxSize += stream.WriteUInt32( this.sample_flags, "sample_flags"); 
		}

		if ((flags  &  0x800) ==  0x800)
		{

			if (version == 0)
			{
				boxSize += stream.WriteUInt32( this.sample_composition_time_offset, "sample_composition_time_offset"); 
			}

			else 
			{
				boxSize += stream.WriteInt32( this.sample_composition_time_offset0, "sample_composition_time_offset0"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;

		if ((flags  &  0x100) ==  0x100)
		{
			boxSize += 32; // sample_duration
		}

		if ((flags  &  0x200) ==  0x200)
		{
			boxSize += 32; // sample_size
		}

		if ((flags  &  0x400) ==  0x400)
		{
			boxSize += 32; // sample_flags
		}

		if ((flags  &  0x800) ==  0x800)
		{

			if (version == 0)
			{
				boxSize += 32; // sample_composition_time_offset
			}

			else 
			{
				boxSize += 32; // sample_composition_time_offset0
			}
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;

		if ((flags  &  0x100) ==  0x100)
		{
			boxSize += tracer.Field("sample_duration", 32, this.sample_duration); 
		}

		if ((flags  &  0x200) ==  0x200)
		{
			boxSize += tracer.Field("sample_size", 32, this.sample_size); 
		}

		if ((flags  &  0x400) ==  0x400)
		{
			boxSize += tracer.Field("sample_flags", 32, this.sample_flags); 
		}

		if ((flags  &  0x800) ==  0x800)
		{

			if (version == 0)
			{
				boxSize += tracer.Field("sample_composition_time_offset", 32, this.sample_composition_time_offset); 
			}

			else 
			{
				boxSize += tracer.Field("sample_composition_time_offset0", 32, this.sample_composition_time_offset0); 
			}
		}
		return boxSize;
	}
}

}
