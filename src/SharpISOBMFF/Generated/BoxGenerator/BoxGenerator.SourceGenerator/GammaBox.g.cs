using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class GammaBox() extends Box ('gama'){
 unsigned int(32) gamma;
 }
*/
public partial class GammaBox : Box
{
	public const string TYPE = "gama";
	public override string DisplayName { get { return "GammaBox"; } }

	protected uint gamma; 
	public uint Gamma { get { return this.gamma; } set { this.gamma = value; } }

	public GammaBox(): base(IsoStream.FromFourCC("gama"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.gamma, "gamma"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.gamma, "gamma"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // gamma
		return boxSize;
	}
}

}
