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
class ViewScalabilityInformationSEIBox extends Box('vsib', size)
{
	unsigned int(8*size-64)	mvcscalinfosei;
}
*/
public partial class ViewScalabilityInformationSEIBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ViewScalabilityInformationSEIBox extends Box('vsib', size)
{
	unsigned int(8*size-64)	mvcscalinfosei;
}"; } }
	public const string TYPE = "vsib";
	public override string DisplayName { get { return "ViewScalabilityInformationSEIBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCMVCSampleEntry(avc1,avc3)", "AVC2MVCSampleEntry(avc2,avc4)", "MVCSampleEntry(mvc1,mvc2,mvc3,mvc4)"); }


	protected byte[] mvcscalinfosei = []; 
	public byte[] Mvcscalinfosei { get { return this.mvcscalinfosei; } set { this.mvcscalinfosei = value; } }

	public ViewScalabilityInformationSEIBox(ulong size = 0): base(IsoStream.FromFourCC("vsib"), size)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*size-64 ),  out this.mvcscalinfosei, "mvcscalinfosei"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits((uint)(8*size-64 ),  this.mvcscalinfosei, "mvcscalinfosei"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += (ulong)(8*size-64 ); // mvcscalinfosei
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("mvcscalinfosei", (ulong)(8*size-64 ), this.mvcscalinfosei); 
		return boxSize;
	}
}

}
