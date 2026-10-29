using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AV1LayeredImageIndexingProperty extends ItemProperty('a1lx') {
	unsigned int(7) reserved = 0;
	unsigned int(1) large_size;
	if(large_size) {
		unsigned int(32) layer_size[3];
	}
	else {
		unsigned int(16) layer_size[3];
	}
}
*/
public partial class AV1LayeredImageIndexingProperty : ItemProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AV1LayeredImageIndexingProperty extends ItemProperty('a1lx') {
	unsigned int(7) reserved = 0;
	unsigned int(1) large_size;
	if(large_size) {
		unsigned int(32) layer_size[3];
	}
	else {
		unsigned int(16) layer_size[3];
	}
}"; } }
	public const string TYPE = "a1lx";
	public override string DisplayName { get { return "AV1LayeredImageIndexingProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool large_size; 
	public bool LargeSize { get { return this.large_size; } set { this.large_size = value; } }

	protected uint[] layer_size = new uint[3]; 
	public uint[] LayerSize { get { return this.layer_size; } set { this.layer_size = value; } }

	public AV1LayeredImageIndexingProperty(): base(IsoStream.FromFourCC("a1lx"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.large_size, "large_size"); 

		if (large_size)
		{
			boxSize += stream.ReadUInt32Array(boxSize, readSize, 3,  out this.layer_size, "layer_size"); 
		}

		else 
		{
			boxSize += stream.ReadUInt16Array(boxSize, readSize, 3,  out this.layer_size, "layer_size"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(7,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.large_size, "large_size"); 

		if (large_size)
		{
			boxSize += stream.WriteUInt32Array(3,  this.layer_size, "layer_size"); 
		}

		else 
		{
			boxSize += stream.WriteUInt16Array(3,  this.layer_size, "layer_size"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 7; // reserved
		boxSize += 1; // large_size

		if (large_size)
		{
			boxSize += 3 * 32; // layer_size
		}

		else 
		{
			boxSize += 3 * 16; // layer_size
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reserved", 7, this.reserved); 
		boxSize += tracer.Field("large_size", 1, this.large_size); 

		if (large_size)
		{
			boxSize += tracer.Field("layer_size", 3 * 32, this.layer_size); 
		}

		else 
		{
			boxSize += tracer.Field("layer_size", 3 * 16, this.layer_size); 
		}
		return boxSize;
	}
}

}
