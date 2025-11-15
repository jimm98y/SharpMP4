using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class CountryListEntry() { 
 unsigned int(16) country_count;
 unsigned int(16) country[ country_count ];
 }

*/
public partial class CountryListEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "CountryListEntry"; } }

	protected ushort country_count; 
	public ushort CountryCount { get { return this.country_count; } set { this.country_count = value; } }

	protected ushort[] country; 
	public ushort[] Country { get { return this.country; } set { this.country = value; } }

	public CountryListEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.country_count, "country_count"); 
		boxSize += stream.ReadUInt16Array(boxSize, readSize, (uint)( country_count ),  out this.country, "country"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.country_count, "country_count"); 
		boxSize += stream.WriteUInt16Array((uint)( country_count ),  this.country, "country"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // country_count
		boxSize += ((ulong)( country_count ) * 16); // country
		return boxSize;
	}
}

}
