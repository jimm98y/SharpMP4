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
class HEVCConfigurationBox extends Box('hvcC') {
	HEVCDecoderConfigurationRecord() HEVCConfig;
}
*/
public partial class HEVCConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class HEVCConfigurationBox extends Box('hvcC') {
	HEVCDecoderConfigurationRecord() HEVCConfig;
}"; } }
	public const string TYPE = "hvcC";
	public override string DisplayName { get { return "HEVCConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco", "HEVCSampleEntry(hvc1,hvc2,hev1,hev2,hvc3,hev3)", "HEVCLHVCSampleEntry(hvc1,hvc2,hev1,hev2,hvc3,hev3)", "RestrictedSampleEntry(resv)", "VisualSampleEntry(encv)", "VisualSampleEntry(dvh1,dvhe)"); }


	protected HEVCDecoderConfigurationRecord HEVCConfig; 
	public HEVCDecoderConfigurationRecord _HEVCConfig { get { return this.HEVCConfig; } set { this.HEVCConfig = value; } }

	public HEVCConfigurationBox(): base(IsoStream.FromFourCC("hvcC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new HEVCDecoderConfigurationRecord(),  out this.HEVCConfig, "HEVCConfig"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.HEVCConfig, "HEVCConfig"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(HEVCConfig); // HEVCConfig
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("HEVCConfig", IsoStream.CalculateClassSize(HEVCConfig), this.HEVCConfig); 
		return boxSize;
	}
}

}
