using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleGroupDescriptionBox ()
	extends FullBox('sgpd', version, flags){
	unsigned int(32) grouping_type;
	if (version>=1) { unsigned int(32) default_length; }
	if (version>=2) {
		unsigned int(32) default_group_description_index;
	}
	unsigned int(32) entry_count;
	int i;
	for (i = 1 ; i <= entry_count ; i++){
		if (version>=1) {
			if (default_length==0) {
				unsigned int(32) description_length;
			}
		}
		SampleGroupDescriptionEntry (grouping_type);
		// an instance of a class derived from SampleGroupDescriptionEntry
		//  that is appropriate and permitted for the media type
	}
}
*/
public partial class SampleGroupDescriptionBox : FullBox
{
	public const string TYPE = "sgpd";
	public override string DisplayName { get { return "SampleGroupDescriptionBox"; } }

	protected uint grouping_type; 
	public uint GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected uint default_length; 
	public uint DefaultLength { get { return this.default_length; } set { this.default_length = value; } }

	protected uint default_group_description_index; 
	public uint DefaultGroupDescriptionIndex { get { return this.default_group_description_index; } set { this.default_group_description_index = value; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected uint[] description_length; 
	public uint[] DescriptionLength { get { return this.description_length; } set { this.description_length = value; } }

	protected SampleGroupDescriptionEntry[] SampleGroupDescriptionEntry;  //  an instance of a class derived from SampleGroupDescriptionEntry
	public SampleGroupDescriptionEntry[] _SampleGroupDescriptionEntry { get { return this.SampleGroupDescriptionEntry; } set { this.SampleGroupDescriptionEntry = value; } }

	public SampleGroupDescriptionBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("sgpd"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type, "grouping_type"); 

		if (version>=1)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_length, "default_length"); 
		}

		if (version>=2)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.default_group_description_index, "default_group_description_index"); 
		}
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 
		

		this.description_length = new uint[IsoStream.GetInt( entry_count )];
		this.SampleGroupDescriptionEntry = new SampleGroupDescriptionEntry[IsoStream.GetInt( entry_count )];
		for (int i = 0 ; i < entry_count ; i++)
		{

			if (version>=1)
			{

				if (default_length==0)
				{
					boxSize += stream.ReadUInt32(boxSize, readSize,  out this.description_length[i], "description_length"); 
				}
			}
			boxSize += stream.ReadClass(boxSize, readSize, this, () => BoxFactory.CreateEntry(IsoStream.ToFourCC(grouping_type)),  out this.SampleGroupDescriptionEntry[i], "SampleGroupDescriptionEntry"); // an instance of a class derived from SampleGroupDescriptionEntry
			/*   that is appropriate and permitted for the media type */
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.grouping_type, "grouping_type"); 

		if (version>=1)
		{
			boxSize += stream.WriteUInt32( this.default_length, "default_length"); 
		}

		if (version>=2)
		{
			boxSize += stream.WriteUInt32( this.default_group_description_index, "default_group_description_index"); 
		}
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 
		

		for (int i = 0 ; i < entry_count ; i++)
		{

			if (version>=1)
			{

				if (default_length==0)
				{
					boxSize += stream.WriteUInt32( this.description_length[i], "description_length"); 
				}
			}
			boxSize += stream.WriteClass( this.SampleGroupDescriptionEntry[i], "SampleGroupDescriptionEntry"); // an instance of a class derived from SampleGroupDescriptionEntry
			/*   that is appropriate and permitted for the media type */
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // grouping_type

		if (version>=1)
		{
			boxSize += 32; // default_length
		}

		if (version>=2)
		{
			boxSize += 32; // default_group_description_index
		}
		boxSize += 32; // entry_count
		

		for (int i = 0 ; i < entry_count ; i++)
		{

			if (version>=1)
			{

				if (default_length==0)
				{
					boxSize += 32; // description_length
				}
			}
			boxSize += IsoStream.CalculateClassSize(SampleGroupDescriptionEntry); // SampleGroupDescriptionEntry
			/*   that is appropriate and permitted for the media type */
		}
		return boxSize;
	}
}

}
