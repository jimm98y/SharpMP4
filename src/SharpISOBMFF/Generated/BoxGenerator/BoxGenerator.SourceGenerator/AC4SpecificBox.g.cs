using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// E.5, Pseudocode E.2: an ac4_dsi_v1() (E.6.1), to the end of the box
aligned(8) class AC4SpecificBox extends Box('dac4') {
 bit(3) ac4_dsi_version;
 bit(7) bitstream_version;
 bit(1) fs_index;
 bit(4) frame_rate_index;
 bit(9) n_presentations;
 if (bitstream_version > 1) {
  bit(1) b_program_id;
  if (b_program_id) {
   bit(16) short_program_id;
   bit(1) b_uuid;
   if (b_uuid) {
    bit(8) program_uuid[16];
   }
  }
 }
 AC4BitrateDsi ac4_bitrate_dsi;
 byte_alignment(); // byte_align, 0..7: to a byte from the start of ac4_dsi_v1
 AC4PresentationDsi presentations[n_presentations];
}
// E.7.1

*/
public partial class AC4SpecificBox : Box
{
	public const string TYPE = "dac4";
	public override string DisplayName { get { return "AC4SpecificBox"; } }

	protected byte ac4_dsi_version; 
	public byte Ac4DsiVersion { get { return this.ac4_dsi_version; } set { this.ac4_dsi_version = value; } }

	protected byte bitstream_version; 
	public byte BitstreamVersion { get { return this.bitstream_version; } set { this.bitstream_version = value; } }

	protected bool fs_index; 
	public bool FsIndex { get { return this.fs_index; } set { this.fs_index = value; } }

	protected byte frame_rate_index; 
	public byte FrameRateIndex { get { return this.frame_rate_index; } set { this.frame_rate_index = value; } }

	protected ushort n_presentations; 
	public ushort nPresentations { get { return this.n_presentations; } set { this.n_presentations = value; } }

	protected bool b_program_id; 
	public bool bProgramId { get { return this.b_program_id; } set { this.b_program_id = value; } }

	protected ushort short_program_id; 
	public ushort ShortProgramId { get { return this.short_program_id; } set { this.short_program_id = value; } }

	protected bool b_uuid; 
	public bool bUuid { get { return this.b_uuid; } set { this.b_uuid = value; } }

	protected byte[] program_uuid; 
	public byte[] ProgramUuid { get { return this.program_uuid; } set { this.program_uuid = value; } }

	protected AC4BitrateDsi ac4_bitrate_dsi; 
	public AC4BitrateDsi Ac4BitrateDsi { get { return this.ac4_bitrate_dsi; } set { this.ac4_bitrate_dsi = value; } }

	protected AlignmentBits byte_alignment;  //  byte_align, 0..7: to a byte from the start of ac4_dsi_v1
	public AlignmentBits ByteAlignment { get { return this.byte_alignment; } set { this.byte_alignment = value; } }

	protected AC4PresentationDsi[] presentations; 
	public AC4PresentationDsi[] Presentations { get { return this.presentations; } set { this.presentations = value; } }

	public AC4SpecificBox(): base(IsoStream.FromFourCC("dac4"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.ac4_dsi_version, "ac4_dsi_version"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.bitstream_version, "bitstream_version"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.fs_index, "fs_index"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.frame_rate_index, "frame_rate_index"); 
		boxSize += stream.ReadBits(boxSize, readSize, 9,  out this.n_presentations, "n_presentations"); 

		if (bitstream_version > 1)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_program_id, "b_program_id"); 

			if (b_program_id)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.short_program_id, "short_program_id"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_uuid, "b_uuid"); 

				if (b_uuid)
				{
					boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.program_uuid, "program_uuid"); 
				}
			}
		}
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4BitrateDsi(),  out this.ac4_bitrate_dsi, "ac4_bitrate_dsi"); 
		boxSize += stream.ReadByteAlignment(boxSize, readSize,  out this.byte_alignment, "byte_alignment"); // byte_align, 0..7: to a byte from the start of ac4_dsi_v1
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(n_presentations), () => new AC4PresentationDsi(),  out this.presentations, "presentations"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(3,  this.ac4_dsi_version, "ac4_dsi_version"); 
		boxSize += stream.WriteBits(7,  this.bitstream_version, "bitstream_version"); 
		boxSize += stream.WriteBit( this.fs_index, "fs_index"); 
		boxSize += stream.WriteBits(4,  this.frame_rate_index, "frame_rate_index"); 
		boxSize += stream.WriteBits(9,  this.n_presentations, "n_presentations"); 

		if (bitstream_version > 1)
		{
			boxSize += stream.WriteBit( this.b_program_id, "b_program_id"); 

			if (b_program_id)
			{
				boxSize += stream.WriteUInt16( this.short_program_id, "short_program_id"); 
				boxSize += stream.WriteBit( this.b_uuid, "b_uuid"); 

				if (b_uuid)
				{
					boxSize += stream.WriteUInt8Array(16,  this.program_uuid, "program_uuid"); 
				}
			}
		}
		boxSize += stream.WriteClass( this.ac4_bitrate_dsi, "ac4_bitrate_dsi"); 
		boxSize += stream.WriteByteAlignment( this.byte_alignment, "byte_alignment"); // byte_align, 0..7: to a byte from the start of ac4_dsi_v1
		boxSize += stream.WriteClass( this.presentations, "presentations"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 3; // ac4_dsi_version
		boxSize += 7; // bitstream_version
		boxSize += 1; // fs_index
		boxSize += 4; // frame_rate_index
		boxSize += 9; // n_presentations

		if (bitstream_version > 1)
		{
			boxSize += 1; // b_program_id

			if (b_program_id)
			{
				boxSize += 16; // short_program_id
				boxSize += 1; // b_uuid

				if (b_uuid)
				{
					boxSize += 16 * 8; // program_uuid
				}
			}
		}
		boxSize += IsoStream.CalculateClassSize(ac4_bitrate_dsi); // ac4_bitrate_dsi
		boxSize += IsoStream.CalculateByteAlignmentSize(boxSize, byte_alignment); // byte_alignment
		boxSize += IsoStream.CalculateClassSize(presentations); // presentations
		return boxSize;
	}
}

}
