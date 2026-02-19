using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class VvcSubpicIDProperty
extends ItemFullProperty('spid', version = 0, flags = 0){
	VvcSubpicIDEntry sid_info; // specified in ISO/IEC 14496-15
}
*/
public partial class VvcSubpicIDProperty : ItemFullProperty
{
	public const string TYPE = "spid";
	public override string DisplayName { get { return "VvcSubpicIDProperty"; } }

	protected VvcSubpicIDEntry sid_info;  //  specified in ISO/IEC 14496-15
	public VvcSubpicIDEntry SidInfo { get { return this.sid_info; } set { this.sid_info = value; } }

	public VvcSubpicIDProperty(): base(IsoStream.FromFourCC("spid"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new VvcSubpicIDEntry(),  out this.sid_info, "sid_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.sid_info, "sid_info"); // specified in ISO/IEC 14496-15
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(sid_info); // sid_info
		return boxSize;
	}
}

}
