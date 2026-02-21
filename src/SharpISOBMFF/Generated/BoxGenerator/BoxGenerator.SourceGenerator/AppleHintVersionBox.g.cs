using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleHintVersionBox() extends Box('hinv') {
 string version;
 } 
*/
public partial class AppleHintVersionBox : Box
{
	public const string TYPE = "hinv";
	public override string DisplayName { get { return "AppleHintVersionBox"; } }

	protected BinaryUTF8String version; 
	public BinaryUTF8String Version { get { return this.version; } set { this.version = value; } }

	public AppleHintVersionBox(): base(IsoStream.FromFourCC("hinv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.version, "version"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.version, "version"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(version); // version
		return boxSize;
	}
}

}
