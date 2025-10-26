using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class MVDDecoderConfigurationRecord { 
unsigned int(8) configurationVersion = 1; 
unsigned int(8) AVCProfileIndication; 
unsigned int(8) profile_compatibility; 
unsigned int(8) AVCLevelIndication;  
 bit(1) complete_representation; 
 bit(1) explicit_au_track; 
bit(4) reserved = '1111'b; 
unsigned int(2) lengthSizeMinusOne;  
bit(1) reserved = '0'b; 
unsigned int(7) numOfSequenceParameterSets; 
for (i=0; i< numOfSequenceParameterSets; i++) { 
unsigned int(16) sequenceParameterSetLength ; 
  bit(8*sequenceParameterSetLength) sequenceParameterSetNALUnit; 
 } 
unsigned int(8) numOfPictureParameterSets; 
for (i=0; i< numOfPictureParameterSets; i++) { 
  unsigned int(16) pictureParameterSetLength; 
  bit(8*pictureParameterSetLength) pictureParameterSetNALUnit; 
 } 
}
*/
public partial class MVDDecoderConfigurationRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "MVDDecoderConfigurationRecord"; } }

	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected byte AVCProfileIndication; 
	public byte _AVCProfileIndication { get { return this.AVCProfileIndication; } set { this.AVCProfileIndication = value; } }

	protected byte profile_compatibility; 
	public byte ProfileCompatibility { get { return this.profile_compatibility; } set { this.profile_compatibility = value; } }

	protected byte AVCLevelIndication; 
	public byte _AVCLevelIndication { get { return this.AVCLevelIndication; } set { this.AVCLevelIndication = value; } }

	protected bool complete_representation; 
	public bool CompleteRepresentation { get { return this.complete_representation; } set { this.complete_representation = value; } }

	protected bool explicit_au_track; 
	public bool ExplicitAuTrack { get { return this.explicit_au_track; } set { this.explicit_au_track = value; } }

	protected byte reserved = 0b1111; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte lengthSizeMinusOne; 
	public byte LengthSizeMinusOne { get { return this.lengthSizeMinusOne; } set { this.lengthSizeMinusOne = value; } }

	protected bool reserved0 = false; 
	public bool Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

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

	public MVDDecoderConfigurationRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.AVCProfileIndication, "AVCProfileIndication"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile_compatibility, "profile_compatibility"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.AVCLevelIndication, "AVCLevelIndication"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.complete_representation, "complete_representation"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.explicit_au_track, "explicit_au_track"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.numOfSequenceParameterSets, "numOfSequenceParameterSets"); 

		this.sequenceParameterSetLength = new ushort[IsoStream.GetInt( numOfSequenceParameterSets)];
		this.sequenceParameterSetNALUnit = new byte[IsoStream.GetInt( numOfSequenceParameterSets)][];
		for (int i=0; i< numOfSequenceParameterSets; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.sequenceParameterSetLength[i], "sequenceParameterSetLength"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*sequenceParameterSetLength[i] ),  out this.sequenceParameterSetNALUnit[i], "sequenceParameterSetNALUnit"); 
		}
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numOfPictureParameterSets, "numOfPictureParameterSets"); 

		this.pictureParameterSetLength = new ushort[IsoStream.GetInt( numOfPictureParameterSets)];
		this.pictureParameterSetNALUnit = new byte[IsoStream.GetInt( numOfPictureParameterSets)][];
		for (int i=0; i< numOfPictureParameterSets; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pictureParameterSetLength[i], "pictureParameterSetLength"); 
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*pictureParameterSetLength[i] ),  out this.pictureParameterSetNALUnit[i], "pictureParameterSetNALUnit"); 
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
		boxSize += stream.WriteBit( this.complete_representation, "complete_representation"); 
		boxSize += stream.WriteBit( this.explicit_au_track, "explicit_au_track"); 
		boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
		boxSize += stream.WriteBits(2,  this.lengthSizeMinusOne, "lengthSizeMinusOne"); 
		boxSize += stream.WriteBit( this.reserved0, "reserved0"); 
		boxSize += stream.WriteBits(7,  this.numOfSequenceParameterSets, "numOfSequenceParameterSets"); 

		for (int i=0; i< numOfSequenceParameterSets; i++)
		{
			boxSize += stream.WriteUInt16( this.sequenceParameterSetLength[i], "sequenceParameterSetLength"); 
			boxSize += stream.WriteBits((uint)(8*sequenceParameterSetLength[i] ),  this.sequenceParameterSetNALUnit[i], "sequenceParameterSetNALUnit"); 
		}
		boxSize += stream.WriteUInt8( this.numOfPictureParameterSets, "numOfPictureParameterSets"); 

		for (int i=0; i< numOfPictureParameterSets; i++)
		{
			boxSize += stream.WriteUInt16( this.pictureParameterSetLength[i], "pictureParameterSetLength"); 
			boxSize += stream.WriteBits((uint)(8*pictureParameterSetLength[i] ),  this.pictureParameterSetNALUnit[i], "pictureParameterSetNALUnit"); 
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
		boxSize += 1; // complete_representation
		boxSize += 1; // explicit_au_track
		boxSize += 4; // reserved
		boxSize += 2; // lengthSizeMinusOne
		boxSize += 1; // reserved0
		boxSize += 7; // numOfSequenceParameterSets

		for (int i=0; i< numOfSequenceParameterSets; i++)
		{
			boxSize += 16; // sequenceParameterSetLength
			boxSize += (ulong)(8*sequenceParameterSetLength[i] ); // sequenceParameterSetNALUnit
		}
		boxSize += 8; // numOfPictureParameterSets

		for (int i=0; i< numOfPictureParameterSets; i++)
		{
			boxSize += 16; // pictureParameterSetLength
			boxSize += (ulong)(8*pictureParameterSetLength[i] ); // pictureParameterSetNALUnit
		}
		return boxSize;
	}
}

}
