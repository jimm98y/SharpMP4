using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class UuidBasedProtectionSystemSpecificHeaderBox() extends FullBox('uuid d08a4f1810f34a82b6c832d8aba183d3') {
	 unsigned int(8) systemID[16];
 unsigned int(32) count;
 unsigned int(8) data[count];
 }
*/
public partial class UuidBasedProtectionSystemSpecificHeaderBox : FullBox
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "UuidBasedProtectionSystemSpecificHeaderBox"; } }

	protected byte[] systemID; 
	public byte[] SystemID { get { return this.systemID; } set { this.systemID = value; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public UuidBasedProtectionSystemSpecificHeaderBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("d08a4f1810f34a82b6c832d8aba183d3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.systemID, "systemID"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(16,  this.systemID, "systemID"); 
		boxSize += stream.WriteUInt32( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16 * 8; // systemID
		boxSize += 32; // count
		boxSize += ((ulong)(count) * 8); // data
		return boxSize;
	}
}

}
