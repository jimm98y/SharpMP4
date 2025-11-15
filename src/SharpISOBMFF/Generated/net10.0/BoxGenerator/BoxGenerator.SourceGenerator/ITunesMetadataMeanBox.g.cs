using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class ITunesMetadataMeanBox() extends FullBox('mean') {
 string domain; 
 }
*/
public partial class ITunesMetadataMeanBox : FullBox
{
	public const string TYPE = "mean";
	public override string DisplayName { get { return "ITunesMetadataMeanBox"; } }

	protected BinaryUTF8String domain; 
	public BinaryUTF8String Domain { get { return this.domain; } set { this.domain = value; } }

	public ITunesMetadataMeanBox(): base(IsoStream.FromFourCC("mean"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.domain, "domain"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.domain, "domain"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(domain); // domain
		return boxSize;
	}
}

}
