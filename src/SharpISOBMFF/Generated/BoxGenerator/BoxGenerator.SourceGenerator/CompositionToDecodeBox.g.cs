using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class CompositionToDecodeBox extends FullBox('cslg', version, 0) {
	if (version==0) {
		signed int(32)	compositionToDTSShift;
		signed int(32)	leastDecodeToDisplayDelta;
		signed int(32)	greatestDecodeToDisplayDelta;
		signed int(32)	compositionStartTime;
		signed int(32)	compositionEndTime;
	} else {
		signed int(64)	compositionToDTSShift;
		signed int(64)	leastDecodeToDisplayDelta;
		signed int(64)	greatestDecodeToDisplayDelta;
		signed int(64)	compositionStartTime;
		signed int(64)	compositionEndTime;
	}
}
*/
public partial class CompositionToDecodeBox : FullBox
{
	public const string TYPE = "cslg";
	public override string DisplayName { get { return "CompositionToDecodeBox"; } }

	protected long compositionToDTSShift; 
	public long CompositionToDTSShift { get { return this.compositionToDTSShift; } set { this.compositionToDTSShift = value; } }

	protected long leastDecodeToDisplayDelta; 
	public long LeastDecodeToDisplayDelta { get { return this.leastDecodeToDisplayDelta; } set { this.leastDecodeToDisplayDelta = value; } }

	protected long greatestDecodeToDisplayDelta; 
	public long GreatestDecodeToDisplayDelta { get { return this.greatestDecodeToDisplayDelta; } set { this.greatestDecodeToDisplayDelta = value; } }

	protected long compositionStartTime; 
	public long CompositionStartTime { get { return this.compositionStartTime; } set { this.compositionStartTime = value; } }

	protected long compositionEndTime; 
	public long CompositionEndTime { get { return this.compositionEndTime; } set { this.compositionEndTime = value; } }

	public CompositionToDecodeBox(byte version = 0): base(IsoStream.FromFourCC("cslg"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==0)
		{
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.compositionToDTSShift, "compositionToDTSShift"); 
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.leastDecodeToDisplayDelta, "leastDecodeToDisplayDelta"); 
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.greatestDecodeToDisplayDelta, "greatestDecodeToDisplayDelta"); 
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.compositionStartTime, "compositionStartTime"); 
			boxSize += stream.ReadInt32(boxSize, readSize,  out this.compositionEndTime, "compositionEndTime"); 
		}

		else 
		{
			boxSize += stream.ReadInt64(boxSize, readSize,  out this.compositionToDTSShift, "compositionToDTSShift"); 
			boxSize += stream.ReadInt64(boxSize, readSize,  out this.leastDecodeToDisplayDelta, "leastDecodeToDisplayDelta"); 
			boxSize += stream.ReadInt64(boxSize, readSize,  out this.greatestDecodeToDisplayDelta, "greatestDecodeToDisplayDelta"); 
			boxSize += stream.ReadInt64(boxSize, readSize,  out this.compositionStartTime, "compositionStartTime"); 
			boxSize += stream.ReadInt64(boxSize, readSize,  out this.compositionEndTime, "compositionEndTime"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==0)
		{
			boxSize += stream.WriteInt32( this.compositionToDTSShift, "compositionToDTSShift"); 
			boxSize += stream.WriteInt32( this.leastDecodeToDisplayDelta, "leastDecodeToDisplayDelta"); 
			boxSize += stream.WriteInt32( this.greatestDecodeToDisplayDelta, "greatestDecodeToDisplayDelta"); 
			boxSize += stream.WriteInt32( this.compositionStartTime, "compositionStartTime"); 
			boxSize += stream.WriteInt32( this.compositionEndTime, "compositionEndTime"); 
		}

		else 
		{
			boxSize += stream.WriteInt64( this.compositionToDTSShift, "compositionToDTSShift"); 
			boxSize += stream.WriteInt64( this.leastDecodeToDisplayDelta, "leastDecodeToDisplayDelta"); 
			boxSize += stream.WriteInt64( this.greatestDecodeToDisplayDelta, "greatestDecodeToDisplayDelta"); 
			boxSize += stream.WriteInt64( this.compositionStartTime, "compositionStartTime"); 
			boxSize += stream.WriteInt64( this.compositionEndTime, "compositionEndTime"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==0)
		{
			boxSize += 32; // compositionToDTSShift
			boxSize += 32; // leastDecodeToDisplayDelta
			boxSize += 32; // greatestDecodeToDisplayDelta
			boxSize += 32; // compositionStartTime
			boxSize += 32; // compositionEndTime
		}

		else 
		{
			boxSize += 64; // compositionToDTSShift
			boxSize += 64; // leastDecodeToDisplayDelta
			boxSize += 64; // greatestDecodeToDisplayDelta
			boxSize += 64; // compositionStartTime
			boxSize += 64; // compositionEndTime
		}
		return boxSize;
	}
}

}
