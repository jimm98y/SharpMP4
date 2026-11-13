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
aligned(8) class SubsegmentIndexBox extends FullBox('ssix', 0, 0) {
	unsigned int(32) subsegment_count;
	for( i=1; i <= subsegment_count; i++)
	{
		unsigned int(32) range_count;
		for ( j=1; j <= range_count; j++) {
			unsigned int(8) level;
			unsigned int(24) range_size;
		}
	}
}
*/
public partial class SubsegmentIndexBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SubsegmentIndexBox extends FullBox('ssix', 0, 0) {
	unsigned int(32) subsegment_count;
	for( i=1; i <= subsegment_count; i++)
	{
		unsigned int(32) range_count;
		for ( j=1; j <= range_count; j++) {
			unsigned int(8) level;
			unsigned int(24) range_size;
		}
	}
}"; } }
	public const string TYPE = "ssix";
	public override string DisplayName { get { return "SubsegmentIndexBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected uint subsegment_count; 
	public uint SubsegmentCount { get { return this.subsegment_count; } set { this.subsegment_count = value; } }

	protected uint[] range_count; 
	public uint[] RangeCount { get { return this.range_count; } set { this.range_count = value; } }

	protected byte[][] level; 
	public byte[][] Level { get { return this.level; } set { this.level = value; } }

	protected uint[][] range_size; 
	public uint[][] RangeSize { get { return this.range_size; } set { this.range_size = value; } }

	public SubsegmentIndexBox(): base(IsoStream.FromFourCC("ssix"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.subsegment_count, "subsegment_count"); 

		this.range_count = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( subsegment_count), "range_count");
		this.level = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( subsegment_count), "level");
		this.range_size = stream.SafeAllocate<uint[]>(boxSize, readSize, IsoStream.GetInt( subsegment_count), "range_size");
		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.range_count[i], "range_count"); 

			this.level[i] = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( range_count[i]), "level[i]");
			this.range_size[i] = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( range_count[i]), "range_size[i]");
			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level[i][j], "level"); 
				boxSize += stream.ReadUInt24(boxSize, readSize,  out this.range_size[i][j], "range_size"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.subsegment_count, "subsegment_count"); 

		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += stream.WriteUInt32( this.range_count[i], "range_count"); 

			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += stream.WriteUInt8( this.level[i][j], "level"); 
				boxSize += stream.WriteUInt24( this.range_size[i][j], "range_size"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // subsegment_count

		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += 32; // range_count

			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += 8; // level
				boxSize += 24; // range_size
			}
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("subsegment_count", 32, this.subsegment_count); 

		for (int  i=0; i < subsegment_count; i++)
		{
			boxSize += tracer.Field($"range_count[{i}]", 32, this.range_count[i]); 

			for (int  j=0; j < range_count[i]; j++)
			{
				boxSize += tracer.Field($"level[{i}][{j}]", 8, this.level[i][j]); 
				boxSize += tracer.Field($"range_size[{i}][{j}]", 24, this.range_size[i][j]); 
			}
		}
		return boxSize;
	}
}

}
