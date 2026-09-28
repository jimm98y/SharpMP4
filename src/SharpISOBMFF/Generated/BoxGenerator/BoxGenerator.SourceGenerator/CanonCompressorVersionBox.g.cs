using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class CanonCompressorVersionBox() extends Box ('CNCV'){
 string compressorVersion;
 }
*/
public partial class CanonCompressorVersionBox : Box
{
	public const string TYPE = "CNCV";
	public override string DisplayName { get { return "CanonCompressorVersionBox"; } }

	protected BinaryUTF8String compressorVersion; 
	public BinaryUTF8String CompressorVersion { get { return this.compressorVersion; } set { this.compressorVersion = value; } }

	public CanonCompressorVersionBox(): base(IsoStream.FromFourCC("CNCV"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.compressorVersion, "compressorVersion"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.compressorVersion, "compressorVersion"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(compressorVersion); // compressorVersion
		return boxSize;
	}
}

}
