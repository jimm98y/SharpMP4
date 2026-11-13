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
aligned(8) class MovieExtendsHeaderBox extends FullBox('mehd', version, 0) {
	if (version==1) {
		unsigned int(64)	fragment_duration;
	} else { // version==0
		unsigned int(32)	fragment_duration;
	}
}
*/
public partial class MovieExtendsHeaderBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MovieExtendsHeaderBox extends FullBox('mehd', version, 0) {
	if (version==1) {
		unsigned int(64)	fragment_duration;
	} else { // version==0
		unsigned int(32)	fragment_duration;
	}
}"; } }
	public const string TYPE = "mehd";
	public override string DisplayName { get { return "MovieExtendsHeaderBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mvex"); }


	protected ulong fragment_duration; 
	public ulong FragmentDuration { get { return this.fragment_duration; } set { this.fragment_duration = value; } }

	public MovieExtendsHeaderBox(byte version = 0): base(IsoStream.FromFourCC("mehd"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragment_duration, "fragment_duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragment_duration, "fragment_duration"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==1)
		{
			boxSize += stream.WriteUInt64( this.fragment_duration, "fragment_duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.WriteUInt32( this.fragment_duration, "fragment_duration"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==1)
		{
			boxSize += 64; // fragment_duration
		}

		else 
		{
			/*  version==0 */
			boxSize += 32; // fragment_duration
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		if (version==1)
		{
			boxSize += tracer.Field("fragment_duration", 64, this.fragment_duration); 
		}

		else 
		{
			/*  version==0 */
			boxSize += tracer.Field("fragment_duration", 32, this.fragment_duration); 
		}
		return boxSize;
	}
}

}
