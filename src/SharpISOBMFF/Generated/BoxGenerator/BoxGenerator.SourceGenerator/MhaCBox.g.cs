using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// MPEG-H 3D Audio's configuration, as GPAC's mhac_box_read and FFmpeg's mov_read_mhac read it
aligned(8) class MhaCBox() extends Box('mhaC') {
	unsigned int(8) configurationVersion = 1;
	unsigned int(8) mpegh3daProfileLevelIndication;
	unsigned int(8) referenceChannelLayout;
	unsigned int(16) mpegh3daConfigLength;
	bit(8*mpegh3daConfigLength) mpegh3daConfig;
}
*/
public partial class MhaCBox : Box
{
	public const string TYPE = "mhaC";
	public override string DisplayName { get { return "MhaCBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(mha1,mha2,mhm1,mhm2)", "wave"); }


	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected byte mpegh3daProfileLevelIndication; 
	public byte Mpegh3daProfileLevelIndication { get { return this.mpegh3daProfileLevelIndication; } set { this.mpegh3daProfileLevelIndication = value; } }

	protected byte referenceChannelLayout; 
	public byte ReferenceChannelLayout { get { return this.referenceChannelLayout; } set { this.referenceChannelLayout = value; } }

	protected ushort mpegh3daConfigLength; 
	public ushort Mpegh3daConfigLength { get { return this.mpegh3daConfigLength; } set { this.mpegh3daConfigLength = value; } }

	protected byte[] mpegh3daConfig; 
	public byte[] Mpegh3daConfig { get { return this.mpegh3daConfig; } set { this.mpegh3daConfig = value; } }

	public MhaCBox(): base(IsoStream.FromFourCC("mhaC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.mpegh3daProfileLevelIndication, "mpegh3daProfileLevelIndication"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.referenceChannelLayout, "referenceChannelLayout"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.mpegh3daConfigLength, "mpegh3daConfigLength"); 
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*mpegh3daConfigLength ),  out this.mpegh3daConfig, "mpegh3daConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.configurationVersion, "configurationVersion"); 
		boxSize += stream.WriteUInt8( this.mpegh3daProfileLevelIndication, "mpegh3daProfileLevelIndication"); 
		boxSize += stream.WriteUInt8( this.referenceChannelLayout, "referenceChannelLayout"); 
		boxSize += stream.WriteUInt16( this.mpegh3daConfigLength, "mpegh3daConfigLength"); 
		boxSize += stream.WriteBits((uint)(8*mpegh3daConfigLength ),  this.mpegh3daConfig, "mpegh3daConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // configurationVersion
		boxSize += 8; // mpegh3daProfileLevelIndication
		boxSize += 8; // referenceChannelLayout
		boxSize += 16; // mpegh3daConfigLength
		boxSize += (ulong)(8*mpegh3daConfigLength ); // mpegh3daConfig
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("configurationVersion", 8, this.configurationVersion); 
		boxSize += builder.Field("mpegh3daProfileLevelIndication", 8, this.mpegh3daProfileLevelIndication); 
		boxSize += builder.Field("referenceChannelLayout", 8, this.referenceChannelLayout); 
		boxSize += builder.Field("mpegh3daConfigLength", 16, this.mpegh3daConfigLength); 
		boxSize += builder.Field("mpegh3daConfig", (ulong)(8*mpegh3daConfigLength ), this.mpegh3daConfig); 
		return boxSize;
	}
}

}
