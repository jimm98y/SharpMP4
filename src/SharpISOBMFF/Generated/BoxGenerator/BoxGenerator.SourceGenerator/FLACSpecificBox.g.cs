using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// 3.3.2 FLAC Specific Box: the metadata blocks of the stream, to the end of the box
aligned(8) class FLACSpecificBox extends FullBox('dfLa', version = 0, 0){
 FLACMetadataBlock blocks[];
 }
 
*/
public partial class FLACSpecificBox : FullBox
{
	public const string TYPE = "dfLa";
	public override string DisplayName { get { return "FLACSpecificBox"; } }

	protected FLACMetadataBlock[] blocks; 
	public FLACMetadataBlock[] Blocks { get { return this.blocks; } set { this.blocks = value; } }

	public FLACSpecificBox(): base(IsoStream.FromFourCC("dfLa"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new FLACMetadataBlock(),  out this.blocks, "blocks"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.blocks, "blocks"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(blocks); // blocks
		return boxSize;
	}
}

}
