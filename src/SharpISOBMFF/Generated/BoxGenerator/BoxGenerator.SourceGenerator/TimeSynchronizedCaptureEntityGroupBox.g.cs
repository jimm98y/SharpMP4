using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23008-12 entity group 'tsyn': the fields of an EntityToGroupBox alone, as 23008-12-entity-groups.json has it
aligned(8) class TimeSynchronizedCaptureEntityGroupBox extends EntityToGroupBox('tsyn',0,0)
{
}
*/
public partial class TimeSynchronizedCaptureEntityGroupBox : EntityToGroupBox
{
	public const string TYPE = "tsyn";
	public override string DisplayName { get { return "TimeSynchronizedCaptureEntityGroupBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("grpl"); }


	public TimeSynchronizedCaptureEntityGroupBox(): base(IsoStream.FromFourCC("tsyn"), 0, 0)
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		return boxSize;
	}
}

}
