using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class BaseLocationBox() extends FullBox('bloc') {
 unsigned int(8) baseLocation[256];
 unsigned int(8) purchaseLocation[256];
 unsigned int(8) reserved[512];
 } 
*/
public partial class BaseLocationBox : FullBox
{
	public const string TYPE = "bloc";
	public override string DisplayName { get { return "BaseLocationBox"; } }

	protected byte[] baseLocation; 
	public byte[] BaseLocation { get { return this.baseLocation; } set { this.baseLocation = value; } }

	protected byte[] purchaseLocation; 
	public byte[] PurchaseLocation { get { return this.purchaseLocation; } set { this.purchaseLocation = value; } }

	protected byte[] reserved; 
	public byte[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public BaseLocationBox(): base(IsoStream.FromFourCC("bloc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 256,  out this.baseLocation, "baseLocation"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 256,  out this.purchaseLocation, "purchaseLocation"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 512,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(256,  this.baseLocation, "baseLocation"); 
		boxSize += stream.WriteUInt8Array(256,  this.purchaseLocation, "purchaseLocation"); 
		boxSize += stream.WriteUInt8Array(512,  this.reserved, "reserved"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 256 * 8; // baseLocation
		boxSize += 256 * 8; // purchaseLocation
		boxSize += 512 * 8; // reserved
		return boxSize;
	}
}

}
