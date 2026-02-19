using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class LevelAssignmentBox extends FullBox('leva', 0, 0)
{
	unsigned int(8)	level_count;
	for (j=1; j <= level_count; j++) {
		unsigned int(32)	track_ID;
		unsigned int(1)	padding_flag;
		unsigned int(7)	assignment_type;
		if (assignment_type == 0) {
			unsigned int(32)	grouping_type;
		}
		else if (assignment_type == 1) {
			unsigned int(32)	grouping_type;
			unsigned int(32)	grouping_type_parameter;
		}
		else if (assignment_type == 2) {}
			// no further syntax elements needed
		else if (assignment_type == 3) {}
			// no further syntax elements needed
		else if (assignment_type == 4) {
			unsigned int(32) sub_track_ID;
		}
		// other assignment_type values are reserved
	}
}
*/
public partial class LevelAssignmentBox : FullBox
{
	public const string TYPE = "leva";
	public override string DisplayName { get { return "LevelAssignmentBox"; } }

	protected byte level_count; 
	public byte LevelCount { get { return this.level_count; } set { this.level_count = value; } }

	protected uint[] track_ID; 
	public uint[] TrackID { get { return this.track_ID; } set { this.track_ID = value; } }

	protected bool[] padding_flag; 
	public bool[] PaddingFlag { get { return this.padding_flag; } set { this.padding_flag = value; } }

	protected byte[] assignment_type; 
	public byte[] AssignmentType { get { return this.assignment_type; } set { this.assignment_type = value; } }

	protected uint[] grouping_type; 
	public uint[] GroupingType { get { return this.grouping_type; } set { this.grouping_type = value; } }

	protected uint[] grouping_type_parameter; 
	public uint[] GroupingTypeParameter { get { return this.grouping_type_parameter; } set { this.grouping_type_parameter = value; } }

	protected uint[] sub_track_ID; 
	public uint[] SubTrackID { get { return this.sub_track_ID; } set { this.sub_track_ID = value; } }

	public LevelAssignmentBox(): base(IsoStream.FromFourCC("leva"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.level_count, "level_count"); 

		this.track_ID = new uint[IsoStream.GetInt( level_count)];
		this.padding_flag = new bool[IsoStream.GetInt( level_count)];
		this.assignment_type = new byte[IsoStream.GetInt( level_count)];
		this.grouping_type = new uint[IsoStream.GetInt( level_count)];
		this.grouping_type_parameter = new uint[IsoStream.GetInt( level_count)];
		this.sub_track_ID = new uint[IsoStream.GetInt( level_count)];
		for (int j=0; j < level_count; j++)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.track_ID[j], "track_ID"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.padding_flag[j], "padding_flag"); 
			boxSize += stream.ReadBits(boxSize, readSize, 7,  out this.assignment_type[j], "assignment_type"); 

			if (assignment_type[j] == 0)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type[j], "grouping_type"); 
			}

			else if (assignment_type[j] == 1)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type[j], "grouping_type"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.grouping_type_parameter[j], "grouping_type_parameter"); 
			}

			else if (assignment_type[j] == 2)
			{
			}
			/*  no further syntax elements needed */

			else if (assignment_type[j] == 3)
			{
			}
			/*  no further syntax elements needed */

			else if (assignment_type[j] == 4)
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.sub_track_ID[j], "sub_track_ID"); 
			}
			/*  other assignment_type values are reserved */
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.level_count, "level_count"); 

		for (int j=0; j < level_count; j++)
		{
			boxSize += stream.WriteUInt32( this.track_ID[j], "track_ID"); 
			boxSize += stream.WriteBit( this.padding_flag[j], "padding_flag"); 
			boxSize += stream.WriteBits(7,  this.assignment_type[j], "assignment_type"); 

			if (assignment_type[j] == 0)
			{
				boxSize += stream.WriteUInt32( this.grouping_type[j], "grouping_type"); 
			}

			else if (assignment_type[j] == 1)
			{
				boxSize += stream.WriteUInt32( this.grouping_type[j], "grouping_type"); 
				boxSize += stream.WriteUInt32( this.grouping_type_parameter[j], "grouping_type_parameter"); 
			}

			else if (assignment_type[j] == 2)
			{
			}
			/*  no further syntax elements needed */

			else if (assignment_type[j] == 3)
			{
			}
			/*  no further syntax elements needed */

			else if (assignment_type[j] == 4)
			{
				boxSize += stream.WriteUInt32( this.sub_track_ID[j], "sub_track_ID"); 
			}
			/*  other assignment_type values are reserved */
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // level_count

		for (int j=0; j < level_count; j++)
		{
			boxSize += 32; // track_ID
			boxSize += 1; // padding_flag
			boxSize += 7; // assignment_type

			if (assignment_type[j] == 0)
			{
				boxSize += 32; // grouping_type
			}

			else if (assignment_type[j] == 1)
			{
				boxSize += 32; // grouping_type
				boxSize += 32; // grouping_type_parameter
			}

			else if (assignment_type[j] == 2)
			{
			}
			/*  no further syntax elements needed */

			else if (assignment_type[j] == 3)
			{
			}
			/*  no further syntax elements needed */

			else if (assignment_type[j] == 4)
			{
				boxSize += 32; // sub_track_ID
			}
			/*  other assignment_type values are reserved */
		}
		return boxSize;
	}
}

}
