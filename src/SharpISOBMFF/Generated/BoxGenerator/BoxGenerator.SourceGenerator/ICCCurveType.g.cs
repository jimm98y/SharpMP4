using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCCurveType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) entry_count;
	if (entry_count == 1) {
		unsigned int(16) gamma;	// u8Fixed8Number
	} else {
		unsigned int(16) curve[entry_count];
	}
}
*/
public partial class ICCCurveType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCCurveType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) entry_count;
	if (entry_count == 1) {
		unsigned int(16) gamma;	// u8Fixed8Number
	} else {
		unsigned int(16) curve[entry_count];
	}
}"; } }
	public virtual string DisplayName { get { return "ICCCurveType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected ushort gamma;  //  u8Fixed8Number
	public ushort Gamma { get { return this.gamma; } set { this.gamma = value; } }

	protected ushort[] curve = []; 
	public ushort[] Curve { get { return this.curve; } set { this.curve = value; } }

	public ICCCurveType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		if (entry_count == 1)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.gamma, "gamma"); // u8Fixed8Number
		}

		else 
		{
			boxSize += stream.ReadUInt16Array(boxSize, readSize, (uint)(entry_count),  out this.curve, "curve"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		if (entry_count == 1)
		{
			boxSize += stream.WriteUInt16( this.gamma, "gamma"); // u8Fixed8Number
		}

		else 
		{
			boxSize += stream.WriteUInt16Array((uint)(entry_count),  this.curve, "curve"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 32; // entry_count

		if (entry_count == 1)
		{
			boxSize += 16; // gamma
		}

		else 
		{
			boxSize += ((ulong)(entry_count) * 16); // curve
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("entry_count", 32, this.entry_count); 

		if (entry_count == 1)
		{
			boxSize += tracer.Field("gamma", 16, this.gamma); // u8Fixed8Number
		}

		else 
		{
			boxSize += tracer.Field("curve", ((ulong)(entry_count) * 16), this.curve); 
		}
		return boxSize;
	}
}

}
