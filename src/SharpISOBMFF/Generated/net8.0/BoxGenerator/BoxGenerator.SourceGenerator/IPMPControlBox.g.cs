using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class IPMPControlBox() extends FullBox('ipmc') {
 Descriptor toollist;
 unsigned int(8) count; Descriptor descriptor[count];
 }
*/
public partial class IPMPControlBox : FullBox
{
	public const string TYPE = "ipmc";
	public override string DisplayName { get { return "IPMPControlBox"; } }

	protected Descriptor toollist; 
	public Descriptor Toollist { get { return this.toollist; } set { this.toollist = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected Descriptor[] descriptor; 
	public Descriptor[] Descriptor { get { return this.descriptor; } set { this.descriptor = value; } }

	public IPMPControlBox(): base(IsoStream.FromFourCC("ipmc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.toollist, "toollist"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteDescriptor( this.toollist, "toollist"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		// boxSize += stream.WriteDescriptor( this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateDescriptorSize(toollist); // toollist
		boxSize += 8; // count
		// boxSize += IsoStream.CalculateDescriptorSize(descriptor); // descriptor
		return boxSize;
	}
}

}
