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
class HVXCconfig()
{
	uimsbf(1) HVXCvarMode;
	uimsbf(2) HVXCrateMode;
	uimsbf(1) extensionFlag;
	if (extensionFlag) {
		// to be defined in MPEG-4 Version 2
	}
}
*/
public partial class HVXCconfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"class HVXCconfig()
{
	uimsbf(1) HVXCvarMode;
	uimsbf(2) HVXCrateMode;
	uimsbf(1) extensionFlag;
	if (extensionFlag) {
		// to be defined in MPEG-4 Version 2
	}
}"; } }
	public virtual string DisplayName { get { return "HVXCconfig"; } }

	protected bool HVXCvarMode; 
	public bool _HVXCvarMode { get { return this.HVXCvarMode; } set { this.HVXCvarMode = value; } }

	protected byte HVXCrateMode; 
	public byte _HVXCrateMode { get { return this.HVXCrateMode; } set { this.HVXCrateMode = value; } }

	protected bool extensionFlag; 
	public bool ExtensionFlag { get { return this.extensionFlag; } set { this.extensionFlag = value; } }

	public HVXCconfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.HVXCvarMode, "HVXCvarMode"); 
		boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.HVXCrateMode, "HVXCrateMode"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.extensionFlag, "extensionFlag"); 

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.HVXCvarMode, "HVXCvarMode"); 
		boxSize += stream.WriteBits(2,  this.HVXCrateMode, "HVXCrateMode"); 
		boxSize += stream.WriteBit( this.extensionFlag, "extensionFlag"); 

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // HVXCvarMode
		boxSize += 2; // HVXCrateMode
		boxSize += 1; // extensionFlag

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("HVXCvarMode", 1, this.HVXCvarMode); 
		boxSize += tracer.Field("HVXCrateMode", 2, this.HVXCrateMode); 
		boxSize += tracer.Field("extensionFlag", 1, this.extensionFlag); 

		if (extensionFlag)
		{
			/*  to be defined in MPEG-4 Version 2 */
		}
		return boxSize;
	}
}

}
