using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AppleImagePreviewBox() extends Box ('pnot'){
 unsigned int(32) modificationDate;
 signed int(16) version;
 signed int(32) atomType;
 signed int(16) atomIndex;
 }
*/
public partial class AppleImagePreviewBox : Box
{
	public const string TYPE = "pnot";
	public override string DisplayName { get { return "AppleImagePreviewBox"; } }

	protected uint modificationDate; 
	public uint ModificationDate { get { return this.modificationDate; } set { this.modificationDate = value; } }

	protected short version; 
	public short Version { get { return this.version; } set { this.version = value; } }

	protected int atomType; 
	public int AtomType { get { return this.atomType; } set { this.atomType = value; } }

	protected short atomIndex; 
	public short AtomIndex { get { return this.atomIndex; } set { this.atomIndex = value; } }

	public AppleImagePreviewBox(): base(IsoStream.FromFourCC("pnot"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.modificationDate, "modificationDate"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.version, "version"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.atomType, "atomType"); 
		boxSize += stream.ReadInt16(boxSize, readSize,  out this.atomIndex, "atomIndex"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.modificationDate, "modificationDate"); 
		boxSize += stream.WriteInt16( this.version, "version"); 
		boxSize += stream.WriteInt32( this.atomType, "atomType"); 
		boxSize += stream.WriteInt16( this.atomIndex, "atomIndex"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // modificationDate
		boxSize += 16; // version
		boxSize += 32; // atomType
		boxSize += 16; // atomIndex
		return boxSize;
	}
}

}
