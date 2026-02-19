using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class TierDependencyBox extends Box('ldep'){
	unsigned int(16) entry_count; 
	for (i=0; i < entry_count; i++)
		unsigned int(16) dependencyTierId;
}
*/
public partial class TierDependencyBox : Box
{
	public const string TYPE = "ldep";
	public override string DisplayName { get { return "TierDependencyBox"; } }

	protected ushort entry_count; 
	public ushort EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected ushort[] dependencyTierId; 
	public ushort[] DependencyTierId { get { return this.dependencyTierId; } set { this.dependencyTierId = value; } }

	public TierDependencyBox(): base(IsoStream.FromFourCC("ldep"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.dependencyTierId = new ushort[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dependencyTierId[i], "dependencyTierId"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{
			boxSize += stream.WriteUInt16( this.dependencyTierId[i], "dependencyTierId"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // entry_count

		for (int i=0; i < entry_count; i++)
		{
			boxSize += 16; // dependencyTierId
		}
		return boxSize;
	}
}

}
