using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleRequirementsBox() extends FullBox('©req', version = 0, 0) {
string requirement;
 } 
*/
public partial class AppleRequirementsBox : FullBox
{
	public const string TYPE = "©req";
	public override string DisplayName { get { return "AppleRequirementsBox"; } }

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
}

}
