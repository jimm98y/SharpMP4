using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintLongestPacket extends Box('dmax') {
	uint(32)	time; }			// longest packet duration, milliseconds
*/
public partial class HintLongestPacket : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintLongestPacket extends Box('dmax') {
	uint(32)	time; }			// longest packet duration, milliseconds"; } }
	public const string TYPE = "dmax";
	public override string DisplayName { get { return "HintLongestPacket"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected uint time; 
	public uint Time { get { return this.time; } set { this.time = value; } }

	public HintLongestPacket(): base(IsoStream.FromFourCC("dmax"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.time, "time"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.time, "time"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // time
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("time", 32, this.time); 
		return boxSize;
	}
}

}
