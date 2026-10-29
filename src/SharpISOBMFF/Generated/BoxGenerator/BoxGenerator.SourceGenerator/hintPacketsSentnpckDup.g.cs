using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class hintPacketsSent extends Box('npck') {
	uint(32)	packetssent; }	// total packets sent
*/
public partial class hintPacketsSentnpckDup : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class hintPacketsSent extends Box('npck') {
	uint(32)	packetssent; }	// total packets sent"; } }
	public const string TYPE = "npck";
	public override string DisplayName { get { return "hintPacketsSentnpckDup"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected uint packetssent; 
	public uint Packetssent { get { return this.packetssent; } set { this.packetssent = value; } }

	public hintPacketsSentnpckDup(): base(IsoStream.FromFourCC("npck"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // packetssent
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("packetssent", 32, this.packetssent); 
		return boxSize;
	}
}

}
