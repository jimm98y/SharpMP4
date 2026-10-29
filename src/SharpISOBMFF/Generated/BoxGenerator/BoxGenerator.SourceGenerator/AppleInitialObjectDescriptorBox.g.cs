using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleInitialObjectDescriptorBox() extends FullBox('iods') {
 Descriptor descriptor[0 .. 255];
 } 
*/
public partial class AppleInitialObjectDescriptorBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AppleInitialObjectDescriptorBox() extends FullBox('iods') {
 Descriptor descriptor[0 .. 255];
 } "; } }
	public const string TYPE = "iods";
	public override string DisplayName { get { return "AppleInitialObjectDescriptorBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("moov"); }


	protected Descriptor[] descriptor = []; 
	public Descriptor[] Descriptor { get { return this.descriptor; } set { this.descriptor = value; } }

	public AppleInitialObjectDescriptorBox(): base(IsoStream.FromFourCC("iods"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDescriptor( this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateDescriptorSize(descriptor); // descriptor
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("descriptor", IsoStream.CalculateDescriptorSize(descriptor), this.descriptor); 
		return boxSize;
	}
}

}
