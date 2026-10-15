using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class SphericalVideoHeader extends FullBox('svhd', 0, 0) {
 string metadata_source;
 }

*/
public partial class SphericalVideoHeader : FullBox
{
	public const string TYPE = "svhd";
	public override string DisplayName { get { return "SphericalVideoHeader"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("sv3d"); }


	protected BinaryUTF8String metadata_source; 
	public BinaryUTF8String MetadataSource { get { return this.metadata_source; } set { this.metadata_source = value; } }

	public SphericalVideoHeader(): base(IsoStream.FromFourCC("svhd"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.metadata_source, "metadata_source"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.metadata_source, "metadata_source"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(metadata_source); // metadata_source
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("metadata_source", IsoStream.CalculateStringSize(metadata_source), this.metadata_source); 
		return boxSize;
	}
}

}
