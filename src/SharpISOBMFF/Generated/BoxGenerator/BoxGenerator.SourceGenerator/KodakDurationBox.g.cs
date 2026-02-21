using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class KodakDurationBox() extends Box ('tima'){
 unsigned int(32) duration;
 }

*/
public partial class KodakDurationBox : Box
{
	public const string TYPE = "tima";
	public override string DisplayName { get { return "KodakDurationBox"; } }

	protected uint duration; 
	public uint Duration { get { return this.duration; } set { this.duration = value; } }

	public KodakDurationBox(): base(IsoStream.FromFourCC("tima"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.duration, "duration"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.duration, "duration"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // duration
		return boxSize;
	}
}

}
