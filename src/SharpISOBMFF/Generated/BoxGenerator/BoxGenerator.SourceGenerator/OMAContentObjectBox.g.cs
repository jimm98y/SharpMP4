using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMAContentObjectBox() extends FullBox('odda') {
	 unsigned int(32) count;
 unsigned int(8) data[count];
 } 
*/
public partial class OMAContentObjectBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OMAContentObjectBox() extends FullBox('odda') {
	 unsigned int(32) count;
 unsigned int(8) data[count];
 } "; } }
	public const string TYPE = "odda";
	public override string DisplayName { get { return "OMAContentObjectBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("odrm"); }


	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected byte[] data = []; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public OMAContentObjectBox(): base(IsoStream.FromFourCC("odda"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // count
		boxSize += ((ulong)(count) * 8); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("count", 32, this.count); 
		boxSize += tracer.Field("data", ((ulong)(count) * 8), this.data); 
		return boxSize;
	}
}

}
