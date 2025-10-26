using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ProjectionHeader extends FullBox('prhd', 0, 0) {
    int(32) pose_yaw_degrees;
    int(32) pose_pitch_degrees;
    int(32) pose_roll_degrees;
 }

*/
public partial class ProjectionHeader : FullBox
{
	public const string TYPE = "prhd";
	public override string DisplayName { get { return "ProjectionHeader"; } }

	protected int pose_yaw_degrees; 
	public int PoseYawDegrees { get { return this.pose_yaw_degrees; } set { this.pose_yaw_degrees = value; } }

	protected int pose_pitch_degrees; 
	public int PosePitchDegrees { get { return this.pose_pitch_degrees; } set { this.pose_pitch_degrees = value; } }

	protected int pose_roll_degrees; 
	public int PoseRollDegrees { get { return this.pose_roll_degrees; } set { this.pose_roll_degrees = value; } }

	public ProjectionHeader(): base(IsoStream.FromFourCC("prhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.pose_yaw_degrees, "pose_yaw_degrees"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.pose_pitch_degrees, "pose_pitch_degrees"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.pose_roll_degrees, "pose_roll_degrees"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.pose_yaw_degrees, "pose_yaw_degrees"); 
		boxSize += stream.WriteInt32( this.pose_pitch_degrees, "pose_pitch_degrees"); 
		boxSize += stream.WriteInt32( this.pose_roll_degrees, "pose_roll_degrees"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // pose_yaw_degrees
		boxSize += 32; // pose_pitch_degrees
		boxSize += 32; // pose_roll_degrees
		return boxSize;
	}
}

}
