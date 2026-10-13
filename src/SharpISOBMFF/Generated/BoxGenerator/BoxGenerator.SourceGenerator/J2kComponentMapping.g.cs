using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class J2kComponentMapping() {
 unsigned int(16) component;
 unsigned int(8) mapping_type;
 unsigned int(8) palette_column;
 } 
*/
public partial class J2kComponentMapping : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "J2kComponentMapping"; } }

	protected ushort component; 
	public ushort Component { get { return this.component; } set { this.component = value; } }

	protected byte mapping_type; 
	public byte MappingType { get { return this.mapping_type; } set { this.mapping_type = value; } }

	protected byte palette_column; 
	public byte PaletteColumn { get { return this.palette_column; } set { this.palette_column = value; } }

	public J2kComponentMapping(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.component, "component"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.mapping_type, "mapping_type"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.palette_column, "palette_column"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.component, "component"); 
		boxSize += stream.WriteUInt8( this.mapping_type, "mapping_type"); 
		boxSize += stream.WriteUInt8( this.palette_column, "palette_column"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // component
		boxSize += 8; // mapping_type
		boxSize += 8; // palette_column
		return boxSize;
	}
}

}
