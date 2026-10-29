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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @""; } }
	public const string TYPE = "avc3";
	public override string DisplayName { get { return "AVCSVCSampleEntryavc3Dup"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("svcconfig", IsoStream.CalculateBoxSize(svcconfig), this.svcconfig); // optional
		// boxSize += tracer.Field("scalability", IsoStream.CalculateBoxSize(scalability), this.scalability); // optional
		// boxSize += tracer.Field("method", IsoStream.CalculateBoxSize(method), this.method); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
