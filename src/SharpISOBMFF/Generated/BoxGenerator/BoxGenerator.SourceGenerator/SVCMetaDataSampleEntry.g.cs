using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SVCMetaDataSampleEntry () extends MetaDataSampleEntry('svcM')
{
	SVCMetadataSampleConfigBox	config;
	SVCPriorityAssignmentBox	methods;		// optional
	SVCPriorityLayerInfoBox		priorities;	// optional
}
*/
public partial class SVCMetaDataSampleEntry : MetaDataSampleEntry
{
	public const string TYPE = "svcM";
	public override string DisplayName { get { return "SVCMetaDataSampleEntry"; } }
	public SVCMetadataSampleConfigBox Config { get { return this.children.OfType<SVCMetadataSampleConfigBox>().FirstOrDefault(); } }
	public SVCPriorityAssignmentBox Methods { get { return this.children.OfType<SVCPriorityAssignmentBox>().FirstOrDefault(); } }
	public SVCPriorityLayerInfoBox Priorities { get { return this.children.OfType<SVCPriorityLayerInfoBox>().FirstOrDefault(); } }

	public SVCMetaDataSampleEntry(): base(IsoStream.FromFourCC("svcM"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.config, "config"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.methods, "methods"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.priorities, "priorities"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.config, "config"); 
		// boxSize += stream.WriteBox( this.methods, "methods"); // optional
		// boxSize += stream.WriteBox( this.priorities, "priorities"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(config); // config
		// boxSize += IsoStream.CalculateBoxSize(methods); // methods
		// boxSize += IsoStream.CalculateBoxSize(priorities); // priorities
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
