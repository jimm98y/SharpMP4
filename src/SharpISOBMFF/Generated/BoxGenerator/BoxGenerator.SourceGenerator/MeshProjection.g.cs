// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MeshProjection extends ProjectionDataBox('mshp', 0, 0) {
	unsigned int(32) crc;
	unsigned int(32) encoding_four_cc;

	// All bytes below this point are compressed according to
	// the algorithm specified by the encoding_four_cc field.
	// MeshBox() meshes[]; // At least 1 mesh box must be present.
	Box boxes[]; // further boxes as needed
}"; } }
	public const string TYPE = "mshp";
	public override string DisplayName { get { return "MeshProjection"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("proj"); }


	protected uint crc; 
	public uint Crc { get { return this.crc; } set { this.crc = value; } }

	protected uint encoding_four_cc;  //  All bytes below this point are compressed according to
	public uint EncodingFourCc { get { return this.encoding_four_cc; } set { this.encoding_four_cc = value; } }

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

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("crc", 32, this.crc); 
		boxSize += tracer.Field("encoding_four_cc", 32, this.encoding_four_cc); // All bytes below this point are compressed according to
		/*  the algorithm specified by the encoding_four_cc field. */
		/*  MeshBox() meshes[]; // At least 1 mesh box must be present. */
		// boxSize += tracer.Field("boxes", IsoStream.CalculateBoxSize(boxes), this.boxes); // further boxes as needed
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
