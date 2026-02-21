using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SeiInformationBox extends Box('seii') {
	unsigned int(16) numRequiredSEIs;
	for (i = 0; i < numRequiredSEIs; i++) {
	unsigned int(16) requiredSEI_ID;
	}
	unsigned int(16) numNotRequiredSEIs;
	for (i = 0; i < numNotRequiredSEIs; i++) {
	unsigned int(16) notrequiredSEI_ID;
	}
}
*/
public partial class SeiInformationBox : Box
{
	public const string TYPE = "seii";
	public override string DisplayName { get { return "SeiInformationBox"; } }

	protected ushort numRequiredSEIs; 
	public ushort NumRequiredSEIs { get { return this.numRequiredSEIs; } set { this.numRequiredSEIs = value; } }

	protected ushort[] requiredSEI_ID; 
	public ushort[] RequiredSEIID { get { return this.requiredSEI_ID; } set { this.requiredSEI_ID = value; } }

	protected ushort numNotRequiredSEIs; 
	public ushort NumNotRequiredSEIs { get { return this.numNotRequiredSEIs; } set { this.numNotRequiredSEIs = value; } }

	protected ushort[] notrequiredSEI_ID; 
	public ushort[] NotrequiredSEIID { get { return this.notrequiredSEI_ID; } set { this.notrequiredSEI_ID = value; } }

	public SeiInformationBox(): base(IsoStream.FromFourCC("seii"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.numRequiredSEIs, "numRequiredSEIs"); 

		this.requiredSEI_ID = new ushort[IsoStream.GetInt( numRequiredSEIs)];
		for (int i = 0; i < numRequiredSEIs; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.requiredSEI_ID[i], "requiredSEI_ID"); 
		}
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.numNotRequiredSEIs, "numNotRequiredSEIs"); 

		this.notrequiredSEI_ID = new ushort[IsoStream.GetInt( numNotRequiredSEIs)];
		for (int i = 0; i < numNotRequiredSEIs; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.notrequiredSEI_ID[i], "notrequiredSEI_ID"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.numRequiredSEIs, "numRequiredSEIs"); 

		for (int i = 0; i < numRequiredSEIs; i++)
		{
			boxSize += stream.WriteUInt16( this.requiredSEI_ID[i], "requiredSEI_ID"); 
		}
		boxSize += stream.WriteUInt16( this.numNotRequiredSEIs, "numNotRequiredSEIs"); 

		for (int i = 0; i < numNotRequiredSEIs; i++)
		{
			boxSize += stream.WriteUInt16( this.notrequiredSEI_ID[i], "notrequiredSEI_ID"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // numRequiredSEIs

		for (int i = 0; i < numRequiredSEIs; i++)
		{
			boxSize += 16; // requiredSEI_ID
		}
		boxSize += 16; // numNotRequiredSEIs

		for (int i = 0; i < numNotRequiredSEIs; i++)
		{
			boxSize += 16; // notrequiredSEI_ID
		}
		return boxSize;
	}
}

}
