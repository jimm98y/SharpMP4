using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class URIInitBox
		extends FullBox('uriI', version = 0, 0) {
	unsigned int(8) uri_initialization_data[];
}"; } }
	public const string TYPE = "uriI";
	public override string DisplayName { get { return "URIInitBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("URIMetaSampleEntry(urim)"); }


	protected byte[] uri_initialization_data = []; 
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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("uri_initialization_data", ((ulong)uri_initialization_data.Length * 8), this.uri_initialization_data); 
		return boxSize;
	}
}

}
