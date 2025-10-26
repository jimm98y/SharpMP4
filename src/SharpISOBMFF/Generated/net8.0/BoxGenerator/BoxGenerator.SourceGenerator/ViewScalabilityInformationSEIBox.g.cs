using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ViewScalabilityInformationSEIBox extends Box('vsib', size)
{
	unsigned int(8*size-64)	mvcscalinfosei; 
}
*/
public partial class ViewScalabilityInformationSEIBox : Box
{
	public const string TYPE = "vsib";
	public override string DisplayName { get { return "ViewScalabilityInformationSEIBox"; } }

	protected byte[] mvcscalinfosei; 
	public byte[] Mvcscalinfosei { get { return this.mvcscalinfosei; } set { this.mvcscalinfosei = value; } }

	public ViewScalabilityInformationSEIBox(ulong size = 0): base(IsoStream.FromFourCC("vsib"), size)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*size-64 ),  out this.mvcscalinfosei, "mvcscalinfosei"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits((uint)(8*size-64 ),  this.mvcscalinfosei, "mvcscalinfosei"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += (ulong)(8*size-64 ); // mvcscalinfosei
		return boxSize;
	}
}

}
