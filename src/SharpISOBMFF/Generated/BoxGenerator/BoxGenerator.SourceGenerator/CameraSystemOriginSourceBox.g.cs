using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemOriginSourceBox extends FullBox('corg', 0, 0) { 
unsigned int(32) source_of_origin; 
// e.g., 'blin'  
} 
*/
public partial class CameraSystemOriginSourceBox : FullBox
{
	public const string TYPE = "corg";
	public override string DisplayName { get { return "CameraSystemOriginSourceBox"; } }

	protected uint source_of_origin;  //  e.g., 'blin'  
	public uint SourceOfOrigin { get { return this.source_of_origin; } set { this.source_of_origin = value; } }

	public CameraSystemOriginSourceBox(): base(IsoStream.FromFourCC("corg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.source_of_origin, "source_of_origin"); // e.g., 'blin'  
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.source_of_origin, "source_of_origin"); // e.g., 'blin'  
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // source_of_origin
		return boxSize;
	}
}

}
