using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class ICCProfileHeader {
	unsigned int(32) profile_size;
	unsigned int(32) preferred_cmm_type;
	unsigned int(8) profile_version_major;
	unsigned int(4) profile_version_minor;
	unsigned int(4) profile_version_bugfix;
	unsigned int(16) reserved = 0;
	unsigned int(32) profile_device_class;	// 'scnr', 'mntr', 'prtr', 'link', 'spac', 'abst', 'nmcl'
	unsigned int(32) colour_space;	// 'XYZ ', 'Lab ', 'RGB ', 'GRAY', 'CMYK'...
	unsigned int(32) profile_connection_space;	// 'XYZ ', 'Lab '
	unsigned int(16) year;
	unsigned int(16) month;
	unsigned int(16) day;
	unsigned int(16) hours;
	unsigned int(16) minutes;
	unsigned int(16) seconds;
	unsigned int(32) profile_file_signature;	// 'acsp'
	unsigned int(32) primary_platform;	// 'APPL', 'MSFT', 'SGI ', 'SUNW'
	unsigned int(32) profile_flags;
	unsigned int(32) device_manufacturer;
	unsigned int(32) device_model;
	unsigned int(64) device_attributes;
	unsigned int(32) rendering_intent;
	signed int(32) pcs_illuminant_x;	// s15Fixed16Number
	signed int(32) pcs_illuminant_y;
	signed int(32) pcs_illuminant_z;
	unsigned int(32) profile_creator;
	unsigned int(8) profile_id[16];	// MD5
	unsigned int(8) reserved[28];
}
*/
public partial class ICCProfileHeader : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public virtual string Syntax { get { return @"aligned(8) class ICCProfileHeader {
	unsigned int(32) profile_size;
	unsigned int(32) preferred_cmm_type;
	unsigned int(8) profile_version_major;
	unsigned int(4) profile_version_minor;
	unsigned int(4) profile_version_bugfix;
	unsigned int(16) reserved = 0;
	unsigned int(32) profile_device_class;	// 'scnr', 'mntr', 'prtr', 'link', 'spac', 'abst', 'nmcl'
	unsigned int(32) colour_space;	// 'XYZ ', 'Lab ', 'RGB ', 'GRAY', 'CMYK'...
	unsigned int(32) profile_connection_space;	// 'XYZ ', 'Lab '
	unsigned int(16) year;
	unsigned int(16) month;
	unsigned int(16) day;
	unsigned int(16) hours;
	unsigned int(16) minutes;
	unsigned int(16) seconds;
	unsigned int(32) profile_file_signature;	// 'acsp'
	unsigned int(32) primary_platform;	// 'APPL', 'MSFT', 'SGI ', 'SUNW'
	unsigned int(32) profile_flags;
	unsigned int(32) device_manufacturer;
	unsigned int(32) device_model;
	unsigned int(64) device_attributes;
	unsigned int(32) rendering_intent;
	signed int(32) pcs_illuminant_x;	// s15Fixed16Number
	signed int(32) pcs_illuminant_y;
	signed int(32) pcs_illuminant_z;
	unsigned int(32) profile_creator;
	unsigned int(8) profile_id[16];	// MD5
	unsigned int(8) reserved[28];
}"; } }
	public virtual string DisplayName { get { return "ICCProfileHeader"; } }

	protected uint profile_size; 
	public uint ProfileSize { get { return this.profile_size; } set { this.profile_size = value; } }

	protected uint preferred_cmm_type; 
	public uint PreferredCmmType { get { return this.preferred_cmm_type; } set { this.preferred_cmm_type = value; } }

	protected byte profile_version_major; 
	public byte ProfileVersionMajor { get { return this.profile_version_major; } set { this.profile_version_major = value; } }

	protected byte profile_version_minor; 
	public byte ProfileVersionMinor { get { return this.profile_version_minor; } set { this.profile_version_minor = value; } }

