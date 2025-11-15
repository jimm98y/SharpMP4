using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class SymbolicMusicSpecificConfig
{  // the bitstream header  
    bit(4) version; //version of this specification is 0b0000 
 
    unsigned int(12) pictureWidth; // rendering window X size 
    unsigned int(12) pictureHight; // rendering window Y size 
 
    bit(1) isScoreMultiwindow; // 0: one window only – 1: multiple windows 
 
    unsigned int(8) numberOfParts; // parts of the main score 
 
    unsigned int(3) notationFormat; // CWMN or other sets 
 
    vluimsbf8 urlMIDIStream_length; //length in bytes 
    byte(urlMIDIStream_length) urlMIDIStream; // reference to the MIDI stream, as url 
    bit(2) codingType; // coding of the XML chunks 
    vluimsbf8 length; //length in bits of decoder configuration, unsigned integer 
    // start of decoderConfiguration 
    if (codingType == 0b11) { 
       bit(3) decoderInitConfig; 
       if (decoderInitConfig == 0b000) { 
           bit(length-3)  decoderInit; 
       }
    } else
    {
      bit(length) reserved;
    }
    // end of decoderConfiguration 
    bit more_data; // 1 if yes, 0 if no 
    while (more_data)
    {
        aligned bit(3) chunk_type;
        bit(5) reserved; // for alignment 
        vluimsbf8 chunk_length;  // length of the chunk in byte 
        switch (chunk_type)
        {
            case 0b000:
                bit(8) sco[chunk_length];
                break;
            case 0b001:
                bit(8) part[chunk_length]; // ID of the part at which the following info refers 
                break;
            case 0b010:
                // this segment is always in binary as stated in Section 9 
                bit(8) sync[chunk_length];
                break;
            case 0b011:
                bit(8) fmt[chunk_length];
                break;
            case 0b100:
                bit(8) lyrics[chunk_length];
  break;
            case 0b101:
                // this segment is always in binary as stated in Section 11.4 
                bit(8) fon[chunk_length];
                break;
            case 0b110: // reserved;
            break;
            case 0b111: // reserved;
            break;
        }
        aligned bit(1) more_data;
        bit(7) reserved; //for alignment 
    } 
} 
*/
public partial class SymbolicMusicSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SymbolicMusicSpecificConfig"; } }

	protected byte version;  // version of this specification is 0b0000 
	public byte Version { get { return this.version; } set { this.version = value; } }

	protected ushort pictureWidth;  //  rendering window X size 
	public ushort PictureWidth { get { return this.pictureWidth; } set { this.pictureWidth = value; } }

	protected ushort pictureHight;  //  rendering window Y size 
	public ushort PictureHight { get { return this.pictureHight; } set { this.pictureHight = value; } }

	protected bool isScoreMultiwindow;  //  0: one window only – 1: multiple windows 
	public bool IsScoreMultiwindow { get { return this.isScoreMultiwindow; } set { this.isScoreMultiwindow = value; } }

	protected byte numberOfParts;  //  parts of the main score 
	public byte NumberOfParts { get { return this.numberOfParts; } set { this.numberOfParts = value; } }

	protected byte notationFormat;  //  CWMN or other sets 
	public byte NotationFormat { get { return this.notationFormat; } set { this.notationFormat = value; } }

	protected byte urlMIDIStream_length;  // length in bytes 
	public byte UrlMIDIStreamLength { get { return this.urlMIDIStream_length; } set { this.urlMIDIStream_length = value; } }

	protected byte[] urlMIDIStream;  //  reference to the MIDI stream, as url 
	public byte[] UrlMIDIStream { get { return this.urlMIDIStream; } set { this.urlMIDIStream = value; } }

	protected byte codingType;  //  coding of the XML chunks 
	public byte CodingType { get { return this.codingType; } set { this.codingType = value; } }

	protected byte length;  // length in bits of decoder configuration, unsigned integer 
	public byte Length { get { return this.length; } set { this.length = value; } }

	protected byte decoderInitConfig; 
	public byte DecoderInitConfig { get { return this.decoderInitConfig; } set { this.decoderInitConfig = value; } }

	protected byte[] decoderInit; 
	public byte[] DecoderInit { get { return this.decoderInit; } set { this.decoderInit = value; } }

	protected byte[] reserved; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool more_data;  //  1 if yes, 0 if no 
	public bool MoreData { get { return this.more_data; } set { this.more_data = value; } }

	protected byte chunk_type; 
	public byte ChunkType { get { return this.chunk_type; } set { this.chunk_type = value; } }

	protected byte reserved0;  //  for alignment 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte chunk_length;  //  length of the chunk in byte 
	public byte ChunkLength { get { return this.chunk_length; } set { this.chunk_length = value; } }

	protected byte[] sco; 
	public byte[] Sco { get { return this.sco; } set { this.sco = value; } }

	protected byte[] part;  //  ID of the part at which the following info refers 
	public byte[] Part { get { return this.part; } set { this.part = value; } }

	protected byte[] sync; 
	public byte[] Sync { get { return this.sync; } set { this.sync = value; } }

	protected byte[] fmt; 
	public byte[] Fmt { get { return this.fmt; } set { this.fmt = value; } }

	protected byte[] lyrics; 
	public byte[] Lyrics { get { return this.lyrics; } set { this.lyrics = value; } }

	protected byte[] fon; 
	public byte[] Fon { get { return this.fon; } set { this.fon = value; } }

	protected byte reserved00;  // for alignment 
	public byte Reserved00 { get { return this.reserved00; } set { this.reserved00 = value; } }

	public SymbolicMusicSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		/*  the bitstream header   */
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.version, "version"); //version of this specification is 0b0000 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.pictureWidth, "pictureWidth"); // rendering window X size 
		boxSize += stream.ReadBits(boxSize, readSize, 12,  out this.pictureHight, "pictureHight"); // rendering window Y size 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.isScoreMultiwindow, "isScoreMultiwindow"); // 0: one window only – 1: multiple windows 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numberOfParts, "numberOfParts"); // parts of the main score 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.notationFormat, "notationFormat"); // CWMN or other sets 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.urlMIDIStream_length, "urlMIDIStream_length"); //length in bytes 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(urlMIDIStream_length ),  out this.urlMIDIStream, "urlMIDIStream"); // reference to the MIDI stream, as url 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.codingType, "codingType"); // coding of the XML chunks 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.length, "length"); //length in bits of decoder configuration, unsigned integer 
		/*  start of decoderConfiguration  */

		if (codingType == 0b11)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.decoderInitConfig, "decoderInitConfig"); 

			if (decoderInitConfig == 0b000)
			{
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(length-3 ),  out this.decoderInit, "decoderInit"); 
			}
		}

		else 
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(length ),  out this.reserved, "reserved"); 
		}
		/*  end of decoderConfiguration  */
		boxSize += stream.ReadBit(boxSize, readSize,  out this.more_data, "more_data"); // 1 if yes, 0 if no 

		while (more_data)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.chunk_type, "chunk_type"); 
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.reserved0, "reserved0"); // for alignment 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.chunk_length, "chunk_length"); // length of the chunk in byte 

			switch (chunk_type)
			{
				case 0b000:
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(chunk_length),  out this.sco, "sco"); 
				break;

				case 0b001:
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(chunk_length),  out this.part, "part"); // ID of the part at which the following info refers 
				break;

				case 0b010:
				/*  this segment is always in binary as stated in Section 9  */
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(chunk_length),  out this.sync, "sync"); 
				break;

				case 0b011:
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(chunk_length),  out this.fmt, "fmt"); 
				break;

				case 0b100:
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(chunk_length),  out this.lyrics, "lyrics"); 
				break;

				case 0b101:
				/*  this segment is always in binary as stated in Section 11.4  */
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(chunk_length),  out this.fon, "fon"); 
				break;

				case 0b110:
				/*  reserved; */
				break;

				case 0b111:
				/*  reserved; */
				break;

			}
			boxSize += stream.ReadBit(boxSize, readSize,  out this.more_data, "more_data"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved00, "reserved00"); //for alignment 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		/*  the bitstream header   */
		boxSize += stream.WriteBits(4,  this.version, "version"); //version of this specification is 0b0000 
		boxSize += stream.WriteBits(12,  this.pictureWidth, "pictureWidth"); // rendering window X size 
		boxSize += stream.WriteBits(12,  this.pictureHight, "pictureHight"); // rendering window Y size 
		boxSize += stream.WriteBit( this.isScoreMultiwindow, "isScoreMultiwindow"); // 0: one window only – 1: multiple windows 
		boxSize += stream.WriteUInt8( this.numberOfParts, "numberOfParts"); // parts of the main score 
		boxSize += stream.WriteBits(3,  this.notationFormat, "notationFormat"); // CWMN or other sets 
		boxSize += stream.WriteUInt8( this.urlMIDIStream_length, "urlMIDIStream_length"); //length in bytes 
		boxSize += stream.WriteBits((uint)(urlMIDIStream_length ),  this.urlMIDIStream, "urlMIDIStream"); // reference to the MIDI stream, as url 
		boxSize += stream.WriteBits(2,  this.codingType, "codingType"); // coding of the XML chunks 
		boxSize += stream.WriteUInt8( this.length, "length"); //length in bits of decoder configuration, unsigned integer 
		/*  start of decoderConfiguration  */

		if (codingType == 0b11)
		{
			boxSize += stream.WriteBits(3,  this.decoderInitConfig, "decoderInitConfig"); 

			if (decoderInitConfig == 0b000)
			{
				boxSize += stream.WriteBits((uint)(length-3 ),  this.decoderInit, "decoderInit"); 
			}
		}

		else 
		{
			boxSize += stream.WriteBits((uint)(length ),  this.reserved, "reserved"); 
		}
		/*  end of decoderConfiguration  */
		boxSize += stream.WriteBit( this.more_data, "more_data"); // 1 if yes, 0 if no 

		while (more_data)
		{
			boxSize += stream.WriteBits(3,  this.chunk_type, "chunk_type"); 
			boxSize += stream.WriteBits(5,  this.reserved0, "reserved0"); // for alignment 
			boxSize += stream.WriteUInt8( this.chunk_length, "chunk_length"); // length of the chunk in byte 

			switch (chunk_type)
			{
				case 0b000:
				boxSize += stream.WriteUInt8Array((uint)(chunk_length),  this.sco, "sco"); 
				break;

				case 0b001:
				boxSize += stream.WriteUInt8Array((uint)(chunk_length),  this.part, "part"); // ID of the part at which the following info refers 
				break;

				case 0b010:
				/*  this segment is always in binary as stated in Section 9  */
				boxSize += stream.WriteUInt8Array((uint)(chunk_length),  this.sync, "sync"); 
				break;

				case 0b011:
				boxSize += stream.WriteUInt8Array((uint)(chunk_length),  this.fmt, "fmt"); 
				break;

				case 0b100:
				boxSize += stream.WriteUInt8Array((uint)(chunk_length),  this.lyrics, "lyrics"); 
				break;

				case 0b101:
				/*  this segment is always in binary as stated in Section 11.4  */
				boxSize += stream.WriteUInt8Array((uint)(chunk_length),  this.fon, "fon"); 
				break;

				case 0b110:
				/*  reserved; */
				break;

				case 0b111:
				/*  reserved; */
				break;

			}
			boxSize += stream.WriteBit( this.more_data, "more_data"); 
			boxSize += stream.WriteBits(7,  this.reserved00, "reserved00"); //for alignment 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		/*  the bitstream header   */
		boxSize += 4; // version
		boxSize += 12; // pictureWidth
		boxSize += 12; // pictureHight
		boxSize += 1; // isScoreMultiwindow
		boxSize += 8; // numberOfParts
		boxSize += 3; // notationFormat
		boxSize += 8; // urlMIDIStream_length
		boxSize += (ulong)(urlMIDIStream_length ); // urlMIDIStream
		boxSize += 2; // codingType
		boxSize += 8; // length
		/*  start of decoderConfiguration  */

		if (codingType == 0b11)
		{
			boxSize += 3; // decoderInitConfig

			if (decoderInitConfig == 0b000)
			{
				boxSize += (ulong)(length-3 ); // decoderInit
			}
		}

		else 
		{
			boxSize += (ulong)(length ); // reserved
		}
		/*  end of decoderConfiguration  */
		boxSize += 1; // more_data

		while (more_data)
		{
			boxSize += 3; // chunk_type
			boxSize += 5; // reserved0
			boxSize += 8; // chunk_length

			switch (chunk_type)
			{
				case 0b000:
				boxSize += ((ulong)(chunk_length) * 8); // sco
				break;

				case 0b001:
				boxSize += ((ulong)(chunk_length) * 8); // part
				break;

				case 0b010:
				/*  this segment is always in binary as stated in Section 9  */
				boxSize += ((ulong)(chunk_length) * 8); // sync
				break;

				case 0b011:
				boxSize += ((ulong)(chunk_length) * 8); // fmt
				break;

				case 0b100:
				boxSize += ((ulong)(chunk_length) * 8); // lyrics
				break;

				case 0b101:
				/*  this segment is always in binary as stated in Section 11.4  */
				boxSize += ((ulong)(chunk_length) * 8); // fon
				break;

				case 0b110:
				/*  reserved; */
				break;

				case 0b111:
				/*  reserved; */
				break;

			}
			boxSize += 1; // more_data
			boxSize += 7; // reserved00
		}
		return boxSize;
	}
}

}
