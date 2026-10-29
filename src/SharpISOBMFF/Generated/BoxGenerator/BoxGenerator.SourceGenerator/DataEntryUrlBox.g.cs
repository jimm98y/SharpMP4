using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DataEntryUrlBox (bit(24) flags)
	extends DataEntryBaseBox('url ', flags) {
	utf8string location;
}
*/
public partial class DataEntryUrlBox : DataEntryBaseBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class DataEntryUrlBox (bit(24) flags)
	extends DataEntryBaseBox('url ', flags) {
	utf8string location;
}"; } }
	public const string TYPE = "url ";
	public override string DisplayName { get { return "DataEntryUrlBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("dref"); }


	protected BinaryUTF8String location; 
	public BinaryUTF8String Location { get { return this.location; } set { this.location = value; } }

	public DataEntryUrlBox(uint flags = 0): base(IsoStream.FromFourCC("url "), flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.location, "location"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.location, "location"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(location); // location
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("location", IsoStream.CalculateStringSize(location), this.location); 
		return boxSize;
	}
}

}
