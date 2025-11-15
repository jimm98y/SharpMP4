using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MeshBox extends Box('mesh') {
    const unsigned int(1) reserved = 0;
    unsigned int(31) coordinate_count;
     for (i = 0; i < coordinate_count; i++) {
      float(32) coordinate;
    }
    const unsigned int(1) reserved = 0;
    unsigned int(31) vertex_count;
    for (i = 0; i < vertex_count; i++) {
      unsigned int(ceil(log2(coordinate_count * 2))) x_index_delta;
      unsigned int(ceil(log2(coordinate_count * 2))) y_index_delta;
      unsigned int(ceil(log2(coordinate_count * 2))) z_index_delta;
      unsigned int(ceil(log2(coordinate_count * 2))) u_index_delta;
      unsigned int(ceil(log2(coordinate_count * 2))) v_index_delta;
    }
    const unsigned int(1) mesh_padding;

    const unsigned int(1) reserved = 0;
    unsigned int(31) vertex_list_count;
    for (i = 0; i < vertex_list_count; i++) {
      unsigned int(8) texture_id;
      unsigned int(8) index_type;
      const unsigned int(1) reserved = 0;
      unsigned int(31) index_count;
      for (j = 0; j < index_count; j++) {
        unsigned int(ceil(log2(vertex_count * 2))) index_as_delta;
      }
      const unsigned int(1) mesh_padding2;
    }
}
*/
public partial class MeshBox : Box
{
	public const string TYPE = "mesh";
	public override string DisplayName { get { return "MeshBox"; } }

	protected bool reserved = false; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint coordinate_count; 
	public uint CoordinateCount { get { return this.coordinate_count; } set { this.coordinate_count = value; } }

	protected double[] coordinate; 
	public double[] Coordinate { get { return this.coordinate; } set { this.coordinate = value; } }

	protected bool reserved0 = false; 
	public bool Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected uint vertex_count; 
	public uint VertexCount { get { return this.vertex_count; } set { this.vertex_count = value; } }

	protected byte[][] x_index_delta; 
	public byte[][] xIndexDelta { get { return this.x_index_delta; } set { this.x_index_delta = value; } }

	protected byte[][] y_index_delta; 
	public byte[][] yIndexDelta { get { return this.y_index_delta; } set { this.y_index_delta = value; } }

	protected byte[][] z_index_delta; 
	public byte[][] zIndexDelta { get { return this.z_index_delta; } set { this.z_index_delta = value; } }

	protected byte[][] u_index_delta; 
	public byte[][] uIndexDelta { get { return this.u_index_delta; } set { this.u_index_delta = value; } }

	protected byte[][] v_index_delta; 
	public byte[][] vIndexDelta { get { return this.v_index_delta; } set { this.v_index_delta = value; } }

	protected bool mesh_padding; 
	public bool MeshPadding { get { return this.mesh_padding; } set { this.mesh_padding = value; } }

	protected bool reserved1 = false; 
	public bool Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected uint vertex_list_count; 
	public uint VertexListCount { get { return this.vertex_list_count; } set { this.vertex_list_count = value; } }

	protected byte[] texture_id; 
	public byte[] TextureId { get { return this.texture_id; } set { this.texture_id = value; } }

	protected byte[] index_type; 
	public byte[] IndexType { get { return this.index_type; } set { this.index_type = value; } }

	protected bool[] reserved2; 
	public bool[] Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected uint[] index_count; 
	public uint[] IndexCount { get { return this.index_count; } set { this.index_count = value; } }

	protected byte[][][] index_as_delta; 
	public byte[][][] IndexAsDelta { get { return this.index_as_delta; } set { this.index_as_delta = value; } }

	protected bool[] mesh_padding2; 
	public bool[] MeshPadding2 { get { return this.mesh_padding2; } set { this.mesh_padding2 = value; } }

