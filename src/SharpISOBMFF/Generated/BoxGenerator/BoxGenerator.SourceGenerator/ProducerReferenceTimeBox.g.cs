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
aligned(8) class ProducerReferenceTimeBox
	extends FullBox('prft', version, flags) {
	unsigned int(32) reference_track_ID;
	unsigned int(64) ntp_timestamp;
	if (version==0) {
		unsigned int(32) media_time;
	} else {
		unsigned int(64) media_time;
	}
}
*/
public partial class ProducerReferenceTimeBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ProducerReferenceTimeBox
	extends FullBox('prft', version, flags) {
	unsigned int(32) reference_track_ID;
	unsigned int(64) ntp_timestamp;
	if (version==0) {
		unsigned int(32) media_time;
	} else {
		unsigned int(64) media_time;
	}
}"; } }
	public const string TYPE = "prft";
	public override string DisplayName { get { return "ProducerReferenceTimeBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected uint reference_track_ID; 
	public uint ReferenceTrackID { get { return this.reference_track_ID; } set { this.reference_track_ID = value; } }

	protected ulong ntp_timestamp; 
	public ulong NtpTimestamp { get { return this.ntp_timestamp; } set { this.ntp_timestamp = value; } }

	protected ulong media_time; 
	public ulong MediaTime { get { return this.media_time; } set { this.media_time = value; } }

	public ProducerReferenceTimeBox(byte version = 0, uint flags = 0): base(IsoStream.FromFourCC("prft"), version, flags)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.reference_track_ID, "reference_track_ID"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.ntp_timestamp, "ntp_timestamp"); 

		if (version==0)
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.media_time, "media_time"); 
		}

		else 
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.media_time, "media_time"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.reference_track_ID, "reference_track_ID"); 
		boxSize += stream.WriteUInt64( this.ntp_timestamp, "ntp_timestamp"); 

		if (version==0)
		{
			boxSize += stream.WriteUInt32( this.media_time, "media_time"); 
		}

		else 
		{
			boxSize += stream.WriteUInt64( this.media_time, "media_time"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // reference_track_ID
		boxSize += 64; // ntp_timestamp

		if (version==0)
		{
			boxSize += 32; // media_time
		}

		else 
		{
			boxSize += 64; // media_time
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("reference_track_ID", 32, this.reference_track_ID); 
		boxSize += tracer.Field("ntp_timestamp", 64, this.ntp_timestamp); 

		if (version==0)
		{
			boxSize += tracer.Field("media_time", 32, this.media_time); 
		}

		else 
		{
			boxSize += tracer.Field("media_time", 64, this.media_time); 
		}
		return boxSize;
	}
}

}
