using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMAContentIDBox() extends FullBox('ccid') {
	 unsigned int(16) contentIDLength;
 unsigned int(8) contentID[contentIDLength];
 } 
*/
public partial class OMAContentIDBox : FullBox
{
	public const string TYPE = "ccid";
	public override string DisplayName { get { return "OMAContentIDBox"; } }

	protected ushort contentIDLength; 
	public ushort ContentIDLength { get { return this.contentIDLength; } set { this.contentIDLength = value; } }

	protected byte[] contentID; 
	public byte[] ContentID { get { return this.contentID; } set { this.contentID = value; } }

	public OMAContentIDBox(): base(IsoStream.FromFourCC("ccid"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.contentIDLength, "contentIDLength"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(contentIDLength),  out this.contentID, "contentID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.contentIDLength, "contentIDLength"); 
		boxSize += stream.WriteUInt8Array((uint)(contentIDLength),  this.contentID, "contentID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // contentIDLength
		boxSize += ((ulong)(contentIDLength) * 8); // contentID
		return boxSize;
	}
}

}
