using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class VvcSubpicOrderProperty
extends ItemFullProperty('spor', version = 0, flags = 0){
	VvcSubpicOrderEntry sor_info; // specified in ISO/IEC 14496-15
}
*/
public partial class VvcSubpicOrderProperty : ItemFullProperty
{
	public const string TYPE = "spor";
	public override string DisplayName { get { return "VvcSubpicOrderProperty"; } }

	protected VvcSubpicOrderEntry sor_info;  //  specified in ISO/IEC 14496-15
	public VvcSubpicOrderEntry SorInfo { get { return this.sor_info; } set { this.sor_info = value; } }

	public VvcSubpicOrderProperty(): base(IsoStream.FromFourCC("spor"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcSubpicOrderEntry(),  out this.sor_info, "sor_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.sor_info, "sor_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(sor_info); // sor_info
		return boxSize;
	}
}

}
