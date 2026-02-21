using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ViewPackingInformationBox extends FullBox('pkin', 0, 0) { 
unsigned int(32) view_packing_kind; // a FourCC for the kind of projection 
} 
*/
public partial class ViewPackingInformationBox : FullBox
{
	public const string TYPE = "pkin";
	public override string DisplayName { get { return "ViewPackingInformationBox"; } }

	protected uint view_packing_kind;  //  a FourCC for the kind of projection 
	public uint ViewPackingKind { get { return this.view_packing_kind; } set { this.view_packing_kind = value; } }

	public ViewPackingInformationBox(): base(IsoStream.FromFourCC("pkin"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.view_packing_kind, "view_packing_kind"); // a FourCC for the kind of projection 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.view_packing_kind, "view_packing_kind"); // a FourCC for the kind of projection 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // view_packing_kind
		return boxSize;
	}
}

}
