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
class VvcSubpicIDProperty
	extends ItemFullProperty('spid', version = 0, flags = 0){
	VvcSubpicIDEntry sid_info; // specified in ISO/IEC 14496-15
}
*/
public partial class VvcSubpicIDProperty : ItemFullProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class VvcSubpicIDProperty
	extends ItemFullProperty('spid', version = 0, flags = 0){
	VvcSubpicIDEntry sid_info; // specified in ISO/IEC 14496-15
}"; } }
	public const string TYPE = "spid";
	public override string DisplayName { get { return "VvcSubpicIDProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected VvcSubpicIDEntry sid_info;  //  specified in ISO/IEC 14496-15
	public VvcSubpicIDEntry SidInfo { get { return this.sid_info; } set { this.sid_info = value; } }

	public VvcSubpicIDProperty(): base(IsoStream.FromFourCC("spid"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcSubpicIDEntry(),  out this.sid_info, "sid_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.sid_info, "sid_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(sid_info); // sid_info
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("sid_info", IsoStream.CalculateClassSize(sid_info), this.sid_info); // specified in ISO/IEC 14496-15
		return boxSize;
	}
}

}
