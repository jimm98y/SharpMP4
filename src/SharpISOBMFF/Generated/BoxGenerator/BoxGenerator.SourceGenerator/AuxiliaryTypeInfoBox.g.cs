using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AuxiliaryTypeInfoBox extends FullBox ('auxi', 0, 0)
{
	string aux_track_type;
}
*/
public partial class AuxiliaryTypeInfoBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class AuxiliaryTypeInfoBox extends FullBox ('auxi', 0, 0)
{
	string aux_track_type;
}"; } }
	public const string TYPE = "auxi";
	public override string DisplayName { get { return "AuxiliaryTypeInfoBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("SampleEntry"); }


	protected BinaryUTF8String aux_track_type; 
	public BinaryUTF8String AuxTrackType { get { return this.aux_track_type; } set { this.aux_track_type = value; } }

	public AuxiliaryTypeInfoBox(): base(IsoStream.FromFourCC("auxi"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.aux_track_type, "aux_track_type"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.aux_track_type, "aux_track_type"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(aux_track_type); // aux_track_type
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("aux_track_type", IsoStream.CalculateStringSize(aux_track_type), this.aux_track_type); 
		return boxSize;
	}
}

}
