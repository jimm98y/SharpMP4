using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class BoxedMetaDataSampleEntry 
	extends MetaDataSampleEntry ('mebx') {
	MetaDataKeyTableBox();				// mandatory
	BitRateBox ();							// optional
}
*/
public partial class BoxedMetaDataSampleEntry : MetaDataSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class BoxedMetaDataSampleEntry 
	extends MetaDataSampleEntry ('mebx') {
	MetaDataKeyTableBox();				// mandatory
	BitRateBox ();							// optional
}"; } }
	public const string TYPE = "mebx";
	public override string DisplayName { get { return "BoxedMetaDataSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public MetaDataKeyTableBox _MetaDataKeyTableBox { get { return this.children.OfType<MetaDataKeyTableBox>().FirstOrDefault(); } }
	public BitRateBox _BitRateBox { get { return this.children.OfType<BitRateBox>().FirstOrDefault(); } }

	public BoxedMetaDataSampleEntry(): base(IsoStream.FromFourCC("mebx"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataKeyTableBox, "MetaDataKeyTableBox"); // mandatory
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.BitRateBox, "BitRateBox"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.MetaDataKeyTableBox, "MetaDataKeyTableBox"); // mandatory
		// boxSize += stream.WriteBox( this.BitRateBox, "BitRateBox"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(MetaDataKeyTableBox); // MetaDataKeyTableBox
		// boxSize += IsoStream.CalculateBoxSize(BitRateBox); // BitRateBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("MetaDataKeyTableBox", IsoStream.CalculateBoxSize(MetaDataKeyTableBox), this.MetaDataKeyTableBox); // mandatory
		// boxSize += tracer.Field("BitRateBox", IsoStream.CalculateBoxSize(BitRateBox), this.BitRateBox); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
