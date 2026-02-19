using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProtectionSystemSpecificHeaderBox() extends FullBox('pssh') {
 unsigned int(8) systemId[16];
 if (version > 0) {
 unsigned int(32) count;
 ProtectionSystemSpecificKeyID keyIDs[count];
 }
  }
 
*/
public partial class ProtectionSystemSpecificHeaderBox : FullBox
{
	public const string TYPE = "pssh";
	public override string DisplayName { get { return "ProtectionSystemSpecificHeaderBox"; } }

	protected byte[] systemId; 
	public byte[] SystemId { get { return this.systemId; } set { this.systemId = value; } }

	protected uint count; 
	public uint Count { get { return this.count; } set { this.count = value; } }

	protected ProtectionSystemSpecificKeyID[] keyIDs; 
	public ProtectionSystemSpecificKeyID[] KeyIDs { get { return this.keyIDs; } set { this.keyIDs = value; } }

	public ProtectionSystemSpecificHeaderBox(): base(IsoStream.FromFourCC("pssh"))
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
		return boxSize;
	}
}

}
