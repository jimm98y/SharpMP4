class AudioSpecificConfig extends DecoderConfigDescriptor() {
  audioObjectType = GetAudioObjectType();
  bslbf(4) samplingFrequencyIndex;
  if(samplingFrequencyIndex == 0xf ) {
    uimsbf(24) samplingFrequency;
  }
  bslbf(4) channelConfiguration;
  sbrPresentFlag = -1;
  psPresentFlag = -1;

  if (audioObjectType == 5 ||
    audioObjectType == 29) {
    extensionAudioObjectType = 5;
    sbrPresentFlag = 1;
    if (audioObjectType == 29) {
      psPresentFlag = 1;
    }
    uimsbf(4) extensionSamplingFrequencyIndex;
    if (extensionSamplingFrequencyIndex == 0xf)
      uimsbf(24) extensionSamplingFrequency;
    uimsbf(4) audioObjectType = GetAudioObjectType();
    if (audioObjectType == 22)
      extensionChannelConfiguration;
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
      GASpecificConfig();
      break;
    case 8:
      CelpSpecificConfig();
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
      ErrorResilientCelpSpecificConfig();
      break;
    case 25:
      ErrorResilientHvxcSpecificConfig();
      break;
    case 26:
    case 27:
      ParametricSpecificConfig();
      break;
    case 28:
      SSCSpecificConfig();
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
      DSTSpecificConfig();
      break;
    case 36:
      bslbf(5) fillBits;
      ALSSpecificConfig();
      break;
    case 37:
    case 38:
      SLSSpecificConfig();
      break;
    case 39:
      ELDSpecificConfig(channelConfiguration);
      break:
    case 40:
    case 41:
      SymbolicMusicSpecificConfig();
      break;
    default:
    /* reserved */
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
          /* tbd */
        }
      }
  }
  if (extensionAudioObjectType != 5 && bits_to_decode() >= 16) {
    bslbf(11) syncExtensionType;
    if (syncExtensionType == 0x2b7) {
      extensionAudioObjectType = GetAudioObjectType();
      if (extensionAudioObjectType == 5) {
        uimsbf(1) sbrPresentFlag;
        if (sbrPresentFlag == 1) {
          uimsbf(4) extensionSamplingFrequencyIndex;
          if (extensionSamplingFrequencyIndex == 0xf) {
            uimsbf(24) extensionSamplingFrequency;
          }
          if (bits_to_decode() >= 12) {
            bslbf(11) syncExtensionType;
            if (syncExtesionType == 0x548) {
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

uimsbf(5) GetAudioObjectType()
{
  uimsbf(5) audioObjectType;
  if (audioObjectType == 31) {
    uimsbf(6) audioObjectTypeExt;
    audioObjectType = 32 + audioObjectTypeExt;
  }
  return audioObjectType;
}

GASpecificConfig(samplingFrequencyIndex,
  channelConfiguration,
  audioObjectType)
{
  bslbf(1) frameLengthFlag;
  bslbf(1) dependsOnCoreCoder;
  if (dependsOnCoreCoder) {
    uimsbf(14) coreCoderDelay;
  }
  bslbf(1) extensionFlag;
  if (!channelConfiguration) {
    program_config_element();
  }
  if ((audioObjectType == 6) || (audioObjectType == 20)) {
    uimsbf(3) layerNr;
  }
  if (extensionFlag) {
    if (audioObjectType == 22) {
      bslbf(5) numOfSubFrame;
      bslbf(11) layer_length;
    }
    if (audioObjectType == 17 || audioObjectType == 19 ||
      audioObjectType == 20 || audioObjectType == 23) {

      bslbf(1) aacSectionDataResilienceFlag;
      bslbf(1) aacScalefactorDataResilienceFlag;
      bslbf(1) aacSpectralDataResilienceFlag;
    }
    bslbf(1) extensionFlag3;
    if (extensionFlag3) {
      /* tbd in version 3 */
    }
  }
}

program_config_element()
{
  uimsbf(4) element_instance_tag;
  uimsbf(2) object_type;
  uimsbf(4) sampling_frequency_index;
  uimsbf(4) num_front_channel_elements;
  uimsbf(4) num_side_channel_elements;
  uimsbf(4) num_back_channel_elements;
  uimsbf(2) num_lfe_channel_elements;
  uimsbf(3) num_assoc_data_elements;
  uimsbf(4) num_valid_cc_elements;
  uimsbf(1) mono_mixdown_present;
  if (mono_mixdown_present == 1)
    uimsbf(4) mono_mixdown_element_number;
  uimsbf(1) stereo_mixdown_present;
  if (stereo_mixdown_present == 1)
    uimsbf(4) stereo_mixdown_element_number;
  uimsbf(1) matrix_mixdown_idx_present;
  if (matrix_mixdown_idx_present == 1) {
    uimsbf(2) matrix_mixdown_idx;
    uimsbf(1) pseudo_surround_enable;
  }
  for (i = 0; i < num_front_channel_elements; i++) {
    bslbf(1) front_element_is_cpe[i];
    uimsbf(4) front_element_tag_select[i];
  }
  for (i = 0; i < num_side_channel_elements; i++) {
    bslbf(1) side_element_is_cpe[i];
    uimsbf(4) side_element_tag_select[i];
  }
  for (i = 0; i < num_back_channel_elements; i++) {
    bslbf(1) back_element_is_cpe[i];
    uimsbf(4) back_element_tag_select[i];
  }
  for (i = 0; i < num_lfe_channel_elements; i++)
    uimsbf(4) lfe_element_tag_select[i];
  for (i = 0; i < num_assoc_data_elements; i++)
    uimsbf(4) assoc_data_element_tag_select[i];
  for (i = 0; i < num_valid_cc_elements; i++) {
    uimsbf(1) cc_element_is_ind_sw[i];
    uimsbf(4) valid_cc_element_tag_select[i];
  }
  byte_alignment();  // Note 1 
  uimsbf(8) comment_field_bytes;
  for (i = 0; i < comment_field_bytes; i++)
    uimsbf(8) comment_field_data[i];
}

CelpSpecificConfig(uint(4) samplingFrequencyIndex)
{
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    CelpHeader(samplingFrequencyIndex);
  }
  else {
    uimsbf(1) isBWSLayer;
    if (isBWSLayer) {
      CelpBWSenhHeader();
    }
    else {
      uimsbf(2) CELPBRSid;
    }
  }
}

CelpHeader(samplingFrequencyIndex)
{
  uimsbf(1) ExcitationMode;
  uimsbf(1) SampleRateMode;
  uimsbf(1) FineRateControl;
  if (ExcitationMode == RPE) {
    uimsbf(3) RPE_Configuration;
  }
  if (ExcitationMode == MPE) {
    uimsbf(5) MPE_Configuration;
    uimsbf(2) NumEnhLayers;
    uimsbf(1) BandwidthScalabilityMode;
  }
}

CelpBWSenhHeader()
{
  uimsbf(2) BWS_configuration;
}

HvxcSpecificConfig() {
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    HVXCconfig()
  }
}

HVXCconfig()
{
  uimsbf(1) HVXCvarMode;
  uimsbf(2) HVXCrateMode;
  uimsbf(1) extensionFlag;
  if (extensionFlag) {
    // to be defined in MPEG-4 Version 2
  }
}

TTSSpecificConfig() {
  TTS_Sequence()
}

TTS_Sequence()
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

ErrorResilientCelpSpecificConfig(uint(4) samplingFrequencyIndex)
{
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    ER_SC_CelpHeader(samplingFrequencyIndex);
  }
  else {
    uimsbf(1) isBWSLayer;
    if (isBWSLayer) {
      CelpBWSenhHeader();
    }
    else {
      uimsbf(2) CELPBRSid;
    }
  }
}

ER_SC_CelpHeader(samplingFrequencyIndex)
{
  uimsbf(1) ExcitationMode;
  uimsbf(1) SampleRateMode;
  uimsbf(1) FineRateControl;
  uimsbf(1) SilenceCompression;
  if (ExcitationMode == RPE) {
    uimsbf(3) RPE_Configuration;
  }
  if (ExcitationMode == MPE) {
    uimsbf(5) MPE_Configuration;
    uimsbf(2) NumEnhLayers;
    uimsbf(1) BandwidthScalabilityMode;
  }
}

ErrorResilientHvxcSpecificConfig() {
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    ErHVXCconfig();
  }
}

ErHVXCconfig()
{
  uimsbf(1) HVXCvarMode;
  uimsbf(2) HVXCrateMode;
  uimsbf(1) extensionFlag;
  if (extensionFlag) {
    uimsbf(1) var_ScalableFlag;
  }
}

ParametricSpecificConfig()
{
  uimsbf(1) isBaseLayer;
  if (isBaseLayer) {
    PARAconfig();
  }
  else {
    HILNenexConfig();
  }
}

PARAconfig()
{
  uimsbf(2) PARAmode;
  if (PARAmode != 1) {
    ErHVXCconfig();
  }
  if (PARAmode != 0) {
    HILNconfig();
  }
  uimsbf(1) PARAextensionFlag;
  if (PARAextensionFlag) {
    /* to be defined in MPEG-4 Phase 3 */
  }
}

HILNconfig()
{
  uimsbf(1) HILNquantMode;
  uimsbf(8) HILNmaxNumLine;
  uimsbf(4) HILNsampleRateCode;
  uimsbf(12) HILNframeLength;
  uimsbf(2) HILNcontMode;
}

HILNenexConfig()
{
  uimsbf(1) HILNenhaLayer;
  if (HILNenhaLayer) {
    uimsbf(2) HILNenhaQuantMode;
  }
}

SSCSpecificConfig(channelConfiguration)
{
  uimsbf(2) decoder_level;
  uimsbf(4) update_rate;
  uimsbf(2) synthesis_method;
  if (channelConfiguration != 1) {
    uimsbf(2) mode_ext;
    if ((channelConfiguration == 2) && (mode_ext == 1)) {
      uimsbf(2) reserved;
    }
  }
}

MPEG_1_2_SpecificConfig()
{
  bslbf(1) extension;
}

DSTSpecificConfig(channelConfiguration) {
  UiMsbf(1) DSDDST_Coded;
  UiMsbf(14) N_Channels;
  UiMsbf(1) reserved;
}

ALSSpecificConfig()
{
  uimsbf(32) als_id;
  uimsbf(32) samp_freq;
  uimsbf(32) samples;
  uimsbf(16) channels;
  uimsbf(3) file_type;
  uimsbf(3) resolution;
  uimsbf(1) floating;
  uimsbf(1) msb_first;
  uimsbf(16) frame_length;
  uimsbf(8) random_access;
  uimsbf(2) ra_flag;
  uimsbf(1) adapt_order;
  uimsbf(2) coef_table;
  uimsbf(1) long_term_prediction;
  uimsbf(10) max_order;
  uimsbf(2) block_switching;
  uimsbf(1) bgmc_mode;
  uimsbf(1) sb_part;
  uimsbf(1) joint_stereo;
  uimsbf(1) mc_coding;
  uimsbf(1) chan_config;
  uimsbf(1) chan_sort;
  uimsbf(1) crc_enabled;
  uimsbf(1) RLSLMS
  uimsbf(5) reserved
  uimsbf(1) aux_data_enabled;
  if (chan_config) {
    uimsbf(16) chan_config_info;
  }
  if (chan_sort) {
    for (c = 0; c <= channels; c++)
      uimsbf(1) chan_pos[c]; // 1..16 uimsbf 
  }
  bslbf(1) byte_align; // TODO: 0..7 bslbf 
  uimsbf(32) header_size;
  uimsbf(32) trailer_size;
  bslbf(header_size * 8) orig_header[];
  bslbf(trailer_size * 8) orig_trailer[];
  if (crc_enabled) {
    uimsbf(32) crc;
  }
  if ((ra_flag == 2) && (random_access > 0)) {
    for (f = 0; f < ((samples - 1) / (frame_length + 1)) + 1; f++) {
      uimsbf(32) ra_unit_size[f];
    }
  }
  if (aux_data_enabled) {
    uimsbf(32) aux_size;
    bslbf(aux_size * 8) aux_data[];
  }
}

SLSSpecificConfig(samplingFrequencyIndex,
  channelConfiguration,
  audioObjectType)
{
  uimsbf(3) pcmWordLength;
  uimsbf(1) aac_core_present;
  uimsbf(1) lle_main_stream;
  uimsbf(1) reserved_bit;
  uimsbf(3) frameLength;
  if (!channelConfiguration) {
    program_config_element();
  }
}

ELDSpecificConfig(channelConfiguration)
{
  bslbf(1) frameLengthFlag;
  bslbf(1) aacSectionDataResilienceFlag;
  bslbf(1) aacScalefactorDataResilienceFlag;
  bslbf(1) aacSpectralDataResilienceFlag;

  bslbf(1) ldSbrPresentFlag;
  if (ldSbrPresentFlag) {
    bslbf(1) ldSbrSamplingRate;
    bslbf(1) ldSbrCrcFlag;
    ld_sbr_header(channelConfiguration);
  }

  while (bslbf(4) eldExtType != ELDEXT_TERM) {
    uimsbf(4) eldExtLen;
    len = eldExtLen;
    if (eldExtLen == 15) {
      eldExtLenAdd;
      len += eldExtLenAdd;
    }
    if (eldExtLenAdd == 255) {
      eldExtLenAddAdd;
      len += eldExtLenAddAdd;
    }
    switch (eldExtType) {
      /* add future eld extension configs here */
      default:
        for (cnt = 0; cnt < len; cnt++) {
          other_byte;
        }
        break;
    }
  }
}

ld_sbr_header(channelConfiguration)
{
  switch (channelConfiguration) {
    case 1:
    case 2:
      numSbrHeader = 1;
      break;
    case 3:
      numSbrHeader = 2;
      break;
    case 4:
    case 5:
    case 6:
      numSbrHeader = 3;
      break;
    case 7:
      numSbrHeader = 4;
      break;
    default:
      numSbrHeader = 0;
      break;
  }
  for (el = 0; el < numSbrHeader; el++) {
    sbr_header();
  }
}

sbr_header()
{
  uimsbf(1) bs_amp_res;
  uimsbf(4) bs_start_freq;
  uimsbf(4) bs_stop_freq;
  uimsbf(3) bs_xover_band;
  uimsbf(2) bs_reserved;
  uimsbf(1) bs_header_extra_1;
  uimsbf(1) bs_header_extra_2;

  if (bs_header_extra_1) {
    uimsbf(2) bs_freq_scale;
    uimsbf(1) bs_alter_scale;
    uimsbf(2) bs_noise_bands;
  }

  if (bs_header_extra_2) {
    uimsbf(2) bs_limiter_bands;
    uimsbf(2) bs_limiter_gains;
    uimsbf(1) bs_interpol_freq;
    uimsbf(1) bs_smoothing_mode;
  }
}

ErrorProtectionSpecificConfig()
{
  uimsbf(8) number_of_predefined_set;
  uimsbf(2) interleave_type;
  uimsbf(3) bit_stuffing;
  uimsbf(3) number_of_concatenated_frame;
  for (i = 0; i < number_of_predefined_set; i++) {
    uimsbf(6) number_of_class[i];
    for (j = 0; j < number_of_class[i]; j++) {
      uimsbf(1) length_escape[i][j];
      uimsbf(1) rate_escape[i][j];
      uimsbf(1) crclen_escape[i][j];
      if (number_of_concatenated_frame != 1) {
        uimsbf(1) concatenate_flag[i][j];
      }
      uimsbf(2) fec_type[i][j];
      if (fec_type[i][j] == 0) {
        uimsbf(1) termination_switch[i][j];
      }
      if (interleave_type == 2) {
        uimsbf(2) interleave_switch[i][j];
      }
      uimsbf(1) class_optional;
      if (length_escape[i][j] == 1) { /* ESC */
        uimsbf(4) number_of_bits_for_length[i][j];
      }
      else {
        uimsbf(16) class_length[i][j];
      }
      if (rate_escape[i][j] != 1) { /* not ESC */
        if (fec_type[i][j]) {
          uimsbf(7) class_rate[i][j];
        } else {
          uimsbf(5) class_rate[i][j];
        }
      }
      if (crclen_escape[i][j] != 1) {  /* not ESC */
        uimsbf(5) class_crclen[i][j];
      }
    }
    uimsbf(1) class_reordered_output;
    if (class_reordered_output == 1) {
      for (j = 0; j < number_of_class[i]; j++) {
        uimsbf(6) class_output_order[i][j];
      }
    }
  }
  uimsbf(1) header_protection;
  if (header_protection == 1) {
    uimsbf(5) header_rate;
    uimsbf(5) header_crclen;
  }
}

class StructuredAudioSpecificConfig {  // the bitstream header  
  bit(1) more_data = 1;
  while(more_data) {  // shall have at least one chunk 
    bit(3) chunk_type;
    switch (chunk_type) {
      case 0b000: orc_file orc; break;
      case 0b001: score_file score; break;
      case 0b010: midi_file SMF; break;
      case 0b011: sample samp; break;
      case 0b100: sbf sample_bank; break;
      case 0b101: symtable sym; break;
    }
    bit(1) more_data;
  }
}

/********************************* 
    orchestra file definitions 
***********************************/
class orch_token {              // a token in an orchestra 
  int done; 
  unsigned int(8) token;         // see standard token table, Annex 5.A 
  switch(token) { 
  case 0xF0 :                   // a symbol 
    symbol sym;                 // the symbol name 
    break;      
  case 0xF1 :                   // a constant value 
    float(32) val;              // the floating-point value 
    break; 
  case 0xF2 :
      // a constant int value 
    unsigned int(32) val;       // the integer value 
    break; 
  case 0xF3 :                   // a string constant  
    int(8) length; 
    unsigned int(8) str[length]; // strings no more than 255 chars 
    break; 
  case 0xF4 :                   // a one-byte constant 
    int(8) val;
    break; 
  case 0xFF : // end of orch 
    done = 1;
    break;
  }
}
class orc_file {                // a whole orch file 
  unsigned int(16) length; 
  orch_token data[length];
}

/********************************* 
      score file definitions 
***********************************/
class instr_event {            // a note-on event 
  bit(1) has_label;
  if(has_label) 
    symbol label; 
  symbol iname_sym;            // the instrument name 
  float(32) dur;               // note duration 
  unsigned int(8) num_pf;
  float(32) pf[num_pf];        // all the pfields (no more than 255) 
}
class control_event {          // a control event 
  bit(1) has_label;
  if(has_label) 
    symbol label; 
  symbol varsym;               // the controller name 
  float(32) value;             // the new value 
}
class table_event { 
  symbol tname;       // the name of the table 
  bit(1) destroy;     // a table destructor 
  if (!destroy) { 
    token tgen;         // a core wavetable generator 
    bit(1) refers_to_sample;
    if (refers_to_sample) 
      symbol table_sym;          // the name of the sample 
    unsigned int(16) num_pf;     // the number of pfields 
    if (tgen == 0x7D) {
    // concat 
    float(32) size; 
    symbol ft[num_pf - 1];
  } else {
    float(32) pf[num_pf];
  }
  // when coding sample generator, leave a blank array slot 
  // for "which" parameter, to maintain alignment for "skip" parameter 
 } 
}

class end_event {
  // fixed at nothing 
}
class tempo_event {  // a tempo event 
  float(32) tempo;
}
class score_line {
  bit(1) has_time;
  if(has_time) {
    bit(1) use_if_late;
    float(32) time;              // the event time 
  }
  bit(1) high_priority;
  bit(3) type;
  switch(type) { 
    case 0b000 : instr_event inst; break; 
    case 0b001 : control_event control; break; 
    case 0b010 : table_event table; break; 
    case 0b100 : end_event end; break; 
    case 0b101 : tempo_event tempo; break;
  }
}
class score_file { 
  unsigned int(20) num_lines;  // a whole score file 
  score_line lines[num_lines];
}

/********************************* 
         MIDI definitions 
***********************************/
class midi_event { 
  unsigned int(24) length        
  unsigned int(8) data[length];
}
class midi_file { 
  unsigned int(32) length; 
  unsigned int(8) data[length];
}

/********************************** 
  sample data 
************************************/
class sample {
  /* note that 'sample' can be used for any big chunk of data 
     that needs to get into a wavetable */ 
  symbol sample_name_sym; 
  unsigned int(24) length;  // length in samples  
  bit(1) has_srate;
  if (has_srate) 
    unsigned int(17) srate; // sampling rate (needs to go to 96 KHz) 
  bit(1) has_loop;
  if (has_loop) { 
    unsigned int(24) loopstart; // loop points in samples 
    unsigned int(24) loopend;
  }
  bit(1) has_base;
  if (has_base)
    float(32) basecps;         // base freq in Hz 
  bit(1) float_sample;
  if (float_sample) {
    float(32) float_sample_data[length]; // data as floats ... 
  }
  else {
    int(16) sample_data[length]; // ... or as ints 
  } 
}

class sbf {
  int(32) length;
  int(8) data[length];
} 
