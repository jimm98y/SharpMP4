using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MovieExtendsHeaderBox extends FullBox('mehd', version, 0) {
	if (version==1) {
		unsigned int(64)	fragment_duration;
	} else { // version==0
		unsigned int(32)	fragment_duration;
	}
}
*/
public partial class MovieExtendsHeaderBox : FullBox
{
	public const string TYPE = "mehd";
	public override string DisplayName { get { return "MovieExtendsHeaderBox"; } }

	protected ulong fragment_duration; 
	public ulong FragmentDuration { get { return this.fragment_duration; } set { this.fragment_duration = value; } }

	public MovieExtendsHeaderBox(byte version = 0): base(IsoStream.FromFourCC("mehd"), version, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);

		if (version==1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.fragment_duration, "fragment_duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.fragment_duration, "fragment_duration"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);

		if (version==1)
		{
			boxSize += stream.WriteUInt64( this.fragment_duration, "fragment_duration"); 
		}

		else 
		{
			/*  version==0 */
			boxSize += stream.WriteUInt32( this.fragment_duration, "fragment_duration"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();

		if (version==1)
		{
			boxSize += 64; // fragment_duration
		}

		else 
		{
			/*  version==0 */
			boxSize += 32; // fragment_duration
		}
		return boxSize;
	}
}

}
