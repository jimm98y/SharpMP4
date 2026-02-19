using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LoudnessBaseBox extends FullBox(loudnessType) { 
unsigned int(3) reserved = 0; 
unsigned int(7) downmix_ID;  // matching downmix 
unsigned int(6) DRC_set_ID;  // to match a DRC box 
signed int(12)  bs_sample_peak_level; 
signed int(12)  bs_true_peak_level; 
unsigned int(4) measurement_system_for_TP; 
unsigned int(4) reliability_for_TP; 
unsigned int(8) measurement_count; 
int i; 
for (i = 1 ; i <= measurement_count; i++){ 
  unsigned int(8) method_definition; 
  unsigned int(8) method_value; 
  unsigned int(4) measurement_system; 
  unsigned int(4) reliability; 
 } 
} 

*/
public partial class LoudnessBaseBox : FullBox
{
	public override string DisplayName { get { return "LoudnessBaseBox"; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte downmix_ID;  //  matching downmix 
	public byte DownmixID { get { return this.downmix_ID; } set { this.downmix_ID = value; } }

	protected byte DRC_set_ID;  //  to match a DRC box 
	public byte DRCSetID { get { return this.DRC_set_ID; } set { this.DRC_set_ID = value; } }

	protected short bs_sample_peak_level; 
	public short BsSamplePeakLevel { get { return this.bs_sample_peak_level; } set { this.bs_sample_peak_level = value; } }

	protected short bs_true_peak_level; 
	public short BsTruePeakLevel { get { return this.bs_true_peak_level; } set { this.bs_true_peak_level = value; } }

	protected byte measurement_system_for_TP; 
	public byte MeasurementSystemForTP { get { return this.measurement_system_for_TP; } set { this.measurement_system_for_TP = value; } }

	protected byte reliability_for_TP; 
	public byte ReliabilityForTP { get { return this.reliability_for_TP; } set { this.reliability_for_TP = value; } }

	protected byte measurement_count; 
	public byte MeasurementCount { get { return this.measurement_count; } set { this.measurement_count = value; } }

	protected byte[] method_definition; 
	public byte[] MethodDefinition { get { return this.method_definition; } set { this.method_definition = value; } }

	protected byte[] method_value; 
	public byte[] MethodValue { get { return this.method_value; } set { this.method_value = value; } }

	protected byte[] measurement_system; 
	public byte[] MeasurementSystem { get { return this.measurement_system; } set { this.measurement_system = value; } }

	protected byte[] reliability; 
	public byte[] Reliability { get { return this.reliability; } set { this.reliability = value; } }

	public LoudnessBaseBox(uint loudnessType): base(loudnessType)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.downmix_ID, "downmix_ID"); // matching downmix 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.DRC_set_ID, "DRC_set_ID"); // to match a DRC box 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.bs_sample_peak_level, "bs_sample_peak_level"); 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.bs_true_peak_level, "bs_true_peak_level"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.measurement_system_for_TP, "measurement_system_for_TP"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reliability_for_TP, "reliability_for_TP"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.measurement_count, "measurement_count"); 
		

		this.method_definition = new byte[IsoStream.GetInt( measurement_count)];
		this.method_value = new byte[IsoStream.GetInt( measurement_count)];
		this.measurement_system = new byte[IsoStream.GetInt( measurement_count)];
		this.reliability = new byte[IsoStream.GetInt( measurement_count)];
		for (int i = 0 ; i < measurement_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.method_definition[i], "method_definition"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.method_value[i], "method_value"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.measurement_system[i], "measurement_system"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reliability[i], "reliability"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(7,  this.downmix_ID, "downmix_ID"); // matching downmix 
		boxSize += stream.WriteBits(6,  this.DRC_set_ID, "DRC_set_ID"); // to match a DRC box 
		boxSize += stream.WriteBits(12,  this.bs_sample_peak_level, "bs_sample_peak_level"); 
		boxSize += stream.WriteBits(12,  this.bs_true_peak_level, "bs_true_peak_level"); 
		boxSize += stream.WriteBits(4,  this.measurement_system_for_TP, "measurement_system_for_TP"); 
		boxSize += stream.WriteBits(4,  this.reliability_for_TP, "reliability_for_TP"); 
		boxSize += stream.WriteUInt8( this.measurement_count, "measurement_count"); 
		

		for (int i = 0 ; i < measurement_count; i++)
		{
			boxSize += stream.WriteUInt8( this.method_definition[i], "method_definition"); 
			boxSize += stream.WriteUInt8( this.method_value[i], "method_value"); 
			boxSize += stream.WriteBits(4,  this.measurement_system[i], "measurement_system"); 
			boxSize += stream.WriteBits(4,  this.reliability[i], "reliability"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 3; // reserved
		boxSize += 7; // downmix_ID
		boxSize += 6; // DRC_set_ID
		boxSize += 12; // bs_sample_peak_level
		boxSize += 12; // bs_true_peak_level
		boxSize += 4; // measurement_system_for_TP
		boxSize += 4; // reliability_for_TP
		boxSize += 8; // measurement_count
		

		for (int i = 0 ; i < measurement_count; i++)
		{
			boxSize += 8; // method_definition
			boxSize += 8; // method_value
			boxSize += 4; // measurement_system
			boxSize += 4; // reliability
		}
		return boxSize;
	}
}

}
