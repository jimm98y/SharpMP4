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
aligned(8) class MetaBoxRelationBox() extends FullBox('mere') {
	unsigned int(32) firstMetaBoxHandlerType;
	unsigned int(32) secondMetaBoxHandlerType;
	bit(8) metaboxRelation;
}
*/
public partial class MetaBoxRelationBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MetaBoxRelationBox() extends FullBox('mere') {
	unsigned int(32) firstMetaBoxHandlerType;
	unsigned int(32) secondMetaBoxHandlerType;
	bit(8) metaboxRelation;
}"; } }
	public const string TYPE = "mere";
	public override string DisplayName { get { return "MetaBoxRelationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("meco"); }


	protected uint firstMetaBoxHandlerType; 
	public uint FirstMetaBoxHandlerType { get { return this.firstMetaBoxHandlerType; } set { this.firstMetaBoxHandlerType = value; } }

	protected uint secondMetaBoxHandlerType; 
	public uint SecondMetaBoxHandlerType { get { return this.secondMetaBoxHandlerType; } set { this.secondMetaBoxHandlerType = value; } }

	protected byte metaboxRelation; 
	public byte MetaboxRelation { get { return this.metaboxRelation; } set { this.metaboxRelation = value; } }

	public MetaBoxRelationBox(): base(IsoStream.FromFourCC("mere"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.firstMetaBoxHandlerType, "firstMetaBoxHandlerType"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.secondMetaBoxHandlerType, "secondMetaBoxHandlerType"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.metaboxRelation, "metaboxRelation"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.firstMetaBoxHandlerType, "firstMetaBoxHandlerType"); 
		boxSize += stream.WriteUInt32( this.secondMetaBoxHandlerType, "secondMetaBoxHandlerType"); 
		boxSize += stream.WriteUInt8( this.metaboxRelation, "metaboxRelation"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // firstMetaBoxHandlerType
		boxSize += 32; // secondMetaBoxHandlerType
		boxSize += 8; // metaboxRelation
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("firstMetaBoxHandlerType", 32, this.firstMetaBoxHandlerType); 
		boxSize += tracer.Field("secondMetaBoxHandlerType", 32, this.secondMetaBoxHandlerType); 
		boxSize += tracer.Field("metaboxRelation", 8, this.metaboxRelation); 
		return boxSize;
	}
}

}
