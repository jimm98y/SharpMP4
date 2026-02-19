using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class StereoViewInformationBox extends FullBox('stri', 0, 0) { 
unsigned int(4) reserved;   
// reserved, set to 0 
unsigned int(1) eye_views_reversed; 
unsigned int(1) has_additional_views; 
unsigned int(1) has_right_eye_view; // video contains a right-eye view 
unsigned int(1) has_left_eye_view; 
// video contains a left-eye view 
} 
*/
public partial class StereoViewInformationBox : FullBox
{
	public const string TYPE = "stri";
	public override string DisplayName { get { return "StereoViewInformationBox"; } }

	protected byte reserved;  //  reserved, set to 0 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool eye_views_reversed; 
	public bool EyeViewsReversed { get { return this.eye_views_reversed; } set { this.eye_views_reversed = value; } }

	protected bool has_additional_views; 
	public bool HasAdditionalViews { get { return this.has_additional_views; } set { this.has_additional_views = value; } }

	protected bool has_right_eye_view;  //  video contains a right-eye view 
	public bool HasRightEyeView { get { return this.has_right_eye_view; } set { this.has_right_eye_view = value; } }

	protected bool has_left_eye_view;  //  video contains a left-eye view 
	public bool HasLeftEyeView { get { return this.has_left_eye_view; } set { this.has_left_eye_view = value; } }

	public StereoViewInformationBox(): base(IsoStream.FromFourCC("stri"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); // reserved, set to 0 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.eye_views_reversed, "eye_views_reversed"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.has_additional_views, "has_additional_views"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.has_right_eye_view, "has_right_eye_view"); // video contains a right-eye view 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.has_left_eye_view, "has_left_eye_view"); // video contains a left-eye view 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits(4,  this.reserved, "reserved"); // reserved, set to 0 
		boxSize += stream.WriteBit( this.eye_views_reversed, "eye_views_reversed"); 
		boxSize += stream.WriteBit( this.has_additional_views, "has_additional_views"); 
		boxSize += stream.WriteBit( this.has_right_eye_view, "has_right_eye_view"); // video contains a right-eye view 
		boxSize += stream.WriteBit( this.has_left_eye_view, "has_left_eye_view"); // video contains a left-eye view 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 4; // reserved
		boxSize += 1; // eye_views_reversed
		boxSize += 1; // has_additional_views
		boxSize += 1; // has_right_eye_view
		boxSize += 1; // has_left_eye_view
		return boxSize;
	}
}

}
