using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AV2CodecConfigurationBox extends Box('av2C')
{
  unsigned int(8) reserved1 = 0;
  unsigned int(8) config_obus_count_minus1;
  for (i = 0; i < config_obus_count_minus1 + 1; ++i) {
    unsigned int(8) config_obu[];
  }
}

*/
public partial class AV2CodecConfigurationBox : Box
{
	public const string TYPE = "av2C";
	public override string DisplayName { get { return "AV2CodecConfigurationBox"; } }

	protected byte reserved1 = 0; 
	public byte Reserved1 { get { return this.reserved1; } set { this.reserved1 = value; } }

	protected byte config_obus_count_minus1; 
	public byte ConfigObusCountMinus1 { get { return this.config_obus_count_minus1; } set { this.config_obus_count_minus1 = value; } }

	protected byte[][] config_obu; 
	public byte[][] ConfigObu { get { return this.config_obu; } set { this.config_obu = value; } }

	public AV2CodecConfigurationBox(): base(IsoStream.FromFourCC("av2C"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.reserved1, "reserved1"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.config_obus_count_minus1, "config_obus_count_minus1"); 

		this.config_obu = stream.SafeAllocate<byte[]>(boxSize, readSize, IsoStream.GetInt( config_obus_count_minus1 + 1 + 1), "config_obu");
		for (int i = 0; i < config_obus_count_minus1 + 1; ++i)
		{
			boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.config_obu[i], "config_obu"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.reserved1, "reserved1"); 
		boxSize += stream.WriteUInt8( this.config_obus_count_minus1, "config_obus_count_minus1"); 

		for (int i = 0; i < config_obus_count_minus1 + 1; ++i)
		{
			boxSize += stream.WriteUInt8ArrayTillEnd( this.config_obu[i], "config_obu"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // reserved1
		boxSize += 8; // config_obus_count_minus1

		for (int i = 0; i < config_obus_count_minus1 + 1; ++i)
		{
			boxSize += ((ulong)config_obu[i].Length * 8); // config_obu
		}
		return boxSize;
	}
}

}
