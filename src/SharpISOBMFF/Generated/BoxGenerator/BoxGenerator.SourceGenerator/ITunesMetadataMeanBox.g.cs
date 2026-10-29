using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ITunesMetadataMeanBox() extends FullBox('mean') {
	string domain;
}
*/
public partial class ITunesMetadataMeanBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ITunesMetadataMeanBox() extends FullBox('mean') {
	string domain;
}"; } }
	public const string TYPE = "mean";
	public override string DisplayName { get { return "ITunesMetadataMeanBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("----"); }


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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("domain", IsoStream.CalculateStringSize(domain), this.domain); 
		return boxSize;
	}
}

}
