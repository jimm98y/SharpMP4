using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ProgressiveDownloadInfoBox
		extends FullBox('pdin', version = 0, 0) {
	// to end of box
	ProgressiveDownloadInfoItem items[];
 }
 
*/
public partial class ProgressiveDownloadInfoBox : FullBox
{
	public const string TYPE = "pdin";
	public override string DisplayName { get { return "ProgressiveDownloadInfoBox"; } }

	protected ProgressiveDownloadInfoItem[] items; 
	public ProgressiveDownloadInfoItem[] Items { get { return this.items; } set { this.items = value; } }

	public ProgressiveDownloadInfoBox(): base(IsoStream.FromFourCC("pdin"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		/*  to end of box */
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new ProgressiveDownloadInfoItem(),  out this.items, "items"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		/*  to end of box */
		boxSize += stream.WriteClass( this.items, "items"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		/*  to end of box */
		boxSize += IsoStream.CalculateClassSize(items); // items
		return boxSize;
	}
}

}
