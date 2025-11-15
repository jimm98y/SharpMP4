using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class URIInitBox
		extends FullBox('uriI', version = 0, 0) {
	unsigned int(8) uri_initialization_data[];
}
*/
public partial class URIInitBox : FullBox
{
	public const string TYPE = "uriI";
	public override string DisplayName { get { return "URIInitBox"; } }

	protected byte[] uri_initialization_data; 
	public byte[] UriInitializationData { get { return this.uri_initialization_data; } set { this.uri_initialization_data = value; } }

	public URIInitBox(): base(IsoStream.FromFourCC("uriI"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.uri_initialization_data, "uri_initialization_data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.uri_initialization_data, "uri_initialization_data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)uri_initialization_data.Length * 8); // uri_initialization_data
		return boxSize;
	}
}

}
