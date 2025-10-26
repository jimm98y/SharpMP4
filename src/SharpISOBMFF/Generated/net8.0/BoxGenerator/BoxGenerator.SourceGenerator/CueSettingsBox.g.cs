using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class CueSettingsBox() extends Box ('sttg') {
 boxstring settings; 
 }
*/
public partial class CueSettingsBox : Box
{
	public const string TYPE = "sttg";
	public override string DisplayName { get { return "CueSettingsBox"; } }

	protected BinaryUTF8String settings; 
	public BinaryUTF8String Settings { get { return this.settings; } set { this.settings = value; } }

	public CueSettingsBox(): base(IsoStream.FromFourCC("sttg"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.settings, "settings"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.settings, "settings"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(settings); // settings
		return boxSize;
	}
}

}
