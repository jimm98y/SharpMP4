using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class PanoramaEntry
extends VisualSampleGroupEntry('pano') {
	unsigned int(16) frame_number;
}

*/
public partial class PanoramaEntry : VisualSampleGroupEntry
{
	public const string TYPE = "pano";
	public override string DisplayName { get { return "PanoramaEntry"; } }

	protected ushort frame_number; 
	public ushort FrameNumber { get { return this.frame_number; } set { this.frame_number = value; } }

	public PanoramaEntry(): base(IsoStream.FromFourCC("pano"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.frame_number, "frame_number"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.frame_number, "frame_number"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // frame_number
		return boxSize;
	}
}

}
