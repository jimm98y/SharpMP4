using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCXYZNumber() {
	signed int(32) x;	// s15Fixed16Number
	signed int(32) y;
	signed int(32) z;
}
*/
public partial class ICCXYZNumber : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCXYZNumber() {
	signed int(32) x;	// s15Fixed16Number
	signed int(32) y;
	signed int(32) z;
}"; } }
	public virtual string DisplayName { get { return "ICCXYZNumber"; } }

	protected int x;  //  s15Fixed16Number
	public int _x { get { return this.x; } set { this.x = value; } }

	protected int y; 
	public int _y { get { return this.y; } set { this.y = value; } }

	protected int z; 
	public int _z { get { return this.z; } set { this.z = value; } }

	public ICCXYZNumber(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.x, "x"); // s15Fixed16Number
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.y, "y"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.z, "z"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteInt32( this.x, "x"); // s15Fixed16Number
		boxSize += stream.WriteInt32( this.y, "y"); 
		boxSize += stream.WriteInt32( this.z, "z"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // x
		boxSize += 32; // y
		boxSize += 32; // z
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("x", 32, this.x); // s15Fixed16Number
		boxSize += tracer.Field("y", 32, this.y); 
		boxSize += tracer.Field("z", 32, this.z); 
		return boxSize;
	}
}

}
