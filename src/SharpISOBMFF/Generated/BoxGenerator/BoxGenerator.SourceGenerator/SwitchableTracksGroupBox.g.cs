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
aligned(8) class SwitchableTracksGroupBox extends EntityToGroupBox('swtk',0,0)
{
	for (i = 0; i < num_entities_in_group; i++)
		unsigned int(16) track_switch_hierarchy_id[i];
}
*/
public partial class SwitchableTracksGroupBox : EntityToGroupBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SwitchableTracksGroupBox extends EntityToGroupBox('swtk',0,0)
{
	for (i = 0; i < num_entities_in_group; i++)
		unsigned int(16) track_switch_hierarchy_id[i];
}"; } }
	public const string TYPE = "swtk";
	public override string DisplayName { get { return "SwitchableTracksGroupBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("grpl"); }


	protected ushort[] track_switch_hierarchy_id; 
	public ushort[] TrackSwitchHierarchyId { get { return this.track_switch_hierarchy_id; } set { this.track_switch_hierarchy_id = value; } }

	public SwitchableTracksGroupBox(): base(IsoStream.FromFourCC("swtk"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		this.track_switch_hierarchy_id = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_entities_in_group), "track_switch_hierarchy_id");
		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.track_switch_hierarchy_id[i], "track_switch_hierarchy_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.WriteUInt16( this.track_switch_hierarchy_id[i], "track_switch_hierarchy_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += 16; // track_switch_hierarchy_id
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += tracer.Field($"track_switch_hierarchy_id[{i}]", 16, this.track_switch_hierarchy_id[i]); 
		}
		return boxSize;
	}
}

}
