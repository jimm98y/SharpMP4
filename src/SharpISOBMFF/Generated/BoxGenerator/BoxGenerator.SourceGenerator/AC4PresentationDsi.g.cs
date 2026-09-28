using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// is ac4_presentation_v1_dsi() (E.10); version 0's (ETSI TS 103 190-1, not at hand) and any other's are bytes
aligned(8) class AC4PresentationDsi() {
 bit(8) presentation_version;
 bit(8) pres_bytes;
 if (pres_bytes == 255) {
  bit(16) add_pres_bytes;
 }
 if (presentation_version == 1) {
  AC4PresentationV1Dsi(pres_bytes + add_pres_bytes) presentation_v1;
 }
 else {
  bit(8) presentation_data[pres_bytes + add_pres_bytes];
 }
}
// E.10.1: in [a, b] written out, the loops over sg counted by i, b_add_emdf_substreams = 1 of config 0x06 in the condition; the spec's dsi_presentation_channel_mode is its dsi_presentation_ch_mode.

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

	protected AC4PresentationV1Dsi presentation_v1; 
	public AC4PresentationV1Dsi PresentationV1 { get { return this.presentation_v1; } set { this.presentation_v1 = value; } }

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

		if (presentation_version == 1)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new AC4PresentationV1Dsi(pres_bytes + add_pres_bytes),  out this.presentation_v1, "presentation_v1"); 
		}

		else 
		{
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(pres_bytes + add_pres_bytes),  out this.presentation_data, "presentation_data"); 
		}
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

		if (presentation_version == 1)
		{
			boxSize += stream.WriteClass( this.presentation_v1, "presentation_v1"); 
		}

		else 
		{
			boxSize += stream.WriteUInt8Array((uint)(pres_bytes + add_pres_bytes),  this.presentation_data, "presentation_data"); 
		}
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

		if (presentation_version == 1)
		{
			boxSize += IsoStream.CalculateClassSize(presentation_v1); // presentation_v1
		}

		else 
		{
			boxSize += ((ulong)(pres_bytes + add_pres_bytes) * 8); // presentation_data
		}
		return boxSize;
	}
}

}
