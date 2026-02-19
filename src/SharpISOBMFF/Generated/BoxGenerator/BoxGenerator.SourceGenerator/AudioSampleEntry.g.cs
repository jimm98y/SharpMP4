using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AudioSampleEntry(codingname) extends SampleEntry (codingname) {
    unsigned int(16) soundversion = 0;
    unsigned int(16) reserved1 = 0;
    unsigned int(32) reserved2 = 0;
    unsigned int(16) channelcount;
    template unsigned int(16) samplesize = 16;

    unsigned int(16) pre_defined = 0;
    const unsigned int(16) reserved = 0;
    template unsigned int(32) samplerate;

    if(codingname != 'mlpa') {
        samplerate = samplerate >> 16;
    }

    if(soundversion == 1 || soundversion == 2) {
       unsigned int(32) samplesPerPacket;
       unsigned int(32) bytesPerPacket;
       unsigned int(32) bytesPerFrame;
       unsigned int(32) bytesPerSample;
    }

    if(soundversion == 2) {
       unsigned int(8)[20] soundVersion2Data;
    }

    ChannelLayout();
    // we permit any number of DownMix or DRC boxes: 
    DownMixInstructions() [];
    DRCCoefficientsBasic() [];
    DRCInstructionsBasic() [];
    DRCCoefficientsUniDRC() [];
    DRCInstructionsUniDRC() [];
    // we permit only one DRC Extension box:
    UniDrcConfigExtension();
    // optional boxes follow
    SamplingRateBox();
    Box (); // further boxes as needed
}
*/
public partial class AudioSampleEntry : SampleEntry
{
	public override string DisplayName { get { return "AudioSampleEntry"; } }

	protected ushort soundversion = 0; 
	public ushort Soundversion { get { return this.soundversion; } set { this.soundversion = value; } }

	protected ushort reserved1 = 0; 
	public ushort Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected uint reserved2 = 0; 
	public uint Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort channelcount; 
	public ushort Channelcount { get { return this.channelcount; } set { this.channelcount = value; } }

	protected ushort samplesize = 16; 
	public ushort Samplesize { get { return this.samplesize; } set { this.samplesize = value; } }

	protected ushort pre_defined = 0; 
	public ushort PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	protected ushort reserved = 0; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint samplerate; 
	public uint Samplerate { get { return this.samplerate; } set { this.samplerate = value; } }

	protected uint samplesPerPacket; 
	public uint SamplesPerPacket { get { return this.samplesPerPacket; } set { this.samplesPerPacket = value; } }

	protected uint bytesPerPacket; 
	public uint BytesPerPacket { get { return this.bytesPerPacket; } set { this.bytesPerPacket = value; } }

	protected uint bytesPerFrame; 
	public uint BytesPerFrame { get { return this.bytesPerFrame; } set { this.bytesPerFrame = value; } }

	protected uint bytesPerSample; 
	public uint BytesPerSample { get { return this.bytesPerSample; } set { this.bytesPerSample = value; } }

	protected byte[] soundVersion2Data; 
	public byte[] SoundVersion2Data { get { return this.soundVersion2Data; } set { this.soundVersion2Data = value; } }
	public ChannelLayout _ChannelLayout { get { return this.children.OfType<ChannelLayout>().FirstOrDefault(); } }
	public IEnumerable<DownMixInstructions> _DownMixInstructions { get { return this.children.OfType<DownMixInstructions>(); } }
	public IEnumerable<DRCCoefficientsBasic> _DRCCoefficientsBasic { get { return this.children.OfType<DRCCoefficientsBasic>(); } }
	public IEnumerable<DRCInstructionsBasic> _DRCInstructionsBasic { get { return this.children.OfType<DRCInstructionsBasic>(); } }
	public IEnumerable<DRCCoefficientsUniDRC> _DRCCoefficientsUniDRC { get { return this.children.OfType<DRCCoefficientsUniDRC>(); } }
	public IEnumerable<DRCInstructionsUniDRC> _DRCInstructionsUniDRC { get { return this.children.OfType<DRCInstructionsUniDRC>(); } }
	public UniDrcConfigExtension _UniDrcConfigExtension { get { return this.children.OfType<UniDrcConfigExtension>().FirstOrDefault(); } }
	public SamplingRateBox _SamplingRateBox { get { return this.children.OfType<SamplingRateBox>().FirstOrDefault(); } }
	public Box _Box { get { return this.children.OfType<Box>().FirstOrDefault(); } }

	public AudioSampleEntry(uint codingname = 0): base(codingname)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.soundversion, "soundversion"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channelcount, "channelcount"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.samplesize, "samplesize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pre_defined, "pre_defined"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samplerate, "samplerate"); 

		if (FourCC != IsoStream.FromFourCC("mlpa"))
		{
			// samplerate = samplerate >> 16;
		}

