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
aligned(8) class VvcSubpicOrderEntry() extends VisualSampleGroupEntry('spor')
{
	unsigned int(1) subpic_id_info_flag;
	unsigned int(15) num_subpic_ref_idx;
	for (i = 0; i < num_subpic_ref_idx; i++)
		unsigned int(16) subp_track_ref_idx[i];
	if (subpic_id_info_flag)
		VVCSubpicIDRewritingInfomationStruct() subpic_id_rewriting_info;
}
*/
public partial class VvcSubpicOrderEntry : VisualSampleGroupEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class VvcSubpicOrderEntry() extends VisualSampleGroupEntry('spor')
{
	unsigned int(1) subpic_id_info_flag;
	unsigned int(15) num_subpic_ref_idx;
	for (i = 0; i < num_subpic_ref_idx; i++)
		unsigned int(16) subp_track_ref_idx[i];
	if (subpic_id_info_flag)
		VVCSubpicIDRewritingInfomationStruct() subpic_id_rewriting_info;
}"; } }
	public const string TYPE = "spor";
	public override string DisplayName { get { return "VvcSubpicOrderEntry"; } }

	protected bool subpic_id_info_flag; 
	public bool SubpicIdInfoFlag { get { return this.subpic_id_info_flag; } set { this.subpic_id_info_flag = value; } }

	protected ushort num_subpic_ref_idx; 
	public ushort NumSubpicRefIdx { get { return this.num_subpic_ref_idx; } set { this.num_subpic_ref_idx = value; } }

	protected ushort[] subp_track_ref_idx; 
	public ushort[] SubpTrackRefIdx { get { return this.subp_track_ref_idx; } set { this.subp_track_ref_idx = value; } }

	protected VVCSubpicIDRewritingInfomationStruct subpic_id_rewriting_info; 
	public VVCSubpicIDRewritingInfomationStruct SubpicIdRewritingInfo { get { return this.subpic_id_rewriting_info; } set { this.subpic_id_rewriting_info = value; } }

	public VvcSubpicOrderEntry(): base(IsoStream.FromFourCC("spor"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.subpic_id_info_flag, "subpic_id_info_flag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 15,  out this.num_subpic_ref_idx, "num_subpic_ref_idx"); 

		this.subp_track_ref_idx = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_subpic_ref_idx), "subp_track_ref_idx");
		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subp_track_ref_idx[i], "subp_track_ref_idx"); 
		}

		if (subpic_id_info_flag)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new VVCSubpicIDRewritingInfomationStruct(),  out this.subpic_id_rewriting_info, "subpic_id_rewriting_info"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.subpic_id_info_flag, "subpic_id_info_flag"); 
		boxSize += stream.WriteBits(15,  this.num_subpic_ref_idx, "num_subpic_ref_idx"); 

		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += stream.WriteUInt16( this.subp_track_ref_idx[i], "subp_track_ref_idx"); 
		}

		if (subpic_id_info_flag)
		{
			boxSize += stream.WriteClass( this.subpic_id_rewriting_info, "subpic_id_rewriting_info"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // subpic_id_info_flag
		boxSize += 15; // num_subpic_ref_idx

		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += 16; // subp_track_ref_idx
		}

		if (subpic_id_info_flag)
		{
			boxSize += IsoStream.CalculateClassSize(subpic_id_rewriting_info); // subpic_id_rewriting_info
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("subpic_id_info_flag", 1, this.subpic_id_info_flag); 
		boxSize += tracer.Field("num_subpic_ref_idx", 15, this.num_subpic_ref_idx); 

		for (int i = 0; i < num_subpic_ref_idx; i++)
		{
			boxSize += tracer.Field($"subp_track_ref_idx[{i}]", 16, this.subp_track_ref_idx[i]); 
		}

		if (subpic_id_info_flag)
		{
			boxSize += tracer.Field("subpic_id_rewriting_info", IsoStream.CalculateClassSize(subpic_id_rewriting_info), this.subpic_id_rewriting_info); 
		}
		return boxSize;
	}
}

}
