using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
	public const string TYPE = "SA3D";
	public override string DisplayName { get { return "SpatialAudioBox"; } }

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

		this.channel_map = new uint[IsoStream.GetInt( num_channels)];
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
}

}
