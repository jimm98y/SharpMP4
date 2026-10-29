using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class CueTimeBox() extends Box ('ctim') {
 boxstring cueCurrentTime; 
 }
*/
public partial class CueTimeBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class CueTimeBox() extends Box ('ctim') {
 boxstring cueCurrentTime; 
 }"; } }
	public const string TYPE = "ctim";
	public override string DisplayName { get { return "CueTimeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	// its definitions place it in no container: wherever it is, it is not misplaced
	public override bool ValidatePlacement() { return IsPlacedIn(); }


	protected BinaryUTF8String cueCurrentTime; 
	public BinaryUTF8String CueCurrentTime { get { return this.cueCurrentTime; } set { this.cueCurrentTime = value; } }

	public CueTimeBox(): base(IsoStream.FromFourCC("ctim"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.cueCurrentTime, "cueCurrentTime"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.cueCurrentTime, "cueCurrentTime"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(cueCurrentTime); // cueCurrentTime
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("cueCurrentTime", IsoStream.CalculateStringSize(cueCurrentTime), this.cueCurrentTime); 
		return boxSize;
	}
}

}
