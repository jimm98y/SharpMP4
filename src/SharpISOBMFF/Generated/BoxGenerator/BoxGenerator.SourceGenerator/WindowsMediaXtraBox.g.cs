using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class WindowsMediaXtraBox() extends Box('Xtra') {
	XtraTag tags[];
}
*/
public partial class WindowsMediaXtraBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class WindowsMediaXtraBox() extends Box('Xtra') {
	XtraTag tags[];
}"; } }
	public const string TYPE = "Xtra";
	public override string DisplayName { get { return "WindowsMediaXtraBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("udta"); }


	protected XtraTag[] tags = []; 
	public XtraTag[] Tags { get { return this.tags; } set { this.tags = value; } }

	public WindowsMediaXtraBox(): base(IsoStream.FromFourCC("Xtra"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new XtraTag(),  out this.tags, "tags"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.tags, "tags"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(tags); // tags
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("tags", IsoStream.CalculateClassSize(tags), this.tags); 
		return boxSize;
	}
}

}
