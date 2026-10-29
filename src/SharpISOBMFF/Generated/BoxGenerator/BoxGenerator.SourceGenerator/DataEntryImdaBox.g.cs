using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DataEntryImdaBox (bit(24) flags)
	extends DataEntryBaseBox('imdt', flags) {
	unsigned int(32) imda_ref_identifier;
}
*/
public partial class DataEntryImdaBox : DataEntryBaseBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class DataEntryImdaBox (bit(24) flags)
	extends DataEntryBaseBox('imdt', flags) {
	unsigned int(32) imda_ref_identifier;
}"; } }
	public const string TYPE = "imdt";
	public override string DisplayName { get { return "DataEntryImdaBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("dref"); }


	protected uint imda_ref_identifier; 
	public uint ImdaRefIdentifier { get { return this.imda_ref_identifier; } set { this.imda_ref_identifier = value; } }

	public DataEntryImdaBox(uint flags = 0): base(IsoStream.FromFourCC("imdt"), flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.imda_ref_identifier, "imda_ref_identifier"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.imda_ref_identifier, "imda_ref_identifier"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // imda_ref_identifier
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("imda_ref_identifier", 32, this.imda_ref_identifier); 
		return boxSize;
	}
}

}
