using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class CueSourceIDBox() extends Box ('vsid') {
 unsigned int(32) sourceID; 
 }
*/
public partial class CueSourceIDBox : Box
{
	public const string TYPE = "vsid";
	public override string DisplayName { get { return "CueSourceIDBox"; } }

	protected uint sourceID; 
	public uint SourceID { get { return this.sourceID; } set { this.sourceID = value; } }

	public CueSourceIDBox(): base(IsoStream.FromFourCC("vsid"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sourceID, "sourceID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.sourceID, "sourceID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // sourceID
		return boxSize;
	}
}

}
