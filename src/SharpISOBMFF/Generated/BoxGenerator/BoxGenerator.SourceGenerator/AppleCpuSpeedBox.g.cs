using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AppleCpuSpeedBox() extends Box ('rmcs'){
 unsigned int(32) flags;
 unsigned int(32) cpuSpeed;
 }
*/
public partial class AppleCpuSpeedBox : Box
{
	public const string TYPE = "rmcs";
	public override string DisplayName { get { return "AppleCpuSpeedBox"; } }

	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	protected uint cpuSpeed; 
	public uint CpuSpeed { get { return this.cpuSpeed; } set { this.cpuSpeed = value; } }

	public AppleCpuSpeedBox(): base(IsoStream.FromFourCC("rmcs"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cpuSpeed, "cpuSpeed"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.flags, "flags"); 
		boxSize += stream.WriteUInt32( this.cpuSpeed, "cpuSpeed"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // flags
		boxSize += 32; // cpuSpeed
		return boxSize;
	}
}

}
