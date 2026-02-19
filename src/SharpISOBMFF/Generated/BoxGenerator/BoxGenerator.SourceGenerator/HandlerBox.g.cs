using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class HandlerBox extends FullBox('hdlr', version = 0, 0) {
	unsigned int(32)	pre_defined = 0;
	unsigned int(32)	handler_type;
	const unsigned int(32)[3]	reserved = 0;
	utf8string	name;
}
*/
public partial class HandlerBox : FullBox
{
	public const string TYPE = "hdlr";
	public override string DisplayName { get { return "HandlerBox"; } }

	protected uint pre_defined = 0; 
	public uint PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	protected uint handler_type; 
	public uint HandlerType { get { return this.handler_type; } set { this.handler_type = value; } }

	protected uint[] reserved = []; 
	public uint[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	public HandlerBox(): base(IsoStream.FromFourCC("hdlr"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.pre_defined, "pre_defined"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.handler_type, "handler_type"); 
		boxSize += stream.ReadUInt32Array(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.pre_defined, "pre_defined"); 
		boxSize += stream.WriteUInt32( this.handler_type, "handler_type"); 
		boxSize += stream.WriteUInt32Array(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // pre_defined
		boxSize += 32; // handler_type
		boxSize += 3 * 32; // reserved
		boxSize += IsoStream.CalculateStringSize(name); // name
		return boxSize;
	}
}

}
