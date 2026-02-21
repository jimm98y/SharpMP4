using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OMATransactionTrackingBox() extends FullBox('odtt') {
	 unsigned int(8) transactionID[16];
 } 
*/
public partial class OMATransactionTrackingBox : FullBox
{
	public const string TYPE = "odtt";
	public override string DisplayName { get { return "OMATransactionTrackingBox"; } }

	protected byte[] transactionID; 
	public byte[] TransactionID { get { return this.transactionID; } set { this.transactionID = value; } }

	public OMATransactionTrackingBox(): base(IsoStream.FromFourCC("odtt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.transactionID, "transactionID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(16,  this.transactionID, "transactionID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16 * 8; // transactionID
		return boxSize;
	}
}

}
