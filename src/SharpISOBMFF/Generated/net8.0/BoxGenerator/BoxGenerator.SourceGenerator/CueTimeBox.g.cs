using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class CueTimeBox() extends Box ('ctim') {
 boxstring cueCurrentTime; 
 }
*/
public partial class CueTimeBox : Box
{
	public const string TYPE = "ctim";
	public override string DisplayName { get { return "CueTimeBox"; } }

	protected BinaryUTF8String cueCurrentTime; 
	public BinaryUTF8String CueCurrentTime { get { return this.cueCurrentTime; } set { this.cueCurrentTime = value; } }

	public CueTimeBox(): base(IsoStream.FromFourCC("ctim"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.cueCurrentTime, "cueCurrentTime"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.cueCurrentTime, "cueCurrentTime"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(cueCurrentTime); // cueCurrentTime
		return boxSize;
	}
}

}
