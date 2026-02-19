using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ChannelLayout extends FullBox('chnl', version, flags=0) {	if (version==0) {
		unsigned int(8) stream_structure;
		if (stream_structure & channelStructured) {
			unsigned int(8) definedLayout;
 			if (definedLayout==0) {
				for (i = 1 ; i <= layout_channel_count ; i++) {
					//  layout_channel_count comes from the sample entry
					unsigned int(8) speaker_position;
					if (speaker_position == 126) {	// explicit position
						signed int (16) azimuth;
						signed int (8)  elevation;
					}
				}
			} else {
				unsigned int(64)	omittedChannelsMap; 
						// a ‘1’ bit indicates ‘not in this track’
			}
		}
		if (stream_structure & objectStructured) {
			unsigned int(8) object_count;
		}
	} else {
		unsigned int(4) stream_structure;
		unsigned int(4) format_ordering;
		unsigned int(8) baseChannelCount;
		if (stream_structure & channelStructured) {
			unsigned int(8) definedLayout;
			if (definedLayout==0) {
				unsigned int(8) layout_channel_count;
				for (i = 1 ; i <= layout_channel_count ; i++) {
					unsigned int(8) speaker_position;
					if (speaker_position == 126) {	// explicit position
						signed int (16) azimuth;
						signed int (8)  elevation;
					}
				}
			} else {
				int(4) reserved = 0;
				unsigned int(3) channel_order_definition;
				unsigned int(1) omitted_channels_present;
				if (omitted_channels_present == 1) {
					unsigned int(64)	omittedChannelsMap; 
							// a ‘1’ bit indicates ‘not in this track’
				}
			}
		}
		if (stream_structure & objectStructured) {
							// object_count is derived from baseChannelCount
		}
	}
}

*/
public partial class ChannelLayout : FullBox
{
	public const string TYPE = "chnl";
	public override string DisplayName { get { return "ChannelLayout"; } }

	protected byte stream_structure; 
	public byte StreamStructure { get { return this.stream_structure; } set { this.stream_structure = value; } }

	protected byte definedLayout; 
	public byte DefinedLayout { get { return this.definedLayout; } set { this.definedLayout = value; } }

	protected byte[] speaker_position; 
	public byte[] SpeakerPosition { get { return this.speaker_position; } set { this.speaker_position = value; } }

	protected short[] azimuth; 
	public short[] Azimuth { get { return this.azimuth; } set { this.azimuth = value; } }

	protected sbyte[] elevation; 
	public sbyte[] Elevation { get { return this.elevation; } set { this.elevation = value; } }

	protected ulong omittedChannelsMap;  //  a ‘1’ bit indicates ‘not in this track’
	public ulong OmittedChannelsMap { get { return this.omittedChannelsMap; } set { this.omittedChannelsMap = value; } }

	protected byte object_count; 
	public byte ObjectCount { get { return this.object_count; } set { this.object_count = value; } }

	protected byte format_ordering; 
	public byte FormatOrdering { get { return this.format_ordering; } set { this.format_ordering = value; } }

	protected byte baseChannelCount; 
	public byte BaseChannelCount { get { return this.baseChannelCount; } set { this.baseChannelCount = value; } }

	protected byte layout_channel_count; 
	public byte LayoutChannelCount { get { return this.layout_channel_count; } set { this.layout_channel_count = value; } }

	protected sbyte reserved = 0; 
	public sbyte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte channel_order_definition; 
	public byte ChannelOrderDefinition { get { return this.channel_order_definition; } set { this.channel_order_definition = value; } }

	protected bool omitted_channels_present; 
	public bool OmittedChannelsPresent { get { return this.omitted_channels_present; } set { this.omitted_channels_present = value; } }

	public ChannelLayout(byte version = 0): base(IsoStream.FromFourCC("chnl"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==0)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.stream_structure, "stream_structure"); 

