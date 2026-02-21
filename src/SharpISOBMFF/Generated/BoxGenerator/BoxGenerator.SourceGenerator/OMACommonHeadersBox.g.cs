using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMACommonHeadersBox() extends FullBox('ohdr') {
	 unsigned int(8) encryptionMethod;
 unsigned int(8) paddingScheme;
 unsigned int(64) plaintextLength;
 unsigned int(16) contentIDLength;
 unsigned int(16) rightsIssuerLength;
 unsigned int(16) textualHeadersLength;
 unsigned int(8) contentID[contentIDLength];
 unsigned int(8) rightsIssuerURL[rightsIssuerLength];
 unsigned int(8) textualHeaders[textualHeadersLength];
 } 
*/
public partial class OMACommonHeadersBox : FullBox
{
	public const string TYPE = "ohdr";
	public override string DisplayName { get { return "OMACommonHeadersBox"; } }

	protected byte encryptionMethod; 
	public byte EncryptionMethod { get { return this.encryptionMethod; } set { this.encryptionMethod = value; } }

	protected byte paddingScheme; 
	public byte PaddingScheme { get { return this.paddingScheme; } set { this.paddingScheme = value; } }

	protected ulong plaintextLength; 
	public ulong PlaintextLength { get { return this.plaintextLength; } set { this.plaintextLength = value; } }

	protected ushort contentIDLength; 
	public ushort ContentIDLength { get { return this.contentIDLength; } set { this.contentIDLength = value; } }

	protected ushort rightsIssuerLength; 
	public ushort RightsIssuerLength { get { return this.rightsIssuerLength; } set { this.rightsIssuerLength = value; } }

	protected ushort textualHeadersLength; 
	public ushort TextualHeadersLength { get { return this.textualHeadersLength; } set { this.textualHeadersLength = value; } }

	protected byte[] contentID; 
	public byte[] ContentID { get { return this.contentID; } set { this.contentID = value; } }

	protected byte[] rightsIssuerURL; 
	public byte[] RightsIssuerURL { get { return this.rightsIssuerURL; } set { this.rightsIssuerURL = value; } }

	protected byte[] textualHeaders; 
	public byte[] TextualHeaders { get { return this.textualHeaders; } set { this.textualHeaders = value; } }

	public OMACommonHeadersBox(): base(IsoStream.FromFourCC("ohdr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.encryptionMethod, "encryptionMethod"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.paddingScheme, "paddingScheme"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.plaintextLength, "plaintextLength"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.contentIDLength, "contentIDLength"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.rightsIssuerLength, "rightsIssuerLength"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.textualHeadersLength, "textualHeadersLength"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(contentIDLength),  out this.contentID, "contentID"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(rightsIssuerLength),  out this.rightsIssuerURL, "rightsIssuerURL"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(textualHeadersLength),  out this.textualHeaders, "textualHeaders"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.encryptionMethod, "encryptionMethod"); 
		boxSize += stream.WriteUInt8( this.paddingScheme, "paddingScheme"); 
		boxSize += stream.WriteUInt64( this.plaintextLength, "plaintextLength"); 
		boxSize += stream.WriteUInt16( this.contentIDLength, "contentIDLength"); 
		boxSize += stream.WriteUInt16( this.rightsIssuerLength, "rightsIssuerLength"); 
		boxSize += stream.WriteUInt16( this.textualHeadersLength, "textualHeadersLength"); 
		boxSize += stream.WriteUInt8Array((uint)(contentIDLength),  this.contentID, "contentID"); 
		boxSize += stream.WriteUInt8Array((uint)(rightsIssuerLength),  this.rightsIssuerURL, "rightsIssuerURL"); 
		boxSize += stream.WriteUInt8Array((uint)(textualHeadersLength),  this.textualHeaders, "textualHeaders"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // encryptionMethod
		boxSize += 8; // paddingScheme
		boxSize += 64; // plaintextLength
		boxSize += 16; // contentIDLength
		boxSize += 16; // rightsIssuerLength
		boxSize += 16; // textualHeadersLength
		boxSize += ((ulong)(contentIDLength) * 8); // contentID
		boxSize += ((ulong)(rightsIssuerLength) * 8); // rightsIssuerURL
		boxSize += ((ulong)(textualHeadersLength) * 8); // textualHeaders
		return boxSize;
	}
}

}
