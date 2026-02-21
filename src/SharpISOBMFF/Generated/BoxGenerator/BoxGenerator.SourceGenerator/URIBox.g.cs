using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class URIBox extends FullBox('uri ', version = 0, 0) {
	utf8string theURI;
}
*/
public partial class URIBox : FullBox
{
	public const string TYPE = "uri ";
	public override string DisplayName { get { return "URIBox"; } }

	protected BinaryUTF8String theURI; 
	public BinaryUTF8String TheURI { get { return this.theURI; } set { this.theURI = value; } }

	public URIBox(): base(IsoStream.FromFourCC("uri "), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.theURI, "theURI"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.theURI, "theURI"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(theURI); // theURI
		return boxSize;
	}
}

}
