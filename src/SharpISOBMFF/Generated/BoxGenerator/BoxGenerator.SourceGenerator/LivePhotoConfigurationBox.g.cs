using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LivePhotoConfigurationBox() extends Box('cfgv') {
 bit(8) binaryPropertyList[];
}

*/
public partial class LivePhotoConfigurationBox : Box
{
	public const string TYPE = "cfgv";
	public override string DisplayName { get { return "LivePhotoConfigurationBox"; } }

	protected byte[] binaryPropertyList; 
	public byte[] BinaryPropertyList { get { return this.binaryPropertyList; } set { this.binaryPropertyList = value; } }

	public LivePhotoConfigurationBox(): base(IsoStream.FromFourCC("cfgv"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.binaryPropertyList, "binaryPropertyList"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.binaryPropertyList, "binaryPropertyList"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)binaryPropertyList.Length * 8); // binaryPropertyList
		return boxSize;
	}
}

}
