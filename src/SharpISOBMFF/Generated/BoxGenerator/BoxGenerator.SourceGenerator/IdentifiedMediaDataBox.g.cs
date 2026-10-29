using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class IdentifiedMediaDataBox extends Box('imda') {
	unsigned int(32) imda_identifier;
	bit(8) data[]; // until the end of the box
}
*/
public partial class IdentifiedMediaDataBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class IdentifiedMediaDataBox extends Box('imda') {
	unsigned int(32) imda_identifier;
	bit(8) data[]; // until the end of the box
}"; } }
	public const string TYPE = "imda";
	public override string DisplayName { get { return "IdentifiedMediaDataBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected uint imda_identifier; 
	public uint ImdaIdentifier { get { return this.imda_identifier; } set { this.imda_identifier = value; } }

	protected byte[] data = [];  //  until the end of the box
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public IdentifiedMediaDataBox(): base(IsoStream.FromFourCC("imda"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.imda_identifier, "imda_identifier"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); // until the end of the box
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.imda_identifier, "imda_identifier"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); // until the end of the box
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // imda_identifier
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("imda_identifier", 32, this.imda_identifier); 
		boxSize += tracer.Field("data", ((ulong)data.Length * 8), this.data); // until the end of the box
		return boxSize;
	}
}

}
