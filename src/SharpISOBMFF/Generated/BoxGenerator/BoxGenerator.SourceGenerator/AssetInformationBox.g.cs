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
aligned(8) class AssetInformationBox() extends FullBox('ainf') {
	unsigned int(8) profileVersion[4];
	string apid;
}
*/
public partial class AssetInformationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AssetInformationBox() extends FullBox('ainf') {
	unsigned int(8) profileVersion[4];
	string apid;
}"; } }
	public const string TYPE = "ainf";
	public override string DisplayName { get { return "AssetInformationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("moov"); }


	protected byte[] profileVersion = new byte[4]; 
	public byte[] ProfileVersion { get { return this.profileVersion; } set { this.profileVersion = value; } }

	protected BinaryUTF8String apid; 
	public BinaryUTF8String Apid { get { return this.apid; } set { this.apid = value; } }

	public AssetInformationBox(): base(IsoStream.FromFourCC("ainf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 4,  out this.profileVersion, "profileVersion"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.apid, "apid"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(4,  this.profileVersion, "profileVersion"); 
		boxSize += stream.WriteStringZeroTerminated( this.apid, "apid"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4 * 8; // profileVersion
		boxSize += IsoStream.CalculateStringSize(apid); // apid
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("profileVersion", 4 * 8, this.profileVersion); 
		boxSize += tracer.Field("apid", IsoStream.CalculateStringSize(apid), this.apid); 
		return boxSize;
	}
}

}
