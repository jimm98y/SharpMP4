using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class Stereoscopic3D extends FullBox('st3d', 0, 0) {
    unsigned int(8) stereo_mode;
 }

*/
public partial class Stereoscopic3D : FullBox
{
	public const string TYPE = "st3d";
	public override string DisplayName { get { return "Stereoscopic3D"; } }

	protected byte stereo_mode; 
	public byte StereoMode { get { return this.stereo_mode; } set { this.stereo_mode = value; } }

	public Stereoscopic3D(): base(IsoStream.FromFourCC("st3d"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.stereo_mode, "stereo_mode"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.stereo_mode, "stereo_mode"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // stereo_mode
		return boxSize;
	}
}

}
