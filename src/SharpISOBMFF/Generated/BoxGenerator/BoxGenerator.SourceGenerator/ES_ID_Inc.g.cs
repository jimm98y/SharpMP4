using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ES_ID_Inc extends BaseDescriptor : bit(8) tag=ES_ID_IncTag {
 bit(32) Track_ID;
 }
 
*/
public partial class ES_ID_Inc : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.ES_ID_IncTag;
	public override string DisplayName { get { return "ES_ID_Inc"; } }

	protected uint Track_ID; 
	public uint TrackID { get { return this.Track_ID; } set { this.Track_ID = value; } }

	public ES_ID_Inc(): base(DescriptorTags.ES_ID_IncTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.Track_ID, "Track_ID"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.Track_ID, "Track_ID"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // Track_ID
		return boxSize;
	}
}

}
