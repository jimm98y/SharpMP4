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
aligned(8) class OpusSpecificBox extends Box('dOps'){
	unsigned int(8) Version;
	unsigned int(8) OutputChannelCount;
	unsigned int(16) PreSkip;
	unsigned int(32) InputSampleRate;
	signed int(16) OutputGain;
	unsigned int(8) ChannelMappingFamily;
	if (ChannelMappingFamily != 0) {
		ChannelMappingTable(OutputChannelCount);
	}
}
*/
public partial class OpusSpecificBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OpusSpecificBox extends Box('dOps'){
	unsigned int(8) Version;
	unsigned int(8) OutputChannelCount;
	unsigned int(16) PreSkip;
	unsigned int(32) InputSampleRate;
	signed int(16) OutputGain;
	unsigned int(8) ChannelMappingFamily;
	if (ChannelMappingFamily != 0) {
		ChannelMappingTable(OutputChannelCount);
	}
}"; } }
	public const string TYPE = "dOps";
	public override string DisplayName { get { return "OpusSpecificBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(Opus,enca)"); }


	protected byte Version; 
	public byte _Version { get { return this.Version; } set { this.Version = value; } }

	protected byte OutputChannelCount; 
	public byte _OutputChannelCount { get { return this.OutputChannelCount; } set { this.OutputChannelCount = value; } }

	protected ushort PreSkip; 
	public ushort _PreSkip { get { return this.PreSkip; } set { this.PreSkip = value; } }

	protected uint InputSampleRate; 
	public uint _InputSampleRate { get { return this.InputSampleRate; } set { this.InputSampleRate = value; } }

	protected short OutputGain; 
	public short _OutputGain { get { return this.OutputGain; } set { this.OutputGain = value; } }

	protected byte ChannelMappingFamily; 
	public byte _ChannelMappingFamily { get { return this.ChannelMappingFamily; } set { this.ChannelMappingFamily = value; } }

	protected ChannelMappingTable ChannelMappingTable; 
	public ChannelMappingTable _ChannelMappingTable { get { return this.ChannelMappingTable; } set { this.ChannelMappingTable = value; } }

	public OpusSpecificBox(): base(IsoStream.FromFourCC("dOps"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.Version, "Version"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.OutputChannelCount, "OutputChannelCount"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.PreSkip, "PreSkip"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.InputSampleRate, "InputSampleRate"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.OutputGain, "OutputGain"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ChannelMappingFamily, "ChannelMappingFamily"); 

		if (ChannelMappingFamily != 0)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ChannelMappingTable(OutputChannelCount),  out this.ChannelMappingTable, "ChannelMappingTable"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.Version, "Version"); 
		boxSize += stream.WriteUInt8( this.OutputChannelCount, "OutputChannelCount"); 
		boxSize += stream.WriteUInt16( this.PreSkip, "PreSkip"); 
		boxSize += stream.WriteUInt32( this.InputSampleRate, "InputSampleRate"); 
		boxSize += stream.WriteInt16( this.OutputGain, "OutputGain"); 
		boxSize += stream.WriteUInt8( this.ChannelMappingFamily, "ChannelMappingFamily"); 

		if (ChannelMappingFamily != 0)
		{
			boxSize += stream.WriteClass( this.ChannelMappingTable, "ChannelMappingTable"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // Version
		boxSize += 8; // OutputChannelCount
		boxSize += 16; // PreSkip
		boxSize += 32; // InputSampleRate
		boxSize += 16; // OutputGain
		boxSize += 8; // ChannelMappingFamily

		if (ChannelMappingFamily != 0)
		{
			boxSize += IsoStream.CalculateClassSize(ChannelMappingTable); // ChannelMappingTable
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("Version", 8, this.Version); 
		boxSize += tracer.Field("OutputChannelCount", 8, this.OutputChannelCount); 
		boxSize += tracer.Field("PreSkip", 16, this.PreSkip); 
		boxSize += tracer.Field("InputSampleRate", 32, this.InputSampleRate); 
		boxSize += tracer.Field("OutputGain", 16, this.OutputGain); 
		boxSize += tracer.Field("ChannelMappingFamily", 8, this.ChannelMappingFamily); 

		if (ChannelMappingFamily != 0)
		{
			boxSize += tracer.Field("ChannelMappingTable", IsoStream.CalculateClassSize(ChannelMappingTable), this.ChannelMappingTable); 
		}
		return boxSize;
	}
}

}
