using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class AssetInformationBox() extends FullBox('ainf') {
 unsigned int(8) profileVersion[4];
 string apid;
 } 
*/
public partial class AssetInformationBox : FullBox
{
	public const string TYPE = "ainf";
	public override string DisplayName { get { return "AssetInformationBox"; } }

	protected byte[] profileVersion; 
	public byte[] ProfileVersion { get { return this.profileVersion; } set { this.profileVersion = value; } }

	protected BinaryUTF8String apid; 
	public BinaryUTF8String Apid { get { return this.apid; } set { this.apid = value; } }

	public AssetInformationBox(): base(IsoStream.FromFourCC("ainf"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 4,  out this.profileVersion, "profileVersion"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.apid, "apid"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8Array(4,  this.profileVersion, "profileVersion"); 
		boxSize += stream.WriteStringZeroTerminated( this.apid, "apid"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4 * 8; // profileVersion
		boxSize += IsoStream.CalculateStringSize(apid); // apid
		return boxSize;
	}
}

}
