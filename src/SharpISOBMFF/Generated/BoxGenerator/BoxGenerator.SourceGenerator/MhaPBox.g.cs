using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// MPEG-H 3D Audio's compatible profile and level sets, as GPAC's mhap_box_read reads them
aligned(8) class MhaPBox() extends Box('mhaP') {
	unsigned int(8) numCompatibleSets;
	for (i=0; i < numCompatibleSets; i++) {
		unsigned int(8) CompatibleSetIndication;
	}
}
*/
public partial class MhaPBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"// MPEG-H 3D Audio's compatible profile and level sets, as GPAC's mhap_box_read reads them
aligned(8) class MhaPBox() extends Box('mhaP') {
	unsigned int(8) numCompatibleSets;
	for (i=0; i < numCompatibleSets; i++) {
		unsigned int(8) CompatibleSetIndication;
	}
}"; } }
	public const string TYPE = "mhaP";
	public override string DisplayName { get { return "MhaPBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AudioSampleEntry(mha1,mha2,mhm1,mhm2)", "wave"); }


	protected byte numCompatibleSets; 
	public byte NumCompatibleSets { get { return this.numCompatibleSets; } set { this.numCompatibleSets = value; } }

	protected byte[] CompatibleSetIndication; 
	public byte[] _CompatibleSetIndication { get { return this.CompatibleSetIndication; } set { this.CompatibleSetIndication = value; } }

	public MhaPBox(): base(IsoStream.FromFourCC("mhaP"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.numCompatibleSets, "numCompatibleSets"); 

		this.CompatibleSetIndication = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( numCompatibleSets), "CompatibleSetIndication");
		for (int i=0; i < numCompatibleSets; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.CompatibleSetIndication[i], "CompatibleSetIndication"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.numCompatibleSets, "numCompatibleSets"); 

		for (int i=0; i < numCompatibleSets; i++)
		{
			boxSize += stream.WriteUInt8( this.CompatibleSetIndication[i], "CompatibleSetIndication"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // numCompatibleSets

		for (int i=0; i < numCompatibleSets; i++)
		{
			boxSize += 8; // CompatibleSetIndication
		}
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("numCompatibleSets", 8, this.numCompatibleSets); 

		for (int i=0; i < numCompatibleSets; i++)
		{
			boxSize += tracer.Field($"CompatibleSetIndication[{i}]", 8, this.CompatibleSetIndication[i]); 
		}
		return boxSize;
	}
}

}
