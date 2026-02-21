using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AppleVersionCheckBox() extends Box ('rmvc'){
 unsigned int(32) flags;
 unsigned int(32) softwarePackage;
 unsigned int(32) version;
 unsigned int(32) mask;
 signed int(16) checkType;
 }
*/
public partial class AppleVersionCheckBox : Box
{
	public const string TYPE = "rmvc";
	public override string DisplayName { get { return "AppleVersionCheckBox"; } }

	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	protected uint softwarePackage; 
	public uint SoftwarePackage { get { return this.softwarePackage; } set { this.softwarePackage = value; } }

	protected uint version; 
	public uint Version { get { return this.version; } set { this.version = value; } }

	protected uint mask; 
	public uint Mask { get { return this.mask; } set { this.mask = value; } }

	protected short checkType; 
	public short CheckType { get { return this.checkType; } set { this.checkType = value; } }

	public AppleVersionCheckBox(): base(IsoStream.FromFourCC("rmvc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.softwarePackage, "softwarePackage"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.mask, "mask"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.checkType, "checkType"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.flags, "flags"); 
		boxSize += stream.WriteUInt32( this.softwarePackage, "softwarePackage"); 
		boxSize += stream.WriteUInt32( this.version, "version"); 
		boxSize += stream.WriteUInt32( this.mask, "mask"); 
		boxSize += stream.WriteInt16( this.checkType, "checkType"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // flags
		boxSize += 32; // softwarePackage
		boxSize += 32; // version
		boxSize += 32; // mask
		boxSize += 16; // checkType
		return boxSize;
	}
}

}
