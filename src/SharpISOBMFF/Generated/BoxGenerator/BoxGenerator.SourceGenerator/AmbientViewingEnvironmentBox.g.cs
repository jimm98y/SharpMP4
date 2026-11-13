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
class AmbientViewingEnvironmentBox extends Box('amve'){
	unsigned int(32) ambient_illuminance;
	unsigned int(16) ambient_light_x;
	unsigned int(16) ambient_light_y;
}
*/
public partial class AmbientViewingEnvironmentBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AmbientViewingEnvironmentBox extends Box('amve'){
	unsigned int(32) ambient_illuminance;
	unsigned int(16) ambient_light_x;
	unsigned int(16) ambient_light_y;
}"; } }
	public const string TYPE = "amve";
	public override string DisplayName { get { return "AmbientViewingEnvironmentBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry", "ipco"); }


	protected uint ambient_illuminance; 
	public uint AmbientIlluminance { get { return this.ambient_illuminance; } set { this.ambient_illuminance = value; } }

	protected ushort ambient_light_x; 
	public ushort AmbientLightx { get { return this.ambient_light_x; } set { this.ambient_light_x = value; } }

	protected ushort ambient_light_y; 
	public ushort AmbientLighty { get { return this.ambient_light_y; } set { this.ambient_light_y = value; } }

	public AmbientViewingEnvironmentBox(): base(IsoStream.FromFourCC("amve"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ambient_illuminance, "ambient_illuminance"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ambient_light_x, "ambient_light_x"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ambient_light_y, "ambient_light_y"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.ambient_illuminance, "ambient_illuminance"); 
		boxSize += stream.WriteUInt16( this.ambient_light_x, "ambient_light_x"); 
		boxSize += stream.WriteUInt16( this.ambient_light_y, "ambient_light_y"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // ambient_illuminance
		boxSize += 16; // ambient_light_x
		boxSize += 16; // ambient_light_y
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("ambient_illuminance", 32, this.ambient_illuminance); 
		boxSize += tracer.Field("ambient_light_x", 16, this.ambient_light_x); 
		boxSize += tracer.Field("ambient_light_y", 16, this.ambient_light_y); 
		return boxSize;
	}
}

}
