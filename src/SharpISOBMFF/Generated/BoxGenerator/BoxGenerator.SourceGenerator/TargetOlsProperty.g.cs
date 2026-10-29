using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class TargetOlsProperty
	extends ItemFullProperty('tols', version = 0, flags = 0){
	unsigned int(16) target_ols_idx;
}
*/
public partial class TargetOlsProperty : ItemFullProperty
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class TargetOlsProperty
	extends ItemFullProperty('tols', version = 0, flags = 0){
	unsigned int(16) target_ols_idx;
}"; } }
	public const string TYPE = "tols";
	public override string DisplayName { get { return "TargetOlsProperty"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("ipco"); }


	protected ushort target_ols_idx; 
	public ushort TargetOlsIdx { get { return this.target_ols_idx; } set { this.target_ols_idx = value; } }

	public TargetOlsProperty(): base(IsoStream.FromFourCC("tols"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.target_ols_idx, "target_ols_idx"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.target_ols_idx, "target_ols_idx"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // target_ols_idx
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("target_ols_idx", 16, this.target_ols_idx); 
		return boxSize;
	}
}

}
