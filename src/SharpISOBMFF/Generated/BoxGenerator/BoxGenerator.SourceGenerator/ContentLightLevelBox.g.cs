using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ContentLightLevelBox extends Box('clli'){
	unsigned int(16) max_content_light_level;
	unsigned int(16) max_pic_average_light_level;
}
*/
public partial class ContentLightLevelBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ContentLightLevelBox extends Box('clli'){
	unsigned int(16) max_content_light_level;
	unsigned int(16) max_pic_average_light_level;
}"; } }
	public const string TYPE = "clli";
	public override string DisplayName { get { return "ContentLightLevelBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("VisualSampleEntry", "ipco"); }


	protected ushort max_content_light_level; 
	public ushort MaxContentLightLevel { get { return this.max_content_light_level; } set { this.max_content_light_level = value; } }

	protected ushort max_pic_average_light_level; 
	public ushort MaxPicAverageLightLevel { get { return this.max_pic_average_light_level; } set { this.max_pic_average_light_level = value; } }

	public ContentLightLevelBox(): base(IsoStream.FromFourCC("clli"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_content_light_level, "max_content_light_level"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.max_pic_average_light_level, "max_pic_average_light_level"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.max_content_light_level, "max_content_light_level"); 
		boxSize += stream.WriteUInt16( this.max_pic_average_light_level, "max_pic_average_light_level"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // max_content_light_level
		boxSize += 16; // max_pic_average_light_level
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("max_content_light_level", 16, this.max_content_light_level); 
		boxSize += tracer.Field("max_pic_average_light_level", 16, this.max_pic_average_light_level); 
		return boxSize;
	}
}

}
