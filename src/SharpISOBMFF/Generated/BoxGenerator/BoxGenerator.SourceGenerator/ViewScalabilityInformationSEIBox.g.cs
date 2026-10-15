using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ViewScalabilityInformationSEIBox extends Box('vsib', size)
{
	unsigned int(8*size-64)	mvcscalinfosei; 
}
*/
public partial class ViewScalabilityInformationSEIBox : Box
{
	public const string TYPE = "vsib";
	public override string DisplayName { get { return "ViewScalabilityInformationSEIBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCMVCSampleEntry(avc1,avc3)", "AVC2MVCSampleEntry(avc2,avc4)", "MVCSampleEntry(mvc1,mvc2,mvc3,mvc4)"); }


	protected byte[] mvcscalinfosei; 
	public byte[] Mvcscalinfosei { get { return this.mvcscalinfosei; } set { this.mvcscalinfosei = value; } }

	public ViewScalabilityInformationSEIBox(ulong size = 0): base(IsoStream.FromFourCC("vsib"), size)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*size-64 ),  out this.mvcscalinfosei, "mvcscalinfosei"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits((uint)(8*size-64 ),  this.mvcscalinfosei, "mvcscalinfosei"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += (ulong)(8*size-64 ); // mvcscalinfosei
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("mvcscalinfosei", (ulong)(8*size-64 ), this.mvcscalinfosei); 
		return boxSize;
	}
}

}
