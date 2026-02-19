using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SMIBox() extends Box('SMI ') {
	 bit(8) metadata[];
 } 
*/
public partial class SMIBox : Box
{
	public const string TYPE = "SMI ";
	public override string DisplayName { get { return "SMIBox"; } }

	protected byte[] metadata; 
	public byte[] Metadata { get { return this.metadata; } set { this.metadata = value; } }

	public SMIBox(): base(IsoStream.FromFourCC("SMI "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.metadata, "metadata"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.metadata, "metadata"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)metadata.Length * 8); // metadata
		return boxSize;
	}
}

}
