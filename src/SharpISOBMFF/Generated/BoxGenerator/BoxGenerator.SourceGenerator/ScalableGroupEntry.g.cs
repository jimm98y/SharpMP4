using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ScalableGroupEntry() extends VisualSampleGroupEntry ('scif') { 
	unsigned int(8) groupID;
	unsigned int(8) primary_groupID;
	unsigned int(1) is_tier_IDR;
	unsigned int(1) noInterLayerPredFlag; 
	unsigned int(1) useRefBasePicFlag;
	unsigned int(1) storeBaseRepFlag; 
	unsigned int(1) is_tl_switching_point;
	bit(3) reserved = 0;
	unsigned int(8) tl_switching_distance;
	
	if (groupID == primary_groupID)	// primary definition of tier
	{
		TierInfoBox(); 				// Mandatory
		SVCDependencyRangeBox();	// Mandatory
		PriorityRangeBox();			// Mandatory

		//Optional Boxes or fields may follow when defined later
		TierBitRateBox();						// optional
		RectRegionBox();						// optional
		BufferingBox();						// optional
		TierDependencyBox(); 				// optional
		InitialParameterSetBox();			// optional
		IroiInfoBox();							// optional
		ProtectionSchemeInfoBox();			// optional
		TranscodingInfoBox();				// optional
	}
}
*/
public partial class ScalableGroupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "scif";
	public override string DisplayName { get { return "ScalableGroupEntry"; } }

	protected byte groupID; 
	public byte GroupID { get { return this.groupID; } set { this.groupID = value; } }

	protected byte primary_groupID; 
	public byte PrimaryGroupID { get { return this.primary_groupID; } set { this.primary_groupID = value; } }

	protected bool is_tier_IDR; 
	public bool IsTierIDR { get { return this.is_tier_IDR; } set { this.is_tier_IDR = value; } }

	protected bool noInterLayerPredFlag; 
	public bool NoInterLayerPredFlag { get { return this.noInterLayerPredFlag; } set { this.noInterLayerPredFlag = value; } }

	protected bool useRefBasePicFlag; 
	public bool UseRefBasePicFlag { get { return this.useRefBasePicFlag; } set { this.useRefBasePicFlag = value; } }

	protected bool storeBaseRepFlag; 
	public bool StoreBaseRepFlag { get { return this.storeBaseRepFlag; } set { this.storeBaseRepFlag = value; } }

	protected bool is_tl_switching_point; 
	public bool IsTlSwitchingPoint { get { return this.is_tl_switching_point; } set { this.is_tl_switching_point = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected byte tl_switching_distance; 
	public byte TlSwitchingDistance { get { return this.tl_switching_distance; } set { this.tl_switching_distance = value; } }
	public TierInfoBox _TierInfoBox { get { return this.children.OfType<TierInfoBox>().FirstOrDefault(); } }
	public SVCDependencyRangeBox _SVCDependencyRangeBox { get { return this.children.OfType<SVCDependencyRangeBox>().FirstOrDefault(); } }
	public PriorityRangeBox _PriorityRangeBox { get { return this.children.OfType<PriorityRangeBox>().FirstOrDefault(); } }
	public TierBitRateBox _TierBitRateBox { get { return this.children.OfType<TierBitRateBox>().FirstOrDefault(); } }
	public RectRegionBox _RectRegionBox { get { return this.children.OfType<RectRegionBox>().FirstOrDefault(); } }
	public BufferingBox _BufferingBox { get { return this.children.OfType<BufferingBox>().FirstOrDefault(); } }
	public TierDependencyBox _TierDependencyBox { get { return this.children.OfType<TierDependencyBox>().FirstOrDefault(); } }
	public InitialParameterSetBox _InitialParameterSetBox { get { return this.children.OfType<InitialParameterSetBox>().FirstOrDefault(); } }
	public IroiInfoBox _IroiInfoBox { get { return this.children.OfType<IroiInfoBox>().FirstOrDefault(); } }
	public ProtectionSchemeInfoBox _ProtectionSchemeInfoBox { get { return this.children.OfType<ProtectionSchemeInfoBox>().FirstOrDefault(); } }
	public TranscodingInfoBox _TranscodingInfoBox { get { return this.children.OfType<TranscodingInfoBox>().FirstOrDefault(); } }

	public ScalableGroupEntry(): base(IsoStream.FromFourCC("scif"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.groupID, "groupID"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.primary_groupID, "primary_groupID"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.is_tier_IDR, "is_tier_IDR"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.noInterLayerPredFlag, "noInterLayerPredFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.useRefBasePicFlag, "useRefBasePicFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.storeBaseRepFlag, "storeBaseRepFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.is_tl_switching_point, "is_tl_switching_point"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.tl_switching_distance, "tl_switching_distance"); 

		if (groupID == primary_groupID)
		{
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TierInfoBox, "TierInfoBox"); // Mandatory
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SVCDependencyRangeBox, "SVCDependencyRangeBox"); // Mandatory
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.PriorityRangeBox, "PriorityRangeBox"); // Mandatory
			/* Optional Boxes or fields may follow when defined later */
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TierBitRateBox, "TierBitRateBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.RectRegionBox, "RectRegionBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.BufferingBox, "BufferingBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TierDependencyBox, "TierDependencyBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.InitialParameterSetBox, "InitialParameterSetBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.IroiInfoBox, "IroiInfoBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ProtectionSchemeInfoBox, "ProtectionSchemeInfoBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TranscodingInfoBox, "TranscodingInfoBox"); // optional
		}
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8( this.groupID, "groupID"); 
		boxSize += stream.WriteUInt8( this.primary_groupID, "primary_groupID"); 
		boxSize += stream.WriteBit( this.is_tier_IDR, "is_tier_IDR"); 
		boxSize += stream.WriteBit( this.noInterLayerPredFlag, "noInterLayerPredFlag"); 
		boxSize += stream.WriteBit( this.useRefBasePicFlag, "useRefBasePicFlag"); 
		boxSize += stream.WriteBit( this.storeBaseRepFlag, "storeBaseRepFlag"); 
		boxSize += stream.WriteBit( this.is_tl_switching_point, "is_tl_switching_point"); 
		boxSize += stream.WriteBits(3,  this.reserved, "reserved"); 
		boxSize += stream.WriteUInt8( this.tl_switching_distance, "tl_switching_distance"); 

		if (groupID == primary_groupID)
		{
			// boxSize += stream.WriteBox( this.TierInfoBox, "TierInfoBox"); // Mandatory
			// boxSize += stream.WriteBox( this.SVCDependencyRangeBox, "SVCDependencyRangeBox"); // Mandatory
			// boxSize += stream.WriteBox( this.PriorityRangeBox, "PriorityRangeBox"); // Mandatory
			/* Optional Boxes or fields may follow when defined later */
			// boxSize += stream.WriteBox( this.TierBitRateBox, "TierBitRateBox"); // optional
			// boxSize += stream.WriteBox( this.RectRegionBox, "RectRegionBox"); // optional
			// boxSize += stream.WriteBox( this.BufferingBox, "BufferingBox"); // optional
			// boxSize += stream.WriteBox( this.TierDependencyBox, "TierDependencyBox"); // optional
			// boxSize += stream.WriteBox( this.InitialParameterSetBox, "InitialParameterSetBox"); // optional
			// boxSize += stream.WriteBox( this.IroiInfoBox, "IroiInfoBox"); // optional
			// boxSize += stream.WriteBox( this.ProtectionSchemeInfoBox, "ProtectionSchemeInfoBox"); // optional
			// boxSize += stream.WriteBox( this.TranscodingInfoBox, "TranscodingInfoBox"); // optional
		}
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 8; // groupID
		boxSize += 8; // primary_groupID
		boxSize += 1; // is_tier_IDR
		boxSize += 1; // noInterLayerPredFlag
		boxSize += 1; // useRefBasePicFlag
		boxSize += 1; // storeBaseRepFlag
		boxSize += 1; // is_tl_switching_point
		boxSize += 3; // reserved
		boxSize += 8; // tl_switching_distance

		if (groupID == primary_groupID)
		{
			// boxSize += IsoStream.CalculateBoxSize(TierInfoBox); // TierInfoBox
			// boxSize += IsoStream.CalculateBoxSize(SVCDependencyRangeBox); // SVCDependencyRangeBox
			// boxSize += IsoStream.CalculateBoxSize(PriorityRangeBox); // PriorityRangeBox
			/* Optional Boxes or fields may follow when defined later */
			// boxSize += IsoStream.CalculateBoxSize(TierBitRateBox); // TierBitRateBox
			// boxSize += IsoStream.CalculateBoxSize(RectRegionBox); // RectRegionBox
			// boxSize += IsoStream.CalculateBoxSize(BufferingBox); // BufferingBox
			// boxSize += IsoStream.CalculateBoxSize(TierDependencyBox); // TierDependencyBox
			// boxSize += IsoStream.CalculateBoxSize(InitialParameterSetBox); // InitialParameterSetBox
			// boxSize += IsoStream.CalculateBoxSize(IroiInfoBox); // IroiInfoBox
			// boxSize += IsoStream.CalculateBoxSize(ProtectionSchemeInfoBox); // ProtectionSchemeInfoBox
			// boxSize += IsoStream.CalculateBoxSize(TranscodingInfoBox); // TranscodingInfoBox
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
