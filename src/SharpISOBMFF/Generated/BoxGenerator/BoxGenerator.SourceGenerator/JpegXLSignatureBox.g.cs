using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// The signature a JPEG XL container starts with: 0x0D0A870A
aligned(8) class JpegXLSignatureBox extends Box('JXL ') {
 unsigned int(32) signature;
 }
*/
public partial class JpegXLSignatureBox : Box
{
	public const string TYPE = "JXL ";
	public override string DisplayName { get { return "JpegXLSignatureBox"; } }

	protected uint signature; 
	public uint Signature { get { return this.signature; } set { this.signature = value; } }

	public JpegXLSignatureBox(): base(IsoStream.FromFourCC("JXL "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.signature, "signature"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.signature, "signature"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // signature
		return boxSize;
	}
}

}
