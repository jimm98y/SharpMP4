using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MetaDataKeyTableBox extends FullBox('keys') { 
        unsigned int(32) entry_count;
        MetaDataKeyBox[];
    }
*/
public partial class MetaDataKeyTableBox : FullBox
{
	public const string TYPE = "keys";
	public override string DisplayName { get { return "MetaDataKeyTableBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }
	public IEnumerable<MetaDataKeyBox> _MetaDataKeyBox { get { return this.children.OfType<MetaDataKeyBox>(); } }

	public MetaDataKeyTableBox(): base(IsoStream.FromFourCC("keys"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataKeyBox, "MetaDataKeyBox"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		// boxSize += stream.WriteBox( this.MetaDataKeyBox, "MetaDataKeyBox"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count
		// boxSize += IsoStream.CalculateBoxSize(MetaDataKeyBox); // MetaDataKeyBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
