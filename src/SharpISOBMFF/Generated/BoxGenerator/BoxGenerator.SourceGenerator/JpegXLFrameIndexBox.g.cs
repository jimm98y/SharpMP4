using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// The index of an animation's key frames (ISO/IEC 18181-2 9.3): of the frames indexed, the ticks a second is of (TNUM/TDEN), and of each frame its offset in the codestream from the frame before, its duration in ticks and the frames it stands for, varints
aligned(8) class JpegXLFrameIndexBox extends Box('jxli') {
	varint frame_count; // NF
	unsigned int(32) tick_numerator; // TNUM
	unsigned int(32) tick_denominator; // TDEN
	for (i = 0; i < frame_count; i++) {
		varint offset; // OFFi
		varint duration; // Ti
		varint frames; // Fi
	}
}
*/
public partial class JpegXLFrameIndexBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// The index of an animation's key frames (ISO/IEC 18181-2 9.3): of the frames indexed, the ticks a second is of (TNUM/TDEN), and of each frame its offset in the codestream from the frame before, its duration in ticks and the frames it stands for, varints
aligned(8) class JpegXLFrameIndexBox extends Box('jxli') {
	varint frame_count; // NF
	unsigned int(32) tick_numerator; // TNUM
	unsigned int(32) tick_denominator; // TDEN
	for (i = 0; i < frame_count; i++) {
		varint offset; // OFFi
		varint duration; // Ti
		varint frames; // Fi
	}
}"; } }
	public const string TYPE = "jxli";
	public override string DisplayName { get { return "JpegXLFrameIndexBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected Leb128 frame_count;  //  NF
	public Leb128 FrameCount { get { return this.frame_count; } set { this.frame_count = value; } }

	protected uint tick_numerator;  //  TNUM
	public uint TickNumerator { get { return this.tick_numerator; } set { this.tick_numerator = value; } }

	protected uint tick_denominator;  //  TDEN
	public uint TickDenominator { get { return this.tick_denominator; } set { this.tick_denominator = value; } }

	protected Leb128[] offset;  //  OFFi
	public Leb128[] Offset { get { return this.offset; } set { this.offset = value; } }

	protected Leb128[] duration;  //  Ti
	public Leb128[] Duration { get { return this.duration; } set { this.duration = value; } }

	protected Leb128[] frames;  //  Fi
	public Leb128[] Frames { get { return this.frames; } set { this.frames = value; } }

	public JpegXLFrameIndexBox(): base(IsoStream.FromFourCC("jxli"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadLeb128(boxSize, readSize,  out this.frame_count, "frame_count"); // NF
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tick_numerator, "tick_numerator"); // TNUM
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tick_denominator, "tick_denominator"); // TDEN

		this.offset = stream.SafeAllocate<Leb128>(boxSize, readSize, IsoStream.GetInt( frame_count), "offset");
		this.duration = stream.SafeAllocate<Leb128>(boxSize, readSize, IsoStream.GetInt( frame_count), "duration");
		this.frames = stream.SafeAllocate<Leb128>(boxSize, readSize, IsoStream.GetInt( frame_count), "frames");
		for (int i = 0; i < frame_count; i++)
		{
			boxSize += stream.ReadLeb128(boxSize, readSize,  out this.offset[i], "offset"); // OFFi
			boxSize += stream.ReadLeb128(boxSize, readSize,  out this.duration[i], "duration"); // Ti
			boxSize += stream.ReadLeb128(boxSize, readSize,  out this.frames[i], "frames"); // Fi
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteLeb128( this.frame_count, "frame_count"); // NF
		boxSize += stream.WriteUInt32( this.tick_numerator, "tick_numerator"); // TNUM
		boxSize += stream.WriteUInt32( this.tick_denominator, "tick_denominator"); // TDEN

		for (int i = 0; i < frame_count; i++)
		{
			boxSize += stream.WriteLeb128( this.offset[i], "offset"); // OFFi
			boxSize += stream.WriteLeb128( this.duration[i], "duration"); // Ti
			boxSize += stream.WriteLeb128( this.frames[i], "frames"); // Fi
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateLeb128Size(frame_count); // frame_count
		boxSize += 32; // tick_numerator
		boxSize += 32; // tick_denominator

		for (int i = 0; i < frame_count; i++)
		{
			boxSize += IsoStream.CalculateLeb128Size(offset[i]); // offset
			boxSize += IsoStream.CalculateLeb128Size(duration[i]); // duration
			boxSize += IsoStream.CalculateLeb128Size(frames[i]); // frames
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("frame_count", IsoStream.CalculateLeb128Size(frame_count), this.frame_count); // NF
		boxSize += tracer.Field("tick_numerator", 32, this.tick_numerator); // TNUM
		boxSize += tracer.Field("tick_denominator", 32, this.tick_denominator); // TDEN

		for (int i = 0; i < frame_count; i++)
		{
			boxSize += tracer.Field($"offset[{i}]", IsoStream.CalculateLeb128Size(offset[i]), this.offset[i]); // OFFi
			boxSize += tracer.Field($"duration[{i}]", IsoStream.CalculateLeb128Size(duration[i]), this.duration[i]); // Ti
			boxSize += tracer.Field($"frames[{i}]", IsoStream.CalculateLeb128Size(frames[i]), this.frames[i]); // Fi
		}
		return boxSize;
	}
}

}
