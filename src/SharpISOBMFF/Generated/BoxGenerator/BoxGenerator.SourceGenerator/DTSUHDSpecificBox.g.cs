using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// B.2.2.3.1, Table B-2; NumPresentations = NumPresentationsCode + 1, as m_ucNumAudioPres is
class DTSUHDSpecificBox extends Box('udts') {
 bit(6) DecoderProfileCode;
 bit(2) FrameDurationCode;
 bit(3) MaxPayloadCode;
 bit(5) NumPresentationsCode;
 unsigned int(32) ChannelMask;
 bit(1) BaseSamplingFrequencyCode;
 bit(2) SampleRateMod;
 bit(3) RepresentationType;
 bit(3) StreamIndex;
 bit(1) ExpansionBoxPresent;
 for (i = 0; i < NumPresentationsCode + 1; i++) {
  bit(1) IDTagPresent; // bit(NumPresentations) IDTagPresent[NumPresentations]
 }
 bit((8 - ((59 + NumPresentationsCode) % 8)) % 8) ByteAlign; // [0..7] to a byte: 58 bits and one a presentation are before it
 for (i = 0; i < NumPresentationsCode + 1; i++) {
  if (IDTagPresent[i]) {
   unsigned int(8) PresentationIDTag[16];
  }
 }
 if (ExpansionBoxPresent) {
  Box DTSExpansionBox[];
 }
}
*/
public partial class DTSUHDSpecificBox : Box
{
	public const string TYPE = "udts";
	public override string DisplayName { get { return "DTSUHDSpecificBox"; } }

	protected byte DecoderProfileCode; 
	public byte _DecoderProfileCode { get { return this.DecoderProfileCode; } set { this.DecoderProfileCode = value; } }

	protected byte FrameDurationCode; 
	public byte _FrameDurationCode { get { return this.FrameDurationCode; } set { this.FrameDurationCode = value; } }

	protected byte MaxPayloadCode; 
	public byte _MaxPayloadCode { get { return this.MaxPayloadCode; } set { this.MaxPayloadCode = value; } }

	protected byte NumPresentationsCode; 
	public byte _NumPresentationsCode { get { return this.NumPresentationsCode; } set { this.NumPresentationsCode = value; } }

	protected uint ChannelMask; 
	public uint _ChannelMask { get { return this.ChannelMask; } set { this.ChannelMask = value; } }

	protected bool BaseSamplingFrequencyCode; 
	public bool _BaseSamplingFrequencyCode { get { return this.BaseSamplingFrequencyCode; } set { this.BaseSamplingFrequencyCode = value; } }

	protected byte SampleRateMod; 
	public byte _SampleRateMod { get { return this.SampleRateMod; } set { this.SampleRateMod = value; } }

	protected byte RepresentationType; 
	public byte _RepresentationType { get { return this.RepresentationType; } set { this.RepresentationType = value; } }

	protected byte StreamIndex; 
	public byte _StreamIndex { get { return this.StreamIndex; } set { this.StreamIndex = value; } }

	protected bool ExpansionBoxPresent; 
	public bool _ExpansionBoxPresent { get { return this.ExpansionBoxPresent; } set { this.ExpansionBoxPresent = value; } }

	protected bool[] IDTagPresent;  //  bit(NumPresentations) IDTagPresent[NumPresentations]
	public bool[] _IDTagPresent { get { return this.IDTagPresent; } set { this.IDTagPresent = value; } }

	protected byte[] ByteAlign;  //  [0..7] to a byte: 58 bits and one a presentation are before it
	public byte[] _ByteAlign { get { return this.ByteAlign; } set { this.ByteAlign = value; } }

	protected byte[][] PresentationIDTag; 
	public byte[][] _PresentationIDTag { get { return this.PresentationIDTag; } set { this.PresentationIDTag = value; } }
	public IEnumerable<Box> _DTSExpansionBox { get { return this.children.OfType<Box>(); } }

