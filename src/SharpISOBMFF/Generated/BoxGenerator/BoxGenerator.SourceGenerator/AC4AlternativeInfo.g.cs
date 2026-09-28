using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AC4AlternativeInfo() {
 bit(16) name_len;
 bit(8) presentation_name[name_len];
 bit(5) n_targets;
 for (i = 0; i < n_targets; i++) {
  bit(3) target_md_compat;
  bit(8) target_device_category;
 }
}
*/
public partial class AC4AlternativeInfo : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AC4AlternativeInfo"; } }

	protected ushort name_len; 
	public ushort NameLen { get { return this.name_len; } set { this.name_len = value; } }

	protected byte[] presentation_name; 
	public byte[] PresentationName { get { return this.presentation_name; } set { this.presentation_name = value; } }

	protected byte n_targets; 
	public byte nTargets { get { return this.n_targets; } set { this.n_targets = value; } }

	protected byte[] target_md_compat; 
	public byte[] TargetMdCompat { get { return this.target_md_compat; } set { this.target_md_compat = value; } }

	protected byte[] target_device_category; 
	public byte[] TargetDeviceCategory { get { return this.target_device_category; } set { this.target_device_category = value; } }

	public AC4AlternativeInfo(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.name_len, "name_len"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(name_len),  out this.presentation_name, "presentation_name"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.n_targets, "n_targets"); 

		this.target_md_compat = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_targets), "target_md_compat");
		this.target_device_category = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_targets), "target_device_category");
		for (int i = 0; i < n_targets; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.target_md_compat[i], "target_md_compat"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.target_device_category[i], "target_device_category"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt16( this.name_len, "name_len"); 
		boxSize += stream.WriteUInt8Array((uint)(name_len),  this.presentation_name, "presentation_name"); 
		boxSize += stream.WriteBits(5,  this.n_targets, "n_targets"); 

		for (int i = 0; i < n_targets; i++)
		{
			boxSize += stream.WriteBits(3,  this.target_md_compat[i], "target_md_compat"); 
			boxSize += stream.WriteUInt8( this.target_device_category[i], "target_device_category"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 16; // name_len
		boxSize += ((ulong)(name_len) * 8); // presentation_name
		boxSize += 5; // n_targets

		for (int i = 0; i < n_targets; i++)
		{
			boxSize += 3; // target_md_compat
			boxSize += 8; // target_device_category
		}
		return boxSize;
	}
}

}
