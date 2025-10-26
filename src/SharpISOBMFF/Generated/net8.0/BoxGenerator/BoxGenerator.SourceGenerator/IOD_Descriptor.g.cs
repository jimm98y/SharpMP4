using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class IOD_Descriptor extends BaseDescriptor : bit(8) tag=MP4_IOD_Tag {
unsigned int(16) odid; unsigned int(8) odProfileLevel;
 unsigned int(8) sceneProfileLevel;
	unsigned int(8) audioProfileId;
	unsigned int(8) videoProfileId;
	unsigned int(8) graphicsProfileLevel;
	IodsSample samples[];
 }
 
*/
public partial class IOD_Descriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.MP4_IOD_Tag;
	public override string DisplayName { get { return "IOD_Descriptor"; } }

	protected ushort odid; 
	public ushort Odid { get { return this.odid; } set { this.odid = value; } }

	protected byte odProfileLevel; 
	public byte OdProfileLevel { get { return this.odProfileLevel; } set { this.odProfileLevel = value; } }

	protected byte sceneProfileLevel; 
	public byte SceneProfileLevel { get { return this.sceneProfileLevel; } set { this.sceneProfileLevel = value; } }

	protected byte audioProfileId; 
	public byte AudioProfileId { get { return this.audioProfileId; } set { this.audioProfileId = value; } }

	protected byte videoProfileId; 
	public byte VideoProfileId { get { return this.videoProfileId; } set { this.videoProfileId = value; } }

	protected byte graphicsProfileLevel; 
	public byte GraphicsProfileLevel { get { return this.graphicsProfileLevel; } set { this.graphicsProfileLevel = value; } }

	protected IodsSample[] samples; 
	public IodsSample[] Samples { get { return this.samples; } set { this.samples = value; } }

	public IOD_Descriptor(): base(DescriptorTags.MP4_IOD_Tag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.odid, "odid"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.odProfileLevel, "odProfileLevel"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sceneProfileLevel, "sceneProfileLevel"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.audioProfileId, "audioProfileId"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.videoProfileId, "videoProfileId"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.graphicsProfileLevel, "graphicsProfileLevel"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new IodsSample(),  out this.samples, "samples"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.odid, "odid"); 
		boxSize += stream.WriteUInt8( this.odProfileLevel, "odProfileLevel"); 
		boxSize += stream.WriteUInt8( this.sceneProfileLevel, "sceneProfileLevel"); 
		boxSize += stream.WriteUInt8( this.audioProfileId, "audioProfileId"); 
		boxSize += stream.WriteUInt8( this.videoProfileId, "videoProfileId"); 
		boxSize += stream.WriteUInt8( this.graphicsProfileLevel, "graphicsProfileLevel"); 
		boxSize += stream.WriteClass( this.samples, "samples"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // odid
		boxSize += 8; // odProfileLevel
		boxSize += 8; // sceneProfileLevel
		boxSize += 8; // audioProfileId
		boxSize += 8; // videoProfileId
		boxSize += 8; // graphicsProfileLevel
		boxSize += IsoStream.CalculateClassSize(samples); // samples
		return boxSize;
	}
}

}
