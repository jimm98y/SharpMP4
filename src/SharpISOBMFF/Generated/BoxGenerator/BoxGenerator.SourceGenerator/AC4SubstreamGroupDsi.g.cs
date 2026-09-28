using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AC4SubstreamGroupDsi() {
 bit(1) b_substreams_present;
 bit(1) b_hsf_ext;
 bit(1) b_channel_coded;
 bit(8) n_substreams;
 for (i = 0; i < n_substreams; i++) {
  bit(2) dsi_sf_multiplier;
  bit(1) b_substream_bitrate_indicator;
  if (b_substream_bitrate_indicator) {
   bit(5) substream_bitrate_indicator;
  }
  if (b_channel_coded) {
   bit(6) reserved_zero;
   bit(18) dsi_substream_channel_groups;
  }
  else {
   bit(1) b_ajoc;
   if (b_ajoc) {
    bit(1) b_static_dmx;
    if (!b_static_dmx) {
     bit(4) n_dmx_objects_minus1;
    }
    bit(6) n_umx_objects_minus1;
   }
   bit(1) b_substream_contains_bed_objects;
   bit(1) b_substream_contains_dynamic_objects;
   bit(1) b_substream_contains_ISF_objects;
   bit(1) reserved;
  }
 }
 bit(1) b_content_type;
 if (b_content_type) {
  bit(3) content_classifier;
  bit(1) b_language_indicator;
  if (b_language_indicator) {
   bit(6) n_language_tag_bytes;
   bit(8) language_tag_bytes[n_language_tag_bytes];
  }
 }
}
// E.12.1, the loop over t counted by i

