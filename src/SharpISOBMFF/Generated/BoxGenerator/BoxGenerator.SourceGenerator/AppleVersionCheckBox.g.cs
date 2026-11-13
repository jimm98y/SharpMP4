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
class AppleVersionCheckBox() extends Box ('rmvc'){
	unsigned int(32) flags;
	unsigned int(32) softwarePackage;
	unsigned int(32) version;
	unsigned int(32) mask;
	signed int(16) checkType;
}
*/
public partial class AppleVersionCheckBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleVersionCheckBox() extends Box ('rmvc'){
	unsigned int(32) flags;
	unsigned int(32) softwarePackage;
	unsigned int(32) version;
	unsigned int(32) mask;
	signed int(16) checkType;
}"; } }
	public const string TYPE = "rmvc";
	public override string DisplayName { get { return "AppleVersionCheckBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("rmda"); }


	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	protected uint softwarePackage; 
	public uint SoftwarePackage { get { return this.softwarePackage; } set { this.softwarePackage = value; } }

	protected uint version; 
	public uint Version { get { return this.version; } set { this.version = value; } }

	protected uint mask; 
	public uint Mask { get { return this.mask; } set { this.mask = value; } }

	protected short checkType; 
	public short CheckType { get { return this.checkType; } set { this.checkType = value; } }

	public AppleVersionCheckBox(): base(IsoStream.FromFourCC("rmvc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.softwarePackage, "softwarePackage"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.mask, "mask"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.checkType, "checkType"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.flags, "flags"); 
		boxSize += stream.WriteUInt32( this.softwarePackage, "softwarePackage"); 
		boxSize += stream.WriteUInt32( this.version, "version"); 
		boxSize += stream.WriteUInt32( this.mask, "mask"); 
		boxSize += stream.WriteInt16( this.checkType, "checkType"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // flags
		boxSize += 32; // softwarePackage
		boxSize += 32; // version
		boxSize += 32; // mask
		boxSize += 16; // checkType
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("flags", 32, this.flags); 
		boxSize += tracer.Field("softwarePackage", 32, this.softwarePackage); 
		boxSize += tracer.Field("version", 32, this.version); 
		boxSize += tracer.Field("mask", 32, this.mask); 
		boxSize += tracer.Field("checkType", 16, this.checkType); 
		return boxSize;
	}
}

}