		if (soundversion == 1 || soundversion == 2)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samplesPerPacket, "samplesPerPacket"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytesPerPacket, "bytesPerPacket"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytesPerFrame, "bytesPerFrame"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.bytesPerSample, "bytesPerSample"); 
		}

		if (soundversion == 2)
		{
			boxSize += stream.ReadUInt8Array(boxSize, readSize, 20,  out this.soundVersion2Data, "soundVersion2Data"); 
		}
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ChannelLayout, "ChannelLayout"); // we permit any number of DownMix or DRC boxes: 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DownMixInstructions, "DownMixInstructions"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCCoefficientsBasic, "DRCCoefficientsBasic"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCInstructionsBasic, "DRCInstructionsBasic"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCCoefficientsUniDRC, "DRCCoefficientsUniDRC"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCInstructionsUniDRC, "DRCInstructionsUniDRC"); // we permit only one DRC Extension box:
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.UniDrcConfigExtension, "UniDrcConfigExtension"); // optional boxes follow
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SamplingRateBox, "SamplingRateBox"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.Box, "Box"); // further boxes as needed
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.soundversion, "soundversion"); 
		boxSize += stream.WriteUInt16( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt32( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt16( this.channelcount, "channelcount"); 
		boxSize += stream.WriteUInt16( this.samplesize, "samplesize"); 
		boxSize += stream.WriteUInt16( this.pre_defined, "pre_defined"); 
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.samplerate, "samplerate"); 

		if (FourCC != IsoStream.FromFourCC("mlpa"))
		{
			// samplerate = samplerate >> 16;
		}

		if (soundversion == 1 || soundversion == 2)
		{
			boxSize += stream.WriteUInt32( this.samplesPerPacket, "samplesPerPacket"); 
			boxSize += stream.WriteUInt32( this.bytesPerPacket, "bytesPerPacket"); 
			boxSize += stream.WriteUInt32( this.bytesPerFrame, "bytesPerFrame"); 
			boxSize += stream.WriteUInt32( this.bytesPerSample, "bytesPerSample"); 
		}

		if (soundversion == 2)
		{
			boxSize += stream.WriteUInt8Array(20,  this.soundVersion2Data, "soundVersion2Data"); 
		}
		// boxSize += stream.WriteBox( this.ChannelLayout, "ChannelLayout"); // we permit any number of DownMix or DRC boxes: 
		// boxSize += stream.WriteBox( this.DownMixInstructions, "DownMixInstructions"); 
		// boxSize += stream.WriteBox( this.DRCCoefficientsBasic, "DRCCoefficientsBasic"); 
		// boxSize += stream.WriteBox( this.DRCInstructionsBasic, "DRCInstructionsBasic"); 
		// boxSize += stream.WriteBox( this.DRCCoefficientsUniDRC, "DRCCoefficientsUniDRC"); 
		// boxSize += stream.WriteBox( this.DRCInstructionsUniDRC, "DRCInstructionsUniDRC"); // we permit only one DRC Extension box:
		// boxSize += stream.WriteBox( this.UniDrcConfigExtension, "UniDrcConfigExtension"); // optional boxes follow
		// boxSize += stream.WriteBox( this.SamplingRateBox, "SamplingRateBox"); 
		// boxSize += stream.WriteBox( this.Box, "Box"); // further boxes as needed
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // soundversion
		boxSize += 16; // reserved1
		boxSize += 32; // reserved2
		boxSize += 16; // channelcount
		boxSize += 16; // samplesize
		boxSize += 16; // pre_defined
		boxSize += 16; // reserved
		boxSize += 32; // samplerate

		if (FourCC != IsoStream.FromFourCC("mlpa"))
		{
			// samplerate = samplerate >> 16;
		}

		if (soundversion == 1 || soundversion == 2)
		{
			boxSize += 32; // samplesPerPacket
			boxSize += 32; // bytesPerPacket
			boxSize += 32; // bytesPerFrame
			boxSize += 32; // bytesPerSample
		}

		if (soundversion == 2)
		{
			boxSize += 20 * 8; // soundVersion2Data
		}
		// boxSize += IsoStream.CalculateBoxSize(ChannelLayout); // ChannelLayout
		// boxSize += IsoStream.CalculateBoxSize(DownMixInstructions); // DownMixInstructions
		// boxSize += IsoStream.CalculateBoxSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
		// boxSize += IsoStream.CalculateBoxSize(DRCInstructionsBasic); // DRCInstructionsBasic
		// boxSize += IsoStream.CalculateBoxSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
		// boxSize += IsoStream.CalculateBoxSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
		// boxSize += IsoStream.CalculateBoxSize(UniDrcConfigExtension); // UniDrcConfigExtension
		// boxSize += IsoStream.CalculateBoxSize(SamplingRateBox); // SamplingRateBox
		// boxSize += IsoStream.CalculateBoxSize(Box); // Box
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
