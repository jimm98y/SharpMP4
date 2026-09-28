using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LoadBox() extends Box('load') {
 signed int(32) preloadStartTime;
 signed int(32) preloadDuration;
 unsigned int(32) preloadFlags;
 unsigned int(32) defaultHints;
 } 
*/
public partial class LoadBox : Box
{
	public const string TYPE = "load";
	public override string DisplayName { get { return "LoadBox"; } }

	protected int preloadStartTime; 
	public int PreloadStartTime { get { return this.preloadStartTime; } set { this.preloadStartTime = value; } }

	protected int preloadDuration; 
	public int PreloadDuration { get { return this.preloadDuration; } set { this.preloadDuration = value; } }

	protected uint preloadFlags; 
	public uint PreloadFlags { get { return this.preloadFlags; } set { this.preloadFlags = value; } }

	protected uint defaultHints; 
	public uint DefaultHints { get { return this.defaultHints; } set { this.defaultHints = value; } }

	public LoadBox(): base(IsoStream.FromFourCC("load"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.preloadStartTime, "preloadStartTime"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.preloadDuration, "preloadDuration"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.preloadFlags, "preloadFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.defaultHints, "defaultHints"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteInt32( this.preloadStartTime, "preloadStartTime"); 
		boxSize += stream.WriteInt32( this.preloadDuration, "preloadDuration"); 
		boxSize += stream.WriteUInt32( this.preloadFlags, "preloadFlags"); 
		boxSize += stream.WriteUInt32( this.defaultHints, "defaultHints"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // preloadStartTime
		boxSize += 32; // preloadDuration
		boxSize += 32; // preloadFlags
		boxSize += 32; // defaultHints
		return boxSize;
	}
}

}
