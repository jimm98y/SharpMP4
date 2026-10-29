using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintLargestPacket extends Box('pmax') {
	uint(32)	bytes; }			// largest packet sent, including RTP header

*/
public partial class HintLargestPacket : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintLargestPacket extends Box('pmax') {
	uint(32)	bytes; }			// largest packet sent, including RTP header
"; } }
	public const string TYPE = "pmax";
	public override string DisplayName { get { return "HintLargestPacket"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected uint bytes; 
	public uint Bytes { get { return this.bytes; } set { this.bytes = value; } }

	public HintLargestPacket(): base(IsoStream.FromFourCC("pmax"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.bytes, "bytes"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // bytes
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("bytes", 32, this.bytes); 
		return boxSize;
	}
}

}
