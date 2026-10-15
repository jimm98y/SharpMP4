using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class AVCMVCSampleEntry() extends AVCSampleEntry ('avc1') {
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
public partial class AVCMVCSampleEntry : AVCSampleEntry
{
	public const string TYPE = "avc1";
	public override string DisplayName { get { return "AVCMVCSampleEntry"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public ViewScalabilityInformationSEIBox Scalability { get { return this.children.OfType<ViewScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public ViewIdentifierBox ViewIdentifiers { get { return this.children.OfType<ViewIdentifierBox>().FirstOrDefault(); } }
	public MVCConfigurationBox Mvcconfig { get { return this.children.OfType<MVCConfigurationBox>().FirstOrDefault(); } }
	public MVCViewPriorityAssignmentBox ViewPriorityMethod { get { return this.children.OfType<MVCViewPriorityAssignmentBox>().FirstOrDefault(); } }
	public IntrinsicCameraParametersBox IntrinsicCameraParams { get { return this.children.OfType<IntrinsicCameraParametersBox>().FirstOrDefault(); } }
	public ExtrinsicCameraParametersBox ExtrinsicCameraParams { get { return this.children.OfType<ExtrinsicCameraParametersBox>().FirstOrDefault(); } }
	public MVCDConfigurationBox Mvcdconfig { get { return this.children.OfType<MVCDConfigurationBox>().FirstOrDefault(); } }
	public MVDScalabilityInformationSEIBox Mvdscalinfosei { get { return this.children.OfType<MVDScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public A3DConfigurationBox A3dconfig { get { return this.children.OfType<A3DConfigurationBox>().FirstOrDefault(); } }

	public AVCMVCSampleEntry(): base(IsoStream.FromFourCC("avc1"))
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

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		// boxSize += builder.Field("scalability", IsoStream.CalculateBoxSize(scalability), this.scalability); // optional
		// boxSize += builder.Field("view_identifiers", IsoStream.CalculateBoxSize(view_identifiers), this.view_identifiers); // optional
		// boxSize += builder.Field("mvcconfig", IsoStream.CalculateBoxSize(mvcconfig), this.mvcconfig); // optional
		// boxSize += builder.Field("view_priority_method", IsoStream.CalculateBoxSize(view_priority_method), this.view_priority_method); // optional
		// boxSize += builder.Field("intrinsic_camera_params", IsoStream.CalculateBoxSize(intrinsic_camera_params), this.intrinsic_camera_params); // optional
		// boxSize += builder.Field("extrinsic_camera_params", IsoStream.CalculateBoxSize(extrinsic_camera_params), this.extrinsic_camera_params); // optional
		// boxSize += builder.Field("mvcdconfig", IsoStream.CalculateBoxSize(mvcdconfig), this.mvcdconfig); // optional
		// boxSize += builder.Field("mvdscalinfosei", IsoStream.CalculateBoxSize(mvdscalinfosei), this.mvdscalinfosei); // optional
		// boxSize += builder.Field("a3dconfig", IsoStream.CalculateBoxSize(a3dconfig), this.a3dconfig); // optional
		boxSize += builder.Children(this);
		return boxSize;
	}
}

}
