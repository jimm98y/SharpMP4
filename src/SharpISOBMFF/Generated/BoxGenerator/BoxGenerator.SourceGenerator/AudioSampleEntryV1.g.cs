using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AudioSampleEntryV1(codingname) extends SampleEntry (codingname){
	unsigned int(16) entry_version;	// shall be 1, 
	// and shall be in an stsd with version ==1
	const unsigned int(16)[3] reserved = 0;
	template unsigned int(16) channelcount;	// shall be correct
	template unsigned int(16) samplesize = 16;
	unsigned int(16) pre_defined = 0;
	const unsigned int(16) reserved = 0 ;
	template unsigned int(32) samplerate = 1<<16;
	// optional boxes follow
	SamplingRateBox();
	Box ();		// further boxes as needed
	ChannelLayout();
	DownMixInstructions() [];
	DRCCoefficientsBasic() [];
	DRCInstructionsBasic() [];
	DRCCoefficientsUniDRC() [];
	DRCInstructionsUniDRC() [];
	// we permit only one DRC Extension box:
	UniDrcConfigExtension();
	// optional boxes follow
	ChannelLayout();
}
*/
public partial class AudioSampleEntryV1 : SampleEntry
{
	public override string DisplayName { get { return "AudioSampleEntryV1"; } }

	protected ushort entry_version;  //  shall be 1, 
	public ushort EntryVersion { get { return this.entry_version; } set { this.entry_version = value; } }

	protected ushort[] reserved = []; 
	public ushort[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected ushort channelcount;  //  shall be correct
	public ushort Channelcount { get { return this.channelcount; } set { this.channelcount = value; } }

	protected ushort samplesize = 16; 
	public ushort Samplesize { get { return this.samplesize; } set { this.samplesize = value; } }

	protected ushort pre_defined = 0; 
	public ushort PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	protected ushort reserved0 = 0 ; 
	public ushort Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected uint samplerate = 1<<16;  //  optional boxes follow
	public uint Samplerate { get { return this.samplerate; } set { this.samplerate = value; } }
	public SamplingRateBox _SamplingRateBox { get { return this.children.OfType<SamplingRateBox>().FirstOrDefault(); } }
	public Box _Box { get { return this.children.OfType<Box>().FirstOrDefault(); } }
	public ChannelLayout _ChannelLayout { get { return this.children.OfType<ChannelLayout>().FirstOrDefault(); } }
	public IEnumerable<DownMixInstructions> _DownMixInstructions { get { return this.children.OfType<DownMixInstructions>(); } }
	public IEnumerable<DRCCoefficientsBasic> _DRCCoefficientsBasic { get { return this.children.OfType<DRCCoefficientsBasic>(); } }
	public IEnumerable<DRCInstructionsBasic> _DRCInstructionsBasic { get { return this.children.OfType<DRCInstructionsBasic>(); } }
	public IEnumerable<DRCCoefficientsUniDRC> _DRCCoefficientsUniDRC { get { return this.children.OfType<DRCCoefficientsUniDRC>(); } }
	public IEnumerable<DRCInstructionsUniDRC> _DRCInstructionsUniDRC { get { return this.children.OfType<DRCInstructionsUniDRC>(); } }
	public UniDrcConfigExtension _UniDrcConfigExtension { get { return this.children.OfType<UniDrcConfigExtension>().FirstOrDefault(); } }

	public AudioSampleEntryV1(uint codingname = 0): base(codingname)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_version, "entry_version"); // shall be 1, 
		/*  and shall be in an stsd with version ==1 */
		boxSize += stream.ReadUInt16Array(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channelcount, "channelcount"); // shall be correct
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.samplesize, "samplesize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pre_defined, "pre_defined"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved0, "reserved0"); 
		if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadUInt32(boxSize, readSize,  out this.samplerate, "samplerate"); // optional boxes follow
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SamplingRateBox, "SamplingRateBox"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.Box, "Box"); // further boxes as needed
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ChannelLayout, "ChannelLayout"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DownMixInstructions, "DownMixInstructions"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCCoefficientsBasic, "DRCCoefficientsBasic"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCInstructionsBasic, "DRCInstructionsBasic"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCCoefficientsUniDRC, "DRCCoefficientsUniDRC"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DRCInstructionsUniDRC, "DRCInstructionsUniDRC"); // we permit only one DRC Extension box:
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.UniDrcConfigExtension, "UniDrcConfigExtension"); // optional boxes follow
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ChannelLayout, "ChannelLayout"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entry_version, "entry_version"); // shall be 1, 
		/*  and shall be in an stsd with version ==1 */
		boxSize += stream.WriteUInt16Array(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt16( this.channelcount, "channelcount"); // shall be correct
		boxSize += stream.WriteUInt16( this.samplesize, "samplesize"); 
		boxSize += stream.WriteUInt16( this.pre_defined, "pre_defined"); 
		boxSize += stream.WriteUInt16( this.reserved0, "reserved0"); 
		boxSize += stream.WriteUInt32( this.samplerate, "samplerate"); // optional boxes follow
		// boxSize += stream.WriteBox( this.SamplingRateBox, "SamplingRateBox"); 
		// boxSize += stream.WriteBox( this.Box, "Box"); // further boxes as needed
		// boxSize += stream.WriteBox( this.ChannelLayout, "ChannelLayout"); 
		// boxSize += stream.WriteBox( this.DownMixInstructions, "DownMixInstructions"); 
		// boxSize += stream.WriteBox( this.DRCCoefficientsBasic, "DRCCoefficientsBasic"); 
		// boxSize += stream.WriteBox( this.DRCInstructionsBasic, "DRCInstructionsBasic"); 
		// boxSize += stream.WriteBox( this.DRCCoefficientsUniDRC, "DRCCoefficientsUniDRC"); 
		// boxSize += stream.WriteBox( this.DRCInstructionsUniDRC, "DRCInstructionsUniDRC"); // we permit only one DRC Extension box:
		// boxSize += stream.WriteBox( this.UniDrcConfigExtension, "UniDrcConfigExtension"); // optional boxes follow
		// boxSize += stream.WriteBox( this.ChannelLayout, "ChannelLayout"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entry_version
		/*  and shall be in an stsd with version ==1 */
		boxSize += 3 * 16; // reserved
		boxSize += 16; // channelcount
		boxSize += 16; // samplesize
		boxSize += 16; // pre_defined
		boxSize += 16; // reserved0
		boxSize += 32; // samplerate
		// boxSize += IsoStream.CalculateBoxSize(SamplingRateBox); // SamplingRateBox
		// boxSize += IsoStream.CalculateBoxSize(Box); // Box
		// boxSize += IsoStream.CalculateBoxSize(ChannelLayout); // ChannelLayout
		// boxSize += IsoStream.CalculateBoxSize(DownMixInstructions); // DownMixInstructions
		// boxSize += IsoStream.CalculateBoxSize(DRCCoefficientsBasic); // DRCCoefficientsBasic
		// boxSize += IsoStream.CalculateBoxSize(DRCInstructionsBasic); // DRCInstructionsBasic
		// boxSize += IsoStream.CalculateBoxSize(DRCCoefficientsUniDRC); // DRCCoefficientsUniDRC
		// boxSize += IsoStream.CalculateBoxSize(DRCInstructionsUniDRC); // DRCInstructionsUniDRC
		// boxSize += IsoStream.CalculateBoxSize(UniDrcConfigExtension); // UniDrcConfigExtension
		// boxSize += IsoStream.CalculateBoxSize(ChannelLayout); // ChannelLayout
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
