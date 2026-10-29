using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class DataBox() extends Box('data') {
	unsigned int(32) dataType;
	unsigned int(32) dataLang;
	bit(8) data[];
}
*/
public partial class DataBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class DataBox() extends Box('data') {
	unsigned int(32) dataType;
	unsigned int(32) dataLang;
	bit(8) data[];
}"; } }
	public const string TYPE = "data";
	public override string DisplayName { get { return "DataBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("*"); }


	protected uint dataType; 
	public uint DataType { get { return this.dataType; } set { this.dataType = value; } }

	protected uint dataLang; 
	public uint DataLang { get { return this.dataLang; } set { this.dataLang = value; } }

	protected byte[] data = []; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public DataBox(): base(IsoStream.FromFourCC("data"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dataType, "dataType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.dataLang, "dataLang"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.dataType, "dataType"); 
		boxSize += stream.WriteUInt32( this.dataLang, "dataLang"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // dataType
		boxSize += 32; // dataLang
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("dataType", 32, this.dataType); 
		boxSize += tracer.Field("dataLang", 32, this.dataLang); 
		boxSize += tracer.Field("data", ((ulong)data.Length * 8), this.data); 
		return boxSize;
	}
}

}
