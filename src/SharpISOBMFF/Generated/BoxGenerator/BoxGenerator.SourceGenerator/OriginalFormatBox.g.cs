using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class OriginalFormatBox(codingname) extends Box ('frma') {
	unsigned int(32)	data_format = codingname;
			// format of decrypted, encoded data (in case of protection)
			// or un-transformed sample entry (in case of restriction
			// and complete track information)
}
*/
public partial class OriginalFormatBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class OriginalFormatBox(codingname) extends Box ('frma') {
	unsigned int(32)	data_format = codingname;
			// format of decrypted, encoded data (in case of protection)
			// or un-transformed sample entry (in case of restriction
			// and complete track information)
}"; } }
	public const string TYPE = "frma";
	public override string DisplayName { get { return "OriginalFormatBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("sinf", "rinf", "wave"); }


	protected uint data_format; // = codingname
	public uint DataFormat { get { return this.data_format; } set { this.data_format = value; } }

	public OriginalFormatBox(uint codingname = 0): base(IsoStream.FromFourCC("frma"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.data_format, "data_format"); // format of decrypted, encoded data (in case of protection)
		/*  or un-transformed sample entry (in case of restriction */
		/*  and complete track information) */
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.data_format, "data_format"); // format of decrypted, encoded data (in case of protection)
		/*  or un-transformed sample entry (in case of restriction */
		/*  and complete track information) */
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // data_format
		/*  or un-transformed sample entry (in case of restriction */
		/*  and complete track information) */
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("data_format", 32, this.data_format); // format of decrypted, encoded data (in case of protection)
		/*  or un-transformed sample entry (in case of restriction */
		/*  and complete track information) */
		return boxSize;
	}
}

}
