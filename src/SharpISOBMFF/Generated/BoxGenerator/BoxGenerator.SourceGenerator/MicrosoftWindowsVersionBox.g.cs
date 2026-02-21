using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MicrosoftWindowsVersionBox() extends Box('uuid 5ca708fb328e4205a861650eca0a9596') {
	 unsigned int(16) unknown1;
 unsigned int(16) count;
 char version[count];
 }
*/
public partial class MicrosoftWindowsVersionBox : Box
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "MicrosoftWindowsVersionBox"; } }

	protected ushort unknown1; 
	public ushort Unknown1 { get { return this.unknown1; } set { this.unknown1 = value; } }

	protected ushort count; 
	public ushort Count { get { return this.count; } set { this.count = value; } }

	protected byte[] version; 
	public byte[] Version { get { return this.version; } set { this.version = value; } }

	public MicrosoftWindowsVersionBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("5ca708fb328e4205a861650eca0a9596"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.unknown1, "unknown1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.version, "version"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.unknown1, "unknown1"); 
		boxSize += stream.WriteUInt16( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.version, "version"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // unknown1
		boxSize += 16; // count
		boxSize += ((ulong)(count) * 8); // version
		return boxSize;
	}
}

}
