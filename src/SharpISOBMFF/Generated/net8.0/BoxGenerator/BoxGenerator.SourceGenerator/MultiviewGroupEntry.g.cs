using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class MultiviewGroupEntry() extends VisualSampleGroupEntry ('mvif') { 
	unsigned int(8) groupID;
	unsigned int(8) primary_groupID;
	bit(4) reserved = 0;
	unsigned int(1) is_tl_switching_point;
	bit(3) reserved = 0;
	unsigned int(8) tl_switching_distance;
	
	if (groupID == primary_groupID)	// primary definition of tier
	{
		ViewIdentifierBox();			// Mandatory
		TierInfoBox(); 				// Mandatory
		TierDependencyBox(); 		// Mandatory
		PriorityRangeBox();			// Mandatory

		//Optional Boxes or fields may follow when defined later
		TierBitRateBox();						// optional
		BufferingBox();						// optional
		InitialParameterSetBox();			// optional
		ProtectionSchemeInfoBox();			// optional
		ViewPriorityBox();					// optional
	}
}
*/
public partial class MultiviewGroupEntry : VisualSampleGroupEntry
{
	public const string TYPE = "mvif";
	public override string DisplayName { get { return "MultiviewGroupEntry"; } }

	protected byte groupID; 
	public byte GroupID { get { return this.groupID; } set { this.groupID = value; } }

	protected byte primary_groupID; 
	public byte PrimaryGroupID { get { return this.primary_groupID; } set { this.primary_groupID = value; } }

	protected byte reserved = 0; 
	public byte Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool is_tl_switching_point; 
	public bool IsTlSwitchingPoint { get { return this.is_tl_switching_point; } set { this.is_tl_switching_point = value; } }

	protected byte reserved0 = 0; 
	public byte Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	protected byte tl_switching_distance; 
	public byte TlSwitchingDistance { get { return this.tl_switching_distance; } set { this.tl_switching_distance = value; } }
	public ViewIdentifierBox _ViewIdentifierBox { get { return this.children.OfType<ViewIdentifierBox>().FirstOrDefault(); } }
	public TierInfoBox _TierInfoBox { get { return this.children.OfType<TierInfoBox>().FirstOrDefault(); } }
	public TierDependencyBox _TierDependencyBox { get { return this.children.OfType<TierDependencyBox>().FirstOrDefault(); } }
	public PriorityRangeBox _PriorityRangeBox { get { return this.children.OfType<PriorityRangeBox>().FirstOrDefault(); } }
	public TierBitRateBox _TierBitRateBox { get { return this.children.OfType<TierBitRateBox>().FirstOrDefault(); } }
	public BufferingBox _BufferingBox { get { return this.children.OfType<BufferingBox>().FirstOrDefault(); } }
	public InitialParameterSetBox _InitialParameterSetBox { get { return this.children.OfType<InitialParameterSetBox>().FirstOrDefault(); } }
	public ProtectionSchemeInfoBox _ProtectionSchemeInfoBox { get { return this.children.OfType<ProtectionSchemeInfoBox>().FirstOrDefault(); } }
	public ViewPriorityBox _ViewPriorityBox { get { return this.children.OfType<ViewPriorityBox>().FirstOrDefault(); } }

	public MultiviewGroupEntry(): base(IsoStream.FromFourCC("mvif"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.groupID, "groupID"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.primary_groupID, "primary_groupID"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.reserved, "reserved"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.is_tl_switching_point, "is_tl_switching_point"); 
		boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.reserved0, "reserved0"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.tl_switching_distance, "tl_switching_distance"); 

		if (groupID == primary_groupID)
		{
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ViewIdentifierBox, "ViewIdentifierBox"); // Mandatory
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TierInfoBox, "TierInfoBox"); // Mandatory
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TierDependencyBox, "TierDependencyBox"); // Mandatory
			// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.PriorityRangeBox, "PriorityRangeBox"); // Mandatory
			/* Optional Boxes or fields may follow when defined later */
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.TierBitRateBox, "TierBitRateBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.BufferingBox, "BufferingBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.InitialParameterSetBox, "InitialParameterSetBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ProtectionSchemeInfoBox, "ProtectionSchemeInfoBox"); // optional
			// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.ViewPriorityBox, "ViewPriorityBox"); // optional
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
		boxSize += stream.WriteBits(4,  this.reserved, "reserved"); 
		boxSize += stream.WriteBit( this.is_tl_switching_point, "is_tl_switching_point"); 
		boxSize += stream.WriteBits(3,  this.reserved0, "reserved0"); 
		boxSize += stream.WriteUInt8( this.tl_switching_distance, "tl_switching_distance"); 

		if (groupID == primary_groupID)
		{
			// boxSize += stream.WriteBox( this.ViewIdentifierBox, "ViewIdentifierBox"); // Mandatory
			// boxSize += stream.WriteBox( this.TierInfoBox, "TierInfoBox"); // Mandatory
			// boxSize += stream.WriteBox( this.TierDependencyBox, "TierDependencyBox"); // Mandatory
			// boxSize += stream.WriteBox( this.PriorityRangeBox, "PriorityRangeBox"); // Mandatory
			/* Optional Boxes or fields may follow when defined later */
			// boxSize += stream.WriteBox( this.TierBitRateBox, "TierBitRateBox"); // optional
			// boxSize += stream.WriteBox( this.BufferingBox, "BufferingBox"); // optional
			// boxSize += stream.WriteBox( this.InitialParameterSetBox, "InitialParameterSetBox"); // optional
			// boxSize += stream.WriteBox( this.ProtectionSchemeInfoBox, "ProtectionSchemeInfoBox"); // optional
			// boxSize += stream.WriteBox( this.ViewPriorityBox, "ViewPriorityBox"); // optional
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
		boxSize += 4; // reserved
		boxSize += 1; // is_tl_switching_point
		boxSize += 3; // reserved0
		boxSize += 8; // tl_switching_distance

		if (groupID == primary_groupID)
		{
			// boxSize += IsoStream.CalculateBoxSize(ViewIdentifierBox); // ViewIdentifierBox
			// boxSize += IsoStream.CalculateBoxSize(TierInfoBox); // TierInfoBox
			// boxSize += IsoStream.CalculateBoxSize(TierDependencyBox); // TierDependencyBox
			// boxSize += IsoStream.CalculateBoxSize(PriorityRangeBox); // PriorityRangeBox
			/* Optional Boxes or fields may follow when defined later */
			// boxSize += IsoStream.CalculateBoxSize(TierBitRateBox); // TierBitRateBox
			// boxSize += IsoStream.CalculateBoxSize(BufferingBox); // BufferingBox
			// boxSize += IsoStream.CalculateBoxSize(InitialParameterSetBox); // InitialParameterSetBox
			// boxSize += IsoStream.CalculateBoxSize(ProtectionSchemeInfoBox); // ProtectionSchemeInfoBox
			// boxSize += IsoStream.CalculateBoxSize(ViewPriorityBox); // ViewPriorityBox
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
