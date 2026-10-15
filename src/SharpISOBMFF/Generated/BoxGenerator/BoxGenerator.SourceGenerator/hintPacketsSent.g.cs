using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class hintPacketsSent extends Box('nump') {
	uint(64)	packetssent; }	// total packets sent

*/
public partial class hintPacketsSent : Box
{
	public const string TYPE = "nump";
	public override string DisplayName { get { return "hintPacketsSent"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected ulong packetssent; 
	public ulong Packetssent { get { return this.packetssent; } set { this.packetssent = value; } }

	public hintPacketsSent(): base(IsoStream.FromFourCC("nump"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.packetssent, "packetssent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // packetssent
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("packetssent", 64, this.packetssent); 
		return boxSize;
	}
}

}
