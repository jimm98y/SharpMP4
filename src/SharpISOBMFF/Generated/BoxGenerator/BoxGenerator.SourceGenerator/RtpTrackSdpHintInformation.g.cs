using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class RtpTrackSdpHintInformation extends Box('sdp ') {
	char	sdptext[];
}
*/
public partial class RtpTrackSdpHintInformation : Box
{
	public const string TYPE = "sdp ";
	public override string DisplayName { get { return "RtpTrackSdpHintInformation"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("hnti"); }


	protected byte[] sdptext; 
	public byte[] Sdptext { get { return this.sdptext; } set { this.sdptext = value; } }

	public RtpTrackSdpHintInformation(): base(IsoStream.FromFourCC("sdp "))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.sdptext, "sdptext"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.sdptext, "sdptext"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)sdptext.Length * 8); // sdptext
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("sdptext", ((ulong)sdptext.Length * 8), this.sdptext); 
		return boxSize;
	}
}

}
