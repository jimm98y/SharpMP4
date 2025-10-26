using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class QoS_Descriptor extends BaseDescriptor : bit(8) tag=QoS_DescrTag {
 bit(8) predefined;
 if (predefined==0) {
 QoS_Qualifier qualifiers[];
 }
 }
*/
public partial class QoS_Descriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.QoS_DescrTag;
	public override string DisplayName { get { return "QoS_Descriptor"; } }

	protected byte predefined; 
	public byte Predefined { get { return this.predefined; } set { this.predefined = value; } }
	public IEnumerable<QoS_Qualifier> Qualifiers { get { return this.children.OfType<QoS_Qualifier>(); } }

	public QoS_Descriptor(): base(DescriptorTags.QoS_DescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.predefined, "predefined"); 

		if (predefined==0)
		{
			// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.qualifiers, "qualifiers"); 
		}
		boxSize += stream.ReadDescriptorsTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.predefined, "predefined"); 

		if (predefined==0)
		{
			// boxSize += stream.WriteDescriptor( this.qualifiers, "qualifiers"); 
		}
		boxSize += stream.WriteDescriptorsTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // predefined

		if (predefined==0)
		{
			// boxSize += IsoStream.CalculateDescriptorSize(qualifiers); // qualifiers
		}
		boxSize += IsoStream.CalculateDescriptors(this);
		return boxSize;
	}
}

}
