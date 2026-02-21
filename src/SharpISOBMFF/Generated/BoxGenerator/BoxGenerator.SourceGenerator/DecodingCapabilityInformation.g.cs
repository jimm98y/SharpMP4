using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class DecodingCapabilityInformation extends VisualSampleGroupEntry ('dcfi') {
	unsigned int(16) dci_nal_unit_length;
	bit(8*dci_nal_unit_length) dci_nal_unit;
}
*/
public partial class DecodingCapabilityInformation : VisualSampleGroupEntry
{
	public const string TYPE = "dcfi";
	public override string DisplayName { get { return "DecodingCapabilityInformation"; } }

	protected ushort dci_nal_unit_length; 
	public ushort DciNalUnitLength { get { return this.dci_nal_unit_length; } set { this.dci_nal_unit_length = value; } }

	protected byte[] dci_nal_unit; 
	public byte[] DciNalUnit { get { return this.dci_nal_unit; } set { this.dci_nal_unit = value; } }

	public DecodingCapabilityInformation(): base(IsoStream.FromFourCC("dcfi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dci_nal_unit_length, "dci_nal_unit_length"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*dci_nal_unit_length ),  out this.dci_nal_unit, "dci_nal_unit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.dci_nal_unit_length, "dci_nal_unit_length"); 
		boxSize += stream.WriteBits((uint)(8*dci_nal_unit_length ),  this.dci_nal_unit, "dci_nal_unit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // dci_nal_unit_length
		boxSize += (ulong)(8*dci_nal_unit_length ); // dci_nal_unit
		return boxSize;
	}
}

}
