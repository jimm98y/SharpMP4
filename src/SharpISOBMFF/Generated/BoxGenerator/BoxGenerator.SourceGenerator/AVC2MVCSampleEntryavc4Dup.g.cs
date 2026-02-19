using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class AVC2MVCSampleEntry() extends AVC2SampleEntry ('avc4') {
	ViewScalabilityInformationSEIBox	scalability;	// optional
	ViewIdentifierBox		view_identifiers;	// optional
	MVCConfigurationBox	mvcconfig;		// optional
	MVCViewPriorityAssignmentBox	view_priority_method;	// optional
	IntrinsicCameraParametersBox	intrinsic_camera_params;	// optional
	ExtrinsicCameraParametersBox	extrinsic_camera_params;	// optional
	MVCDConfigurationBox	mvcdconfig;	// optional
	MVDScalabilityInformationSEIBox	mvdscalinfosei;	// optional
	A3DConfigurationBox	a3dconfig;	// optional
}
*/
public partial class AVC2MVCSampleEntryavc4Dup : AVC2SampleEntry
{
	public const string TYPE = "avc4";
	public override string DisplayName { get { return "AVC2MVCSampleEntryavc4Dup"; } }
	public ViewScalabilityInformationSEIBox Scalability { get { return this.children.OfType<ViewScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public ViewIdentifierBox ViewIdentifiers { get { return this.children.OfType<ViewIdentifierBox>().FirstOrDefault(); } }
	public MVCConfigurationBox Mvcconfig { get { return this.children.OfType<MVCConfigurationBox>().FirstOrDefault(); } }
	public MVCViewPriorityAssignmentBox ViewPriorityMethod { get { return this.children.OfType<MVCViewPriorityAssignmentBox>().FirstOrDefault(); } }
	public IntrinsicCameraParametersBox IntrinsicCameraParams { get { return this.children.OfType<IntrinsicCameraParametersBox>().FirstOrDefault(); } }
	public ExtrinsicCameraParametersBox ExtrinsicCameraParams { get { return this.children.OfType<ExtrinsicCameraParametersBox>().FirstOrDefault(); } }
	public MVCDConfigurationBox Mvcdconfig { get { return this.children.OfType<MVCDConfigurationBox>().FirstOrDefault(); } }
	public MVDScalabilityInformationSEIBox Mvdscalinfosei { get { return this.children.OfType<MVDScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public A3DConfigurationBox A3dconfig { get { return this.children.OfType<A3DConfigurationBox>().FirstOrDefault(); } }

	public AVC2MVCSampleEntryavc4Dup(): base(IsoStream.FromFourCC("avc4"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.scalability, "scalability"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.view_identifiers, "view_identifiers"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.mvcconfig, "mvcconfig"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.view_priority_method, "view_priority_method"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.intrinsic_camera_params, "intrinsic_camera_params"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.extrinsic_camera_params, "extrinsic_camera_params"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.mvcdconfig, "mvcdconfig"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.mvdscalinfosei, "mvdscalinfosei"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.a3dconfig, "a3dconfig"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.scalability, "scalability"); // optional
		// boxSize += stream.WriteBox( this.view_identifiers, "view_identifiers"); // optional
		// boxSize += stream.WriteBox( this.mvcconfig, "mvcconfig"); // optional
		// boxSize += stream.WriteBox( this.view_priority_method, "view_priority_method"); // optional
		// boxSize += stream.WriteBox( this.intrinsic_camera_params, "intrinsic_camera_params"); // optional
		// boxSize += stream.WriteBox( this.extrinsic_camera_params, "extrinsic_camera_params"); // optional
		// boxSize += stream.WriteBox( this.mvcdconfig, "mvcdconfig"); // optional
		// boxSize += stream.WriteBox( this.mvdscalinfosei, "mvdscalinfosei"); // optional
		// boxSize += stream.WriteBox( this.a3dconfig, "a3dconfig"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(scalability); // scalability
		// boxSize += IsoStream.CalculateBoxSize(view_identifiers); // view_identifiers
		// boxSize += IsoStream.CalculateBoxSize(mvcconfig); // mvcconfig
		// boxSize += IsoStream.CalculateBoxSize(view_priority_method); // view_priority_method
		// boxSize += IsoStream.CalculateBoxSize(intrinsic_camera_params); // intrinsic_camera_params
		// boxSize += IsoStream.CalculateBoxSize(extrinsic_camera_params); // extrinsic_camera_params
		// boxSize += IsoStream.CalculateBoxSize(mvcdconfig); // mvcdconfig
		// boxSize += IsoStream.CalculateBoxSize(mvdscalinfosei); // mvdscalinfosei
		// boxSize += IsoStream.CalculateBoxSize(a3dconfig); // a3dconfig
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
