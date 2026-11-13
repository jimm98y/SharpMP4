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
aligned(8) class DependencyInfo
{
	unsigned int(8)   subSeqDirectionFlag;
	unsigned int(8)   layerNumber;
	unsigned int(16)  subSequenceIdentifier;
}
*/
public partial class DependencyInfo : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class DependencyInfo
{
	unsigned int(8)   subSeqDirectionFlag;
	unsigned int(8)   layerNumber;
	unsigned int(16)  subSequenceIdentifier;
}"; } }
	public virtual string DisplayName { get { return "DependencyInfo"; } }

	protected byte subSeqDirectionFlag; 
	public byte SubSeqDirectionFlag { get { return this.subSeqDirectionFlag; } set { this.subSeqDirectionFlag = value; } }

	protected byte layerNumber; 
	public byte LayerNumber { get { return this.layerNumber; } set { this.layerNumber = value; } }

	protected ushort subSequenceIdentifier; 
	public ushort SubSequenceIdentifier { get { return this.subSequenceIdentifier; } set { this.subSequenceIdentifier = value; } }

	public DependencyInfo(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.subSeqDirectionFlag, "subSeqDirectionFlag"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layerNumber, "layerNumber"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subSequenceIdentifier, "subSequenceIdentifier"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.subSeqDirectionFlag, "subSeqDirectionFlag"); 
		boxSize += stream.WriteUInt8( this.layerNumber, "layerNumber"); 
		boxSize += stream.WriteUInt16( this.subSequenceIdentifier, "subSequenceIdentifier"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // subSeqDirectionFlag
		boxSize += 8; // layerNumber
		boxSize += 16; // subSequenceIdentifier
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("subSeqDirectionFlag", 8, this.subSeqDirectionFlag); 
		boxSize += tracer.Field("layerNumber", 8, this.layerNumber); 
		boxSize += tracer.Field("subSequenceIdentifier", 16, this.subSequenceIdentifier); 
		return boxSize;
	}
}

}
