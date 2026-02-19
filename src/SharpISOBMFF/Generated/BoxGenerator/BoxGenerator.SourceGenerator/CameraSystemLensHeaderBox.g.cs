using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class CameraSystemLensHeaderBox extends FullBox('lnhd', 0, 0) { 
 unsigned int(32) lens_identifier; // an integer unique to the enclosing CameraSystemLensBox 
 unsigned int(32) lens_algorithm_kind; // a FourCC for the kind of projection 
 unsigned int(32) lens_domain; // a FourCC for the kind of lens (e.g., color) 
 unsigned int(32) lens_role; // a FourCC indicating which lens this is (e.g., left or right for a stereo system) 
}
*/
public partial class CameraSystemLensHeaderBox : FullBox
{
	public const string TYPE = "lnhd";
	public override string DisplayName { get { return "CameraSystemLensHeaderBox"; } }

	protected uint lens_identifier;  //  an integer unique to the enclosing CameraSystemLensBox 
	public uint LensIdentifier { get { return this.lens_identifier; } set { this.lens_identifier = value; } }

	protected uint lens_algorithm_kind;  //  a FourCC for the kind of projection 
	public uint LensAlgorithmKind { get { return this.lens_algorithm_kind; } set { this.lens_algorithm_kind = value; } }

	protected uint lens_domain;  //  a FourCC for the kind of lens (e.g., color) 
	public uint LensDomain { get { return this.lens_domain; } set { this.lens_domain = value; } }

	protected uint lens_role;  //  a FourCC indicating which lens this is (e.g., left or right for a stereo system) 
	public uint LensRole { get { return this.lens_role; } set { this.lens_role = value; } }

	public CameraSystemLensHeaderBox(): base(IsoStream.FromFourCC("lnhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.lens_identifier, "lens_identifier"); // an integer unique to the enclosing CameraSystemLensBox 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.lens_algorithm_kind, "lens_algorithm_kind"); // a FourCC for the kind of projection 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.lens_domain, "lens_domain"); // a FourCC for the kind of lens (e.g., color) 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.lens_role, "lens_role"); // a FourCC indicating which lens this is (e.g., left or right for a stereo system) 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.lens_identifier, "lens_identifier"); // an integer unique to the enclosing CameraSystemLensBox 
		boxSize += stream.WriteUInt32( this.lens_algorithm_kind, "lens_algorithm_kind"); // a FourCC for the kind of projection 
		boxSize += stream.WriteUInt32( this.lens_domain, "lens_domain"); // a FourCC for the kind of lens (e.g., color) 
		boxSize += stream.WriteUInt32( this.lens_role, "lens_role"); // a FourCC indicating which lens this is (e.g., left or right for a stereo system) 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // lens_identifier
		boxSize += 32; // lens_algorithm_kind
		boxSize += 32; // lens_domain
		boxSize += 32; // lens_role
		return boxSize;
	}
}

}
