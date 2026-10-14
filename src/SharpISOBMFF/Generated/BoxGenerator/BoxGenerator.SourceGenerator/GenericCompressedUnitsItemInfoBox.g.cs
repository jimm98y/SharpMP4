using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-17 generic compressed units: of each unit compressed alone, where it is and its size - of fields sized by the codes it starts with, which CompressedUnitsInfo reads
aligned(8) class GenericCompressedUnitsItemInfoBox() extends FullBox('icef') {
	 CompressedUnitsInfo units;
 } 
*/
public partial class GenericCompressedUnitsItemInfoBox : FullBox
{
	public const string TYPE = "icef";
	public override string DisplayName { get { return "GenericCompressedUnitsItemInfoBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected CompressedUnitsInfo units; 
	public CompressedUnitsInfo Units { get { return this.units; } set { this.units = value; } }

	public GenericCompressedUnitsItemInfoBox(): base(IsoStream.FromFourCC("icef"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new CompressedUnitsInfo(),  out this.units, "units"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.units, "units"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(units); // units
		return boxSize;
	}
}

}
