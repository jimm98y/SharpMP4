using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ID3TagBox() extends FullBox('ID32') {
 bit(1) pad;
 unsigned int(5)[3] language;
 bit(8) data[]; 
 }
*/
public partial class ID3TagBox : FullBox
{
	public const string TYPE = "ID32";
	public override string DisplayName { get { return "ID3TagBox"; } }

	protected bool pad; 
	public bool Pad { get { return this.pad; } set { this.pad = value; } }

	protected string language; 
	public string Language { get { return this.language; } set { this.language = value; } }

	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public ID3TagBox(): base(IsoStream.FromFourCC("ID32"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pad, "pad"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBit( this.pad, "pad"); 
		boxSize += stream.WriteIso639( this.language, "language"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 1; // pad
		boxSize += 15; // language
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}
}

}
