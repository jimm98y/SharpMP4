using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AV1CodecConfigurationBox
	extends Box('av1C')
{
	AV1CodecConfigurationRecord av1Config;
}
*/
public partial class AV1CodecConfigurationBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class AV1CodecConfigurationBox
	extends Box('av1C')
{
	AV1CodecConfigurationRecord av1Config;
}"; } }
	public const string TYPE = "av1C";
	public override string DisplayName { get { return "AV1CodecConfigurationBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry(av01,encv,resv)", "ipco"); }


	protected AV1CodecConfigurationRecord av1Config; 
	public AV1CodecConfigurationRecord Av1Config { get { return this.av1Config; } set { this.av1Config = value; } }

	public AV1CodecConfigurationBox(): base(IsoStream.FromFourCC("av1C"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new AV1CodecConfigurationRecord(),  out this.av1Config, "av1Config"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.av1Config, "av1Config"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(av1Config); // av1Config
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("av1Config", IsoStream.CalculateClassSize(av1Config), this.av1Config); 
		return boxSize;
	}
}

}
