using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

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
	public const string TYPE = "imdt";
	public override string DisplayName { get { return "DataEntryImdaBox"; } }

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
}

}
