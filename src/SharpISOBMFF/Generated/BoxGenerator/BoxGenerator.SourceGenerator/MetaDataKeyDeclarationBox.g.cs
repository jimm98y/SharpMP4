using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataKeyDeclarationBox extends Box('keyd') {
 unsigned int(32) key_namespace;
 unsigned int(8) key_value[];
}


*/
public partial class MetaDataKeyDeclarationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MetaDataKeyDeclarationBox extends Box('keyd') {
 unsigned int(32) key_namespace;
 unsigned int(8) key_value[];
}

"; } }
	public const string TYPE = "keyd";
	public override string DisplayName { get { return "MetaDataKeyDeclarationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("*"); }


	protected uint key_namespace; 
	public uint KeyNamespace { get { return this.key_namespace; } set { this.key_namespace = value; } }

	protected byte[] key_value = []; 
	public byte[] KeyValue { get { return this.key_value; } set { this.key_value = value; } }

	public MetaDataKeyDeclarationBox(): base(IsoStream.FromFourCC("keyd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.key_namespace, "key_namespace"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.key_value, "key_value"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.key_namespace, "key_namespace"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.key_value, "key_value"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // key_namespace
		boxSize += ((ulong)key_value.Length * 8); // key_value
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("key_namespace", 32, this.key_namespace); 
		boxSize += tracer.Field("key_value", ((ulong)key_value.Length * 8), this.key_value); 
		return boxSize;
	}
}

}
