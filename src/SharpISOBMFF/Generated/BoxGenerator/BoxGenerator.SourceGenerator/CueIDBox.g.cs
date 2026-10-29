using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class CueIDBox() extends Box ('iden') {
	boxstring cueID;
}
*/
public partial class CueIDBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class CueIDBox() extends Box ('iden') {
	boxstring cueID;
}"; } }
	public const string TYPE = "iden";
	public override string DisplayName { get { return "CueIDBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	// its definitions place it in no container: wherever it is, it is not misplaced
	public override bool ValidatePlacement() { return IsPlacedIn(); }


	protected BinaryUTF8String cueID; 
	public BinaryUTF8String CueID { get { return this.cueID; } set { this.cueID = value; } }

	public CueIDBox(): base(IsoStream.FromFourCC("iden"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.cueID, "cueID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.cueID, "cueID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(cueID); // cueID
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("cueID", IsoStream.CalculateStringSize(cueID), this.cueID); 
		return boxSize;
	}
}

}
