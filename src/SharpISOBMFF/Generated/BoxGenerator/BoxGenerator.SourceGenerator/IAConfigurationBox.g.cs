using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// 6.2.4: unsigned int (8 x configOBUs_size) configOBUs, as bytes; what a later version puts after them is left
class IAConfigurationBox extends Box('iacb') {
 unsigned int(8) configurationVersion = 1;
 leb128() configOBUs_size;
 unsigned int(8) configOBUs[configOBUs_size];
}
*/
public partial class IAConfigurationBox : Box
{
	public const string TYPE = "iacb";
	public override string DisplayName { get { return "IAConfigurationBox"; } }

	protected byte configurationVersion = 1; 
	public byte ConfigurationVersion { get { return this.configurationVersion; } set { this.configurationVersion = value; } }

	protected Leb128 configOBUs_size; 
	public Leb128 ConfigOBUsSize { get { return this.configOBUs_size; } set { this.configOBUs_size = value; } }

	protected byte[] configOBUs; 
	public byte[] ConfigOBUs { get { return this.configOBUs; } set { this.configOBUs = value; } }

	public IAConfigurationBox(): base(IsoStream.FromFourCC("iacb"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.configurationVersion, "configurationVersion"); 
		boxSize += stream.ReadLeb128(boxSize, readSize,  out this.configOBUs_size, "configOBUs_size"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(configOBUs_size),  out this.configOBUs, "configOBUs"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.configurationVersion, "configurationVersion"); 
		boxSize += stream.WriteLeb128( this.configOBUs_size, "configOBUs_size"); 
		boxSize += stream.WriteUInt8Array((uint)(configOBUs_size),  this.configOBUs, "configOBUs"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // configurationVersion
		boxSize += IsoStream.CalculateLeb128Size(configOBUs_size); // configOBUs_size
		boxSize += ((ulong)(configOBUs_size) * 8); // configOBUs
		return boxSize;
	}
}

}
