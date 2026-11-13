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
aligned(8) class SpatialAudioBox extends Box('SA3D') {
	unsigned int(8)  version;
	unsigned int(8)  ambisonic_type;
	unsigned int(32) ambisonic_order;
	unsigned int(8)  ambisonic_channel_ordering;
	unsigned int(8)  ambisonic_normalization;
	unsigned int(32) num_channels;
	for (i = 0; i < num_channels; i++) {
		unsigned int(32) channel_map;
	}
}
*/
public partial class SpatialAudioBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class SpatialAudioBox extends Box('SA3D') {
	unsigned int(8)  version;
	unsigned int(8)  ambisonic_type;
	unsigned int(32) ambisonic_order;
	unsigned int(8)  ambisonic_channel_ordering;
	unsigned int(8)  ambisonic_normalization;
	unsigned int(32) num_channels;
	for (i = 0; i < num_channels; i++) {
		unsigned int(32) channel_map;
	}
}"; } }
	public const string TYPE = "SA3D";
	public override string DisplayName { get { return "SpatialAudioBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry"); }


	protected byte version; 
	public byte Version { get { return this.version; } set { this.version = value; } }

	protected byte ambisonic_type; 
	public byte AmbisonicType { get { return this.ambisonic_type; } set { this.ambisonic_type = value; } }

	protected uint ambisonic_order; 
	public uint AmbisonicOrder { get { return this.ambisonic_order; } set { this.ambisonic_order = value; } }

	protected byte ambisonic_channel_ordering; 
	public byte AmbisonicChannelOrdering { get { return this.ambisonic_channel_ordering; } set { this.ambisonic_channel_ordering = value; } }

	protected byte ambisonic_normalization; 
	public byte AmbisonicNormalization { get { return this.ambisonic_normalization; } set { this.ambisonic_normalization = value; } }

	protected uint num_channels; 
	public uint NumChannels { get { return this.num_channels; } set { this.num_channels = value; } }

	protected uint[] channel_map; 
	public uint[] ChannelMap { get { return this.channel_map; } set { this.channel_map = value; } }

	public SpatialAudioBox(): base(IsoStream.FromFourCC("SA3D"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ambisonic_type, "ambisonic_type"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ambisonic_order, "ambisonic_order"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ambisonic_channel_ordering, "ambisonic_channel_ordering"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.ambisonic_normalization, "ambisonic_normalization"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_channels, "num_channels"); 

		this.channel_map = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( num_channels), "channel_map");
		for (int i = 0; i < num_channels; i++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.channel_map[i], "channel_map"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.version, "version"); 
		boxSize += stream.WriteUInt8( this.ambisonic_type, "ambisonic_type"); 
		boxSize += stream.WriteUInt32( this.ambisonic_order, "ambisonic_order"); 
		boxSize += stream.WriteUInt8( this.ambisonic_channel_ordering, "ambisonic_channel_ordering"); 
		boxSize += stream.WriteUInt8( this.ambisonic_normalization, "ambisonic_normalization"); 
		boxSize += stream.WriteUInt32( this.num_channels, "num_channels"); 

		for (int i = 0; i < num_channels; i++)
		{
			boxSize += stream.WriteUInt32( this.channel_map[i], "channel_map"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // version
		boxSize += 8; // ambisonic_type
		boxSize += 32; // ambisonic_order
		boxSize += 8; // ambisonic_channel_ordering
		boxSize += 8; // ambisonic_normalization
		boxSize += 32; // num_channels

		for (int i = 0; i < num_channels; i++)
		{
			boxSize += 32; // channel_map
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("version", 8, this.version); 
		boxSize += tracer.Field("ambisonic_type", 8, this.ambisonic_type); 
		boxSize += tracer.Field("ambisonic_order", 32, this.ambisonic_order); 
		boxSize += tracer.Field("ambisonic_channel_ordering", 8, this.ambisonic_channel_ordering); 
		boxSize += tracer.Field("ambisonic_normalization", 8, this.ambisonic_normalization); 
		boxSize += tracer.Field("num_channels", 32, this.num_channels); 

		for (int i = 0; i < num_channels; i++)
		{
			boxSize += tracer.Field($"channel_map[{i}]", 32, this.channel_map[i]); 
		}
		return boxSize;
	}
}

}
