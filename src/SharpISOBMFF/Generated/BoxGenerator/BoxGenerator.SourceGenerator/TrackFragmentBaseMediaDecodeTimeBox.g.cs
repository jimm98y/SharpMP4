using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TrackFragmentBaseMediaDecodeTimeBox
	extends FullBox('tfdt', version, 0) {
	if (version==1) {
		unsigned int(64) baseMediaDecodeTime;
	} else { // version==0
		unsigned int(32) baseMediaDecodeTime;
	}
}
*/
public partial class TrackFragmentBaseMediaDecodeTimeBox : FullBox
{
	public const string TYPE = "tfdt";
	public override string DisplayName { get { return "TrackFragmentBaseMediaDecodeTimeBox"; } }

	protected ulong baseMediaDecodeTime; 
	public ulong BaseMediaDecodeTime { get { return this.baseMediaDecodeTime; } set { this.baseMediaDecodeTime = value; } }

	public TrackFragmentBaseMediaDecodeTimeBox(byte version = 0): base(IsoStream.FromFourCC("tfdt"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.baseMediaDecodeTime, "baseMediaDecodeTime"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.baseMediaDecodeTime, "baseMediaDecodeTime"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==1)
		{
			boxSize += stream.WriteUInt64( this.baseMediaDecodeTime, "baseMediaDecodeTime"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.WriteUInt32( this.baseMediaDecodeTime, "baseMediaDecodeTime"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==1)
		{
			boxSize += 64; // baseMediaDecodeTime
		}

		else 
		{
			/*  version==0 */
			boxSize += 32; // baseMediaDecodeTime
		}
		return boxSize;
	}
}

}
