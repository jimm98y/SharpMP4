using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PixelInformationProperty
extends ItemFullProperty('pixi', version = 0, flags = 0){
	unsigned int(8) num_channels;
	for (i=0; i<num_channels; i++) {
		unsigned int(8) bits_per_channel;
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

	public PixelInformationProperty(): base(IsoStream.FromFourCC("pixi"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.num_channels, "num_channels"); 

		this.bits_per_channel = new byte[IsoStream.GetInt(num_channels)];
		for (int i=0; i<num_channels; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.bits_per_channel[i], "bits_per_channel"); 
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
		return boxSize;
	}
}

}
