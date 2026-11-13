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
// ISO/IEC 23001-7:2016/Amd 1:2019, 8.4: the type of an item's auxiliary information, as a sample's in 14496-12
aligned(8) class ItemAuxiliaryInformationBox extends ItemFullProperty('iaux', version=0, flags=0) {
	unsigned int(32) aux_info_type;
	unsigned int(32) aux_info_type_parameter;
}
*/
public partial class ItemAuxiliaryInformationBox : ItemFullProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// ISO/IEC 23001-7:2016/Amd 1:2019, 8.4: the type of an item's auxiliary information, as a sample's in 14496-12
aligned(8) class ItemAuxiliaryInformationBox extends ItemFullProperty('iaux', version=0, flags=0) {
	unsigned int(32) aux_info_type;
	unsigned int(32) aux_info_type_parameter;
}"; } }
	public const string TYPE = "iaux";
	public override string DisplayName { get { return "ItemAuxiliaryInformationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected uint aux_info_type; 
	public uint AuxInfoType { get { return this.aux_info_type; } set { this.aux_info_type = value; } }

	protected uint aux_info_type_parameter; 
	public uint AuxInfoTypeParameter { get { return this.aux_info_type_parameter; } set { this.aux_info_type_parameter = value; } }

	public ItemAuxiliaryInformationBox(): base(IsoStream.FromFourCC("iaux"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_info_type, "aux_info_type"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.aux_info_type_parameter, "aux_info_type_parameter"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.aux_info_type, "aux_info_type"); 
		boxSize += stream.WriteUInt32( this.aux_info_type_parameter, "aux_info_type_parameter"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // aux_info_type
		boxSize += 32; // aux_info_type_parameter
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("aux_info_type", 32, this.aux_info_type); 
		boxSize += tracer.Field("aux_info_type_parameter", 32, this.aux_info_type_parameter); 
		return boxSize;
	}
}

}
