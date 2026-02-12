using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AVCDecoderConfigurationRecord { 
 unsigned int(8) configurationVersion = 1; 
 unsigned int(8) AVCProfileIndication; 
 unsigned int(8) profile_compatibility; 
 unsigned int(8) AVCLevelIndication;  
 bit(6) reserved = '111111'b; 
 unsigned int(2) lengthSizeMinusOne;  
 bit(3) reserved = '111'b; 
 unsigned int(5) numOfSequenceParameterSets; 
 for (i=0; i< numOfSequenceParameterSets;  i++) { 
  unsigned int(16) sequenceParameterSetLength ; 
  bit(8*sequenceParameterSetLength) sequenceParameterSetNALUnit; 
 } 
 unsigned int(8) numOfPictureParameterSets; 
 for (i=0; i< numOfPictureParameterSets;  i++) { 
  unsigned int(16) pictureParameterSetLength; 
  bit(8*pictureParameterSetLength) pictureParameterSetNALUnit; 
 } 
 if( AVCProfileIndication  ==  100  ||  AVCProfileIndication  ==  110  || 
    AVCProfileIndication  ==  122  ||  AVCProfileIndication  ==  144 ) 
 { 
  bit(6) reserved = '111111'b; 
  unsigned int(2) chroma_format; 
  bit(5) reserved = '11111'b; 
  unsigned int(3) bit_depth_luma_minus8; 
  bit(5) reserved = '11111'b; 
  unsigned int(3) bit_depth_chroma_minus8; 
  unsigned int(8) numOfSequenceParameterSetExt; 
  for (i=0; i< numOfSequenceParameterSetExt; i++) { 
   unsigned int(16) sequenceParameterSetExtLength; 
   bit(8*sequenceParameterSetExtLength) sequenceParameterSetExtNALUnit; 
  }
 } 
}
*/
public partial class AVCDecoderConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AVCDecoderConfigurationRecord"; } }

	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected byte AVCProfileIndication; 
	public byte _AVCProfileIndication { get { return this.AVCProfileIndication; } set { this.AVCProfileIndication = value; } }

	protected byte profile_compatibility; 
	public byte ProfileCompatibility { get { return this.profile_compatibility; } set { this.profile_compatibility = value; } }

	protected byte AVCLevelIndication; 
	public byte _AVCLevelIndication { get { return this.AVCLevelIndication; } set { this.AVCLevelIndication = value; } }

	protected byte reserved = 0b111111; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte lengthSizeMinusOne; 
	public byte LengthSizeMinusOne { get { return this.lengthSizeMinusOne; } set { this.lengthSizeMinusOne = value; } }

	protected byte reserved0 = 0b111; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte numOfSequenceParameterSets; 
	public byte NumOfSequenceParameterSets { get { return this.numOfSequenceParameterSets; } set { this.numOfSequenceParameterSets = value; } }

	protected ushort[] sequenceParameterSetLength; 
	public ushort[] SequenceParameterSetLength { get { return this.sequenceParameterSetLength; } set { this.sequenceParameterSetLength = value; } }

	protected byte[][] sequenceParameterSetNALUnit; 
	public byte[][] SequenceParameterSetNALUnit { get { return this.sequenceParameterSetNALUnit; } set { this.sequenceParameterSetNALUnit = value; } }

	protected byte numOfPictureParameterSets; 
	public byte NumOfPictureParameterSets { get { return this.numOfPictureParameterSets; } set { this.numOfPictureParameterSets = value; } }

	protected ushort[] pictureParameterSetLength; 
	public ushort[] PictureParameterSetLength { get { return this.pictureParameterSetLength; } set { this.pictureParameterSetLength = value; } }

	protected byte[][] pictureParameterSetNALUnit; 
	public byte[][] PictureParameterSetNALUnit { get { return this.pictureParameterSetNALUnit; } set { this.pictureParameterSetNALUnit = value; } }

	protected byte reserved1 = 0b111111; 
	public byte Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte chroma_format; 
	public byte ChromaFormat { get { return this.chroma_format; } set { this.chroma_format = value; } }

	protected byte reserved00 = 0b11111; 
	public byte Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	protected byte bit_depth_luma_minus8; 
	public byte BitDepthLumaMinus8 { get { return this.bit_depth_luma_minus8; } set { this.bit_depth_luma_minus8 = value; } }

	protected byte reserved10 = 0b11111; 
	public byte Reserved10 { get { return this.reserved10; } set { this.reserved10 = value; } }

	protected byte bit_depth_chroma_minus8; 
	public byte BitDepthChromaMinus8 { get { return this.bit_depth_chroma_minus8; } set { this.bit_depth_chroma_minus8 = value; } }

	protected byte numOfSequenceParameterSetExt; 
	public byte NumOfSequenceParameterSetExt { get { return this.numOfSequenceParameterSetExt; } set { this.numOfSequenceParameterSetExt = value; } }

	protected ushort[] sequenceParameterSetExtLength; 
	public ushort[] SequenceParameterSetExtLength { get { return this.sequenceParameterSetExtLength; } set { this.sequenceParameterSetExtLength = value; } }

	protected byte[][] sequenceParameterSetExtNALUnit; 
	public byte[][] SequenceParameterSetExtNALUnit { get { return this.sequenceParameterSetExtNALUnit; } set { this.sequenceParameterSetExtNALUnit = value; } }
