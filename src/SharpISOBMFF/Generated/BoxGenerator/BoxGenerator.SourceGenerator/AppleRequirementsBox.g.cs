using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleRequirementsBox() extends FullBox('©req', version = 0, 0) {
	string requirement;
}
*/
public partial class AppleRequirementsBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleRequirementsBox() extends FullBox('©req', version = 0, 0) {
	string requirement;
}"; } }
	public const string TYPE = "©req";
	public override string DisplayName { get { return "AppleRequirementsBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta", "ilst"); }


	protected BinaryUTF8String requirement; 
	public BinaryUTF8String Requirement { get { return this.requirement; } set { this.requirement = value; } }

	public AppleRequirementsBox(): base(IsoStream.FromFourCC("©req"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.requirement, "requirement"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.requirement, "requirement"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(requirement); // requirement
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("requirement", IsoStream.CalculateStringSize(requirement), this.requirement); 
		return boxSize;
	}
}

}
