using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AVC2SVCSampleEntry() extends AVC2SampleEntry('avc2' or 'avc4') {
	SVCConfigurationBox	svcconfig;			// optional
	ScalabilityInformationSEIBox	scalability;	// optional
	SVCPriorityAssignmentBox	method;			// optional
}
*/
public partial class AVC2SVCSampleEntry : AVC2SampleEntry
{
	public const string TYPE = "avc2";
	public override string DisplayName { get { return "AVC2SVCSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public SVCConfigurationBox Svcconfig { get { return this.children.OfType<SVCConfigurationBox>().FirstOrDefault(); } }
	public ScalabilityInformationSEIBox Scalability { get { return this.children.OfType<ScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public SVCPriorityAssignmentBox Method { get { return this.children.OfType<SVCPriorityAssignmentBox>().FirstOrDefault(); } }

	public AVC2SVCSampleEntry(): base(IsoStream.FromFourCC("avc2"))
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		// boxSize += builder.Field("svcconfig", IsoStream.CalculateBoxSize(svcconfig), this.svcconfig); // optional
		// boxSize += builder.Field("scalability", IsoStream.CalculateBoxSize(scalability), this.scalability); // optional
		// boxSize += builder.Field("method", IsoStream.CalculateBoxSize(method), this.method); // optional
		boxSize += builder.Children(this);
		return boxSize;
	}
}

}
