using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ApplePtvBox() extends Box('ptv ') {
	 unsigned int(16) displaySize;
 unsigned int(16) reserved1;
 unsigned int(16) reserved2;
 unsigned int(8) slideShow;
 unsigned int(8) playOnOpen;
 } 
*/
public partial class ApplePtvBox : Box
{
	public const string TYPE = "ptv ";
	public override string DisplayName { get { return "ApplePtvBox"; } }

	protected ushort displaySize; 
	public ushort DisplaySize { get { return this.displaySize; } set { this.displaySize = value; } }

	protected ushort reserved1; 
	public ushort Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected ushort reserved2; 
	public ushort Reserved2 { get { return this.reserved2; } set { this.reserved2 = value; } }

	protected byte slideShow; 
	public byte SlideShow { get { return this.slideShow; } set { this.slideShow = value; } }

	protected byte playOnOpen; 
	public byte PlayOnOpen { get { return this.playOnOpen; } set { this.playOnOpen = value; } }

	public ApplePtvBox(): base(IsoStream.FromFourCC("ptv "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.displaySize, "displaySize"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved2, "reserved2"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.slideShow, "slideShow"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.playOnOpen, "playOnOpen"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.displaySize, "displaySize"); 
		boxSize += stream.WriteUInt16( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt16( this.reserved2, "reserved2"); 
		boxSize += stream.WriteUInt8( this.slideShow, "slideShow"); 
		boxSize += stream.WriteUInt8( this.playOnOpen, "playOnOpen"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // displaySize
		boxSize += 16; // reserved1
		boxSize += 16; // reserved2
		boxSize += 8; // slideShow
		boxSize += 8; // playOnOpen
		return boxSize;
	}
}

}
