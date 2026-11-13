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
aligned(8) class RtpMovieHintInformation extends Box('rtp ') {
	uint(32) descriptionformat = 'sdp ';
	char  sdptext[];
}
*/
public partial class RtpMovieHintInformation : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class RtpMovieHintInformation extends Box('rtp ') {
	uint(32) descriptionformat = 'sdp ';
	char  sdptext[];
}"; } }
	public const string TYPE = "rtp ";
	public override string DisplayName { get { return "RtpMovieHintInformation"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hnti", "stsd"); }


	protected uint descriptionformat = IsoStream.FromFourCC("sdp "); 
	public uint Descriptionformat { get { return this.descriptionformat; } set { this.descriptionformat = value; } }

	protected byte[] sdptext = []; 
	public byte[] Sdptext { get { return this.sdptext; } set { this.sdptext = value; } }

	public RtpMovieHintInformation(): base(IsoStream.FromFourCC("rtp "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.descriptionformat, "descriptionformat"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.sdptext, "sdptext"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.descriptionformat, "descriptionformat"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.sdptext, "sdptext"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // descriptionformat
		boxSize += ((ulong)sdptext.Length * 8); // sdptext
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("descriptionformat", 32, this.descriptionformat); 
		boxSize += tracer.Field("sdptext", ((ulong)sdptext.Length * 8), this.sdptext); 
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
