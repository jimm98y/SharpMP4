using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// JPEG 2000 component mapping (ISO/IEC 15444-1 I.5.3.5): of each channel, its component, and whether it is mapped directly or through the palette's column
aligned(8) class J2kComponentMappingBox() extends Box('cmap') {
	 J2kComponentMapping mappings[];
 } 
 
*/
public partial class J2kComponentMappingBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// JPEG 2000 component mapping (ISO/IEC 15444-1 I.5.3.5): of each channel, its component, and whether it is mapped directly or through the palette's column
aligned(8) class J2kComponentMappingBox() extends Box('cmap') {
	 J2kComponentMapping mappings[];
 } 
 "; } }
	public const string TYPE = "cmap";
	public override string DisplayName { get { return "J2kComponentMappingBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("j2kH"); }


	protected J2kComponentMapping[] mappings = []; 
	public J2kComponentMapping[] Mappings { get { return this.mappings; } set { this.mappings = value; } }

	public J2kComponentMappingBox(): base(IsoStream.FromFourCC("cmap"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new J2kComponentMapping(),  out this.mappings, "mappings"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.mappings, "mappings"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(mappings); // mappings
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("mappings", IsoStream.CalculateClassSize(mappings), this.mappings); 
		return boxSize;
	}
}

}
