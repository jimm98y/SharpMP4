using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemLensReferenceDimensionsBox extends FullBox('rdim', 0, 0) { 
unsigned int(32) reference_width; 
unsigned int(32) reference_height; 
}
*/
public partial class CameraSystemLensReferenceDimensionsBox : FullBox
{
	public const string TYPE = "rdim";
	public override string DisplayName { get { return "CameraSystemLensReferenceDimensionsBox"; } }

	protected uint reference_width; 
	public uint ReferenceWidth { get { return this.reference_width; } set { this.reference_width = value; } }

	protected uint reference_height; 
	public uint ReferenceHeight { get { return this.reference_height; } set { this.reference_height = value; } }

	public CameraSystemLensReferenceDimensionsBox(): base(IsoStream.FromFourCC("rdim"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reference_width, "reference_width"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reference_height, "reference_height"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reference_width, "reference_width"); 
		boxSize += stream.WriteUInt32( this.reference_height, "reference_height"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reference_width
		boxSize += 32; // reference_height
		return boxSize;
	}
}

}
