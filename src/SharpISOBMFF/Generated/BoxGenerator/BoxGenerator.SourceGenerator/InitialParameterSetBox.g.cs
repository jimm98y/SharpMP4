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
class InitialParameterSetBox extends Box ('svip') {
	unsigned int(8) sps_id_count;
	for (i=0; i< sps_id_count; i++)
		unsigned int(8) SPS_index;
	unsigned int(8) pps_id_count;
	for (i=0; i< pps_id_count; i++)
		unsigned int(8) PPS_index;
}
*/
public partial class InitialParameterSetBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class InitialParameterSetBox extends Box ('svip') {
	unsigned int(8) sps_id_count;
	for (i=0; i< sps_id_count; i++)
		unsigned int(8) SPS_index;
	unsigned int(8) pps_id_count;
	for (i=0; i< pps_id_count; i++)
		unsigned int(8) PPS_index;
}"; } }
	public const string TYPE = "svip";
	public override string DisplayName { get { return "InitialParameterSetBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("scif", "mvif"); }


	protected byte sps_id_count; 
	public byte SpsIdCount { get { return this.sps_id_count; } set { this.sps_id_count = value; } }

	protected byte[] SPS_index; 
	public byte[] SPSIndex { get { return this.SPS_index; } set { this.SPS_index = value; } }

	protected byte pps_id_count; 
	public byte PpsIdCount { get { return this.pps_id_count; } set { this.pps_id_count = value; } }

	protected byte[] PPS_index; 
	public byte[] PPSIndex { get { return this.PPS_index; } set { this.PPS_index = value; } }

	public InitialParameterSetBox(): base(IsoStream.FromFourCC("svip"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sps_id_count, "sps_id_count"); 

		this.SPS_index = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( sps_id_count), "SPS_index");
		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.SPS_index[i], "SPS_index"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pps_id_count, "pps_id_count"); 

		this.PPS_index = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( pps_id_count), "PPS_index");
		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.PPS_index[i], "PPS_index"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.sps_id_count, "sps_id_count"); 

		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += stream.WriteUInt8( this.SPS_index[i], "SPS_index"); 
		}
		boxSize += stream.WriteUInt8( this.pps_id_count, "pps_id_count"); 

		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += stream.WriteUInt8( this.PPS_index[i], "PPS_index"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // sps_id_count

		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += 8; // SPS_index
		}
		boxSize += 8; // pps_id_count

		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += 8; // PPS_index
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("sps_id_count", 8, this.sps_id_count); 

		for (int i=0; i< sps_id_count; i++)
		{
			boxSize += tracer.Field($"SPS_index[{i}]", 8, this.SPS_index[i]); 
		}
		boxSize += tracer.Field("pps_id_count", 8, this.pps_id_count); 

		for (int i=0; i< pps_id_count; i++)
		{
			boxSize += tracer.Field($"PPS_index[{i}]", 8, this.PPS_index[i]); 
		}
		return boxSize;
	}
}

}
