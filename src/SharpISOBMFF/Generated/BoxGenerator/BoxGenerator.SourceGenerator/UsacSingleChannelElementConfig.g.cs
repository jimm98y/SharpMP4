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
class UsacSingleChannelElementConfig(coreSbrFrameLengthIndex)
{
	bslbf(1) tw_mdct;
	bslbf(1) noiseFilling;
	if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4) {
		SbrConfig() sbrConfig;
	}
}
*/
public partial class UsacSingleChannelElementConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class UsacSingleChannelElementConfig(coreSbrFrameLengthIndex)
{
	bslbf(1) tw_mdct;
	bslbf(1) noiseFilling;
	if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4) {
		SbrConfig() sbrConfig;
	}
}"; } }
	public virtual string DisplayName { get { return "UsacSingleChannelElementConfig"; } }

	protected bool tw_mdct; 
	public bool TwMdct { get { return this.tw_mdct; } set { this.tw_mdct = value; } }

	protected bool noiseFilling; 
	public bool NoiseFilling { get { return this.noiseFilling; } set { this.noiseFilling = value; } }

	protected SbrConfig sbrConfig; 
	public SbrConfig SbrConfig { get { return this.sbrConfig; } set { this.sbrConfig = value; } }

	protected int coreSbrFrameLengthIndex; 
	public int CoreSbrFrameLengthIndex { get { return this.coreSbrFrameLengthIndex; } set { this.coreSbrFrameLengthIndex = value; } }

	public UsacSingleChannelElementConfig(int coreSbrFrameLengthIndex = 0): base()
	{
		this.coreSbrFrameLengthIndex = coreSbrFrameLengthIndex;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.tw_mdct, "tw_mdct"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.noiseFilling, "noiseFilling"); 

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new SbrConfig(),  out this.sbrConfig, "sbrConfig"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.tw_mdct, "tw_mdct"); 
		boxSize += stream.WriteBit( this.noiseFilling, "noiseFilling"); 

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += stream.WriteClass( this.sbrConfig, "sbrConfig"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // tw_mdct
		boxSize += 1; // noiseFilling

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += IsoStream.CalculateClassSize(sbrConfig); // sbrConfig
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("tw_mdct", 1, this.tw_mdct); 
		boxSize += tracer.Field("noiseFilling", 1, this.noiseFilling); 

		if (coreSbrFrameLengthIndex >= 2 && coreSbrFrameLengthIndex <= 4)
		{
			boxSize += tracer.Field("sbrConfig", IsoStream.CalculateClassSize(sbrConfig), this.sbrConfig); 
		}
		return boxSize;
	}
}

}
