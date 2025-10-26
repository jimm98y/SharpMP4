using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ComponentDescription() { 
 unsigned int(32) componentType;
 unsigned int(32) componentSubType;
 unsigned int(32) componentManufacturer;
 unsigned int(32) componentFlags;
 unsigned int(32) componentFlagsMask;
 }
*/
public partial class ComponentDescription : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ComponentDescription"; } }

	protected uint componentType; 
	public uint ComponentType { get { return this.componentType; } set { this.componentType = value; } }

	protected uint componentSubType; 
	public uint ComponentSubType { get { return this.componentSubType; } set { this.componentSubType = value; } }

	protected uint componentManufacturer; 
	public uint ComponentManufacturer { get { return this.componentManufacturer; } set { this.componentManufacturer = value; } }

	protected uint componentFlags; 
	public uint ComponentFlags { get { return this.componentFlags; } set { this.componentFlags = value; } }

	protected uint componentFlagsMask; 
	public uint ComponentFlagsMask { get { return this.componentFlagsMask; } set { this.componentFlagsMask = value; } }

	public ComponentDescription(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentType, "componentType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentSubType, "componentSubType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentManufacturer, "componentManufacturer"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentFlags, "componentFlags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.componentFlagsMask, "componentFlagsMask"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.componentType, "componentType"); 
		boxSize += stream.WriteUInt32( this.componentSubType, "componentSubType"); 
		boxSize += stream.WriteUInt32( this.componentManufacturer, "componentManufacturer"); 
		boxSize += stream.WriteUInt32( this.componentFlags, "componentFlags"); 
		boxSize += stream.WriteUInt32( this.componentFlagsMask, "componentFlagsMask"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // componentType
		boxSize += 32; // componentSubType
		boxSize += 32; // componentManufacturer
		boxSize += 32; // componentFlags
		boxSize += 32; // componentFlagsMask
		return boxSize;
	}
}

}
