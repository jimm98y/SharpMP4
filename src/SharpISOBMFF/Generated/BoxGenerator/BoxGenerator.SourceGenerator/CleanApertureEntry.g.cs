using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class CleanApertureEntry() extends VisualSampleGroupEntry ('casg'){
	unsigned int(32) cleanApertureWidthN;
	unsigned int(32) cleanApertureWidthD;

	unsigned int(32) cleanApertureHeightN;
	unsigned int(32) cleanApertureHeightD;


	unsigned int(32) horizOffN;
	unsigned int(32) horizOffD;


	unsigned int(32) vertOffN;
	unsigned int(32) vertOffD;

}
*/
public partial class CleanApertureEntry : VisualSampleGroupEntry
{
	public const string TYPE = "casg";
	public override string DisplayName { get { return "CleanApertureEntry"; } }

	protected uint cleanApertureWidthN; 
	public uint CleanApertureWidthN { get { return this.cleanApertureWidthN; } set { this.cleanApertureWidthN = value; } }

	protected uint cleanApertureWidthD; 
	public uint CleanApertureWidthD { get { return this.cleanApertureWidthD; } set { this.cleanApertureWidthD = value; } }

	protected uint cleanApertureHeightN; 
	public uint CleanApertureHeightN { get { return this.cleanApertureHeightN; } set { this.cleanApertureHeightN = value; } }

	protected uint cleanApertureHeightD; 
	public uint CleanApertureHeightD { get { return this.cleanApertureHeightD; } set { this.cleanApertureHeightD = value; } }

	protected uint horizOffN; 
	public uint HorizOffN { get { return this.horizOffN; } set { this.horizOffN = value; } }

	protected uint horizOffD; 
	public uint HorizOffD { get { return this.horizOffD; } set { this.horizOffD = value; } }

	protected uint vertOffN; 
	public uint VertOffN { get { return this.vertOffN; } set { this.vertOffN = value; } }

	protected uint vertOffD; 
	public uint VertOffD { get { return this.vertOffD; } set { this.vertOffD = value; } }

	public CleanApertureEntry(): base(IsoStream.FromFourCC("casg"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cleanApertureWidthN, "cleanApertureWidthN"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cleanApertureWidthD, "cleanApertureWidthD"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cleanApertureHeightN, "cleanApertureHeightN"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.cleanApertureHeightD, "cleanApertureHeightD"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.horizOffN, "horizOffN"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.horizOffD, "horizOffD"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vertOffN, "vertOffN"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.vertOffD, "vertOffD"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.cleanApertureWidthN, "cleanApertureWidthN"); 
		boxSize += stream.WriteUInt32( this.cleanApertureWidthD, "cleanApertureWidthD"); 
		boxSize += stream.WriteUInt32( this.cleanApertureHeightN, "cleanApertureHeightN"); 
		boxSize += stream.WriteUInt32( this.cleanApertureHeightD, "cleanApertureHeightD"); 
		boxSize += stream.WriteUInt32( this.horizOffN, "horizOffN"); 
		boxSize += stream.WriteUInt32( this.horizOffD, "horizOffD"); 
		boxSize += stream.WriteUInt32( this.vertOffN, "vertOffN"); 
		boxSize += stream.WriteUInt32( this.vertOffD, "vertOffD"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // cleanApertureWidthN
		boxSize += 32; // cleanApertureWidthD
		boxSize += 32; // cleanApertureHeightN
		boxSize += 32; // cleanApertureHeightD
		boxSize += 32; // horizOffN
		boxSize += 32; // horizOffD
		boxSize += 32; // vertOffN
		boxSize += 32; // vertOffD
		return boxSize;
	}
}

}
