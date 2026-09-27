using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataSetupBox extends Box('setu') { // 'init' instead?
 bit(8) data[]; // as the key's namespace has it
}


*/
public partial class MetaDataSetupBox : Box
{
	public const string TYPE = "setu";
	public override string DisplayName { get { return "MetaDataSetupBox"; } }

	protected byte[] data;  //  as the key's namespace has it
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public MetaDataSetupBox(): base(IsoStream.FromFourCC("setu"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  'init' instead? */
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); // as the key's namespace has it
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  'init' instead? */
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); // as the key's namespace has it
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  'init' instead? */
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
