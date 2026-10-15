using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MultiviewSceneInfoBox extends Box ('vwdi') 
{
	unsigned int(8) 	max_disparity;
}
*/
public partial class MultiviewSceneInfoBox : Box
{
	public const string TYPE = "vwdi";
	public override string DisplayName { get { return "MultiviewSceneInfoBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("mvcg"); }


	protected byte max_disparity; 
	public byte MaxDisparity { get { return this.max_disparity; } set { this.max_disparity = value; } }

	public MultiviewSceneInfoBox(): base(IsoStream.FromFourCC("vwdi"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.max_disparity, "max_disparity"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.max_disparity, "max_disparity"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // max_disparity
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("max_disparity", 8, this.max_disparity); 
		return boxSize;
	}
}

}
