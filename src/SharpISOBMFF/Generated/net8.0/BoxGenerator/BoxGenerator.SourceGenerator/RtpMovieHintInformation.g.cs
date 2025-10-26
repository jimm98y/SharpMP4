using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class RtpMovieHintInformation extends Box('rtp ') {
	uint(32) descriptionformat = 'sdp ';
	char  sdptext[];
}
*/
public partial class RtpMovieHintInformation : Box
{
	public const string TYPE = "rtp ";
	public override string DisplayName { get { return "RtpMovieHintInformation"; } }

	protected uint descriptionformat = IsoStream.FromFourCC("sdp "); 
	public uint Descriptionformat { get { return this.descriptionformat; } set { this.descriptionformat = value; } }

	protected byte[] sdptext; 
	public byte[] Sdptext { get { return this.sdptext; } set { this.sdptext = value; } }

	public RtpMovieHintInformation(): base(IsoStream.FromFourCC("rtp "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.descriptionformat, "descriptionformat"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.sdptext, "sdptext"); 
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.descriptionformat, "descriptionformat"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.sdptext, "sdptext"); 
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // descriptionformat
		boxSize += ((ulong)sdptext.Length * 8); // sdptext
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
