using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ParametricImmersiveMediaProjectionBox extends 
FullBox('prim', 0, 0) { 
// fields reserved for future use 
} 
*/
public partial class ParametricImmersiveMediaProjectionBox : FullBox
{
	public const string TYPE = "prim";
	public override string DisplayName { get { return "ParametricImmersiveMediaProjectionBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("proj"); }


	public ParametricImmersiveMediaProjectionBox(): base(IsoStream.FromFourCC("prim"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  fields reserved for future use  */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  fields reserved for future use  */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  fields reserved for future use  */
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		/*  fields reserved for future use  */
		return boxSize;
	}
}

}
