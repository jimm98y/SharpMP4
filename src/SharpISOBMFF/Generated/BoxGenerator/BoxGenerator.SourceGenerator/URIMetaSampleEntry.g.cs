using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class URIMetaSampleEntry() extends MetaDataSampleEntry ('urim') {
	URIBox			the_label;
	URIInitBox		init;		// optional
}
*/
public partial class URIMetaSampleEntry : MetaDataSampleEntry
{
	public const string TYPE = "urim";
	public override string DisplayName { get { return "URIMetaSampleEntry"; } }
	public URIBox TheLabel { get { return this.children.OfType<URIBox>().FirstOrDefault(); } }
	public URIInitBox Init { get { return this.children.OfType<URIInitBox>().FirstOrDefault(); } }

	public URIMetaSampleEntry(): base(IsoStream.FromFourCC("urim"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.the_label, "the_label"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.init, "init"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.the_label, "the_label"); 
		// boxSize += stream.WriteBox( this.init, "init"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(the_label); // the_label
		// boxSize += IsoStream.CalculateBoxSize(init); // init
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
