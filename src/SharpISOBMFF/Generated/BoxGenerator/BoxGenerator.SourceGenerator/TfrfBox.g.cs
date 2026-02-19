using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TfrfBox() extends FullBox('uuid d4807ef2ca3946958e5426cb9e46a79f') {
	 unsigned int(8) fragmentCount;
 for(i = 0; i < fragmentCount; i++) {
 if(version == 0x1) {
 unsigned int(64) fragmentAbsoluteTime;
 unsigned int(64) fragmentAbsoluteDuration;
 } else {
 unsigned int(32) fragmentAbsoluteTime;
 unsigned int(32) fragmentAbsoluteDuration;
 }
 } }
*/
public partial class TfrfBox : FullBox
{
	public const string TYPE = "uuid";
	public override string DisplayName { get { return "TfrfBox"; } }

	protected byte fragmentCount; 
	public byte FragmentCount { get { return this.fragmentCount; } set { this.fragmentCount = value; } }

	protected ulong[] fragmentAbsoluteTime; 
	public ulong[] FragmentAbsoluteTime { get { return this.fragmentAbsoluteTime; } set { this.fragmentAbsoluteTime = value; } }

	protected ulong[] fragmentAbsoluteDuration; 
	public ulong[] FragmentAbsoluteDuration { get { return this.fragmentAbsoluteDuration; } set { this.fragmentAbsoluteDuration = value; } }

	public TfrfBox(): base(IsoStream.FromFourCC("uuid"), ConvertEx.FromHexString("d4807ef2ca3946958e5426cb9e46a79f"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.fragmentCount, "fragmentCount"); 

		this.fragmentAbsoluteTime = new ulong[IsoStream.GetInt( fragmentCount)];
		this.fragmentAbsoluteDuration = new ulong[IsoStream.GetInt( fragmentCount)];
		for (int i = 0; i < fragmentCount; i++)
		{

			if (version == 0x1)
			{
				boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragmentAbsoluteTime[i], "fragmentAbsoluteTime"); 
				boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragmentAbsoluteDuration[i], "fragmentAbsoluteDuration"); 
			}

			else 
			{
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragmentAbsoluteTime[i], "fragmentAbsoluteTime"); 
				boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragmentAbsoluteDuration[i], "fragmentAbsoluteDuration"); 
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.fragmentCount, "fragmentCount"); 

		for (int i = 0; i < fragmentCount; i++)
		{

			if (version == 0x1)
			{
				boxSize += stream.WriteUInt64( this.fragmentAbsoluteTime[i], "fragmentAbsoluteTime"); 
				boxSize += stream.WriteUInt64( this.fragmentAbsoluteDuration[i], "fragmentAbsoluteDuration"); 
			}

			else 
			{
				boxSize += stream.WriteUInt32( this.fragmentAbsoluteTime[i], "fragmentAbsoluteTime"); 
				boxSize += stream.WriteUInt32( this.fragmentAbsoluteDuration[i], "fragmentAbsoluteDuration"); 
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // fragmentCount

		for (int i = 0; i < fragmentCount; i++)
		{

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
		}
		return boxSize;
	}
}

}
