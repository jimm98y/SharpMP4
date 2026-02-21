using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MVDScalabilityInformationSEIBox extends Box('3sib', size)
{
	unsigned int(8*size-64)	mvdscalinfosei;
}
*/
public partial class MVDScalabilityInformationSEIBox : Box
{
	public const string TYPE = "3sib";
	public override string DisplayName { get { return "MVDScalabilityInformationSEIBox"; } }

	protected byte[] mvdscalinfosei; 
	public byte[] Mvdscalinfosei { get { return this.mvdscalinfosei; } set { this.mvdscalinfosei = value; } }

	public MVDScalabilityInformationSEIBox(ulong size = 0): base(IsoStream.FromFourCC("3sib"), size)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*size-64 ),  out this.mvdscalinfosei, "mvdscalinfosei"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits((uint)(8*size-64 ),  this.mvdscalinfosei, "mvdscalinfosei"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += (ulong)(8*size-64 ); // mvdscalinfosei
		return boxSize;
	}
}

}
