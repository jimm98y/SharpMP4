using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SharpH26X;

namespace SharpH266
{

    public partial class H266Context : IItuContext
    {
        public NalUnit NalHeader { get; set; }
        public DecodingCapabilityInformationRbsp DecodingCapabilityInformationRbsp { get; set; }
        public OperatingPointInformationRbsp OperatingPointInformationRbsp { get; set; }
        public VideoParameterSetRbsp VideoParameterSetRbsp { get; set; }
        public SeqParameterSetRbsp SeqParameterSetRbsp { get; set; }
        public PicParameterSetRbsp PicParameterSetRbsp { get; set; }
        public AdaptationParameterSetRbsp AdaptationParameterSetRbsp { get; set; }
        public SliceLayerRbsp SliceLayerRbsp { get; set; }
        public PictureHeaderRbsp PictureHeaderRbsp { get; set; }
        public SeiRbsp SeiRbsp { get; set; }
        public AccessUnitDelimiterRbsp AccessUnitDelimiterRbsp { get; set; }
        public EndOfSeqRbsp EndOfSeqRbsp { get; set; }
        public EndOfBitstreamRbsp EndOfBitstreamRbsp { get; set; }
        public FillerDataRbsp FillerDataRbsp { get; set; }

    }

    /*
nal_unit( NumBytesInNalUnit ) { 
nal_unit_header()  
/*NumBytesInRbsp = 0  *//*
/*for( i = 2; i < NumBytesInNalUnit; i++ )  *//*
/*if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 )  ==  0x000003 ) {  *//*
/*rbsp_byte[ NumBytesInRbsp++ ] b(8) *//*
/*rbsp_byte[ NumBytesInRbsp++ ] b(8) *//*
/*i  +=  2  *//*
/*emulation_prevention_three_byte*//*  /* equal to 0x03 *//*/* f(8) *//*
/*} else  *//*
/*rbsp_byte[ NumBytesInRbsp++ ] b(8) *//*
}
    */
    public class NalUnit : IItuSerializable
    {
        private uint numBytesInNalUnit;
        public uint NumBytesInNalUnit { get { return numBytesInNalUnit; } set { numBytesInNalUnit = value; } }
        private NalUnitHeader nal_unit_header;
        public NalUnitHeader NalUnitHeader { get { return nal_unit_header; } set { nal_unit_header = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NalUnit(uint NumBytesInNalUnit)
        {
            this.numBytesInNalUnit = NumBytesInNalUnit;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            this.nal_unit_header = new NalUnitHeader();
            size += stream.ReadClass<NalUnitHeader>(size, context, this.nal_unit_header); //NumBytesInRbsp = 0  
            /* for( i = 2; i < NumBytesInNalUnit; i++ )   */

            /* if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 )  ==  0x000003 ) {   */

            /* rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /* rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /* i  +=  2   */

            /* emulation_prevention_three_byte */

            /*  equal to 0x03  */

            /*  f(8)  */

            /* } else   */

            /* rbsp_byte[ NumBytesInRbsp++ ] b(8)  */


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<NalUnitHeader>(context, this.nal_unit_header); //NumBytesInRbsp = 0  
            /* for( i = 2; i < NumBytesInNalUnit; i++ )   */

            /* if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 )  ==  0x000003 ) {   */

            /* rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /* rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /* i  +=  2   */

            /* emulation_prevention_three_byte */

            /*  equal to 0x03  */

            /*  f(8)  */

            /* } else   */

            /* rbsp_byte[ NumBytesInRbsp++ ] b(8)  */


            return size;
        }

    }

    /*


nal_unit_header() {  
 forbidden_zero_bit f(1) 
 nuh_reserved_zero_bit u(1) 
 nuh_layer_id u(6) 
 nal_unit_type u(5) 
 nuh_temporal_id_plus1 u(3) 
}
    */
    public class NalUnitHeader : IItuSerializable
    {
        private uint forbidden_zero_bit;
        public uint ForbiddenZeroBit { get { return forbidden_zero_bit; } set { forbidden_zero_bit = value; } }
        private byte nuh_reserved_zero_bit;
        public byte NuhReservedZeroBit { get { return nuh_reserved_zero_bit; } set { nuh_reserved_zero_bit = value; } }
        private uint nuh_layer_id;
        public uint NuhLayerId { get { return nuh_layer_id; } set { nuh_layer_id = value; } }
        private uint nal_unit_type;
        public uint NalUnitType { get { return nal_unit_type; } set { nal_unit_type = value; } }
        private uint nuh_temporal_id_plus1;
        public uint NuhTemporalIdPlus1 { get { return nuh_temporal_id_plus1; } set { nuh_temporal_id_plus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NalUnitHeader()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadFixed(size, 1, out this.forbidden_zero_bit);
            size += stream.ReadUnsignedInt(size, 1, out this.nuh_reserved_zero_bit);
            size += stream.ReadUnsignedInt(size, 6, out this.nuh_layer_id);
            size += stream.ReadUnsignedInt(size, 5, out this.nal_unit_type);
            size += stream.ReadUnsignedInt(size, 3, out this.nuh_temporal_id_plus1);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteFixed(1, this.forbidden_zero_bit);
            size += stream.WriteUnsignedInt(1, this.nuh_reserved_zero_bit);
            size += stream.WriteUnsignedInt(6, this.nuh_layer_id);
            size += stream.WriteUnsignedInt(5, this.nal_unit_type);
            size += stream.WriteUnsignedInt(3, this.nuh_temporal_id_plus1);

            return size;
        }

    }

    /*
  

decoding_capability_information_rbsp() {  
 dci_reserved_zero_4bits u(4) 
 dci_num_ptls_minus1 u(4) 
 for( i = 0; i  <=  dci_num_ptls_minus1; i++ )  
  profile_tier_level( 1, 0 )  
 dci_extension_flag u(1) 
 if( dci_extension_flag )  
  while( more_rbsp_data() )  
   dci_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}
    */
    public class DecodingCapabilityInformationRbsp : IItuSerializable
    {
        private uint dci_reserved_zero_4bits;
        public uint DciReservedZero4bits { get { return dci_reserved_zero_4bits; } set { dci_reserved_zero_4bits = value; } }
        private uint dci_num_ptls_minus1;
        public uint DciNumPtlsMinus1 { get { return dci_num_ptls_minus1; } set { dci_num_ptls_minus1 = value; } }
        private ProfileTierLevel[] profile_tier_level;
        public ProfileTierLevel[] ProfileTierLevel { get { return profile_tier_level; } set { profile_tier_level = value; } }
        private byte dci_extension_flag;
        public byte DciExtensionFlag { get { return dci_extension_flag; } set { dci_extension_flag = value; } }
        private Dictionary<int, byte> dci_extension_data_flag = new Dictionary<int, byte>();
        public Dictionary<int, byte> DciExtensionDataFlag { get { return dci_extension_data_flag; } set { dci_extension_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DecodingCapabilityInformationRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 4, out this.dci_reserved_zero_4bits);
            size += stream.ReadUnsignedInt(size, 4, out this.dci_num_ptls_minus1);

            this.profile_tier_level = new ProfileTierLevel[dci_num_ptls_minus1 + 1];
            for (i = 0; i <= dci_num_ptls_minus1; i++)
            {
                this.profile_tier_level[i] = new ProfileTierLevel(1, 0);
                size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level[i]);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.dci_extension_flag);

            if (dci_extension_flag != 0)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.dci_extension_data_flag);
                }
            }
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(4, this.dci_reserved_zero_4bits);
            size += stream.WriteUnsignedInt(4, this.dci_num_ptls_minus1);

            for (i = 0; i <= dci_num_ptls_minus1; i++)
            {
                size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level[i]);
            }
            size += stream.WriteUnsignedInt(1, this.dci_extension_flag);

            if (dci_extension_flag != 0)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.dci_extension_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

operating_point_information_rbsp() {  
 opi_ols_info_present_flag u(1) 
 opi_htid_info_present_flag u(1) 
 if( opi_ols_info_present_flag )  
  opi_ols_idx ue(v) 
 if( opi_htid_info_present_flag )  
  opi_htid_plus1 u(3) 
 opi_extension_flag u(1) 
 if( opi_extension_flag )  
  while( more_rbsp_data() )  
   opi_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}
    */
    public class OperatingPointInformationRbsp : IItuSerializable
    {
        private byte opi_ols_info_present_flag;
        public byte OpiOlsInfoPresentFlag { get { return opi_ols_info_present_flag; } set { opi_ols_info_present_flag = value; } }
        private byte opi_htid_info_present_flag;
        public byte OpiHtidInfoPresentFlag { get { return opi_htid_info_present_flag; } set { opi_htid_info_present_flag = value; } }
        private uint opi_ols_idx;
        public uint OpiOlsIdx { get { return opi_ols_idx; } set { opi_ols_idx = value; } }
        private uint opi_htid_plus1;
        public uint OpiHtidPlus1 { get { return opi_htid_plus1; } set { opi_htid_plus1 = value; } }
        private byte opi_extension_flag;
        public byte OpiExtensionFlag { get { return opi_extension_flag; } set { opi_extension_flag = value; } }
        private Dictionary<int, byte> opi_extension_data_flag = new Dictionary<int, byte>();
        public Dictionary<int, byte> OpiExtensionDataFlag { get { return opi_extension_data_flag; } set { opi_extension_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public OperatingPointInformationRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 1, out this.opi_ols_info_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.opi_htid_info_present_flag);

            if (opi_ols_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.opi_ols_idx);
            }

            if (opi_htid_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.opi_htid_plus1);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.opi_extension_flag);

            if (opi_extension_flag != 0)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.opi_extension_data_flag);
                }
            }
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.WriteUnsignedInt(1, this.opi_ols_info_present_flag);
            size += stream.WriteUnsignedInt(1, this.opi_htid_info_present_flag);

            if (opi_ols_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.opi_ols_idx);
            }

            if (opi_htid_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(3, this.opi_htid_plus1);
            }
            size += stream.WriteUnsignedInt(1, this.opi_extension_flag);

            if (opi_extension_flag != 0)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.opi_extension_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

video_parameter_set_rbsp() {  
 vps_video_parameter_set_id u(4)
  vps_max_layers_minus1 u(6) 
 vps_max_sublayers_minus1 u(3) 
 if( vps_max_layers_minus1 > 0  &&  vps_max_sublayers_minus1 > 0 )  
  vps_default_ptl_dpb_hrd_max_tid_flag u(1) 
 if( vps_max_layers_minus1 > 0 )  
  vps_all_independent_layers_flag u(1) 
 for( i = 0; i  <=  vps_max_layers_minus1; i++ ) {  
  vps_layer_id[ i ] u(6) 
  if( i > 0  &&  !vps_all_independent_layers_flag ) {  
   vps_independent_layer_flag[ i ] u(1) 
   if( !vps_independent_layer_flag[ i ] ) {  
    vps_max_tid_ref_present_flag[ i ] u(1) 
    for( j = 0; j < i; j++ ) {  
     vps_direct_ref_layer_flag[ i ][ j ] u(1) 
     if( vps_max_tid_ref_present_flag[ i ]  &&  vps_direct_ref_layer_flag[ i ][ j ] )  
      vps_max_tid_il_ref_pics_plus1[ i ][ j ] u(3) 
    }  
   }  
  }  
 }  
 if( vps_max_layers_minus1 > 0 ) {  
  if( vps_all_independent_layers_flag )  
   vps_each_layer_is_an_ols_flag u(1) 
  if( !vps_each_layer_is_an_ols_flag ) {  
   if( !vps_all_independent_layers_flag )  
    vps_ols_mode_idc u(2) 
   if( vps_ols_mode_idc  ==  2 ) {  
    vps_num_output_layer_sets_minus2 u(8) 
    for( i = 1; i  <=  vps_num_output_layer_sets_minus2 + 1; i ++ )  
     for( j = 0; j  <=  vps_max_layers_minus1; j++ )  
      vps_ols_output_layer_flag[ i ][ j ] u(1) 
   }  
  }  
  vps_num_ptls_minus1 u(8) 
 }  
 for( i = 0; i  <=  vps_num_ptls_minus1; i++ ) {  
  if( i > 0 )  
   vps_pt_present_flag[ i ] u(1) 
  if( !vps_default_ptl_dpb_hrd_max_tid_flag )  
   vps_ptl_max_tid[ i ] u(3) 
 }  
 while( !byte_aligned() )  
  vps_ptl_alignment_zero_bit  /* equal to 0 *//* f(1) 
 for( i = 0; i  <=  vps_num_ptls_minus1; i++ )  
  profile_tier_level( vps_pt_present_flag[ i ], vps_ptl_max_tid[ i ] )  
 for( i = 0; i < TotalNumOlss; i++ )  
  if( vps_num_ptls_minus1 > 0  &&  vps_num_ptls_minus1 + 1  !=  TotalNumOlss )  
   vps_ols_ptl_idx[ i ] u(8) 
    if( !vps_each_layer_is_an_ols_flag ) {  
  vps_num_dpb_params_minus1 ue(v) 
  if( vps_max_sublayers_minus1 > 0 )  
   vps_sublayer_dpb_params_present_flag u(1) 
  for( i = 0; i < VpsNumDpbParams; i++ ) {  
   if( !vps_default_ptl_dpb_hrd_max_tid_flag )  
    vps_dpb_max_tid[ i ] u(3) 
   dpb_parameters( vps_dpb_max_tid[ i ], 
     vps_sublayer_dpb_params_present_flag ) 
 
  }  
  for( i = 0; i < NumMultiLayerOlss; i++ ) {  
   vps_ols_dpb_pic_width[ i ] ue(v) 
   vps_ols_dpb_pic_height[ i ] ue(v) 
   vps_ols_dpb_chroma_format[ i ] u(2) 
   vps_ols_dpb_bitdepth_minus8[ i ] ue(v) 
   if( VpsNumDpbParams > 1  &&  VpsNumDpbParams  !=  NumMultiLayerOlss )  
    vps_ols_dpb_params_idx[ i ] ue(v) 
  }  
  vps_timing_hrd_params_present_flag u(1) 
  if( vps_timing_hrd_params_present_flag ) {  
   general_timing_hrd_parameters()  
   if( vps_max_sublayers_minus1 > 0 )  
    vps_sublayer_cpb_params_present_flag u(1) 
   vps_num_ols_timing_hrd_params_minus1 ue(v) 
   for( i = 0; i  <=  vps_num_ols_timing_hrd_params_minus1; i++ ) {  
    if( !vps_default_ptl_dpb_hrd_max_tid_flag )  
     vps_hrd_max_tid[ i ] u(3) 
    firstSubLayer = vps_sublayer_cpb_params_present_flag != 0 ? 0 : vps_hrd_max_tid[ i ]  
    ols_timing_hrd_parameters( firstSubLayer, vps_hrd_max_tid[ i ] )  
   }  
   if( vps_num_ols_timing_hrd_params_minus1 > 0  && 
     vps_num_ols_timing_hrd_params_minus1 + 1  !=  NumMultiLayerOlss ) 
 
    for( i = 0; i < NumMultiLayerOlss; i++ )  
     vps_ols_timing_hrd_idx[ i ] ue(v) 
  }  
 }  
 vps_extension_flag u(1) 
 if( vps_extension_flag )  
  while( more_rbsp_data() )  
   vps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}
    */
    public class VideoParameterSetRbsp : IItuSerializable
    {
        private uint vps_video_parameter_set_id;
        public uint VpsVideoParameterSetId { get { return vps_video_parameter_set_id; } set { vps_video_parameter_set_id = value; } }
        private uint vps_max_layers_minus1;
        public uint VpsMaxLayersMinus1 { get { return vps_max_layers_minus1; } set { vps_max_layers_minus1 = value; } }
        private uint vps_max_sublayers_minus1;
        public uint VpsMaxSublayersMinus1 { get { return vps_max_sublayers_minus1; } set { vps_max_sublayers_minus1 = value; } }
        private byte vps_default_ptl_dpb_hrd_max_tid_flag;
        public byte VpsDefaultPtlDpbHrdMaxTidFlag { get { return vps_default_ptl_dpb_hrd_max_tid_flag; } set { vps_default_ptl_dpb_hrd_max_tid_flag = value; } }
        private byte vps_all_independent_layers_flag;
        public byte VpsAllIndependentLayersFlag { get { return vps_all_independent_layers_flag; } set { vps_all_independent_layers_flag = value; } }
        private uint[] vps_layer_id;
        public uint[] VpsLayerId { get { return vps_layer_id; } set { vps_layer_id = value; } }
        private byte[] vps_independent_layer_flag;
        public byte[] VpsIndependentLayerFlag { get { return vps_independent_layer_flag; } set { vps_independent_layer_flag = value; } }
        private byte[] vps_max_tid_ref_present_flag;
        public byte[] VpsMaxTidRefPresentFlag { get { return vps_max_tid_ref_present_flag; } set { vps_max_tid_ref_present_flag = value; } }
        private byte[][] vps_direct_ref_layer_flag;
        public byte[][] VpsDirectRefLayerFlag { get { return vps_direct_ref_layer_flag; } set { vps_direct_ref_layer_flag = value; } }
        private uint[][] vps_max_tid_il_ref_pics_plus1;
        public uint[][] VpsMaxTidIlRefPicsPlus1 { get { return vps_max_tid_il_ref_pics_plus1; } set { vps_max_tid_il_ref_pics_plus1 = value; } }
        private byte vps_each_layer_is_an_ols_flag;
        public byte VpsEachLayerIsAnOlsFlag { get { return vps_each_layer_is_an_ols_flag; } set { vps_each_layer_is_an_ols_flag = value; } }
        private uint vps_ols_mode_idc;
        public uint VpsOlsModeIdc { get { return vps_ols_mode_idc; } set { vps_ols_mode_idc = value; } }
        private uint vps_num_output_layer_sets_minus2;
        public uint VpsNumOutputLayerSetsMinus2 { get { return vps_num_output_layer_sets_minus2; } set { vps_num_output_layer_sets_minus2 = value; } }
        private byte[][] vps_ols_output_layer_flag;
        public byte[][] VpsOlsOutputLayerFlag { get { return vps_ols_output_layer_flag; } set { vps_ols_output_layer_flag = value; } }
        private uint vps_num_ptls_minus1;
        public uint VpsNumPtlsMinus1 { get { return vps_num_ptls_minus1; } set { vps_num_ptls_minus1 = value; } }
        private byte[] vps_pt_present_flag;
        public byte[] VpsPtPresentFlag { get { return vps_pt_present_flag; } set { vps_pt_present_flag = value; } }
        private uint[] vps_ptl_max_tid;
        public uint[] VpsPtlMaxTid { get { return vps_ptl_max_tid; } set { vps_ptl_max_tid = value; } }
        private Dictionary<int, uint> vps_ptl_alignment_zero_bit = new Dictionary<int, uint>();
        public Dictionary<int, uint> VpsPtlAlignmentZeroBit { get { return vps_ptl_alignment_zero_bit; } set { vps_ptl_alignment_zero_bit = value; } }
        private ProfileTierLevel[] profile_tier_level;
        public ProfileTierLevel[] ProfileTierLevel { get { return profile_tier_level; } set { profile_tier_level = value; } }
        private uint[] vps_ols_ptl_idx;
        public uint[] VpsOlsPtlIdx { get { return vps_ols_ptl_idx; } set { vps_ols_ptl_idx = value; } }
        private uint vps_num_dpb_params_minus1;
        public uint VpsNumDpbParamsMinus1 { get { return vps_num_dpb_params_minus1; } set { vps_num_dpb_params_minus1 = value; } }
        private byte vps_sublayer_dpb_params_present_flag;
        public byte VpsSublayerDpbParamsPresentFlag { get { return vps_sublayer_dpb_params_present_flag; } set { vps_sublayer_dpb_params_present_flag = value; } }
        private uint[] vps_dpb_max_tid;
        public uint[] VpsDpbMaxTid { get { return vps_dpb_max_tid; } set { vps_dpb_max_tid = value; } }
        private DpbParameters[] dpb_parameters;
        public DpbParameters[] DpbParameters { get { return dpb_parameters; } set { dpb_parameters = value; } }
        private uint[] vps_ols_dpb_pic_width;
        public uint[] VpsOlsDpbPicWidth { get { return vps_ols_dpb_pic_width; } set { vps_ols_dpb_pic_width = value; } }
        private uint[] vps_ols_dpb_pic_height;
        public uint[] VpsOlsDpbPicHeight { get { return vps_ols_dpb_pic_height; } set { vps_ols_dpb_pic_height = value; } }
        private uint[] vps_ols_dpb_chroma_format;
        public uint[] VpsOlsDpbChromaFormat { get { return vps_ols_dpb_chroma_format; } set { vps_ols_dpb_chroma_format = value; } }
        private uint[] vps_ols_dpb_bitdepth_minus8;
        public uint[] VpsOlsDpbBitdepthMinus8 { get { return vps_ols_dpb_bitdepth_minus8; } set { vps_ols_dpb_bitdepth_minus8 = value; } }
        private uint[] vps_ols_dpb_params_idx;
        public uint[] VpsOlsDpbParamsIdx { get { return vps_ols_dpb_params_idx; } set { vps_ols_dpb_params_idx = value; } }
        private byte vps_timing_hrd_params_present_flag;
        public byte VpsTimingHrdParamsPresentFlag { get { return vps_timing_hrd_params_present_flag; } set { vps_timing_hrd_params_present_flag = value; } }
        private GeneralTimingHrdParameters general_timing_hrd_parameters;
        public GeneralTimingHrdParameters GeneralTimingHrdParameters { get { return general_timing_hrd_parameters; } set { general_timing_hrd_parameters = value; } }
        private byte vps_sublayer_cpb_params_present_flag;
        public byte VpsSublayerCpbParamsPresentFlag { get { return vps_sublayer_cpb_params_present_flag; } set { vps_sublayer_cpb_params_present_flag = value; } }
        private uint vps_num_ols_timing_hrd_params_minus1;
        public uint VpsNumOlsTimingHrdParamsMinus1 { get { return vps_num_ols_timing_hrd_params_minus1; } set { vps_num_ols_timing_hrd_params_minus1 = value; } }
        private uint[] vps_hrd_max_tid;
        public uint[] VpsHrdMaxTid { get { return vps_hrd_max_tid; } set { vps_hrd_max_tid = value; } }
        private OlsTimingHrdParameters[] ols_timing_hrd_parameters;
        public OlsTimingHrdParameters[] OlsTimingHrdParameters { get { return ols_timing_hrd_parameters; } set { ols_timing_hrd_parameters = value; } }
        private uint[] vps_ols_timing_hrd_idx;
        public uint[] VpsOlsTimingHrdIdx { get { return vps_ols_timing_hrd_idx; } set { vps_ols_timing_hrd_idx = value; } }
        private byte vps_extension_flag;
        public byte VpsExtensionFlag { get { return vps_extension_flag; } set { vps_extension_flag = value; } }
        private Dictionary<int, byte> vps_extension_data_flag = new Dictionary<int, byte>();
        public Dictionary<int, byte> VpsExtensionDataFlag { get { return vps_extension_data_flag; } set { vps_extension_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VideoParameterSetRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            int whileIndex = -1;
            uint firstSubLayer = 0;
            size += stream.ReadUnsignedInt(size, 4, out this.vps_video_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 6, out this.vps_max_layers_minus1);
            size += stream.ReadUnsignedInt(size, 3, out this.vps_max_sublayers_minus1);

            if (vps_max_layers_minus1 > 0 && vps_max_sublayers_minus1 > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.vps_default_ptl_dpb_hrd_max_tid_flag);
            }

            if (vps_max_layers_minus1 > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.vps_all_independent_layers_flag);
            }

            this.vps_layer_id = new uint[vps_max_layers_minus1 + 1];
            this.vps_independent_layer_flag = new byte[vps_max_layers_minus1 + 1];
            this.vps_max_tid_ref_present_flag = new byte[vps_max_layers_minus1 + 1];
            this.vps_direct_ref_layer_flag = new byte[vps_max_layers_minus1 + 1][];
            this.vps_max_tid_il_ref_pics_plus1 = new uint[vps_max_layers_minus1 + 1][];
            for (i = 0; i <= vps_max_layers_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.vps_layer_id[i]);

                if (i > 0 && vps_all_independent_layers_flag == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vps_independent_layer_flag[i]);

                    if (vps_independent_layer_flag[i] == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.vps_max_tid_ref_present_flag[i]);

                        this.vps_direct_ref_layer_flag[i] = new byte[i];
                        this.vps_max_tid_il_ref_pics_plus1[i] = new uint[i];
                        for (j = 0; j < i; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.vps_direct_ref_layer_flag[i][j]);
                            ((H266Context)context).OnVpsDirectRefLayerFlag();

                            if (vps_max_tid_ref_present_flag[i] != 0 && vps_direct_ref_layer_flag[i][j] != 0)
                            {
                                size += stream.ReadUnsignedInt(size, 3, out this.vps_max_tid_il_ref_pics_plus1[i][j]);
                            }
                        }
                    }
                }
            }

            if (vps_max_layers_minus1 > 0)
            {

                if (vps_all_independent_layers_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vps_each_layer_is_an_ols_flag);
                }

                if (vps_each_layer_is_an_ols_flag == 0)
                {

                    if (vps_all_independent_layers_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 2, out this.vps_ols_mode_idc);
                    }

                    if (vps_ols_mode_idc == 2)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.vps_num_output_layer_sets_minus2);
                        ((H266Context)context).OnVpsNumOutputLayerSetsMinus2();

                        this.vps_ols_output_layer_flag = new byte[vps_num_output_layer_sets_minus2 + 1][];
                        for (i = 1; i <= vps_num_output_layer_sets_minus2 + 1; i++)
                        {

                            this.vps_ols_output_layer_flag[i] = new byte[vps_max_layers_minus1 + 1];
                            for (j = 0; j <= vps_max_layers_minus1; j++)
                            {
                                size += stream.ReadUnsignedInt(size, 1, out this.vps_ols_output_layer_flag[i][j]);
                                ((H266Context)context).OnVpsOlsOutputLayerFlag(j);
                            }
                        }
                    }
                }
                size += stream.ReadUnsignedInt(size, 8, out this.vps_num_ptls_minus1);
            }

            this.vps_pt_present_flag = new byte[vps_num_ptls_minus1 + 1];
            this.vps_ptl_max_tid = new uint[vps_num_ptls_minus1 + 1];
            for (i = 0; i <= vps_num_ptls_minus1; i++)
            {

                if (i > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vps_pt_present_flag[i]);
                }

                if (vps_default_ptl_dpb_hrd_max_tid_flag == 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.vps_ptl_max_tid[i]);
                }
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadFixed(size, 1, whileIndex, this.vps_ptl_alignment_zero_bit); // equal to 0 
            }

            this.profile_tier_level = new ProfileTierLevel[vps_num_ptls_minus1 + 1];
            for (i = 0; i <= vps_num_ptls_minus1; i++)
            {
                this.profile_tier_level[i] = new ProfileTierLevel(vps_pt_present_flag[i], vps_ptl_max_tid[i]);
                size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level[i]);
            }

            this.vps_ols_ptl_idx = new uint[((H266Context)context).TotalNumOlss];
            for (i = 0; i < ((H266Context)context).TotalNumOlss; i++)
            {

                if (vps_num_ptls_minus1 > 0 && vps_num_ptls_minus1 + 1 != ((H266Context)context).TotalNumOlss)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.vps_ols_ptl_idx[i]);
                }
            }

            if (vps_each_layer_is_an_ols_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_dpb_params_minus1);
                ((H266Context)context).OnVpsNumDpbParamsMinus1();

                if (vps_max_sublayers_minus1 > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vps_sublayer_dpb_params_present_flag);
                }

                this.vps_dpb_max_tid = new uint[((H266Context)context).VpsNumDpbParams];
                this.dpb_parameters = new DpbParameters[((H266Context)context).VpsNumDpbParams];
                for (i = 0; i < ((H266Context)context).VpsNumDpbParams; i++)
                {

                    if (vps_default_ptl_dpb_hrd_max_tid_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.vps_dpb_max_tid[i]);
                    }
                    this.dpb_parameters[i] = new DpbParameters(vps_dpb_max_tid[i], vps_sublayer_dpb_params_present_flag);
                    size += stream.ReadClass<DpbParameters>(size, context, this.dpb_parameters[i]);
                }

                this.vps_ols_dpb_pic_width = new uint[((H266Context)context).NumMultiLayerOlss];
                this.vps_ols_dpb_pic_height = new uint[((H266Context)context).NumMultiLayerOlss];
                this.vps_ols_dpb_chroma_format = new uint[((H266Context)context).NumMultiLayerOlss];
                this.vps_ols_dpb_bitdepth_minus8 = new uint[((H266Context)context).NumMultiLayerOlss];
                this.vps_ols_dpb_params_idx = new uint[((H266Context)context).NumMultiLayerOlss];
                for (i = 0; i < ((H266Context)context).NumMultiLayerOlss; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vps_ols_dpb_pic_width[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.vps_ols_dpb_pic_height[i]);
                    size += stream.ReadUnsignedInt(size, 2, out this.vps_ols_dpb_chroma_format[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.vps_ols_dpb_bitdepth_minus8[i]);

                    if (((H266Context)context).VpsNumDpbParams > 1 && ((H266Context)context).VpsNumDpbParams != ((H266Context)context).NumMultiLayerOlss)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.vps_ols_dpb_params_idx[i]);
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vps_timing_hrd_params_present_flag);

                if (vps_timing_hrd_params_present_flag != 0)
                {
                    this.general_timing_hrd_parameters = new GeneralTimingHrdParameters();
                    size += stream.ReadClass<GeneralTimingHrdParameters>(size, context, this.general_timing_hrd_parameters);
                    ((H266Context)context).SetGeneralTimingHrdParameters(general_timing_hrd_parameters);

                    if (vps_max_sublayers_minus1 > 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.vps_sublayer_cpb_params_present_flag);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_ols_timing_hrd_params_minus1);

                    this.vps_hrd_max_tid = new uint[vps_num_ols_timing_hrd_params_minus1 + 1];
                    this.ols_timing_hrd_parameters = new OlsTimingHrdParameters[vps_num_ols_timing_hrd_params_minus1 + 1];
                    for (i = 0; i <= vps_num_ols_timing_hrd_params_minus1; i++)
                    {

                        if (vps_default_ptl_dpb_hrd_max_tid_flag == 0)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.vps_hrd_max_tid[i]);
                        }
                        firstSubLayer = vps_sublayer_cpb_params_present_flag != 0 ? 0 : vps_hrd_max_tid[i];
                        this.ols_timing_hrd_parameters[i] = new OlsTimingHrdParameters(firstSubLayer, vps_hrd_max_tid[i]);
                        size += stream.ReadClass<OlsTimingHrdParameters>(size, context, this.ols_timing_hrd_parameters[i]);
                    }

                    if (vps_num_ols_timing_hrd_params_minus1 > 0 &&
     vps_num_ols_timing_hrd_params_minus1 + 1 != ((H266Context)context).NumMultiLayerOlss)
                    {

                        this.vps_ols_timing_hrd_idx = new uint[((H266Context)context).NumMultiLayerOlss];
                        for (i = 0; i < ((H266Context)context).NumMultiLayerOlss; i++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.vps_ols_timing_hrd_idx[i]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_extension_flag);

            if (vps_extension_flag != 0)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.vps_extension_data_flag);
                }
            }
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            int whileIndex = -1;
            uint firstSubLayer = 0;
            size += stream.WriteUnsignedInt(4, this.vps_video_parameter_set_id);
            size += stream.WriteUnsignedInt(6, this.vps_max_layers_minus1);
            size += stream.WriteUnsignedInt(3, this.vps_max_sublayers_minus1);

            if (vps_max_layers_minus1 > 0 && vps_max_sublayers_minus1 > 0)
            {
                size += stream.WriteUnsignedInt(1, this.vps_default_ptl_dpb_hrd_max_tid_flag);
            }

            if (vps_max_layers_minus1 > 0)
            {
                size += stream.WriteUnsignedInt(1, this.vps_all_independent_layers_flag);
            }

            for (i = 0; i <= vps_max_layers_minus1; i++)
            {
                size += stream.WriteUnsignedInt(6, this.vps_layer_id[i]);

                if (i > 0 && vps_all_independent_layers_flag == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vps_independent_layer_flag[i]);

                    if (vps_independent_layer_flag[i] == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.vps_max_tid_ref_present_flag[i]);

                        for (j = 0; j < i; j++)
                        {
                            size += stream.WriteUnsignedInt(1, this.vps_direct_ref_layer_flag[i][j]);
                            ((H266Context)context).OnVpsDirectRefLayerFlag();

                            if (vps_max_tid_ref_present_flag[i] != 0 && vps_direct_ref_layer_flag[i][j] != 0)
                            {
                                size += stream.WriteUnsignedInt(3, this.vps_max_tid_il_ref_pics_plus1[i][j]);
                            }
                        }
                    }
                }
            }

            if (vps_max_layers_minus1 > 0)
            {

                if (vps_all_independent_layers_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vps_each_layer_is_an_ols_flag);
                }

                if (vps_each_layer_is_an_ols_flag == 0)
                {

                    if (vps_all_independent_layers_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(2, this.vps_ols_mode_idc);
                    }

                    if (vps_ols_mode_idc == 2)
                    {
                        size += stream.WriteUnsignedInt(8, this.vps_num_output_layer_sets_minus2);
                        ((H266Context)context).OnVpsNumOutputLayerSetsMinus2();

                        for (i = 1; i <= vps_num_output_layer_sets_minus2 + 1; i++)
                        {

                            for (j = 0; j <= vps_max_layers_minus1; j++)
                            {
                                size += stream.WriteUnsignedInt(1, this.vps_ols_output_layer_flag[i][j]);
                                ((H266Context)context).OnVpsOlsOutputLayerFlag(j);
                            }
                        }
                    }
                }
                size += stream.WriteUnsignedInt(8, this.vps_num_ptls_minus1);
            }

            for (i = 0; i <= vps_num_ptls_minus1; i++)
            {

                if (i > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vps_pt_present_flag[i]);
                }

                if (vps_default_ptl_dpb_hrd_max_tid_flag == 0)
                {
                    size += stream.WriteUnsignedInt(3, this.vps_ptl_max_tid[i]);
                }
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteFixed(1, whileIndex, this.vps_ptl_alignment_zero_bit); // equal to 0 
            }

            for (i = 0; i <= vps_num_ptls_minus1; i++)
            {
                size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level[i]);
            }

            for (i = 0; i < ((H266Context)context).TotalNumOlss; i++)
            {

                if (vps_num_ptls_minus1 > 0 && vps_num_ptls_minus1 + 1 != ((H266Context)context).TotalNumOlss)
                {
                    size += stream.WriteUnsignedInt(8, this.vps_ols_ptl_idx[i]);
                }
            }

            if (vps_each_layer_is_an_ols_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.vps_num_dpb_params_minus1);
                ((H266Context)context).OnVpsNumDpbParamsMinus1();

                if (vps_max_sublayers_minus1 > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vps_sublayer_dpb_params_present_flag);
                }

                for (i = 0; i < ((H266Context)context).VpsNumDpbParams; i++)
                {

                    if (vps_default_ptl_dpb_hrd_max_tid_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(3, this.vps_dpb_max_tid[i]);
                    }
                    size += stream.WriteClass<DpbParameters>(context, this.dpb_parameters[i]);
                }

                for (i = 0; i < ((H266Context)context).NumMultiLayerOlss; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vps_ols_dpb_pic_width[i]);
                    size += stream.WriteUnsignedIntGolomb(this.vps_ols_dpb_pic_height[i]);
                    size += stream.WriteUnsignedInt(2, this.vps_ols_dpb_chroma_format[i]);
                    size += stream.WriteUnsignedIntGolomb(this.vps_ols_dpb_bitdepth_minus8[i]);

                    if (((H266Context)context).VpsNumDpbParams > 1 && ((H266Context)context).VpsNumDpbParams != ((H266Context)context).NumMultiLayerOlss)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.vps_ols_dpb_params_idx[i]);
                    }
                }
                size += stream.WriteUnsignedInt(1, this.vps_timing_hrd_params_present_flag);

                if (vps_timing_hrd_params_present_flag != 0)
                {
                    size += stream.WriteClass<GeneralTimingHrdParameters>(context, this.general_timing_hrd_parameters);
                    ((H266Context)context).SetGeneralTimingHrdParameters(general_timing_hrd_parameters);

                    if (vps_max_sublayers_minus1 > 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.vps_sublayer_cpb_params_present_flag);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.vps_num_ols_timing_hrd_params_minus1);

                    for (i = 0; i <= vps_num_ols_timing_hrd_params_minus1; i++)
                    {

                        if (vps_default_ptl_dpb_hrd_max_tid_flag == 0)
                        {
                            size += stream.WriteUnsignedInt(3, this.vps_hrd_max_tid[i]);
                        }
                        firstSubLayer = vps_sublayer_cpb_params_present_flag != 0 ? 0 : vps_hrd_max_tid[i];
                        size += stream.WriteClass<OlsTimingHrdParameters>(context, this.ols_timing_hrd_parameters[i]);
                    }

                    if (vps_num_ols_timing_hrd_params_minus1 > 0 &&
     vps_num_ols_timing_hrd_params_minus1 + 1 != ((H266Context)context).NumMultiLayerOlss)
                    {

                        for (i = 0; i < ((H266Context)context).NumMultiLayerOlss; i++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.vps_ols_timing_hrd_idx[i]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.vps_extension_flag);

            if (vps_extension_flag != 0)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.vps_extension_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
 

seq_parameter_set_rbsp() {  
 sps_seq_parameter_set_id u(4) 
 sps_video_parameter_set_id u(4)
 sps_max_sublayers_minus1 u(3) 
 sps_chroma_format_idc u(2) 
 sps_log2_ctu_size_minus5 u(2) 
 sps_ptl_dpb_hrd_params_present_flag u(1) 
 if( sps_ptl_dpb_hrd_params_present_flag )  
  profile_tier_level( 1, sps_max_sublayers_minus1 )  
 sps_gdr_enabled_flag u(1) 
 sps_ref_pic_resampling_enabled_flag u(1) 
 if( sps_ref_pic_resampling_enabled_flag )  
  sps_res_change_in_clvs_allowed_flag u(1) 
 sps_pic_width_max_in_luma_samples ue(v) 
 sps_pic_height_max_in_luma_samples ue(v) 
 sps_conformance_window_flag u(1) 
 if( sps_conformance_window_flag ) {  
  sps_conf_win_left_offset ue(v) 
  sps_conf_win_right_offset ue(v) 
  sps_conf_win_top_offset ue(v) 
  sps_conf_win_bottom_offset ue(v) 
 }  
 sps_subpic_info_present_flag u(1) 
 if( sps_subpic_info_present_flag ) {  
  sps_num_subpics_minus1 ue(v) 
  if( sps_num_subpics_minus1 > 0 ) {  
   sps_independent_subpics_flag u(1) 
   sps_subpic_same_size_flag u(1) 
  }  
  for( i = 0; sps_num_subpics_minus1 > 0  &&  i  <=  sps_num_subpics_minus1; i++ ) {  
   if( !sps_subpic_same_size_flag  ||  i  ==  0 ) {  
    if( i > 0  &&  sps_pic_width_max_in_luma_samples > CtbSizeY )  
     sps_subpic_ctu_top_left_x[ i ] u(v) 
    if( i > 0  &&  sps_pic_height_max_in_luma_samples > CtbSizeY )  
     sps_subpic_ctu_top_left_y[ i ] u(v) 
    if( i < sps_num_subpics_minus1  && 
      sps_pic_width_max_in_luma_samples > CtbSizeY ) 
 
     sps_subpic_width_minus1[ i ] u(v) 
    if( i < sps_num_subpics_minus1  && 
      sps_pic_height_max_in_luma_samples > CtbSizeY ) 
 
     sps_subpic_height_minus1[ i ] u(v) 
   }  
   if( !sps_independent_subpics_flag) {  
    sps_subpic_treated_as_pic_flag[ i ] u(1) 
    sps_loop_filter_across_subpic_enabled_flag[ i ] u(1) 
   }  
  }  
  sps_subpic_id_len_minus1 ue(v) 
  sps_subpic_id_mapping_explicitly_signalled_flag u(1) 
  if( sps_subpic_id_mapping_explicitly_signalled_flag ) {  
   sps_subpic_id_mapping_present_flag u(1) 
   if( sps_subpic_id_mapping_present_flag )  
    for( i = 0; i  <=   sps_num_subpics_minus1; i++ )  
     sps_subpic_id[ i ] u(v) 
  }  
 }  
 sps_bitdepth_minus8 ue(v) 
 sps_entropy_coding_sync_enabled_flag u(1) 
 sps_entry_point_offsets_present_flag u(1) 
 sps_log2_max_pic_order_cnt_lsb_minus4 u(4) 
 sps_poc_msb_cycle_flag u(1) 
 if( sps_poc_msb_cycle_flag )  
  sps_poc_msb_cycle_len_minus1 ue(v) 
 sps_num_extra_ph_bytes u(2) 
 for( i = 0; i < (sps_num_extra_ph_bytes * 8 ); i++ )  
  sps_extra_ph_bit_present_flag[ i ] u(1) 
 sps_num_extra_sh_bytes u(2) 
 for( i = 0; i < (sps_num_extra_sh_bytes * 8 ); i++ )  
  sps_extra_sh_bit_present_flag[ i ] u(1) 
 if( sps_ptl_dpb_hrd_params_present_flag ) {  
  if( sps_max_sublayers_minus1 > 0 )  
   sps_sublayer_dpb_params_flag u(1) 
  dpb_parameters( sps_max_sublayers_minus1, sps_sublayer_dpb_params_flag )  
 }  
 sps_log2_min_luma_coding_block_size_minus2 ue(v) 
 sps_partition_constraints_override_enabled_flag u(1) 
 sps_log2_diff_min_qt_min_cb_intra_slice_luma ue(v) 
 sps_max_mtt_hierarchy_depth_intra_slice_luma ue(v) 
 if( sps_max_mtt_hierarchy_depth_intra_slice_luma  !=  0 ) {  
  sps_log2_diff_max_bt_min_qt_intra_slice_luma ue(v) 
  sps_log2_diff_max_tt_min_qt_intra_slice_luma ue(v) 
 }  
 if( sps_chroma_format_idc  !=  0 )  
  sps_qtbtt_dual_tree_intra_flag u(1) 
 if( sps_qtbtt_dual_tree_intra_flag ) {  
  sps_log2_diff_min_qt_min_cb_intra_slice_chroma ue(v) 
  sps_max_mtt_hierarchy_depth_intra_slice_chroma ue(v) 
  if( sps_max_mtt_hierarchy_depth_intra_slice_chroma  !=  0 ) {  
   sps_log2_diff_max_bt_min_qt_intra_slice_chroma ue(v) 
   sps_log2_diff_max_tt_min_qt_intra_slice_chroma ue(v) 
  }  
 }  
 sps_log2_diff_min_qt_min_cb_inter_slice ue(v) 
 sps_max_mtt_hierarchy_depth_inter_slice ue(v) 
 if( sps_max_mtt_hierarchy_depth_inter_slice  !=  0 ) {  
  sps_log2_diff_max_bt_min_qt_inter_slice ue(v) 
  sps_log2_diff_max_tt_min_qt_inter_slice ue(v) 
 }  
 if( CtbSizeY > 32 )  
  sps_max_luma_transform_size_64_flag u(1) 
 sps_transform_skip_enabled_flag u(1) 
 if( sps_transform_skip_enabled_flag ) {  
  sps_log2_transform_skip_max_size_minus2 ue(v) 
  sps_bdpcm_enabled_flag u(1) 
 }  
 sps_mts_enabled_flag u(1) 
 if( sps_mts_enabled_flag ) {  
  sps_explicit_mts_intra_enabled_flag u(1) 
  sps_explicit_mts_inter_enabled_flag u(1) 
 }  
 sps_lfnst_enabled_flag u(1) 
 if( sps_chroma_format_idc  !=  0 ) {  
  sps_joint_cbcr_enabled_flag u(1) 
  sps_same_qp_table_for_chroma_flag u(1) 
  numQpTables = sps_same_qp_table_for_chroma_flag != 0 ? 1 : ( sps_joint_cbcr_enabled_flag != 0 ? 3 : 2 )  
 
  for( i = 0; i < numQpTables; i++ ) {  
   sps_qp_table_start_minus26[ i ] se(v) 
   sps_num_points_in_qp_table_minus1[ i ] ue(v) 
   for( j = 0; j  <=  sps_num_points_in_qp_table_minus1[ i ]; j++ ) {  
    sps_delta_qp_in_val_minus1[ i ][ j ] ue(v) 
    sps_delta_qp_diff_val[ i ][ j ] ue(v) 
   }  
  }  
 }  
 sps_sao_enabled_flag u(1) 
 sps_alf_enabled_flag u(1) 
 if( sps_alf_enabled_flag  &&  sps_chroma_format_idc  !=  0 )  
  sps_ccalf_enabled_flag u(1) 
 sps_lmcs_enabled_flag u(1) 
 sps_weighted_pred_flag u(1) 
 sps_weighted_bipred_flag u(1) 
 sps_long_term_ref_pics_flag u(1) 
 if( sps_video_parameter_set_id > 0 )  
  sps_inter_layer_prediction_enabled_flag u(1) 
 sps_idr_rpl_present_flag u(1) 
 sps_rpl1_same_as_rpl0_flag u(1) 
 for( i = 0; i < ( sps_rpl1_same_as_rpl0_flag != 0 ? 1 : 2 ); i++ ) {  
  sps_num_ref_pic_lists[ i ] ue(v) 
  for( j = 0; j < sps_num_ref_pic_lists[ i ]; j++)  
   ref_pic_list_struct( i, j )  
 }  
 sps_ref_wraparound_enabled_flag u(1) 
 sps_temporal_mvp_enabled_flag u(1) 
 if( sps_temporal_mvp_enabled_flag )  
  sps_sbtmvp_enabled_flag u(1)
 sps_amvr_enabled_flag u(1) 
 sps_bdof_enabled_flag u(1) 
 if( sps_bdof_enabled_flag )  
  sps_bdof_control_present_in_ph_flag u(1) 
 sps_smvd_enabled_flag u(1) 
 sps_dmvr_enabled_flag u(1) 
 if( sps_dmvr_enabled_flag)  
  sps_dmvr_control_present_in_ph_flag u(1) 
 sps_mmvd_enabled_flag u(1) 
 if( sps_mmvd_enabled_flag )  
  sps_mmvd_fullpel_only_enabled_flag u(1) 
 sps_six_minus_max_num_merge_cand ue(v) 
 sps_sbt_enabled_flag u(1) 
 sps_affine_enabled_flag u(1) 
 if( sps_affine_enabled_flag ) {  
  sps_five_minus_max_num_subblock_merge_cand ue(v) 
  sps_6param_affine_enabled_flag u(1) 
  if( sps_amvr_enabled_flag )  
   sps_affine_amvr_enabled_flag u(1) 
  sps_affine_prof_enabled_flag u(1) 
  if( sps_affine_prof_enabled_flag )  
   sps_prof_control_present_in_ph_flag u(1) 
 }  
 sps_bcw_enabled_flag u(1) 
 sps_ciip_enabled_flag u(1) 
 if( MaxNumMergeCand  >=  2 ) {  
  sps_gpm_enabled_flag u(1) 
  if( sps_gpm_enabled_flag  &&  MaxNumMergeCand  >=  3 )  
   sps_max_num_merge_cand_minus_max_num_gpm_cand ue(v) 
 }  
 sps_log2_parallel_merge_level_minus2 ue(v) 
 sps_isp_enabled_flag u(1) 
 sps_mrl_enabled_flag u(1) 
 sps_mip_enabled_flag u(1) 
 if( sps_chroma_format_idc  !=  0 )  
  sps_cclm_enabled_flag u(1) 
 if( sps_chroma_format_idc  ==  1 ) {  
  sps_chroma_horizontal_collocated_flag u(1) 
  sps_chroma_vertical_collocated_flag u(1) 
 }  
 sps_palette_enabled_flag u(1) 
 if( sps_chroma_format_idc  ==  3  &&  !sps_max_luma_transform_size_64_flag )  
  sps_act_enabled_flag u(1) 
 if( sps_transform_skip_enabled_flag  ||  sps_palette_enabled_flag )  
  sps_min_qp_prime_ts ue(v) 
 sps_ibc_enabled_flag u(1) 
 if( sps_ibc_enabled_flag )  
  sps_six_minus_max_num_ibc_merge_cand ue(v) 
 sps_ladf_enabled_flag u(1) 
 if( sps_ladf_enabled_flag ) {  
  sps_num_ladf_intervals_minus2 u(2) 
  sps_ladf_lowest_interval_qp_offset se(v) 
  for( i = 0; i < sps_num_ladf_intervals_minus2 + 1; i++ ) {  
   sps_ladf_qp_offset[ i ] se(v) 
   sps_ladf_delta_threshold_minus1[ i ] ue(v) 
  }  
 }  
 sps_explicit_scaling_list_enabled_flag u(1) 
 if( sps_lfnst_enabled_flag  &&  sps_explicit_scaling_list_enabled_flag )  
  sps_scaling_matrix_for_lfnst_disabled_flag u(1) 
 if( sps_act_enabled_flag  &&  sps_explicit_scaling_list_enabled_flag )  
  sps_scaling_matrix_for_alternative_colour_space_disabled_flag u(1) 
 if( sps_scaling_matrix_for_alternative_colour_space_disabled_flag )  
  sps_scaling_matrix_designated_colour_space_flag u(1) 
 sps_dep_quant_enabled_flag u(1) 
 sps_sign_data_hiding_enabled_flag u(1) 
 sps_virtual_boundaries_enabled_flag u(1) 
 if( sps_virtual_boundaries_enabled_flag ) {  
  sps_virtual_boundaries_present_flag u(1) 
  if( sps_virtual_boundaries_present_flag ) {  
   sps_num_ver_virtual_boundaries ue(v) 
   for( i = 0; i < sps_num_ver_virtual_boundaries; i++ )  
    sps_virtual_boundary_pos_x_minus1[ i ] ue(v) 
   sps_num_hor_virtual_boundaries ue(v) 
   for( i = 0; i < sps_num_hor_virtual_boundaries; i++ )  
    sps_virtual_boundary_pos_y_minus1[ i ] ue(v) 
  }  
 }  
 if( sps_ptl_dpb_hrd_params_present_flag ) {  
  sps_timing_hrd_params_present_flag u(1) 
  if( sps_timing_hrd_params_present_flag ) {  
   general_timing_hrd_parameters()  
   if( sps_max_sublayers_minus1 > 0 )  
    sps_sublayer_cpb_params_present_flag u(1) 
   firstSubLayer = sps_sublayer_cpb_params_present_flag != 0 ? 0 : sps_max_sublayers_minus1 
 
   ols_timing_hrd_parameters( firstSubLayer, sps_max_sublayers_minus1 )  
  }  
 }  
 sps_field_seq_flag u(1) 
 sps_vui_parameters_present_flag u(1) 
 if( sps_vui_parameters_present_flag ) {  
  sps_vui_payload_size_minus1 ue(v) 
  while( !byte_aligned() )  
   sps_vui_alignment_zero_bit f(1) 
  vui_payload( sps_vui_payload_size_minus1 + 1 )  
 }  
 sps_extension_flag u(1) 
 if( sps_extension_flag )  
  while( more_rbsp_data() )  
   sps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}
    */
    public class SeqParameterSetRbsp : IItuSerializable
    {
        private uint sps_seq_parameter_set_id;
        public uint SpsSeqParameterSetId { get { return sps_seq_parameter_set_id; } set { sps_seq_parameter_set_id = value; } }
        private uint sps_video_parameter_set_id;
        public uint SpsVideoParameterSetId { get { return sps_video_parameter_set_id; } set { sps_video_parameter_set_id = value; } }
        private uint sps_max_sublayers_minus1;
        public uint SpsMaxSublayersMinus1 { get { return sps_max_sublayers_minus1; } set { sps_max_sublayers_minus1 = value; } }
        private uint sps_chroma_format_idc;
        public uint SpsChromaFormatIdc { get { return sps_chroma_format_idc; } set { sps_chroma_format_idc = value; } }
        private uint sps_log2_ctu_size_minus5;
        public uint SpsLog2CtuSizeMinus5 { get { return sps_log2_ctu_size_minus5; } set { sps_log2_ctu_size_minus5 = value; } }
        private byte sps_ptl_dpb_hrd_params_present_flag;
        public byte SpsPtlDpbHrdParamsPresentFlag { get { return sps_ptl_dpb_hrd_params_present_flag; } set { sps_ptl_dpb_hrd_params_present_flag = value; } }
        private ProfileTierLevel profile_tier_level;
        public ProfileTierLevel ProfileTierLevel { get { return profile_tier_level; } set { profile_tier_level = value; } }
        private byte sps_gdr_enabled_flag;
        public byte SpsGdrEnabledFlag { get { return sps_gdr_enabled_flag; } set { sps_gdr_enabled_flag = value; } }
        private byte sps_ref_pic_resampling_enabled_flag;
        public byte SpsRefPicResamplingEnabledFlag { get { return sps_ref_pic_resampling_enabled_flag; } set { sps_ref_pic_resampling_enabled_flag = value; } }
        private byte sps_res_change_in_clvs_allowed_flag;
        public byte SpsResChangeInClvsAllowedFlag { get { return sps_res_change_in_clvs_allowed_flag; } set { sps_res_change_in_clvs_allowed_flag = value; } }
        private uint sps_pic_width_max_in_luma_samples;
        public uint SpsPicWidthMaxInLumaSamples { get { return sps_pic_width_max_in_luma_samples; } set { sps_pic_width_max_in_luma_samples = value; } }
        private uint sps_pic_height_max_in_luma_samples;
        public uint SpsPicHeightMaxInLumaSamples { get { return sps_pic_height_max_in_luma_samples; } set { sps_pic_height_max_in_luma_samples = value; } }
        private byte sps_conformance_window_flag;
        public byte SpsConformanceWindowFlag { get { return sps_conformance_window_flag; } set { sps_conformance_window_flag = value; } }
        private uint sps_conf_win_left_offset;
        public uint SpsConfWinLeftOffset { get { return sps_conf_win_left_offset; } set { sps_conf_win_left_offset = value; } }
        private uint sps_conf_win_right_offset;
        public uint SpsConfWinRightOffset { get { return sps_conf_win_right_offset; } set { sps_conf_win_right_offset = value; } }
        private uint sps_conf_win_top_offset;
        public uint SpsConfWinTopOffset { get { return sps_conf_win_top_offset; } set { sps_conf_win_top_offset = value; } }
        private uint sps_conf_win_bottom_offset;
        public uint SpsConfWinBottomOffset { get { return sps_conf_win_bottom_offset; } set { sps_conf_win_bottom_offset = value; } }
        private byte sps_subpic_info_present_flag;
        public byte SpsSubpicInfoPresentFlag { get { return sps_subpic_info_present_flag; } set { sps_subpic_info_present_flag = value; } }
        private uint sps_num_subpics_minus1;
        public uint SpsNumSubpicsMinus1 { get { return sps_num_subpics_minus1; } set { sps_num_subpics_minus1 = value; } }
        private byte sps_independent_subpics_flag;
        public byte SpsIndependentSubpicsFlag { get { return sps_independent_subpics_flag; } set { sps_independent_subpics_flag = value; } }
        private byte sps_subpic_same_size_flag;
        public byte SpsSubpicSameSizeFlag { get { return sps_subpic_same_size_flag; } set { sps_subpic_same_size_flag = value; } }
        private uint[] sps_subpic_ctu_top_left_x;
        public uint[] SpsSubpicCtuTopLeftx { get { return sps_subpic_ctu_top_left_x; } set { sps_subpic_ctu_top_left_x = value; } }
        private uint[] sps_subpic_ctu_top_left_y;
        public uint[] SpsSubpicCtuTopLefty { get { return sps_subpic_ctu_top_left_y; } set { sps_subpic_ctu_top_left_y = value; } }
        private uint[] sps_subpic_width_minus1;
        public uint[] SpsSubpicWidthMinus1 { get { return sps_subpic_width_minus1; } set { sps_subpic_width_minus1 = value; } }
        private uint[] sps_subpic_height_minus1;
        public uint[] SpsSubpicHeightMinus1 { get { return sps_subpic_height_minus1; } set { sps_subpic_height_minus1 = value; } }
        private byte[] sps_subpic_treated_as_pic_flag;
        public byte[] SpsSubpicTreatedAsPicFlag { get { return sps_subpic_treated_as_pic_flag; } set { sps_subpic_treated_as_pic_flag = value; } }
        private byte[] sps_loop_filter_across_subpic_enabled_flag;
        public byte[] SpsLoopFilterAcrossSubpicEnabledFlag { get { return sps_loop_filter_across_subpic_enabled_flag; } set { sps_loop_filter_across_subpic_enabled_flag = value; } }
        private uint sps_subpic_id_len_minus1;
        public uint SpsSubpicIdLenMinus1 { get { return sps_subpic_id_len_minus1; } set { sps_subpic_id_len_minus1 = value; } }
        private byte sps_subpic_id_mapping_explicitly_signalled_flag;
        public byte SpsSubpicIdMappingExplicitlySignalledFlag { get { return sps_subpic_id_mapping_explicitly_signalled_flag; } set { sps_subpic_id_mapping_explicitly_signalled_flag = value; } }
        private byte sps_subpic_id_mapping_present_flag;
        public byte SpsSubpicIdMappingPresentFlag { get { return sps_subpic_id_mapping_present_flag; } set { sps_subpic_id_mapping_present_flag = value; } }
        private uint[] sps_subpic_id;
        public uint[] SpsSubpicId { get { return sps_subpic_id; } set { sps_subpic_id = value; } }
        private uint sps_bitdepth_minus8;
        public uint SpsBitdepthMinus8 { get { return sps_bitdepth_minus8; } set { sps_bitdepth_minus8 = value; } }
        private byte sps_entropy_coding_sync_enabled_flag;
        public byte SpsEntropyCodingSyncEnabledFlag { get { return sps_entropy_coding_sync_enabled_flag; } set { sps_entropy_coding_sync_enabled_flag = value; } }
        private byte sps_entry_point_offsets_present_flag;
        public byte SpsEntryPointOffsetsPresentFlag { get { return sps_entry_point_offsets_present_flag; } set { sps_entry_point_offsets_present_flag = value; } }
        private uint sps_log2_max_pic_order_cnt_lsb_minus4;
        public uint SpsLog2MaxPicOrderCntLsbMinus4 { get { return sps_log2_max_pic_order_cnt_lsb_minus4; } set { sps_log2_max_pic_order_cnt_lsb_minus4 = value; } }
        private byte sps_poc_msb_cycle_flag;
        public byte SpsPocMsbCycleFlag { get { return sps_poc_msb_cycle_flag; } set { sps_poc_msb_cycle_flag = value; } }
        private uint sps_poc_msb_cycle_len_minus1;
        public uint SpsPocMsbCycleLenMinus1 { get { return sps_poc_msb_cycle_len_minus1; } set { sps_poc_msb_cycle_len_minus1 = value; } }
        private uint sps_num_extra_ph_bytes;
        public uint SpsNumExtraPhBytes { get { return sps_num_extra_ph_bytes; } set { sps_num_extra_ph_bytes = value; } }
        private byte[] sps_extra_ph_bit_present_flag;
        public byte[] SpsExtraPhBitPresentFlag { get { return sps_extra_ph_bit_present_flag; } set { sps_extra_ph_bit_present_flag = value; } }
        private uint sps_num_extra_sh_bytes;
        public uint SpsNumExtraShBytes { get { return sps_num_extra_sh_bytes; } set { sps_num_extra_sh_bytes = value; } }
        private byte[] sps_extra_sh_bit_present_flag;
        public byte[] SpsExtraShBitPresentFlag { get { return sps_extra_sh_bit_present_flag; } set { sps_extra_sh_bit_present_flag = value; } }
        private byte sps_sublayer_dpb_params_flag;
        public byte SpsSublayerDpbParamsFlag { get { return sps_sublayer_dpb_params_flag; } set { sps_sublayer_dpb_params_flag = value; } }
        private DpbParameters dpb_parameters;
        public DpbParameters DpbParameters { get { return dpb_parameters; } set { dpb_parameters = value; } }
        private uint sps_log2_min_luma_coding_block_size_minus2;
        public uint SpsLog2MinLumaCodingBlockSizeMinus2 { get { return sps_log2_min_luma_coding_block_size_minus2; } set { sps_log2_min_luma_coding_block_size_minus2 = value; } }
        private byte sps_partition_constraints_override_enabled_flag;
        public byte SpsPartitionConstraintsOverrideEnabledFlag { get { return sps_partition_constraints_override_enabled_flag; } set { sps_partition_constraints_override_enabled_flag = value; } }
        private uint sps_log2_diff_min_qt_min_cb_intra_slice_luma;
        public uint SpsLog2DiffMinQtMinCbIntraSliceLuma { get { return sps_log2_diff_min_qt_min_cb_intra_slice_luma; } set { sps_log2_diff_min_qt_min_cb_intra_slice_luma = value; } }
        private uint sps_max_mtt_hierarchy_depth_intra_slice_luma;
        public uint SpsMaxMttHierarchyDepthIntraSliceLuma { get { return sps_max_mtt_hierarchy_depth_intra_slice_luma; } set { sps_max_mtt_hierarchy_depth_intra_slice_luma = value; } }
        private uint sps_log2_diff_max_bt_min_qt_intra_slice_luma;
        public uint SpsLog2DiffMaxBtMinQtIntraSliceLuma { get { return sps_log2_diff_max_bt_min_qt_intra_slice_luma; } set { sps_log2_diff_max_bt_min_qt_intra_slice_luma = value; } }
        private uint sps_log2_diff_max_tt_min_qt_intra_slice_luma;
        public uint SpsLog2DiffMaxTtMinQtIntraSliceLuma { get { return sps_log2_diff_max_tt_min_qt_intra_slice_luma; } set { sps_log2_diff_max_tt_min_qt_intra_slice_luma = value; } }
        private byte sps_qtbtt_dual_tree_intra_flag;
        public byte SpsQtbttDualTreeIntraFlag { get { return sps_qtbtt_dual_tree_intra_flag; } set { sps_qtbtt_dual_tree_intra_flag = value; } }
        private uint sps_log2_diff_min_qt_min_cb_intra_slice_chroma;
        public uint SpsLog2DiffMinQtMinCbIntraSliceChroma { get { return sps_log2_diff_min_qt_min_cb_intra_slice_chroma; } set { sps_log2_diff_min_qt_min_cb_intra_slice_chroma = value; } }
        private uint sps_max_mtt_hierarchy_depth_intra_slice_chroma;
        public uint SpsMaxMttHierarchyDepthIntraSliceChroma { get { return sps_max_mtt_hierarchy_depth_intra_slice_chroma; } set { sps_max_mtt_hierarchy_depth_intra_slice_chroma = value; } }
        private uint sps_log2_diff_max_bt_min_qt_intra_slice_chroma;
        public uint SpsLog2DiffMaxBtMinQtIntraSliceChroma { get { return sps_log2_diff_max_bt_min_qt_intra_slice_chroma; } set { sps_log2_diff_max_bt_min_qt_intra_slice_chroma = value; } }
        private uint sps_log2_diff_max_tt_min_qt_intra_slice_chroma;
        public uint SpsLog2DiffMaxTtMinQtIntraSliceChroma { get { return sps_log2_diff_max_tt_min_qt_intra_slice_chroma; } set { sps_log2_diff_max_tt_min_qt_intra_slice_chroma = value; } }
        private uint sps_log2_diff_min_qt_min_cb_inter_slice;
        public uint SpsLog2DiffMinQtMinCbInterSlice { get { return sps_log2_diff_min_qt_min_cb_inter_slice; } set { sps_log2_diff_min_qt_min_cb_inter_slice = value; } }
        private uint sps_max_mtt_hierarchy_depth_inter_slice;
        public uint SpsMaxMttHierarchyDepthInterSlice { get { return sps_max_mtt_hierarchy_depth_inter_slice; } set { sps_max_mtt_hierarchy_depth_inter_slice = value; } }
        private uint sps_log2_diff_max_bt_min_qt_inter_slice;
        public uint SpsLog2DiffMaxBtMinQtInterSlice { get { return sps_log2_diff_max_bt_min_qt_inter_slice; } set { sps_log2_diff_max_bt_min_qt_inter_slice = value; } }
        private uint sps_log2_diff_max_tt_min_qt_inter_slice;
        public uint SpsLog2DiffMaxTtMinQtInterSlice { get { return sps_log2_diff_max_tt_min_qt_inter_slice; } set { sps_log2_diff_max_tt_min_qt_inter_slice = value; } }
        private byte sps_max_luma_transform_size_64_flag;
        public byte SpsMaxLumaTransformSize64Flag { get { return sps_max_luma_transform_size_64_flag; } set { sps_max_luma_transform_size_64_flag = value; } }
        private byte sps_transform_skip_enabled_flag;
        public byte SpsTransformSkipEnabledFlag { get { return sps_transform_skip_enabled_flag; } set { sps_transform_skip_enabled_flag = value; } }
        private uint sps_log2_transform_skip_max_size_minus2;
        public uint SpsLog2TransformSkipMaxSizeMinus2 { get { return sps_log2_transform_skip_max_size_minus2; } set { sps_log2_transform_skip_max_size_minus2 = value; } }
        private byte sps_bdpcm_enabled_flag;
        public byte SpsBdpcmEnabledFlag { get { return sps_bdpcm_enabled_flag; } set { sps_bdpcm_enabled_flag = value; } }
        private byte sps_mts_enabled_flag;
        public byte SpsMtsEnabledFlag { get { return sps_mts_enabled_flag; } set { sps_mts_enabled_flag = value; } }
        private byte sps_explicit_mts_intra_enabled_flag;
        public byte SpsExplicitMtsIntraEnabledFlag { get { return sps_explicit_mts_intra_enabled_flag; } set { sps_explicit_mts_intra_enabled_flag = value; } }
        private byte sps_explicit_mts_inter_enabled_flag;
        public byte SpsExplicitMtsInterEnabledFlag { get { return sps_explicit_mts_inter_enabled_flag; } set { sps_explicit_mts_inter_enabled_flag = value; } }
        private byte sps_lfnst_enabled_flag;
        public byte SpsLfnstEnabledFlag { get { return sps_lfnst_enabled_flag; } set { sps_lfnst_enabled_flag = value; } }
        private byte sps_joint_cbcr_enabled_flag;
        public byte SpsJointCbcrEnabledFlag { get { return sps_joint_cbcr_enabled_flag; } set { sps_joint_cbcr_enabled_flag = value; } }
        private byte sps_same_qp_table_for_chroma_flag;
        public byte SpsSameQpTableForChromaFlag { get { return sps_same_qp_table_for_chroma_flag; } set { sps_same_qp_table_for_chroma_flag = value; } }
        private int[] sps_qp_table_start_minus26;
        public int[] SpsQpTableStartMinus26 { get { return sps_qp_table_start_minus26; } set { sps_qp_table_start_minus26 = value; } }
        private uint[] sps_num_points_in_qp_table_minus1;
        public uint[] SpsNumPointsInQpTableMinus1 { get { return sps_num_points_in_qp_table_minus1; } set { sps_num_points_in_qp_table_minus1 = value; } }
        private uint[][] sps_delta_qp_in_val_minus1;
        public uint[][] SpsDeltaQpInValMinus1 { get { return sps_delta_qp_in_val_minus1; } set { sps_delta_qp_in_val_minus1 = value; } }
        private uint[][] sps_delta_qp_diff_val;
        public uint[][] SpsDeltaQpDiffVal { get { return sps_delta_qp_diff_val; } set { sps_delta_qp_diff_val = value; } }
        private byte sps_sao_enabled_flag;
        public byte SpsSaoEnabledFlag { get { return sps_sao_enabled_flag; } set { sps_sao_enabled_flag = value; } }
        private byte sps_alf_enabled_flag;
        public byte SpsAlfEnabledFlag { get { return sps_alf_enabled_flag; } set { sps_alf_enabled_flag = value; } }
        private byte sps_ccalf_enabled_flag;
        public byte SpsCcalfEnabledFlag { get { return sps_ccalf_enabled_flag; } set { sps_ccalf_enabled_flag = value; } }
        private byte sps_lmcs_enabled_flag;
        public byte SpsLmcsEnabledFlag { get { return sps_lmcs_enabled_flag; } set { sps_lmcs_enabled_flag = value; } }
        private byte sps_weighted_pred_flag;
        public byte SpsWeightedPredFlag { get { return sps_weighted_pred_flag; } set { sps_weighted_pred_flag = value; } }
        private byte sps_weighted_bipred_flag;
        public byte SpsWeightedBipredFlag { get { return sps_weighted_bipred_flag; } set { sps_weighted_bipred_flag = value; } }
        private byte sps_long_term_ref_pics_flag;
        public byte SpsLongTermRefPicsFlag { get { return sps_long_term_ref_pics_flag; } set { sps_long_term_ref_pics_flag = value; } }
        private byte sps_inter_layer_prediction_enabled_flag;
        public byte SpsInterLayerPredictionEnabledFlag { get { return sps_inter_layer_prediction_enabled_flag; } set { sps_inter_layer_prediction_enabled_flag = value; } }
        private byte sps_idr_rpl_present_flag;
        public byte SpsIdrRplPresentFlag { get { return sps_idr_rpl_present_flag; } set { sps_idr_rpl_present_flag = value; } }
        private byte sps_rpl1_same_as_rpl0_flag;
        public byte SpsRpl1SameAsRpl0Flag { get { return sps_rpl1_same_as_rpl0_flag; } set { sps_rpl1_same_as_rpl0_flag = value; } }
        private uint[] sps_num_ref_pic_lists;
        public uint[] SpsNumRefPicLists { get { return sps_num_ref_pic_lists; } set { sps_num_ref_pic_lists = value; } }
        private RefPicListStruct ref_pic_list_struct;
        public RefPicListStruct RefPicListStruct { get { return ref_pic_list_struct; } set { ref_pic_list_struct = value; } }
        private byte sps_ref_wraparound_enabled_flag;
        public byte SpsRefWraparoundEnabledFlag { get { return sps_ref_wraparound_enabled_flag; } set { sps_ref_wraparound_enabled_flag = value; } }
        private byte sps_temporal_mvp_enabled_flag;
        public byte SpsTemporalMvpEnabledFlag { get { return sps_temporal_mvp_enabled_flag; } set { sps_temporal_mvp_enabled_flag = value; } }
        private byte sps_sbtmvp_enabled_flag;
        public byte SpsSbtmvpEnabledFlag { get { return sps_sbtmvp_enabled_flag; } set { sps_sbtmvp_enabled_flag = value; } }
        private byte sps_amvr_enabled_flag;
        public byte SpsAmvrEnabledFlag { get { return sps_amvr_enabled_flag; } set { sps_amvr_enabled_flag = value; } }
        private byte sps_bdof_enabled_flag;
        public byte SpsBdofEnabledFlag { get { return sps_bdof_enabled_flag; } set { sps_bdof_enabled_flag = value; } }
        private byte sps_bdof_control_present_in_ph_flag;
        public byte SpsBdofControlPresentInPhFlag { get { return sps_bdof_control_present_in_ph_flag; } set { sps_bdof_control_present_in_ph_flag = value; } }
        private byte sps_smvd_enabled_flag;
        public byte SpsSmvdEnabledFlag { get { return sps_smvd_enabled_flag; } set { sps_smvd_enabled_flag = value; } }
        private byte sps_dmvr_enabled_flag;
        public byte SpsDmvrEnabledFlag { get { return sps_dmvr_enabled_flag; } set { sps_dmvr_enabled_flag = value; } }
        private byte sps_dmvr_control_present_in_ph_flag;
        public byte SpsDmvrControlPresentInPhFlag { get { return sps_dmvr_control_present_in_ph_flag; } set { sps_dmvr_control_present_in_ph_flag = value; } }
        private byte sps_mmvd_enabled_flag;
        public byte SpsMmvdEnabledFlag { get { return sps_mmvd_enabled_flag; } set { sps_mmvd_enabled_flag = value; } }
        private byte sps_mmvd_fullpel_only_enabled_flag;
        public byte SpsMmvdFullpelOnlyEnabledFlag { get { return sps_mmvd_fullpel_only_enabled_flag; } set { sps_mmvd_fullpel_only_enabled_flag = value; } }
        private uint sps_six_minus_max_num_merge_cand;
        public uint SpsSixMinusMaxNumMergeCand { get { return sps_six_minus_max_num_merge_cand; } set { sps_six_minus_max_num_merge_cand = value; } }
        private byte sps_sbt_enabled_flag;
        public byte SpsSbtEnabledFlag { get { return sps_sbt_enabled_flag; } set { sps_sbt_enabled_flag = value; } }
        private byte sps_affine_enabled_flag;
        public byte SpsAffineEnabledFlag { get { return sps_affine_enabled_flag; } set { sps_affine_enabled_flag = value; } }
        private uint sps_five_minus_max_num_subblock_merge_cand;
        public uint SpsFiveMinusMaxNumSubblockMergeCand { get { return sps_five_minus_max_num_subblock_merge_cand; } set { sps_five_minus_max_num_subblock_merge_cand = value; } }
        private byte sps_6param_affine_enabled_flag;
        public byte Sps6paramAffineEnabledFlag { get { return sps_6param_affine_enabled_flag; } set { sps_6param_affine_enabled_flag = value; } }
        private byte sps_affine_amvr_enabled_flag;
        public byte SpsAffineAmvrEnabledFlag { get { return sps_affine_amvr_enabled_flag; } set { sps_affine_amvr_enabled_flag = value; } }
        private byte sps_affine_prof_enabled_flag;
        public byte SpsAffineProfEnabledFlag { get { return sps_affine_prof_enabled_flag; } set { sps_affine_prof_enabled_flag = value; } }
        private byte sps_prof_control_present_in_ph_flag;
        public byte SpsProfControlPresentInPhFlag { get { return sps_prof_control_present_in_ph_flag; } set { sps_prof_control_present_in_ph_flag = value; } }
        private byte sps_bcw_enabled_flag;
        public byte SpsBcwEnabledFlag { get { return sps_bcw_enabled_flag; } set { sps_bcw_enabled_flag = value; } }
        private byte sps_ciip_enabled_flag;
        public byte SpsCiipEnabledFlag { get { return sps_ciip_enabled_flag; } set { sps_ciip_enabled_flag = value; } }
        private byte sps_gpm_enabled_flag;
        public byte SpsGpmEnabledFlag { get { return sps_gpm_enabled_flag; } set { sps_gpm_enabled_flag = value; } }
        private uint sps_max_num_merge_cand_minus_max_num_gpm_cand;
        public uint SpsMaxNumMergeCandMinusMaxNumGpmCand { get { return sps_max_num_merge_cand_minus_max_num_gpm_cand; } set { sps_max_num_merge_cand_minus_max_num_gpm_cand = value; } }
        private uint sps_log2_parallel_merge_level_minus2;
        public uint SpsLog2ParallelMergeLevelMinus2 { get { return sps_log2_parallel_merge_level_minus2; } set { sps_log2_parallel_merge_level_minus2 = value; } }
        private byte sps_isp_enabled_flag;
        public byte SpsIspEnabledFlag { get { return sps_isp_enabled_flag; } set { sps_isp_enabled_flag = value; } }
        private byte sps_mrl_enabled_flag;
        public byte SpsMrlEnabledFlag { get { return sps_mrl_enabled_flag; } set { sps_mrl_enabled_flag = value; } }
        private byte sps_mip_enabled_flag;
        public byte SpsMipEnabledFlag { get { return sps_mip_enabled_flag; } set { sps_mip_enabled_flag = value; } }
        private byte sps_cclm_enabled_flag;
        public byte SpsCclmEnabledFlag { get { return sps_cclm_enabled_flag; } set { sps_cclm_enabled_flag = value; } }
        private byte sps_chroma_horizontal_collocated_flag;
        public byte SpsChromaHorizontalCollocatedFlag { get { return sps_chroma_horizontal_collocated_flag; } set { sps_chroma_horizontal_collocated_flag = value; } }
        private byte sps_chroma_vertical_collocated_flag;
        public byte SpsChromaVerticalCollocatedFlag { get { return sps_chroma_vertical_collocated_flag; } set { sps_chroma_vertical_collocated_flag = value; } }
        private byte sps_palette_enabled_flag;
        public byte SpsPaletteEnabledFlag { get { return sps_palette_enabled_flag; } set { sps_palette_enabled_flag = value; } }
        private byte sps_act_enabled_flag;
        public byte SpsActEnabledFlag { get { return sps_act_enabled_flag; } set { sps_act_enabled_flag = value; } }
        private uint sps_min_qp_prime_ts;
        public uint SpsMinQpPrimeTs { get { return sps_min_qp_prime_ts; } set { sps_min_qp_prime_ts = value; } }
        private byte sps_ibc_enabled_flag;
        public byte SpsIbcEnabledFlag { get { return sps_ibc_enabled_flag; } set { sps_ibc_enabled_flag = value; } }
        private uint sps_six_minus_max_num_ibc_merge_cand;
        public uint SpsSixMinusMaxNumIbcMergeCand { get { return sps_six_minus_max_num_ibc_merge_cand; } set { sps_six_minus_max_num_ibc_merge_cand = value; } }
        private byte sps_ladf_enabled_flag;
        public byte SpsLadfEnabledFlag { get { return sps_ladf_enabled_flag; } set { sps_ladf_enabled_flag = value; } }
        private uint sps_num_ladf_intervals_minus2;
        public uint SpsNumLadfIntervalsMinus2 { get { return sps_num_ladf_intervals_minus2; } set { sps_num_ladf_intervals_minus2 = value; } }
        private int sps_ladf_lowest_interval_qp_offset;
        public int SpsLadfLowestIntervalQpOffset { get { return sps_ladf_lowest_interval_qp_offset; } set { sps_ladf_lowest_interval_qp_offset = value; } }
        private int[] sps_ladf_qp_offset;
        public int[] SpsLadfQpOffset { get { return sps_ladf_qp_offset; } set { sps_ladf_qp_offset = value; } }
        private uint[] sps_ladf_delta_threshold_minus1;
        public uint[] SpsLadfDeltaThresholdMinus1 { get { return sps_ladf_delta_threshold_minus1; } set { sps_ladf_delta_threshold_minus1 = value; } }
        private byte sps_explicit_scaling_list_enabled_flag;
        public byte SpsExplicitScalingListEnabledFlag { get { return sps_explicit_scaling_list_enabled_flag; } set { sps_explicit_scaling_list_enabled_flag = value; } }
        private byte sps_scaling_matrix_for_lfnst_disabled_flag;
        public byte SpsScalingMatrixForLfnstDisabledFlag { get { return sps_scaling_matrix_for_lfnst_disabled_flag; } set { sps_scaling_matrix_for_lfnst_disabled_flag = value; } }
        private byte sps_scaling_matrix_for_alternative_colour_space_disabled_flag;
        public byte SpsScalingMatrixForAlternativeColourSpaceDisabledFlag { get { return sps_scaling_matrix_for_alternative_colour_space_disabled_flag; } set { sps_scaling_matrix_for_alternative_colour_space_disabled_flag = value; } }
        private byte sps_scaling_matrix_designated_colour_space_flag;
        public byte SpsScalingMatrixDesignatedColourSpaceFlag { get { return sps_scaling_matrix_designated_colour_space_flag; } set { sps_scaling_matrix_designated_colour_space_flag = value; } }
        private byte sps_dep_quant_enabled_flag;
        public byte SpsDepQuantEnabledFlag { get { return sps_dep_quant_enabled_flag; } set { sps_dep_quant_enabled_flag = value; } }
        private byte sps_sign_data_hiding_enabled_flag;
        public byte SpsSignDataHidingEnabledFlag { get { return sps_sign_data_hiding_enabled_flag; } set { sps_sign_data_hiding_enabled_flag = value; } }
        private byte sps_virtual_boundaries_enabled_flag;
        public byte SpsVirtualBoundariesEnabledFlag { get { return sps_virtual_boundaries_enabled_flag; } set { sps_virtual_boundaries_enabled_flag = value; } }
        private byte sps_virtual_boundaries_present_flag;
        public byte SpsVirtualBoundariesPresentFlag { get { return sps_virtual_boundaries_present_flag; } set { sps_virtual_boundaries_present_flag = value; } }
        private uint sps_num_ver_virtual_boundaries;
        public uint SpsNumVerVirtualBoundaries { get { return sps_num_ver_virtual_boundaries; } set { sps_num_ver_virtual_boundaries = value; } }
        private uint[] sps_virtual_boundary_pos_x_minus1;
        public uint[] SpsVirtualBoundaryPosxMinus1 { get { return sps_virtual_boundary_pos_x_minus1; } set { sps_virtual_boundary_pos_x_minus1 = value; } }
        private uint sps_num_hor_virtual_boundaries;
        public uint SpsNumHorVirtualBoundaries { get { return sps_num_hor_virtual_boundaries; } set { sps_num_hor_virtual_boundaries = value; } }
        private uint[] sps_virtual_boundary_pos_y_minus1;
        public uint[] SpsVirtualBoundaryPosyMinus1 { get { return sps_virtual_boundary_pos_y_minus1; } set { sps_virtual_boundary_pos_y_minus1 = value; } }
        private byte sps_timing_hrd_params_present_flag;
        public byte SpsTimingHrdParamsPresentFlag { get { return sps_timing_hrd_params_present_flag; } set { sps_timing_hrd_params_present_flag = value; } }
        private GeneralTimingHrdParameters general_timing_hrd_parameters;
        public GeneralTimingHrdParameters GeneralTimingHrdParameters { get { return general_timing_hrd_parameters; } set { general_timing_hrd_parameters = value; } }
        private byte sps_sublayer_cpb_params_present_flag;
        public byte SpsSublayerCpbParamsPresentFlag { get { return sps_sublayer_cpb_params_present_flag; } set { sps_sublayer_cpb_params_present_flag = value; } }
        private OlsTimingHrdParameters ols_timing_hrd_parameters;
        public OlsTimingHrdParameters OlsTimingHrdParameters { get { return ols_timing_hrd_parameters; } set { ols_timing_hrd_parameters = value; } }
        private byte sps_field_seq_flag;
        public byte SpsFieldSeqFlag { get { return sps_field_seq_flag; } set { sps_field_seq_flag = value; } }
        private byte sps_vui_parameters_present_flag;
        public byte SpsVuiParametersPresentFlag { get { return sps_vui_parameters_present_flag; } set { sps_vui_parameters_present_flag = value; } }
        private uint sps_vui_payload_size_minus1;
        public uint SpsVuiPayloadSizeMinus1 { get { return sps_vui_payload_size_minus1; } set { sps_vui_payload_size_minus1 = value; } }
        private Dictionary<int, uint> sps_vui_alignment_zero_bit = new Dictionary<int, uint>();
        public Dictionary<int, uint> SpsVuiAlignmentZeroBit { get { return sps_vui_alignment_zero_bit; } set { sps_vui_alignment_zero_bit = value; } }
        private VuiPayload vui_payload;
        public VuiPayload VuiPayload { get { return vui_payload; } set { vui_payload = value; } }
        private byte sps_extension_flag;
        public byte SpsExtensionFlag { get { return sps_extension_flag; } set { sps_extension_flag = value; } }
        private Dictionary<int, byte> sps_extension_data_flag = new Dictionary<int, byte>();
        public Dictionary<int, byte> SpsExtensionDataFlag { get { return sps_extension_data_flag; } set { sps_extension_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeqParameterSetRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            long numQpTables = 0;
            uint j = 0;
            uint firstSubLayer = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 4, out this.sps_seq_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 4, out this.sps_video_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 3, out this.sps_max_sublayers_minus1);
            size += stream.ReadUnsignedInt(size, 2, out this.sps_chroma_format_idc);
            size += stream.ReadUnsignedInt(size, 2, out this.sps_log2_ctu_size_minus5);
            ((H266Context)context).OnSpsLog2CtuSizeMinus5();
            size += stream.ReadUnsignedInt(size, 1, out this.sps_ptl_dpb_hrd_params_present_flag);

            if (sps_ptl_dpb_hrd_params_present_flag != 0)
            {
                this.profile_tier_level = new ProfileTierLevel(1, sps_max_sublayers_minus1);
                size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_gdr_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_ref_pic_resampling_enabled_flag);

            if (sps_ref_pic_resampling_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_res_change_in_clvs_allowed_flag);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_pic_width_max_in_luma_samples);
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_pic_height_max_in_luma_samples);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_conformance_window_flag);

            if (sps_conformance_window_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_conf_win_left_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_conf_win_right_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_conf_win_top_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_conf_win_bottom_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_subpic_info_present_flag);

            if (sps_subpic_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_num_subpics_minus1);

                if (sps_num_subpics_minus1 > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_independent_subpics_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_subpic_same_size_flag);
                }

                this.sps_subpic_ctu_top_left_x = new uint[sps_num_subpics_minus1 + 1];
                this.sps_subpic_ctu_top_left_y = new uint[sps_num_subpics_minus1 + 1];
                this.sps_subpic_width_minus1 = new uint[sps_num_subpics_minus1 + 1];
                this.sps_subpic_height_minus1 = new uint[sps_num_subpics_minus1 + 1];
                this.sps_subpic_treated_as_pic_flag = new byte[sps_num_subpics_minus1 + 1];
                this.sps_loop_filter_across_subpic_enabled_flag = new byte[sps_num_subpics_minus1 + 1];
                for (i = 0; sps_num_subpics_minus1 > 0 && i <= sps_num_subpics_minus1; i++)
                {

                    if (sps_subpic_same_size_flag == 0 || i == 0)
                    {

                        if (i > 0 && sps_pic_width_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2((sps_pic_width_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), out this.sps_subpic_ctu_top_left_x[i]);
                        }

                        if (i > 0 && sps_pic_height_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2((sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), out this.sps_subpic_ctu_top_left_y[i]);
                        }

                        if (i < sps_num_subpics_minus1 &&
      sps_pic_width_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2((sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), out this.sps_subpic_width_minus1[i]);
                        }

                        if (i < sps_num_subpics_minus1 &&
      sps_pic_height_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2((sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), out this.sps_subpic_height_minus1[i]);
                        }
                    }

                    if (sps_independent_subpics_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sps_subpic_treated_as_pic_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sps_loop_filter_across_subpic_enabled_flag[i]);
                    }
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_subpic_id_len_minus1);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_subpic_id_mapping_explicitly_signalled_flag);

                if (sps_subpic_id_mapping_explicitly_signalled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_subpic_id_mapping_present_flag);

                    if (sps_subpic_id_mapping_present_flag != 0)
                    {

                        this.sps_subpic_id = new uint[sps_num_subpics_minus1 + 1];
                        for (i = 0; i <= sps_num_subpics_minus1; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, sps_subpic_id_len_minus1 + 1, out this.sps_subpic_id[i]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_bitdepth_minus8);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_entropy_coding_sync_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_entry_point_offsets_present_flag);
            size += stream.ReadUnsignedInt(size, 4, out this.sps_log2_max_pic_order_cnt_lsb_minus4);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_poc_msb_cycle_flag);

            if (sps_poc_msb_cycle_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_poc_msb_cycle_len_minus1);
            }
            size += stream.ReadUnsignedInt(size, 2, out this.sps_num_extra_ph_bytes);

            this.sps_extra_ph_bit_present_flag = new byte[(sps_num_extra_ph_bytes * 8)];
            for (i = 0; i < (sps_num_extra_ph_bytes * 8); i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_extra_ph_bit_present_flag[i]);
                ((H266Context)context).OnSpsExtraPhBitPresentFlag();
            }
            size += stream.ReadUnsignedInt(size, 2, out this.sps_num_extra_sh_bytes);

            this.sps_extra_sh_bit_present_flag = new byte[(sps_num_extra_sh_bytes * 8)];
            for (i = 0; i < (sps_num_extra_sh_bytes * 8); i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_extra_sh_bit_present_flag[i]);
                ((H266Context)context).OnSpsExtraShBitPresentFlag();
            }

            if (sps_ptl_dpb_hrd_params_present_flag != 0)
            {

                if (sps_max_sublayers_minus1 > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_sublayer_dpb_params_flag);
                }
                this.dpb_parameters = new DpbParameters(sps_max_sublayers_minus1, sps_sublayer_dpb_params_flag);
                size += stream.ReadClass<DpbParameters>(size, context, this.dpb_parameters);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_min_luma_coding_block_size_minus2);
            ((H266Context)context).OnSpsLog2MinLumaCodingBlockSizeMinus2();
            size += stream.ReadUnsignedInt(size, 1, out this.sps_partition_constraints_override_enabled_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_min_qt_min_cb_intra_slice_luma);
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_mtt_hierarchy_depth_intra_slice_luma);

            if (sps_max_mtt_hierarchy_depth_intra_slice_luma != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_max_bt_min_qt_intra_slice_luma);
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_max_tt_min_qt_intra_slice_luma);
            }

            if (sps_chroma_format_idc != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_qtbtt_dual_tree_intra_flag);
            }

            if (sps_qtbtt_dual_tree_intra_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_min_qt_min_cb_intra_slice_chroma);
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_mtt_hierarchy_depth_intra_slice_chroma);

                if (sps_max_mtt_hierarchy_depth_intra_slice_chroma != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_max_bt_min_qt_intra_slice_chroma);
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_max_tt_min_qt_intra_slice_chroma);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_min_qt_min_cb_inter_slice);
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_mtt_hierarchy_depth_inter_slice);

            if (sps_max_mtt_hierarchy_depth_inter_slice != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_max_bt_min_qt_inter_slice);
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_diff_max_tt_min_qt_inter_slice);
            }

            if (((H266Context)context).CtbSizeY > 32)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_max_luma_transform_size_64_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_transform_skip_enabled_flag);

            if (sps_transform_skip_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_transform_skip_max_size_minus2);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_bdpcm_enabled_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_mts_enabled_flag);

            if (sps_mts_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_explicit_mts_intra_enabled_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_explicit_mts_inter_enabled_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_lfnst_enabled_flag);

            if (sps_chroma_format_idc != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_joint_cbcr_enabled_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_same_qp_table_for_chroma_flag);
                numQpTables = sps_same_qp_table_for_chroma_flag != 0 ? 1 : (sps_joint_cbcr_enabled_flag != 0 ? 3 : 2);

                this.sps_qp_table_start_minus26 = new int[numQpTables];
                this.sps_num_points_in_qp_table_minus1 = new uint[numQpTables];
                this.sps_delta_qp_in_val_minus1 = new uint[numQpTables][];
                this.sps_delta_qp_diff_val = new uint[numQpTables][];
                for (i = 0; i < numQpTables; i++)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.sps_qp_table_start_minus26[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_num_points_in_qp_table_minus1[i]);

                    this.sps_delta_qp_in_val_minus1[i] = new uint[sps_num_points_in_qp_table_minus1[i] + 1];
                    this.sps_delta_qp_diff_val[i] = new uint[sps_num_points_in_qp_table_minus1[i] + 1];
                    for (j = 0; j <= sps_num_points_in_qp_table_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.sps_delta_qp_in_val_minus1[i][j]);
                        size += stream.ReadUnsignedIntGolomb(size, out this.sps_delta_qp_diff_val[i][j]);
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_sao_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_alf_enabled_flag);

            if (sps_alf_enabled_flag != 0 && sps_chroma_format_idc != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_ccalf_enabled_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_lmcs_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_weighted_pred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_weighted_bipred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_long_term_ref_pics_flag);

            if (sps_video_parameter_set_id > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_inter_layer_prediction_enabled_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_idr_rpl_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_rpl1_same_as_rpl0_flag);

            this.sps_num_ref_pic_lists = new uint[(sps_rpl1_same_as_rpl0_flag != 0 ? 1 : 2)];
            if (((H266Context)context).num_ref_entries == null)
                ((H266Context)context).num_ref_entries = new uint[2][];
            if (((H266Context)context).inter_layer_ref_pic_flag == null)
                ((H266Context)context).inter_layer_ref_pic_flag = new byte[2][][];
            if (((H266Context)context).st_ref_pic_flag == null)
                ((H266Context)context).st_ref_pic_flag = new byte[2][][];
            if (((H266Context)context).abs_delta_poc_st == null)
                ((H266Context)context).abs_delta_poc_st = new uint[2][][];
            if (((H266Context)context).strp_entry_sign_flag == null)
                ((H266Context)context).strp_entry_sign_flag = new byte[2][][];
            if (((H266Context)context).rpls_poc_lsb_lt == null)
                ((H266Context)context).rpls_poc_lsb_lt = new uint[2][][];
            if (((H266Context)context).ilrp_idx == null)
                ((H266Context)context).ilrp_idx = new uint[2][][];
            for (i = 0; i < (sps_rpl1_same_as_rpl0_flag != 0 ? 1 : 2); i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_num_ref_pic_lists[i]);

                if (((H266Context)context).num_ref_entries[i] == null)
                    ((H266Context)context).num_ref_entries[i] = new uint[sps_num_ref_pic_lists[i] + 1];
                if (((H266Context)context).inter_layer_ref_pic_flag[i] == null)
                    ((H266Context)context).inter_layer_ref_pic_flag[i] = new byte[sps_num_ref_pic_lists[i] + 1][];
                if (((H266Context)context).st_ref_pic_flag[i] == null)
                    ((H266Context)context).st_ref_pic_flag[i] = new byte[sps_num_ref_pic_lists[i] + 1][];
                if (((H266Context)context).abs_delta_poc_st[i] == null)
                    ((H266Context)context).abs_delta_poc_st[i] = new uint[sps_num_ref_pic_lists[i] + 1][];
                if (((H266Context)context).strp_entry_sign_flag[i] == null)
                    ((H266Context)context).strp_entry_sign_flag[i] = new byte[sps_num_ref_pic_lists[i] + 1][];
                if (((H266Context)context).rpls_poc_lsb_lt[i] == null)
                    ((H266Context)context).rpls_poc_lsb_lt[i] = new uint[sps_num_ref_pic_lists[i] + 1][];
                if (((H266Context)context).ilrp_idx[i] == null)
                    ((H266Context)context).ilrp_idx[i] = new uint[sps_num_ref_pic_lists[i] + 1][];
                for (j = 0; j < sps_num_ref_pic_lists[i]; j++)
                {
                    this.ref_pic_list_struct = new RefPicListStruct(i, j);
                    size += stream.ReadClass<RefPicListStruct>(size, context, this.ref_pic_list_struct);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_ref_wraparound_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_temporal_mvp_enabled_flag);

            if (sps_temporal_mvp_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_sbtmvp_enabled_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_amvr_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_bdof_enabled_flag);

            if (sps_bdof_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_bdof_control_present_in_ph_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_smvd_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_dmvr_enabled_flag);

            if (sps_dmvr_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_dmvr_control_present_in_ph_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_mmvd_enabled_flag);

            if (sps_mmvd_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_mmvd_fullpel_only_enabled_flag);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_six_minus_max_num_merge_cand);
            ((H266Context)context).OnSpsSixMinusMaxNumMergeCand();
            size += stream.ReadUnsignedInt(size, 1, out this.sps_sbt_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_affine_enabled_flag);

            if (sps_affine_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_five_minus_max_num_subblock_merge_cand);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_6param_affine_enabled_flag);

                if (sps_amvr_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_affine_amvr_enabled_flag);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.sps_affine_prof_enabled_flag);

                if (sps_affine_prof_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_prof_control_present_in_ph_flag);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_bcw_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_ciip_enabled_flag);

            if (((H266Context)context).MaxNumMergeCand >= 2)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_gpm_enabled_flag);

                if (sps_gpm_enabled_flag != 0 && ((H266Context)context).MaxNumMergeCand >= 3)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_num_merge_cand_minus_max_num_gpm_cand);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_log2_parallel_merge_level_minus2);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_isp_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_mrl_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_mip_enabled_flag);

            if (sps_chroma_format_idc != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_cclm_enabled_flag);
            }

            if (sps_chroma_format_idc == 1)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_chroma_horizontal_collocated_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_chroma_vertical_collocated_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_palette_enabled_flag);

            if (sps_chroma_format_idc == 3 && sps_max_luma_transform_size_64_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_act_enabled_flag);
            }

            if (sps_transform_skip_enabled_flag != 0 || sps_palette_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_min_qp_prime_ts);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_ibc_enabled_flag);

            if (sps_ibc_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_six_minus_max_num_ibc_merge_cand);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_ladf_enabled_flag);

            if (sps_ladf_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.sps_num_ladf_intervals_minus2);
                size += stream.ReadSignedIntGolomb(size, out this.sps_ladf_lowest_interval_qp_offset);

                this.sps_ladf_qp_offset = new int[sps_num_ladf_intervals_minus2 + 1];
                this.sps_ladf_delta_threshold_minus1 = new uint[sps_num_ladf_intervals_minus2 + 1];
                for (i = 0; i < sps_num_ladf_intervals_minus2 + 1; i++)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.sps_ladf_qp_offset[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_ladf_delta_threshold_minus1[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_explicit_scaling_list_enabled_flag);

            if (sps_lfnst_enabled_flag != 0 && sps_explicit_scaling_list_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_scaling_matrix_for_lfnst_disabled_flag);
            }

            if (sps_act_enabled_flag != 0 && sps_explicit_scaling_list_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_scaling_matrix_for_alternative_colour_space_disabled_flag);
            }

            if (sps_scaling_matrix_for_alternative_colour_space_disabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_scaling_matrix_designated_colour_space_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_dep_quant_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_sign_data_hiding_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_virtual_boundaries_enabled_flag);

            if (sps_virtual_boundaries_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_virtual_boundaries_present_flag);

                if (sps_virtual_boundaries_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_num_ver_virtual_boundaries);

                    this.sps_virtual_boundary_pos_x_minus1 = new uint[sps_num_ver_virtual_boundaries];
                    for (i = 0; i < sps_num_ver_virtual_boundaries; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.sps_virtual_boundary_pos_x_minus1[i]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_num_hor_virtual_boundaries);

                    this.sps_virtual_boundary_pos_y_minus1 = new uint[sps_num_hor_virtual_boundaries];
                    for (i = 0; i < sps_num_hor_virtual_boundaries; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.sps_virtual_boundary_pos_y_minus1[i]);
                    }
                }
            }

            if (sps_ptl_dpb_hrd_params_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_timing_hrd_params_present_flag);

                if (sps_timing_hrd_params_present_flag != 0)
                {
                    this.general_timing_hrd_parameters = new GeneralTimingHrdParameters();
                    size += stream.ReadClass<GeneralTimingHrdParameters>(size, context, this.general_timing_hrd_parameters);
                    ((H266Context)context).SetGeneralTimingHrdParameters(general_timing_hrd_parameters);

                    if (sps_max_sublayers_minus1 > 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sps_sublayer_cpb_params_present_flag);
                    }
                    firstSubLayer = sps_sublayer_cpb_params_present_flag != 0 ? 0 : sps_max_sublayers_minus1;
                    this.ols_timing_hrd_parameters = new OlsTimingHrdParameters(firstSubLayer, sps_max_sublayers_minus1);
                    size += stream.ReadClass<OlsTimingHrdParameters>(size, context, this.ols_timing_hrd_parameters);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_field_seq_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sps_vui_parameters_present_flag);

            if (sps_vui_parameters_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sps_vui_payload_size_minus1);

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadFixed(size, 1, whileIndex, this.sps_vui_alignment_zero_bit);
                }
                stream.MarkCurrentBitsPosition();
                this.vui_payload = new VuiPayload(sps_vui_payload_size_minus1 + 1);
                size += stream.ReadClass<VuiPayload>(size, context, this.vui_payload);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_extension_flag);

            if (sps_extension_flag != 0)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.sps_extension_data_flag);
                }
            }
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            long numQpTables = 0;
            uint j = 0;
            uint firstSubLayer = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(4, this.sps_seq_parameter_set_id);
            size += stream.WriteUnsignedInt(4, this.sps_video_parameter_set_id);
            size += stream.WriteUnsignedInt(3, this.sps_max_sublayers_minus1);
            size += stream.WriteUnsignedInt(2, this.sps_chroma_format_idc);
            size += stream.WriteUnsignedInt(2, this.sps_log2_ctu_size_minus5);
            ((H266Context)context).OnSpsLog2CtuSizeMinus5();
            size += stream.WriteUnsignedInt(1, this.sps_ptl_dpb_hrd_params_present_flag);

            if (sps_ptl_dpb_hrd_params_present_flag != 0)
            {
                size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level);
            }
            size += stream.WriteUnsignedInt(1, this.sps_gdr_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_ref_pic_resampling_enabled_flag);

            if (sps_ref_pic_resampling_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_res_change_in_clvs_allowed_flag);
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_pic_width_max_in_luma_samples);
            size += stream.WriteUnsignedIntGolomb(this.sps_pic_height_max_in_luma_samples);
            size += stream.WriteUnsignedInt(1, this.sps_conformance_window_flag);

            if (sps_conformance_window_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_conf_win_left_offset);
                size += stream.WriteUnsignedIntGolomb(this.sps_conf_win_right_offset);
                size += stream.WriteUnsignedIntGolomb(this.sps_conf_win_top_offset);
                size += stream.WriteUnsignedIntGolomb(this.sps_conf_win_bottom_offset);
            }
            size += stream.WriteUnsignedInt(1, this.sps_subpic_info_present_flag);

            if (sps_subpic_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_num_subpics_minus1);

                if (sps_num_subpics_minus1 > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sps_independent_subpics_flag);
                    size += stream.WriteUnsignedInt(1, this.sps_subpic_same_size_flag);
                }

                for (i = 0; sps_num_subpics_minus1 > 0 && i <= sps_num_subpics_minus1; i++)
                {

                    if (sps_subpic_same_size_flag == 0 || i == 0)
                    {

                        if (i > 0 && sps_pic_width_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2((sps_pic_width_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), this.sps_subpic_ctu_top_left_x[i]);
                        }

                        if (i > 0 && sps_pic_height_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2((sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), this.sps_subpic_ctu_top_left_y[i]);
                        }

                        if (i < sps_num_subpics_minus1 &&
      sps_pic_width_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2((sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), this.sps_subpic_width_minus1[i]);
                        }

                        if (i < sps_num_subpics_minus1 &&
      sps_pic_height_max_in_luma_samples > ((H266Context)context).CtbSizeY)
                        {
                            size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2((sps_pic_height_max_in_luma_samples + ((H266Context)context).CtbSizeY - 1) / ((H266Context)context).CtbSizeY)), this.sps_subpic_height_minus1[i]);
                        }
                    }

                    if (sps_independent_subpics_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sps_subpic_treated_as_pic_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sps_loop_filter_across_subpic_enabled_flag[i]);
                    }
                }
                size += stream.WriteUnsignedIntGolomb(this.sps_subpic_id_len_minus1);
                size += stream.WriteUnsignedInt(1, this.sps_subpic_id_mapping_explicitly_signalled_flag);

                if (sps_subpic_id_mapping_explicitly_signalled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sps_subpic_id_mapping_present_flag);

                    if (sps_subpic_id_mapping_present_flag != 0)
                    {

                        for (i = 0; i <= sps_num_subpics_minus1; i++)
                        {
                            size += stream.WriteUnsignedIntVariable(sps_subpic_id_len_minus1 + 1, this.sps_subpic_id[i]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_bitdepth_minus8);
            size += stream.WriteUnsignedInt(1, this.sps_entropy_coding_sync_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_entry_point_offsets_present_flag);
            size += stream.WriteUnsignedInt(4, this.sps_log2_max_pic_order_cnt_lsb_minus4);
            size += stream.WriteUnsignedInt(1, this.sps_poc_msb_cycle_flag);

            if (sps_poc_msb_cycle_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_poc_msb_cycle_len_minus1);
            }
            size += stream.WriteUnsignedInt(2, this.sps_num_extra_ph_bytes);

            for (i = 0; i < (sps_num_extra_ph_bytes * 8); i++)
            {
                size += stream.WriteUnsignedInt(1, this.sps_extra_ph_bit_present_flag[i]);
                ((H266Context)context).OnSpsExtraPhBitPresentFlag();
            }
            size += stream.WriteUnsignedInt(2, this.sps_num_extra_sh_bytes);

            for (i = 0; i < (sps_num_extra_sh_bytes * 8); i++)
            {
                size += stream.WriteUnsignedInt(1, this.sps_extra_sh_bit_present_flag[i]);
                ((H266Context)context).OnSpsExtraShBitPresentFlag();
            }

            if (sps_ptl_dpb_hrd_params_present_flag != 0)
            {

                if (sps_max_sublayers_minus1 > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sps_sublayer_dpb_params_flag);
                }
                size += stream.WriteClass<DpbParameters>(context, this.dpb_parameters);
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_log2_min_luma_coding_block_size_minus2);
            ((H266Context)context).OnSpsLog2MinLumaCodingBlockSizeMinus2();
            size += stream.WriteUnsignedInt(1, this.sps_partition_constraints_override_enabled_flag);
            size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_min_qt_min_cb_intra_slice_luma);
            size += stream.WriteUnsignedIntGolomb(this.sps_max_mtt_hierarchy_depth_intra_slice_luma);

            if (sps_max_mtt_hierarchy_depth_intra_slice_luma != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_max_bt_min_qt_intra_slice_luma);
                size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_max_tt_min_qt_intra_slice_luma);
            }

            if (sps_chroma_format_idc != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_qtbtt_dual_tree_intra_flag);
            }

            if (sps_qtbtt_dual_tree_intra_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_min_qt_min_cb_intra_slice_chroma);
                size += stream.WriteUnsignedIntGolomb(this.sps_max_mtt_hierarchy_depth_intra_slice_chroma);

                if (sps_max_mtt_hierarchy_depth_intra_slice_chroma != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_max_bt_min_qt_intra_slice_chroma);
                    size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_max_tt_min_qt_intra_slice_chroma);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_min_qt_min_cb_inter_slice);
            size += stream.WriteUnsignedIntGolomb(this.sps_max_mtt_hierarchy_depth_inter_slice);

            if (sps_max_mtt_hierarchy_depth_inter_slice != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_max_bt_min_qt_inter_slice);
                size += stream.WriteUnsignedIntGolomb(this.sps_log2_diff_max_tt_min_qt_inter_slice);
            }

            if (((H266Context)context).CtbSizeY > 32)
            {
                size += stream.WriteUnsignedInt(1, this.sps_max_luma_transform_size_64_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_transform_skip_enabled_flag);

            if (sps_transform_skip_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_log2_transform_skip_max_size_minus2);
                size += stream.WriteUnsignedInt(1, this.sps_bdpcm_enabled_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_mts_enabled_flag);

            if (sps_mts_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_explicit_mts_intra_enabled_flag);
                size += stream.WriteUnsignedInt(1, this.sps_explicit_mts_inter_enabled_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_lfnst_enabled_flag);

            if (sps_chroma_format_idc != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_joint_cbcr_enabled_flag);
                size += stream.WriteUnsignedInt(1, this.sps_same_qp_table_for_chroma_flag);
                numQpTables = sps_same_qp_table_for_chroma_flag != 0 ? 1 : (sps_joint_cbcr_enabled_flag != 0 ? 3 : 2);

                for (i = 0; i < numQpTables; i++)
                {
                    size += stream.WriteSignedIntGolomb(this.sps_qp_table_start_minus26[i]);
                    size += stream.WriteUnsignedIntGolomb(this.sps_num_points_in_qp_table_minus1[i]);

                    for (j = 0; j <= sps_num_points_in_qp_table_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.sps_delta_qp_in_val_minus1[i][j]);
                        size += stream.WriteUnsignedIntGolomb(this.sps_delta_qp_diff_val[i][j]);
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.sps_sao_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_alf_enabled_flag);

            if (sps_alf_enabled_flag != 0 && sps_chroma_format_idc != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_ccalf_enabled_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_lmcs_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_weighted_pred_flag);
            size += stream.WriteUnsignedInt(1, this.sps_weighted_bipred_flag);
            size += stream.WriteUnsignedInt(1, this.sps_long_term_ref_pics_flag);

            if (sps_video_parameter_set_id > 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_inter_layer_prediction_enabled_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_idr_rpl_present_flag);
            size += stream.WriteUnsignedInt(1, this.sps_rpl1_same_as_rpl0_flag);

            for (i = 0; i < (sps_rpl1_same_as_rpl0_flag != 0 ? 1 : 2); i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_num_ref_pic_lists[i]);

                for (j = 0; j < sps_num_ref_pic_lists[i]; j++)
                {
                    this.ref_pic_list_struct.ListIdx = i;
                    this.ref_pic_list_struct.RplsIdx = j;
                    size += stream.WriteClass<RefPicListStruct>(context, this.ref_pic_list_struct);
                }
            }
            size += stream.WriteUnsignedInt(1, this.sps_ref_wraparound_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_temporal_mvp_enabled_flag);

            if (sps_temporal_mvp_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_sbtmvp_enabled_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_amvr_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_bdof_enabled_flag);

            if (sps_bdof_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_bdof_control_present_in_ph_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_smvd_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_dmvr_enabled_flag);

            if (sps_dmvr_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_dmvr_control_present_in_ph_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_mmvd_enabled_flag);

            if (sps_mmvd_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_mmvd_fullpel_only_enabled_flag);
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_six_minus_max_num_merge_cand);
            ((H266Context)context).OnSpsSixMinusMaxNumMergeCand();
            size += stream.WriteUnsignedInt(1, this.sps_sbt_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_affine_enabled_flag);

            if (sps_affine_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_five_minus_max_num_subblock_merge_cand);
                size += stream.WriteUnsignedInt(1, this.sps_6param_affine_enabled_flag);

                if (sps_amvr_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sps_affine_amvr_enabled_flag);
                }
                size += stream.WriteUnsignedInt(1, this.sps_affine_prof_enabled_flag);

                if (sps_affine_prof_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sps_prof_control_present_in_ph_flag);
                }
            }
            size += stream.WriteUnsignedInt(1, this.sps_bcw_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_ciip_enabled_flag);

            if (((H266Context)context).MaxNumMergeCand >= 2)
            {
                size += stream.WriteUnsignedInt(1, this.sps_gpm_enabled_flag);

                if (sps_gpm_enabled_flag != 0 && ((H266Context)context).MaxNumMergeCand >= 3)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sps_max_num_merge_cand_minus_max_num_gpm_cand);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_log2_parallel_merge_level_minus2);
            size += stream.WriteUnsignedInt(1, this.sps_isp_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_mrl_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_mip_enabled_flag);

            if (sps_chroma_format_idc != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_cclm_enabled_flag);
            }

            if (sps_chroma_format_idc == 1)
            {
                size += stream.WriteUnsignedInt(1, this.sps_chroma_horizontal_collocated_flag);
                size += stream.WriteUnsignedInt(1, this.sps_chroma_vertical_collocated_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_palette_enabled_flag);

            if (sps_chroma_format_idc == 3 && sps_max_luma_transform_size_64_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_act_enabled_flag);
            }

            if (sps_transform_skip_enabled_flag != 0 || sps_palette_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_min_qp_prime_ts);
            }
            size += stream.WriteUnsignedInt(1, this.sps_ibc_enabled_flag);

            if (sps_ibc_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_six_minus_max_num_ibc_merge_cand);
            }
            size += stream.WriteUnsignedInt(1, this.sps_ladf_enabled_flag);

            if (sps_ladf_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(2, this.sps_num_ladf_intervals_minus2);
                size += stream.WriteSignedIntGolomb(this.sps_ladf_lowest_interval_qp_offset);

                for (i = 0; i < sps_num_ladf_intervals_minus2 + 1; i++)
                {
                    size += stream.WriteSignedIntGolomb(this.sps_ladf_qp_offset[i]);
                    size += stream.WriteUnsignedIntGolomb(this.sps_ladf_delta_threshold_minus1[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.sps_explicit_scaling_list_enabled_flag);

            if (sps_lfnst_enabled_flag != 0 && sps_explicit_scaling_list_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_scaling_matrix_for_lfnst_disabled_flag);
            }

            if (sps_act_enabled_flag != 0 && sps_explicit_scaling_list_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_scaling_matrix_for_alternative_colour_space_disabled_flag);
            }

            if (sps_scaling_matrix_for_alternative_colour_space_disabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_scaling_matrix_designated_colour_space_flag);
            }
            size += stream.WriteUnsignedInt(1, this.sps_dep_quant_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_sign_data_hiding_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sps_virtual_boundaries_enabled_flag);

            if (sps_virtual_boundaries_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_virtual_boundaries_present_flag);

                if (sps_virtual_boundaries_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sps_num_ver_virtual_boundaries);

                    for (i = 0; i < sps_num_ver_virtual_boundaries; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.sps_virtual_boundary_pos_x_minus1[i]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.sps_num_hor_virtual_boundaries);

                    for (i = 0; i < sps_num_hor_virtual_boundaries; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.sps_virtual_boundary_pos_y_minus1[i]);
                    }
                }
            }

            if (sps_ptl_dpb_hrd_params_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_timing_hrd_params_present_flag);

                if (sps_timing_hrd_params_present_flag != 0)
                {
                    size += stream.WriteClass<GeneralTimingHrdParameters>(context, this.general_timing_hrd_parameters);
                    ((H266Context)context).SetGeneralTimingHrdParameters(general_timing_hrd_parameters);

                    if (sps_max_sublayers_minus1 > 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sps_sublayer_cpb_params_present_flag);
                    }
                    firstSubLayer = sps_sublayer_cpb_params_present_flag != 0 ? 0 : sps_max_sublayers_minus1;
                    size += stream.WriteClass<OlsTimingHrdParameters>(context, this.ols_timing_hrd_parameters);
                }
            }
            size += stream.WriteUnsignedInt(1, this.sps_field_seq_flag);
            size += stream.WriteUnsignedInt(1, this.sps_vui_parameters_present_flag);

            if (sps_vui_parameters_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sps_vui_payload_size_minus1);

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteFixed(1, whileIndex, this.sps_vui_alignment_zero_bit);
                }
                stream.MarkCurrentBitsPosition();
                size += stream.WriteClass<VuiPayload>(context, this.vui_payload);
            }
            size += stream.WriteUnsignedInt(1, this.sps_extension_flag);

            if (sps_extension_flag != 0)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.sps_extension_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

pic_parameter_set_rbsp() {  
 pps_pic_parameter_set_id u(6) 
 pps_seq_parameter_set_id u(4) 
 pps_mixed_nalu_types_in_pic_flag u(1) 
 pps_pic_width_in_luma_samples ue(v) 
 pps_pic_height_in_luma_samples ue(v) 
 pps_conformance_window_flag u(1) 
 if( pps_conformance_window_flag ) {  
  pps_conf_win_left_offset ue(v) 
  pps_conf_win_right_offset ue(v) 
  pps_conf_win_top_offset ue(v) 
  pps_conf_win_bottom_offset ue(v) 
 }  
 pps_scaling_window_explicit_signalling_flag u(1) 
 if( pps_scaling_window_explicit_signalling_flag ) {  
  pps_scaling_win_left_offset se(v) 
  pps_scaling_win_right_offset se(v) 
  pps_scaling_win_top_offset se(v) 
  pps_scaling_win_bottom_offset se(v) 
 }  
 pps_output_flag_present_flag u(1) 
 pps_no_pic_partition_flag u(1) 
 pps_subpic_id_mapping_present_flag u(1) 
 if( pps_subpic_id_mapping_present_flag ) {  
  if( !pps_no_pic_partition_flag )  
   pps_num_subpics_minus1 ue(v) 
  pps_subpic_id_len_minus1 ue(v) 
  for( i = 0; i  <=  pps_num_subpics_minus1; i++ )  
   pps_subpic_id[ i ] u(v) 
    }  
 if( !pps_no_pic_partition_flag ) {  
  pps_log2_ctu_size_minus5 u(2) 
  pps_num_exp_tile_columns_minus1 ue(v) 
  pps_num_exp_tile_rows_minus1 ue(v) 
  for( i = 0; i  <=  pps_num_exp_tile_columns_minus1; i++ )  
     pps_tile_column_width_minus1[ i ] ue(v) 
  for( i = 0; i  <=  pps_num_exp_tile_rows_minus1; i++ )  
   pps_tile_row_height_minus1[ i ] ue(v) 
  if( NumTilesInPic > 1 ) {  
   pps_loop_filter_across_tiles_enabled_flag u(1) 
   pps_rect_slice_flag u(1) 
  }  
  if( pps_rect_slice_flag )  
   pps_single_slice_per_subpic_flag u(1) 
  if( pps_rect_slice_flag  &&  !pps_single_slice_per_subpic_flag ) {  
   pps_num_slices_in_pic_minus1 ue(v) 
   if( pps_num_slices_in_pic_minus1 > 1 )  
    pps_tile_idx_delta_present_flag u(1) 
   for( i = 0; i < pps_num_slices_in_pic_minus1; i++ ) {  
    if( SliceTopLeftTileIdx[ i ] % NumTileColumns  !=  NumTileColumns - 1 )  
     pps_slice_width_in_tiles_minus1[ i ] ue(v) 
    if( SliceTopLeftTileIdx[ i ] / NumTileColumns  !=  NumTileRows - 1  && 
      ( pps_tile_idx_delta_present_flag  || 
      SliceTopLeftTileIdx[ i ] % NumTileColumns  ==  0 ) ) 
 
     pps_slice_height_in_tiles_minus1[ i ] ue(v) 
    if( pps_slice_width_in_tiles_minus1[ i ]  ==  0  && 
      pps_slice_height_in_tiles_minus1[ i ]  ==  0  && 
      RowHeightVal[ SliceTopLeftTileIdx[ i ] / NumTileColumns ] > 1 ) { 
 
     pps_num_exp_slices_in_tile[ i ] ue(v) 
     for( j = 0; j < pps_num_exp_slices_in_tile[ i ]; j++ )  
      pps_exp_slice_height_in_ctus_minus1[ i ][ j ] ue(v) 
     i  +=  NumSlicesInTile[ i ] - 1  
    }  
    if( pps_tile_idx_delta_present_flag  &&  i < pps_num_slices_in_pic_minus1 )  
     pps_tile_idx_delta_val[ i ] se(v) 
   }  
  }  
  if( !pps_rect_slice_flag  ||  pps_single_slice_per_subpic_flag  || 
    pps_num_slices_in_pic_minus1 > 0 ) 
 
   pps_loop_filter_across_slices_enabled_flag u(1) 
 }  
 pps_cabac_init_present_flag u(1) 
 for( i = 0; i < 2; i++ )  
  pps_num_ref_idx_default_active_minus1[ i ] ue(v) 
 pps_rpl1_idx_present_flag u(1) 
 pps_weighted_pred_flag u(1) 
 pps_weighted_bipred_flag u(1) 
 pps_ref_wraparound_enabled_flag u(1) 
 if( pps_ref_wraparound_enabled_flag )  
  pps_pic_width_minus_wraparound_offset ue(v) 
 pps_init_qp_minus26 se(v) 
 pps_cu_qp_delta_enabled_flag u(1) 
 pps_chroma_tool_offsets_present_flag u(1) 
 if( pps_chroma_tool_offsets_present_flag ) {  
   pps_cb_qp_offset se(v) 
  pps_cr_qp_offset se(v) 
  pps_joint_cbcr_qp_offset_present_flag u(1) 
  if( pps_joint_cbcr_qp_offset_present_flag )  
   pps_joint_cbcr_qp_offset_value se(v) 
  pps_slice_chroma_qp_offsets_present_flag u(1) 
  pps_cu_chroma_qp_offset_list_enabled_flag u(1) 
  if( pps_cu_chroma_qp_offset_list_enabled_flag ) {  
   pps_chroma_qp_offset_list_len_minus1 ue(v) 
   for( i = 0; i  <=  pps_chroma_qp_offset_list_len_minus1; i++ ) {  
    pps_cb_qp_offset_list[ i ] se(v) 
    pps_cr_qp_offset_list[ i ] se(v) 
    if( pps_joint_cbcr_qp_offset_present_flag )  
     pps_joint_cbcr_qp_offset_list[ i ] se(v) 
   }  
  }  
 }  
 pps_deblocking_filter_control_present_flag u(1) 
 if( pps_deblocking_filter_control_present_flag ) {  
  pps_deblocking_filter_override_enabled_flag u(1) 
  pps_deblocking_filter_disabled_flag u(1) 
  if( !pps_no_pic_partition_flag  &&  pps_deblocking_filter_override_enabled_flag )  
   pps_dbf_info_in_ph_flag u(1) 
  if( !pps_deblocking_filter_disabled_flag ) {  
   pps_luma_beta_offset_div2 se(v) 
   pps_luma_tc_offset_div2 se(v) 
   if( pps_chroma_tool_offsets_present_flag ) {  
    pps_cb_beta_offset_div2 se(v) 
    pps_cb_tc_offset_div2 se(v) 
    pps_cr_beta_offset_div2 se(v) 
    pps_cr_tc_offset_div2 se(v) 
   }  
  }  
 }  
 if( !pps_no_pic_partition_flag ) {  
  pps_rpl_info_in_ph_flag u(1) 
  pps_sao_info_in_ph_flag u(1) 
  pps_alf_info_in_ph_flag u(1) 
  if( ( pps_weighted_pred_flag  ||  pps_weighted_bipred_flag )  && 
    pps_rpl_info_in_ph_flag ) 
 
   pps_wp_info_in_ph_flag u(1) 
  pps_qp_delta_info_in_ph_flag u(1) 
 }  
 pps_picture_header_extension_present_flag u(1) 
 pps_slice_header_extension_present_flag u(1) 
 pps_extension_flag u(1) 
 if( pps_extension_flag )  
  while( more_rbsp_data() )  
   pps_extension_data_flag u(1) 
    rbsp_trailing_bits()  
}
    */
    public class PicParameterSetRbsp : IItuSerializable
    {
        private uint pps_pic_parameter_set_id;
        public uint PpsPicParameterSetId { get { return pps_pic_parameter_set_id; } set { pps_pic_parameter_set_id = value; } }
        private uint pps_seq_parameter_set_id;
        public uint PpsSeqParameterSetId { get { return pps_seq_parameter_set_id; } set { pps_seq_parameter_set_id = value; } }
        private byte pps_mixed_nalu_types_in_pic_flag;
        public byte PpsMixedNaluTypesInPicFlag { get { return pps_mixed_nalu_types_in_pic_flag; } set { pps_mixed_nalu_types_in_pic_flag = value; } }
        private uint pps_pic_width_in_luma_samples;
        public uint PpsPicWidthInLumaSamples { get { return pps_pic_width_in_luma_samples; } set { pps_pic_width_in_luma_samples = value; } }
        private uint pps_pic_height_in_luma_samples;
        public uint PpsPicHeightInLumaSamples { get { return pps_pic_height_in_luma_samples; } set { pps_pic_height_in_luma_samples = value; } }
        private byte pps_conformance_window_flag;
        public byte PpsConformanceWindowFlag { get { return pps_conformance_window_flag; } set { pps_conformance_window_flag = value; } }
        private uint pps_conf_win_left_offset;
        public uint PpsConfWinLeftOffset { get { return pps_conf_win_left_offset; } set { pps_conf_win_left_offset = value; } }
        private uint pps_conf_win_right_offset;
        public uint PpsConfWinRightOffset { get { return pps_conf_win_right_offset; } set { pps_conf_win_right_offset = value; } }
        private uint pps_conf_win_top_offset;
        public uint PpsConfWinTopOffset { get { return pps_conf_win_top_offset; } set { pps_conf_win_top_offset = value; } }
        private uint pps_conf_win_bottom_offset;
        public uint PpsConfWinBottomOffset { get { return pps_conf_win_bottom_offset; } set { pps_conf_win_bottom_offset = value; } }
        private byte pps_scaling_window_explicit_signalling_flag;
        public byte PpsScalingWindowExplicitSignallingFlag { get { return pps_scaling_window_explicit_signalling_flag; } set { pps_scaling_window_explicit_signalling_flag = value; } }
        private int pps_scaling_win_left_offset;
        public int PpsScalingWinLeftOffset { get { return pps_scaling_win_left_offset; } set { pps_scaling_win_left_offset = value; } }
        private int pps_scaling_win_right_offset;
        public int PpsScalingWinRightOffset { get { return pps_scaling_win_right_offset; } set { pps_scaling_win_right_offset = value; } }
        private int pps_scaling_win_top_offset;
        public int PpsScalingWinTopOffset { get { return pps_scaling_win_top_offset; } set { pps_scaling_win_top_offset = value; } }
        private int pps_scaling_win_bottom_offset;
        public int PpsScalingWinBottomOffset { get { return pps_scaling_win_bottom_offset; } set { pps_scaling_win_bottom_offset = value; } }
        private byte pps_output_flag_present_flag;
        public byte PpsOutputFlagPresentFlag { get { return pps_output_flag_present_flag; } set { pps_output_flag_present_flag = value; } }
        private byte pps_no_pic_partition_flag;
        public byte PpsNoPicPartitionFlag { get { return pps_no_pic_partition_flag; } set { pps_no_pic_partition_flag = value; } }
        private byte pps_subpic_id_mapping_present_flag;
        public byte PpsSubpicIdMappingPresentFlag { get { return pps_subpic_id_mapping_present_flag; } set { pps_subpic_id_mapping_present_flag = value; } }
        private uint pps_num_subpics_minus1;
        public uint PpsNumSubpicsMinus1 { get { return pps_num_subpics_minus1; } set { pps_num_subpics_minus1 = value; } }
        private uint pps_subpic_id_len_minus1;
        public uint PpsSubpicIdLenMinus1 { get { return pps_subpic_id_len_minus1; } set { pps_subpic_id_len_minus1 = value; } }
        private uint[] pps_subpic_id;
        public uint[] PpsSubpicId { get { return pps_subpic_id; } set { pps_subpic_id = value; } }
        private uint pps_log2_ctu_size_minus5;
        public uint PpsLog2CtuSizeMinus5 { get { return pps_log2_ctu_size_minus5; } set { pps_log2_ctu_size_minus5 = value; } }
        private uint pps_num_exp_tile_columns_minus1;
        public uint PpsNumExpTileColumnsMinus1 { get { return pps_num_exp_tile_columns_minus1; } set { pps_num_exp_tile_columns_minus1 = value; } }
        private uint pps_num_exp_tile_rows_minus1;
        public uint PpsNumExpTileRowsMinus1 { get { return pps_num_exp_tile_rows_minus1; } set { pps_num_exp_tile_rows_minus1 = value; } }
        private uint[] pps_tile_column_width_minus1;
        public uint[] PpsTileColumnWidthMinus1 { get { return pps_tile_column_width_minus1; } set { pps_tile_column_width_minus1 = value; } }
        private uint[] pps_tile_row_height_minus1;
        public uint[] PpsTileRowHeightMinus1 { get { return pps_tile_row_height_minus1; } set { pps_tile_row_height_minus1 = value; } }
        private byte pps_loop_filter_across_tiles_enabled_flag;
        public byte PpsLoopFilterAcrossTilesEnabledFlag { get { return pps_loop_filter_across_tiles_enabled_flag; } set { pps_loop_filter_across_tiles_enabled_flag = value; } }
        private byte pps_rect_slice_flag;
        public byte PpsRectSliceFlag { get { return pps_rect_slice_flag; } set { pps_rect_slice_flag = value; } }
        private byte pps_single_slice_per_subpic_flag;
        public byte PpsSingleSlicePerSubpicFlag { get { return pps_single_slice_per_subpic_flag; } set { pps_single_slice_per_subpic_flag = value; } }
        private uint pps_num_slices_in_pic_minus1;
        public uint PpsNumSlicesInPicMinus1 { get { return pps_num_slices_in_pic_minus1; } set { pps_num_slices_in_pic_minus1 = value; } }
        private byte pps_tile_idx_delta_present_flag;
        public byte PpsTileIdxDeltaPresentFlag { get { return pps_tile_idx_delta_present_flag; } set { pps_tile_idx_delta_present_flag = value; } }
        private uint[] pps_slice_width_in_tiles_minus1;
        public uint[] PpsSliceWidthInTilesMinus1 { get { return pps_slice_width_in_tiles_minus1; } set { pps_slice_width_in_tiles_minus1 = value; } }
        private uint[] pps_slice_height_in_tiles_minus1;
        public uint[] PpsSliceHeightInTilesMinus1 { get { return pps_slice_height_in_tiles_minus1; } set { pps_slice_height_in_tiles_minus1 = value; } }
        private uint[] pps_num_exp_slices_in_tile;
        public uint[] PpsNumExpSlicesInTile { get { return pps_num_exp_slices_in_tile; } set { pps_num_exp_slices_in_tile = value; } }
        private uint[][] pps_exp_slice_height_in_ctus_minus1;
        public uint[][] PpsExpSliceHeightInCtusMinus1 { get { return pps_exp_slice_height_in_ctus_minus1; } set { pps_exp_slice_height_in_ctus_minus1 = value; } }
        private int[] pps_tile_idx_delta_val;
        public int[] PpsTileIdxDeltaVal { get { return pps_tile_idx_delta_val; } set { pps_tile_idx_delta_val = value; } }
        private byte pps_loop_filter_across_slices_enabled_flag;
        public byte PpsLoopFilterAcrossSlicesEnabledFlag { get { return pps_loop_filter_across_slices_enabled_flag; } set { pps_loop_filter_across_slices_enabled_flag = value; } }
        private byte pps_cabac_init_present_flag;
        public byte PpsCabacInitPresentFlag { get { return pps_cabac_init_present_flag; } set { pps_cabac_init_present_flag = value; } }
        private uint[] pps_num_ref_idx_default_active_minus1;
        public uint[] PpsNumRefIdxDefaultActiveMinus1 { get { return pps_num_ref_idx_default_active_minus1; } set { pps_num_ref_idx_default_active_minus1 = value; } }
        private byte pps_rpl1_idx_present_flag;
        public byte PpsRpl1IdxPresentFlag { get { return pps_rpl1_idx_present_flag; } set { pps_rpl1_idx_present_flag = value; } }
        private byte pps_weighted_pred_flag;
        public byte PpsWeightedPredFlag { get { return pps_weighted_pred_flag; } set { pps_weighted_pred_flag = value; } }
        private byte pps_weighted_bipred_flag;
        public byte PpsWeightedBipredFlag { get { return pps_weighted_bipred_flag; } set { pps_weighted_bipred_flag = value; } }
        private byte pps_ref_wraparound_enabled_flag;
        public byte PpsRefWraparoundEnabledFlag { get { return pps_ref_wraparound_enabled_flag; } set { pps_ref_wraparound_enabled_flag = value; } }
        private uint pps_pic_width_minus_wraparound_offset;
        public uint PpsPicWidthMinusWraparoundOffset { get { return pps_pic_width_minus_wraparound_offset; } set { pps_pic_width_minus_wraparound_offset = value; } }
        private int pps_init_qp_minus26;
        public int PpsInitQpMinus26 { get { return pps_init_qp_minus26; } set { pps_init_qp_minus26 = value; } }
        private byte pps_cu_qp_delta_enabled_flag;
        public byte PpsCuQpDeltaEnabledFlag { get { return pps_cu_qp_delta_enabled_flag; } set { pps_cu_qp_delta_enabled_flag = value; } }
        private byte pps_chroma_tool_offsets_present_flag;
        public byte PpsChromaToolOffsetsPresentFlag { get { return pps_chroma_tool_offsets_present_flag; } set { pps_chroma_tool_offsets_present_flag = value; } }
        private int pps_cb_qp_offset;
        public int PpsCbQpOffset { get { return pps_cb_qp_offset; } set { pps_cb_qp_offset = value; } }
        private int pps_cr_qp_offset;
        public int PpsCrQpOffset { get { return pps_cr_qp_offset; } set { pps_cr_qp_offset = value; } }
        private byte pps_joint_cbcr_qp_offset_present_flag;
        public byte PpsJointCbcrQpOffsetPresentFlag { get { return pps_joint_cbcr_qp_offset_present_flag; } set { pps_joint_cbcr_qp_offset_present_flag = value; } }
        private int pps_joint_cbcr_qp_offset_value;
        public int PpsJointCbcrQpOffsetValue { get { return pps_joint_cbcr_qp_offset_value; } set { pps_joint_cbcr_qp_offset_value = value; } }
        private byte pps_slice_chroma_qp_offsets_present_flag;
        public byte PpsSliceChromaQpOffsetsPresentFlag { get { return pps_slice_chroma_qp_offsets_present_flag; } set { pps_slice_chroma_qp_offsets_present_flag = value; } }
        private byte pps_cu_chroma_qp_offset_list_enabled_flag;
        public byte PpsCuChromaQpOffsetListEnabledFlag { get { return pps_cu_chroma_qp_offset_list_enabled_flag; } set { pps_cu_chroma_qp_offset_list_enabled_flag = value; } }
        private uint pps_chroma_qp_offset_list_len_minus1;
        public uint PpsChromaQpOffsetListLenMinus1 { get { return pps_chroma_qp_offset_list_len_minus1; } set { pps_chroma_qp_offset_list_len_minus1 = value; } }
        private int[] pps_cb_qp_offset_list;
        public int[] PpsCbQpOffsetList { get { return pps_cb_qp_offset_list; } set { pps_cb_qp_offset_list = value; } }
        private int[] pps_cr_qp_offset_list;
        public int[] PpsCrQpOffsetList { get { return pps_cr_qp_offset_list; } set { pps_cr_qp_offset_list = value; } }
        private int[] pps_joint_cbcr_qp_offset_list;
        public int[] PpsJointCbcrQpOffsetList { get { return pps_joint_cbcr_qp_offset_list; } set { pps_joint_cbcr_qp_offset_list = value; } }
        private byte pps_deblocking_filter_control_present_flag;
        public byte PpsDeblockingFilterControlPresentFlag { get { return pps_deblocking_filter_control_present_flag; } set { pps_deblocking_filter_control_present_flag = value; } }
        private byte pps_deblocking_filter_override_enabled_flag;
        public byte PpsDeblockingFilterOverrideEnabledFlag { get { return pps_deblocking_filter_override_enabled_flag; } set { pps_deblocking_filter_override_enabled_flag = value; } }
        private byte pps_deblocking_filter_disabled_flag;
        public byte PpsDeblockingFilterDisabledFlag { get { return pps_deblocking_filter_disabled_flag; } set { pps_deblocking_filter_disabled_flag = value; } }
        private byte pps_dbf_info_in_ph_flag;
        public byte PpsDbfInfoInPhFlag { get { return pps_dbf_info_in_ph_flag; } set { pps_dbf_info_in_ph_flag = value; } }
        private int pps_luma_beta_offset_div2;
        public int PpsLumaBetaOffsetDiv2 { get { return pps_luma_beta_offset_div2; } set { pps_luma_beta_offset_div2 = value; } }
        private int pps_luma_tc_offset_div2;
        public int PpsLumaTcOffsetDiv2 { get { return pps_luma_tc_offset_div2; } set { pps_luma_tc_offset_div2 = value; } }
        private int pps_cb_beta_offset_div2;
        public int PpsCbBetaOffsetDiv2 { get { return pps_cb_beta_offset_div2; } set { pps_cb_beta_offset_div2 = value; } }
        private int pps_cb_tc_offset_div2;
        public int PpsCbTcOffsetDiv2 { get { return pps_cb_tc_offset_div2; } set { pps_cb_tc_offset_div2 = value; } }
        private int pps_cr_beta_offset_div2;
        public int PpsCrBetaOffsetDiv2 { get { return pps_cr_beta_offset_div2; } set { pps_cr_beta_offset_div2 = value; } }
        private int pps_cr_tc_offset_div2;
        public int PpsCrTcOffsetDiv2 { get { return pps_cr_tc_offset_div2; } set { pps_cr_tc_offset_div2 = value; } }
        private byte pps_rpl_info_in_ph_flag;
        public byte PpsRplInfoInPhFlag { get { return pps_rpl_info_in_ph_flag; } set { pps_rpl_info_in_ph_flag = value; } }
        private byte pps_sao_info_in_ph_flag;
        public byte PpsSaoInfoInPhFlag { get { return pps_sao_info_in_ph_flag; } set { pps_sao_info_in_ph_flag = value; } }
        private byte pps_alf_info_in_ph_flag;
        public byte PpsAlfInfoInPhFlag { get { return pps_alf_info_in_ph_flag; } set { pps_alf_info_in_ph_flag = value; } }
        private byte pps_wp_info_in_ph_flag;
        public byte PpsWpInfoInPhFlag { get { return pps_wp_info_in_ph_flag; } set { pps_wp_info_in_ph_flag = value; } }
        private byte pps_qp_delta_info_in_ph_flag;
        public byte PpsQpDeltaInfoInPhFlag { get { return pps_qp_delta_info_in_ph_flag; } set { pps_qp_delta_info_in_ph_flag = value; } }
        private byte pps_picture_header_extension_present_flag;
        public byte PpsPictureHeaderExtensionPresentFlag { get { return pps_picture_header_extension_present_flag; } set { pps_picture_header_extension_present_flag = value; } }
        private byte pps_slice_header_extension_present_flag;
        public byte PpsSliceHeaderExtensionPresentFlag { get { return pps_slice_header_extension_present_flag; } set { pps_slice_header_extension_present_flag = value; } }
        private byte pps_extension_flag;
        public byte PpsExtensionFlag { get { return pps_extension_flag; } set { pps_extension_flag = value; } }
        private Dictionary<int, byte> pps_extension_data_flag = new Dictionary<int, byte>();
        public Dictionary<int, byte> PpsExtensionDataFlag { get { return pps_extension_data_flag; } set { pps_extension_data_flag = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PicParameterSetRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 6, out this.pps_pic_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 4, out this.pps_seq_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_mixed_nalu_types_in_pic_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.pps_pic_width_in_luma_samples);
            size += stream.ReadUnsignedIntGolomb(size, out this.pps_pic_height_in_luma_samples);
            ((H266Context)context).OnPpsPicHeightInLumaSamples();
            size += stream.ReadUnsignedInt(size, 1, out this.pps_conformance_window_flag);

            if (pps_conformance_window_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_conf_win_left_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_conf_win_right_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_conf_win_top_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_conf_win_bottom_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_scaling_window_explicit_signalling_flag);

            if (pps_scaling_window_explicit_signalling_flag != 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.pps_scaling_win_left_offset);
                size += stream.ReadSignedIntGolomb(size, out this.pps_scaling_win_right_offset);
                size += stream.ReadSignedIntGolomb(size, out this.pps_scaling_win_top_offset);
                size += stream.ReadSignedIntGolomb(size, out this.pps_scaling_win_bottom_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_output_flag_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_no_pic_partition_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_subpic_id_mapping_present_flag);

            if (pps_subpic_id_mapping_present_flag != 0)
            {

                if (pps_no_pic_partition_flag == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_subpics_minus1);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_subpic_id_len_minus1);

                this.pps_subpic_id = new uint[pps_num_subpics_minus1 + 1];
                for (i = 0; i <= pps_num_subpics_minus1; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, pps_subpic_id_len_minus1 + 1, out this.pps_subpic_id[i]);
                    ((H266Context)context).OnPpsSubpicId();
                }
            }

            if (pps_no_pic_partition_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.pps_log2_ctu_size_minus5);
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_exp_tile_columns_minus1);
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_exp_tile_rows_minus1);

                this.pps_tile_column_width_minus1 = new uint[pps_num_exp_tile_columns_minus1 + 1];
                for (i = 0; i <= pps_num_exp_tile_columns_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.pps_tile_column_width_minus1[i]);
                }

                this.pps_tile_row_height_minus1 = new uint[pps_num_exp_tile_rows_minus1 + 1];
                for (i = 0; i <= pps_num_exp_tile_rows_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.pps_tile_row_height_minus1[i]);
                    ((H266Context)context).OnPpsTileRowHeightMinus1();
                }

                if (((H266Context)context).NumTilesInPic > 1)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pps_loop_filter_across_tiles_enabled_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.pps_rect_slice_flag);
                }

                if (pps_rect_slice_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pps_single_slice_per_subpic_flag);
                }

                if (pps_rect_slice_flag != 0 && pps_single_slice_per_subpic_flag == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_slices_in_pic_minus1);

                    if (pps_num_slices_in_pic_minus1 > 1)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.pps_tile_idx_delta_present_flag);
                    }

                    this.pps_slice_width_in_tiles_minus1 = new uint[pps_num_slices_in_pic_minus1 + 1];
                    this.pps_slice_height_in_tiles_minus1 = new uint[pps_num_slices_in_pic_minus1 + 1];
                    this.pps_num_exp_slices_in_tile = new uint[pps_num_slices_in_pic_minus1 + 1];
                    this.pps_exp_slice_height_in_ctus_minus1 = new uint[pps_num_slices_in_pic_minus1 + 1][];
                    this.pps_tile_idx_delta_val = new int[pps_num_slices_in_pic_minus1 + 1];
                    for (i = 0; i < pps_num_slices_in_pic_minus1; i++)
                    {

                        if (((H266Context)context).SliceTopLeftTileIdx[i] % ((H266Context)context).NumTileColumns != ((H266Context)context).NumTileColumns - 1)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.pps_slice_width_in_tiles_minus1[i]);
                        }

                        if (((H266Context)context).SliceTopLeftTileIdx[i] / ((H266Context)context).NumTileColumns != ((H266Context)context).NumTileRows - 1 &&
      (pps_tile_idx_delta_present_flag != 0 ||
      ((H266Context)context).SliceTopLeftTileIdx[i] % ((H266Context)context).NumTileColumns == 0))
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.pps_slice_height_in_tiles_minus1[i]);
                        }

                        if (pps_slice_width_in_tiles_minus1[i] == 0 &&
      pps_slice_height_in_tiles_minus1[i] == 0 &&
      ((H266Context)context).RowHeightVal[((H266Context)context).SliceTopLeftTileIdx[i] / ((H266Context)context).NumTileColumns] > 1)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_exp_slices_in_tile[i]);

                            this.pps_exp_slice_height_in_ctus_minus1[i] = new uint[pps_num_exp_slices_in_tile[i]];
                            for (j = 0; j < pps_num_exp_slices_in_tile[i]; j++)
                            {
                                size += stream.ReadUnsignedIntGolomb(size, out this.pps_exp_slice_height_in_ctus_minus1[i][j]);
                            }
                            i += ((H266Context)context).NumSlicesInTile[i] - 1;
                        }

                        if (pps_tile_idx_delta_present_flag != 0 && i < pps_num_slices_in_pic_minus1)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.pps_tile_idx_delta_val[i]);
                        }
                    }
                }

                if (pps_rect_slice_flag == 0 || pps_single_slice_per_subpic_flag != 0 ||
    pps_num_slices_in_pic_minus1 > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pps_loop_filter_across_slices_enabled_flag);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_cabac_init_present_flag);

            this.pps_num_ref_idx_default_active_minus1 = new uint[2];
            for (i = 0; i < 2; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_ref_idx_default_active_minus1[i]);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_rpl1_idx_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_weighted_pred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_weighted_bipred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_ref_wraparound_enabled_flag);

            if (pps_ref_wraparound_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_pic_width_minus_wraparound_offset);
            }
            size += stream.ReadSignedIntGolomb(size, out this.pps_init_qp_minus26);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_cu_qp_delta_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_chroma_tool_offsets_present_flag);

            if (pps_chroma_tool_offsets_present_flag != 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.pps_cb_qp_offset);
                size += stream.ReadSignedIntGolomb(size, out this.pps_cr_qp_offset);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_joint_cbcr_qp_offset_present_flag);

                if (pps_joint_cbcr_qp_offset_present_flag != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.pps_joint_cbcr_qp_offset_value);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.pps_slice_chroma_qp_offsets_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_cu_chroma_qp_offset_list_enabled_flag);

                if (pps_cu_chroma_qp_offset_list_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.pps_chroma_qp_offset_list_len_minus1);

                    this.pps_cb_qp_offset_list = new int[pps_chroma_qp_offset_list_len_minus1 + 1];
                    this.pps_cr_qp_offset_list = new int[pps_chroma_qp_offset_list_len_minus1 + 1];
                    this.pps_joint_cbcr_qp_offset_list = new int[pps_chroma_qp_offset_list_len_minus1 + 1];
                    for (i = 0; i <= pps_chroma_qp_offset_list_len_minus1; i++)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.pps_cb_qp_offset_list[i]);
                        size += stream.ReadSignedIntGolomb(size, out this.pps_cr_qp_offset_list[i]);

                        if (pps_joint_cbcr_qp_offset_present_flag != 0)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.pps_joint_cbcr_qp_offset_list[i]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_deblocking_filter_control_present_flag);

            if (pps_deblocking_filter_control_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pps_deblocking_filter_override_enabled_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_deblocking_filter_disabled_flag);

                if (pps_no_pic_partition_flag == 0 && pps_deblocking_filter_override_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pps_dbf_info_in_ph_flag);
                }

                if (pps_deblocking_filter_disabled_flag == 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.pps_luma_beta_offset_div2);
                    size += stream.ReadSignedIntGolomb(size, out this.pps_luma_tc_offset_div2);

                    if (pps_chroma_tool_offsets_present_flag != 0)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.pps_cb_beta_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.pps_cb_tc_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.pps_cr_beta_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.pps_cr_tc_offset_div2);
                    }
                }
            }

            if (pps_no_pic_partition_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pps_rpl_info_in_ph_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_sao_info_in_ph_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_alf_info_in_ph_flag);

                if ((pps_weighted_pred_flag != 0 || pps_weighted_bipred_flag != 0) &&
    pps_rpl_info_in_ph_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pps_wp_info_in_ph_flag);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.pps_qp_delta_info_in_ph_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_picture_header_extension_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_slice_header_extension_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_extension_flag);

            if (pps_extension_flag != 0)
            {

                while (stream.ReadMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.pps_extension_data_flag);
                }
            }
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(6, this.pps_pic_parameter_set_id);
            size += stream.WriteUnsignedInt(4, this.pps_seq_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.pps_mixed_nalu_types_in_pic_flag);
            size += stream.WriteUnsignedIntGolomb(this.pps_pic_width_in_luma_samples);
            size += stream.WriteUnsignedIntGolomb(this.pps_pic_height_in_luma_samples);
            ((H266Context)context).OnPpsPicHeightInLumaSamples();
            size += stream.WriteUnsignedInt(1, this.pps_conformance_window_flag);

            if (pps_conformance_window_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pps_conf_win_left_offset);
                size += stream.WriteUnsignedIntGolomb(this.pps_conf_win_right_offset);
                size += stream.WriteUnsignedIntGolomb(this.pps_conf_win_top_offset);
                size += stream.WriteUnsignedIntGolomb(this.pps_conf_win_bottom_offset);
            }
            size += stream.WriteUnsignedInt(1, this.pps_scaling_window_explicit_signalling_flag);

            if (pps_scaling_window_explicit_signalling_flag != 0)
            {
                size += stream.WriteSignedIntGolomb(this.pps_scaling_win_left_offset);
                size += stream.WriteSignedIntGolomb(this.pps_scaling_win_right_offset);
                size += stream.WriteSignedIntGolomb(this.pps_scaling_win_top_offset);
                size += stream.WriteSignedIntGolomb(this.pps_scaling_win_bottom_offset);
            }
            size += stream.WriteUnsignedInt(1, this.pps_output_flag_present_flag);
            size += stream.WriteUnsignedInt(1, this.pps_no_pic_partition_flag);
            size += stream.WriteUnsignedInt(1, this.pps_subpic_id_mapping_present_flag);

            if (pps_subpic_id_mapping_present_flag != 0)
            {

                if (pps_no_pic_partition_flag == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.pps_num_subpics_minus1);
                }
                size += stream.WriteUnsignedIntGolomb(this.pps_subpic_id_len_minus1);

                for (i = 0; i <= pps_num_subpics_minus1; i++)
                {
                    size += stream.WriteUnsignedIntVariable(pps_subpic_id_len_minus1 + 1, this.pps_subpic_id[i]);
                    ((H266Context)context).OnPpsSubpicId();
                }
            }

            if (pps_no_pic_partition_flag == 0)
            {
                size += stream.WriteUnsignedInt(2, this.pps_log2_ctu_size_minus5);
                size += stream.WriteUnsignedIntGolomb(this.pps_num_exp_tile_columns_minus1);
                size += stream.WriteUnsignedIntGolomb(this.pps_num_exp_tile_rows_minus1);

                for (i = 0; i <= pps_num_exp_tile_columns_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.pps_tile_column_width_minus1[i]);
                }

                for (i = 0; i <= pps_num_exp_tile_rows_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.pps_tile_row_height_minus1[i]);
                    ((H266Context)context).OnPpsTileRowHeightMinus1();
                }

                if (((H266Context)context).NumTilesInPic > 1)
                {
                    size += stream.WriteUnsignedInt(1, this.pps_loop_filter_across_tiles_enabled_flag);
                    size += stream.WriteUnsignedInt(1, this.pps_rect_slice_flag);
                }

                if (pps_rect_slice_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.pps_single_slice_per_subpic_flag);
                }

                if (pps_rect_slice_flag != 0 && pps_single_slice_per_subpic_flag == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.pps_num_slices_in_pic_minus1);

                    if (pps_num_slices_in_pic_minus1 > 1)
                    {
                        size += stream.WriteUnsignedInt(1, this.pps_tile_idx_delta_present_flag);
                    }

                    for (i = 0; i < pps_num_slices_in_pic_minus1; i++)
                    {

                        if (((H266Context)context).SliceTopLeftTileIdx[i] % ((H266Context)context).NumTileColumns != ((H266Context)context).NumTileColumns - 1)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.pps_slice_width_in_tiles_minus1[i]);
                        }

                        if (((H266Context)context).SliceTopLeftTileIdx[i] / ((H266Context)context).NumTileColumns != ((H266Context)context).NumTileRows - 1 &&
      (pps_tile_idx_delta_present_flag != 0 ||
      ((H266Context)context).SliceTopLeftTileIdx[i] % ((H266Context)context).NumTileColumns == 0))
                        {
                            size += stream.WriteUnsignedIntGolomb(this.pps_slice_height_in_tiles_minus1[i]);
                        }

                        if (pps_slice_width_in_tiles_minus1[i] == 0 &&
      pps_slice_height_in_tiles_minus1[i] == 0 &&
      ((H266Context)context).RowHeightVal[((H266Context)context).SliceTopLeftTileIdx[i] / ((H266Context)context).NumTileColumns] > 1)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.pps_num_exp_slices_in_tile[i]);

                            for (j = 0; j < pps_num_exp_slices_in_tile[i]; j++)
                            {
                                size += stream.WriteUnsignedIntGolomb(this.pps_exp_slice_height_in_ctus_minus1[i][j]);
                            }
                            i += ((H266Context)context).NumSlicesInTile[i] - 1;
                        }

                        if (pps_tile_idx_delta_present_flag != 0 && i < pps_num_slices_in_pic_minus1)
                        {
                            size += stream.WriteSignedIntGolomb(this.pps_tile_idx_delta_val[i]);
                        }
                    }
                }

                if (pps_rect_slice_flag == 0 || pps_single_slice_per_subpic_flag != 0 ||
    pps_num_slices_in_pic_minus1 > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.pps_loop_filter_across_slices_enabled_flag);
                }
            }
            size += stream.WriteUnsignedInt(1, this.pps_cabac_init_present_flag);

            for (i = 0; i < 2; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.pps_num_ref_idx_default_active_minus1[i]);
            }
            size += stream.WriteUnsignedInt(1, this.pps_rpl1_idx_present_flag);
            size += stream.WriteUnsignedInt(1, this.pps_weighted_pred_flag);
            size += stream.WriteUnsignedInt(1, this.pps_weighted_bipred_flag);
            size += stream.WriteUnsignedInt(1, this.pps_ref_wraparound_enabled_flag);

            if (pps_ref_wraparound_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pps_pic_width_minus_wraparound_offset);
            }
            size += stream.WriteSignedIntGolomb(this.pps_init_qp_minus26);
            size += stream.WriteUnsignedInt(1, this.pps_cu_qp_delta_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.pps_chroma_tool_offsets_present_flag);

            if (pps_chroma_tool_offsets_present_flag != 0)
            {
                size += stream.WriteSignedIntGolomb(this.pps_cb_qp_offset);
                size += stream.WriteSignedIntGolomb(this.pps_cr_qp_offset);
                size += stream.WriteUnsignedInt(1, this.pps_joint_cbcr_qp_offset_present_flag);

                if (pps_joint_cbcr_qp_offset_present_flag != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.pps_joint_cbcr_qp_offset_value);
                }
                size += stream.WriteUnsignedInt(1, this.pps_slice_chroma_qp_offsets_present_flag);
                size += stream.WriteUnsignedInt(1, this.pps_cu_chroma_qp_offset_list_enabled_flag);

                if (pps_cu_chroma_qp_offset_list_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.pps_chroma_qp_offset_list_len_minus1);

                    for (i = 0; i <= pps_chroma_qp_offset_list_len_minus1; i++)
                    {
                        size += stream.WriteSignedIntGolomb(this.pps_cb_qp_offset_list[i]);
                        size += stream.WriteSignedIntGolomb(this.pps_cr_qp_offset_list[i]);

                        if (pps_joint_cbcr_qp_offset_present_flag != 0)
                        {
                            size += stream.WriteSignedIntGolomb(this.pps_joint_cbcr_qp_offset_list[i]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.pps_deblocking_filter_control_present_flag);

            if (pps_deblocking_filter_control_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.pps_deblocking_filter_override_enabled_flag);
                size += stream.WriteUnsignedInt(1, this.pps_deblocking_filter_disabled_flag);

                if (pps_no_pic_partition_flag == 0 && pps_deblocking_filter_override_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.pps_dbf_info_in_ph_flag);
                }

                if (pps_deblocking_filter_disabled_flag == 0)
                {
                    size += stream.WriteSignedIntGolomb(this.pps_luma_beta_offset_div2);
                    size += stream.WriteSignedIntGolomb(this.pps_luma_tc_offset_div2);

                    if (pps_chroma_tool_offsets_present_flag != 0)
                    {
                        size += stream.WriteSignedIntGolomb(this.pps_cb_beta_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.pps_cb_tc_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.pps_cr_beta_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.pps_cr_tc_offset_div2);
                    }
                }
            }

            if (pps_no_pic_partition_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.pps_rpl_info_in_ph_flag);
                size += stream.WriteUnsignedInt(1, this.pps_sao_info_in_ph_flag);
                size += stream.WriteUnsignedInt(1, this.pps_alf_info_in_ph_flag);

                if ((pps_weighted_pred_flag != 0 || pps_weighted_bipred_flag != 0) &&
    pps_rpl_info_in_ph_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.pps_wp_info_in_ph_flag);
                }
                size += stream.WriteUnsignedInt(1, this.pps_qp_delta_info_in_ph_flag);
            }
            size += stream.WriteUnsignedInt(1, this.pps_picture_header_extension_present_flag);
            size += stream.WriteUnsignedInt(1, this.pps_slice_header_extension_present_flag);
            size += stream.WriteUnsignedInt(1, this.pps_extension_flag);

            if (pps_extension_flag != 0)
            {

                while (stream.WriteMoreRbspData(this))
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.pps_extension_data_flag);
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

adaptation_parameter_set_rbsp() {  
 aps_params_type u(3) 
 aps_adaptation_parameter_set_id u(5) 
 aps_chroma_present_flag u(1) 
 if( aps_params_type  ==  ALF_APS )  
  alf_data()  
 else if( aps_params_type  ==  LMCS_APS )  
  lmcs_data()  
 /* else if( aps_params_type  ==  SCALING_APS ) *//*
 /* scaling_list_data()  *//*
 /*aps_extension_flag u(1) *//*
 /*if( aps_extension_flag )  *//*
  /*while( more_rbsp_data() )  *//*
  /* aps_extension_data_flag u(1) *//*
 /*rbsp_trailing_bits()  *//*
}
    */
    public class AdaptationParameterSetRbsp : IItuSerializable
    {
        private uint aps_params_type;
        public uint ApsParamsType { get { return aps_params_type; } set { aps_params_type = value; } }
        private uint aps_adaptation_parameter_set_id;
        public uint ApsAdaptationParameterSetId { get { return aps_adaptation_parameter_set_id; } set { aps_adaptation_parameter_set_id = value; } }
        private byte aps_chroma_present_flag;
        public byte ApsChromaPresentFlag { get { return aps_chroma_present_flag; } set { aps_chroma_present_flag = value; } }
        private AlfData alf_data;
        public AlfData AlfData { get { return alf_data; } set { alf_data = value; } }
        private LmcsData lmcs_data;
        public LmcsData LmcsData { get { return lmcs_data; } set { lmcs_data = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AdaptationParameterSetRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 3, out this.aps_params_type);
            size += stream.ReadUnsignedInt(size, 5, out this.aps_adaptation_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.aps_chroma_present_flag);

            if (aps_params_type == H266Constants.ALF_APS)
            {
                this.alf_data = new AlfData();
                size += stream.ReadClass<AlfData>(size, context, this.alf_data);
            }
            else if (aps_params_type == H266Constants.LMCS_APS)
            {
                this.lmcs_data = new LmcsData();
                size += stream.ReadClass<LmcsData>(size, context, this.lmcs_data); // else if( aps_params_type  ==  SCALING_APS ) 
            }
            /*  scaling_list_data()   */

            /* aps_extension_flag u(1)  */

            /* if( aps_extension_flag )   */

            /* while( more_rbsp_data() )   */

            /*  aps_extension_data_flag u(1)  */

            /* rbsp_trailing_bits()   */


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(3, this.aps_params_type);
            size += stream.WriteUnsignedInt(5, this.aps_adaptation_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.aps_chroma_present_flag);

            if (aps_params_type == H266Constants.ALF_APS)
            {
                size += stream.WriteClass<AlfData>(context, this.alf_data);
            }
            else if (aps_params_type == H266Constants.LMCS_APS)
            {
                size += stream.WriteClass<LmcsData>(context, this.lmcs_data); // else if( aps_params_type  ==  SCALING_APS ) 
            }
            /*  scaling_list_data()   */

            /* aps_extension_flag u(1)  */

            /* if( aps_extension_flag )   */

            /* while( more_rbsp_data() )   */

            /*  aps_extension_data_flag u(1)  */

            /* rbsp_trailing_bits()   */


            return size;
        }

    }

    /*
 

slice_layer_rbsp() {
    slice_header()
    /* slice_data() *//*
    /* rbsp_slice_trailing_bits() *//*
}
    */
    public class SliceLayerRbsp : IItuSerializable
    {
        private SliceHeader slice_header;
        public SliceHeader SliceHeader { get { return slice_header; } set { slice_header = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SliceLayerRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            this.slice_header = new SliceHeader();
            size += stream.ReadClass<SliceHeader>(size, context, this.slice_header); // slice_data() 
            /*  rbsp_slice_trailing_bits()  */


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<SliceHeader>(context, this.slice_header); // slice_data() 
            /*  rbsp_slice_trailing_bits()  */


            return size;
        }

    }

    /*
  

slice_header() {  
    sh_picture_header_in_slice_header_flag u(1)
    if (sh_picture_header_in_slice_header_flag)
        picture_header_structure()   
 
    if (sps_subpic_info_present_flag)
        sh_subpic_id u(v)
    if ((pps_rect_slice_flag && NumSlicesInSubpic[CurrSubpicIdx] > 1) ||
        (!pps_rect_slice_flag && NumTilesInPic > 1)) 
 
    sh_slice_address u(v)
    for (i = 0; i < NumExtraShBits; i++)
        sh_extra_bit[i] u(1)
    if (!pps_rect_slice_flag && NumTilesInPic - sh_slice_address > 1 )  
        sh_num_tiles_in_slice_minus1 ue(v)
    if (ph_inter_slice_allowed_flag)  
        sh_slice_type ue(v)
    if (nal_unit_type == IDR_W_RADL || nal_unit_type == IDR_N_LP ||
        nal_unit_type == CRA_NUT || nal_unit_type == GDR_NUT) 
        sh_no_output_of_prior_pics_flag u(1)
    if (sps_alf_enabled_flag && !pps_alf_info_in_ph_flag) {  
        sh_alf_enabled_flag u(1)
        if (sh_alf_enabled_flag) {  
            sh_num_alf_aps_ids_luma u(3)
            for (i = 0; i < sh_num_alf_aps_ids_luma; i++)
                sh_alf_aps_id_luma[i] u(3)
            if (sps_chroma_format_idc != 0) {  
                sh_alf_cb_enabled_flag u(1) 
                sh_alf_cr_enabled_flag u(1)
            }
            if (sh_alf_cb_enabled_flag || sh_alf_cr_enabled_flag)  
                sh_alf_aps_id_chroma u(3)
            if (sps_ccalf_enabled_flag) {  
                sh_alf_cc_cb_enabled_flag u(1)
                if (sh_alf_cc_cb_enabled_flag)  
                    sh_alf_cc_cb_aps_id u(3) 
                sh_alf_cc_cr_enabled_flag u(1)
                if (sh_alf_cc_cr_enabled_flag)  
                    sh_alf_cc_cr_aps_id u(3)
            }
        }
    }
    if (ph_lmcs_enabled_flag && !sh_picture_header_in_slice_header_flag)  
        sh_lmcs_used_flag u(1)
    if (ph_explicit_scaling_list_enabled_flag && !sh_picture_header_in_slice_header_flag)  
        sh_explicit_scaling_list_used_flag u(1)
    if (!pps_rpl_info_in_ph_flag && ((nal_unit_type != IDR_W_RADL &&
        nal_unit_type != IDR_N_LP) || sps_idr_rpl_present_flag))

        ref_pic_lists()
    if ((sh_slice_type != I && num_ref_entries[0][RplsIdx[0]] > 1) ||
        (sh_slice_type == B && num_ref_entries[1][RplsIdx[1]] > 1)) { 
 
        sh_num_ref_idx_active_override_flag u(1)
        if (sh_num_ref_idx_active_override_flag)
            for (i = 0; i < (sh_slice_type == B ? 2 : 1); i++)
                if (num_ref_entries[i][RplsIdx[i]] > 1)
                    sh_num_ref_idx_active_minus1[i] ue(v)
    }
    if (sh_slice_type != I) {
        if (pps_cabac_init_present_flag)  
            sh_cabac_init_flag u(1)
        if (ph_temporal_mvp_enabled_flag && !pps_rpl_info_in_ph_flag) {
            if (sh_slice_type == B)  
                sh_collocated_from_l0_flag u(1)
            if ((sh_collocated_from_l0_flag && NumRefIdxActive[0] > 1) ||
                (!sh_collocated_from_l0_flag && NumRefIdxActive[1] > 1)) 
 
            sh_collocated_ref_idx ue(v)
        }
        if (!pps_wp_info_in_ph_flag &&
            ((pps_weighted_pred_flag && sh_slice_type == P) ||
                (pps_weighted_bipred_flag && sh_slice_type == B)))
            pred_weight_table()
    }
    if (!pps_qp_delta_info_in_ph_flag)  
        sh_qp_delta se(v)
    if (pps_slice_chroma_qp_offsets_present_flag) {  
        sh_cb_qp_offset se(v) 
        sh_cr_qp_offset se(v)
        if (sps_joint_cbcr_enabled_flag)  
            sh_joint_cbcr_qp_offset se(v)
    }
    if (pps_cu_chroma_qp_offset_list_enabled_flag)  
        sh_cu_chroma_qp_offset_enabled_flag u(1)
    if (sps_sao_enabled_flag && !pps_sao_info_in_ph_flag) {  
        sh_sao_luma_used_flag u(1)
        if (sps_chroma_format_idc != 0)  
            sh_sao_chroma_used_flag u(1)
    }
    if (pps_deblocking_filter_override_enabled_flag && !pps_dbf_info_in_ph_flag)  
        sh_deblocking_params_present_flag u(1)
    if (sh_deblocking_params_present_flag) {
        if (!pps_deblocking_filter_disabled_flag)  
        sh_deblocking_filter_disabled_flag u(1)
        if (!sh_deblocking_filter_disabled_flag) {  
            sh_luma_beta_offset_div2 se(v) 
            sh_luma_tc_offset_div2 se(v)
            if (pps_chroma_tool_offsets_present_flag) {  
                sh_cb_beta_offset_div2 se(v) 
                sh_cb_tc_offset_div2 se(v) 
                sh_cr_beta_offset_div2 se(v) 
                sh_cr_tc_offset_div2 se(v)
            }
        }
    }
    if (sps_dep_quant_enabled_flag)  
        sh_dep_quant_used_flag u(1)
    if (sps_sign_data_hiding_enabled_flag && !sh_dep_quant_used_flag)  
        sh_sign_data_hiding_used_flag u(1)
    if (sps_transform_skip_enabled_flag && !sh_dep_quant_used_flag &&
        !sh_sign_data_hiding_used_flag) 
 
        sh_ts_residual_coding_disabled_flag u(1)
    if (pps_slice_header_extension_present_flag) {  
        sh_slice_header_extension_length ue(v)
        for (i = 0; i < sh_slice_header_extension_length; i++)
            sh_slice_header_extension_data_byte[i] u(8)
    }
    if (NumEntryPoints > 0) {  
        sh_entry_offset_len_minus1 ue(v)
        for (i = 0; i < NumEntryPoints; i++)
            sh_entry_point_offset_minus1[i] u(v)
    }
    byte_alignment()
}
    */
    public class SliceHeader : IItuSerializable
    {
        private byte sh_picture_header_in_slice_header_flag;
        public byte ShPictureHeaderInSliceHeaderFlag { get { return sh_picture_header_in_slice_header_flag; } set { sh_picture_header_in_slice_header_flag = value; } }
        private PictureHeaderStructure picture_header_structure;
        public PictureHeaderStructure PictureHeaderStructure { get { return picture_header_structure; } set { picture_header_structure = value; } }
        private uint sh_subpic_id;
        public uint ShSubpicId { get { return sh_subpic_id; } set { sh_subpic_id = value; } }
        private uint sh_slice_address;
        public uint ShSliceAddress { get { return sh_slice_address; } set { sh_slice_address = value; } }
        private byte[] sh_extra_bit;
        public byte[] ShExtraBit { get { return sh_extra_bit; } set { sh_extra_bit = value; } }
        private uint sh_num_tiles_in_slice_minus1;
        public uint ShNumTilesInSliceMinus1 { get { return sh_num_tiles_in_slice_minus1; } set { sh_num_tiles_in_slice_minus1 = value; } }
        private uint sh_slice_type;
        public uint ShSliceType { get { return sh_slice_type; } set { sh_slice_type = value; } }
        private byte sh_no_output_of_prior_pics_flag;
        public byte ShNoOutputOfPriorPicsFlag { get { return sh_no_output_of_prior_pics_flag; } set { sh_no_output_of_prior_pics_flag = value; } }
        private byte sh_alf_enabled_flag;
        public byte ShAlfEnabledFlag { get { return sh_alf_enabled_flag; } set { sh_alf_enabled_flag = value; } }
        private uint sh_num_alf_aps_ids_luma;
        public uint ShNumAlfApsIdsLuma { get { return sh_num_alf_aps_ids_luma; } set { sh_num_alf_aps_ids_luma = value; } }
        private uint[] sh_alf_aps_id_luma;
        public uint[] ShAlfApsIdLuma { get { return sh_alf_aps_id_luma; } set { sh_alf_aps_id_luma = value; } }
        private byte sh_alf_cb_enabled_flag;
        public byte ShAlfCbEnabledFlag { get { return sh_alf_cb_enabled_flag; } set { sh_alf_cb_enabled_flag = value; } }
        private byte sh_alf_cr_enabled_flag;
        public byte ShAlfCrEnabledFlag { get { return sh_alf_cr_enabled_flag; } set { sh_alf_cr_enabled_flag = value; } }
        private uint sh_alf_aps_id_chroma;
        public uint ShAlfApsIdChroma { get { return sh_alf_aps_id_chroma; } set { sh_alf_aps_id_chroma = value; } }
        private byte sh_alf_cc_cb_enabled_flag;
        public byte ShAlfCcCbEnabledFlag { get { return sh_alf_cc_cb_enabled_flag; } set { sh_alf_cc_cb_enabled_flag = value; } }
        private uint sh_alf_cc_cb_aps_id;
        public uint ShAlfCcCbApsId { get { return sh_alf_cc_cb_aps_id; } set { sh_alf_cc_cb_aps_id = value; } }
        private byte sh_alf_cc_cr_enabled_flag;
        public byte ShAlfCcCrEnabledFlag { get { return sh_alf_cc_cr_enabled_flag; } set { sh_alf_cc_cr_enabled_flag = value; } }
        private uint sh_alf_cc_cr_aps_id;
        public uint ShAlfCcCrApsId { get { return sh_alf_cc_cr_aps_id; } set { sh_alf_cc_cr_aps_id = value; } }
        private byte sh_lmcs_used_flag;
        public byte ShLmcsUsedFlag { get { return sh_lmcs_used_flag; } set { sh_lmcs_used_flag = value; } }
        private byte sh_explicit_scaling_list_used_flag;
        public byte ShExplicitScalingListUsedFlag { get { return sh_explicit_scaling_list_used_flag; } set { sh_explicit_scaling_list_used_flag = value; } }
        private RefPicLists ref_pic_lists;
        public RefPicLists RefPicLists { get { return ref_pic_lists; } set { ref_pic_lists = value; } }
        private byte sh_num_ref_idx_active_override_flag;
        public byte ShNumRefIdxActiveOverrideFlag { get { return sh_num_ref_idx_active_override_flag; } set { sh_num_ref_idx_active_override_flag = value; } }
        private uint[] sh_num_ref_idx_active_minus1;
        public uint[] ShNumRefIdxActiveMinus1 { get { return sh_num_ref_idx_active_minus1; } set { sh_num_ref_idx_active_minus1 = value; } }
        private byte sh_cabac_init_flag;
        public byte ShCabacInitFlag { get { return sh_cabac_init_flag; } set { sh_cabac_init_flag = value; } }
        private byte sh_collocated_from_l0_flag;
        public byte ShCollocatedFromL0Flag { get { return sh_collocated_from_l0_flag; } set { sh_collocated_from_l0_flag = value; } }
        private uint sh_collocated_ref_idx;
        public uint ShCollocatedRefIdx { get { return sh_collocated_ref_idx; } set { sh_collocated_ref_idx = value; } }
        private PredWeightTable pred_weight_table;
        public PredWeightTable PredWeightTable { get { return pred_weight_table; } set { pred_weight_table = value; } }
        private int sh_qp_delta;
        public int ShQpDelta { get { return sh_qp_delta; } set { sh_qp_delta = value; } }
        private int sh_cb_qp_offset;
        public int ShCbQpOffset { get { return sh_cb_qp_offset; } set { sh_cb_qp_offset = value; } }
        private int sh_cr_qp_offset;
        public int ShCrQpOffset { get { return sh_cr_qp_offset; } set { sh_cr_qp_offset = value; } }
        private int sh_joint_cbcr_qp_offset;
        public int ShJointCbcrQpOffset { get { return sh_joint_cbcr_qp_offset; } set { sh_joint_cbcr_qp_offset = value; } }
        private byte sh_cu_chroma_qp_offset_enabled_flag;
        public byte ShCuChromaQpOffsetEnabledFlag { get { return sh_cu_chroma_qp_offset_enabled_flag; } set { sh_cu_chroma_qp_offset_enabled_flag = value; } }
        private byte sh_sao_luma_used_flag;
        public byte ShSaoLumaUsedFlag { get { return sh_sao_luma_used_flag; } set { sh_sao_luma_used_flag = value; } }
        private byte sh_sao_chroma_used_flag;
        public byte ShSaoChromaUsedFlag { get { return sh_sao_chroma_used_flag; } set { sh_sao_chroma_used_flag = value; } }
        private byte sh_deblocking_params_present_flag;
        public byte ShDeblockingParamsPresentFlag { get { return sh_deblocking_params_present_flag; } set { sh_deblocking_params_present_flag = value; } }
        private byte sh_deblocking_filter_disabled_flag;
        public byte ShDeblockingFilterDisabledFlag { get { return sh_deblocking_filter_disabled_flag; } set { sh_deblocking_filter_disabled_flag = value; } }
        private int sh_luma_beta_offset_div2;
        public int ShLumaBetaOffsetDiv2 { get { return sh_luma_beta_offset_div2; } set { sh_luma_beta_offset_div2 = value; } }
        private int sh_luma_tc_offset_div2;
        public int ShLumaTcOffsetDiv2 { get { return sh_luma_tc_offset_div2; } set { sh_luma_tc_offset_div2 = value; } }
        private int sh_cb_beta_offset_div2;
        public int ShCbBetaOffsetDiv2 { get { return sh_cb_beta_offset_div2; } set { sh_cb_beta_offset_div2 = value; } }
        private int sh_cb_tc_offset_div2;
        public int ShCbTcOffsetDiv2 { get { return sh_cb_tc_offset_div2; } set { sh_cb_tc_offset_div2 = value; } }
        private int sh_cr_beta_offset_div2;
        public int ShCrBetaOffsetDiv2 { get { return sh_cr_beta_offset_div2; } set { sh_cr_beta_offset_div2 = value; } }
        private int sh_cr_tc_offset_div2;
        public int ShCrTcOffsetDiv2 { get { return sh_cr_tc_offset_div2; } set { sh_cr_tc_offset_div2 = value; } }
        private byte sh_dep_quant_used_flag;
        public byte ShDepQuantUsedFlag { get { return sh_dep_quant_used_flag; } set { sh_dep_quant_used_flag = value; } }
        private byte sh_sign_data_hiding_used_flag;
        public byte ShSignDataHidingUsedFlag { get { return sh_sign_data_hiding_used_flag; } set { sh_sign_data_hiding_used_flag = value; } }
        private byte sh_ts_residual_coding_disabled_flag;
        public byte ShTsResidualCodingDisabledFlag { get { return sh_ts_residual_coding_disabled_flag; } set { sh_ts_residual_coding_disabled_flag = value; } }
        private uint sh_slice_header_extension_length;
        public uint ShSliceHeaderExtensionLength { get { return sh_slice_header_extension_length; } set { sh_slice_header_extension_length = value; } }
        private uint[] sh_slice_header_extension_data_byte;
        public uint[] ShSliceHeaderExtensionDataByte { get { return sh_slice_header_extension_data_byte; } set { sh_slice_header_extension_data_byte = value; } }
        private uint sh_entry_offset_len_minus1;
        public uint ShEntryOffsetLenMinus1 { get { return sh_entry_offset_len_minus1; } set { sh_entry_offset_len_minus1 = value; } }
        private uint[] sh_entry_point_offset_minus1;
        public uint[] ShEntryPointOffsetMinus1 { get { return sh_entry_point_offset_minus1; } set { sh_entry_point_offset_minus1 = value; } }
        private ByteAlignment byte_alignment;
        public ByteAlignment ByteAlignment { get { return byte_alignment; } set { byte_alignment = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SliceHeader()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.sh_picture_header_in_slice_header_flag);

            if (sh_picture_header_in_slice_header_flag != 0)
            {
                this.picture_header_structure = new PictureHeaderStructure();
                size += stream.ReadClass<PictureHeaderStructure>(size, context, this.picture_header_structure);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSubpicInfoPresentFlag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeqParameterSetRbsp.SpsSubpicIdLenMinus1 + 1, out this.sh_subpic_id);
                ((H266Context)context).OnShSubpicId(sh_subpic_id);
            }

            if ((((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag != 0 && ((H266Context)context).NumSlicesInSubpic[((H266Context)context).CurrSubpicIdx] > 1) ||
        (((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 && ((H266Context)context).NumTilesInPic > 1))
            {
                size += stream.ReadUnsignedIntVariable(size, (uint)(((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 ? Math.Ceiling(Math.Log2(((H266Context)context).NumTilesInPic)) : Math.Ceiling(Math.Log2(((H266Context)context).NumSlicesInSubpic[((H266Context)context).CurrSubpicIdx]))), out this.sh_slice_address);
            }

            this.sh_extra_bit = new byte[((H266Context)context).NumExtraShBits];
            for (i = 0; i < ((H266Context)context).NumExtraShBits; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_extra_bit[i]);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 && ((H266Context)context).NumTilesInPic - sh_slice_address > 1)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sh_num_tiles_in_slice_minus1);
                ((H266Context)context).OnShNumTilesInSliceMinus1();
            }

            if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhInterSliceAllowedFlag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sh_slice_type);
            }

            if (((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.IDR_W_RADL || ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.IDR_N_LP ||
        ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.CRA_NUT || ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.GDR_NUT)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_no_output_of_prior_pics_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsAlfEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsAlfInfoInPhFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_alf_enabled_flag);

                if (sh_alf_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.sh_num_alf_aps_ids_luma);

                    this.sh_alf_aps_id_luma = new uint[sh_num_alf_aps_ids_luma];
                    for (i = 0; i < sh_num_alf_aps_ids_luma; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.sh_alf_aps_id_luma[i]);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sh_alf_cb_enabled_flag);
                        size += stream.ReadUnsignedInt(size, 1, out this.sh_alf_cr_enabled_flag);
                    }

                    if (sh_alf_cb_enabled_flag != 0 || sh_alf_cr_enabled_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.sh_alf_aps_id_chroma);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsCcalfEnabledFlag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sh_alf_cc_cb_enabled_flag);

                        if (sh_alf_cc_cb_enabled_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.sh_alf_cc_cb_aps_id);
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.sh_alf_cc_cr_enabled_flag);

                        if (sh_alf_cc_cr_enabled_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.sh_alf_cc_cr_aps_id);
                        }
                    }
                }
            }

            if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhLmcsEnabledFlag != 0 && sh_picture_header_in_slice_header_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_lmcs_used_flag);
            }

            if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhExplicitScalingListEnabledFlag != 0 && sh_picture_header_in_slice_header_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_explicit_scaling_list_used_flag);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag == 0 && ((((H266Context)context).NalHeader.NalUnitHeader.NalUnitType != H266NALTypes.IDR_W_RADL &&
        ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType != H266NALTypes.IDR_N_LP) || ((H266Context)context).SeqParameterSetRbsp.SpsIdrRplPresentFlag != 0))
            {
                this.ref_pic_lists = new RefPicLists();
                size += stream.ReadClass<RefPicLists>(size, context, this.ref_pic_lists);
            }

            if ((sh_slice_type != H266FrameTypes.I && ((H266Context)context).num_ref_entries[0][((H266Context)context).RplsIdx[0]] > 1) ||
        (sh_slice_type == H266FrameTypes.B && ((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 1))
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_num_ref_idx_active_override_flag);

                if (sh_num_ref_idx_active_override_flag != 0)
                {

                    this.sh_num_ref_idx_active_minus1 = new uint[(sh_slice_type == H266FrameTypes.B ? 2 : 1)];
                    for (i = 0; i < (sh_slice_type == H266FrameTypes.B ? 2 : 1); i++)
                    {

                        if (((H266Context)context).num_ref_entries[i][((H266Context)context).RplsIdx[i]] > 1)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.sh_num_ref_idx_active_minus1[i]);
                            ((H266Context)context).OnShNumRefIdxActiveMinus1();
                        }
                    }
                }
            }

            if (sh_slice_type != H266FrameTypes.I)
            {

                if (((H266Context)context).PicParameterSetRbsp.PpsCabacInitPresentFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sh_cabac_init_flag);
                }

                if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhTemporalMvpEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag == 0)
                {

                    if (sh_slice_type == H266FrameTypes.B)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sh_collocated_from_l0_flag);
                    }

                    if ((sh_collocated_from_l0_flag != 0 && ((H266Context)context).NumRefIdxActive[0] > 1) ||
                (sh_collocated_from_l0_flag == 0 && ((H266Context)context).NumRefIdxActive[1] > 1))
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.sh_collocated_ref_idx);
                    }
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag == 0 &&
            ((((H266Context)context).PicParameterSetRbsp.PpsWeightedPredFlag != 0 && sh_slice_type == H266FrameTypes.P) ||
                (((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag != 0 && sh_slice_type == H266FrameTypes.B)))
                {
                    this.pred_weight_table = new PredWeightTable();
                    size += stream.ReadClass<PredWeightTable>(size, context, this.pred_weight_table);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsQpDeltaInfoInPhFlag == 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.sh_qp_delta);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsSliceChromaQpOffsetsPresentFlag != 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.sh_cb_qp_offset);
                size += stream.ReadSignedIntGolomb(size, out this.sh_cr_qp_offset);

                if (((H266Context)context).SeqParameterSetRbsp.SpsJointCbcrEnabledFlag != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.sh_joint_cbcr_qp_offset);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_cu_chroma_qp_offset_enabled_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSaoEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsSaoInfoInPhFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_sao_luma_used_flag);

                if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sh_sao_chroma_used_flag);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterOverrideEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsDbfInfoInPhFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_deblocking_params_present_flag);
            }

            if (sh_deblocking_params_present_flag != 0)
            {

                if (((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterDisabledFlag == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sh_deblocking_filter_disabled_flag);
                }

                if (sh_deblocking_filter_disabled_flag == 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.sh_luma_beta_offset_div2);
                    size += stream.ReadSignedIntGolomb(size, out this.sh_luma_tc_offset_div2);

                    if (((H266Context)context).PicParameterSetRbsp.PpsChromaToolOffsetsPresentFlag != 0)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.sh_cb_beta_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.sh_cb_tc_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.sh_cr_beta_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.sh_cr_tc_offset_div2);
                    }
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsDepQuantEnabledFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_dep_quant_used_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSignDataHidingEnabledFlag != 0 && sh_dep_quant_used_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_sign_data_hiding_used_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsTransformSkipEnabledFlag != 0 && sh_dep_quant_used_flag == 0 &&
        sh_sign_data_hiding_used_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sh_ts_residual_coding_disabled_flag);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsSliceHeaderExtensionPresentFlag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sh_slice_header_extension_length);

                this.sh_slice_header_extension_data_byte = new uint[sh_slice_header_extension_length];
                for (i = 0; i < sh_slice_header_extension_length; i++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.sh_slice_header_extension_data_byte[i]);
                    ((H266Context)context).OnShSliceHeaderExtensionDataByte();
                }
            }

            if (((H266Context)context).NumEntryPoints > 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sh_entry_offset_len_minus1);

                this.sh_entry_point_offset_minus1 = new uint[((H266Context)context).NumEntryPoints];
                for (i = 0; i < ((H266Context)context).NumEntryPoints; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, sh_entry_offset_len_minus1 + 1, out this.sh_entry_point_offset_minus1[i]);
                }
            }
            this.byte_alignment = new ByteAlignment();
            size += stream.ReadClass<ByteAlignment>(size, context, this.byte_alignment);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.sh_picture_header_in_slice_header_flag);

            if (sh_picture_header_in_slice_header_flag != 0)
            {
                size += stream.WriteClass<PictureHeaderStructure>(context, this.picture_header_structure);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSubpicInfoPresentFlag != 0)
            {
                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeqParameterSetRbsp.SpsSubpicIdLenMinus1 + 1, this.sh_subpic_id);
                ((H266Context)context).OnShSubpicId(sh_subpic_id);
            }

            if ((((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag != 0 && ((H266Context)context).NumSlicesInSubpic[((H266Context)context).CurrSubpicIdx] > 1) ||
        (((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 && ((H266Context)context).NumTilesInPic > 1))
            {
                size += stream.WriteUnsignedIntVariable((uint)(((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 ? Math.Ceiling(Math.Log2(((H266Context)context).NumTilesInPic)) : Math.Ceiling(Math.Log2(((H266Context)context).NumSlicesInSubpic[((H266Context)context).CurrSubpicIdx]))), this.sh_slice_address);
            }

            for (i = 0; i < ((H266Context)context).NumExtraShBits; i++)
            {
                size += stream.WriteUnsignedInt(1, this.sh_extra_bit[i]);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsRectSliceFlag == 0 && ((H266Context)context).NumTilesInPic - sh_slice_address > 1)
            {
                size += stream.WriteUnsignedIntGolomb(this.sh_num_tiles_in_slice_minus1);
                ((H266Context)context).OnShNumTilesInSliceMinus1();
            }

            if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhInterSliceAllowedFlag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sh_slice_type);
            }

            if (((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.IDR_W_RADL || ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.IDR_N_LP ||
        ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.CRA_NUT || ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.GDR_NUT)
            {
                size += stream.WriteUnsignedInt(1, this.sh_no_output_of_prior_pics_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsAlfEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsAlfInfoInPhFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_alf_enabled_flag);

                if (sh_alf_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.sh_num_alf_aps_ids_luma);

                    for (i = 0; i < sh_num_alf_aps_ids_luma; i++)
                    {
                        size += stream.WriteUnsignedInt(3, this.sh_alf_aps_id_luma[i]);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sh_alf_cb_enabled_flag);
                        size += stream.WriteUnsignedInt(1, this.sh_alf_cr_enabled_flag);
                    }

                    if (sh_alf_cb_enabled_flag != 0 || sh_alf_cr_enabled_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(3, this.sh_alf_aps_id_chroma);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsCcalfEnabledFlag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sh_alf_cc_cb_enabled_flag);

                        if (sh_alf_cc_cb_enabled_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(3, this.sh_alf_cc_cb_aps_id);
                        }
                        size += stream.WriteUnsignedInt(1, this.sh_alf_cc_cr_enabled_flag);

                        if (sh_alf_cc_cr_enabled_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(3, this.sh_alf_cc_cr_aps_id);
                        }
                    }
                }
            }

            if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhLmcsEnabledFlag != 0 && sh_picture_header_in_slice_header_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_lmcs_used_flag);
            }

            if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhExplicitScalingListEnabledFlag != 0 && sh_picture_header_in_slice_header_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_explicit_scaling_list_used_flag);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag == 0 && ((((H266Context)context).NalHeader.NalUnitHeader.NalUnitType != H266NALTypes.IDR_W_RADL &&
        ((H266Context)context).NalHeader.NalUnitHeader.NalUnitType != H266NALTypes.IDR_N_LP) || ((H266Context)context).SeqParameterSetRbsp.SpsIdrRplPresentFlag != 0))
            {
                size += stream.WriteClass<RefPicLists>(context, this.ref_pic_lists);
            }

            if ((sh_slice_type != H266FrameTypes.I && ((H266Context)context).num_ref_entries[0][((H266Context)context).RplsIdx[0]] > 1) ||
        (sh_slice_type == H266FrameTypes.B && ((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 1))
            {
                size += stream.WriteUnsignedInt(1, this.sh_num_ref_idx_active_override_flag);

                if (sh_num_ref_idx_active_override_flag != 0)
                {

                    for (i = 0; i < (sh_slice_type == H266FrameTypes.B ? 2 : 1); i++)
                    {

                        if (((H266Context)context).num_ref_entries[i][((H266Context)context).RplsIdx[i]] > 1)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.sh_num_ref_idx_active_minus1[i]);
                            ((H266Context)context).OnShNumRefIdxActiveMinus1();
                        }
                    }
                }
            }

            if (sh_slice_type != H266FrameTypes.I)
            {

                if (((H266Context)context).PicParameterSetRbsp.PpsCabacInitPresentFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sh_cabac_init_flag);
                }

                if (((H266Context)context).SliceLayerRbsp.SliceHeader.PictureHeaderStructure.PhTemporalMvpEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag == 0)
                {

                    if (sh_slice_type == H266FrameTypes.B)
                    {
                        size += stream.WriteUnsignedInt(1, this.sh_collocated_from_l0_flag);
                    }

                    if ((sh_collocated_from_l0_flag != 0 && ((H266Context)context).NumRefIdxActive[0] > 1) ||
                (sh_collocated_from_l0_flag == 0 && ((H266Context)context).NumRefIdxActive[1] > 1))
                    {
                        size += stream.WriteUnsignedIntGolomb(this.sh_collocated_ref_idx);
                    }
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag == 0 &&
            ((((H266Context)context).PicParameterSetRbsp.PpsWeightedPredFlag != 0 && sh_slice_type == H266FrameTypes.P) ||
                (((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag != 0 && sh_slice_type == H266FrameTypes.B)))
                {
                    size += stream.WriteClass<PredWeightTable>(context, this.pred_weight_table);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsQpDeltaInfoInPhFlag == 0)
            {
                size += stream.WriteSignedIntGolomb(this.sh_qp_delta);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsSliceChromaQpOffsetsPresentFlag != 0)
            {
                size += stream.WriteSignedIntGolomb(this.sh_cb_qp_offset);
                size += stream.WriteSignedIntGolomb(this.sh_cr_qp_offset);

                if (((H266Context)context).SeqParameterSetRbsp.SpsJointCbcrEnabledFlag != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.sh_joint_cbcr_qp_offset);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_cu_chroma_qp_offset_enabled_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSaoEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsSaoInfoInPhFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_sao_luma_used_flag);

                if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sh_sao_chroma_used_flag);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterOverrideEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsDbfInfoInPhFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_deblocking_params_present_flag);
            }

            if (sh_deblocking_params_present_flag != 0)
            {

                if (((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterDisabledFlag == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sh_deblocking_filter_disabled_flag);
                }

                if (sh_deblocking_filter_disabled_flag == 0)
                {
                    size += stream.WriteSignedIntGolomb(this.sh_luma_beta_offset_div2);
                    size += stream.WriteSignedIntGolomb(this.sh_luma_tc_offset_div2);

                    if (((H266Context)context).PicParameterSetRbsp.PpsChromaToolOffsetsPresentFlag != 0)
                    {
                        size += stream.WriteSignedIntGolomb(this.sh_cb_beta_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.sh_cb_tc_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.sh_cr_beta_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.sh_cr_tc_offset_div2);
                    }
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsDepQuantEnabledFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_dep_quant_used_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSignDataHidingEnabledFlag != 0 && sh_dep_quant_used_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_sign_data_hiding_used_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsTransformSkipEnabledFlag != 0 && sh_dep_quant_used_flag == 0 &&
        sh_sign_data_hiding_used_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sh_ts_residual_coding_disabled_flag);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsSliceHeaderExtensionPresentFlag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sh_slice_header_extension_length);

                for (i = 0; i < sh_slice_header_extension_length; i++)
                {
                    size += stream.WriteUnsignedInt(8, this.sh_slice_header_extension_data_byte[i]);
                    ((H266Context)context).OnShSliceHeaderExtensionDataByte();
                }
            }

            if (((H266Context)context).NumEntryPoints > 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sh_entry_offset_len_minus1);

                for (i = 0; i < ((H266Context)context).NumEntryPoints; i++)
                {
                    size += stream.WriteUnsignedIntVariable(sh_entry_offset_len_minus1 + 1, this.sh_entry_point_offset_minus1[i]);
                }
            }
            size += stream.WriteClass<ByteAlignment>(context, this.byte_alignment);

            return size;
        }

    }

    /*


picture_header_rbsp() {  
 picture_header_structure()  
 rbsp_trailing_bits()  
}
    */
    public class PictureHeaderRbsp : IItuSerializable
    {
        private PictureHeaderStructure picture_header_structure;
        public PictureHeaderStructure PictureHeaderStructure { get { return picture_header_structure; } set { picture_header_structure = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PictureHeaderRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            this.picture_header_structure = new PictureHeaderStructure();
            size += stream.ReadClass<PictureHeaderStructure>(size, context, this.picture_header_structure);
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<PictureHeaderStructure>(context, this.picture_header_structure);
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

picture_header_structure() {  
 ph_gdr_or_irap_pic_flag u(1) 
 ph_non_ref_pic_flag u(1) 
 if( ph_gdr_or_irap_pic_flag )  
  ph_gdr_pic_flag u(1) 
 ph_inter_slice_allowed_flag u(1) 
 if( ph_inter_slice_allowed_flag )  
  ph_intra_slice_allowed_flag u(1) 
 ph_pic_parameter_set_id ue(v) 
 ph_pic_order_cnt_lsb u(v) 
 if( ph_gdr_pic_flag )  
  ph_recovery_poc_cnt ue(v) 
 for( i = 0; i < NumExtraPhBits; i++ )  
  ph_extra_bit[ i ] u(1) 
 if( sps_poc_msb_cycle_flag ) {  
   ph_poc_msb_cycle_present_flag u(1) 
  if( ph_poc_msb_cycle_present_flag )  
   ph_poc_msb_cycle_val u(v) 
    }  
 if( sps_alf_enabled_flag  &&  pps_alf_info_in_ph_flag ) {  
  ph_alf_enabled_flag u(1) 
  if( ph_alf_enabled_flag ) {  
   ph_num_alf_aps_ids_luma u(3) 
   for( i = 0; i < ph_num_alf_aps_ids_luma; i++ )  
    ph_alf_aps_id_luma[ i ] u(3) 
   if( sps_chroma_format_idc  !=  0 ) {  
    ph_alf_cb_enabled_flag u(1) 
    ph_alf_cr_enabled_flag u(1) 
   }  
   if( ph_alf_cb_enabled_flag  ||  ph_alf_cr_enabled_flag )  
    ph_alf_aps_id_chroma u(3) 
   if( sps_ccalf_enabled_flag ) {  
    ph_alf_cc_cb_enabled_flag u(1) 
    if( ph_alf_cc_cb_enabled_flag )  
     ph_alf_cc_cb_aps_id u(3) 
    ph_alf_cc_cr_enabled_flag u(1) 
    if( ph_alf_cc_cr_enabled_flag )  
     ph_alf_cc_cr_aps_id u(3) 
   }  
  }  
 }  
 if( sps_lmcs_enabled_flag ) {  
  ph_lmcs_enabled_flag u(1) 
  if( ph_lmcs_enabled_flag ) {  
   ph_lmcs_aps_id u(2) 
   if( sps_chroma_format_idc  !=  0 )  
    ph_chroma_residual_scale_flag u(1) 
  }  
 }  
 if( sps_explicit_scaling_list_enabled_flag ) {  
  ph_explicit_scaling_list_enabled_flag u(1) 
  if( ph_explicit_scaling_list_enabled_flag )  
   ph_scaling_list_aps_id u(3) 
 }  
 if( sps_virtual_boundaries_enabled_flag  &&  !sps_virtual_boundaries_present_flag ) {  
  ph_virtual_boundaries_present_flag u(1) 
  if( ph_virtual_boundaries_present_flag ) {  
   ph_num_ver_virtual_boundaries ue(v) 
   for( i = 0; i < ph_num_ver_virtual_boundaries; i++ )  
    ph_virtual_boundary_pos_x_minus1[ i ] ue(v) 
   ph_num_hor_virtual_boundaries ue(v) 
   for( i = 0; i < ph_num_hor_virtual_boundaries; i++ )  
    ph_virtual_boundary_pos_y_minus1[ i ] ue(v) 
      }  
 }  
 if( pps_output_flag_present_flag  &&  !ph_non_ref_pic_flag )  
  ph_pic_output_flag u(1) 
 if( pps_rpl_info_in_ph_flag )  
  ref_pic_lists()  
 if( sps_partition_constraints_override_enabled_flag )  
  ph_partition_constraints_override_flag u(1) 
 if( ph_intra_slice_allowed_flag ) {  
  if( ph_partition_constraints_override_flag ) {  
   ph_log2_diff_min_qt_min_cb_intra_slice_luma ue(v) 
   ph_max_mtt_hierarchy_depth_intra_slice_luma ue(v) 
   if( ph_max_mtt_hierarchy_depth_intra_slice_luma  !=  0 ) {  
    ph_log2_diff_max_bt_min_qt_intra_slice_luma ue(v) 
    ph_log2_diff_max_tt_min_qt_intra_slice_luma ue(v) 
   }  
   if( sps_qtbtt_dual_tree_intra_flag ) {   
    ph_log2_diff_min_qt_min_cb_intra_slice_chroma ue(v) 
    ph_max_mtt_hierarchy_depth_intra_slice_chroma ue(v) 
    if( ph_max_mtt_hierarchy_depth_intra_slice_chroma  !=  0 ) {  
     ph_log2_diff_max_bt_min_qt_intra_slice_chroma ue(v) 
     ph_log2_diff_max_tt_min_qt_intra_slice_chroma ue(v) 
    }  
   }  
  }  
  if( pps_cu_qp_delta_enabled_flag )  
   ph_cu_qp_delta_subdiv_intra_slice ue(v) 
  if( pps_cu_chroma_qp_offset_list_enabled_flag )  
   ph_cu_chroma_qp_offset_subdiv_intra_slice ue(v) 
 }  
 if( ph_inter_slice_allowed_flag ) {  
  if( ph_partition_constraints_override_flag ) {  
   ph_log2_diff_min_qt_min_cb_inter_slice ue(v) 
   ph_max_mtt_hierarchy_depth_inter_slice ue(v) 
   if( ph_max_mtt_hierarchy_depth_inter_slice  !=  0 ) {  
    ph_log2_diff_max_bt_min_qt_inter_slice ue(v) 
    ph_log2_diff_max_tt_min_qt_inter_slice ue(v) 
   }  
  }  
  if( pps_cu_qp_delta_enabled_flag )  
   ph_cu_qp_delta_subdiv_inter_slice ue(v) 
  if( pps_cu_chroma_qp_offset_list_enabled_flag )  
   ph_cu_chroma_qp_offset_subdiv_inter_slice ue(v) 
  if( sps_temporal_mvp_enabled_flag ) {  
   ph_temporal_mvp_enabled_flag u(1) 
   if( ph_temporal_mvp_enabled_flag  &&  pps_rpl_info_in_ph_flag ) {  
    if( num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 0 )  
     ph_collocated_from_l0_flag u(1) 
         if( ( ph_collocated_from_l0_flag  && 
      num_ref_entries[ 0 ][ RplsIdx[ 0 ] ] > 1 )  || 
      ( !ph_collocated_from_l0_flag  && 
      num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 1 ) ) 
 
     ph_collocated_ref_idx ue(v) 
   }  
  }  
  if( sps_mmvd_fullpel_only_enabled_flag )  
   ph_mmvd_fullpel_only_flag u(1) 
  presenceFlag = 0  
  if( !pps_rpl_info_in_ph_flag ) /* This condition is intentionally not merged into the next, 
    to avoid possible interpretation of RplsIdx[ i ] not having a specified value. *//* 
 
   presenceFlag = 1  
  else if( num_ref_entries[ 1 ][ RplsIdx[ 1 ] ] > 0 )  
   presenceFlag = 1  
  if( presenceFlag ) {  
   ph_mvd_l1_zero_flag u(1) 
   if( sps_bdof_control_present_in_ph_flag )  
    ph_bdof_disabled_flag u(1) 
   if( sps_dmvr_control_present_in_ph_flag )  
    ph_dmvr_disabled_flag u(1) 
  }  
  if( sps_prof_control_present_in_ph_flag )  
   ph_prof_disabled_flag u(1) 
  if( ( pps_weighted_pred_flag  ||  pps_weighted_bipred_flag )  && 
    pps_wp_info_in_ph_flag ) 
 
   pred_weight_table()  
 }  
 if( pps_qp_delta_info_in_ph_flag )  
  ph_qp_delta se(v) 
 if( sps_joint_cbcr_enabled_flag )  
  ph_joint_cbcr_sign_flag u(1) 
 if( sps_sao_enabled_flag  &&  pps_sao_info_in_ph_flag ) {  
  ph_sao_luma_enabled_flag u(1) 
  if( sps_chroma_format_idc  !=  0 )  
   ph_sao_chroma_enabled_flag u(1) 
 }  
 if( pps_dbf_info_in_ph_flag ) {  
  ph_deblocking_params_present_flag u(1) 
  if( ph_deblocking_params_present_flag ) {  
   if( !pps_deblocking_filter_disabled_flag )  
    ph_deblocking_filter_disabled_flag u(1) 
   if( !ph_deblocking_filter_disabled_flag ) {  
    ph_luma_beta_offset_div2 se(v) 
    ph_luma_tc_offset_div2 se(v) 
    if( pps_chroma_tool_offsets_present_flag ) {  
     ph_cb_beta_offset_div2 se(v) 
     ph_cb_tc_offset_div2 se(v) 
     ph_cr_beta_offset_div2 se(v) 

     ph_cr_tc_offset_div2 se(v)
    }  
   }  
  }  
}  
if( pps_picture_header_extension_present_flag ) {  
ph_extension_length ue(v) 
for( i = 0; i < ph_extension_length; i++)   
ph_extension_data_byte[ i ] u(8) 
}  
}
    */
    public class PictureHeaderStructure : IItuSerializable
    {
        private byte ph_gdr_or_irap_pic_flag;
        public byte PhGdrOrIrapPicFlag { get { return ph_gdr_or_irap_pic_flag; } set { ph_gdr_or_irap_pic_flag = value; } }
        private byte ph_non_ref_pic_flag;
        public byte PhNonRefPicFlag { get { return ph_non_ref_pic_flag; } set { ph_non_ref_pic_flag = value; } }
        private byte ph_gdr_pic_flag;
        public byte PhGdrPicFlag { get { return ph_gdr_pic_flag; } set { ph_gdr_pic_flag = value; } }
        private byte ph_inter_slice_allowed_flag;
        public byte PhInterSliceAllowedFlag { get { return ph_inter_slice_allowed_flag; } set { ph_inter_slice_allowed_flag = value; } }
        private byte ph_intra_slice_allowed_flag;
        public byte PhIntraSliceAllowedFlag { get { return ph_intra_slice_allowed_flag; } set { ph_intra_slice_allowed_flag = value; } }
        private uint ph_pic_parameter_set_id;
        public uint PhPicParameterSetId { get { return ph_pic_parameter_set_id; } set { ph_pic_parameter_set_id = value; } }
        private uint ph_pic_order_cnt_lsb;
        public uint PhPicOrderCntLsb { get { return ph_pic_order_cnt_lsb; } set { ph_pic_order_cnt_lsb = value; } }
        private uint ph_recovery_poc_cnt;
        public uint PhRecoveryPocCnt { get { return ph_recovery_poc_cnt; } set { ph_recovery_poc_cnt = value; } }
        private byte[] ph_extra_bit;
        public byte[] PhExtraBit { get { return ph_extra_bit; } set { ph_extra_bit = value; } }
        private byte ph_poc_msb_cycle_present_flag;
        public byte PhPocMsbCyclePresentFlag { get { return ph_poc_msb_cycle_present_flag; } set { ph_poc_msb_cycle_present_flag = value; } }
        private uint ph_poc_msb_cycle_val;
        public uint PhPocMsbCycleVal { get { return ph_poc_msb_cycle_val; } set { ph_poc_msb_cycle_val = value; } }
        private byte ph_alf_enabled_flag;
        public byte PhAlfEnabledFlag { get { return ph_alf_enabled_flag; } set { ph_alf_enabled_flag = value; } }
        private uint ph_num_alf_aps_ids_luma;
        public uint PhNumAlfApsIdsLuma { get { return ph_num_alf_aps_ids_luma; } set { ph_num_alf_aps_ids_luma = value; } }
        private uint[] ph_alf_aps_id_luma;
        public uint[] PhAlfApsIdLuma { get { return ph_alf_aps_id_luma; } set { ph_alf_aps_id_luma = value; } }
        private byte ph_alf_cb_enabled_flag;
        public byte PhAlfCbEnabledFlag { get { return ph_alf_cb_enabled_flag; } set { ph_alf_cb_enabled_flag = value; } }
        private byte ph_alf_cr_enabled_flag;
        public byte PhAlfCrEnabledFlag { get { return ph_alf_cr_enabled_flag; } set { ph_alf_cr_enabled_flag = value; } }
        private uint ph_alf_aps_id_chroma;
        public uint PhAlfApsIdChroma { get { return ph_alf_aps_id_chroma; } set { ph_alf_aps_id_chroma = value; } }
        private byte ph_alf_cc_cb_enabled_flag;
        public byte PhAlfCcCbEnabledFlag { get { return ph_alf_cc_cb_enabled_flag; } set { ph_alf_cc_cb_enabled_flag = value; } }
        private uint ph_alf_cc_cb_aps_id;
        public uint PhAlfCcCbApsId { get { return ph_alf_cc_cb_aps_id; } set { ph_alf_cc_cb_aps_id = value; } }
        private byte ph_alf_cc_cr_enabled_flag;
        public byte PhAlfCcCrEnabledFlag { get { return ph_alf_cc_cr_enabled_flag; } set { ph_alf_cc_cr_enabled_flag = value; } }
        private uint ph_alf_cc_cr_aps_id;
        public uint PhAlfCcCrApsId { get { return ph_alf_cc_cr_aps_id; } set { ph_alf_cc_cr_aps_id = value; } }
        private byte ph_lmcs_enabled_flag;
        public byte PhLmcsEnabledFlag { get { return ph_lmcs_enabled_flag; } set { ph_lmcs_enabled_flag = value; } }
        private uint ph_lmcs_aps_id;
        public uint PhLmcsApsId { get { return ph_lmcs_aps_id; } set { ph_lmcs_aps_id = value; } }
        private byte ph_chroma_residual_scale_flag;
        public byte PhChromaResidualScaleFlag { get { return ph_chroma_residual_scale_flag; } set { ph_chroma_residual_scale_flag = value; } }
        private byte ph_explicit_scaling_list_enabled_flag;
        public byte PhExplicitScalingListEnabledFlag { get { return ph_explicit_scaling_list_enabled_flag; } set { ph_explicit_scaling_list_enabled_flag = value; } }
        private uint ph_scaling_list_aps_id;
        public uint PhScalingListApsId { get { return ph_scaling_list_aps_id; } set { ph_scaling_list_aps_id = value; } }
        private byte ph_virtual_boundaries_present_flag;
        public byte PhVirtualBoundariesPresentFlag { get { return ph_virtual_boundaries_present_flag; } set { ph_virtual_boundaries_present_flag = value; } }
        private uint ph_num_ver_virtual_boundaries;
        public uint PhNumVerVirtualBoundaries { get { return ph_num_ver_virtual_boundaries; } set { ph_num_ver_virtual_boundaries = value; } }
        private uint[] ph_virtual_boundary_pos_x_minus1;
        public uint[] PhVirtualBoundaryPosxMinus1 { get { return ph_virtual_boundary_pos_x_minus1; } set { ph_virtual_boundary_pos_x_minus1 = value; } }
        private uint ph_num_hor_virtual_boundaries;
        public uint PhNumHorVirtualBoundaries { get { return ph_num_hor_virtual_boundaries; } set { ph_num_hor_virtual_boundaries = value; } }
        private uint[] ph_virtual_boundary_pos_y_minus1;
        public uint[] PhVirtualBoundaryPosyMinus1 { get { return ph_virtual_boundary_pos_y_minus1; } set { ph_virtual_boundary_pos_y_minus1 = value; } }
        private byte ph_pic_output_flag;
        public byte PhPicOutputFlag { get { return ph_pic_output_flag; } set { ph_pic_output_flag = value; } }
        private RefPicLists ref_pic_lists;
        public RefPicLists RefPicLists { get { return ref_pic_lists; } set { ref_pic_lists = value; } }
        private byte ph_partition_constraints_override_flag;
        public byte PhPartitionConstraintsOverrideFlag { get { return ph_partition_constraints_override_flag; } set { ph_partition_constraints_override_flag = value; } }
        private uint ph_log2_diff_min_qt_min_cb_intra_slice_luma;
        public uint PhLog2DiffMinQtMinCbIntraSliceLuma { get { return ph_log2_diff_min_qt_min_cb_intra_slice_luma; } set { ph_log2_diff_min_qt_min_cb_intra_slice_luma = value; } }
        private uint ph_max_mtt_hierarchy_depth_intra_slice_luma;
        public uint PhMaxMttHierarchyDepthIntraSliceLuma { get { return ph_max_mtt_hierarchy_depth_intra_slice_luma; } set { ph_max_mtt_hierarchy_depth_intra_slice_luma = value; } }
        private uint ph_log2_diff_max_bt_min_qt_intra_slice_luma;
        public uint PhLog2DiffMaxBtMinQtIntraSliceLuma { get { return ph_log2_diff_max_bt_min_qt_intra_slice_luma; } set { ph_log2_diff_max_bt_min_qt_intra_slice_luma = value; } }
        private uint ph_log2_diff_max_tt_min_qt_intra_slice_luma;
        public uint PhLog2DiffMaxTtMinQtIntraSliceLuma { get { return ph_log2_diff_max_tt_min_qt_intra_slice_luma; } set { ph_log2_diff_max_tt_min_qt_intra_slice_luma = value; } }
        private uint ph_log2_diff_min_qt_min_cb_intra_slice_chroma;
        public uint PhLog2DiffMinQtMinCbIntraSliceChroma { get { return ph_log2_diff_min_qt_min_cb_intra_slice_chroma; } set { ph_log2_diff_min_qt_min_cb_intra_slice_chroma = value; } }
        private uint ph_max_mtt_hierarchy_depth_intra_slice_chroma;
        public uint PhMaxMttHierarchyDepthIntraSliceChroma { get { return ph_max_mtt_hierarchy_depth_intra_slice_chroma; } set { ph_max_mtt_hierarchy_depth_intra_slice_chroma = value; } }
        private uint ph_log2_diff_max_bt_min_qt_intra_slice_chroma;
        public uint PhLog2DiffMaxBtMinQtIntraSliceChroma { get { return ph_log2_diff_max_bt_min_qt_intra_slice_chroma; } set { ph_log2_diff_max_bt_min_qt_intra_slice_chroma = value; } }
        private uint ph_log2_diff_max_tt_min_qt_intra_slice_chroma;
        public uint PhLog2DiffMaxTtMinQtIntraSliceChroma { get { return ph_log2_diff_max_tt_min_qt_intra_slice_chroma; } set { ph_log2_diff_max_tt_min_qt_intra_slice_chroma = value; } }
        private uint ph_cu_qp_delta_subdiv_intra_slice;
        public uint PhCuQpDeltaSubdivIntraSlice { get { return ph_cu_qp_delta_subdiv_intra_slice; } set { ph_cu_qp_delta_subdiv_intra_slice = value; } }
        private uint ph_cu_chroma_qp_offset_subdiv_intra_slice;
        public uint PhCuChromaQpOffsetSubdivIntraSlice { get { return ph_cu_chroma_qp_offset_subdiv_intra_slice; } set { ph_cu_chroma_qp_offset_subdiv_intra_slice = value; } }
        private uint ph_log2_diff_min_qt_min_cb_inter_slice;
        public uint PhLog2DiffMinQtMinCbInterSlice { get { return ph_log2_diff_min_qt_min_cb_inter_slice; } set { ph_log2_diff_min_qt_min_cb_inter_slice = value; } }
        private uint ph_max_mtt_hierarchy_depth_inter_slice;
        public uint PhMaxMttHierarchyDepthInterSlice { get { return ph_max_mtt_hierarchy_depth_inter_slice; } set { ph_max_mtt_hierarchy_depth_inter_slice = value; } }
        private uint ph_log2_diff_max_bt_min_qt_inter_slice;
        public uint PhLog2DiffMaxBtMinQtInterSlice { get { return ph_log2_diff_max_bt_min_qt_inter_slice; } set { ph_log2_diff_max_bt_min_qt_inter_slice = value; } }
        private uint ph_log2_diff_max_tt_min_qt_inter_slice;
        public uint PhLog2DiffMaxTtMinQtInterSlice { get { return ph_log2_diff_max_tt_min_qt_inter_slice; } set { ph_log2_diff_max_tt_min_qt_inter_slice = value; } }
        private uint ph_cu_qp_delta_subdiv_inter_slice;
        public uint PhCuQpDeltaSubdivInterSlice { get { return ph_cu_qp_delta_subdiv_inter_slice; } set { ph_cu_qp_delta_subdiv_inter_slice = value; } }
        private uint ph_cu_chroma_qp_offset_subdiv_inter_slice;
        public uint PhCuChromaQpOffsetSubdivInterSlice { get { return ph_cu_chroma_qp_offset_subdiv_inter_slice; } set { ph_cu_chroma_qp_offset_subdiv_inter_slice = value; } }
        private byte ph_temporal_mvp_enabled_flag;
        public byte PhTemporalMvpEnabledFlag { get { return ph_temporal_mvp_enabled_flag; } set { ph_temporal_mvp_enabled_flag = value; } }
        private byte ph_collocated_from_l0_flag;
        public byte PhCollocatedFromL0Flag { get { return ph_collocated_from_l0_flag; } set { ph_collocated_from_l0_flag = value; } }
        private uint ph_collocated_ref_idx;
        public uint PhCollocatedRefIdx { get { return ph_collocated_ref_idx; } set { ph_collocated_ref_idx = value; } }
        private byte ph_mmvd_fullpel_only_flag;
        public byte PhMmvdFullpelOnlyFlag { get { return ph_mmvd_fullpel_only_flag; } set { ph_mmvd_fullpel_only_flag = value; } }
        private byte ph_mvd_l1_zero_flag;
        public byte PhMvdL1ZeroFlag { get { return ph_mvd_l1_zero_flag; } set { ph_mvd_l1_zero_flag = value; } }
        private byte ph_bdof_disabled_flag;
        public byte PhBdofDisabledFlag { get { return ph_bdof_disabled_flag; } set { ph_bdof_disabled_flag = value; } }
        private byte ph_dmvr_disabled_flag;
        public byte PhDmvrDisabledFlag { get { return ph_dmvr_disabled_flag; } set { ph_dmvr_disabled_flag = value; } }
        private byte ph_prof_disabled_flag;
        public byte PhProfDisabledFlag { get { return ph_prof_disabled_flag; } set { ph_prof_disabled_flag = value; } }
        private PredWeightTable pred_weight_table;
        public PredWeightTable PredWeightTable { get { return pred_weight_table; } set { pred_weight_table = value; } }
        private int ph_qp_delta;
        public int PhQpDelta { get { return ph_qp_delta; } set { ph_qp_delta = value; } }
        private byte ph_joint_cbcr_sign_flag;
        public byte PhJointCbcrSignFlag { get { return ph_joint_cbcr_sign_flag; } set { ph_joint_cbcr_sign_flag = value; } }
        private byte ph_sao_luma_enabled_flag;
        public byte PhSaoLumaEnabledFlag { get { return ph_sao_luma_enabled_flag; } set { ph_sao_luma_enabled_flag = value; } }
        private byte ph_sao_chroma_enabled_flag;
        public byte PhSaoChromaEnabledFlag { get { return ph_sao_chroma_enabled_flag; } set { ph_sao_chroma_enabled_flag = value; } }
        private byte ph_deblocking_params_present_flag;
        public byte PhDeblockingParamsPresentFlag { get { return ph_deblocking_params_present_flag; } set { ph_deblocking_params_present_flag = value; } }
        private byte ph_deblocking_filter_disabled_flag;
        public byte PhDeblockingFilterDisabledFlag { get { return ph_deblocking_filter_disabled_flag; } set { ph_deblocking_filter_disabled_flag = value; } }
        private int ph_luma_beta_offset_div2;
        public int PhLumaBetaOffsetDiv2 { get { return ph_luma_beta_offset_div2; } set { ph_luma_beta_offset_div2 = value; } }
        private int ph_luma_tc_offset_div2;
        public int PhLumaTcOffsetDiv2 { get { return ph_luma_tc_offset_div2; } set { ph_luma_tc_offset_div2 = value; } }
        private int ph_cb_beta_offset_div2;
        public int PhCbBetaOffsetDiv2 { get { return ph_cb_beta_offset_div2; } set { ph_cb_beta_offset_div2 = value; } }
        private int ph_cb_tc_offset_div2;
        public int PhCbTcOffsetDiv2 { get { return ph_cb_tc_offset_div2; } set { ph_cb_tc_offset_div2 = value; } }
        private int ph_cr_beta_offset_div2;
        public int PhCrBetaOffsetDiv2 { get { return ph_cr_beta_offset_div2; } set { ph_cr_beta_offset_div2 = value; } }
        private int ph_cr_tc_offset_div2;
        public int PhCrTcOffsetDiv2 { get { return ph_cr_tc_offset_div2; } set { ph_cr_tc_offset_div2 = value; } }
        private uint ph_extension_length;
        public uint PhExtensionLength { get { return ph_extension_length; } set { ph_extension_length = value; } }
        private uint[] ph_extension_data_byte;
        public uint[] PhExtensionDataByte { get { return ph_extension_data_byte; } set { ph_extension_data_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PictureHeaderStructure()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint presenceFlag = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.ph_gdr_or_irap_pic_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.ph_non_ref_pic_flag);

            if (ph_gdr_or_irap_pic_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_gdr_pic_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.ph_inter_slice_allowed_flag);

            if (ph_inter_slice_allowed_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_intra_slice_allowed_flag);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.ph_pic_parameter_set_id);
            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4, out this.ph_pic_order_cnt_lsb);

            if (ph_gdr_pic_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.ph_recovery_poc_cnt);
            }

            this.ph_extra_bit = new byte[((H266Context)context).NumExtraPhBits];
            for (i = 0; i < ((H266Context)context).NumExtraPhBits; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_extra_bit[i]);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsPocMsbCycleFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_poc_msb_cycle_present_flag);

                if (ph_poc_msb_cycle_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeqParameterSetRbsp.SpsPocMsbCycleLenMinus1 + 1, out this.ph_poc_msb_cycle_val);
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsAlfEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsAlfInfoInPhFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_alf_enabled_flag);

                if (ph_alf_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.ph_num_alf_aps_ids_luma);

                    this.ph_alf_aps_id_luma = new uint[ph_num_alf_aps_ids_luma];
                    for (i = 0; i < ph_num_alf_aps_ids_luma; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.ph_alf_aps_id_luma[i]);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_alf_cb_enabled_flag);
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_alf_cr_enabled_flag);
                    }

                    if (ph_alf_cb_enabled_flag != 0 || ph_alf_cr_enabled_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.ph_alf_aps_id_chroma);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsCcalfEnabledFlag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_alf_cc_cb_enabled_flag);

                        if (ph_alf_cc_cb_enabled_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.ph_alf_cc_cb_aps_id);
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_alf_cc_cr_enabled_flag);

                        if (ph_alf_cc_cr_enabled_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.ph_alf_cc_cr_aps_id);
                        }
                    }
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsLmcsEnabledFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_lmcs_enabled_flag);

                if (ph_lmcs_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.ph_lmcs_aps_id);

                    if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_chroma_residual_scale_flag);
                    }
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsExplicitScalingListEnabledFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_explicit_scaling_list_enabled_flag);

                if (ph_explicit_scaling_list_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.ph_scaling_list_aps_id);
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsVirtualBoundariesEnabledFlag != 0 && ((H266Context)context).SeqParameterSetRbsp.SpsVirtualBoundariesPresentFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_virtual_boundaries_present_flag);

                if (ph_virtual_boundaries_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_num_ver_virtual_boundaries);

                    this.ph_virtual_boundary_pos_x_minus1 = new uint[ph_num_ver_virtual_boundaries];
                    for (i = 0; i < ph_num_ver_virtual_boundaries; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_virtual_boundary_pos_x_minus1[i]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_num_hor_virtual_boundaries);

                    this.ph_virtual_boundary_pos_y_minus1 = new uint[ph_num_hor_virtual_boundaries];
                    for (i = 0; i < ph_num_hor_virtual_boundaries; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_virtual_boundary_pos_y_minus1[i]);
                    }
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsOutputFlagPresentFlag != 0 && ph_non_ref_pic_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_pic_output_flag);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag != 0)
            {
                this.ref_pic_lists = new RefPicLists();
                size += stream.ReadClass<RefPicLists>(size, context, this.ref_pic_lists);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsPartitionConstraintsOverrideEnabledFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_partition_constraints_override_flag);
            }

            if (ph_intra_slice_allowed_flag != 0)
            {

                if (ph_partition_constraints_override_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_min_qt_min_cb_intra_slice_luma);
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_max_mtt_hierarchy_depth_intra_slice_luma);

                    if (ph_max_mtt_hierarchy_depth_intra_slice_luma != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_max_bt_min_qt_intra_slice_luma);
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_max_tt_min_qt_intra_slice_luma);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsQtbttDualTreeIntraFlag != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_min_qt_min_cb_intra_slice_chroma);
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_max_mtt_hierarchy_depth_intra_slice_chroma);

                        if (ph_max_mtt_hierarchy_depth_intra_slice_chroma != 0)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_max_bt_min_qt_intra_slice_chroma);
                            size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_max_tt_min_qt_intra_slice_chroma);
                        }
                    }
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuQpDeltaEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_cu_qp_delta_subdiv_intra_slice);
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_cu_chroma_qp_offset_subdiv_intra_slice);
                }
            }

            if (ph_inter_slice_allowed_flag != 0)
            {

                if (ph_partition_constraints_override_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_min_qt_min_cb_inter_slice);
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_max_mtt_hierarchy_depth_inter_slice);

                    if (ph_max_mtt_hierarchy_depth_inter_slice != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_max_bt_min_qt_inter_slice);
                        size += stream.ReadUnsignedIntGolomb(size, out this.ph_log2_diff_max_tt_min_qt_inter_slice);
                    }
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuQpDeltaEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_cu_qp_delta_subdiv_inter_slice);
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ph_cu_chroma_qp_offset_subdiv_inter_slice);
                }

                if (((H266Context)context).SeqParameterSetRbsp.SpsTemporalMvpEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ph_temporal_mvp_enabled_flag);

                    if (ph_temporal_mvp_enabled_flag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag != 0)
                    {

                        if (((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.ph_collocated_from_l0_flag);
                        }

                        if ((ph_collocated_from_l0_flag != 0 &&
      ((H266Context)context).num_ref_entries[0][((H266Context)context).RplsIdx[0]] > 1) ||
      (ph_collocated_from_l0_flag == 0 &&
      ((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 1))
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.ph_collocated_ref_idx);
                        }
                    }
                }

                if (((H266Context)context).SeqParameterSetRbsp.SpsMmvdFullpelOnlyEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ph_mmvd_fullpel_only_flag);
                }
                presenceFlag = 0;

                if (((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag == 0)
                {
                    presenceFlag = 1;
                }
                else if (((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 0)
                {
                    presenceFlag = 1;
                }

                if (presenceFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ph_mvd_l1_zero_flag);

                    if (((H266Context)context).SeqParameterSetRbsp.SpsBdofControlPresentInPhFlag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_bdof_disabled_flag);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsDmvrControlPresentInPhFlag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_dmvr_disabled_flag);
                    }
                }

                if (((H266Context)context).SeqParameterSetRbsp.SpsProfControlPresentInPhFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ph_prof_disabled_flag);
                }

                if ((((H266Context)context).PicParameterSetRbsp.PpsWeightedPredFlag != 0 || ((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag != 0) &&
    ((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag != 0)
                {
                    this.pred_weight_table = new PredWeightTable();
                    size += stream.ReadClass<PredWeightTable>(size, context, this.pred_weight_table);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsQpDeltaInfoInPhFlag != 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.ph_qp_delta);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsJointCbcrEnabledFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_joint_cbcr_sign_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSaoEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsSaoInfoInPhFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_sao_luma_enabled_flag);

                if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ph_sao_chroma_enabled_flag);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsDbfInfoInPhFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ph_deblocking_params_present_flag);

                if (ph_deblocking_params_present_flag != 0)
                {

                    if (((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterDisabledFlag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.ph_deblocking_filter_disabled_flag);
                    }

                    if (ph_deblocking_filter_disabled_flag == 0)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.ph_luma_beta_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.ph_luma_tc_offset_div2);

                        if (((H266Context)context).PicParameterSetRbsp.PpsChromaToolOffsetsPresentFlag != 0)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.ph_cb_beta_offset_div2);
                            size += stream.ReadSignedIntGolomb(size, out this.ph_cb_tc_offset_div2);
                            size += stream.ReadSignedIntGolomb(size, out this.ph_cr_beta_offset_div2);
                            size += stream.ReadSignedIntGolomb(size, out this.ph_cr_tc_offset_div2);
                        }
                    }
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsPictureHeaderExtensionPresentFlag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.ph_extension_length);

                this.ph_extension_data_byte = new uint[ph_extension_length];
                for (i = 0; i < ph_extension_length; i++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.ph_extension_data_byte[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint presenceFlag = 0;
            size += stream.WriteUnsignedInt(1, this.ph_gdr_or_irap_pic_flag);
            size += stream.WriteUnsignedInt(1, this.ph_non_ref_pic_flag);

            if (ph_gdr_or_irap_pic_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_gdr_pic_flag);
            }
            size += stream.WriteUnsignedInt(1, this.ph_inter_slice_allowed_flag);

            if (ph_inter_slice_allowed_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_intra_slice_allowed_flag);
            }
            size += stream.WriteUnsignedIntGolomb(this.ph_pic_parameter_set_id);
            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4, this.ph_pic_order_cnt_lsb);

            if (ph_gdr_pic_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.ph_recovery_poc_cnt);
            }

            for (i = 0; i < ((H266Context)context).NumExtraPhBits; i++)
            {
                size += stream.WriteUnsignedInt(1, this.ph_extra_bit[i]);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsPocMsbCycleFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_poc_msb_cycle_present_flag);

                if (ph_poc_msb_cycle_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntVariable(((H266Context)context).SeqParameterSetRbsp.SpsPocMsbCycleLenMinus1 + 1, this.ph_poc_msb_cycle_val);
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsAlfEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsAlfInfoInPhFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_alf_enabled_flag);

                if (ph_alf_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.ph_num_alf_aps_ids_luma);

                    for (i = 0; i < ph_num_alf_aps_ids_luma; i++)
                    {
                        size += stream.WriteUnsignedInt(3, this.ph_alf_aps_id_luma[i]);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.ph_alf_cb_enabled_flag);
                        size += stream.WriteUnsignedInt(1, this.ph_alf_cr_enabled_flag);
                    }

                    if (ph_alf_cb_enabled_flag != 0 || ph_alf_cr_enabled_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(3, this.ph_alf_aps_id_chroma);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsCcalfEnabledFlag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.ph_alf_cc_cb_enabled_flag);

                        if (ph_alf_cc_cb_enabled_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(3, this.ph_alf_cc_cb_aps_id);
                        }
                        size += stream.WriteUnsignedInt(1, this.ph_alf_cc_cr_enabled_flag);

                        if (ph_alf_cc_cr_enabled_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(3, this.ph_alf_cc_cr_aps_id);
                        }
                    }
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsLmcsEnabledFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_lmcs_enabled_flag);

                if (ph_lmcs_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(2, this.ph_lmcs_aps_id);

                    if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.ph_chroma_residual_scale_flag);
                    }
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsExplicitScalingListEnabledFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_explicit_scaling_list_enabled_flag);

                if (ph_explicit_scaling_list_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.ph_scaling_list_aps_id);
                }
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsVirtualBoundariesEnabledFlag != 0 && ((H266Context)context).SeqParameterSetRbsp.SpsVirtualBoundariesPresentFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_virtual_boundaries_present_flag);

                if (ph_virtual_boundaries_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_num_ver_virtual_boundaries);

                    for (i = 0; i < ph_num_ver_virtual_boundaries; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ph_virtual_boundary_pos_x_minus1[i]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.ph_num_hor_virtual_boundaries);

                    for (i = 0; i < ph_num_hor_virtual_boundaries; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ph_virtual_boundary_pos_y_minus1[i]);
                    }
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsOutputFlagPresentFlag != 0 && ph_non_ref_pic_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_pic_output_flag);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag != 0)
            {
                size += stream.WriteClass<RefPicLists>(context, this.ref_pic_lists);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsPartitionConstraintsOverrideEnabledFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_partition_constraints_override_flag);
            }

            if (ph_intra_slice_allowed_flag != 0)
            {

                if (ph_partition_constraints_override_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_min_qt_min_cb_intra_slice_luma);
                    size += stream.WriteUnsignedIntGolomb(this.ph_max_mtt_hierarchy_depth_intra_slice_luma);

                    if (ph_max_mtt_hierarchy_depth_intra_slice_luma != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_max_bt_min_qt_intra_slice_luma);
                        size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_max_tt_min_qt_intra_slice_luma);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsQtbttDualTreeIntraFlag != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_min_qt_min_cb_intra_slice_chroma);
                        size += stream.WriteUnsignedIntGolomb(this.ph_max_mtt_hierarchy_depth_intra_slice_chroma);

                        if (ph_max_mtt_hierarchy_depth_intra_slice_chroma != 0)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_max_bt_min_qt_intra_slice_chroma);
                            size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_max_tt_min_qt_intra_slice_chroma);
                        }
                    }
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuQpDeltaEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_cu_qp_delta_subdiv_intra_slice);
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_cu_chroma_qp_offset_subdiv_intra_slice);
                }
            }

            if (ph_inter_slice_allowed_flag != 0)
            {

                if (ph_partition_constraints_override_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_min_qt_min_cb_inter_slice);
                    size += stream.WriteUnsignedIntGolomb(this.ph_max_mtt_hierarchy_depth_inter_slice);

                    if (ph_max_mtt_hierarchy_depth_inter_slice != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_max_bt_min_qt_inter_slice);
                        size += stream.WriteUnsignedIntGolomb(this.ph_log2_diff_max_tt_min_qt_inter_slice);
                    }
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuQpDeltaEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_cu_qp_delta_subdiv_inter_slice);
                }

                if (((H266Context)context).PicParameterSetRbsp.PpsCuChromaQpOffsetListEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ph_cu_chroma_qp_offset_subdiv_inter_slice);
                }

                if (((H266Context)context).SeqParameterSetRbsp.SpsTemporalMvpEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ph_temporal_mvp_enabled_flag);

                    if (ph_temporal_mvp_enabled_flag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag != 0)
                    {

                        if (((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.ph_collocated_from_l0_flag);
                        }

                        if ((ph_collocated_from_l0_flag != 0 &&
      ((H266Context)context).num_ref_entries[0][((H266Context)context).RplsIdx[0]] > 1) ||
      (ph_collocated_from_l0_flag == 0 &&
      ((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 1))
                        {
                            size += stream.WriteUnsignedIntGolomb(this.ph_collocated_ref_idx);
                        }
                    }
                }

                if (((H266Context)context).SeqParameterSetRbsp.SpsMmvdFullpelOnlyEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ph_mmvd_fullpel_only_flag);
                }
                presenceFlag = 0;

                if (((H266Context)context).PicParameterSetRbsp.PpsRplInfoInPhFlag == 0)
                {
                    presenceFlag = 1;
                }
                else if (((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 0)
                {
                    presenceFlag = 1;
                }

                if (presenceFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ph_mvd_l1_zero_flag);

                    if (((H266Context)context).SeqParameterSetRbsp.SpsBdofControlPresentInPhFlag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.ph_bdof_disabled_flag);
                    }

                    if (((H266Context)context).SeqParameterSetRbsp.SpsDmvrControlPresentInPhFlag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.ph_dmvr_disabled_flag);
                    }
                }

                if (((H266Context)context).SeqParameterSetRbsp.SpsProfControlPresentInPhFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ph_prof_disabled_flag);
                }

                if ((((H266Context)context).PicParameterSetRbsp.PpsWeightedPredFlag != 0 || ((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag != 0) &&
    ((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag != 0)
                {
                    size += stream.WriteClass<PredWeightTable>(context, this.pred_weight_table);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsQpDeltaInfoInPhFlag != 0)
            {
                size += stream.WriteSignedIntGolomb(this.ph_qp_delta);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsJointCbcrEnabledFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_joint_cbcr_sign_flag);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsSaoEnabledFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsSaoInfoInPhFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_sao_luma_enabled_flag);

                if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ph_sao_chroma_enabled_flag);
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsDbfInfoInPhFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ph_deblocking_params_present_flag);

                if (ph_deblocking_params_present_flag != 0)
                {

                    if (((H266Context)context).PicParameterSetRbsp.PpsDeblockingFilterDisabledFlag == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.ph_deblocking_filter_disabled_flag);
                    }

                    if (ph_deblocking_filter_disabled_flag == 0)
                    {
                        size += stream.WriteSignedIntGolomb(this.ph_luma_beta_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.ph_luma_tc_offset_div2);

                        if (((H266Context)context).PicParameterSetRbsp.PpsChromaToolOffsetsPresentFlag != 0)
                        {
                            size += stream.WriteSignedIntGolomb(this.ph_cb_beta_offset_div2);
                            size += stream.WriteSignedIntGolomb(this.ph_cb_tc_offset_div2);
                            size += stream.WriteSignedIntGolomb(this.ph_cr_beta_offset_div2);
                            size += stream.WriteSignedIntGolomb(this.ph_cr_tc_offset_div2);
                        }
                    }
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsPictureHeaderExtensionPresentFlag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.ph_extension_length);

                for (i = 0; i < ph_extension_length; i++)
                {
                    size += stream.WriteUnsignedInt(8, this.ph_extension_data_byte[i]);
                }
            }

            return size;
        }

    }

    /*
  

ref_pic_lists() {
    for (i = 0; i < 2; i++) {
        if (sps_num_ref_pic_lists[i] > 0 &&
            (i == 0 || (i == 1 && pps_rpl1_idx_present_flag)))

            rpl_sps_flag[i] u(1)
        if (rpl_sps_flag[i]) {
            if (sps_num_ref_pic_lists[i] > 1 &&
                (i == 0 || (i == 1 && pps_rpl1_idx_present_flag)))

                rpl_idx[i] u(v)
        } else
            ref_pic_list_struct(i, sps_num_ref_pic_lists[i])
        for (j = 0; j < NumLtrpEntries[i][RplsIdx[i]]; j++) {
            if (ltrp_in_header_flag[i][RplsIdx[i]])
                poc_lsb_lt[i][j] u(v)
            delta_poc_msb_cycle_present_flag[i][j] u(1)
            if (delta_poc_msb_cycle_present_flag[i][j])
                delta_poc_msb_cycle_lt[i][j] ue(v)
        }
    }
}
    */
    public class RefPicLists : IItuSerializable
    {
        private byte[] rpl_sps_flag;
        public byte[] RplSpsFlag { get { return rpl_sps_flag; } set { rpl_sps_flag = value; } }
        private uint[] rpl_idx;
        public uint[] RplIdx { get { return rpl_idx; } set { rpl_idx = value; } }
        private RefPicListStruct ref_pic_list_struct;
        public RefPicListStruct RefPicListStruct { get { return ref_pic_list_struct; } set { ref_pic_list_struct = value; } }
        private uint[][] poc_lsb_lt;
        public uint[][] PocLsbLt { get { return poc_lsb_lt; } set { poc_lsb_lt = value; } }
        private byte[][] delta_poc_msb_cycle_present_flag;
        public byte[][] DeltaPocMsbCyclePresentFlag { get { return delta_poc_msb_cycle_present_flag; } set { delta_poc_msb_cycle_present_flag = value; } }
        private uint[][] delta_poc_msb_cycle_lt;
        public uint[][] DeltaPocMsbCycleLt { get { return delta_poc_msb_cycle_lt; } set { delta_poc_msb_cycle_lt = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RefPicLists()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            this.rpl_sps_flag = new byte[2];
            this.rpl_idx = new uint[2];
            if (((H266Context)context).num_ref_entries == null)
                ((H266Context)context).num_ref_entries = new uint[2][] { new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1], new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1] };
            if (((H266Context)context).inter_layer_ref_pic_flag == null)
                ((H266Context)context).inter_layer_ref_pic_flag = new byte[2][][] { new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };
            if (((H266Context)context).st_ref_pic_flag == null)
                ((H266Context)context).st_ref_pic_flag = new byte[2][][] { new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };
            if (((H266Context)context).abs_delta_poc_st == null)
                ((H266Context)context).abs_delta_poc_st = new uint[2][][] { new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };
            if (((H266Context)context).strp_entry_sign_flag == null)
                ((H266Context)context).strp_entry_sign_flag = new byte[2][][] { new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new byte[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };
            if (((H266Context)context).rpls_poc_lsb_lt == null)
                ((H266Context)context).rpls_poc_lsb_lt = new uint[2][][] { new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };
            if (((H266Context)context).ilrp_idx == null)
                ((H266Context)context).ilrp_idx = new uint[2][][] { new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[0] + 1][], new uint[((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[1] + 1][] };
            this.poc_lsb_lt = new uint[2][];
            this.delta_poc_msb_cycle_present_flag = new byte[2][];
            this.delta_poc_msb_cycle_lt = new uint[2][];
            for (i = 0; i < 2; i++)
            {

                if (((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i] > 0 &&
            (i == 0 || (i == 1 && ((H266Context)context).PicParameterSetRbsp.PpsRpl1IdxPresentFlag != 0)))
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.rpl_sps_flag[i]);
                }

                if (rpl_sps_flag[i] != 0)
                {

                    if (((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i] > 1 &&
                (i == 0 || (i == 1 && ((H266Context)context).PicParameterSetRbsp.PpsRpl1IdxPresentFlag != 0)))
                    {
                        size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2(((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i])), out this.rpl_idx[i]);
                        ((H266Context)context).OnRplIdx(this, i);
                    }
                }
                else
                {
                    this.ref_pic_list_struct = new RefPicListStruct(i, ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i]);
                    size += stream.ReadClass<RefPicListStruct>(size, context, this.ref_pic_list_struct);
                }

                this.poc_lsb_lt[i] = new uint[((H266Context)context).NumLtrpEntries[i][((H266Context)context).RplsIdx[i]]];
                this.delta_poc_msb_cycle_present_flag[i] = new byte[((H266Context)context).NumLtrpEntries[i][((H266Context)context).RplsIdx[i]]];
                this.delta_poc_msb_cycle_lt[i] = new uint[((H266Context)context).NumLtrpEntries[i][((H266Context)context).RplsIdx[i]]];
                for (j = 0; j < ((H266Context)context).NumLtrpEntries[i][((H266Context)context).RplsIdx[i]]; j++)
                {

                    if (ref_pic_list_struct.LtrpInHeaderFlag[i][((H266Context)context).RplsIdx[i]] != 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4, out this.poc_lsb_lt[i][j]);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.delta_poc_msb_cycle_present_flag[i][j]);

                    if (delta_poc_msb_cycle_present_flag[i][j] != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.delta_poc_msb_cycle_lt[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            for (i = 0; i < 2; i++)
            {

                if (((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i] > 0 &&
            (i == 0 || (i == 1 && ((H266Context)context).PicParameterSetRbsp.PpsRpl1IdxPresentFlag != 0)))
                {
                    size += stream.WriteUnsignedInt(1, this.rpl_sps_flag[i]);
                }

                if (rpl_sps_flag[i] != 0)
                {

                    if (((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i] > 1 &&
                (i == 0 || (i == 1 && ((H266Context)context).PicParameterSetRbsp.PpsRpl1IdxPresentFlag != 0)))
                    {
                        size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2(((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i])), this.rpl_idx[i]);
                        ((H266Context)context).OnRplIdx(this, i);
                    }
                }
                else
                {
                    this.ref_pic_list_struct.ListIdx = i;
                    this.ref_pic_list_struct.RplsIdx = ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[i];
                    size += stream.WriteClass<RefPicListStruct>(context, this.ref_pic_list_struct);
                }

                for (j = 0; j < ((H266Context)context).NumLtrpEntries[i][((H266Context)context).RplsIdx[i]]; j++)
                {

                    if (ref_pic_list_struct.LtrpInHeaderFlag[i][((H266Context)context).RplsIdx[i]] != 0)
                    {
                        size += stream.WriteUnsignedIntVariable(((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4, this.poc_lsb_lt[i][j]);
                    }
                    size += stream.WriteUnsignedInt(1, this.delta_poc_msb_cycle_present_flag[i][j]);

                    if (delta_poc_msb_cycle_present_flag[i][j] != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.delta_poc_msb_cycle_lt[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
   

pred_weight_table() {  
 luma_log2_weight_denom ue(v)
    if (sps_chroma_format_idc != 0)  
  delta_chroma_log2_weight_denom se(v)
    if (pps_wp_info_in_ph_flag)  
  num_l0_weights ue(v)
    for (i = 0; i < NumWeightsL0; i++)
        luma_weight_l0_flag[i] u(1)
    if (sps_chroma_format_idc != 0)
        for (i = 0; i < NumWeightsL0; i++)
            chroma_weight_l0_flag[i] u(1)
    for (i = 0; i < NumWeightsL0; i++) {
        if (luma_weight_l0_flag[i]) {
            delta_luma_weight_l0[i] se(v)
            luma_offset_l0[i] se(v)
        }
        if (chroma_weight_l0_flag[i])
            for (j = 0; j < 2; j++) {
                delta_chroma_weight_l0[i][j] se(v)
                delta_chroma_offset_l0[i][j] se(v)
            }
    }
    if (pps_weighted_bipred_flag && pps_wp_info_in_ph_flag &&
        num_ref_entries[1][RplsIdx[1]] > 0) 
 
  num_l1_weights ue(v)
    for (i = 0; i < NumWeightsL1; i++)
        luma_weight_l1_flag[i] u(1)
    if (sps_chroma_format_idc != 0)
        for (i = 0; i < NumWeightsL1; i++)
            chroma_weight_l1_flag[i] u(1)
    for (i = 0; i < NumWeightsL1; i++) {
        if (luma_weight_l1_flag[i]) {
            delta_luma_weight_l1[i] se(v)
            luma_offset_l1[i] se(v)
        }
        if (chroma_weight_l1_flag[i])
            for (j = 0; j < 2; j++) {
                delta_chroma_weight_l1[i][j] se(v)
                delta_chroma_offset_l1[i][j] se(v)
            }
    }
}
    */
    public class PredWeightTable : IItuSerializable
    {
        private uint luma_log2_weight_denom;
        public uint LumaLog2WeightDenom { get { return luma_log2_weight_denom; } set { luma_log2_weight_denom = value; } }
        private int delta_chroma_log2_weight_denom;
        public int DeltaChromaLog2WeightDenom { get { return delta_chroma_log2_weight_denom; } set { delta_chroma_log2_weight_denom = value; } }
        private uint num_l0_weights;
        public uint NumL0Weights { get { return num_l0_weights; } set { num_l0_weights = value; } }
        private byte[] luma_weight_l0_flag;
        public byte[] LumaWeightL0Flag { get { return luma_weight_l0_flag; } set { luma_weight_l0_flag = value; } }
        private byte[] chroma_weight_l0_flag;
        public byte[] ChromaWeightL0Flag { get { return chroma_weight_l0_flag; } set { chroma_weight_l0_flag = value; } }
        private int[] delta_luma_weight_l0;
        public int[] DeltaLumaWeightL0 { get { return delta_luma_weight_l0; } set { delta_luma_weight_l0 = value; } }
        private int[] luma_offset_l0;
        public int[] LumaOffsetL0 { get { return luma_offset_l0; } set { luma_offset_l0 = value; } }
        private int[][] delta_chroma_weight_l0;
        public int[][] DeltaChromaWeightL0 { get { return delta_chroma_weight_l0; } set { delta_chroma_weight_l0 = value; } }
        private int[][] delta_chroma_offset_l0;
        public int[][] DeltaChromaOffsetL0 { get { return delta_chroma_offset_l0; } set { delta_chroma_offset_l0 = value; } }
        private uint num_l1_weights;
        public uint NumL1Weights { get { return num_l1_weights; } set { num_l1_weights = value; } }
        private byte[] luma_weight_l1_flag;
        public byte[] LumaWeightL1Flag { get { return luma_weight_l1_flag; } set { luma_weight_l1_flag = value; } }
        private byte[] chroma_weight_l1_flag;
        public byte[] ChromaWeightL1Flag { get { return chroma_weight_l1_flag; } set { chroma_weight_l1_flag = value; } }
        private int[] delta_luma_weight_l1;
        public int[] DeltaLumaWeightL1 { get { return delta_luma_weight_l1; } set { delta_luma_weight_l1 = value; } }
        private int[] luma_offset_l1;
        public int[] LumaOffsetL1 { get { return luma_offset_l1; } set { luma_offset_l1 = value; } }
        private int[][] delta_chroma_weight_l1;
        public int[][] DeltaChromaWeightL1 { get { return delta_chroma_weight_l1; } set { delta_chroma_weight_l1 = value; } }
        private int[][] delta_chroma_offset_l1;
        public int[][] DeltaChromaOffsetL1 { get { return delta_chroma_offset_l1; } set { delta_chroma_offset_l1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PredWeightTable()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.luma_log2_weight_denom);

            if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.delta_chroma_log2_weight_denom);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_l0_weights);
                ((H266Context)context).OnNumL0Weights(num_l0_weights);
            }

            this.luma_weight_l0_flag = new byte[((H266Context)context).NumWeightsL0];
            for (i = 0; i < ((H266Context)context).NumWeightsL0; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.luma_weight_l0_flag[i]);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
            {

                this.chroma_weight_l0_flag = new byte[((H266Context)context).NumWeightsL0];
                for (i = 0; i < ((H266Context)context).NumWeightsL0; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.chroma_weight_l0_flag[i]);
                }
            }

            this.delta_luma_weight_l0 = new int[((H266Context)context).NumWeightsL0];
            this.luma_offset_l0 = new int[((H266Context)context).NumWeightsL0];
            this.delta_chroma_weight_l0 = new int[((H266Context)context).NumWeightsL0][];
            this.delta_chroma_offset_l0 = new int[((H266Context)context).NumWeightsL0][];
            for (i = 0; i < ((H266Context)context).NumWeightsL0; i++)
            {

                if (luma_weight_l0_flag[i] != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.delta_luma_weight_l0[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.luma_offset_l0[i]);
                }

                if (chroma_weight_l0_flag[i] != 0)
                {

                    this.delta_chroma_weight_l0[i] = new int[2];
                    this.delta_chroma_offset_l0[i] = new int[2];
                    for (j = 0; j < 2; j++)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.delta_chroma_weight_l0[i][j]);
                        size += stream.ReadSignedIntGolomb(size, out this.delta_chroma_offset_l0[i][j]);
                    }
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag != 0 &&
        ((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_l1_weights);
                ((H266Context)context).OnNumL1Weights(num_l1_weights);
            }

            this.luma_weight_l1_flag = new byte[((H266Context)context).NumWeightsL1];
            for (i = 0; i < ((H266Context)context).NumWeightsL1; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.luma_weight_l1_flag[i]);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
            {

                this.chroma_weight_l1_flag = new byte[((H266Context)context).NumWeightsL1];
                for (i = 0; i < ((H266Context)context).NumWeightsL1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.chroma_weight_l1_flag[i]);
                }
            }

            this.delta_luma_weight_l1 = new int[((H266Context)context).NumWeightsL1];
            this.luma_offset_l1 = new int[((H266Context)context).NumWeightsL1];
            this.delta_chroma_weight_l1 = new int[((H266Context)context).NumWeightsL1][];
            this.delta_chroma_offset_l1 = new int[((H266Context)context).NumWeightsL1][];
            for (i = 0; i < ((H266Context)context).NumWeightsL1; i++)
            {

                if (luma_weight_l1_flag[i] != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.delta_luma_weight_l1[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.luma_offset_l1[i]);
                }

                if (chroma_weight_l1_flag[i] != 0)
                {

                    this.delta_chroma_weight_l1[i] = new int[2];
                    this.delta_chroma_offset_l1[i] = new int[2];
                    for (j = 0; j < 2; j++)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.delta_chroma_weight_l1[i][j]);
                        size += stream.ReadSignedIntGolomb(size, out this.delta_chroma_offset_l1[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.luma_log2_weight_denom);

            if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
            {
                size += stream.WriteSignedIntGolomb(this.delta_chroma_log2_weight_denom);
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_l0_weights);
                ((H266Context)context).OnNumL0Weights(num_l0_weights);
            }

            for (i = 0; i < ((H266Context)context).NumWeightsL0; i++)
            {
                size += stream.WriteUnsignedInt(1, this.luma_weight_l0_flag[i]);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
            {

                for (i = 0; i < ((H266Context)context).NumWeightsL0; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.chroma_weight_l0_flag[i]);
                }
            }

            for (i = 0; i < ((H266Context)context).NumWeightsL0; i++)
            {

                if (luma_weight_l0_flag[i] != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.delta_luma_weight_l0[i]);
                    size += stream.WriteSignedIntGolomb(this.luma_offset_l0[i]);
                }

                if (chroma_weight_l0_flag[i] != 0)
                {

                    for (j = 0; j < 2; j++)
                    {
                        size += stream.WriteSignedIntGolomb(this.delta_chroma_weight_l0[i][j]);
                        size += stream.WriteSignedIntGolomb(this.delta_chroma_offset_l0[i][j]);
                    }
                }
            }

            if (((H266Context)context).PicParameterSetRbsp.PpsWeightedBipredFlag != 0 && ((H266Context)context).PicParameterSetRbsp.PpsWpInfoInPhFlag != 0 &&
        ((H266Context)context).num_ref_entries[1][((H266Context)context).RplsIdx[1]] > 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_l1_weights);
                ((H266Context)context).OnNumL1Weights(num_l1_weights);
            }

            for (i = 0; i < ((H266Context)context).NumWeightsL1; i++)
            {
                size += stream.WriteUnsignedInt(1, this.luma_weight_l1_flag[i]);
            }

            if (((H266Context)context).SeqParameterSetRbsp.SpsChromaFormatIdc != 0)
            {

                for (i = 0; i < ((H266Context)context).NumWeightsL1; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.chroma_weight_l1_flag[i]);
                }
            }

            for (i = 0; i < ((H266Context)context).NumWeightsL1; i++)
            {

                if (luma_weight_l1_flag[i] != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.delta_luma_weight_l1[i]);
                    size += stream.WriteSignedIntGolomb(this.luma_offset_l1[i]);
                }

                if (chroma_weight_l1_flag[i] != 0)
                {

                    for (j = 0; j < 2; j++)
                    {
                        size += stream.WriteSignedIntGolomb(this.delta_chroma_weight_l1[i][j]);
                        size += stream.WriteSignedIntGolomb(this.delta_chroma_offset_l1[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

sei_rbsp() { 
do  
sei_message()  
while( more_rbsp_data() )  
rbsp_trailing_bits()  
}
    */
    public class SeiRbsp : IItuSerializable
    {
        private Dictionary<int, SeiMessage> sei_message = new Dictionary<int, SeiMessage>();
        public Dictionary<int, SeiMessage> SeiMessage { get { return sei_message; } set { sei_message = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            do
            {
                whileIndex++;

                this.sei_message.Add(whileIndex, new SeiMessage());
                size += stream.ReadClass<SeiMessage>(size, context, this.sei_message[whileIndex]);
            } while (stream.ReadMoreRbspData(this));
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            do
            {
                whileIndex++;

                size += stream.WriteClass<SeiMessage>(context, whileIndex, this.sei_message);
            } while (stream.WriteMoreRbspData(this));
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
 

access_unit_delimiter_rbsp() {  
aud_irap_or_gdr_flag u(1) 
aud_pic_type u(3) 
rbsp_trailing_bits()  
}
    */
    public class AccessUnitDelimiterRbsp : IItuSerializable
    {
        private byte aud_irap_or_gdr_flag;
        public byte AudIrapOrGdrFlag { get { return aud_irap_or_gdr_flag; } set { aud_irap_or_gdr_flag = value; } }
        private uint aud_pic_type;
        public uint AudPicType { get { return aud_pic_type; } set { aud_pic_type = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AccessUnitDelimiterRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.aud_irap_or_gdr_flag);
            size += stream.ReadUnsignedInt(size, 3, out this.aud_pic_type);
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.aud_irap_or_gdr_flag);
            size += stream.WriteUnsignedInt(3, this.aud_pic_type);
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

end_of_seq_rbsp() { 
}
    */
    public class EndOfSeqRbsp : IItuSerializable
    {

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public EndOfSeqRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

    }

    /*
  

end_of_bitstream_rbsp() { 
}
    */
    public class EndOfBitstreamRbsp : IItuSerializable
    {

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public EndOfBitstreamRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

    }

    /*


filler_data_rbsp() { 
while( next_bits( 8 )  ==  0xFF )  
fd_ff_byte  /* equal to 0xFF *//* f(8)
rbsp_trailing_bits()  
}
    */
    public class FillerDataRbsp : IItuSerializable
    {
        private Dictionary<int, uint> fd_ff_byte = new Dictionary<int, uint>();
        public Dictionary<int, uint> FdFfByte { get { return fd_ff_byte; } set { fd_ff_byte = value; } }
        private RbspTrailingBits rbsp_trailing_bits;
        public RbspTrailingBits RbspTrailingBits { get { return rbsp_trailing_bits; } set { rbsp_trailing_bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FillerDataRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            while (stream.ReadNextBits(this, 8) == 0xFF)
            {
                whileIndex++;

                size += stream.ReadFixed(size, 8, whileIndex, this.fd_ff_byte); // equal to 0xFF 
            }
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            while (stream.WriteNextBits(this, 8) == 0xFF)
            {
                whileIndex++;

                size += stream.WriteFixed(8, whileIndex, this.fd_ff_byte); // equal to 0xFF 
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

rbsp_trailing_bits() { 
rbsp_stop_one_bit  /* equal to 1 *//* f(1) 
while( !byte_aligned() )  
rbsp_alignment_zero_bit  /* equal to 0 *//* f(1)
}
    */
    public class RbspTrailingBits : IItuSerializable
    {
        private uint rbsp_stop_one_bit;
        public uint RbspStopOneBit { get { return rbsp_stop_one_bit; } set { rbsp_stop_one_bit = value; } }
        private Dictionary<int, uint> rbsp_alignment_zero_bit = new Dictionary<int, uint>();
        public Dictionary<int, uint> RbspAlignmentZeroBit { get { return rbsp_alignment_zero_bit; } set { rbsp_alignment_zero_bit = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RbspTrailingBits()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.ReadFixed(size, 1, out this.rbsp_stop_one_bit); // equal to 1 

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadFixed(size, 1, whileIndex, this.rbsp_alignment_zero_bit); // equal to 0 
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.WriteFixed(1, this.rbsp_stop_one_bit); // equal to 1 

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteFixed(1, whileIndex, this.rbsp_alignment_zero_bit); // equal to 0 
            }

            return size;
        }

    }

    /*
  

byte_alignment() { 
byte_alignment_bit_equal_to_one  /* equal to 1 *//* f(1) 
while( !byte_aligned() )  
byte_alignment_bit_equal_to_zero  /* equal to 0 *//* f(1)
}
    */
    public class ByteAlignment : IItuSerializable
    {
        private uint byte_alignment_bit_equal_to_one;
        public uint ByteAlignmentBitEqualToOne { get { return byte_alignment_bit_equal_to_one; } set { byte_alignment_bit_equal_to_one = value; } }
        private Dictionary<int, uint> byte_alignment_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> ByteAlignmentBitEqualToZero { get { return byte_alignment_bit_equal_to_zero; } set { byte_alignment_bit_equal_to_zero = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ByteAlignment()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.ReadFixed(size, 1, out this.byte_alignment_bit_equal_to_one); // equal to 1 

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadFixed(size, 1, whileIndex, this.byte_alignment_bit_equal_to_zero); // equal to 0 
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.WriteFixed(1, this.byte_alignment_bit_equal_to_one); // equal to 1 

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteFixed(1, whileIndex, this.byte_alignment_bit_equal_to_zero); // equal to 0 
            }

            return size;
        }

    }

    /*
  

alf_data() { 
alf_luma_filter_signal_flag  u(1)
if( aps_chroma_present_flag ) {  
alf_chroma_filter_signal_flag u(1)
alf_cc_cb_filter_signal_flag u(1)
alf_cc_cr_filter_signal_flag u(1)
}  

 if( alf_luma_filter_signal_flag ) {  
  alf_luma_clip_flag u(1) 
  alf_luma_num_filters_signalled_minus1 ue(v) 
  if( alf_luma_num_filters_signalled_minus1 > 0 )  
   for( filtIdx = 0; filtIdx < NumAlfFilters; filtIdx++ )  
    alf_luma_coeff_delta_idx[ filtIdx ] u(v) 
  for( sfIdx = 0; sfIdx  <=  alf_luma_num_filters_signalled_minus1; sfIdx++ )  
   for( j = 0; j < 12; j++ ) {  
    alf_luma_coeff_abs[ sfIdx ][ j ] ue(v) 
    if( alf_luma_coeff_abs[ sfIdx ][ j ] )  
     alf_luma_coeff_sign[ sfIdx ][ j ] u(1) 
   }  
  if( alf_luma_clip_flag )  
   for( sfIdx = 0; sfIdx  <=  alf_luma_num_filters_signalled_minus1; sfIdx++ )  
    for( j = 0; j < 12; j++ )  
     alf_luma_clip_idx[ sfIdx ][ j ] u(2) 
 }  
 if( alf_chroma_filter_signal_flag ) {  
  alf_chroma_clip_flag u(1) 
  alf_chroma_num_alt_filters_minus1 ue(v) 
  for( altIdx = 0; altIdx  <=  alf_chroma_num_alt_filters_minus1; altIdx++ ) {  
   for( j = 0; j < 6; j++ ) {  
    alf_chroma_coeff_abs[ altIdx ][ j ] ue(v) 
    if( alf_chroma_coeff_abs[ altIdx ][ j ] > 0 )  
     alf_chroma_coeff_sign[ altIdx ][ j ] u(1) 
   }  
   if( alf_chroma_clip_flag )  
    for( j = 0; j < 6; j++ )  
     alf_chroma_clip_idx[ altIdx ][ j ] u(2) 
  }  
 }  
 if( alf_cc_cb_filter_signal_flag ) {  
  alf_cc_cb_filters_signalled_minus1 ue(v) 
  for( k = 0; k < alf_cc_cb_filters_signalled_minus1 + 1; k++ ) {  
   for( j = 0; j < 7; j++ ) {  
    alf_cc_cb_mapped_coeff_abs[ k ][ j ] u(3) 
    if( alf_cc_cb_mapped_coeff_abs[ k ][ j ] )  
     alf_cc_cb_coeff_sign[ k ][ j ] u(1) 
   }  
  }  
 }  
 if( alf_cc_cr_filter_signal_flag ) {  
  alf_cc_cr_filters_signalled_minus1 ue(v) 
  for( k = 0; k < alf_cc_cr_filters_signalled_minus1 + 1; k++ ) {  
   for( j = 0; j < 7; j++ ) {  
    alf_cc_cr_mapped_coeff_abs[ k ][ j ] u(3) 
     if( alf_cc_cr_mapped_coeff_abs[ k ][ j ] )  
     alf_cc_cr_coeff_sign[ k ][ j ] u(1)
   }  
  }  
 }  
}
    */
    public class AlfData : IItuSerializable
    {
        private byte alf_luma_filter_signal_flag;
        public byte AlfLumaFilterSignalFlag { get { return alf_luma_filter_signal_flag; } set { alf_luma_filter_signal_flag = value; } }
        private byte alf_chroma_filter_signal_flag;
        public byte AlfChromaFilterSignalFlag { get { return alf_chroma_filter_signal_flag; } set { alf_chroma_filter_signal_flag = value; } }
        private byte alf_cc_cb_filter_signal_flag;
        public byte AlfCcCbFilterSignalFlag { get { return alf_cc_cb_filter_signal_flag; } set { alf_cc_cb_filter_signal_flag = value; } }
        private byte alf_cc_cr_filter_signal_flag;
        public byte AlfCcCrFilterSignalFlag { get { return alf_cc_cr_filter_signal_flag; } set { alf_cc_cr_filter_signal_flag = value; } }
        private byte alf_luma_clip_flag;
        public byte AlfLumaClipFlag { get { return alf_luma_clip_flag; } set { alf_luma_clip_flag = value; } }
        private uint alf_luma_num_filters_signalled_minus1;
        public uint AlfLumaNumFiltersSignalledMinus1 { get { return alf_luma_num_filters_signalled_minus1; } set { alf_luma_num_filters_signalled_minus1 = value; } }
        private uint[] alf_luma_coeff_delta_idx;
        public uint[] AlfLumaCoeffDeltaIdx { get { return alf_luma_coeff_delta_idx; } set { alf_luma_coeff_delta_idx = value; } }
        private uint[][] alf_luma_coeff_abs;
        public uint[][] AlfLumaCoeffAbs { get { return alf_luma_coeff_abs; } set { alf_luma_coeff_abs = value; } }
        private byte[][] alf_luma_coeff_sign;
        public byte[][] AlfLumaCoeffSign { get { return alf_luma_coeff_sign; } set { alf_luma_coeff_sign = value; } }
        private uint[][] alf_luma_clip_idx;
        public uint[][] AlfLumaClipIdx { get { return alf_luma_clip_idx; } set { alf_luma_clip_idx = value; } }
        private byte alf_chroma_clip_flag;
        public byte AlfChromaClipFlag { get { return alf_chroma_clip_flag; } set { alf_chroma_clip_flag = value; } }
        private uint alf_chroma_num_alt_filters_minus1;
        public uint AlfChromaNumAltFiltersMinus1 { get { return alf_chroma_num_alt_filters_minus1; } set { alf_chroma_num_alt_filters_minus1 = value; } }
        private uint[][] alf_chroma_coeff_abs;
        public uint[][] AlfChromaCoeffAbs { get { return alf_chroma_coeff_abs; } set { alf_chroma_coeff_abs = value; } }
        private byte[][] alf_chroma_coeff_sign;
        public byte[][] AlfChromaCoeffSign { get { return alf_chroma_coeff_sign; } set { alf_chroma_coeff_sign = value; } }
        private uint[][] alf_chroma_clip_idx;
        public uint[][] AlfChromaClipIdx { get { return alf_chroma_clip_idx; } set { alf_chroma_clip_idx = value; } }
        private uint alf_cc_cb_filters_signalled_minus1;
        public uint AlfCcCbFiltersSignalledMinus1 { get { return alf_cc_cb_filters_signalled_minus1; } set { alf_cc_cb_filters_signalled_minus1 = value; } }
        private uint[][] alf_cc_cb_mapped_coeff_abs;
        public uint[][] AlfCcCbMappedCoeffAbs { get { return alf_cc_cb_mapped_coeff_abs; } set { alf_cc_cb_mapped_coeff_abs = value; } }
        private byte[][] alf_cc_cb_coeff_sign;
        public byte[][] AlfCcCbCoeffSign { get { return alf_cc_cb_coeff_sign; } set { alf_cc_cb_coeff_sign = value; } }
        private uint alf_cc_cr_filters_signalled_minus1;
        public uint AlfCcCrFiltersSignalledMinus1 { get { return alf_cc_cr_filters_signalled_minus1; } set { alf_cc_cr_filters_signalled_minus1 = value; } }
        private uint[][] alf_cc_cr_mapped_coeff_abs;
        public uint[][] AlfCcCrMappedCoeffAbs { get { return alf_cc_cr_mapped_coeff_abs; } set { alf_cc_cr_mapped_coeff_abs = value; } }
        private byte[][] alf_cc_cr_coeff_sign;
        public byte[][] AlfCcCrCoeffSign { get { return alf_cc_cr_coeff_sign; } set { alf_cc_cr_coeff_sign = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AlfData()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint filtIdx = 0;
            uint sfIdx = 0;
            uint j = 0;
            uint altIdx = 0;
            uint k = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.alf_luma_filter_signal_flag);

            if (((H266Context)context).AdaptationParameterSetRbsp.ApsChromaPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.alf_chroma_filter_signal_flag);
                ((H266Context)context).OnAlfChromaFilterSignalFlag();
                size += stream.ReadUnsignedInt(size, 1, out this.alf_cc_cb_filter_signal_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.alf_cc_cr_filter_signal_flag);
            }

            if (alf_luma_filter_signal_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.alf_luma_clip_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.alf_luma_num_filters_signalled_minus1);

                if (alf_luma_num_filters_signalled_minus1 > 0)
                {

                    this.alf_luma_coeff_delta_idx = new uint[((H266Context)context).NumAlfFilters];
                    for (filtIdx = 0; filtIdx < ((H266Context)context).NumAlfFilters; filtIdx++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2(alf_luma_num_filters_signalled_minus1 + 1)), out this.alf_luma_coeff_delta_idx[filtIdx]);
                    }
                }

                this.alf_luma_coeff_abs = new uint[alf_luma_num_filters_signalled_minus1 + 1][];
                this.alf_luma_coeff_sign = new byte[alf_luma_num_filters_signalled_minus1 + 1][];
                for (sfIdx = 0; sfIdx <= alf_luma_num_filters_signalled_minus1; sfIdx++)
                {

                    this.alf_luma_coeff_abs[sfIdx] = new uint[12];
                    this.alf_luma_coeff_sign[sfIdx] = new byte[12];
                    for (j = 0; j < 12; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.alf_luma_coeff_abs[sfIdx][j]);

                        if (alf_luma_coeff_abs[sfIdx][j] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.alf_luma_coeff_sign[sfIdx][j]);
                        }
                    }
                }

                if (alf_luma_clip_flag != 0)
                {

                    this.alf_luma_clip_idx = new uint[alf_luma_num_filters_signalled_minus1 + 1][];
                    for (sfIdx = 0; sfIdx <= alf_luma_num_filters_signalled_minus1; sfIdx++)
                    {

                        this.alf_luma_clip_idx[sfIdx] = new uint[12];
                        for (j = 0; j < 12; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 2, out this.alf_luma_clip_idx[sfIdx][j]);
                        }
                    }
                }
            }

            if (alf_chroma_filter_signal_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.alf_chroma_clip_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.alf_chroma_num_alt_filters_minus1);

                this.alf_chroma_coeff_abs = new uint[alf_chroma_num_alt_filters_minus1 + 1][];
                this.alf_chroma_coeff_sign = new byte[alf_chroma_num_alt_filters_minus1 + 1][];
                this.alf_chroma_clip_idx = new uint[alf_chroma_num_alt_filters_minus1 + 1][];
                for (altIdx = 0; altIdx <= alf_chroma_num_alt_filters_minus1; altIdx++)
                {

                    this.alf_chroma_coeff_abs[altIdx] = new uint[6];
                    this.alf_chroma_coeff_sign[altIdx] = new byte[6];
                    for (j = 0; j < 6; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.alf_chroma_coeff_abs[altIdx][j]);

                        if (alf_chroma_coeff_abs[altIdx][j] > 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.alf_chroma_coeff_sign[altIdx][j]);
                        }
                    }

                    if (alf_chroma_clip_flag != 0)
                    {

                        this.alf_chroma_clip_idx[altIdx] = new uint[6];
                        for (j = 0; j < 6; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 2, out this.alf_chroma_clip_idx[altIdx][j]);
                        }
                    }
                }
            }

            if (alf_cc_cb_filter_signal_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.alf_cc_cb_filters_signalled_minus1);

                this.alf_cc_cb_mapped_coeff_abs = new uint[alf_cc_cb_filters_signalled_minus1 + 1 + 1][];
                this.alf_cc_cb_coeff_sign = new byte[alf_cc_cb_filters_signalled_minus1 + 1 + 1][];
                for (k = 0; k < alf_cc_cb_filters_signalled_minus1 + 1; k++)
                {

                    this.alf_cc_cb_mapped_coeff_abs[k] = new uint[7];
                    this.alf_cc_cb_coeff_sign[k] = new byte[7];
                    for (j = 0; j < 7; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.alf_cc_cb_mapped_coeff_abs[k][j]);

                        if (alf_cc_cb_mapped_coeff_abs[k][j] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.alf_cc_cb_coeff_sign[k][j]);
                        }
                    }
                }
            }

            if (alf_cc_cr_filter_signal_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.alf_cc_cr_filters_signalled_minus1);

                this.alf_cc_cr_mapped_coeff_abs = new uint[alf_cc_cr_filters_signalled_minus1 + 1 + 1][];
                this.alf_cc_cr_coeff_sign = new byte[alf_cc_cr_filters_signalled_minus1 + 1 + 1][];
                for (k = 0; k < alf_cc_cr_filters_signalled_minus1 + 1; k++)
                {

                    this.alf_cc_cr_mapped_coeff_abs[k] = new uint[7];
                    this.alf_cc_cr_coeff_sign[k] = new byte[7];
                    for (j = 0; j < 7; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 3, out this.alf_cc_cr_mapped_coeff_abs[k][j]);

                        if (alf_cc_cr_mapped_coeff_abs[k][j] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.alf_cc_cr_coeff_sign[k][j]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint filtIdx = 0;
            uint sfIdx = 0;
            uint j = 0;
            uint altIdx = 0;
            uint k = 0;
            size += stream.WriteUnsignedInt(1, this.alf_luma_filter_signal_flag);

            if (((H266Context)context).AdaptationParameterSetRbsp.ApsChromaPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.alf_chroma_filter_signal_flag);
                ((H266Context)context).OnAlfChromaFilterSignalFlag();
                size += stream.WriteUnsignedInt(1, this.alf_cc_cb_filter_signal_flag);
                size += stream.WriteUnsignedInt(1, this.alf_cc_cr_filter_signal_flag);
            }

            if (alf_luma_filter_signal_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.alf_luma_clip_flag);
                size += stream.WriteUnsignedIntGolomb(this.alf_luma_num_filters_signalled_minus1);

                if (alf_luma_num_filters_signalled_minus1 > 0)
                {

                    for (filtIdx = 0; filtIdx < ((H266Context)context).NumAlfFilters; filtIdx++)
                    {
                        size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2(alf_luma_num_filters_signalled_minus1 + 1)), this.alf_luma_coeff_delta_idx[filtIdx]);
                    }
                }

                for (sfIdx = 0; sfIdx <= alf_luma_num_filters_signalled_minus1; sfIdx++)
                {

                    for (j = 0; j < 12; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.alf_luma_coeff_abs[sfIdx][j]);

                        if (alf_luma_coeff_abs[sfIdx][j] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.alf_luma_coeff_sign[sfIdx][j]);
                        }
                    }
                }

                if (alf_luma_clip_flag != 0)
                {

                    for (sfIdx = 0; sfIdx <= alf_luma_num_filters_signalled_minus1; sfIdx++)
                    {

                        for (j = 0; j < 12; j++)
                        {
                            size += stream.WriteUnsignedInt(2, this.alf_luma_clip_idx[sfIdx][j]);
                        }
                    }
                }
            }

            if (alf_chroma_filter_signal_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.alf_chroma_clip_flag);
                size += stream.WriteUnsignedIntGolomb(this.alf_chroma_num_alt_filters_minus1);

                for (altIdx = 0; altIdx <= alf_chroma_num_alt_filters_minus1; altIdx++)
                {

                    for (j = 0; j < 6; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.alf_chroma_coeff_abs[altIdx][j]);

                        if (alf_chroma_coeff_abs[altIdx][j] > 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.alf_chroma_coeff_sign[altIdx][j]);
                        }
                    }

                    if (alf_chroma_clip_flag != 0)
                    {

                        for (j = 0; j < 6; j++)
                        {
                            size += stream.WriteUnsignedInt(2, this.alf_chroma_clip_idx[altIdx][j]);
                        }
                    }
                }
            }

            if (alf_cc_cb_filter_signal_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.alf_cc_cb_filters_signalled_minus1);

                for (k = 0; k < alf_cc_cb_filters_signalled_minus1 + 1; k++)
                {

                    for (j = 0; j < 7; j++)
                    {
                        size += stream.WriteUnsignedInt(3, this.alf_cc_cb_mapped_coeff_abs[k][j]);

                        if (alf_cc_cb_mapped_coeff_abs[k][j] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.alf_cc_cb_coeff_sign[k][j]);
                        }
                    }
                }
            }

            if (alf_cc_cr_filter_signal_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.alf_cc_cr_filters_signalled_minus1);

                for (k = 0; k < alf_cc_cr_filters_signalled_minus1 + 1; k++)
                {

                    for (j = 0; j < 7; j++)
                    {
                        size += stream.WriteUnsignedInt(3, this.alf_cc_cr_mapped_coeff_abs[k][j]);

                        if (alf_cc_cr_mapped_coeff_abs[k][j] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.alf_cc_cr_coeff_sign[k][j]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
 

lmcs_data() {  
 lmcs_min_bin_idx ue(v) 
 lmcs_delta_max_bin_idx ue(v) 
 lmcs_delta_cw_prec_minus1 ue(v) 
 for( i = lmcs_min_bin_idx; i  <=  LmcsMaxBinIdx; i++ ) {  
  lmcs_delta_abs_cw[ i ] u(v) 
  if( lmcs_delta_abs_cw[ i ] > 0 )   
   lmcs_delta_sign_cw_flag[ i ] u(1) 
 }  
 if( aps_chroma_present_flag ) {  
  lmcs_delta_abs_crs u(3) 
  if( lmcs_delta_abs_crs > 0 )  
   lmcs_delta_sign_crs_flag u(1) 
 }  
}
    */
    public class LmcsData : IItuSerializable
    {
        private uint lmcs_min_bin_idx;
        public uint LmcsMinBinIdx { get { return lmcs_min_bin_idx; } set { lmcs_min_bin_idx = value; } }
        private uint lmcs_delta_max_bin_idx;
        public uint LmcsDeltaMaxBinIdx { get { return lmcs_delta_max_bin_idx; } set { lmcs_delta_max_bin_idx = value; } }
        private uint lmcs_delta_cw_prec_minus1;
        public uint LmcsDeltaCwPrecMinus1 { get { return lmcs_delta_cw_prec_minus1; } set { lmcs_delta_cw_prec_minus1 = value; } }
        private uint[] lmcs_delta_abs_cw;
        public uint[] LmcsDeltaAbsCw { get { return lmcs_delta_abs_cw; } set { lmcs_delta_abs_cw = value; } }
        private byte[] lmcs_delta_sign_cw_flag;
        public byte[] LmcsDeltaSignCwFlag { get { return lmcs_delta_sign_cw_flag; } set { lmcs_delta_sign_cw_flag = value; } }
        private uint lmcs_delta_abs_crs;
        public uint LmcsDeltaAbsCrs { get { return lmcs_delta_abs_crs; } set { lmcs_delta_abs_crs = value; } }
        private byte lmcs_delta_sign_crs_flag;
        public byte LmcsDeltaSignCrsFlag { get { return lmcs_delta_sign_crs_flag; } set { lmcs_delta_sign_crs_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public LmcsData()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.lmcs_min_bin_idx);
            size += stream.ReadUnsignedIntGolomb(size, out this.lmcs_delta_max_bin_idx);
            ((H266Context)context).OnLmcsDeltaMaxBinIdx();
            size += stream.ReadUnsignedIntGolomb(size, out this.lmcs_delta_cw_prec_minus1);

            this.lmcs_delta_abs_cw = new uint[((H266Context)context).LmcsMaxBinIdx];
            this.lmcs_delta_sign_cw_flag = new byte[((H266Context)context).LmcsMaxBinIdx];
            for (i = lmcs_min_bin_idx; i <= ((H266Context)context).LmcsMaxBinIdx; i++)
            {
                size += stream.ReadUnsignedIntVariable(size, lmcs_delta_cw_prec_minus1 + 1, out this.lmcs_delta_abs_cw[i]);

                if (lmcs_delta_abs_cw[i] > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.lmcs_delta_sign_cw_flag[i]);
                }
            }

            if (((H266Context)context).AdaptationParameterSetRbsp.ApsChromaPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.lmcs_delta_abs_crs);

                if (lmcs_delta_abs_crs > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.lmcs_delta_sign_crs_flag);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.lmcs_min_bin_idx);
            size += stream.WriteUnsignedIntGolomb(this.lmcs_delta_max_bin_idx);
            ((H266Context)context).OnLmcsDeltaMaxBinIdx();
            size += stream.WriteUnsignedIntGolomb(this.lmcs_delta_cw_prec_minus1);

            for (i = lmcs_min_bin_idx; i <= ((H266Context)context).LmcsMaxBinIdx; i++)
            {
                size += stream.WriteUnsignedIntVariable(lmcs_delta_cw_prec_minus1 + 1, this.lmcs_delta_abs_cw[i]);

                if (lmcs_delta_abs_cw[i] > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.lmcs_delta_sign_cw_flag[i]);
                }
            }

            if (((H266Context)context).AdaptationParameterSetRbsp.ApsChromaPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(3, this.lmcs_delta_abs_crs);

                if (lmcs_delta_abs_crs > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.lmcs_delta_sign_crs_flag);
                }
            }

            return size;
        }

    }

    /*
  

vui_payload( payloadSize ) {  
 vui_parameters( payloadSize ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
 if( more_data_in_payload() ) {  
  if( payload_extension_present() )  
   vui_reserved_payload_extension_data u(v) 
  vui_payload_bit_equal_to_one /* equal to 1 *//* f(1) 
  while( !byte_aligned() )  
   vui_payload_bit_equal_to_zero /* equal to 0 *//* f(1) 
 }  
}
    */
    public class VuiPayload : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private VuiParameters vui_parameters;
        public VuiParameters VuiParameters { get { return vui_parameters; } set { vui_parameters = value; } }
        private uint vui_reserved_payload_extension_data;
        public uint VuiReservedPayloadExtensionData { get { return vui_reserved_payload_extension_data; } set { vui_reserved_payload_extension_data = value; } }
        private uint vui_payload_bit_equal_to_one;
        public uint VuiPayloadBitEqualToOne { get { return vui_payload_bit_equal_to_one; } set { vui_payload_bit_equal_to_one = value; } }
        private Dictionary<int, uint> vui_payload_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> VuiPayloadBitEqualToZero { get { return vui_payload_bit_equal_to_zero; } set { vui_payload_bit_equal_to_zero = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VuiPayload(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            this.vui_parameters = new VuiParameters(payloadSize);
            size += stream.ReadClass<VuiParameters>(size, context, this.vui_parameters); // Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 

            if ((!(stream.ByteAligned() && 8 * payloadSize == stream.GetBitsPositionSinceLastMark())))
            {

                if (stream.ReadMoreRbspData(this, payloadSize))
                {
                    size += stream.ReadUnsignedIntVariable(size, 0 /* TODO */, out this.vui_reserved_payload_extension_data);
                }
                size += stream.ReadFixed(size, 1, out this.vui_payload_bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadFixed(size, 1, whileIndex, this.vui_payload_bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.WriteClass<VuiParameters>(context, this.vui_parameters); // Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 

            if ((!(stream.ByteAligned() && 8 * payloadSize == stream.GetBitsPositionSinceLastMark())))
            {

                if (stream.ReadMoreRbspData(this, payloadSize))
                {
                    size += stream.WriteUnsignedIntVariable(0 /* TODO */, this.vui_reserved_payload_extension_data);
                }
                size += stream.WriteFixed(1, this.vui_payload_bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteFixed(1, whileIndex, this.vui_payload_bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

    }

    /*
  

vui_parameters(payloadSize) {
    vui_progressive_source_flag u(1)
    vui_interlaced_source_flag u(1)
    vui_non_packed_constraint_flag u(1)
    vui_non_projected_constraint_flag u(1)
    vui_aspect_ratio_info_present_flag u(1)
    if (vui_aspect_ratio_info_present_flag) {
        vui_aspect_ratio_constant_flag u(1)
        vui_aspect_ratio_idc u(8)
        if (vui_aspect_ratio_idc == 255) {
            vui_sar_width u(16)
            vui_sar_height u(16)
        }
    }
    vui_overscan_info_present_flag u(1)
    if (vui_overscan_info_present_flag)  
  vui_overscan_appropriate_flag u(1) 
 vui_colour_description_present_flag u(1)
    if (vui_colour_description_present_flag) {  
  vui_colour_primaries u(8) 
  vui_transfer_characteristics u(8) 
  vui_matrix_coeffs u(8) 
  vui_full_range_flag u(1)
    }  
 vui_chroma_loc_info_present_flag u(1)
    if (vui_chroma_loc_info_present_flag) {
        if (vui_progressive_source_flag && !vui_interlaced_source_flag)  
   vui_chroma_sample_loc_type_frame ue(v) 
  else {  
   vui_chroma_sample_loc_type_top_field ue(v) 
   vui_chroma_sample_loc_type_bottom_field ue(v)
        }
    }
}
    */
    public class VuiParameters : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte vui_progressive_source_flag;
        public byte VuiProgressiveSourceFlag { get { return vui_progressive_source_flag; } set { vui_progressive_source_flag = value; } }
        private byte vui_interlaced_source_flag;
        public byte VuiInterlacedSourceFlag { get { return vui_interlaced_source_flag; } set { vui_interlaced_source_flag = value; } }
        private byte vui_non_packed_constraint_flag;
        public byte VuiNonPackedConstraintFlag { get { return vui_non_packed_constraint_flag; } set { vui_non_packed_constraint_flag = value; } }
        private byte vui_non_projected_constraint_flag;
        public byte VuiNonProjectedConstraintFlag { get { return vui_non_projected_constraint_flag; } set { vui_non_projected_constraint_flag = value; } }
        private byte vui_aspect_ratio_info_present_flag;
        public byte VuiAspectRatioInfoPresentFlag { get { return vui_aspect_ratio_info_present_flag; } set { vui_aspect_ratio_info_present_flag = value; } }
        private byte vui_aspect_ratio_constant_flag;
        public byte VuiAspectRatioConstantFlag { get { return vui_aspect_ratio_constant_flag; } set { vui_aspect_ratio_constant_flag = value; } }
        private uint vui_aspect_ratio_idc;
        public uint VuiAspectRatioIdc { get { return vui_aspect_ratio_idc; } set { vui_aspect_ratio_idc = value; } }
        private uint vui_sar_width;
        public uint VuiSarWidth { get { return vui_sar_width; } set { vui_sar_width = value; } }
        private uint vui_sar_height;
        public uint VuiSarHeight { get { return vui_sar_height; } set { vui_sar_height = value; } }
        private byte vui_overscan_info_present_flag;
        public byte VuiOverscanInfoPresentFlag { get { return vui_overscan_info_present_flag; } set { vui_overscan_info_present_flag = value; } }
        private byte vui_overscan_appropriate_flag;
        public byte VuiOverscanAppropriateFlag { get { return vui_overscan_appropriate_flag; } set { vui_overscan_appropriate_flag = value; } }
        private byte vui_colour_description_present_flag;
        public byte VuiColourDescriptionPresentFlag { get { return vui_colour_description_present_flag; } set { vui_colour_description_present_flag = value; } }
        private uint vui_colour_primaries;
        public uint VuiColourPrimaries { get { return vui_colour_primaries; } set { vui_colour_primaries = value; } }
        private uint vui_transfer_characteristics;
        public uint VuiTransferCharacteristics { get { return vui_transfer_characteristics; } set { vui_transfer_characteristics = value; } }
        private uint vui_matrix_coeffs;
        public uint VuiMatrixCoeffs { get { return vui_matrix_coeffs; } set { vui_matrix_coeffs = value; } }
        private byte vui_full_range_flag;
        public byte VuiFullRangeFlag { get { return vui_full_range_flag; } set { vui_full_range_flag = value; } }
        private byte vui_chroma_loc_info_present_flag;
        public byte VuiChromaLocInfoPresentFlag { get { return vui_chroma_loc_info_present_flag; } set { vui_chroma_loc_info_present_flag = value; } }
        private uint vui_chroma_sample_loc_type_frame;
        public uint VuiChromaSampleLocTypeFrame { get { return vui_chroma_sample_loc_type_frame; } set { vui_chroma_sample_loc_type_frame = value; } }
        private uint vui_chroma_sample_loc_type_top_field;
        public uint VuiChromaSampleLocTypeTopField { get { return vui_chroma_sample_loc_type_top_field; } set { vui_chroma_sample_loc_type_top_field = value; } }
        private uint vui_chroma_sample_loc_type_bottom_field;
        public uint VuiChromaSampleLocTypeBottomField { get { return vui_chroma_sample_loc_type_bottom_field; } set { vui_chroma_sample_loc_type_bottom_field = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VuiParameters(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.vui_progressive_source_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vui_interlaced_source_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vui_non_packed_constraint_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vui_non_projected_constraint_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vui_aspect_ratio_info_present_flag);

            if (vui_aspect_ratio_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.vui_aspect_ratio_constant_flag);
                size += stream.ReadUnsignedInt(size, 8, out this.vui_aspect_ratio_idc);

                if (vui_aspect_ratio_idc == 255)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.vui_sar_width);
                    size += stream.ReadUnsignedInt(size, 16, out this.vui_sar_height);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vui_overscan_info_present_flag);

            if (vui_overscan_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.vui_overscan_appropriate_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vui_colour_description_present_flag);

            if (vui_colour_description_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.vui_colour_primaries);
                size += stream.ReadUnsignedInt(size, 8, out this.vui_transfer_characteristics);
                size += stream.ReadUnsignedInt(size, 8, out this.vui_matrix_coeffs);
                size += stream.ReadUnsignedInt(size, 1, out this.vui_full_range_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vui_chroma_loc_info_present_flag);

            if (vui_chroma_loc_info_present_flag != 0)
            {

                if (vui_progressive_source_flag != 0 && vui_interlaced_source_flag == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vui_chroma_sample_loc_type_frame);
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vui_chroma_sample_loc_type_top_field);
                    size += stream.ReadUnsignedIntGolomb(size, out this.vui_chroma_sample_loc_type_bottom_field);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.vui_progressive_source_flag);
            size += stream.WriteUnsignedInt(1, this.vui_interlaced_source_flag);
            size += stream.WriteUnsignedInt(1, this.vui_non_packed_constraint_flag);
            size += stream.WriteUnsignedInt(1, this.vui_non_projected_constraint_flag);
            size += stream.WriteUnsignedInt(1, this.vui_aspect_ratio_info_present_flag);

            if (vui_aspect_ratio_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.vui_aspect_ratio_constant_flag);
                size += stream.WriteUnsignedInt(8, this.vui_aspect_ratio_idc);

                if (vui_aspect_ratio_idc == 255)
                {
                    size += stream.WriteUnsignedInt(16, this.vui_sar_width);
                    size += stream.WriteUnsignedInt(16, this.vui_sar_height);
                }
            }
            size += stream.WriteUnsignedInt(1, this.vui_overscan_info_present_flag);

            if (vui_overscan_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.vui_overscan_appropriate_flag);
            }
            size += stream.WriteUnsignedInt(1, this.vui_colour_description_present_flag);

            if (vui_colour_description_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(8, this.vui_colour_primaries);
                size += stream.WriteUnsignedInt(8, this.vui_transfer_characteristics);
                size += stream.WriteUnsignedInt(8, this.vui_matrix_coeffs);
                size += stream.WriteUnsignedInt(1, this.vui_full_range_flag);
            }
            size += stream.WriteUnsignedInt(1, this.vui_chroma_loc_info_present_flag);

            if (vui_chroma_loc_info_present_flag != 0)
            {

                if (vui_progressive_source_flag != 0 && vui_interlaced_source_flag == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vui_chroma_sample_loc_type_frame);
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.vui_chroma_sample_loc_type_top_field);
                    size += stream.WriteUnsignedIntGolomb(this.vui_chroma_sample_loc_type_bottom_field);
                }
            }

            return size;
        }

    }

    /*
  

profile_tier_level( profileTierPresentFlag, MaxNumSubLayersMinus1 ) {  
 if( profileTierPresentFlag ) {  
  general_profile_idc u(7) 
  general_tier_flag u(1) 
 }  
 general_level_idc u(8) 
 ptl_frame_only_constraint_flag u(1) 
 ptl_multilayer_enabled_flag u(1) 
 if( profileTierPresentFlag )  
  general_constraints_info()  
 for( i = MaxNumSubLayersMinus1 - 1; i  >=  0; i-- )  
  ptl_sublayer_level_present_flag[ i ] u(1) 
 while( !byte_aligned() )  
  ptl_reserved_zero_bit u(1) 
 for( i = MaxNumSubLayersMinus1 - 1; i  >=  0; i-- )  
  if( ptl_sublayer_level_present_flag[ i ] )  
   sublayer_level_idc[ i ] u(8) 
 if( profileTierPresentFlag ) {  
  ptl_num_sub_profiles u(8) 
  for( i = 0; i < ptl_num_sub_profiles; i++ )  
   general_sub_profile_idc[ i ] u(32) 
 }  
 }
    */
    public class ProfileTierLevel : IItuSerializable
    {
        private uint profileTierPresentFlag;
        public uint ProfileTierPresentFlag { get { return profileTierPresentFlag; } set { profileTierPresentFlag = value; } }
        private uint maxNumSubLayersMinus1;
        public uint MaxNumSubLayersMinus1 { get { return maxNumSubLayersMinus1; } set { maxNumSubLayersMinus1 = value; } }
        private uint general_profile_idc;
        public uint GeneralProfileIdc { get { return general_profile_idc; } set { general_profile_idc = value; } }
        private byte general_tier_flag;
        public byte GeneralTierFlag { get { return general_tier_flag; } set { general_tier_flag = value; } }
        private uint general_level_idc;
        public uint GeneralLevelIdc { get { return general_level_idc; } set { general_level_idc = value; } }
        private byte ptl_frame_only_constraint_flag;
        public byte PtlFrameOnlyConstraintFlag { get { return ptl_frame_only_constraint_flag; } set { ptl_frame_only_constraint_flag = value; } }
        private byte ptl_multilayer_enabled_flag;
        public byte PtlMultilayerEnabledFlag { get { return ptl_multilayer_enabled_flag; } set { ptl_multilayer_enabled_flag = value; } }
        private GeneralConstraintsInfo general_constraints_info;
        public GeneralConstraintsInfo GeneralConstraintsInfo { get { return general_constraints_info; } set { general_constraints_info = value; } }
        private byte[] ptl_sublayer_level_present_flag;
        public byte[] PtlSublayerLevelPresentFlag { get { return ptl_sublayer_level_present_flag; } set { ptl_sublayer_level_present_flag = value; } }
        private Dictionary<int, byte> ptl_reserved_zero_bit = new Dictionary<int, byte>();
        public Dictionary<int, byte> PtlReservedZeroBit { get { return ptl_reserved_zero_bit; } set { ptl_reserved_zero_bit = value; } }
        private uint[] sublayer_level_idc;
        public uint[] SublayerLevelIdc { get { return sublayer_level_idc; } set { sublayer_level_idc = value; } }
        private uint ptl_num_sub_profiles;
        public uint PtlNumSubProfiles { get { return ptl_num_sub_profiles; } set { ptl_num_sub_profiles = value; } }
        private uint[] general_sub_profile_idc;
        public uint[] GeneralSubProfileIdc { get { return general_sub_profile_idc; } set { general_sub_profile_idc = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ProfileTierLevel(uint profileTierPresentFlag, uint MaxNumSubLayersMinus1)
        {
            this.profileTierPresentFlag = profileTierPresentFlag;
            this.maxNumSubLayersMinus1 = MaxNumSubLayersMinus1;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int i = 0;
            int whileIndex = -1;

            if (profileTierPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 7, out this.general_profile_idc);
                size += stream.ReadUnsignedInt(size, 1, out this.general_tier_flag);
            }
            size += stream.ReadUnsignedInt(size, 8, out this.general_level_idc);
            size += stream.ReadUnsignedInt(size, 1, out this.ptl_frame_only_constraint_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.ptl_multilayer_enabled_flag);

            if (profileTierPresentFlag != 0)
            {
                this.general_constraints_info = new GeneralConstraintsInfo();
                size += stream.ReadClass<GeneralConstraintsInfo>(size, context, this.general_constraints_info);
            }

            this.ptl_sublayer_level_present_flag = new byte[MaxNumSubLayersMinus1];
            for (i = (int)MaxNumSubLayersMinus1 - 1; i >= 0; i--)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ptl_sublayer_level_present_flag[i]);
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 1, whileIndex, this.ptl_reserved_zero_bit);
            }

            this.sublayer_level_idc = new uint[MaxNumSubLayersMinus1];
            for (i = (int)MaxNumSubLayersMinus1 - 1; i >= 0; i--)
            {

                if (ptl_sublayer_level_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.sublayer_level_idc[i]);
                }
            }

            if (profileTierPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.ptl_num_sub_profiles);

                this.general_sub_profile_idc = new uint[ptl_num_sub_profiles];
                for (i = 0; i < ptl_num_sub_profiles; i++)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.general_sub_profile_idc[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int i = 0;
            int whileIndex = -1;

            if (profileTierPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(7, this.general_profile_idc);
                size += stream.WriteUnsignedInt(1, this.general_tier_flag);
            }
            size += stream.WriteUnsignedInt(8, this.general_level_idc);
            size += stream.WriteUnsignedInt(1, this.ptl_frame_only_constraint_flag);
            size += stream.WriteUnsignedInt(1, this.ptl_multilayer_enabled_flag);

            if (profileTierPresentFlag != 0)
            {
                size += stream.WriteClass<GeneralConstraintsInfo>(context, this.general_constraints_info);
            }

            for (i = (int)MaxNumSubLayersMinus1 - 1; i >= 0; i--)
            {
                size += stream.WriteUnsignedInt(1, this.ptl_sublayer_level_present_flag[i]);
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(1, whileIndex, this.ptl_reserved_zero_bit);
            }

            for (i = (int)MaxNumSubLayersMinus1 - 1; i >= 0; i--)
            {

                if (ptl_sublayer_level_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(8, this.sublayer_level_idc[i]);
                }
            }

            if (profileTierPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(8, this.ptl_num_sub_profiles);

                for (i = 0; i < ptl_num_sub_profiles; i++)
                {
                    size += stream.WriteUnsignedInt(32, this.general_sub_profile_idc[i]);
                }
            }

            return size;
        }

    }

    /*
  

general_constraints_info() {  
 gci_present_flag u(1) 
 if( gci_present_flag ) {  
 /* general *//*  
  gci_intra_only_constraint_flag u(1) 
  gci_all_layers_independent_constraint_flag u(1) 
  gci_one_au_only_constraint_flag  u(1) 
 /* picture format *//*  
  gci_sixteen_minus_max_bitdepth_constraint_idc u(4) 
  gci_three_minus_max_chroma_format_constraint_idc u(2) 
 /* NAL unit type related *//*  
  gci_no_mixed_nalu_types_in_pic_constraint_flag u(1) 
  gci_no_trail_constraint_flag u(1) 
  gci_no_stsa_constraint_flag u(1) 
  gci_no_rasl_constraint_flag u(1) 
  gci_no_radl_constraint_flag u(1) 
  gci_no_idr_constraint_flag u(1) 
  gci_no_cra_constraint_flag u(1) 
  gci_no_gdr_constraint_flag u(1) 
  gci_no_aps_constraint_flag u(1) 
  gci_no_idr_rpl_constraint_flag u(1) 
 /* tile, slice, subpicture partitioning *//*  
  gci_one_tile_per_pic_constraint_flag u(1) 
  gci_pic_header_in_slice_header_constraint_flag u(1) 
  gci_one_slice_per_pic_constraint_flag u(1) 
  gci_no_rectangular_slice_constraint_flag u(1) 
  gci_one_slice_per_subpic_constraint_flag u(1) 
  gci_no_subpic_info_constraint_flag u(1) 
 /* CTU and block partitioning *//*  
  gci_three_minus_max_log2_ctu_size_constraint_idc u(2) 
  gci_no_partition_constraints_override_constraint_flag u(1) 
  gci_no_mtt_constraint_flag u(1) 
  gci_no_qtbtt_dual_tree_intra_constraint_flag u(1) 
 /* intra *//*  
  gci_no_palette_constraint_flag u(1) 
  gci_no_ibc_constraint_flag u(1) 
  gci_no_isp_constraint_flag u(1) 
  gci_no_mrl_constraint_flag u(1) 
  gci_no_mip_constraint_flag u(1) 
  gci_no_cclm_constraint_flag u(1) 
 /* inter *//*  
  gci_no_ref_pic_resampling_constraint_flag u(1) 
  gci_no_res_change_in_clvs_constraint_flag u(1)
    gci_no_weighted_prediction_constraint_flag u(1) 
  gci_no_ref_wraparound_constraint_flag u(1) 
  gci_no_temporal_mvp_constraint_flag u(1) 
  gci_no_sbtmvp_constraint_flag  u(1) 
  gci_no_amvr_constraint_flag u(1) 
  gci_no_bdof_constraint_flag u(1) 
  gci_no_smvd_constraint_flag u(1) 
  gci_no_dmvr_constraint_flag u(1) 
  gci_no_mmvd_constraint_flag u(1) 
  gci_no_affine_motion_constraint_flag u(1) 
  gci_no_prof_constraint_flag u(1) 
  gci_no_bcw_constraint_flag u(1) 
  gci_no_ciip_constraint_flag u(1) 
  gci_no_gpm_constraint_flag u(1) 
 /* transform, quantization, residual *//*  
  gci_no_luma_transform_size_64_constraint_flag u(1) 
  gci_no_transform_skip_constraint_flag u(1) 
  gci_no_bdpcm_constraint_flag u(1) 
  gci_no_mts_constraint_flag u(1) 
  gci_no_lfnst_constraint_flag u(1) 
  gci_no_joint_cbcr_constraint_flag u(1) 
  gci_no_sbt_constraint_flag u(1) 
  gci_no_act_constraint_flag u(1) 
  gci_no_explicit_scaling_list_constraint_flag u(1) 
  gci_no_dep_quant_constraint_flag u(1) 
  gci_no_sign_data_hiding_constraint_flag u(1) 
  gci_no_cu_qp_delta_constraint_flag u(1) 
  gci_no_chroma_qp_offset_constraint_flag u(1) 
 /* loop filter *//*  
  gci_no_sao_constraint_flag u(1) 
  gci_no_alf_constraint_flag u(1) 
  gci_no_ccalf_constraint_flag u(1) 
  gci_no_lmcs_constraint_flag u(1) 
  gci_no_ladf_constraint_flag u(1) 
  gci_no_virtual_boundaries_constraint_flag u(1) 
  gci_num_reserved_bits u(8) 
  for( i = 0; i < gci_num_reserved_bits; i++ )  
   gci_reserved_zero_bit[ i ] u(1) 
 }  
 while( !byte_aligned() )  
  gci_alignment_zero_bit f(1) 
}
    */
    public class GeneralConstraintsInfo : IItuSerializable
    {
        private byte gci_present_flag;
        public byte GciPresentFlag { get { return gci_present_flag; } set { gci_present_flag = value; } }
        private byte gci_intra_only_constraint_flag;
        public byte GciIntraOnlyConstraintFlag { get { return gci_intra_only_constraint_flag; } set { gci_intra_only_constraint_flag = value; } }
        private byte gci_all_layers_independent_constraint_flag;
        public byte GciAllLayersIndependentConstraintFlag { get { return gci_all_layers_independent_constraint_flag; } set { gci_all_layers_independent_constraint_flag = value; } }
        private byte gci_one_au_only_constraint_flag;
        public byte GciOneAuOnlyConstraintFlag { get { return gci_one_au_only_constraint_flag; } set { gci_one_au_only_constraint_flag = value; } }
        private uint gci_sixteen_minus_max_bitdepth_constraint_idc;
        public uint GciSixteenMinusMaxBitdepthConstraintIdc { get { return gci_sixteen_minus_max_bitdepth_constraint_idc; } set { gci_sixteen_minus_max_bitdepth_constraint_idc = value; } }
        private uint gci_three_minus_max_chroma_format_constraint_idc;
        public uint GciThreeMinusMaxChromaFormatConstraintIdc { get { return gci_three_minus_max_chroma_format_constraint_idc; } set { gci_three_minus_max_chroma_format_constraint_idc = value; } }
        private byte gci_no_mixed_nalu_types_in_pic_constraint_flag;
        public byte GciNoMixedNaluTypesInPicConstraintFlag { get { return gci_no_mixed_nalu_types_in_pic_constraint_flag; } set { gci_no_mixed_nalu_types_in_pic_constraint_flag = value; } }
        private byte gci_no_trail_constraint_flag;
        public byte GciNoTrailConstraintFlag { get { return gci_no_trail_constraint_flag; } set { gci_no_trail_constraint_flag = value; } }
        private byte gci_no_stsa_constraint_flag;
        public byte GciNoStsaConstraintFlag { get { return gci_no_stsa_constraint_flag; } set { gci_no_stsa_constraint_flag = value; } }
        private byte gci_no_rasl_constraint_flag;
        public byte GciNoRaslConstraintFlag { get { return gci_no_rasl_constraint_flag; } set { gci_no_rasl_constraint_flag = value; } }
        private byte gci_no_radl_constraint_flag;
        public byte GciNoRadlConstraintFlag { get { return gci_no_radl_constraint_flag; } set { gci_no_radl_constraint_flag = value; } }
        private byte gci_no_idr_constraint_flag;
        public byte GciNoIdrConstraintFlag { get { return gci_no_idr_constraint_flag; } set { gci_no_idr_constraint_flag = value; } }
        private byte gci_no_cra_constraint_flag;
        public byte GciNoCraConstraintFlag { get { return gci_no_cra_constraint_flag; } set { gci_no_cra_constraint_flag = value; } }
        private byte gci_no_gdr_constraint_flag;
        public byte GciNoGdrConstraintFlag { get { return gci_no_gdr_constraint_flag; } set { gci_no_gdr_constraint_flag = value; } }
        private byte gci_no_aps_constraint_flag;
        public byte GciNoApsConstraintFlag { get { return gci_no_aps_constraint_flag; } set { gci_no_aps_constraint_flag = value; } }
        private byte gci_no_idr_rpl_constraint_flag;
        public byte GciNoIdrRplConstraintFlag { get { return gci_no_idr_rpl_constraint_flag; } set { gci_no_idr_rpl_constraint_flag = value; } }
        private byte gci_one_tile_per_pic_constraint_flag;
        public byte GciOneTilePerPicConstraintFlag { get { return gci_one_tile_per_pic_constraint_flag; } set { gci_one_tile_per_pic_constraint_flag = value; } }
        private byte gci_pic_header_in_slice_header_constraint_flag;
        public byte GciPicHeaderInSliceHeaderConstraintFlag { get { return gci_pic_header_in_slice_header_constraint_flag; } set { gci_pic_header_in_slice_header_constraint_flag = value; } }
        private byte gci_one_slice_per_pic_constraint_flag;
        public byte GciOneSlicePerPicConstraintFlag { get { return gci_one_slice_per_pic_constraint_flag; } set { gci_one_slice_per_pic_constraint_flag = value; } }
        private byte gci_no_rectangular_slice_constraint_flag;
        public byte GciNoRectangularSliceConstraintFlag { get { return gci_no_rectangular_slice_constraint_flag; } set { gci_no_rectangular_slice_constraint_flag = value; } }
        private byte gci_one_slice_per_subpic_constraint_flag;
        public byte GciOneSlicePerSubpicConstraintFlag { get { return gci_one_slice_per_subpic_constraint_flag; } set { gci_one_slice_per_subpic_constraint_flag = value; } }
        private byte gci_no_subpic_info_constraint_flag;
        public byte GciNoSubpicInfoConstraintFlag { get { return gci_no_subpic_info_constraint_flag; } set { gci_no_subpic_info_constraint_flag = value; } }
        private uint gci_three_minus_max_log2_ctu_size_constraint_idc;
        public uint GciThreeMinusMaxLog2CtuSizeConstraintIdc { get { return gci_three_minus_max_log2_ctu_size_constraint_idc; } set { gci_three_minus_max_log2_ctu_size_constraint_idc = value; } }
        private byte gci_no_partition_constraints_override_constraint_flag;
        public byte GciNoPartitionConstraintsOverrideConstraintFlag { get { return gci_no_partition_constraints_override_constraint_flag; } set { gci_no_partition_constraints_override_constraint_flag = value; } }
        private byte gci_no_mtt_constraint_flag;
        public byte GciNoMttConstraintFlag { get { return gci_no_mtt_constraint_flag; } set { gci_no_mtt_constraint_flag = value; } }
        private byte gci_no_qtbtt_dual_tree_intra_constraint_flag;
        public byte GciNoQtbttDualTreeIntraConstraintFlag { get { return gci_no_qtbtt_dual_tree_intra_constraint_flag; } set { gci_no_qtbtt_dual_tree_intra_constraint_flag = value; } }
        private byte gci_no_palette_constraint_flag;
        public byte GciNoPaletteConstraintFlag { get { return gci_no_palette_constraint_flag; } set { gci_no_palette_constraint_flag = value; } }
        private byte gci_no_ibc_constraint_flag;
        public byte GciNoIbcConstraintFlag { get { return gci_no_ibc_constraint_flag; } set { gci_no_ibc_constraint_flag = value; } }
        private byte gci_no_isp_constraint_flag;
        public byte GciNoIspConstraintFlag { get { return gci_no_isp_constraint_flag; } set { gci_no_isp_constraint_flag = value; } }
        private byte gci_no_mrl_constraint_flag;
        public byte GciNoMrlConstraintFlag { get { return gci_no_mrl_constraint_flag; } set { gci_no_mrl_constraint_flag = value; } }
        private byte gci_no_mip_constraint_flag;
        public byte GciNoMipConstraintFlag { get { return gci_no_mip_constraint_flag; } set { gci_no_mip_constraint_flag = value; } }
        private byte gci_no_cclm_constraint_flag;
        public byte GciNoCclmConstraintFlag { get { return gci_no_cclm_constraint_flag; } set { gci_no_cclm_constraint_flag = value; } }
        private byte gci_no_ref_pic_resampling_constraint_flag;
        public byte GciNoRefPicResamplingConstraintFlag { get { return gci_no_ref_pic_resampling_constraint_flag; } set { gci_no_ref_pic_resampling_constraint_flag = value; } }
        private byte gci_no_res_change_in_clvs_constraint_flag;
        public byte GciNoResChangeInClvsConstraintFlag { get { return gci_no_res_change_in_clvs_constraint_flag; } set { gci_no_res_change_in_clvs_constraint_flag = value; } }
        private byte gci_no_weighted_prediction_constraint_flag;
        public byte GciNoWeightedPredictionConstraintFlag { get { return gci_no_weighted_prediction_constraint_flag; } set { gci_no_weighted_prediction_constraint_flag = value; } }
        private byte gci_no_ref_wraparound_constraint_flag;
        public byte GciNoRefWraparoundConstraintFlag { get { return gci_no_ref_wraparound_constraint_flag; } set { gci_no_ref_wraparound_constraint_flag = value; } }
        private byte gci_no_temporal_mvp_constraint_flag;
        public byte GciNoTemporalMvpConstraintFlag { get { return gci_no_temporal_mvp_constraint_flag; } set { gci_no_temporal_mvp_constraint_flag = value; } }
        private byte gci_no_sbtmvp_constraint_flag;
        public byte GciNoSbtmvpConstraintFlag { get { return gci_no_sbtmvp_constraint_flag; } set { gci_no_sbtmvp_constraint_flag = value; } }
        private byte gci_no_amvr_constraint_flag;
        public byte GciNoAmvrConstraintFlag { get { return gci_no_amvr_constraint_flag; } set { gci_no_amvr_constraint_flag = value; } }
        private byte gci_no_bdof_constraint_flag;
        public byte GciNoBdofConstraintFlag { get { return gci_no_bdof_constraint_flag; } set { gci_no_bdof_constraint_flag = value; } }
        private byte gci_no_smvd_constraint_flag;
        public byte GciNoSmvdConstraintFlag { get { return gci_no_smvd_constraint_flag; } set { gci_no_smvd_constraint_flag = value; } }
        private byte gci_no_dmvr_constraint_flag;
        public byte GciNoDmvrConstraintFlag { get { return gci_no_dmvr_constraint_flag; } set { gci_no_dmvr_constraint_flag = value; } }
        private byte gci_no_mmvd_constraint_flag;
        public byte GciNoMmvdConstraintFlag { get { return gci_no_mmvd_constraint_flag; } set { gci_no_mmvd_constraint_flag = value; } }
        private byte gci_no_affine_motion_constraint_flag;
        public byte GciNoAffineMotionConstraintFlag { get { return gci_no_affine_motion_constraint_flag; } set { gci_no_affine_motion_constraint_flag = value; } }
        private byte gci_no_prof_constraint_flag;
        public byte GciNoProfConstraintFlag { get { return gci_no_prof_constraint_flag; } set { gci_no_prof_constraint_flag = value; } }
        private byte gci_no_bcw_constraint_flag;
        public byte GciNoBcwConstraintFlag { get { return gci_no_bcw_constraint_flag; } set { gci_no_bcw_constraint_flag = value; } }
        private byte gci_no_ciip_constraint_flag;
        public byte GciNoCiipConstraintFlag { get { return gci_no_ciip_constraint_flag; } set { gci_no_ciip_constraint_flag = value; } }
        private byte gci_no_gpm_constraint_flag;
        public byte GciNoGpmConstraintFlag { get { return gci_no_gpm_constraint_flag; } set { gci_no_gpm_constraint_flag = value; } }
        private byte gci_no_luma_transform_size_64_constraint_flag;
        public byte GciNoLumaTransformSize64ConstraintFlag { get { return gci_no_luma_transform_size_64_constraint_flag; } set { gci_no_luma_transform_size_64_constraint_flag = value; } }
        private byte gci_no_transform_skip_constraint_flag;
        public byte GciNoTransformSkipConstraintFlag { get { return gci_no_transform_skip_constraint_flag; } set { gci_no_transform_skip_constraint_flag = value; } }
        private byte gci_no_bdpcm_constraint_flag;
        public byte GciNoBdpcmConstraintFlag { get { return gci_no_bdpcm_constraint_flag; } set { gci_no_bdpcm_constraint_flag = value; } }
        private byte gci_no_mts_constraint_flag;
        public byte GciNoMtsConstraintFlag { get { return gci_no_mts_constraint_flag; } set { gci_no_mts_constraint_flag = value; } }
        private byte gci_no_lfnst_constraint_flag;
        public byte GciNoLfnstConstraintFlag { get { return gci_no_lfnst_constraint_flag; } set { gci_no_lfnst_constraint_flag = value; } }
        private byte gci_no_joint_cbcr_constraint_flag;
        public byte GciNoJointCbcrConstraintFlag { get { return gci_no_joint_cbcr_constraint_flag; } set { gci_no_joint_cbcr_constraint_flag = value; } }
        private byte gci_no_sbt_constraint_flag;
        public byte GciNoSbtConstraintFlag { get { return gci_no_sbt_constraint_flag; } set { gci_no_sbt_constraint_flag = value; } }
        private byte gci_no_act_constraint_flag;
        public byte GciNoActConstraintFlag { get { return gci_no_act_constraint_flag; } set { gci_no_act_constraint_flag = value; } }
        private byte gci_no_explicit_scaling_list_constraint_flag;
        public byte GciNoExplicitScalingListConstraintFlag { get { return gci_no_explicit_scaling_list_constraint_flag; } set { gci_no_explicit_scaling_list_constraint_flag = value; } }
        private byte gci_no_dep_quant_constraint_flag;
        public byte GciNoDepQuantConstraintFlag { get { return gci_no_dep_quant_constraint_flag; } set { gci_no_dep_quant_constraint_flag = value; } }
        private byte gci_no_sign_data_hiding_constraint_flag;
        public byte GciNoSignDataHidingConstraintFlag { get { return gci_no_sign_data_hiding_constraint_flag; } set { gci_no_sign_data_hiding_constraint_flag = value; } }
        private byte gci_no_cu_qp_delta_constraint_flag;
        public byte GciNoCuQpDeltaConstraintFlag { get { return gci_no_cu_qp_delta_constraint_flag; } set { gci_no_cu_qp_delta_constraint_flag = value; } }
        private byte gci_no_chroma_qp_offset_constraint_flag;
        public byte GciNoChromaQpOffsetConstraintFlag { get { return gci_no_chroma_qp_offset_constraint_flag; } set { gci_no_chroma_qp_offset_constraint_flag = value; } }
        private byte gci_no_sao_constraint_flag;
        public byte GciNoSaoConstraintFlag { get { return gci_no_sao_constraint_flag; } set { gci_no_sao_constraint_flag = value; } }
        private byte gci_no_alf_constraint_flag;
        public byte GciNoAlfConstraintFlag { get { return gci_no_alf_constraint_flag; } set { gci_no_alf_constraint_flag = value; } }
        private byte gci_no_ccalf_constraint_flag;
        public byte GciNoCcalfConstraintFlag { get { return gci_no_ccalf_constraint_flag; } set { gci_no_ccalf_constraint_flag = value; } }
        private byte gci_no_lmcs_constraint_flag;
        public byte GciNoLmcsConstraintFlag { get { return gci_no_lmcs_constraint_flag; } set { gci_no_lmcs_constraint_flag = value; } }
        private byte gci_no_ladf_constraint_flag;
        public byte GciNoLadfConstraintFlag { get { return gci_no_ladf_constraint_flag; } set { gci_no_ladf_constraint_flag = value; } }
        private byte gci_no_virtual_boundaries_constraint_flag;
        public byte GciNoVirtualBoundariesConstraintFlag { get { return gci_no_virtual_boundaries_constraint_flag; } set { gci_no_virtual_boundaries_constraint_flag = value; } }
        private uint gci_num_reserved_bits;
        public uint GciNumReservedBits { get { return gci_num_reserved_bits; } set { gci_num_reserved_bits = value; } }
        private byte[] gci_reserved_zero_bit;
        public byte[] GciReservedZeroBit { get { return gci_reserved_zero_bit; } set { gci_reserved_zero_bit = value; } }
        private Dictionary<int, uint> gci_alignment_zero_bit = new Dictionary<int, uint>();
        public Dictionary<int, uint> GciAlignmentZeroBit { get { return gci_alignment_zero_bit; } set { gci_alignment_zero_bit = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public GeneralConstraintsInfo()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 1, out this.gci_present_flag);

            if (gci_present_flag != 0)
            {
                /*  general  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_intra_only_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_all_layers_independent_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_one_au_only_constraint_flag);
                /*  picture format  */

                size += stream.ReadUnsignedInt(size, 4, out this.gci_sixteen_minus_max_bitdepth_constraint_idc);
                size += stream.ReadUnsignedInt(size, 2, out this.gci_three_minus_max_chroma_format_constraint_idc);
                /*  NAL unit type related  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_mixed_nalu_types_in_pic_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_trail_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_stsa_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_rasl_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_radl_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_idr_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_cra_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_gdr_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_aps_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_idr_rpl_constraint_flag);
                /*  tile, slice, subpicture partitioning  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_one_tile_per_pic_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_pic_header_in_slice_header_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_one_slice_per_pic_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_rectangular_slice_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_one_slice_per_subpic_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_subpic_info_constraint_flag);
                /*  CTU and block partitioning  */

                size += stream.ReadUnsignedInt(size, 2, out this.gci_three_minus_max_log2_ctu_size_constraint_idc);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_partition_constraints_override_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_mtt_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_qtbtt_dual_tree_intra_constraint_flag);
                /*  intra  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_palette_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_ibc_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_isp_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_mrl_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_mip_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_cclm_constraint_flag);
                /*  inter  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_ref_pic_resampling_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_res_change_in_clvs_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_weighted_prediction_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_ref_wraparound_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_temporal_mvp_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_sbtmvp_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_amvr_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_bdof_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_smvd_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_dmvr_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_mmvd_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_affine_motion_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_prof_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_bcw_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_ciip_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_gpm_constraint_flag);
                /*  transform, quantization, residual  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_luma_transform_size_64_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_transform_skip_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_bdpcm_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_mts_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_lfnst_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_joint_cbcr_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_sbt_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_act_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_explicit_scaling_list_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_dep_quant_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_sign_data_hiding_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_cu_qp_delta_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_chroma_qp_offset_constraint_flag);
                /*  loop filter  */

                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_sao_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_alf_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_ccalf_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_lmcs_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_ladf_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.gci_no_virtual_boundaries_constraint_flag);
                size += stream.ReadUnsignedInt(size, 8, out this.gci_num_reserved_bits);

                this.gci_reserved_zero_bit = new byte[gci_num_reserved_bits];
                for (i = 0; i < gci_num_reserved_bits; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.gci_reserved_zero_bit[i]);
                }
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadFixed(size, 1, whileIndex, this.gci_alignment_zero_bit);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(1, this.gci_present_flag);

            if (gci_present_flag != 0)
            {
                /*  general  */

                size += stream.WriteUnsignedInt(1, this.gci_intra_only_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_all_layers_independent_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_one_au_only_constraint_flag);
                /*  picture format  */

                size += stream.WriteUnsignedInt(4, this.gci_sixteen_minus_max_bitdepth_constraint_idc);
                size += stream.WriteUnsignedInt(2, this.gci_three_minus_max_chroma_format_constraint_idc);
                /*  NAL unit type related  */

                size += stream.WriteUnsignedInt(1, this.gci_no_mixed_nalu_types_in_pic_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_trail_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_stsa_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_rasl_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_radl_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_idr_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_cra_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_gdr_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_aps_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_idr_rpl_constraint_flag);
                /*  tile, slice, subpicture partitioning  */

                size += stream.WriteUnsignedInt(1, this.gci_one_tile_per_pic_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_pic_header_in_slice_header_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_one_slice_per_pic_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_rectangular_slice_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_one_slice_per_subpic_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_subpic_info_constraint_flag);
                /*  CTU and block partitioning  */

                size += stream.WriteUnsignedInt(2, this.gci_three_minus_max_log2_ctu_size_constraint_idc);
                size += stream.WriteUnsignedInt(1, this.gci_no_partition_constraints_override_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_mtt_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_qtbtt_dual_tree_intra_constraint_flag);
                /*  intra  */

                size += stream.WriteUnsignedInt(1, this.gci_no_palette_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_ibc_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_isp_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_mrl_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_mip_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_cclm_constraint_flag);
                /*  inter  */

                size += stream.WriteUnsignedInt(1, this.gci_no_ref_pic_resampling_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_res_change_in_clvs_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_weighted_prediction_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_ref_wraparound_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_temporal_mvp_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_sbtmvp_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_amvr_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_bdof_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_smvd_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_dmvr_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_mmvd_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_affine_motion_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_prof_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_bcw_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_ciip_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_gpm_constraint_flag);
                /*  transform, quantization, residual  */

                size += stream.WriteUnsignedInt(1, this.gci_no_luma_transform_size_64_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_transform_skip_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_bdpcm_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_mts_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_lfnst_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_joint_cbcr_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_sbt_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_act_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_explicit_scaling_list_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_dep_quant_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_sign_data_hiding_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_cu_qp_delta_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_chroma_qp_offset_constraint_flag);
                /*  loop filter  */

                size += stream.WriteUnsignedInt(1, this.gci_no_sao_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_alf_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_ccalf_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_lmcs_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_ladf_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.gci_no_virtual_boundaries_constraint_flag);
                size += stream.WriteUnsignedInt(8, this.gci_num_reserved_bits);

                for (i = 0; i < gci_num_reserved_bits; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.gci_reserved_zero_bit[i]);
                }
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteFixed(1, whileIndex, this.gci_alignment_zero_bit);
            }

            return size;
        }

    }

    /*
 

dpb_parameters( MaxSubLayersMinus1, subLayerInfoFlag ) {  
 for( i = ( subLayerInfoFlag != 0 ? 0 : MaxSubLayersMinus1 ); 
   i  <=  MaxSubLayersMinus1; i++ ) { 
   dpb_max_dec_pic_buffering_minus1[ i ] ue(v) 
  dpb_max_num_reorder_pics[ i ] ue(v) 
  dpb_max_latency_increase_plus1[ i ] ue(v) 
 }  
}
    */
    public class DpbParameters : IItuSerializable
    {
        private uint maxSubLayersMinus1;
        public uint MaxSubLayersMinus1 { get { return maxSubLayersMinus1; } set { maxSubLayersMinus1 = value; } }
        private uint subLayerInfoFlag;
        public uint SubLayerInfoFlag { get { return subLayerInfoFlag; } set { subLayerInfoFlag = value; } }
        private uint[] dpb_max_dec_pic_buffering_minus1;
        public uint[] DpbMaxDecPicBufferingMinus1 { get { return dpb_max_dec_pic_buffering_minus1; } set { dpb_max_dec_pic_buffering_minus1 = value; } }
        private uint[] dpb_max_num_reorder_pics;
        public uint[] DpbMaxNumReorderPics { get { return dpb_max_num_reorder_pics; } set { dpb_max_num_reorder_pics = value; } }
        private uint[] dpb_max_latency_increase_plus1;
        public uint[] DpbMaxLatencyIncreasePlus1 { get { return dpb_max_latency_increase_plus1; } set { dpb_max_latency_increase_plus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DpbParameters(uint MaxSubLayersMinus1, uint subLayerInfoFlag)
        {
            this.maxSubLayersMinus1 = MaxSubLayersMinus1;
            this.subLayerInfoFlag = subLayerInfoFlag;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            this.dpb_max_dec_pic_buffering_minus1 = new uint[MaxSubLayersMinus1 + 1];
            this.dpb_max_num_reorder_pics = new uint[MaxSubLayersMinus1 + 1];
            this.dpb_max_latency_increase_plus1 = new uint[MaxSubLayersMinus1 + 1];
            for (i = (subLayerInfoFlag != 0 ? 0 : MaxSubLayersMinus1);
   i <= MaxSubLayersMinus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.dpb_max_dec_pic_buffering_minus1[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.dpb_max_num_reorder_pics[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.dpb_max_latency_increase_plus1[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            for (i = (subLayerInfoFlag != 0 ? 0 : MaxSubLayersMinus1);
   i <= MaxSubLayersMinus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.dpb_max_dec_pic_buffering_minus1[i]);
                size += stream.WriteUnsignedIntGolomb(this.dpb_max_num_reorder_pics[i]);
                size += stream.WriteUnsignedIntGolomb(this.dpb_max_latency_increase_plus1[i]);
            }

            return size;
        }

    }

    /*
  

general_timing_hrd_parameters() {  
 num_units_in_tick u(32) 
 time_scale u(32) 
 general_nal_hrd_params_present_flag u(1) 
 general_vcl_hrd_params_present_flag u(1) 
 if( general_nal_hrd_params_present_flag  ||  general_vcl_hrd_params_present_flag ) {  
  general_same_pic_timing_in_all_ols_flag u(1) 
  general_du_hrd_params_present_flag u(1) 
  if( general_du_hrd_params_present_flag )  
   tick_divisor_minus2 u(8) 
  bit_rate_scale u(4) 
  cpb_size_scale u(4) 
  if( general_du_hrd_params_present_flag )  
   cpb_size_du_scale u(4) 
  hrd_cpb_cnt_minus1 ue(v) 
 }  
}
    */
    public class GeneralTimingHrdParameters : IItuSerializable
    {
        private uint num_units_in_tick;
        public uint NumUnitsInTick { get { return num_units_in_tick; } set { num_units_in_tick = value; } }
        private uint time_scale;
        public uint TimeScale { get { return time_scale; } set { time_scale = value; } }
        private byte general_nal_hrd_params_present_flag;
        public byte GeneralNalHrdParamsPresentFlag { get { return general_nal_hrd_params_present_flag; } set { general_nal_hrd_params_present_flag = value; } }
        private byte general_vcl_hrd_params_present_flag;
        public byte GeneralVclHrdParamsPresentFlag { get { return general_vcl_hrd_params_present_flag; } set { general_vcl_hrd_params_present_flag = value; } }
        private byte general_same_pic_timing_in_all_ols_flag;
        public byte GeneralSamePicTimingInAllOlsFlag { get { return general_same_pic_timing_in_all_ols_flag; } set { general_same_pic_timing_in_all_ols_flag = value; } }
        private byte general_du_hrd_params_present_flag;
        public byte GeneralDuHrdParamsPresentFlag { get { return general_du_hrd_params_present_flag; } set { general_du_hrd_params_present_flag = value; } }
        private uint tick_divisor_minus2;
        public uint TickDivisorMinus2 { get { return tick_divisor_minus2; } set { tick_divisor_minus2 = value; } }
        private uint bit_rate_scale;
        public uint BitRateScale { get { return bit_rate_scale; } set { bit_rate_scale = value; } }
        private uint cpb_size_scale;
        public uint CpbSizeScale { get { return cpb_size_scale; } set { cpb_size_scale = value; } }
        private uint cpb_size_du_scale;
        public uint CpbSizeDuScale { get { return cpb_size_du_scale; } set { cpb_size_du_scale = value; } }
        private uint hrd_cpb_cnt_minus1;
        public uint HrdCpbCntMinus1 { get { return hrd_cpb_cnt_minus1; } set { hrd_cpb_cnt_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public GeneralTimingHrdParameters()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 32, out this.num_units_in_tick);
            size += stream.ReadUnsignedInt(size, 32, out this.time_scale);
            size += stream.ReadUnsignedInt(size, 1, out this.general_nal_hrd_params_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.general_vcl_hrd_params_present_flag);

            if (general_nal_hrd_params_present_flag != 0 || general_vcl_hrd_params_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.general_same_pic_timing_in_all_ols_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.general_du_hrd_params_present_flag);

                if (general_du_hrd_params_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.tick_divisor_minus2);
                }
                size += stream.ReadUnsignedInt(size, 4, out this.bit_rate_scale);
                size += stream.ReadUnsignedInt(size, 4, out this.cpb_size_scale);

                if (general_du_hrd_params_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.cpb_size_du_scale);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.hrd_cpb_cnt_minus1);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(32, this.num_units_in_tick);
            size += stream.WriteUnsignedInt(32, this.time_scale);
            size += stream.WriteUnsignedInt(1, this.general_nal_hrd_params_present_flag);
            size += stream.WriteUnsignedInt(1, this.general_vcl_hrd_params_present_flag);

            if (general_nal_hrd_params_present_flag != 0 || general_vcl_hrd_params_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.general_same_pic_timing_in_all_ols_flag);
                size += stream.WriteUnsignedInt(1, this.general_du_hrd_params_present_flag);

                if (general_du_hrd_params_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(8, this.tick_divisor_minus2);
                }
                size += stream.WriteUnsignedInt(4, this.bit_rate_scale);
                size += stream.WriteUnsignedInt(4, this.cpb_size_scale);

                if (general_du_hrd_params_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(4, this.cpb_size_du_scale);
                }
                size += stream.WriteUnsignedIntGolomb(this.hrd_cpb_cnt_minus1);
            }

            return size;
        }

    }

    /*
  

ols_timing_hrd_parameters( firstSubLayer, MaxSubLayersVal ) {  
 for( i = firstSubLayer; i  <=  MaxSubLayersVal; i++ ) {  
  fixed_pic_rate_general_flag[ i ] u(1) 
  if( !fixed_pic_rate_general_flag[ i ] )  
   fixed_pic_rate_within_cvs_flag[ i ] u(1) 
  if( fixed_pic_rate_within_cvs_flag[ i ] )  
   elemental_duration_in_tc_minus1[ i ] ue(v) 
  else if( ( general_nal_hrd_params_present_flag  || 
    general_vcl_hrd_params_present_flag )  &&  hrd_cpb_cnt_minus1  ==  0 ) 
 
   low_delay_hrd_flag[ i ] u(1) 
  if( general_nal_hrd_params_present_flag )  
   sublayer_hrd_parameters( i )  
  if( general_vcl_hrd_params_present_flag )  
   sublayer_hrd_parameters( i )  
 }  
}
    */
    public class OlsTimingHrdParameters : IItuSerializable
    {
        private uint firstSubLayer;
        public uint FirstSubLayer { get { return firstSubLayer; } set { firstSubLayer = value; } }
        private uint maxSubLayersVal;
        public uint MaxSubLayersVal { get { return maxSubLayersVal; } set { maxSubLayersVal = value; } }
        private byte[] fixed_pic_rate_general_flag;
        public byte[] FixedPicRateGeneralFlag { get { return fixed_pic_rate_general_flag; } set { fixed_pic_rate_general_flag = value; } }
        private byte[] fixed_pic_rate_within_cvs_flag;
        public byte[] FixedPicRateWithinCvsFlag { get { return fixed_pic_rate_within_cvs_flag; } set { fixed_pic_rate_within_cvs_flag = value; } }
        private uint[] elemental_duration_in_tc_minus1;
        public uint[] ElementalDurationInTcMinus1 { get { return elemental_duration_in_tc_minus1; } set { elemental_duration_in_tc_minus1 = value; } }
        private byte[] low_delay_hrd_flag;
        public byte[] LowDelayHrdFlag { get { return low_delay_hrd_flag; } set { low_delay_hrd_flag = value; } }
        private SublayerHrdParameters sublayer_hrd_parameters;
        public SublayerHrdParameters SublayerHrdParameters { get { return sublayer_hrd_parameters; } set { sublayer_hrd_parameters = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public OlsTimingHrdParameters(uint firstSubLayer, uint MaxSubLayersVal)
        {
            this.firstSubLayer = firstSubLayer;
            this.maxSubLayersVal = MaxSubLayersVal;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            this.fixed_pic_rate_general_flag = new byte[MaxSubLayersVal + 1];
            this.fixed_pic_rate_within_cvs_flag = new byte[MaxSubLayersVal + 1];
            this.elemental_duration_in_tc_minus1 = new uint[MaxSubLayersVal + 1];
            this.low_delay_hrd_flag = new byte[MaxSubLayersVal + 1];
            if (((H266Context)context).cbr_flag == null)
                ((H266Context)context).cbr_flag = new byte[MaxSubLayersVal + 1][];
            if (((H266Context)context).bit_rate_du_value_minus1 == null)
                ((H266Context)context).bit_rate_du_value_minus1 = new uint[MaxSubLayersVal + 1][];
            if (((H266Context)context).cpb_size_du_value_minus1 == null)
                ((H266Context)context).cpb_size_du_value_minus1 = new uint[MaxSubLayersVal + 1][];
            if (((H266Context)context).bit_rate_value_minus1 == null)
                ((H266Context)context).bit_rate_value_minus1 = new uint[MaxSubLayersVal + 1][];
            if (((H266Context)context).cpb_size_value_minus1 == null)
                ((H266Context)context).cpb_size_value_minus1 = new uint[MaxSubLayersVal + 1][];

            for (i = firstSubLayer; i <= MaxSubLayersVal; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.fixed_pic_rate_general_flag[i]);

                if (fixed_pic_rate_general_flag[i] == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.fixed_pic_rate_within_cvs_flag[i]);
                }

                if (fixed_pic_rate_within_cvs_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.elemental_duration_in_tc_minus1[i]);
                }
                else if ((((H266Context)context).GeneralTimingHrdParameters.GeneralNalHrdParamsPresentFlag != 0 ||
    ((H266Context)context).GeneralTimingHrdParameters.GeneralVclHrdParamsPresentFlag != 0) && ((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.low_delay_hrd_flag[i]);
                }

                if (((H266Context)context).GeneralTimingHrdParameters.GeneralNalHrdParamsPresentFlag != 0)
                {
                    this.sublayer_hrd_parameters = new SublayerHrdParameters(i);
                    size += stream.ReadClass<SublayerHrdParameters>(size, context, this.sublayer_hrd_parameters);
                }

                if (((H266Context)context).GeneralTimingHrdParameters.GeneralVclHrdParamsPresentFlag != 0)
                {
                    this.sublayer_hrd_parameters = new SublayerHrdParameters(i);
                    size += stream.ReadClass<SublayerHrdParameters>(size, context, this.sublayer_hrd_parameters);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            for (i = firstSubLayer; i <= MaxSubLayersVal; i++)
            {
                size += stream.WriteUnsignedInt(1, this.fixed_pic_rate_general_flag[i]);

                if (fixed_pic_rate_general_flag[i] == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.fixed_pic_rate_within_cvs_flag[i]);
                }

                if (fixed_pic_rate_within_cvs_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.elemental_duration_in_tc_minus1[i]);
                }
                else if ((((H266Context)context).GeneralTimingHrdParameters.GeneralNalHrdParamsPresentFlag != 0 ||
    ((H266Context)context).GeneralTimingHrdParameters.GeneralVclHrdParamsPresentFlag != 0) && ((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.low_delay_hrd_flag[i]);
                }

                if (((H266Context)context).GeneralTimingHrdParameters.GeneralNalHrdParamsPresentFlag != 0)
                {
                    sublayer_hrd_parameters.SubLayerId = i;
                    size += stream.WriteClass<SublayerHrdParameters>(context, this.sublayer_hrd_parameters);
                }

                if (((H266Context)context).GeneralTimingHrdParameters.GeneralVclHrdParamsPresentFlag != 0)
                {
                    sublayer_hrd_parameters.SubLayerId = i;
                    size += stream.WriteClass<SublayerHrdParameters>(context, this.sublayer_hrd_parameters);
                }
            }

            return size;
        }

    }

    /*
  

sublayer_hrd_parameters( subLayerId ) {  
 for( j = 0; j  <=  hrd_cpb_cnt_minus1; j++ ) {  
  bit_rate_value_minus1[ subLayerId ][ j ] ue(v) 
  cpb_size_value_minus1[ subLayerId ][ j ] ue(v) 
  if( general_du_hrd_params_present_flag ) {  
   cpb_size_du_value_minus1[ subLayerId ][ j ] ue(v) 
   bit_rate_du_value_minus1[ subLayerId ][ j ] ue(v) 
  }  
  cbr_flag[ subLayerId ][ j ] u(1) 
 }  
}
    */
    public class SublayerHrdParameters : IItuSerializable
    {
        private uint subLayerId;
        public uint SubLayerId { get { return subLayerId; } set { subLayerId = value; } }
        private uint[][] bit_rate_value_minus1;
        public uint[][] BitRateValueMinus1 { get { return bit_rate_value_minus1; } set { bit_rate_value_minus1 = value; } }
        private uint[][] cpb_size_value_minus1;
        public uint[][] CpbSizeValueMinus1 { get { return cpb_size_value_minus1; } set { cpb_size_value_minus1 = value; } }
        private uint[][] cpb_size_du_value_minus1;
        public uint[][] CpbSizeDuValueMinus1 { get { return cpb_size_du_value_minus1; } set { cpb_size_du_value_minus1 = value; } }
        private uint[][] bit_rate_du_value_minus1;
        public uint[][] BitRateDuValueMinus1 { get { return bit_rate_du_value_minus1; } set { bit_rate_du_value_minus1 = value; } }
        private byte[][] cbr_flag;
        public byte[][] CbrFlag { get { return cbr_flag; } set { cbr_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SublayerHrdParameters(uint subLayerId)
        {
            this.subLayerId = subLayerId;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint j = 0;
            ((H266Context)context).cbr_flag[subLayerId] = new byte[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];
            ((H266Context)context).bit_rate_du_value_minus1[subLayerId] = new uint[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];
            ((H266Context)context).cpb_size_du_value_minus1[subLayerId] = new uint[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];
            ((H266Context)context).bit_rate_value_minus1[subLayerId] = new uint[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];
            ((H266Context)context).cpb_size_value_minus1[subLayerId] = new uint[((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1 + 1];
            this.bit_rate_value_minus1 = ((H266Context)context).bit_rate_value_minus1;
            this.cpb_size_value_minus1 = ((H266Context)context).cpb_size_value_minus1;
            this.cpb_size_du_value_minus1 = ((H266Context)context).cpb_size_du_value_minus1;
            this.bit_rate_du_value_minus1 = ((H266Context)context).bit_rate_du_value_minus1;
            this.cbr_flag = ((H266Context)context).cbr_flag;

            for (j = 0; j <= ((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1; j++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_rate_value_minus1[subLayerId][j]);
                size += stream.ReadUnsignedIntGolomb(size, out this.cpb_size_value_minus1[subLayerId][j]);

                if (((H266Context)context).GeneralTimingHrdParameters.GeneralDuHrdParamsPresentFlag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.cpb_size_du_value_minus1[subLayerId][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.bit_rate_du_value_minus1[subLayerId][j]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.cbr_flag[subLayerId][j]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint j = 0;

            for (j = 0; j <= ((H266Context)context).GeneralTimingHrdParameters.HrdCpbCntMinus1; j++)
            {
                size += stream.WriteUnsignedIntGolomb(this.bit_rate_value_minus1[subLayerId][j]);
                size += stream.WriteUnsignedIntGolomb(this.cpb_size_value_minus1[subLayerId][j]);

                if (((H266Context)context).GeneralTimingHrdParameters.GeneralDuHrdParamsPresentFlag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.cpb_size_du_value_minus1[subLayerId][j]);
                    size += stream.WriteUnsignedIntGolomb(this.bit_rate_du_value_minus1[subLayerId][j]);
                }
                size += stream.WriteUnsignedInt(1, this.cbr_flag[subLayerId][j]);
            }

            return size;
        }

    }

    /*
  

sei_message() {  
 payloadType = 0  
 do {  
  payload_type_byte u(8) 
  payloadType  +=  payload_type_byte  
 } while( payload_type_byte  ==  0xFF )  
 payloadSize = 0  
 do {  
  payload_size_byte u(8) 
  payloadSize  +=  payload_size_byte  
 } while( payload_size_byte  ==  0xFF )  
 sei_payload( payloadType, payloadSize )  
}
    */
    public class SeiMessage : IItuSerializable
    {
        private Dictionary<int, uint> payload_type_byte = new Dictionary<int, uint>();
        public Dictionary<int, uint> PayloadTypeByte { get { return payload_type_byte; } set { payload_type_byte = value; } }
        private Dictionary<int, uint> payload_size_byte = new Dictionary<int, uint>();
        public Dictionary<int, uint> PayloadSizeByte { get { return payload_size_byte; } set { payload_size_byte = value; } }
        private SeiPayload sei_payload;
        public SeiPayload SeiPayload { get { return sei_payload; } set { sei_payload = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiMessage()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint payloadType = 0;
            int whileIndex = -1;
            uint payloadSize = 0;
            payloadType = 0;

            do
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 8, whileIndex, this.payload_type_byte);
                payloadType += payload_type_byte[whileIndex];
            } while (payload_type_byte[whileIndex] == 0xFF);
            payloadSize = 0;

            do
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 8, whileIndex, this.payload_size_byte);
                payloadSize += payload_size_byte[whileIndex];
            } while (payload_size_byte[whileIndex] == 0xFF);
            stream.MarkCurrentBitsPosition();
            this.sei_payload = new SeiPayload(payloadType, payloadSize);
            size += stream.ReadClass<SeiPayload>(size, context, this.sei_payload);
            ((H266Context)context).SetSeiPayload(sei_payload);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint payloadType = 0;
            int whileIndex = -1;
            uint payloadSize = 0;
            payloadType = 0;

            do
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(8, whileIndex, this.payload_type_byte);
                payloadType += payload_type_byte[whileIndex];
            } while (payload_type_byte[whileIndex] == 0xFF);
            payloadSize = 0;

            do
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(8, whileIndex, this.payload_size_byte);
                payloadSize += payload_size_byte[whileIndex];
            } while (payload_size_byte[whileIndex] == 0xFF);
            stream.MarkCurrentBitsPosition();
            size += stream.WriteClass<SeiPayload>(context, this.sei_payload);
            ((H266Context)context).SetSeiPayload(sei_payload);

            return size;
        }

    }

    /*
  

ref_pic_list_struct( listIdx, rplsIdx ) {  
 num_ref_entries[ listIdx ][ rplsIdx ] ue(v) 
 if( sps_long_term_ref_pics_flag  &&  rplsIdx < sps_num_ref_pic_lists[ listIdx ]  && 
   num_ref_entries[ listIdx ][ rplsIdx ] > 0 ) 
 
  ltrp_in_header_flag[ listIdx ][ rplsIdx ] u(1) 
 for( i = 0, j = 0; i < num_ref_entries[ listIdx ][ rplsIdx ]; i++) {  
  if( sps_inter_layer_prediction_enabled_flag )  
   inter_layer_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] u(1) 
  if( !inter_layer_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] ) {  
   if( sps_long_term_ref_pics_flag )  
          st_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] u(1) 
   else
      st_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] = 1 /* LukasV added default *//*
   if( st_ref_pic_flag[ listIdx ][ rplsIdx ][ i ] ) {  
    abs_delta_poc_st[ listIdx ][ rplsIdx ][ i ] ue(v) 
    if( AbsDeltaPocSt[ listIdx ][ rplsIdx ][ i ] > 0 )  
     strp_entry_sign_flag[ listIdx ][ rplsIdx ][ i ] u(1) 
   } else if( !ltrp_in_header_flag[ listIdx ][ rplsIdx ] )  
    rpls_poc_lsb_lt[ listIdx ][ rplsIdx ][ j++ ] u(v) 
      } else  
   ilrp_idx[ listIdx ][ rplsIdx ][ i ] ue(v) 
 }  
}
    */
    public class RefPicListStruct : IItuSerializable
    {
        private uint listIdx;
        public uint ListIdx { get { return listIdx; } set { listIdx = value; } }
        private uint rplsIdx;
        public uint RplsIdx { get { return rplsIdx; } set { rplsIdx = value; } }
        private uint[][] num_ref_entries;
        public uint[][] NumRefEntries { get { return num_ref_entries; } set { num_ref_entries = value; } }
        private byte[][] ltrp_in_header_flag;
        public byte[][] LtrpInHeaderFlag { get { return ltrp_in_header_flag; } set { ltrp_in_header_flag = value; } }
        private byte[][][] inter_layer_ref_pic_flag;
        public byte[][][] InterLayerRefPicFlag { get { return inter_layer_ref_pic_flag; } set { inter_layer_ref_pic_flag = value; } }
        private byte[][][] st_ref_pic_flag;
        public byte[][][] StRefPicFlag { get { return st_ref_pic_flag; } set { st_ref_pic_flag = value; } }
        private uint[][][] abs_delta_poc_st;
        public uint[][][] AbsDeltaPocSt { get { return abs_delta_poc_st; } set { abs_delta_poc_st = value; } }
        private byte[][][] strp_entry_sign_flag;
        public byte[][][] StrpEntrySignFlag { get { return strp_entry_sign_flag; } set { strp_entry_sign_flag = value; } }
        private uint[][][] rpls_poc_lsb_lt;
        public uint[][][] RplsPocLsbLt { get { return rpls_poc_lsb_lt; } set { rpls_poc_lsb_lt = value; } }
        private uint[][][] ilrp_idx;
        public uint[][][] IlrpIdx { get { return ilrp_idx; } set { ilrp_idx = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RefPicListStruct(uint listIdx, uint rplsIdx)
        {
            this.listIdx = listIdx;
            this.rplsIdx = rplsIdx;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;

            this.num_ref_entries = ((H266Context)context).num_ref_entries;
            this.inter_layer_ref_pic_flag = ((H266Context)context).inter_layer_ref_pic_flag;
            this.st_ref_pic_flag = ((H266Context)context).st_ref_pic_flag;
            this.abs_delta_poc_st = ((H266Context)context).abs_delta_poc_st;
            this.strp_entry_sign_flag = ((H266Context)context).strp_entry_sign_flag;
            this.rpls_poc_lsb_lt = ((H266Context)context).rpls_poc_lsb_lt;
            this.ilrp_idx = ((H266Context)context).ilrp_idx;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_entries[listIdx][rplsIdx]);
            ((H266Context)context).inter_layer_ref_pic_flag[listIdx][rplsIdx] = new byte[this.num_ref_entries[listIdx][rplsIdx]];
            ((H266Context)context).st_ref_pic_flag[listIdx][rplsIdx] = new byte[this.num_ref_entries[listIdx][rplsIdx]];
            ((H266Context)context).abs_delta_poc_st[listIdx][rplsIdx] = new uint[this.num_ref_entries[listIdx][rplsIdx]];
            ((H266Context)context).strp_entry_sign_flag[listIdx][rplsIdx] = new byte[this.num_ref_entries[listIdx][rplsIdx]];
            ((H266Context)context).rpls_poc_lsb_lt[listIdx][rplsIdx] = new uint[this.num_ref_entries[listIdx][rplsIdx]];
            ((H266Context)context).ilrp_idx[listIdx][rplsIdx] = new uint[this.num_ref_entries[listIdx][rplsIdx]];

            if (((H266Context)context).SeqParameterSetRbsp.SpsLongTermRefPicsFlag != 0 && rplsIdx < ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[listIdx] &&
   num_ref_entries[listIdx][rplsIdx] > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ltrp_in_header_flag[listIdx][rplsIdx]);
            }

            for (i = 0, j = 0; i < num_ref_entries[listIdx][rplsIdx]; i++)
            {

                if (((H266Context)context).SeqParameterSetRbsp.SpsInterLayerPredictionEnabledFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.inter_layer_ref_pic_flag[listIdx][rplsIdx][i]);
                }

                if (inter_layer_ref_pic_flag[listIdx][rplsIdx][i] == 0)
                {

                    if (((H266Context)context).SeqParameterSetRbsp.SpsLongTermRefPicsFlag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.st_ref_pic_flag[listIdx][rplsIdx][i]);
                        ((H266Context)context).OnStRefPicFlag(listIdx, rplsIdx, this);
                    }
                    else
                    {
                        st_ref_pic_flag[listIdx][rplsIdx][i] = 1 /* LukasV added default */;
                        ((H266Context)context).OnStRefPicFlag(listIdx, rplsIdx, this);
                    }

                    if (st_ref_pic_flag[listIdx][rplsIdx][i] != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.abs_delta_poc_st[listIdx][rplsIdx][i]);
                        ((H266Context)context).OnAbsDeltaPocSt(listIdx, rplsIdx, i, this);

                        if (((H266Context)context).AbsDeltaPocSt[listIdx][rplsIdx][i] > 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.strp_entry_sign_flag[listIdx][rplsIdx][i]);
                        }
                    }
                    else if (ltrp_in_header_flag[listIdx][rplsIdx] == 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4, out this.rpls_poc_lsb_lt[listIdx][rplsIdx][j++]);
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ilrp_idx[listIdx][rplsIdx][i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_ref_entries[listIdx][rplsIdx]);

            if (((H266Context)context).SeqParameterSetRbsp.SpsLongTermRefPicsFlag != 0 && rplsIdx < ((H266Context)context).SeqParameterSetRbsp.SpsNumRefPicLists[listIdx] &&
   num_ref_entries[listIdx][rplsIdx] > 0)
            {
                size += stream.WriteUnsignedInt(1, this.ltrp_in_header_flag[listIdx][rplsIdx]);
            }

            for (i = 0, j = 0; i < num_ref_entries[listIdx][rplsIdx]; i++)
            {

                if (((H266Context)context).SeqParameterSetRbsp.SpsInterLayerPredictionEnabledFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.inter_layer_ref_pic_flag[listIdx][rplsIdx][i]);
                }

                if (inter_layer_ref_pic_flag[listIdx][rplsIdx][i] == 0)
                {

                    if (((H266Context)context).SeqParameterSetRbsp.SpsLongTermRefPicsFlag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.st_ref_pic_flag[listIdx][rplsIdx][i]);
                        ((H266Context)context).OnStRefPicFlag(listIdx, rplsIdx, this);
                    }
                    else
                    {
                        st_ref_pic_flag[listIdx][rplsIdx][i] = 1 /* LukasV added default */;
                        ((H266Context)context).OnStRefPicFlag(listIdx, rplsIdx, this);
                    }

                    if (st_ref_pic_flag[listIdx][rplsIdx][i] != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.abs_delta_poc_st[listIdx][rplsIdx][i]);
                        ((H266Context)context).OnAbsDeltaPocSt(listIdx, rplsIdx, i, this);

                        if (((H266Context)context).AbsDeltaPocSt[listIdx][rplsIdx][i] > 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.strp_entry_sign_flag[listIdx][rplsIdx][i]);
                        }
                    }
                    else if (ltrp_in_header_flag[listIdx][rplsIdx] == 0)
                    {
                        size += stream.WriteUnsignedIntVariable(((H266Context)context).SeqParameterSetRbsp.SpsLog2MaxPicOrderCntLsbMinus4 + 4, this.rpls_poc_lsb_lt[listIdx][rplsIdx][j++]);
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntGolomb(this.ilrp_idx[listIdx][rplsIdx][i]);
                }
            }

            return size;
        }

    }

    /*
  

sei_payload( payloadType, payloadSize ) {  
 if( nal_unit_type  ==  PREFIX_SEI_NUT )  
  if( payloadType  ==  0 )  
   buffering_period( payloadSize )  
  else if( payloadType  ==  1 )  
   pic_timing( payloadSize )  
  else if( payloadType  ==  3 )  
   filler_payload( payloadSize ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
  else if( payloadType  ==  4 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   user_data_registered_itu_t_t35( payloadSize )  
  else if( payloadType  ==  5 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   user_data_unregistered( payloadSize )  
  else if( payloadType  ==  19 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   film_grain_characteristics( payloadSize )  
  else if( payloadType  ==  45 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   frame_packing_arrangement( payloadSize )  
  else if( payloadType  ==  129 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   parameter_sets_inclusion_indication( payloadSize )  
  else if( payloadType  ==  130 )  
   decoding_unit_info( payloadSize )  
  else if( payloadType  ==  133 )  
   scalable_nesting( payloadSize )  
  else if( payloadType  ==  137 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   mastering_display_colour_volume( payloadSize )  
  else if( payloadType  ==  144 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   content_light_level_info( payloadSize )  
  else if( payloadType  ==  145 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   dependent_rap_indication( payloadSize )  
  else if( payloadType  ==  147 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   alternative_transfer_characteristics( payloadSize )  
  else if( payloadType  ==  148 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   ambient_viewing_environment( payloadSize )  
  else if( payloadType  ==  149 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   content_colour_volume( payloadSize )  
  else if( payloadType  ==  150 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   equirectangular_projection( payloadSize )  
  else if( payloadType  ==  153 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   generalized_cubemap_projection( payloadSize )  
  else if( payloadType  ==  154 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   sphere_rotation( payloadSize )  
  else if( payloadType  ==  155 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   regionwise_packing( payloadSize )  
  else if( payloadType  ==  156 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   omni_viewport( payloadSize )  
  else if( payloadType  ==  168 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   frame_field_info( payloadSize )  
  else if( payloadType  ==  203 )  
   subpic_level_info( payloadSize )  
  else if( payloadType  ==  204 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   sample_aspect_ratio_info( payloadSize )  
  else                                             /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   reserved_message( payloadSize )  
 else /* nal_unit_type  ==  SUFFIX_SEI_NUT *//*  
  if( payloadType  ==  3 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   filler_payload( payloadSize )  
  else if( payloadType  ==  132 ) /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   decoded_picture_hash( payloadSize )  
  else if( payloadType  ==  133 )  
   scalable_nesting( payloadSize )  
  else                                     /* Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 *//*  
   reserved_message( payloadSize )  
 if( more_data_in_payload() ) {  
  if( payload_extension_present() )  
   sei_reserved_payload_extension_data u(v) 
  sei_payload_bit_equal_to_one /* equal to 1 *//* f(1) 
  while( !byte_aligned() )  
   sei_payload_bit_equal_to_zero /* equal to 0 *//* f(1) 
 }  
}
    */
    public class SeiPayload : IItuSerializable
    {
        private uint payloadType;
        public uint PayloadType { get { return payloadType; } set { payloadType = value; } }
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private BufferingPeriod buffering_period;
        public BufferingPeriod BufferingPeriod { get { return buffering_period; } set { buffering_period = value; } }
        private PicTiming pic_timing;
        public PicTiming PicTiming { get { return pic_timing; } set { pic_timing = value; } }
        private FillerPayload filler_payload;
        public FillerPayload FillerPayload { get { return filler_payload; } set { filler_payload = value; } }
        private UserDataRegisteredItutT35 user_data_registered_itu_t_t35;
        public UserDataRegisteredItutT35 UserDataRegisteredItutT35 { get { return user_data_registered_itu_t_t35; } set { user_data_registered_itu_t_t35 = value; } }
        private UserDataUnregistered user_data_unregistered;
        public UserDataUnregistered UserDataUnregistered { get { return user_data_unregistered; } set { user_data_unregistered = value; } }
        private FilmGrainCharacteristics film_grain_characteristics;
        public FilmGrainCharacteristics FilmGrainCharacteristics { get { return film_grain_characteristics; } set { film_grain_characteristics = value; } }
        private FramePackingArrangement frame_packing_arrangement;
        public FramePackingArrangement FramePackingArrangement { get { return frame_packing_arrangement; } set { frame_packing_arrangement = value; } }
        private ParameterSetsInclusionIndication parameter_sets_inclusion_indication;
        public ParameterSetsInclusionIndication ParameterSetsInclusionIndication { get { return parameter_sets_inclusion_indication; } set { parameter_sets_inclusion_indication = value; } }
        private DecodingUnitInfo decoding_unit_info;
        public DecodingUnitInfo DecodingUnitInfo { get { return decoding_unit_info; } set { decoding_unit_info = value; } }
        private ScalableNesting scalable_nesting;
        public ScalableNesting ScalableNesting { get { return scalable_nesting; } set { scalable_nesting = value; } }
        private MasteringDisplayColourVolume mastering_display_colour_volume;
        public MasteringDisplayColourVolume MasteringDisplayColourVolume { get { return mastering_display_colour_volume; } set { mastering_display_colour_volume = value; } }
        private ContentLightLevelInfo content_light_level_info;
        public ContentLightLevelInfo ContentLightLevelInfo { get { return content_light_level_info; } set { content_light_level_info = value; } }
        private DependentRapIndication dependent_rap_indication;
        public DependentRapIndication DependentRapIndication { get { return dependent_rap_indication; } set { dependent_rap_indication = value; } }
        private AlternativeTransferCharacteristics alternative_transfer_characteristics;
        public AlternativeTransferCharacteristics AlternativeTransferCharacteristics { get { return alternative_transfer_characteristics; } set { alternative_transfer_characteristics = value; } }
        private AmbientViewingEnvironment ambient_viewing_environment;
        public AmbientViewingEnvironment AmbientViewingEnvironment { get { return ambient_viewing_environment; } set { ambient_viewing_environment = value; } }
        private ContentColourVolume content_colour_volume;
        public ContentColourVolume ContentColourVolume { get { return content_colour_volume; } set { content_colour_volume = value; } }
        private EquirectangularProjection equirectangular_projection;
        public EquirectangularProjection EquirectangularProjection { get { return equirectangular_projection; } set { equirectangular_projection = value; } }
        private GeneralizedCubemapProjection generalized_cubemap_projection;
        public GeneralizedCubemapProjection GeneralizedCubemapProjection { get { return generalized_cubemap_projection; } set { generalized_cubemap_projection = value; } }
        private SphereRotation sphere_rotation;
        public SphereRotation SphereRotation { get { return sphere_rotation; } set { sphere_rotation = value; } }
        private RegionwisePacking regionwise_packing;
        public RegionwisePacking RegionwisePacking { get { return regionwise_packing; } set { regionwise_packing = value; } }
        private OmniViewport omni_viewport;
        public OmniViewport OmniViewport { get { return omni_viewport; } set { omni_viewport = value; } }
        private FrameFieldInfo frame_field_info;
        public FrameFieldInfo FrameFieldInfo { get { return frame_field_info; } set { frame_field_info = value; } }
        private SubpicLevelInfo subpic_level_info;
        public SubpicLevelInfo SubpicLevelInfo { get { return subpic_level_info; } set { subpic_level_info = value; } }
        private SampleAspectRatioInfo sample_aspect_ratio_info;
        public SampleAspectRatioInfo SampleAspectRatioInfo { get { return sample_aspect_ratio_info; } set { sample_aspect_ratio_info = value; } }
        private ReservedMessage reserved_message;
        public ReservedMessage ReservedMessage { get { return reserved_message; } set { reserved_message = value; } }
        private DecodedPictureHash decoded_picture_hash;
        public DecodedPictureHash DecodedPictureHash { get { return decoded_picture_hash; } set { decoded_picture_hash = value; } }
        private uint sei_reserved_payload_extension_data;
        public uint SeiReservedPayloadExtensionData { get { return sei_reserved_payload_extension_data; } set { sei_reserved_payload_extension_data = value; } }
        private uint sei_payload_bit_equal_to_one;
        public uint SeiPayloadBitEqualToOne { get { return sei_payload_bit_equal_to_one; } set { sei_payload_bit_equal_to_one = value; } }
        private Dictionary<int, uint> sei_payload_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> SeiPayloadBitEqualToZero { get { return sei_payload_bit_equal_to_zero; } set { sei_payload_bit_equal_to_zero = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SeiPayload(uint payloadType, uint payloadSize)
        {
            this.payloadType = payloadType;
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            if (((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.PREFIX_SEI_NUT)
            {

                if (payloadType == 0)
                {
                    this.buffering_period = new BufferingPeriod(payloadSize);
                    size += stream.ReadClass<BufferingPeriod>(size, context, this.buffering_period);
                }
                else if (payloadType == 1)
                {
                    this.pic_timing = new PicTiming(payloadSize);
                    size += stream.ReadClass<PicTiming>(size, context, this.pic_timing);
                }
                else if (payloadType == 3)
                {
                    this.filler_payload = new FillerPayload(payloadSize);
                    size += stream.ReadClass<FillerPayload>(size, context, this.filler_payload); // Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 
                }
                else if (payloadType == 4)
                {
                    this.user_data_registered_itu_t_t35 = new UserDataRegisteredItutT35(payloadSize);
                    size += stream.ReadClass<UserDataRegisteredItutT35>(size, context, this.user_data_registered_itu_t_t35);
                }
                else if (payloadType == 5)
                {
                    this.user_data_unregistered = new UserDataUnregistered(payloadSize);
                    size += stream.ReadClass<UserDataUnregistered>(size, context, this.user_data_unregistered);
                }
                else if (payloadType == 19)
                {
                    this.film_grain_characteristics = new FilmGrainCharacteristics(payloadSize);
                    size += stream.ReadClass<FilmGrainCharacteristics>(size, context, this.film_grain_characteristics);
                }
                else if (payloadType == 45)
                {
                    this.frame_packing_arrangement = new FramePackingArrangement(payloadSize);
                    size += stream.ReadClass<FramePackingArrangement>(size, context, this.frame_packing_arrangement);
                }
                else if (payloadType == 129)
                {
                    this.parameter_sets_inclusion_indication = new ParameterSetsInclusionIndication(payloadSize);
                    size += stream.ReadClass<ParameterSetsInclusionIndication>(size, context, this.parameter_sets_inclusion_indication);
                }
                else if (payloadType == 130)
                {
                    this.decoding_unit_info = new DecodingUnitInfo(payloadSize);
                    size += stream.ReadClass<DecodingUnitInfo>(size, context, this.decoding_unit_info);
                }
                else if (payloadType == 133)
                {
                    this.scalable_nesting = new ScalableNesting(payloadSize);
                    size += stream.ReadClass<ScalableNesting>(size, context, this.scalable_nesting);
                }
                else if (payloadType == 137)
                {
                    this.mastering_display_colour_volume = new MasteringDisplayColourVolume(payloadSize);
                    size += stream.ReadClass<MasteringDisplayColourVolume>(size, context, this.mastering_display_colour_volume);
                }
                else if (payloadType == 144)
                {
                    this.content_light_level_info = new ContentLightLevelInfo(payloadSize);
                    size += stream.ReadClass<ContentLightLevelInfo>(size, context, this.content_light_level_info);
                }
                else if (payloadType == 145)
                {
                    this.dependent_rap_indication = new DependentRapIndication(payloadSize);
                    size += stream.ReadClass<DependentRapIndication>(size, context, this.dependent_rap_indication);
                }
                else if (payloadType == 147)
                {
                    this.alternative_transfer_characteristics = new AlternativeTransferCharacteristics(payloadSize);
                    size += stream.ReadClass<AlternativeTransferCharacteristics>(size, context, this.alternative_transfer_characteristics);
                }
                else if (payloadType == 148)
                {
                    this.ambient_viewing_environment = new AmbientViewingEnvironment(payloadSize);
                    size += stream.ReadClass<AmbientViewingEnvironment>(size, context, this.ambient_viewing_environment);
                }
                else if (payloadType == 149)
                {
                    this.content_colour_volume = new ContentColourVolume(payloadSize);
                    size += stream.ReadClass<ContentColourVolume>(size, context, this.content_colour_volume);
                }
                else if (payloadType == 150)
                {
                    this.equirectangular_projection = new EquirectangularProjection(payloadSize);
                    size += stream.ReadClass<EquirectangularProjection>(size, context, this.equirectangular_projection);
                }
                else if (payloadType == 153)
                {
                    this.generalized_cubemap_projection = new GeneralizedCubemapProjection(payloadSize);
                    size += stream.ReadClass<GeneralizedCubemapProjection>(size, context, this.generalized_cubemap_projection);
                }
                else if (payloadType == 154)
                {
                    this.sphere_rotation = new SphereRotation(payloadSize);
                    size += stream.ReadClass<SphereRotation>(size, context, this.sphere_rotation);
                }
                else if (payloadType == 155)
                {
                    this.regionwise_packing = new RegionwisePacking(payloadSize);
                    size += stream.ReadClass<RegionwisePacking>(size, context, this.regionwise_packing);
                }
                else if (payloadType == 156)
                {
                    this.omni_viewport = new OmniViewport(payloadSize);
                    size += stream.ReadClass<OmniViewport>(size, context, this.omni_viewport);
                }
                else if (payloadType == 168)
                {
                    this.frame_field_info = new FrameFieldInfo(payloadSize);
                    size += stream.ReadClass<FrameFieldInfo>(size, context, this.frame_field_info);
                }
                else if (payloadType == 203)
                {
                    this.subpic_level_info = new SubpicLevelInfo(payloadSize);
                    size += stream.ReadClass<SubpicLevelInfo>(size, context, this.subpic_level_info);
                }
                else if (payloadType == 204)
                {
                    this.sample_aspect_ratio_info = new SampleAspectRatioInfo(payloadSize);
                    size += stream.ReadClass<SampleAspectRatioInfo>(size, context, this.sample_aspect_ratio_info);
                }
                else
                {
                    this.reserved_message = new ReservedMessage(payloadSize);
                    size += stream.ReadClass<ReservedMessage>(size, context, this.reserved_message);
                }
            }
            else
            {

                if (payloadType == 3)
                {
                    this.filler_payload = new FillerPayload(payloadSize);
                    size += stream.ReadClass<FillerPayload>(size, context, this.filler_payload);
                }
                else if (payloadType == 132)
                {
                    this.decoded_picture_hash = new DecodedPictureHash(payloadSize);
                    size += stream.ReadClass<DecodedPictureHash>(size, context, this.decoded_picture_hash);
                }
                else if (payloadType == 133)
                {
                    this.scalable_nesting = new ScalableNesting(payloadSize);
                    size += stream.ReadClass<ScalableNesting>(size, context, this.scalable_nesting);
                }
                else
                {
                    this.reserved_message = new ReservedMessage(payloadSize);
                    size += stream.ReadClass<ReservedMessage>(size, context, this.reserved_message);
                }
            }

            if ((!(stream.ByteAligned() && 8 * payloadSize == stream.GetBitsPositionSinceLastMark())))
            {

                if (stream.ReadMoreRbspData(this, payloadSize))
                {
                    size += stream.ReadUnsignedIntVariable(size, 0 /* TODO */, out this.sei_reserved_payload_extension_data);
                }
                size += stream.ReadFixed(size, 1, out this.sei_payload_bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadFixed(size, 1, whileIndex, this.sei_payload_bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            if (((H266Context)context).NalHeader.NalUnitHeader.NalUnitType == H266NALTypes.PREFIX_SEI_NUT)
            {

                if (payloadType == 0)
                {
                    size += stream.WriteClass<BufferingPeriod>(context, this.buffering_period);
                }
                else if (payloadType == 1)
                {
                    size += stream.WriteClass<PicTiming>(context, this.pic_timing);
                }
                else if (payloadType == 3)
                {
                    size += stream.WriteClass<FillerPayload>(context, this.filler_payload); // Specified in Rec. ITU-T H.274 | ISO/IEC 23002-7 
                }
                else if (payloadType == 4)
                {
                    size += stream.WriteClass<UserDataRegisteredItutT35>(context, this.user_data_registered_itu_t_t35);
                }
                else if (payloadType == 5)
                {
                    size += stream.WriteClass<UserDataUnregistered>(context, this.user_data_unregistered);
                }
                else if (payloadType == 19)
                {
                    size += stream.WriteClass<FilmGrainCharacteristics>(context, this.film_grain_characteristics);
                }
                else if (payloadType == 45)
                {
                    size += stream.WriteClass<FramePackingArrangement>(context, this.frame_packing_arrangement);
                }
                else if (payloadType == 129)
                {
                    size += stream.WriteClass<ParameterSetsInclusionIndication>(context, this.parameter_sets_inclusion_indication);
                }
                else if (payloadType == 130)
                {
                    size += stream.WriteClass<DecodingUnitInfo>(context, this.decoding_unit_info);
                }
                else if (payloadType == 133)
                {
                    size += stream.WriteClass<ScalableNesting>(context, this.scalable_nesting);
                }
                else if (payloadType == 137)
                {
                    size += stream.WriteClass<MasteringDisplayColourVolume>(context, this.mastering_display_colour_volume);
                }
                else if (payloadType == 144)
                {
                    size += stream.WriteClass<ContentLightLevelInfo>(context, this.content_light_level_info);
                }
                else if (payloadType == 145)
                {
                    size += stream.WriteClass<DependentRapIndication>(context, this.dependent_rap_indication);
                }
                else if (payloadType == 147)
                {
                    size += stream.WriteClass<AlternativeTransferCharacteristics>(context, this.alternative_transfer_characteristics);
                }
                else if (payloadType == 148)
                {
                    size += stream.WriteClass<AmbientViewingEnvironment>(context, this.ambient_viewing_environment);
                }
                else if (payloadType == 149)
                {
                    size += stream.WriteClass<ContentColourVolume>(context, this.content_colour_volume);
                }
                else if (payloadType == 150)
                {
                    size += stream.WriteClass<EquirectangularProjection>(context, this.equirectangular_projection);
                }
                else if (payloadType == 153)
                {
                    size += stream.WriteClass<GeneralizedCubemapProjection>(context, this.generalized_cubemap_projection);
                }
                else if (payloadType == 154)
                {
                    size += stream.WriteClass<SphereRotation>(context, this.sphere_rotation);
                }
                else if (payloadType == 155)
                {
                    size += stream.WriteClass<RegionwisePacking>(context, this.regionwise_packing);
                }
                else if (payloadType == 156)
                {
                    size += stream.WriteClass<OmniViewport>(context, this.omni_viewport);
                }
                else if (payloadType == 168)
                {
                    size += stream.WriteClass<FrameFieldInfo>(context, this.frame_field_info);
                }
                else if (payloadType == 203)
                {
                    size += stream.WriteClass<SubpicLevelInfo>(context, this.subpic_level_info);
                }
                else if (payloadType == 204)
                {
                    size += stream.WriteClass<SampleAspectRatioInfo>(context, this.sample_aspect_ratio_info);
                }
                else
                {
                    size += stream.WriteClass<ReservedMessage>(context, this.reserved_message);
                }
            }
            else
            {

                if (payloadType == 3)
                {
                    size += stream.WriteClass<FillerPayload>(context, this.filler_payload);
                }
                else if (payloadType == 132)
                {
                    size += stream.WriteClass<DecodedPictureHash>(context, this.decoded_picture_hash);
                }
                else if (payloadType == 133)
                {
                    size += stream.WriteClass<ScalableNesting>(context, this.scalable_nesting);
                }
                else
                {
                    size += stream.WriteClass<ReservedMessage>(context, this.reserved_message);
                }
            }

            if ((!(stream.ByteAligned() && 8 * payloadSize == stream.GetBitsPositionSinceLastMark())))
            {

                if (stream.ReadMoreRbspData(this, payloadSize))
                {
                    size += stream.WriteUnsignedIntVariable(0 /* TODO */, this.sei_reserved_payload_extension_data);
                }
                size += stream.WriteFixed(1, this.sei_payload_bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteFixed(1, whileIndex, this.sei_payload_bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

    }

    /*
 

filler_payload(payloadSize) {
    for (k = 0; k < payloadSize; k++)
        ff_byte  /* equal to 0xFF *//* f(8)
}
    */
    public class FillerPayload : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] ff_byte;
        public uint[] FfByte { get { return ff_byte; } set { ff_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FillerPayload(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;

            this.ff_byte = new uint[payloadSize];
            for (k = 0; k < payloadSize; k++)
            {
                size += stream.ReadFixed(size, 8, out this.ff_byte[k]); // equal to 0xFF 
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;

            for (k = 0; k < payloadSize; k++)
            {
                size += stream.WriteFixed(8, this.ff_byte[k]); // equal to 0xFF 
            }

            return size;
        }

    }

    /*


user_data_registered_itu_t_t35(payloadSize) {
    itu_t_t35_country_code b(8)
    if (itu_t_t35_country_code != 0xFF)
        i = 1
    else {        
        itu_t_t35_country_code_extension_byte b(8)
        i = 2
    }
    do {
        itu_t_t35_payload_byte b(8)
        i++
    } while (i < payloadSize)
}
    */
    public class UserDataRegisteredItutT35 : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte itu_t_t35_country_code;
        public byte ItutT35CountryCode { get { return itu_t_t35_country_code; } set { itu_t_t35_country_code = value; } }
        private byte itu_t_t35_country_code_extension_byte;
        public byte ItutT35CountryCodeExtensionByte { get { return itu_t_t35_country_code_extension_byte; } set { itu_t_t35_country_code_extension_byte = value; } }
        private Dictionary<int, byte> itu_t_t35_payload_byte = new Dictionary<int, byte>();
        public Dictionary<int, byte> ItutT35PayloadByte { get { return itu_t_t35_payload_byte; } set { itu_t_t35_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public UserDataRegisteredItutT35(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.ReadBits(size, 8, out this.itu_t_t35_country_code);

            if (itu_t_t35_country_code != 0xFF)
            {
                i = 1;
            }
            else
            {
                size += stream.ReadBits(size, 8, out this.itu_t_t35_country_code_extension_byte);
                i = 2;
            }

            do
            {
                whileIndex++;

                size += stream.ReadBits(size, 8, whileIndex, this.itu_t_t35_payload_byte);
                i++;
            } while (i < payloadSize);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.WriteBits(8, this.itu_t_t35_country_code);

            if (itu_t_t35_country_code != 0xFF)
            {
                i = 1;
            }
            else
            {
                size += stream.WriteBits(8, this.itu_t_t35_country_code_extension_byte);
                i = 2;
            }

            do
            {
                whileIndex++;

                size += stream.WriteBits(8, whileIndex, this.itu_t_t35_payload_byte);
                i++;
            } while (i < payloadSize);

            return size;
        }

    }

    /*


user_data_unregistered(payloadSize) {
    uuid_iso_iec_11578 u(128)
    for (i = 16; i < payloadSize; i++)  
        user_data_payload_byte b(8)
}
    */
    public class UserDataUnregistered : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private BigInteger uuid_iso_iec_11578;
        public BigInteger UuidIsoIec11578 { get { return uuid_iso_iec_11578; } set { uuid_iso_iec_11578 = value; } }
        private byte[] user_data_payload_byte;
        public byte[] UserDataPayloadByte { get { return user_data_payload_byte; } set { user_data_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public UserDataUnregistered(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 128, out this.uuid_iso_iec_11578);

            this.user_data_payload_byte = new byte[payloadSize];
            for (i = 16; i < payloadSize; i++)
            {
                size += stream.ReadBits(size, 8, out this.user_data_payload_byte[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(128, this.uuid_iso_iec_11578);

            for (i = 16; i < payloadSize; i++)
            {
                size += stream.WriteBits(8, this.user_data_payload_byte[i]);
            }

            return size;
        }

    }

    /*


film_grain_characteristics(payloadSize) {
 fg_characteristics_cancel_flag u(1)
    if (!fg_characteristics_cancel_flag) {  
  fg_model_id u(2) 
  fg_separate_colour_description_present_flag u(1)
        if (fg_separate_colour_description_present_flag) {  
   fg_bit_depth_luma_minus8 u(3) 
   fg_bit_depth_chroma_minus8 u(3) 
   fg_full_range_flag u(1) 
   fg_colour_primaries u(8) 
   fg_transfer_characteristics u(8) 
   fg_matrix_coeffs u(8)
        }  
  fg_blending_mode_id u(2) 
  fg_log2_scale_factor u(4)
        for (c = 0; c < 3; c++)
            fg_comp_model_present_flag[c] u(1)
        for (c = 0; c < 3; c++)
            if (fg_comp_model_present_flag[c]) {
                fg_num_intensity_intervals_minus1[c] u(8)
                fg_num_model_values_minus1[c] u(3)
                for (i = 0; i <= fg_num_intensity_intervals_minus1[c]; i++) {
                    fg_intensity_interval_lower_bound[c][i] u(8)
                    fg_intensity_interval_upper_bound[c][i] u(8)
                    for (j = 0; j <= fg_num_model_values_minus1[c]; j++)
                        fg_comp_model_value[c][i][j] se(v)
                }
            }  
  fg_characteristics_persistence_flag u(1)
    }
}
    */
    public class FilmGrainCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte fg_characteristics_cancel_flag;
        public byte FgCharacteristicsCancelFlag { get { return fg_characteristics_cancel_flag; } set { fg_characteristics_cancel_flag = value; } }
        private uint fg_model_id;
        public uint FgModelId { get { return fg_model_id; } set { fg_model_id = value; } }
        private byte fg_separate_colour_description_present_flag;
        public byte FgSeparateColourDescriptionPresentFlag { get { return fg_separate_colour_description_present_flag; } set { fg_separate_colour_description_present_flag = value; } }
        private uint fg_bit_depth_luma_minus8;
        public uint FgBitDepthLumaMinus8 { get { return fg_bit_depth_luma_minus8; } set { fg_bit_depth_luma_minus8 = value; } }
        private uint fg_bit_depth_chroma_minus8;
        public uint FgBitDepthChromaMinus8 { get { return fg_bit_depth_chroma_minus8; } set { fg_bit_depth_chroma_minus8 = value; } }
        private byte fg_full_range_flag;
        public byte FgFullRangeFlag { get { return fg_full_range_flag; } set { fg_full_range_flag = value; } }
        private uint fg_colour_primaries;
        public uint FgColourPrimaries { get { return fg_colour_primaries; } set { fg_colour_primaries = value; } }
        private uint fg_transfer_characteristics;
        public uint FgTransferCharacteristics { get { return fg_transfer_characteristics; } set { fg_transfer_characteristics = value; } }
        private uint fg_matrix_coeffs;
        public uint FgMatrixCoeffs { get { return fg_matrix_coeffs; } set { fg_matrix_coeffs = value; } }
        private uint fg_blending_mode_id;
        public uint FgBlendingModeId { get { return fg_blending_mode_id; } set { fg_blending_mode_id = value; } }
        private uint fg_log2_scale_factor;
        public uint FgLog2ScaleFactor { get { return fg_log2_scale_factor; } set { fg_log2_scale_factor = value; } }
        private byte[] fg_comp_model_present_flag;
        public byte[] FgCompModelPresentFlag { get { return fg_comp_model_present_flag; } set { fg_comp_model_present_flag = value; } }
        private uint[] fg_num_intensity_intervals_minus1;
        public uint[] FgNumIntensityIntervalsMinus1 { get { return fg_num_intensity_intervals_minus1; } set { fg_num_intensity_intervals_minus1 = value; } }
        private uint[] fg_num_model_values_minus1;
        public uint[] FgNumModelValuesMinus1 { get { return fg_num_model_values_minus1; } set { fg_num_model_values_minus1 = value; } }
        private uint[][] fg_intensity_interval_lower_bound;
        public uint[][] FgIntensityIntervalLowerBound { get { return fg_intensity_interval_lower_bound; } set { fg_intensity_interval_lower_bound = value; } }
        private uint[][] fg_intensity_interval_upper_bound;
        public uint[][] FgIntensityIntervalUpperBound { get { return fg_intensity_interval_upper_bound; } set { fg_intensity_interval_upper_bound = value; } }
        private int[][][] fg_comp_model_value;
        public int[][][] FgCompModelValue { get { return fg_comp_model_value; } set { fg_comp_model_value = value; } }
        private byte fg_characteristics_persistence_flag;
        public byte FgCharacteristicsPersistenceFlag { get { return fg_characteristics_persistence_flag; } set { fg_characteristics_persistence_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FilmGrainCharacteristics(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.fg_characteristics_cancel_flag);

            if (fg_characteristics_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.fg_model_id);
                size += stream.ReadUnsignedInt(size, 1, out this.fg_separate_colour_description_present_flag);

                if (fg_separate_colour_description_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.fg_bit_depth_luma_minus8);
                    size += stream.ReadUnsignedInt(size, 3, out this.fg_bit_depth_chroma_minus8);
                    size += stream.ReadUnsignedInt(size, 1, out this.fg_full_range_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.fg_colour_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.fg_transfer_characteristics);
                    size += stream.ReadUnsignedInt(size, 8, out this.fg_matrix_coeffs);
                }
                size += stream.ReadUnsignedInt(size, 2, out this.fg_blending_mode_id);
                size += stream.ReadUnsignedInt(size, 4, out this.fg_log2_scale_factor);

                this.fg_comp_model_present_flag = new byte[3];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.fg_comp_model_present_flag[c]);
                }

                this.fg_num_intensity_intervals_minus1 = new uint[3];
                this.fg_num_model_values_minus1 = new uint[3];
                this.fg_intensity_interval_lower_bound = new uint[3][];
                this.fg_intensity_interval_upper_bound = new uint[3][];
                this.fg_comp_model_value = new int[3][][];
                for (c = 0; c < 3; c++)
                {

                    if (fg_comp_model_present_flag[c] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.fg_num_intensity_intervals_minus1[c]);
                        size += stream.ReadUnsignedInt(size, 3, out this.fg_num_model_values_minus1[c]);

                        this.fg_intensity_interval_lower_bound[c] = new uint[fg_num_intensity_intervals_minus1[c] + 1];
                        this.fg_intensity_interval_upper_bound[c] = new uint[fg_num_intensity_intervals_minus1[c] + 1];
                        this.fg_comp_model_value[c] = new int[fg_num_intensity_intervals_minus1[c] + 1][];
                        for (i = 0; i <= fg_num_intensity_intervals_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedInt(size, 8, out this.fg_intensity_interval_lower_bound[c][i]);
                            size += stream.ReadUnsignedInt(size, 8, out this.fg_intensity_interval_upper_bound[c][i]);

                            this.fg_comp_model_value[c][i] = new int[fg_num_model_values_minus1[c] + 1];
                            for (j = 0; j <= fg_num_model_values_minus1[c]; j++)
                            {
                                size += stream.ReadSignedIntGolomb(size, out this.fg_comp_model_value[c][i][j]);
                            }
                        }
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.fg_characteristics_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.fg_characteristics_cancel_flag);

            if (fg_characteristics_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(2, this.fg_model_id);
                size += stream.WriteUnsignedInt(1, this.fg_separate_colour_description_present_flag);

                if (fg_separate_colour_description_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.fg_bit_depth_luma_minus8);
                    size += stream.WriteUnsignedInt(3, this.fg_bit_depth_chroma_minus8);
                    size += stream.WriteUnsignedInt(1, this.fg_full_range_flag);
                    size += stream.WriteUnsignedInt(8, this.fg_colour_primaries);
                    size += stream.WriteUnsignedInt(8, this.fg_transfer_characteristics);
                    size += stream.WriteUnsignedInt(8, this.fg_matrix_coeffs);
                }
                size += stream.WriteUnsignedInt(2, this.fg_blending_mode_id);
                size += stream.WriteUnsignedInt(4, this.fg_log2_scale_factor);

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(1, this.fg_comp_model_present_flag[c]);
                }

                for (c = 0; c < 3; c++)
                {

                    if (fg_comp_model_present_flag[c] != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.fg_num_intensity_intervals_minus1[c]);
                        size += stream.WriteUnsignedInt(3, this.fg_num_model_values_minus1[c]);

                        for (i = 0; i <= fg_num_intensity_intervals_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedInt(8, this.fg_intensity_interval_lower_bound[c][i]);
                            size += stream.WriteUnsignedInt(8, this.fg_intensity_interval_upper_bound[c][i]);

                            for (j = 0; j <= fg_num_model_values_minus1[c]; j++)
                            {
                                size += stream.WriteSignedIntGolomb(this.fg_comp_model_value[c][i][j]);
                            }
                        }
                    }
                }
                size += stream.WriteUnsignedInt(1, this.fg_characteristics_persistence_flag);
            }

            return size;
        }

    }

    /*
 

frame_packing_arrangement(payloadSize) {
 fp_arrangement_id ue(v) 
 fp_arrangement_cancel_flag u(1)
    if (!fp_arrangement_cancel_flag) {  
  fp_arrangement_type u(7) 
  fp_quincunx_sampling_flag u(1) 
  fp_content_interpretation_type u(6) 
  fp_spatial_flipping_flag u(1) 
  fp_frame0_flipped_flag u(1) 
  fp_field_views_flag u(1) 
  fp_current_frame_is_frame0_flag u(1) 
  fp_frame0_self_contained_flag u(1) 
  fp_frame1_self_contained_flag u(1)
        if (!fp_quincunx_sampling_flag && fp_arrangement_type != 5) {  
   fp_frame0_grid_position_x u(4) 
   fp_frame0_grid_position_y u(4) 
   fp_frame1_grid_position_x u(4) 
   fp_frame1_grid_position_y u(4)
        }  
  fp_arrangement_reserved_byte u(8) 
  fp_arrangement_persistence_flag u(1)
    }  
 fp_upsampled_aspect_ratio_flag u(1)
}
    */
    public class FramePackingArrangement : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint fp_arrangement_id;
        public uint FpArrangementId { get { return fp_arrangement_id; } set { fp_arrangement_id = value; } }
        private byte fp_arrangement_cancel_flag;
        public byte FpArrangementCancelFlag { get { return fp_arrangement_cancel_flag; } set { fp_arrangement_cancel_flag = value; } }
        private uint fp_arrangement_type;
        public uint FpArrangementType { get { return fp_arrangement_type; } set { fp_arrangement_type = value; } }
        private byte fp_quincunx_sampling_flag;
        public byte FpQuincunxSamplingFlag { get { return fp_quincunx_sampling_flag; } set { fp_quincunx_sampling_flag = value; } }
        private uint fp_content_interpretation_type;
        public uint FpContentInterpretationType { get { return fp_content_interpretation_type; } set { fp_content_interpretation_type = value; } }
        private byte fp_spatial_flipping_flag;
        public byte FpSpatialFlippingFlag { get { return fp_spatial_flipping_flag; } set { fp_spatial_flipping_flag = value; } }
        private byte fp_frame0_flipped_flag;
        public byte FpFrame0FlippedFlag { get { return fp_frame0_flipped_flag; } set { fp_frame0_flipped_flag = value; } }
        private byte fp_field_views_flag;
        public byte FpFieldViewsFlag { get { return fp_field_views_flag; } set { fp_field_views_flag = value; } }
        private byte fp_current_frame_is_frame0_flag;
        public byte FpCurrentFrameIsFrame0Flag { get { return fp_current_frame_is_frame0_flag; } set { fp_current_frame_is_frame0_flag = value; } }
        private byte fp_frame0_self_contained_flag;
        public byte FpFrame0SelfContainedFlag { get { return fp_frame0_self_contained_flag; } set { fp_frame0_self_contained_flag = value; } }
        private byte fp_frame1_self_contained_flag;
        public byte FpFrame1SelfContainedFlag { get { return fp_frame1_self_contained_flag; } set { fp_frame1_self_contained_flag = value; } }
        private uint fp_frame0_grid_position_x;
        public uint FpFrame0GridPositionx { get { return fp_frame0_grid_position_x; } set { fp_frame0_grid_position_x = value; } }
        private uint fp_frame0_grid_position_y;
        public uint FpFrame0GridPositiony { get { return fp_frame0_grid_position_y; } set { fp_frame0_grid_position_y = value; } }
        private uint fp_frame1_grid_position_x;
        public uint FpFrame1GridPositionx { get { return fp_frame1_grid_position_x; } set { fp_frame1_grid_position_x = value; } }
        private uint fp_frame1_grid_position_y;
        public uint FpFrame1GridPositiony { get { return fp_frame1_grid_position_y; } set { fp_frame1_grid_position_y = value; } }
        private uint fp_arrangement_reserved_byte;
        public uint FpArrangementReservedByte { get { return fp_arrangement_reserved_byte; } set { fp_arrangement_reserved_byte = value; } }
        private byte fp_arrangement_persistence_flag;
        public byte FpArrangementPersistenceFlag { get { return fp_arrangement_persistence_flag; } set { fp_arrangement_persistence_flag = value; } }
        private byte fp_upsampled_aspect_ratio_flag;
        public byte FpUpsampledAspectRatioFlag { get { return fp_upsampled_aspect_ratio_flag; } set { fp_upsampled_aspect_ratio_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FramePackingArrangement(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.fp_arrangement_id);
            size += stream.ReadUnsignedInt(size, 1, out this.fp_arrangement_cancel_flag);

            if (fp_arrangement_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 7, out this.fp_arrangement_type);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_quincunx_sampling_flag);
                size += stream.ReadUnsignedInt(size, 6, out this.fp_content_interpretation_type);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_spatial_flipping_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_frame0_flipped_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_field_views_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_current_frame_is_frame0_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_frame0_self_contained_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_frame1_self_contained_flag);

                if (fp_quincunx_sampling_flag == 0 && fp_arrangement_type != 5)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.fp_frame0_grid_position_x);
                    size += stream.ReadUnsignedInt(size, 4, out this.fp_frame0_grid_position_y);
                    size += stream.ReadUnsignedInt(size, 4, out this.fp_frame1_grid_position_x);
                    size += stream.ReadUnsignedInt(size, 4, out this.fp_frame1_grid_position_y);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.fp_arrangement_reserved_byte);
                size += stream.ReadUnsignedInt(size, 1, out this.fp_arrangement_persistence_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.fp_upsampled_aspect_ratio_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.fp_arrangement_id);
            size += stream.WriteUnsignedInt(1, this.fp_arrangement_cancel_flag);

            if (fp_arrangement_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(7, this.fp_arrangement_type);
                size += stream.WriteUnsignedInt(1, this.fp_quincunx_sampling_flag);
                size += stream.WriteUnsignedInt(6, this.fp_content_interpretation_type);
                size += stream.WriteUnsignedInt(1, this.fp_spatial_flipping_flag);
                size += stream.WriteUnsignedInt(1, this.fp_frame0_flipped_flag);
                size += stream.WriteUnsignedInt(1, this.fp_field_views_flag);
                size += stream.WriteUnsignedInt(1, this.fp_current_frame_is_frame0_flag);
                size += stream.WriteUnsignedInt(1, this.fp_frame0_self_contained_flag);
                size += stream.WriteUnsignedInt(1, this.fp_frame1_self_contained_flag);

                if (fp_quincunx_sampling_flag == 0 && fp_arrangement_type != 5)
                {
                    size += stream.WriteUnsignedInt(4, this.fp_frame0_grid_position_x);
                    size += stream.WriteUnsignedInt(4, this.fp_frame0_grid_position_y);
                    size += stream.WriteUnsignedInt(4, this.fp_frame1_grid_position_x);
                    size += stream.WriteUnsignedInt(4, this.fp_frame1_grid_position_y);
                }
                size += stream.WriteUnsignedInt(8, this.fp_arrangement_reserved_byte);
                size += stream.WriteUnsignedInt(1, this.fp_arrangement_persistence_flag);
            }
            size += stream.WriteUnsignedInt(1, this.fp_upsampled_aspect_ratio_flag);

            return size;
        }

    }

    /*
  

mastering_display_colour_volume(payloadSize) {
    for (c = 0; c < 3; c++) {
        mdcv_display_primaries_x[c] u(16)
        mdcv_display_primaries_y[c] u(16)
    }
    mdcv_white_point_x u(16)
    mdcv_white_point_y u(16)
    mdcv_max_display_mastering_luminance u(32)
    mdcv_min_display_mastering_luminance u(32)
}
    */
    public class MasteringDisplayColourVolume : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] mdcv_display_primaries_x;
        public uint[] MdcvDisplayPrimariesx { get { return mdcv_display_primaries_x; } set { mdcv_display_primaries_x = value; } }
        private uint[] mdcv_display_primaries_y;
        public uint[] MdcvDisplayPrimariesy { get { return mdcv_display_primaries_y; } set { mdcv_display_primaries_y = value; } }
        private uint mdcv_white_point_x;
        public uint MdcvWhitePointx { get { return mdcv_white_point_x; } set { mdcv_white_point_x = value; } }
        private uint mdcv_white_point_y;
        public uint MdcvWhitePointy { get { return mdcv_white_point_y; } set { mdcv_white_point_y = value; } }
        private uint mdcv_max_display_mastering_luminance;
        public uint MdcvMaxDisplayMasteringLuminance { get { return mdcv_max_display_mastering_luminance; } set { mdcv_max_display_mastering_luminance = value; } }
        private uint mdcv_min_display_mastering_luminance;
        public uint MdcvMinDisplayMasteringLuminance { get { return mdcv_min_display_mastering_luminance; } set { mdcv_min_display_mastering_luminance = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MasteringDisplayColourVolume(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;

            this.mdcv_display_primaries_x = new uint[3];
            this.mdcv_display_primaries_y = new uint[3];
            for (c = 0; c < 3; c++)
            {
                size += stream.ReadUnsignedInt(size, 16, out this.mdcv_display_primaries_x[c]);
                size += stream.ReadUnsignedInt(size, 16, out this.mdcv_display_primaries_y[c]);
            }
            size += stream.ReadUnsignedInt(size, 16, out this.mdcv_white_point_x);
            size += stream.ReadUnsignedInt(size, 16, out this.mdcv_white_point_y);
            size += stream.ReadUnsignedInt(size, 32, out this.mdcv_max_display_mastering_luminance);
            size += stream.ReadUnsignedInt(size, 32, out this.mdcv_min_display_mastering_luminance);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;

            for (c = 0; c < 3; c++)
            {
                size += stream.WriteUnsignedInt(16, this.mdcv_display_primaries_x[c]);
                size += stream.WriteUnsignedInt(16, this.mdcv_display_primaries_y[c]);
            }
            size += stream.WriteUnsignedInt(16, this.mdcv_white_point_x);
            size += stream.WriteUnsignedInt(16, this.mdcv_white_point_y);
            size += stream.WriteUnsignedInt(32, this.mdcv_max_display_mastering_luminance);
            size += stream.WriteUnsignedInt(32, this.mdcv_min_display_mastering_luminance);

            return size;
        }

    }

    /*


content_light_level_info(payloadSize) {
    clli_max_content_light_level u(16)
    clli_max_pic_average_light_level u(16)
}
    */
    public class ContentLightLevelInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint clli_max_content_light_level;
        public uint ClliMaxContentLightLevel { get { return clli_max_content_light_level; } set { clli_max_content_light_level = value; } }
        private uint clli_max_pic_average_light_level;
        public uint ClliMaxPicAverageLightLevel { get { return clli_max_pic_average_light_level; } set { clli_max_pic_average_light_level = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ContentLightLevelInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 16, out this.clli_max_content_light_level);
            size += stream.ReadUnsignedInt(size, 16, out this.clli_max_pic_average_light_level);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(16, this.clli_max_content_light_level);
            size += stream.WriteUnsignedInt(16, this.clli_max_pic_average_light_level);

            return size;
        }

    }

    /*


dependent_rap_indication(payloadSize) { 
}
    */
    public class DependentRapIndication : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DependentRapIndication(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;


            return size;
        }

    }

    /*
 

alternative_transfer_characteristics(payloadSize) {
    preferred_transfer_characteristics u(8)
}
    */
    public class AlternativeTransferCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint preferred_transfer_characteristics;
        public uint PreferredTransferCharacteristics { get { return preferred_transfer_characteristics; } set { preferred_transfer_characteristics = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AlternativeTransferCharacteristics(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.preferred_transfer_characteristics);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.preferred_transfer_characteristics);

            return size;
        }

    }

    /*


ambient_viewing_environment(payloadSize) {
    ambient_illuminance u(32)  
    ambient_light_x u(16) 
    ambient_light_y u(16)
}
    */
    public class AmbientViewingEnvironment : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint ambient_illuminance;
        public uint AmbientIlluminance { get { return ambient_illuminance; } set { ambient_illuminance = value; } }
        private uint ambient_light_x;
        public uint AmbientLightx { get { return ambient_light_x; } set { ambient_light_x = value; } }
        private uint ambient_light_y;
        public uint AmbientLighty { get { return ambient_light_y; } set { ambient_light_y = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AmbientViewingEnvironment(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 32, out this.ambient_illuminance);
            size += stream.ReadUnsignedInt(size, 16, out this.ambient_light_x);
            size += stream.ReadUnsignedInt(size, 16, out this.ambient_light_y);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(32, this.ambient_illuminance);
            size += stream.WriteUnsignedInt(16, this.ambient_light_x);
            size += stream.WriteUnsignedInt(16, this.ambient_light_y);

            return size;
        }

    }

    /*


content_colour_volume(payloadSize) {
 ccv_cancel_flag u(1)
    if (!ccv_cancel_flag) {  
  ccv_persistence_flag u(1) 
  ccv_primaries_present_flag u(1) 
  ccv_min_luminance_value_present_flag u(1) 
  ccv_max_luminance_value_present_flag u(1) 
  ccv_avg_luminance_value_present_flag u(1) 
  ccv_reserved_zero_2bits u(2)
        if (ccv_primaries_present_flag)
            for (c = 0; c < 3; c++) {
                ccv_primaries_x[c] i(32)
                ccv_primaries_y[c] i(32)
            }
        if (ccv_min_luminance_value_present_flag)  
   ccv_min_luminance_value u(32)
        if (ccv_max_luminance_value_present_flag)  
   ccv_max_luminance_value u(32)
        if (ccv_avg_luminance_value_present_flag)  
   ccv_avg_luminance_value u(32)
    }
}
    */
    public class ContentColourVolume : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte ccv_cancel_flag;
        public byte CcvCancelFlag { get { return ccv_cancel_flag; } set { ccv_cancel_flag = value; } }
        private byte ccv_persistence_flag;
        public byte CcvPersistenceFlag { get { return ccv_persistence_flag; } set { ccv_persistence_flag = value; } }
        private byte ccv_primaries_present_flag;
        public byte CcvPrimariesPresentFlag { get { return ccv_primaries_present_flag; } set { ccv_primaries_present_flag = value; } }
        private byte ccv_min_luminance_value_present_flag;
        public byte CcvMinLuminanceValuePresentFlag { get { return ccv_min_luminance_value_present_flag; } set { ccv_min_luminance_value_present_flag = value; } }
        private byte ccv_max_luminance_value_present_flag;
        public byte CcvMaxLuminanceValuePresentFlag { get { return ccv_max_luminance_value_present_flag; } set { ccv_max_luminance_value_present_flag = value; } }
        private byte ccv_avg_luminance_value_present_flag;
        public byte CcvAvgLuminanceValuePresentFlag { get { return ccv_avg_luminance_value_present_flag; } set { ccv_avg_luminance_value_present_flag = value; } }
        private uint ccv_reserved_zero_2bits;
        public uint CcvReservedZero2bits { get { return ccv_reserved_zero_2bits; } set { ccv_reserved_zero_2bits = value; } }
        private int[] ccv_primaries_x;
        public int[] CcvPrimariesx { get { return ccv_primaries_x; } set { ccv_primaries_x = value; } }
        private int[] ccv_primaries_y;
        public int[] CcvPrimariesy { get { return ccv_primaries_y; } set { ccv_primaries_y = value; } }
        private uint ccv_min_luminance_value;
        public uint CcvMinLuminanceValue { get { return ccv_min_luminance_value; } set { ccv_min_luminance_value = value; } }
        private uint ccv_max_luminance_value;
        public uint CcvMaxLuminanceValue { get { return ccv_max_luminance_value; } set { ccv_max_luminance_value = value; } }
        private uint ccv_avg_luminance_value;
        public uint CcvAvgLuminanceValue { get { return ccv_avg_luminance_value; } set { ccv_avg_luminance_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ContentColourVolume(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.ccv_cancel_flag);

            if (ccv_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_primaries_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_min_luminance_value_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_max_luminance_value_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ccv_avg_luminance_value_present_flag);
                size += stream.ReadUnsignedInt(size, 2, out this.ccv_reserved_zero_2bits);

                if (ccv_primaries_present_flag != 0)
                {

                    this.ccv_primaries_x = new int[3];
                    this.ccv_primaries_y = new int[3];
                    for (c = 0; c < 3; c++)
                    {
                        size += stream.ReadSignedInt(size, 32, out this.ccv_primaries_x[c]);
                        size += stream.ReadSignedInt(size, 32, out this.ccv_primaries_y[c]);
                    }
                }

                if (ccv_min_luminance_value_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.ccv_min_luminance_value);
                }

                if (ccv_max_luminance_value_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.ccv_max_luminance_value);
                }

                if (ccv_avg_luminance_value_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.ccv_avg_luminance_value);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            size += stream.WriteUnsignedInt(1, this.ccv_cancel_flag);

            if (ccv_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.ccv_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_primaries_present_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_min_luminance_value_present_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_max_luminance_value_present_flag);
                size += stream.WriteUnsignedInt(1, this.ccv_avg_luminance_value_present_flag);
                size += stream.WriteUnsignedInt(2, this.ccv_reserved_zero_2bits);

                if (ccv_primaries_present_flag != 0)
                {

                    for (c = 0; c < 3; c++)
                    {
                        size += stream.WriteSignedInt(32, this.ccv_primaries_x[c]);
                        size += stream.WriteSignedInt(32, this.ccv_primaries_y[c]);
                    }
                }

                if (ccv_min_luminance_value_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.ccv_min_luminance_value);
                }

                if (ccv_max_luminance_value_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.ccv_max_luminance_value);
                }

                if (ccv_avg_luminance_value_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(32, this.ccv_avg_luminance_value);
                }
            }

            return size;
        }

    }

    /*
  

equirectangular_projection(payloadSize) {
    erp_cancel_flag u(1)
    if (!erp_cancel_flag) {
        erp_persistence_flag u(1)
        erp_guard_band_flag u(1)
        erp_reserved_zero_2bits u(2)
        if (erp_guard_band_flag == 1) {
            erp_guard_band_type u(3)
            erp_left_guard_band_width u(8)
            erp_right_guard_band_width u(8)
        }
    }
}
    */
    public class EquirectangularProjection : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte erp_cancel_flag;
        public byte ErpCancelFlag { get { return erp_cancel_flag; } set { erp_cancel_flag = value; } }
        private byte erp_persistence_flag;
        public byte ErpPersistenceFlag { get { return erp_persistence_flag; } set { erp_persistence_flag = value; } }
        private byte erp_guard_band_flag;
        public byte ErpGuardBandFlag { get { return erp_guard_band_flag; } set { erp_guard_band_flag = value; } }
        private uint erp_reserved_zero_2bits;
        public uint ErpReservedZero2bits { get { return erp_reserved_zero_2bits; } set { erp_reserved_zero_2bits = value; } }
        private uint erp_guard_band_type;
        public uint ErpGuardBandType { get { return erp_guard_band_type; } set { erp_guard_band_type = value; } }
        private uint erp_left_guard_band_width;
        public uint ErpLeftGuardBandWidth { get { return erp_left_guard_band_width; } set { erp_left_guard_band_width = value; } }
        private uint erp_right_guard_band_width;
        public uint ErpRightGuardBandWidth { get { return erp_right_guard_band_width; } set { erp_right_guard_band_width = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public EquirectangularProjection(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.erp_cancel_flag);

            if (erp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.erp_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.erp_guard_band_flag);
                size += stream.ReadUnsignedInt(size, 2, out this.erp_reserved_zero_2bits);

                if (erp_guard_band_flag == 1)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.erp_guard_band_type);
                    size += stream.ReadUnsignedInt(size, 8, out this.erp_left_guard_band_width);
                    size += stream.ReadUnsignedInt(size, 8, out this.erp_right_guard_band_width);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.erp_cancel_flag);

            if (erp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.erp_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.erp_guard_band_flag);
                size += stream.WriteUnsignedInt(2, this.erp_reserved_zero_2bits);

                if (erp_guard_band_flag == 1)
                {
                    size += stream.WriteUnsignedInt(3, this.erp_guard_band_type);
                    size += stream.WriteUnsignedInt(8, this.erp_left_guard_band_width);
                    size += stream.WriteUnsignedInt(8, this.erp_right_guard_band_width);
                }
            }

            return size;
        }

    }

    /*


generalized_cubemap_projection(payloadSize) {
    gcmp_cancel_flag u(1)
    if (!gcmp_cancel_flag) {       
        gcmp_persistence_flag u(1)
        gcmp_packing_type u(3) 
        gcmp_mapping_function_type u(2)
        for (i = 0; i < ((gcmp_packing_type == 4 || gcmp_packing_type  == 5) ? 5 : 6); i++ ) {

            gcmp_face_index[i] u(3)
            gcmp_face_rotation[i] u(2)
            if (gcmp_mapping_function_type == 2) {
                gcmp_function_coeff_u[i] u(7)
                gcmp_function_u_affected_by_v_flag[i] u(1)
                gcmp_function_coeff_v[i] u(7)
                gcmp_function_v_affected_by_u_flag[i] u(1)
            }
        }  
  gcmp_guard_band_flag u(1)
        if (gcmp_guard_band_flag) {  
   gcmp_guard_band_type u(3) 
   gcmp_guard_band_boundary_exterior_flag u(1) 
   gcmp_guard_band_samples_minus1 u(4)
        }
    }
}
    */
    public class GeneralizedCubemapProjection : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte gcmp_cancel_flag;
        public byte GcmpCancelFlag { get { return gcmp_cancel_flag; } set { gcmp_cancel_flag = value; } }
        private byte gcmp_persistence_flag;
        public byte GcmpPersistenceFlag { get { return gcmp_persistence_flag; } set { gcmp_persistence_flag = value; } }
        private uint gcmp_packing_type;
        public uint GcmpPackingType { get { return gcmp_packing_type; } set { gcmp_packing_type = value; } }
        private uint gcmp_mapping_function_type;
        public uint GcmpMappingFunctionType { get { return gcmp_mapping_function_type; } set { gcmp_mapping_function_type = value; } }
        private uint[] gcmp_face_index;
        public uint[] GcmpFaceIndex { get { return gcmp_face_index; } set { gcmp_face_index = value; } }
        private uint[] gcmp_face_rotation;
        public uint[] GcmpFaceRotation { get { return gcmp_face_rotation; } set { gcmp_face_rotation = value; } }
        private uint[] gcmp_function_coeff_u;
        public uint[] GcmpFunctionCoeffu { get { return gcmp_function_coeff_u; } set { gcmp_function_coeff_u = value; } }
        private byte[] gcmp_function_u_affected_by_v_flag;
        public byte[] GcmpFunctionuAffectedByvFlag { get { return gcmp_function_u_affected_by_v_flag; } set { gcmp_function_u_affected_by_v_flag = value; } }
        private uint[] gcmp_function_coeff_v;
        public uint[] GcmpFunctionCoeffv { get { return gcmp_function_coeff_v; } set { gcmp_function_coeff_v = value; } }
        private byte[] gcmp_function_v_affected_by_u_flag;
        public byte[] GcmpFunctionvAffectedByuFlag { get { return gcmp_function_v_affected_by_u_flag; } set { gcmp_function_v_affected_by_u_flag = value; } }
        private byte gcmp_guard_band_flag;
        public byte GcmpGuardBandFlag { get { return gcmp_guard_band_flag; } set { gcmp_guard_band_flag = value; } }
        private uint gcmp_guard_band_type;
        public uint GcmpGuardBandType { get { return gcmp_guard_band_type; } set { gcmp_guard_band_type = value; } }
        private byte gcmp_guard_band_boundary_exterior_flag;
        public byte GcmpGuardBandBoundaryExteriorFlag { get { return gcmp_guard_band_boundary_exterior_flag; } set { gcmp_guard_band_boundary_exterior_flag = value; } }
        private uint gcmp_guard_band_samples_minus1;
        public uint GcmpGuardBandSamplesMinus1 { get { return gcmp_guard_band_samples_minus1; } set { gcmp_guard_band_samples_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public GeneralizedCubemapProjection(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.gcmp_cancel_flag);

            if (gcmp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.gcmp_persistence_flag);
                size += stream.ReadUnsignedInt(size, 3, out this.gcmp_packing_type);
                size += stream.ReadUnsignedInt(size, 2, out this.gcmp_mapping_function_type);

                this.gcmp_face_index = new uint[((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6)];
                this.gcmp_face_rotation = new uint[((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6)];
                this.gcmp_function_coeff_u = new uint[((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6)];
                this.gcmp_function_u_affected_by_v_flag = new byte[((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6)];
                this.gcmp_function_coeff_v = new uint[((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6)];
                this.gcmp_function_v_affected_by_u_flag = new byte[((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6)];
                for (i = 0; i < ((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6); i++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.gcmp_face_index[i]);
                    size += stream.ReadUnsignedInt(size, 2, out this.gcmp_face_rotation[i]);

                    if (gcmp_mapping_function_type == 2)
                    {
                        size += stream.ReadUnsignedInt(size, 7, out this.gcmp_function_coeff_u[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.gcmp_function_u_affected_by_v_flag[i]);
                        size += stream.ReadUnsignedInt(size, 7, out this.gcmp_function_coeff_v[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.gcmp_function_v_affected_by_u_flag[i]);
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.gcmp_guard_band_flag);

                if (gcmp_guard_band_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.gcmp_guard_band_type);
                    size += stream.ReadUnsignedInt(size, 1, out this.gcmp_guard_band_boundary_exterior_flag);
                    size += stream.ReadUnsignedInt(size, 4, out this.gcmp_guard_band_samples_minus1);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.gcmp_cancel_flag);

            if (gcmp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.gcmp_persistence_flag);
                size += stream.WriteUnsignedInt(3, this.gcmp_packing_type);
                size += stream.WriteUnsignedInt(2, this.gcmp_mapping_function_type);

                for (i = 0; i < ((gcmp_packing_type == 4 || gcmp_packing_type == 5) ? 5 : 6); i++)
                {
                    size += stream.WriteUnsignedInt(3, this.gcmp_face_index[i]);
                    size += stream.WriteUnsignedInt(2, this.gcmp_face_rotation[i]);

                    if (gcmp_mapping_function_type == 2)
                    {
                        size += stream.WriteUnsignedInt(7, this.gcmp_function_coeff_u[i]);
                        size += stream.WriteUnsignedInt(1, this.gcmp_function_u_affected_by_v_flag[i]);
                        size += stream.WriteUnsignedInt(7, this.gcmp_function_coeff_v[i]);
                        size += stream.WriteUnsignedInt(1, this.gcmp_function_v_affected_by_u_flag[i]);
                    }
                }
                size += stream.WriteUnsignedInt(1, this.gcmp_guard_band_flag);

                if (gcmp_guard_band_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.gcmp_guard_band_type);
                    size += stream.WriteUnsignedInt(1, this.gcmp_guard_band_boundary_exterior_flag);
                    size += stream.WriteUnsignedInt(4, this.gcmp_guard_band_samples_minus1);
                }
            }

            return size;
        }

    }

    /*
  

regionwise_packing(payloadSize) {
    rwp_cancel_flag  u(1)
    if (!rwp_cancel_flag) {
        rwp_persistence_flag  u(1)
        rwp_constituent_picture_matching_flag  u(1)
        rwp_reserved_zero_5bits  u(5)
        rwp_num_packed_regions  u(8)
        rwp_proj_picture_width u(32)
        rwp_proj_picture_height u(32)
         rwp_packed_picture_width u(16) 
  rwp_packed_picture_height u(16)
        for (i = 0; i < rwp_num_packed_regions; i++) {
            rwp_reserved_zero_4bits[i] u(4)
            rwp_transform_type[i] u(3)
            rwp_guard_band_flag[i] u(1)
            rwp_proj_region_width[i] u(32)
            rwp_proj_region_height[i] u(32)
            rwp_proj_region_top[i] u(32)
            rwp_proj_region_left[i] u(32)
            rwp_packed_region_width[i] u(16)
            rwp_packed_region_height[i] u(16)
            rwp_packed_region_top[i] u(16)
            rwp_packed_region_left[i] u(16)
            if (rwp_guard_band_flag[i]) {
                rwp_left_guard_band_width[i] u(8)
                rwp_right_guard_band_width[i] u(8)
                rwp_top_guard_band_height[i] u(8)
                rwp_bottom_guard_band_height[i] u(8)
                rwp_guard_band_not_used_for_pred_flag[i] u(1)
                for (j = 0; j < 4; j++)
                    rwp_guard_band_type[i][j] u(3)
                rwp_guard_band_reserved_zero_3bits[i] u(3)
            }
        }
    }
}
    */
    public class RegionwisePacking : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte rwp_cancel_flag;
        public byte RwpCancelFlag { get { return rwp_cancel_flag; } set { rwp_cancel_flag = value; } }
        private byte rwp_persistence_flag;
        public byte RwpPersistenceFlag { get { return rwp_persistence_flag; } set { rwp_persistence_flag = value; } }
        private byte rwp_constituent_picture_matching_flag;
        public byte RwpConstituentPictureMatchingFlag { get { return rwp_constituent_picture_matching_flag; } set { rwp_constituent_picture_matching_flag = value; } }
        private uint rwp_reserved_zero_5bits;
        public uint RwpReservedZero5bits { get { return rwp_reserved_zero_5bits; } set { rwp_reserved_zero_5bits = value; } }
        private uint rwp_num_packed_regions;
        public uint RwpNumPackedRegions { get { return rwp_num_packed_regions; } set { rwp_num_packed_regions = value; } }
        private uint rwp_proj_picture_width;
        public uint RwpProjPictureWidth { get { return rwp_proj_picture_width; } set { rwp_proj_picture_width = value; } }
        private uint rwp_proj_picture_height;
        public uint RwpProjPictureHeight { get { return rwp_proj_picture_height; } set { rwp_proj_picture_height = value; } }
        private uint rwp_packed_picture_width;
        public uint RwpPackedPictureWidth { get { return rwp_packed_picture_width; } set { rwp_packed_picture_width = value; } }
        private uint rwp_packed_picture_height;
        public uint RwpPackedPictureHeight { get { return rwp_packed_picture_height; } set { rwp_packed_picture_height = value; } }
        private uint[] rwp_reserved_zero_4bits;
        public uint[] RwpReservedZero4bits { get { return rwp_reserved_zero_4bits; } set { rwp_reserved_zero_4bits = value; } }
        private uint[] rwp_transform_type;
        public uint[] RwpTransformType { get { return rwp_transform_type; } set { rwp_transform_type = value; } }
        private byte[] rwp_guard_band_flag;
        public byte[] RwpGuardBandFlag { get { return rwp_guard_band_flag; } set { rwp_guard_band_flag = value; } }
        private uint[] rwp_proj_region_width;
        public uint[] RwpProjRegionWidth { get { return rwp_proj_region_width; } set { rwp_proj_region_width = value; } }
        private uint[] rwp_proj_region_height;
        public uint[] RwpProjRegionHeight { get { return rwp_proj_region_height; } set { rwp_proj_region_height = value; } }
        private uint[] rwp_proj_region_top;
        public uint[] RwpProjRegionTop { get { return rwp_proj_region_top; } set { rwp_proj_region_top = value; } }
        private uint[] rwp_proj_region_left;
        public uint[] RwpProjRegionLeft { get { return rwp_proj_region_left; } set { rwp_proj_region_left = value; } }
        private uint[] rwp_packed_region_width;
        public uint[] RwpPackedRegionWidth { get { return rwp_packed_region_width; } set { rwp_packed_region_width = value; } }
        private uint[] rwp_packed_region_height;
        public uint[] RwpPackedRegionHeight { get { return rwp_packed_region_height; } set { rwp_packed_region_height = value; } }
        private uint[] rwp_packed_region_top;
        public uint[] RwpPackedRegionTop { get { return rwp_packed_region_top; } set { rwp_packed_region_top = value; } }
        private uint[] rwp_packed_region_left;
        public uint[] RwpPackedRegionLeft { get { return rwp_packed_region_left; } set { rwp_packed_region_left = value; } }
        private uint[] rwp_left_guard_band_width;
        public uint[] RwpLeftGuardBandWidth { get { return rwp_left_guard_band_width; } set { rwp_left_guard_band_width = value; } }
        private uint[] rwp_right_guard_band_width;
        public uint[] RwpRightGuardBandWidth { get { return rwp_right_guard_band_width; } set { rwp_right_guard_band_width = value; } }
        private uint[] rwp_top_guard_band_height;
        public uint[] RwpTopGuardBandHeight { get { return rwp_top_guard_band_height; } set { rwp_top_guard_band_height = value; } }
        private uint[] rwp_bottom_guard_band_height;
        public uint[] RwpBottomGuardBandHeight { get { return rwp_bottom_guard_band_height; } set { rwp_bottom_guard_band_height = value; } }
        private byte[] rwp_guard_band_not_used_for_pred_flag;
        public byte[] RwpGuardBandNotUsedForPredFlag { get { return rwp_guard_band_not_used_for_pred_flag; } set { rwp_guard_band_not_used_for_pred_flag = value; } }
        private uint[][] rwp_guard_band_type;
        public uint[][] RwpGuardBandType { get { return rwp_guard_band_type; } set { rwp_guard_band_type = value; } }
        private uint[] rwp_guard_band_reserved_zero_3bits;
        public uint[] RwpGuardBandReservedZero3bits { get { return rwp_guard_band_reserved_zero_3bits; } set { rwp_guard_band_reserved_zero_3bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RegionwisePacking(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.rwp_cancel_flag);

            if (rwp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.rwp_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.rwp_constituent_picture_matching_flag);
                size += stream.ReadUnsignedInt(size, 5, out this.rwp_reserved_zero_5bits);
                size += stream.ReadUnsignedInt(size, 8, out this.rwp_num_packed_regions);
                size += stream.ReadUnsignedInt(size, 32, out this.rwp_proj_picture_width);
                size += stream.ReadUnsignedInt(size, 32, out this.rwp_proj_picture_height);
                size += stream.ReadUnsignedInt(size, 16, out this.rwp_packed_picture_width);
                size += stream.ReadUnsignedInt(size, 16, out this.rwp_packed_picture_height);

                this.rwp_reserved_zero_4bits = new uint[rwp_num_packed_regions];
                this.rwp_transform_type = new uint[rwp_num_packed_regions];
                this.rwp_guard_band_flag = new byte[rwp_num_packed_regions];
                this.rwp_proj_region_width = new uint[rwp_num_packed_regions];
                this.rwp_proj_region_height = new uint[rwp_num_packed_regions];
                this.rwp_proj_region_top = new uint[rwp_num_packed_regions];
                this.rwp_proj_region_left = new uint[rwp_num_packed_regions];
                this.rwp_packed_region_width = new uint[rwp_num_packed_regions];
                this.rwp_packed_region_height = new uint[rwp_num_packed_regions];
                this.rwp_packed_region_top = new uint[rwp_num_packed_regions];
                this.rwp_packed_region_left = new uint[rwp_num_packed_regions];
                this.rwp_left_guard_band_width = new uint[rwp_num_packed_regions];
                this.rwp_right_guard_band_width = new uint[rwp_num_packed_regions];
                this.rwp_top_guard_band_height = new uint[rwp_num_packed_regions];
                this.rwp_bottom_guard_band_height = new uint[rwp_num_packed_regions];
                this.rwp_guard_band_not_used_for_pred_flag = new byte[rwp_num_packed_regions];
                this.rwp_guard_band_type = new uint[rwp_num_packed_regions][];
                this.rwp_guard_band_reserved_zero_3bits = new uint[rwp_num_packed_regions];
                for (i = 0; i < rwp_num_packed_regions; i++)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.rwp_reserved_zero_4bits[i]);
                    size += stream.ReadUnsignedInt(size, 3, out this.rwp_transform_type[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.rwp_guard_band_flag[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.rwp_proj_region_width[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.rwp_proj_region_height[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.rwp_proj_region_top[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.rwp_proj_region_left[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.rwp_packed_region_width[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.rwp_packed_region_height[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.rwp_packed_region_top[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.rwp_packed_region_left[i]);

                    if (rwp_guard_band_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.rwp_left_guard_band_width[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.rwp_right_guard_band_width[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.rwp_top_guard_band_height[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.rwp_bottom_guard_band_height[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.rwp_guard_band_not_used_for_pred_flag[i]);

                        this.rwp_guard_band_type[i] = new uint[4];
                        for (j = 0; j < 4; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.rwp_guard_band_type[i][j]);
                        }
                        size += stream.ReadUnsignedInt(size, 3, out this.rwp_guard_band_reserved_zero_3bits[i]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.rwp_cancel_flag);

            if (rwp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.rwp_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.rwp_constituent_picture_matching_flag);
                size += stream.WriteUnsignedInt(5, this.rwp_reserved_zero_5bits);
                size += stream.WriteUnsignedInt(8, this.rwp_num_packed_regions);
                size += stream.WriteUnsignedInt(32, this.rwp_proj_picture_width);
                size += stream.WriteUnsignedInt(32, this.rwp_proj_picture_height);
                size += stream.WriteUnsignedInt(16, this.rwp_packed_picture_width);
                size += stream.WriteUnsignedInt(16, this.rwp_packed_picture_height);

                for (i = 0; i < rwp_num_packed_regions; i++)
                {
                    size += stream.WriteUnsignedInt(4, this.rwp_reserved_zero_4bits[i]);
                    size += stream.WriteUnsignedInt(3, this.rwp_transform_type[i]);
                    size += stream.WriteUnsignedInt(1, this.rwp_guard_band_flag[i]);
                    size += stream.WriteUnsignedInt(32, this.rwp_proj_region_width[i]);
                    size += stream.WriteUnsignedInt(32, this.rwp_proj_region_height[i]);
                    size += stream.WriteUnsignedInt(32, this.rwp_proj_region_top[i]);
                    size += stream.WriteUnsignedInt(32, this.rwp_proj_region_left[i]);
                    size += stream.WriteUnsignedInt(16, this.rwp_packed_region_width[i]);
                    size += stream.WriteUnsignedInt(16, this.rwp_packed_region_height[i]);
                    size += stream.WriteUnsignedInt(16, this.rwp_packed_region_top[i]);
                    size += stream.WriteUnsignedInt(16, this.rwp_packed_region_left[i]);

                    if (rwp_guard_band_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.rwp_left_guard_band_width[i]);
                        size += stream.WriteUnsignedInt(8, this.rwp_right_guard_band_width[i]);
                        size += stream.WriteUnsignedInt(8, this.rwp_top_guard_band_height[i]);
                        size += stream.WriteUnsignedInt(8, this.rwp_bottom_guard_band_height[i]);
                        size += stream.WriteUnsignedInt(1, this.rwp_guard_band_not_used_for_pred_flag[i]);

                        for (j = 0; j < 4; j++)
                        {
                            size += stream.WriteUnsignedInt(3, this.rwp_guard_band_type[i][j]);
                        }
                        size += stream.WriteUnsignedInt(3, this.rwp_guard_band_reserved_zero_3bits[i]);
                    }
                }
            }

            return size;
        }

    }

    /*
 

omni_viewport(payloadSize) {
 omni_viewport_id u(10) 
 omni_viewport_cancel_flag u(1)
    if (!omni_viewport_cancel_flag) {  
  omni_viewport_persistence_flag u(1) 
  omni_viewport_cnt_minus1 u(4)
        for (i = 0; i <= omni_viewport_cnt_minus1; i++) {
            omni_viewport_azimuth_centre[i] i(32)
            omni_viewport_elevation_centre[i] i(32)
            omni_viewport_tilt_centre[i] i(32)
            omni_viewport_hor_range[i] u(32)
            omni_viewport_ver_range[i] u(32)
        }
    }
}
    */
    public class OmniViewport : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint omni_viewport_id;
        public uint OmniViewportId { get { return omni_viewport_id; } set { omni_viewport_id = value; } }
        private byte omni_viewport_cancel_flag;
        public byte OmniViewportCancelFlag { get { return omni_viewport_cancel_flag; } set { omni_viewport_cancel_flag = value; } }
        private byte omni_viewport_persistence_flag;
        public byte OmniViewportPersistenceFlag { get { return omni_viewport_persistence_flag; } set { omni_viewport_persistence_flag = value; } }
        private uint omni_viewport_cnt_minus1;
        public uint OmniViewportCntMinus1 { get { return omni_viewport_cnt_minus1; } set { omni_viewport_cnt_minus1 = value; } }
        private int[] omni_viewport_azimuth_centre;
        public int[] OmniViewportAzimuthCentre { get { return omni_viewport_azimuth_centre; } set { omni_viewport_azimuth_centre = value; } }
        private int[] omni_viewport_elevation_centre;
        public int[] OmniViewportElevationCentre { get { return omni_viewport_elevation_centre; } set { omni_viewport_elevation_centre = value; } }
        private int[] omni_viewport_tilt_centre;
        public int[] OmniViewportTiltCentre { get { return omni_viewport_tilt_centre; } set { omni_viewport_tilt_centre = value; } }
        private uint[] omni_viewport_hor_range;
        public uint[] OmniViewportHorRange { get { return omni_viewport_hor_range; } set { omni_viewport_hor_range = value; } }
        private uint[] omni_viewport_ver_range;
        public uint[] OmniViewportVerRange { get { return omni_viewport_ver_range; } set { omni_viewport_ver_range = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public OmniViewport(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 10, out this.omni_viewport_id);
            size += stream.ReadUnsignedInt(size, 1, out this.omni_viewport_cancel_flag);

            if (omni_viewport_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.omni_viewport_persistence_flag);
                size += stream.ReadUnsignedInt(size, 4, out this.omni_viewport_cnt_minus1);

                this.omni_viewport_azimuth_centre = new int[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_elevation_centre = new int[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_tilt_centre = new int[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_hor_range = new uint[omni_viewport_cnt_minus1 + 1];
                this.omni_viewport_ver_range = new uint[omni_viewport_cnt_minus1 + 1];
                for (i = 0; i <= omni_viewport_cnt_minus1; i++)
                {
                    size += stream.ReadSignedInt(size, 32, out this.omni_viewport_azimuth_centre[i]);
                    size += stream.ReadSignedInt(size, 32, out this.omni_viewport_elevation_centre[i]);
                    size += stream.ReadSignedInt(size, 32, out this.omni_viewport_tilt_centre[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.omni_viewport_hor_range[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.omni_viewport_ver_range[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(10, this.omni_viewport_id);
            size += stream.WriteUnsignedInt(1, this.omni_viewport_cancel_flag);

            if (omni_viewport_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.omni_viewport_persistence_flag);
                size += stream.WriteUnsignedInt(4, this.omni_viewport_cnt_minus1);

                for (i = 0; i <= omni_viewport_cnt_minus1; i++)
                {
                    size += stream.WriteSignedInt(32, this.omni_viewport_azimuth_centre[i]);
                    size += stream.WriteSignedInt(32, this.omni_viewport_elevation_centre[i]);
                    size += stream.WriteSignedInt(32, this.omni_viewport_tilt_centre[i]);
                    size += stream.WriteUnsignedInt(32, this.omni_viewport_hor_range[i]);
                    size += stream.WriteUnsignedInt(32, this.omni_viewport_ver_range[i]);
                }
            }

            return size;
        }

    }

    /*
 

frame_field_info(payloadSize) {
    ffi_field_pic_flag  u(1)
    if (ffi_field_pic_flag) {
        ffi_bottom_field_flag u(1)
        ffi_pairing_indicated_flag u(1)
        
        if (ffi_pairing_indicated_flag)
            ffi_paired_with_next_field_flag u(1)
     } else {
        
        ffi_display_fields_from_frame_flag u(1)
        if (ffi_display_fields_from_frame_flag)
            ffi_top_field_first_flag u(1)
        ffi_display_elemental_periods_minus1 u(8)
    }
    ffi_source_scan_type u(2)
    ffi_duplicate_flag u(1)
}
    */
    public class FrameFieldInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte ffi_field_pic_flag;
        public byte FfiFieldPicFlag { get { return ffi_field_pic_flag; } set { ffi_field_pic_flag = value; } }
        private byte ffi_bottom_field_flag;
        public byte FfiBottomFieldFlag { get { return ffi_bottom_field_flag; } set { ffi_bottom_field_flag = value; } }
        private byte ffi_pairing_indicated_flag;
        public byte FfiPairingIndicatedFlag { get { return ffi_pairing_indicated_flag; } set { ffi_pairing_indicated_flag = value; } }
        private byte ffi_paired_with_next_field_flag;
        public byte FfiPairedWithNextFieldFlag { get { return ffi_paired_with_next_field_flag; } set { ffi_paired_with_next_field_flag = value; } }
        private byte ffi_display_fields_from_frame_flag;
        public byte FfiDisplayFieldsFromFrameFlag { get { return ffi_display_fields_from_frame_flag; } set { ffi_display_fields_from_frame_flag = value; } }
        private byte ffi_top_field_first_flag;
        public byte FfiTopFieldFirstFlag { get { return ffi_top_field_first_flag; } set { ffi_top_field_first_flag = value; } }
        private uint ffi_display_elemental_periods_minus1;
        public uint FfiDisplayElementalPeriodsMinus1 { get { return ffi_display_elemental_periods_minus1; } set { ffi_display_elemental_periods_minus1 = value; } }
        private uint ffi_source_scan_type;
        public uint FfiSourceScanType { get { return ffi_source_scan_type; } set { ffi_source_scan_type = value; } }
        private byte ffi_duplicate_flag;
        public byte FfiDuplicateFlag { get { return ffi_duplicate_flag; } set { ffi_duplicate_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FrameFieldInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.ffi_field_pic_flag);

            if (ffi_field_pic_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ffi_bottom_field_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ffi_pairing_indicated_flag);

                if (ffi_pairing_indicated_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ffi_paired_with_next_field_flag);
                }
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ffi_display_fields_from_frame_flag);

                if (ffi_display_fields_from_frame_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ffi_top_field_first_flag);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.ffi_display_elemental_periods_minus1);
            }
            size += stream.ReadUnsignedInt(size, 2, out this.ffi_source_scan_type);
            size += stream.ReadUnsignedInt(size, 1, out this.ffi_duplicate_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.ffi_field_pic_flag);

            if (ffi_field_pic_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.ffi_bottom_field_flag);
                size += stream.WriteUnsignedInt(1, this.ffi_pairing_indicated_flag);

                if (ffi_pairing_indicated_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ffi_paired_with_next_field_flag);
                }
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.ffi_display_fields_from_frame_flag);

                if (ffi_display_fields_from_frame_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ffi_top_field_first_flag);
                }
                size += stream.WriteUnsignedInt(8, this.ffi_display_elemental_periods_minus1);
            }
            size += stream.WriteUnsignedInt(2, this.ffi_source_scan_type);
            size += stream.WriteUnsignedInt(1, this.ffi_duplicate_flag);

            return size;
        }

    }

    /*
 

sample_aspect_ratio_info(payloadSize) {
    sari_cancel_flag u(1)
    if (!sari_cancel_flag) {
        sari_persistence_flag u(1)
        sari_aspect_ratio_idc u(8)
        if (sari_aspect_ratio_idc == 255) {
            sari_sar_width u(16) 
            sari_sar_height u(16) 
        }
    }
}
    */
    public class SampleAspectRatioInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte sari_cancel_flag;
        public byte SariCancelFlag { get { return sari_cancel_flag; } set { sari_cancel_flag = value; } }
        private byte sari_persistence_flag;
        public byte SariPersistenceFlag { get { return sari_persistence_flag; } set { sari_persistence_flag = value; } }
        private uint sari_aspect_ratio_idc;
        public uint SariAspectRatioIdc { get { return sari_aspect_ratio_idc; } set { sari_aspect_ratio_idc = value; } }
        private uint sari_sar_width;
        public uint SariSarWidth { get { return sari_sar_width; } set { sari_sar_width = value; } }
        private uint sari_sar_height;
        public uint SariSarHeight { get { return sari_sar_height; } set { sari_sar_height = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SampleAspectRatioInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.sari_cancel_flag);

            if (sari_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sari_persistence_flag);
                size += stream.ReadUnsignedInt(size, 8, out this.sari_aspect_ratio_idc);

                if (sari_aspect_ratio_idc == 255)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.sari_sar_width);
                    size += stream.ReadUnsignedInt(size, 16, out this.sari_sar_height);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.sari_cancel_flag);

            if (sari_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sari_persistence_flag);
                size += stream.WriteUnsignedInt(8, this.sari_aspect_ratio_idc);

                if (sari_aspect_ratio_idc == 255)
                {
                    size += stream.WriteUnsignedInt(16, this.sari_sar_width);
                    size += stream.WriteUnsignedInt(16, this.sari_sar_height);
                }
            }

            return size;
        }

    }

    /*


annotated_regions(payloadSize) {
    ar_cancel_flag u(1)
    if (!ar_cancel_flag) {      
        ar_not_optimized_for_viewing_flag u(1)
        ar_true_motion_flag u(1)
        ar_occluded_object_flag u(1)
        ar_partial_object_flag_present_flag u(1)
        ar_object_label_present_flag u(1)
        ar_object_confidence_info_present_flag u(1)
        if (ar_object_confidence_info_present_flag)
            ar_object_confidence_length_minus1 u(4)
        if (ar_object_label_present_flag) {  
            ar_object_label_language_present_flag u(1)
            if (ar_object_label_language_present_flag) {
                while (!byte_aligned())  
                    ar_bit_equal_to_zero /* equal to 0 *//* f(1) 
                ar_object_label_language st(v)
            }  
            ar_num_label_updates ue(v)
            for (i = 0; i < ar_num_label_updates; i++) {
                ar_label_idx[i] ue(v) 
                ar_label_cancel_flag u(1)
                LabelAssigned[ar_label_idx[i]] = !ar_label_cancel_flag
                if (!ar_label_cancel_flag) {
                    while (!byte_aligned())  
                        ar_bit_equal_to_zero /* equal to 0 *//* f(1)
                    ar_label[ar_label_idx[i]] st(v)
                }
            }
        }  
        ar_num_object_updates ue(v)
        for (i = 0; i < ar_num_object_updates; i++) {
            ar_object_idx[i] ue(v) 
            ar_object_cancel_flag u(1)
            ObjectTracked[ar_object_idx[i]] = !ar_object_cancel_flag
            if (!ar_object_cancel_flag) {
                if (ar_object_label_present_flag) {  
                    ar_object_label_update_flag u(1)
                    if (ar_object_label_update_flag)
                        ar_object_label_idx[ar_object_idx[i]] ue(v)
                }  
                ar_bounding_box_update_flag u(1)
                if (ar_bounding_box_update_flag) {  
                    ar_bounding_box_cancel_flag u(1)
                    ObjectBoundingBoxAvail[ar_object_idx[i]] =
                        !ar_bounding_box_cancel_flag

                    if (!ar_bounding_box_cancel_flag) {
                        ar_bounding_box_top[ar_object_idx[i]] u(16)
                        ar_bounding_box_left[ar_object_idx[i]] u(16)
                        ar_bounding_box_width[ar_object_idx[i]] u(16)
                        ar_bounding_box_height[ar_object_idx[i]] u(16)
                        if (ar_partial_object_flag_present_flag)
                            ar_partial_object_flag[ar_object_idx[i]] u(1)
                        if (ar_object_confidence_info_present_flag)
                            ar_object_confidence[ar_object_idx[i]] u(v)
                    }
                }
            }
        }
    }
}
    */
    public class AnnotatedRegions : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte ar_cancel_flag;
        public byte ArCancelFlag { get { return ar_cancel_flag; } set { ar_cancel_flag = value; } }
        private byte ar_not_optimized_for_viewing_flag;
        public byte ArNotOptimizedForViewingFlag { get { return ar_not_optimized_for_viewing_flag; } set { ar_not_optimized_for_viewing_flag = value; } }
        private byte ar_true_motion_flag;
        public byte ArTrueMotionFlag { get { return ar_true_motion_flag; } set { ar_true_motion_flag = value; } }
        private byte ar_occluded_object_flag;
        public byte ArOccludedObjectFlag { get { return ar_occluded_object_flag; } set { ar_occluded_object_flag = value; } }
        private byte ar_partial_object_flag_present_flag;
        public byte ArPartialObjectFlagPresentFlag { get { return ar_partial_object_flag_present_flag; } set { ar_partial_object_flag_present_flag = value; } }
        private byte ar_object_label_present_flag;
        public byte ArObjectLabelPresentFlag { get { return ar_object_label_present_flag; } set { ar_object_label_present_flag = value; } }
        private byte ar_object_confidence_info_present_flag;
        public byte ArObjectConfidenceInfoPresentFlag { get { return ar_object_confidence_info_present_flag; } set { ar_object_confidence_info_present_flag = value; } }
        private uint ar_object_confidence_length_minus1;
        public uint ArObjectConfidenceLengthMinus1 { get { return ar_object_confidence_length_minus1; } set { ar_object_confidence_length_minus1 = value; } }
        private byte ar_object_label_language_present_flag;
        public byte ArObjectLabelLanguagePresentFlag { get { return ar_object_label_language_present_flag; } set { ar_object_label_language_present_flag = value; } }
        private Dictionary<int, uint> ar_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> ArBitEqualToZero { get { return ar_bit_equal_to_zero; } set { ar_bit_equal_to_zero = value; } }
        private byte[] ar_object_label_language;
        public byte[] ArObjectLabelLanguage { get { return ar_object_label_language; } set { ar_object_label_language = value; } }
        private uint ar_num_label_updates;
        public uint ArNumLabelUpdates { get { return ar_num_label_updates; } set { ar_num_label_updates = value; } }
        private uint[] ar_label_idx;
        public uint[] ArLabelIdx { get { return ar_label_idx; } set { ar_label_idx = value; } }
        private byte[] ar_label_cancel_flag;
        public byte[] ArLabelCancelFlag { get { return ar_label_cancel_flag; } set { ar_label_cancel_flag = value; } }
        private byte[][] ar_label;
        public byte[][] ArLabel { get { return ar_label; } set { ar_label = value; } }
        private uint ar_num_object_updates;
        public uint ArNumObjectUpdates { get { return ar_num_object_updates; } set { ar_num_object_updates = value; } }
        private uint[] ar_object_idx;
        public uint[] ArObjectIdx { get { return ar_object_idx; } set { ar_object_idx = value; } }
        private byte[] ar_object_cancel_flag;
        public byte[] ArObjectCancelFlag { get { return ar_object_cancel_flag; } set { ar_object_cancel_flag = value; } }
        private byte[] ar_object_label_update_flag;
        public byte[] ArObjectLabelUpdateFlag { get { return ar_object_label_update_flag; } set { ar_object_label_update_flag = value; } }
        private uint[] ar_object_label_idx;
        public uint[] ArObjectLabelIdx { get { return ar_object_label_idx; } set { ar_object_label_idx = value; } }
        private byte[] ar_bounding_box_update_flag;
        public byte[] ArBoundingBoxUpdateFlag { get { return ar_bounding_box_update_flag; } set { ar_bounding_box_update_flag = value; } }
        private byte[] ar_bounding_box_cancel_flag;
        public byte[] ArBoundingBoxCancelFlag { get { return ar_bounding_box_cancel_flag; } set { ar_bounding_box_cancel_flag = value; } }
        private uint[] ar_bounding_box_top;
        public uint[] ArBoundingBoxTop { get { return ar_bounding_box_top; } set { ar_bounding_box_top = value; } }
        private uint[] ar_bounding_box_left;
        public uint[] ArBoundingBoxLeft { get { return ar_bounding_box_left; } set { ar_bounding_box_left = value; } }
        private uint[] ar_bounding_box_width;
        public uint[] ArBoundingBoxWidth { get { return ar_bounding_box_width; } set { ar_bounding_box_width = value; } }
        private uint[] ar_bounding_box_height;
        public uint[] ArBoundingBoxHeight { get { return ar_bounding_box_height; } set { ar_bounding_box_height = value; } }
        private byte[] ar_partial_object_flag;
        public byte[] ArPartialObjectFlag { get { return ar_partial_object_flag; } set { ar_partial_object_flag = value; } }
        private uint[] ar_object_confidence;
        public uint[] ArObjectConfidence { get { return ar_object_confidence; } set { ar_object_confidence = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AnnotatedRegions(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            uint i = 0;
            uint[] LabelAssigned = null;
            uint[] ObjectTracked = null;
            uint[] ObjectBoundingBoxAvail = null;
            size += stream.ReadUnsignedInt(size, 1, out this.ar_cancel_flag);

            if (ar_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ar_not_optimized_for_viewing_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_true_motion_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_occluded_object_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_partial_object_flag_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_object_label_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.ar_object_confidence_info_present_flag);

                if (ar_object_confidence_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.ar_object_confidence_length_minus1);
                }

                if (ar_object_label_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.ar_object_label_language_present_flag);

                    if (ar_object_label_language_present_flag != 0)
                    {

                        while (!stream.ByteAligned())
                        {
                            whileIndex++;

                            size += stream.ReadFixed(size, 1, whileIndex, this.ar_bit_equal_to_zero); // equal to 0 
                        }
                        size += stream.ReadUtf8String(size, out this.ar_object_label_language);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.ar_num_label_updates);

                    this.ar_label_idx = new uint[ar_num_label_updates];
                    this.ar_label_cancel_flag = new byte[ar_num_label_updates];
                    this.ar_label = new byte[ar_num_label_updates][];
                    for (i = 0; i < ar_num_label_updates; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ar_label_idx[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.ar_label_cancel_flag[i]);
                        LabelAssigned[ar_label_idx[i]] = ar_label_cancel_flag[i] == 0 ? (uint)1 : (uint)0;

                        if (ar_label_cancel_flag[i] == 0)
                        {

                            while (!stream.ByteAligned())
                            {
                                whileIndex++;

                                size += stream.ReadFixed(size, 1, whileIndex, this.ar_bit_equal_to_zero); // equal to 0 
                            }
                            size += stream.ReadUtf8String(size, out this.ar_label[ar_label_idx[i]]);
                        }
                    }
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.ar_num_object_updates);

                this.ar_object_idx = new uint[ar_num_object_updates];
                this.ar_object_cancel_flag = new byte[ar_num_object_updates];
                this.ar_object_label_update_flag = new byte[ar_num_object_updates];
                this.ar_object_label_idx = new uint[ar_num_object_updates];
                this.ar_bounding_box_update_flag = new byte[ar_num_object_updates];
                this.ar_bounding_box_cancel_flag = new byte[ar_num_object_updates];
                this.ar_bounding_box_top = new uint[ar_num_object_updates];
                this.ar_bounding_box_left = new uint[ar_num_object_updates];
                this.ar_bounding_box_width = new uint[ar_num_object_updates];
                this.ar_bounding_box_height = new uint[ar_num_object_updates];
                this.ar_partial_object_flag = new byte[ar_num_object_updates];
                this.ar_object_confidence = new uint[ar_num_object_updates];
                for (i = 0; i < ar_num_object_updates; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ar_object_idx[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.ar_object_cancel_flag[i]);
                    ObjectTracked[ar_object_idx[i]] = ar_object_cancel_flag[i] == 0 ? (uint)1 : (uint)0;

                    if (ar_object_cancel_flag[i] == 0)
                    {

                        if (ar_object_label_present_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.ar_object_label_update_flag[i]);

                            if (ar_object_label_update_flag[i] != 0)
                            {
                                size += stream.ReadUnsignedIntGolomb(size, out this.ar_object_label_idx[ar_object_idx[i]]);
                            }
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.ar_bounding_box_update_flag[i]);

                        if (ar_bounding_box_update_flag[i] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.ar_bounding_box_cancel_flag[i]);
                            ObjectBoundingBoxAvail[ar_object_idx[i]] = ar_bounding_box_cancel_flag[i] == 0 ? (uint)1 : (uint)0;

                            if (ar_bounding_box_cancel_flag[i] == 0)
                            {
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_top[ar_object_idx[i]]);
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_left[ar_object_idx[i]]);
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_width[ar_object_idx[i]]);
                                size += stream.ReadUnsignedInt(size, 16, out this.ar_bounding_box_height[ar_object_idx[i]]);

                                if (ar_partial_object_flag_present_flag != 0)
                                {
                                    size += stream.ReadUnsignedInt(size, 1, out this.ar_partial_object_flag[ar_object_idx[i]]);
                                }

                                if (ar_object_confidence_info_present_flag != 0)
                                {
                                    size += stream.ReadUnsignedIntVariable(size, ar_object_confidence_length_minus1 + 1, out this.ar_object_confidence[ar_object_idx[i]]);
                                }
                            }
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            uint i = 0;
            uint[] LabelAssigned = null;
            uint[] ObjectTracked = null;
            uint[] ObjectBoundingBoxAvail = null;
            size += stream.WriteUnsignedInt(1, this.ar_cancel_flag);

            if (ar_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.ar_not_optimized_for_viewing_flag);
                size += stream.WriteUnsignedInt(1, this.ar_true_motion_flag);
                size += stream.WriteUnsignedInt(1, this.ar_occluded_object_flag);
                size += stream.WriteUnsignedInt(1, this.ar_partial_object_flag_present_flag);
                size += stream.WriteUnsignedInt(1, this.ar_object_label_present_flag);
                size += stream.WriteUnsignedInt(1, this.ar_object_confidence_info_present_flag);

                if (ar_object_confidence_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(4, this.ar_object_confidence_length_minus1);
                }

                if (ar_object_label_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.ar_object_label_language_present_flag);

                    if (ar_object_label_language_present_flag != 0)
                    {

                        while (!stream.ByteAligned())
                        {
                            whileIndex++;

                            size += stream.WriteFixed(1, whileIndex, this.ar_bit_equal_to_zero); // equal to 0 
                        }
                        size += stream.WriteUtf8String(this.ar_object_label_language);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.ar_num_label_updates);

                    for (i = 0; i < ar_num_label_updates; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ar_label_idx[i]);
                        size += stream.WriteUnsignedInt(1, this.ar_label_cancel_flag[i]);
                        LabelAssigned[ar_label_idx[i]] = ar_label_cancel_flag[i] == 0 ? (uint)1 : (uint)0;

                        if (ar_label_cancel_flag[i] == 0)
                        {

                            while (!stream.ByteAligned())
                            {
                                whileIndex++;

                                size += stream.WriteFixed(1, whileIndex, this.ar_bit_equal_to_zero); // equal to 0 
                            }
                            size += stream.WriteUtf8String(this.ar_label[ar_label_idx[i]]);
                        }
                    }
                }
                size += stream.WriteUnsignedIntGolomb(this.ar_num_object_updates);

                for (i = 0; i < ar_num_object_updates; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ar_object_idx[i]);
                    size += stream.WriteUnsignedInt(1, this.ar_object_cancel_flag[i]);
                    ObjectTracked[ar_object_idx[i]] = ar_object_cancel_flag[i] == 0 ? (uint)1 : (uint)0;

                    if (ar_object_cancel_flag[i] == 0)
                    {

                        if (ar_object_label_present_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.ar_object_label_update_flag[i]);

                            if (ar_object_label_update_flag[i] != 0)
                            {
                                size += stream.WriteUnsignedIntGolomb(this.ar_object_label_idx[ar_object_idx[i]]);
                            }
                        }
                        size += stream.WriteUnsignedInt(1, this.ar_bounding_box_update_flag[i]);

                        if (ar_bounding_box_update_flag[i] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.ar_bounding_box_cancel_flag[i]);
                            ObjectBoundingBoxAvail[ar_object_idx[i]] = ar_bounding_box_cancel_flag[i] == 0 ? (uint)1 : (uint)0;

                            if (ar_bounding_box_cancel_flag[i] == 0)
                            {
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_top[ar_object_idx[i]]);
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_left[ar_object_idx[i]]);
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_width[ar_object_idx[i]]);
                                size += stream.WriteUnsignedInt(16, this.ar_bounding_box_height[ar_object_idx[i]]);

                                if (ar_partial_object_flag_present_flag != 0)
                                {
                                    size += stream.WriteUnsignedInt(1, this.ar_partial_object_flag[ar_object_idx[i]]);
                                }

                                if (ar_object_confidence_info_present_flag != 0)
                                {
                                    size += stream.WriteUnsignedIntVariable(ar_object_confidence_length_minus1 + 1, this.ar_object_confidence[ar_object_idx[i]]);
                                }
                            }
                        }
                    }
                }
            }

            return size;
        }

    }

    /*


scalability_dimension_info(payloadSize) {
 sdi_max_layers_minus1 u(6) 
 sdi_multiview_info_flag u(1) 
 sdi_auxiliary_info_flag u(1)
    if (sdi_multiview_info_flag || sdi_auxiliary_info_flag) {
        if (sdi_multiview_info_flag)  
            sdi_view_id_len_minus1 u(4)
        for (i = 0; i <= sdi_max_layers_minus1; i++) {
            sdi_layer_id[i] u(6)
            if (sdi_multiview_info_flag)
                sdi_view_id_val[i] u(v)
            if (sdi_auxiliary_info_flag)
                sdi_aux_id[i] u(8)
            if (sdi_aux_id[i] > 0) {
                sdi_num_associated_primary_layers_minus1[i] u(6)
                for (j = 0; j <= sdi_num_associated_primary_layers_minus1[i]; j++)
                    sdi_associated_primary_layer_idx[i][j] u(6)
            }
        }
    }
}
    */
    public class ScalabilityDimensionInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sdi_max_layers_minus1;
        public uint SdiMaxLayersMinus1 { get { return sdi_max_layers_minus1; } set { sdi_max_layers_minus1 = value; } }
        private byte sdi_multiview_info_flag;
        public byte SdiMultiviewInfoFlag { get { return sdi_multiview_info_flag; } set { sdi_multiview_info_flag = value; } }
        private byte sdi_auxiliary_info_flag;
        public byte SdiAuxiliaryInfoFlag { get { return sdi_auxiliary_info_flag; } set { sdi_auxiliary_info_flag = value; } }
        private uint sdi_view_id_len_minus1;
        public uint SdiViewIdLenMinus1 { get { return sdi_view_id_len_minus1; } set { sdi_view_id_len_minus1 = value; } }
        private uint[] sdi_layer_id;
        public uint[] SdiLayerId { get { return sdi_layer_id; } set { sdi_layer_id = value; } }
        private uint[] sdi_view_id_val;
        public uint[] SdiViewIdVal { get { return sdi_view_id_val; } set { sdi_view_id_val = value; } }
        private uint[] sdi_aux_id;
        public uint[] SdiAuxId { get { return sdi_aux_id; } set { sdi_aux_id = value; } }
        private uint[] sdi_num_associated_primary_layers_minus1;
        public uint[] SdiNumAssociatedPrimaryLayersMinus1 { get { return sdi_num_associated_primary_layers_minus1; } set { sdi_num_associated_primary_layers_minus1 = value; } }
        private uint[][] sdi_associated_primary_layer_idx;
        public uint[][] SdiAssociatedPrimaryLayerIdx { get { return sdi_associated_primary_layer_idx; } set { sdi_associated_primary_layer_idx = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ScalabilityDimensionInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 6, out this.sdi_max_layers_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.sdi_multiview_info_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sdi_auxiliary_info_flag);

            if (sdi_multiview_info_flag != 0 || sdi_auxiliary_info_flag != 0)
            {

                if (sdi_multiview_info_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.sdi_view_id_len_minus1);
                }

                this.sdi_layer_id = new uint[sdi_max_layers_minus1 + 1];
                this.sdi_view_id_val = new uint[sdi_max_layers_minus1 + 1];
                this.sdi_aux_id = new uint[sdi_max_layers_minus1 + 1];
                this.sdi_num_associated_primary_layers_minus1 = new uint[sdi_max_layers_minus1 + 1];
                this.sdi_associated_primary_layer_idx = new uint[sdi_max_layers_minus1 + 1][];
                for (i = 0; i <= sdi_max_layers_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 6, out this.sdi_layer_id[i]);

                    if (sdi_multiview_info_flag != 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, sdi_view_id_len_minus1 + 1, out this.sdi_view_id_val[i]);
                    }

                    if (sdi_auxiliary_info_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.sdi_aux_id[i]);
                    }

                    if (sdi_aux_id[i] > 0)
                    {
                        size += stream.ReadUnsignedInt(size, 6, out this.sdi_num_associated_primary_layers_minus1[i]);

                        this.sdi_associated_primary_layer_idx[i] = new uint[sdi_num_associated_primary_layers_minus1[i] + 1];
                        for (j = 0; j <= sdi_num_associated_primary_layers_minus1[i]; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 6, out this.sdi_associated_primary_layer_idx[i][j]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(6, this.sdi_max_layers_minus1);
            size += stream.WriteUnsignedInt(1, this.sdi_multiview_info_flag);
            size += stream.WriteUnsignedInt(1, this.sdi_auxiliary_info_flag);

            if (sdi_multiview_info_flag != 0 || sdi_auxiliary_info_flag != 0)
            {

                if (sdi_multiview_info_flag != 0)
                {
                    size += stream.WriteUnsignedInt(4, this.sdi_view_id_len_minus1);
                }

                for (i = 0; i <= sdi_max_layers_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(6, this.sdi_layer_id[i]);

                    if (sdi_multiview_info_flag != 0)
                    {
                        size += stream.WriteUnsignedIntVariable(sdi_view_id_len_minus1 + 1, this.sdi_view_id_val[i]);
                    }

                    if (sdi_auxiliary_info_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.sdi_aux_id[i]);
                    }

                    if (sdi_aux_id[i] > 0)
                    {
                        size += stream.WriteUnsignedInt(6, this.sdi_num_associated_primary_layers_minus1[i]);

                        for (j = 0; j <= sdi_num_associated_primary_layers_minus1[i]; j++)
                        {
                            size += stream.WriteUnsignedInt(6, this.sdi_associated_primary_layer_idx[i][j]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

multiview_acquisition_info(payloadSize) {
    intrinsic_param_flag u(1)
    extrinsic_param_flag u(1)
    num_views_minus1 ue(v)
    if (intrinsic_param_flag) {
        intrinsic_params_equal_flag u(1)
        prec_focal_length ue(v) 
  prec_principal_point ue(v) 
  prec_skew_factor ue(v)
        for (i = 0; i <= (intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1); i++) {
            sign_focal_length_x[i] u(1)
            exponent_focal_length_x[i] u(6)
            mantissa_focal_length_x[i] u(v)
            sign_focal_length_y[i] u(1)
            exponent_focal_length_y[i] u(6)
            mantissa_focal_length_y[i] u(v)
            sign_principal_point_x[i] u(1)
            exponent_principal_point_x[i] u(6)
            mantissa_principal_point_x[i] u(v)
            sign_principal_point_y[i] u(1)
            exponent_principal_point_y[i] u(6)
            mantissa_principal_point_y[i] u(v)
            sign_skew_factor[i] u(1)
            exponent_skew_factor[i] u(6)
            mantissa_skew_factor[i] u(v)
        }
    }
    if (extrinsic_param_flag) {  
  prec_rotation_param ue(v) 
  prec_translation_param ue(v)
        for (i = 0; i <= num_views_minus1; i++)
            for (j = 0; j < 3; j++) { /* row *//*
                for (k = 0; k < 3; k++) { /* column *//*
                    sign_r[i][j][k] u(1)
                    exponent_r[i][j][k] u(6)
                    mantissa_r[i][j][k] u(v)
                }
                sign_t[i][j] u(1)
                exponent_t[i][j] u(6)
                mantissa_t[i][j] u(v)
            }
    }
}
    */
    public class MultiviewAcquisitionInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte intrinsic_param_flag;
        public byte IntrinsicParamFlag { get { return intrinsic_param_flag; } set { intrinsic_param_flag = value; } }
        private byte extrinsic_param_flag;
        public byte ExtrinsicParamFlag { get { return extrinsic_param_flag; } set { extrinsic_param_flag = value; } }
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private byte intrinsic_params_equal_flag;
        public byte IntrinsicParamsEqualFlag { get { return intrinsic_params_equal_flag; } set { intrinsic_params_equal_flag = value; } }
        private uint prec_focal_length;
        public uint PrecFocalLength { get { return prec_focal_length; } set { prec_focal_length = value; } }
        private uint prec_principal_point;
        public uint PrecPrincipalPoint { get { return prec_principal_point; } set { prec_principal_point = value; } }
        private uint prec_skew_factor;
        public uint PrecSkewFactor { get { return prec_skew_factor; } set { prec_skew_factor = value; } }
        private byte[] sign_focal_length_x;
        public byte[] SignFocalLengthx { get { return sign_focal_length_x; } set { sign_focal_length_x = value; } }
        private uint[] exponent_focal_length_x;
        public uint[] ExponentFocalLengthx { get { return exponent_focal_length_x; } set { exponent_focal_length_x = value; } }
        private uint[] mantissa_focal_length_x;
        public uint[] MantissaFocalLengthx { get { return mantissa_focal_length_x; } set { mantissa_focal_length_x = value; } }
        private byte[] sign_focal_length_y;
        public byte[] SignFocalLengthy { get { return sign_focal_length_y; } set { sign_focal_length_y = value; } }
        private uint[] exponent_focal_length_y;
        public uint[] ExponentFocalLengthy { get { return exponent_focal_length_y; } set { exponent_focal_length_y = value; } }
        private uint[] mantissa_focal_length_y;
        public uint[] MantissaFocalLengthy { get { return mantissa_focal_length_y; } set { mantissa_focal_length_y = value; } }
        private byte[] sign_principal_point_x;
        public byte[] SignPrincipalPointx { get { return sign_principal_point_x; } set { sign_principal_point_x = value; } }
        private uint[] exponent_principal_point_x;
        public uint[] ExponentPrincipalPointx { get { return exponent_principal_point_x; } set { exponent_principal_point_x = value; } }
        private uint[] mantissa_principal_point_x;
        public uint[] MantissaPrincipalPointx { get { return mantissa_principal_point_x; } set { mantissa_principal_point_x = value; } }
        private byte[] sign_principal_point_y;
        public byte[] SignPrincipalPointy { get { return sign_principal_point_y; } set { sign_principal_point_y = value; } }
        private uint[] exponent_principal_point_y;
        public uint[] ExponentPrincipalPointy { get { return exponent_principal_point_y; } set { exponent_principal_point_y = value; } }
        private uint[] mantissa_principal_point_y;
        public uint[] MantissaPrincipalPointy { get { return mantissa_principal_point_y; } set { mantissa_principal_point_y = value; } }
        private byte[] sign_skew_factor;
        public byte[] SignSkewFactor { get { return sign_skew_factor; } set { sign_skew_factor = value; } }
        private uint[] exponent_skew_factor;
        public uint[] ExponentSkewFactor { get { return exponent_skew_factor; } set { exponent_skew_factor = value; } }
        private uint[] mantissa_skew_factor;
        public uint[] MantissaSkewFactor { get { return mantissa_skew_factor; } set { mantissa_skew_factor = value; } }
        private uint prec_rotation_param;
        public uint PrecRotationParam { get { return prec_rotation_param; } set { prec_rotation_param = value; } }
        private uint prec_translation_param;
        public uint PrecTranslationParam { get { return prec_translation_param; } set { prec_translation_param = value; } }
        private byte[][][] sign_r;
        public byte[][][] Signr { get { return sign_r; } set { sign_r = value; } }
        private uint[][][] exponent_r;
        public uint[][][] Exponentr { get { return exponent_r; } set { exponent_r = value; } }
        private uint[][][] mantissa_r;
        public uint[][][] Mantissar { get { return mantissa_r; } set { mantissa_r = value; } }
        private byte[][] sign_t;
        public byte[][] Signt { get { return sign_t; } set { sign_t = value; } }
        private uint[][] exponent_t;
        public uint[][] Exponentt { get { return exponent_t; } set { exponent_t = value; } }
        private uint[][] mantissa_t;
        public uint[][] Mantissat { get { return mantissa_t; } set { mantissa_t = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MultiviewAcquisitionInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_param_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.extrinsic_param_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);

            if (intrinsic_param_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_params_equal_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_focal_length);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_principal_point);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_skew_factor);

                this.sign_focal_length_x = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_focal_length_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_focal_length_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_focal_length_y = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_focal_length_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_focal_length_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_principal_point_x = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_principal_point_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_principal_point_x = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_principal_point_y = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_principal_point_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_principal_point_y = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.sign_skew_factor = new byte[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.exponent_skew_factor = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                this.mantissa_skew_factor = new uint[(intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1 + 1)];
                for (i = 0; i <= (intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1); i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_focal_length_x[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_focal_length_x[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_focal_length_x[i] == 0 ? Math.Max(0, prec_focal_length - 30) : Math.Max(0, exponent_focal_length_x[i] + prec_focal_length - 31)), out this.mantissa_focal_length_x[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_focal_length_y[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_focal_length_y[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_focal_length_y[i] == 0 ? Math.Max(0, prec_focal_length - 30) : Math.Max(0, exponent_focal_length_y[i] + prec_focal_length - 31)), out this.mantissa_focal_length_y[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_principal_point_x[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_principal_point_x[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_principal_point_x[i] == 0 ? Math.Max(0, prec_principal_point - 30) : Math.Max(0, exponent_principal_point_x[i] + prec_principal_point - 31)), out this.mantissa_principal_point_x[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_principal_point_y[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_principal_point_y[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_principal_point_y[i] == 0 ? Math.Max(0, prec_principal_point - 30) : Math.Max(0, exponent_principal_point_y[i] + prec_principal_point - 31)), out this.mantissa_principal_point_y[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_skew_factor[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_skew_factor[i]);
                    size += stream.ReadUnsignedIntVariable(size, (exponent_skew_factor[i] == 0 ? Math.Max(0, prec_skew_factor - 30) : Math.Max(0, exponent_skew_factor[i] + prec_skew_factor - 31)), out this.mantissa_skew_factor[i]);
                }
            }

            if (extrinsic_param_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_rotation_param);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_translation_param);

                this.sign_r = new byte[num_views_minus1 + 1][][];
                this.exponent_r = new uint[num_views_minus1 + 1][][];
                this.mantissa_r = new uint[num_views_minus1 + 1][][];
                this.sign_t = new byte[num_views_minus1 + 1][];
                this.exponent_t = new uint[num_views_minus1 + 1][];
                this.mantissa_t = new uint[num_views_minus1 + 1][];
                for (i = 0; i <= num_views_minus1; i++)
                {

                    this.sign_r[i] = new byte[3][];
                    this.exponent_r[i] = new uint[3][];
                    this.mantissa_r[i] = new uint[3][];
                    this.sign_t[i] = new byte[3];
                    this.exponent_t[i] = new uint[3];
                    this.mantissa_t[i] = new uint[3];
                    for (j = 0; j < 3; j++)
                    {
                        /*  row  */


                        this.sign_r[i][j] = new byte[3];
                        this.exponent_r[i][j] = new uint[3];
                        this.mantissa_r[i][j] = new uint[3];
                        for (k = 0; k < 3; k++)
                        {
                            /*  column  */

                            size += stream.ReadUnsignedInt(size, 1, out this.sign_r[i][j][k]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exponent_r[i][j][k]);
                            size += stream.ReadUnsignedIntVariable(size, (exponent_r[i][j][k] == 0 ? Math.Max(0, prec_rotation_param - 30) : Math.Max(0, exponent_r[i][j][k] + prec_rotation_param - 31)), out this.mantissa_r[i][j][k]);
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_t[i][j]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exponent_t[i][j]);
                        size += stream.ReadUnsignedIntVariable(size, (exponent_t[i][j] == 0 ? Math.Max(0, prec_translation_param - 30) : Math.Max(0, exponent_t[i][j] + prec_translation_param - 31)), out this.mantissa_t[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.WriteUnsignedInt(1, this.intrinsic_param_flag);
            size += stream.WriteUnsignedInt(1, this.extrinsic_param_flag);
            size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);

            if (intrinsic_param_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.intrinsic_params_equal_flag);
                size += stream.WriteUnsignedIntGolomb(this.prec_focal_length);
                size += stream.WriteUnsignedIntGolomb(this.prec_principal_point);
                size += stream.WriteUnsignedIntGolomb(this.prec_skew_factor);

                for (i = 0; i <= (intrinsic_params_equal_flag != 0 ? 0 : num_views_minus1); i++)
                {
                    size += stream.WriteUnsignedInt(1, this.sign_focal_length_x[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_focal_length_x[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_focal_length_x[i] == 0 ? Math.Max(0, prec_focal_length - 30) : Math.Max(0, exponent_focal_length_x[i] + prec_focal_length - 31)), this.mantissa_focal_length_x[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_focal_length_y[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_focal_length_y[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_focal_length_y[i] == 0 ? Math.Max(0, prec_focal_length - 30) : Math.Max(0, exponent_focal_length_y[i] + prec_focal_length - 31)), this.mantissa_focal_length_y[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_principal_point_x[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_principal_point_x[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_principal_point_x[i] == 0 ? Math.Max(0, prec_principal_point - 30) : Math.Max(0, exponent_principal_point_x[i] + prec_principal_point - 31)), this.mantissa_principal_point_x[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_principal_point_y[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_principal_point_y[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_principal_point_y[i] == 0 ? Math.Max(0, prec_principal_point - 30) : Math.Max(0, exponent_principal_point_y[i] + prec_principal_point - 31)), this.mantissa_principal_point_y[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_skew_factor[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_skew_factor[i]);
                    size += stream.WriteUnsignedIntVariable((exponent_skew_factor[i] == 0 ? Math.Max(0, prec_skew_factor - 30) : Math.Max(0, exponent_skew_factor[i] + prec_skew_factor - 31)), this.mantissa_skew_factor[i]);
                }
            }

            if (extrinsic_param_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.prec_rotation_param);
                size += stream.WriteUnsignedIntGolomb(this.prec_translation_param);

                for (i = 0; i <= num_views_minus1; i++)
                {

                    for (j = 0; j < 3; j++)
                    {
                        /*  row  */


                        for (k = 0; k < 3; k++)
                        {
                            /*  column  */

                            size += stream.WriteUnsignedInt(1, this.sign_r[i][j][k]);
                            size += stream.WriteUnsignedInt(6, this.exponent_r[i][j][k]);
                            size += stream.WriteUnsignedIntVariable((exponent_r[i][j][k] == 0 ? Math.Max(0, prec_rotation_param - 30) : Math.Max(0, exponent_r[i][j][k] + prec_rotation_param - 31)), this.mantissa_r[i][j][k]);
                        }
                        size += stream.WriteUnsignedInt(1, this.sign_t[i][j]);
                        size += stream.WriteUnsignedInt(6, this.exponent_t[i][j]);
                        size += stream.WriteUnsignedIntVariable((exponent_t[i][j] == 0 ? Math.Max(0, prec_translation_param - 30) : Math.Max(0, exponent_t[i][j] + prec_translation_param - 31)), this.mantissa_t[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*


multiview_view_position(payloadSize) {
    num_views_minus1 ue(v)
    for (i = 0; i <= num_views_minus1; i++ )
        view_position[i] ue(v)
}
    */
    public class MultiviewViewPosition : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_views_minus1;
        public uint NumViewsMinus1 { get { return num_views_minus1; } set { num_views_minus1 = value; } }
        private uint[] view_position;
        public uint[] ViewPosition { get { return view_position; } set { view_position = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MultiviewViewPosition(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_views_minus1);

            this.view_position = new uint[num_views_minus1 + 1];
            for (i = 0; i <= num_views_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.view_position[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_views_minus1);

            for (i = 0; i <= num_views_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.view_position[i]);
            }

            return size;
        }

    }

    /*


depth_representation_info(payloadSize) {
    z_near_flag u(1)
    z_far_flag u(1)
    d_min_flag u(1)
    d_max_flag u(1)
    depth_representation_type ue(v) 
    
    if (d_min_flag || d_max_flag)
        disparity_ref_view_id ue(v) 
    if (z_near_flag)
        depth_rep_info_element(ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen)
    if (z_far_flag)
        depth_rep_info_element(ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen)
    if (d_min_flag)
        depth_rep_info_element(DMinSign, DMinExp, DMinMantissa, DMinManLen)
    if (d_max_flag)
        depth_rep_info_element(DMaxSign, DMaxExp, DMaxMantissa, DMaxManLen)
    if (depth_representation_type == 3) {
        depth_nonlinear_representation_num_minus1 ue(v)
        for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
            depth_nonlinear_representation_model[i]  ue(v)
    }
}
    */
    public class DepthRepresentationInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte z_near_flag;
        public byte zNearFlag { get { return z_near_flag; } set { z_near_flag = value; } }
        private byte z_far_flag;
        public byte zFarFlag { get { return z_far_flag; } set { z_far_flag = value; } }
        private byte d_min_flag;
        public byte dMinFlag { get { return d_min_flag; } set { d_min_flag = value; } }
        private byte d_max_flag;
        public byte dMaxFlag { get { return d_max_flag; } set { d_max_flag = value; } }
        private uint depth_representation_type;
        public uint DepthRepresentationType { get { return depth_representation_type; } set { depth_representation_type = value; } }
        private uint disparity_ref_view_id;
        public uint DisparityRefViewId { get { return disparity_ref_view_id; } set { disparity_ref_view_id = value; } }
        private DepthRepInfoElement depth_rep_info_element;
        public DepthRepInfoElement DepthRepInfoElement { get { return depth_rep_info_element; } set { depth_rep_info_element = value; } }
        private DepthRepInfoElement depth_rep_info_element0;
        public DepthRepInfoElement DepthRepInfoElement0 { get { return depth_rep_info_element0; } set { depth_rep_info_element0 = value; } }
        private DepthRepInfoElement depth_rep_info_element1;
        public DepthRepInfoElement DepthRepInfoElement1 { get { return depth_rep_info_element1; } set { depth_rep_info_element1 = value; } }
        private DepthRepInfoElement depth_rep_info_element2;
        public DepthRepInfoElement DepthRepInfoElement2 { get { return depth_rep_info_element2; } set { depth_rep_info_element2 = value; } }
        private uint depth_nonlinear_representation_num_minus1;
        public uint DepthNonlinearRepresentationNumMinus1 { get { return depth_nonlinear_representation_num_minus1; } set { depth_nonlinear_representation_num_minus1 = value; } }
        private uint[] depth_nonlinear_representation_model;
        public uint[] DepthNonlinearRepresentationModel { get { return depth_nonlinear_representation_model; } set { depth_nonlinear_representation_model = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthRepresentationInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.z_near_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.z_far_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.d_min_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.d_max_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.depth_representation_type);

            if (d_min_flag != 0 || d_max_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.disparity_ref_view_id);
            }

            if (z_near_flag != 0)
            {
                this.depth_rep_info_element = new DepthRepInfoElement();
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element);
            }

            if (z_far_flag != 0)
            {
                this.depth_rep_info_element0 = new DepthRepInfoElement();
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element0);
            }

            if (d_min_flag != 0)
            {
                this.depth_rep_info_element1 = new DepthRepInfoElement();
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element1);
            }

            if (d_max_flag != 0)
            {
                this.depth_rep_info_element2 = new DepthRepInfoElement();
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element2);
            }

            if (depth_representation_type == 3)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.depth_nonlinear_representation_num_minus1);

                this.depth_nonlinear_representation_model = new uint[depth_nonlinear_representation_num_minus1 + 1 + 1];
                for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.depth_nonlinear_representation_model[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.z_near_flag);
            size += stream.WriteUnsignedInt(1, this.z_far_flag);
            size += stream.WriteUnsignedInt(1, this.d_min_flag);
            size += stream.WriteUnsignedInt(1, this.d_max_flag);
            size += stream.WriteUnsignedIntGolomb(this.depth_representation_type);

            if (d_min_flag != 0 || d_max_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.disparity_ref_view_id);
            }

            if (z_near_flag != 0)
            {
                size += stream.WriteClass<DepthRepInfoElement>(context, this.depth_rep_info_element);
            }

            if (z_far_flag != 0)
            {
                size += stream.WriteClass<DepthRepInfoElement>(context, this.depth_rep_info_element0);
            }

            if (d_min_flag != 0)
            {
                size += stream.WriteClass<DepthRepInfoElement>(context, this.depth_rep_info_element1);
            }

            if (d_max_flag != 0)
            {
                size += stream.WriteClass<DepthRepInfoElement>(context, this.depth_rep_info_element2);
            }

            if (depth_representation_type == 3)
            {
                size += stream.WriteUnsignedIntGolomb(this.depth_nonlinear_representation_num_minus1);

                for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.depth_nonlinear_representation_model[i]);
                }
            }

            return size;
        }

    }

    /*
  

depth_rep_info_element(OutSign, OutExp, OutMantissa, OutManLen) {
    da_sign_flag u(1)
    da_exponent u(7)
    da_mantissa_len_minus1 u(5)
    da_mantissa u(v)  
}
    */
    public class DepthRepInfoElement : IItuSerializable
    {
        private byte da_sign_flag;
        public byte DaSignFlag { get { return da_sign_flag; } set { da_sign_flag = value; } }
        private uint da_exponent;
        public uint DaExponent { get { return da_exponent; } set { da_exponent = value; } }
        private uint da_mantissa_len_minus1;
        public uint DaMantissaLenMinus1 { get { return da_mantissa_len_minus1; } set { da_mantissa_len_minus1 = value; } }
        private uint da_mantissa;
        public uint DaMantissa { get { return da_mantissa; } set { da_mantissa = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DepthRepInfoElement()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.da_sign_flag);
            size += stream.ReadUnsignedInt(size, 7, out this.da_exponent);
            size += stream.ReadUnsignedInt(size, 5, out this.da_mantissa_len_minus1);
            size += stream.ReadUnsignedIntVariable(size, (this.da_mantissa_len_minus1 + 1), out this.da_mantissa);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.da_sign_flag);
            size += stream.WriteUnsignedInt(7, this.da_exponent);
            size += stream.WriteUnsignedInt(5, this.da_mantissa_len_minus1);
            size += stream.WriteUnsignedIntVariable((this.da_mantissa_len_minus1 + 1), this.da_mantissa);

            return size;
        }

    }

    /*


alpha_channel_info(payloadSize) {
    alpha_channel_cancel_flag u(1)
    if (!alpha_channel_cancel_flag) {
        alpha_channel_use_idc u(3)
        alpha_channel_bit_depth_minus8 u(3)
        alpha_transparent_value u(v)
        alpha_opaque_value u(v)
        alpha_channel_incr_flag u(1)
        alpha_channel_clip_flag u(1)
        if (alpha_channel_clip_flag)
            alpha_channel_clip_type_flag u(1)
    }
}
    */
    public class AlphaChannelInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte alpha_channel_cancel_flag;
        public byte AlphaChannelCancelFlag { get { return alpha_channel_cancel_flag; } set { alpha_channel_cancel_flag = value; } }
        private uint alpha_channel_use_idc;
        public uint AlphaChannelUseIdc { get { return alpha_channel_use_idc; } set { alpha_channel_use_idc = value; } }
        private uint alpha_channel_bit_depth_minus8;
        public uint AlphaChannelBitDepthMinus8 { get { return alpha_channel_bit_depth_minus8; } set { alpha_channel_bit_depth_minus8 = value; } }
        private uint alpha_transparent_value;
        public uint AlphaTransparentValue { get { return alpha_transparent_value; } set { alpha_transparent_value = value; } }
        private uint alpha_opaque_value;
        public uint AlphaOpaqueValue { get { return alpha_opaque_value; } set { alpha_opaque_value = value; } }
        private byte alpha_channel_incr_flag;
        public byte AlphaChannelIncrFlag { get { return alpha_channel_incr_flag; } set { alpha_channel_incr_flag = value; } }
        private byte alpha_channel_clip_flag;
        public byte AlphaChannelClipFlag { get { return alpha_channel_clip_flag; } set { alpha_channel_clip_flag = value; } }
        private byte alpha_channel_clip_type_flag;
        public byte AlphaChannelClipTypeFlag { get { return alpha_channel_clip_type_flag; } set { alpha_channel_clip_type_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AlphaChannelInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.alpha_channel_cancel_flag);

            if (alpha_channel_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.alpha_channel_use_idc);
                size += stream.ReadUnsignedInt(size, 3, out this.alpha_channel_bit_depth_minus8);
                size += stream.ReadUnsignedIntVariable(size, alpha_channel_bit_depth_minus8 + 9, out this.alpha_transparent_value);
                size += stream.ReadUnsignedIntVariable(size, alpha_channel_bit_depth_minus8 + 9, out this.alpha_opaque_value);
                size += stream.ReadUnsignedInt(size, 1, out this.alpha_channel_incr_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.alpha_channel_clip_flag);

                if (alpha_channel_clip_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.alpha_channel_clip_type_flag);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.alpha_channel_cancel_flag);

            if (alpha_channel_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(3, this.alpha_channel_use_idc);
                size += stream.WriteUnsignedInt(3, this.alpha_channel_bit_depth_minus8);
                size += stream.WriteUnsignedIntVariable(alpha_channel_bit_depth_minus8 + 9, this.alpha_transparent_value);
                size += stream.WriteUnsignedIntVariable(alpha_channel_bit_depth_minus8 + 9, this.alpha_opaque_value);
                size += stream.WriteUnsignedInt(1, this.alpha_channel_incr_flag);
                size += stream.WriteUnsignedInt(1, this.alpha_channel_clip_flag);

                if (alpha_channel_clip_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.alpha_channel_clip_type_flag);
                }
            }

            return size;
        }

    }

    /*
  

display_orientation(payloadSize) {
    display_orientation_cancel_flag u(1)
    if (!display_orientation_cancel_flag) {
        display_orientation_persistence_flag u(1)
        display_orientation_transform_type u(3)
        display_orientation_reserved_zero_3bits u(3)
    }
}
    */
    public class DisplayOrientation : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte display_orientation_cancel_flag;
        public byte DisplayOrientationCancelFlag { get { return display_orientation_cancel_flag; } set { display_orientation_cancel_flag = value; } }
        private byte display_orientation_persistence_flag;
        public byte DisplayOrientationPersistenceFlag { get { return display_orientation_persistence_flag; } set { display_orientation_persistence_flag = value; } }
        private uint display_orientation_transform_type;
        public uint DisplayOrientationTransformType { get { return display_orientation_transform_type; } set { display_orientation_transform_type = value; } }
        private uint display_orientation_reserved_zero_3bits;
        public uint DisplayOrientationReservedZero3bits { get { return display_orientation_reserved_zero_3bits; } set { display_orientation_reserved_zero_3bits = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DisplayOrientation(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.display_orientation_cancel_flag);

            if (display_orientation_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.display_orientation_persistence_flag);
                size += stream.ReadUnsignedInt(size, 3, out this.display_orientation_transform_type);
                size += stream.ReadUnsignedInt(size, 3, out this.display_orientation_reserved_zero_3bits);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.display_orientation_cancel_flag);

            if (display_orientation_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.display_orientation_persistence_flag);
                size += stream.WriteUnsignedInt(3, this.display_orientation_transform_type);
                size += stream.WriteUnsignedInt(3, this.display_orientation_reserved_zero_3bits);
            }

            return size;
        }

    }

    /*


colour_transform_info(payloadSize) {
    colour_transform_id ue(v)
    colour_transform_cancel_flag u(1)      
    if (!colour_transform_cancel_flag) {
        colour_transform_persistence_flag u(1)
        colour_transform_video_signal_info_present_flag u(1)
        if (colour_transform_video_signal_info_present_flag) {
            colour_transform_full_range_flag u(1)
            colour_tranform_primaries u(8)
            colour_transform_transfer_function u(8)
            colour_transform_matrix_coefficients u(8)
        }
        colour_transform_bit_depth_minus8 u(4)
        colour_transform_log2_number_of_points_per_lut_minus1 u(3)
        colour_transform_cross_component_flag u(1)
        if (colour_transform_cross_component_flag )
            colour_transform_cross_comp_inferred_flag u(1)
        for (i = 0; i < colourTransformSize; i++)
            colour_transf_lut[0][i] u(v)
        if (colour_transform_cross_component_flag == 0 ||
            colour_transform_cross_comp_inferred_flag == 0) {
            colour_transform_lut2_present_flag u(1)
            for (i = 0; i < colourTransformSize; i++)
                colour_transf_lut[1][i] u(1)
            if (colour_transform_lut2_present_flag)
                for (i = 0; i < colourTransformSize; i++)
                    colour_transf_lut[2][i] u(v)
        } else { 
            colour_transform_chroma_offset u(v)
        }
    }
}
    */
    public class ColourTransformInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint colour_transform_id;
        public uint ColourTransformId { get { return colour_transform_id; } set { colour_transform_id = value; } }
        private byte colour_transform_cancel_flag;
        public byte ColourTransformCancelFlag { get { return colour_transform_cancel_flag; } set { colour_transform_cancel_flag = value; } }
        private byte colour_transform_persistence_flag;
        public byte ColourTransformPersistenceFlag { get { return colour_transform_persistence_flag; } set { colour_transform_persistence_flag = value; } }
        private byte colour_transform_video_signal_info_present_flag;
        public byte ColourTransformVideoSignalInfoPresentFlag { get { return colour_transform_video_signal_info_present_flag; } set { colour_transform_video_signal_info_present_flag = value; } }
        private byte colour_transform_full_range_flag;
        public byte ColourTransformFullRangeFlag { get { return colour_transform_full_range_flag; } set { colour_transform_full_range_flag = value; } }
        private uint colour_tranform_primaries;
        public uint ColourTranformPrimaries { get { return colour_tranform_primaries; } set { colour_tranform_primaries = value; } }
        private uint colour_transform_transfer_function;
        public uint ColourTransformTransferFunction { get { return colour_transform_transfer_function; } set { colour_transform_transfer_function = value; } }
        private uint colour_transform_matrix_coefficients;
        public uint ColourTransformMatrixCoefficients { get { return colour_transform_matrix_coefficients; } set { colour_transform_matrix_coefficients = value; } }
        private uint colour_transform_bit_depth_minus8;
        public uint ColourTransformBitDepthMinus8 { get { return colour_transform_bit_depth_minus8; } set { colour_transform_bit_depth_minus8 = value; } }
        private uint colour_transform_log2_number_of_points_per_lut_minus1;
        public uint ColourTransformLog2NumberOfPointsPerLutMinus1 { get { return colour_transform_log2_number_of_points_per_lut_minus1; } set { colour_transform_log2_number_of_points_per_lut_minus1 = value; } }
        private byte colour_transform_cross_component_flag;
        public byte ColourTransformCrossComponentFlag { get { return colour_transform_cross_component_flag; } set { colour_transform_cross_component_flag = value; } }
        private byte colour_transform_cross_comp_inferred_flag;
        public byte ColourTransformCrossCompInferredFlag { get { return colour_transform_cross_comp_inferred_flag; } set { colour_transform_cross_comp_inferred_flag = value; } }
        private uint[][] colour_transf_lut;
        public uint[][] ColourTransfLut { get { return colour_transf_lut; } set { colour_transf_lut = value; } }
        private byte colour_transform_lut2_present_flag;
        public byte ColourTransformLut2PresentFlag { get { return colour_transform_lut2_present_flag; } set { colour_transform_lut2_present_flag = value; } }
        private uint colour_transform_chroma_offset;
        public uint ColourTransformChromaOffset { get { return colour_transform_chroma_offset; } set { colour_transform_chroma_offset = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ColourTransformInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.colour_transform_id);
            size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_cancel_flag);

            if (colour_transform_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_video_signal_info_present_flag);

                if (colour_transform_video_signal_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_full_range_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_tranform_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_transform_transfer_function);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_transform_matrix_coefficients);
                }
                size += stream.ReadUnsignedInt(size, 4, out this.colour_transform_bit_depth_minus8);
                size += stream.ReadUnsignedInt(size, 3, out this.colour_transform_log2_number_of_points_per_lut_minus1);
                size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_cross_component_flag);

                if (colour_transform_cross_component_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_cross_comp_inferred_flag);
                }

                this.colour_transf_lut = new uint[((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1 + 1)) + 1)][];
                for (i = 0; i < ((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1)) + 1); i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, (uint)(2 + (colour_transform_bit_depth_minus8 + 8) - (colour_transform_log2_number_of_points_per_lut_minus1 + 1)), out this.colour_transf_lut[0][i]);
                }

                if (colour_transform_cross_component_flag == 0 ||
            colour_transform_cross_comp_inferred_flag == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.colour_transform_lut2_present_flag);

                    for (i = 0; i < ((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1)) + 1); i++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.colour_transf_lut[1][i]);
                    }

                    if (colour_transform_lut2_present_flag != 0)
                    {

                        for (i = 0; i < ((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1)) + 1); i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (uint)(2 + (colour_transform_bit_depth_minus8 + 8) - (colour_transform_log2_number_of_points_per_lut_minus1 + 1)), out this.colour_transf_lut[2][i]);
                        }
                    }
                }
                else
                {
                    size += stream.ReadUnsignedIntVariable(size, (uint)(2 + (colour_transform_bit_depth_minus8 + 8) - (colour_transform_log2_number_of_points_per_lut_minus1 + 1)), out this.colour_transform_chroma_offset);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.colour_transform_id);
            size += stream.WriteUnsignedInt(1, this.colour_transform_cancel_flag);

            if (colour_transform_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.colour_transform_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.colour_transform_video_signal_info_present_flag);

                if (colour_transform_video_signal_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.colour_transform_full_range_flag);
                    size += stream.WriteUnsignedInt(8, this.colour_tranform_primaries);
                    size += stream.WriteUnsignedInt(8, this.colour_transform_transfer_function);
                    size += stream.WriteUnsignedInt(8, this.colour_transform_matrix_coefficients);
                }
                size += stream.WriteUnsignedInt(4, this.colour_transform_bit_depth_minus8);
                size += stream.WriteUnsignedInt(3, this.colour_transform_log2_number_of_points_per_lut_minus1);
                size += stream.WriteUnsignedInt(1, this.colour_transform_cross_component_flag);

                if (colour_transform_cross_component_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.colour_transform_cross_comp_inferred_flag);
                }

                for (i = 0; i < ((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1)) + 1); i++)
                {
                    size += stream.WriteUnsignedIntVariable((uint)(2 + (colour_transform_bit_depth_minus8 + 8) - (colour_transform_log2_number_of_points_per_lut_minus1 + 1)), this.colour_transf_lut[0][i]);
                }

                if (colour_transform_cross_component_flag == 0 ||
            colour_transform_cross_comp_inferred_flag == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.colour_transform_lut2_present_flag);

                    for (i = 0; i < ((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1)) + 1); i++)
                    {
                        size += stream.WriteUnsignedInt(1, this.colour_transf_lut[1][i]);
                    }

                    if (colour_transform_lut2_present_flag != 0)
                    {

                        for (i = 0; i < ((1 << ((int)colour_transform_log2_number_of_points_per_lut_minus1 + 1)) + 1); i++)
                        {
                            size += stream.WriteUnsignedIntVariable((uint)(2 + (colour_transform_bit_depth_minus8 + 8) - (colour_transform_log2_number_of_points_per_lut_minus1 + 1)), this.colour_transf_lut[2][i]);
                        }
                    }
                }
                else
                {
                    size += stream.WriteUnsignedIntVariable((uint)(2 + (colour_transform_bit_depth_minus8 + 8) - (colour_transform_log2_number_of_points_per_lut_minus1 + 1)), this.colour_transform_chroma_offset);
                }
            }

            return size;
        }

    }

    /*
  

shutter_interval_info(payloadSize) {
    sii_time_scale  u(32)
    sii_fixed_shutter_interval_within_clvs_flag u(1)
    if (sii_fixed_shutter_interval_within_clvs_flag)  
        sii_num_units_in_shutter_interval u(32)
    else {
        sii_max_sub_layers_minus1 u(3)
        for (i = 0; i <= sii_max_sub_layers_minus1; i++)
            sii_sub_layer_num_units_in_shutter_interval[i] u(32)
    }
}
    */
    public class ShutterIntervalInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sii_time_scale;
        public uint SiiTimeScale { get { return sii_time_scale; } set { sii_time_scale = value; } }
        private byte sii_fixed_shutter_interval_within_clvs_flag;
        public byte SiiFixedShutterIntervalWithinClvsFlag { get { return sii_fixed_shutter_interval_within_clvs_flag; } set { sii_fixed_shutter_interval_within_clvs_flag = value; } }
        private uint sii_num_units_in_shutter_interval;
        public uint SiiNumUnitsInShutterInterval { get { return sii_num_units_in_shutter_interval; } set { sii_num_units_in_shutter_interval = value; } }
        private uint sii_max_sub_layers_minus1;
        public uint SiiMaxSubLayersMinus1 { get { return sii_max_sub_layers_minus1; } set { sii_max_sub_layers_minus1 = value; } }
        private uint[] sii_sub_layer_num_units_in_shutter_interval;
        public uint[] SiiSubLayerNumUnitsInShutterInterval { get { return sii_sub_layer_num_units_in_shutter_interval; } set { sii_sub_layer_num_units_in_shutter_interval = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ShutterIntervalInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 32, out this.sii_time_scale);
            size += stream.ReadUnsignedInt(size, 1, out this.sii_fixed_shutter_interval_within_clvs_flag);

            if (sii_fixed_shutter_interval_within_clvs_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 32, out this.sii_num_units_in_shutter_interval);
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 3, out this.sii_max_sub_layers_minus1);

                this.sii_sub_layer_num_units_in_shutter_interval = new uint[sii_max_sub_layers_minus1 + 1];
                for (i = 0; i <= sii_max_sub_layers_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.sii_sub_layer_num_units_in_shutter_interval[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(32, this.sii_time_scale);
            size += stream.WriteUnsignedInt(1, this.sii_fixed_shutter_interval_within_clvs_flag);

            if (sii_fixed_shutter_interval_within_clvs_flag != 0)
            {
                size += stream.WriteUnsignedInt(32, this.sii_num_units_in_shutter_interval);
            }
            else
            {
                size += stream.WriteUnsignedInt(3, this.sii_max_sub_layers_minus1);

                for (i = 0; i <= sii_max_sub_layers_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(32, this.sii_sub_layer_num_units_in_shutter_interval[i]);
                }
            }

            return size;
        }

    }

    /*


phase_indication(payloadSize) {
    pi_hor_phase_num u(8)
    pi_hor_phase_den_minus1 u(8)
    pi_ver_phase_num u(8)
    pi_ver_phase_den_minus1 u(8)
}
    */
    public class PhaseIndication : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint pi_hor_phase_num;
        public uint PiHorPhaseNum { get { return pi_hor_phase_num; } set { pi_hor_phase_num = value; } }
        private uint pi_hor_phase_den_minus1;
        public uint PiHorPhaseDenMinus1 { get { return pi_hor_phase_den_minus1; } set { pi_hor_phase_den_minus1 = value; } }
        private uint pi_ver_phase_num;
        public uint PiVerPhaseNum { get { return pi_ver_phase_num; } set { pi_ver_phase_num = value; } }
        private uint pi_ver_phase_den_minus1;
        public uint PiVerPhaseDenMinus1 { get { return pi_ver_phase_den_minus1; } set { pi_ver_phase_den_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PhaseIndication(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.pi_hor_phase_num);
            size += stream.ReadUnsignedInt(size, 8, out this.pi_hor_phase_den_minus1);
            size += stream.ReadUnsignedInt(size, 8, out this.pi_ver_phase_num);
            size += stream.ReadUnsignedInt(size, 8, out this.pi_ver_phase_den_minus1);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.pi_hor_phase_num);
            size += stream.WriteUnsignedInt(8, this.pi_hor_phase_den_minus1);
            size += stream.WriteUnsignedInt(8, this.pi_ver_phase_num);
            size += stream.WriteUnsignedInt(8, this.pi_ver_phase_den_minus1);

            return size;
        }

    }

    /*


reserved_message(payloadSize) {
    for (i = 0; i < payloadSize; i++)
        reserved_message_payload_byte u(8)    
}
    */
    public class ReservedMessage : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] reserved_message_payload_byte;
        public uint[] ReservedMessagePayloadByte { get { return reserved_message_payload_byte; } set { reserved_message_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ReservedMessage(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            this.reserved_message_payload_byte = new uint[payloadSize];
            for (i = 0; i < payloadSize; i++)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.reserved_message_payload_byte[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            for (i = 0; i < payloadSize; i++)
            {
                size += stream.WriteUnsignedInt(8, this.reserved_message_payload_byte[i]);
            }

            return size;
        }

    }

    /*


buffering_period( payloadSize ) {  
 bp_nal_hrd_params_present_flag u(1) 
 bp_vcl_hrd_params_present_flag u(1) 
 bp_cpb_initial_removal_delay_length_minus1 u(5) 
 bp_cpb_removal_delay_length_minus1 u(5) 
 bp_dpb_output_delay_length_minus1 u(5) 
 bp_du_hrd_params_present_flag u(1) 
 if( bp_du_hrd_params_present_flag ) {  
  bp_du_cpb_removal_delay_increment_length_minus1 u(5) 
  bp_dpb_output_delay_du_length_minus1 u(5) 
  bp_du_cpb_params_in_pic_timing_sei_flag u(1) 
  bp_du_dpb_params_in_pic_timing_sei_flag u(1) 
 }  
 bp_concatenation_flag u(1) 
 bp_additional_concatenation_info_present_flag u(1) 
 if( bp_additional_concatenation_info_present_flag )  
  bp_max_initial_removal_delay_for_concatenation u(v) 
 bp_cpb_removal_delay_delta_minus1 u(v) 
 bp_max_sublayers_minus1 u(3) 
 if( bp_max_sublayers_minus1 > 0 )  
  bp_cpb_removal_delay_deltas_present_flag u(1) 
 if( bp_cpb_removal_delay_deltas_present_flag ) {  
  bp_num_cpb_removal_delay_deltas_minus1 ue(v) 
  for( i = 0; i  <=  bp_num_cpb_removal_delay_deltas_minus1; i++ )  
   bp_cpb_removal_delay_delta_val[ i ] u(v) 
 }  
 bp_cpb_cnt_minus1 ue(v) 
 if( bp_max_sublayers_minus1 > 0 )  
  bp_sublayer_initial_cpb_removal_delay_present_flag u(1) 
 for( i = ( bp_sublayer_initial_cpb_removal_delay_present_flag != 0 ? 
   0 : bp_max_sublayers_minus1 ); i  <=  bp_max_sublayers_minus1; i++ ) { 
 
  if( bp_nal_hrd_params_present_flag )  
   for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
    bp_nal_initial_cpb_removal_delay[ i ][ j ] u(v) 
    bp_nal_initial_cpb_removal_offset[ i ][ j ] u(v) 
    if( bp_du_hrd_params_present_flag) {  
     bp_nal_initial_alt_cpb_removal_delay[ i ][ j ] u(v) 
     bp_nal_initial_alt_cpb_removal_offset[ i ][ j ] u(v) 
    }  
   }  
  if( bp_vcl_hrd_params_present_flag )  
     for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
    bp_vcl_initial_cpb_removal_delay[ i ][ j ] u(v) 
    bp_vcl_initial_cpb_removal_offset[ i ][ j ] u(v) 
    if( bp_du_hrd_params_present_flag ) {  
     bp_vcl_initial_alt_cpb_removal_delay[ i ][ j ] u(v) 
     bp_vcl_initial_alt_cpb_removal_offset[ i ][ j ] u(v) 
    }  
  }  
 }  
 if( bp_max_sublayers_minus1 > 0 )  
  bp_sublayer_dpb_output_offsets_present_flag u(1) 
 if( bp_sublayer_dpb_output_offsets_present_flag )  
  for( i = 0; i < bp_max_sublayers_minus1; i++ )  
   bp_dpb_output_tid_offset[ i ] ue(v) 
 bp_alt_cpb_params_present_flag u(1) 
 if( bp_alt_cpb_params_present_flag )  
  bp_use_alt_cpb_params_flag u(1) 
}
    */
    public class BufferingPeriod : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte bp_nal_hrd_params_present_flag;
        public byte BpNalHrdParamsPresentFlag { get { return bp_nal_hrd_params_present_flag; } set { bp_nal_hrd_params_present_flag = value; } }
        private byte bp_vcl_hrd_params_present_flag;
        public byte BpVclHrdParamsPresentFlag { get { return bp_vcl_hrd_params_present_flag; } set { bp_vcl_hrd_params_present_flag = value; } }
        private uint bp_cpb_initial_removal_delay_length_minus1;
        public uint BpCpbInitialRemovalDelayLengthMinus1 { get { return bp_cpb_initial_removal_delay_length_minus1; } set { bp_cpb_initial_removal_delay_length_minus1 = value; } }
        private uint bp_cpb_removal_delay_length_minus1;
        public uint BpCpbRemovalDelayLengthMinus1 { get { return bp_cpb_removal_delay_length_minus1; } set { bp_cpb_removal_delay_length_minus1 = value; } }
        private uint bp_dpb_output_delay_length_minus1;
        public uint BpDpbOutputDelayLengthMinus1 { get { return bp_dpb_output_delay_length_minus1; } set { bp_dpb_output_delay_length_minus1 = value; } }
        private byte bp_du_hrd_params_present_flag;
        public byte BpDuHrdParamsPresentFlag { get { return bp_du_hrd_params_present_flag; } set { bp_du_hrd_params_present_flag = value; } }
        private uint bp_du_cpb_removal_delay_increment_length_minus1;
        public uint BpDuCpbRemovalDelayIncrementLengthMinus1 { get { return bp_du_cpb_removal_delay_increment_length_minus1; } set { bp_du_cpb_removal_delay_increment_length_minus1 = value; } }
        private uint bp_dpb_output_delay_du_length_minus1;
        public uint BpDpbOutputDelayDuLengthMinus1 { get { return bp_dpb_output_delay_du_length_minus1; } set { bp_dpb_output_delay_du_length_minus1 = value; } }
        private byte bp_du_cpb_params_in_pic_timing_sei_flag;
        public byte BpDuCpbParamsInPicTimingSeiFlag { get { return bp_du_cpb_params_in_pic_timing_sei_flag; } set { bp_du_cpb_params_in_pic_timing_sei_flag = value; } }
        private byte bp_du_dpb_params_in_pic_timing_sei_flag;
        public byte BpDuDpbParamsInPicTimingSeiFlag { get { return bp_du_dpb_params_in_pic_timing_sei_flag; } set { bp_du_dpb_params_in_pic_timing_sei_flag = value; } }
        private byte bp_concatenation_flag;
        public byte BpConcatenationFlag { get { return bp_concatenation_flag; } set { bp_concatenation_flag = value; } }
        private byte bp_additional_concatenation_info_present_flag;
        public byte BpAdditionalConcatenationInfoPresentFlag { get { return bp_additional_concatenation_info_present_flag; } set { bp_additional_concatenation_info_present_flag = value; } }
        private uint bp_max_initial_removal_delay_for_concatenation;
        public uint BpMaxInitialRemovalDelayForConcatenation { get { return bp_max_initial_removal_delay_for_concatenation; } set { bp_max_initial_removal_delay_for_concatenation = value; } }
        private uint bp_cpb_removal_delay_delta_minus1;
        public uint BpCpbRemovalDelayDeltaMinus1 { get { return bp_cpb_removal_delay_delta_minus1; } set { bp_cpb_removal_delay_delta_minus1 = value; } }
        private uint bp_max_sublayers_minus1;
        public uint BpMaxSublayersMinus1 { get { return bp_max_sublayers_minus1; } set { bp_max_sublayers_minus1 = value; } }
        private byte bp_cpb_removal_delay_deltas_present_flag;
        public byte BpCpbRemovalDelayDeltasPresentFlag { get { return bp_cpb_removal_delay_deltas_present_flag; } set { bp_cpb_removal_delay_deltas_present_flag = value; } }
        private uint bp_num_cpb_removal_delay_deltas_minus1;
        public uint BpNumCpbRemovalDelayDeltasMinus1 { get { return bp_num_cpb_removal_delay_deltas_minus1; } set { bp_num_cpb_removal_delay_deltas_minus1 = value; } }
        private uint[] bp_cpb_removal_delay_delta_val;
        public uint[] BpCpbRemovalDelayDeltaVal { get { return bp_cpb_removal_delay_delta_val; } set { bp_cpb_removal_delay_delta_val = value; } }
        private uint bp_cpb_cnt_minus1;
        public uint BpCpbCntMinus1 { get { return bp_cpb_cnt_minus1; } set { bp_cpb_cnt_minus1 = value; } }
        private byte bp_sublayer_initial_cpb_removal_delay_present_flag;
        public byte BpSublayerInitialCpbRemovalDelayPresentFlag { get { return bp_sublayer_initial_cpb_removal_delay_present_flag; } set { bp_sublayer_initial_cpb_removal_delay_present_flag = value; } }
        private uint[][] bp_nal_initial_cpb_removal_delay;
        public uint[][] BpNalInitialCpbRemovalDelay { get { return bp_nal_initial_cpb_removal_delay; } set { bp_nal_initial_cpb_removal_delay = value; } }
        private uint[][] bp_nal_initial_cpb_removal_offset;
        public uint[][] BpNalInitialCpbRemovalOffset { get { return bp_nal_initial_cpb_removal_offset; } set { bp_nal_initial_cpb_removal_offset = value; } }
        private uint[][] bp_nal_initial_alt_cpb_removal_delay;
        public uint[][] BpNalInitialAltCpbRemovalDelay { get { return bp_nal_initial_alt_cpb_removal_delay; } set { bp_nal_initial_alt_cpb_removal_delay = value; } }
        private uint[][] bp_nal_initial_alt_cpb_removal_offset;
        public uint[][] BpNalInitialAltCpbRemovalOffset { get { return bp_nal_initial_alt_cpb_removal_offset; } set { bp_nal_initial_alt_cpb_removal_offset = value; } }
        private uint[][] bp_vcl_initial_cpb_removal_delay;
        public uint[][] BpVclInitialCpbRemovalDelay { get { return bp_vcl_initial_cpb_removal_delay; } set { bp_vcl_initial_cpb_removal_delay = value; } }
        private uint[][] bp_vcl_initial_cpb_removal_offset;
        public uint[][] BpVclInitialCpbRemovalOffset { get { return bp_vcl_initial_cpb_removal_offset; } set { bp_vcl_initial_cpb_removal_offset = value; } }
        private uint[][] bp_vcl_initial_alt_cpb_removal_delay;
        public uint[][] BpVclInitialAltCpbRemovalDelay { get { return bp_vcl_initial_alt_cpb_removal_delay; } set { bp_vcl_initial_alt_cpb_removal_delay = value; } }
        private uint[][] bp_vcl_initial_alt_cpb_removal_offset;
        public uint[][] BpVclInitialAltCpbRemovalOffset { get { return bp_vcl_initial_alt_cpb_removal_offset; } set { bp_vcl_initial_alt_cpb_removal_offset = value; } }
        private byte bp_sublayer_dpb_output_offsets_present_flag;
        public byte BpSublayerDpbOutputOffsetsPresentFlag { get { return bp_sublayer_dpb_output_offsets_present_flag; } set { bp_sublayer_dpb_output_offsets_present_flag = value; } }
        private uint[] bp_dpb_output_tid_offset;
        public uint[] BpDpbOutputTidOffset { get { return bp_dpb_output_tid_offset; } set { bp_dpb_output_tid_offset = value; } }
        private byte bp_alt_cpb_params_present_flag;
        public byte BpAltCpbParamsPresentFlag { get { return bp_alt_cpb_params_present_flag; } set { bp_alt_cpb_params_present_flag = value; } }
        private byte bp_use_alt_cpb_params_flag;
        public byte BpUseAltCpbParamsFlag { get { return bp_use_alt_cpb_params_flag; } set { bp_use_alt_cpb_params_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public BufferingPeriod(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.bp_nal_hrd_params_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.bp_vcl_hrd_params_present_flag);
            size += stream.ReadUnsignedInt(size, 5, out this.bp_cpb_initial_removal_delay_length_minus1);
            size += stream.ReadUnsignedInt(size, 5, out this.bp_cpb_removal_delay_length_minus1);
            size += stream.ReadUnsignedInt(size, 5, out this.bp_dpb_output_delay_length_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.bp_du_hrd_params_present_flag);

            if (bp_du_hrd_params_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 5, out this.bp_du_cpb_removal_delay_increment_length_minus1);
                size += stream.ReadUnsignedInt(size, 5, out this.bp_dpb_output_delay_du_length_minus1);
                size += stream.ReadUnsignedInt(size, 1, out this.bp_du_cpb_params_in_pic_timing_sei_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.bp_du_dpb_params_in_pic_timing_sei_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.bp_concatenation_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.bp_additional_concatenation_info_present_flag);

            if (bp_additional_concatenation_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_max_initial_removal_delay_for_concatenation);
            }
            size += stream.ReadUnsignedIntVariable(size, bp_cpb_removal_delay_length_minus1 + 1, out this.bp_cpb_removal_delay_delta_minus1);
            size += stream.ReadUnsignedInt(size, 3, out this.bp_max_sublayers_minus1);

            if (bp_max_sublayers_minus1 > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.bp_cpb_removal_delay_deltas_present_flag);
            }

            if (bp_cpb_removal_delay_deltas_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.bp_num_cpb_removal_delay_deltas_minus1);

                this.bp_cpb_removal_delay_delta_val = new uint[bp_num_cpb_removal_delay_deltas_minus1 + 1];
                for (i = 0; i <= bp_num_cpb_removal_delay_deltas_minus1; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, bp_cpb_removal_delay_length_minus1 + 1, out this.bp_cpb_removal_delay_delta_val[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.bp_cpb_cnt_minus1);

            if (bp_max_sublayers_minus1 > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.bp_sublayer_initial_cpb_removal_delay_present_flag);
            }

            this.bp_nal_initial_cpb_removal_delay = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_nal_initial_cpb_removal_offset = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_nal_initial_alt_cpb_removal_delay = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_nal_initial_alt_cpb_removal_offset = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_vcl_initial_cpb_removal_delay = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_vcl_initial_cpb_removal_offset = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_vcl_initial_alt_cpb_removal_delay = new uint[bp_max_sublayers_minus1 + 1][];
            this.bp_vcl_initial_alt_cpb_removal_offset = new uint[bp_max_sublayers_minus1 + 1][];
            for (i = (bp_sublayer_initial_cpb_removal_delay_present_flag != 0 ?
   0 : bp_max_sublayers_minus1); i <= bp_max_sublayers_minus1; i++)
            {

                if (bp_nal_hrd_params_present_flag != 0)
                {

                    this.bp_nal_initial_cpb_removal_delay[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    this.bp_nal_initial_cpb_removal_offset[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    this.bp_nal_initial_alt_cpb_removal_delay[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    this.bp_nal_initial_alt_cpb_removal_offset[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    for (j = 0; j < bp_cpb_cnt_minus1 + 1; j++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_nal_initial_cpb_removal_delay[i][j]);
                        size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_nal_initial_cpb_removal_offset[i][j]);

                        if (bp_du_hrd_params_present_flag != 0)
                        {
                            size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_nal_initial_alt_cpb_removal_delay[i][j]);
                            size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_nal_initial_alt_cpb_removal_offset[i][j]);
                        }
                    }
                }

                if (bp_vcl_hrd_params_present_flag != 0)
                {

                    this.bp_vcl_initial_cpb_removal_delay[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    this.bp_vcl_initial_cpb_removal_offset[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    this.bp_vcl_initial_alt_cpb_removal_delay[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    this.bp_vcl_initial_alt_cpb_removal_offset[i] = new uint[bp_cpb_cnt_minus1 + 1 + 1];
                    for (j = 0; j < bp_cpb_cnt_minus1 + 1; j++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_vcl_initial_cpb_removal_delay[i][j]);
                        size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_vcl_initial_cpb_removal_offset[i][j]);

                        if (bp_du_hrd_params_present_flag != 0)
                        {
                            size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_vcl_initial_alt_cpb_removal_delay[i][j]);
                            size += stream.ReadUnsignedIntVariable(size, bp_cpb_initial_removal_delay_length_minus1 + 1, out this.bp_vcl_initial_alt_cpb_removal_offset[i][j]);
                        }
                    }
                }
            }

            if (bp_max_sublayers_minus1 > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.bp_sublayer_dpb_output_offsets_present_flag);
            }

            if (bp_sublayer_dpb_output_offsets_present_flag != 0)
            {

                this.bp_dpb_output_tid_offset = new uint[bp_max_sublayers_minus1 + 1];
                for (i = 0; i < bp_max_sublayers_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.bp_dpb_output_tid_offset[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.bp_alt_cpb_params_present_flag);

            if (bp_alt_cpb_params_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.bp_use_alt_cpb_params_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.bp_nal_hrd_params_present_flag);
            size += stream.WriteUnsignedInt(1, this.bp_vcl_hrd_params_present_flag);
            size += stream.WriteUnsignedInt(5, this.bp_cpb_initial_removal_delay_length_minus1);
            size += stream.WriteUnsignedInt(5, this.bp_cpb_removal_delay_length_minus1);
            size += stream.WriteUnsignedInt(5, this.bp_dpb_output_delay_length_minus1);
            size += stream.WriteUnsignedInt(1, this.bp_du_hrd_params_present_flag);

            if (bp_du_hrd_params_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(5, this.bp_du_cpb_removal_delay_increment_length_minus1);
                size += stream.WriteUnsignedInt(5, this.bp_dpb_output_delay_du_length_minus1);
                size += stream.WriteUnsignedInt(1, this.bp_du_cpb_params_in_pic_timing_sei_flag);
                size += stream.WriteUnsignedInt(1, this.bp_du_dpb_params_in_pic_timing_sei_flag);
            }
            size += stream.WriteUnsignedInt(1, this.bp_concatenation_flag);
            size += stream.WriteUnsignedInt(1, this.bp_additional_concatenation_info_present_flag);

            if (bp_additional_concatenation_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_max_initial_removal_delay_for_concatenation);
            }
            size += stream.WriteUnsignedIntVariable(bp_cpb_removal_delay_length_minus1 + 1, this.bp_cpb_removal_delay_delta_minus1);
            size += stream.WriteUnsignedInt(3, this.bp_max_sublayers_minus1);

            if (bp_max_sublayers_minus1 > 0)
            {
                size += stream.WriteUnsignedInt(1, this.bp_cpb_removal_delay_deltas_present_flag);
            }

            if (bp_cpb_removal_delay_deltas_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.bp_num_cpb_removal_delay_deltas_minus1);

                for (i = 0; i <= bp_num_cpb_removal_delay_deltas_minus1; i++)
                {
                    size += stream.WriteUnsignedIntVariable(bp_cpb_removal_delay_length_minus1 + 1, this.bp_cpb_removal_delay_delta_val[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.bp_cpb_cnt_minus1);

            if (bp_max_sublayers_minus1 > 0)
            {
                size += stream.WriteUnsignedInt(1, this.bp_sublayer_initial_cpb_removal_delay_present_flag);
            }

            for (i = (bp_sublayer_initial_cpb_removal_delay_present_flag != 0 ?
   0 : bp_max_sublayers_minus1); i <= bp_max_sublayers_minus1; i++)
            {

                if (bp_nal_hrd_params_present_flag != 0)
                {

                    for (j = 0; j < bp_cpb_cnt_minus1 + 1; j++)
                    {
                        size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_nal_initial_cpb_removal_delay[i][j]);
                        size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_nal_initial_cpb_removal_offset[i][j]);

                        if (bp_du_hrd_params_present_flag != 0)
                        {
                            size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_nal_initial_alt_cpb_removal_delay[i][j]);
                            size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_nal_initial_alt_cpb_removal_offset[i][j]);
                        }
                    }
                }

                if (bp_vcl_hrd_params_present_flag != 0)
                {

                    for (j = 0; j < bp_cpb_cnt_minus1 + 1; j++)
                    {
                        size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_vcl_initial_cpb_removal_delay[i][j]);
                        size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_vcl_initial_cpb_removal_offset[i][j]);

                        if (bp_du_hrd_params_present_flag != 0)
                        {
                            size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_vcl_initial_alt_cpb_removal_delay[i][j]);
                            size += stream.WriteUnsignedIntVariable(bp_cpb_initial_removal_delay_length_minus1 + 1, this.bp_vcl_initial_alt_cpb_removal_offset[i][j]);
                        }
                    }
                }
            }

            if (bp_max_sublayers_minus1 > 0)
            {
                size += stream.WriteUnsignedInt(1, this.bp_sublayer_dpb_output_offsets_present_flag);
            }

            if (bp_sublayer_dpb_output_offsets_present_flag != 0)
            {

                for (i = 0; i < bp_max_sublayers_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.bp_dpb_output_tid_offset[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.bp_alt_cpb_params_present_flag);

            if (bp_alt_cpb_params_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.bp_use_alt_cpb_params_flag);
            }

            return size;
        }

    }

    /*
 

pic_timing( payloadSize ) {  
 pt_cpb_removal_delay_minus1[ bp_max_sublayers_minus1 ] u(v) 
 for( i = TemporalId; i < bp_max_sublayers_minus1; i++ ) {  
  pt_sublayer_delays_present_flag[ i ] u(1) 
  if( pt_sublayer_delays_present_flag[ i ] ) {  
   if( bp_cpb_removal_delay_deltas_present_flag )  
    pt_cpb_removal_delay_delta_enabled_flag[ i ] u(1) 
   if( pt_cpb_removal_delay_delta_enabled_flag[ i ] ) {  
    if( bp_num_cpb_removal_delay_deltas_minus1 > 0 )  
         pt_cpb_removal_delay_delta_idx[ i ] u(v) 
   } else  
    pt_cpb_removal_delay_minus1[ i ] u(v) 
  }  
 }  
 pt_dpb_output_delay u(v) 
 if( bp_alt_cpb_params_present_flag ) {  
  pt_cpb_alt_timing_info_present_flag u(1) 
  if( pt_cpb_alt_timing_info_present_flag ) {  
   if( bp_nal_hrd_params_present_flag ) {  
    for( i = ( bp_sublayer_initial_cpb_removal_delay_present_flag != 0 ? 0 : 
      bp_max_sublayers_minus1 ); i  <=  bp_max_sublayers_minus1; i++ ) { 
 
     for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
      pt_nal_cpb_alt_initial_removal_delay_delta[ i ][ j ] u(v) 
      pt_nal_cpb_alt_initial_removal_offset_delta[ i ][ j ] u(v) 
     }  
     pt_nal_cpb_delay_offset[ i ] u(v) 
     pt_nal_dpb_delay_offset[ i ] u(v) 
    }  
   }  
   if( bp_vcl_hrd_params_present_flag ) {  
    for( i = ( bp_sublayer_initial_cpb_removal_delay_present_flag != 0 ? 0 : 
      bp_max_sublayers_minus1 ); i  <=  bp_max_sublayers_minus1; i++ ) { 
 
     for( j = 0; j < bp_cpb_cnt_minus1 + 1; j++ ) {  
      pt_vcl_cpb_alt_initial_removal_delay_delta[ i ][ j ] u(v) 
      pt_vcl_cpb_alt_initial_removal_offset_delta[ i ][ j ] u(v) 
     }  
     pt_vcl_cpb_delay_offset[ i ] u(v) 
     pt_vcl_dpb_delay_offset[ i ] u(v) 
    }  
   }  
  }  
 }  
 if( bp_du_hrd_params_present_flag  && 
   bp_du_dpb_params_in_pic_timing_sei_flag ) 
 
  pt_dpb_output_du_delay u(v) 
 if( bp_du_hrd_params_present_flag  && 
   bp_du_cpb_params_in_pic_timing_sei_flag ) { 
 
  pt_num_decoding_units_minus1 ue(v) 
  if( pt_num_decoding_units_minus1 > 0 ) {  
   pt_du_common_cpb_removal_delay_flag u(1) 
   if( pt_du_common_cpb_removal_delay_flag )  
    for( i = TemporalId; i  <=  bp_max_sublayers_minus1; i++ )  
     if( pt_sublayer_delays_present_flag[ i ] )  
      pt_du_common_cpb_removal_delay_increment_minus1[ i ] u(v) 
   for( i = 0; i  <=  pt_num_decoding_units_minus1; i++ ) {  
    pt_num_nalus_in_du_minus1[ i ] ue(v) 
    if( !pt_du_common_cpb_removal_delay_flag  && 
      i < pt_num_decoding_units_minus1 ) 
      for( j = TemporalId; j  <=  bp_max_sublayers_minus1; j++ )  
      if( pt_sublayer_delays_present_flag[ j ] )  
       pt_du_cpb_removal_delay_increment_minus1[ i ][ j ] u(v) 
   }  
  }  
 }  
 if( bp_additional_concatenation_info_present_flag )  
  pt_delay_for_concatenation_ensured_flag u(1) 
 pt_display_elemental_periods_minus1 u(8) 
}
    */
    public class PicTiming : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] pt_cpb_removal_delay_minus1;
        public uint[] PtCpbRemovalDelayMinus1 { get { return pt_cpb_removal_delay_minus1; } set { pt_cpb_removal_delay_minus1 = value; } }
        private byte[] pt_sublayer_delays_present_flag;
        public byte[] PtSublayerDelaysPresentFlag { get { return pt_sublayer_delays_present_flag; } set { pt_sublayer_delays_present_flag = value; } }
        private byte[] pt_cpb_removal_delay_delta_enabled_flag;
        public byte[] PtCpbRemovalDelayDeltaEnabledFlag { get { return pt_cpb_removal_delay_delta_enabled_flag; } set { pt_cpb_removal_delay_delta_enabled_flag = value; } }
        private uint[] pt_cpb_removal_delay_delta_idx;
        public uint[] PtCpbRemovalDelayDeltaIdx { get { return pt_cpb_removal_delay_delta_idx; } set { pt_cpb_removal_delay_delta_idx = value; } }
        private uint pt_dpb_output_delay;
        public uint PtDpbOutputDelay { get { return pt_dpb_output_delay; } set { pt_dpb_output_delay = value; } }
        private byte pt_cpb_alt_timing_info_present_flag;
        public byte PtCpbAltTimingInfoPresentFlag { get { return pt_cpb_alt_timing_info_present_flag; } set { pt_cpb_alt_timing_info_present_flag = value; } }
        private uint[][] pt_nal_cpb_alt_initial_removal_delay_delta;
        public uint[][] PtNalCpbAltInitialRemovalDelayDelta { get { return pt_nal_cpb_alt_initial_removal_delay_delta; } set { pt_nal_cpb_alt_initial_removal_delay_delta = value; } }
        private uint[][] pt_nal_cpb_alt_initial_removal_offset_delta;
        public uint[][] PtNalCpbAltInitialRemovalOffsetDelta { get { return pt_nal_cpb_alt_initial_removal_offset_delta; } set { pt_nal_cpb_alt_initial_removal_offset_delta = value; } }
        private uint[] pt_nal_cpb_delay_offset;
        public uint[] PtNalCpbDelayOffset { get { return pt_nal_cpb_delay_offset; } set { pt_nal_cpb_delay_offset = value; } }
        private uint[] pt_nal_dpb_delay_offset;
        public uint[] PtNalDpbDelayOffset { get { return pt_nal_dpb_delay_offset; } set { pt_nal_dpb_delay_offset = value; } }
        private uint[][] pt_vcl_cpb_alt_initial_removal_delay_delta;
        public uint[][] PtVclCpbAltInitialRemovalDelayDelta { get { return pt_vcl_cpb_alt_initial_removal_delay_delta; } set { pt_vcl_cpb_alt_initial_removal_delay_delta = value; } }
        private uint[][] pt_vcl_cpb_alt_initial_removal_offset_delta;
        public uint[][] PtVclCpbAltInitialRemovalOffsetDelta { get { return pt_vcl_cpb_alt_initial_removal_offset_delta; } set { pt_vcl_cpb_alt_initial_removal_offset_delta = value; } }
        private uint[] pt_vcl_cpb_delay_offset;
        public uint[] PtVclCpbDelayOffset { get { return pt_vcl_cpb_delay_offset; } set { pt_vcl_cpb_delay_offset = value; } }
        private uint[] pt_vcl_dpb_delay_offset;
        public uint[] PtVclDpbDelayOffset { get { return pt_vcl_dpb_delay_offset; } set { pt_vcl_dpb_delay_offset = value; } }
        private uint pt_dpb_output_du_delay;
        public uint PtDpbOutputDuDelay { get { return pt_dpb_output_du_delay; } set { pt_dpb_output_du_delay = value; } }
        private uint pt_num_decoding_units_minus1;
        public uint PtNumDecodingUnitsMinus1 { get { return pt_num_decoding_units_minus1; } set { pt_num_decoding_units_minus1 = value; } }
        private byte pt_du_common_cpb_removal_delay_flag;
        public byte PtDuCommonCpbRemovalDelayFlag { get { return pt_du_common_cpb_removal_delay_flag; } set { pt_du_common_cpb_removal_delay_flag = value; } }
        private uint[] pt_du_common_cpb_removal_delay_increment_minus1;
        public uint[] PtDuCommonCpbRemovalDelayIncrementMinus1 { get { return pt_du_common_cpb_removal_delay_increment_minus1; } set { pt_du_common_cpb_removal_delay_increment_minus1 = value; } }
        private uint[] pt_num_nalus_in_du_minus1;
        public uint[] PtNumNalusInDuMinus1 { get { return pt_num_nalus_in_du_minus1; } set { pt_num_nalus_in_du_minus1 = value; } }
        private uint[][] pt_du_cpb_removal_delay_increment_minus1;
        public uint[][] PtDuCpbRemovalDelayIncrementMinus1 { get { return pt_du_cpb_removal_delay_increment_minus1; } set { pt_du_cpb_removal_delay_increment_minus1 = value; } }
        private byte pt_delay_for_concatenation_ensured_flag;
        public byte PtDelayForConcatenationEnsuredFlag { get { return pt_delay_for_concatenation_ensured_flag; } set { pt_delay_for_concatenation_ensured_flag = value; } }
        private uint pt_display_elemental_periods_minus1;
        public uint PtDisplayElementalPeriodsMinus1 { get { return pt_display_elemental_periods_minus1; } set { pt_display_elemental_periods_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PicTiming(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            if (this.pt_cpb_removal_delay_minus1 == null) this.pt_cpb_removal_delay_minus1 = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, out this.pt_cpb_removal_delay_minus1[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1]);

            this.pt_sublayer_delays_present_flag = new byte[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
            this.pt_sublayer_delays_present_flag[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1] = 1;
            this.pt_cpb_removal_delay_delta_enabled_flag = new byte[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
            this.pt_cpb_removal_delay_delta_idx = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
            for (i = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); i < ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pt_sublayer_delays_present_flag[i]);

                if (pt_sublayer_delays_present_flag[i] != 0)
                {

                    if (((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayDeltasPresentFlag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.pt_cpb_removal_delay_delta_enabled_flag[i]);
                    }

                    if (pt_cpb_removal_delay_delta_enabled_flag[i] != 0)
                    {

                        if (((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 > 0)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (uint)Math.Ceiling(Math.Log2(((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 + 1)), out this.pt_cpb_removal_delay_delta_idx[i]);
                        }
                    }
                    else
                    {
                        if (this.pt_cpb_removal_delay_minus1 == null) this.pt_cpb_removal_delay_minus1 = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, out this.pt_cpb_removal_delay_minus1[i]);
                    }
                }
            }
            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1, out this.pt_dpb_output_delay);

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpAltCpbParamsPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pt_cpb_alt_timing_info_present_flag);

                if (pt_cpb_alt_timing_info_present_flag != 0)
                {

                    if (((H266Context)context).SeiPayload.BufferingPeriod.BpNalHrdParamsPresentFlag != 0)
                    {

                        this.pt_nal_cpb_alt_initial_removal_delay_delta = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1][];
                        this.pt_nal_cpb_alt_initial_removal_offset_delta = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1][];
                        this.pt_nal_cpb_delay_offset = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        this.pt_nal_dpb_delay_offset = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        for (i = (((H266Context)context).SeiPayload.BufferingPeriod.BpSublayerInitialCpbRemovalDelayPresentFlag != 0 ? 0 :
      ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                        {

                            this.pt_nal_cpb_alt_initial_removal_delay_delta[i] = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1 + 1];
                            this.pt_nal_cpb_alt_initial_removal_offset_delta[i] = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1 + 1];
                            for (j = 0; j < ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1; j++)
                            {
                                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, out this.pt_nal_cpb_alt_initial_removal_delay_delta[i][j]);
                                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, out this.pt_nal_cpb_alt_initial_removal_offset_delta[i][j]);
                            }
                            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, out this.pt_nal_cpb_delay_offset[i]);
                            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1, out this.pt_nal_dpb_delay_offset[i]);
                        }
                    }

                    if (((H266Context)context).SeiPayload.BufferingPeriod.BpVclHrdParamsPresentFlag != 0)
                    {

                        this.pt_vcl_cpb_alt_initial_removal_delay_delta = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1][];
                        this.pt_vcl_cpb_alt_initial_removal_offset_delta = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1][];
                        this.pt_vcl_cpb_delay_offset = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        this.pt_vcl_dpb_delay_offset = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        for (i = (((H266Context)context).SeiPayload.BufferingPeriod.BpSublayerInitialCpbRemovalDelayPresentFlag != 0 ? 0 :
      ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                        {

                            this.pt_vcl_cpb_alt_initial_removal_delay_delta[i] = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1 + 1];
                            this.pt_vcl_cpb_alt_initial_removal_offset_delta[i] = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1 + 1];
                            for (j = 0; j < ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1; j++)
                            {
                                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, out this.pt_vcl_cpb_alt_initial_removal_delay_delta[i][j]);
                                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, out this.pt_vcl_cpb_alt_initial_removal_offset_delta[i][j]);
                            }
                            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, out this.pt_vcl_cpb_delay_offset[i]);
                            size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1, out this.pt_vcl_dpb_delay_offset[i]);
                        }
                    }
                }
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuHrdParamsPresentFlag != 0 &&
   ((H266Context)context).SeiPayload.BufferingPeriod.BpDuDpbParamsInPicTimingSeiFlag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayDuLengthMinus1 + 1, out this.pt_dpb_output_du_delay);
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuHrdParamsPresentFlag != 0 &&
   ((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbParamsInPicTimingSeiFlag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pt_num_decoding_units_minus1);

                if (pt_num_decoding_units_minus1 > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pt_du_common_cpb_removal_delay_flag);

                    if (pt_du_common_cpb_removal_delay_flag != 0)
                    {

                        this.pt_du_common_cpb_removal_delay_increment_minus1 = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        for (i = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                        {

                            if (pt_sublayer_delays_present_flag[i] != 0)
                            {
                                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1, out this.pt_du_common_cpb_removal_delay_increment_minus1[i]);
                            }
                        }
                    }

                    this.pt_num_nalus_in_du_minus1 = new uint[pt_num_decoding_units_minus1 + 1];
                    this.pt_du_cpb_removal_delay_increment_minus1 = new uint[pt_num_decoding_units_minus1 + 1][];
                    for (i = 0; i <= pt_num_decoding_units_minus1; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.pt_num_nalus_in_du_minus1[i]);

                        if (pt_du_common_cpb_removal_delay_flag == 0 &&
      i < pt_num_decoding_units_minus1)
                        {

                            this.pt_du_cpb_removal_delay_increment_minus1[i] = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                            for (j = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); j <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; j++)
                            {

                                if (pt_sublayer_delays_present_flag[j] != 0)
                                {
                                    size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1, out this.pt_du_cpb_removal_delay_increment_minus1[i][j]);
                                }
                            }
                        }
                    }
                }
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpAdditionalConcatenationInfoPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pt_delay_for_concatenation_ensured_flag);
            }
            size += stream.ReadUnsignedInt(size, 8, out this.pt_display_elemental_periods_minus1);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            if (this.pt_cpb_removal_delay_minus1 == null) this.pt_cpb_removal_delay_minus1 = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, this.pt_cpb_removal_delay_minus1[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1]);

            for (i = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); i < ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
            {
                size += stream.WriteUnsignedInt(1, this.pt_sublayer_delays_present_flag[i]);

                if (pt_sublayer_delays_present_flag[i] != 0)
                {

                    if (((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayDeltasPresentFlag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.pt_cpb_removal_delay_delta_enabled_flag[i]);
                    }

                    if (pt_cpb_removal_delay_delta_enabled_flag[i] != 0)
                    {

                        if (((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 > 0)
                        {
                            size += stream.WriteUnsignedIntVariable((uint)Math.Ceiling(Math.Log2(((H266Context)context).SeiPayload.BufferingPeriod.BpNumCpbRemovalDelayDeltasMinus1 + 1)), this.pt_cpb_removal_delay_delta_idx[i]);
                        }
                    }
                    else
                    {
                        if (this.pt_cpb_removal_delay_minus1 == null) this.pt_cpb_removal_delay_minus1 = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                        size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, this.pt_cpb_removal_delay_minus1[i]);
                    }
                }
            }
            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1, this.pt_dpb_output_delay);

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpAltCpbParamsPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.pt_cpb_alt_timing_info_present_flag);

                if (pt_cpb_alt_timing_info_present_flag != 0)
                {

                    if (((H266Context)context).SeiPayload.BufferingPeriod.BpNalHrdParamsPresentFlag != 0)
                    {

                        for (i = (((H266Context)context).SeiPayload.BufferingPeriod.BpSublayerInitialCpbRemovalDelayPresentFlag != 0 ? 0 :
      ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                        {

                            for (j = 0; j < ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1; j++)
                            {
                                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, this.pt_nal_cpb_alt_initial_removal_delay_delta[i][j]);
                                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, this.pt_nal_cpb_alt_initial_removal_offset_delta[i][j]);
                            }
                            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, this.pt_nal_cpb_delay_offset[i]);
                            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1, this.pt_nal_dpb_delay_offset[i]);
                        }
                    }

                    if (((H266Context)context).SeiPayload.BufferingPeriod.BpVclHrdParamsPresentFlag != 0)
                    {

                        for (i = (((H266Context)context).SeiPayload.BufferingPeriod.BpSublayerInitialCpbRemovalDelayPresentFlag != 0 ? 0 :
      ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                        {

                            for (j = 0; j < ((H266Context)context).SeiPayload.BufferingPeriod.BpCpbCntMinus1 + 1; j++)
                            {
                                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, this.pt_vcl_cpb_alt_initial_removal_delay_delta[i][j]);
                                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbInitialRemovalDelayLengthMinus1 + 1, this.pt_vcl_cpb_alt_initial_removal_offset_delta[i][j]);
                            }
                            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpCpbRemovalDelayLengthMinus1 + 1, this.pt_vcl_cpb_delay_offset[i]);
                            size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayLengthMinus1 + 1, this.pt_vcl_dpb_delay_offset[i]);
                        }
                    }
                }
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuHrdParamsPresentFlag != 0 &&
   ((H266Context)context).SeiPayload.BufferingPeriod.BpDuDpbParamsInPicTimingSeiFlag != 0)
            {
                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayDuLengthMinus1 + 1, this.pt_dpb_output_du_delay);
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuHrdParamsPresentFlag != 0 &&
   ((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbParamsInPicTimingSeiFlag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pt_num_decoding_units_minus1);

                if (pt_num_decoding_units_minus1 > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.pt_du_common_cpb_removal_delay_flag);

                    if (pt_du_common_cpb_removal_delay_flag != 0)
                    {

                        for (i = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                        {

                            if (pt_sublayer_delays_present_flag[i] != 0)
                            {
                                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1, this.pt_du_common_cpb_removal_delay_increment_minus1[i]);
                            }
                        }
                    }

                    for (i = 0; i <= pt_num_decoding_units_minus1; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.pt_num_nalus_in_du_minus1[i]);

                        if (pt_du_common_cpb_removal_delay_flag == 0 &&
      i < pt_num_decoding_units_minus1)
                        {

                            for (j = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); j <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; j++)
                            {

                                if (pt_sublayer_delays_present_flag[j] != 0)
                                {
                                    size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1, this.pt_du_cpb_removal_delay_increment_minus1[i][j]);
                                }
                            }
                        }
                    }
                }
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpAdditionalConcatenationInfoPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.pt_delay_for_concatenation_ensured_flag);
            }
            size += stream.WriteUnsignedInt(8, this.pt_display_elemental_periods_minus1);

            return size;
        }

    }

    /*
  


decoding_unit_info( payloadSize ) { 
dui_decoding_unit_idx  ue(v)
if( !bp_du_cpb_params_in_pic_timing_sei_flag )  
for( i = TemporalId; i  <=  bp_max_sublayers_minus1; i++ ) {  
if( i < bp_max_sublayers_minus1 )  
dui_sublayer_delays_present_flag[ i ]  u(1) 
if( dui_sublayer_delays_present_flag[ i ] )  
dui_du_cpb_removal_delay_increment[ i ] u(v) 
}  
if( !bp_du_dpb_params_in_pic_timing_sei_flag )  
dui_dpb_output_du_delay_present_flag u(1) 
if( dui_dpb_output_du_delay_present_flag )  
dui_dpb_output_du_delay u(v) 
}
    */
    public class DecodingUnitInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint dui_decoding_unit_idx;
        public uint DuiDecodingUnitIdx { get { return dui_decoding_unit_idx; } set { dui_decoding_unit_idx = value; } }
        private byte[] dui_sublayer_delays_present_flag;
        public byte[] DuiSublayerDelaysPresentFlag { get { return dui_sublayer_delays_present_flag; } set { dui_sublayer_delays_present_flag = value; } }
        private uint[] dui_du_cpb_removal_delay_increment;
        public uint[] DuiDuCpbRemovalDelayIncrement { get { return dui_du_cpb_removal_delay_increment; } set { dui_du_cpb_removal_delay_increment = value; } }
        private byte dui_dpb_output_du_delay_present_flag;
        public byte DuiDpbOutputDuDelayPresentFlag { get { return dui_dpb_output_du_delay_present_flag; } set { dui_dpb_output_du_delay_present_flag = value; } }
        private uint dui_dpb_output_du_delay;
        public uint DuiDpbOutputDuDelay { get { return dui_dpb_output_du_delay; } set { dui_dpb_output_du_delay = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DecodingUnitInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.dui_decoding_unit_idx);

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbParamsInPicTimingSeiFlag == 0)
            {

                this.dui_sublayer_delays_present_flag = new byte[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                this.dui_du_cpb_removal_delay_increment = new uint[((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1 + 1];
                for (i = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                {

                    if (i < ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.dui_sublayer_delays_present_flag[i]);
                    }

                    if (dui_sublayer_delays_present_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1, out this.dui_du_cpb_removal_delay_increment[i]);
                    }
                }
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuDpbParamsInPicTimingSeiFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.dui_dpb_output_du_delay_present_flag);
            }

            if (dui_dpb_output_du_delay_present_flag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, ((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayDuLengthMinus1 + 1, out this.dui_dpb_output_du_delay);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.dui_decoding_unit_idx);

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbParamsInPicTimingSeiFlag == 0)
            {

                for (i = (((H266Context)context).NalHeader.NalUnitHeader.NuhTemporalIdPlus1 - 1); i <= ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1; i++)
                {

                    if (i < ((H266Context)context).SeiPayload.BufferingPeriod.BpMaxSublayersMinus1)
                    {
                        size += stream.WriteUnsignedInt(1, this.dui_sublayer_delays_present_flag[i]);
                    }

                    if (dui_sublayer_delays_present_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDuCpbRemovalDelayIncrementLengthMinus1 + 1, this.dui_du_cpb_removal_delay_increment[i]);
                    }
                }
            }

            if (((H266Context)context).SeiPayload.BufferingPeriod.BpDuDpbParamsInPicTimingSeiFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.dui_dpb_output_du_delay_present_flag);
            }

            if (dui_dpb_output_du_delay_present_flag != 0)
            {
                size += stream.WriteUnsignedIntVariable(((H266Context)context).SeiPayload.BufferingPeriod.BpDpbOutputDelayDuLengthMinus1 + 1, this.dui_dpb_output_du_delay);
            }

            return size;
        }

    }

    /*
  
scalable_nesting( payloadSize ) { 
sn_ols_flag  u(1) 
sn_subpic_flag u(1) 
sn_num_olss_minus1 ue(v) 
if( sn_ols_flag ) {  
for( i = 0; i  <=  sn_num_olss_minus1; i++ )  
sn_ols_idx_delta_minus1[ i ] ue(v) 
} else { 
sn_all_layers_flag u(1) 
if( !sn_all_layers_flag ) {  
sn_num_layers_minus1 ue(v) 
for( i = 1; i  <=  sn_num_layers_minus1; i++ )  
sn_layer_id[ i ] u(6) 
}  
}  
if( sn_subpic_flag ) {  
sn_num_subpics_minus1 ue(v) 
sn_subpic_id_len_minus1 ue(v) 
for( i = 0; i  <=  sn_num_subpics_minus1; i++ )  
sn_subpic_id[ i ] u(v) 
}  
sn_num_seis_minus1 ue(v) 
while( !byte_aligned() )  
sn_zero_bit /* equal to 0 *//* u(1) 
for( i = 0; i  <=  sn_num_seis_minus1; i++ )  
sei_message()  
}
    */
    public class ScalableNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte sn_ols_flag;
        public byte SnOlsFlag { get { return sn_ols_flag; } set { sn_ols_flag = value; } }
        private byte sn_subpic_flag;
        public byte SnSubpicFlag { get { return sn_subpic_flag; } set { sn_subpic_flag = value; } }
        private uint sn_num_olss_minus1;
        public uint SnNumOlssMinus1 { get { return sn_num_olss_minus1; } set { sn_num_olss_minus1 = value; } }
        private uint[] sn_ols_idx_delta_minus1;
        public uint[] SnOlsIdxDeltaMinus1 { get { return sn_ols_idx_delta_minus1; } set { sn_ols_idx_delta_minus1 = value; } }
        private byte sn_all_layers_flag;
        public byte SnAllLayersFlag { get { return sn_all_layers_flag; } set { sn_all_layers_flag = value; } }
        private uint sn_num_layers_minus1;
        public uint SnNumLayersMinus1 { get { return sn_num_layers_minus1; } set { sn_num_layers_minus1 = value; } }
        private uint[] sn_layer_id;
        public uint[] SnLayerId { get { return sn_layer_id; } set { sn_layer_id = value; } }
        private uint sn_num_subpics_minus1;
        public uint SnNumSubpicsMinus1 { get { return sn_num_subpics_minus1; } set { sn_num_subpics_minus1 = value; } }
        private uint sn_subpic_id_len_minus1;
        public uint SnSubpicIdLenMinus1 { get { return sn_subpic_id_len_minus1; } set { sn_subpic_id_len_minus1 = value; } }
        private uint[] sn_subpic_id;
        public uint[] SnSubpicId { get { return sn_subpic_id; } set { sn_subpic_id = value; } }
        private uint sn_num_seis_minus1;
        public uint SnNumSeisMinus1 { get { return sn_num_seis_minus1; } set { sn_num_seis_minus1 = value; } }
        private Dictionary<int, byte> sn_zero_bit = new Dictionary<int, byte>();
        public Dictionary<int, byte> SnZeroBit { get { return sn_zero_bit; } set { sn_zero_bit = value; } }
        private SeiMessage[] sei_message;
        public SeiMessage[] SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ScalableNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 1, out this.sn_ols_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sn_subpic_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.sn_num_olss_minus1);

            if (sn_ols_flag != 0)
            {

                this.sn_ols_idx_delta_minus1 = new uint[sn_num_olss_minus1 + 1];
                for (i = 0; i <= sn_num_olss_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sn_ols_idx_delta_minus1[i]);
                }
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sn_all_layers_flag);

                if (sn_all_layers_flag == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sn_num_layers_minus1);

                    this.sn_layer_id = new uint[sn_num_layers_minus1 + 1];
                    for (i = 1; i <= sn_num_layers_minus1; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 6, out this.sn_layer_id[i]);
                    }
                }
            }

            if (sn_subpic_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sn_num_subpics_minus1);
                size += stream.ReadUnsignedIntGolomb(size, out this.sn_subpic_id_len_minus1);

                this.sn_subpic_id = new uint[sn_num_subpics_minus1 + 1];
                for (i = 0; i <= sn_num_subpics_minus1; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, sn_subpic_id_len_minus1 + 1, out this.sn_subpic_id[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sn_num_seis_minus1);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 1, whileIndex, this.sn_zero_bit); // equal to 0 
            }

            this.sei_message = new SeiMessage[sn_num_seis_minus1 + 1];
            for (i = 0; i <= sn_num_seis_minus1; i++)
            {
                this.sei_message[i] = new SeiMessage();
                size += stream.ReadClass<SeiMessage>(size, context, this.sei_message[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(1, this.sn_ols_flag);
            size += stream.WriteUnsignedInt(1, this.sn_subpic_flag);
            size += stream.WriteUnsignedIntGolomb(this.sn_num_olss_minus1);

            if (sn_ols_flag != 0)
            {

                for (i = 0; i <= sn_num_olss_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sn_ols_idx_delta_minus1[i]);
                }
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.sn_all_layers_flag);

                if (sn_all_layers_flag == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sn_num_layers_minus1);

                    for (i = 1; i <= sn_num_layers_minus1; i++)
                    {
                        size += stream.WriteUnsignedInt(6, this.sn_layer_id[i]);
                    }
                }
            }

            if (sn_subpic_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sn_num_subpics_minus1);
                size += stream.WriteUnsignedIntGolomb(this.sn_subpic_id_len_minus1);

                for (i = 0; i <= sn_num_subpics_minus1; i++)
                {
                    size += stream.WriteUnsignedIntVariable(sn_subpic_id_len_minus1 + 1, this.sn_subpic_id[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.sn_num_seis_minus1);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(1, whileIndex, this.sn_zero_bit); // equal to 0 
            }

            for (i = 0; i <= sn_num_seis_minus1; i++)
            {
                size += stream.WriteClass<SeiMessage>(context, this.sei_message[i]);
            }

            return size;
        }

    }

    /*
  

subpic_level_info( payloadSize ) { 
sli_num_ref_levels_minus1  u(3) 
sli_cbr_constraint_flag u(1) 
sli_explicit_fraction_present_flag u(1) 

if( sli_explicit_fraction_present_flag )  

sli_num_subpics_minus1 ue(v) 
sli_max_sublayers_minus1 u(3) 
sli_sublayer_info_present_flag u(1) 

while( !byte_aligned() )  

sli_alignment_zero_bit f(1) 
for( k = sli_sublayer_info_present_flag != 0 ? 0 : sli_max_sublayers_minus1; k  <=  sli_max_sublayers_minus1; k++ ) 
for( i = 0; i  <=  sli_num_ref_levels_minus1; i++ ) {  
sli_non_subpic_layers_fraction[ i ][ k ] u(8) 
sli_ref_level_idc[ i ][ k ] u(8) 
if( sli_explicit_fraction_present_flag )  
for( j = 0; j  <=  sli_num_subpics_minus1; j++ )  
sli_ref_level_fraction_minus1[ i ][ j ][ k ] u(8) 
}  
}
    */
    public class SubpicLevelInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sli_num_ref_levels_minus1;
        public uint SliNumRefLevelsMinus1 { get { return sli_num_ref_levels_minus1; } set { sli_num_ref_levels_minus1 = value; } }
        private byte sli_cbr_constraint_flag;
        public byte SliCbrConstraintFlag { get { return sli_cbr_constraint_flag; } set { sli_cbr_constraint_flag = value; } }
        private byte sli_explicit_fraction_present_flag;
        public byte SliExplicitFractionPresentFlag { get { return sli_explicit_fraction_present_flag; } set { sli_explicit_fraction_present_flag = value; } }
        private uint sli_num_subpics_minus1;
        public uint SliNumSubpicsMinus1 { get { return sli_num_subpics_minus1; } set { sli_num_subpics_minus1 = value; } }
        private uint sli_max_sublayers_minus1;
        public uint SliMaxSublayersMinus1 { get { return sli_max_sublayers_minus1; } set { sli_max_sublayers_minus1 = value; } }
        private byte sli_sublayer_info_present_flag;
        public byte SliSublayerInfoPresentFlag { get { return sli_sublayer_info_present_flag; } set { sli_sublayer_info_present_flag = value; } }
        private Dictionary<int, uint> sli_alignment_zero_bit = new Dictionary<int, uint>();
        public Dictionary<int, uint> SliAlignmentZeroBit { get { return sli_alignment_zero_bit; } set { sli_alignment_zero_bit = value; } }
        private uint[][] sli_non_subpic_layers_fraction;
        public uint[][] SliNonSubpicLayersFraction { get { return sli_non_subpic_layers_fraction; } set { sli_non_subpic_layers_fraction = value; } }
        private uint[][] sli_ref_level_idc;
        public uint[][] SliRefLevelIdc { get { return sli_ref_level_idc; } set { sli_ref_level_idc = value; } }
        private uint[][][] sli_ref_level_fraction_minus1;
        public uint[][][] SliRefLevelFractionMinus1 { get { return sli_ref_level_fraction_minus1; } set { sli_ref_level_fraction_minus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubpicLevelInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            uint k = 0;
            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 3, out this.sli_num_ref_levels_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.sli_cbr_constraint_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sli_explicit_fraction_present_flag);

            if (sli_explicit_fraction_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.sli_num_subpics_minus1);
            }
            size += stream.ReadUnsignedInt(size, 3, out this.sli_max_sublayers_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.sli_sublayer_info_present_flag);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadFixed(size, 1, whileIndex, this.sli_alignment_zero_bit);
            }

            this.sli_non_subpic_layers_fraction = new uint[sli_max_sublayers_minus1 + 1][];
            this.sli_ref_level_idc = new uint[sli_max_sublayers_minus1 + 1][];
            this.sli_ref_level_fraction_minus1 = new uint[sli_max_sublayers_minus1 + 1][][];
            for (k = sli_sublayer_info_present_flag != 0 ? 0 : sli_max_sublayers_minus1; k <= sli_max_sublayers_minus1; k++)
            {

                this.sli_non_subpic_layers_fraction[k] = new uint[sli_num_ref_levels_minus1 + 1];
                this.sli_ref_level_idc[k] = new uint[sli_num_ref_levels_minus1 + 1];
                this.sli_ref_level_fraction_minus1[k] = new uint[sli_num_ref_levels_minus1 + 1][];
                for (i = 0; i <= sli_num_ref_levels_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.sli_non_subpic_layers_fraction[i][k]);
                    size += stream.ReadUnsignedInt(size, 8, out this.sli_ref_level_idc[i][k]);

                    if (sli_explicit_fraction_present_flag != 0)
                    {

                        this.sli_ref_level_fraction_minus1[k][i] = new uint[sli_num_subpics_minus1 + 1];
                        for (j = 0; j <= sli_num_subpics_minus1; j++)
                        {
                            size += stream.ReadUnsignedInt(size, 8, out this.sli_ref_level_fraction_minus1[i][j][k]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            uint k = 0;
            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(3, this.sli_num_ref_levels_minus1);
            size += stream.WriteUnsignedInt(1, this.sli_cbr_constraint_flag);
            size += stream.WriteUnsignedInt(1, this.sli_explicit_fraction_present_flag);

            if (sli_explicit_fraction_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.sli_num_subpics_minus1);
            }
            size += stream.WriteUnsignedInt(3, this.sli_max_sublayers_minus1);
            size += stream.WriteUnsignedInt(1, this.sli_sublayer_info_present_flag);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteFixed(1, whileIndex, this.sli_alignment_zero_bit);
            }

            for (k = sli_sublayer_info_present_flag != 0 ? 0 : sli_max_sublayers_minus1; k <= sli_max_sublayers_minus1; k++)
            {

                for (i = 0; i <= sli_num_ref_levels_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(8, this.sli_non_subpic_layers_fraction[i][k]);
                    size += stream.WriteUnsignedInt(8, this.sli_ref_level_idc[i][k]);

                    if (sli_explicit_fraction_present_flag != 0)
                    {

                        for (j = 0; j <= sli_num_subpics_minus1; j++)
                        {
                            size += stream.WriteUnsignedInt(8, this.sli_ref_level_fraction_minus1[i][j][k]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

parameter_sets_inclusion_indication(payloadSize) {
    psii_self_contained_clvs_flag  u(1)
}
    */
    public class ParameterSetsInclusionIndication : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte psii_self_contained_clvs_flag;
        public byte PsiiSelfContainedClvsFlag { get { return psii_self_contained_clvs_flag; } set { psii_self_contained_clvs_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ParameterSetsInclusionIndication(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.psii_self_contained_clvs_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.psii_self_contained_clvs_flag);

            return size;
        }

    }

    /*


sphere_rotation(payloadSize) { 
    sphere_rotation_cancel_flag u(1)
    if (!sphere_rotation_cancel_flag) {  
      sphere_rotation_persistence_flag u(1) 
      sphere_rotation_reserved_zero_6bits u(6) 
      yaw_rotation i(32) 
      pitch_rotation i(32) 
      roll_rotation i(32)
    }
}
    */
    public class SphereRotation : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte sphere_rotation_cancel_flag;
        public byte SphereRotationCancelFlag { get { return sphere_rotation_cancel_flag; } set { sphere_rotation_cancel_flag = value; } }
        private byte sphere_rotation_persistence_flag;
        public byte SphereRotationPersistenceFlag { get { return sphere_rotation_persistence_flag; } set { sphere_rotation_persistence_flag = value; } }
        private uint sphere_rotation_reserved_zero_6bits;
        public uint SphereRotationReservedZero6bits { get { return sphere_rotation_reserved_zero_6bits; } set { sphere_rotation_reserved_zero_6bits = value; } }
        private int yaw_rotation;
        public int YawRotation { get { return yaw_rotation; } set { yaw_rotation = value; } }
        private int pitch_rotation;
        public int PitchRotation { get { return pitch_rotation; } set { pitch_rotation = value; } }
        private int roll_rotation;
        public int RollRotation { get { return roll_rotation; } set { roll_rotation = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SphereRotation(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.sphere_rotation_cancel_flag);

            if (sphere_rotation_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sphere_rotation_persistence_flag);
                size += stream.ReadUnsignedInt(size, 6, out this.sphere_rotation_reserved_zero_6bits);
                size += stream.ReadSignedInt(size, 32, out this.yaw_rotation);
                size += stream.ReadSignedInt(size, 32, out this.pitch_rotation);
                size += stream.ReadSignedInt(size, 32, out this.roll_rotation);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.sphere_rotation_cancel_flag);

            if (sphere_rotation_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sphere_rotation_persistence_flag);
                size += stream.WriteUnsignedInt(6, this.sphere_rotation_reserved_zero_6bits);
                size += stream.WriteSignedInt(32, this.yaw_rotation);
                size += stream.WriteSignedInt(32, this.pitch_rotation);
                size += stream.WriteSignedInt(32, this.roll_rotation);
            }

            return size;
        }

    }

    /*


decoded_picture_hash(payloadSize) {
 dph_sei_hash_type u(8) 
 dph_sei_single_component_flag u(1) 
 dph_sei_reserved_zero_7bits u(7)
    for (cIdx = 0; cIdx < (dph_sei_single_component_flag != 0 ?1 : 3); cIdx++)
        if (dph_sei_hash_type == 0)
            for (i = 0; i < 16; i++)
                dph_sei_picture_md5[cIdx][i] b(8) 
  else if (dph_sei_hash_type == 1)
        dph_sei_picture_crc[cIdx] u(16) 
  else if (dph_sei_hash_type == 2)
        dph_sei_picture_checksum[cIdx] u(32)
}
    */
    public class DecodedPictureHash : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint dph_sei_hash_type;
        public uint DphSeiHashType { get { return dph_sei_hash_type; } set { dph_sei_hash_type = value; } }
        private byte dph_sei_single_component_flag;
        public byte DphSeiSingleComponentFlag { get { return dph_sei_single_component_flag; } set { dph_sei_single_component_flag = value; } }
        private uint dph_sei_reserved_zero_7bits;
        public uint DphSeiReservedZero7bits { get { return dph_sei_reserved_zero_7bits; } set { dph_sei_reserved_zero_7bits = value; } }
        private byte[][] dph_sei_picture_md5;
        public byte[][] DphSeiPictureMd5 { get { return dph_sei_picture_md5; } set { dph_sei_picture_md5 = value; } }
        private uint[] dph_sei_picture_crc;
        public uint[] DphSeiPictureCrc { get { return dph_sei_picture_crc; } set { dph_sei_picture_crc = value; } }
        private uint[] dph_sei_picture_checksum;
        public uint[] DphSeiPictureChecksum { get { return dph_sei_picture_checksum; } set { dph_sei_picture_checksum = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DecodedPictureHash(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint cIdx = 0;
            uint i = 0;
            size += stream.ReadUnsignedInt(size, 8, out this.dph_sei_hash_type);
            size += stream.ReadUnsignedInt(size, 1, out this.dph_sei_single_component_flag);
            size += stream.ReadUnsignedInt(size, 7, out this.dph_sei_reserved_zero_7bits);

            this.dph_sei_picture_md5 = new byte[(dph_sei_single_component_flag != 0 ? 1 : 3)][];
            this.dph_sei_picture_crc = new uint[(dph_sei_single_component_flag != 0 ? 1 : 3)];
            this.dph_sei_picture_checksum = new uint[(dph_sei_single_component_flag != 0 ? 1 : 3)];
            for (cIdx = 0; cIdx < (dph_sei_single_component_flag != 0 ? 1 : 3); cIdx++)
            {

                if (dph_sei_hash_type == 0)
                {

                    this.dph_sei_picture_md5[cIdx] = new byte[16];
                    for (i = 0; i < 16; i++)
                    {
                        size += stream.ReadBits(size, 8, out this.dph_sei_picture_md5[cIdx][i]);
                    }
                }
                else if (dph_sei_hash_type == 1)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.dph_sei_picture_crc[cIdx]);
                }
                else if (dph_sei_hash_type == 2)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.dph_sei_picture_checksum[cIdx]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint cIdx = 0;
            uint i = 0;
            size += stream.WriteUnsignedInt(8, this.dph_sei_hash_type);
            size += stream.WriteUnsignedInt(1, this.dph_sei_single_component_flag);
            size += stream.WriteUnsignedInt(7, this.dph_sei_reserved_zero_7bits);

            for (cIdx = 0; cIdx < (dph_sei_single_component_flag != 0 ? 1 : 3); cIdx++)
            {

                if (dph_sei_hash_type == 0)
                {

                    for (i = 0; i < 16; i++)
                    {
                        size += stream.WriteBits(8, this.dph_sei_picture_md5[cIdx][i]);
                    }
                }
                else if (dph_sei_hash_type == 1)
                {
                    size += stream.WriteUnsignedInt(16, this.dph_sei_picture_crc[cIdx]);
                }
                else if (dph_sei_hash_type == 2)
                {
                    size += stream.WriteUnsignedInt(32, this.dph_sei_picture_checksum[cIdx]);
                }
            }

            return size;
        }

    }

}
