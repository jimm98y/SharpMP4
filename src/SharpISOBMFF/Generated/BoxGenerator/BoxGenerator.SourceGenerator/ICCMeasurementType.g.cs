using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCMeasurementType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) standard_observer;	// 1: CIE 1931, 2: CIE 1964
	ICCXYZNumber backing;	// the tristimulus values of the measurement backing
	unsigned int(32) geometry;
	unsigned int(32) flare;	// u16Fixed16Number
	unsigned int(32) illuminant;	// 1: D50, 2: D65, 3: D93, 4: F2, 5: D55, 6: A, 7: E, 8: F8
}
*/
public partial class ICCMeasurementType : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCMeasurementType {
	unsigned int(32) type_signature;
	unsigned int(32) reserved = 0;
	unsigned int(32) standard_observer;	// 1: CIE 1931, 2: CIE 1964
	ICCXYZNumber backing;	// the tristimulus values of the measurement backing
	unsigned int(32) geometry;
	unsigned int(32) flare;	// u16Fixed16Number
	unsigned int(32) illuminant;	// 1: D50, 2: D65, 3: D93, 4: F2, 5: D55, 6: A, 7: E, 8: F8
}"; } }
	public virtual string DisplayName { get { return "ICCMeasurementType"; } }

	protected uint type_signature; 
	public uint TypeSignature { get { return this.type_signature; } set { this.type_signature = value; } }

	protected uint reserved = 0; 
	public uint Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint standard_observer;  //  1: CIE 1931, 2: CIE 1964
	public uint StandardObserver { get { return this.standard_observer; } set { this.standard_observer = value; } }

	protected ICCXYZNumber backing;  //  the tristimulus values of the measurement backing
	public ICCXYZNumber Backing { get { return this.backing; } set { this.backing = value; } }

	protected uint geometry; 
	public uint Geometry { get { return this.geometry; } set { this.geometry = value; } }

	protected uint flare;  //  u16Fixed16Number
	public uint Flare { get { return this.flare; } set { this.flare = value; } }

	protected uint illuminant;  //  1: D50, 2: D65, 3: D93, 4: F2, 5: D55, 6: A, 7: E, 8: F8
	public uint Illuminant { get { return this.illuminant; } set { this.illuminant = value; } }

	public ICCMeasurementType(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type_signature, "type_signature"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.standard_observer, "standard_observer"); // 1: CIE 1931, 2: CIE 1964
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new ICCXYZNumber(),  out this.backing, "backing"); // the tristimulus values of the measurement backing
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.geometry, "geometry"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.flare, "flare"); // u16Fixed16Number
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.illuminant, "illuminant"); // 1: D50, 2: D65, 3: D93, 4: F2, 5: D55, 6: A, 7: E, 8: F8
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.type_signature, "type_signature"); 
		boxSize += stream.WriteUInt32( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.standard_observer, "standard_observer"); // 1: CIE 1931, 2: CIE 1964
		boxSize += stream.WriteClass( this.backing, "backing"); // the tristimulus values of the measurement backing
		boxSize += stream.WriteUInt32( this.geometry, "geometry"); 
		boxSize += stream.WriteUInt32( this.flare, "flare"); // u16Fixed16Number
		boxSize += stream.WriteUInt32( this.illuminant, "illuminant"); // 1: D50, 2: D65, 3: D93, 4: F2, 5: D55, 6: A, 7: E, 8: F8
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // type_signature
		boxSize += 32; // reserved
		boxSize += 32; // standard_observer
		boxSize += IsoStream.CalculateClassSize(backing); // backing
		boxSize += 32; // geometry
		boxSize += 32; // flare
		boxSize += 32; // illuminant
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("type_signature", 32, this.type_signature); 
		boxSize += tracer.Field("reserved", 32, this.reserved); 
		boxSize += tracer.Field("standard_observer", 32, this.standard_observer); // 1: CIE 1931, 2: CIE 1964
		boxSize += tracer.Field("backing", IsoStream.CalculateClassSize(backing), this.backing); // the tristimulus values of the measurement backing
		boxSize += tracer.Field("geometry", 32, this.geometry); 
		boxSize += tracer.Field("flare", 32, this.flare); // u16Fixed16Number
		boxSize += tracer.Field("illuminant", 32, this.illuminant); // 1: D50, 2: D65, 3: D93, 4: F2, 5: D55, 6: A, 7: E, 8: F8
		return boxSize;
	}
}

}
