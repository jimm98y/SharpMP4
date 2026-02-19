using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AmbientViewingEnvironmentBox extends Box('amve'){
	unsigned int(32) ambient_illuminance; 
	unsigned int(16) ambient_light_x;
	unsigned int(16) ambient_light_y;
}
*/
public partial class AmbientViewingEnvironmentBox : Box
{
	public const string TYPE = "amve";
	public override string DisplayName { get { return "AmbientViewingEnvironmentBox"; } }

	protected uint ambient_illuminance; 
	public uint AmbientIlluminance { get { return this.ambient_illuminance; } set { this.ambient_illuminance = value; } }

	protected ushort ambient_light_x; 
	public ushort AmbientLightx { get { return this.ambient_light_x; } set { this.ambient_light_x = value; } }

	protected ushort ambient_light_y; 
	public ushort AmbientLighty { get { return this.ambient_light_y; } set { this.ambient_light_y = value; } }

	public AmbientViewingEnvironmentBox(): base(IsoStream.FromFourCC("amve"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.ambient_illuminance, "ambient_illuminance"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ambient_light_x, "ambient_light_x"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ambient_light_y, "ambient_light_y"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.ambient_illuminance, "ambient_illuminance"); 
		boxSize += stream.WriteUInt16( this.ambient_light_x, "ambient_light_x"); 
		boxSize += stream.WriteUInt16( this.ambient_light_y, "ambient_light_y"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // ambient_illuminance
		boxSize += 16; // ambient_light_x
		boxSize += 16; // ambient_light_y
		return boxSize;
	}
}

}
