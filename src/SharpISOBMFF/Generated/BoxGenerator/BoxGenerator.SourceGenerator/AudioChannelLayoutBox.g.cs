using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AudioChannelLayoutBox() extends FullBox('chan') {
 unsigned int(32) channelLayoutTag;
 unsigned int(32) channelBitmap;
 unsigned int(32) numberChannelDescriptions;
 AudioChannelDescription channelDescriptions[numberChannelDescriptions];
 }
 
*/
public partial class AudioChannelLayoutBox : FullBox
{
	public const string TYPE = "chan";
	public override string DisplayName { get { return "AudioChannelLayoutBox"; } }

	protected uint channelLayoutTag; 
	public uint ChannelLayoutTag { get { return this.channelLayoutTag; } set { this.channelLayoutTag = value; } }

	protected uint channelBitmap; 
	public uint ChannelBitmap { get { return this.channelBitmap; } set { this.channelBitmap = value; } }

	protected uint numberChannelDescriptions; 
	public uint NumberChannelDescriptions { get { return this.numberChannelDescriptions; } set { this.numberChannelDescriptions = value; } }

	protected AudioChannelDescription[] channelDescriptions; 
	public AudioChannelDescription[] ChannelDescriptions { get { return this.channelDescriptions; } set { this.channelDescriptions = value; } }

	public AudioChannelLayoutBox(): base(IsoStream.FromFourCC("chan"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.channelLayoutTag, "channelLayoutTag"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.channelBitmap, "channelBitmap"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.numberChannelDescriptions, "numberChannelDescriptions"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(numberChannelDescriptions), () => new AudioChannelDescription(),  out this.channelDescriptions, "channelDescriptions"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.channelLayoutTag, "channelLayoutTag"); 
		boxSize += stream.WriteUInt32( this.channelBitmap, "channelBitmap"); 
		boxSize += stream.WriteUInt32( this.numberChannelDescriptions, "numberChannelDescriptions"); 
		boxSize += stream.WriteClass( this.channelDescriptions, "channelDescriptions"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // channelLayoutTag
		boxSize += 32; // channelBitmap
		boxSize += 32; // numberChannelDescriptions
		boxSize += IsoStream.CalculateClassSize(channelDescriptions); // channelDescriptions
		return boxSize;
	}
}

}
