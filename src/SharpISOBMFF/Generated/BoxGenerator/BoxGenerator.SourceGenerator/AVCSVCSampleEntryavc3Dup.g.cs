using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*

*/
public partial class AVCSVCSampleEntryavc3Dup : AVCSampleEntry
{
	public const string TYPE = "avc3";
	public override string DisplayName { get { return "AVCSVCSampleEntryavc3Dup"; } }
	public SVCConfigurationBox Svcconfig { get { return this.children.OfType<SVCConfigurationBox>().FirstOrDefault(); } }
	public ScalabilityInformationSEIBox Scalability { get { return this.children.OfType<ScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public SVCPriorityAssignmentBox Method { get { return this.children.OfType<SVCPriorityAssignmentBox>().FirstOrDefault(); } }

	public AVCSVCSampleEntryavc3Dup(): base(IsoStream.FromFourCC("avc3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.svcconfig, "svcconfig"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.scalability, "scalability"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.method, "method"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.svcconfig, "svcconfig"); // optional
		// boxSize += stream.WriteBox( this.scalability, "scalability"); // optional
		// boxSize += stream.WriteBox( this.method, "method"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(svcconfig); // svcconfig
		// boxSize += IsoStream.CalculateBoxSize(scalability); // scalability
		// boxSize += IsoStream.CalculateBoxSize(method); // method
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
