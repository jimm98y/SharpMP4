using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ARDroneTelemetryBox() extends Box ('prrt'){
 bit(8) data[];
 }
*/
public partial class ARDroneTelemetryBox : Box
{
	public const string TYPE = "prrt";
	public override string DisplayName { get { return "ARDroneTelemetryBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("file"); }


	protected byte[] data; 
	public byte[] Data { get { return this.data; } set { this.data = value; } }

	public ARDroneTelemetryBox(): base(IsoStream.FromFourCC("prrt"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.data, "data"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt8ArrayTillEnd( this.data, "data"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += ((ulong)data.Length * 8); // data
		return boxSize;
	}

	public override ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += base.ToString(builder);
		boxSize += builder.Field("data", ((ulong)data.Length * 8), this.data); 
		return boxSize;
	}
}

}
