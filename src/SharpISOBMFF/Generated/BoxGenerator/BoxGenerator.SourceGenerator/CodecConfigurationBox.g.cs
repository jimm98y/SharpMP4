using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CodecConfigurationBox(unsigned int(32) format) extends Box(format) {
	bit(8) data[];
}
*/
public partial class CodecConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CodecConfigurationBox(unsigned int(32) format) extends Box(format) {
	bit(8) data[];
}"; } }
	public override string DisplayName { get { return "CodecConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "SampleEntry", "wave" }); }


	protected byte[] data = []; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public CodecConfigurationBox(uint format): base(format)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("data", ((ulong)data.Length * 8), this.data); 
		return boxSize;
	}
}

}
