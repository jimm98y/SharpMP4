using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class FielBox() extends Box('fiel') {
 unsigned int(8) total;
 unsigned int(8) order;
 } 
*/
public partial class FielBox : Box
{
	public const string TYPE = "fiel";
	public override string DisplayName { get { return "FielBox"; } }

	protected byte total; 
	public byte Total { get { return this.total; } set { this.total = value; } }

	protected byte order; 
	public byte Order { get { return this.order; } set { this.order = value; } }

	public FielBox(): base(IsoStream.FromFourCC("fiel"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.total, "total"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.order, "order"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.total, "total"); 
		boxSize += stream.WriteUInt8( this.order, "order"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // total
		boxSize += 8; // order
		return boxSize;
	}
}

}
