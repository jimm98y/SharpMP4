using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class XMPCustomBox() extends Box('uuid be7acfcb97a942e89c71999491e3afac') {
	 string data;
 }
*/
public partial class XMPCustomBox : Box
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "XMPCustomBox"; } }

	protected BinaryUTF8String data; 
	public BinaryUTF8String Data { get { return this.data; } set { this.data = value; } }

	public XMPCustomBox(): base(IsoStream.FromFourCC("uuid"), Convert.FromHexString("be7acfcb97a942e89c71999491e3afac"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(data); // data
		return boxSize;
	}
}

}
