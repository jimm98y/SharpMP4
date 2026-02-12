using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MpegSampleEntry() extends SampleEntry ('mp4s') {
 Box ES;
 }
*/
public partial class MpegSampleEntry : SampleEntry
{
	public const string TYPE = "mp4s";
	public override string DisplayName { get { return "MpegSampleEntry"; } }
	public Box _ES { get { return this.children.OfType<Box>().FirstOrDefault(); } }

	public MpegSampleEntry(): base(IsoStream.FromFourCC("mp4s"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ES, "ES"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.ES, "ES"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(ES); // ES
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
