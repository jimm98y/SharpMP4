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
class MVCViewPriorityAssignmentBox extends Box('mvcP')
{
	unsigned int(8)	method_count;
	string PriorityAssignmentURI[method_count];
}
*/
public partial class MVCViewPriorityAssignmentBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class MVCViewPriorityAssignmentBox extends Box('mvcP')
{
	unsigned int(8)	method_count;
	string PriorityAssignmentURI[method_count];
}"; } }
	public const string TYPE = "mvcP";
	public override string DisplayName { get { return "MVCViewPriorityAssignmentBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCMVCSampleEntry(avc1,avc3)", "AVC2MVCSampleEntry(avc2,avc4)", "MVCSampleEntry(mvc1,mvc2,mvc3,mvc4)"); }


	protected byte method_count; 
	public byte MethodCount { get { return this.method_count; } set { this.method_count = value; } }

	protected BinaryUTF8String[] PriorityAssignmentURI = []; 
	public BinaryUTF8String[] _PriorityAssignmentURI { get { return this.PriorityAssignmentURI; } set { this.PriorityAssignmentURI = value; } }

	public MVCViewPriorityAssignmentBox(): base(IsoStream.FromFourCC("mvcP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.method_count, "method_count"); 
		boxSize += stream.ReadStringZeroTerminatedArray(boxSize, readSize, (uint)(method_count),  out this.PriorityAssignmentURI, "PriorityAssignmentURI"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.method_count, "method_count"); 
		boxSize += stream.WriteStringZeroTerminatedArray((uint)(method_count),  this.PriorityAssignmentURI, "PriorityAssignmentURI"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // method_count
		boxSize += IsoStream.CalculateStringSize(PriorityAssignmentURI); // PriorityAssignmentURI
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("method_count", 8, this.method_count); 
		boxSize += tracer.Field("PriorityAssignmentURI", IsoStream.CalculateStringSize(PriorityAssignmentURI), this.PriorityAssignmentURI); 
		return boxSize;
	}
}

}
