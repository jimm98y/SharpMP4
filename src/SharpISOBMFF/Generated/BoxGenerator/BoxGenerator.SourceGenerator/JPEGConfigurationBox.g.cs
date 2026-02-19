using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class JPEGConfigurationBox extends Box('jpgC') {
	unsigned int(8) JPEGprefix[];
}
*/
public partial class JPEGConfigurationBox : Box
{
	public const string TYPE = "jpgC";
	public override string DisplayName { get { return "JPEGConfigurationBox"; } }

	protected byte[] JPEGprefix; 
	public byte[] _JPEGprefix { get { return this.JPEGprefix; } set { this.JPEGprefix = value; } }

	public JPEGConfigurationBox(): base(IsoStream.FromFourCC("jpgC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.JPEGprefix, "JPEGprefix"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.JPEGprefix, "JPEGprefix"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)JPEGprefix.Length * 8); // JPEGprefix
		return boxSize;
	}
}

}
