using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class IpirBox() extends Box('ipir') {
	 unsigned int(32) entry_count;
 unsigned int(32) trackIDs[ entry_count ];
 } 
*/
public partial class IpirBox : Box
{
	public const string TYPE = "ipir";
	public override string DisplayName { get { return "IpirBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] trackIDs; 
	public uint[] TrackIDs { get { return this.trackIDs; } set { this.trackIDs = value; } }

	public IpirBox(): base(IsoStream.FromFourCC("ipir"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		boxSize += stream.ReadUInt32Array(boxSize, readSize, (uint)( entry_count ),  out this.trackIDs, "trackIDs"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		boxSize += stream.WriteUInt32Array((uint)( entry_count ),  this.trackIDs, "trackIDs"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		boxSize += ((ulong)( entry_count ) * 32); // trackIDs
		return boxSize;
	}
}

}
