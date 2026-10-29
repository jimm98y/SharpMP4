using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class GenericCompressionConfigurationBox() extends FullBox('cmpC') {
	 unsigned int(32) compression_type;
 unsigned int(8) unit_type;
 } 
*/
public partial class GenericCompressionConfigurationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class GenericCompressionConfigurationBox() extends FullBox('cmpC') {
	 unsigned int(32) compression_type;
 unsigned int(8) unit_type;
 } "; } }
	public const string TYPE = "cmpC";
	public override string DisplayName { get { return "GenericCompressionConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("schi", "ipco"); }


	protected uint compression_type; 
	public uint CompressionType { get { return this.compression_type; } set { this.compression_type = value; } }

	protected byte unit_type; 
	public byte UnitType { get { return this.unit_type; } set { this.unit_type = value; } }

	public GenericCompressionConfigurationBox(): base(IsoStream.FromFourCC("cmpC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.compression_type, "compression_type"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.unit_type, "unit_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.compression_type, "compression_type"); 
		boxSize += stream.WriteUInt8( this.unit_type, "unit_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // compression_type
		boxSize += 8; // unit_type
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("compression_type", 32, this.compression_type); 
		boxSize += tracer.Field("unit_type", 8, this.unit_type); 
		return boxSize;
	}
}

}
