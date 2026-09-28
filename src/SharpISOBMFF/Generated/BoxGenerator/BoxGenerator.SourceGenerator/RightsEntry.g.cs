using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class RightsEntry() {
 unsigned int(32) key; // veID, plat, aver, tran, song, tool, medi, mode
 unsigned int(32) value;
}
*/
public partial class RightsEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "RightsEntry"; } }

	protected uint key;  //  veID, plat, aver, tran, song, tool, medi, mode
	public uint Key { get { return this.key; } set { this.key = value; } }

	protected uint value; 
	public uint Value { get { return this.value; } set { this.value = value; } }

	public RightsEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.key, "key"); // veID, plat, aver, tran, song, tool, medi, mode
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.value, "value"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.key, "key"); // veID, plat, aver, tran, song, tool, medi, mode
		boxSize += stream.WriteUInt32( this.value, "value"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // key
		boxSize += 32; // value
		return boxSize;
	}
}

}
