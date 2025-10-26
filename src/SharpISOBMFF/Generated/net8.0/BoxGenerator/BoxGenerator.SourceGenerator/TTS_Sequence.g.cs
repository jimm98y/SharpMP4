using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class TTS_Sequence()
{
  uimsbf(5) TTS_Sequence_ID;
  uimsbf(18) Language_Code;
  bslbf(1) Gender_Enable;
  bslbf(1) Age_Enable;
  bslbf(1) Speech_Rate_Enable;
  bslbf(1) Prosody_Enable;
  bslbf(1) Video_Enable;
  bslbf(1) Lip_Shape_Enable;
  bslbf(1) Trick_Mode_Enable;
}


*/
public partial class TTS_Sequence : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "TTS_Sequence"; } }

	protected byte TTS_Sequence_ID; 
	public byte TTSSequenceID { get { return this.TTS_Sequence_ID; } set { this.TTS_Sequence_ID = value; } }

	protected uint Language_Code; 
	public uint LanguageCode { get { return this.Language_Code; } set { this.Language_Code = value; } }

	protected bool Gender_Enable; 
	public bool GenderEnable { get { return this.Gender_Enable; } set { this.Gender_Enable = value; } }

	protected bool Age_Enable; 
	public bool AgeEnable { get { return this.Age_Enable; } set { this.Age_Enable = value; } }

	protected bool Speech_Rate_Enable; 
	public bool SpeechRateEnable { get { return this.Speech_Rate_Enable; } set { this.Speech_Rate_Enable = value; } }

	protected bool Prosody_Enable; 
	public bool ProsodyEnable { get { return this.Prosody_Enable; } set { this.Prosody_Enable = value; } }

	protected bool Video_Enable; 
	public bool VideoEnable { get { return this.Video_Enable; } set { this.Video_Enable = value; } }

	protected bool Lip_Shape_Enable; 
	public bool LipShapeEnable { get { return this.Lip_Shape_Enable; } set { this.Lip_Shape_Enable = value; } }

	protected bool Trick_Mode_Enable; 
	public bool TrickModeEnable { get { return this.Trick_Mode_Enable; } set { this.Trick_Mode_Enable = value; } }

	public TTS_Sequence(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.TTS_Sequence_ID, "TTS_Sequence_ID"); 
		boxSize += stream.ReadBits(boxSize, readSize, 18,  out this.Language_Code, "Language_Code"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Gender_Enable, "Gender_Enable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Age_Enable, "Age_Enable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Speech_Rate_Enable, "Speech_Rate_Enable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Prosody_Enable, "Prosody_Enable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Video_Enable, "Video_Enable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Lip_Shape_Enable, "Lip_Shape_Enable"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.Trick_Mode_Enable, "Trick_Mode_Enable"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBits(5,  this.TTS_Sequence_ID, "TTS_Sequence_ID"); 
		boxSize += stream.WriteBits(18,  this.Language_Code, "Language_Code"); 
		boxSize += stream.WriteBit( this.Gender_Enable, "Gender_Enable"); 
		boxSize += stream.WriteBit( this.Age_Enable, "Age_Enable"); 
		boxSize += stream.WriteBit( this.Speech_Rate_Enable, "Speech_Rate_Enable"); 
		boxSize += stream.WriteBit( this.Prosody_Enable, "Prosody_Enable"); 
		boxSize += stream.WriteBit( this.Video_Enable, "Video_Enable"); 
		boxSize += stream.WriteBit( this.Lip_Shape_Enable, "Lip_Shape_Enable"); 
		boxSize += stream.WriteBit( this.Trick_Mode_Enable, "Trick_Mode_Enable"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 5; // TTS_Sequence_ID
		boxSize += 18; // Language_Code
		boxSize += 1; // Gender_Enable
		boxSize += 1; // Age_Enable
		boxSize += 1; // Speech_Rate_Enable
		boxSize += 1; // Prosody_Enable
		boxSize += 1; // Video_Enable
		boxSize += 1; // Lip_Shape_Enable
		boxSize += 1; // Trick_Mode_Enable
		return boxSize;
	}
}

}