	protected byte profile_version_bugfix; 
	public byte ProfileVersionBugfix { get { return this.profile_version_bugfix; } set { this.profile_version_bugfix = value; } }

	protected ushort reserved = 0; 
	public ushort Reserved { get { return this.reserved; } set { this.reserved = value; } }

	protected uint profile_device_class;  //  'scnr', 'mntr', 'prtr', 'link', 'spac', 'abst', 'nmcl'
	public uint ProfileDeviceClass { get { return this.profile_device_class; } set { this.profile_device_class = value; } }

	protected uint colour_space;  //  'XYZ ', 'Lab ', 'RGB ', 'GRAY', 'CMYK'...
	public uint ColourSpace { get { return this.colour_space; } set { this.colour_space = value; } }

	protected uint profile_connection_space;  //  'XYZ ', 'Lab '
	public uint ProfileConnectionSpace { get { return this.profile_connection_space; } set { this.profile_connection_space = value; } }

	protected ushort year; 
	public ushort Year { get { return this.year; } set { this.year = value; } }

	protected ushort month; 
	public ushort Month { get { return this.month; } set { this.month = value; } }

	protected ushort day; 
	public ushort Day { get { return this.day; } set { this.day = value; } }

	protected ushort hours; 
	public ushort Hours { get { return this.hours; } set { this.hours = value; } }

	protected ushort minutes; 
	public ushort Minutes { get { return this.minutes; } set { this.minutes = value; } }

	protected ushort seconds; 
	public ushort Seconds { get { return this.seconds; } set { this.seconds = value; } }

	protected uint profile_file_signature;  //  'acsp'
	public uint ProfileFileSignature { get { return this.profile_file_signature; } set { this.profile_file_signature = value; } }

	protected uint primary_platform;  //  'APPL', 'MSFT', 'SGI ', 'SUNW'
	public uint PrimaryPlatform { get { return this.primary_platform; } set { this.primary_platform = value; } }

	protected uint profile_flags; 
	public uint ProfileFlags { get { return this.profile_flags; } set { this.profile_flags = value; } }

	protected uint device_manufacturer; 
	public uint DeviceManufacturer { get { return this.device_manufacturer; } set { this.device_manufacturer = value; } }

	protected uint device_model; 
	public uint DeviceModel { get { return this.device_model; } set { this.device_model = value; } }

	protected ulong device_attributes; 
	public ulong DeviceAttributes { get { return this.device_attributes; } set { this.device_attributes = value; } }

	protected uint rendering_intent; 
	public uint RenderingIntent { get { return this.rendering_intent; } set { this.rendering_intent = value; } }

	protected int pcs_illuminant_x;  //  s15Fixed16Number
	public int PcsIlluminantx { get { return this.pcs_illuminant_x; } set { this.pcs_illuminant_x = value; } }

	protected int pcs_illuminant_y; 
	public int PcsIlluminanty { get { return this.pcs_illuminant_y; } set { this.pcs_illuminant_y = value; } }

	protected int pcs_illuminant_z; 
	public int PcsIlluminantz { get { return this.pcs_illuminant_z; } set { this.pcs_illuminant_z = value; } }

	protected uint profile_creator; 
	public uint ProfileCreator { get { return this.profile_creator; } set { this.profile_creator = value; } }

	protected byte[] profile_id = new byte[16];  //  MD5
	public byte[] ProfileId { get { return this.profile_id; } set { this.profile_id = value; } }

	protected byte[] reserved0 = new byte[28]; 
	public byte[] Reserved0 { get { return this.reserved0; } set { this.reserved0 = value; } }

