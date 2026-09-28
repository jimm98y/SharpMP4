using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23008-12 image pyramid entity group, as libheif's Box_pymd reads and writes it (libheif/box.cc)
aligned(8) class ImagePyramidEntityGroupBox extends EntityToGroupBox('pymd',0,0)
{
	unsigned int(16) tile_size_x;
	unsigned int(16) tile_size_y;
	for (i = 0; i < num_entities_in_group; i++) {
		unsigned int(16) layer_binning[i];
		unsigned int(16) tiles_in_layer_row_minus1[i];
		unsigned int(16) tiles_in_layer_column_minus1[i];
	}
}
*/
public partial class ImagePyramidEntityGroupBox : EntityToGroupBox
{
	public const string TYPE = "pymd";
	public override string DisplayName { get { return "ImagePyramidEntityGroupBox"; } }

	protected ushort tile_size_x; 
	public ushort TileSizex { get { return this.tile_size_x; } set { this.tile_size_x = value; } }

	protected ushort tile_size_y; 
	public ushort TileSizey { get { return this.tile_size_y; } set { this.tile_size_y = value; } }

	protected ushort[] layer_binning; 
	public ushort[] LayerBinning { get { return this.layer_binning; } set { this.layer_binning = value; } }

	protected ushort[] tiles_in_layer_row_minus1; 
	public ushort[] TilesInLayerRowMinus1 { get { return this.tiles_in_layer_row_minus1; } set { this.tiles_in_layer_row_minus1 = value; } }

	protected ushort[] tiles_in_layer_column_minus1; 
	public ushort[] TilesInLayerColumnMinus1 { get { return this.tiles_in_layer_column_minus1; } set { this.tiles_in_layer_column_minus1 = value; } }

	public ImagePyramidEntityGroupBox(): base(IsoStream.FromFourCC("pymd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tile_size_x, "tile_size_x"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tile_size_y, "tile_size_y"); 

		this.layer_binning = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_entities_in_group), "layer_binning");
		this.tiles_in_layer_row_minus1 = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_entities_in_group), "tiles_in_layer_row_minus1");
		this.tiles_in_layer_column_minus1 = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_entities_in_group), "tiles_in_layer_column_minus1");
		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.layer_binning[i], "layer_binning"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tiles_in_layer_row_minus1[i], "tiles_in_layer_row_minus1"); 
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.tiles_in_layer_column_minus1[i], "tiles_in_layer_column_minus1"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.tile_size_x, "tile_size_x"); 
		boxSize += stream.WriteUInt16( this.tile_size_y, "tile_size_y"); 

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += stream.WriteUInt16( this.layer_binning[i], "layer_binning"); 
			boxSize += stream.WriteUInt16( this.tiles_in_layer_row_minus1[i], "tiles_in_layer_row_minus1"); 
			boxSize += stream.WriteUInt16( this.tiles_in_layer_column_minus1[i], "tiles_in_layer_column_minus1"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // tile_size_x
		boxSize += 16; // tile_size_y

		for (int i = 0; i < num_entities_in_group; i++)
		{
			boxSize += 16; // layer_binning
			boxSize += 16; // tiles_in_layer_row_minus1
			boxSize += 16; // tiles_in_layer_column_minus1
		}
		return boxSize;
	}
}

}
