using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ScalabilityInformationSEIBox extends Box('seib', size)
{
	unsigned int(8*size-64)	scalinfosei; 
}
*/
public partial class ScalabilityInformationSEIBox : Box
{
	/// <summary>The syntax the class is defined of, as its definition gives it.</summary>
	public override string Syntax { get { return @"class ScalabilityInformationSEIBox extends Box('seib', size)
{
	unsigned int(8*size-64)	scalinfosei; 
}"; } }
	public const string TYPE = "seib";
	public override string DisplayName { get { return "ScalabilityInformationSEIBox"; } }
	/// <summary>Whether the box is in a container its definitions place it in: see <see cref="Box.ValidatePlacement"/>.</summary>
	public override bool ValidatePlacement() { return IsPlacedIn("AVCSVCSampleEntry(avc1,avc3)", "AVC2SVCSampleEntry(avc2,avc4)", "SVCSampleEntry(svc1,svc2)"); }


	protected byte[] scalinfosei = []; 
	public byte[] Scalinfosei { get { return this.scalinfosei; } set { this.scalinfosei = value; } }

	public ScalabilityInformationSEIBox(ulong size = 0): base(IsoStream.FromFourCC("seib"), size)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadBits(boxSize, readSize, (uint)(8*size-64 ),  out this.scalinfosei, "scalinfosei"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteBits((uint)(8*size-64 ),  this.scalinfosei, "scalinfosei"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += (ulong)(8*size-64 ); // scalinfosei
		return boxSize;
	}

	public override ulong Trace(BoxTracer tracer)
	{
		ulong boxSize = 0;
		boxSize += base.Trace(tracer);
		boxSize += tracer.Field("scalinfosei", (ulong)(8*size-64 ), this.scalinfosei); 
		return boxSize;
	}
}

}