			if ((stream_structure  &  1) ==  1)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.definedLayout, "definedLayout"); 

				if (definedLayout==0)
				{

					this.speaker_position = new byte[IsoStream.GetInt( layout_channel_count )];
					this.azimuth = new short[IsoStream.GetInt( layout_channel_count )];
					this.elevation = new sbyte[IsoStream.GetInt( layout_channel_count )];
					for (int i = 0 ; i < layout_channel_count ; i++)
					{
						/*   layout_channel_count comes from the sample entry */
						boxSize += stream.ReadUInt8(boxSize, readSize,  out this.speaker_position[i], "speaker_position"); 

						if (speaker_position[i] == 126)
						{
							/*  explicit position */
							boxSize += stream.ReadInt16(boxSize, readSize,  out this.azimuth[i], "azimuth"); 
							boxSize += stream.ReadInt8(boxSize, readSize,  out this.elevation[i], "elevation"); 
						}
					}
				}

				else 
				{
					boxSize += stream.ReadUInt64(boxSize, readSize,  out this.omittedChannelsMap, "omittedChannelsMap"); // a ‘1’ bit indicates ‘not in this track’
				}
			}

			if ((stream_structure  &  2) ==  2)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.object_count, "object_count"); 
			}
		}

		else 
		{
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.stream_structure, "stream_structure"); 
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.format_ordering, "format_ordering"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.baseChannelCount, "baseChannelCount"); 

			if ((stream_structure  &  1) ==  1)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.definedLayout, "definedLayout"); 

				if (definedLayout==0)
				{
					boxSize += stream.ReadUInt8(boxSize, readSize,  out this.layout_channel_count, "layout_channel_count"); 

					for (int i = 0 ; i < layout_channel_count ; i++)
					{
						boxSize += stream.ReadUInt8(boxSize, readSize,  out this.speaker_position[i], "speaker_position"); 

						if (speaker_position[i] == 126)
						{
							/*  explicit position */
							boxSize += stream.ReadInt16(boxSize, readSize,  out this.azimuth[i], "azimuth"); 
							boxSize += stream.ReadInt8(boxSize, readSize,  out this.elevation[i], "elevation"); 
						}
					}
				}

				else 
				{
					boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
					boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.channel_order_definition, "channel_order_definition"); 
					boxSize += stream.ReadBit(boxSize, readSize,  out this.omitted_channels_present, "omitted_channels_present"); 

					if (omitted_channels_present == true)
					{
						boxSize += stream.ReadUInt64(boxSize, readSize,  out this.omittedChannelsMap, "omittedChannelsMap"); // a ‘1’ bit indicates ‘not in this track’
					}
				}
			}

			if ((stream_structure  &  2) ==  2)
			{
				/*  object_count is derived from baseChannelCount */
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==0)
		{
			boxSize += stream.WriteUInt8( this.stream_structure, "stream_structure"); 

			if ((stream_structure  &  1) ==  1)
			{
				boxSize += stream.WriteUInt8( this.definedLayout, "definedLayout"); 

				if (definedLayout==0)
				{

					for (int i = 0 ; i < layout_channel_count ; i++)
					{
						/*   layout_channel_count comes from the sample entry */
						boxSize += stream.WriteUInt8( this.speaker_position[i], "speaker_position"); 

						if (speaker_position[i] == 126)
						{
							/*  explicit position */
							boxSize += stream.WriteInt16( this.azimuth[i], "azimuth"); 
							boxSize += stream.WriteInt8( this.elevation[i], "elevation"); 
						}
					}
				}

				else 
				{
					boxSize += stream.WriteUInt64( this.omittedChannelsMap, "omittedChannelsMap"); // a ‘1’ bit indicates ‘not in this track’
				}
			}

			if ((stream_structure  &  2) ==  2)
			{
				boxSize += stream.WriteUInt8( this.object_count, "object_count"); 
			}
		}

		else 
		{
			boxSize += stream.WriteBits(4,  this.stream_structure, "stream_structure"); 
			boxSize += stream.WriteBits(4,  this.format_ordering, "format_ordering"); 
			boxSize += stream.WriteUInt8( this.baseChannelCount, "baseChannelCount"); 

			if ((stream_structure  &  1) ==  1)
			{
				boxSize += stream.WriteUInt8( this.definedLayout, "definedLayout"); 

				if (definedLayout==0)
				{
					boxSize += stream.WriteUInt8( this.layout_channel_count, "layout_channel_count"); 

					for (int i = 0 ; i < layout_channel_count ; i++)
					{
						boxSize += stream.WriteUInt8( this.speaker_position[i], "speaker_position"); 

						if (speaker_position[i] == 126)
						{
							/*  explicit position */
							boxSize += stream.WriteInt16( this.azimuth[i], "azimuth"); 
							boxSize += stream.WriteInt8( this.elevation[i], "elevation"); 
						}
					}
				}

				else 
				{
					boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
					boxSize += stream.WriteBits(3,  this.channel_order_definition, "channel_order_definition"); 
					boxSize += stream.WriteBit( this.omitted_channels_present, "omitted_channels_present"); 

					if (omitted_channels_present == true)
					{
						boxSize += stream.WriteUInt64( this.omittedChannelsMap, "omittedChannelsMap"); // a ‘1’ bit indicates ‘not in this track’
					}
				}
			}

			if ((stream_structure  &  2) ==  2)
			{
				/*  object_count is derived from baseChannelCount */
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==0)
		{
			boxSize += 8; // stream_structure

			if ((stream_structure  &  1) ==  1)
			{
				boxSize += 8; // definedLayout

				if (definedLayout==0)
				{

					for (int i = 0 ; i < layout_channel_count ; i++)
					{
						/*   layout_channel_count comes from the sample entry */
						boxSize += 8; // speaker_position

						if (speaker_position[i] == 126)
						{
							/*  explicit position */
							boxSize += 16; // azimuth
							boxSize += 8; // elevation
						}
					}
				}

				else 
				{
					boxSize += 64; // omittedChannelsMap
				}
			}

			if ((stream_structure  &  2) ==  2)
			{
				boxSize += 8; // object_count
			}
		}

		else 
		{
			boxSize += 4; // stream_structure
			boxSize += 4; // format_ordering
			boxSize += 8; // baseChannelCount

			if ((stream_structure  &  1) ==  1)
			{
				boxSize += 8; // definedLayout

				if (definedLayout==0)
				{
					boxSize += 8; // layout_channel_count

					for (int i = 0 ; i < layout_channel_count ; i++)
					{
						boxSize += 8; // speaker_position

						if (speaker_position[i] == 126)
						{
							/*  explicit position */
							boxSize += 16; // azimuth
							boxSize += 8; // elevation
						}
					}
				}

				else 
				{
					boxSize += 4; // reserved
					boxSize += 3; // channel_order_definition
					boxSize += 1; // omitted_channels_present

					if (omitted_channels_present == true)
					{
						boxSize += 64; // omittedChannelsMap
					}
				}
			}

			if ((stream_structure  &  2) ==  2)
			{
				/*  object_count is derived from baseChannelCount */
			}
		}
		return boxSize;
	}
}

}