	public DTSUHDSpecificBox(): base(IsoStream.FromFourCC("udts"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.DecoderProfileCode, "DecoderProfileCode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.FrameDurationCode, "FrameDurationCode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.MaxPayloadCode, "MaxPayloadCode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.NumPresentationsCode, "NumPresentationsCode"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ChannelMask, "ChannelMask"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.BaseSamplingFrequencyCode, "BaseSamplingFrequencyCode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.SampleRateMod, "SampleRateMod"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.RepresentationType, "RepresentationType"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.StreamIndex, "StreamIndex"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ExpansionBoxPresent, "ExpansionBoxPresent"); 

		this.IDTagPresent = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( NumPresentationsCode + 1), "IDTagPresent");
		for (int i = 0; i < NumPresentationsCode + 1; i++)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.IDTagPresent[i], "IDTagPresent"); // bit(NumPresentations) IDTagPresent[NumPresentations]
		}
		boxSize += stream.ReadBits(boxSize, readSize, (uint)((8 - ((59 + NumPresentationsCode) % 8)) % 8 ),  out this.ByteAlign, "ByteAlign"); // [0..7] to a byte: 58 bits and one a presentation are before it

		this.PresentationIDTag = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( NumPresentationsCode + 1), "PresentationIDTag");
		for (int i = 0; i < NumPresentationsCode + 1; i++)
		{

			if (IDTagPresent[i])
			{
				boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.PresentationIDTag[i], "PresentationIDTag"); 
			}
		}

		if (ExpansionBoxPresent)
		{
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.DTSExpansionBox, "DTSExpansionBox"); 
		}
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(6,  this.DecoderProfileCode, "DecoderProfileCode"); 
		boxSize += stream.WriteBits(2,  this.FrameDurationCode, "FrameDurationCode"); 
		boxSize += stream.WriteBits(3,  this.MaxPayloadCode, "MaxPayloadCode"); 
		boxSize += stream.WriteBits(5,  this.NumPresentationsCode, "NumPresentationsCode"); 
		boxSize += stream.WriteUInt32( this.ChannelMask, "ChannelMask"); 
		boxSize += stream.WriteBit( this.BaseSamplingFrequencyCode, "BaseSamplingFrequencyCode"); 
		boxSize += stream.WriteBits(2,  this.SampleRateMod, "SampleRateMod"); 
		boxSize += stream.WriteBits(3,  this.RepresentationType, "RepresentationType"); 
		boxSize += stream.WriteBits(3,  this.StreamIndex, "StreamIndex"); 
		boxSize += stream.WriteBit( this.ExpansionBoxPresent, "ExpansionBoxPresent"); 

		for (int i = 0; i < NumPresentationsCode + 1; i++)
		{
			boxSize += stream.WriteBit( this.IDTagPresent[i], "IDTagPresent"); // bit(NumPresentations) IDTagPresent[NumPresentations]
		}
		boxSize += stream.WriteBits((uint)((8 - ((59 + NumPresentationsCode) % 8)) % 8 ),  this.ByteAlign, "ByteAlign"); // [0..7] to a byte: 58 bits and one a presentation are before it

		for (int i = 0; i < NumPresentationsCode + 1; i++)
		{

			if (IDTagPresent[i])
			{
				boxSize += stream.WriteUInt8Array(16,  this.PresentationIDTag[i], "PresentationIDTag"); 
			}
		}

		if (ExpansionBoxPresent)
		{
			// boxSize += stream.WriteBox( this.DTSExpansionBox, "DTSExpansionBox"); 
		}
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 6; // DecoderProfileCode
		boxSize += 2; // FrameDurationCode
		boxSize += 3; // MaxPayloadCode
		boxSize += 5; // NumPresentationsCode
		boxSize += 32; // ChannelMask
		boxSize += 1; // BaseSamplingFrequencyCode
		boxSize += 2; // SampleRateMod
		boxSize += 3; // RepresentationType
		boxSize += 3; // StreamIndex
		boxSize += 1; // ExpansionBoxPresent

		for (int i = 0; i < NumPresentationsCode + 1; i++)
		{
			boxSize += 1; // IDTagPresent
		}
		boxSize += (ulong)((8 - ((59 + NumPresentationsCode) % 8)) % 8 ); // ByteAlign

		for (int i = 0; i < NumPresentationsCode + 1; i++)
		{

			if (IDTagPresent[i])
			{
				boxSize += 16 * 8; // PresentationIDTag
			}
		}

		if (ExpansionBoxPresent)
		{
			// boxSize += IsoStream.CalculateBoxSize(DTSExpansionBox); // DTSExpansionBox
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
