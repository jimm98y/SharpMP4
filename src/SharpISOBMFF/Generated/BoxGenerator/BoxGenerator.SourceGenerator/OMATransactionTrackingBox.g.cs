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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OMATransactionTrackingBox() extends FullBox('odtt') {
	 unsigned int(8) transactionID[16];
 } "; } }
	public const string TYPE = "odtt";
	public override string DisplayName { get { return "OMATransactionTrackingBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mdri"); }


	protected byte[] transactionID = new byte[16]; 
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("transactionID", 16 * 8, this.transactionID); 
		return boxSize;
	}
}

}
