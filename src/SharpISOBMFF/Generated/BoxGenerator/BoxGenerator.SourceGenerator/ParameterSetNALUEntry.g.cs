using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class ParameterSetNALUEntry() extends VisualSampleGroupEntry ('pase')
{
	unsigned int(16) ps_nalu_length;
	bit(8* ps_nalu_length) ps_nal_unit;
}
*/
public partial class ParameterSetNALUEntry : VisualSampleGroupEntry
{
	public const string TYPE = "pase";
	public override string DisplayName { get { return "ParameterSetNALUEntry"; } }

	protected ushort ps_nalu_length; 
	public ushort PsNaluLength { get { return this.ps_nalu_length; } set { this.ps_nalu_length = value; } }

	protected byte[] ps_nal_unit; 
	public byte[] PsNalUnit { get { return this.ps_nal_unit; } set { this.ps_nal_unit = value; } }

	public ParameterSetNALUEntry(): base(IsoStream.FromFourCC("pase"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ps_nalu_length, "ps_nalu_length"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8* ps_nalu_length ),  out this.ps_nal_unit, "ps_nal_unit"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.ps_nalu_length, "ps_nalu_length"); 
		boxSize += stream.WriteBits((uint)(8* ps_nalu_length ),  this.ps_nal_unit, "ps_nal_unit"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // ps_nalu_length
		boxSize += (ulong)(8* ps_nalu_length ); // ps_nal_unit
		return boxSize;
	}
}

}
