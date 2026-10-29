using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Apple's camera extrinsic matrix, of the layout of 'cmex', as an iPhone's spatial photo has it of each eye
aligned(8) class UuidCameraExtrinsicMatrixBox() extends FullBox('uuid 4363e9145b7d4aab97aebea69803b434') {
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
public partial class UuidCameraExtrinsicMatrixBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Apple's camera extrinsic matrix, of the layout of 'cmex', as an iPhone's spatial photo has it of each eye
aligned(8) class UuidCameraExtrinsicMatrixBox() extends FullBox('uuid 4363e9145b7d4aab97aebea69803b434') {
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
 } "; } }
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "UuidCameraExtrinsicMatrixBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


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

	public UuidCameraExtrinsicMatrixBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("4363e9145b7d4aab97aebea69803b434"))
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);

		if (( flags  &  0x1 ) ==  0x1 )
		{
			boxSize += tracer.Field("pos_x", 32, this.pos_x); 
		}

		if (( flags  &  0x2 ) ==  0x2 )
		{
			boxSize += tracer.Field("pos_y", 32, this.pos_y); 
		}

		if (( flags  &  0x4 ) ==  0x4 )
		{
			boxSize += tracer.Field("pos_z", 32, this.pos_z); 
		}

		if (( flags  &  0x8 ) ==  0x8 )
		{

			if (( flags  &  0x10 ) ==  0x10 )
			{
				boxSize += tracer.Field("quat_x", 32, this.quat_x); 
				boxSize += tracer.Field("quat_y", 32, this.quat_y); 
				boxSize += tracer.Field("quat_z", 32, this.quat_z); 
			}

			else 
			{
				boxSize += tracer.Field("quat_x_short", 16, this.quat_x_short); 
				boxSize += tracer.Field("quat_y_short", 16, this.quat_y_short); 
				boxSize += tracer.Field("quat_z_short", 16, this.quat_z_short); 
			}
		}

		if (( flags  &  0x20 ) ==  0x20 )
		{
			boxSize += tracer.Field("world_coordinate_system_id", 32, this.world_coordinate_system_id); 
		}
		return boxSize;
	}
}

}
