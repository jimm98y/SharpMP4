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
class RegistrationDescriptor extends BaseDescriptor : bit(8) tag=RegistrationDescrTag {
	bit(32) formatIdentifier;
	bit(8) additionalIdentificationInfo[sizeOfInstance-4];
}
*/
public partial class RegistrationDescriptor : BaseDescriptor
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class RegistrationDescriptor extends BaseDescriptor : bit(8) tag=RegistrationDescrTag {
	bit(32) formatIdentifier;
	bit(8) additionalIdentificationInfo[sizeOfInstance-4];
}"; } }
	public const byte TYPE = DescriptorTags.RegistrationDescrTag;
	public override string DisplayName { get { return "RegistrationDescriptor"; } }

	protected uint formatIdentifier; 
	public uint FormatIdentifier { get { return this.formatIdentifier; } set { this.formatIdentifier = value; } }

	protected byte[] additionalIdentificationInfo = []; 
	public byte[] AdditionalIdentificationInfo { get { return this.additionalIdentificationInfo; } set { this.additionalIdentificationInfo = value; } }

	public RegistrationDescriptor(): base(DescriptorTags.RegistrationDescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.formatIdentifier, "formatIdentifier"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(sizeOfInstance-4),  out this.additionalIdentificationInfo, "additionalIdentificationInfo"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.formatIdentifier, "formatIdentifier"); 
		boxSize += stream.WriteUInt8Array((uint)(sizeOfInstance-4),  this.additionalIdentificationInfo, "additionalIdentificationInfo"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // formatIdentifier
		boxSize += ((ulong)(sizeOfInstance-4) * 8); // additionalIdentificationInfo
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("formatIdentifier", 32, this.formatIdentifier); 
		boxSize += tracer.Field("additionalIdentificationInfo", ((ulong)(sizeOfInstance-4) * 8), this.additionalIdentificationInfo); 
		return boxSize;
	}
}

}
