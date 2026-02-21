using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AppleComponentDetectBox() extends Box ('rmcd'){
 unsigned int(32) flags;
 ComponentDescription description;
 unsigned int(32) minimumVersion;
 } 
*/
public partial class AppleComponentDetectBox : Box
{
	public const string TYPE = "rmcd";
	public override string DisplayName { get { return "AppleComponentDetectBox"; } }

	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	protected ComponentDescription description; 
	public ComponentDescription Description { get { return this.description; } set { this.description = value; } }

	protected uint minimumVersion; 
	public uint MinimumVersion { get { return this.minimumVersion; } set { this.minimumVersion = value; } }

	public AppleComponentDetectBox(): base(IsoStream.FromFourCC("rmcd"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.flags, "flags"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new ComponentDescription(),  out this.description, "description"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.minimumVersion, "minimumVersion"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.flags, "flags"); 
		boxSize += stream.WriteClass( this.description, "description"); 
		boxSize += stream.WriteUInt32( this.minimumVersion, "minimumVersion"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // flags
		boxSize += IsoStream.CalculateClassSize(description); // description
		boxSize += 32; // minimumVersion
		return boxSize;
	}
}

}
