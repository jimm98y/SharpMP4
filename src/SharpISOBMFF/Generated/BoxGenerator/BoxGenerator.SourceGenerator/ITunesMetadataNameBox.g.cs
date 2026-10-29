using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ITunesMetadataNameBox() extends FullBox('name') {
	string name;
}
*/
public partial class ITunesMetadataNameBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ITunesMetadataNameBox() extends FullBox('name') {
	string name;
}"; } }
	public const string TYPE = "name";
	public override string DisplayName { get { return "ITunesMetadataNameBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("----", "udta", "tmcd", "schi"); }


	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	public ITunesMetadataNameBox(): base(IsoStream.FromFourCC("name"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(name); // name
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("name", IsoStream.CalculateStringSize(name), this.name); 
		return boxSize;
	}
}

}
