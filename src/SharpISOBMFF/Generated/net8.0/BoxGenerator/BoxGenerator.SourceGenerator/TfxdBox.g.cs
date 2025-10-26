using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class TfxdBox() extends FullBox('uuid 6d1d9b0542d544e680e2141daff757b2') {
	 if(version == 0x1) {
 unsigned int(64) fragmentAbsoluteTime;
 unsigned int(64) fragmentAbsoluteDuration;
 } else {
 unsigned int(32) fragmentAbsoluteTime;
 unsigned int(32) fragmentAbsoluteDuration;
 } }
*/
public partial class TfxdBox : FullBox
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "TfxdBox"; } }

	protected ulong fragmentAbsoluteTime; 
	public ulong FragmentAbsoluteTime { get { return this.fragmentAbsoluteTime; } set { this.fragmentAbsoluteTime = value; } }

	protected ulong fragmentAbsoluteDuration; 
	public ulong FragmentAbsoluteDuration { get { return this.fragmentAbsoluteDuration; } set { this.fragmentAbsoluteDuration = value; } }

	public TfxdBox(): base(IsoStream.FromFourCC("uuid"), Convert.FromHexString("6d1d9b0542d544e680e2141daff757b2"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version == 0x1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}

		else 
		{
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version == 0x1)
		{
			boxSize += stream.WriteUInt64( this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.WriteUInt64( this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}

		else 
		{
			boxSize += stream.WriteUInt32( this.fragmentAbsoluteTime, "fragmentAbsoluteTime"); 
			boxSize += stream.WriteUInt32( this.fragmentAbsoluteDuration, "fragmentAbsoluteDuration"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version == 0x1)
		{
			boxSize += 64; // fragmentAbsoluteTime
			boxSize += 64; // fragmentAbsoluteDuration
		}

		else 
		{
			boxSize += 32; // fragmentAbsoluteTime
			boxSize += 32; // fragmentAbsoluteDuration
		}
		return boxSize;
	}
}

}