	public MeshBox(): base(IsoStream.FromFourCC("mesh"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.coordinate_count, "coordinate_count"); 

		this.coordinate = new double[IsoStream.GetInt( coordinate_count)];
		for (int i = 0; i < coordinate_count; i++)
		{
			boxSize += stream.ReadDouble32(boxSize, readSize,  out this.coordinate[i], "coordinate"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.vertex_count, "vertex_count"); 

		this.x_index_delta = new byte[IsoStream.GetInt( vertex_count)][];
		this.y_index_delta = new byte[IsoStream.GetInt( vertex_count)][];
		this.z_index_delta = new byte[IsoStream.GetInt( vertex_count)][];
		this.u_index_delta = new byte[IsoStream.GetInt( vertex_count)][];
		this.v_index_delta = new byte[IsoStream.GetInt( vertex_count)][];
		for (int i = 0; i < vertex_count; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  out this.x_index_delta[i], "x_index_delta"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  out this.y_index_delta[i], "y_index_delta"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  out this.z_index_delta[i], "z_index_delta"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  out this.u_index_delta[i], "u_index_delta"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  out this.v_index_delta[i], "v_index_delta"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.mesh_padding, "mesh_padding"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.vertex_list_count, "vertex_list_count"); 

		this.texture_id = new byte[IsoStream.GetInt( vertex_list_count)];
		this.index_type = new byte[IsoStream.GetInt( vertex_list_count)];
		this.reserved2 = new bool[IsoStream.GetInt( vertex_list_count)];
		this.index_count = new uint[IsoStream.GetInt( vertex_list_count)];
		this.index_as_delta = new byte[IsoStream.GetInt( vertex_list_count)][][];
		this.mesh_padding2 = new bool[IsoStream.GetInt( vertex_list_count)];
		for (int i = 0; i < vertex_list_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.texture_id[i], "texture_id"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.index_type[i], "index_type"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved2[i], "reserved2"); 
			boxSize += stream.ReadBits(boxSize, readSize, 31,  out this.index_count[i], "index_count"); 

			this.index_as_delta[i] = new byte[IsoStream.GetInt( index_count[i])][];
			for (int j = 0; j < index_count[i]; j++)
			{
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(Math.Ceiling(Math.Log2(vertex_count * 2)) ),  out this.index_as_delta[i][j], "index_as_delta"); 
			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.mesh_padding2[i], "mesh_padding2"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		boxSize += stream.WriteBits(31,  this.coordinate_count, "coordinate_count"); 

		for (int i = 0; i < coordinate_count; i++)
		{
			boxSize += stream.WriteDouble32( this.coordinate[i], "coordinate"); 
		}
		boxSize += stream.WriteBit( this.reserved0, "reserved0"); 
		boxSize += stream.WriteBits(31,  this.vertex_count, "vertex_count"); 

		for (int i = 0; i < vertex_count; i++)
		{
			boxSize += stream.WriteBits((uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  this.x_index_delta[i], "x_index_delta"); 
			boxSize += stream.WriteBits((uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  this.y_index_delta[i], "y_index_delta"); 
			boxSize += stream.WriteBits((uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  this.z_index_delta[i], "z_index_delta"); 
			boxSize += stream.WriteBits((uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  this.u_index_delta[i], "u_index_delta"); 
			boxSize += stream.WriteBits((uint)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ),  this.v_index_delta[i], "v_index_delta"); 
		}
		boxSize += stream.WriteBit( this.mesh_padding, "mesh_padding"); 
		boxSize += stream.WriteBit( this.reserved1, "reserved1"); 
		boxSize += stream.WriteBits(31,  this.vertex_list_count, "vertex_list_count"); 

		for (int i = 0; i < vertex_list_count; i++)
		{
			boxSize += stream.WriteUInt8( this.texture_id[i], "texture_id"); 
			boxSize += stream.WriteUInt8( this.index_type[i], "index_type"); 
			boxSize += stream.WriteBit( this.reserved2[i], "reserved2"); 
			boxSize += stream.WriteBits(31,  this.index_count[i], "index_count"); 

			for (int j = 0; j < index_count[i]; j++)
			{
				boxSize += stream.WriteBits((uint)(Math.Ceiling(Math.Log2(vertex_count * 2)) ),  this.index_as_delta[i][j], "index_as_delta"); 
			}
			boxSize += stream.WriteBit( this.mesh_padding2[i], "mesh_padding2"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // reserved
		boxSize += 31; // coordinate_count

		for (int i = 0; i < coordinate_count; i++)
		{
			boxSize += 32; // coordinate
		}
		boxSize += 1; // reserved0
		boxSize += 31; // vertex_count

		for (int i = 0; i < vertex_count; i++)
		{
			boxSize += (ulong)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ); // x_index_delta
			boxSize += (ulong)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ); // y_index_delta
			boxSize += (ulong)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ); // z_index_delta
			boxSize += (ulong)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ); // u_index_delta
			boxSize += (ulong)(Math.Ceiling(Math.Log2(coordinate_count * 2)) ); // v_index_delta
		}
		boxSize += 1; // mesh_padding
		boxSize += 1; // reserved1
		boxSize += 31; // vertex_list_count

		for (int i = 0; i < vertex_list_count; i++)
		{
			boxSize += 8; // texture_id
			boxSize += 8; // index_type
			boxSize += 1; // reserved2
			boxSize += 31; // index_count

			for (int j = 0; j < index_count[i]; j++)
			{
				boxSize += (ulong)(Math.Ceiling(Math.Log2(vertex_count * 2)) ); // index_as_delta
			}
			boxSize += 1; // mesh_padding2
		}
		return boxSize;
	}
}

}
