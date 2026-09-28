using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// The salt of the key, a box and not a full box, as GPAC reads it
aligned(8) class ISMACrypSaltBox extends Box('iSLT') {
 unsigned int(64) salt;
 }
*/
public partial class ISMACrypSaltBox : Box
{
	public const string TYPE = "iSLT";
	public override string DisplayName { get { return "ISMACrypSaltBox"; } }

	protected ulong salt; 
	public ulong Salt { get { return this.salt; } set { this.salt = value; } }

	public ISMACrypSaltBox(): base(IsoStream.FromFourCC("iSLT"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.salt, "salt"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt64( this.salt, "salt"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 64; // salt
		return boxSize;
	}
}

}
