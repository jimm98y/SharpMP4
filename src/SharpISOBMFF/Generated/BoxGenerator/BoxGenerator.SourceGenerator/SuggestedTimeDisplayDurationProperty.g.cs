using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class SuggestedTimeDisplayDurationProperty
extends ItemFullProperty('ssld', version=0, flags=0) {
	unsigned int(16) duration;
}
*/
public partial class SuggestedTimeDisplayDurationProperty : ItemFullProperty
{
	public const string TYPE = "ssld";
	public override string DisplayName { get { return "SuggestedTimeDisplayDurationProperty"; } }

	protected ushort duration; 
	public ushort Duration { get { return this.duration; } set { this.duration = value; } }

	public SuggestedTimeDisplayDurationProperty(): base(IsoStream.FromFourCC("ssld"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.duration, "duration"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.duration, "duration"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // duration
		return boxSize;
	}
}

}
