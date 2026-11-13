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
class UsacChannelConfig()
{
	EscapedValue(5, 8, 16) numOutChannels;
	for (i = 0; i < numOutChannels; i++) {
		uimsbf(5) bsOutputChannelPos;
	}
}

// sbrRatioIndex, of coreSbrFrameLengthIndex 2, 3 and 4, is not 0: SBR is used
*/
public partial class UsacChannelConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class UsacChannelConfig()
{
	EscapedValue(5, 8, 16) numOutChannels;
	for (i = 0; i < numOutChannels; i++) {
		uimsbf(5) bsOutputChannelPos;
	}
}

// sbrRatioIndex, of coreSbrFrameLengthIndex 2, 3 and 4, is not 0: SBR is used"; } }
	public virtual string DisplayName { get { return "UsacChannelConfig"; } }

	protected EscapedValue numOutChannels; 
	public EscapedValue NumOutChannels { get { return this.numOutChannels; } set { this.numOutChannels = value; } }

	protected byte[] bsOutputChannelPos; 
	public byte[] BsOutputChannelPos { get { return this.bsOutputChannelPos; } set { this.bsOutputChannelPos = value; } }

	public UsacChannelConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new EscapedValue(5, 8, 16),  out this.numOutChannels, "numOutChannels"); 

		this.bsOutputChannelPos = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( numOutChannels), "bsOutputChannelPos");
		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.bsOutputChannelPos[i], "bsOutputChannelPos"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteClass( this.numOutChannels, "numOutChannels"); 

		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += stream.WriteBits(5,  this.bsOutputChannelPos[i], "bsOutputChannelPos"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += IsoStream.CalculateClassSize(numOutChannels); // numOutChannels

		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += 5; // bsOutputChannelPos
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("numOutChannels", IsoStream.CalculateClassSize(numOutChannels), this.numOutChannels); 

		for (int i = 0; i < numOutChannels; i++)
		{
			boxSize += tracer.Field($"bsOutputChannelPos[{i}]", 5, this.bsOutputChannelPos[i]); 
		}
		return boxSize;
	}
}

}
