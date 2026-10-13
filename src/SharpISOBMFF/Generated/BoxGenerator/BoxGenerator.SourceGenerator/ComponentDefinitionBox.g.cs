using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ISO/IEC 23001-17 component definition: what each component is - of a type of 0x8000 or more, of a URI
aligned(8) class ComponentDefinitionBox() extends Box('cmpd') {
	 unsigned int(32) component_count;
 for (i = 0; i < component_count; i++) {
 unsigned int(16) component_type;
 if (component_type >= 0x8000) {
 utf8string component_type_uri;
 }
 }
 } 
*/
public partial class ComponentDefinitionBox : Box
{
	public const string TYPE = "cmpd";
	public override string DisplayName { get { return "ComponentDefinitionBox"; } }

	protected uint component_count; 
	public uint ComponentCount { get { return this.component_count; } set { this.component_count = value; } }

	protected ushort[] component_type; 
	public ushort[] ComponentType { get { return this.component_type; } set { this.component_type = value; } }

	protected BinaryUTF8String[] component_type_uri; 
	public BinaryUTF8String[] ComponentTypeUri { get { return this.component_type_uri; } set { this.component_type_uri = value; } }

	public ComponentDefinitionBox(): base(IsoStream.FromFourCC("cmpd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.component_count, "component_count"); 

		this.component_type = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( component_count), "component_type");
		this.component_type_uri = stream.SafeAllocate<BinaryUTF8String>(boxSize, readSize, IsoStream.GetInt( component_count), "component_type_uri");
		for (int i = 0; i < component_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.component_type[i], "component_type"); 

			if (component_type[i] >= 0x8000)
			{
				boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.component_type_uri[i], "component_type_uri"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.component_count, "component_count"); 

		for (int i = 0; i < component_count; i++)
		{
			boxSize += stream.WriteUInt16( this.component_type[i], "component_type"); 

			if (component_type[i] >= 0x8000)
			{
				boxSize += stream.WriteStringZeroTerminated( this.component_type_uri[i], "component_type_uri"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // component_count

		for (int i = 0; i < component_count; i++)
		{
			boxSize += 16; // component_type

			if (component_type[i] >= 0x8000)
			{
				boxSize += IsoStream.CalculateStringSize(component_type_uri[i]); // component_type_uri
			}
		}
		return boxSize;
	}
}

}
