using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemOriginSourceBox extends FullBox('corg', 0, 0) {
	unsigned int(32) source_of_origin;
	// e.g., 'blin'
}
*/
public partial class CameraSystemOriginSourceBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class CameraSystemOriginSourceBox extends FullBox('corg', 0, 0) {
	unsigned int(32) source_of_origin;
	// e.g., 'blin'
}"; } }
	public const string TYPE = "corg";
	public override string DisplayName { get { return "CameraSystemOriginSourceBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("lnex"); }


	protected uint source_of_origin;  //  e.g., 'blin'
	public uint SourceOfOrigin { get { return this.source_of_origin; } set { this.source_of_origin = value; } }

	public CameraSystemOriginSourceBox(): base(IsoStream.FromFourCC("corg"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.source_of_origin, "source_of_origin"); // e.g., 'blin'
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.source_of_origin, "source_of_origin"); // e.g., 'blin'
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // source_of_origin
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("source_of_origin", 32, this.source_of_origin); // e.g., 'blin'
		return boxSize;
	}
}

}
