using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleLoopingBox() extends Box('LOOP') {
	 unsigned int(32) data; // 0 normal, 1 palindromic
 } 
*/
public partial class AppleLoopingBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleLoopingBox() extends Box('LOOP') {
	 unsigned int(32) data; // 0 normal, 1 palindromic
 } "; } }
	public const string TYPE = "LOOP";
	public override string DisplayName { get { return "AppleLoopingBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected uint data;  //  0 normal, 1 palindromic
	public uint Data { get { return this.data; } set { this.data = value; } }

	public AppleLoopingBox(): base(IsoStream.FromFourCC("LOOP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data, "data"); // 0 normal, 1 palindromic
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.data, "data"); // 0 normal, 1 palindromic
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
		boxSize += tracer.Field("data", 32, this.data); // 0 normal, 1 palindromic
		return boxSize;
	}
}

}
