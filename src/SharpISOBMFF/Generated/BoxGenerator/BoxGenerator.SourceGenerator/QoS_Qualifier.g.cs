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
abstract aligned(8) expandable(228-1) class QoS_Qualifier : bit(8) tag=0x01..0xff {
	// empty. To be filled by classes extending this class.
}
*/
public abstract partial class QoS_Qualifier : Descriptor
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"abstract aligned(8) expandable(228-1) class QoS_Qualifier : bit(8) tag=0x01..0xff {
	// empty. To be filled by classes extending this class.
}"; } }
	public byte TagMin { get; set; } = 0x01;
	public byte TagMax { get; set; } = 0xff;	public override string DisplayName { get { return "QoS_Qualifier"; } }

	public QoS_Qualifier(byte tag): base(tag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		/*  empty. To be filled by classes extending this class. */
		return boxSize;
	}
}

}
