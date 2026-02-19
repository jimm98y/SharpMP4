using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class ELDSpecificConfig(channelConfiguration)
{
  bslbf(1) frameLengthFlag;
  bslbf(1) aacSectionDataResilienceFlag;
  bslbf(1) aacScalefactorDataResilienceFlag;
  bslbf(1) aacSpectralDataResilienceFlag;

  bslbf(1) ldSbrPresentFlag;
  if (ldSbrPresentFlag) {
    bslbf(1) ldSbrSamplingRate;
    bslbf(1) ldSbrCrcFlag;
    ld_sbr_header(channelConfiguration);
  }

  bslbf(4) eldExtType;
while (eldExtType != ELDEXT_TERM) {
    uimsbf(4) eldExtLen;
    len = eldExtLen;
    if (eldExtLen == 15) {
      uimsbf(8) eldExtLenAdd;
      len += eldExtLenAdd;
    }
    if (eldExtLenAdd == 255) {
      uimsbf(16) eldExtLenAddAdd;
      len += eldExtLenAddAdd;
    }
    switch (eldExtType) {
      /* add future eld extension configs here *//*
      default:
        int cntt;
        for (cnt = 0; cnt < len; cnt++) {
           uimsbf(8) other_byte;
        }
        break;
    }
bslbf(4) eldExtType;
  }
}


*/
public partial class ELDSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ELDSpecificConfig"; } }

	protected bool frameLengthFlag; 
	public bool FrameLengthFlag { get { return this.frameLengthFlag; } set { this.frameLengthFlag = value; } }

	protected bool aacSectionDataResilienceFlag; 
	public bool AacSectionDataResilienceFlag { get { return this.aacSectionDataResilienceFlag; } set { this.aacSectionDataResilienceFlag = value; } }

	protected bool aacScalefactorDataResilienceFlag; 
	public bool AacScalefactorDataResilienceFlag { get { return this.aacScalefactorDataResilienceFlag; } set { this.aacScalefactorDataResilienceFlag = value; } }

	protected bool aacSpectralDataResilienceFlag; 
	public bool AacSpectralDataResilienceFlag { get { return this.aacSpectralDataResilienceFlag; } set { this.aacSpectralDataResilienceFlag = value; } }

	protected bool ldSbrPresentFlag; 
	public bool LdSbrPresentFlag { get { return this.ldSbrPresentFlag; } set { this.ldSbrPresentFlag = value; } }

	protected bool ldSbrSamplingRate; 
	public bool LdSbrSamplingRate { get { return this.ldSbrSamplingRate; } set { this.ldSbrSamplingRate = value; } }

	protected bool ldSbrCrcFlag; 
	public bool LdSbrCrcFlag { get { return this.ldSbrCrcFlag; } set { this.ldSbrCrcFlag = value; } }

	protected ld_sbr_header ld_sbr_header; 
	public ld_sbr_header LdSbrHeader { get { return this.ld_sbr_header; } set { this.ld_sbr_header = value; } }

	protected byte eldExtType; 
	public byte EldExtType { get { return this.eldExtType; } set { this.eldExtType = value; } }

	protected byte eldExtLen; 
	public byte EldExtLen { get { return this.eldExtLen; } set { this.eldExtLen = value; } }

	protected byte eldExtLenAdd; 
	public byte EldExtLenAdd { get { return this.eldExtLenAdd; } set { this.eldExtLenAdd = value; } }

	protected ushort eldExtLenAddAdd; 
	public ushort EldExtLenAddAdd { get { return this.eldExtLenAddAdd; } set { this.eldExtLenAddAdd = value; } }

	protected int cntt; 
	public int Cntt { get { return this.cntt; } set { this.cntt = value; } }

	protected byte[] other_byte; 
	public byte[] OtherByte { get { return this.other_byte; } set { this.other_byte = value; } }

	protected int channelConfiguration; 
	public int ChannelConfiguration { get { return this.channelConfiguration; } set { this.channelConfiguration = value; } }

	public ELDSpecificConfig(int channelConfiguration): base()
	{
		this.channelConfiguration = channelConfiguration;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		int len = 0;

		const byte ELDEXT_TERM = 0;

		boxSize += stream.ReadBit(boxSize, readSize,  out this.frameLengthFlag, "frameLengthFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aacSectionDataResilienceFlag, "aacSectionDataResilienceFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aacScalefactorDataResilienceFlag, "aacScalefactorDataResilienceFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.aacSpectralDataResilienceFlag, "aacSpectralDataResilienceFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.ldSbrPresentFlag, "ldSbrPresentFlag"); 

		if (ldSbrPresentFlag)
		{
			boxSize += stream.ReadBit(boxSize, readSize,  out this.ldSbrSamplingRate, "ldSbrSamplingRate"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.ldSbrCrcFlag, "ldSbrCrcFlag"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ld_sbr_header(channelConfiguration),  out this.ld_sbr_header, "ld_sbr_header"); 
		}
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.eldExtType, "eldExtType"); 

		while (eldExtType != ELDEXT_TERM)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.eldExtLen, "eldExtLen"); 
			len = eldExtLen;

			if (eldExtLen == 15)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.eldExtLenAdd, "eldExtLenAdd"); 
				len += eldExtLenAdd;
			}

			if (eldExtLenAdd == 255)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.eldExtLenAddAdd, "eldExtLenAddAdd"); 
				len += eldExtLenAddAdd;
			}

			switch (eldExtType)
			{
				/*  add future eld extension configs here  */
				default:

				boxSize += stream.ReadInt32(boxSize, readSize,  out this.cntt, "cntt"); 

				this.other_byte = new byte[IsoStream.GetInt( len)];
				for (int cnt = 0; cnt < len; cnt++)
				{
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.other_byte[cnt], "other_byte"); 
				}
				break;

			}
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.eldExtType, "eldExtType"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		int len = 0;

		const byte ELDEXT_TERM = 0;

		boxSize += stream.WriteBit( this.frameLengthFlag, "frameLengthFlag"); 
		boxSize += stream.WriteBit( this.aacSectionDataResilienceFlag, "aacSectionDataResilienceFlag"); 
		boxSize += stream.WriteBit( this.aacScalefactorDataResilienceFlag, "aacScalefactorDataResilienceFlag"); 
		boxSize += stream.WriteBit( this.aacSpectralDataResilienceFlag, "aacSpectralDataResilienceFlag"); 
		boxSize += stream.WriteBit( this.ldSbrPresentFlag, "ldSbrPresentFlag"); 

		if (ldSbrPresentFlag)
		{
			boxSize += stream.WriteBit( this.ldSbrSamplingRate, "ldSbrSamplingRate"); 
			boxSize += stream.WriteBit( this.ldSbrCrcFlag, "ldSbrCrcFlag"); 
			boxSize += stream.WriteClass( this.ld_sbr_header, "ld_sbr_header"); 
		}
		boxSize += stream.WriteBits(4,  this.eldExtType, "eldExtType"); 

		while (eldExtType != ELDEXT_TERM)
		{
			boxSize += stream.WriteBits(4,  this.eldExtLen, "eldExtLen"); 
			len = eldExtLen;

			if (eldExtLen == 15)
			{
				boxSize += stream.WriteUInt8( this.eldExtLenAdd, "eldExtLenAdd"); 
				len += eldExtLenAdd;
			}

			if (eldExtLenAdd == 255)
			{
				boxSize += stream.WriteUInt16( this.eldExtLenAddAdd, "eldExtLenAddAdd"); 
				len += eldExtLenAddAdd;
			}

			switch (eldExtType)
			{
				/*  add future eld extension configs here  */
				default:

				boxSize += stream.WriteInt32( this.cntt, "cntt"); 

				for (int cnt = 0; cnt < len; cnt++)
				{
					boxSize += stream.WriteUInt8( this.other_byte[cnt], "other_byte"); 
				}
				break;

			}
			boxSize += stream.WriteBits(4,  this.eldExtType, "eldExtType"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		int len = 0;

		const byte ELDEXT_TERM = 0;

		boxSize += 1; // frameLengthFlag
		boxSize += 1; // aacSectionDataResilienceFlag
		boxSize += 1; // aacScalefactorDataResilienceFlag
		boxSize += 1; // aacSpectralDataResilienceFlag
		boxSize += 1; // ldSbrPresentFlag

		if (ldSbrPresentFlag)
		{
			boxSize += 1; // ldSbrSamplingRate
			boxSize += 1; // ldSbrCrcFlag
			boxSize += IsoStream.CalculateClassSize(ld_sbr_header); // ld_sbr_header
		}
		boxSize += 4; // eldExtType

		while (eldExtType != ELDEXT_TERM)
		{
			boxSize += 4; // eldExtLen
			len = eldExtLen;

			if (eldExtLen == 15)
			{
				boxSize += 8; // eldExtLenAdd
				len += eldExtLenAdd;
			}

			if (eldExtLenAdd == 255)
			{
				boxSize += 16; // eldExtLenAddAdd
				len += eldExtLenAddAdd;
			}

			switch (eldExtType)
			{
				/*  add future eld extension configs here  */
				default:

				boxSize += 32; // cntt

				for (int cnt = 0; cnt < len; cnt++)
				{
					boxSize += 8; // other_byte
				}
				break;

			}
			boxSize += 4; // eldExtType
		}
		return boxSize;
	}
}

}
