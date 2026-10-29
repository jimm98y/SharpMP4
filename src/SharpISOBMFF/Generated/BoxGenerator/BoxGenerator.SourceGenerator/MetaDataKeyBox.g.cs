using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class MetaDataKeyBox extends	Box(local_key_id) {
 MetaDataKeyDeclarationBox();
 MetaDataDatatypeBox();	  // optional
 MetaDataLocaleBox();	  // optional
 MetaDataSetupBox();	  // optional
 MetaDataExtensionsBox();  // optional
}

*/
public partial class MetaDataKeyBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"aligned(8) class MetaDataKeyBox extends	Box(local_key_id) {
 MetaDataKeyDeclarationBox();
 MetaDataDatatypeBox();	  // optional
 MetaDataLocaleBox();	  // optional
 MetaDataSetupBox();	  // optional
 MetaDataExtensionsBox();  // optional
}
"; } }
	public override string DisplayName { get { return "MetaDataKeyBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn(BoxPlacements.Of(this) ?? BoxPlacements.OfType(FourCC)); }

	public MetaDataKeyDeclarationBox _MetaDataKeyDeclarationBox { get { return this.children.OfType<MetaDataKeyDeclarationBox>().FirstOrDefault(); } }
	public MetaDataDatatypeBox _MetaDataDatatypeBox { get { return this.children.OfType<MetaDataDatatypeBox>().FirstOrDefault(); } }
	public MetaDataLocaleBox _MetaDataLocaleBox { get { return this.children.OfType<MetaDataLocaleBox>().FirstOrDefault(); } }
	public MetaDataSetupBox _MetaDataSetupBox { get { return this.children.OfType<MetaDataSetupBox>().FirstOrDefault(); } }
	public MetaDataExtensionsBox _MetaDataExtensionsBox { get { return this.children.OfType<MetaDataExtensionsBox>().FirstOrDefault(); } }

	public MetaDataKeyBox(uint local_key_id): base(local_key_id)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataKeyDeclarationBox, "MetaDataKeyDeclarationBox"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataDatatypeBox, "MetaDataDatatypeBox"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataLocaleBox, "MetaDataLocaleBox"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataSetupBox, "MetaDataSetupBox"); // optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.MetaDataExtensionsBox, "MetaDataExtensionsBox"); // optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.MetaDataKeyDeclarationBox, "MetaDataKeyDeclarationBox"); 
		// boxSize += stream.WriteBox( this.MetaDataDatatypeBox, "MetaDataDatatypeBox"); // optional
		// boxSize += stream.WriteBox( this.MetaDataLocaleBox, "MetaDataLocaleBox"); // optional
		// boxSize += stream.WriteBox( this.MetaDataSetupBox, "MetaDataSetupBox"); // optional
		// boxSize += stream.WriteBox( this.MetaDataExtensionsBox, "MetaDataExtensionsBox"); // optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(MetaDataKeyDeclarationBox); // MetaDataKeyDeclarationBox
		// boxSize += IsoStream.CalculateBoxSize(MetaDataDatatypeBox); // MetaDataDatatypeBox
		// boxSize += IsoStream.CalculateBoxSize(MetaDataLocaleBox); // MetaDataLocaleBox
		// boxSize += IsoStream.CalculateBoxSize(MetaDataSetupBox); // MetaDataSetupBox
		// boxSize += IsoStream.CalculateBoxSize(MetaDataExtensionsBox); // MetaDataExtensionsBox
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		// boxSize += tracer.Field("MetaDataKeyDeclarationBox", IsoStream.CalculateBoxSize(MetaDataKeyDeclarationBox), this.MetaDataKeyDeclarationBox); 
		// boxSize += tracer.Field("MetaDataDatatypeBox", IsoStream.CalculateBoxSize(MetaDataDatatypeBox), this.MetaDataDatatypeBox); // optional
		// boxSize += tracer.Field("MetaDataLocaleBox", IsoStream.CalculateBoxSize(MetaDataLocaleBox), this.MetaDataLocaleBox); // optional
		// boxSize += tracer.Field("MetaDataSetupBox", IsoStream.CalculateBoxSize(MetaDataSetupBox), this.MetaDataSetupBox); // optional
		// boxSize += tracer.Field("MetaDataExtensionsBox", IsoStream.CalculateBoxSize(MetaDataExtensionsBox), this.MetaDataExtensionsBox); // optional
		boxSize += tracer.Children(this);
		return boxSize;
	}
}

}
