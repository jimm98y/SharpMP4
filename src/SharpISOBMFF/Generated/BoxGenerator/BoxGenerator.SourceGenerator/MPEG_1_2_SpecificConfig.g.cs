using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MPEG_1_2_SpecificConfig()
{
	bslbf(1) extension;
}
*/
public partial class MPEG_1_2_SpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class MPEG_1_2_SpecificConfig()
{
	bslbf(1) extension;
}"; } }
	public virtual string DisplayName { get { return "MPEG_1_2_SpecificConfig"; } }

	protected bool extension; 
	public bool Extension { get { return this.extension; } set { this.extension = value; } }

	public MPEG_1_2_SpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.extension, "extension"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.extension, "extension"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // extension
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("extension", 1, this.extension); 
		return boxSize;
	}
}

}
