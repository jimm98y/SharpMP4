using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrickPlayEntry() {
	unsigned int(2) picType;
	unsigned int(6) dependencyLevel;
}
*/
public partial class TrickPlayEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class TrickPlayEntry() {
	unsigned int(2) picType;
	unsigned int(6) dependencyLevel;
}"; } }
	public virtual string DisplayName { get { return "TrickPlayEntry"; } }

	protected byte picType; 
	public byte PicType { get { return this.picType; } set { this.picType = value; } }

	protected byte dependencyLevel; 
	public byte DependencyLevel { get { return this.dependencyLevel; } set { this.dependencyLevel = value; } }

	public TrickPlayEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.picType, "picType"); 
		boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.dependencyLevel, "dependencyLevel"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(2,  this.picType, "picType"); 
		boxSize += stream.WriteBits(6,  this.dependencyLevel, "dependencyLevel"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 2; // picType
		boxSize += 6; // dependencyLevel
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("picType", 2, this.picType); 
		boxSize += tracer.Field("dependencyLevel", 6, this.dependencyLevel); 
		return boxSize;
	}
}

}