public bool HasExtensions { get; set; } = false;

	public AVCDecoderConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.AVCProfileIndication, "AVCProfileIndication"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile_compatibility, "profile_compatibility"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.AVCLevelIndication, "AVCLevelIndication"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.numOfSequenceParameterSets, "numOfSequenceParameterSets"); 

		this.sequenceParameterSetLength = new ushort[IsoStream.GetInt( numOfSequenceParameterSets)];
		this.sequenceParameterSetNALUnit = new byte[IsoStream.GetInt( numOfSequenceParameterSets)][];
		for (int i=0; i< numOfSequenceParameterSets;  i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.sequenceParameterSetLength[i], "sequenceParameterSetLength"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*sequenceParameterSetLength[i] ),  out this.sequenceParameterSetNALUnit[i], "sequenceParameterSetNALUnit"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numOfPictureParameterSets, "numOfPictureParameterSets"); 

		this.pictureParameterSetLength = new ushort[IsoStream.GetInt( numOfPictureParameterSets)];
		this.pictureParameterSetNALUnit = new byte[IsoStream.GetInt( numOfPictureParameterSets)][];
		for (int i=0; i< numOfPictureParameterSets;  i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pictureParameterSetLength[i], "pictureParameterSetLength"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*pictureParameterSetLength[i] ),  out this.pictureParameterSetNALUnit[i], "pictureParameterSetNALUnit"); 
		}

		if (boxSize >= readSize || (readSize - boxSize) < 4) return boxSize; else HasExtensions = true;
		if ( AVCProfileIndication  ==  100  ||  AVCProfileIndication  ==  110  || 
    AVCProfileIndication  ==  122  ||  AVCProfileIndication  ==  144 )
		{
			boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved1, "reserved1"); 
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.chroma_format, "chroma_format"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved00, "reserved00"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_luma_minus8, "bit_depth_luma_minus8"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved10, "reserved10"); 
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.bit_depth_chroma_minus8, "bit_depth_chroma_minus8"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numOfSequenceParameterSetExt, "numOfSequenceParameterSetExt"); 

			this.sequenceParameterSetExtLength = new ushort[IsoStream.GetInt( numOfSequenceParameterSetExt)];
			this.sequenceParameterSetExtNALUnit = new byte[IsoStream.GetInt( numOfSequenceParameterSetExt)][];
			for (int i=0; i< numOfSequenceParameterSetExt; i++)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.sequenceParameterSetExtLength[i], "sequenceParameterSetExtLength"); 
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*sequenceParameterSetExtLength[i] ),  out this.sequenceParameterSetExtNALUnit[i], "sequenceParameterSetExtNALUnit"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.configurationVersion, "configurationVersion"); 
		boxSize += stream.WriteUInt8( this.AVCProfileIndication, "AVCProfileIndication"); 
		boxSize += stream.WriteUInt8( this.profile_compatibility, "profile_compatibility"); 
		boxSize += stream.WriteUInt8( this.AVCLevelIndication, "AVCLevelIndication"); 
		boxSize += stream.WriteBits(6,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.WriteBits(3,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteBits(5,  this.numOfSequenceParameterSets, "numOfSequenceParameterSets"); 

		for (int i=0; i< numOfSequenceParameterSets;  i++)
		{
			boxSize += stream.WriteUInt16( this.sequenceParameterSetLength[i], "sequenceParameterSetLength"); 
			boxSize += stream.WriteBits((uint)(8*sequenceParameterSetLength[i] ),  this.sequenceParameterSetNALUnit[i], "sequenceParameterSetNALUnit"); 
		}
		boxSize += stream.WriteUInt8( this.numOfPictureParameterSets, "numOfPictureParameterSets"); 

		for (int i=0; i< numOfPictureParameterSets;  i++)
		{
			boxSize += stream.WriteUInt16( this.pictureParameterSetLength[i], "pictureParameterSetLength"); 
			boxSize += stream.WriteBits((uint)(8*pictureParameterSetLength[i] ),  this.pictureParameterSetNALUnit[i], "pictureParameterSetNALUnit"); 
		}

		if (!HasExtensions) return boxSize;
		if ( AVCProfileIndication  ==  100  ||  AVCProfileIndication  ==  110  || 
    AVCProfileIndication  ==  122  ||  AVCProfileIndication  ==  144 )
		{
			boxSize += stream.WriteBits(6,  this.reserved1, "reserved1"); 
			boxSize += stream.WriteBits(2,  this.chroma_format, "chroma_format"); 
			boxSize += stream.WriteBits(5,  this.reserved00, "reserved00"); 
			boxSize += stream.WriteBits(3,  this.bit_depth_luma_minus8, "bit_depth_luma_minus8"); 
			boxSize += stream.WriteBits(5,  this.reserved10, "reserved10"); 
			boxSize += stream.WriteBits(3,  this.bit_depth_chroma_minus8, "bit_depth_chroma_minus8"); 
			boxSize += stream.WriteUInt8( this.numOfSequenceParameterSetExt, "numOfSequenceParameterSetExt"); 

			for (int i=0; i< numOfSequenceParameterSetExt; i++)
			{
				boxSize += stream.WriteUInt16( this.sequenceParameterSetExtLength[i], "sequenceParameterSetExtLength"); 
				boxSize += stream.WriteBits((uint)(8*sequenceParameterSetExtLength[i] ),  this.sequenceParameterSetExtNALUnit[i], "sequenceParameterSetExtNALUnit"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // configurationVersion
		boxSize += 8; // AVCProfileIndication
		boxSize += 8; // profile_compatibility
		boxSize += 8; // AVCLevelIndication
		boxSize += 6; // reserved
		boxSize += 2; // lengthSizeMinusOne
		boxSize += 3; // reserved0
		boxSize += 5; // numOfSequenceParameterSets

		for (int i=0; i< numOfSequenceParameterSets;  i++)
		{
			boxSize += 16; // sequenceParameterSetLength
			boxSize += (ulong)(8*sequenceParameterSetLength[i] ); // sequenceParameterSetNALUnit
		}
		boxSize += 8; // numOfPictureParameterSets

		for (int i=0; i< numOfPictureParameterSets;  i++)
		{
			boxSize += 16; // pictureParameterSetLength
			boxSize += (ulong)(8*pictureParameterSetLength[i] ); // pictureParameterSetNALUnit
		}

		if (!HasExtensions) return boxSize;
		if ( AVCProfileIndication  ==  100  ||  AVCProfileIndication  ==  110  || 
    AVCProfileIndication  ==  122  ||  AVCProfileIndication  ==  144 )
		{
			boxSize += 6; // reserved1
			boxSize += 2; // chroma_format
			boxSize += 5; // reserved00
			boxSize += 3; // bit_depth_luma_minus8
			boxSize += 5; // reserved10
			boxSize += 3; // bit_depth_chroma_minus8
			boxSize += 8; // numOfSequenceParameterSetExt

			for (int i=0; i< numOfSequenceParameterSetExt; i++)
			{
				boxSize += 16; // sequenceParameterSetExtLength
				boxSize += (ulong)(8*sequenceParameterSetExtLength[i] ); // sequenceParameterSetExtNALUnit
			}
		}
		return boxSize;
	}
}

}
