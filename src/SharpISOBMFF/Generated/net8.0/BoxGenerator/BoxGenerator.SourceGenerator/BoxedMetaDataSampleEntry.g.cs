using System;
using System.Linq;
using System.Collections.Generic;

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
	public const string TYPE = "mebx";
	public override string DisplayName { get { return "BoxedMetaDataSampleEntry"; } }
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
}

}
