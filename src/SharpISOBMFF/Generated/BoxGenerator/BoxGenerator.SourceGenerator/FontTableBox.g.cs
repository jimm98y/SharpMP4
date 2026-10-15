using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FontTableBox() extends Box('ftab') {
 unsigned int(16) entryCount;
 FontRecord fontEntries[entryCount];
 }
 
*/
public partial class FontTableBox : Box
{
	public const string TYPE = "ftab";
	public override string DisplayName { get { return "FontTableBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("SampleEntry(tx3g,enct)"); }


	protected ushort entryCount; 
	public ushort EntryCount { get { return this.entryCount; } set { this.entryCount = value; } }

	protected FontRecord[] fontEntries; 
	public FontRecord[] FontEntries { get { return this.fontEntries; } set { this.fontEntries = value; } }

	public FontTableBox(): base(IsoStream.FromFourCC("ftab"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entryCount, "entryCount"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(entryCount), () => new FontRecord(),  out this.fontEntries, "fontEntries"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entryCount, "entryCount"); 
		boxSize += stream.WriteClass( this.fontEntries, "fontEntries"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entryCount
		boxSize += IsoStream.CalculateClassSize(fontEntries); // fontEntries
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("entryCount", 16, this.entryCount); 
		boxSize += builder.Field("fontEntries", IsoStream.CalculateClassSize(fontEntries), this.fontEntries); 
		return boxSize;
	}
}

}
