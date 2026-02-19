using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class VideoMediaHeaderBox
	extends FullBox('vmhd', version = 0, 1) {
	template unsigned int(16)		graphicsmode = 0;	// copy, see below
	template unsigned int(16)[3]	opcolor = {0, 0, 0};
}
*/
public partial class VideoMediaHeaderBox : FullBox
{
	public const string TYPE = "vmhd";
	public override string DisplayName { get { return "VideoMediaHeaderBox"; } }

	protected ushort graphicsmode = 0;  //  copy, see below
	public ushort Graphicsmode { get { return this.graphicsmode; } set { this.graphicsmode = value; } }

	protected ushort[] opcolor = {0, 0, 0}; 
	public ushort[] Opcolor { get { return this.opcolor; } set { this.opcolor = value; } }

	public VideoMediaHeaderBox(): base(IsoStream.FromFourCC("vmhd"), 0, 1)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.graphicsmode, "graphicsmode"); // copy, see below
		boxSize += stream.ReadUInt16Array(boxSize, readSize, 3,  out this.opcolor, "opcolor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.graphicsmode, "graphicsmode"); // copy, see below
		boxSize += stream.WriteUInt16Array(3,  this.opcolor, "opcolor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // graphicsmode
		boxSize += 3 * 16; // opcolor
		return boxSize;
	}
}

}
