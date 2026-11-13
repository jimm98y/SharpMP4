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
class ContentLightLevelBox extends Box('clli'){
	unsigned int(16) max_content_light_level;
	unsigned int(16) max_pic_average_light_level;
}
*/
public partial class ContentLightLevelBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ContentLightLevelBox extends Box('clli'){
	unsigned int(16) max_content_light_level;
	unsigned int(16) max_pic_average_light_level;
}"; } }
	public const string TYPE = "clli";
	public override string DisplayName { get { return "ContentLightLevelBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry", "ipco"); }


	protected ushort max_content_light_level; 
	public ushort MaxContentLightLevel { get { return this.max_content_light_level; } set { this.max_content_light_level = value; } }

	protected ushort max_pic_average_light_level; 
	public ushort MaxPicAverageLightLevel { get { return this.max_pic_average_light_level; } set { this.max_pic_average_light_level = value; } }

	public ContentLightLevelBox(): base(IsoStream.FromFourCC("clli"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_content_light_level, "max_content_light_level"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_pic_average_light_level, "max_pic_average_light_level"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.max_content_light_level, "max_content_light_level"); 
		boxSize += stream.WriteUInt16( this.max_pic_average_light_level, "max_pic_average_light_level"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // max_content_light_level
		boxSize += 16; // max_pic_average_light_level
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("max_content_light_level", 16, this.max_content_light_level); 
		boxSize += tracer.Field("max_pic_average_light_level", 16, this.max_pic_average_light_level); 
		return boxSize;
	}
}

}
