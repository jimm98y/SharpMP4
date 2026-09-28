using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// ac4_presentation_v1_dsi() (E.10), ac4_presentation_v0_dsi() (TS 103 190-1), then skip_area - as bytes, to be defined
aligned(8) class AC4PresentationDsi() {
 bit(8) presentation_version;
 bit(8) pres_bytes;
 if (pres_bytes == 255) {
  bit(16) add_pres_bytes;
 }
 bit(8) presentation_data[pres_bytes + add_pres_bytes];
}
*/
public partial class AC4PresentationDsi : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AC4PresentationDsi"; } }

	protected byte presentation_version; 
	public byte PresentationVersion { get { return this.presentation_version; } set { this.presentation_version = value; } }

	protected byte pres_bytes; 
	public byte PresBytes { get { return this.pres_bytes; } set { this.pres_bytes = value; } }

	protected ushort add_pres_bytes; 
	public ushort AddPresBytes { get { return this.add_pres_bytes; } set { this.add_pres_bytes = value; } }

	protected byte[] presentation_data; 
	public byte[] PresentationData { get { return this.presentation_data; } set { this.presentation_data = value; } }

	public AC4PresentationDsi(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.presentation_version, "presentation_version"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.pres_bytes, "pres_bytes"); 

		if (pres_bytes == 255)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.add_pres_bytes, "add_pres_bytes"); 
		}
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(pres_bytes + add_pres_bytes),  out this.presentation_data, "presentation_data"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt8( this.presentation_version, "presentation_version"); 
		boxSize += stream.WriteUInt8( this.pres_bytes, "pres_bytes"); 

		if (pres_bytes == 255)
		{
			boxSize += stream.WriteUInt16( this.add_pres_bytes, "add_pres_bytes"); 
		}
		boxSize += stream.WriteUInt8Array((uint)(pres_bytes + add_pres_bytes),  this.presentation_data, "presentation_data"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 8; // presentation_version
		boxSize += 8; // pres_bytes

		if (pres_bytes == 255)
		{
			boxSize += 16; // add_pres_bytes
		}
		boxSize += ((ulong)(pres_bytes + add_pres_bytes) * 8); // presentation_data
		return boxSize;
	}
}

}
