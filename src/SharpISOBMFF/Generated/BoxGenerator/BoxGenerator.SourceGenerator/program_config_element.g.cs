using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class program_config_element()
{
  uimsbf(4) element_instance_tag;
  uimsbf(2) object_type;
  uimsbf(4) sampling_frequency_index;
  uimsbf(4) num_front_channel_elements;
  uimsbf(4) num_side_channel_elements;
  uimsbf(4) num_back_channel_elements;
  uimsbf(2) num_lfe_channel_elements;
  uimsbf(3) num_assoc_data_elements;
  uimsbf(4) num_valid_cc_elements;
  uimsbf(1) mono_mixdown_present;
  if (mono_mixdown_present == 1)
    uimsbf(4) mono_mixdown_element_number;
  uimsbf(1) stereo_mixdown_present;
  if (stereo_mixdown_present == 1)
    uimsbf(4) stereo_mixdown_element_number;
  uimsbf(1) matrix_mixdown_idx_present;
  if (matrix_mixdown_idx_present == 1) {
    uimsbf(2) matrix_mixdown_idx;
    uimsbf(1) pseudo_surround_enable;
  }
  for (i = 0; i < num_front_channel_elements; i++) {
    bslbf(1) front_element_is_cpe[i];
    uimsbf(4) front_element_tag_select[i];
  }
  for (i = 0; i < num_side_channel_elements; i++) {
    bslbf(1) side_element_is_cpe[i];
    uimsbf(4) side_element_tag_select[i];
  }
  for (i = 0; i < num_back_channel_elements; i++) {
    bslbf(1) back_element_is_cpe[i];
    uimsbf(4) back_element_tag_select[i];
  }
  for (i = 0; i < num_lfe_channel_elements; i++)
    uimsbf(4) lfe_element_tag_select[i];
  for (i = 0; i < num_assoc_data_elements; i++)
    uimsbf(4) assoc_data_element_tag_select[i];
  for (i = 0; i < num_valid_cc_elements; i++) {
    uimsbf(1) cc_element_is_ind_sw[i];
    uimsbf(4) valid_cc_element_tag_select[i];
  }
  byte_alignment();  // Note 1 
  uimsbf(8) comment_field_bytes;
  for (i = 0; i < comment_field_bytes; i++)
    uimsbf(8) comment_field_data[i];
}


