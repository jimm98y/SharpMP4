using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class MVCDSampleEntry() extends VisualSampleEntry ('mvd3') {
	MVCDConfigurationBox	mvcdconfig;		// mandatory
	MVDScalabilityInformationSEIBox	mvdscalinfosei;	// optional
	ViewIdentifierBox	view_identifiers;		// mandatory
	MPEG4ExtensionDescriptorsBox descr;		// optional
	IntrinsicCameraParametersBox	intrinsic_camera_params;	// optional
	ExtrinsicCameraParametersBox	extrinsic_camera_params;	// optional
	A3DConfigurationBox	a3dconfig;	// optional
}
*/
public partial class MVCDSampleEntrymvd3Dup : VisualSampleEntry
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class MVCDSampleEntry() extends VisualSampleEntry ('mvd3') {
	MVCDConfigurationBox	mvcdconfig;		// mandatory
	MVDScalabilityInformationSEIBox	mvdscalinfosei;	// optional
	ViewIdentifierBox	view_identifiers;		// mandatory
	MPEG4ExtensionDescriptorsBox descr;		// optional
	IntrinsicCameraParametersBox	intrinsic_camera_params;	// optional
	ExtrinsicCameraParametersBox	extrinsic_camera_params;	// optional
	A3DConfigurationBox	a3dconfig;	// optional
}"; } }
	public const string TYPE = "mvd3";
	public override string DisplayName { get { return "MVCDSampleEntrymvd3Dup"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("stsd"); }

	public MVCDConfigurationBox Mvcdconfig { get { return this.children.OfType<MVCDConfigurationBox>().FirstOrDefault(); } }
	public MVDScalabilityInformationSEIBox Mvdscalinfosei { get { return this.children.OfType<MVDScalabilityInformationSEIBox>().FirstOrDefault(); } }
	public ViewIdentifierBox ViewIdentifiers { get { return this.children.OfType<ViewIdentifierBox>().FirstOrDefault(); } }
	public MPEG4ExtensionDescriptorsBox Descr { get { return this.children.OfType<MPEG4ExtensionDescriptorsBox>().FirstOrDefault(); } }
	public IntrinsicCameraParametersBox IntrinsicCameraParams { get { return this.children.OfType<IntrinsicCameraParametersBox>().FirstOrDefault(); } }
	public ExtrinsicCameraParametersBox ExtrinsicCameraParams { get { return this.children.OfType<ExtrinsicCameraParametersBox>().FirstOrDefault(); } }
	public A3DConfigurationBox A3dconfig { get { return this.children.OfType<A3DConfigurationBox>().FirstOrDefault(); } }

	public MVCDSampleEntrymvd3Dup(): base(IsoStream.FromFourCC("mvd3"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.mvcdconfig, "mvcdconfig"); // mandatory
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.mvdscalinfosei, "mvdscalinfosei"); // optional
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.view_identifiers, "view_identifiers"); // mandatory
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.descr, "descr"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.intrinsic_camera_params, "intrinsic_camera_params"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.extrinsic_camera_params, "extrinsic_camera_params"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.a3dconfig, "a3dconfig"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.mvcdconfig, "mvcdconfig"); // mandatory
		// boxSize += stream.WriteBox( this.mvdscalinfosei, "mvdscalinfosei"); // optional
		// boxSize += stream.WriteBox( this.view_identifiers, "view_identifiers"); // mandatory
		// boxSize += stream.WriteBox( this.descr, "descr"); // optional
		// boxSize += stream.WriteBox( this.intrinsic_camera_params, "intrinsic_camera_params"); // optional
		// boxSize += stream.WriteBox( this.extrinsic_camera_params, "extrinsic_camera_params"); // optional
		// boxSize += stream.WriteBox( this.a3dconfig, "a3dconfig"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(mvcdconfig); // mvcdconfig
		// boxSize += IsoStream.CalculateBoxSize(mvdscalinfosei); // mvdscalinfosei
		// boxSize += IsoStream.CalculateBoxSize(view_identifiers); // view_identifiers
		// boxSize += IsoStream.CalculateBoxSize(descr); // descr
		// boxSize += IsoStream.CalculateBoxSize(intrinsic_camera_params); // intrinsic_camera_params
		// boxSize += IsoStream.CalculateBoxSize(extrinsic_camera_params); // extrinsic_camera_params
		// boxSize += IsoStream.CalculateBoxSize(a3dconfig); // a3dconfig
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("mvcdconfig", IsoStream.CalculateBoxSize(mvcdconfig), this.mvcdconfig); // mandatory
		// boxSize += tracer.Field("mvdscalinfosei", IsoStream.CalculateBoxSize(mvdscalinfosei), this.mvdscalinfosei); // optional
		// boxSize += tracer.Field("view_identifiers", IsoStream.CalculateBoxSize(view_identifiers), this.view_identifiers); // mandatory
		// boxSize += tracer.Field("descr", IsoStream.CalculateBoxSize(descr), this.descr); // optional
		// boxSize += tracer.Field("intrinsic_camera_params", IsoStream.CalculateBoxSize(intrinsic_camera_params), this.intrinsic_camera_params); // optional
		// boxSize += tracer.Field("extrinsic_camera_params", IsoStream.CalculateBoxSize(extrinsic_camera_params), this.extrinsic_camera_params); // optional
		// boxSize += tracer.Field("a3dconfig", IsoStream.CalculateBoxSize(a3dconfig), this.a3dconfig); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
