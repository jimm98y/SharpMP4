using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaBox (handler_type)
	extends FullBox('meta', version = 0, 0) {
	HandlerBox(handler_type)	theHandler;
	PrimaryItemBox		primary_resource;		// optional
	DataInformationBox	file_locations;		// optional
	ItemLocationBox		item_locations;		// optional
	ItemProtectionBox	protections;			// optional
	ItemInfoBox			item_infos;				// optional
	IPMPControlBox		IPMP_control;			// optional
	ItemReferenceBox		item_refs;					// optional
	ItemDataBox			item_data;					// optional
	Box	other_boxes[];								// optional
}
*/
public partial class MetaBox : FullBox
{
	public const string TYPE = "meta";
	public override string DisplayName { get { return "MetaBox"; } }
	public HandlerBox TheHandler { get { return this.children.OfType<HandlerBox>().FirstOrDefault(); } }
	public PrimaryItemBox PrimaryResource { get { return this.children.OfType<PrimaryItemBox>().FirstOrDefault(); } }
	public DataInformationBox FileLocations { get { return this.children.OfType<DataInformationBox>().FirstOrDefault(); } }
	public ItemLocationBox ItemLocations { get { return this.children.OfType<ItemLocationBox>().FirstOrDefault(); } }
	public ItemProtectionBox Protections { get { return this.children.OfType<ItemProtectionBox>().FirstOrDefault(); } }
	public ItemInfoBox ItemInfos { get { return this.children.OfType<ItemInfoBox>().FirstOrDefault(); } }
	public IPMPControlBox IPMPControl { get { return this.children.OfType<IPMPControlBox>().FirstOrDefault(); } }
	public ItemReferenceBox ItemRefs { get { return this.children.OfType<ItemReferenceBox>().FirstOrDefault(); } }
	public ItemDataBox ItemData { get { return this.children.OfType<ItemDataBox>().FirstOrDefault(); } }
	public IEnumerable<Box> OtherBoxes { get { return this.children.OfType<Box>(); } }
public bool IsQuickTime { get { return (GetParent() == null || (((Box)GetParent()).FourCC == IsoStream.FromFourCC("udta") || ((Box)GetParent()).FourCC == IsoStream.FromFourCC("trak"))); } }

	public MetaBox(uint handler_type = 0): base(IsoStream.FromFourCC("meta"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		if(IsQuickTime) boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.theHandler, "theHandler"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.primary_resource, "primary_resource"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.file_locations, "file_locations"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.item_locations, "item_locations"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.protections, "protections"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.item_infos, "item_infos"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.IPMP_control, "IPMP_control"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.item_refs, "item_refs"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.item_data, "item_data"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.other_boxes, "other_boxes"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		if(IsQuickTime) boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.theHandler, "theHandler"); 
		// boxSize += stream.WriteBox( this.primary_resource, "primary_resource"); // optional
		// boxSize += stream.WriteBox( this.file_locations, "file_locations"); // optional
		// boxSize += stream.WriteBox( this.item_locations, "item_locations"); // optional
		// boxSize += stream.WriteBox( this.protections, "protections"); // optional
		// boxSize += stream.WriteBox( this.item_infos, "item_infos"); // optional
		// boxSize += stream.WriteBox( this.IPMP_control, "IPMP_control"); // optional
		// boxSize += stream.WriteBox( this.item_refs, "item_refs"); // optional
		// boxSize += stream.WriteBox( this.item_data, "item_data"); // optional
		// boxSize += stream.WriteBox( this.other_boxes, "other_boxes"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		if(IsQuickTime) boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(theHandler); // theHandler
		// boxSize += IsoStream.CalculateBoxSize(primary_resource); // primary_resource
		// boxSize += IsoStream.CalculateBoxSize(file_locations); // file_locations
		// boxSize += IsoStream.CalculateBoxSize(item_locations); // item_locations
		// boxSize += IsoStream.CalculateBoxSize(protections); // protections
		// boxSize += IsoStream.CalculateBoxSize(item_infos); // item_infos
		// boxSize += IsoStream.CalculateBoxSize(IPMP_control); // IPMP_control
		// boxSize += IsoStream.CalculateBoxSize(item_refs); // item_refs
		// boxSize += IsoStream.CalculateBoxSize(item_data); // item_data
		// boxSize += IsoStream.CalculateBoxSize(other_boxes); // other_boxes
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