	public ICCProfileHeader(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile_size, "profile_size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.preferred_cmm_type, "preferred_cmm_type"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.profile_version_major, "profile_version_major"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.profile_version_minor, "profile_version_minor"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.profile_version_bugfix, "profile_version_bugfix"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.reserved, "reserved"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile_device_class, "profile_device_class"); // 'scnr', 'mntr', 'prtr', 'link', 'spac', 'abst', 'nmcl'
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.colour_space, "colour_space"); // 'XYZ ', 'Lab ', 'RGB ', 'GRAY', 'CMYK'...
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile_connection_space, "profile_connection_space"); // 'XYZ ', 'Lab '
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.year, "year"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.month, "month"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.day, "day"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.hours, "hours"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.minutes, "minutes"); 
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.seconds, "seconds"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile_file_signature, "profile_file_signature"); // 'acsp'
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.primary_platform, "primary_platform"); // 'APPL', 'MSFT', 'SGI ', 'SUNW'
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile_flags, "profile_flags"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.device_manufacturer, "device_manufacturer"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.device_model, "device_model"); 
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.device_attributes, "device_attributes"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.rendering_intent, "rendering_intent"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.pcs_illuminant_x, "pcs_illuminant_x"); // s15Fixed16Number
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.pcs_illuminant_y, "pcs_illuminant_y"); 
		boxSize += stream.ReadInt32(boxSize, readSize,  out this.pcs_illuminant_z, "pcs_illuminant_z"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.profile_creator, "profile_creator"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.profile_id, "profile_id"); // MD5
		boxSize += stream.ReadUInt8Array(boxSize, readSize, 28,  out this.reserved0, "reserved0"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.profile_size, "profile_size"); 
		boxSize += stream.WriteUInt32( this.preferred_cmm_type, "preferred_cmm_type"); 
		boxSize += stream.WriteUInt8( this.profile_version_major, "profile_version_major"); 
		boxSize += stream.WriteBits(4,  this.profile_version_minor, "profile_version_minor"); 
		boxSize += stream.WriteBits(4,  this.profile_version_bugfix, "profile_version_bugfix"); 
		boxSize += stream.WriteUInt16( this.reserved, "reserved"); 
		boxSize += stream.WriteUInt32( this.profile_device_class, "profile_device_class"); // 'scnr', 'mntr', 'prtr', 'link', 'spac', 'abst', 'nmcl'
		boxSize += stream.WriteUInt32( this.colour_space, "colour_space"); // 'XYZ ', 'Lab ', 'RGB ', 'GRAY', 'CMYK'...
		boxSize += stream.WriteUInt32( this.profile_connection_space, "profile_connection_space"); // 'XYZ ', 'Lab '
		boxSize += stream.WriteUInt16( this.year, "year"); 
		boxSize += stream.WriteUInt16( this.month, "month"); 
		boxSize += stream.WriteUInt16( this.day, "day"); 
		boxSize += stream.WriteUInt16( this.hours, "hours"); 
		boxSize += stream.WriteUInt16( this.minutes, "minutes"); 
		boxSize += stream.WriteUInt16( this.seconds, "seconds"); 
		boxSize += stream.WriteUInt32( this.profile_file_signature, "profile_file_signature"); // 'acsp'
		boxSize += stream.WriteUInt32( this.primary_platform, "primary_platform"); // 'APPL', 'MSFT', 'SGI ', 'SUNW'
		boxSize += stream.WriteUInt32( this.profile_flags, "profile_flags"); 
		boxSize += stream.WriteUInt32( this.device_manufacturer, "device_manufacturer"); 
		boxSize += stream.WriteUInt32( this.device_model, "device_model"); 
		boxSize += stream.WriteUInt64( this.device_attributes, "device_attributes"); 
		boxSize += stream.WriteUInt32( this.rendering_intent, "rendering_intent"); 
		boxSize += stream.WriteInt32( this.pcs_illuminant_x, "pcs_illuminant_x"); // s15Fixed16Number
		boxSize += stream.WriteInt32( this.pcs_illuminant_y, "pcs_illuminant_y"); 
		boxSize += stream.WriteInt32( this.pcs_illuminant_z, "pcs_illuminant_z"); 
		boxSize += stream.WriteUInt32( this.profile_creator, "profile_creator"); 
		boxSize += stream.WriteUInt8Array(16,  this.profile_id, "profile_id"); // MD5
		boxSize += stream.WriteUInt8Array(28,  this.reserved0, "reserved0"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // profile_size
		boxSize += 32; // preferred_cmm_type
		boxSize += 8; // profile_version_major
		boxSize += 4; // profile_version_minor
		boxSize += 4; // profile_version_bugfix
		boxSize += 16; // reserved
		boxSize += 32; // profile_device_class
		boxSize += 32; // colour_space
		boxSize += 32; // profile_connection_space
		boxSize += 16; // year
		boxSize += 16; // month
		boxSize += 16; // day
		boxSize += 16; // hours
		boxSize += 16; // minutes
		boxSize += 16; // seconds
		boxSize += 32; // profile_file_signature
		boxSize += 32; // primary_platform
		boxSize += 32; // profile_flags
		boxSize += 32; // device_manufacturer
		boxSize += 32; // device_model
		boxSize += 64; // device_attributes
		boxSize += 32; // rendering_intent
		boxSize += 32; // pcs_illuminant_x
		boxSize += 32; // pcs_illuminant_y
		boxSize += 32; // pcs_illuminant_z
		boxSize += 32; // profile_creator
		boxSize += 16 * 8; // profile_id
		boxSize += 28 * 8; // reserved0
		return boxSize;
	}

	public virtual ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += tracer.Field("profile_size", 32, this.profile_size); 
		boxSize += tracer.Field("preferred_cmm_type", 32, this.preferred_cmm_type); 
		boxSize += tracer.Field("profile_version_major", 8, this.profile_version_major); 
		boxSize += tracer.Field("profile_version_minor", 4, this.profile_version_minor); 
		boxSize += tracer.Field("profile_version_bugfix", 4, this.profile_version_bugfix); 
		boxSize += tracer.Field("reserved", 16, this.reserved); 
		boxSize += tracer.Field("profile_device_class", 32, this.profile_device_class); // 'scnr', 'mntr', 'prtr', 'link', 'spac', 'abst', 'nmcl'
		boxSize += tracer.Field("colour_space", 32, this.colour_space); // 'XYZ ', 'Lab ', 'RGB ', 'GRAY', 'CMYK'...
		boxSize += tracer.Field("profile_connection_space", 32, this.profile_connection_space); // 'XYZ ', 'Lab '
		boxSize += tracer.Field("year", 16, this.year); 
		boxSize += tracer.Field("month", 16, this.month); 
		boxSize += tracer.Field("day", 16, this.day); 
		boxSize += tracer.Field("hours", 16, this.hours); 
		boxSize += tracer.Field("minutes", 16, this.minutes); 
		boxSize += tracer.Field("seconds", 16, this.seconds); 
		boxSize += tracer.Field("profile_file_signature", 32, this.profile_file_signature); // 'acsp'
		boxSize += tracer.Field("primary_platform", 32, this.primary_platform); // 'APPL', 'MSFT', 'SGI ', 'SUNW'
		boxSize += tracer.Field("profile_flags", 32, this.profile_flags); 
		boxSize += tracer.Field("device_manufacturer", 32, this.device_manufacturer); 
		boxSize += tracer.Field("device_model", 32, this.device_model); 
		boxSize += tracer.Field("device_attributes", 64, this.device_attributes); 
		boxSize += tracer.Field("rendering_intent", 32, this.rendering_intent); 
		boxSize += tracer.Field("pcs_illuminant_x", 32, this.pcs_illuminant_x); // s15Fixed16Number
		boxSize += tracer.Field("pcs_illuminant_y", 32, this.pcs_illuminant_y); 
		boxSize += tracer.Field("pcs_illuminant_z", 32, this.pcs_illuminant_z); 
		boxSize += tracer.Field("profile_creator", 32, this.profile_creator); 
		boxSize += tracer.Field("profile_id", 16 * 8, this.profile_id); // MD5
		boxSize += tracer.Field("reserved0", 28 * 8, this.reserved0); 
		return boxSize;
	}
}

}
