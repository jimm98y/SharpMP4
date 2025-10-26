using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class AudioSpecificConfig() extends BaseDescriptor : bit(8) tag=DecSpecificInfoTag {
  GetAudioObjectType() audioObjectType;
  bslbf(4) samplingFrequencyIndex;
  if(samplingFrequencyIndex == 0xf ) {
    uimsbf(24) samplingFrequency;
  }
  bslbf(4) channelConfiguration;
  sbrPresentFlag = -1;
  psPresentFlag = -1;

  if (audioObjectType == 5 || audioObjectType == 29) {
    extensionAudioObjectType = 5;
    sbrPresentFlag = 1;
    if (audioObjectType == 29) {
      psPresentFlag = 1;
    }
    uimsbf(4) extensionSamplingFrequencyIndex;
    if (extensionSamplingFrequencyIndex == 0xf)
      uimsbf(24) extensionSamplingFrequency;
    GetAudioObjectType() audioObjectType;
    if (audioObjectType == 22)
      uimsbf(4) extensionChannelConfiguration;
  }
  else {
    extensionAudioObjectType = 0;
  }
  switch (audioObjectType) {
    case 1:
    case 2:
    case 3:
    case 4:
    case 6:
    case 7:
    case 17:
    case 19:
    case 20:
    case 21:
    case 22:
    case 23:
      GASpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType);
      break;
    case 8:
      CelpSpecificConfig(samplingFrequencyIndex);
      break;
    case 9:
      HvxcSpecificConfig();
      break;
    case 12:
      TTSSpecificConfig();
      break;
    case 13:
    case 14:
    case 15:
    case 16:
      StructuredAudioSpecificConfig();
      break;
    case 24:
      ErrorResilientCelpSpecificConfig(samplingFrequencyIndex);
      break;
    case 25:
      ErrorResilientHvxcSpecificConfig();
      break;
    case 26:
    case 27:
      ParametricSpecificConfig();
      break;
    case 28:
      SSCSpecificConfig(channelConfiguration);
      break;
    case 30:
      uimsbf(1) sacPayloadEmbedding;
      SpatialSpecificConfig();
      break;
    case 32:
    case 33:
    case 34:
      MPEG_1_2_SpecificConfig();
      break;
    case 35:
      DSTSpecificConfig(channelConfiguration);
      break;
    case 36:
      bslbf(5) fillBits;
      ALSSpecificConfig();
      break;
    case 37:
    case 38:
      SLSSpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType);
      break;
    case 39:
      ELDSpecificConfig(channelConfiguration);
      break;
    case 40:
    case 41:
      SymbolicMusicSpecificConfig();
      break;
    default:
      /* reserved *//*
      break;
  }
  switch (audioObjectType) {
    case 17:
    case 19:
    case 20:
    case 21:
    case 22:
    case 23:
    case 24:
    case 25:
    case 26:
    case 27:
    case 39:
      bslbf(2) epConfig;
      if (epConfig == 2 || epConfig == 3) {
        ErrorProtectionSpecificConfig();
      }
      if (epConfig == 3) {
        bslbf(1) directMapping;
        if (!directMapping) {
          /* tbd *//*
        }
      }
      break;
  }
  if (extensionAudioObjectType != 5 && bits_to_decode() >= 16) {
    bslbf(11) syncExtensionType;
    if (syncExtensionType == 0x2b7) {
      GetAudioObjectType() extensionAudioObjectType;
      if (extensionAudioObjectType == 5) {
        uimsbf(1) sbrPresentFlag;
        if (sbrPresentFlag == 1) {
          uimsbf(4) extensionSamplingFrequencyIndex;
          if (extensionSamplingFrequencyIndex == 0xf) {
            uimsbf(24) extensionSamplingFrequency;
          }
          if (bits_to_decode() >= 12) {
            bslbf(11) syncExtensionType;
            if (syncExtensionType == 0x548) {
              uimsbf(1) psPresentFlag;
            }
          }
        }
      }
      if (extensionAudioObjectType == 22) {
        uimsbf(1) sbrPresentFlag;
        if (sbrPresentFlag == 1) {
          uimsbf(4) extensionSamplingFrequencyIndex;
          if (extensionSamplingFrequencyIndex == 0xf) {
            uimsbf(24) extensionSamplingFrequency;
          }
        }
        uimsbf(4) extensionChannelConfiguration;
      }
    }
  }   
}


*/
public partial class AudioSpecificConfig : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.DecSpecificInfoTag;
	public override string DisplayName { get { return "AudioSpecificConfig"; } }

	protected GetAudioObjectType audioObjectType; 
	public GetAudioObjectType AudioObjectType { get { return this.audioObjectType; } set { this.audioObjectType = value; } }

	protected byte samplingFrequencyIndex; 
	public byte SamplingFrequencyIndex { get { return this.samplingFrequencyIndex; } set { this.samplingFrequencyIndex = value; } }

	protected uint samplingFrequency; 
	public uint SamplingFrequency { get { return this.samplingFrequency; } set { this.samplingFrequency = value; } }

	protected byte channelConfiguration; 
	public byte ChannelConfiguration { get { return this.channelConfiguration; } set { this.channelConfiguration = value; } }

	protected byte extensionSamplingFrequencyIndex; 
	public byte ExtensionSamplingFrequencyIndex { get { return this.extensionSamplingFrequencyIndex; } set { this.extensionSamplingFrequencyIndex = value; } }

	protected uint extensionSamplingFrequency; 
	public uint ExtensionSamplingFrequency { get { return this.extensionSamplingFrequency; } set { this.extensionSamplingFrequency = value; } }

	protected byte extensionChannelConfiguration; 
	public byte ExtensionChannelConfiguration { get { return this.extensionChannelConfiguration; } set { this.extensionChannelConfiguration = value; } }

	protected GASpecificConfig GASpecificConfig; 
	public GASpecificConfig _GASpecificConfig { get { return this.GASpecificConfig; } set { this.GASpecificConfig = value; } }

	protected CelpSpecificConfig CelpSpecificConfig; 
	public CelpSpecificConfig _CelpSpecificConfig { get { return this.CelpSpecificConfig; } set { this.CelpSpecificConfig = value; } }

	protected HvxcSpecificConfig HvxcSpecificConfig; 
	public HvxcSpecificConfig _HvxcSpecificConfig { get { return this.HvxcSpecificConfig; } set { this.HvxcSpecificConfig = value; } }

	protected TTSSpecificConfig TTSSpecificConfig; 
	public TTSSpecificConfig _TTSSpecificConfig { get { return this.TTSSpecificConfig; } set { this.TTSSpecificConfig = value; } }

	protected StructuredAudioSpecificConfig StructuredAudioSpecificConfig; 
	public StructuredAudioSpecificConfig _StructuredAudioSpecificConfig { get { return this.StructuredAudioSpecificConfig; } set { this.StructuredAudioSpecificConfig = value; } }

	protected ErrorResilientCelpSpecificConfig ErrorResilientCelpSpecificConfig; 
	public ErrorResilientCelpSpecificConfig _ErrorResilientCelpSpecificConfig { get { return this.ErrorResilientCelpSpecificConfig; } set { this.ErrorResilientCelpSpecificConfig = value; } }

	protected ErrorResilientHvxcSpecificConfig ErrorResilientHvxcSpecificConfig; 
	public ErrorResilientHvxcSpecificConfig _ErrorResilientHvxcSpecificConfig { get { return this.ErrorResilientHvxcSpecificConfig; } set { this.ErrorResilientHvxcSpecificConfig = value; } }

	protected ParametricSpecificConfig ParametricSpecificConfig; 
	public ParametricSpecificConfig _ParametricSpecificConfig { get { return this.ParametricSpecificConfig; } set { this.ParametricSpecificConfig = value; } }

	protected SSCSpecificConfig SSCSpecificConfig; 
	public SSCSpecificConfig _SSCSpecificConfig { get { return this.SSCSpecificConfig; } set { this.SSCSpecificConfig = value; } }

	protected bool sacPayloadEmbedding; 
	public bool SacPayloadEmbedding { get { return this.sacPayloadEmbedding; } set { this.sacPayloadEmbedding = value; } }

	protected SpatialSpecificConfig SpatialSpecificConfig; 
	public SpatialSpecificConfig _SpatialSpecificConfig { get { return this.SpatialSpecificConfig; } set { this.SpatialSpecificConfig = value; } }

	protected MPEG_1_2_SpecificConfig MPEG_1_2_SpecificConfig; 
	public MPEG_1_2_SpecificConfig MPEG12SpecificConfig { get { return this.MPEG_1_2_SpecificConfig; } set { this.MPEG_1_2_SpecificConfig = value; } }

	protected DSTSpecificConfig DSTSpecificConfig; 
	public DSTSpecificConfig _DSTSpecificConfig { get { return this.DSTSpecificConfig; } set { this.DSTSpecificConfig = value; } }

	protected byte fillBits; 
	public byte FillBits { get { return this.fillBits; } set { this.fillBits = value; } }

	protected ALSSpecificConfig ALSSpecificConfig; 
	public ALSSpecificConfig _ALSSpecificConfig { get { return this.ALSSpecificConfig; } set { this.ALSSpecificConfig = value; } }

	protected SLSSpecificConfig SLSSpecificConfig; 
	public SLSSpecificConfig _SLSSpecificConfig { get { return this.SLSSpecificConfig; } set { this.SLSSpecificConfig = value; } }

	protected ELDSpecificConfig ELDSpecificConfig; 
	public ELDSpecificConfig _ELDSpecificConfig { get { return this.ELDSpecificConfig; } set { this.ELDSpecificConfig = value; } }

	protected SymbolicMusicSpecificConfig SymbolicMusicSpecificConfig; 
	public SymbolicMusicSpecificConfig _SymbolicMusicSpecificConfig { get { return this.SymbolicMusicSpecificConfig; } set { this.SymbolicMusicSpecificConfig = value; } }

	protected byte epConfig; 
	public byte EpConfig { get { return this.epConfig; } set { this.epConfig = value; } }

	protected ErrorProtectionSpecificConfig ErrorProtectionSpecificConfig; 
	public ErrorProtectionSpecificConfig _ErrorProtectionSpecificConfig { get { return this.ErrorProtectionSpecificConfig; } set { this.ErrorProtectionSpecificConfig = value; } }

	protected bool directMapping; 
	public bool DirectMapping { get { return this.directMapping; } set { this.directMapping = value; } }

	protected ushort syncExtensionType; 
	public ushort SyncExtensionType { get { return this.syncExtensionType; } set { this.syncExtensionType = value; } }

	protected GetAudioObjectType extensionAudioObjectType= new GetAudioObjectType(); 
	public GetAudioObjectType ExtensionAudioObjectType { get { return this.extensionAudioObjectType; } set { this.extensionAudioObjectType = value; } }

	protected bool sbrPresentFlag; 
	public bool SbrPresentFlag { get { return this.sbrPresentFlag; } set { this.sbrPresentFlag = value; } }

	protected bool psPresentFlag; 
	public bool PsPresentFlag { get { return this.psPresentFlag; } set { this.psPresentFlag = value; } }

	public AudioSpecificConfig(): base(DescriptorTags.DecSpecificInfoTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadClass(boxSize, readSize, this, () => new GetAudioObjectType(),  out this.audioObjectType, "audioObjectType"); 
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.samplingFrequencyIndex, "samplingFrequencyIndex"); 

		if (samplingFrequencyIndex == 0xf )
		{
			boxSize += stream.ReadUInt24(boxSize, readSize,  out this.samplingFrequency, "samplingFrequency"); 
		}
		boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.channelConfiguration, "channelConfiguration"); 
		sbrPresentFlag = false;
		psPresentFlag = false;

		if (audioObjectType.AudioObjectType == 5 || audioObjectType.AudioObjectType == 29)
		{
			extensionAudioObjectType.AudioObjectType = 5;
			sbrPresentFlag = true;

			if (audioObjectType.AudioObjectType == 29)
			{
				psPresentFlag = true;
			}
			boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.extensionSamplingFrequencyIndex, "extensionSamplingFrequencyIndex"); 

			if (extensionSamplingFrequencyIndex == 0xf)
			{
				boxSize += stream.ReadUInt24(boxSize, readSize,  out this.extensionSamplingFrequency, "extensionSamplingFrequency"); 
			}
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new GetAudioObjectType(),  out this.audioObjectType, "audioObjectType"); 

			if (audioObjectType.AudioObjectType == 22)
			{
				boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.extensionChannelConfiguration, "extensionChannelConfiguration"); 
			}
		}

		else 
		{
			extensionAudioObjectType.AudioObjectType = 0;
		}

		switch (audioObjectType.AudioObjectType)
		{
			case 1:
			case 2:
			case 3:
			case 4:
			case 6:
			case 7:
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new GASpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType.AudioObjectType),  out this.GASpecificConfig, "GASpecificConfig"); 
			break;

			case 8:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new CelpSpecificConfig(samplingFrequencyIndex),  out this.CelpSpecificConfig, "CelpSpecificConfig"); 
			break;

			case 9:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new HvxcSpecificConfig(),  out this.HvxcSpecificConfig, "HvxcSpecificConfig"); 
			break;

			case 12:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new TTSSpecificConfig(),  out this.TTSSpecificConfig, "TTSSpecificConfig"); 
			break;

			case 13:
			case 14:
			case 15:
			case 16:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new StructuredAudioSpecificConfig(),  out this.StructuredAudioSpecificConfig, "StructuredAudioSpecificConfig"); 
			break;

			case 24:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ErrorResilientCelpSpecificConfig(samplingFrequencyIndex),  out this.ErrorResilientCelpSpecificConfig, "ErrorResilientCelpSpecificConfig"); 
			break;

			case 25:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ErrorResilientHvxcSpecificConfig(),  out this.ErrorResilientHvxcSpecificConfig, "ErrorResilientHvxcSpecificConfig"); 
			break;

			case 26:
			case 27:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ParametricSpecificConfig(),  out this.ParametricSpecificConfig, "ParametricSpecificConfig"); 
			break;

			case 28:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new SSCSpecificConfig(channelConfiguration),  out this.SSCSpecificConfig, "SSCSpecificConfig"); 
			break;

			case 30:
			boxSize += stream.ReadBit(boxSize, readSize,  out this.sacPayloadEmbedding, "sacPayloadEmbedding"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new SpatialSpecificConfig(),  out this.SpatialSpecificConfig, "SpatialSpecificConfig"); 
			break;

			case 32:
			case 33:
			case 34:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new MPEG_1_2_SpecificConfig(),  out this.MPEG_1_2_SpecificConfig, "MPEG_1_2_SpecificConfig"); 
			break;

			case 35:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new DSTSpecificConfig(channelConfiguration),  out this.DSTSpecificConfig, "DSTSpecificConfig"); 
			break;

			case 36:
			boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.fillBits, "fillBits"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ALSSpecificConfig(),  out this.ALSSpecificConfig, "ALSSpecificConfig"); 
			break;

			case 37:
			case 38:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new SLSSpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType.AudioObjectType),  out this.SLSSpecificConfig, "SLSSpecificConfig"); 
			break;

			case 39:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new ELDSpecificConfig(channelConfiguration),  out this.ELDSpecificConfig, "ELDSpecificConfig"); 
			break;

			case 40:
			case 41:
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new SymbolicMusicSpecificConfig(),  out this.SymbolicMusicSpecificConfig, "SymbolicMusicSpecificConfig"); 
			break;

			default:

			/*  reserved  */
			break;

		}

		switch (audioObjectType.AudioObjectType)
		{
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 39:
			boxSize += stream.ReadBits(boxSize, readSize, 2,  out this.epConfig, "epConfig"); 

			if (epConfig == 2 || epConfig == 3)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new ErrorProtectionSpecificConfig(),  out this.ErrorProtectionSpecificConfig, "ErrorProtectionSpecificConfig"); 
			}

			if (epConfig == 3)
			{
				boxSize += stream.ReadBit(boxSize, readSize,  out this.directMapping, "directMapping"); 

				if (!directMapping)
				{
					/*  tbd  */
				}
			}
			break;

		}

		if (extensionAudioObjectType.AudioObjectType != 5 && IsoStream.BitsToDecode(boxSize, readSize) >= 16)
		{
			boxSize += stream.ReadBits(boxSize, readSize, 11,  out this.syncExtensionType, "syncExtensionType"); 

			if (syncExtensionType == 0x2b7)
			{
				boxSize += stream.ReadClass(boxSize, readSize, this, () => new GetAudioObjectType(),  out this.extensionAudioObjectType, "extensionAudioObjectType"); 

				if (extensionAudioObjectType.AudioObjectType == 5)
				{
					boxSize += stream.ReadBit(boxSize, readSize,  out this.sbrPresentFlag, "sbrPresentFlag"); 

					if (sbrPresentFlag == true)
					{
						boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.extensionSamplingFrequencyIndex, "extensionSamplingFrequencyIndex"); 

						if (extensionSamplingFrequencyIndex == 0xf)
						{
							boxSize += stream.ReadUInt24(boxSize, readSize,  out this.extensionSamplingFrequency, "extensionSamplingFrequency"); 
						}

						if (IsoStream.BitsToDecode(boxSize, readSize) >= 12)
						{
							boxSize += stream.ReadBits(boxSize, readSize, 11,  out this.syncExtensionType, "syncExtensionType"); 

							if (syncExtensionType == 0x548)
							{
								boxSize += stream.ReadBit(boxSize, readSize,  out this.psPresentFlag, "psPresentFlag"); 
							}
						}
					}
				}

				if (extensionAudioObjectType.AudioObjectType == 22)
				{
					boxSize += stream.ReadBit(boxSize, readSize,  out this.sbrPresentFlag, "sbrPresentFlag"); 

					if (sbrPresentFlag == true)
					{
						boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.extensionSamplingFrequencyIndex, "extensionSamplingFrequencyIndex"); 

						if (extensionSamplingFrequencyIndex == 0xf)
						{
							boxSize += stream.ReadUInt24(boxSize, readSize,  out this.extensionSamplingFrequency, "extensionSamplingFrequency"); 
						}
					}
					boxSize += stream.ReadBits(boxSize, readSize, 4,  out this.extensionChannelConfiguration, "extensionChannelConfiguration"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteClass( this.audioObjectType, "audioObjectType"); 
		boxSize += stream.WriteBits(4,  this.samplingFrequencyIndex, "samplingFrequencyIndex"); 

		if (samplingFrequencyIndex == 0xf )
		{
			boxSize += stream.WriteUInt24( this.samplingFrequency, "samplingFrequency"); 
		}
		boxSize += stream.WriteBits(4,  this.channelConfiguration, "channelConfiguration"); 
		sbrPresentFlag = false;
		psPresentFlag = false;

		if (audioObjectType.AudioObjectType == 5 || audioObjectType.AudioObjectType == 29)
		{
			extensionAudioObjectType.AudioObjectType = 5;
			sbrPresentFlag = true;

			if (audioObjectType.AudioObjectType == 29)
			{
				psPresentFlag = true;
			}
			boxSize += stream.WriteBits(4,  this.extensionSamplingFrequencyIndex, "extensionSamplingFrequencyIndex"); 

			if (extensionSamplingFrequencyIndex == 0xf)
			{
				boxSize += stream.WriteUInt24( this.extensionSamplingFrequency, "extensionSamplingFrequency"); 
			}
			boxSize += stream.WriteClass( this.audioObjectType, "audioObjectType"); 

			if (audioObjectType.AudioObjectType == 22)
			{
				boxSize += stream.WriteBits(4,  this.extensionChannelConfiguration, "extensionChannelConfiguration"); 
			}
		}

		else 
		{
			extensionAudioObjectType.AudioObjectType = 0;
		}

		switch (audioObjectType.AudioObjectType)
		{
			case 1:
			case 2:
			case 3:
			case 4:
			case 6:
			case 7:
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			boxSize += stream.WriteClass( this.GASpecificConfig, "GASpecificConfig"); 
			break;

			case 8:
			boxSize += stream.WriteClass( this.CelpSpecificConfig, "CelpSpecificConfig"); 
			break;

			case 9:
			boxSize += stream.WriteClass( this.HvxcSpecificConfig, "HvxcSpecificConfig"); 
			break;

			case 12:
			boxSize += stream.WriteClass( this.TTSSpecificConfig, "TTSSpecificConfig"); 
			break;

			case 13:
			case 14:
			case 15:
			case 16:
			boxSize += stream.WriteClass( this.StructuredAudioSpecificConfig, "StructuredAudioSpecificConfig"); 
			break;

			case 24:
			boxSize += stream.WriteClass( this.ErrorResilientCelpSpecificConfig, "ErrorResilientCelpSpecificConfig"); 
			break;

			case 25:
			boxSize += stream.WriteClass( this.ErrorResilientHvxcSpecificConfig, "ErrorResilientHvxcSpecificConfig"); 
			break;

			case 26:
			case 27:
			boxSize += stream.WriteClass( this.ParametricSpecificConfig, "ParametricSpecificConfig"); 
			break;

			case 28:
			boxSize += stream.WriteClass( this.SSCSpecificConfig, "SSCSpecificConfig"); 
			break;

			case 30:
			boxSize += stream.WriteBit( this.sacPayloadEmbedding, "sacPayloadEmbedding"); 
			boxSize += stream.WriteClass( this.SpatialSpecificConfig, "SpatialSpecificConfig"); 
			break;

			case 32:
			case 33:
			case 34:
			boxSize += stream.WriteClass( this.MPEG_1_2_SpecificConfig, "MPEG_1_2_SpecificConfig"); 
			break;

			case 35:
			boxSize += stream.WriteClass( this.DSTSpecificConfig, "DSTSpecificConfig"); 
			break;

			case 36:
			boxSize += stream.WriteBits(5,  this.fillBits, "fillBits"); 
			boxSize += stream.WriteClass( this.ALSSpecificConfig, "ALSSpecificConfig"); 
			break;

			case 37:
			case 38:
			boxSize += stream.WriteClass( this.SLSSpecificConfig, "SLSSpecificConfig"); 
			break;

			case 39:
			boxSize += stream.WriteClass( this.ELDSpecificConfig, "ELDSpecificConfig"); 
			break;

			case 40:
			case 41:
			boxSize += stream.WriteClass( this.SymbolicMusicSpecificConfig, "SymbolicMusicSpecificConfig"); 
			break;

			default:

			/*  reserved  */
			break;

		}

		switch (audioObjectType.AudioObjectType)
		{
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 39:
			boxSize += stream.WriteBits(2,  this.epConfig, "epConfig"); 

			if (epConfig == 2 || epConfig == 3)
			{
				boxSize += stream.WriteClass( this.ErrorProtectionSpecificConfig, "ErrorProtectionSpecificConfig"); 
			}

			if (epConfig == 3)
			{
				boxSize += stream.WriteBit( this.directMapping, "directMapping"); 

				if (!directMapping)
				{
					/*  tbd  */
				}
			}
			break;

		}

		if (extensionAudioObjectType.AudioObjectType != 5 && IsoStream.BitsToDecode(boxSize, SizeOfInstance) >= 16)
		{
			boxSize += stream.WriteBits(11,  this.syncExtensionType, "syncExtensionType"); 

			if (syncExtensionType == 0x2b7)
			{
				boxSize += stream.WriteClass( this.extensionAudioObjectType, "extensionAudioObjectType"); 

				if (extensionAudioObjectType.AudioObjectType == 5)
				{
					boxSize += stream.WriteBit( this.sbrPresentFlag, "sbrPresentFlag"); 

					if (sbrPresentFlag == true)
					{
						boxSize += stream.WriteBits(4,  this.extensionSamplingFrequencyIndex, "extensionSamplingFrequencyIndex"); 

						if (extensionSamplingFrequencyIndex == 0xf)
						{
							boxSize += stream.WriteUInt24( this.extensionSamplingFrequency, "extensionSamplingFrequency"); 
						}

						if (IsoStream.BitsToDecode(boxSize, SizeOfInstance) >= 12)
						{
							boxSize += stream.WriteBits(11,  this.syncExtensionType, "syncExtensionType"); 

							if (syncExtensionType == 0x548)
							{
								boxSize += stream.WriteBit( this.psPresentFlag, "psPresentFlag"); 
							}
						}
					}
				}

				if (extensionAudioObjectType.AudioObjectType == 22)
				{
					boxSize += stream.WriteBit( this.sbrPresentFlag, "sbrPresentFlag"); 

					if (sbrPresentFlag == true)
					{
						boxSize += stream.WriteBits(4,  this.extensionSamplingFrequencyIndex, "extensionSamplingFrequencyIndex"); 

						if (extensionSamplingFrequencyIndex == 0xf)
						{
							boxSize += stream.WriteUInt24( this.extensionSamplingFrequency, "extensionSamplingFrequency"); 
						}
					}
					boxSize += stream.WriteBits(4,  this.extensionChannelConfiguration, "extensionChannelConfiguration"); 
				}
			}
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateClassSize(audioObjectType); // audioObjectType
		boxSize += 4; // samplingFrequencyIndex

		if (samplingFrequencyIndex == 0xf )
		{
			boxSize += 24; // samplingFrequency
		}
		boxSize += 4; // channelConfiguration
		sbrPresentFlag = false;
		psPresentFlag = false;

		if (audioObjectType.AudioObjectType == 5 || audioObjectType.AudioObjectType == 29)
		{
			extensionAudioObjectType.AudioObjectType = 5;
			sbrPresentFlag = true;

			if (audioObjectType.AudioObjectType == 29)
			{
				psPresentFlag = true;
			}
			boxSize += 4; // extensionSamplingFrequencyIndex

			if (extensionSamplingFrequencyIndex == 0xf)
			{
				boxSize += 24; // extensionSamplingFrequency
			}
			boxSize += IsoStream.CalculateClassSize(audioObjectType); // audioObjectType

			if (audioObjectType.AudioObjectType == 22)
			{
				boxSize += 4; // extensionChannelConfiguration
			}
		}

		else 
		{
			extensionAudioObjectType.AudioObjectType = 0;
		}

		switch (audioObjectType.AudioObjectType)
		{
			case 1:
			case 2:
			case 3:
			case 4:
			case 6:
			case 7:
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			boxSize += IsoStream.CalculateClassSize(GASpecificConfig); // GASpecificConfig
			break;

			case 8:
			boxSize += IsoStream.CalculateClassSize(CelpSpecificConfig); // CelpSpecificConfig
			break;

			case 9:
			boxSize += IsoStream.CalculateClassSize(HvxcSpecificConfig); // HvxcSpecificConfig
			break;

			case 12:
			boxSize += IsoStream.CalculateClassSize(TTSSpecificConfig); // TTSSpecificConfig
			break;

			case 13:
			case 14:
			case 15:
			case 16:
			boxSize += IsoStream.CalculateClassSize(StructuredAudioSpecificConfig); // StructuredAudioSpecificConfig
			break;

			case 24:
			boxSize += IsoStream.CalculateClassSize(ErrorResilientCelpSpecificConfig); // ErrorResilientCelpSpecificConfig
			break;

			case 25:
			boxSize += IsoStream.CalculateClassSize(ErrorResilientHvxcSpecificConfig); // ErrorResilientHvxcSpecificConfig
			break;

			case 26:
			case 27:
			boxSize += IsoStream.CalculateClassSize(ParametricSpecificConfig); // ParametricSpecificConfig
			break;

			case 28:
			boxSize += IsoStream.CalculateClassSize(SSCSpecificConfig); // SSCSpecificConfig
			break;

			case 30:
			boxSize += 1; // sacPayloadEmbedding
			boxSize += IsoStream.CalculateClassSize(SpatialSpecificConfig); // SpatialSpecificConfig
			break;

			case 32:
			case 33:
			case 34:
			boxSize += IsoStream.CalculateClassSize(MPEG_1_2_SpecificConfig); // MPEG_1_2_SpecificConfig
			break;

			case 35:
			boxSize += IsoStream.CalculateClassSize(DSTSpecificConfig); // DSTSpecificConfig
			break;

			case 36:
			boxSize += 5; // fillBits
			boxSize += IsoStream.CalculateClassSize(ALSSpecificConfig); // ALSSpecificConfig
			break;

			case 37:
			case 38:
			boxSize += IsoStream.CalculateClassSize(SLSSpecificConfig); // SLSSpecificConfig
			break;

			case 39:
			boxSize += IsoStream.CalculateClassSize(ELDSpecificConfig); // ELDSpecificConfig
			break;

			case 40:
			case 41:
			boxSize += IsoStream.CalculateClassSize(SymbolicMusicSpecificConfig); // SymbolicMusicSpecificConfig
			break;

			default:

			/*  reserved  */
			break;

		}

		switch (audioObjectType.AudioObjectType)
		{
			case 17:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 39:
			boxSize += 2; // epConfig

			if (epConfig == 2 || epConfig == 3)
			{
				boxSize += IsoStream.CalculateClassSize(ErrorProtectionSpecificConfig); // ErrorProtectionSpecificConfig
			}

			if (epConfig == 3)
			{
				boxSize += 1; // directMapping

				if (!directMapping)
				{
					/*  tbd  */
				}
			}
			break;

		}

		if (extensionAudioObjectType.AudioObjectType != 5 && IsoStream.BitsToDecode(boxSize, SizeOfInstance) >= 16)
		{
			boxSize += 11; // syncExtensionType

			if (syncExtensionType == 0x2b7)
			{
				boxSize += IsoStream.CalculateClassSize(extensionAudioObjectType); // extensionAudioObjectType

				if (extensionAudioObjectType.AudioObjectType == 5)
				{
					boxSize += 1; // sbrPresentFlag

					if (sbrPresentFlag == true)
					{
						boxSize += 4; // extensionSamplingFrequencyIndex

						if (extensionSamplingFrequencyIndex == 0xf)
						{
							boxSize += 24; // extensionSamplingFrequency
						}

						if (IsoStream.BitsToDecode(boxSize, SizeOfInstance) >= 12)
						{
							boxSize += 11; // syncExtensionType

							if (syncExtensionType == 0x548)
							{
								boxSize += 1; // psPresentFlag
							}
						}
					}
				}

				if (extensionAudioObjectType.AudioObjectType == 22)
				{
					boxSize += 1; // sbrPresentFlag

					if (sbrPresentFlag == true)
					{
						boxSize += 4; // extensionSamplingFrequencyIndex

						if (extensionSamplingFrequencyIndex == 0xf)
						{
							boxSize += 24; // extensionSamplingFrequency
						}
					}
					boxSize += 4; // extensionChannelConfiguration
				}
			}
		}
		return boxSize;
	}
}

}
