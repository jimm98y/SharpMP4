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
// 6.2.4: unsigned int (8 x configOBUs_size) configOBUs, as bytes; what a later version puts after them is left
class IAConfigurationBox extends Box('iacb') {
	unsigned int(8) configurationVersion = 1;
	leb128() configOBUs_size;
	unsigned int(8) configOBUs[configOBUs_size];
}
*/
public partial class IAConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// 6.2.4: unsigned int (8 x configOBUs_size) configOBUs, as bytes; what a later version puts after them is left
class IAConfigurationBox extends Box('iacb') {
	unsigned int(8) configurationVersion = 1;
	leb128() configOBUs_size;
	unsigned int(8) configOBUs[configOBUs_size];
}"; } }
	public const string TYPE = "iacb";
	public override string DisplayName { get { return "IAConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(iamf)"); }


	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected Leb128 configOBUs_size; 
	public Leb128 ConfigOBUsSize { get { return this.configOBUs_size; } set { this.configOBUs_size = value; } }

	protected byte[] configOBUs = []; 
	public byte[] ConfigOBUs { get { return this.configOBUs; } set { this.configOBUs = value; } }

	public IAConfigurationBox(): base(IsoStream.FromFourCC("iacb"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadLeb128(boxSize, readSize,  out this.configOBUs_size, "configOBUs_size"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(configOBUs_size),  out this.configOBUs, "configOBUs"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.configurationVersion, "configurationVersion"); 
		boxSize += stream.WriteLeb128( this.configOBUs_size, "configOBUs_size"); 
		boxSize += stream.WriteUInt8Array((uint)(configOBUs_size),  this.configOBUs, "configOBUs"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // configurationVersion
		boxSize += IsoStream.CalculateLeb128Size(configOBUs_size); // configOBUs_size
		boxSize += ((ulong)(configOBUs_size) * 8); // configOBUs
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("configurationVersion", 8, this.configurationVersion); 
		boxSize += tracer.Field("configOBUs_size", IsoStream.CalculateLeb128Size(configOBUs_size), this.configOBUs_size); 
		boxSize += tracer.Field("configOBUs", ((ulong)(configOBUs_size) * 8), this.configOBUs); 
		return boxSize;
	}
}

}
