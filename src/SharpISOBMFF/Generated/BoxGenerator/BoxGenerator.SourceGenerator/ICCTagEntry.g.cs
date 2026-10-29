using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCTagEntry {
	unsigned int(32) tag_signature;	// 'desc', 'cprt', 'wtpt', 'rXYZ', 'rTRC'...
	unsigned int(32) offset;	// of the tag's element, from the start of the profile
	unsigned int(32) size;	// of the tag's element
}
*/
public partial class ICCTagEntry : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCTagEntry {
	unsigned int(32) tag_signature;	// 'desc', 'cprt', 'wtpt', 'rXYZ', 'rTRC'...
	unsigned int(32) offset;	// of the tag's element, from the start of the profile
	unsigned int(32) size;	// of the tag's element
}"; } }
	public virtual string DisplayName { get { return "ICCTagEntry"; } }

	protected uint tag_signature;  //  'desc', 'cprt', 'wtpt', 'rXYZ', 'rTRC'...
	public uint TagSignature { get { return this.tag_signature; } set { this.tag_signature = value; } }

	protected uint offset;  //  of the tag's element, from the start of the profile
	public uint Offset { get { return this.offset; } set { this.offset = value; } }

	protected uint size;  //  of the tag's element
	public uint Size { get { return this.size; } set { this.size = value; } }

	public ICCTagEntry(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.tag_signature, "tag_signature"); // 'desc', 'cprt', 'wtpt', 'rXYZ', 'rTRC'...
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.offset, "offset"); // of the tag's element, from the start of the profile
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.size, "size"); // of the tag's element
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.tag_signature, "tag_signature"); // 'desc', 'cprt', 'wtpt', 'rXYZ', 'rTRC'...
		boxSize += stream.WriteUInt32( this.offset, "offset"); // of the tag's element, from the start of the profile
		boxSize += stream.WriteUInt32( this.size, "size"); // of the tag's element
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // tag_signature
		boxSize += 32; // offset
		boxSize += 32; // size
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("tag_signature", 32, this.tag_signature); // 'desc', 'cprt', 'wtpt', 'rXYZ', 'rTRC'...
		boxSize += tracer.Field("offset", 32, this.offset); // of the tag's element, from the start of the profile
		boxSize += tracer.Field("size", 32, this.size); // of the tag's element
		return boxSize;
	}
}

}
