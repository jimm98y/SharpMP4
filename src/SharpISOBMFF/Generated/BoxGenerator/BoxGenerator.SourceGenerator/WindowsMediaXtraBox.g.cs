using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class WindowsMediaXtraBox() extends Box('Xtra') {
 XtraTag tags[]; 
 }
 
*/
public partial class WindowsMediaXtraBox : Box
{
	public const string TYPE = "Xtra";
	public override string DisplayName { get { return "WindowsMediaXtraBox"; } }

	protected XtraTag[] tags; 
	public XtraTag[] Tags { get { return this.tags; } set { this.tags = value; } }

	public WindowsMediaXtraBox(): base(IsoStream.FromFourCC("Xtra"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(uint.MaxValue), () => new XtraTag(),  out this.tags, "tags"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.tags, "tags"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(tags); // tags
		return boxSize;
	}
}

}
