using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ViewPriorityBox extends Box ('vipr') {
 ViprEntry entries[]; 
} 
 
*/
public partial class ViewPriorityBox : Box
{
	public const string TYPE = "vipr";
	public override string DisplayName { get { return "ViewPriorityBox"; } }

	protected ViprEntry[] entries; 
	public ViprEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public ViewPriorityBox(): base(IsoStream.FromFourCC("vipr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new ViprEntry(),  out this.entries, "entries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.entries, "entries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(entries); // entries
		return boxSize;
	}
}

}
