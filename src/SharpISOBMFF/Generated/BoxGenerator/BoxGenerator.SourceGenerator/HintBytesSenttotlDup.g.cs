using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HintBytesSent extends Box('totl') {
	uint(32)	bytessent; }	// total bytes sent, including 12-byte RTP headers
*/
public partial class HintBytesSenttotlDup : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class HintBytesSent extends Box('totl') {
	uint(32)	bytessent; }	// total bytes sent, including 12-byte RTP headers"; } }
	public const string TYPE = "totl";
	public override string DisplayName { get { return "HintBytesSenttotlDup"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hinf"); }


	protected uint bytessent; 
	public uint Bytessent { get { return this.bytessent; } set { this.bytessent = value; } }

	public HintBytesSenttotlDup(): base(IsoStream.FromFourCC("totl"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.bytessent, "bytessent"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // bytessent
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("bytessent", 32, this.bytessent); 
		return boxSize;
	}
}

}
