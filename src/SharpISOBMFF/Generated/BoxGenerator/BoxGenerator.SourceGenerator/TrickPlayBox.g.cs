using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrickPlayBox() extends FullBox('trik') {
 TrickPlayEntry entries[];
 }
 
*/
public partial class TrickPlayBox : FullBox
{
	public const string TYPE = "trik";
	public override string DisplayName { get { return "TrickPlayBox"; } }

	protected TrickPlayEntry[] entries; 
	public TrickPlayEntry[] Entries { get { return this.entries; } set { this.entries = value; } }

	public TrickPlayBox(): base(IsoStream.FromFourCC("trik"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new TrickPlayEntry(),  out this.entries, "entries"); 
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
