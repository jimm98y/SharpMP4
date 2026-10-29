using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OMAContentIDBox() extends FullBox('ccid') {
	unsigned int(16) contentIDLength;
	unsigned int(8) contentID[contentIDLength];
}"; } }
	public const string TYPE = "ccid";
	public override string DisplayName { get { return "OMAContentIDBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected ushort contentIDLength; 
	public ushort ContentIDLength { get { return this.contentIDLength; } set { this.contentIDLength = value; } }

	protected byte[] contentID = []; 
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("contentIDLength", 16, this.contentIDLength); 
		boxSize += tracer.Field("contentID", ((ulong)(contentIDLength) * 8), this.contentID); 
		return boxSize;
	}
}

}
