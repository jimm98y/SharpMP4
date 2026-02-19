using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ImageSpatialExtentsProperty
extends ItemFullProperty('ispe', version = 0, flags = 0) {
	unsigned int(32) image_width;
	unsigned int(32) image_height;
}

*/
public partial class ImageSpatialExtentsProperty : ItemFullProperty
{
	public const string TYPE = "ispe";
	public override string DisplayName { get { return "ImageSpatialExtentsProperty"; } }

	protected uint image_width; 
	public uint ImageWidth { get { return this.image_width; } set { this.image_width = value; } }

	protected uint image_height; 
	public uint ImageHeight { get { return this.image_height; } set { this.image_height = value; } }

	public ImageSpatialExtentsProperty(): base(IsoStream.FromFourCC("ispe"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.image_width, "image_width"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.image_height, "image_height"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.image_width, "image_width"); 
		boxSize += stream.WriteUInt32( this.image_height, "image_height"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // image_width
		boxSize += 32; // image_height
		return boxSize;
	}
}

}
