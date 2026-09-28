using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// 3GPP TS 26.244 6.7, Table 6.6: AMRDecSpecStruc
aligned(8) class AmrSpecificBox() extends Box('damr') {
 unsigned int(32) vendor;
 unsigned int(8) decoder_version;
 unsigned int(16) mode_set;
 unsigned int(8) mode_change_period;
 unsigned int(8) frames_per_sample;
 } 
*/
public partial class AmrSpecificBox : Box
{
	public const string TYPE = "damr";
	public override string DisplayName { get { return "AmrSpecificBox"; } }

	protected uint vendor; 
	public uint Vendor { get { return this.vendor; } set { this.vendor = value; } }

	protected byte decoder_version; 
	public byte DecoderVersion { get { return this.decoder_version; } set { this.decoder_version = value; } }

	protected ushort mode_set; 
	public ushort ModeSet { get { return this.mode_set; } set { this.mode_set = value; } }

	protected byte mode_change_period; 
	public byte ModeChangePeriod { get { return this.mode_change_period; } set { this.mode_change_period = value; } }

	protected byte frames_per_sample; 
	public byte FramesPerSample { get { return this.frames_per_sample; } set { this.frames_per_sample = value; } }

	public AmrSpecificBox(): base(IsoStream.FromFourCC("damr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vendor, "vendor"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.decoder_version, "decoder_version"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.mode_set, "mode_set"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.mode_change_period, "mode_change_period"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.frames_per_sample, "frames_per_sample"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.vendor, "vendor"); 
		boxSize += stream.WriteUInt8( this.decoder_version, "decoder_version"); 
		boxSize += stream.WriteUInt16( this.mode_set, "mode_set"); 
		boxSize += stream.WriteUInt8( this.mode_change_period, "mode_change_period"); 
		boxSize += stream.WriteUInt8( this.frames_per_sample, "frames_per_sample"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // vendor
		boxSize += 8; // decoder_version
		boxSize += 16; // mode_set
		boxSize += 8; // mode_change_period
		boxSize += 8; // frames_per_sample
		return boxSize;
	}
}

}
