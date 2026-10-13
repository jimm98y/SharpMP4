using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ControllerTypeBox() extends Box ('ctyp'){
 // the movie controller's type, a four character code (QuickTime 'ctyp')
 unsigned int(32) controller_type;
 }
*/
public partial class ControllerTypeBox : Box
{
	public const string TYPE = "ctyp";
	public override string DisplayName { get { return "ControllerTypeBox"; } }

	protected uint controller_type; 
	public uint ControllerType { get { return this.controller_type; } set { this.controller_type = value; } }

	public ControllerTypeBox(): base(IsoStream.FromFourCC("ctyp"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  the movie controller's type, a four character code (QuickTime 'ctyp') */
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.controller_type, "controller_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  the movie controller's type, a four character code (QuickTime 'ctyp') */
		boxSize += stream.WriteUInt32( this.controller_type, "controller_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  the movie controller's type, a four character code (QuickTime 'ctyp') */
		boxSize += 32; // controller_type
		return boxSize;
	}
}

}
