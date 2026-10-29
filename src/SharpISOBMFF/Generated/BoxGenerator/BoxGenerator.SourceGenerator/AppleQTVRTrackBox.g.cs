using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AppleQTVRTrackBox() extends Box ('qtvr'){
 unsigned int(32) reserved1;
 unsigned int(16) reserved2;
 unsigned int(16) dataRefIndex;
 unsigned int(32) data;
 }
*/
public partial class AppleQTVRTrackBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AppleQTVRTrackBox() extends Box ('qtvr'){
 unsigned int(32) reserved1;
 unsigned int(16) reserved2;
 unsigned int(16) dataRefIndex;
 unsigned int(32) data;
 }"; } }
	public const string TYPE = "qtvr";
	public override string DisplayName { get { return "AppleQTVRTrackBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }


	protected uint reserved1; 
	public uint Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort reserved2; 
	public ushort Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected ushort dataRefIndex; 
	public ushort DataRefIndex { get { return this.dataRefIndex; } set { this.dataRefIndex = value; } }

	protected uint data; 
	public uint Data { get { return this.data; } set { this.data = value; } }

	public AppleQTVRTrackBox(): base(IsoStream.FromFourCC("qtvr"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dataRefIndex, "dataRefIndex"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt16( this.dataRefIndex, "dataRefIndex"); 
		boxSize += stream.WriteUInt32( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reserved1
		boxSize += 16; // reserved2
		boxSize += 16; // dataRefIndex
		boxSize += 32; // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved1", 32, this.reserved1); 
		boxSize += tracer.Field("reserved2", 16, this.reserved2); 
		boxSize += tracer.Field("dataRefIndex", 16, this.dataRefIndex); 
		boxSize += tracer.Field("data", 32, this.data); 
		return boxSize;
	}
}

}
