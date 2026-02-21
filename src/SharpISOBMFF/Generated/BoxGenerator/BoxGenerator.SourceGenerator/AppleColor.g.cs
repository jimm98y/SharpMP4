using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AppleColor() {
unsigned int(16) alpha;
 unsigned int(16) red;
 unsigned int(16) green;
 unsigned int(16) blue;
 }

*/
public partial class AppleColor : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AppleColor"; } }

	protected ushort alpha; 
	public ushort Alpha { get { return this.alpha; } set { this.alpha = value; } }

	protected ushort red; 
	public ushort Red { get { return this.red; } set { this.red = value; } }

	protected ushort green; 
	public ushort Green { get { return this.green; } set { this.green = value; } }

	protected ushort blue; 
	public ushort Blue { get { return this.blue; } set { this.blue = value; } }

	public AppleColor(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.alpha, "alpha"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.red, "red"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.green, "green"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.blue, "blue"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.alpha, "alpha"); 
		boxSize += stream.WriteUInt16( this.red, "red"); 
		boxSize += stream.WriteUInt16( this.green, "green"); 
		boxSize += stream.WriteUInt16( this.blue, "blue"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // alpha
		boxSize += 16; // red
		boxSize += 16; // green
		boxSize += 16; // blue
		return boxSize;
	}
}

}