*/
public partial class program_config_element : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "program_config_element"; } }

	protected byte element_instance_tag; 
	public byte ElementInstanceTag { get { return this.element_instance_tag; } set { this.element_instance_tag = value; } }

	protected byte object_type; 
	public byte ObjectType { get { return this.object_type; } set { this.object_type = value; } }

	protected byte sampling_frequency_index; 
	public byte SamplingFrequencyIndex { get { return this.sampling_frequency_index; } set { this.sampling_frequency_index = value; } }

	protected byte num_front_channel_elements; 
	public byte NumFrontChannelElements { get { return this.num_front_channel_elements; } set { this.num_front_channel_elements = value; } }

	protected byte num_side_channel_elements; 
	public byte NumSideChannelElements { get { return this.num_side_channel_elements; } set { this.num_side_channel_elements = value; } }

	protected byte num_back_channel_elements; 
	public byte NumBackChannelElements { get { return this.num_back_channel_elements; } set { this.num_back_channel_elements = value; } }

	protected byte num_lfe_channel_elements; 
	public byte NumLfeChannelElements { get { return this.num_lfe_channel_elements; } set { this.num_lfe_channel_elements = value; } }

	protected byte num_assoc_data_elements; 
	public byte NumAssocDataElements { get { return this.num_assoc_data_elements; } set { this.num_assoc_data_elements = value; } }

	protected byte num_valid_cc_elements; 
	public byte NumValidCcElements { get { return this.num_valid_cc_elements; } set { this.num_valid_cc_elements = value; } }

	protected bool mono_mixdown_present; 
	public bool MonoMixdownPresent { get { return this.mono_mixdown_present; } set { this.mono_mixdown_present = value; } }

	protected byte mono_mixdown_element_number; 
	public byte MonoMixdownElementNumber { get { return this.mono_mixdown_element_number; } set { this.mono_mixdown_element_number = value; } }

	protected bool stereo_mixdown_present; 
	public bool StereoMixdownPresent { get { return this.stereo_mixdown_present; } set { this.stereo_mixdown_present = value; } }

	protected byte stereo_mixdown_element_number; 
	public byte StereoMixdownElementNumber { get { return this.stereo_mixdown_element_number; } set { this.stereo_mixdown_element_number = value; } }

	protected bool matrix_mixdown_idx_present; 
	public bool MatrixMixdownIdxPresent { get { return this.matrix_mixdown_idx_present; } set { this.matrix_mixdown_idx_present = value; } }

	protected byte matrix_mixdown_idx; 
	public byte MatrixMixdownIdx { get { return this.matrix_mixdown_idx; } set { this.matrix_mixdown_idx = value; } }

	protected bool pseudo_surround_enable; 
	public bool PseudoSurroundEnable { get { return this.pseudo_surround_enable; } set { this.pseudo_surround_enable = value; } }

	protected bool[] front_element_is_cpe; 
	public bool[] FrontElementIsCpe { get { return this.front_element_is_cpe; } set { this.front_element_is_cpe = value; } }

	protected byte[] front_element_tag_select; 
	public byte[] FrontElementTagSelect { get { return this.front_element_tag_select; } set { this.front_element_tag_select = value; } }

	protected bool[] side_element_is_cpe; 
	public bool[] SideElementIsCpe { get { return this.side_element_is_cpe; } set { this.side_element_is_cpe = value; } }

	protected byte[] side_element_tag_select; 
	public byte[] SideElementTagSelect { get { return this.side_element_tag_select; } set { this.side_element_tag_select = value; } }

	protected bool[] back_element_is_cpe; 
	public bool[] BackElementIsCpe { get { return this.back_element_is_cpe; } set { this.back_element_is_cpe = value; } }

	protected byte[] back_element_tag_select; 
	public byte[] BackElementTagSelect { get { return this.back_element_tag_select; } set { this.back_element_tag_select = value; } }

	protected byte[] lfe_element_tag_select; 
	public byte[] LfeElementTagSelect { get { return this.lfe_element_tag_select; } set { this.lfe_element_tag_select = value; } }

	protected byte[] assoc_data_element_tag_select; 
	public byte[] AssocDataElementTagSelect { get { return this.assoc_data_element_tag_select; } set { this.assoc_data_element_tag_select = value; } }

	protected bool[] cc_element_is_ind_sw; 
	public bool[] CcElementIsIndSw { get { return this.cc_element_is_ind_sw; } set { this.cc_element_is_ind_sw = value; } }

	protected byte[] valid_cc_element_tag_select; 
	public byte[] ValidCcElementTagSelect { get { return this.valid_cc_element_tag_select; } set { this.valid_cc_element_tag_select = value; } }

	protected byte byte_alignment;  //  Note 1 
	public byte ByteAlignment { get { return this.byte_alignment; } set { this.byte_alignment = value; } }

	protected byte comment_field_bytes; 
	public byte CommentFieldBytes { get { return this.comment_field_bytes; } set { this.comment_field_bytes = value; } }

	protected byte[] comment_field_data; 
	public byte[] CommentFieldData { get { return this.comment_field_data; } set { this.comment_field_data = value; } }

	public program_config_element(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.element_instance_tag, "element_instance_tag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.object_type, "object_type"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.sampling_frequency_index, "sampling_frequency_index"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.num_front_channel_elements, "num_front_channel_elements"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.num_side_channel_elements, "num_side_channel_elements"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.num_back_channel_elements, "num_back_channel_elements"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.num_lfe_channel_elements, "num_lfe_channel_elements"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.num_assoc_data_elements, "num_assoc_data_elements"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.num_valid_cc_elements, "num_valid_cc_elements"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.mono_mixdown_present, "mono_mixdown_present"); 

		if (mono_mixdown_present == true)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.mono_mixdown_element_number, "mono_mixdown_element_number"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.stereo_mixdown_present, "stereo_mixdown_present"); 

		if (stereo_mixdown_present == true)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.stereo_mixdown_element_number, "stereo_mixdown_element_number"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.matrix_mixdown_idx_present, "matrix_mixdown_idx_present"); 

		if (matrix_mixdown_idx_present == true)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.matrix_mixdown_idx, "matrix_mixdown_idx"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.pseudo_surround_enable, "pseudo_surround_enable"); 
		}

		this.front_element_is_cpe = new bool[IsoStream.GetInt( num_front_channel_elements)];
		this.front_element_tag_select = new byte[IsoStream.GetInt( num_front_channel_elements)];
		for (int i = 0; i < num_front_channel_elements; i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.front_element_is_cpe[i], "front_element_is_cpe"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.front_element_tag_select[i], "front_element_tag_select"); 
		}

		this.side_element_is_cpe = new bool[IsoStream.GetInt( num_side_channel_elements)];
		this.side_element_tag_select = new byte[IsoStream.GetInt( num_side_channel_elements)];
		for (int i = 0; i < num_side_channel_elements; i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.side_element_is_cpe[i], "side_element_is_cpe"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.side_element_tag_select[i], "side_element_tag_select"); 
		}

		this.back_element_is_cpe = new bool[IsoStream.GetInt( num_back_channel_elements)];
		this.back_element_tag_select = new byte[IsoStream.GetInt( num_back_channel_elements)];
		for (int i = 0; i < num_back_channel_elements; i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.back_element_is_cpe[i], "back_element_is_cpe"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.back_element_tag_select[i], "back_element_tag_select"); 
		}

		this.lfe_element_tag_select = new byte[IsoStream.GetInt( num_lfe_channel_elements)];
		for (int i = 0; i < num_lfe_channel_elements; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.lfe_element_tag_select[i], "lfe_element_tag_select"); 
		}

		this.assoc_data_element_tag_select = new byte[IsoStream.GetInt( num_assoc_data_elements)];
		for (int i = 0; i < num_assoc_data_elements; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.assoc_data_element_tag_select[i], "assoc_data_element_tag_select"); 
		}

		this.cc_element_is_ind_sw = new bool[IsoStream.GetInt( num_valid_cc_elements)];
		this.valid_cc_element_tag_select = new byte[IsoStream.GetInt( num_valid_cc_elements)];
		for (int i = 0; i < num_valid_cc_elements; i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.cc_element_is_ind_sw[i], "cc_element_is_ind_sw"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.valid_cc_element_tag_select[i], "valid_cc_element_tag_select"); 
		}
		boxSize += stream.ReadByteAlignment(boxSize, readSize,  out this.byte_alignment, "byte_alignment"); // Note 1 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.comment_field_bytes, "comment_field_bytes"); 

		this.comment_field_data = new byte[IsoStream.GetInt( comment_field_bytes)];
		for (int i = 0; i < comment_field_bytes; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.comment_field_data[i], "comment_field_data"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(4,  this.element_instance_tag, "element_instance_tag"); 
		boxSize += stream.WriteBits(2,  this.object_type, "object_type"); 
		boxSize += stream.WriteBits(4,  this.sampling_frequency_index, "sampling_frequency_index"); 
		boxSize += stream.WriteBits(4,  this.num_front_channel_elements, "num_front_channel_elements"); 
		boxSize += stream.WriteBits(4,  this.num_side_channel_elements, "num_side_channel_elements"); 
		boxSize += stream.WriteBits(4,  this.num_back_channel_elements, "num_back_channel_elements"); 
		boxSize += stream.WriteBits(2,  this.num_lfe_channel_elements, "num_lfe_channel_elements"); 
		boxSize += stream.WriteBits(3,  this.num_assoc_data_elements, "num_assoc_data_elements"); 
		boxSize += stream.WriteBits(4,  this.num_valid_cc_elements, "num_valid_cc_elements"); 
		boxSize += stream.WriteBit( this.mono_mixdown_present, "mono_mixdown_present"); 

		if (mono_mixdown_present == true)
		{
			boxSize += stream.WriteBits(4,  this.mono_mixdown_element_number, "mono_mixdown_element_number"); 
		}
		boxSize += stream.WriteBit( this.stereo_mixdown_present, "stereo_mixdown_present"); 

		if (stereo_mixdown_present == true)
		{
			boxSize += stream.WriteBits(4,  this.stereo_mixdown_element_number, "stereo_mixdown_element_number"); 
		}
		boxSize += stream.WriteBit( this.matrix_mixdown_idx_present, "matrix_mixdown_idx_present"); 

		if (matrix_mixdown_idx_present == true)
		{
			boxSize += stream.WriteBits(2,  this.matrix_mixdown_idx, "matrix_mixdown_idx"); 
			boxSize += stream.WriteBit( this.pseudo_surround_enable, "pseudo_surround_enable"); 
		}

		for (int i = 0; i < num_front_channel_elements; i++)
		{
			boxSize += stream.WriteBit( this.front_element_is_cpe[i], "front_element_is_cpe"); 
			boxSize += stream.WriteBits(4,  this.front_element_tag_select[i], "front_element_tag_select"); 
		}

		for (int i = 0; i < num_side_channel_elements; i++)
		{
			boxSize += stream.WriteBit( this.side_element_is_cpe[i], "side_element_is_cpe"); 
			boxSize += stream.WriteBits(4,  this.side_element_tag_select[i], "side_element_tag_select"); 
		}

		for (int i = 0; i < num_back_channel_elements; i++)
		{
			boxSize += stream.WriteBit( this.back_element_is_cpe[i], "back_element_is_cpe"); 
			boxSize += stream.WriteBits(4,  this.back_element_tag_select[i], "back_element_tag_select"); 
		}

		for (int i = 0; i < num_lfe_channel_elements; i++)
		{
			boxSize += stream.WriteBits(4,  this.lfe_element_tag_select[i], "lfe_element_tag_select"); 
		}

		for (int i = 0; i < num_assoc_data_elements; i++)
		{
			boxSize += stream.WriteBits(4,  this.assoc_data_element_tag_select[i], "assoc_data_element_tag_select"); 
		}

		for (int i = 0; i < num_valid_cc_elements; i++)
		{
			boxSize += stream.WriteBit( this.cc_element_is_ind_sw[i], "cc_element_is_ind_sw"); 
			boxSize += stream.WriteBits(4,  this.valid_cc_element_tag_select[i], "valid_cc_element_tag_select"); 
		}
		boxSize += stream.WriteByteAlignment( this.byte_alignment, "byte_alignment"); // Note 1 
		boxSize += stream.WriteUInt8( this.comment_field_bytes, "comment_field_bytes"); 

		for (int i = 0; i < comment_field_bytes; i++)
		{
			boxSize += stream.WriteUInt8( this.comment_field_data[i], "comment_field_data"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 4; // element_instance_tag
		boxSize += 2; // object_type
		boxSize += 4; // sampling_frequency_index
		boxSize += 4; // num_front_channel_elements
		boxSize += 4; // num_side_channel_elements
		boxSize += 4; // num_back_channel_elements
		boxSize += 2; // num_lfe_channel_elements
		boxSize += 3; // num_assoc_data_elements
		boxSize += 4; // num_valid_cc_elements
		boxSize += 1; // mono_mixdown_present

		if (mono_mixdown_present == true)
		{
			boxSize += 4; // mono_mixdown_element_number
		}
		boxSize += 1; // stereo_mixdown_present

		if (stereo_mixdown_present == true)
		{
			boxSize += 4; // stereo_mixdown_element_number
		}
		boxSize += 1; // matrix_mixdown_idx_present

		if (matrix_mixdown_idx_present == true)
		{
			boxSize += 2; // matrix_mixdown_idx
			boxSize += 1; // pseudo_surround_enable
		}

		for (int i = 0; i < num_front_channel_elements; i++)
		{
			boxSize += 1; // front_element_is_cpe
			boxSize += 4; // front_element_tag_select
		}

		for (int i = 0; i < num_side_channel_elements; i++)
		{
			boxSize += 1; // side_element_is_cpe
			boxSize += 4; // side_element_tag_select
		}

		for (int i = 0; i < num_back_channel_elements; i++)
		{
			boxSize += 1; // back_element_is_cpe
			boxSize += 4; // back_element_tag_select
		}

		for (int i = 0; i < num_lfe_channel_elements; i++)
		{
			boxSize += 4; // lfe_element_tag_select
		}

		for (int i = 0; i < num_assoc_data_elements; i++)
		{
			boxSize += 4; // assoc_data_element_tag_select
		}

		for (int i = 0; i < num_valid_cc_elements; i++)
		{
			boxSize += 1; // cc_element_is_ind_sw
			boxSize += 4; // valid_cc_element_tag_select
		}
		boxSize += IsoStream.CalculateByteAlignmentSize(boxSize, byte_alignment); // byte_alignment
		boxSize += 8; // comment_field_bytes

		for (int i = 0; i < comment_field_bytes; i++)
		{
			boxSize += 8; // comment_field_data
		}
		return boxSize;
	}
}

}
