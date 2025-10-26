using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ViewPriorityEntry() extends VisualSampleGroupEntry ('vipr')
{
	ViewPriorityBox();
}
*/
public partial class ViewPriorityEntry : VisualSampleGroupEntry
{
	public const string TYPE = "vipr";
	public override string DisplayName { get { return "ViewPriorityEntry"; } }
	public ViewPriorityBox _ViewPriorityBox { get { return this.children.OfType<ViewPriorityBox>().FirstOrDefault(); } }

	public ViewPriorityEntry(): base(IsoStream.FromFourCC("vipr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ViewPriorityBox, "ViewPriorityBox"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.ViewPriorityBox, "ViewPriorityBox"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(ViewPriorityBox); // ViewPriorityBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
