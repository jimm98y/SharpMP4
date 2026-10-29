using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MPEG4ExtensionDescriptorsBox extends Box('m4ds') {
	Descriptor Descr[0 .. 255];
}
*/
public partial class MPEG4ExtensionDescriptorsBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class MPEG4ExtensionDescriptorsBox extends Box('m4ds') {
	Descriptor Descr[0 .. 255];
}"; } }
	public const string TYPE = "m4ds";
	public override string DisplayName { get { return "MPEG4ExtensionDescriptorsBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCSampleEntry(avc1,avc3)", "AVC2SampleEntry(avc2,avc4)", "SVCSampleEntry(svc1,svc2)", "MVCSampleEntry(mvc1,mvc2,mvc3,mvc4)", "MVCDSampleEntry(mvd1,mvd2,mvd3,mvd4)", "A3DSampleEntry(a3d1,a3d2,a3d3,a3d4)", "HEVCSampleEntry(hvc1,hev1,hvc2,hev2,hvc3,hev3)", "LHEVCSampleEntry(lhv1,lhe1)", "VvcSampleEntry(vvc1,vvi1)", "EVCSampleEntry(evc1)"); }


	protected Descriptor[] Descr = []; 
	public Descriptor[] _Descr { get { return this.Descr; } set { this.Descr = value; } }

	public MPEG4ExtensionDescriptorsBox(): base(IsoStream.FromFourCC("m4ds"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.Descr, "Descr"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDescriptor( this.Descr, "Descr"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateDescriptorSize(Descr); // Descr
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("Descr", IsoStream.CalculateDescriptorSize(Descr), this.Descr); 
		return boxSize;
	}
}

}
