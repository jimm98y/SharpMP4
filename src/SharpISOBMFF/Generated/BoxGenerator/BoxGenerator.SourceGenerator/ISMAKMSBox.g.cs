using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// The URI of the key management system, to the end of the box
aligned(8) class ISMAKMSBox extends FullBox('iKMS', 0, 0) {
 string kms_URI;
 }
*/
public partial class ISMAKMSBox : FullBox
{
	public const string TYPE = "iKMS";
	public override string DisplayName { get { return "ISMAKMSBox"; } }

	protected BinaryUTF8String kms_URI; 
	public BinaryUTF8String KmsURI { get { return this.kms_URI; } set { this.kms_URI = value; } }

	public ISMAKMSBox(): base(IsoStream.FromFourCC("iKMS"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.kms_URI, "kms_URI"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.kms_URI, "kms_URI"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(kms_URI); // kms_URI
		return boxSize;
	}
}

}
