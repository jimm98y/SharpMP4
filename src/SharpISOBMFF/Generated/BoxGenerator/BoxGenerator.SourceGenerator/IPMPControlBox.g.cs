using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class IPMPControlBox() extends FullBox('ipmc') {
 Descriptor toollist;
 unsigned int(8) count; Descriptor descriptor[count];
 }
*/
public partial class IPMPControlBox : FullBox
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class IPMPControlBox() extends FullBox('ipmc') {
 Descriptor toollist;
 unsigned int(8) count; Descriptor descriptor[count];
 }"; } }
	public const string TYPE = "ipmc";
	public override string DisplayName { get { return "IPMPControlBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("moov", "meta"); }


	protected Descriptor toollist; 
	public Descriptor Toollist { get { return this.toollist; } set { this.toollist = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected Descriptor[] descriptor = []; 
	public Descriptor[] Descriptor { get { return this.descriptor; } set { this.descriptor = value; } }

	public IPMPControlBox(): base(IsoStream.FromFourCC("ipmc"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.toollist, "toollist"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteDescriptor( this.toollist, "toollist"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		// boxSize += stream.WriteDescriptor( this.descriptor, "descriptor"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateDescriptorSize(toollist); // toollist
		boxSize += 8; // count
		// boxSize += IsoStream.CalculateDescriptorSize(descriptor); // descriptor
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("toollist", IsoStream.CalculateDescriptorSize(toollist), this.toollist); 
		boxSize += tracer.Field("count", 8, this.count); 
		// boxSize += tracer.Field("descriptor", IsoStream.CalculateDescriptorSize(descriptor), this.descriptor); 
		return boxSize;
	}
}

}
