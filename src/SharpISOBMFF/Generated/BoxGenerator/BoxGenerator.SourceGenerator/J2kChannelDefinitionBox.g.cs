using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class J2kChannelDefinitionBox() extends Box('cdef') {
	 unsigned int(16) channel_count;
 Channel channels[];
 } 
*/
public partial class J2kChannelDefinitionBox : Box
{
	public const string TYPE = "cdef";
	public override string DisplayName { get { return "J2kChannelDefinitionBox"; } }

	protected ushort channel_count; 
	public ushort ChannelCount { get { return this.channel_count; } set { this.channel_count = value; } }

	protected Channel[] channels; 
	public Channel[] Channels { get { return this.channels; } set { this.channels = value; } }

	public J2kChannelDefinitionBox(): base(IsoStream.FromFourCC("cdef"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.channel_count, "channel_count"); 
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new Channel(),  out this.channels, "channels"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.channel_count, "channel_count"); 
		boxSize += stream.WriteClass( this.channels, "channels"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // channel_count
		boxSize += IsoStream.CalculateClassSize(channels); // channels
		return boxSize;
	}
}

}
