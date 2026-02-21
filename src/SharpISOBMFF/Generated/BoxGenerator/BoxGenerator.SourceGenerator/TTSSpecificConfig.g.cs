using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class TTSSpecificConfig() {
    TTS_Sequence();
}


*/
public partial class TTSSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "TTSSpecificConfig"; } }

	protected TTS_Sequence TTS_Sequence; 
	public TTS_Sequence TTSSequence { get { return this.TTS_Sequence; } set { this.TTS_Sequence = value; } }

	public TTSSpecificConfig(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new TTS_Sequence(),  out this.TTS_Sequence, "TTS_Sequence"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteClass( this.TTS_Sequence, "TTS_Sequence"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += IsoStream.CalculateClassSize(TTS_Sequence); // TTS_Sequence
		return boxSize;
	}
}

}
