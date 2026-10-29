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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class UuidBasedProtectionSystemSpecificHeaderBox() extends FullBox('uuid d08a4f1810f34a82b6c832d8aba183d3') {
	 unsigned int(8) systemID[16];
 unsigned int(32) count;
 unsigned int(8) data[count];
 }"; } }
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "UuidBasedProtectionSystemSpecificHeaderBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? new[] { "*" }); }


	protected byte[] systemID = new byte[16]; 
	public byte[] SystemID { get { return this.systemID; } set { this.systemID = value; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected byte[] data = []; 
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("systemID", 16 * 8, this.systemID); 
		boxSize += tracer.Field("count", 32, this.count); 
		boxSize += tracer.Field("data", ((ulong)(count) * 8), this.data); 
		return boxSize;
	}
}

}
