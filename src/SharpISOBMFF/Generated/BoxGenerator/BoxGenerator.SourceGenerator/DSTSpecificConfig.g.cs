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
class DSTSpecificConfig(channelConfiguration) {
	uimsbf(1) DSDDST_Coded;
	uimsbf(14) N_Channels;
	uimsbf(1) reserved;
}
*/
public partial class DSTSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class DSTSpecificConfig(channelConfiguration) {
	uimsbf(1) DSDDST_Coded;
	uimsbf(14) N_Channels;
	uimsbf(1) reserved;
}"; } }
	public virtual string DisplayName { get { return "DSTSpecificConfig"; } }

	protected bool DSDDST_Coded; 
	public bool DSDDSTCoded { get { return this.DSDDST_Coded; } set { this.DSDDST_Coded = value; } }

	protected ushort N_Channels; 
	public ushort NChannels { get { return this.N_Channels; } set { this.N_Channels = value; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public DSTSpecificConfig(int channelConfiguration): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.DSDDST_Coded, "DSDDST_Coded"); 
		boxSize += stream.ReadBits(boxSize, readSize, 14,  out this.N_Channels, "N_Channels"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.DSDDST_Coded, "DSDDST_Coded"); 
		boxSize += stream.WriteBits(14,  this.N_Channels, "N_Channels"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // DSDDST_Coded
		boxSize += 14; // N_Channels
		boxSize += 1; // reserved
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("DSDDST_Coded", 1, this.DSDDST_Coded); 
		boxSize += tracer.Field("N_Channels", 14, this.N_Channels); 
		boxSize += tracer.Field("reserved", 1, this.reserved); 
		return boxSize;
	}
}

}
