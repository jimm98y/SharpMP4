using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ErrorProtectionSpecificConfig()
{
  uimsbf(8) number_of_predefined_set;
  uimsbf(2) interleave_type;
  uimsbf(3) bit_stuffing;
  uimsbf(3) number_of_concatenated_frame;
  for (i = 0; i < number_of_predefined_set; i++) {
    uimsbf(6) number_of_class[i];
    for (j = 0; j < number_of_class[i]; j++) {
      uimsbf(1) length_escape[i][j];
      uimsbf(1) rate_escape[i][j];
      uimsbf(1) crclen_escape[i][j];
      if (number_of_concatenated_frame != 1) {
        uimsbf(1) concatenate_flag[i][j];
      }
      uimsbf(2) fec_type[i][j];
      if (fec_type[i][j] == 0) {
        uimsbf(1) termination_switch[i][j];
      }
      if (interleave_type == 2) {
        uimsbf(2) interleave_switch[i][j];
      }
      uimsbf(1) class_optional;
      if (length_escape[i][j] == 1) { /* ESC *//*
        uimsbf(4) number_of_bits_for_length[i][j];
      }
      else {
        uimsbf(16) class_length[i][j];
      }
      if (rate_escape[i][j] != 1) { /* not ESC *//*
        if (fec_type[i][j] != 0) {
          uimsbf(7) class_rate[i][j];
        } else {
          uimsbf(5) class_rate[i][j];
        }
      }
      if (crclen_escape[i][j] != 1) {  /* not ESC *//*
        uimsbf(5) class_crclen[i][j];
      }
    }
    uimsbf(1) class_reordered_output;
    if (class_reordered_output == 1) {
      for (j = 0; j < number_of_class[i]; j++) {
        uimsbf(6) class_output_order[i][j];
      }
    }
  }
  uimsbf(1) header_protection;
  if (header_protection == 1) {
    uimsbf(5) header_rate;
    uimsbf(5) header_crclen;
  }
}

*/
public partial class ErrorProtectionSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ErrorProtectionSpecificConfig"; } }

	protected byte number_of_predefined_set; 
	public byte NumberOfPredefinedSet { get { return this.number_of_predefined_set; } set { this.number_of_predefined_set = value; } }

	protected byte interleave_type; 
	public byte InterleaveType { get { return this.interleave_type; } set { this.interleave_type = value; } }

	protected byte bit_stuffing; 
	public byte BitStuffing { get { return this.bit_stuffing; } set { this.bit_stuffing = value; } }

	protected byte number_of_concatenated_frame; 
	public byte NumberOfConcatenatedFrame { get { return this.number_of_concatenated_frame; } set { this.number_of_concatenated_frame = value; } }

	protected byte[] number_of_class; 
	public byte[] NumberOfClass { get { return this.number_of_class; } set { this.number_of_class = value; } }

	protected bool[][] length_escape; 
	public bool[][] LengthEscape { get { return this.length_escape; } set { this.length_escape = value; } }

	protected bool[][] rate_escape; 
	public bool[][] RateEscape { get { return this.rate_escape; } set { this.rate_escape = value; } }

	protected bool[][] crclen_escape; 
	public bool[][] CrclenEscape { get { return this.crclen_escape; } set { this.crclen_escape = value; } }

	protected bool[][] concatenate_flag; 
	public bool[][] ConcatenateFlag { get { return this.concatenate_flag; } set { this.concatenate_flag = value; } }

	protected byte[][] fec_type; 
	public byte[][] FecType { get { return this.fec_type; } set { this.fec_type = value; } }

	protected bool[][] termination_switch; 
	public bool[][] TerminationSwitch { get { return this.termination_switch; } set { this.termination_switch = value; } }

	protected byte[][] interleave_switch; 
	public byte[][] InterleaveSwitch { get { return this.interleave_switch; } set { this.interleave_switch = value; } }

	protected bool[][] class_optional; 
	public bool[][] ClassOptional { get { return this.class_optional; } set { this.class_optional = value; } }

	protected byte[][] number_of_bits_for_length; 
	public byte[][] NumberOfBitsForLength { get { return this.number_of_bits_for_length; } set { this.number_of_bits_for_length = value; } }

	protected ushort[][] class_length; 
	public ushort[][] ClassLength { get { return this.class_length; } set { this.class_length = value; } }

	protected byte[][] class_rate; 
	public byte[][] ClassRate { get { return this.class_rate; } set { this.class_rate = value; } }

	protected byte[][] class_crclen; 
	public byte[][] ClassCrclen { get { return this.class_crclen; } set { this.class_crclen = value; } }

	protected bool[] class_reordered_output; 
	public bool[] ClassReorderedOutput { get { return this.class_reordered_output; } set { this.class_reordered_output = value; } }

	protected byte[][] class_output_order; 
	public byte[][] ClassOutputOrder { get { return this.class_output_order; } set { this.class_output_order = value; } }

	protected bool header_protection; 
	public bool HeaderProtection { get { return this.header_protection; } set { this.header_protection = value; } }

	protected byte header_rate; 
	public byte HeaderRate { get { return this.header_rate; } set { this.header_rate = value; } }

	protected byte header_crclen; 
	public byte HeaderCrclen { get { return this.header_crclen; } set { this.header_crclen = value; } }

	public ErrorProtectionSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.number_of_predefined_set, "number_of_predefined_set"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.interleave_type, "interleave_type"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_stuffing, "bit_stuffing"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.number_of_concatenated_frame, "number_of_concatenated_frame"); 

		this.number_of_class = new byte[IsoStream.GetInt( number_of_predefined_set)];
		this.length_escape = new bool[IsoStream.GetInt( number_of_predefined_set)][];
		this.rate_escape = new bool[IsoStream.GetInt( number_of_predefined_set)][];
		this.crclen_escape = new bool[IsoStream.GetInt( number_of_predefined_set)][];
		this.concatenate_flag = new bool[IsoStream.GetInt( number_of_predefined_set)][];
		this.fec_type = new byte[IsoStream.GetInt( number_of_predefined_set)][];
		this.termination_switch = new bool[IsoStream.GetInt( number_of_predefined_set)][];
		this.interleave_switch = new byte[IsoStream.GetInt( number_of_predefined_set)][];
		this.class_optional = new bool[IsoStream.GetInt( number_of_predefined_set)][];
		this.number_of_bits_for_length = new byte[IsoStream.GetInt( number_of_predefined_set)][];
		this.class_length = new ushort[IsoStream.GetInt( number_of_predefined_set)][];
		this.class_rate = new byte[IsoStream.GetInt( number_of_predefined_set)][];
		this.class_crclen = new byte[IsoStream.GetInt( number_of_predefined_set)][];
		this.class_reordered_output = new bool[IsoStream.GetInt( number_of_predefined_set)];
		this.class_output_order = new byte[IsoStream.GetInt( number_of_predefined_set)][];
		for (int i = 0; i < number_of_predefined_set; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.number_of_class[i], "number_of_class"); 

			this.length_escape[i] = new bool[IsoStream.GetInt( number_of_class[i])];
			this.rate_escape[i] = new bool[IsoStream.GetInt( number_of_class[i])];
			this.crclen_escape[i] = new bool[IsoStream.GetInt( number_of_class[i])];
			this.concatenate_flag[i] = new bool[IsoStream.GetInt( number_of_class[i])];
			this.fec_type[i] = new byte[IsoStream.GetInt( number_of_class[i])];
			this.termination_switch[i] = new bool[IsoStream.GetInt( number_of_class[i])];
			this.interleave_switch[i] = new byte[IsoStream.GetInt( number_of_class[i])];
			this.class_optional[i] = new bool[IsoStream.GetInt( number_of_class[i])];
			this.number_of_bits_for_length[i] = new byte[IsoStream.GetInt( number_of_class[i])];
			this.class_length[i] = new ushort[IsoStream.GetInt( number_of_class[i])];
			this.class_rate[i] = new byte[IsoStream.GetInt( number_of_class[i])];
			this.class_crclen[i] = new byte[IsoStream.GetInt( number_of_class[i])];
			for (int j = 0; j < number_of_class[i]; j++)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.length_escape[i][j], "length_escape"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.rate_escape[i][j], "rate_escape"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.crclen_escape[i][j], "crclen_escape"); 

				if (number_of_concatenated_frame != 1)
				{
					boxSize += stream.ReadBit(boxSize, readSize,  out this.concatenate_flag[i][j], "concatenate_flag"); 
				}
				boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.fec_type[i][j], "fec_type"); 

				if (fec_type[i][j] == 0)
				{
					boxSize += stream.ReadBit(boxSize, readSize,  out this.termination_switch[i][j], "termination_switch"); 
				}

				if (interleave_type == 2)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.interleave_switch[i][j], "interleave_switch"); 
				}
				boxSize += stream.ReadBit(boxSize, readSize,  out this.class_optional[i][j], "class_optional"); 

				if (length_escape[i][j] == true)
				{
					/*  ESC  */
					boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.number_of_bits_for_length[i][j], "number_of_bits_for_length"); 
				}

				else 
				{
					boxSize += stream.ReadUInt16(boxSize, readSize,  out this.class_length[i][j], "class_length"); 
				}

				if (rate_escape[i][j] != true)
				{
					/*  not ESC  */

					if (fec_type[i][j] != 0)
					{
						boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.class_rate[i][j], "class_rate"); 
					}

					else 
					{
						boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.class_rate[i][j], "class_rate"); 
					}
				}

				if (crclen_escape[i][j] != true)
				{
					/*  not ESC  */
					boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.class_crclen[i][j], "class_crclen"); 
				}
			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.class_reordered_output[i], "class_reordered_output"); 

			if (class_reordered_output[i] == true)
			{

				this.class_output_order[i] = new byte[IsoStream.GetInt( number_of_class[i])];
				for (int j = 0; j < number_of_class[i]; j++)
				{
					boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.class_output_order[i][j], "class_output_order"); 
				}
			}
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.header_protection, "header_protection"); 

		if (header_protection == true)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.header_rate, "header_rate"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.header_crclen, "header_crclen"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.number_of_predefined_set, "number_of_predefined_set"); 
		boxSize += stream.WriteBits(2,  this.interleave_type, "interleave_type"); 
		boxSize += stream.WriteBits(3,  this.bit_stuffing, "bit_stuffing"); 
		boxSize += stream.WriteBits(3,  this.number_of_concatenated_frame, "number_of_concatenated_frame"); 

		for (int i = 0; i < number_of_predefined_set; i++)
		{
			boxSize += stream.WriteBits(6,  this.number_of_class[i], "number_of_class"); 

			for (int j = 0; j < number_of_class[i]; j++)
			{
				boxSize += stream.WriteBit( this.length_escape[i][j], "length_escape"); 
				boxSize += stream.WriteBit( this.rate_escape[i][j], "rate_escape"); 
				boxSize += stream.WriteBit( this.crclen_escape[i][j], "crclen_escape"); 

				if (number_of_concatenated_frame != 1)
				{
					boxSize += stream.WriteBit( this.concatenate_flag[i][j], "concatenate_flag"); 
				}
				boxSize += stream.WriteBits(2,  this.fec_type[i][j], "fec_type"); 

				if (fec_type[i][j] == 0)
				{
					boxSize += stream.WriteBit( this.termination_switch[i][j], "termination_switch"); 
				}

				if (interleave_type == 2)
				{
					boxSize += stream.WriteBits(2,  this.interleave_switch[i][j], "interleave_switch"); 
				}
				boxSize += stream.WriteBit( this.class_optional[i][j], "class_optional"); 

				if (length_escape[i][j] == true)
				{
					/*  ESC  */
					boxSize += stream.WriteBits(4,  this.number_of_bits_for_length[i][j], "number_of_bits_for_length"); 
				}

				else 
				{
					boxSize += stream.WriteUInt16( this.class_length[i][j], "class_length"); 
				}

				if (rate_escape[i][j] != true)
				{
					/*  not ESC  */

					if (fec_type[i][j] != 0)
					{
						boxSize += stream.WriteBits(7,  this.class_rate[i][j], "class_rate"); 
					}

					else 
					{
						boxSize += stream.WriteBits(5,  this.class_rate[i][j], "class_rate"); 
					}
				}

				if (crclen_escape[i][j] != true)
				{
					/*  not ESC  */
					boxSize += stream.WriteBits(5,  this.class_crclen[i][j], "class_crclen"); 
				}
			}
			boxSize += stream.WriteBit( this.class_reordered_output[i], "class_reordered_output"); 

			if (class_reordered_output[i] == true)
			{

				for (int j = 0; j < number_of_class[i]; j++)
				{
					boxSize += stream.WriteBits(6,  this.class_output_order[i][j], "class_output_order"); 
				}
			}
		}
		boxSize += stream.WriteBit( this.header_protection, "header_protection"); 

		if (header_protection == true)
		{
			boxSize += stream.WriteBits(5,  this.header_rate, "header_rate"); 
			boxSize += stream.WriteBits(5,  this.header_crclen, "header_crclen"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // number_of_predefined_set
		boxSize += 2; // interleave_type
		boxSize += 3; // bit_stuffing
		boxSize += 3; // number_of_concatenated_frame

		for (int i = 0; i < number_of_predefined_set; i++)
		{
			boxSize += 6; // number_of_class

			for (int j = 0; j < number_of_class[i]; j++)
			{
				boxSize += 1; // length_escape
				boxSize += 1; // rate_escape
				boxSize += 1; // crclen_escape

				if (number_of_concatenated_frame != 1)
				{
					boxSize += 1; // concatenate_flag
				}
				boxSize += 2; // fec_type

				if (fec_type[i][j] == 0)
				{
					boxSize += 1; // termination_switch
				}

				if (interleave_type == 2)
				{
					boxSize += 2; // interleave_switch
				}
				boxSize += 1; // class_optional

				if (length_escape[i][j] == true)
				{
					/*  ESC  */
					boxSize += 4; // number_of_bits_for_length
				}

				else 
				{
					boxSize += 16; // class_length
				}

				if (rate_escape[i][j] != true)
				{
					/*  not ESC  */

					if (fec_type[i][j] != 0)
					{
						boxSize += 7; // class_rate
					}

					else 
					{
						boxSize += 5; // class_rate
					}
				}

				if (crclen_escape[i][j] != true)
				{
					/*  not ESC  */
					boxSize += 5; // class_crclen
				}
			}
			boxSize += 1; // class_reordered_output

			if (class_reordered_output[i] == true)
			{

				for (int j = 0; j < number_of_class[i]; j++)
				{
					boxSize += 6; // class_output_order
				}
			}
		}
		boxSize += 1; // header_protection

		if (header_protection == true)
		{
			boxSize += 5; // header_rate
			boxSize += 5; // header_crclen
		}
		return boxSize;
	}
}

}
