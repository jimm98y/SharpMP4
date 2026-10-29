using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// XMP, a box of the file, not a full box as the XMLBox of a 'meta' is (JPEG 2000's XML box too)
aligned(8) class JpegXMLBox extends Box('xml ') {
	string data;
}
*/
public partial class JpegXMLBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// XMP, a box of the file, not a full box as the XMLBox of a 'meta' is (JPEG 2000's XML box too)
aligned(8) class JpegXMLBox extends Box('xml ') {
	string data;
}"; } }
	public const string TYPE = "xml ";
	public override string DisplayName { get { return "JpegXMLBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected BinaryUTF8String data; 
	public BinaryUTF8String Data { get { return this.data; } set { this.data = value; } }

	public JpegXMLBox(): base(IsoStream.FromFourCC("xml "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(data); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("data", IsoStream.CalculateStringSize(data), this.data); 
		return boxSize;
	}
}

}
