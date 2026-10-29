using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProtectionSystemSpecificHeaderBox() extends FullBox('pssh', version, flags = 0) {
 unsigned int(8) systemId[16];
 if (version > 0) {
  unsigned int(32) count;
  ProtectionSystemSpecificKeyID keyIDs[count];
 }
 unsigned int(32) dataSize;
 unsigned int(8) data[dataSize];
 }
 
*/
public partial class ProtectionSystemSpecificHeaderBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ProtectionSystemSpecificHeaderBox() extends FullBox('pssh', version, flags = 0) {
 unsigned int(8) systemId[16];
 if (version > 0) {
  unsigned int(32) count;
  ProtectionSystemSpecificKeyID keyIDs[count];
 }
 unsigned int(32) dataSize;
 unsigned int(8) data[dataSize];
 }
 "; } }
	public const string TYPE = "pssh";
	public override string DisplayName { get { return "ProtectionSystemSpecificHeaderBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("moov", "moof", "meta"); }


	protected byte[] systemId = new byte[16]; 
	public byte[] SystemId { get { return this.systemId; } set { this.systemId = value; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected ProtectionSystemSpecificKeyID[] keyIDs = []; 
	public ProtectionSystemSpecificKeyID[] KeyIDs { get { return this.keyIDs; } set { this.keyIDs = value; } }

	protected uint dataSize; 
	public uint DataSize { get { return this.dataSize; } set { this.dataSize = value; } }

	protected byte[] data = []; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public ProtectionSystemSpecificHeaderBox(byte version = 0): base(IsoStream.FromFourCC("pssh"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.systemId, "systemId"); 

		if (version > 0)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.count, "count"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(count), () => new ProtectionSystemSpecificKeyID(),  out this.keyIDs, "keyIDs"); 
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dataSize, "dataSize"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(dataSize),  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(16,  this.systemId, "systemId"); 

		if (version > 0)
		{
			boxSize += stream.WriteUInt32( this.count, "count"); 
			boxSize += stream.WriteClass( this.keyIDs, "keyIDs"); 
		}
		boxSize += stream.WriteUInt32( this.dataSize, "dataSize"); 
		boxSize += stream.WriteUInt8Array((uint)(dataSize),  this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16 * 8; // systemId

		if (version > 0)
		{
			boxSize += 32; // count
			boxSize += IsoStream.CalculateClassSize(keyIDs); // keyIDs
		}
		boxSize += 32; // dataSize
		boxSize += ((ulong)(dataSize) * 8); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("systemId", 16 * 8, this.systemId); 

		if (version > 0)
		{
			boxSize += tracer.Field("count", 32, this.count); 
			boxSize += tracer.Field("keyIDs", IsoStream.CalculateClassSize(keyIDs), this.keyIDs); 
		}
		boxSize += tracer.Field("dataSize", 32, this.dataSize); 
		boxSize += tracer.Field("data", ((ulong)(dataSize) * 8), this.data); 
		return boxSize;
	}
}

}
