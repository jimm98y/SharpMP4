using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AppleInitialObjectDescriptorBox() extends FullBox('iods') {
 Descriptor descriptor[0 .. 255];
 } 
*/
public partial class AppleInitialObjectDescriptorBox : FullBox
{
	public const string TYPE = "iods";
	public override string DisplayName { get { return "AppleInitialObjectDescriptorBox"; } }

	protected Descriptor[] descriptor; 
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
}

}
