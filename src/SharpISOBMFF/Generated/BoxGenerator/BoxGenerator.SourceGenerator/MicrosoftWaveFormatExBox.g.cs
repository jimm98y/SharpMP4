using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Microsoft's WAVEFORMATEX (mmreg.h), as QuickTime's 'wfex' holds it of a codec of Windows': little-endian, which the generator reads it as; the size of what follows and it, where the structure has them
class MicrosoftWaveFormatExBox() extends Box ('wfex'){
	unsigned int(16) format_tag;
	unsigned int(16) channels;
	unsigned int(32) samples_per_sec;
	unsigned int(32) avg_bytes_per_sec;
	unsigned int(16) block_align;
	unsigned int(16) bits_per_sample;
	unsigned int(16) extra_size; // optional
	bit(8) extra[];
}
*/
public partial class MicrosoftWaveFormatExBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Microsoft's WAVEFORMATEX (mmreg.h), as QuickTime's 'wfex' holds it of a codec of Windows': little-endian, which the generator reads it as; the size of what follows and it, where the structure has them
class MicrosoftWaveFormatExBox() extends Box ('wfex'){
	unsigned int(16) format_tag;
	unsigned int(16) channels;
	unsigned int(32) samples_per_sec;
	unsigned int(32) avg_bytes_per_sec;
	unsigned int(16) block_align;
	unsigned int(16) bits_per_sample;
	unsigned int(16) extra_size; // optional
	bit(8) extra[];
}"; } }
	public const string TYPE = "wfex";
	public override string DisplayName { get { return "MicrosoftWaveFormatExBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(wma )"); }


	protected ushort format_tag; 
	public ushort FormatTag { get { return this.format_tag; } set { this.format_tag = value; } }

	protected ushort channels; 
	public ushort Channels { get { return this.channels; } set { this.channels = value; } }

	protected uint samples_per_sec; 
	public uint SamplesPerSec { get { return this.samples_per_sec; } set { this.samples_per_sec = value; } }

	protected uint avg_bytes_per_sec; 
	public uint AvgBytesPerSec { get { return this.avg_bytes_per_sec; } set { this.avg_bytes_per_sec = value; } }

	protected ushort block_align; 
	public ushort BlockAlign { get { return this.block_align; } set { this.block_align = value; } }

	protected ushort bits_per_sample; 
	public ushort BitsPerSample { get { return this.bits_per_sample; } set { this.bits_per_sample = value; } }

	protected ushort extra_size;  //  optional
	protected bool extra_sizePresent;
	public ushort ExtraSize { get { return this.extra_size; } set { this.extra_size = value; this.extra_sizePresent = true; } }
	public bool ExtraSizePresent { get { return this.extra_sizePresent; } set { this.extra_sizePresent = value; } }

	protected byte[] extra = []; 
	public byte[] Extra { get { return this.extra; } set { this.extra = value; } }

	public MicrosoftWaveFormatExBox(): base(IsoStream.FromFourCC("wfex"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16LE(boxSize, readSize,  out this.format_tag, "format_tag"); 
		boxSize += stream.ReadUInt16LE(boxSize, readSize,  out this.channels, "channels"); 
		boxSize += stream.ReadUInt32LE(boxSize, readSize,  out this.samples_per_sec, "samples_per_sec"); 
		boxSize += stream.ReadUInt32LE(boxSize, readSize,  out this.avg_bytes_per_sec, "avg_bytes_per_sec"); 
		boxSize += stream.ReadUInt16LE(boxSize, readSize,  out this.block_align, "block_align"); 
		boxSize += stream.ReadUInt16LE(boxSize, readSize,  out this.bits_per_sample, "bits_per_sample"); 
		if (stream.HasMoreData(boxSize, readSize)) { boxSize += stream.ReadUInt16LE(boxSize, readSize,  out this.extra_size, "extra_size"); this.extra_sizePresent = true; } // optional
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.extra, "extra"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16LE( this.format_tag, "format_tag"); 
		boxSize += stream.WriteUInt16LE( this.channels, "channels"); 
		boxSize += stream.WriteUInt32LE( this.samples_per_sec, "samples_per_sec"); 
		boxSize += stream.WriteUInt32LE( this.avg_bytes_per_sec, "avg_bytes_per_sec"); 
		boxSize += stream.WriteUInt16LE( this.block_align, "block_align"); 
		boxSize += stream.WriteUInt16LE( this.bits_per_sample, "bits_per_sample"); 
		if (this.extra_sizePresent) boxSize += stream.WriteUInt16LE( this.extra_size, "extra_size"); // optional
		boxSize += stream.WriteUInt8ArrayTillEnd( this.extra, "extra"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // format_tag
		boxSize += 16; // channels
		boxSize += 32; // samples_per_sec
		boxSize += 32; // avg_bytes_per_sec
		boxSize += 16; // block_align
		boxSize += 16; // bits_per_sample
		if (this.extra_sizePresent) boxSize += 16; // extra_size
		boxSize += ((ulong)extra.Length * 8); // extra
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("format_tag", 16, this.format_tag); 
		boxSize += tracer.Field("channels", 16, this.channels); 
		boxSize += tracer.Field("samples_per_sec", 32, this.samples_per_sec); 
		boxSize += tracer.Field("avg_bytes_per_sec", 32, this.avg_bytes_per_sec); 
		boxSize += tracer.Field("block_align", 16, this.block_align); 
		boxSize += tracer.Field("bits_per_sample", 16, this.bits_per_sample); 
		if (this.extra_sizePresent) boxSize += tracer.Field("extra_size", 16, this.extra_size); // optional
		boxSize += tracer.Field("extra", ((ulong)extra.Length * 8), this.extra); 
		return boxSize;
	}
}

}
