using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ScalabilityInformationSEIBox extends Box('seib', size)
{
	unsigned int(8*size-64)	scalinfosei; 
}
*/
public partial class ScalabilityInformationSEIBox : Box
{
	public const string TYPE = "seib";
	public override string DisplayName { get { return "ScalabilityInformationSEIBox"; } }

	protected byte[] scalinfosei; 
	public byte[] Scalinfosei { get { return this.scalinfosei; } set { this.scalinfosei = value; } }

	public ScalabilityInformationSEIBox(ulong size = 0): base(IsoStream.FromFourCC("seib"), size)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*size-64 ),  out this.scalinfosei, "scalinfosei"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits((uint)(8*size-64 ),  this.scalinfosei, "scalinfosei"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += (ulong)(8*size-64 ); // scalinfosei
		return boxSize;
	}
}

}
