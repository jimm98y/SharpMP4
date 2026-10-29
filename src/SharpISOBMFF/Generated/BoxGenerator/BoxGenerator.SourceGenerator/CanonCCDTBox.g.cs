using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// Canon.pm, CCDT: the type of an image, and the track it is in
aligned(8) class CanonCCDTBox() extends Box('CCDT') {
	unsigned int(64) imageType;
	unsigned int(32) dualPixel;
	unsigned int(32) trackIndex;
}
*/
public partial class CanonCCDTBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// Canon.pm, CCDT: the type of an image, and the track it is in
aligned(8) class CanonCCDTBox() extends Box('CCDT') {
	unsigned int(64) imageType;
	unsigned int(32) dualPixel;
	unsigned int(32) trackIndex;
}"; } }
	public const string TYPE = "CCDT";
	public override string DisplayName { get { return "CanonCCDTBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("CCTP"); }


	protected ulong imageType; 
	public ulong ImageType { get { return this.imageType; } set { this.imageType = value; } }

	protected uint dualPixel; 
	public uint DualPixel { get { return this.dualPixel; } set { this.dualPixel = value; } }

	protected uint trackIndex; 
	public uint TrackIndex { get { return this.trackIndex; } set { this.trackIndex = value; } }

	public CanonCCDTBox(): base(IsoStream.FromFourCC("CCDT"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.imageType, "imageType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dualPixel, "dualPixel"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.trackIndex, "trackIndex"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.imageType, "imageType"); 
		boxSize += stream.WriteUInt32( this.dualPixel, "dualPixel"); 
		boxSize += stream.WriteUInt32( this.trackIndex, "trackIndex"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // imageType
		boxSize += 32; // dualPixel
		boxSize += 32; // trackIndex
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("imageType", 64, this.imageType); 
		boxSize += tracer.Field("dualPixel", 32, this.dualPixel); 
		boxSize += tracer.Field("trackIndex", 32, this.trackIndex); 
		return boxSize;
	}
}

}
