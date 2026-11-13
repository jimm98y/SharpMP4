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
class ParametricSpecificConfig()
{
	uimsbf(1) isBaseLayer;
	if (isBaseLayer) {
		PARAconfig();
	}
	else {
		HILNenexConfig();
	}
}
*/
public partial class ParametricSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class ParametricSpecificConfig()
{
	uimsbf(1) isBaseLayer;
	if (isBaseLayer) {
		PARAconfig();
	}
	else {
		HILNenexConfig();
	}
}"; } }
	public virtual string DisplayName { get { return "ParametricSpecificConfig"; } }

	protected bool isBaseLayer; 
	public bool IsBaseLayer { get { return this.isBaseLayer; } set { this.isBaseLayer = value; } }

	protected PARAconfig PARAconfig; 
	public PARAconfig _PARAconfig { get { return this.PARAconfig; } set { this.PARAconfig = value; } }

	protected HILNenexConfig HILNenexConfig; 
	public HILNenexConfig _HILNenexConfig { get { return this.HILNenexConfig; } set { this.HILNenexConfig = value; } }

	public ParametricSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new PARAconfig(),  out this.PARAconfig, "PARAconfig"); 
		}

		else 
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new HILNenexConfig(),  out this.HILNenexConfig, "HILNenexConfig"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.isBaseLayer, "isBaseLayer"); 

		if (isBaseLayer)
		{
			boxSize += stream.WriteClass( this.PARAconfig, "PARAconfig"); 
		}

		else 
		{
			boxSize += stream.WriteClass( this.HILNenexConfig, "HILNenexConfig"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // isBaseLayer

		if (isBaseLayer)
		{
			boxSize += IsoStream.CalculateClassSize(PARAconfig); // PARAconfig
		}

		else 
		{
			boxSize += IsoStream.CalculateClassSize(HILNenexConfig); // HILNenexConfig
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("isBaseLayer", 1, this.isBaseLayer); 

		if (isBaseLayer)
		{
			boxSize += tracer.Field("PARAconfig", IsoStream.CalculateClassSize(PARAconfig), this.PARAconfig); 
		}

		else 
		{
			boxSize += tracer.Field("HILNenexConfig", IsoStream.CalculateClassSize(HILNenexConfig), this.HILNenexConfig); 
		}
		return boxSize;
	}
}

}
