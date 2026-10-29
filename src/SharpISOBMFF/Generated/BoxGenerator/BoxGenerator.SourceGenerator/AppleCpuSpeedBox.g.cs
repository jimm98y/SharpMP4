using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleCpuSpeedBox() extends Box ('rmcs'){
	unsigned int(32) flags;
	unsigned int(32) cpuSpeed;
}"; } }
	public const string TYPE = "rmcs";
	public override string DisplayName { get { return "AppleCpuSpeedBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("rmda"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("flags", 32, this.flags); 
		boxSize += tracer.Field("cpuSpeed", 32, this.cpuSpeed); 
		return boxSize;
	}
}

}