*/
public partial class AC4SubstreamGroupDsi : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "AC4SubstreamGroupDsi"; } }

	protected bool b_substreams_present; 
	public bool bSubstreamsPresent { get { return this.b_substreams_present; } set { this.b_substreams_present = value; } }

	protected bool b_hsf_ext; 
	public bool bHsfExt { get { return this.b_hsf_ext; } set { this.b_hsf_ext = value; } }

	protected bool b_channel_coded; 
	public bool bChannelCoded { get { return this.b_channel_coded; } set { this.b_channel_coded = value; } }

	protected byte n_substreams; 
	public byte nSubstreams { get { return this.n_substreams; } set { this.n_substreams = value; } }

	protected byte[] dsi_sf_multiplier; 
	public byte[] DsiSfMultiplier { get { return this.dsi_sf_multiplier; } set { this.dsi_sf_multiplier = value; } }

	protected bool[] b_substream_bitrate_indicator; 
	public bool[] bSubstreamBitrateIndicator { get { return this.b_substream_bitrate_indicator; } set { this.b_substream_bitrate_indicator = value; } }

	protected byte[] substream_bitrate_indicator; 
	public byte[] SubstreamBitrateIndicator { get { return this.substream_bitrate_indicator; } set { this.substream_bitrate_indicator = value; } }

	protected byte[] reserved_zero; 
	public byte[] ReservedZero { get { return this.reserved_zero; } set { this.reserved_zero = value; } }

	protected uint[] dsi_substream_channel_groups; 
	public uint[] DsiSubstreamChannelGroups { get { return this.dsi_substream_channel_groups; } set { this.dsi_substream_channel_groups = value; } }

	protected bool[] b_ajoc; 
	public bool[] bAjoc { get { return this.b_ajoc; } set { this.b_ajoc = value; } }

	protected bool[] b_static_dmx; 
	public bool[] bStaticDmx { get { return this.b_static_dmx; } set { this.b_static_dmx = value; } }

	protected byte[] n_dmx_objects_minus1; 
	public byte[] nDmxObjectsMinus1 { get { return this.n_dmx_objects_minus1; } set { this.n_dmx_objects_minus1 = value; } }

	protected byte[] n_umx_objects_minus1; 
	public byte[] nUmxObjectsMinus1 { get { return this.n_umx_objects_minus1; } set { this.n_umx_objects_minus1 = value; } }

	protected bool[] b_substream_contains_bed_objects; 
	public bool[] bSubstreamContainsBedObjects { get { return this.b_substream_contains_bed_objects; } set { this.b_substream_contains_bed_objects = value; } }

	protected bool[] b_substream_contains_dynamic_objects; 
	public bool[] bSubstreamContainsDynamicObjects { get { return this.b_substream_contains_dynamic_objects; } set { this.b_substream_contains_dynamic_objects = value; } }

	protected bool[] b_substream_contains_ISF_objects; 
	public bool[] bSubstreamContainsISFObjects { get { return this.b_substream_contains_ISF_objects; } set { this.b_substream_contains_ISF_objects = value; } }

	protected bool[] reserved; 
	public bool[] Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected bool b_content_type; 
	public bool bContentType { get { return this.b_content_type; } set { this.b_content_type = value; } }

	protected byte content_classifier; 
	public byte ContentClassifier { get { return this.content_classifier; } set { this.content_classifier = value; } }

	protected bool b_language_indicator; 
	public bool bLanguageIndicator { get { return this.b_language_indicator; } set { this.b_language_indicator = value; } }

	protected byte n_language_tag_bytes; 
	public byte nLanguageTagBytes { get { return this.n_language_tag_bytes; } set { this.n_language_tag_bytes = value; } }

	protected byte[] language_tag_bytes; 
	public byte[] LanguageTagBytes { get { return this.language_tag_bytes; } set { this.language_tag_bytes = value; } }

	public AC4SubstreamGroupDsi(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.b_substreams_present, "b_substreams_present"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.b_hsf_ext, "b_hsf_ext"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.b_channel_coded, "b_channel_coded"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.n_substreams, "n_substreams"); 

		this.dsi_sf_multiplier = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_substreams), "dsi_sf_multiplier");
		this.b_substream_bitrate_indicator = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "b_substream_bitrate_indicator");
		this.substream_bitrate_indicator = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_substreams), "substream_bitrate_indicator");
		this.reserved_zero = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_substreams), "reserved_zero");
		this.dsi_substream_channel_groups = stream.SafeAllocate<uint>(boxSize, readSize, IsoStream.GetInt( n_substreams), "dsi_substream_channel_groups");
		this.b_ajoc = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "b_ajoc");
		this.b_static_dmx = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "b_static_dmx");
		this.n_dmx_objects_minus1 = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_substreams), "n_dmx_objects_minus1");
		this.n_umx_objects_minus1 = stream.SafeAllocate<byte>(boxSize, readSize, IsoStream.GetInt( n_substreams), "n_umx_objects_minus1");
		this.b_substream_contains_bed_objects = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "b_substream_contains_bed_objects");
		this.b_substream_contains_dynamic_objects = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "b_substream_contains_dynamic_objects");
		this.b_substream_contains_ISF_objects = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "b_substream_contains_ISF_objects");
		this.reserved = stream.SafeAllocate<bool>(boxSize, readSize, IsoStream.GetInt( n_substreams), "reserved");
		for (int i = 0; i < n_substreams; i++)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.dsi_sf_multiplier[i], "dsi_sf_multiplier"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_substream_bitrate_indicator[i], "b_substream_bitrate_indicator"); 

			if (b_substream_bitrate_indicator[i])
			{
				boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.substream_bitrate_indicator[i], "substream_bitrate_indicator"); 
			}

			if (b_channel_coded)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.reserved_zero[i], "reserved_zero"); 
				boxSize += stream.ReadBits(boxSize, readSize, 18,  out this.dsi_substream_channel_groups[i], "dsi_substream_channel_groups"); 
			}

			else 
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_ajoc[i], "b_ajoc"); 

				if (b_ajoc[i])
				{
					boxSize += stream.ReadBit(boxSize, readSize,  out this.b_static_dmx[i], "b_static_dmx"); 

					if (!b_static_dmx[i])
					{
						boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.n_dmx_objects_minus1[i], "n_dmx_objects_minus1"); 
					}
					boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.n_umx_objects_minus1[i], "n_umx_objects_minus1"); 
				}
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_substream_contains_bed_objects[i], "b_substream_contains_bed_objects"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_substream_contains_dynamic_objects[i], "b_substream_contains_dynamic_objects"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.b_substream_contains_ISF_objects[i], "b_substream_contains_ISF_objects"); 
				boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved[i], "reserved"); 
			}
		}
		boxSize += stream.ReadBit(boxSize, readSize,  out this.b_content_type, "b_content_type"); 

		if (b_content_type)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 3,  out this.content_classifier, "content_classifier"); 
			boxSize += stream.ReadBit(boxSize, readSize,  out this.b_language_indicator, "b_language_indicator"); 

			if (b_language_indicator)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 6,  out this.n_language_tag_bytes, "n_language_tag_bytes"); 
				boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(n_language_tag_bytes),  out this.language_tag_bytes, "language_tag_bytes"); 
			}
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.b_substreams_present, "b_substreams_present"); 
		boxSize += stream.WriteBit( this.b_hsf_ext, "b_hsf_ext"); 
		boxSize += stream.WriteBit( this.b_channel_coded, "b_channel_coded"); 
		boxSize += stream.WriteUInt8( this.n_substreams, "n_substreams"); 

		for (int i = 0; i < n_substreams; i++)
		{
			boxSize += stream.WriteBits(2,  this.dsi_sf_multiplier[i], "dsi_sf_multiplier"); 
			boxSize += stream.WriteBit( this.b_substream_bitrate_indicator[i], "b_substream_bitrate_indicator"); 

			if (b_substream_bitrate_indicator[i])
			{
				boxSize += stream.WriteBits(5,  this.substream_bitrate_indicator[i], "substream_bitrate_indicator"); 
			}

			if (b_channel_coded)
			{
				boxSize += stream.WriteBits(6,  this.reserved_zero[i], "reserved_zero"); 
				boxSize += stream.WriteBits(18,  this.dsi_substream_channel_groups[i], "dsi_substream_channel_groups"); 
			}

			else 
			{
				boxSize += stream.WriteBit( this.b_ajoc[i], "b_ajoc"); 

				if (b_ajoc[i])
				{
					boxSize += stream.WriteBit( this.b_static_dmx[i], "b_static_dmx"); 

					if (!b_static_dmx[i])
					{
						boxSize += stream.WriteBits(4,  this.n_dmx_objects_minus1[i], "n_dmx_objects_minus1"); 
					}
					boxSize += stream.WriteBits(6,  this.n_umx_objects_minus1[i], "n_umx_objects_minus1"); 
				}
				boxSize += stream.WriteBit( this.b_substream_contains_bed_objects[i], "b_substream_contains_bed_objects"); 
				boxSize += stream.WriteBit( this.b_substream_contains_dynamic_objects[i], "b_substream_contains_dynamic_objects"); 
				boxSize += stream.WriteBit( this.b_substream_contains_ISF_objects[i], "b_substream_contains_ISF_objects"); 
				boxSize += stream.WriteBit( this.reserved[i], "reserved"); 
			}
		}
		boxSize += stream.WriteBit( this.b_content_type, "b_content_type"); 

		if (b_content_type)
		{
			boxSize += stream.WriteBits(3,  this.content_classifier, "content_classifier"); 
			boxSize += stream.WriteBit( this.b_language_indicator, "b_language_indicator"); 

			if (b_language_indicator)
			{
				boxSize += stream.WriteBits(6,  this.n_language_tag_bytes, "n_language_tag_bytes"); 
				boxSize += stream.WriteUInt8Array((uint)(n_language_tag_bytes),  this.language_tag_bytes, "language_tag_bytes"); 
			}
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // b_substreams_present
		boxSize += 1; // b_hsf_ext
		boxSize += 1; // b_channel_coded
		boxSize += 8; // n_substreams

		for (int i = 0; i < n_substreams; i++)
		{
			boxSize += 2; // dsi_sf_multiplier
			boxSize += 1; // b_substream_bitrate_indicator

			if (b_substream_bitrate_indicator[i])
			{
				boxSize += 5; // substream_bitrate_indicator
			}

			if (b_channel_coded)
			{
				boxSize += 6; // reserved_zero
				boxSize += 18; // dsi_substream_channel_groups
			}

			else 
			{
				boxSize += 1; // b_ajoc

				if (b_ajoc[i])
				{
					boxSize += 1; // b_static_dmx

					if (!b_static_dmx[i])
					{
						boxSize += 4; // n_dmx_objects_minus1
					}
					boxSize += 6; // n_umx_objects_minus1
				}
				boxSize += 1; // b_substream_contains_bed_objects
				boxSize += 1; // b_substream_contains_dynamic_objects
				boxSize += 1; // b_substream_contains_ISF_objects
				boxSize += 1; // reserved
			}
		}
		boxSize += 1; // b_content_type

		if (b_content_type)
		{
			boxSize += 3; // content_classifier
			boxSize += 1; // b_language_indicator

			if (b_language_indicator)
			{
				boxSize += 6; // n_language_tag_bytes
				boxSize += ((ulong)(n_language_tag_bytes) * 8); // language_tag_bytes
			}
		}
		return boxSize;
	}
}

}
