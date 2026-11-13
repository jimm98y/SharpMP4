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
aligned(8) class StyleRecord() {
	unsigned int(16) startChar;
	unsigned int(16) endChar;
	unsigned int(16) fontId;
	unsigned int(8) faceStyleFlags;
	unsigned int(8) fontSize;
	unsigned int(8) textColor[4]; }
*/
public partial class StyleRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class StyleRecord() {
	unsigned int(16) startChar;
	unsigned int(16) endChar;
	unsigned int(16) fontId;
	unsigned int(8) faceStyleFlags;
	unsigned int(8) fontSize;
	unsigned int(8) textColor[4]; }"; } }
	public virtual string DisplayName { get { return "StyleRecord"; } }

	protected ushort startChar; 
	public ushort StartChar { get { return this.startChar; } set { this.startChar = value; } }

	protected ushort endChar; 
	public ushort EndChar { get { return this.endChar; } set { this.endChar = value; } }

	protected ushort fontId; 
	public ushort FontId { get { return this.fontId; } set { this.fontId = value; } }

	protected byte faceStyleFlags; 
	public byte FaceStyleFlags { get { return this.faceStyleFlags; } set { this.faceStyleFlags = value; } }

	protected byte fontSize; 
	public byte FontSize { get { return this.fontSize; } set { this.fontSize = value; } }

	protected byte[] textColor = new byte[4]; 
	public byte[] TextColor { get { return this.textColor; } set { this.textColor = value; } }

	public StyleRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.startChar, "startChar"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.endChar, "endChar"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.fontId, "fontId"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.faceStyleFlags, "faceStyleFlags"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.fontSize, "fontSize"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 4,  out this.textColor, "textColor"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.startChar, "startChar"); 
		boxSize += stream.WriteUInt16( this.endChar, "endChar"); 
		boxSize += stream.WriteUInt16( this.fontId, "fontId"); 
		boxSize += stream.WriteUInt8( this.faceStyleFlags, "faceStyleFlags"); 
		boxSize += stream.WriteUInt8( this.fontSize, "fontSize"); 
		boxSize += stream.WriteUInt8Array(4,  this.textColor, "textColor"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // startChar
		boxSize += 16; // endChar
		boxSize += 16; // fontId
		boxSize += 8; // faceStyleFlags
		boxSize += 8; // fontSize
		boxSize += 4 * 8; // textColor
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("startChar", 16, this.startChar); 
		boxSize += tracer.Field("endChar", 16, this.endChar); 
		boxSize += tracer.Field("fontId", 16, this.fontId); 
		boxSize += tracer.Field("faceStyleFlags", 8, this.faceStyleFlags); 
		boxSize += tracer.Field("fontSize", 8, this.fontSize); 
		boxSize += tracer.Field("textColor", 4 * 8, this.textColor); 
		return boxSize;
	}
}

}
