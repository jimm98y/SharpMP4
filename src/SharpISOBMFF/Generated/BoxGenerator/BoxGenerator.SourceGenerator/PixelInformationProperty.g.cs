using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PixelInformationProperty
extends ItemFullProperty('pixi', version = 0, flags){
	unsigned int(8) num_channels;
	for (i=0; i<num_channels; i++) {
		unsigned int(8) bits_per_channel;
	}
	if (flags & 1) {
		for (i=0; i<num_channels; i++) {
			unsigned int(3) channel_idc;
			unsigned int(1) reserved = 0;
			unsigned int(2) component_format;
			unsigned int(1) subsampling_flag;
			unsigned int(1) channel_label_flag;
			if (subsampling_flag) {
				unsigned int(4) subsampling_type;
				unsigned int(4) subsampling_location;
			}
			if (channel_label_flag) {
				utf8string channel_label;
			}
		}
	}
}
*/
public partial class PixelInformationProperty : ItemFullProperty
{
	public const string TYPE = "pixi";
	public override string DisplayName { get { return "PixelInformationProperty"; } }

	protected byte num_channels; 
	public byte NumChannels { get { return this.num_channels; } set { this.num_channels = value; } }

	protected byte[] bits_per_channel; 
	public byte[] BitsPerChannel { get { return this.bits_per_channel; } set { this.bits_per_channel = value; } }

	protected byte[] channel_idc; 
	public byte[] ChannelIdc { get { return this.channel_idc; } set { this.channel_idc = value; } }

	protected bool[] reserved; 
	public bool[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte[] component_format; 
	public byte[] ComponentFormat { get { return this.component_format; } set { this.component_format = value; } }

	protected bool[] subsampling_flag; 
	public bool[] SubsamplingFlag { get { return this.subsampling_flag; } set { this.subsampling_flag = value; } }

	protected bool[] channel_label_flag; 
	public bool[] ChannelLabelFlag { get { return this.channel_label_flag; } set { this.channel_label_flag = value; } }

	protected byte[] subsampling_type; 
	public byte[] SubsamplingType { get { return this.subsampling_type; } set { this.subsampling_type = value; } }

	protected byte[] subsampling_location; 
	public byte[] SubsamplingLocation { get { return this.subsampling_location; } set { this.subsampling_location = value; } }

	protected BinaryUTF8String[] channel_label; 
	public BinaryUTF8String[] ChannelLabel { get { return this.channel_label; } set { this.channel_label = value; } }

	public PixelInformationProperty(uint flags = 0): base(IsoStream.FromFourCC("pixi"), 0, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_channels, "num_channels"); 

		this.bits_per_channel = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_channels), "bits_per_channel");
		for (int i=0; i<num_channels; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.bits_per_channel[i], "bits_per_channel"); 
		}

		if ((flags  &  1) ==  1)
		{

			this.channel_idc = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_channels), "channel_idc");
			this.reserved = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_channels), "reserved");
			this.component_format = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_channels), "component_format");
			this.subsampling_flag = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_channels), "subsampling_flag");
			this.channel_label_flag = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt(num_channels), "channel_label_flag");
			this.subsampling_type = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_channels), "subsampling_type");
			this.subsampling_location = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt(num_channels), "subsampling_location");
			this.channel_label = stream.SafeAllocate<BinaryUTF8String>(boxSize, readSize, IsoStream.GetInt(num_channels), "channel_label");
			for (int i=0; i<num_channels; i++)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.channel_idc[i], "channel_idc"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved[i], "reserved"); 
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.component_format[i], "component_format"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.subsampling_flag[i], "subsampling_flag"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.channel_label_flag[i], "channel_label_flag"); 

				if (subsampling_flag[i])
				{
					boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.subsampling_type[i], "subsampling_type"); 
					boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.subsampling_location[i], "subsampling_location"); 
				}

				if (channel_label_flag[i])
				{
					boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.channel_label[i], "channel_label"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.num_channels, "num_channels"); 

		for (int i=0; i<num_channels; i++)
		{
			boxSize += stream.WriteUInt8( this.bits_per_channel[i], "bits_per_channel"); 
		}

		if ((flags  &  1) ==  1)
		{

			for (int i=0; i<num_channels; i++)
			{
				boxSize += stream.WriteBits(3,  this.channel_idc[i], "channel_idc"); 
				boxSize += stream.WriteBit( this.reserved[i], "reserved"); 
				boxSize += stream.WriteBits(2,  this.component_format[i], "component_format"); 
				boxSize += stream.WriteBit( this.subsampling_flag[i], "subsampling_flag"); 
				boxSize += stream.WriteBit( this.channel_label_flag[i], "channel_label_flag"); 

				if (subsampling_flag[i])
				{
					boxSize += stream.WriteBits(4,  this.subsampling_type[i], "subsampling_type"); 
					boxSize += stream.WriteBits(4,  this.subsampling_location[i], "subsampling_location"); 
				}

				if (channel_label_flag[i])
				{
					boxSize += stream.WriteStringZeroTerminated( this.channel_label[i], "channel_label"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // num_channels

		for (int i=0; i<num_channels; i++)
		{
			boxSize += 8; // bits_per_channel
		}

		if ((flags  &  1) ==  1)
		{

			for (int i=0; i<num_channels; i++)
			{
				boxSize += 3; // channel_idc
				boxSize += 1; // reserved
				boxSize += 2; // component_format
				boxSize += 1; // subsampling_flag
				boxSize += 1; // channel_label_flag

				if (subsampling_flag[i])
				{
					boxSize += 4; // subsampling_type
					boxSize += 4; // subsampling_location
				}

				if (channel_label_flag[i])
				{
					boxSize += IsoStream.CalculateStringSize(channel_label[i]); // channel_label
				}
			}
		}
		return boxSize;
	}
}

}
