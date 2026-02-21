using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EditListBox extends FullBox('elst', version, flags) {
	unsigned int(32)	entry_count;
	for (i=1; i <= entry_count; i++) {
		if (version==1) {
			unsigned int(64) edit_duration;
			int(64) media_time;
		} else { // version==0
			unsigned int(32) edit_duration;
			int(32)	media_time;
		}
		int(16) media_rate_integer;
		int(16) media_rate_fraction;
	}
}
*/
public partial class EditListBox : FullBox
{
	public const string TYPE = "elst";
	public override string DisplayName { get { return "EditListBox"; } }

	protected uint entry_count; 
	public uint EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected ulong[] edit_duration; 
	public ulong[] EditDuration { get { return this.edit_duration; } set { this.edit_duration = value; } }

	protected long[] media_time; 
	public long[] MediaTime { get { return this.media_time; } set { this.media_time = value; } }

	protected short[] media_rate_integer; 
	public short[] MediaRateInteger { get { return this.media_rate_integer; } set { this.media_rate_integer = value; } }

	protected short[] media_rate_fraction; 
	public short[] MediaRateFraction { get { return this.media_rate_fraction; } set { this.media_rate_fraction = value; } }

	public EditListBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("elst"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.edit_duration = new ulong[IsoStream.GetInt( entry_count)];
		this.media_time = new long[IsoStream.GetInt( entry_count)];
		this.media_rate_integer = new short[IsoStream.GetInt( entry_count)];
		this.media_rate_fraction = new short[IsoStream.GetInt( entry_count)];
		for (int i=0; i < entry_count; i++)
		{

			if (version==1)
			{
				boxSize += stream.ReadUInt64(boxSize, readSize,  out this.edit_duration[i], "edit_duration"); 
				boxSize += stream.ReadInt64(boxSize, readSize,  out this.media_time[i], "media_time"); 
			}

			else 
			{
				/*  version==0 */
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.edit_duration[i], "edit_duration"); 
				boxSize += stream.ReadInt32(boxSize, readSize,  out this.media_time[i], "media_time"); 
			}
			boxSize += stream.ReadInt16(boxSize, readSize,  out this.media_rate_integer[i], "media_rate_integer"); 
			boxSize += stream.ReadInt16(boxSize, readSize,  out this.media_rate_fraction[i], "media_rate_fraction"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.entry_count, "entry_count"); 

		for (int i=0; i < entry_count; i++)
		{

			if (version==1)
			{
				boxSize += stream.WriteUInt64( this.edit_duration[i], "edit_duration"); 
				boxSize += stream.WriteInt64( this.media_time[i], "media_time"); 
			}

			else 
			{
				/*  version==0 */
				boxSize += stream.WriteUInt32( this.edit_duration[i], "edit_duration"); 
				boxSize += stream.WriteInt32( this.media_time[i], "media_time"); 
			}
			boxSize += stream.WriteInt16( this.media_rate_integer[i], "media_rate_integer"); 
			boxSize += stream.WriteInt16( this.media_rate_fraction[i], "media_rate_fraction"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // entry_count

		for (int i=0; i < entry_count; i++)
		{

			if (version==1)
			{
				boxSize += 64; // edit_duration
				boxSize += 64; // media_time
			}

			else 
			{
				/*  version==0 */
				boxSize += 32; // edit_duration
				boxSize += 32; // media_time
			}
			boxSize += 16; // media_rate_integer
			boxSize += 16; // media_rate_fraction
		}
		return boxSize;
	}
}

}
