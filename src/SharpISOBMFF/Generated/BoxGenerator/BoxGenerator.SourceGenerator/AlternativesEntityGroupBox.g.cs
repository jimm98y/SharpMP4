using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 14496-12 entity group 'altr': the fields of an EntityToGroupBox alone, as 14496-12-entity-groups.json has it
aligned(8) class AlternativesEntityGroupBox extends EntityToGroupBox('altr',0,0)
{
}
*/
public partial class AlternativesEntityGroupBox : EntityToGroupBox
{
	public const string TYPE = "altr";
	public override string DisplayName { get { return "AlternativesEntityGroupBox"; } }

	public AlternativesEntityGroupBox(): base(IsoStream.FromFourCC("altr"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		return boxSize;
	}
}

}
