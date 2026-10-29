using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ESDBox
	extends FullBox('esds', version = 0, 0) {
	ES_Descriptor ES;
}
*/
public partial class ESDBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class ESDBox
	extends FullBox('esds', version = 0, 0) {
	ES_Descriptor ES;
}"; } }
	public const string TYPE = "esds";
	public override string DisplayName { get { return "ESDBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(mp4a,enca)", "VisualSampleEntry(mp4v,encv)", "MpegSampleEntry(mp4s,encs)", "wave", "AudioSampleEntry(m4ae)"); }


	protected ES_Descriptor ES; 
	public ES_Descriptor _ES { get { return this.ES; } set { this.ES = value; } }

	public ESDBox(): base(IsoStream.FromFourCC("esds"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.ES, "ES"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteDescriptor( this.ES, "ES"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateDescriptorSize(ES); // ES
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("ES", IsoStream.CalculateDescriptorSize(ES), this.ES); 
		return boxSize;
	}
}

}
