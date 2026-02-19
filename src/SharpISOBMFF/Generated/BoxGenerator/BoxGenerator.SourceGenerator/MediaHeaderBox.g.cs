using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MediaHeaderBox extends FullBox('mdhd', version, 0) {
	if (version==1) {
		unsigned int(64)	creation_time;
		unsigned int(64)	modification_time;
		unsigned int(32)	timescale;
		unsigned int(64)	duration;
	} else { // version==0
		unsigned int(32)	creation_time;
		unsigned int(32)	modification_time;
		unsigned int(32)	timescale;
		unsigned int(32)	duration;
	}
	bit(1)	pad = 0;
	unsigned int(5)[3]	language;	// ISO-639-2/T language code
	unsigned int(16)	pre_defined = 0;
}
*/
public partial class MediaHeaderBox : FullBox
{
	public const string TYPE = "mdhd";
	public override string DisplayName { get { return "MediaHeaderBox"; } }

	protected ulong creation_time; 
	public ulong CreationTime { get { return this.creation_time; } set { this.creation_time = value; } }

	protected ulong modification_time; 
	public ulong ModificationTime { get { return this.modification_time; } set { this.modification_time = value; } }

	protected uint timescale; 
	public uint Timescale { get { return this.timescale; } set { this.timescale = value; } }

	protected ulong duration; 
	public ulong Duration { get { return this.duration; } set { this.duration = value; } }

	protected bool pad = false; 
	public bool Pad { get { return this.pad; } set { this.pad = value; } }

	protected string language;  //  ISO-639-2/T language code
	public string Language { get { return this.language; } set { this.language = value; } }

	protected ushort pre_defined = 0; 
	public ushort PreDefined { get { return this.pre_defined; } set { this.pre_defined = value; } }

	public MediaHeaderBox(byte version = 0): base(IsoStream.FromFourCC("mdhd"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.creation_time, "creation_time"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.modification_time, "modification_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.duration, "duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.creation_time, "creation_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.modification_time, "modification_time"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.duration, "duration"); 
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.pad, "pad"); 
		boxSize += stream.ReadIso639(boxSize, readSize,  out this.language, "language"); // ISO-639-2/T language code
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.pre_defined, "pre_defined"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==1)
		{
			boxSize += stream.WriteUInt64( this.creation_time, "creation_time"); 
			boxSize += stream.WriteUInt64( this.modification_time, "modification_time"); 
			boxSize += stream.WriteUInt32( this.timescale, "timescale"); 
			boxSize += stream.WriteUInt64( this.duration, "duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.WriteUInt32( this.creation_time, "creation_time"); 
			boxSize += stream.WriteUInt32( this.modification_time, "modification_time"); 
			boxSize += stream.WriteUInt32( this.timescale, "timescale"); 
			boxSize += stream.WriteUInt32( this.duration, "duration"); 
		}
		boxSize += stream.WriteBit( this.pad, "pad"); 
		boxSize += stream.WriteIso639( this.language, "language"); // ISO-639-2/T language code
		boxSize += stream.WriteUInt16( this.pre_defined, "pre_defined"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==1)
		{
			boxSize += 64; // creation_time
			boxSize += 64; // modification_time
			boxSize += 32; // timescale
			boxSize += 64; // duration
		}

		else 
		{
			/*  version==0 */
			boxSize += 32; // creation_time
			boxSize += 32; // modification_time
			boxSize += 32; // timescale
			boxSize += 32; // duration
		}
		boxSize += 1; // pad
		boxSize += 15; // language
		boxSize += 16; // pre_defined
		return boxSize;
	}
}

}
