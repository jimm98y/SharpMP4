using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class RightsBox() extends Box('righ') {
 RightsEntry entries[];
}

*/
public partial class RightsBox : Box
{
	public const string TYPE = "righ";
	public override string DisplayName { get { return "RightsBox"; } }

	protected RightsEntry[] entries; 
	public RightsEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public RightsBox(): base(IsoStream.FromFourCC("righ"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new RightsEntry(),  out this.entries, "entries"); 
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
