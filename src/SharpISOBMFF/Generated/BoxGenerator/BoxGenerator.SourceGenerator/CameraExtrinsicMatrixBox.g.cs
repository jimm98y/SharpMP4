using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23008-12 camera extrinsic matrix: the camera's position, in micrometres, and its orientation, a unit quaternion of its x, y and z - of 2^-30 in 32 bits, of 2^-14 in 16 - each where its flag says it is there: 0x1, 0x2 and 0x4 the position, 0x8 the orientation, 0x10 of 32 bits, 0x20 the world coordinate system
aligned(8) class CameraExtrinsicMatrixBox() extends FullBox('cmex') {
	 if( flags & 0x1 ) { 
 signed int(32) pos_x;
 }
 if( flags & 0x2 ) { 
 signed int(32) pos_y;
 }
 if( flags & 0x4 ) { 
 signed int(32) pos_z;
 }
 if( flags & 0x8 ) { 
 if( flags & 0x10 ) { 
 signed int(32) quat_x;
 signed int(32) quat_y;
 signed int(32) quat_z;
 }
 else { 
 signed int(16) quat_x_short;
 signed int(16) quat_y_short;
 signed int(16) quat_z_short;
 }
 }
 if( flags & 0x20 ) { 
 unsigned int(32) world_coordinate_system_id;
 }
 } 
*/
public partial class CameraExtrinsicMatrixBox : FullBox
{
	public const string TYPE = "cmex";
	public override string DisplayName { get { return "CameraExtrinsicMatrixBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected int pos_x; 
	public int Posx { get { return this.pos_x; } set { this.pos_x = value; } }

	protected int pos_y; 
	public int Posy { get { return this.pos_y; } set { this.pos_y = value; } }

	protected int pos_z; 
	public int Posz { get { return this.pos_z; } set { this.pos_z = value; } }

	protected int quat_x; 
	public int Quatx { get { return this.quat_x; } set { this.quat_x = value; } }

	protected int quat_y; 
	public int Quaty { get { return this.quat_y; } set { this.quat_y = value; } }

	protected int quat_z; 
	public int Quatz { get { return this.quat_z; } set { this.quat_z = value; } }

	protected short quat_x_short; 
	public short QuatxShort { get { return this.quat_x_short; } set { this.quat_x_short = value; } }

	protected short quat_y_short; 
	public short QuatyShort { get { return this.quat_y_short; } set { this.quat_y_short = value; } }

	protected short quat_z_short; 
	public short QuatzShort { get { return this.quat_z_short; } set { this.quat_z_short = value; } }

	protected uint world_coordinate_system_id; 
	public uint WorldCoordinateSystemId { get { return this.world_coordinate_system_id; } set { this.world_coordinate_system_id = value; } }

	public CameraExtrinsicMatrixBox(): base(IsoStream.FromFourCC("cmex"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.pos_x, "pos_x"); 
		}

		if (( flags  &  0x2 ) ==  0x2 )
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.pos_y, "pos_y"); 
		}

		if (( flags  &  0x4 ) ==  0x4 )
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.pos_z, "pos_z"); 
		}

		if (( flags  &  0x8 ) ==  0x8 )
		{

			if (( flags  &  0x10 ) ==  0x10 )
			{
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.quat_x, "quat_x"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.quat_y, "quat_y"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.quat_z, "quat_z"); 
			}

			else 
			{
				boxSize += stream.ReadInt16(boxSize, readSize,  out this.quat_x_short, "quat_x_short"); 
				boxSize += stream.ReadInt16(boxSize, readSize,  out this.quat_y_short, "quat_y_short"); 
				boxSize += stream.ReadInt16(boxSize, readSize,  out this.quat_z_short, "quat_z_short"); 
			}
		}

		if (( flags  &  0x20 ) ==  0x20 )
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.world_coordinate_system_id, "world_coordinate_system_id"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += stream.WriteInt32( this.pos_x, "pos_x"); 
		}

		if (( flags  &  0x2 ) ==  0x2 )
		{
			boxSize += stream.WriteInt32( this.pos_y, "pos_y"); 
		}

		if (( flags  &  0x4 ) ==  0x4 )
		{
			boxSize += stream.WriteInt32( this.pos_z, "pos_z"); 
		}

		if (( flags  &  0x8 ) ==  0x8 )
		{

			if (( flags  &  0x10 ) ==  0x10 )
			{
				boxSize += stream.WriteInt32( this.quat_x, "quat_x"); 
				boxSize += stream.WriteInt32( this.quat_y, "quat_y"); 
				boxSize += stream.WriteInt32( this.quat_z, "quat_z"); 
			}

			else 
			{
				boxSize += stream.WriteInt16( this.quat_x_short, "quat_x_short"); 
				boxSize += stream.WriteInt16( this.quat_y_short, "quat_y_short"); 
				boxSize += stream.WriteInt16( this.quat_z_short, "quat_z_short"); 
			}
		}

		if (( flags  &  0x20 ) ==  0x20 )
		{
			boxSize += stream.WriteUInt32( this.world_coordinate_system_id, "world_coordinate_system_id"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += 32; // pos_x
		}

		if (( flags  &  0x2 ) ==  0x2 )
		{
			boxSize += 32; // pos_y
		}

		if (( flags  &  0x4 ) ==  0x4 )
		{
			boxSize += 32; // pos_z
		}

		if (( flags  &  0x8 ) ==  0x8 )
		{

			if (( flags  &  0x10 ) ==  0x10 )
			{
				boxSize += 32; // quat_x
				boxSize += 32; // quat_y
				boxSize += 32; // quat_z
			}

			else 
			{
				boxSize += 16; // quat_x_short
				boxSize += 16; // quat_y_short
				boxSize += 16; // quat_z_short
			}
		}

		if (( flags  &  0x20 ) ==  0x20 )
		{
			boxSize += 32; // world_coordinate_system_id
		}
		return boxSize;
	}
}

}
