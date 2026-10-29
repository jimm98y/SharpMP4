using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
abstract aligned(8) class SampleEntry (unsigned int(32) format) extends Box(format){
	const unsigned int(8)[6] reservedSampleEntry = 0;
	unsigned int(16) data_reference_index;
}
*/
public abstract partial class SampleEntry : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"abstract aligned(8) class SampleEntry (unsigned int(32) format) extends Box(format){
	const unsigned int(8)[6] reservedSampleEntry = 0;
	unsigned int(16) data_reference_index;
}"; } }
	public override string DisplayName { get { return "SampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	protected byte[] reservedSampleEntry = new byte[6]; 
	public byte[] ReservedSampleEntry { get { return this.reservedSampleEntry; } set { this.reservedSampleEntry = value; } }

	protected ushort data_reference_index; 
	public ushort DataReferenceIndex { get { return this.data_reference_index; } set { this.data_reference_index = value; } }

	public SampleEntry(uint format): base(format)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 6,  out this.reservedSampleEntry, "reservedSampleEntry"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.data_reference_index, "data_reference_index"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(6,  this.reservedSampleEntry, "reservedSampleEntry"); 
		boxSize += stream.WriteUInt16( this.data_reference_index, "data_reference_index"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6 * 8; // reservedSampleEntry
		boxSize += 16; // data_reference_index
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reservedSampleEntry", 6 * 8, this.reservedSampleEntry); 
		boxSize += tracer.Field("data_reference_index", 16, this.data_reference_index); 
		return boxSize;
	}
}

}
