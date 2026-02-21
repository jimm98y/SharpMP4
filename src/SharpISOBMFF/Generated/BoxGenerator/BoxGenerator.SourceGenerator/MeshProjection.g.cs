using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MeshProjection extends ProjectionDataBox('mshp', 0, 0) {
    unsigned int(32) crc;
    unsigned int(32) encoding_four_cc;

    // All bytes below this point are compressed according to
    // the algorithm specified by the encoding_four_cc field.
    // MeshBox() meshes[]; // At least 1 mesh box must be present.
    Box boxes[]; // further boxes as needed
}
*/
public partial class MeshProjection : ProjectionDataBox
{
	public const string TYPE = "mshp";
	public override string DisplayName { get { return "MeshProjection"; } }

	protected uint crc; 
	public uint Crc { get { return this.crc; } set { this.crc = value; } }

	protected uint encoding_four_cc;  //  All bytes below this point are compressed according to
	public uint EncodingFourCc { get { return this.encoding_four_cc; } set { this.encoding_four_cc = value; } }
	public IEnumerable<Box> Boxes { get { return this.children.OfType<Box>(); } }

	public MeshProjection(): base(IsoStream.FromFourCC("mshp"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.crc, "crc"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.encoding_four_cc, "encoding_four_cc"); // All bytes below this point are compressed according to
		/*  the algorithm specified by the encoding_four_cc field. */
		/*  MeshBox() meshes[]; // At least 1 mesh box must be present. */
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.boxes, "boxes"); // further boxes as needed
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.crc, "crc"); 
		boxSize += stream.WriteUInt32( this.encoding_four_cc, "encoding_four_cc"); // All bytes below this point are compressed according to
		/*  the algorithm specified by the encoding_four_cc field. */
		/*  MeshBox() meshes[]; // At least 1 mesh box must be present. */
		// boxSize += stream.WriteBox( this.boxes, "boxes"); // further boxes as needed
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // crc
		boxSize += 32; // encoding_four_cc
		/*  the algorithm specified by the encoding_four_cc field. */
		/*  MeshBox() meshes[]; // At least 1 mesh box must be present. */
		// boxSize += IsoStream.CalculateBoxSize(boxes); // boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
