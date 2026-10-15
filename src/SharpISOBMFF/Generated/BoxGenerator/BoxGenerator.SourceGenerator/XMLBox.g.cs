using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class XMLBox
	extends FullBox('xml ', version = 0, 0) {
	utfstring xml;
}
*/
public partial class XMLBox : FullBox
{
	public const string TYPE = "xml ";
	public override string DisplayName { get { return "XMLBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("FullBox(meta)"); }


	protected BinaryUTF8String xml; 
	public BinaryUTF8String Xml { get { return this.xml; } set { this.xml = value; } }

	public XMLBox(): base(IsoStream.FromFourCC("xml "), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.xml, "xml"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.xml, "xml"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(xml); // xml
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("xml", IsoStream.CalculateStringSize(xml), this.xml); 
		return boxSize;
	}
}

}
