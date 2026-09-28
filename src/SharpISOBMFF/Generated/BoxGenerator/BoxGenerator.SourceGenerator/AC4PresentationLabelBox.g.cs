using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
// E.5a, Pseudocode E.2a
aligned(8) class AC4PresentationLabelBox extends FullBox('lac4', version = 0, 0) {
 unsigned int(16) num_presentation_labels;
 utf8string language_tag;
 for (i=0; i < num_presentation_labels; i++) {
  unsigned int(16) presentation_id;
  utf8string presentation_label;
 }
}
*/
public partial class AC4PresentationLabelBox : FullBox
{
	public const string TYPE = "lac4";
	public override string DisplayName { get { return "AC4PresentationLabelBox"; } }

	protected ushort num_presentation_labels; 
	public ushort NumPresentationLabels { get { return this.num_presentation_labels; } set { this.num_presentation_labels = value; } }

	protected BinaryUTF8String language_tag; 
	public BinaryUTF8String LanguageTag { get { return this.language_tag; } set { this.language_tag = value; } }

	protected ushort[] presentation_id; 
	public ushort[] PresentationId { get { return this.presentation_id; } set { this.presentation_id = value; } }

	protected BinaryUTF8String[] presentation_label; 
	public BinaryUTF8String[] PresentationLabel { get { return this.presentation_label; } set { this.presentation_label = value; } }

	public AC4PresentationLabelBox(): base(IsoStream.FromFourCC("lac4"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.num_presentation_labels, "num_presentation_labels"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.language_tag, "language_tag"); 

		this.presentation_id = stream.SafeAllocate<ushort>(boxSize, readSize, IsoStream.GetInt( num_presentation_labels), "presentation_id");
		this.presentation_label = stream.SafeAllocate<BinaryUTF8String>(boxSize, readSize, IsoStream.GetInt( num_presentation_labels), "presentation_label");
		for (int i=0; i < num_presentation_labels; i++)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.presentation_id[i], "presentation_id"); 
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.presentation_label[i], "presentation_label"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.num_presentation_labels, "num_presentation_labels"); 
		boxSize += stream.WriteStringZeroTerminated( this.language_tag, "language_tag"); 

		for (int i=0; i < num_presentation_labels; i++)
		{
			boxSize += stream.WriteUInt16( this.presentation_id[i], "presentation_id"); 
			boxSize += stream.WriteStringZeroTerminated( this.presentation_label[i], "presentation_label"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // num_presentation_labels
		boxSize += IsoStream.CalculateStringSize(language_tag); // language_tag

		for (int i=0; i < num_presentation_labels; i++)
		{
			boxSize += 16; // presentation_id
			boxSize += IsoStream.CalculateStringSize(presentation_label[i]); // presentation_label
		}
		return boxSize;
	}
}

}
