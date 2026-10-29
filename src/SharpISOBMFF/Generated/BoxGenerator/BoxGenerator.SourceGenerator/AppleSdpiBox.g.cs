using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleSdpiBox extends Box('sdpi') {
 unsigned int(32) data; //TODO 
}

*/
public partial class AppleSdpiBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleSdpiBox extends Box('sdpi') {
 unsigned int(32) data; //TODO 
}
"; } }
	public const string TYPE = "sdpi";
	public override string DisplayName { get { return "AppleSdpiBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("sdpd"); }


	protected uint data;  // TODO 
	public uint Data { get { return this.data; } set { this.data = value; } }

	public AppleSdpiBox(): base(IsoStream.FromFourCC("sdpi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data, "data"); //TODO 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.data, "data"); //TODO 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("data", 32, this.data); //TODO 
		return boxSize;
	}
}

}
