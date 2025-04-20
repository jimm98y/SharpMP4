using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SharpH26X;

namespace SharpH265
{

    public class H265Context : IItuContext
    {
        public NalUnit NalHeader { get; set; }
        public PicParameterSetRbsp PicParameterSetRbsp { get; set; }
        public SeiRbsp SeiRbsp { get; set; }
        public AccessUnitDelimiterRbsp AccessUnitDelimiterRbsp { get; set; }
        public EndOfSeqRbsp EndOfSeqRbsp { get; set; }
        public EndOfBitstreamRbsp EndOfBitstreamRbsp { get; set; }
        public FillerDataRbsp FillerDataRbsp { get; set; }
        public SliceSegmentLayerRbsp SliceSegmentLayerRbsp { get; set; }
        public SeqParameterSetRbsp SeqParameterSetRbsp { get; set; }
        public VideoParameterSetRbsp VideoParameterSetRbsp { get; set; }

    }

    /*
nal_unit( NumBytesInNalUnit ) { 
  nal_unit_header()  
  /*NumBytesInRbsp = 0 *//* 
  /*for( i = 2; i < NumBytesInNalUnit; i++ )  *//*
  /* if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 ) == 0x000003 ) {  *//*
  /*   rbsp_byte[ NumBytesInRbsp++ ] b(8) *//*
  /*   rbsp_byte[ NumBytesInRbsp++ ] b(8) *//*
  /*   i +=  2  *//*
  /*   emulation_prevention_three_byte  *//*/* equal to 0x03 *//*/* f(8) *//*
  /*  } else  *//*
  /*    rbsp_byte[ NumBytesInRbsp++ ] b(8) *//*
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

            /*  if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 ) == 0x000003 ) {   */

            /*    rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /*    rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /*    i +=  2   */

            /*    emulation_prevention_three_byte   */

            /*  equal to 0x03  */

            /*  f(8)  */

            /*   } else   */

            /*     rbsp_byte[ NumBytesInRbsp++ ] b(8)  */


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<NalUnitHeader>(context, this.nal_unit_header); //NumBytesInRbsp = 0 
            /* for( i = 2; i < NumBytesInNalUnit; i++ )   */

            /*  if( i + 2 < NumBytesInNalUnit  &&  next_bits( 24 ) == 0x000003 ) {   */

            /*    rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /*    rbsp_byte[ NumBytesInRbsp++ ] b(8)  */

            /*    i +=  2   */

            /*    emulation_prevention_three_byte   */

            /*  equal to 0x03  */

            /*  f(8)  */

            /*   } else   */

            /*     rbsp_byte[ NumBytesInRbsp++ ] b(8)  */


            return size;
        }

    }

    /*
 

nal_unit_header() { 
  forbidden_zero_bit f(1)
  nal_unit_type u(6)
  nuh_layer_id u(6)
  nuh_temporal_id_plus1 u(3)
}
    */
    public class NalUnitHeader : IItuSerializable
    {
        private uint forbidden_zero_bit;
        public uint ForbiddenZeroBit { get { return forbidden_zero_bit; } set { forbidden_zero_bit = value; } }
        private uint nal_unit_type;
        public uint NalUnitType { get { return nal_unit_type; } set { nal_unit_type = value; } }
        private uint nuh_layer_id;
        public uint NuhLayerId { get { return nuh_layer_id; } set { nuh_layer_id = value; } }
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
            size += stream.ReadUnsignedInt(size, 6, out this.nal_unit_type);
            size += stream.ReadUnsignedInt(size, 6, out this.nuh_layer_id);
            size += stream.ReadUnsignedInt(size, 3, out this.nuh_temporal_id_plus1);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteFixed(1, this.forbidden_zero_bit);
            size += stream.WriteUnsignedInt(6, this.nal_unit_type);
            size += stream.WriteUnsignedInt(6, this.nuh_layer_id);
            size += stream.WriteUnsignedInt(3, this.nuh_temporal_id_plus1);

            return size;
        }

    }

    /*
 

sps_range_extension() {  
 transform_skip_rotation_enabled_flag u(1) 
 transform_skip_context_enabled_flag u(1) 
 implicit_rdpcm_enabled_flag u(1) 
 explicit_rdpcm_enabled_flag u(1) 
 extended_precision_processing_flag u(1) 
 intra_smoothing_disabled_flag u(1) 
 high_precision_offsets_enabled_flag u(1) 
 persistent_rice_adaptation_enabled_flag u(1) 
 cabac_bypass_alignment_enabled_flag u(1) 
}
    */
    public class SpsRangeExtension : IItuSerializable
    {
        private byte transform_skip_rotation_enabled_flag;
        public byte TransformSkipRotationEnabledFlag { get { return transform_skip_rotation_enabled_flag; } set { transform_skip_rotation_enabled_flag = value; } }
        private byte transform_skip_context_enabled_flag;
        public byte TransformSkipContextEnabledFlag { get { return transform_skip_context_enabled_flag; } set { transform_skip_context_enabled_flag = value; } }
        private byte implicit_rdpcm_enabled_flag;
        public byte ImplicitRdpcmEnabledFlag { get { return implicit_rdpcm_enabled_flag; } set { implicit_rdpcm_enabled_flag = value; } }
        private byte explicit_rdpcm_enabled_flag;
        public byte ExplicitRdpcmEnabledFlag { get { return explicit_rdpcm_enabled_flag; } set { explicit_rdpcm_enabled_flag = value; } }
        private byte extended_precision_processing_flag;
        public byte ExtendedPrecisionProcessingFlag { get { return extended_precision_processing_flag; } set { extended_precision_processing_flag = value; } }
        private byte intra_smoothing_disabled_flag;
        public byte IntraSmoothingDisabledFlag { get { return intra_smoothing_disabled_flag; } set { intra_smoothing_disabled_flag = value; } }
        private byte high_precision_offsets_enabled_flag;
        public byte HighPrecisionOffsetsEnabledFlag { get { return high_precision_offsets_enabled_flag; } set { high_precision_offsets_enabled_flag = value; } }
        private byte persistent_rice_adaptation_enabled_flag;
        public byte PersistentRiceAdaptationEnabledFlag { get { return persistent_rice_adaptation_enabled_flag; } set { persistent_rice_adaptation_enabled_flag = value; } }
        private byte cabac_bypass_alignment_enabled_flag;
        public byte CabacBypassAlignmentEnabledFlag { get { return cabac_bypass_alignment_enabled_flag; } set { cabac_bypass_alignment_enabled_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SpsRangeExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.transform_skip_rotation_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.transform_skip_context_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.implicit_rdpcm_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.explicit_rdpcm_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.extended_precision_processing_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.intra_smoothing_disabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.high_precision_offsets_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.persistent_rice_adaptation_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.cabac_bypass_alignment_enabled_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.transform_skip_rotation_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.transform_skip_context_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.implicit_rdpcm_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.explicit_rdpcm_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.extended_precision_processing_flag);
            size += stream.WriteUnsignedInt(1, this.intra_smoothing_disabled_flag);
            size += stream.WriteUnsignedInt(1, this.high_precision_offsets_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.persistent_rice_adaptation_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.cabac_bypass_alignment_enabled_flag);

            return size;
        }

    }

    /*
  

sps_scc_extension() {  
 sps_curr_pic_ref_enabled_flag u(1) 
 palette_mode_enabled_flag u(1) 
 if( palette_mode_enabled_flag ) {  
  palette_max_size ue(v) 
  delta_palette_max_predictor_size ue(v) 
  sps_palette_predictor_initializers_present_flag u(1) 
  if( sps_palette_predictor_initializers_present_flag ) {  
   sps_num_palette_predictor_initializers_minus1 ue(v) 
   numComps = ( chroma_format_idc == 0 ) ? 1 : 3  
   for( comp = 0; comp < numComps; comp++ )  
    for( i = 0; i <= sps_num_palette_predictor_initializers_minus1; i++ )  
     sps_palette_predictor_initializer[ comp ][ i ] u(v) 
  }  
 }  
 motion_vector_resolution_control_idc u(2) 
 intra_boundary_filtering_disabled_flag u(1) 
}
    */
    public class SpsSccExtension : IItuSerializable
    {
        private byte sps_curr_pic_ref_enabled_flag;
        public byte SpsCurrPicRefEnabledFlag { get { return sps_curr_pic_ref_enabled_flag; } set { sps_curr_pic_ref_enabled_flag = value; } }
        private byte palette_mode_enabled_flag;
        public byte PaletteModeEnabledFlag { get { return palette_mode_enabled_flag; } set { palette_mode_enabled_flag = value; } }
        private uint palette_max_size;
        public uint PaletteMaxSize { get { return palette_max_size; } set { palette_max_size = value; } }
        private uint delta_palette_max_predictor_size;
        public uint DeltaPaletteMaxPredictorSize { get { return delta_palette_max_predictor_size; } set { delta_palette_max_predictor_size = value; } }
        private byte sps_palette_predictor_initializers_present_flag;
        public byte SpsPalettePredictorInitializersPresentFlag { get { return sps_palette_predictor_initializers_present_flag; } set { sps_palette_predictor_initializers_present_flag = value; } }
        private uint sps_num_palette_predictor_initializers_minus1;
        public uint SpsNumPalettePredictorInitializersMinus1 { get { return sps_num_palette_predictor_initializers_minus1; } set { sps_num_palette_predictor_initializers_minus1 = value; } }
        private uint[][] sps_palette_predictor_initializer;
        public uint[][] SpsPalettePredictorInitializer { get { return sps_palette_predictor_initializer; } set { sps_palette_predictor_initializer = value; } }
        private uint motion_vector_resolution_control_idc;
        public uint MotionVectorResolutionControlIdc { get { return motion_vector_resolution_control_idc; } set { motion_vector_resolution_control_idc = value; } }
        private byte intra_boundary_filtering_disabled_flag;
        public byte IntraBoundaryFilteringDisabledFlag { get { return intra_boundary_filtering_disabled_flag; } set { intra_boundary_filtering_disabled_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SpsSccExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            long numComps = 0;
            uint comp = 0;
            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.sps_curr_pic_ref_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.palette_mode_enabled_flag);

            if (palette_mode_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.palette_max_size);
                size += stream.ReadUnsignedIntGolomb(size, out this.delta_palette_max_predictor_size);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_palette_predictor_initializers_present_flag);

                if (sps_palette_predictor_initializers_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_num_palette_predictor_initializers_minus1);
                    numComps = (((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0) ? 1 : 3;

                    this.sps_palette_predictor_initializer = new uint[numComps][];
                    for (comp = 0; comp < numComps; comp++)
                    {

                        this.sps_palette_predictor_initializer[comp] = new uint[sps_num_palette_predictor_initializers_minus1 + 1];
                        for (i = 0; i <= sps_num_palette_predictor_initializers_minus1; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, (((H265Context)context).SeqParameterSetRbsp.BitDepthChromaMinus8 + 8), out this.sps_palette_predictor_initializer[comp][i]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 2, out this.motion_vector_resolution_control_idc);
            size += stream.ReadUnsignedInt(size, 1, out this.intra_boundary_filtering_disabled_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            long numComps = 0;
            uint comp = 0;
            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.sps_curr_pic_ref_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.palette_mode_enabled_flag);

            if (palette_mode_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.palette_max_size);
                size += stream.WriteUnsignedIntGolomb(this.delta_palette_max_predictor_size);
                size += stream.WriteUnsignedInt(1, this.sps_palette_predictor_initializers_present_flag);

                if (sps_palette_predictor_initializers_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sps_num_palette_predictor_initializers_minus1);
                    numComps = (((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0) ? 1 : 3;

                    for (comp = 0; comp < numComps; comp++)
                    {

                        for (i = 0; i <= sps_num_palette_predictor_initializers_minus1; i++)
                        {
                            size += stream.WriteUnsignedIntVariable((((H265Context)context).SeqParameterSetRbsp.BitDepthChromaMinus8 + 8), this.sps_palette_predictor_initializer[comp][i]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(2, this.motion_vector_resolution_control_idc);
            size += stream.WriteUnsignedInt(1, this.intra_boundary_filtering_disabled_flag);

            return size;
        }

    }

    /*
  

pic_parameter_set_rbsp() { 
 pps_pic_parameter_set_id ue(v) 
 pps_seq_parameter_set_id ue(v) 
 dependent_slice_segments_enabled_flag u(1) 
 output_flag_present_flag u(1) 
 num_extra_slice_header_bits u(3) 
 sign_data_hiding_enabled_flag u(1) 
 cabac_init_present_flag u(1) 
  num_ref_idx_l0_default_active_minus1 ue(v) 
 num_ref_idx_l1_default_active_minus1 ue(v) 
 init_qp_minus26 se(v) 
 constrained_intra_pred_flag u(1) 
 transform_skip_enabled_flag u(1) 
 cu_qp_delta_enabled_flag u(1) 
 if( cu_qp_delta_enabled_flag )  
  diff_cu_qp_delta_depth ue(v) 
 pps_cb_qp_offset se(v) 
 pps_cr_qp_offset se(v) 
 pps_slice_chroma_qp_offsets_present_flag u(1) 
 weighted_pred_flag u(1) 
 weighted_bipred_flag u(1) 
 transquant_bypass_enabled_flag u(1) 
 tiles_enabled_flag u(1) 
 entropy_coding_sync_enabled_flag u(1) 
 if( tiles_enabled_flag ) {  
  num_tile_columns_minus1 ue(v) 
  num_tile_rows_minus1 ue(v) 
  uniform_spacing_flag u(1) 
  if( !uniform_spacing_flag ) {  
   for( i = 0; i < num_tile_columns_minus1; i++ )  
    column_width_minus1[ i ] ue(v) 
   for( i = 0; i < num_tile_rows_minus1; i++ )  
    row_height_minus1[ i ] ue(v) 
  }  
  loop_filter_across_tiles_enabled_flag u(1) 
 }  
 pps_loop_filter_across_slices_enabled_flag u(1) 
 deblocking_filter_control_present_flag u(1) 
 if( deblocking_filter_control_present_flag ) {  
  deblocking_filter_override_enabled_flag u(1) 
  pps_deblocking_filter_disabled_flag u(1) 
  if( !pps_deblocking_filter_disabled_flag ) {  
   pps_beta_offset_div2 se(v) 
   pps_tc_offset_div2 se(v) 
  }  
 }  
 pps_scaling_list_data_present_flag u(1) 
 if( pps_scaling_list_data_present_flag )  
  scaling_list_data()  
 lists_modification_present_flag u(1) 
 log2_parallel_merge_level_minus2 ue(v) 
 slice_segment_header_extension_present_flag u(1) 
 pps_extension_present_flag u(1) 
 if( pps_extension_present_flag ) {  
  pps_range_extension_flag u(1) 
  pps_multilayer_extension_flag u(1)
    pps_3d_extension_flag u(1) 
  pps_scc_extension_flag u(1) 
  pps_extension_4bits u(4) 
 }  
 if( pps_range_extension_flag )  
  pps_range_extension()  
 if( pps_multilayer_extension_flag )  
  pps_multilayer_extension()  /* specified in Annex F *//*  
 if( pps_3d_extension_flag )  
  pps_3d_extension()  /* specified in Annex I *//*  
 if( pps_scc_extension_flag )  
  pps_scc_extension()  
 if( pps_extension_4bits )  
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
        private byte dependent_slice_segments_enabled_flag;
        public byte DependentSliceSegmentsEnabledFlag { get { return dependent_slice_segments_enabled_flag; } set { dependent_slice_segments_enabled_flag = value; } }
        private byte output_flag_present_flag;
        public byte OutputFlagPresentFlag { get { return output_flag_present_flag; } set { output_flag_present_flag = value; } }
        private uint num_extra_slice_header_bits;
        public uint NumExtraSliceHeaderBits { get { return num_extra_slice_header_bits; } set { num_extra_slice_header_bits = value; } }
        private byte sign_data_hiding_enabled_flag;
        public byte SignDataHidingEnabledFlag { get { return sign_data_hiding_enabled_flag; } set { sign_data_hiding_enabled_flag = value; } }
        private byte cabac_init_present_flag;
        public byte CabacInitPresentFlag { get { return cabac_init_present_flag; } set { cabac_init_present_flag = value; } }
        private uint num_ref_idx_l0_default_active_minus1;
        public uint NumRefIdxL0DefaultActiveMinus1 { get { return num_ref_idx_l0_default_active_minus1; } set { num_ref_idx_l0_default_active_minus1 = value; } }
        private uint num_ref_idx_l1_default_active_minus1;
        public uint NumRefIdxL1DefaultActiveMinus1 { get { return num_ref_idx_l1_default_active_minus1; } set { num_ref_idx_l1_default_active_minus1 = value; } }
        private int init_qp_minus26;
        public int InitQpMinus26 { get { return init_qp_minus26; } set { init_qp_minus26 = value; } }
        private byte constrained_intra_pred_flag;
        public byte ConstrainedIntraPredFlag { get { return constrained_intra_pred_flag; } set { constrained_intra_pred_flag = value; } }
        private byte transform_skip_enabled_flag;
        public byte TransformSkipEnabledFlag { get { return transform_skip_enabled_flag; } set { transform_skip_enabled_flag = value; } }
        private byte cu_qp_delta_enabled_flag;
        public byte CuQpDeltaEnabledFlag { get { return cu_qp_delta_enabled_flag; } set { cu_qp_delta_enabled_flag = value; } }
        private uint diff_cu_qp_delta_depth;
        public uint DiffCuQpDeltaDepth { get { return diff_cu_qp_delta_depth; } set { diff_cu_qp_delta_depth = value; } }
        private int pps_cb_qp_offset;
        public int PpsCbQpOffset { get { return pps_cb_qp_offset; } set { pps_cb_qp_offset = value; } }
        private int pps_cr_qp_offset;
        public int PpsCrQpOffset { get { return pps_cr_qp_offset; } set { pps_cr_qp_offset = value; } }
        private byte pps_slice_chroma_qp_offsets_present_flag;
        public byte PpsSliceChromaQpOffsetsPresentFlag { get { return pps_slice_chroma_qp_offsets_present_flag; } set { pps_slice_chroma_qp_offsets_present_flag = value; } }
        private byte weighted_pred_flag;
        public byte WeightedPredFlag { get { return weighted_pred_flag; } set { weighted_pred_flag = value; } }
        private byte weighted_bipred_flag;
        public byte WeightedBipredFlag { get { return weighted_bipred_flag; } set { weighted_bipred_flag = value; } }
        private byte transquant_bypass_enabled_flag;
        public byte TransquantBypassEnabledFlag { get { return transquant_bypass_enabled_flag; } set { transquant_bypass_enabled_flag = value; } }
        private byte tiles_enabled_flag;
        public byte TilesEnabledFlag { get { return tiles_enabled_flag; } set { tiles_enabled_flag = value; } }
        private byte entropy_coding_sync_enabled_flag;
        public byte EntropyCodingSyncEnabledFlag { get { return entropy_coding_sync_enabled_flag; } set { entropy_coding_sync_enabled_flag = value; } }
        private uint num_tile_columns_minus1;
        public uint NumTileColumnsMinus1 { get { return num_tile_columns_minus1; } set { num_tile_columns_minus1 = value; } }
        private uint num_tile_rows_minus1;
        public uint NumTileRowsMinus1 { get { return num_tile_rows_minus1; } set { num_tile_rows_minus1 = value; } }
        private byte uniform_spacing_flag;
        public byte UniformSpacingFlag { get { return uniform_spacing_flag; } set { uniform_spacing_flag = value; } }
        private uint[] column_width_minus1;
        public uint[] ColumnWidthMinus1 { get { return column_width_minus1; } set { column_width_minus1 = value; } }
        private uint[] row_height_minus1;
        public uint[] RowHeightMinus1 { get { return row_height_minus1; } set { row_height_minus1 = value; } }
        private byte loop_filter_across_tiles_enabled_flag;
        public byte LoopFilterAcrossTilesEnabledFlag { get { return loop_filter_across_tiles_enabled_flag; } set { loop_filter_across_tiles_enabled_flag = value; } }
        private byte pps_loop_filter_across_slices_enabled_flag;
        public byte PpsLoopFilterAcrossSlicesEnabledFlag { get { return pps_loop_filter_across_slices_enabled_flag; } set { pps_loop_filter_across_slices_enabled_flag = value; } }
        private byte deblocking_filter_control_present_flag;
        public byte DeblockingFilterControlPresentFlag { get { return deblocking_filter_control_present_flag; } set { deblocking_filter_control_present_flag = value; } }
        private byte deblocking_filter_override_enabled_flag;
        public byte DeblockingFilterOverrideEnabledFlag { get { return deblocking_filter_override_enabled_flag; } set { deblocking_filter_override_enabled_flag = value; } }
        private byte pps_deblocking_filter_disabled_flag;
        public byte PpsDeblockingFilterDisabledFlag { get { return pps_deblocking_filter_disabled_flag; } set { pps_deblocking_filter_disabled_flag = value; } }
        private int pps_beta_offset_div2;
        public int PpsBetaOffsetDiv2 { get { return pps_beta_offset_div2; } set { pps_beta_offset_div2 = value; } }
        private int pps_tc_offset_div2;
        public int PpsTcOffsetDiv2 { get { return pps_tc_offset_div2; } set { pps_tc_offset_div2 = value; } }
        private byte pps_scaling_list_data_present_flag;
        public byte PpsScalingListDataPresentFlag { get { return pps_scaling_list_data_present_flag; } set { pps_scaling_list_data_present_flag = value; } }
        private ScalingListData scaling_list_data;
        public ScalingListData ScalingListData { get { return scaling_list_data; } set { scaling_list_data = value; } }
        private byte lists_modification_present_flag;
        public byte ListsModificationPresentFlag { get { return lists_modification_present_flag; } set { lists_modification_present_flag = value; } }
        private uint log2_parallel_merge_level_minus2;
        public uint Log2ParallelMergeLevelMinus2 { get { return log2_parallel_merge_level_minus2; } set { log2_parallel_merge_level_minus2 = value; } }
        private byte slice_segment_header_extension_present_flag;
        public byte SliceSegmentHeaderExtensionPresentFlag { get { return slice_segment_header_extension_present_flag; } set { slice_segment_header_extension_present_flag = value; } }
        private byte pps_extension_present_flag;
        public byte PpsExtensionPresentFlag { get { return pps_extension_present_flag; } set { pps_extension_present_flag = value; } }
        private byte pps_range_extension_flag;
        public byte PpsRangeExtensionFlag { get { return pps_range_extension_flag; } set { pps_range_extension_flag = value; } }
        private byte pps_multilayer_extension_flag;
        public byte PpsMultilayerExtensionFlag { get { return pps_multilayer_extension_flag; } set { pps_multilayer_extension_flag = value; } }
        private byte pps_3d_extension_flag;
        public byte Pps3dExtensionFlag { get { return pps_3d_extension_flag; } set { pps_3d_extension_flag = value; } }
        private byte pps_scc_extension_flag;
        public byte PpsSccExtensionFlag { get { return pps_scc_extension_flag; } set { pps_scc_extension_flag = value; } }
        private uint pps_extension_4bits;
        public uint PpsExtension4bits { get { return pps_extension_4bits; } set { pps_extension_4bits = value; } }
        private PpsRangeExtension pps_range_extension;
        public PpsRangeExtension PpsRangeExtension { get { return pps_range_extension; } set { pps_range_extension = value; } }
        private PpsMultilayerExtension pps_multilayer_extension;
        public PpsMultilayerExtension PpsMultilayerExtension { get { return pps_multilayer_extension; } set { pps_multilayer_extension = value; } }
        private Pps3dExtension pps_3d_extension;
        public Pps3dExtension Pps3dExtension { get { return pps_3d_extension; } set { pps_3d_extension = value; } }
        private PpsSccExtension pps_scc_extension;
        public PpsSccExtension PpsSccExtension { get { return pps_scc_extension; } set { pps_scc_extension = value; } }
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
            int whileIndex = -1;
            size += stream.ReadUnsignedIntGolomb(size, out this.pps_pic_parameter_set_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.pps_seq_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.dependent_slice_segments_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.output_flag_present_flag);
            size += stream.ReadUnsignedInt(size, 3, out this.num_extra_slice_header_bits);
            size += stream.ReadUnsignedInt(size, 1, out this.sign_data_hiding_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.cabac_init_present_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_idx_l0_default_active_minus1);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_idx_l1_default_active_minus1);
            size += stream.ReadSignedIntGolomb(size, out this.init_qp_minus26);
            size += stream.ReadUnsignedInt(size, 1, out this.constrained_intra_pred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.transform_skip_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.cu_qp_delta_enabled_flag);

            if (cu_qp_delta_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.diff_cu_qp_delta_depth);
            }
            size += stream.ReadSignedIntGolomb(size, out this.pps_cb_qp_offset);
            size += stream.ReadSignedIntGolomb(size, out this.pps_cr_qp_offset);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_slice_chroma_qp_offsets_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.weighted_pred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.weighted_bipred_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.transquant_bypass_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.tiles_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.entropy_coding_sync_enabled_flag);

            if (tiles_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_tile_columns_minus1);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_tile_rows_minus1);
                size += stream.ReadUnsignedInt(size, 1, out this.uniform_spacing_flag);

                if (uniform_spacing_flag == 0)
                {

                    this.column_width_minus1 = new uint[num_tile_columns_minus1 + 1];
                    for (i = 0; i < num_tile_columns_minus1; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.column_width_minus1[i]);
                    }

                    this.row_height_minus1 = new uint[num_tile_rows_minus1 + 1];
                    for (i = 0; i < num_tile_rows_minus1; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.row_height_minus1[i]);
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.loop_filter_across_tiles_enabled_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_loop_filter_across_slices_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.deblocking_filter_control_present_flag);

            if (deblocking_filter_control_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.deblocking_filter_override_enabled_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_deblocking_filter_disabled_flag);

                if (pps_deblocking_filter_disabled_flag == 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.pps_beta_offset_div2);
                    size += stream.ReadSignedIntGolomb(size, out this.pps_tc_offset_div2);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_scaling_list_data_present_flag);

            if (pps_scaling_list_data_present_flag != 0)
            {
                this.scaling_list_data = new ScalingListData();
                size += stream.ReadClass<ScalingListData>(size, context, this.scaling_list_data);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.lists_modification_present_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_parallel_merge_level_minus2);
            size += stream.ReadUnsignedInt(size, 1, out this.slice_segment_header_extension_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_extension_present_flag);

            if (pps_extension_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pps_range_extension_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_multilayer_extension_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_3d_extension_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.pps_scc_extension_flag);
                size += stream.ReadUnsignedInt(size, 4, out this.pps_extension_4bits);
            }

            if (pps_range_extension_flag != 0)
            {
                this.pps_range_extension = new PpsRangeExtension();
                size += stream.ReadClass<PpsRangeExtension>(size, context, this.pps_range_extension);
            }

            if (pps_multilayer_extension_flag != 0)
            {
                this.pps_multilayer_extension = new PpsMultilayerExtension();
                size += stream.ReadClass<PpsMultilayerExtension>(size, context, this.pps_multilayer_extension); // specified in Annex F 
            }

            if (pps_3d_extension_flag != 0)
            {
                this.pps_3d_extension = new Pps3dExtension();
                size += stream.ReadClass<Pps3dExtension>(size, context, this.pps_3d_extension); // specified in Annex I 
            }

            if (pps_scc_extension_flag != 0)
            {
                this.pps_scc_extension = new PpsSccExtension();
                size += stream.ReadClass<PpsSccExtension>(size, context, this.pps_scc_extension);
            }

            if (pps_extension_4bits != 0)
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
            int whileIndex = -1;
            size += stream.WriteUnsignedIntGolomb(this.pps_pic_parameter_set_id);
            size += stream.WriteUnsignedIntGolomb(this.pps_seq_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.dependent_slice_segments_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.output_flag_present_flag);
            size += stream.WriteUnsignedInt(3, this.num_extra_slice_header_bits);
            size += stream.WriteUnsignedInt(1, this.sign_data_hiding_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.cabac_init_present_flag);
            size += stream.WriteUnsignedIntGolomb(this.num_ref_idx_l0_default_active_minus1);
            size += stream.WriteUnsignedIntGolomb(this.num_ref_idx_l1_default_active_minus1);
            size += stream.WriteSignedIntGolomb(this.init_qp_minus26);
            size += stream.WriteUnsignedInt(1, this.constrained_intra_pred_flag);
            size += stream.WriteUnsignedInt(1, this.transform_skip_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.cu_qp_delta_enabled_flag);

            if (cu_qp_delta_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.diff_cu_qp_delta_depth);
            }
            size += stream.WriteSignedIntGolomb(this.pps_cb_qp_offset);
            size += stream.WriteSignedIntGolomb(this.pps_cr_qp_offset);
            size += stream.WriteUnsignedInt(1, this.pps_slice_chroma_qp_offsets_present_flag);
            size += stream.WriteUnsignedInt(1, this.weighted_pred_flag);
            size += stream.WriteUnsignedInt(1, this.weighted_bipred_flag);
            size += stream.WriteUnsignedInt(1, this.transquant_bypass_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.tiles_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.entropy_coding_sync_enabled_flag);

            if (tiles_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_tile_columns_minus1);
                size += stream.WriteUnsignedIntGolomb(this.num_tile_rows_minus1);
                size += stream.WriteUnsignedInt(1, this.uniform_spacing_flag);

                if (uniform_spacing_flag == 0)
                {

                    for (i = 0; i < num_tile_columns_minus1; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.column_width_minus1[i]);
                    }

                    for (i = 0; i < num_tile_rows_minus1; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.row_height_minus1[i]);
                    }
                }
                size += stream.WriteUnsignedInt(1, this.loop_filter_across_tiles_enabled_flag);
            }
            size += stream.WriteUnsignedInt(1, this.pps_loop_filter_across_slices_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.deblocking_filter_control_present_flag);

            if (deblocking_filter_control_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.deblocking_filter_override_enabled_flag);
                size += stream.WriteUnsignedInt(1, this.pps_deblocking_filter_disabled_flag);

                if (pps_deblocking_filter_disabled_flag == 0)
                {
                    size += stream.WriteSignedIntGolomb(this.pps_beta_offset_div2);
                    size += stream.WriteSignedIntGolomb(this.pps_tc_offset_div2);
                }
            }
            size += stream.WriteUnsignedInt(1, this.pps_scaling_list_data_present_flag);

            if (pps_scaling_list_data_present_flag != 0)
            {
                size += stream.WriteClass<ScalingListData>(context, this.scaling_list_data);
            }
            size += stream.WriteUnsignedInt(1, this.lists_modification_present_flag);
            size += stream.WriteUnsignedIntGolomb(this.log2_parallel_merge_level_minus2);
            size += stream.WriteUnsignedInt(1, this.slice_segment_header_extension_present_flag);
            size += stream.WriteUnsignedInt(1, this.pps_extension_present_flag);

            if (pps_extension_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.pps_range_extension_flag);
                size += stream.WriteUnsignedInt(1, this.pps_multilayer_extension_flag);
                size += stream.WriteUnsignedInt(1, this.pps_3d_extension_flag);
                size += stream.WriteUnsignedInt(1, this.pps_scc_extension_flag);
                size += stream.WriteUnsignedInt(4, this.pps_extension_4bits);
            }

            if (pps_range_extension_flag != 0)
            {
                size += stream.WriteClass<PpsRangeExtension>(context, this.pps_range_extension);
            }

            if (pps_multilayer_extension_flag != 0)
            {
                size += stream.WriteClass<PpsMultilayerExtension>(context, this.pps_multilayer_extension); // specified in Annex F 
            }

            if (pps_3d_extension_flag != 0)
            {
                size += stream.WriteClass<Pps3dExtension>(context, this.pps_3d_extension); // specified in Annex I 
            }

            if (pps_scc_extension_flag != 0)
            {
                size += stream.WriteClass<PpsSccExtension>(context, this.pps_scc_extension);
            }

            if (pps_extension_4bits != 0)
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
 

pps_range_extension() { 
 if( transform_skip_enabled_flag )  
  log2_max_transform_skip_block_size_minus2 ue(v) 
 cross_component_prediction_enabled_flag u(1) 
 chroma_qp_offset_list_enabled_flag u(1) 
 if( chroma_qp_offset_list_enabled_flag ) {  
  diff_cu_chroma_qp_offset_depth ue(v) 
  chroma_qp_offset_list_len_minus1 ue(v) 
  for( i = 0; i <= chroma_qp_offset_list_len_minus1; i++ ) {  
   cb_qp_offset_list[ i ] se(v) 
   cr_qp_offset_list[ i ] se(v) 
  }  
 }  
 log2_sao_offset_scale_luma ue(v) 
 log2_sao_offset_scale_chroma ue(v) 
}
    */
    public class PpsRangeExtension : IItuSerializable
    {
        private uint log2_max_transform_skip_block_size_minus2;
        public uint Log2MaxTransformSkipBlockSizeMinus2 { get { return log2_max_transform_skip_block_size_minus2; } set { log2_max_transform_skip_block_size_minus2 = value; } }
        private byte cross_component_prediction_enabled_flag;
        public byte CrossComponentPredictionEnabledFlag { get { return cross_component_prediction_enabled_flag; } set { cross_component_prediction_enabled_flag = value; } }
        private byte chroma_qp_offset_list_enabled_flag;
        public byte ChromaQpOffsetListEnabledFlag { get { return chroma_qp_offset_list_enabled_flag; } set { chroma_qp_offset_list_enabled_flag = value; } }
        private uint diff_cu_chroma_qp_offset_depth;
        public uint DiffCuChromaQpOffsetDepth { get { return diff_cu_chroma_qp_offset_depth; } set { diff_cu_chroma_qp_offset_depth = value; } }
        private uint chroma_qp_offset_list_len_minus1;
        public uint ChromaQpOffsetListLenMinus1 { get { return chroma_qp_offset_list_len_minus1; } set { chroma_qp_offset_list_len_minus1 = value; } }
        private int[] cb_qp_offset_list;
        public int[] CbQpOffsetList { get { return cb_qp_offset_list; } set { cb_qp_offset_list = value; } }
        private int[] cr_qp_offset_list;
        public int[] CrQpOffsetList { get { return cr_qp_offset_list; } set { cr_qp_offset_list = value; } }
        private uint log2_sao_offset_scale_luma;
        public uint Log2SaoOffsetScaleLuma { get { return log2_sao_offset_scale_luma; } set { log2_sao_offset_scale_luma = value; } }
        private uint log2_sao_offset_scale_chroma;
        public uint Log2SaoOffsetScaleChroma { get { return log2_sao_offset_scale_chroma; } set { log2_sao_offset_scale_chroma = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PpsRangeExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            if (((H265Context)context).PicParameterSetRbsp.TransformSkipEnabledFlag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_transform_skip_block_size_minus2);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.cross_component_prediction_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.chroma_qp_offset_list_enabled_flag);

            if (chroma_qp_offset_list_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.diff_cu_chroma_qp_offset_depth);
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_qp_offset_list_len_minus1);

                this.cb_qp_offset_list = new int[chroma_qp_offset_list_len_minus1 + 1];
                this.cr_qp_offset_list = new int[chroma_qp_offset_list_len_minus1 + 1];
                for (i = 0; i <= chroma_qp_offset_list_len_minus1; i++)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.cb_qp_offset_list[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.cr_qp_offset_list[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_sao_offset_scale_luma);
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_sao_offset_scale_chroma);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            if (((H265Context)context).PicParameterSetRbsp.TransformSkipEnabledFlag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.log2_max_transform_skip_block_size_minus2);
            }
            size += stream.WriteUnsignedInt(1, this.cross_component_prediction_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.chroma_qp_offset_list_enabled_flag);

            if (chroma_qp_offset_list_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.diff_cu_chroma_qp_offset_depth);
                size += stream.WriteUnsignedIntGolomb(this.chroma_qp_offset_list_len_minus1);

                for (i = 0; i <= chroma_qp_offset_list_len_minus1; i++)
                {
                    size += stream.WriteSignedIntGolomb(this.cb_qp_offset_list[i]);
                    size += stream.WriteSignedIntGolomb(this.cr_qp_offset_list[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.log2_sao_offset_scale_luma);
            size += stream.WriteUnsignedIntGolomb(this.log2_sao_offset_scale_chroma);

            return size;
        }

    }

    /*
  

pps_scc_extension() { 
 pps_curr_pic_ref_enabled_flag u(1) 
 residual_adaptive_colour_transform_enabled_flag u(1) 
 if( residual_adaptive_colour_transform_enabled_flag ) {  
  pps_slice_act_qp_offsets_present_flag u(1) 
  pps_act_y_qp_offset_plus5 se(v) 
  pps_act_cb_qp_offset_plus5 se(v) 
  pps_act_cr_qp_offset_plus3 se(v) 
 }  
 pps_palette_predictor_initializers_present_flag u(1) 
 if( pps_palette_predictor_initializers_present_flag ) {  
  pps_num_palette_predictor_initializers ue(v) 
  if( pps_num_palette_predictor_initializers > 0 ) {  
   monochrome_palette_flag u(1) 
   luma_bit_depth_entry_minus8 ue(v) 
   if( !monochrome_palette_flag )  
    chroma_bit_depth_entry_minus8 ue(v) 
   numComps = monochrome_palette_flag != 0 ?  1 : 3  
   for( comp = 0; comp < numComps; comp++ )  
    for( i = 0; i < pps_num_palette_predictor_initializers; i++ )  
     pps_palette_predictor_initializer[ comp ][ i ] u(v) 
  }  
 }  
}
    */
    public class PpsSccExtension : IItuSerializable
    {
        private byte pps_curr_pic_ref_enabled_flag;
        public byte PpsCurrPicRefEnabledFlag { get { return pps_curr_pic_ref_enabled_flag; } set { pps_curr_pic_ref_enabled_flag = value; } }
        private byte residual_adaptive_colour_transform_enabled_flag;
        public byte ResidualAdaptiveColourTransformEnabledFlag { get { return residual_adaptive_colour_transform_enabled_flag; } set { residual_adaptive_colour_transform_enabled_flag = value; } }
        private byte pps_slice_act_qp_offsets_present_flag;
        public byte PpsSliceActQpOffsetsPresentFlag { get { return pps_slice_act_qp_offsets_present_flag; } set { pps_slice_act_qp_offsets_present_flag = value; } }
        private int pps_act_y_qp_offset_plus5;
        public int PpsActyQpOffsetPlus5 { get { return pps_act_y_qp_offset_plus5; } set { pps_act_y_qp_offset_plus5 = value; } }
        private int pps_act_cb_qp_offset_plus5;
        public int PpsActCbQpOffsetPlus5 { get { return pps_act_cb_qp_offset_plus5; } set { pps_act_cb_qp_offset_plus5 = value; } }
        private int pps_act_cr_qp_offset_plus3;
        public int PpsActCrQpOffsetPlus3 { get { return pps_act_cr_qp_offset_plus3; } set { pps_act_cr_qp_offset_plus3 = value; } }
        private byte pps_palette_predictor_initializers_present_flag;
        public byte PpsPalettePredictorInitializersPresentFlag { get { return pps_palette_predictor_initializers_present_flag; } set { pps_palette_predictor_initializers_present_flag = value; } }
        private uint pps_num_palette_predictor_initializers;
        public uint PpsNumPalettePredictorInitializers { get { return pps_num_palette_predictor_initializers; } set { pps_num_palette_predictor_initializers = value; } }
        private byte monochrome_palette_flag;
        public byte MonochromePaletteFlag { get { return monochrome_palette_flag; } set { monochrome_palette_flag = value; } }
        private uint luma_bit_depth_entry_minus8;
        public uint LumaBitDepthEntryMinus8 { get { return luma_bit_depth_entry_minus8; } set { luma_bit_depth_entry_minus8 = value; } }
        private uint chroma_bit_depth_entry_minus8;
        public uint ChromaBitDepthEntryMinus8 { get { return chroma_bit_depth_entry_minus8; } set { chroma_bit_depth_entry_minus8 = value; } }
        private uint[][] pps_palette_predictor_initializer;
        public uint[][] PpsPalettePredictorInitializer { get { return pps_palette_predictor_initializer; } set { pps_palette_predictor_initializer = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PpsSccExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            long numComps = 0;
            uint comp = 0;
            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.pps_curr_pic_ref_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.residual_adaptive_colour_transform_enabled_flag);

            if (residual_adaptive_colour_transform_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.pps_slice_act_qp_offsets_present_flag);
                size += stream.ReadSignedIntGolomb(size, out this.pps_act_y_qp_offset_plus5);
                size += stream.ReadSignedIntGolomb(size, out this.pps_act_cb_qp_offset_plus5);
                size += stream.ReadSignedIntGolomb(size, out this.pps_act_cr_qp_offset_plus3);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.pps_palette_predictor_initializers_present_flag);

            if (pps_palette_predictor_initializers_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pps_num_palette_predictor_initializers);

                if (pps_num_palette_predictor_initializers > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.monochrome_palette_flag);
                    size += stream.ReadUnsignedIntGolomb(size, out this.luma_bit_depth_entry_minus8);

                    if (monochrome_palette_flag == 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.chroma_bit_depth_entry_minus8);
                    }
                    numComps = monochrome_palette_flag != 0 ? 1 : 3;

                    this.pps_palette_predictor_initializer = new uint[numComps][];
                    for (comp = 0; comp < numComps; comp++)
                    {

                        this.pps_palette_predictor_initializer[comp] = new uint[pps_num_palette_predictor_initializers];
                        for (i = 0; i < pps_num_palette_predictor_initializers; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, comp == 0 ? (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.LumaBitDepthEntryMinus8 + 8) : (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.ChromaBitDepthEntryMinus8 + 8), out this.pps_palette_predictor_initializer[comp][i]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            long numComps = 0;
            uint comp = 0;
            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.pps_curr_pic_ref_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.residual_adaptive_colour_transform_enabled_flag);

            if (residual_adaptive_colour_transform_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.pps_slice_act_qp_offsets_present_flag);
                size += stream.WriteSignedIntGolomb(this.pps_act_y_qp_offset_plus5);
                size += stream.WriteSignedIntGolomb(this.pps_act_cb_qp_offset_plus5);
                size += stream.WriteSignedIntGolomb(this.pps_act_cr_qp_offset_plus3);
            }
            size += stream.WriteUnsignedInt(1, this.pps_palette_predictor_initializers_present_flag);

            if (pps_palette_predictor_initializers_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pps_num_palette_predictor_initializers);

                if (pps_num_palette_predictor_initializers > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.monochrome_palette_flag);
                    size += stream.WriteUnsignedIntGolomb(this.luma_bit_depth_entry_minus8);

                    if (monochrome_palette_flag == 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.chroma_bit_depth_entry_minus8);
                    }
                    numComps = monochrome_palette_flag != 0 ? 1 : 3;

                    for (comp = 0; comp < numComps; comp++)
                    {

                        for (i = 0; i < pps_num_palette_predictor_initializers; i++)
                        {
                            size += stream.WriteUnsignedIntVariable(comp == 0 ? (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.LumaBitDepthEntryMinus8 + 8) : (((H265Context)context).PicParameterSetRbsp.PpsSccExtension.ChromaBitDepthEntryMinus8 + 8), this.pps_palette_predictor_initializer[comp][i]);
                        }
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
 pic_type u(3) 
 rbsp_trailing_bits()  
}
    */
    public class AccessUnitDelimiterRbsp : IItuSerializable
    {
        private uint pic_type;
        public uint PicType { get { return pic_type; } set { pic_type = value; } }
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

            size += stream.ReadUnsignedInt(size, 3, out this.pic_type);
            this.rbsp_trailing_bits = new RbspTrailingBits();
            size += stream.ReadClass<RbspTrailingBits>(size, context, this.rbsp_trailing_bits);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(3, this.pic_type);
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
while( next_bits( 8 ) == 0xFF )  
ff_byte  /* equal to 0xFF *//* f(8) 
rbsp_trailing_bits()  
}
    */
    public class FillerDataRbsp : IItuSerializable
    {
        private Dictionary<int, uint> ff_byte = new Dictionary<int, uint>();
        public Dictionary<int, uint> FfByte { get { return ff_byte; } set { ff_byte = value; } }
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

                size += stream.ReadFixed(size, 8, whileIndex, this.ff_byte); // equal to 0xFF 
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

                size += stream.WriteFixed(8, whileIndex, this.ff_byte); // equal to 0xFF 
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
  

slice_segment_layer_rbsp() { 
slice_segment_header()  
/*slice_segment_data()  *//*
/* rbsp_slice_segment_trailing_bits()   *//*
}
    */
    public class SliceSegmentLayerRbsp : IItuSerializable
    {
        private SliceSegmentHeader slice_segment_header;
        public SliceSegmentHeader SliceSegmentHeader { get { return slice_segment_header; } set { slice_segment_header = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SliceSegmentLayerRbsp()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            this.slice_segment_header = new SliceSegmentHeader();
            size += stream.ReadClass<SliceSegmentHeader>(size, context, this.slice_segment_header); //slice_segment_data()  
            /*  rbsp_slice_segment_trailing_bits()    */


            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteClass<SliceSegmentHeader>(context, this.slice_segment_header); //slice_segment_data()  
            /*  rbsp_slice_segment_trailing_bits()    */


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
 alignment_bit_equal_to_one  /* equal to 1 *//* f(1) 
 while( !byte_aligned() )  
  alignment_bit_equal_to_zero  /* equal to 0 *//* f(1) 
}
    */
    public class ByteAlignment : IItuSerializable
    {
        private uint alignment_bit_equal_to_one;
        public uint AlignmentBitEqualToOne { get { return alignment_bit_equal_to_one; } set { alignment_bit_equal_to_one = value; } }
        private Dictionary<int, uint> alignment_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> AlignmentBitEqualToZero { get { return alignment_bit_equal_to_zero; } set { alignment_bit_equal_to_zero = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ByteAlignment()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.ReadFixed(size, 1, out this.alignment_bit_equal_to_one); // equal to 1 

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadFixed(size, 1, whileIndex, this.alignment_bit_equal_to_zero); // equal to 0 
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            size += stream.WriteFixed(1, this.alignment_bit_equal_to_one); // equal to 1 

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteFixed(1, whileIndex, this.alignment_bit_equal_to_zero); // equal to 0 
            }

            return size;
        }

    }

    /*
  

profile_tier_level( profilePresentFlag, maxNumSubLayersMinus1 ) { 
 if( profilePresentFlag ) {  
  general_profile_space u(2) 
  general_tier_flag u(1) 
  general_profile_idc u(5) 
  for( j = 0; j < 32; j++ )  
   general_profile_compatibility_flag[ j ] u(1) 
  general_progressive_source_flag u(1) 
  general_interlaced_source_flag u(1) 
  general_non_packed_constraint_flag u(1) 
  general_frame_only_constraint_flag u(1) 
  if( general_profile_idc == 4  ||  general_profile_compatibility_flag[ 4 ]  || 
   general_profile_idc == 5  ||  general_profile_compatibility_flag[ 5 ]  || 
   general_profile_idc == 6  ||  general_profile_compatibility_flag[ 6 ]  || 
   general_profile_idc == 7  ||  general_profile_compatibility_flag[ 7 ]  || 
   general_profile_idc == 8  ||  general_profile_compatibility_flag[ 8 ]  || 
   general_profile_idc == 9  ||  general_profile_compatibility_flag[ 9 ]  || 
   general_profile_idc == 10  ||  general_profile_compatibility_flag[ 10 ] ) { 
   /* The number of bits in this syntax structure is not affected by this condition *//* 
 
   general_max_12bit_constraint_flag u(1) 
   general_max_10bit_constraint_flag u(1) 
   general_max_8bit_constraint_flag u(1) 
   general_max_422chroma_constraint_flag u(1) 
   general_max_420chroma_constraint_flag u(1) 
   general_max_monochrome_constraint_flag u(1) 
   general_intra_constraint_flag u(1) 
   general_one_picture_only_constraint_flag u(1) 
   general_lower_bit_rate_constraint_flag u(1) 
   if( general_profile_idc == 5 ||  general_profile_compatibility_flag[ 5 ]  || 
    general_profile_idc == 9  ||  general_profile_compatibility_flag[ 9 ]  || 
    general_profile_idc == 10  ||  general_profile_compatibility_flag[ 10 ] ) { 
 
    general_max_14bit_constraint_flag u(1) 
    general_reserved_zero_33bits u(33) 
   } else  
    general_reserved_zero_34bits u(34) 
  } else if( general_profile_idc == 2  ||  general_profile_compatibility_flag[ 2 ] ) {  
   general_reserved_zero_7bits u(7) 
   general_one_picture_only_constraint_flag u(1) 
   general_reserved_zero_35bits u(35) 
  } else  
     general_reserved_zero_43bits u(43) 
  if( ( general_profile_idc >= 1  &&  general_profile_idc <= 5 )  || 
    general_profile_idc == 9  || 
    general_profile_compatibility_flag[ 1 ]  ||  general_profile_compatibility_flag[ 2 ]  || 
    general_profile_compatibility_flag[ 3 ]  ||  general_profile_compatibility_flag[ 4 ]  || 
    general_profile_compatibility_flag[ 5 ]  ||  general_profile_compatibility_flag[ 9 ] ) 
   /* The number of bits in this syntax structure is not affected by this condition *//* 
   general_inbld_flag u(1) 
  else  
   general_reserved_zero_bit u(1) 
 }  
 general_level_idc u(8) 
 for( i = 0; i < maxNumSubLayersMinus1; i++ ) {  
  sub_layer_profile_present_flag[ i ] u(1) 
  sub_layer_level_present_flag[ i ] u(1) 
 }  
 if( maxNumSubLayersMinus1 > 0 )  
  for( i = maxNumSubLayersMinus1; i < 8; i++ )  
   reserved_zero_2bits[ i ] u(2) 
 for( i = 0; i < maxNumSubLayersMinus1; i++ ) {  
  if( sub_layer_profile_present_flag[ i ] ) {  
   sub_layer_profile_space[ i ] u(2) 
   sub_layer_tier_flag[ i ] u(1) 
   sub_layer_profile_idc[ i ] u(5) 
   for( j = 0; j < 32; j++ )  
    sub_layer_profile_compatibility_flag[ i ][ j ] u(1) 
   sub_layer_progressive_source_flag[ i ] u(1) 
   sub_layer_interlaced_source_flag[ i ] u(1) 
   sub_layer_non_packed_constraint_flag[ i ] u(1) 
   sub_layer_frame_only_constraint_flag[ i ] u(1) 
   if( sub_layer_profile_idc[ i ] == 4  ||  sub_layer_profile_compatibility_flag[ i ][ 4 ]  || 
    sub_layer_profile_idc[ i ] == 5  ||  sub_layer_profile_compatibility_flag[ i ][ 5 ]  || 
    sub_layer_profile_idc[ i ] == 6  ||  sub_layer_profile_compatibility_flag[ i ][ 6 ]  || 
    sub_layer_profile_idc[ i ] == 7  ||  sub_layer_profile_compatibility_flag[ i ][ 7 ]  || 
    sub_layer_profile_idc[ i ] == 8  ||  sub_layer_profile_compatibility_flag[ i ][ 8 ]  || 
    sub_layer_profile_idc[ i ] == 9  ||  sub_layer_profile_compatibility_flag[ i ][ 9 ]  || 
    sub_layer_profile_idc[ i ] == 10  ||  sub_layer_profile_compatibility_flag[ i ][ 10 ] 
    ) { 
    /* The number of bits in this syntax structure is not affected by this condition *//* 
 
    sub_layer_max_12bit_constraint_flag[ i ] u(1) 
    sub_layer_max_10bit_constraint_flag[ i ] u(1) 
    sub_layer_max_8bit_constraint_flag[ i ] u(1) 
    sub_layer_max_422chroma_constraint_flag[ i ] u(1) 
    sub_layer_max_420chroma_constraint_flag[ i ] u(1) 
    sub_layer_max_monochrome_constraint_flag[ i ] u(1) 
    sub_layer_intra_constraint_flag[ i ] u(1) 
    sub_layer_one_picture_only_constraint_flag[ i ] u(1) 
    sub_layer_lower_bit_rate_constraint_flag[ i ] u(1)
       if( sub_layer_profile_idc[ i ] == 5 || 
     sub_layer_profile_compatibility_flag[ i ][ 5 ] ) { 
 
     sub_layer_max_14bit_constraint_flag[ i ] u(1) 
     sub_layer_reserved_zero_33bits[ i ] u(33) 
    } else  
     sub_layer_reserved_zero_34bits[ i ] u(34) 
   } else if( sub_layer_profile_idc[ i ] == 2  || 
       sub_layer_profile_compatibility_flag[ i ][ 2 ] ) { 
 
    sub_layer_reserved_zero_7bits[ i ] u(7) 
    sub_layer_one_picture_only_constraint_flag[ i ] u(1) 
    sub_layer_reserved_zero_35bits[ i ] u(35) 
   } else  
    sub_layer_reserved_zero_43bits[ i ] u(43) 
   if( ( sub_layer_profile_idc[ i ] >= 1  &&  sub_layer_profile_idc[ i ] <= 5 )  || 
     sub_layer_profile_idc[ i ] == 9  || 
     sub_layer_profile_compatibility_flag[ i ][ 1 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 2 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 3 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 4 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 5 ]  || 
     sub_layer_profile_compatibility_flag[ i ][ 9 ] ) 
    /* The number of bits in this syntax structure is not affected by this condition *//* 
 
    sub_layer_inbld_flag[ i ] u(1) 
   else  
    sub_layer_reserved_zero_bit[ i ] u(1) 
  }  
  if( sub_layer_level_present_flag[ i ] )  
   sub_layer_level_idc[ i ] u(8) 
 }  
}
    */
    public class ProfileTierLevel : IItuSerializable
    {
        private uint profilePresentFlag;
        public uint ProfilePresentFlag { get { return profilePresentFlag; } set { profilePresentFlag = value; } }
        private uint maxNumSubLayersMinus1;
        public uint MaxNumSubLayersMinus1 { get { return maxNumSubLayersMinus1; } set { maxNumSubLayersMinus1 = value; } }
        private uint general_profile_space;
        public uint GeneralProfileSpace { get { return general_profile_space; } set { general_profile_space = value; } }
        private byte general_tier_flag;
        public byte GeneralTierFlag { get { return general_tier_flag; } set { general_tier_flag = value; } }
        private uint general_profile_idc;
        public uint GeneralProfileIdc { get { return general_profile_idc; } set { general_profile_idc = value; } }
        private byte[] general_profile_compatibility_flag;
        public byte[] GeneralProfileCompatibilityFlag { get { return general_profile_compatibility_flag; } set { general_profile_compatibility_flag = value; } }
        private byte general_progressive_source_flag;
        public byte GeneralProgressiveSourceFlag { get { return general_progressive_source_flag; } set { general_progressive_source_flag = value; } }
        private byte general_interlaced_source_flag;
        public byte GeneralInterlacedSourceFlag { get { return general_interlaced_source_flag; } set { general_interlaced_source_flag = value; } }
        private byte general_non_packed_constraint_flag;
        public byte GeneralNonPackedConstraintFlag { get { return general_non_packed_constraint_flag; } set { general_non_packed_constraint_flag = value; } }
        private byte general_frame_only_constraint_flag;
        public byte GeneralFrameOnlyConstraintFlag { get { return general_frame_only_constraint_flag; } set { general_frame_only_constraint_flag = value; } }
        private byte general_max_12bit_constraint_flag;
        public byte GeneralMax12bitConstraintFlag { get { return general_max_12bit_constraint_flag; } set { general_max_12bit_constraint_flag = value; } }
        private byte general_max_10bit_constraint_flag;
        public byte GeneralMax10bitConstraintFlag { get { return general_max_10bit_constraint_flag; } set { general_max_10bit_constraint_flag = value; } }
        private byte general_max_8bit_constraint_flag;
        public byte GeneralMax8bitConstraintFlag { get { return general_max_8bit_constraint_flag; } set { general_max_8bit_constraint_flag = value; } }
        private byte general_max_422chroma_constraint_flag;
        public byte GeneralMax422chromaConstraintFlag { get { return general_max_422chroma_constraint_flag; } set { general_max_422chroma_constraint_flag = value; } }
        private byte general_max_420chroma_constraint_flag;
        public byte GeneralMax420chromaConstraintFlag { get { return general_max_420chroma_constraint_flag; } set { general_max_420chroma_constraint_flag = value; } }
        private byte general_max_monochrome_constraint_flag;
        public byte GeneralMaxMonochromeConstraintFlag { get { return general_max_monochrome_constraint_flag; } set { general_max_monochrome_constraint_flag = value; } }
        private byte general_intra_constraint_flag;
        public byte GeneralIntraConstraintFlag { get { return general_intra_constraint_flag; } set { general_intra_constraint_flag = value; } }
        private byte general_one_picture_only_constraint_flag;
        public byte GeneralOnePictureOnlyConstraintFlag { get { return general_one_picture_only_constraint_flag; } set { general_one_picture_only_constraint_flag = value; } }
        private byte general_lower_bit_rate_constraint_flag;
        public byte GeneralLowerBitRateConstraintFlag { get { return general_lower_bit_rate_constraint_flag; } set { general_lower_bit_rate_constraint_flag = value; } }
        private byte general_max_14bit_constraint_flag;
        public byte GeneralMax14bitConstraintFlag { get { return general_max_14bit_constraint_flag; } set { general_max_14bit_constraint_flag = value; } }
        private ulong general_reserved_zero_33bits;
        public ulong GeneralReservedZero33bits { get { return general_reserved_zero_33bits; } set { general_reserved_zero_33bits = value; } }
        private ulong general_reserved_zero_34bits;
        public ulong GeneralReservedZero34bits { get { return general_reserved_zero_34bits; } set { general_reserved_zero_34bits = value; } }
        private uint general_reserved_zero_7bits;
        public uint GeneralReservedZero7bits { get { return general_reserved_zero_7bits; } set { general_reserved_zero_7bits = value; } }
        private ulong general_reserved_zero_35bits;
        public ulong GeneralReservedZero35bits { get { return general_reserved_zero_35bits; } set { general_reserved_zero_35bits = value; } }
        private ulong general_reserved_zero_43bits;
        public ulong GeneralReservedZero43bits { get { return general_reserved_zero_43bits; } set { general_reserved_zero_43bits = value; } }
        private byte general_inbld_flag;
        public byte GeneralInbldFlag { get { return general_inbld_flag; } set { general_inbld_flag = value; } }
        private byte general_reserved_zero_bit;
        public byte GeneralReservedZeroBit { get { return general_reserved_zero_bit; } set { general_reserved_zero_bit = value; } }
        private uint general_level_idc;
        public uint GeneralLevelIdc { get { return general_level_idc; } set { general_level_idc = value; } }
        private byte[] sub_layer_profile_present_flag;
        public byte[] SubLayerProfilePresentFlag { get { return sub_layer_profile_present_flag; } set { sub_layer_profile_present_flag = value; } }
        private byte[] sub_layer_level_present_flag;
        public byte[] SubLayerLevelPresentFlag { get { return sub_layer_level_present_flag; } set { sub_layer_level_present_flag = value; } }
        private uint[] reserved_zero_2bits;
        public uint[] ReservedZero2bits { get { return reserved_zero_2bits; } set { reserved_zero_2bits = value; } }
        private uint[] sub_layer_profile_space;
        public uint[] SubLayerProfileSpace { get { return sub_layer_profile_space; } set { sub_layer_profile_space = value; } }
        private byte[] sub_layer_tier_flag;
        public byte[] SubLayerTierFlag { get { return sub_layer_tier_flag; } set { sub_layer_tier_flag = value; } }
        private uint[] sub_layer_profile_idc;
        public uint[] SubLayerProfileIdc { get { return sub_layer_profile_idc; } set { sub_layer_profile_idc = value; } }
        private byte[][] sub_layer_profile_compatibility_flag;
        public byte[][] SubLayerProfileCompatibilityFlag { get { return sub_layer_profile_compatibility_flag; } set { sub_layer_profile_compatibility_flag = value; } }
        private byte[] sub_layer_progressive_source_flag;
        public byte[] SubLayerProgressiveSourceFlag { get { return sub_layer_progressive_source_flag; } set { sub_layer_progressive_source_flag = value; } }
        private byte[] sub_layer_interlaced_source_flag;
        public byte[] SubLayerInterlacedSourceFlag { get { return sub_layer_interlaced_source_flag; } set { sub_layer_interlaced_source_flag = value; } }
        private byte[] sub_layer_non_packed_constraint_flag;
        public byte[] SubLayerNonPackedConstraintFlag { get { return sub_layer_non_packed_constraint_flag; } set { sub_layer_non_packed_constraint_flag = value; } }
        private byte[] sub_layer_frame_only_constraint_flag;
        public byte[] SubLayerFrameOnlyConstraintFlag { get { return sub_layer_frame_only_constraint_flag; } set { sub_layer_frame_only_constraint_flag = value; } }
        private byte[] sub_layer_max_12bit_constraint_flag;
        public byte[] SubLayerMax12bitConstraintFlag { get { return sub_layer_max_12bit_constraint_flag; } set { sub_layer_max_12bit_constraint_flag = value; } }
        private byte[] sub_layer_max_10bit_constraint_flag;
        public byte[] SubLayerMax10bitConstraintFlag { get { return sub_layer_max_10bit_constraint_flag; } set { sub_layer_max_10bit_constraint_flag = value; } }
        private byte[] sub_layer_max_8bit_constraint_flag;
        public byte[] SubLayerMax8bitConstraintFlag { get { return sub_layer_max_8bit_constraint_flag; } set { sub_layer_max_8bit_constraint_flag = value; } }
        private byte[] sub_layer_max_422chroma_constraint_flag;
        public byte[] SubLayerMax422chromaConstraintFlag { get { return sub_layer_max_422chroma_constraint_flag; } set { sub_layer_max_422chroma_constraint_flag = value; } }
        private byte[] sub_layer_max_420chroma_constraint_flag;
        public byte[] SubLayerMax420chromaConstraintFlag { get { return sub_layer_max_420chroma_constraint_flag; } set { sub_layer_max_420chroma_constraint_flag = value; } }
        private byte[] sub_layer_max_monochrome_constraint_flag;
        public byte[] SubLayerMaxMonochromeConstraintFlag { get { return sub_layer_max_monochrome_constraint_flag; } set { sub_layer_max_monochrome_constraint_flag = value; } }
        private byte[] sub_layer_intra_constraint_flag;
        public byte[] SubLayerIntraConstraintFlag { get { return sub_layer_intra_constraint_flag; } set { sub_layer_intra_constraint_flag = value; } }
        private byte[] sub_layer_one_picture_only_constraint_flag;
        public byte[] SubLayerOnePictureOnlyConstraintFlag { get { return sub_layer_one_picture_only_constraint_flag; } set { sub_layer_one_picture_only_constraint_flag = value; } }
        private byte[] sub_layer_lower_bit_rate_constraint_flag;
        public byte[] SubLayerLowerBitRateConstraintFlag { get { return sub_layer_lower_bit_rate_constraint_flag; } set { sub_layer_lower_bit_rate_constraint_flag = value; } }
        private byte[] sub_layer_max_14bit_constraint_flag;
        public byte[] SubLayerMax14bitConstraintFlag { get { return sub_layer_max_14bit_constraint_flag; } set { sub_layer_max_14bit_constraint_flag = value; } }
        private ulong[] sub_layer_reserved_zero_33bits;
        public ulong[] SubLayerReservedZero33bits { get { return sub_layer_reserved_zero_33bits; } set { sub_layer_reserved_zero_33bits = value; } }
        private ulong[] sub_layer_reserved_zero_34bits;
        public ulong[] SubLayerReservedZero34bits { get { return sub_layer_reserved_zero_34bits; } set { sub_layer_reserved_zero_34bits = value; } }
        private uint[] sub_layer_reserved_zero_7bits;
        public uint[] SubLayerReservedZero7bits { get { return sub_layer_reserved_zero_7bits; } set { sub_layer_reserved_zero_7bits = value; } }
        private ulong[] sub_layer_reserved_zero_35bits;
        public ulong[] SubLayerReservedZero35bits { get { return sub_layer_reserved_zero_35bits; } set { sub_layer_reserved_zero_35bits = value; } }
        private ulong[] sub_layer_reserved_zero_43bits;
        public ulong[] SubLayerReservedZero43bits { get { return sub_layer_reserved_zero_43bits; } set { sub_layer_reserved_zero_43bits = value; } }
        private byte[] sub_layer_inbld_flag;
        public byte[] SubLayerInbldFlag { get { return sub_layer_inbld_flag; } set { sub_layer_inbld_flag = value; } }
        private byte[] sub_layer_reserved_zero_bit;
        public byte[] SubLayerReservedZeroBit { get { return sub_layer_reserved_zero_bit; } set { sub_layer_reserved_zero_bit = value; } }
        private uint[] sub_layer_level_idc;
        public uint[] SubLayerLevelIdc { get { return sub_layer_level_idc; } set { sub_layer_level_idc = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ProfileTierLevel(uint profilePresentFlag, uint maxNumSubLayersMinus1)
        {
            this.profilePresentFlag = profilePresentFlag;
            this.maxNumSubLayersMinus1 = maxNumSubLayersMinus1;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint j = 0;
            uint i = 0;

            if (profilePresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.general_profile_space);
                size += stream.ReadUnsignedInt(size, 1, out this.general_tier_flag);
                size += stream.ReadUnsignedInt(size, 5, out this.general_profile_idc);

                this.general_profile_compatibility_flag = new byte[32];
                for (j = 0; j < 32; j++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.general_profile_compatibility_flag[j]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.general_progressive_source_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.general_interlaced_source_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.general_non_packed_constraint_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.general_frame_only_constraint_flag);

                if (general_profile_idc == 4 || general_profile_compatibility_flag[4] != 0 ||
   general_profile_idc == 5 || general_profile_compatibility_flag[5] != 0 ||
   general_profile_idc == 6 || general_profile_compatibility_flag[6] != 0 ||
   general_profile_idc == 7 || general_profile_compatibility_flag[7] != 0 ||
   general_profile_idc == 8 || general_profile_compatibility_flag[8] != 0 ||
   general_profile_idc == 9 || general_profile_compatibility_flag[9] != 0 ||
   general_profile_idc == 10 || general_profile_compatibility_flag[10] != 0)
                {
                    /*  The number of bits in this syntax structure is not affected by this condition  */

                    size += stream.ReadUnsignedInt(size, 1, out this.general_max_12bit_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_max_10bit_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_max_8bit_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_max_422chroma_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_max_420chroma_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_max_monochrome_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_intra_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_one_picture_only_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_lower_bit_rate_constraint_flag);

                    if (general_profile_idc == 5 || general_profile_compatibility_flag[5] != 0 ||
    general_profile_idc == 9 || general_profile_compatibility_flag[9] != 0 ||
    general_profile_idc == 10 || general_profile_compatibility_flag[10] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.general_max_14bit_constraint_flag);
                        size += stream.ReadUnsignedInt(size, 33, out this.general_reserved_zero_33bits);
                    }
                    else
                    {
                        size += stream.ReadUnsignedInt(size, 34, out this.general_reserved_zero_34bits);
                    }
                }
                else if (general_profile_idc == 2 || general_profile_compatibility_flag[2] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 7, out this.general_reserved_zero_7bits);
                    size += stream.ReadUnsignedInt(size, 1, out this.general_one_picture_only_constraint_flag);
                    size += stream.ReadUnsignedInt(size, 35, out this.general_reserved_zero_35bits);
                }
                else
                {
                    size += stream.ReadUnsignedInt(size, 43, out this.general_reserved_zero_43bits);
                }

                if ((general_profile_idc >= 1 && general_profile_idc <= 5) ||
    general_profile_idc == 9 ||
    general_profile_compatibility_flag[1] != 0 || general_profile_compatibility_flag[2] != 0 ||
    general_profile_compatibility_flag[3] != 0 || general_profile_compatibility_flag[4] != 0 ||
    general_profile_compatibility_flag[5] != 0 || general_profile_compatibility_flag[9] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.general_inbld_flag);
                }
                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.general_reserved_zero_bit);
                }
            }
            size += stream.ReadUnsignedInt(size, 8, out this.general_level_idc);

            this.sub_layer_profile_present_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_level_present_flag = new byte[maxNumSubLayersMinus1 + 1];
            for (i = 0; i < maxNumSubLayersMinus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_profile_present_flag[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_level_present_flag[i]);
            }

            if (maxNumSubLayersMinus1 > 0)
            {

                this.reserved_zero_2bits = new uint[8];
                for (i = maxNumSubLayersMinus1; i < 8; i++)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.reserved_zero_2bits[i]);
                }
            }

            this.sub_layer_profile_space = new uint[maxNumSubLayersMinus1 + 1];
            this.sub_layer_tier_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_profile_idc = new uint[maxNumSubLayersMinus1 + 1];
            this.sub_layer_profile_compatibility_flag = new byte[maxNumSubLayersMinus1 + 1][];
            this.sub_layer_progressive_source_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_interlaced_source_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_non_packed_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_frame_only_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_12bit_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_10bit_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_8bit_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_422chroma_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_420chroma_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_monochrome_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_intra_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_one_picture_only_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_lower_bit_rate_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_max_14bit_constraint_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_reserved_zero_33bits = new ulong[maxNumSubLayersMinus1 + 1];
            this.sub_layer_reserved_zero_34bits = new ulong[maxNumSubLayersMinus1 + 1];
            this.sub_layer_reserved_zero_7bits = new uint[maxNumSubLayersMinus1 + 1];
            this.sub_layer_reserved_zero_35bits = new ulong[maxNumSubLayersMinus1 + 1];
            this.sub_layer_reserved_zero_43bits = new ulong[maxNumSubLayersMinus1 + 1];
            this.sub_layer_inbld_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_reserved_zero_bit = new byte[maxNumSubLayersMinus1 + 1];
            this.sub_layer_level_idc = new uint[maxNumSubLayersMinus1 + 1];
            for (i = 0; i < maxNumSubLayersMinus1; i++)
            {

                if (sub_layer_profile_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.sub_layer_profile_space[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_tier_flag[i]);
                    size += stream.ReadUnsignedInt(size, 5, out this.sub_layer_profile_idc[i]);

                    this.sub_layer_profile_compatibility_flag[i] = new byte[32];
                    for (j = 0; j < 32; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_profile_compatibility_flag[i][j]);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_progressive_source_flag[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_interlaced_source_flag[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_non_packed_constraint_flag[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_frame_only_constraint_flag[i]);

                    if (sub_layer_profile_idc[i] == 4 || sub_layer_profile_compatibility_flag[i][4] != 0 ||
    sub_layer_profile_idc[i] == 5 || sub_layer_profile_compatibility_flag[i][5] != 0 ||
    sub_layer_profile_idc[i] == 6 || sub_layer_profile_compatibility_flag[i][6] != 0 ||
    sub_layer_profile_idc[i] == 7 || sub_layer_profile_compatibility_flag[i][7] != 0 ||
    sub_layer_profile_idc[i] == 8 || sub_layer_profile_compatibility_flag[i][8] != 0 ||
    sub_layer_profile_idc[i] == 9 || sub_layer_profile_compatibility_flag[i][9] != 0 ||
    sub_layer_profile_idc[i] == 10 || sub_layer_profile_compatibility_flag[i][10] != 0
    )
                    {
                        /*  The number of bits in this syntax structure is not affected by this condition  */

                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_12bit_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_10bit_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_8bit_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_422chroma_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_420chroma_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_monochrome_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_intra_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_one_picture_only_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_lower_bit_rate_constraint_flag[i]);

                        if (sub_layer_profile_idc[i] == 5 ||
     sub_layer_profile_compatibility_flag[i][5] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_max_14bit_constraint_flag[i]);
                            size += stream.ReadUnsignedInt(size, 33, out this.sub_layer_reserved_zero_33bits[i]);
                        }
                        else
                        {
                            size += stream.ReadUnsignedInt(size, 34, out this.sub_layer_reserved_zero_34bits[i]);
                        }
                    }
                    else if (sub_layer_profile_idc[i] == 2 ||
       sub_layer_profile_compatibility_flag[i][2] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 7, out this.sub_layer_reserved_zero_7bits[i]);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_one_picture_only_constraint_flag[i]);
                        size += stream.ReadUnsignedInt(size, 35, out this.sub_layer_reserved_zero_35bits[i]);
                    }
                    else
                    {
                        size += stream.ReadUnsignedInt(size, 43, out this.sub_layer_reserved_zero_43bits[i]);
                    }

                    if ((sub_layer_profile_idc[i] >= 1 && sub_layer_profile_idc[i] <= 5) ||
     sub_layer_profile_idc[i] == 9 ||
     sub_layer_profile_compatibility_flag[i][1] != 0 ||
     sub_layer_profile_compatibility_flag[i][2] != 0 ||
     sub_layer_profile_compatibility_flag[i][3] != 0 ||
     sub_layer_profile_compatibility_flag[i][4] != 0 ||
     sub_layer_profile_compatibility_flag[i][5] != 0 ||
     sub_layer_profile_compatibility_flag[i][9] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_inbld_flag[i]);
                    }
                    else
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_reserved_zero_bit[i]);
                    }
                }

                if (sub_layer_level_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.sub_layer_level_idc[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint j = 0;
            uint i = 0;

            if (profilePresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(2, this.general_profile_space);
                size += stream.WriteUnsignedInt(1, this.general_tier_flag);
                size += stream.WriteUnsignedInt(5, this.general_profile_idc);

                for (j = 0; j < 32; j++)
                {
                    size += stream.WriteUnsignedInt(1, this.general_profile_compatibility_flag[j]);
                }
                size += stream.WriteUnsignedInt(1, this.general_progressive_source_flag);
                size += stream.WriteUnsignedInt(1, this.general_interlaced_source_flag);
                size += stream.WriteUnsignedInt(1, this.general_non_packed_constraint_flag);
                size += stream.WriteUnsignedInt(1, this.general_frame_only_constraint_flag);

                if (general_profile_idc == 4 || general_profile_compatibility_flag[4] != 0 ||
   general_profile_idc == 5 || general_profile_compatibility_flag[5] != 0 ||
   general_profile_idc == 6 || general_profile_compatibility_flag[6] != 0 ||
   general_profile_idc == 7 || general_profile_compatibility_flag[7] != 0 ||
   general_profile_idc == 8 || general_profile_compatibility_flag[8] != 0 ||
   general_profile_idc == 9 || general_profile_compatibility_flag[9] != 0 ||
   general_profile_idc == 10 || general_profile_compatibility_flag[10] != 0)
                {
                    /*  The number of bits in this syntax structure is not affected by this condition  */

                    size += stream.WriteUnsignedInt(1, this.general_max_12bit_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_max_10bit_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_max_8bit_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_max_422chroma_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_max_420chroma_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_max_monochrome_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_intra_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_one_picture_only_constraint_flag);
                    size += stream.WriteUnsignedInt(1, this.general_lower_bit_rate_constraint_flag);

                    if (general_profile_idc == 5 || general_profile_compatibility_flag[5] != 0 ||
    general_profile_idc == 9 || general_profile_compatibility_flag[9] != 0 ||
    general_profile_idc == 10 || general_profile_compatibility_flag[10] != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.general_max_14bit_constraint_flag);
                        size += stream.WriteUnsignedInt(33, this.general_reserved_zero_33bits);
                    }
                    else
                    {
                        size += stream.WriteUnsignedInt(34, this.general_reserved_zero_34bits);
                    }
                }
                else if (general_profile_idc == 2 || general_profile_compatibility_flag[2] != 0)
                {
                    size += stream.WriteUnsignedInt(7, this.general_reserved_zero_7bits);
                    size += stream.WriteUnsignedInt(1, this.general_one_picture_only_constraint_flag);
                    size += stream.WriteUnsignedInt(35, this.general_reserved_zero_35bits);
                }
                else
                {
                    size += stream.WriteUnsignedInt(43, this.general_reserved_zero_43bits);
                }

                if ((general_profile_idc >= 1 && general_profile_idc <= 5) ||
    general_profile_idc == 9 ||
    general_profile_compatibility_flag[1] != 0 || general_profile_compatibility_flag[2] != 0 ||
    general_profile_compatibility_flag[3] != 0 || general_profile_compatibility_flag[4] != 0 ||
    general_profile_compatibility_flag[5] != 0 || general_profile_compatibility_flag[9] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.general_inbld_flag);
                }
                else
                {
                    size += stream.WriteUnsignedInt(1, this.general_reserved_zero_bit);
                }
            }
            size += stream.WriteUnsignedInt(8, this.general_level_idc);

            for (i = 0; i < maxNumSubLayersMinus1; i++)
            {
                size += stream.WriteUnsignedInt(1, this.sub_layer_profile_present_flag[i]);
                size += stream.WriteUnsignedInt(1, this.sub_layer_level_present_flag[i]);
            }

            if (maxNumSubLayersMinus1 > 0)
            {

                for (i = maxNumSubLayersMinus1; i < 8; i++)
                {
                    size += stream.WriteUnsignedInt(2, this.reserved_zero_2bits[i]);
                }
            }

            for (i = 0; i < maxNumSubLayersMinus1; i++)
            {

                if (sub_layer_profile_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(2, this.sub_layer_profile_space[i]);
                    size += stream.WriteUnsignedInt(1, this.sub_layer_tier_flag[i]);
                    size += stream.WriteUnsignedInt(5, this.sub_layer_profile_idc[i]);

                    for (j = 0; j < 32; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.sub_layer_profile_compatibility_flag[i][j]);
                    }
                    size += stream.WriteUnsignedInt(1, this.sub_layer_progressive_source_flag[i]);
                    size += stream.WriteUnsignedInt(1, this.sub_layer_interlaced_source_flag[i]);
                    size += stream.WriteUnsignedInt(1, this.sub_layer_non_packed_constraint_flag[i]);
                    size += stream.WriteUnsignedInt(1, this.sub_layer_frame_only_constraint_flag[i]);

                    if (sub_layer_profile_idc[i] == 4 || sub_layer_profile_compatibility_flag[i][4] != 0 ||
    sub_layer_profile_idc[i] == 5 || sub_layer_profile_compatibility_flag[i][5] != 0 ||
    sub_layer_profile_idc[i] == 6 || sub_layer_profile_compatibility_flag[i][6] != 0 ||
    sub_layer_profile_idc[i] == 7 || sub_layer_profile_compatibility_flag[i][7] != 0 ||
    sub_layer_profile_idc[i] == 8 || sub_layer_profile_compatibility_flag[i][8] != 0 ||
    sub_layer_profile_idc[i] == 9 || sub_layer_profile_compatibility_flag[i][9] != 0 ||
    sub_layer_profile_idc[i] == 10 || sub_layer_profile_compatibility_flag[i][10] != 0
    )
                    {
                        /*  The number of bits in this syntax structure is not affected by this condition  */

                        size += stream.WriteUnsignedInt(1, this.sub_layer_max_12bit_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_max_10bit_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_max_8bit_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_max_422chroma_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_max_420chroma_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_max_monochrome_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_intra_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_one_picture_only_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_lower_bit_rate_constraint_flag[i]);

                        if (sub_layer_profile_idc[i] == 5 ||
     sub_layer_profile_compatibility_flag[i][5] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.sub_layer_max_14bit_constraint_flag[i]);
                            size += stream.WriteUnsignedInt(33, this.sub_layer_reserved_zero_33bits[i]);
                        }
                        else
                        {
                            size += stream.WriteUnsignedInt(34, this.sub_layer_reserved_zero_34bits[i]);
                        }
                    }
                    else if (sub_layer_profile_idc[i] == 2 ||
       sub_layer_profile_compatibility_flag[i][2] != 0)
                    {
                        size += stream.WriteUnsignedInt(7, this.sub_layer_reserved_zero_7bits[i]);
                        size += stream.WriteUnsignedInt(1, this.sub_layer_one_picture_only_constraint_flag[i]);
                        size += stream.WriteUnsignedInt(35, this.sub_layer_reserved_zero_35bits[i]);
                    }
                    else
                    {
                        size += stream.WriteUnsignedInt(43, this.sub_layer_reserved_zero_43bits[i]);
                    }

                    if ((sub_layer_profile_idc[i] >= 1 && sub_layer_profile_idc[i] <= 5) ||
     sub_layer_profile_idc[i] == 9 ||
     sub_layer_profile_compatibility_flag[i][1] != 0 ||
     sub_layer_profile_compatibility_flag[i][2] != 0 ||
     sub_layer_profile_compatibility_flag[i][3] != 0 ||
     sub_layer_profile_compatibility_flag[i][4] != 0 ||
     sub_layer_profile_compatibility_flag[i][5] != 0 ||
     sub_layer_profile_compatibility_flag[i][9] != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sub_layer_inbld_flag[i]);
                    }
                    else
                    {
                        size += stream.WriteUnsignedInt(1, this.sub_layer_reserved_zero_bit[i]);
                    }
                }

                if (sub_layer_level_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(8, this.sub_layer_level_idc[i]);
                }
            }

            return size;
        }

    }

    /*
  

scaling_list_data() { 
 for( sizeId = 0; sizeId < 4; sizeId++ )  
  for( matrixId = 0; matrixId < 6; matrixId  +=  ( sizeId == 3 ) ? 3 : 1 ) {  
   scaling_list_pred_mode_flag[ sizeId ][ matrixId ] u(1) 
   if( !scaling_list_pred_mode_flag[ sizeId ][ matrixId ] )  
    scaling_list_pred_matrix_id_delta[ sizeId ][ matrixId ] ue(v) 
   else {  
    nextCoef = 8  
    coefNum = Min( 64, ( 1  <<  ( 4 + ( sizeId  <<  1 ) ) ) )  
    if( sizeId > 1 ) {  
     scaling_list_dc_coef_minus8[ sizeId - 2 ][ matrixId ] se(v) 
     nextCoef = scaling_list_dc_coef_minus8[ sizeId - 2 ][ matrixId ] + 8  
    }  
    for( i = 0; i < coefNum; i++ ) {  
     scaling_list_delta_coef se(v) 
     nextCoef = ( nextCoef + scaling_list_delta_coef + 256 ) % 256  
     ScalingList[ sizeId ][ matrixId ][ i ] = nextCoef  
    }  
   }  
  }  
}
    */
    public class ScalingListData : IItuSerializable
    {
        private byte[][] scaling_list_pred_mode_flag;
        public byte[][] ScalingListPredModeFlag { get { return scaling_list_pred_mode_flag; } set { scaling_list_pred_mode_flag = value; } }
        private uint[][] scaling_list_pred_matrix_id_delta;
        public uint[][] ScalingListPredMatrixIdDelta { get { return scaling_list_pred_matrix_id_delta; } set { scaling_list_pred_matrix_id_delta = value; } }
        private int[][] scaling_list_dc_coef_minus8;
        public int[][] ScalingListDcCoefMinus8 { get { return scaling_list_dc_coef_minus8; } set { scaling_list_dc_coef_minus8 = value; } }
        private int[][][] scaling_list_delta_coef;
        public int[][][] ScalingListDeltaCoef { get { return scaling_list_delta_coef; } set { scaling_list_delta_coef = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ScalingListData()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint sizeId = 0;
            int matrixId = 0;
            uint nextCoef = 0;
            uint coefNum = 0;
            uint i = 0;
            uint[][][] ScalingList = null;

            this.scaling_list_pred_mode_flag = new byte[4][];
            this.scaling_list_pred_matrix_id_delta = new uint[4][];
            this.scaling_list_dc_coef_minus8 = new int[4][];
            this.scaling_list_delta_coef = new int[4][][];
            for (sizeId = 0; sizeId < 4; sizeId++)
            {

                this.scaling_list_pred_mode_flag[sizeId] = new byte[6];
                this.scaling_list_pred_matrix_id_delta[sizeId] = new uint[6];
                this.scaling_list_dc_coef_minus8[sizeId] = new int[6];
                this.scaling_list_delta_coef[sizeId] = new int[6][];
                for (matrixId = 0; matrixId < 6; matrixId += (sizeId == 3) ? 3 : 1)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.scaling_list_pred_mode_flag[sizeId][matrixId]);

                    if (scaling_list_pred_mode_flag[sizeId][matrixId] == 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.scaling_list_pred_matrix_id_delta[sizeId][matrixId]);
                    }
                    else
                    {
                        nextCoef = 8;
                        coefNum = (uint)Math.Min(64, (1 << (int)(4 + (sizeId << (int)1))));

                        if (sizeId > 1)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.scaling_list_dc_coef_minus8[sizeId - 2][matrixId]);
                            nextCoef = (uint)scaling_list_dc_coef_minus8[sizeId - 2][matrixId] + 8;
                        }

                        this.scaling_list_delta_coef[sizeId][matrixId] = new int[coefNum];
                        for (i = 0; i < coefNum; i++)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.scaling_list_delta_coef[sizeId][matrixId][i]);
                            nextCoef = (uint)(nextCoef + scaling_list_delta_coef[sizeId][sizeId][i] + 256) % 256;
                            ScalingList[sizeId][matrixId][i] = nextCoef;
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint sizeId = 0;
            int matrixId = 0;
            uint nextCoef = 0;
            uint coefNum = 0;
            uint i = 0;
            uint[][][] ScalingList = null;

            for (sizeId = 0; sizeId < 4; sizeId++)
            {

                for (matrixId = 0; matrixId < 6; matrixId += (sizeId == 3) ? 3 : 1)
                {
                    size += stream.WriteUnsignedInt(1, this.scaling_list_pred_mode_flag[sizeId][matrixId]);

                    if (scaling_list_pred_mode_flag[sizeId][matrixId] == 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.scaling_list_pred_matrix_id_delta[sizeId][matrixId]);
                    }
                    else
                    {
                        nextCoef = 8;
                        coefNum = (uint)Math.Min(64, (1 << (int)(4 + (sizeId << (int)1))));

                        if (sizeId > 1)
                        {
                            size += stream.WriteSignedIntGolomb(this.scaling_list_dc_coef_minus8[sizeId - 2][matrixId]);
                            nextCoef = (uint)scaling_list_dc_coef_minus8[sizeId - 2][matrixId] + 8;
                        }

                        for (i = 0; i < coefNum; i++)
                        {
                            size += stream.WriteSignedIntGolomb(this.scaling_list_delta_coef[sizeId][matrixId][i]);
                            nextCoef = (uint)(nextCoef + scaling_list_delta_coef[sizeId][sizeId][i] + 256) % 256;
                            ScalingList[sizeId][matrixId][i] = nextCoef;
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
 

sei_message() {  
 payloadType = 0  
 while( next_bits( 8 ) == 0xFF ) {  
  ff_byte  /* equal to 0xFF *//* f(8) 
  payloadType  +=  255  
 }  
 last_payload_type_byte u(8) 
 payloadType  +=  last_payload_type_byte  
 payloadSize = 0  
 while( next_bits( 8 ) == 0xFF ) {  
  ff_byte  /* equal to 0xFF *//* f(8) 
  payloadSize  +=  255  
 }  
 last_payload_size_byte u(8) 
 payloadSize  +=  last_payload_size_byte  
 sei_payload( payloadType, payloadSize )  
}
    */
    public class SeiMessage : IItuSerializable
    {
        private Dictionary<int, uint> ff_byte = new Dictionary<int, uint>();
        public Dictionary<int, uint> FfByte { get { return ff_byte; } set { ff_byte = value; } }
        private uint last_payload_type_byte;
        public uint LastPayloadTypeByte { get { return last_payload_type_byte; } set { last_payload_type_byte = value; } }
        private uint last_payload_size_byte;
        public uint LastPayloadSizeByte { get { return last_payload_size_byte; } set { last_payload_size_byte = value; } }
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

            while (stream.ReadNextBits(this, 8) == 0xFF)
            {
                whileIndex++;

                size += stream.ReadFixed(size, 8, whileIndex, this.ff_byte); // equal to 0xFF 
                payloadType += 255;
            }
            size += stream.ReadUnsignedInt(size, 8, out this.last_payload_type_byte);
            payloadType += last_payload_type_byte;
            payloadSize = 0;

            while (stream.ReadNextBits(this, 8) == 0xFF)
            {
                whileIndex++;

                size += stream.ReadFixed(size, 8, whileIndex, this.ff_byte); // equal to 0xFF 
                payloadSize += 255;
            }
            size += stream.ReadUnsignedInt(size, 8, out this.last_payload_size_byte);
            payloadSize += last_payload_size_byte;
            this.sei_payload = new SeiPayload(payloadType, payloadSize);
            size += stream.ReadClass<SeiPayload>(size, context, this.sei_payload);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint payloadType = 0;
            int whileIndex = -1;
            uint payloadSize = 0;
            payloadType = 0;

            while (stream.WriteNextBits(this, 8) == 0xFF)
            {
                whileIndex++;

                size += stream.WriteFixed(8, whileIndex, this.ff_byte); // equal to 0xFF 
                payloadType += 255;
            }
            size += stream.WriteUnsignedInt(8, this.last_payload_type_byte);
            payloadType += last_payload_type_byte;
            payloadSize = 0;

            while (stream.WriteNextBits(this, 8) == 0xFF)
            {
                whileIndex++;

                size += stream.WriteFixed(8, whileIndex, this.ff_byte); // equal to 0xFF 
                payloadSize += 255;
            }
            size += stream.WriteUnsignedInt(8, this.last_payload_size_byte);
            payloadSize += last_payload_size_byte;
            size += stream.WriteClass<SeiPayload>(context, this.sei_payload);

            return size;
        }

    }

    /*
  

ref_pic_lists_modification() { 
ref_pic_list_modification_flag_l0  u(1)
if( ref_pic_list_modification_flag_l0 )  
for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ )  
list_entry_l0[ i ] u(v) 
if( slice_type == B ) {   
ref_pic_list_modification_flag_l1 u(1)
if( ref_pic_list_modification_flag_l1 )  
for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ )  
list_entry_l1[ i ] u(v) 
}  
}
    */
    public class RefPicListsModification : IItuSerializable
    {
        private byte ref_pic_list_modification_flag_l0;
        public byte RefPicListModificationFlagL0 { get { return ref_pic_list_modification_flag_l0; } set { ref_pic_list_modification_flag_l0 = value; } }
        private uint[] list_entry_l0;
        public uint[] ListEntryL0 { get { return list_entry_l0; } set { list_entry_l0 = value; } }
        private byte ref_pic_list_modification_flag_l1;
        public byte RefPicListModificationFlagL1 { get { return ref_pic_list_modification_flag_l1; } set { ref_pic_list_modification_flag_l1 = value; } }
        private uint[] list_entry_l1;
        public uint[] ListEntryL1 { get { return list_entry_l1; } set { list_entry_l1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RefPicListsModification()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.ref_pic_list_modification_flag_l0);

            if (ref_pic_list_modification_flag_l0 != 0)
            {

                this.list_entry_l0 = new uint[num_ref_idx_l0_active_minus1 + 1];
                for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, list_entry_l0, out this.list_entry_l0[i]);
                }
            }

            if (H265FrameTypes.IsB(slice_type))
            {
                size += stream.ReadUnsignedInt(size, 1, out this.ref_pic_list_modification_flag_l1);

                if (ref_pic_list_modification_flag_l1 != 0)
                {

                    this.list_entry_l1 = new uint[num_ref_idx_l1_active_minus1 + 1];
                    for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, list_entry_l1, out this.list_entry_l1[i]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.ref_pic_list_modification_flag_l0);

            if (ref_pic_list_modification_flag_l0 != 0)
            {

                for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
                {
                    size += stream.WriteUnsignedIntVariable(list_entry_l0[i], this.list_entry_l0[i]);
                }
            }

            if (H265FrameTypes.IsB(slice_type))
            {
                size += stream.WriteUnsignedInt(1, this.ref_pic_list_modification_flag_l1);

                if (ref_pic_list_modification_flag_l1 != 0)
                {

                    for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
                    {
                        size += stream.WriteUnsignedIntVariable(list_entry_l1[i], this.list_entry_l1[i]);
                    }
                }
            }

            return size;
        }

    }

    /*
 

pred_weight_table() {  
 luma_log2_weight_denom ue(v) 
 if( ChromaArrayType != 0 )  
  delta_chroma_log2_weight_denom se(v) 
 for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ )  
  if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
   ( PicOrderCnt( RefPicList0[ i ] ) != PicOrderCnt( CurrPic ) ) ) 
 
   luma_weight_l0_flag[ i ] u(1) 
 if( ChromaArrayType != 0 )  
  for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ )  
   if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
    ( PicOrderCnt(RefPicList0[ i ]) != PicOrderCnt( CurrPic ) ) ) 
 
    chroma_weight_l0_flag[ i ] u(1) 
 for( i = 0; i <= num_ref_idx_l0_active_minus1; i++ ) {  
  if( luma_weight_l0_flag[ i ] ) {  
   delta_luma_weight_l0[ i ] se(v) 
   luma_offset_l0[ i ] se(v) 
  }  
  if( chroma_weight_l0_flag[ i ] )  
   for( j = 0; j < 2; j++ ) {  
    delta_chroma_weight_l0[ i ][ j ] se(v) 
    delta_chroma_offset_l0[ i ][ j ] se(v) 
   }  
 }  
 if( slice_type == B ) {  
  for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ )  
   if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
    ( PicOrderCnt(RefPicList1[ i ]) != PicOrderCnt( CurrPic ) ) ) 
 
    luma_weight_l1_flag[ i ] u(1) 
  if( ChromaArrayType != 0 )  
   for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ )  
    if( ( pic_layer_id( RefPicList0[ i ] ) != nuh_layer_id )  || 
     ( PicOrderCnt(RefPicList1[ i ]) != PicOrderCnt( CurrPic ) ) ) 
 
     chroma_weight_l1_flag[ i ] u(1) 
  for( i = 0; i <= num_ref_idx_l1_active_minus1; i++ ) {  
   if( luma_weight_l1_flag[ i ] ) {  
    delta_luma_weight_l1[ i ] se(v) 
    luma_offset_l1[ i ] se(v) 
   }  
   if( chroma_weight_l1_flag[ i ] )  
    for( j = 0; j < 2; j++ ) {  
     delta_chroma_weight_l1[ i ][ j ] se(v) 
     delta_chroma_offset_l1[ i ][ j ] se(v) 
    }  
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

            if (ChromaArrayType != 0)
            {
                size += stream.ReadSignedIntGolomb(size, out this.delta_chroma_log2_weight_denom);
            }

            this.luma_weight_l0_flag = new byte[num_ref_idx_l0_active_minus1 + 1];
            for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
            {

                if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
   (PicOrderCnt(RefPicList0[i]) != PicOrderCnt(CurrPic)))
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.luma_weight_l0_flag[i]);
                }
            }

            if (ChromaArrayType != 0)
            {

                this.chroma_weight_l0_flag = new byte[num_ref_idx_l0_active_minus1 + 1];
                for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
                {

                    if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
    (PicOrderCnt(RefPicList0[i]) != PicOrderCnt(CurrPic)))
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.chroma_weight_l0_flag[i]);
                    }
                }
            }

            this.delta_luma_weight_l0 = new int[num_ref_idx_l0_active_minus1 + 1];
            this.luma_offset_l0 = new int[num_ref_idx_l0_active_minus1 + 1];
            this.delta_chroma_weight_l0 = new int[num_ref_idx_l0_active_minus1 + 1][];
            this.delta_chroma_offset_l0 = new int[num_ref_idx_l0_active_minus1 + 1][];
            for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
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

            if (H265FrameTypes.IsB(slice_type))
            {

                this.luma_weight_l1_flag = new byte[num_ref_idx_l1_active_minus1 + 1];
                for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
                {

                    if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
    (PicOrderCnt(RefPicList1[i]) != PicOrderCnt(CurrPic)))
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.luma_weight_l1_flag[i]);
                    }
                }

                if (ChromaArrayType != 0)
                {

                    this.chroma_weight_l1_flag = new byte[num_ref_idx_l1_active_minus1 + 1];
                    for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
                    {

                        if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
     (PicOrderCnt(RefPicList1[i]) != PicOrderCnt(CurrPic)))
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.chroma_weight_l1_flag[i]);
                        }
                    }
                }

                this.delta_luma_weight_l1 = new int[num_ref_idx_l1_active_minus1 + 1];
                this.luma_offset_l1 = new int[num_ref_idx_l1_active_minus1 + 1];
                this.delta_chroma_weight_l1 = new int[num_ref_idx_l1_active_minus1 + 1][];
                this.delta_chroma_offset_l1 = new int[num_ref_idx_l1_active_minus1 + 1][];
                for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
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
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.luma_log2_weight_denom);

            if (ChromaArrayType != 0)
            {
                size += stream.WriteSignedIntGolomb(this.delta_chroma_log2_weight_denom);
            }

            for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
            {

                if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
   (PicOrderCnt(RefPicList0[i]) != PicOrderCnt(CurrPic)))
                {
                    size += stream.WriteUnsignedInt(1, this.luma_weight_l0_flag[i]);
                }
            }

            if (ChromaArrayType != 0)
            {

                for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
                {

                    if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
    (PicOrderCnt(RefPicList0[i]) != PicOrderCnt(CurrPic)))
                    {
                        size += stream.WriteUnsignedInt(1, this.chroma_weight_l0_flag[i]);
                    }
                }
            }

            for (i = 0; i <= num_ref_idx_l0_active_minus1; i++)
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

            if (H265FrameTypes.IsB(slice_type))
            {

                for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
                {

                    if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
    (PicOrderCnt(RefPicList1[i]) != PicOrderCnt(CurrPic)))
                    {
                        size += stream.WriteUnsignedInt(1, this.luma_weight_l1_flag[i]);
                    }
                }

                if (ChromaArrayType != 0)
                {

                    for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
                    {

                        if ((pic_layer_id(RefPicList0[i]) != nuh_layer_id) ||
     (PicOrderCnt(RefPicList1[i]) != PicOrderCnt(CurrPic)))
                        {
                            size += stream.WriteUnsignedInt(1, this.chroma_weight_l1_flag[i]);
                        }
                    }
                }

                for (i = 0; i <= num_ref_idx_l1_active_minus1; i++)
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
            }

            return size;
        }

    }

    /*
  

st_ref_pic_set( stRpsIdx ) {  
 if( stRpsIdx != 0 )  
  inter_ref_pic_set_prediction_flag u(1) 
 if( inter_ref_pic_set_prediction_flag ) {  
  if( stRpsIdx == num_short_term_ref_pic_sets )  
   delta_idx_minus1 ue(v) 
  delta_rps_sign u(1) 
  abs_delta_rps_minus1 ue(v) 
  for( j = 0; j <= NumDeltaPocs[ RefRpsIdx ]; j++ ) {  
   used_by_curr_pic_flag[ j ] u(1) 
   if( !used_by_curr_pic_flag[ j ] )  
    use_delta_flag[ j ] u(1) 
  }  
 } else {  
  num_negative_pics ue(v) 
  num_positive_pics ue(v) 
  for( i = 0; i < num_negative_pics; i++ ) {  
   delta_poc_s0_minus1[ i ] ue(v) 
   used_by_curr_pic_s0_flag[ i ] u(1) 
  }  
  for( i = 0; i < num_positive_pics; i++ ) {  
   delta_poc_s1_minus1[ i ] ue(v) 
   used_by_curr_pic_s1_flag[ i ] u(1) 
  }  
 }  
}
    */
    public class StRefPicSet : IItuSerializable
    {
        private uint stRpsIdx;
        public uint StRpsIdx { get { return stRpsIdx; } set { stRpsIdx = value; } }
        private byte inter_ref_pic_set_prediction_flag;
        public byte InterRefPicSetPredictionFlag { get { return inter_ref_pic_set_prediction_flag; } set { inter_ref_pic_set_prediction_flag = value; } }
        private uint delta_idx_minus1;
        public uint DeltaIdxMinus1 { get { return delta_idx_minus1; } set { delta_idx_minus1 = value; } }
        private byte delta_rps_sign;
        public byte DeltaRpsSign { get { return delta_rps_sign; } set { delta_rps_sign = value; } }
        private uint abs_delta_rps_minus1;
        public uint AbsDeltaRpsMinus1 { get { return abs_delta_rps_minus1; } set { abs_delta_rps_minus1 = value; } }
        private byte[] used_by_curr_pic_flag;
        public byte[] UsedByCurrPicFlag { get { return used_by_curr_pic_flag; } set { used_by_curr_pic_flag = value; } }
        private byte[] use_delta_flag;
        public byte[] UseDeltaFlag { get { return use_delta_flag; } set { use_delta_flag = value; } }
        private uint num_negative_pics;
        public uint NumNegativePics { get { return num_negative_pics; } set { num_negative_pics = value; } }
        private uint num_positive_pics;
        public uint NumPositivePics { get { return num_positive_pics; } set { num_positive_pics = value; } }
        private uint[] delta_poc_s0_minus1;
        public uint[] DeltaPocS0Minus1 { get { return delta_poc_s0_minus1; } set { delta_poc_s0_minus1 = value; } }
        private byte[] used_by_curr_pic_s0_flag;
        public byte[] UsedByCurrPicS0Flag { get { return used_by_curr_pic_s0_flag; } set { used_by_curr_pic_s0_flag = value; } }
        private uint[] delta_poc_s1_minus1;
        public uint[] DeltaPocS1Minus1 { get { return delta_poc_s1_minus1; } set { delta_poc_s1_minus1 = value; } }
        private byte[] used_by_curr_pic_s1_flag;
        public byte[] UsedByCurrPicS1Flag { get { return used_by_curr_pic_s1_flag; } set { used_by_curr_pic_s1_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public StRefPicSet(uint stRpsIdx)
        {
            this.stRpsIdx = stRpsIdx;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint j = 0;
            uint i = 0;

            if (stRpsIdx != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.inter_ref_pic_set_prediction_flag);
            }

            if (inter_ref_pic_set_prediction_flag != 0)
            {

                if (stRpsIdx == num_short_term_ref_pic_sets)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.delta_idx_minus1);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.delta_rps_sign);
                size += stream.ReadUnsignedIntGolomb(size, out this.abs_delta_rps_minus1);

                this.used_by_curr_pic_flag = new byte[NumDeltaPocs[RefRpsIdx]];
                this.use_delta_flag = new byte[NumDeltaPocs[RefRpsIdx]];
                for (j = 0; j <= NumDeltaPocs[RefRpsIdx]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.used_by_curr_pic_flag[j]);

                    if (used_by_curr_pic_flag[j] == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.use_delta_flag[j]);
                    }
                }
            }
            else
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_negative_pics);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_positive_pics);

                this.delta_poc_s0_minus1 = new uint[num_negative_pics];
                this.used_by_curr_pic_s0_flag = new byte[num_negative_pics];
                for (i = 0; i < num_negative_pics; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.delta_poc_s0_minus1[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.used_by_curr_pic_s0_flag[i]);
                }

                this.delta_poc_s1_minus1 = new uint[num_positive_pics];
                this.used_by_curr_pic_s1_flag = new byte[num_positive_pics];
                for (i = 0; i < num_positive_pics; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.delta_poc_s1_minus1[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.used_by_curr_pic_s1_flag[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint j = 0;
            uint i = 0;

            if (stRpsIdx != 0)
            {
                size += stream.WriteUnsignedInt(1, this.inter_ref_pic_set_prediction_flag);
            }

            if (inter_ref_pic_set_prediction_flag != 0)
            {

                if (stRpsIdx == num_short_term_ref_pic_sets)
                {
                    size += stream.WriteUnsignedIntGolomb(this.delta_idx_minus1);
                }
                size += stream.WriteUnsignedInt(1, this.delta_rps_sign);
                size += stream.WriteUnsignedIntGolomb(this.abs_delta_rps_minus1);

                for (j = 0; j <= NumDeltaPocs[RefRpsIdx]; j++)
                {
                    size += stream.WriteUnsignedInt(1, this.used_by_curr_pic_flag[j]);

                    if (used_by_curr_pic_flag[j] == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.use_delta_flag[j]);
                    }
                }
            }
            else
            {
                size += stream.WriteUnsignedIntGolomb(this.num_negative_pics);
                size += stream.WriteUnsignedIntGolomb(this.num_positive_pics);

                for (i = 0; i < num_negative_pics; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.delta_poc_s0_minus1[i]);
                    size += stream.WriteUnsignedInt(1, this.used_by_curr_pic_s0_flag[i]);
                }

                for (i = 0; i < num_positive_pics; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.delta_poc_s1_minus1[i]);
                    size += stream.WriteUnsignedInt(1, this.used_by_curr_pic_s1_flag[i]);
                }
            }

            return size;
        }

    }

    /*
  

sei_payload( payloadType, payloadSize ) {  
 if( nal_unit_type == PREFIX_SEI_NUT )  
  if( payloadType == 0 )  
   buffering_period( payloadSize )  
  else if( payloadType == 1 )  
   pic_timing( payloadSize )  
  else if( payloadType == 2 )  
   pan_scan_rect( payloadSize )  
  else if( payloadType == 3 )  
   filler_payload( payloadSize )  
  else if( payloadType == 4 )  
   user_data_registered_itu_t_t35( payloadSize )  
  else if( payloadType == 5 )  
   user_data_unregistered( payloadSize )  
  else if( payloadType == 6 )  
   recovery_point( payloadSize )  
  else if( payloadType == 9 )  
   scene_info( payloadSize )  
  else if( payloadType == 15 )  
   picture_snapshot( payloadSize )  
  else if( payloadType == 16 )  
   progressive_refinement_segment_start( payloadSize )  
  else if( payloadType == 17 )  
   progressive_refinement_segment_end( payloadSize )  
  else if( payloadType == 19 )  
   film_grain_characteristics( payloadSize )  
  else if( payloadType == 22 )  
   post_filter_hint( payloadSize )  
  else if( payloadType == 23 )  
   tone_mapping_info( payloadSize )  
  else if( payloadType == 45 ) 
     frame_packing_arrangement( payloadSize )  
  else if( payloadType == 47 )  
   display_orientation( payloadSize )  
  else if( payloadType == 56 )  
   green_metadata( payloadsize ) /* specified in ISO/IEC 23001-11 *//*  
  else if( payloadType == 128 )  
   structure_of_pictures_info( payloadSize )  
  else if( payloadType == 129 )  
   active_parameter_sets( payloadSize )  
  else if( payloadType == 130 )  
   decoding_unit_info( payloadSize )  
  else if( payloadType == 131 )  
   temporal_sub_layer_zero_idx( payloadSize )  
  else if( payloadType == 133 )  
   scalable_nesting( payloadSize )  
  else if( payloadType == 134 )  
   region_refresh_info( payloadSize )  
  else if( payloadType == 135 )  
   no_display( payloadSize )  
  else if( payloadType == 136 )  
   time_code( payloadSize )  
  else if( payloadType == 137 )  
   mastering_display_colour_volume( payloadSize )  
  else if( payloadType == 138 )  
   segmented_rect_frame_packing_arrangement( payloadSize )  
  else if( payloadType == 139 )  
   temporal_motion_constrained_tile_sets( payloadSize )  
  else if( payloadType == 140 )  
   chroma_resampling_filter_hint( payloadSize )  
  else if( payloadType == 141 )  
   knee_function_info( payloadSize )  
  else if( payloadType == 142 )  
   colour_remapping_info( payloadSize )  
  else if( payloadType == 143 )  
   deinterlaced_field_identification( payloadSize )  
  else if( payloadType == 144 )  
   content_light_level_info( payloadSize )  
  else if( payloadType == 145 )  
   dependent_rap_indication( payloadSize )  
  else if( payloadType == 146 )  
   coded_region_completion( payloadSize )  
  else if( payloadType == 147 )  
   alternative_transfer_characteristics( payloadSize )  
  else if( payloadType == 148 )  
   ambient_viewing_environment( payloadSize )  
  else if( payloadType == 149 )  
   content_colour_volume( payloadSize )  
  else if( payloadType == 150 )  
   equirectangular_projection( payloadSize )  
  else if( payloadType == 151 )  
   cubemap_projection( payloadSize )  
  else if( payloadType == 154 )  
   sphere_rotation( payloadSize ) 
     else if( payloadType == 155 )  
   regionwise_packing( payloadSize )  
  else if( payloadType == 156 )  
   omni_viewport( payloadSize )  
  else if( payloadType == 157 )  
   regional_nesting( payloadSize )  
  else if( payloadType == 158 )  
   mcts_extraction_info_sets( payloadSize )  
  else if( payloadType == 159 )  
   mcts_extraction_info_nesting( payloadSize )  
  else if( payloadType == 160 )  
   layers_not_present( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 161 )  
   inter_layer_constrained_tile_sets( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 162 )  
   bsp_nesting( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 163 )  
   bsp_initial_arrival_time( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 164 )  
   sub_bitstream_property( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 165 )  
   alpha_channel_info( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 166 )  
   overlay_info( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 167 )  
   temporal_mv_prediction_constraints( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 168 )  
   frame_field_info( payloadSize )  /* specified in Annex F *//*  
  else if( payloadType == 176 )  
   three_dimensional_reference_displays_info( payloadSize )  /* specified in Annex G *//*  
  else if( payloadType == 177 )  
   depth_representation_info( payloadSize )  /* specified in Annex G *//*  
  else if( payloadType == 178 )  
   multiview_scene_info( payloadSize )  /* specified in Annex G *//*  
  else if( payloadType == 179 )  
   multiview_acquisition_info( payloadSize )  /* specified in Annex G *//*  
  else if( payloadType == 180 )  
   multiview_view_position( payloadSize )  /* specified in Annex G *//*  
  else if( payloadType == 181 )  
   alternative_depth_info( payloadSize )  /* specified in Annex I *//*  
  else  
   reserved_sei_message( payloadSize )  
 else /* nal_unit_type == SUFFIX_SEI_NUT *//*  
  if( payloadType == 3 )  
   filler_payload( payloadSize )  
  else if( payloadType == 4 )  
   user_data_registered_itu_t_t35( payloadSize )  
  else if( payloadType == 5 )  
   user_data_unregistered( payloadSize )  
  else if( payloadType == 17 )  
   progressive_refinement_segment_end( payloadSize )  
  else if( payloadType == 22 )  
   post_filter_hint( payloadSize ) 
     else if( payloadType == 132 )  
   decoded_picture_hash( payloadSize )  
  else if( payloadType == 146 )  
   coded_region_completion( payloadSize )  
  else  
   reserved_sei_message( payloadSize )  
 if( more_data_in_payload() ) {  
  if( payload_extension_present() )  
   reserved_payload_extension_data u(v) 
  payload_bit_equal_to_one /* equal to 1 *//* f(1) 
  while( !byte_aligned() )  
   payload_bit_equal_to_zero /* equal to 0 *//* f(1) 
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
        private PanScanRect pan_scan_rect;
        public PanScanRect PanScanRect { get { return pan_scan_rect; } set { pan_scan_rect = value; } }
        private FillerPayload filler_payload;
        public FillerPayload FillerPayload { get { return filler_payload; } set { filler_payload = value; } }
        private UserDataRegisteredItutT35 user_data_registered_itu_t_t35;
        public UserDataRegisteredItutT35 UserDataRegisteredItutT35 { get { return user_data_registered_itu_t_t35; } set { user_data_registered_itu_t_t35 = value; } }
        private UserDataUnregistered user_data_unregistered;
        public UserDataUnregistered UserDataUnregistered { get { return user_data_unregistered; } set { user_data_unregistered = value; } }
        private RecoveryPoint recovery_point;
        public RecoveryPoint RecoveryPoint { get { return recovery_point; } set { recovery_point = value; } }
        private SceneInfo scene_info;
        public SceneInfo SceneInfo { get { return scene_info; } set { scene_info = value; } }
        private PictureSnapshot picture_snapshot;
        public PictureSnapshot PictureSnapshot { get { return picture_snapshot; } set { picture_snapshot = value; } }
        private ProgressiveRefinementSegmentStart progressive_refinement_segment_start;
        public ProgressiveRefinementSegmentStart ProgressiveRefinementSegmentStart { get { return progressive_refinement_segment_start; } set { progressive_refinement_segment_start = value; } }
        private ProgressiveRefinementSegmentEnd progressive_refinement_segment_end;
        public ProgressiveRefinementSegmentEnd ProgressiveRefinementSegmentEnd { get { return progressive_refinement_segment_end; } set { progressive_refinement_segment_end = value; } }
        private FilmGrainCharacteristics film_grain_characteristics;
        public FilmGrainCharacteristics FilmGrainCharacteristics { get { return film_grain_characteristics; } set { film_grain_characteristics = value; } }
        private PostFilterHint post_filter_hint;
        public PostFilterHint PostFilterHint { get { return post_filter_hint; } set { post_filter_hint = value; } }
        private ToneMappingInfo tone_mapping_info;
        public ToneMappingInfo ToneMappingInfo { get { return tone_mapping_info; } set { tone_mapping_info = value; } }
        private FramePackingArrangement frame_packing_arrangement;
        public FramePackingArrangement FramePackingArrangement { get { return frame_packing_arrangement; } set { frame_packing_arrangement = value; } }
        private DisplayOrientation display_orientation;
        public DisplayOrientation DisplayOrientation { get { return display_orientation; } set { display_orientation = value; } }
        private GreenMetadata green_metadata;
        public GreenMetadata GreenMetadata { get { return green_metadata; } set { green_metadata = value; } }
        private StructureOfPicturesInfo structure_of_pictures_info;
        public StructureOfPicturesInfo StructureOfPicturesInfo { get { return structure_of_pictures_info; } set { structure_of_pictures_info = value; } }
        private ActiveParameterSets active_parameter_sets;
        public ActiveParameterSets ActiveParameterSets { get { return active_parameter_sets; } set { active_parameter_sets = value; } }
        private DecodingUnitInfo decoding_unit_info;
        public DecodingUnitInfo DecodingUnitInfo { get { return decoding_unit_info; } set { decoding_unit_info = value; } }
        private TemporalSubLayerZeroIdx temporal_sub_layer_zero_idx;
        public TemporalSubLayerZeroIdx TemporalSubLayerZeroIdx { get { return temporal_sub_layer_zero_idx; } set { temporal_sub_layer_zero_idx = value; } }
        private ScalableNesting scalable_nesting;
        public ScalableNesting ScalableNesting { get { return scalable_nesting; } set { scalable_nesting = value; } }
        private RegionRefreshInfo region_refresh_info;
        public RegionRefreshInfo RegionRefreshInfo { get { return region_refresh_info; } set { region_refresh_info = value; } }
        private NoDisplay no_display;
        public NoDisplay NoDisplay { get { return no_display; } set { no_display = value; } }
        private TimeCode time_code;
        public TimeCode TimeCode { get { return time_code; } set { time_code = value; } }
        private MasteringDisplayColourVolume mastering_display_colour_volume;
        public MasteringDisplayColourVolume MasteringDisplayColourVolume { get { return mastering_display_colour_volume; } set { mastering_display_colour_volume = value; } }
        private SegmentedRectFramePackingArrangement segmented_rect_frame_packing_arrangement;
        public SegmentedRectFramePackingArrangement SegmentedRectFramePackingArrangement { get { return segmented_rect_frame_packing_arrangement; } set { segmented_rect_frame_packing_arrangement = value; } }
        private TemporalMotionConstrainedTileSets temporal_motion_constrained_tile_sets;
        public TemporalMotionConstrainedTileSets TemporalMotionConstrainedTileSets { get { return temporal_motion_constrained_tile_sets; } set { temporal_motion_constrained_tile_sets = value; } }
        private ChromaResamplingFilterHint chroma_resampling_filter_hint;
        public ChromaResamplingFilterHint ChromaResamplingFilterHint { get { return chroma_resampling_filter_hint; } set { chroma_resampling_filter_hint = value; } }
        private KneeFunctionInfo knee_function_info;
        public KneeFunctionInfo KneeFunctionInfo { get { return knee_function_info; } set { knee_function_info = value; } }
        private ColourRemappingInfo colour_remapping_info;
        public ColourRemappingInfo ColourRemappingInfo { get { return colour_remapping_info; } set { colour_remapping_info = value; } }
        private DeinterlacedFieldIdentification deinterlaced_field_identification;
        public DeinterlacedFieldIdentification DeinterlacedFieldIdentification { get { return deinterlaced_field_identification; } set { deinterlaced_field_identification = value; } }
        private ContentLightLevelInfo content_light_level_info;
        public ContentLightLevelInfo ContentLightLevelInfo { get { return content_light_level_info; } set { content_light_level_info = value; } }
        private DependentRapIndication dependent_rap_indication;
        public DependentRapIndication DependentRapIndication { get { return dependent_rap_indication; } set { dependent_rap_indication = value; } }
        private CodedRegionCompletion coded_region_completion;
        public CodedRegionCompletion CodedRegionCompletion { get { return coded_region_completion; } set { coded_region_completion = value; } }
        private AlternativeTransferCharacteristics alternative_transfer_characteristics;
        public AlternativeTransferCharacteristics AlternativeTransferCharacteristics { get { return alternative_transfer_characteristics; } set { alternative_transfer_characteristics = value; } }
        private AmbientViewingEnvironment ambient_viewing_environment;
        public AmbientViewingEnvironment AmbientViewingEnvironment { get { return ambient_viewing_environment; } set { ambient_viewing_environment = value; } }
        private ContentColourVolume content_colour_volume;
        public ContentColourVolume ContentColourVolume { get { return content_colour_volume; } set { content_colour_volume = value; } }
        private EquirectangularProjection equirectangular_projection;
        public EquirectangularProjection EquirectangularProjection { get { return equirectangular_projection; } set { equirectangular_projection = value; } }
        private CubemapProjection cubemap_projection;
        public CubemapProjection CubemapProjection { get { return cubemap_projection; } set { cubemap_projection = value; } }
        private SphereRotation sphere_rotation;
        public SphereRotation SphereRotation { get { return sphere_rotation; } set { sphere_rotation = value; } }
        private RegionwisePacking regionwise_packing;
        public RegionwisePacking RegionwisePacking { get { return regionwise_packing; } set { regionwise_packing = value; } }
        private OmniViewport omni_viewport;
        public OmniViewport OmniViewport { get { return omni_viewport; } set { omni_viewport = value; } }
        private RegionalNesting regional_nesting;
        public RegionalNesting RegionalNesting { get { return regional_nesting; } set { regional_nesting = value; } }
        private MctsExtractionInfoSets mcts_extraction_info_sets;
        public MctsExtractionInfoSets MctsExtractionInfoSets { get { return mcts_extraction_info_sets; } set { mcts_extraction_info_sets = value; } }
        private MctsExtractionInfoNesting mcts_extraction_info_nesting;
        public MctsExtractionInfoNesting MctsExtractionInfoNesting { get { return mcts_extraction_info_nesting; } set { mcts_extraction_info_nesting = value; } }
        private LayersNotPresent layers_not_present;
        public LayersNotPresent LayersNotPresent { get { return layers_not_present; } set { layers_not_present = value; } }
        private InterLayerConstrainedTileSets inter_layer_constrained_tile_sets;
        public InterLayerConstrainedTileSets InterLayerConstrainedTileSets { get { return inter_layer_constrained_tile_sets; } set { inter_layer_constrained_tile_sets = value; } }
        private BspNesting bsp_nesting;
        public BspNesting BspNesting { get { return bsp_nesting; } set { bsp_nesting = value; } }
        private BspInitialArrivalTime bsp_initial_arrival_time;
        public BspInitialArrivalTime BspInitialArrivalTime { get { return bsp_initial_arrival_time; } set { bsp_initial_arrival_time = value; } }
        private SubBitstreamProperty sub_bitstream_property;
        public SubBitstreamProperty SubBitstreamProperty { get { return sub_bitstream_property; } set { sub_bitstream_property = value; } }
        private AlphaChannelInfo alpha_channel_info;
        public AlphaChannelInfo AlphaChannelInfo { get { return alpha_channel_info; } set { alpha_channel_info = value; } }
        private OverlayInfo overlay_info;
        public OverlayInfo OverlayInfo { get { return overlay_info; } set { overlay_info = value; } }
        private TemporalMvPredictionConstraints temporal_mv_prediction_constraints;
        public TemporalMvPredictionConstraints TemporalMvPredictionConstraints { get { return temporal_mv_prediction_constraints; } set { temporal_mv_prediction_constraints = value; } }
        private FrameFieldInfo frame_field_info;
        public FrameFieldInfo FrameFieldInfo { get { return frame_field_info; } set { frame_field_info = value; } }
        private ThreeDimensionalReferenceDisplaysInfo three_dimensional_reference_displays_info;
        public ThreeDimensionalReferenceDisplaysInfo ThreeDimensionalReferenceDisplaysInfo { get { return three_dimensional_reference_displays_info; } set { three_dimensional_reference_displays_info = value; } }
        private DepthRepresentationInfo depth_representation_info;
        public DepthRepresentationInfo DepthRepresentationInfo { get { return depth_representation_info; } set { depth_representation_info = value; } }
        private MultiviewSceneInfo multiview_scene_info;
        public MultiviewSceneInfo MultiviewSceneInfo { get { return multiview_scene_info; } set { multiview_scene_info = value; } }
        private MultiviewAcquisitionInfo multiview_acquisition_info;
        public MultiviewAcquisitionInfo MultiviewAcquisitionInfo { get { return multiview_acquisition_info; } set { multiview_acquisition_info = value; } }
        private MultiviewViewPosition multiview_view_position;
        public MultiviewViewPosition MultiviewViewPosition { get { return multiview_view_position; } set { multiview_view_position = value; } }
        private AlternativeDepthInfo alternative_depth_info;
        public AlternativeDepthInfo AlternativeDepthInfo { get { return alternative_depth_info; } set { alternative_depth_info = value; } }
        private ReservedSeiMessage reserved_sei_message;
        public ReservedSeiMessage ReservedSeiMessage { get { return reserved_sei_message; } set { reserved_sei_message = value; } }
        private DecodedPictureHash decoded_picture_hash;
        public DecodedPictureHash DecodedPictureHash { get { return decoded_picture_hash; } set { decoded_picture_hash = value; } }
        private uint reserved_payload_extension_data;
        public uint ReservedPayloadExtensionData { get { return reserved_payload_extension_data; } set { reserved_payload_extension_data = value; } }
        private uint payload_bit_equal_to_one;
        public uint PayloadBitEqualToOne { get { return payload_bit_equal_to_one; } set { payload_bit_equal_to_one = value; } }
        private Dictionary<int, uint> payload_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> PayloadBitEqualToZero { get { return payload_bit_equal_to_zero; } set { payload_bit_equal_to_zero = value; } }

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

            if (nal_unit_type == H265NALTypes.PREFIX_SEI_NUT)
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
                else if (payloadType == 2)
                {
                    this.pan_scan_rect = new PanScanRect(payloadSize);
                    size += stream.ReadClass<PanScanRect>(size, context, this.pan_scan_rect);
                }
                else if (payloadType == 3)
                {
                    this.filler_payload = new FillerPayload(payloadSize);
                    size += stream.ReadClass<FillerPayload>(size, context, this.filler_payload);
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
                else if (payloadType == 6)
                {
                    this.recovery_point = new RecoveryPoint(payloadSize);
                    size += stream.ReadClass<RecoveryPoint>(size, context, this.recovery_point);
                }
                else if (payloadType == 9)
                {
                    this.scene_info = new SceneInfo(payloadSize);
                    size += stream.ReadClass<SceneInfo>(size, context, this.scene_info);
                }
                else if (payloadType == 15)
                {
                    this.picture_snapshot = new PictureSnapshot(payloadSize);
                    size += stream.ReadClass<PictureSnapshot>(size, context, this.picture_snapshot);
                }
                else if (payloadType == 16)
                {
                    this.progressive_refinement_segment_start = new ProgressiveRefinementSegmentStart(payloadSize);
                    size += stream.ReadClass<ProgressiveRefinementSegmentStart>(size, context, this.progressive_refinement_segment_start);
                }
                else if (payloadType == 17)
                {
                    this.progressive_refinement_segment_end = new ProgressiveRefinementSegmentEnd(payloadSize);
                    size += stream.ReadClass<ProgressiveRefinementSegmentEnd>(size, context, this.progressive_refinement_segment_end);
                }
                else if (payloadType == 19)
                {
                    this.film_grain_characteristics = new FilmGrainCharacteristics(payloadSize);
                    size += stream.ReadClass<FilmGrainCharacteristics>(size, context, this.film_grain_characteristics);
                }
                else if (payloadType == 22)
                {
                    this.post_filter_hint = new PostFilterHint(payloadSize);
                    size += stream.ReadClass<PostFilterHint>(size, context, this.post_filter_hint);
                }
                else if (payloadType == 23)
                {
                    this.tone_mapping_info = new ToneMappingInfo(payloadSize);
                    size += stream.ReadClass<ToneMappingInfo>(size, context, this.tone_mapping_info);
                }
                else if (payloadType == 45)
                {
                    this.frame_packing_arrangement = new FramePackingArrangement(payloadSize);
                    size += stream.ReadClass<FramePackingArrangement>(size, context, this.frame_packing_arrangement);
                }
                else if (payloadType == 47)
                {
                    this.display_orientation = new DisplayOrientation(payloadSize);
                    size += stream.ReadClass<DisplayOrientation>(size, context, this.display_orientation);
                }
                else if (payloadType == 56)
                {
                    this.green_metadata = new GreenMetadata(payloadsize);
                    size += stream.ReadClass<GreenMetadata>(size, context, this.green_metadata); // specified in ISO/IEC 23001-11 
                }
                else if (payloadType == 128)
                {
                    this.structure_of_pictures_info = new StructureOfPicturesInfo(payloadSize);
                    size += stream.ReadClass<StructureOfPicturesInfo>(size, context, this.structure_of_pictures_info);
                }
                else if (payloadType == 129)
                {
                    this.active_parameter_sets = new ActiveParameterSets(payloadSize);
                    size += stream.ReadClass<ActiveParameterSets>(size, context, this.active_parameter_sets);
                }
                else if (payloadType == 130)
                {
                    this.decoding_unit_info = new DecodingUnitInfo(payloadSize);
                    size += stream.ReadClass<DecodingUnitInfo>(size, context, this.decoding_unit_info);
                }
                else if (payloadType == 131)
                {
                    this.temporal_sub_layer_zero_idx = new TemporalSubLayerZeroIdx(payloadSize);
                    size += stream.ReadClass<TemporalSubLayerZeroIdx>(size, context, this.temporal_sub_layer_zero_idx);
                }
                else if (payloadType == 133)
                {
                    this.scalable_nesting = new ScalableNesting(payloadSize);
                    size += stream.ReadClass<ScalableNesting>(size, context, this.scalable_nesting);
                }
                else if (payloadType == 134)
                {
                    this.region_refresh_info = new RegionRefreshInfo(payloadSize);
                    size += stream.ReadClass<RegionRefreshInfo>(size, context, this.region_refresh_info);
                }
                else if (payloadType == 135)
                {
                    this.no_display = new NoDisplay(payloadSize);
                    size += stream.ReadClass<NoDisplay>(size, context, this.no_display);
                }
                else if (payloadType == 136)
                {
                    this.time_code = new TimeCode(payloadSize);
                    size += stream.ReadClass<TimeCode>(size, context, this.time_code);
                }
                else if (payloadType == 137)
                {
                    this.mastering_display_colour_volume = new MasteringDisplayColourVolume(payloadSize);
                    size += stream.ReadClass<MasteringDisplayColourVolume>(size, context, this.mastering_display_colour_volume);
                }
                else if (payloadType == 138)
                {
                    this.segmented_rect_frame_packing_arrangement = new SegmentedRectFramePackingArrangement(payloadSize);
                    size += stream.ReadClass<SegmentedRectFramePackingArrangement>(size, context, this.segmented_rect_frame_packing_arrangement);
                }
                else if (payloadType == 139)
                {
                    this.temporal_motion_constrained_tile_sets = new TemporalMotionConstrainedTileSets(payloadSize);
                    size += stream.ReadClass<TemporalMotionConstrainedTileSets>(size, context, this.temporal_motion_constrained_tile_sets);
                }
                else if (payloadType == 140)
                {
                    this.chroma_resampling_filter_hint = new ChromaResamplingFilterHint(payloadSize);
                    size += stream.ReadClass<ChromaResamplingFilterHint>(size, context, this.chroma_resampling_filter_hint);
                }
                else if (payloadType == 141)
                {
                    this.knee_function_info = new KneeFunctionInfo(payloadSize);
                    size += stream.ReadClass<KneeFunctionInfo>(size, context, this.knee_function_info);
                }
                else if (payloadType == 142)
                {
                    this.colour_remapping_info = new ColourRemappingInfo(payloadSize);
                    size += stream.ReadClass<ColourRemappingInfo>(size, context, this.colour_remapping_info);
                }
                else if (payloadType == 143)
                {
                    this.deinterlaced_field_identification = new DeinterlacedFieldIdentification(payloadSize);
                    size += stream.ReadClass<DeinterlacedFieldIdentification>(size, context, this.deinterlaced_field_identification);
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
                else if (payloadType == 146)
                {
                    this.coded_region_completion = new CodedRegionCompletion(payloadSize);
                    size += stream.ReadClass<CodedRegionCompletion>(size, context, this.coded_region_completion);
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
                else if (payloadType == 151)
                {
                    this.cubemap_projection = new CubemapProjection(payloadSize);
                    size += stream.ReadClass<CubemapProjection>(size, context, this.cubemap_projection);
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
                else if (payloadType == 157)
                {
                    this.regional_nesting = new RegionalNesting(payloadSize);
                    size += stream.ReadClass<RegionalNesting>(size, context, this.regional_nesting);
                }
                else if (payloadType == 158)
                {
                    this.mcts_extraction_info_sets = new MctsExtractionInfoSets(payloadSize);
                    size += stream.ReadClass<MctsExtractionInfoSets>(size, context, this.mcts_extraction_info_sets);
                }
                else if (payloadType == 159)
                {
                    this.mcts_extraction_info_nesting = new MctsExtractionInfoNesting(payloadSize);
                    size += stream.ReadClass<MctsExtractionInfoNesting>(size, context, this.mcts_extraction_info_nesting);
                }
                else if (payloadType == 160)
                {
                    this.layers_not_present = new LayersNotPresent(payloadSize);
                    size += stream.ReadClass<LayersNotPresent>(size, context, this.layers_not_present); // specified in Annex F 
                }
                else if (payloadType == 161)
                {
                    this.inter_layer_constrained_tile_sets = new InterLayerConstrainedTileSets(payloadSize);
                    size += stream.ReadClass<InterLayerConstrainedTileSets>(size, context, this.inter_layer_constrained_tile_sets); // specified in Annex F 
                }
                else if (payloadType == 162)
                {
                    this.bsp_nesting = new BspNesting(payloadSize);
                    size += stream.ReadClass<BspNesting>(size, context, this.bsp_nesting); // specified in Annex F 
                }
                else if (payloadType == 163)
                {
                    this.bsp_initial_arrival_time = new BspInitialArrivalTime(payloadSize);
                    size += stream.ReadClass<BspInitialArrivalTime>(size, context, this.bsp_initial_arrival_time); // specified in Annex F 
                }
                else if (payloadType == 164)
                {
                    this.sub_bitstream_property = new SubBitstreamProperty(payloadSize);
                    size += stream.ReadClass<SubBitstreamProperty>(size, context, this.sub_bitstream_property); // specified in Annex F 
                }
                else if (payloadType == 165)
                {
                    this.alpha_channel_info = new AlphaChannelInfo(payloadSize);
                    size += stream.ReadClass<AlphaChannelInfo>(size, context, this.alpha_channel_info); // specified in Annex F 
                }
                else if (payloadType == 166)
                {
                    this.overlay_info = new OverlayInfo(payloadSize);
                    size += stream.ReadClass<OverlayInfo>(size, context, this.overlay_info); // specified in Annex F 
                }
                else if (payloadType == 167)
                {
                    this.temporal_mv_prediction_constraints = new TemporalMvPredictionConstraints(payloadSize);
                    size += stream.ReadClass<TemporalMvPredictionConstraints>(size, context, this.temporal_mv_prediction_constraints); // specified in Annex F 
                }
                else if (payloadType == 168)
                {
                    this.frame_field_info = new FrameFieldInfo(payloadSize);
                    size += stream.ReadClass<FrameFieldInfo>(size, context, this.frame_field_info); // specified in Annex F 
                }
                else if (payloadType == 176)
                {
                    this.three_dimensional_reference_displays_info = new ThreeDimensionalReferenceDisplaysInfo(payloadSize);
                    size += stream.ReadClass<ThreeDimensionalReferenceDisplaysInfo>(size, context, this.three_dimensional_reference_displays_info); // specified in Annex G 
                }
                else if (payloadType == 177)
                {
                    this.depth_representation_info = new DepthRepresentationInfo(payloadSize);
                    size += stream.ReadClass<DepthRepresentationInfo>(size, context, this.depth_representation_info); // specified in Annex G 
                }
                else if (payloadType == 178)
                {
                    this.multiview_scene_info = new MultiviewSceneInfo(payloadSize);
                    size += stream.ReadClass<MultiviewSceneInfo>(size, context, this.multiview_scene_info); // specified in Annex G 
                }
                else if (payloadType == 179)
                {
                    this.multiview_acquisition_info = new MultiviewAcquisitionInfo(payloadSize);
                    size += stream.ReadClass<MultiviewAcquisitionInfo>(size, context, this.multiview_acquisition_info); // specified in Annex G 
                }
                else if (payloadType == 180)
                {
                    this.multiview_view_position = new MultiviewViewPosition(payloadSize);
                    size += stream.ReadClass<MultiviewViewPosition>(size, context, this.multiview_view_position); // specified in Annex G 
                }
                else if (payloadType == 181)
                {
                    this.alternative_depth_info = new AlternativeDepthInfo(payloadSize);
                    size += stream.ReadClass<AlternativeDepthInfo>(size, context, this.alternative_depth_info); // specified in Annex I 
                }
                else
                {
                    this.reserved_sei_message = new ReservedSeiMessage(payloadSize);
                    size += stream.ReadClass<ReservedSeiMessage>(size, context, this.reserved_sei_message);
                }
            }
            else
            {

                if (payloadType == 3)
                {
                    this.filler_payload = new FillerPayload(payloadSize);
                    size += stream.ReadClass<FillerPayload>(size, context, this.filler_payload);
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
                else if (payloadType == 17)
                {
                    this.progressive_refinement_segment_end = new ProgressiveRefinementSegmentEnd(payloadSize);
                    size += stream.ReadClass<ProgressiveRefinementSegmentEnd>(size, context, this.progressive_refinement_segment_end);
                }
                else if (payloadType == 22)
                {
                    this.post_filter_hint = new PostFilterHint(payloadSize);
                    size += stream.ReadClass<PostFilterHint>(size, context, this.post_filter_hint);
                }
                else if (payloadType == 132)
                {
                    this.decoded_picture_hash = new DecodedPictureHash(payloadSize);
                    size += stream.ReadClass<DecodedPictureHash>(size, context, this.decoded_picture_hash);
                }
                else if (payloadType == 146)
                {
                    this.coded_region_completion = new CodedRegionCompletion(payloadSize);
                    size += stream.ReadClass<CodedRegionCompletion>(size, context, this.coded_region_completion);
                }
                else
                {
                    this.reserved_sei_message = new ReservedSeiMessage(payloadSize);
                    size += stream.ReadClass<ReservedSeiMessage>(size, context, this.reserved_sei_message);
                }
            }

            if (more_data_in_payload())
            {

                if (payload_extension_present())
                {
                    size += stream.ReadUnsignedIntVariable(size, reserved_payload_extension_data, out this.reserved_payload_extension_data);
                }
                size += stream.ReadFixed(size, 1, out this.payload_bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadFixed(size, 1, whileIndex, this.payload_bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;

            if (nal_unit_type == H265NALTypes.PREFIX_SEI_NUT)
            {

                if (payloadType == 0)
                {
                    size += stream.WriteClass<BufferingPeriod>(context, this.buffering_period);
                }
                else if (payloadType == 1)
                {
                    size += stream.WriteClass<PicTiming>(context, this.pic_timing);
                }
                else if (payloadType == 2)
                {
                    size += stream.WriteClass<PanScanRect>(context, this.pan_scan_rect);
                }
                else if (payloadType == 3)
                {
                    size += stream.WriteClass<FillerPayload>(context, this.filler_payload);
                }
                else if (payloadType == 4)
                {
                    size += stream.WriteClass<UserDataRegisteredItutT35>(context, this.user_data_registered_itu_t_t35);
                }
                else if (payloadType == 5)
                {
                    size += stream.WriteClass<UserDataUnregistered>(context, this.user_data_unregistered);
                }
                else if (payloadType == 6)
                {
                    size += stream.WriteClass<RecoveryPoint>(context, this.recovery_point);
                }
                else if (payloadType == 9)
                {
                    size += stream.WriteClass<SceneInfo>(context, this.scene_info);
                }
                else if (payloadType == 15)
                {
                    size += stream.WriteClass<PictureSnapshot>(context, this.picture_snapshot);
                }
                else if (payloadType == 16)
                {
                    size += stream.WriteClass<ProgressiveRefinementSegmentStart>(context, this.progressive_refinement_segment_start);
                }
                else if (payloadType == 17)
                {
                    size += stream.WriteClass<ProgressiveRefinementSegmentEnd>(context, this.progressive_refinement_segment_end);
                }
                else if (payloadType == 19)
                {
                    size += stream.WriteClass<FilmGrainCharacteristics>(context, this.film_grain_characteristics);
                }
                else if (payloadType == 22)
                {
                    size += stream.WriteClass<PostFilterHint>(context, this.post_filter_hint);
                }
                else if (payloadType == 23)
                {
                    size += stream.WriteClass<ToneMappingInfo>(context, this.tone_mapping_info);
                }
                else if (payloadType == 45)
                {
                    size += stream.WriteClass<FramePackingArrangement>(context, this.frame_packing_arrangement);
                }
                else if (payloadType == 47)
                {
                    size += stream.WriteClass<DisplayOrientation>(context, this.display_orientation);
                }
                else if (payloadType == 56)
                {
                    size += stream.WriteClass<GreenMetadata>(context, this.green_metadata); // specified in ISO/IEC 23001-11 
                }
                else if (payloadType == 128)
                {
                    size += stream.WriteClass<StructureOfPicturesInfo>(context, this.structure_of_pictures_info);
                }
                else if (payloadType == 129)
                {
                    size += stream.WriteClass<ActiveParameterSets>(context, this.active_parameter_sets);
                }
                else if (payloadType == 130)
                {
                    size += stream.WriteClass<DecodingUnitInfo>(context, this.decoding_unit_info);
                }
                else if (payloadType == 131)
                {
                    size += stream.WriteClass<TemporalSubLayerZeroIdx>(context, this.temporal_sub_layer_zero_idx);
                }
                else if (payloadType == 133)
                {
                    size += stream.WriteClass<ScalableNesting>(context, this.scalable_nesting);
                }
                else if (payloadType == 134)
                {
                    size += stream.WriteClass<RegionRefreshInfo>(context, this.region_refresh_info);
                }
                else if (payloadType == 135)
                {
                    size += stream.WriteClass<NoDisplay>(context, this.no_display);
                }
                else if (payloadType == 136)
                {
                    size += stream.WriteClass<TimeCode>(context, this.time_code);
                }
                else if (payloadType == 137)
                {
                    size += stream.WriteClass<MasteringDisplayColourVolume>(context, this.mastering_display_colour_volume);
                }
                else if (payloadType == 138)
                {
                    size += stream.WriteClass<SegmentedRectFramePackingArrangement>(context, this.segmented_rect_frame_packing_arrangement);
                }
                else if (payloadType == 139)
                {
                    size += stream.WriteClass<TemporalMotionConstrainedTileSets>(context, this.temporal_motion_constrained_tile_sets);
                }
                else if (payloadType == 140)
                {
                    size += stream.WriteClass<ChromaResamplingFilterHint>(context, this.chroma_resampling_filter_hint);
                }
                else if (payloadType == 141)
                {
                    size += stream.WriteClass<KneeFunctionInfo>(context, this.knee_function_info);
                }
                else if (payloadType == 142)
                {
                    size += stream.WriteClass<ColourRemappingInfo>(context, this.colour_remapping_info);
                }
                else if (payloadType == 143)
                {
                    size += stream.WriteClass<DeinterlacedFieldIdentification>(context, this.deinterlaced_field_identification);
                }
                else if (payloadType == 144)
                {
                    size += stream.WriteClass<ContentLightLevelInfo>(context, this.content_light_level_info);
                }
                else if (payloadType == 145)
                {
                    size += stream.WriteClass<DependentRapIndication>(context, this.dependent_rap_indication);
                }
                else if (payloadType == 146)
                {
                    size += stream.WriteClass<CodedRegionCompletion>(context, this.coded_region_completion);
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
                else if (payloadType == 151)
                {
                    size += stream.WriteClass<CubemapProjection>(context, this.cubemap_projection);
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
                else if (payloadType == 157)
                {
                    size += stream.WriteClass<RegionalNesting>(context, this.regional_nesting);
                }
                else if (payloadType == 158)
                {
                    size += stream.WriteClass<MctsExtractionInfoSets>(context, this.mcts_extraction_info_sets);
                }
                else if (payloadType == 159)
                {
                    size += stream.WriteClass<MctsExtractionInfoNesting>(context, this.mcts_extraction_info_nesting);
                }
                else if (payloadType == 160)
                {
                    size += stream.WriteClass<LayersNotPresent>(context, this.layers_not_present); // specified in Annex F 
                }
                else if (payloadType == 161)
                {
                    size += stream.WriteClass<InterLayerConstrainedTileSets>(context, this.inter_layer_constrained_tile_sets); // specified in Annex F 
                }
                else if (payloadType == 162)
                {
                    size += stream.WriteClass<BspNesting>(context, this.bsp_nesting); // specified in Annex F 
                }
                else if (payloadType == 163)
                {
                    size += stream.WriteClass<BspInitialArrivalTime>(context, this.bsp_initial_arrival_time); // specified in Annex F 
                }
                else if (payloadType == 164)
                {
                    size += stream.WriteClass<SubBitstreamProperty>(context, this.sub_bitstream_property); // specified in Annex F 
                }
                else if (payloadType == 165)
                {
                    size += stream.WriteClass<AlphaChannelInfo>(context, this.alpha_channel_info); // specified in Annex F 
                }
                else if (payloadType == 166)
                {
                    size += stream.WriteClass<OverlayInfo>(context, this.overlay_info); // specified in Annex F 
                }
                else if (payloadType == 167)
                {
                    size += stream.WriteClass<TemporalMvPredictionConstraints>(context, this.temporal_mv_prediction_constraints); // specified in Annex F 
                }
                else if (payloadType == 168)
                {
                    size += stream.WriteClass<FrameFieldInfo>(context, this.frame_field_info); // specified in Annex F 
                }
                else if (payloadType == 176)
                {
                    size += stream.WriteClass<ThreeDimensionalReferenceDisplaysInfo>(context, this.three_dimensional_reference_displays_info); // specified in Annex G 
                }
                else if (payloadType == 177)
                {
                    size += stream.WriteClass<DepthRepresentationInfo>(context, this.depth_representation_info); // specified in Annex G 
                }
                else if (payloadType == 178)
                {
                    size += stream.WriteClass<MultiviewSceneInfo>(context, this.multiview_scene_info); // specified in Annex G 
                }
                else if (payloadType == 179)
                {
                    size += stream.WriteClass<MultiviewAcquisitionInfo>(context, this.multiview_acquisition_info); // specified in Annex G 
                }
                else if (payloadType == 180)
                {
                    size += stream.WriteClass<MultiviewViewPosition>(context, this.multiview_view_position); // specified in Annex G 
                }
                else if (payloadType == 181)
                {
                    size += stream.WriteClass<AlternativeDepthInfo>(context, this.alternative_depth_info); // specified in Annex I 
                }
                else
                {
                    size += stream.WriteClass<ReservedSeiMessage>(context, this.reserved_sei_message);
                }
            }
            else
            {

                if (payloadType == 3)
                {
                    size += stream.WriteClass<FillerPayload>(context, this.filler_payload);
                }
                else if (payloadType == 4)
                {
                    size += stream.WriteClass<UserDataRegisteredItutT35>(context, this.user_data_registered_itu_t_t35);
                }
                else if (payloadType == 5)
                {
                    size += stream.WriteClass<UserDataUnregistered>(context, this.user_data_unregistered);
                }
                else if (payloadType == 17)
                {
                    size += stream.WriteClass<ProgressiveRefinementSegmentEnd>(context, this.progressive_refinement_segment_end);
                }
                else if (payloadType == 22)
                {
                    size += stream.WriteClass<PostFilterHint>(context, this.post_filter_hint);
                }
                else if (payloadType == 132)
                {
                    size += stream.WriteClass<DecodedPictureHash>(context, this.decoded_picture_hash);
                }
                else if (payloadType == 146)
                {
                    size += stream.WriteClass<CodedRegionCompletion>(context, this.coded_region_completion);
                }
                else
                {
                    size += stream.WriteClass<ReservedSeiMessage>(context, this.reserved_sei_message);
                }
            }

            if (more_data_in_payload())
            {

                if (payload_extension_present())
                {
                    size += stream.WriteUnsignedIntVariable(reserved_payload_extension_data, this.reserved_payload_extension_data);
                }
                size += stream.WriteFixed(1, this.payload_bit_equal_to_one); // equal to 1 

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteFixed(1, whileIndex, this.payload_bit_equal_to_zero); // equal to 0 
                }
            }

            return size;
        }

    }

    /*
 

buffering_period( payloadSize ) {  
 bp_seq_parameter_set_id ue(v) 
 if( !sub_pic_hrd_params_present_flag )  
  irap_cpb_params_present_flag u(1) 
 if( irap_cpb_params_present_flag ) {  
  cpb_delay_offset u(v) 
  dpb_delay_offset u(v) 
 }  
 concatenation_flag u(1) 
 au_cpb_removal_delay_delta_minus1 u(v) 
 if( NalHrdBpPresentFlag ) {  
  for( i = 0; i < CpbCnt; i++ ) {  
   nal_initial_cpb_removal_delay[ i ] u(v) 
   nal_initial_cpb_removal_offset[ i ] u(v) 
   if( sub_pic_hrd_params_present_flag  ||  irap_cpb_params_present_flag ) {  
    nal_initial_alt_cpb_removal_delay[ i ] u(v) 
    nal_initial_alt_cpb_removal_offset[ i ] u(v) 
   }  
  }  
 }  
 if( VclHrdBpPresentFlag ) {  
  for( i = 0; i < CpbCnt; i++ ) {  
   vcl_initial_cpb_removal_delay[ i ] u(v) 
   vcl_initial_cpb_removal_offset[ i ] u(v) 
   if( sub_pic_hrd_params_present_flag  ||  irap_cpb_params_present_flag ) {  
    vcl_initial_alt_cpb_removal_delay[ i ] u(v) 
    vcl_initial_alt_cpb_removal_offset[ i ] u(v) 
   }  
  }  
 }  
 if( payload_extension_present() )  
  use_alt_cpb_params_flag u(1) 
}
    */
    public class BufferingPeriod : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint bp_seq_parameter_set_id;
        public uint BpSeqParameterSetId { get { return bp_seq_parameter_set_id; } set { bp_seq_parameter_set_id = value; } }
        private byte irap_cpb_params_present_flag;
        public byte IrapCpbParamsPresentFlag { get { return irap_cpb_params_present_flag; } set { irap_cpb_params_present_flag = value; } }
        private uint cpb_delay_offset;
        public uint CpbDelayOffset { get { return cpb_delay_offset; } set { cpb_delay_offset = value; } }
        private uint dpb_delay_offset;
        public uint DpbDelayOffset { get { return dpb_delay_offset; } set { dpb_delay_offset = value; } }
        private byte concatenation_flag;
        public byte ConcatenationFlag { get { return concatenation_flag; } set { concatenation_flag = value; } }
        private uint au_cpb_removal_delay_delta_minus1;
        public uint AuCpbRemovalDelayDeltaMinus1 { get { return au_cpb_removal_delay_delta_minus1; } set { au_cpb_removal_delay_delta_minus1 = value; } }
        private uint[] nal_initial_cpb_removal_delay;
        public uint[] NalInitialCpbRemovalDelay { get { return nal_initial_cpb_removal_delay; } set { nal_initial_cpb_removal_delay = value; } }
        private uint[] nal_initial_cpb_removal_offset;
        public uint[] NalInitialCpbRemovalOffset { get { return nal_initial_cpb_removal_offset; } set { nal_initial_cpb_removal_offset = value; } }
        private uint[] nal_initial_alt_cpb_removal_delay;
        public uint[] NalInitialAltCpbRemovalDelay { get { return nal_initial_alt_cpb_removal_delay; } set { nal_initial_alt_cpb_removal_delay = value; } }
        private uint[] nal_initial_alt_cpb_removal_offset;
        public uint[] NalInitialAltCpbRemovalOffset { get { return nal_initial_alt_cpb_removal_offset; } set { nal_initial_alt_cpb_removal_offset = value; } }
        private uint[] vcl_initial_cpb_removal_delay;
        public uint[] VclInitialCpbRemovalDelay { get { return vcl_initial_cpb_removal_delay; } set { vcl_initial_cpb_removal_delay = value; } }
        private uint[] vcl_initial_cpb_removal_offset;
        public uint[] VclInitialCpbRemovalOffset { get { return vcl_initial_cpb_removal_offset; } set { vcl_initial_cpb_removal_offset = value; } }
        private uint[] vcl_initial_alt_cpb_removal_delay;
        public uint[] VclInitialAltCpbRemovalDelay { get { return vcl_initial_alt_cpb_removal_delay; } set { vcl_initial_alt_cpb_removal_delay = value; } }
        private uint[] vcl_initial_alt_cpb_removal_offset;
        public uint[] VclInitialAltCpbRemovalOffset { get { return vcl_initial_alt_cpb_removal_offset; } set { vcl_initial_alt_cpb_removal_offset = value; } }
        private byte use_alt_cpb_params_flag;
        public byte UseAltCpbParamsFlag { get { return use_alt_cpb_params_flag; } set { use_alt_cpb_params_flag = value; } }

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
            size += stream.ReadUnsignedIntGolomb(size, out this.bp_seq_parameter_set_id);

            if (sub_pic_hrd_params_present_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.irap_cpb_params_present_flag);
            }

            if (irap_cpb_params_present_flag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, cpb_delay_offset, out this.cpb_delay_offset);
                size += stream.ReadUnsignedIntVariable(size, dpb_delay_offset, out this.dpb_delay_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.concatenation_flag);
            size += stream.ReadUnsignedIntVariable(size, au_cpb_removal_delay_delta_minus1, out this.au_cpb_removal_delay_delta_minus1);

            if (NalHrdBpPresentFlag != 0)
            {

                this.nal_initial_cpb_removal_delay = new uint[CpbCnt];
                this.nal_initial_cpb_removal_offset = new uint[CpbCnt];
                this.nal_initial_alt_cpb_removal_delay = new uint[CpbCnt];
                this.nal_initial_alt_cpb_removal_offset = new uint[CpbCnt];
                for (i = 0; i < CpbCnt; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, nal_initial_cpb_removal_delay, out this.nal_initial_cpb_removal_delay[i]);
                    size += stream.ReadUnsignedIntVariable(size, nal_initial_cpb_removal_offset, out this.nal_initial_cpb_removal_offset[i]);

                    if (sub_pic_hrd_params_present_flag != 0 || irap_cpb_params_present_flag != 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, nal_initial_alt_cpb_removal_delay, out this.nal_initial_alt_cpb_removal_delay[i]);
                        size += stream.ReadUnsignedIntVariable(size, nal_initial_alt_cpb_removal_offset, out this.nal_initial_alt_cpb_removal_offset[i]);
                    }
                }
            }

            if (VclHrdBpPresentFlag != 0)
            {

                this.vcl_initial_cpb_removal_delay = new uint[CpbCnt];
                this.vcl_initial_cpb_removal_offset = new uint[CpbCnt];
                this.vcl_initial_alt_cpb_removal_delay = new uint[CpbCnt];
                this.vcl_initial_alt_cpb_removal_offset = new uint[CpbCnt];
                for (i = 0; i < CpbCnt; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, vcl_initial_cpb_removal_delay, out this.vcl_initial_cpb_removal_delay[i]);
                    size += stream.ReadUnsignedIntVariable(size, vcl_initial_cpb_removal_offset, out this.vcl_initial_cpb_removal_offset[i]);

                    if (sub_pic_hrd_params_present_flag != 0 || irap_cpb_params_present_flag != 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, vcl_initial_alt_cpb_removal_delay, out this.vcl_initial_alt_cpb_removal_delay[i]);
                        size += stream.ReadUnsignedIntVariable(size, vcl_initial_alt_cpb_removal_offset, out this.vcl_initial_alt_cpb_removal_offset[i]);
                    }
                }
            }

            if (payload_extension_present())
            {
                size += stream.ReadUnsignedInt(size, 1, out this.use_alt_cpb_params_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.bp_seq_parameter_set_id);

            if (sub_pic_hrd_params_present_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.irap_cpb_params_present_flag);
            }

            if (irap_cpb_params_present_flag != 0)
            {
                size += stream.WriteUnsignedIntVariable(cpb_delay_offset, this.cpb_delay_offset);
                size += stream.WriteUnsignedIntVariable(dpb_delay_offset, this.dpb_delay_offset);
            }
            size += stream.WriteUnsignedInt(1, this.concatenation_flag);
            size += stream.WriteUnsignedIntVariable(au_cpb_removal_delay_delta_minus1, this.au_cpb_removal_delay_delta_minus1);

            if (NalHrdBpPresentFlag != 0)
            {

                for (i = 0; i < CpbCnt; i++)
                {
                    size += stream.WriteUnsignedIntVariable(nal_initial_cpb_removal_delay[i], this.nal_initial_cpb_removal_delay[i]);
                    size += stream.WriteUnsignedIntVariable(nal_initial_cpb_removal_offset[i], this.nal_initial_cpb_removal_offset[i]);

                    if (sub_pic_hrd_params_present_flag != 0 || irap_cpb_params_present_flag != 0)
                    {
                        size += stream.WriteUnsignedIntVariable(nal_initial_alt_cpb_removal_delay[i], this.nal_initial_alt_cpb_removal_delay[i]);
                        size += stream.WriteUnsignedIntVariable(nal_initial_alt_cpb_removal_offset[i], this.nal_initial_alt_cpb_removal_offset[i]);
                    }
                }
            }

            if (VclHrdBpPresentFlag != 0)
            {

                for (i = 0; i < CpbCnt; i++)
                {
                    size += stream.WriteUnsignedIntVariable(vcl_initial_cpb_removal_delay[i], this.vcl_initial_cpb_removal_delay[i]);
                    size += stream.WriteUnsignedIntVariable(vcl_initial_cpb_removal_offset[i], this.vcl_initial_cpb_removal_offset[i]);

                    if (sub_pic_hrd_params_present_flag != 0 || irap_cpb_params_present_flag != 0)
                    {
                        size += stream.WriteUnsignedIntVariable(vcl_initial_alt_cpb_removal_delay[i], this.vcl_initial_alt_cpb_removal_delay[i]);
                        size += stream.WriteUnsignedIntVariable(vcl_initial_alt_cpb_removal_offset[i], this.vcl_initial_alt_cpb_removal_offset[i]);
                    }
                }
            }

            if (payload_extension_present())
            {
                size += stream.WriteUnsignedInt(1, this.use_alt_cpb_params_flag);
            }

            return size;
        }

    }

    /*
  

pic_timing( payloadSize ) {  
 if( frame_field_info_present_flag ) {  
  pic_struct u(4) 
  source_scan_type u(2) 
  duplicate_flag u(1) 
 }  
 if( CpbDpbDelaysPresentFlag ) {  
  au_cpb_removal_delay_minus1 u(v) 
  pic_dpb_output_delay u(v) 
  if( sub_pic_hrd_params_present_flag )  
   pic_dpb_output_du_delay u(v) 
  if( sub_pic_hrd_params_present_flag  && 
    sub_pic_cpb_params_in_pic_timing_sei_flag ) { 
 
   num_decoding_units_minus1 ue(v) 
   du_common_cpb_removal_delay_flag u(1) 
   if( du_common_cpb_removal_delay_flag )  
    du_common_cpb_removal_delay_increment_minus1 u(v) 
   for( i = 0; i <= num_decoding_units_minus1; i++ ) {  
    num_nalus_in_du_minus1[ i ] ue(v) 
    if( !du_common_cpb_removal_delay_flag  &&  i < num_decoding_units_minus1 )  
     du_cpb_removal_delay_increment_minus1[ i ] u(v) 
   }  
  }  
 }  
}
    */
    public class PicTiming : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint pic_struct;
        public uint PicStruct { get { return pic_struct; } set { pic_struct = value; } }
        private uint source_scan_type;
        public uint SourceScanType { get { return source_scan_type; } set { source_scan_type = value; } }
        private byte duplicate_flag;
        public byte DuplicateFlag { get { return duplicate_flag; } set { duplicate_flag = value; } }
        private uint au_cpb_removal_delay_minus1;
        public uint AuCpbRemovalDelayMinus1 { get { return au_cpb_removal_delay_minus1; } set { au_cpb_removal_delay_minus1 = value; } }
        private uint pic_dpb_output_delay;
        public uint PicDpbOutputDelay { get { return pic_dpb_output_delay; } set { pic_dpb_output_delay = value; } }
        private uint pic_dpb_output_du_delay;
        public uint PicDpbOutputDuDelay { get { return pic_dpb_output_du_delay; } set { pic_dpb_output_du_delay = value; } }
        private uint num_decoding_units_minus1;
        public uint NumDecodingUnitsMinus1 { get { return num_decoding_units_minus1; } set { num_decoding_units_minus1 = value; } }
        private byte du_common_cpb_removal_delay_flag;
        public byte DuCommonCpbRemovalDelayFlag { get { return du_common_cpb_removal_delay_flag; } set { du_common_cpb_removal_delay_flag = value; } }
        private uint du_common_cpb_removal_delay_increment_minus1;
        public uint DuCommonCpbRemovalDelayIncrementMinus1 { get { return du_common_cpb_removal_delay_increment_minus1; } set { du_common_cpb_removal_delay_increment_minus1 = value; } }
        private uint[] num_nalus_in_du_minus1;
        public uint[] NumNalusInDuMinus1 { get { return num_nalus_in_du_minus1; } set { num_nalus_in_du_minus1 = value; } }
        private uint[] du_cpb_removal_delay_increment_minus1;
        public uint[] DuCpbRemovalDelayIncrementMinus1 { get { return du_cpb_removal_delay_increment_minus1; } set { du_cpb_removal_delay_increment_minus1 = value; } }

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

            if (frame_field_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 4, out this.pic_struct);
                size += stream.ReadUnsignedInt(size, 2, out this.source_scan_type);
                size += stream.ReadUnsignedInt(size, 1, out this.duplicate_flag);
            }

            if (CpbDpbDelaysPresentFlag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, au_cpb_removal_delay_minus1, out this.au_cpb_removal_delay_minus1);
                size += stream.ReadUnsignedIntVariable(size, pic_dpb_output_delay, out this.pic_dpb_output_delay);

                if (sub_pic_hrd_params_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntVariable(size, pic_dpb_output_du_delay, out this.pic_dpb_output_du_delay);
                }

                if (sub_pic_hrd_params_present_flag != 0 &&
    sub_pic_cpb_params_in_pic_timing_sei_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_decoding_units_minus1);
                    size += stream.ReadUnsignedInt(size, 1, out this.du_common_cpb_removal_delay_flag);

                    if (du_common_cpb_removal_delay_flag != 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, du_common_cpb_removal_delay_increment_minus1, out this.du_common_cpb_removal_delay_increment_minus1);
                    }

                    this.num_nalus_in_du_minus1 = new uint[num_decoding_units_minus1 + 1];
                    this.du_cpb_removal_delay_increment_minus1 = new uint[num_decoding_units_minus1 + 1];
                    for (i = 0; i <= num_decoding_units_minus1; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_nalus_in_du_minus1[i]);

                        if (du_common_cpb_removal_delay_flag == 0 && i < num_decoding_units_minus1)
                        {
                            size += stream.ReadUnsignedIntVariable(size, du_cpb_removal_delay_increment_minus1, out this.du_cpb_removal_delay_increment_minus1[i]);
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

            if (frame_field_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(4, this.pic_struct);
                size += stream.WriteUnsignedInt(2, this.source_scan_type);
                size += stream.WriteUnsignedInt(1, this.duplicate_flag);
            }

            if (CpbDpbDelaysPresentFlag != 0)
            {
                size += stream.WriteUnsignedIntVariable(au_cpb_removal_delay_minus1, this.au_cpb_removal_delay_minus1);
                size += stream.WriteUnsignedIntVariable(pic_dpb_output_delay, this.pic_dpb_output_delay);

                if (sub_pic_hrd_params_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntVariable(pic_dpb_output_du_delay, this.pic_dpb_output_du_delay);
                }

                if (sub_pic_hrd_params_present_flag != 0 &&
    sub_pic_cpb_params_in_pic_timing_sei_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_decoding_units_minus1);
                    size += stream.WriteUnsignedInt(1, this.du_common_cpb_removal_delay_flag);

                    if (du_common_cpb_removal_delay_flag != 0)
                    {
                        size += stream.WriteUnsignedIntVariable(du_common_cpb_removal_delay_increment_minus1, this.du_common_cpb_removal_delay_increment_minus1);
                    }

                    for (i = 0; i <= num_decoding_units_minus1; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_nalus_in_du_minus1[i]);

                        if (du_common_cpb_removal_delay_flag == 0 && i < num_decoding_units_minus1)
                        {
                            size += stream.WriteUnsignedIntVariable(du_cpb_removal_delay_increment_minus1[i], this.du_cpb_removal_delay_increment_minus1[i]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
 

pan_scan_rect( payloadSize ) {  
 pan_scan_rect_id ue(v) 
 pan_scan_rect_cancel_flag u(1) 
 if( !pan_scan_rect_cancel_flag ) {  
  pan_scan_cnt_minus1 ue(v) 
  for( i = 0; i <= pan_scan_cnt_minus1; i++ ) {  
   pan_scan_rect_left_offset[ i ] se(v) 
   pan_scan_rect_right_offset[ i ] se(v) 
   pan_scan_rect_top_offset[ i ] se(v) 
   pan_scan_rect_bottom_offset[ i ] se(v) 
  }  
  pan_scan_rect_persistence_flag u(1) 
 }  
}
    */
    public class PanScanRect : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint pan_scan_rect_id;
        public uint PanScanRectId { get { return pan_scan_rect_id; } set { pan_scan_rect_id = value; } }
        private byte pan_scan_rect_cancel_flag;
        public byte PanScanRectCancelFlag { get { return pan_scan_rect_cancel_flag; } set { pan_scan_rect_cancel_flag = value; } }
        private uint pan_scan_cnt_minus1;
        public uint PanScanCntMinus1 { get { return pan_scan_cnt_minus1; } set { pan_scan_cnt_minus1 = value; } }
        private int[] pan_scan_rect_left_offset;
        public int[] PanScanRectLeftOffset { get { return pan_scan_rect_left_offset; } set { pan_scan_rect_left_offset = value; } }
        private int[] pan_scan_rect_right_offset;
        public int[] PanScanRectRightOffset { get { return pan_scan_rect_right_offset; } set { pan_scan_rect_right_offset = value; } }
        private int[] pan_scan_rect_top_offset;
        public int[] PanScanRectTopOffset { get { return pan_scan_rect_top_offset; } set { pan_scan_rect_top_offset = value; } }
        private int[] pan_scan_rect_bottom_offset;
        public int[] PanScanRectBottomOffset { get { return pan_scan_rect_bottom_offset; } set { pan_scan_rect_bottom_offset = value; } }
        private byte pan_scan_rect_persistence_flag;
        public byte PanScanRectPersistenceFlag { get { return pan_scan_rect_persistence_flag; } set { pan_scan_rect_persistence_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PanScanRect(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.pan_scan_rect_id);
            size += stream.ReadUnsignedInt(size, 1, out this.pan_scan_rect_cancel_flag);

            if (pan_scan_rect_cancel_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.pan_scan_cnt_minus1);

                this.pan_scan_rect_left_offset = new int[pan_scan_cnt_minus1 + 1];
                this.pan_scan_rect_right_offset = new int[pan_scan_cnt_minus1 + 1];
                this.pan_scan_rect_top_offset = new int[pan_scan_cnt_minus1 + 1];
                this.pan_scan_rect_bottom_offset = new int[pan_scan_cnt_minus1 + 1];
                for (i = 0; i <= pan_scan_cnt_minus1; i++)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_left_offset[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_right_offset[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_top_offset[i]);
                    size += stream.ReadSignedIntGolomb(size, out this.pan_scan_rect_bottom_offset[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.pan_scan_rect_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.pan_scan_rect_id);
            size += stream.WriteUnsignedInt(1, this.pan_scan_rect_cancel_flag);

            if (pan_scan_rect_cancel_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.pan_scan_cnt_minus1);

                for (i = 0; i <= pan_scan_cnt_minus1; i++)
                {
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_left_offset[i]);
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_right_offset[i]);
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_top_offset[i]);
                    size += stream.WriteSignedIntGolomb(this.pan_scan_rect_bottom_offset[i]);
                }
                size += stream.WriteUnsignedInt(1, this.pan_scan_rect_persistence_flag);
            }

            return size;
        }

    }

    /*
 

filler_payload( payloadSize ) {  
 for( k = 0; k < payloadSize; k++)  
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
 

user_data_registered_itu_t_t35( payloadSize ) {  
 itu_t_t35_country_code b(8) 
 if( itu_t_t35_country_code != 0xFF )  
  i = 1  
 else {  
  itu_t_t35_country_code_extension_byte b(8) 
  i = 2  
 }  
 do {  
  itu_t_t35_payload_byte b(8) 
  i++  
 } while( i < payloadSize )  
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
 

user_data_unregistered( payloadSize ) {  
 uuid_iso_iec_11578 u(128) 
 for( i = 16; i < payloadSize; i++ )  
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
 

recovery_point( payloadSize ) {  
 recovery_poc_cnt se(v) 
 exact_match_flag u(1) 
 broken_link_flag u(1) 
}
    */
    public class RecoveryPoint : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private int recovery_poc_cnt;
        public int RecoveryPocCnt { get { return recovery_poc_cnt; } set { recovery_poc_cnt = value; } }
        private byte exact_match_flag;
        public byte ExactMatchFlag { get { return exact_match_flag; } set { exact_match_flag = value; } }
        private byte broken_link_flag;
        public byte BrokenLinkFlag { get { return broken_link_flag; } set { broken_link_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RecoveryPoint(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadSignedIntGolomb(size, out this.recovery_poc_cnt);
            size += stream.ReadUnsignedInt(size, 1, out this.exact_match_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.broken_link_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteSignedIntGolomb(this.recovery_poc_cnt);
            size += stream.WriteUnsignedInt(1, this.exact_match_flag);
            size += stream.WriteUnsignedInt(1, this.broken_link_flag);

            return size;
        }

    }

    /*
 

scene_info( payloadSize ) { 
scene_info_present_flag  u(1)
if( scene_info_present_flag ) {  
prev_scene_id_valid_flag u(1)
scene_id ue(v) 
scene_transition_type ue(v)
if( scene_transition_type > 3 ) 
  second_scene_id ue(v) 
}  
}
    */
    public class SceneInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte scene_info_present_flag;
        public byte SceneInfoPresentFlag { get { return scene_info_present_flag; } set { scene_info_present_flag = value; } }
        private byte prev_scene_id_valid_flag;
        public byte PrevSceneIdValidFlag { get { return prev_scene_id_valid_flag; } set { prev_scene_id_valid_flag = value; } }
        private uint scene_id;
        public uint SceneId { get { return scene_id; } set { scene_id = value; } }
        private uint scene_transition_type;
        public uint SceneTransitionType { get { return scene_transition_type; } set { scene_transition_type = value; } }
        private uint second_scene_id;
        public uint SecondSceneId { get { return second_scene_id; } set { second_scene_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SceneInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.scene_info_present_flag);

            if (scene_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.prev_scene_id_valid_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.scene_id);
                size += stream.ReadUnsignedIntGolomb(size, out this.scene_transition_type);

                if (scene_transition_type > 3)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.second_scene_id);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.scene_info_present_flag);

            if (scene_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.prev_scene_id_valid_flag);
                size += stream.WriteUnsignedIntGolomb(this.scene_id);
                size += stream.WriteUnsignedIntGolomb(this.scene_transition_type);

                if (scene_transition_type > 3)
                {
                    size += stream.WriteUnsignedIntGolomb(this.second_scene_id);
                }
            }

            return size;
        }

    }

    /*
 

picture_snapshot( payloadSize ) { 
 snapshot_id ue(v) 
}
    */
    public class PictureSnapshot : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint snapshot_id;
        public uint SnapshotId { get { return snapshot_id; } set { snapshot_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PictureSnapshot(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.snapshot_id);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.snapshot_id);

            return size;
        }

    }

    /*
 

progressive_refinement_segment_start( payloadSize ) { 
  progressive_refinement_id ue(v) 
  pic_order_cnt_delta ue(v) 
}
    */
    public class ProgressiveRefinementSegmentStart : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint progressive_refinement_id;
        public uint ProgressiveRefinementId { get { return progressive_refinement_id; } set { progressive_refinement_id = value; } }
        private uint pic_order_cnt_delta;
        public uint PicOrderCntDelta { get { return pic_order_cnt_delta; } set { pic_order_cnt_delta = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ProgressiveRefinementSegmentStart(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.progressive_refinement_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.pic_order_cnt_delta);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.progressive_refinement_id);
            size += stream.WriteUnsignedIntGolomb(this.pic_order_cnt_delta);

            return size;
        }

    }

    /*
 

progressive_refinement_segment_end( payloadSize ) { 
  progressive_refinement_id ue(v) 
}
    */
    public class ProgressiveRefinementSegmentEnd : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint progressive_refinement_id;
        public uint ProgressiveRefinementId { get { return progressive_refinement_id; } set { progressive_refinement_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ProgressiveRefinementSegmentEnd(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.progressive_refinement_id);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.progressive_refinement_id);

            return size;
        }

    }

    /*
 

film_grain_characteristics( payloadSize ) {  
 film_grain_characteristics_cancel_flag u(1) 
 if( !film_grain_characteristics_cancel_flag ) {  
  film_grain_model_id u(2) 
  separate_colour_description_present_flag u(1) 
  if( separate_colour_description_present_flag ) {  
   film_grain_bit_depth_luma_minus8 u(3) 
   film_grain_bit_depth_chroma_minus8 u(3) 
   film_grain_full_range_flag u(1) 
   film_grain_colour_primaries u(8) 
   film_grain_transfer_characteristics u(8) 
   film_grain_matrix_coeffs u(8) 
  }  
  blending_mode_id u(2) 
  log2_scale_factor u(4) 
  for( c = 0; c < 3; c++ )  
   comp_model_present_flag[ c ] u(1) 
  for( c = 0; c < 3; c++ )  
   if( comp_model_present_flag[ c ] ) {  
    num_intensity_intervals_minus1[ c ] u(8) 
    num_model_values_minus1[ c ] u(3) 
    for( i = 0; i <= num_intensity_intervals_minus1[ c ]; i++ ) {  
     intensity_interval_lower_bound[ c ][ i ] u(8) 
     intensity_interval_upper_bound[ c ][ i ] u(8) 
     for( j = 0; j <= num_model_values_minus1[ c ]; j++ )  
      comp_model_value[ c ][ i ][ j ] se(v) 
    }  
   }  
 film_grain_characteristics_persistence_flag u(1) 
 }  
}
    */
    public class FilmGrainCharacteristics : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte film_grain_characteristics_cancel_flag;
        public byte FilmGrainCharacteristicsCancelFlag { get { return film_grain_characteristics_cancel_flag; } set { film_grain_characteristics_cancel_flag = value; } }
        private uint film_grain_model_id;
        public uint FilmGrainModelId { get { return film_grain_model_id; } set { film_grain_model_id = value; } }
        private byte separate_colour_description_present_flag;
        public byte SeparateColourDescriptionPresentFlag { get { return separate_colour_description_present_flag; } set { separate_colour_description_present_flag = value; } }
        private uint film_grain_bit_depth_luma_minus8;
        public uint FilmGrainBitDepthLumaMinus8 { get { return film_grain_bit_depth_luma_minus8; } set { film_grain_bit_depth_luma_minus8 = value; } }
        private uint film_grain_bit_depth_chroma_minus8;
        public uint FilmGrainBitDepthChromaMinus8 { get { return film_grain_bit_depth_chroma_minus8; } set { film_grain_bit_depth_chroma_minus8 = value; } }
        private byte film_grain_full_range_flag;
        public byte FilmGrainFullRangeFlag { get { return film_grain_full_range_flag; } set { film_grain_full_range_flag = value; } }
        private uint film_grain_colour_primaries;
        public uint FilmGrainColourPrimaries { get { return film_grain_colour_primaries; } set { film_grain_colour_primaries = value; } }
        private uint film_grain_transfer_characteristics;
        public uint FilmGrainTransferCharacteristics { get { return film_grain_transfer_characteristics; } set { film_grain_transfer_characteristics = value; } }
        private uint film_grain_matrix_coeffs;
        public uint FilmGrainMatrixCoeffs { get { return film_grain_matrix_coeffs; } set { film_grain_matrix_coeffs = value; } }
        private uint blending_mode_id;
        public uint BlendingModeId { get { return blending_mode_id; } set { blending_mode_id = value; } }
        private uint log2_scale_factor;
        public uint Log2ScaleFactor { get { return log2_scale_factor; } set { log2_scale_factor = value; } }
        private byte[] comp_model_present_flag;
        public byte[] CompModelPresentFlag { get { return comp_model_present_flag; } set { comp_model_present_flag = value; } }
        private uint[] num_intensity_intervals_minus1;
        public uint[] NumIntensityIntervalsMinus1 { get { return num_intensity_intervals_minus1; } set { num_intensity_intervals_minus1 = value; } }
        private uint[] num_model_values_minus1;
        public uint[] NumModelValuesMinus1 { get { return num_model_values_minus1; } set { num_model_values_minus1 = value; } }
        private uint[][] intensity_interval_lower_bound;
        public uint[][] IntensityIntervalLowerBound { get { return intensity_interval_lower_bound; } set { intensity_interval_lower_bound = value; } }
        private uint[][] intensity_interval_upper_bound;
        public uint[][] IntensityIntervalUpperBound { get { return intensity_interval_upper_bound; } set { intensity_interval_upper_bound = value; } }
        private int[][][] comp_model_value;
        public int[][][] CompModelValue { get { return comp_model_value; } set { comp_model_value = value; } }
        private byte film_grain_characteristics_persistence_flag;
        public byte FilmGrainCharacteristicsPersistenceFlag { get { return film_grain_characteristics_persistence_flag; } set { film_grain_characteristics_persistence_flag = value; } }

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
            size += stream.ReadUnsignedInt(size, 1, out this.film_grain_characteristics_cancel_flag);

            if (film_grain_characteristics_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.film_grain_model_id);
                size += stream.ReadUnsignedInt(size, 1, out this.separate_colour_description_present_flag);

                if (separate_colour_description_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.film_grain_bit_depth_luma_minus8);
                    size += stream.ReadUnsignedInt(size, 3, out this.film_grain_bit_depth_chroma_minus8);
                    size += stream.ReadUnsignedInt(size, 1, out this.film_grain_full_range_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.film_grain_colour_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.film_grain_transfer_characteristics);
                    size += stream.ReadUnsignedInt(size, 8, out this.film_grain_matrix_coeffs);
                }
                size += stream.ReadUnsignedInt(size, 2, out this.blending_mode_id);
                size += stream.ReadUnsignedInt(size, 4, out this.log2_scale_factor);

                this.comp_model_present_flag = new byte[3];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.comp_model_present_flag[c]);
                }

                this.num_intensity_intervals_minus1 = new uint[3];
                this.num_model_values_minus1 = new uint[3];
                this.intensity_interval_lower_bound = new uint[3][];
                this.intensity_interval_upper_bound = new uint[3][];
                this.comp_model_value = new int[3][][];
                for (c = 0; c < 3; c++)
                {

                    if (comp_model_present_flag[c] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.num_intensity_intervals_minus1[c]);
                        size += stream.ReadUnsignedInt(size, 3, out this.num_model_values_minus1[c]);

                        this.intensity_interval_lower_bound[c] = new uint[num_intensity_intervals_minus1[c] + 1];
                        this.intensity_interval_upper_bound[c] = new uint[num_intensity_intervals_minus1[c] + 1];
                        this.comp_model_value[c] = new int[num_intensity_intervals_minus1[c] + 1][];
                        for (i = 0; i <= num_intensity_intervals_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedInt(size, 8, out this.intensity_interval_lower_bound[c][i]);
                            size += stream.ReadUnsignedInt(size, 8, out this.intensity_interval_upper_bound[c][i]);

                            this.comp_model_value[c][i] = new int[num_model_values_minus1[c] + 1];
                            for (j = 0; j <= num_model_values_minus1[c]; j++)
                            {
                                size += stream.ReadSignedIntGolomb(size, out this.comp_model_value[c][i][j]);
                            }
                        }
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.film_grain_characteristics_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.film_grain_characteristics_cancel_flag);

            if (film_grain_characteristics_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(2, this.film_grain_model_id);
                size += stream.WriteUnsignedInt(1, this.separate_colour_description_present_flag);

                if (separate_colour_description_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(3, this.film_grain_bit_depth_luma_minus8);
                    size += stream.WriteUnsignedInt(3, this.film_grain_bit_depth_chroma_minus8);
                    size += stream.WriteUnsignedInt(1, this.film_grain_full_range_flag);
                    size += stream.WriteUnsignedInt(8, this.film_grain_colour_primaries);
                    size += stream.WriteUnsignedInt(8, this.film_grain_transfer_characteristics);
                    size += stream.WriteUnsignedInt(8, this.film_grain_matrix_coeffs);
                }
                size += stream.WriteUnsignedInt(2, this.blending_mode_id);
                size += stream.WriteUnsignedInt(4, this.log2_scale_factor);

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(1, this.comp_model_present_flag[c]);
                }

                for (c = 0; c < 3; c++)
                {

                    if (comp_model_present_flag[c] != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.num_intensity_intervals_minus1[c]);
                        size += stream.WriteUnsignedInt(3, this.num_model_values_minus1[c]);

                        for (i = 0; i <= num_intensity_intervals_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedInt(8, this.intensity_interval_lower_bound[c][i]);
                            size += stream.WriteUnsignedInt(8, this.intensity_interval_upper_bound[c][i]);

                            for (j = 0; j <= num_model_values_minus1[c]; j++)
                            {
                                size += stream.WriteSignedIntGolomb(this.comp_model_value[c][i][j]);
                            }
                        }
                    }
                }
                size += stream.WriteUnsignedInt(1, this.film_grain_characteristics_persistence_flag);
            }

            return size;
        }

    }

    /*
  

post_filter_hint( payloadSize ) {  
 filter_hint_size_y ue(v) 
 filter_hint_size_x ue(v) 
 filter_hint_type u(2) 
 for( cIdx = 0; cIdx < ( chroma_format_idc == 0 ? 1 : 3 ); cIdx++ )  
  for( cy = 0; cy < filter_hint_size_y; cy++ )  
   for( cx = 0; cx < filter_hint_size_x; cx++ )  
    filter_hint_value[ cIdx ][ cy ][ cx ] se(v) 
}
    */
    public class PostFilterHint : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint filter_hint_size_y;
        public uint FilterHintSizey { get { return filter_hint_size_y; } set { filter_hint_size_y = value; } }
        private uint filter_hint_size_x;
        public uint FilterHintSizex { get { return filter_hint_size_x; } set { filter_hint_size_x = value; } }
        private uint filter_hint_type;
        public uint FilterHintType { get { return filter_hint_type; } set { filter_hint_type = value; } }
        private int[][][] filter_hint_value;
        public int[][][] FilterHintValue { get { return filter_hint_value; } set { filter_hint_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PostFilterHint(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint cIdx = 0;
            uint cy = 0;
            uint cx = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.filter_hint_size_y);
            size += stream.ReadUnsignedIntGolomb(size, out this.filter_hint_size_x);
            size += stream.ReadUnsignedInt(size, 2, out this.filter_hint_type);

            this.filter_hint_value = new int[(((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3)][][];
            for (cIdx = 0; cIdx < (((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3); cIdx++)
            {

                this.filter_hint_value[cIdx] = new int[filter_hint_size_y][];
                for (cy = 0; cy < filter_hint_size_y; cy++)
                {

                    this.filter_hint_value[cIdx][cy] = new int[filter_hint_size_x];
                    for (cx = 0; cx < filter_hint_size_x; cx++)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.filter_hint_value[cIdx][cy][cx]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint cIdx = 0;
            uint cy = 0;
            uint cx = 0;
            size += stream.WriteUnsignedIntGolomb(this.filter_hint_size_y);
            size += stream.WriteUnsignedIntGolomb(this.filter_hint_size_x);
            size += stream.WriteUnsignedInt(2, this.filter_hint_type);

            for (cIdx = 0; cIdx < (((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3); cIdx++)
            {

                for (cy = 0; cy < filter_hint_size_y; cy++)
                {

                    for (cx = 0; cx < filter_hint_size_x; cx++)
                    {
                        size += stream.WriteSignedIntGolomb(this.filter_hint_value[cIdx][cy][cx]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

tone_mapping_info( payloadSize ) {  
 tone_map_id ue(v) 
 tone_map_cancel_flag u(1) 
 if( !tone_map_cancel_flag ) {  
  tone_map_persistence_flag u(1) 
  coded_data_bit_depth u(8) 
  target_bit_depth u(8) 
  tone_map_model_id ue(v) 
  if( tone_map_model_id == 0 ) {  
   min_value u(32) 
   max_value u(32) 
  } else if( tone_map_model_id == 1 ) {  
   sigmoid_midpoint u(32) 
   sigmoid_width u(32) 
  } else if( tone_map_model_id == 2 )  
   for( i = 0; i < ( 1  <<  target_bit_depth ); i++ )  
    start_of_coded_interval[ i ] u(v) 
  else if( tone_map_model_id == 3 ) {  
   num_pivots u(16) 
   for( i = 0; i < num_pivots; i++ ) {  
    coded_pivot_value[ i ] u(v) 
    target_pivot_value[ i ] u(v) 
   }  
  } else if( tone_map_model_id == 4 ) {  
   camera_iso_speed_idc u(8) 
   if( camera_iso_speed_idc == EXTENDED_ISO )  
    camera_iso_speed_value u(32) 
   exposure_idx_idc u(8) 
   if( exposure_idx_idc == EXTENDED_ISO )  
    exposure_idx_value u(32) 
   exposure_compensation_value_sign_flag u(1) 
   exposure_compensation_value_numerator u(16) 
   exposure_compensation_value_denom_idc u(16) 
   ref_screen_luminance_white u(32) 
   extended_range_white_level u(32) 
   nominal_black_level_code_value u(16) 
   nominal_white_level_code_value u(16) 
   extended_white_level_code_value u(16) 
  }  
 }  
}
    */
    public class ToneMappingInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint tone_map_id;
        public uint ToneMapId { get { return tone_map_id; } set { tone_map_id = value; } }
        private byte tone_map_cancel_flag;
        public byte ToneMapCancelFlag { get { return tone_map_cancel_flag; } set { tone_map_cancel_flag = value; } }
        private byte tone_map_persistence_flag;
        public byte ToneMapPersistenceFlag { get { return tone_map_persistence_flag; } set { tone_map_persistence_flag = value; } }
        private uint coded_data_bit_depth;
        public uint CodedDataBitDepth { get { return coded_data_bit_depth; } set { coded_data_bit_depth = value; } }
        private uint target_bit_depth;
        public uint TargetBitDepth { get { return target_bit_depth; } set { target_bit_depth = value; } }
        private uint tone_map_model_id;
        public uint ToneMapModelId { get { return tone_map_model_id; } set { tone_map_model_id = value; } }
        private uint min_value;
        public uint MinValue { get { return min_value; } set { min_value = value; } }
        private uint max_value;
        public uint MaxValue { get { return max_value; } set { max_value = value; } }
        private uint sigmoid_midpoint;
        public uint SigmoidMidpoint { get { return sigmoid_midpoint; } set { sigmoid_midpoint = value; } }
        private uint sigmoid_width;
        public uint SigmoidWidth { get { return sigmoid_width; } set { sigmoid_width = value; } }
        private uint[] start_of_coded_interval;
        public uint[] StartOfCodedInterval { get { return start_of_coded_interval; } set { start_of_coded_interval = value; } }
        private uint num_pivots;
        public uint NumPivots { get { return num_pivots; } set { num_pivots = value; } }
        private uint[] coded_pivot_value;
        public uint[] CodedPivotValue { get { return coded_pivot_value; } set { coded_pivot_value = value; } }
        private uint[] target_pivot_value;
        public uint[] TargetPivotValue { get { return target_pivot_value; } set { target_pivot_value = value; } }
        private uint camera_iso_speed_idc;
        public uint CameraIsoSpeedIdc { get { return camera_iso_speed_idc; } set { camera_iso_speed_idc = value; } }
        private uint camera_iso_speed_value;
        public uint CameraIsoSpeedValue { get { return camera_iso_speed_value; } set { camera_iso_speed_value = value; } }
        private uint exposure_idx_idc;
        public uint ExposureIdxIdc { get { return exposure_idx_idc; } set { exposure_idx_idc = value; } }
        private uint exposure_idx_value;
        public uint ExposureIdxValue { get { return exposure_idx_value; } set { exposure_idx_value = value; } }
        private byte exposure_compensation_value_sign_flag;
        public byte ExposureCompensationValueSignFlag { get { return exposure_compensation_value_sign_flag; } set { exposure_compensation_value_sign_flag = value; } }
        private uint exposure_compensation_value_numerator;
        public uint ExposureCompensationValueNumerator { get { return exposure_compensation_value_numerator; } set { exposure_compensation_value_numerator = value; } }
        private uint exposure_compensation_value_denom_idc;
        public uint ExposureCompensationValueDenomIdc { get { return exposure_compensation_value_denom_idc; } set { exposure_compensation_value_denom_idc = value; } }
        private uint ref_screen_luminance_white;
        public uint RefScreenLuminanceWhite { get { return ref_screen_luminance_white; } set { ref_screen_luminance_white = value; } }
        private uint extended_range_white_level;
        public uint ExtendedRangeWhiteLevel { get { return extended_range_white_level; } set { extended_range_white_level = value; } }
        private uint nominal_black_level_code_value;
        public uint NominalBlackLevelCodeValue { get { return nominal_black_level_code_value; } set { nominal_black_level_code_value = value; } }
        private uint nominal_white_level_code_value;
        public uint NominalWhiteLevelCodeValue { get { return nominal_white_level_code_value; } set { nominal_white_level_code_value = value; } }
        private uint extended_white_level_code_value;
        public uint ExtendedWhiteLevelCodeValue { get { return extended_white_level_code_value; } set { extended_white_level_code_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ToneMappingInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.tone_map_id);
            size += stream.ReadUnsignedInt(size, 1, out this.tone_map_cancel_flag);

            if (tone_map_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.tone_map_persistence_flag);
                size += stream.ReadUnsignedInt(size, 8, out this.coded_data_bit_depth);
                size += stream.ReadUnsignedInt(size, 8, out this.target_bit_depth);
                size += stream.ReadUnsignedIntGolomb(size, out this.tone_map_model_id);

                if (tone_map_model_id == 0)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.min_value);
                    size += stream.ReadUnsignedInt(size, 32, out this.max_value);
                }
                else if (tone_map_model_id == 1)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.sigmoid_midpoint);
                    size += stream.ReadUnsignedInt(size, 32, out this.sigmoid_width);
                }
                else if (tone_map_model_id == 2)
                {

                    this.start_of_coded_interval = new uint[(1 << (int)target_bit_depth)];
                    for (i = 0; i < (1 << (int)target_bit_depth); i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, start_of_coded_interval, out this.start_of_coded_interval[i]);
                    }
                }
                else if (tone_map_model_id == 3)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.num_pivots);

                    this.coded_pivot_value = new uint[num_pivots];
                    this.target_pivot_value = new uint[num_pivots];
                    for (i = 0; i < num_pivots; i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, coded_pivot_value, out this.coded_pivot_value[i]);
                        size += stream.ReadUnsignedIntVariable(size, target_pivot_value, out this.target_pivot_value[i]);
                    }
                }
                else if (tone_map_model_id == 4)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.camera_iso_speed_idc);

                    if (camera_iso_speed_idc == H265Constants.EXTENDED_ISO)
                    {
                        size += stream.ReadUnsignedInt(size, 32, out this.camera_iso_speed_value);
                    }
                    size += stream.ReadUnsignedInt(size, 8, out this.exposure_idx_idc);

                    if (exposure_idx_idc == H265Constants.EXTENDED_ISO)
                    {
                        size += stream.ReadUnsignedInt(size, 32, out this.exposure_idx_value);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.exposure_compensation_value_sign_flag);
                    size += stream.ReadUnsignedInt(size, 16, out this.exposure_compensation_value_numerator);
                    size += stream.ReadUnsignedInt(size, 16, out this.exposure_compensation_value_denom_idc);
                    size += stream.ReadUnsignedInt(size, 32, out this.ref_screen_luminance_white);
                    size += stream.ReadUnsignedInt(size, 32, out this.extended_range_white_level);
                    size += stream.ReadUnsignedInt(size, 16, out this.nominal_black_level_code_value);
                    size += stream.ReadUnsignedInt(size, 16, out this.nominal_white_level_code_value);
                    size += stream.ReadUnsignedInt(size, 16, out this.extended_white_level_code_value);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.tone_map_id);
            size += stream.WriteUnsignedInt(1, this.tone_map_cancel_flag);

            if (tone_map_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.tone_map_persistence_flag);
                size += stream.WriteUnsignedInt(8, this.coded_data_bit_depth);
                size += stream.WriteUnsignedInt(8, this.target_bit_depth);
                size += stream.WriteUnsignedIntGolomb(this.tone_map_model_id);

                if (tone_map_model_id == 0)
                {
                    size += stream.WriteUnsignedInt(32, this.min_value);
                    size += stream.WriteUnsignedInt(32, this.max_value);
                }
                else if (tone_map_model_id == 1)
                {
                    size += stream.WriteUnsignedInt(32, this.sigmoid_midpoint);
                    size += stream.WriteUnsignedInt(32, this.sigmoid_width);
                }
                else if (tone_map_model_id == 2)
                {

                    for (i = 0; i < (1 << (int)target_bit_depth); i++)
                    {
                        size += stream.WriteUnsignedIntVariable(start_of_coded_interval[i], this.start_of_coded_interval[i]);
                    }
                }
                else if (tone_map_model_id == 3)
                {
                    size += stream.WriteUnsignedInt(16, this.num_pivots);

                    for (i = 0; i < num_pivots; i++)
                    {
                        size += stream.WriteUnsignedIntVariable(coded_pivot_value[i], this.coded_pivot_value[i]);
                        size += stream.WriteUnsignedIntVariable(target_pivot_value[i], this.target_pivot_value[i]);
                    }
                }
                else if (tone_map_model_id == 4)
                {
                    size += stream.WriteUnsignedInt(8, this.camera_iso_speed_idc);

                    if (camera_iso_speed_idc == H265Constants.EXTENDED_ISO)
                    {
                        size += stream.WriteUnsignedInt(32, this.camera_iso_speed_value);
                    }
                    size += stream.WriteUnsignedInt(8, this.exposure_idx_idc);

                    if (exposure_idx_idc == H265Constants.EXTENDED_ISO)
                    {
                        size += stream.WriteUnsignedInt(32, this.exposure_idx_value);
                    }
                    size += stream.WriteUnsignedInt(1, this.exposure_compensation_value_sign_flag);
                    size += stream.WriteUnsignedInt(16, this.exposure_compensation_value_numerator);
                    size += stream.WriteUnsignedInt(16, this.exposure_compensation_value_denom_idc);
                    size += stream.WriteUnsignedInt(32, this.ref_screen_luminance_white);
                    size += stream.WriteUnsignedInt(32, this.extended_range_white_level);
                    size += stream.WriteUnsignedInt(16, this.nominal_black_level_code_value);
                    size += stream.WriteUnsignedInt(16, this.nominal_white_level_code_value);
                    size += stream.WriteUnsignedInt(16, this.extended_white_level_code_value);
                }
            }

            return size;
        }

    }

    /*
  

frame_packing_arrangement( payloadSize ) {  
 frame_packing_arrangement_id ue(v) 
 frame_packing_arrangement_cancel_flag u(1) 
 if( !frame_packing_arrangement_cancel_flag ) {  
  frame_packing_arrangement_type u(7) 
  quincunx_sampling_flag u(1) 
  content_interpretation_type u(6) 
  spatial_flipping_flag u(1) 
  frame0_flipped_flag u(1) 
  field_views_flag u(1) 
  current_frame_is_frame0_flag u(1) 
  frame0_self_contained_flag u(1) 
  frame1_self_contained_flag u(1) 
  if( !quincunx_sampling_flag  &&  frame_packing_arrangement_type != 5 ) {  
   frame0_grid_position_x u(4) 
   frame0_grid_position_y u(4) 
   frame1_grid_position_x u(4) 
   frame1_grid_position_y u(4) 
  }  
  frame_packing_arrangement_reserved_byte u(8) 
  frame_packing_arrangement_persistence_flag u(1) 
 }  
 upsampled_aspect_ratio_flag u(1) 
}
    */
    public class FramePackingArrangement : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint frame_packing_arrangement_id;
        public uint FramePackingArrangementId { get { return frame_packing_arrangement_id; } set { frame_packing_arrangement_id = value; } }
        private byte frame_packing_arrangement_cancel_flag;
        public byte FramePackingArrangementCancelFlag { get { return frame_packing_arrangement_cancel_flag; } set { frame_packing_arrangement_cancel_flag = value; } }
        private uint frame_packing_arrangement_type;
        public uint FramePackingArrangementType { get { return frame_packing_arrangement_type; } set { frame_packing_arrangement_type = value; } }
        private byte quincunx_sampling_flag;
        public byte QuincunxSamplingFlag { get { return quincunx_sampling_flag; } set { quincunx_sampling_flag = value; } }
        private uint content_interpretation_type;
        public uint ContentInterpretationType { get { return content_interpretation_type; } set { content_interpretation_type = value; } }
        private byte spatial_flipping_flag;
        public byte SpatialFlippingFlag { get { return spatial_flipping_flag; } set { spatial_flipping_flag = value; } }
        private byte frame0_flipped_flag;
        public byte Frame0FlippedFlag { get { return frame0_flipped_flag; } set { frame0_flipped_flag = value; } }
        private byte field_views_flag;
        public byte FieldViewsFlag { get { return field_views_flag; } set { field_views_flag = value; } }
        private byte current_frame_is_frame0_flag;
        public byte CurrentFrameIsFrame0Flag { get { return current_frame_is_frame0_flag; } set { current_frame_is_frame0_flag = value; } }
        private byte frame0_self_contained_flag;
        public byte Frame0SelfContainedFlag { get { return frame0_self_contained_flag; } set { frame0_self_contained_flag = value; } }
        private byte frame1_self_contained_flag;
        public byte Frame1SelfContainedFlag { get { return frame1_self_contained_flag; } set { frame1_self_contained_flag = value; } }
        private uint frame0_grid_position_x;
        public uint Frame0GridPositionx { get { return frame0_grid_position_x; } set { frame0_grid_position_x = value; } }
        private uint frame0_grid_position_y;
        public uint Frame0GridPositiony { get { return frame0_grid_position_y; } set { frame0_grid_position_y = value; } }
        private uint frame1_grid_position_x;
        public uint Frame1GridPositionx { get { return frame1_grid_position_x; } set { frame1_grid_position_x = value; } }
        private uint frame1_grid_position_y;
        public uint Frame1GridPositiony { get { return frame1_grid_position_y; } set { frame1_grid_position_y = value; } }
        private uint frame_packing_arrangement_reserved_byte;
        public uint FramePackingArrangementReservedByte { get { return frame_packing_arrangement_reserved_byte; } set { frame_packing_arrangement_reserved_byte = value; } }
        private byte frame_packing_arrangement_persistence_flag;
        public byte FramePackingArrangementPersistenceFlag { get { return frame_packing_arrangement_persistence_flag; } set { frame_packing_arrangement_persistence_flag = value; } }
        private byte upsampled_aspect_ratio_flag;
        public byte UpsampledAspectRatioFlag { get { return upsampled_aspect_ratio_flag; } set { upsampled_aspect_ratio_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FramePackingArrangement(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.frame_packing_arrangement_id);
            size += stream.ReadUnsignedInt(size, 1, out this.frame_packing_arrangement_cancel_flag);

            if (frame_packing_arrangement_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 7, out this.frame_packing_arrangement_type);
                size += stream.ReadUnsignedInt(size, 1, out this.quincunx_sampling_flag);
                size += stream.ReadUnsignedInt(size, 6, out this.content_interpretation_type);
                size += stream.ReadUnsignedInt(size, 1, out this.spatial_flipping_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.frame0_flipped_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.field_views_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.current_frame_is_frame0_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.frame0_self_contained_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.frame1_self_contained_flag);

                if (quincunx_sampling_flag == 0 && frame_packing_arrangement_type != 5)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.frame0_grid_position_x);
                    size += stream.ReadUnsignedInt(size, 4, out this.frame0_grid_position_y);
                    size += stream.ReadUnsignedInt(size, 4, out this.frame1_grid_position_x);
                    size += stream.ReadUnsignedInt(size, 4, out this.frame1_grid_position_y);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.frame_packing_arrangement_reserved_byte);
                size += stream.ReadUnsignedInt(size, 1, out this.frame_packing_arrangement_persistence_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.upsampled_aspect_ratio_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.frame_packing_arrangement_id);
            size += stream.WriteUnsignedInt(1, this.frame_packing_arrangement_cancel_flag);

            if (frame_packing_arrangement_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(7, this.frame_packing_arrangement_type);
                size += stream.WriteUnsignedInt(1, this.quincunx_sampling_flag);
                size += stream.WriteUnsignedInt(6, this.content_interpretation_type);
                size += stream.WriteUnsignedInt(1, this.spatial_flipping_flag);
                size += stream.WriteUnsignedInt(1, this.frame0_flipped_flag);
                size += stream.WriteUnsignedInt(1, this.field_views_flag);
                size += stream.WriteUnsignedInt(1, this.current_frame_is_frame0_flag);
                size += stream.WriteUnsignedInt(1, this.frame0_self_contained_flag);
                size += stream.WriteUnsignedInt(1, this.frame1_self_contained_flag);

                if (quincunx_sampling_flag == 0 && frame_packing_arrangement_type != 5)
                {
                    size += stream.WriteUnsignedInt(4, this.frame0_grid_position_x);
                    size += stream.WriteUnsignedInt(4, this.frame0_grid_position_y);
                    size += stream.WriteUnsignedInt(4, this.frame1_grid_position_x);
                    size += stream.WriteUnsignedInt(4, this.frame1_grid_position_y);
                }
                size += stream.WriteUnsignedInt(8, this.frame_packing_arrangement_reserved_byte);
                size += stream.WriteUnsignedInt(1, this.frame_packing_arrangement_persistence_flag);
            }
            size += stream.WriteUnsignedInt(1, this.upsampled_aspect_ratio_flag);

            return size;
        }

    }

    /*
  

display_orientation( payloadSize ) {  
 display_orientation_cancel_flag u(1) 
 if( !display_orientation_cancel_flag ) {  
  hor_flip u(1) 
  ver_flip u(1) 
  anticlockwise_rotation u(16) 
  display_orientation_persistence_flag u(1) 
 }  
}
    */
    public class DisplayOrientation : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte display_orientation_cancel_flag;
        public byte DisplayOrientationCancelFlag { get { return display_orientation_cancel_flag; } set { display_orientation_cancel_flag = value; } }
        private byte hor_flip;
        public byte HorFlip { get { return hor_flip; } set { hor_flip = value; } }
        private byte ver_flip;
        public byte VerFlip { get { return ver_flip; } set { ver_flip = value; } }
        private uint anticlockwise_rotation;
        public uint AnticlockwiseRotation { get { return anticlockwise_rotation; } set { anticlockwise_rotation = value; } }
        private byte display_orientation_persistence_flag;
        public byte DisplayOrientationPersistenceFlag { get { return display_orientation_persistence_flag; } set { display_orientation_persistence_flag = value; } }

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
                size += stream.ReadUnsignedInt(size, 1, out this.hor_flip);
                size += stream.ReadUnsignedInt(size, 1, out this.ver_flip);
                size += stream.ReadUnsignedInt(size, 16, out this.anticlockwise_rotation);
                size += stream.ReadUnsignedInt(size, 1, out this.display_orientation_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.display_orientation_cancel_flag);

            if (display_orientation_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.hor_flip);
                size += stream.WriteUnsignedInt(1, this.ver_flip);
                size += stream.WriteUnsignedInt(16, this.anticlockwise_rotation);
                size += stream.WriteUnsignedInt(1, this.display_orientation_persistence_flag);
            }

            return size;
        }

    }

    /*
 

structure_of_pictures_info( payloadSize ) {  
 sop_seq_parameter_set_id ue(v) 
 num_entries_in_sop_minus1 ue(v) 
 for( i = 0; i <= num_entries_in_sop_minus1; i++ ) {  
  sop_vcl_nut[ i ] u(6) 
  sop_temporal_id[ i ] u(3) 
  if( sop_vcl_nut[ i ] != IDR_W_RADL  &&  sop_vcl_nut[ i ] != IDR_N_LP )  
   sop_short_term_rps_idx[ i ] ue(v) 
  if( i > 0 )  
   sop_poc_delta[ i ] se(v) 
 }  
}
    */
    public class StructureOfPicturesInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sop_seq_parameter_set_id;
        public uint SopSeqParameterSetId { get { return sop_seq_parameter_set_id; } set { sop_seq_parameter_set_id = value; } }
        private uint num_entries_in_sop_minus1;
        public uint NumEntriesInSopMinus1 { get { return num_entries_in_sop_minus1; } set { num_entries_in_sop_minus1 = value; } }
        private uint[] sop_vcl_nut;
        public uint[] SopVclNut { get { return sop_vcl_nut; } set { sop_vcl_nut = value; } }
        private uint[] sop_temporal_id;
        public uint[] SopTemporalId { get { return sop_temporal_id; } set { sop_temporal_id = value; } }
        private uint[] sop_short_term_rps_idx;
        public uint[] SopShortTermRpsIdx { get { return sop_short_term_rps_idx; } set { sop_short_term_rps_idx = value; } }
        private int[] sop_poc_delta;
        public int[] SopPocDelta { get { return sop_poc_delta; } set { sop_poc_delta = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public StructureOfPicturesInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.sop_seq_parameter_set_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_entries_in_sop_minus1);

            this.sop_vcl_nut = new uint[num_entries_in_sop_minus1 + 1];
            this.sop_temporal_id = new uint[num_entries_in_sop_minus1 + 1];
            this.sop_short_term_rps_idx = new uint[num_entries_in_sop_minus1 + 1];
            this.sop_poc_delta = new int[num_entries_in_sop_minus1 + 1];
            for (i = 0; i <= num_entries_in_sop_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.sop_vcl_nut[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.sop_temporal_id[i]);

                if (sop_vcl_nut[i] != H265NALTypes.IDR_W_RADL && sop_vcl_nut[i] != H265NALTypes.IDR_N_LP)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sop_short_term_rps_idx[i]);
                }

                if (i > 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.sop_poc_delta[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.sop_seq_parameter_set_id);
            size += stream.WriteUnsignedIntGolomb(this.num_entries_in_sop_minus1);

            for (i = 0; i <= num_entries_in_sop_minus1; i++)
            {
                size += stream.WriteUnsignedInt(6, this.sop_vcl_nut[i]);
                size += stream.WriteUnsignedInt(3, this.sop_temporal_id[i]);

                if (sop_vcl_nut[i] != H265NALTypes.IDR_W_RADL && sop_vcl_nut[i] != H265NALTypes.IDR_N_LP)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sop_short_term_rps_idx[i]);
                }

                if (i > 0)
                {
                    size += stream.WriteSignedIntGolomb(this.sop_poc_delta[i]);
                }
            }

            return size;
        }

    }

    /*
 

decoded_picture_hash( payloadSize ) {  
 hash_type u(8) 
 for( cIdx = 0; cIdx < ( chroma_format_idc == 0 ? 1 : 3 ); cIdx++ )  
  if( hash_type == 0 )  
   for( i = 0; i < 16; i++)  
    picture_md5[ cIdx ][ i ] b(8) 
  else if( hash_type == 1 )  
   picture_crc[ cIdx ] u(16) 
  else if( hash_type == 2 )  
   picture_checksum[ cIdx ] u(32) 
}
    */
    public class DecodedPictureHash : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint hash_type;
        public uint HashType { get { return hash_type; } set { hash_type = value; } }
        private byte[][] picture_md5;
        public byte[][] PictureMd5 { get { return picture_md5; } set { picture_md5 = value; } }
        private uint[] picture_crc;
        public uint[] PictureCrc { get { return picture_crc; } set { picture_crc = value; } }
        private uint[] picture_checksum;
        public uint[] PictureChecksum { get { return picture_checksum; } set { picture_checksum = value; } }

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
            size += stream.ReadUnsignedInt(size, 8, out this.hash_type);

            this.picture_md5 = new byte[(((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3)][];
            this.picture_crc = new uint[(((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3)];
            this.picture_checksum = new uint[(((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3)];
            for (cIdx = 0; cIdx < (((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3); cIdx++)
            {

                if (hash_type == 0)
                {

                    this.picture_md5[cIdx] = new byte[16];
                    for (i = 0; i < 16; i++)
                    {
                        size += stream.ReadBits(size, 8, out this.picture_md5[cIdx][i]);
                    }
                }
                else if (hash_type == 1)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.picture_crc[cIdx]);
                }
                else if (hash_type == 2)
                {
                    size += stream.ReadUnsignedInt(size, 32, out this.picture_checksum[cIdx]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint cIdx = 0;
            uint i = 0;
            size += stream.WriteUnsignedInt(8, this.hash_type);

            for (cIdx = 0; cIdx < (((H265Context)context).SeqParameterSetRbsp.ChromaFormatIdc == 0 ? 1 : 3); cIdx++)
            {

                if (hash_type == 0)
                {

                    for (i = 0; i < 16; i++)
                    {
                        size += stream.WriteBits(8, this.picture_md5[cIdx][i]);
                    }
                }
                else if (hash_type == 1)
                {
                    size += stream.WriteUnsignedInt(16, this.picture_crc[cIdx]);
                }
                else if (hash_type == 2)
                {
                    size += stream.WriteUnsignedInt(32, this.picture_checksum[cIdx]);
                }
            }

            return size;
        }

    }

    /*
 

active_parameter_sets( payloadSize ) {  
 active_video_parameter_set_id u(4) 
 self_contained_cvs_flag u(1) 
 no_parameter_set_update_flag u(1) 
 num_sps_ids_minus1 ue(v) 
 for( i = 0; i <= num_sps_ids_minus1; i++ )  
  active_seq_parameter_set_id[ i ] ue(v) 
 for( i = vps_base_layer_internal_flag; i <= MaxLayersMinus1; i++ )  
  layer_sps_idx[ i ] ue(v) 
}
    */
    public class ActiveParameterSets : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint active_video_parameter_set_id;
        public uint ActiveVideoParameterSetId { get { return active_video_parameter_set_id; } set { active_video_parameter_set_id = value; } }
        private byte self_contained_cvs_flag;
        public byte SelfContainedCvsFlag { get { return self_contained_cvs_flag; } set { self_contained_cvs_flag = value; } }
        private byte no_parameter_set_update_flag;
        public byte NoParameterSetUpdateFlag { get { return no_parameter_set_update_flag; } set { no_parameter_set_update_flag = value; } }
        private uint num_sps_ids_minus1;
        public uint NumSpsIdsMinus1 { get { return num_sps_ids_minus1; } set { num_sps_ids_minus1 = value; } }
        private uint[] active_seq_parameter_set_id;
        public uint[] ActiveSeqParameterSetId { get { return active_seq_parameter_set_id; } set { active_seq_parameter_set_id = value; } }
        private uint[] layer_sps_idx;
        public uint[] LayerSpsIdx { get { return layer_sps_idx; } set { layer_sps_idx = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ActiveParameterSets(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 4, out this.active_video_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.self_contained_cvs_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.no_parameter_set_update_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_sps_ids_minus1);

            this.active_seq_parameter_set_id = new uint[num_sps_ids_minus1 + 1];
            for (i = 0; i <= num_sps_ids_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.active_seq_parameter_set_id[i]);
            }

            this.layer_sps_idx = new uint[MaxLayersMinus1 + 1];
            for (i = vps_base_layer_internal_flag; i <= MaxLayersMinus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.layer_sps_idx[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(4, this.active_video_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.self_contained_cvs_flag);
            size += stream.WriteUnsignedInt(1, this.no_parameter_set_update_flag);
            size += stream.WriteUnsignedIntGolomb(this.num_sps_ids_minus1);

            for (i = 0; i <= num_sps_ids_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.active_seq_parameter_set_id[i]);
            }

            for (i = vps_base_layer_internal_flag; i <= MaxLayersMinus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.layer_sps_idx[i]);
            }

            return size;
        }

    }

    /*
 

decoding_unit_info( payloadSize ) {  
 decoding_unit_idx ue(v) 
 if( !sub_pic_cpb_params_in_pic_timing_sei_flag )  
  du_spt_cpb_removal_delay_increment u(v) 
 dpb_output_du_delay_present_flag u(1) 
 if( dpb_output_du_delay_present_flag )  
  pic_spt_dpb_output_du_delay u(v) 
}
    */
    public class DecodingUnitInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint decoding_unit_idx;
        public uint DecodingUnitIdx { get { return decoding_unit_idx; } set { decoding_unit_idx = value; } }
        private uint du_spt_cpb_removal_delay_increment;
        public uint DuSptCpbRemovalDelayIncrement { get { return du_spt_cpb_removal_delay_increment; } set { du_spt_cpb_removal_delay_increment = value; } }
        private byte dpb_output_du_delay_present_flag;
        public byte DpbOutputDuDelayPresentFlag { get { return dpb_output_du_delay_present_flag; } set { dpb_output_du_delay_present_flag = value; } }
        private uint pic_spt_dpb_output_du_delay;
        public uint PicSptDpbOutputDuDelay { get { return pic_spt_dpb_output_du_delay; } set { pic_spt_dpb_output_du_delay = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DecodingUnitInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.decoding_unit_idx);

            if (sub_pic_cpb_params_in_pic_timing_sei_flag == 0)
            {
                size += stream.ReadUnsignedIntVariable(size, du_spt_cpb_removal_delay_increment, out this.du_spt_cpb_removal_delay_increment);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.dpb_output_du_delay_present_flag);

            if (dpb_output_du_delay_present_flag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, pic_spt_dpb_output_du_delay, out this.pic_spt_dpb_output_du_delay);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.decoding_unit_idx);

            if (sub_pic_cpb_params_in_pic_timing_sei_flag == 0)
            {
                size += stream.WriteUnsignedIntVariable(du_spt_cpb_removal_delay_increment, this.du_spt_cpb_removal_delay_increment);
            }
            size += stream.WriteUnsignedInt(1, this.dpb_output_du_delay_present_flag);

            if (dpb_output_du_delay_present_flag != 0)
            {
                size += stream.WriteUnsignedIntVariable(pic_spt_dpb_output_du_delay, this.pic_spt_dpb_output_du_delay);
            }

            return size;
        }

    }

    /*
  

temporal_sub_layer_zero_idx( payloadSize ) {  
 temporal_sub_layer_zero_idx u(8) 
 irap_pic_id u(8) 
}
    */
    public class TemporalSubLayerZeroIdx : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint temporal_sub_layer_zero_idx;
        public uint _TemporalSubLayerZeroIdx { get { return temporal_sub_layer_zero_idx; } set { temporal_sub_layer_zero_idx = value; } }
        private uint irap_pic_id;
        public uint IrapPicId { get { return irap_pic_id; } set { irap_pic_id = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public TemporalSubLayerZeroIdx(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.temporal_sub_layer_zero_idx);
            size += stream.ReadUnsignedInt(size, 8, out this.irap_pic_id);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.temporal_sub_layer_zero_idx);
            size += stream.WriteUnsignedInt(8, this.irap_pic_id);

            return size;
        }

    }

    /*
  

scalable_nesting( payloadSize ) {  
 bitstream_subset_flag u(1) 
 nesting_op_flag u(1) 
 if( nesting_op_flag ) {  
  default_op_flag u(1) 
  nesting_num_ops_minus1 ue(v) 
  for( i = default_op_flag; i <= nesting_num_ops_minus1; i++ ) {  
   nesting_max_temporal_id_plus1[ i ] u(3) 
   nesting_op_idx[ i ] ue(v) 
  }  
 } else {  
  all_layers_flag u(1) 
  if( !all_layers_flag ) {  
   nesting_no_op_max_temporal_id_plus1 u(3) 
   nesting_num_layers_minus1 ue(v) 
   for( i = 0; i <= nesting_num_layers_minus1; i++ )  
    nesting_layer_id[ i ] u(6) 
  }  
 }  
 while( !byte_aligned() )  
  nesting_zero_bit /* equal to 0 *//* u(1) 
 do  
  sei_message()  
 while( more_rbsp_data() )  
}
    */
    public class ScalableNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte bitstream_subset_flag;
        public byte BitstreamSubsetFlag { get { return bitstream_subset_flag; } set { bitstream_subset_flag = value; } }
        private byte nesting_op_flag;
        public byte NestingOpFlag { get { return nesting_op_flag; } set { nesting_op_flag = value; } }
        private byte default_op_flag;
        public byte DefaultOpFlag { get { return default_op_flag; } set { default_op_flag = value; } }
        private uint nesting_num_ops_minus1;
        public uint NestingNumOpsMinus1 { get { return nesting_num_ops_minus1; } set { nesting_num_ops_minus1 = value; } }
        private uint[] nesting_max_temporal_id_plus1;
        public uint[] NestingMaxTemporalIdPlus1 { get { return nesting_max_temporal_id_plus1; } set { nesting_max_temporal_id_plus1 = value; } }
        private uint[] nesting_op_idx;
        public uint[] NestingOpIdx { get { return nesting_op_idx; } set { nesting_op_idx = value; } }
        private byte all_layers_flag;
        public byte AllLayersFlag { get { return all_layers_flag; } set { all_layers_flag = value; } }
        private uint nesting_no_op_max_temporal_id_plus1;
        public uint NestingNoOpMaxTemporalIdPlus1 { get { return nesting_no_op_max_temporal_id_plus1; } set { nesting_no_op_max_temporal_id_plus1 = value; } }
        private uint nesting_num_layers_minus1;
        public uint NestingNumLayersMinus1 { get { return nesting_num_layers_minus1; } set { nesting_num_layers_minus1 = value; } }
        private uint[] nesting_layer_id;
        public uint[] NestingLayerId { get { return nesting_layer_id; } set { nesting_layer_id = value; } }
        private Dictionary<int, byte> nesting_zero_bit = new Dictionary<int, byte>();
        public Dictionary<int, byte> NestingZeroBit { get { return nesting_zero_bit; } set { nesting_zero_bit = value; } }
        private Dictionary<int, SeiMessage> sei_message = new Dictionary<int, SeiMessage>();
        public Dictionary<int, SeiMessage> SeiMessage { get { return sei_message; } set { sei_message = value; } }

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
            size += stream.ReadUnsignedInt(size, 1, out this.bitstream_subset_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.nesting_op_flag);

            if (nesting_op_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.default_op_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.nesting_num_ops_minus1);

                this.nesting_max_temporal_id_plus1 = new uint[nesting_num_ops_minus1 + 1];
                this.nesting_op_idx = new uint[nesting_num_ops_minus1 + 1];
                for (i = default_op_flag; i <= nesting_num_ops_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.nesting_max_temporal_id_plus1[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.nesting_op_idx[i]);
                }
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.all_layers_flag);

                if (all_layers_flag == 0)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.nesting_no_op_max_temporal_id_plus1);
                    size += stream.ReadUnsignedIntGolomb(size, out this.nesting_num_layers_minus1);

                    this.nesting_layer_id = new uint[nesting_num_layers_minus1 + 1];
                    for (i = 0; i <= nesting_num_layers_minus1; i++)
                    {
                        size += stream.ReadUnsignedInt(size, 6, out this.nesting_layer_id[i]);
                    }
                }
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 1, whileIndex, this.nesting_zero_bit); // equal to 0 
            }

            do
            {
                whileIndex++;

                this.sei_message.Add(whileIndex, new SeiMessage());
                size += stream.ReadClass<SeiMessage>(size, context, this.sei_message[whileIndex]);
            } while (stream.ReadMoreRbspData(this));

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(1, this.bitstream_subset_flag);
            size += stream.WriteUnsignedInt(1, this.nesting_op_flag);

            if (nesting_op_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.default_op_flag);
                size += stream.WriteUnsignedIntGolomb(this.nesting_num_ops_minus1);

                for (i = default_op_flag; i <= nesting_num_ops_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(3, this.nesting_max_temporal_id_plus1[i]);
                    size += stream.WriteUnsignedIntGolomb(this.nesting_op_idx[i]);
                }
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.all_layers_flag);

                if (all_layers_flag == 0)
                {
                    size += stream.WriteUnsignedInt(3, this.nesting_no_op_max_temporal_id_plus1);
                    size += stream.WriteUnsignedIntGolomb(this.nesting_num_layers_minus1);

                    for (i = 0; i <= nesting_num_layers_minus1; i++)
                    {
                        size += stream.WriteUnsignedInt(6, this.nesting_layer_id[i]);
                    }
                }
            }

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(1, whileIndex, this.nesting_zero_bit); // equal to 0 
            }

            do
            {
                whileIndex++;

                size += stream.WriteClass<SeiMessage>(context, whileIndex, this.sei_message);
            } while (stream.WriteMoreRbspData(this));

            return size;
        }

    }

    /*
 

region_refresh_info( payloadSize ) {  
 refreshed_region_flag u(1) 
}
    */
    public class RegionRefreshInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte refreshed_region_flag;
        public byte RefreshedRegionFlag { get { return refreshed_region_flag; } set { refreshed_region_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RegionRefreshInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.refreshed_region_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.refreshed_region_flag);

            return size;
        }

    }

    /*


no_display( payloadSize ) {  
}
    */
    public class NoDisplay : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public NoDisplay(uint payloadSize)
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


time_code( payloadSize ) {  
 num_clock_ts u(2) 
 for( i = 0; i < num_clock_ts; i++ ) {  
  clock_timestamp_flag[ i ] u(1) 
  if( clock_timestamp_flag[ i ] ) {  
   units_field_based_flag[ i ] u(1) 
   counting_type[ i ] u(5) 
   full_timestamp_flag[ i ] u(1) 
   discontinuity_flag[ i ] u(1) 
   cnt_dropped_flag[ i ] u(1) 
   n_frames[ i ] u(9) 
   if( full_timestamp_flag[ i ] ) {  
    seconds_value[ i ] u(6) /* 0..59 *//*
    minutes_value[ i ] u(6) /* 0..59 *//*
    hours_value[ i ] u(5) /* 0..23 *//*
   } else {  
    seconds_flag[ i ] u(1) 
    if( seconds_flag[ i ] ) {  
     seconds_value[ i ] u(6) /* 0..59 *//*
     minutes_flag[ i ] u(1) 
     if( minutes_flag[ i ] ) {  
      minutes_value[ i ]  u(6) /* 0..59 *//*
      hours_flag[ i ] u(1) 
      if( hours_flag[ i ] )  
       hours_value[ i ]  u(5) /* 0..23 *//*
     }  
    }  
   }  
   time_offset_length[ i ] u(5) 
   if( time_offset_length[ i ] > 0 )  
    time_offset_value[ i ] i(v) 
  }  
 }  
}
    */
    public class TimeCode : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_clock_ts;
        public uint NumClockTs { get { return num_clock_ts; } set { num_clock_ts = value; } }
        private byte[] clock_timestamp_flag;
        public byte[] ClockTimestampFlag { get { return clock_timestamp_flag; } set { clock_timestamp_flag = value; } }
        private byte[] units_field_based_flag;
        public byte[] UnitsFieldBasedFlag { get { return units_field_based_flag; } set { units_field_based_flag = value; } }
        private uint[] counting_type;
        public uint[] CountingType { get { return counting_type; } set { counting_type = value; } }
        private byte[] full_timestamp_flag;
        public byte[] FullTimestampFlag { get { return full_timestamp_flag; } set { full_timestamp_flag = value; } }
        private byte[] discontinuity_flag;
        public byte[] DiscontinuityFlag { get { return discontinuity_flag; } set { discontinuity_flag = value; } }
        private byte[] cnt_dropped_flag;
        public byte[] CntDroppedFlag { get { return cnt_dropped_flag; } set { cnt_dropped_flag = value; } }
        private uint[] n_frames;
        public uint[] nFrames { get { return n_frames; } set { n_frames = value; } }
        private uint[] seconds_value;
        public uint[] SecondsValue { get { return seconds_value; } set { seconds_value = value; } }
        private uint[] minutes_value;
        public uint[] MinutesValue { get { return minutes_value; } set { minutes_value = value; } }
        private uint[] hours_value;
        public uint[] HoursValue { get { return hours_value; } set { hours_value = value; } }
        private byte[] seconds_flag;
        public byte[] SecondsFlag { get { return seconds_flag; } set { seconds_flag = value; } }
        private byte[] minutes_flag;
        public byte[] MinutesFlag { get { return minutes_flag; } set { minutes_flag = value; } }
        private byte[] hours_flag;
        public byte[] HoursFlag { get { return hours_flag; } set { hours_flag = value; } }
        private uint[] time_offset_length;
        public uint[] TimeOffsetLength { get { return time_offset_length; } set { time_offset_length = value; } }
        private int[] time_offset_value;
        public int[] TimeOffsetValue { get { return time_offset_value; } set { time_offset_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public TimeCode(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 2, out this.num_clock_ts);

            this.clock_timestamp_flag = new byte[num_clock_ts];
            this.units_field_based_flag = new byte[num_clock_ts];
            this.counting_type = new uint[num_clock_ts];
            this.full_timestamp_flag = new byte[num_clock_ts];
            this.discontinuity_flag = new byte[num_clock_ts];
            this.cnt_dropped_flag = new byte[num_clock_ts];
            this.n_frames = new uint[num_clock_ts];
            this.seconds_value = new uint[num_clock_ts];
            this.minutes_value = new uint[num_clock_ts];
            this.hours_value = new uint[num_clock_ts];
            this.seconds_flag = new byte[num_clock_ts];
            this.minutes_flag = new byte[num_clock_ts];
            this.hours_flag = new byte[num_clock_ts];
            this.time_offset_length = new uint[num_clock_ts];
            this.time_offset_value = new int[num_clock_ts];
            for (i = 0; i < num_clock_ts; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.clock_timestamp_flag[i]);

                if (clock_timestamp_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.units_field_based_flag[i]);
                    size += stream.ReadUnsignedInt(size, 5, out this.counting_type[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.full_timestamp_flag[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.discontinuity_flag[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.cnt_dropped_flag[i]);
                    size += stream.ReadUnsignedInt(size, 9, out this.n_frames[i]);

                    if (full_timestamp_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 6, out this.seconds_value[i]);
                        /*  0..59  */

                        size += stream.ReadUnsignedInt(size, 6, out this.minutes_value[i]);
                        /*  0..59  */

                        size += stream.ReadUnsignedInt(size, 5, out this.hours_value[i]);
                        /*  0..23  */

                    }
                    else
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.seconds_flag[i]);

                        if (seconds_flag[i] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 6, out this.seconds_value[i]);
                            /*  0..59  */

                            size += stream.ReadUnsignedInt(size, 1, out this.minutes_flag[i]);

                            if (minutes_flag[i] != 0)
                            {
                                size += stream.ReadUnsignedInt(size, 6, out this.minutes_value[i]);
                                /*  0..59  */

                                size += stream.ReadUnsignedInt(size, 1, out this.hours_flag[i]);

                                if (hours_flag[i] != 0)
                                {
                                    size += stream.ReadUnsignedInt(size, 5, out this.hours_value[i]);
                                }
                                /*  0..23  */

                            }
                        }
                    }
                    size += stream.ReadUnsignedInt(size, 5, out this.time_offset_length[i]);

                    if (time_offset_length[i] > 0)
                    {
                        size += stream.ReadSignedIntVariable(size, time_offset_value, out this.time_offset_value[i]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(2, this.num_clock_ts);

            for (i = 0; i < num_clock_ts; i++)
            {
                size += stream.WriteUnsignedInt(1, this.clock_timestamp_flag[i]);

                if (clock_timestamp_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.units_field_based_flag[i]);
                    size += stream.WriteUnsignedInt(5, this.counting_type[i]);
                    size += stream.WriteUnsignedInt(1, this.full_timestamp_flag[i]);
                    size += stream.WriteUnsignedInt(1, this.discontinuity_flag[i]);
                    size += stream.WriteUnsignedInt(1, this.cnt_dropped_flag[i]);
                    size += stream.WriteUnsignedInt(9, this.n_frames[i]);

                    if (full_timestamp_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(6, this.seconds_value[i]);
                        /*  0..59  */

                        size += stream.WriteUnsignedInt(6, this.minutes_value[i]);
                        /*  0..59  */

                        size += stream.WriteUnsignedInt(5, this.hours_value[i]);
                        /*  0..23  */

                    }
                    else
                    {
                        size += stream.WriteUnsignedInt(1, this.seconds_flag[i]);

                        if (seconds_flag[i] != 0)
                        {
                            size += stream.WriteUnsignedInt(6, this.seconds_value[i]);
                            /*  0..59  */

                            size += stream.WriteUnsignedInt(1, this.minutes_flag[i]);

                            if (minutes_flag[i] != 0)
                            {
                                size += stream.WriteUnsignedInt(6, this.minutes_value[i]);
                                /*  0..59  */

                                size += stream.WriteUnsignedInt(1, this.hours_flag[i]);

                                if (hours_flag[i] != 0)
                                {
                                    size += stream.WriteUnsignedInt(5, this.hours_value[i]);
                                }
                                /*  0..23  */

                            }
                        }
                    }
                    size += stream.WriteUnsignedInt(5, this.time_offset_length[i]);

                    if (time_offset_length[i] > 0)
                    {
                        size += stream.WriteSignedIntVariable(time_offset_value[i], this.time_offset_value[i]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

mastering_display_colour_volume( payloadSize ) { 
for( c = 0; c < 3; c++ ) {  
display_primaries_x[ c ] u(16) 
display_primaries_y[ c ] u(16) 
}  
white_point_x u(16) 
white_point_y u(16) 
max_display_mastering_luminance u(32) 
min_display_mastering_luminance u(32) 
}
    */
    public class MasteringDisplayColourVolume : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] display_primaries_x;
        public uint[] DisplayPrimariesx { get { return display_primaries_x; } set { display_primaries_x = value; } }
        private uint[] display_primaries_y;
        public uint[] DisplayPrimariesy { get { return display_primaries_y; } set { display_primaries_y = value; } }
        private uint white_point_x;
        public uint WhitePointx { get { return white_point_x; } set { white_point_x = value; } }
        private uint white_point_y;
        public uint WhitePointy { get { return white_point_y; } set { white_point_y = value; } }
        private uint max_display_mastering_luminance;
        public uint MaxDisplayMasteringLuminance { get { return max_display_mastering_luminance; } set { max_display_mastering_luminance = value; } }
        private uint min_display_mastering_luminance;
        public uint MinDisplayMasteringLuminance { get { return min_display_mastering_luminance; } set { min_display_mastering_luminance = value; } }

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

            this.display_primaries_x = new uint[3];
            this.display_primaries_y = new uint[3];
            for (c = 0; c < 3; c++)
            {
                size += stream.ReadUnsignedInt(size, 16, out this.display_primaries_x[c]);
                size += stream.ReadUnsignedInt(size, 16, out this.display_primaries_y[c]);
            }
            size += stream.ReadUnsignedInt(size, 16, out this.white_point_x);
            size += stream.ReadUnsignedInt(size, 16, out this.white_point_y);
            size += stream.ReadUnsignedInt(size, 32, out this.max_display_mastering_luminance);
            size += stream.ReadUnsignedInt(size, 32, out this.min_display_mastering_luminance);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;

            for (c = 0; c < 3; c++)
            {
                size += stream.WriteUnsignedInt(16, this.display_primaries_x[c]);
                size += stream.WriteUnsignedInt(16, this.display_primaries_y[c]);
            }
            size += stream.WriteUnsignedInt(16, this.white_point_x);
            size += stream.WriteUnsignedInt(16, this.white_point_y);
            size += stream.WriteUnsignedInt(32, this.max_display_mastering_luminance);
            size += stream.WriteUnsignedInt(32, this.min_display_mastering_luminance);

            return size;
        }

    }

    /*


segmented_rect_frame_packing_arrangement( payloadSize ) { 
segmented_rect_frame_packing_arrangement_cancel_flag  u(1) 
if( !segmented_rect_frame_packing_arrangement_cancel_flag ) {  
segmented_rect_content_interpretation_type u(2) 
segmented_rect_frame_packing_arrangement_persistence_flag u(1) 
}  
}
    */
    public class SegmentedRectFramePackingArrangement : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte segmented_rect_frame_packing_arrangement_cancel_flag;
        public byte SegmentedRectFramePackingArrangementCancelFlag { get { return segmented_rect_frame_packing_arrangement_cancel_flag; } set { segmented_rect_frame_packing_arrangement_cancel_flag = value; } }
        private uint segmented_rect_content_interpretation_type;
        public uint SegmentedRectContentInterpretationType { get { return segmented_rect_content_interpretation_type; } set { segmented_rect_content_interpretation_type = value; } }
        private byte segmented_rect_frame_packing_arrangement_persistence_flag;
        public byte SegmentedRectFramePackingArrangementPersistenceFlag { get { return segmented_rect_frame_packing_arrangement_persistence_flag; } set { segmented_rect_frame_packing_arrangement_persistence_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SegmentedRectFramePackingArrangement(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.segmented_rect_frame_packing_arrangement_cancel_flag);

            if (segmented_rect_frame_packing_arrangement_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.segmented_rect_content_interpretation_type);
                size += stream.ReadUnsignedInt(size, 1, out this.segmented_rect_frame_packing_arrangement_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.segmented_rect_frame_packing_arrangement_cancel_flag);

            if (segmented_rect_frame_packing_arrangement_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(2, this.segmented_rect_content_interpretation_type);
                size += stream.WriteUnsignedInt(1, this.segmented_rect_frame_packing_arrangement_persistence_flag);
            }

            return size;
        }

    }

    /*


temporal_motion_constrained_tile_sets( payloadSize ) {  
 mc_all_tiles_exact_sample_value_match_flag u(1) 
 each_tile_one_tile_set_flag u(1) 
 if( !each_tile_one_tile_set_flag ) {  
  limited_tile_set_display_flag u(1) 
  num_sets_in_message_minus1 ue(v) 
  for( i = 0; i <= num_sets_in_message_minus1; i++ ) {  
   mcts_id[ i ] ue(v) 
   if( limited_tile_set_display_flag )  
    display_tile_set_flag[ i ] u(1) 
   num_tile_rects_in_set_minus1[ i ] ue(v) 
   for( j = 0; j <= num_tile_rects_in_set_minus1[ i ]; j++ ) {  
    top_left_tile_idx[ i ][ j ] ue(v) 
    bottom_right_tile_idx[ i ][ j ] ue(v) 
   }  
   if( !mc_all_tiles_exact_sample_value_match_flag )  
    mc_exact_sample_value_match_flag[ i ] u(1) 
   mcts_tier_level_idc_present_flag[ i ] u(1) 
   if( mcts_tier_level_idc_present_flag[ i ] ) {  
    mcts_tier_flag[ i ] u(1) 
    mcts_level_idc[ i ] u(8) 
   }  
  }  
 } else {  
  max_mcs_tier_level_idc_present_flag u(1) 
  if( mcts_max_tier_level_idc_present_flag ) {  
   mcts_max_tier_flag u(1) 
   mcts_max_level_idc u(8) 
  }  
 }  
}
    */
    public class TemporalMotionConstrainedTileSets : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte mc_all_tiles_exact_sample_value_match_flag;
        public byte McAllTilesExactSampleValueMatchFlag { get { return mc_all_tiles_exact_sample_value_match_flag; } set { mc_all_tiles_exact_sample_value_match_flag = value; } }
        private byte each_tile_one_tile_set_flag;
        public byte EachTileOneTileSetFlag { get { return each_tile_one_tile_set_flag; } set { each_tile_one_tile_set_flag = value; } }
        private byte limited_tile_set_display_flag;
        public byte LimitedTileSetDisplayFlag { get { return limited_tile_set_display_flag; } set { limited_tile_set_display_flag = value; } }
        private uint num_sets_in_message_minus1;
        public uint NumSetsInMessageMinus1 { get { return num_sets_in_message_minus1; } set { num_sets_in_message_minus1 = value; } }
        private uint[] mcts_id;
        public uint[] MctsId { get { return mcts_id; } set { mcts_id = value; } }
        private byte[] display_tile_set_flag;
        public byte[] DisplayTileSetFlag { get { return display_tile_set_flag; } set { display_tile_set_flag = value; } }
        private uint[] num_tile_rects_in_set_minus1;
        public uint[] NumTileRectsInSetMinus1 { get { return num_tile_rects_in_set_minus1; } set { num_tile_rects_in_set_minus1 = value; } }
        private uint[][] top_left_tile_idx;
        public uint[][] TopLeftTileIdx { get { return top_left_tile_idx; } set { top_left_tile_idx = value; } }
        private uint[][] bottom_right_tile_idx;
        public uint[][] BottomRightTileIdx { get { return bottom_right_tile_idx; } set { bottom_right_tile_idx = value; } }
        private byte[] mc_exact_sample_value_match_flag;
        public byte[] McExactSampleValueMatchFlag { get { return mc_exact_sample_value_match_flag; } set { mc_exact_sample_value_match_flag = value; } }
        private byte[] mcts_tier_level_idc_present_flag;
        public byte[] MctsTierLevelIdcPresentFlag { get { return mcts_tier_level_idc_present_flag; } set { mcts_tier_level_idc_present_flag = value; } }
        private byte[] mcts_tier_flag;
        public byte[] MctsTierFlag { get { return mcts_tier_flag; } set { mcts_tier_flag = value; } }
        private uint[] mcts_level_idc;
        public uint[] MctsLevelIdc { get { return mcts_level_idc; } set { mcts_level_idc = value; } }
        private byte max_mcs_tier_level_idc_present_flag;
        public byte MaxMcsTierLevelIdcPresentFlag { get { return max_mcs_tier_level_idc_present_flag; } set { max_mcs_tier_level_idc_present_flag = value; } }
        private byte mcts_max_tier_flag;
        public byte MctsMaxTierFlag { get { return mcts_max_tier_flag; } set { mcts_max_tier_flag = value; } }
        private uint mcts_max_level_idc;
        public uint MctsMaxLevelIdc { get { return mcts_max_level_idc; } set { mcts_max_level_idc = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public TemporalMotionConstrainedTileSets(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.mc_all_tiles_exact_sample_value_match_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.each_tile_one_tile_set_flag);

            if (each_tile_one_tile_set_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.limited_tile_set_display_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_sets_in_message_minus1);

                this.mcts_id = new uint[num_sets_in_message_minus1 + 1];
                this.display_tile_set_flag = new byte[num_sets_in_message_minus1 + 1];
                this.num_tile_rects_in_set_minus1 = new uint[num_sets_in_message_minus1 + 1];
                this.top_left_tile_idx = new uint[num_sets_in_message_minus1 + 1][];
                this.bottom_right_tile_idx = new uint[num_sets_in_message_minus1 + 1][];
                this.mc_exact_sample_value_match_flag = new byte[num_sets_in_message_minus1 + 1];
                this.mcts_tier_level_idc_present_flag = new byte[num_sets_in_message_minus1 + 1];
                this.mcts_tier_flag = new byte[num_sets_in_message_minus1 + 1];
                this.mcts_level_idc = new uint[num_sets_in_message_minus1 + 1];
                for (i = 0; i <= num_sets_in_message_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.mcts_id[i]);

                    if (limited_tile_set_display_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.display_tile_set_flag[i]);
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_tile_rects_in_set_minus1[i]);

                    this.top_left_tile_idx[i] = new uint[num_tile_rects_in_set_minus1[i] + 1];
                    this.bottom_right_tile_idx[i] = new uint[num_tile_rects_in_set_minus1[i] + 1];
                    for (j = 0; j <= num_tile_rects_in_set_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.top_left_tile_idx[i][j]);
                        size += stream.ReadUnsignedIntGolomb(size, out this.bottom_right_tile_idx[i][j]);
                    }

                    if (mc_all_tiles_exact_sample_value_match_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.mc_exact_sample_value_match_flag[i]);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.mcts_tier_level_idc_present_flag[i]);

                    if (mcts_tier_level_idc_present_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.mcts_tier_flag[i]);
                        size += stream.ReadUnsignedInt(size, 8, out this.mcts_level_idc[i]);
                    }
                }
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 1, out this.max_mcs_tier_level_idc_present_flag);

                if (mcts_max_tier_level_idc_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.mcts_max_tier_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.mcts_max_level_idc);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.mc_all_tiles_exact_sample_value_match_flag);
            size += stream.WriteUnsignedInt(1, this.each_tile_one_tile_set_flag);

            if (each_tile_one_tile_set_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.limited_tile_set_display_flag);
                size += stream.WriteUnsignedIntGolomb(this.num_sets_in_message_minus1);

                for (i = 0; i <= num_sets_in_message_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.mcts_id[i]);

                    if (limited_tile_set_display_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.display_tile_set_flag[i]);
                    }
                    size += stream.WriteUnsignedIntGolomb(this.num_tile_rects_in_set_minus1[i]);

                    for (j = 0; j <= num_tile_rects_in_set_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.top_left_tile_idx[i][j]);
                        size += stream.WriteUnsignedIntGolomb(this.bottom_right_tile_idx[i][j]);
                    }

                    if (mc_all_tiles_exact_sample_value_match_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.mc_exact_sample_value_match_flag[i]);
                    }
                    size += stream.WriteUnsignedInt(1, this.mcts_tier_level_idc_present_flag[i]);

                    if (mcts_tier_level_idc_present_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.mcts_tier_flag[i]);
                        size += stream.WriteUnsignedInt(8, this.mcts_level_idc[i]);
                    }
                }
            }
            else
            {
                size += stream.WriteUnsignedInt(1, this.max_mcs_tier_level_idc_present_flag);

                if (mcts_max_tier_level_idc_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.mcts_max_tier_flag);
                    size += stream.WriteUnsignedInt(8, this.mcts_max_level_idc);
                }
            }

            return size;
        }

    }

    /*
  

chroma_resampling_filter_hint( payloadSize ) {  
 ver_chroma_filter_idc u(8) 
 hor_chroma_filter_idc u(8) 
 ver_filtering_field_processing_flag u(1) 
 if( ver_chroma_filter_idc == 1  ||  hor_chroma_filter_idc == 1 ) {  
  target_format_idc ue(v) 
  if( ver_chroma_filter_idc == 1 ) {  
   num_vertical_filters ue(v) 
   for( i = 0; i < num_vertical_filters; i++ ) {  
    ver_tap_length_minus1[ i ] ue(v) 
    for( j = 0; j <= ver_tap_length_minus1[ i ]; j++ )  
     ver_filter_coeff[ i ][ j ] se(v) 
   }  
  }  
  if( hor_chroma_filter_idc == 1 ) {  
   num_horizontal_filters ue(v) 
   for( i = 0; i < num_horizontal_filters; i++ ) {  
    hor_tap_length_minus1[ i ] ue(v) 
    for( j = 0; j <= hor_tap_length_minus1[ i ]; j++ )  
     hor_filter_coeff[ i ][ j ] se(v) 
   }  
  }  
 }  
}
    */
    public class ChromaResamplingFilterHint : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint ver_chroma_filter_idc;
        public uint VerChromaFilterIdc { get { return ver_chroma_filter_idc; } set { ver_chroma_filter_idc = value; } }
        private uint hor_chroma_filter_idc;
        public uint HorChromaFilterIdc { get { return hor_chroma_filter_idc; } set { hor_chroma_filter_idc = value; } }
        private byte ver_filtering_field_processing_flag;
        public byte VerFilteringFieldProcessingFlag { get { return ver_filtering_field_processing_flag; } set { ver_filtering_field_processing_flag = value; } }
        private uint target_format_idc;
        public uint TargetFormatIdc { get { return target_format_idc; } set { target_format_idc = value; } }
        private uint num_vertical_filters;
        public uint NumVerticalFilters { get { return num_vertical_filters; } set { num_vertical_filters = value; } }
        private uint[] ver_tap_length_minus1;
        public uint[] VerTapLengthMinus1 { get { return ver_tap_length_minus1; } set { ver_tap_length_minus1 = value; } }
        private int[][] ver_filter_coeff;
        public int[][] VerFilterCoeff { get { return ver_filter_coeff; } set { ver_filter_coeff = value; } }
        private uint num_horizontal_filters;
        public uint NumHorizontalFilters { get { return num_horizontal_filters; } set { num_horizontal_filters = value; } }
        private uint[] hor_tap_length_minus1;
        public uint[] HorTapLengthMinus1 { get { return hor_tap_length_minus1; } set { hor_tap_length_minus1 = value; } }
        private int[][] hor_filter_coeff;
        public int[][] HorFilterCoeff { get { return hor_filter_coeff; } set { hor_filter_coeff = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ChromaResamplingFilterHint(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 8, out this.ver_chroma_filter_idc);
            size += stream.ReadUnsignedInt(size, 8, out this.hor_chroma_filter_idc);
            size += stream.ReadUnsignedInt(size, 1, out this.ver_filtering_field_processing_flag);

            if (ver_chroma_filter_idc == 1 || hor_chroma_filter_idc == 1)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.target_format_idc);

                if (ver_chroma_filter_idc == 1)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_vertical_filters);

                    this.ver_tap_length_minus1 = new uint[num_vertical_filters];
                    this.ver_filter_coeff = new int[num_vertical_filters][];
                    for (i = 0; i < num_vertical_filters; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.ver_tap_length_minus1[i]);

                        this.ver_filter_coeff[i] = new int[ver_tap_length_minus1[i] + 1];
                        for (j = 0; j <= ver_tap_length_minus1[i]; j++)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.ver_filter_coeff[i][j]);
                        }
                    }
                }

                if (hor_chroma_filter_idc == 1)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_horizontal_filters);

                    this.hor_tap_length_minus1 = new uint[num_horizontal_filters];
                    this.hor_filter_coeff = new int[num_horizontal_filters][];
                    for (i = 0; i < num_horizontal_filters; i++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.hor_tap_length_minus1[i]);

                        this.hor_filter_coeff[i] = new int[hor_tap_length_minus1[i] + 1];
                        for (j = 0; j <= hor_tap_length_minus1[i]; j++)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.hor_filter_coeff[i][j]);
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
            size += stream.WriteUnsignedInt(8, this.ver_chroma_filter_idc);
            size += stream.WriteUnsignedInt(8, this.hor_chroma_filter_idc);
            size += stream.WriteUnsignedInt(1, this.ver_filtering_field_processing_flag);

            if (ver_chroma_filter_idc == 1 || hor_chroma_filter_idc == 1)
            {
                size += stream.WriteUnsignedIntGolomb(this.target_format_idc);

                if (ver_chroma_filter_idc == 1)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_vertical_filters);

                    for (i = 0; i < num_vertical_filters; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.ver_tap_length_minus1[i]);

                        for (j = 0; j <= ver_tap_length_minus1[i]; j++)
                        {
                            size += stream.WriteSignedIntGolomb(this.ver_filter_coeff[i][j]);
                        }
                    }
                }

                if (hor_chroma_filter_idc == 1)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_horizontal_filters);

                    for (i = 0; i < num_horizontal_filters; i++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.hor_tap_length_minus1[i]);

                        for (j = 0; j <= hor_tap_length_minus1[i]; j++)
                        {
                            size += stream.WriteSignedIntGolomb(this.hor_filter_coeff[i][j]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

knee_function_info( payloadSize ) {  
 knee_function_id ue(v) 
 knee_function_cancel_flag u(1) 
 if( !knee_function_cancel_flag ) {  
  knee_function_persistence_flag u(1) 
  input_d_range u(32) 
  input_disp_luminance u(32) 
  output_d_range u(32) 
  output_disp_luminance u(32) 
  num_knee_points_minus1 ue(v) 
  for( i = 0; i <= num_knee_points_minus1; i++ ) {  
   input_knee_point[ i ] u(10) 
   output_knee_point[ i ] u(10) 
  }  
 }  
}
    */
    public class KneeFunctionInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint knee_function_id;
        public uint KneeFunctionId { get { return knee_function_id; } set { knee_function_id = value; } }
        private byte knee_function_cancel_flag;
        public byte KneeFunctionCancelFlag { get { return knee_function_cancel_flag; } set { knee_function_cancel_flag = value; } }
        private byte knee_function_persistence_flag;
        public byte KneeFunctionPersistenceFlag { get { return knee_function_persistence_flag; } set { knee_function_persistence_flag = value; } }
        private uint input_d_range;
        public uint InputdRange { get { return input_d_range; } set { input_d_range = value; } }
        private uint input_disp_luminance;
        public uint InputDispLuminance { get { return input_disp_luminance; } set { input_disp_luminance = value; } }
        private uint output_d_range;
        public uint OutputdRange { get { return output_d_range; } set { output_d_range = value; } }
        private uint output_disp_luminance;
        public uint OutputDispLuminance { get { return output_disp_luminance; } set { output_disp_luminance = value; } }
        private uint num_knee_points_minus1;
        public uint NumKneePointsMinus1 { get { return num_knee_points_minus1; } set { num_knee_points_minus1 = value; } }
        private uint[] input_knee_point;
        public uint[] InputKneePoint { get { return input_knee_point; } set { input_knee_point = value; } }
        private uint[] output_knee_point;
        public uint[] OutputKneePoint { get { return output_knee_point; } set { output_knee_point = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public KneeFunctionInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.knee_function_id);
            size += stream.ReadUnsignedInt(size, 1, out this.knee_function_cancel_flag);

            if (knee_function_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.knee_function_persistence_flag);
                size += stream.ReadUnsignedInt(size, 32, out this.input_d_range);
                size += stream.ReadUnsignedInt(size, 32, out this.input_disp_luminance);
                size += stream.ReadUnsignedInt(size, 32, out this.output_d_range);
                size += stream.ReadUnsignedInt(size, 32, out this.output_disp_luminance);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_knee_points_minus1);

                this.input_knee_point = new uint[num_knee_points_minus1 + 1];
                this.output_knee_point = new uint[num_knee_points_minus1 + 1];
                for (i = 0; i <= num_knee_points_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 10, out this.input_knee_point[i]);
                    size += stream.ReadUnsignedInt(size, 10, out this.output_knee_point[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.knee_function_id);
            size += stream.WriteUnsignedInt(1, this.knee_function_cancel_flag);

            if (knee_function_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.knee_function_persistence_flag);
                size += stream.WriteUnsignedInt(32, this.input_d_range);
                size += stream.WriteUnsignedInt(32, this.input_disp_luminance);
                size += stream.WriteUnsignedInt(32, this.output_d_range);
                size += stream.WriteUnsignedInt(32, this.output_disp_luminance);
                size += stream.WriteUnsignedIntGolomb(this.num_knee_points_minus1);

                for (i = 0; i <= num_knee_points_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(10, this.input_knee_point[i]);
                    size += stream.WriteUnsignedInt(10, this.output_knee_point[i]);
                }
            }

            return size;
        }

    }

    /*
 

colour_remapping_info( payloadSize ) {  
 colour_remap_id ue(v) 
 colour_remap_cancel_flag u(1) 
 if( !colour_remap_cancel_flag ) {  
  colour_remap_persistence_flag u(1) 
  colour_remap_video_signal_info_present_flag u(1) 
  if( colour_remap_video_signal_info_present_flag ) {  
   colour_remap_full_range_flag u(1) 
   colour_remap_primaries u(8) 
   colour_remap_transfer_function u(8) 
   colour_remap_matrix_coefficients u(8) 
  }  
  colour_remap_input_bit_depth u(8) 
  colour_remap_output_bit_depth u(8) 
  for( c = 0; c < 3; c++ ) {  
   pre_lut_num_val_minus1[ c ] u(8) 
   if( pre_lut_num_val_minus1[ c ] > 0 )  
    for( i = 0; i <= pre_lut_num_val_minus1[ c ]; i++ ) {  
     pre_lut_coded_value[ c ][ i ] u(v) 
     pre_lut_target_value[ c ][ i ] u(v) 
    }  
  }  
  colour_remap_matrix_present_flag u(1) 
  if( colour_remap_matrix_present_flag ) {  
   log2_matrix_denom u(4) 
   for( c = 0; c < 3; c++ )  
    for( i = 0; i < 3; i++ )  
     colour_remap_coeffs[ c ][ i ] se(v) 
  }  
  for( c = 0; c < 3; c++ ) {  
   post_lut_num_val_minus1[ c ] u(8) 
   if( post_lut_num_val_minus1[ c ] > 0 )  
    for( i = 0; i <= post_lut_num_val_minus1[ c ]; i++ ) {  
     post_lut_coded_value[ c ][ i ] u(v) 
     post_lut_target_value[ c ][ i ] u(v) 
    }  
  }  
 }  
}
    */
    public class ColourRemappingInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint colour_remap_id;
        public uint ColourRemapId { get { return colour_remap_id; } set { colour_remap_id = value; } }
        private byte colour_remap_cancel_flag;
        public byte ColourRemapCancelFlag { get { return colour_remap_cancel_flag; } set { colour_remap_cancel_flag = value; } }
        private byte colour_remap_persistence_flag;
        public byte ColourRemapPersistenceFlag { get { return colour_remap_persistence_flag; } set { colour_remap_persistence_flag = value; } }
        private byte colour_remap_video_signal_info_present_flag;
        public byte ColourRemapVideoSignalInfoPresentFlag { get { return colour_remap_video_signal_info_present_flag; } set { colour_remap_video_signal_info_present_flag = value; } }
        private byte colour_remap_full_range_flag;
        public byte ColourRemapFullRangeFlag { get { return colour_remap_full_range_flag; } set { colour_remap_full_range_flag = value; } }
        private uint colour_remap_primaries;
        public uint ColourRemapPrimaries { get { return colour_remap_primaries; } set { colour_remap_primaries = value; } }
        private uint colour_remap_transfer_function;
        public uint ColourRemapTransferFunction { get { return colour_remap_transfer_function; } set { colour_remap_transfer_function = value; } }
        private uint colour_remap_matrix_coefficients;
        public uint ColourRemapMatrixCoefficients { get { return colour_remap_matrix_coefficients; } set { colour_remap_matrix_coefficients = value; } }
        private uint colour_remap_input_bit_depth;
        public uint ColourRemapInputBitDepth { get { return colour_remap_input_bit_depth; } set { colour_remap_input_bit_depth = value; } }
        private uint colour_remap_output_bit_depth;
        public uint ColourRemapOutputBitDepth { get { return colour_remap_output_bit_depth; } set { colour_remap_output_bit_depth = value; } }
        private uint[] pre_lut_num_val_minus1;
        public uint[] PreLutNumValMinus1 { get { return pre_lut_num_val_minus1; } set { pre_lut_num_val_minus1 = value; } }
        private uint[][] pre_lut_coded_value;
        public uint[][] PreLutCodedValue { get { return pre_lut_coded_value; } set { pre_lut_coded_value = value; } }
        private uint[][] pre_lut_target_value;
        public uint[][] PreLutTargetValue { get { return pre_lut_target_value; } set { pre_lut_target_value = value; } }
        private byte colour_remap_matrix_present_flag;
        public byte ColourRemapMatrixPresentFlag { get { return colour_remap_matrix_present_flag; } set { colour_remap_matrix_present_flag = value; } }
        private uint log2_matrix_denom;
        public uint Log2MatrixDenom { get { return log2_matrix_denom; } set { log2_matrix_denom = value; } }
        private int[][] colour_remap_coeffs;
        public int[][] ColourRemapCoeffs { get { return colour_remap_coeffs; } set { colour_remap_coeffs = value; } }
        private uint[] post_lut_num_val_minus1;
        public uint[] PostLutNumValMinus1 { get { return post_lut_num_val_minus1; } set { post_lut_num_val_minus1 = value; } }
        private uint[][] post_lut_coded_value;
        public uint[][] PostLutCodedValue { get { return post_lut_coded_value; } set { post_lut_coded_value = value; } }
        private uint[][] post_lut_target_value;
        public uint[][] PostLutTargetValue { get { return post_lut_target_value; } set { post_lut_target_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ColourRemappingInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.colour_remap_id);
            size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_cancel_flag);

            if (colour_remap_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_persistence_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_video_signal_info_present_flag);

                if (colour_remap_video_signal_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_full_range_flag);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_transfer_function);
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_matrix_coefficients);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_input_bit_depth);
                size += stream.ReadUnsignedInt(size, 8, out this.colour_remap_output_bit_depth);

                this.pre_lut_num_val_minus1 = new uint[3];
                this.pre_lut_coded_value = new uint[3][];
                this.pre_lut_target_value = new uint[3][];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.pre_lut_num_val_minus1[c]);

                    if (pre_lut_num_val_minus1[c] > 0)
                    {

                        this.pre_lut_coded_value[c] = new uint[pre_lut_num_val_minus1[c] + 1];
                        this.pre_lut_target_value[c] = new uint[pre_lut_num_val_minus1[c] + 1];
                        for (i = 0; i <= pre_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, pre_lut_coded_value, out this.pre_lut_coded_value[c][i]);
                            size += stream.ReadUnsignedIntVariable(size, pre_lut_target_value, out this.pre_lut_target_value[c][i]);
                        }
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.colour_remap_matrix_present_flag);

                if (colour_remap_matrix_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.log2_matrix_denom);

                    this.colour_remap_coeffs = new int[3][];
                    for (c = 0; c < 3; c++)
                    {

                        this.colour_remap_coeffs[c] = new int[3];
                        for (i = 0; i < 3; i++)
                        {
                            size += stream.ReadSignedIntGolomb(size, out this.colour_remap_coeffs[c][i]);
                        }
                    }
                }

                this.post_lut_num_val_minus1 = new uint[3];
                this.post_lut_coded_value = new uint[3][];
                this.post_lut_target_value = new uint[3][];
                for (c = 0; c < 3; c++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.post_lut_num_val_minus1[c]);

                    if (post_lut_num_val_minus1[c] > 0)
                    {

                        this.post_lut_coded_value[c] = new uint[post_lut_num_val_minus1[c] + 1];
                        this.post_lut_target_value[c] = new uint[post_lut_num_val_minus1[c] + 1];
                        for (i = 0; i <= post_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, post_lut_coded_value, out this.post_lut_coded_value[c][i]);
                            size += stream.ReadUnsignedIntVariable(size, post_lut_target_value, out this.post_lut_target_value[c][i]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint c = 0;
            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.colour_remap_id);
            size += stream.WriteUnsignedInt(1, this.colour_remap_cancel_flag);

            if (colour_remap_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.colour_remap_persistence_flag);
                size += stream.WriteUnsignedInt(1, this.colour_remap_video_signal_info_present_flag);

                if (colour_remap_video_signal_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.colour_remap_full_range_flag);
                    size += stream.WriteUnsignedInt(8, this.colour_remap_primaries);
                    size += stream.WriteUnsignedInt(8, this.colour_remap_transfer_function);
                    size += stream.WriteUnsignedInt(8, this.colour_remap_matrix_coefficients);
                }
                size += stream.WriteUnsignedInt(8, this.colour_remap_input_bit_depth);
                size += stream.WriteUnsignedInt(8, this.colour_remap_output_bit_depth);

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(8, this.pre_lut_num_val_minus1[c]);

                    if (pre_lut_num_val_minus1[c] > 0)
                    {

                        for (i = 0; i <= pre_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedIntVariable(pre_lut_coded_value[c][i], this.pre_lut_coded_value[c][i]);
                            size += stream.WriteUnsignedIntVariable(pre_lut_target_value[c][i], this.pre_lut_target_value[c][i]);
                        }
                    }
                }
                size += stream.WriteUnsignedInt(1, this.colour_remap_matrix_present_flag);

                if (colour_remap_matrix_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(4, this.log2_matrix_denom);

                    for (c = 0; c < 3; c++)
                    {

                        for (i = 0; i < 3; i++)
                        {
                            size += stream.WriteSignedIntGolomb(this.colour_remap_coeffs[c][i]);
                        }
                    }
                }

                for (c = 0; c < 3; c++)
                {
                    size += stream.WriteUnsignedInt(8, this.post_lut_num_val_minus1[c]);

                    if (post_lut_num_val_minus1[c] > 0)
                    {

                        for (i = 0; i <= post_lut_num_val_minus1[c]; i++)
                        {
                            size += stream.WriteUnsignedIntVariable(post_lut_coded_value[c][i], this.post_lut_coded_value[c][i]);
                            size += stream.WriteUnsignedIntVariable(post_lut_target_value[c][i], this.post_lut_target_value[c][i]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
 

deinterlaced_field_indentification( payloadSize ) { 
deinterlaced_picture_source_parity_flag  u(1) 
}
    */
    public class DeinterlacedFieldIndentification : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte deinterlaced_picture_source_parity_flag;
        public byte DeinterlacedPictureSourceParityFlag { get { return deinterlaced_picture_source_parity_flag; } set { deinterlaced_picture_source_parity_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DeinterlacedFieldIndentification(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.deinterlaced_picture_source_parity_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.deinterlaced_picture_source_parity_flag);

            return size;
        }

    }

    /*


content_light_level_info( payloadSize ) { 
max_content_light_level  u(16) 
max_pic_average_light_level u(16) 
}
    */
    public class ContentLightLevelInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint max_content_light_level;
        public uint MaxContentLightLevel { get { return max_content_light_level; } set { max_content_light_level = value; } }
        private uint max_pic_average_light_level;
        public uint MaxPicAverageLightLevel { get { return max_pic_average_light_level; } set { max_pic_average_light_level = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ContentLightLevelInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 16, out this.max_content_light_level);
            size += stream.ReadUnsignedInt(size, 16, out this.max_pic_average_light_level);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(16, this.max_content_light_level);
            size += stream.WriteUnsignedInt(16, this.max_pic_average_light_level);

            return size;
        }

    }

    /*


dependent_rap_indication( payloadSize ) { 
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


coded_region_completion( payloadSize ) { 
next_segment_address ue(v) 
if( next_segment_address > 0 )  
independent_slice_segment_flag u(1) 
}
    */
    public class CodedRegionCompletion : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint next_segment_address;
        public uint NextSegmentAddress { get { return next_segment_address; } set { next_segment_address = value; } }
        private byte independent_slice_segment_flag;
        public byte IndependentSliceSegmentFlag { get { return independent_slice_segment_flag; } set { independent_slice_segment_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public CodedRegionCompletion(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedIntGolomb(size, out this.next_segment_address);

            if (next_segment_address > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.independent_slice_segment_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedIntGolomb(this.next_segment_address);

            if (next_segment_address > 0)
            {
                size += stream.WriteUnsignedInt(1, this.independent_slice_segment_flag);
            }

            return size;
        }

    }

    /*


alternative_transfer_characteristics ( payloadSize ) { 
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


ambient_viewing_environment( payloadSize ) { 
ambient_illuminance  u(32) 
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


content_colour_volume( payloadSize ) {  
 ccv_cancel_flag u(1) 
 if( !ccv_cancel_flag ) {  
  ccv_persistence_flag u(1) 
  ccv_primaries_present_flag u(1) 
  ccv_min_luminance_value_present_flag u(1) 
  ccv_max_luminance_value_present_flag u(1) 
  ccv_avg_luminance_value_present_flag u(1) 
  ccv_reserved_zero_2bits u(2) 
  if( ccv_primaries_present_flag )  
   for( c = 0; c < 3; c++ ) {  
    ccv_primaries_x[ c ] i(32) 
    ccv_primaries_y[ c ] i(32) 
   }  
  if( ccv_min_luminance_value_present_flag )  
   ccv_min_luminance_value u(32) 
  if( ccv_max_luminance_value_present_flag )  
   ccv_max_luminance_value u(32) 
  if( ccv_avg_luminance_value_present_flag )  
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
 

equirectangular_projection( payloadSize ) {  
 erp_cancel_flag u(1) 
 if( !erp_cancel_flag ) {  
  erp_persistence_flag u(1) 
  erp_guard_band_flag u(1) 
  erp_reserved_zero_2bits u(2) 
  if( erp_guard_band_flag == 1 ) {  
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
 

cubemap_projection( payloadSize ) { 
cmp_cancel_flag u(1) 
if( !cmp_cancel_flag )  
cmp_persistence_flag u(1) 
}
    */
    public class CubemapProjection : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte cmp_cancel_flag;
        public byte CmpCancelFlag { get { return cmp_cancel_flag; } set { cmp_cancel_flag = value; } }
        private byte cmp_persistence_flag;
        public byte CmpPersistenceFlag { get { return cmp_persistence_flag; } set { cmp_persistence_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public CubemapProjection(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.cmp_cancel_flag);

            if (cmp_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.cmp_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.cmp_cancel_flag);

            if (cmp_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.cmp_persistence_flag);
            }

            return size;
        }

    }

    /*
  

sphere_rotation( payloadSize ) { 
sphere_rotation_cancel_flag  u(1) 
if( !sphere_rotation_cancel_flag ) {  
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
 

regionwise_packing( payloadSize ) {  
 rwp_cancel_flag u(1) 
 if( !rwp_cancel_flag ) {  
  rwp_persistence_flag u(1) 
  constituent_picture_matching_flag u(1) 
  rwp_reserved_zero_5bits u(5) 
  num_packed_regions u(8) 
  proj_picture_width u(32) 
  proj_picture_height u(32) 
  packed_picture_width u(16) 
  packed_picture_height u(16) 
  for( i = 0; i < num_packed_regions; i++ ) {  
   rwp_reserved_zero_4bits[ i ] u(4) 
   rwp_transform_type[ i ] u(3) 
   rwp_guard_band_flag[ i ] u(1) 
   proj_region_width[ i ] u(32) 
   proj_region_height[ i ] u(32) 
   proj_region_top[ i ] u(32) 
   proj_region_left[ i ] u(32) 
   packed_region_width[ i ] u(16) 
   packed_region_height[ i ] u(16) 
   packed_region_top[ i ] u(16) 
   packed_region_left[ i ] u(16) 
   if( rwp_guard_band_flag[ i ] ) {  
    rwp_left_guard_band_width[ i ] u(8) 
    rwp_right_guard_band_width[ i ] u(8) 
    rwp_top_guard_band_height[ i ] u(8) 
    rwp_bottom_guard_band_height[ i ] u(8) 
    rwp_guard_band_not_used_for_pred_flag[ i ] u(1) 
    for( j = 0; j < 4; j++ )  
     rwp_guard_band_type[ i ][ j ] u(3) 
    rwp_guard_band_reserved_zero_3bits[ i ] u(3) 
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
        private byte constituent_picture_matching_flag;
        public byte ConstituentPictureMatchingFlag { get { return constituent_picture_matching_flag; } set { constituent_picture_matching_flag = value; } }
        private uint rwp_reserved_zero_5bits;
        public uint RwpReservedZero5bits { get { return rwp_reserved_zero_5bits; } set { rwp_reserved_zero_5bits = value; } }
        private uint num_packed_regions;
        public uint NumPackedRegions { get { return num_packed_regions; } set { num_packed_regions = value; } }
        private uint proj_picture_width;
        public uint ProjPictureWidth { get { return proj_picture_width; } set { proj_picture_width = value; } }
        private uint proj_picture_height;
        public uint ProjPictureHeight { get { return proj_picture_height; } set { proj_picture_height = value; } }
        private uint packed_picture_width;
        public uint PackedPictureWidth { get { return packed_picture_width; } set { packed_picture_width = value; } }
        private uint packed_picture_height;
        public uint PackedPictureHeight { get { return packed_picture_height; } set { packed_picture_height = value; } }
        private uint[] rwp_reserved_zero_4bits;
        public uint[] RwpReservedZero4bits { get { return rwp_reserved_zero_4bits; } set { rwp_reserved_zero_4bits = value; } }
        private uint[] rwp_transform_type;
        public uint[] RwpTransformType { get { return rwp_transform_type; } set { rwp_transform_type = value; } }
        private byte[] rwp_guard_band_flag;
        public byte[] RwpGuardBandFlag { get { return rwp_guard_band_flag; } set { rwp_guard_band_flag = value; } }
        private uint[] proj_region_width;
        public uint[] ProjRegionWidth { get { return proj_region_width; } set { proj_region_width = value; } }
        private uint[] proj_region_height;
        public uint[] ProjRegionHeight { get { return proj_region_height; } set { proj_region_height = value; } }
        private uint[] proj_region_top;
        public uint[] ProjRegionTop { get { return proj_region_top; } set { proj_region_top = value; } }
        private uint[] proj_region_left;
        public uint[] ProjRegionLeft { get { return proj_region_left; } set { proj_region_left = value; } }
        private uint[] packed_region_width;
        public uint[] PackedRegionWidth { get { return packed_region_width; } set { packed_region_width = value; } }
        private uint[] packed_region_height;
        public uint[] PackedRegionHeight { get { return packed_region_height; } set { packed_region_height = value; } }
        private uint[] packed_region_top;
        public uint[] PackedRegionTop { get { return packed_region_top; } set { packed_region_top = value; } }
        private uint[] packed_region_left;
        public uint[] PackedRegionLeft { get { return packed_region_left; } set { packed_region_left = value; } }
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
                size += stream.ReadUnsignedInt(size, 1, out this.constituent_picture_matching_flag);
                size += stream.ReadUnsignedInt(size, 5, out this.rwp_reserved_zero_5bits);
                size += stream.ReadUnsignedInt(size, 8, out this.num_packed_regions);
                size += stream.ReadUnsignedInt(size, 32, out this.proj_picture_width);
                size += stream.ReadUnsignedInt(size, 32, out this.proj_picture_height);
                size += stream.ReadUnsignedInt(size, 16, out this.packed_picture_width);
                size += stream.ReadUnsignedInt(size, 16, out this.packed_picture_height);

                this.rwp_reserved_zero_4bits = new uint[num_packed_regions];
                this.rwp_transform_type = new uint[num_packed_regions];
                this.rwp_guard_band_flag = new byte[num_packed_regions];
                this.proj_region_width = new uint[num_packed_regions];
                this.proj_region_height = new uint[num_packed_regions];
                this.proj_region_top = new uint[num_packed_regions];
                this.proj_region_left = new uint[num_packed_regions];
                this.packed_region_width = new uint[num_packed_regions];
                this.packed_region_height = new uint[num_packed_regions];
                this.packed_region_top = new uint[num_packed_regions];
                this.packed_region_left = new uint[num_packed_regions];
                this.rwp_left_guard_band_width = new uint[num_packed_regions];
                this.rwp_right_guard_band_width = new uint[num_packed_regions];
                this.rwp_top_guard_band_height = new uint[num_packed_regions];
                this.rwp_bottom_guard_band_height = new uint[num_packed_regions];
                this.rwp_guard_band_not_used_for_pred_flag = new byte[num_packed_regions];
                this.rwp_guard_band_type = new uint[num_packed_regions][];
                this.rwp_guard_band_reserved_zero_3bits = new uint[num_packed_regions];
                for (i = 0; i < num_packed_regions; i++)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.rwp_reserved_zero_4bits[i]);
                    size += stream.ReadUnsignedInt(size, 3, out this.rwp_transform_type[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.rwp_guard_band_flag[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_width[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_height[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_top[i]);
                    size += stream.ReadUnsignedInt(size, 32, out this.proj_region_left[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_width[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_height[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_top[i]);
                    size += stream.ReadUnsignedInt(size, 16, out this.packed_region_left[i]);

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
                size += stream.WriteUnsignedInt(1, this.constituent_picture_matching_flag);
                size += stream.WriteUnsignedInt(5, this.rwp_reserved_zero_5bits);
                size += stream.WriteUnsignedInt(8, this.num_packed_regions);
                size += stream.WriteUnsignedInt(32, this.proj_picture_width);
                size += stream.WriteUnsignedInt(32, this.proj_picture_height);
                size += stream.WriteUnsignedInt(16, this.packed_picture_width);
                size += stream.WriteUnsignedInt(16, this.packed_picture_height);

                for (i = 0; i < num_packed_regions; i++)
                {
                    size += stream.WriteUnsignedInt(4, this.rwp_reserved_zero_4bits[i]);
                    size += stream.WriteUnsignedInt(3, this.rwp_transform_type[i]);
                    size += stream.WriteUnsignedInt(1, this.rwp_guard_band_flag[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_width[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_height[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_top[i]);
                    size += stream.WriteUnsignedInt(32, this.proj_region_left[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_width[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_height[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_top[i]);
                    size += stream.WriteUnsignedInt(16, this.packed_region_left[i]);

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
 

omni_viewport( payloadSize ) {  
 omni_viewport_id u(10) 
 omni_viewport_cancel_flag u(1) 
 if( !omni_viewport_cancel_flag ) {  
  omni_viewport_persistence_flag u(1) 
  omni_viewport_cnt_minus1 u(4) 
  for( i = 0; i <= omni_viewport_cnt_minus1; i++ ) {  
   omni_viewport_azimuth_centre[ i ] i(32) 
   omni_viewport_elevation_centre[ i ] i(32) 
   omni_viewport_tilt_centre[ i ] i(32) 
   omni_viewport_hor_range[ i ] u(32) 
   omni_viewport_ver_range[ i ] u(32) 
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
 

regional_nesting( payloadSize ) {  
 regional_nesting_id u(16) 
 regional_nesting_num_rect_regions u(8) 
 for( i = 0; i < regional_nesting_num_rect_regions; i++ ) {  
  regional_nesting_rect_region_id[ i ] u(8) 
  regional_nesting_rect_left_offset[ i ] u(16) 
  regional_nesting_rect_right_offset[ i ] u(16) 
  regional_nesting_rect_top_offset[ i ] u(16) 
  regional_nesting_rect_bottom_offset[ i ] u(16) 
 }  
 num_sei_messages_in_regional_nesting_minus1 u(8) 
 for( i = 0; i <= num_sei_messages_in_regional_nesting_minus1; i++ ) {  
  num_regions_for_sei_message[ i ] u(8) 
  for(j = 0; j < num_regions_for_sei_message[ i ]; j++ )  
   regional_nesting_sei_region_idx[ i ][ j ] u(8) 
  sei_message()  
 }  
}
    */
    public class RegionalNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint regional_nesting_id;
        public uint RegionalNestingId { get { return regional_nesting_id; } set { regional_nesting_id = value; } }
        private uint regional_nesting_num_rect_regions;
        public uint RegionalNestingNumRectRegions { get { return regional_nesting_num_rect_regions; } set { regional_nesting_num_rect_regions = value; } }
        private uint[] regional_nesting_rect_region_id;
        public uint[] RegionalNestingRectRegionId { get { return regional_nesting_rect_region_id; } set { regional_nesting_rect_region_id = value; } }
        private uint[] regional_nesting_rect_left_offset;
        public uint[] RegionalNestingRectLeftOffset { get { return regional_nesting_rect_left_offset; } set { regional_nesting_rect_left_offset = value; } }
        private uint[] regional_nesting_rect_right_offset;
        public uint[] RegionalNestingRectRightOffset { get { return regional_nesting_rect_right_offset; } set { regional_nesting_rect_right_offset = value; } }
        private uint[] regional_nesting_rect_top_offset;
        public uint[] RegionalNestingRectTopOffset { get { return regional_nesting_rect_top_offset; } set { regional_nesting_rect_top_offset = value; } }
        private uint[] regional_nesting_rect_bottom_offset;
        public uint[] RegionalNestingRectBottomOffset { get { return regional_nesting_rect_bottom_offset; } set { regional_nesting_rect_bottom_offset = value; } }
        private uint num_sei_messages_in_regional_nesting_minus1;
        public uint NumSeiMessagesInRegionalNestingMinus1 { get { return num_sei_messages_in_regional_nesting_minus1; } set { num_sei_messages_in_regional_nesting_minus1 = value; } }
        private uint[] num_regions_for_sei_message;
        public uint[] NumRegionsForSeiMessage { get { return num_regions_for_sei_message; } set { num_regions_for_sei_message = value; } }
        private uint[][] regional_nesting_sei_region_idx;
        public uint[][] RegionalNestingSeiRegionIdx { get { return regional_nesting_sei_region_idx; } set { regional_nesting_sei_region_idx = value; } }
        private SeiMessage[] sei_message;
        public SeiMessage[] SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RegionalNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 16, out this.regional_nesting_id);
            size += stream.ReadUnsignedInt(size, 8, out this.regional_nesting_num_rect_regions);

            this.regional_nesting_rect_region_id = new uint[regional_nesting_num_rect_regions];
            this.regional_nesting_rect_left_offset = new uint[regional_nesting_num_rect_regions];
            this.regional_nesting_rect_right_offset = new uint[regional_nesting_num_rect_regions];
            this.regional_nesting_rect_top_offset = new uint[regional_nesting_num_rect_regions];
            this.regional_nesting_rect_bottom_offset = new uint[regional_nesting_num_rect_regions];
            for (i = 0; i < regional_nesting_num_rect_regions; i++)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.regional_nesting_rect_region_id[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.regional_nesting_rect_left_offset[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.regional_nesting_rect_right_offset[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.regional_nesting_rect_top_offset[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.regional_nesting_rect_bottom_offset[i]);
            }
            size += stream.ReadUnsignedInt(size, 8, out this.num_sei_messages_in_regional_nesting_minus1);

            this.num_regions_for_sei_message = new uint[num_sei_messages_in_regional_nesting_minus1 + 1];
            this.regional_nesting_sei_region_idx = new uint[num_sei_messages_in_regional_nesting_minus1 + 1][];
            this.sei_message = new SeiMessage[num_sei_messages_in_regional_nesting_minus1 + 1];
            for (i = 0; i <= num_sei_messages_in_regional_nesting_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.num_regions_for_sei_message[i]);

                this.regional_nesting_sei_region_idx[i] = new uint[num_regions_for_sei_message[i]];
                for (j = 0; j < num_regions_for_sei_message[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.regional_nesting_sei_region_idx[i][j]);
                }
                this.sei_message[i] = new SeiMessage();
                size += stream.ReadClass<SeiMessage>(size, context, this.sei_message[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(16, this.regional_nesting_id);
            size += stream.WriteUnsignedInt(8, this.regional_nesting_num_rect_regions);

            for (i = 0; i < regional_nesting_num_rect_regions; i++)
            {
                size += stream.WriteUnsignedInt(8, this.regional_nesting_rect_region_id[i]);
                size += stream.WriteUnsignedInt(16, this.regional_nesting_rect_left_offset[i]);
                size += stream.WriteUnsignedInt(16, this.regional_nesting_rect_right_offset[i]);
                size += stream.WriteUnsignedInt(16, this.regional_nesting_rect_top_offset[i]);
                size += stream.WriteUnsignedInt(16, this.regional_nesting_rect_bottom_offset[i]);
            }
            size += stream.WriteUnsignedInt(8, this.num_sei_messages_in_regional_nesting_minus1);

            for (i = 0; i <= num_sei_messages_in_regional_nesting_minus1; i++)
            {
                size += stream.WriteUnsignedInt(8, this.num_regions_for_sei_message[i]);

                for (j = 0; j < num_regions_for_sei_message[i]; j++)
                {
                    size += stream.WriteUnsignedInt(8, this.regional_nesting_sei_region_idx[i][j]);
                }
                size += stream.WriteClass<SeiMessage>(context, this.sei_message[i]);
            }

            return size;
        }

    }

    /*
 

mcts_extraction_info_sets( payloadSize ) {  
 num_info_sets_minus1 ue(v) 
 for( i = 0; i <= num_info_sets_minus1; i++ ) {  
  num_mcts_sets_minus1[ i ] ue(v) 
  for( j = 0; j <= num_mcts_sets_minus1[ i ]; j++ ) {  
   num_mcts_in_set_minus1[ i ][ j ] ue(v) 
   for( k = 0; k <= num_mcts_in_set_minus1[ i ][ j ]; k++ )  
    idx_of_mcts_in_set[ i ][ j ][ k ] ue(v) 
  }  
  slice_reordering_enabled_flag[ i ] u(1) 
  if( slice_reordering_enabled_flag[ i ] ) {  
   num_slice_segments_minus1[ i ] ue(v) 
   for( j = 0; j <= num_slice_segments_minus1[ i ]; j++ )  
    output_slice_segment_address[ i ][ j ] u(v) 
  }  
  num_vps_in_info_set_minus1[ i ] ue(v) 
  for( j = 0; j <= num_vps_in_info_set_minus1[ i ]; j++ )  
   vps_rbsp_data_length[ i ][ j ] ue(v) 
  num_sps_in_info_set_minus1[ i ] ue(v) 
  for( j = 0; j <= num_sps_in_info_set_minus1[ i ]; j++ )  
   sps_rbsp_data_length[ i ][ j ] ue(v) 
  num_pps_in_info_set_minus1[ i ] ue(v) 
  for( j = 0; j <= num_pps_in_info_set_minus1[ i ]; j++ ) {  
   pps_nuh_temporal_id_plus1[ i ][ j ] u(3) 
   pps_rbsp_data_length[ i ][ j ] ue(v) 
  }  
  while( !byte_aligned() )  
   mcts_alignment_bit_equal_to_zero f(1) 
  for( j = 0; j <= num_vps_in_info_set_minus1[ i ]; j++ )  
   for( k = 0; k <= vps_rbsp_data_length[ i ][ j ]; k++ )  
    vps_rbsp_data_byte[ i ][ j ][ k ] u(8) 
  for( j = 0; j <= num_sps_in_info_set_minus1[ i ]; j++ )  
   for( k = 0; k <= sps_rbsp_data_length[ i ][ j ]; k++ )  
    sps_rbsp_data_byte[ i ][ j ][ k ] u(8) 
  for( j = 0; j <= num_pps_in_info_set_minus1[ i ]; j++ )  
   for( k = 0; k <= pps_rbsp_data_length[ i ][ j ]; k++ )  
    pps_rbsp_data_byte[ i ][ j ][ k ] u(8) 
 }  
}
    */
    public class MctsExtractionInfoSets : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint num_info_sets_minus1;
        public uint NumInfoSetsMinus1 { get { return num_info_sets_minus1; } set { num_info_sets_minus1 = value; } }
        private uint[] num_mcts_sets_minus1;
        public uint[] NumMctsSetsMinus1 { get { return num_mcts_sets_minus1; } set { num_mcts_sets_minus1 = value; } }
        private uint[][] num_mcts_in_set_minus1;
        public uint[][] NumMctsInSetMinus1 { get { return num_mcts_in_set_minus1; } set { num_mcts_in_set_minus1 = value; } }
        private uint[][][] idx_of_mcts_in_set;
        public uint[][][] IdxOfMctsInSet { get { return idx_of_mcts_in_set; } set { idx_of_mcts_in_set = value; } }
        private byte[] slice_reordering_enabled_flag;
        public byte[] SliceReorderingEnabledFlag { get { return slice_reordering_enabled_flag; } set { slice_reordering_enabled_flag = value; } }
        private uint[] num_slice_segments_minus1;
        public uint[] NumSliceSegmentsMinus1 { get { return num_slice_segments_minus1; } set { num_slice_segments_minus1 = value; } }
        private uint[][] output_slice_segment_address;
        public uint[][] OutputSliceSegmentAddress { get { return output_slice_segment_address; } set { output_slice_segment_address = value; } }
        private uint[] num_vps_in_info_set_minus1;
        public uint[] NumVpsInInfoSetMinus1 { get { return num_vps_in_info_set_minus1; } set { num_vps_in_info_set_minus1 = value; } }
        private uint[][] vps_rbsp_data_length;
        public uint[][] VpsRbspDataLength { get { return vps_rbsp_data_length; } set { vps_rbsp_data_length = value; } }
        private uint[] num_sps_in_info_set_minus1;
        public uint[] NumSpsInInfoSetMinus1 { get { return num_sps_in_info_set_minus1; } set { num_sps_in_info_set_minus1 = value; } }
        private uint[][] sps_rbsp_data_length;
        public uint[][] SpsRbspDataLength { get { return sps_rbsp_data_length; } set { sps_rbsp_data_length = value; } }
        private uint[] num_pps_in_info_set_minus1;
        public uint[] NumPpsInInfoSetMinus1 { get { return num_pps_in_info_set_minus1; } set { num_pps_in_info_set_minus1 = value; } }
        private uint[][] pps_nuh_temporal_id_plus1;
        public uint[][] PpsNuhTemporalIdPlus1 { get { return pps_nuh_temporal_id_plus1; } set { pps_nuh_temporal_id_plus1 = value; } }
        private uint[][] pps_rbsp_data_length;
        public uint[][] PpsRbspDataLength { get { return pps_rbsp_data_length; } set { pps_rbsp_data_length = value; } }
        private Dictionary<int, uint> mcts_alignment_bit_equal_to_zero = new Dictionary<int, uint>();
        public Dictionary<int, uint> MctsAlignmentBitEqualToZero { get { return mcts_alignment_bit_equal_to_zero; } set { mcts_alignment_bit_equal_to_zero = value; } }
        private uint[][][] vps_rbsp_data_byte;
        public uint[][][] VpsRbspDataByte { get { return vps_rbsp_data_byte; } set { vps_rbsp_data_byte = value; } }
        private uint[][][] sps_rbsp_data_byte;
        public uint[][][] SpsRbspDataByte { get { return sps_rbsp_data_byte; } set { sps_rbsp_data_byte = value; } }
        private uint[][][] pps_rbsp_data_byte;
        public uint[][][] PpsRbspDataByte { get { return pps_rbsp_data_byte; } set { pps_rbsp_data_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MctsExtractionInfoSets(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_info_sets_minus1);

            this.num_mcts_sets_minus1 = new uint[num_info_sets_minus1 + 1];
            this.num_mcts_in_set_minus1 = new uint[num_info_sets_minus1 + 1][];
            this.idx_of_mcts_in_set = new uint[num_info_sets_minus1 + 1][][];
            this.slice_reordering_enabled_flag = new byte[num_info_sets_minus1 + 1];
            this.num_slice_segments_minus1 = new uint[num_info_sets_minus1 + 1];
            this.output_slice_segment_address = new uint[num_info_sets_minus1 + 1][];
            this.num_vps_in_info_set_minus1 = new uint[num_info_sets_minus1 + 1];
            this.vps_rbsp_data_length = new uint[num_info_sets_minus1 + 1][];
            this.num_sps_in_info_set_minus1 = new uint[num_info_sets_minus1 + 1];
            this.sps_rbsp_data_length = new uint[num_info_sets_minus1 + 1][];
            this.num_pps_in_info_set_minus1 = new uint[num_info_sets_minus1 + 1];
            this.pps_nuh_temporal_id_plus1 = new uint[num_info_sets_minus1 + 1][];
            this.pps_rbsp_data_length = new uint[num_info_sets_minus1 + 1][];
            this.vps_rbsp_data_byte = new uint[num_info_sets_minus1 + 1][][];
            this.sps_rbsp_data_byte = new uint[num_info_sets_minus1 + 1][][];
            this.pps_rbsp_data_byte = new uint[num_info_sets_minus1 + 1][][];
            for (i = 0; i <= num_info_sets_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_mcts_sets_minus1[i]);

                this.num_mcts_in_set_minus1[i] = new uint[num_mcts_sets_minus1[i] + 1];
                this.idx_of_mcts_in_set[i] = new uint[num_mcts_sets_minus1[i] + 1][];
                for (j = 0; j <= num_mcts_sets_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_mcts_in_set_minus1[i][j]);

                    this.idx_of_mcts_in_set[i][j] = new uint[num_mcts_in_set_minus1[i][j] + 1];
                    for (k = 0; k <= num_mcts_in_set_minus1[i][j]; k++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.idx_of_mcts_in_set[i][j][k]);
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.slice_reordering_enabled_flag[i]);

                if (slice_reordering_enabled_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_slice_segments_minus1[i]);

                    this.output_slice_segment_address[i] = new uint[num_slice_segments_minus1[i] + 1];
                    for (j = 0; j <= num_slice_segments_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, output_slice_segment_address, out this.output_slice_segment_address[i][j]);
                    }
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.num_vps_in_info_set_minus1[i]);

                this.vps_rbsp_data_length[i] = new uint[num_vps_in_info_set_minus1[i] + 1];
                for (j = 0; j <= num_vps_in_info_set_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vps_rbsp_data_length[i][j]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.num_sps_in_info_set_minus1[i]);

                this.sps_rbsp_data_length[i] = new uint[num_sps_in_info_set_minus1[i] + 1];
                for (j = 0; j <= num_sps_in_info_set_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_rbsp_data_length[i][j]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.num_pps_in_info_set_minus1[i]);

                this.pps_nuh_temporal_id_plus1[i] = new uint[num_pps_in_info_set_minus1[i] + 1];
                this.pps_rbsp_data_length[i] = new uint[num_pps_in_info_set_minus1[i] + 1];
                for (j = 0; j <= num_pps_in_info_set_minus1[i]; j++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.pps_nuh_temporal_id_plus1[i][j]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.pps_rbsp_data_length[i][j]);
                }

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadFixed(size, 1, whileIndex, this.mcts_alignment_bit_equal_to_zero);
                }

                this.vps_rbsp_data_byte[i] = new uint[num_vps_in_info_set_minus1[i] + 1][];
                for (j = 0; j <= num_vps_in_info_set_minus1[i]; j++)
                {

                    this.vps_rbsp_data_byte[i][j] = new uint[vps_rbsp_data_length[i][j]];
                    for (k = 0; k <= vps_rbsp_data_length[i][j]; k++)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.vps_rbsp_data_byte[i][j][k]);
                    }
                }

                this.sps_rbsp_data_byte[i] = new uint[num_sps_in_info_set_minus1[i] + 1][];
                for (j = 0; j <= num_sps_in_info_set_minus1[i]; j++)
                {

                    this.sps_rbsp_data_byte[i][j] = new uint[sps_rbsp_data_length[i][j]];
                    for (k = 0; k <= sps_rbsp_data_length[i][j]; k++)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.sps_rbsp_data_byte[i][j][k]);
                    }
                }

                this.pps_rbsp_data_byte[i] = new uint[num_pps_in_info_set_minus1[i] + 1][];
                for (j = 0; j <= num_pps_in_info_set_minus1[i]; j++)
                {

                    this.pps_rbsp_data_byte[i][j] = new uint[pps_rbsp_data_length[i][j]];
                    for (k = 0; k <= pps_rbsp_data_length[i][j]; k++)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.pps_rbsp_data_byte[i][j][k]);
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
            int whileIndex = -1;
            size += stream.WriteUnsignedIntGolomb(this.num_info_sets_minus1);

            for (i = 0; i <= num_info_sets_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_mcts_sets_minus1[i]);

                for (j = 0; j <= num_mcts_sets_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_mcts_in_set_minus1[i][j]);

                    for (k = 0; k <= num_mcts_in_set_minus1[i][j]; k++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.idx_of_mcts_in_set[i][j][k]);
                    }
                }
                size += stream.WriteUnsignedInt(1, this.slice_reordering_enabled_flag[i]);

                if (slice_reordering_enabled_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_slice_segments_minus1[i]);

                    for (j = 0; j <= num_slice_segments_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntVariable(output_slice_segment_address[i][j], this.output_slice_segment_address[i][j]);
                    }
                }
                size += stream.WriteUnsignedIntGolomb(this.num_vps_in_info_set_minus1[i]);

                for (j = 0; j <= num_vps_in_info_set_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vps_rbsp_data_length[i][j]);
                }
                size += stream.WriteUnsignedIntGolomb(this.num_sps_in_info_set_minus1[i]);

                for (j = 0; j <= num_sps_in_info_set_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sps_rbsp_data_length[i][j]);
                }
                size += stream.WriteUnsignedIntGolomb(this.num_pps_in_info_set_minus1[i]);

                for (j = 0; j <= num_pps_in_info_set_minus1[i]; j++)
                {
                    size += stream.WriteUnsignedInt(3, this.pps_nuh_temporal_id_plus1[i][j]);
                    size += stream.WriteUnsignedIntGolomb(this.pps_rbsp_data_length[i][j]);
                }

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteFixed(1, whileIndex, this.mcts_alignment_bit_equal_to_zero);
                }

                for (j = 0; j <= num_vps_in_info_set_minus1[i]; j++)
                {

                    for (k = 0; k <= vps_rbsp_data_length[i][j]; k++)
                    {
                        size += stream.WriteUnsignedInt(8, this.vps_rbsp_data_byte[i][j][k]);
                    }
                }

                for (j = 0; j <= num_sps_in_info_set_minus1[i]; j++)
                {

                    for (k = 0; k <= sps_rbsp_data_length[i][j]; k++)
                    {
                        size += stream.WriteUnsignedInt(8, this.sps_rbsp_data_byte[i][j][k]);
                    }
                }

                for (j = 0; j <= num_pps_in_info_set_minus1[i]; j++)
                {

                    for (k = 0; k <= pps_rbsp_data_length[i][j]; k++)
                    {
                        size += stream.WriteUnsignedInt(8, this.pps_rbsp_data_byte[i][j][k]);
                    }
                }
            }

            return size;
        }

    }

    /*
  

mcts_extraction_info_nesting( payloadSize ) { 
all_mcts_flag  u(1) 
if( !all_mcts_flag ) {  
num_associated_mcts_minus1 ue(v) 
for( i = 0; i <= num_associated_mcts_minus1; i++ )  
idx_of_associated_mcts[ i ] ue(v) 
}  
num_sei_messages_in_mcts_extraction_nesting_minus1 ue(v) 
while( !byte_aligned() )  
mcts_nesting_zero_bit /* equal to 0 *//* u(1)
for( i = 0; i <= num_sei_messages_in_mcts_extraction_nesting_minus1; i++ )  
sei_message()  
}
    */
    public class MctsExtractionInfoNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte all_mcts_flag;
        public byte AllMctsFlag { get { return all_mcts_flag; } set { all_mcts_flag = value; } }
        private uint num_associated_mcts_minus1;
        public uint NumAssociatedMctsMinus1 { get { return num_associated_mcts_minus1; } set { num_associated_mcts_minus1 = value; } }
        private uint[] idx_of_associated_mcts;
        public uint[] IdxOfAssociatedMcts { get { return idx_of_associated_mcts; } set { idx_of_associated_mcts = value; } }
        private uint num_sei_messages_in_mcts_extraction_nesting_minus1;
        public uint NumSeiMessagesInMctsExtractionNestingMinus1 { get { return num_sei_messages_in_mcts_extraction_nesting_minus1; } set { num_sei_messages_in_mcts_extraction_nesting_minus1 = value; } }
        private Dictionary<int, byte> mcts_nesting_zero_bit = new Dictionary<int, byte>();
        public Dictionary<int, byte> MctsNestingZeroBit { get { return mcts_nesting_zero_bit; } set { mcts_nesting_zero_bit = value; } }
        private SeiMessage[] sei_message;
        public SeiMessage[] SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MctsExtractionInfoNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 1, out this.all_mcts_flag);

            if (all_mcts_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_associated_mcts_minus1);

                this.idx_of_associated_mcts = new uint[num_associated_mcts_minus1 + 1];
                for (i = 0; i <= num_associated_mcts_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.idx_of_associated_mcts[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_sei_messages_in_mcts_extraction_nesting_minus1);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 1, whileIndex, this.mcts_nesting_zero_bit); // equal to 0 
            }

            this.sei_message = new SeiMessage[num_sei_messages_in_mcts_extraction_nesting_minus1 + 1];
            for (i = 0; i <= num_sei_messages_in_mcts_extraction_nesting_minus1; i++)
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
            size += stream.WriteUnsignedInt(1, this.all_mcts_flag);

            if (all_mcts_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_associated_mcts_minus1);

                for (i = 0; i <= num_associated_mcts_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.idx_of_associated_mcts[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.num_sei_messages_in_mcts_extraction_nesting_minus1);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(1, whileIndex, this.mcts_nesting_zero_bit); // equal to 0 
            }

            for (i = 0; i <= num_sei_messages_in_mcts_extraction_nesting_minus1; i++)
            {
                size += stream.WriteClass<SeiMessage>(context, this.sei_message[i]);
            }

            return size;
        }

    }

    /*
  

reserved_sei_message( payloadSize ) { 
for( i = 0; i < payloadSize; i++ )  
reserved_sei_message_payload_byte b(8) 
}
    */
    public class ReservedSeiMessage : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte[] reserved_sei_message_payload_byte;
        public byte[] ReservedSeiMessagePayloadByte { get { return reserved_sei_message_payload_byte; } set { reserved_sei_message_payload_byte = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ReservedSeiMessage(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            this.reserved_sei_message_payload_byte = new byte[payloadSize];
            for (i = 0; i < payloadSize; i++)
            {
                size += stream.ReadBits(size, 8, out this.reserved_sei_message_payload_byte[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            for (i = 0; i < payloadSize; i++)
            {
                size += stream.WriteBits(8, this.reserved_sei_message_payload_byte[i]);
            }

            return size;
        }

    }

    /*


vui_parameters() {  
 aspect_ratio_info_present_flag u(1) 
 if( aspect_ratio_info_present_flag ) {  
  aspect_ratio_idc u(8) 
  if( aspect_ratio_idc == EXTENDED_SAR ) {  
   sar_width u(16) 
   sar_height u(16) 
  }  
 }  
 overscan_info_present_flag u(1) 
 if( overscan_info_present_flag )  
  overscan_appropriate_flag u(1) 
 video_signal_type_present_flag u(1) 
 if( video_signal_type_present_flag ) {  
  video_format u(3) 
  video_full_range_flag u(1) 
  colour_description_present_flag u(1) 
  if( colour_description_present_flag ) {  
   colour_primaries u(8) 
   transfer_characteristics u(8) 
   matrix_coeffs u(8) 
  }  
 }  
 chroma_loc_info_present_flag u(1) 
 if( chroma_loc_info_present_flag ) {  
  chroma_sample_loc_type_top_field ue(v) 
  chroma_sample_loc_type_bottom_field ue(v)
   }  
 neutral_chroma_indication_flag u(1) 
 field_seq_flag u(1) 
 frame_field_info_present_flag u(1) 
 default_display_window_flag u(1) 
 if( default_display_window_flag ) {  
  def_disp_win_left_offset ue(v) 
  def_disp_win_right_offset ue(v) 
  def_disp_win_top_offset ue(v) 
  def_disp_win_bottom_offset ue(v) 
 }  
 vui_timing_info_present_flag u(1) 
 if( vui_timing_info_present_flag ) {  
  vui_num_units_in_tick u(32) 
  vui_time_scale u(32) 
  vui_poc_proportional_to_timing_flag u(1) 
  if( vui_poc_proportional_to_timing_flag )  
   vui_num_ticks_poc_diff_one_minus1 ue(v) 
  vui_hrd_parameters_present_flag u(1) 
  if( vui_hrd_parameters_present_flag )  
   hrd_parameters( 1, sps_max_sub_layers_minus1 )  
 }  
 bitstream_restriction_flag u(1) 
 if( bitstream_restriction_flag ) {  
  tiles_fixed_structure_flag u(1) 
  motion_vectors_over_pic_boundaries_flag u(1) 
  restricted_ref_pic_lists_flag u(1) 
  min_spatial_segmentation_idc ue(v) 
  max_bytes_per_pic_denom ue(v) 
  max_bits_per_min_cu_denom ue(v) 
  log2_max_mv_length_horizontal ue(v) 
  log2_max_mv_length_vertical ue(v) 
 }  
}
    */
    public class VuiParameters : IItuSerializable
    {
        private byte aspect_ratio_info_present_flag;
        public byte AspectRatioInfoPresentFlag { get { return aspect_ratio_info_present_flag; } set { aspect_ratio_info_present_flag = value; } }
        private uint aspect_ratio_idc;
        public uint AspectRatioIdc { get { return aspect_ratio_idc; } set { aspect_ratio_idc = value; } }
        private uint sar_width;
        public uint SarWidth { get { return sar_width; } set { sar_width = value; } }
        private uint sar_height;
        public uint SarHeight { get { return sar_height; } set { sar_height = value; } }
        private byte overscan_info_present_flag;
        public byte OverscanInfoPresentFlag { get { return overscan_info_present_flag; } set { overscan_info_present_flag = value; } }
        private byte overscan_appropriate_flag;
        public byte OverscanAppropriateFlag { get { return overscan_appropriate_flag; } set { overscan_appropriate_flag = value; } }
        private byte video_signal_type_present_flag;
        public byte VideoSignalTypePresentFlag { get { return video_signal_type_present_flag; } set { video_signal_type_present_flag = value; } }
        private uint video_format;
        public uint VideoFormat { get { return video_format; } set { video_format = value; } }
        private byte video_full_range_flag;
        public byte VideoFullRangeFlag { get { return video_full_range_flag; } set { video_full_range_flag = value; } }
        private byte colour_description_present_flag;
        public byte ColourDescriptionPresentFlag { get { return colour_description_present_flag; } set { colour_description_present_flag = value; } }
        private uint colour_primaries;
        public uint ColourPrimaries { get { return colour_primaries; } set { colour_primaries = value; } }
        private uint transfer_characteristics;
        public uint TransferCharacteristics { get { return transfer_characteristics; } set { transfer_characteristics = value; } }
        private uint matrix_coeffs;
        public uint MatrixCoeffs { get { return matrix_coeffs; } set { matrix_coeffs = value; } }
        private byte chroma_loc_info_present_flag;
        public byte ChromaLocInfoPresentFlag { get { return chroma_loc_info_present_flag; } set { chroma_loc_info_present_flag = value; } }
        private uint chroma_sample_loc_type_top_field;
        public uint ChromaSampleLocTypeTopField { get { return chroma_sample_loc_type_top_field; } set { chroma_sample_loc_type_top_field = value; } }
        private uint chroma_sample_loc_type_bottom_field;
        public uint ChromaSampleLocTypeBottomField { get { return chroma_sample_loc_type_bottom_field; } set { chroma_sample_loc_type_bottom_field = value; } }
        private byte neutral_chroma_indication_flag;
        public byte NeutralChromaIndicationFlag { get { return neutral_chroma_indication_flag; } set { neutral_chroma_indication_flag = value; } }
        private byte field_seq_flag;
        public byte FieldSeqFlag { get { return field_seq_flag; } set { field_seq_flag = value; } }
        private byte frame_field_info_present_flag;
        public byte FrameFieldInfoPresentFlag { get { return frame_field_info_present_flag; } set { frame_field_info_present_flag = value; } }
        private byte default_display_window_flag;
        public byte DefaultDisplayWindowFlag { get { return default_display_window_flag; } set { default_display_window_flag = value; } }
        private uint def_disp_win_left_offset;
        public uint DefDispWinLeftOffset { get { return def_disp_win_left_offset; } set { def_disp_win_left_offset = value; } }
        private uint def_disp_win_right_offset;
        public uint DefDispWinRightOffset { get { return def_disp_win_right_offset; } set { def_disp_win_right_offset = value; } }
        private uint def_disp_win_top_offset;
        public uint DefDispWinTopOffset { get { return def_disp_win_top_offset; } set { def_disp_win_top_offset = value; } }
        private uint def_disp_win_bottom_offset;
        public uint DefDispWinBottomOffset { get { return def_disp_win_bottom_offset; } set { def_disp_win_bottom_offset = value; } }
        private byte vui_timing_info_present_flag;
        public byte VuiTimingInfoPresentFlag { get { return vui_timing_info_present_flag; } set { vui_timing_info_present_flag = value; } }
        private uint vui_num_units_in_tick;
        public uint VuiNumUnitsInTick { get { return vui_num_units_in_tick; } set { vui_num_units_in_tick = value; } }
        private uint vui_time_scale;
        public uint VuiTimeScale { get { return vui_time_scale; } set { vui_time_scale = value; } }
        private byte vui_poc_proportional_to_timing_flag;
        public byte VuiPocProportionalToTimingFlag { get { return vui_poc_proportional_to_timing_flag; } set { vui_poc_proportional_to_timing_flag = value; } }
        private uint vui_num_ticks_poc_diff_one_minus1;
        public uint VuiNumTicksPocDiffOneMinus1 { get { return vui_num_ticks_poc_diff_one_minus1; } set { vui_num_ticks_poc_diff_one_minus1 = value; } }
        private byte vui_hrd_parameters_present_flag;
        public byte VuiHrdParametersPresentFlag { get { return vui_hrd_parameters_present_flag; } set { vui_hrd_parameters_present_flag = value; } }
        private HrdParameters hrd_parameters;
        public HrdParameters HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte bitstream_restriction_flag;
        public byte BitstreamRestrictionFlag { get { return bitstream_restriction_flag; } set { bitstream_restriction_flag = value; } }
        private byte tiles_fixed_structure_flag;
        public byte TilesFixedStructureFlag { get { return tiles_fixed_structure_flag; } set { tiles_fixed_structure_flag = value; } }
        private byte motion_vectors_over_pic_boundaries_flag;
        public byte MotionVectorsOverPicBoundariesFlag { get { return motion_vectors_over_pic_boundaries_flag; } set { motion_vectors_over_pic_boundaries_flag = value; } }
        private byte restricted_ref_pic_lists_flag;
        public byte RestrictedRefPicListsFlag { get { return restricted_ref_pic_lists_flag; } set { restricted_ref_pic_lists_flag = value; } }
        private uint min_spatial_segmentation_idc;
        public uint MinSpatialSegmentationIdc { get { return min_spatial_segmentation_idc; } set { min_spatial_segmentation_idc = value; } }
        private uint max_bytes_per_pic_denom;
        public uint MaxBytesPerPicDenom { get { return max_bytes_per_pic_denom; } set { max_bytes_per_pic_denom = value; } }
        private uint max_bits_per_min_cu_denom;
        public uint MaxBitsPerMinCuDenom { get { return max_bits_per_min_cu_denom; } set { max_bits_per_min_cu_denom = value; } }
        private uint log2_max_mv_length_horizontal;
        public uint Log2MaxMvLengthHorizontal { get { return log2_max_mv_length_horizontal; } set { log2_max_mv_length_horizontal = value; } }
        private uint log2_max_mv_length_vertical;
        public uint Log2MaxMvLengthVertical { get { return log2_max_mv_length_vertical; } set { log2_max_mv_length_vertical = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VuiParameters()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.aspect_ratio_info_present_flag);

            if (aspect_ratio_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.aspect_ratio_idc);

                if (aspect_ratio_idc == H265Constants.EXTENDED_SAR)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.sar_width);
                    size += stream.ReadUnsignedInt(size, 16, out this.sar_height);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.overscan_info_present_flag);

            if (overscan_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.overscan_appropriate_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.video_signal_type_present_flag);

            if (video_signal_type_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.video_format);
                size += stream.ReadUnsignedInt(size, 1, out this.video_full_range_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.colour_description_present_flag);

                if (colour_description_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.colour_primaries);
                    size += stream.ReadUnsignedInt(size, 8, out this.transfer_characteristics);
                    size += stream.ReadUnsignedInt(size, 8, out this.matrix_coeffs);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.chroma_loc_info_present_flag);

            if (chroma_loc_info_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_sample_loc_type_top_field);
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_sample_loc_type_bottom_field);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.neutral_chroma_indication_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.field_seq_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.frame_field_info_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.default_display_window_flag);

            if (default_display_window_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.def_disp_win_left_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.def_disp_win_right_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.def_disp_win_top_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.def_disp_win_bottom_offset);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vui_timing_info_present_flag);

            if (vui_timing_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 32, out this.vui_num_units_in_tick);
                size += stream.ReadUnsignedInt(size, 32, out this.vui_time_scale);
                size += stream.ReadUnsignedInt(size, 1, out this.vui_poc_proportional_to_timing_flag);

                if (vui_poc_proportional_to_timing_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vui_num_ticks_poc_diff_one_minus1);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.vui_hrd_parameters_present_flag);

                if (vui_hrd_parameters_present_flag != 0)
                {
                    this.hrd_parameters = new HrdParameters(1, sps_max_sub_layers_minus1);
                    size += stream.ReadClass<HrdParameters>(size, context, this.hrd_parameters);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.bitstream_restriction_flag);

            if (bitstream_restriction_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.tiles_fixed_structure_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.motion_vectors_over_pic_boundaries_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.restricted_ref_pic_lists_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.min_spatial_segmentation_idc);
                size += stream.ReadUnsignedIntGolomb(size, out this.max_bytes_per_pic_denom);
                size += stream.ReadUnsignedIntGolomb(size, out this.max_bits_per_min_cu_denom);
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_horizontal);
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_mv_length_vertical);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.aspect_ratio_info_present_flag);

            if (aspect_ratio_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(8, this.aspect_ratio_idc);

                if (aspect_ratio_idc == H265Constants.EXTENDED_SAR)
                {
                    size += stream.WriteUnsignedInt(16, this.sar_width);
                    size += stream.WriteUnsignedInt(16, this.sar_height);
                }
            }
            size += stream.WriteUnsignedInt(1, this.overscan_info_present_flag);

            if (overscan_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.overscan_appropriate_flag);
            }
            size += stream.WriteUnsignedInt(1, this.video_signal_type_present_flag);

            if (video_signal_type_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(3, this.video_format);
                size += stream.WriteUnsignedInt(1, this.video_full_range_flag);
                size += stream.WriteUnsignedInt(1, this.colour_description_present_flag);

                if (colour_description_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(8, this.colour_primaries);
                    size += stream.WriteUnsignedInt(8, this.transfer_characteristics);
                    size += stream.WriteUnsignedInt(8, this.matrix_coeffs);
                }
            }
            size += stream.WriteUnsignedInt(1, this.chroma_loc_info_present_flag);

            if (chroma_loc_info_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.chroma_sample_loc_type_top_field);
                size += stream.WriteUnsignedIntGolomb(this.chroma_sample_loc_type_bottom_field);
            }
            size += stream.WriteUnsignedInt(1, this.neutral_chroma_indication_flag);
            size += stream.WriteUnsignedInt(1, this.field_seq_flag);
            size += stream.WriteUnsignedInt(1, this.frame_field_info_present_flag);
            size += stream.WriteUnsignedInt(1, this.default_display_window_flag);

            if (default_display_window_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.def_disp_win_left_offset);
                size += stream.WriteUnsignedIntGolomb(this.def_disp_win_right_offset);
                size += stream.WriteUnsignedIntGolomb(this.def_disp_win_top_offset);
                size += stream.WriteUnsignedIntGolomb(this.def_disp_win_bottom_offset);
            }
            size += stream.WriteUnsignedInt(1, this.vui_timing_info_present_flag);

            if (vui_timing_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(32, this.vui_num_units_in_tick);
                size += stream.WriteUnsignedInt(32, this.vui_time_scale);
                size += stream.WriteUnsignedInt(1, this.vui_poc_proportional_to_timing_flag);

                if (vui_poc_proportional_to_timing_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vui_num_ticks_poc_diff_one_minus1);
                }
                size += stream.WriteUnsignedInt(1, this.vui_hrd_parameters_present_flag);

                if (vui_hrd_parameters_present_flag != 0)
                {
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters);
                }
            }
            size += stream.WriteUnsignedInt(1, this.bitstream_restriction_flag);

            if (bitstream_restriction_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.tiles_fixed_structure_flag);
                size += stream.WriteUnsignedInt(1, this.motion_vectors_over_pic_boundaries_flag);
                size += stream.WriteUnsignedInt(1, this.restricted_ref_pic_lists_flag);
                size += stream.WriteUnsignedIntGolomb(this.min_spatial_segmentation_idc);
                size += stream.WriteUnsignedIntGolomb(this.max_bytes_per_pic_denom);
                size += stream.WriteUnsignedIntGolomb(this.max_bits_per_min_cu_denom);
                size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_horizontal);
                size += stream.WriteUnsignedIntGolomb(this.log2_max_mv_length_vertical);
            }

            return size;
        }

    }

    /*
 

hrd_parameters( commonInfPresentFlag, maxNumSubLayersMinus1 ) {  
 if( commonInfPresentFlag ) {  
  nal_hrd_parameters_present_flag u(1) 
  vcl_hrd_parameters_present_flag u(1) 
  if( nal_hrd_parameters_present_flag  ||  vcl_hrd_parameters_present_flag ) {  
   sub_pic_hrd_params_present_flag u(1) 
   if( sub_pic_hrd_params_present_flag ) {  
    tick_divisor_minus2 u(8) 
    du_cpb_removal_delay_increment_length_minus1 u(5) 
    sub_pic_cpb_params_in_pic_timing_sei_flag u(1) 
    dpb_output_delay_du_length_minus1 u(5) 
   }  
   bit_rate_scale u(4) 
   cpb_size_scale u(4) 
   if( sub_pic_hrd_params_present_flag )  
    cpb_size_du_scale u(4) 
   initial_cpb_removal_delay_length_minus1 u(5) 
   au_cpb_removal_delay_length_minus1 u(5) 
   dpb_output_delay_length_minus1 u(5) 
  }  
 }  
 for( i = 0; i <= maxNumSubLayersMinus1; i++ ) {  
  fixed_pic_rate_general_flag[ i ] u(1) 
  if( !fixed_pic_rate_general_flag[ i ] )  
   fixed_pic_rate_within_cvs_flag[ i ] u(1) 
  if( fixed_pic_rate_within_cvs_flag[ i ] )  
   elemental_duration_in_tc_minus1[ i ] ue(v) 
  else  
   low_delay_hrd_flag[ i ] u(1) 
  if( !low_delay_hrd_flag[ i ] )  
   cpb_cnt_minus1[ i ] ue(v) 
  if( nal_hrd_parameters_present_flag )  
   sub_layer_hrd_parameters( i )  
  if( vcl_hrd_parameters_present_flag )  
   sub_layer_hrd_parameters( i )  
 }  
}
    */
    public class HrdParameters : IItuSerializable
    {
        private uint commonInfPresentFlag;
        public uint CommonInfPresentFlag { get { return commonInfPresentFlag; } set { commonInfPresentFlag = value; } }
        private uint maxNumSubLayersMinus1;
        public uint MaxNumSubLayersMinus1 { get { return maxNumSubLayersMinus1; } set { maxNumSubLayersMinus1 = value; } }
        private byte nal_hrd_parameters_present_flag;
        public byte NalHrdParametersPresentFlag { get { return nal_hrd_parameters_present_flag; } set { nal_hrd_parameters_present_flag = value; } }
        private byte vcl_hrd_parameters_present_flag;
        public byte VclHrdParametersPresentFlag { get { return vcl_hrd_parameters_present_flag; } set { vcl_hrd_parameters_present_flag = value; } }
        private byte sub_pic_hrd_params_present_flag;
        public byte SubPicHrdParamsPresentFlag { get { return sub_pic_hrd_params_present_flag; } set { sub_pic_hrd_params_present_flag = value; } }
        private uint tick_divisor_minus2;
        public uint TickDivisorMinus2 { get { return tick_divisor_minus2; } set { tick_divisor_minus2 = value; } }
        private uint du_cpb_removal_delay_increment_length_minus1;
        public uint DuCpbRemovalDelayIncrementLengthMinus1 { get { return du_cpb_removal_delay_increment_length_minus1; } set { du_cpb_removal_delay_increment_length_minus1 = value; } }
        private byte sub_pic_cpb_params_in_pic_timing_sei_flag;
        public byte SubPicCpbParamsInPicTimingSeiFlag { get { return sub_pic_cpb_params_in_pic_timing_sei_flag; } set { sub_pic_cpb_params_in_pic_timing_sei_flag = value; } }
        private uint dpb_output_delay_du_length_minus1;
        public uint DpbOutputDelayDuLengthMinus1 { get { return dpb_output_delay_du_length_minus1; } set { dpb_output_delay_du_length_minus1 = value; } }
        private uint bit_rate_scale;
        public uint BitRateScale { get { return bit_rate_scale; } set { bit_rate_scale = value; } }
        private uint cpb_size_scale;
        public uint CpbSizeScale { get { return cpb_size_scale; } set { cpb_size_scale = value; } }
        private uint cpb_size_du_scale;
        public uint CpbSizeDuScale { get { return cpb_size_du_scale; } set { cpb_size_du_scale = value; } }
        private uint initial_cpb_removal_delay_length_minus1;
        public uint InitialCpbRemovalDelayLengthMinus1 { get { return initial_cpb_removal_delay_length_minus1; } set { initial_cpb_removal_delay_length_minus1 = value; } }
        private uint au_cpb_removal_delay_length_minus1;
        public uint AuCpbRemovalDelayLengthMinus1 { get { return au_cpb_removal_delay_length_minus1; } set { au_cpb_removal_delay_length_minus1 = value; } }
        private uint dpb_output_delay_length_minus1;
        public uint DpbOutputDelayLengthMinus1 { get { return dpb_output_delay_length_minus1; } set { dpb_output_delay_length_minus1 = value; } }
        private byte[] fixed_pic_rate_general_flag;
        public byte[] FixedPicRateGeneralFlag { get { return fixed_pic_rate_general_flag; } set { fixed_pic_rate_general_flag = value; } }
        private byte[] fixed_pic_rate_within_cvs_flag;
        public byte[] FixedPicRateWithinCvsFlag { get { return fixed_pic_rate_within_cvs_flag; } set { fixed_pic_rate_within_cvs_flag = value; } }
        private uint[] elemental_duration_in_tc_minus1;
        public uint[] ElementalDurationInTcMinus1 { get { return elemental_duration_in_tc_minus1; } set { elemental_duration_in_tc_minus1 = value; } }
        private byte[] low_delay_hrd_flag;
        public byte[] LowDelayHrdFlag { get { return low_delay_hrd_flag; } set { low_delay_hrd_flag = value; } }
        private uint[] cpb_cnt_minus1;
        public uint[] CpbCntMinus1 { get { return cpb_cnt_minus1; } set { cpb_cnt_minus1 = value; } }
        private SubLayerHrdParameters[] sub_layer_hrd_parameters;
        public SubLayerHrdParameters[] SubLayerHrdParameters { get { return sub_layer_hrd_parameters; } set { sub_layer_hrd_parameters = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public HrdParameters(uint commonInfPresentFlag, uint maxNumSubLayersMinus1)
        {
            this.commonInfPresentFlag = commonInfPresentFlag;
            this.maxNumSubLayersMinus1 = maxNumSubLayersMinus1;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            if (commonInfPresentFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.nal_hrd_parameters_present_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.vcl_hrd_parameters_present_flag);

                if (nal_hrd_parameters_present_flag != 0 || vcl_hrd_parameters_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sub_pic_hrd_params_present_flag);

                    if (sub_pic_hrd_params_present_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 8, out this.tick_divisor_minus2);
                        size += stream.ReadUnsignedInt(size, 5, out this.du_cpb_removal_delay_increment_length_minus1);
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_pic_cpb_params_in_pic_timing_sei_flag);
                        size += stream.ReadUnsignedInt(size, 5, out this.dpb_output_delay_du_length_minus1);
                    }
                    size += stream.ReadUnsignedInt(size, 4, out this.bit_rate_scale);
                    size += stream.ReadUnsignedInt(size, 4, out this.cpb_size_scale);

                    if (sub_pic_hrd_params_present_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 4, out this.cpb_size_du_scale);
                    }
                    size += stream.ReadUnsignedInt(size, 5, out this.initial_cpb_removal_delay_length_minus1);
                    size += stream.ReadUnsignedInt(size, 5, out this.au_cpb_removal_delay_length_minus1);
                    size += stream.ReadUnsignedInt(size, 5, out this.dpb_output_delay_length_minus1);
                }
            }

            this.fixed_pic_rate_general_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.fixed_pic_rate_within_cvs_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.elemental_duration_in_tc_minus1 = new uint[maxNumSubLayersMinus1 + 1];
            this.low_delay_hrd_flag = new byte[maxNumSubLayersMinus1 + 1];
            this.cpb_cnt_minus1 = new uint[maxNumSubLayersMinus1 + 1];
            this.sub_layer_hrd_parameters = new SubLayerHrdParameters[maxNumSubLayersMinus1 + 1];
            for (i = 0; i <= maxNumSubLayersMinus1; i++)
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
                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.low_delay_hrd_flag[i]);
                }

                if (low_delay_hrd_flag[i] == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.cpb_cnt_minus1[i]);
                }

                if (nal_hrd_parameters_present_flag != 0)
                {
                    this.sub_layer_hrd_parameters[i] = new SubLayerHrdParameters(i);
                    size += stream.ReadClass<SubLayerHrdParameters>(size, context, this.sub_layer_hrd_parameters[i]);
                }

                if (vcl_hrd_parameters_present_flag != 0)
                {
                    this.sub_layer_hrd_parameters[i] = new SubLayerHrdParameters(i);
                    size += stream.ReadClass<SubLayerHrdParameters>(size, context, this.sub_layer_hrd_parameters[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            if (commonInfPresentFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.nal_hrd_parameters_present_flag);
                size += stream.WriteUnsignedInt(1, this.vcl_hrd_parameters_present_flag);

                if (nal_hrd_parameters_present_flag != 0 || vcl_hrd_parameters_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sub_pic_hrd_params_present_flag);

                    if (sub_pic_hrd_params_present_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(8, this.tick_divisor_minus2);
                        size += stream.WriteUnsignedInt(5, this.du_cpb_removal_delay_increment_length_minus1);
                        size += stream.WriteUnsignedInt(1, this.sub_pic_cpb_params_in_pic_timing_sei_flag);
                        size += stream.WriteUnsignedInt(5, this.dpb_output_delay_du_length_minus1);
                    }
                    size += stream.WriteUnsignedInt(4, this.bit_rate_scale);
                    size += stream.WriteUnsignedInt(4, this.cpb_size_scale);

                    if (sub_pic_hrd_params_present_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(4, this.cpb_size_du_scale);
                    }
                    size += stream.WriteUnsignedInt(5, this.initial_cpb_removal_delay_length_minus1);
                    size += stream.WriteUnsignedInt(5, this.au_cpb_removal_delay_length_minus1);
                    size += stream.WriteUnsignedInt(5, this.dpb_output_delay_length_minus1);
                }
            }

            for (i = 0; i <= maxNumSubLayersMinus1; i++)
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
                else
                {
                    size += stream.WriteUnsignedInt(1, this.low_delay_hrd_flag[i]);
                }

                if (low_delay_hrd_flag[i] == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.cpb_cnt_minus1[i]);
                }

                if (nal_hrd_parameters_present_flag != 0)
                {
                    size += stream.WriteClass<SubLayerHrdParameters>(context, this.sub_layer_hrd_parameters[i]);
                }

                if (vcl_hrd_parameters_present_flag != 0)
                {
                    size += stream.WriteClass<SubLayerHrdParameters>(context, this.sub_layer_hrd_parameters[i]);
                }
            }

            return size;
        }

    }

    /*
 

sub_layer_hrd_parameters( subLayerId ) { 
for( i = 0; i < CpbCnt; i++ ) {  
bit_rate_value_minus1[ i ] ue(v)
cpb_size_value_minus1[ i ]  ue(v) 
if( sub_pic_hrd_params_present_flag ) {  
cpb_size_du_value_minus1[ i ] ue(v)
bit_rate_du_value_minus1[ i ] ue(v)
}  
cbr_flag[ i ] u(1)
}  
}
    */
    public class SubLayerHrdParameters : IItuSerializable
    {
        private uint subLayerId;
        public uint SubLayerId { get { return subLayerId; } set { subLayerId = value; } }
        private uint[] bit_rate_value_minus1;
        public uint[] BitRateValueMinus1 { get { return bit_rate_value_minus1; } set { bit_rate_value_minus1 = value; } }
        private uint[] cpb_size_value_minus1;
        public uint[] CpbSizeValueMinus1 { get { return cpb_size_value_minus1; } set { cpb_size_value_minus1 = value; } }
        private uint[] cpb_size_du_value_minus1;
        public uint[] CpbSizeDuValueMinus1 { get { return cpb_size_du_value_minus1; } set { cpb_size_du_value_minus1 = value; } }
        private uint[] bit_rate_du_value_minus1;
        public uint[] BitRateDuValueMinus1 { get { return bit_rate_du_value_minus1; } set { bit_rate_du_value_minus1 = value; } }
        private byte[] cbr_flag;
        public byte[] CbrFlag { get { return cbr_flag; } set { cbr_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubLayerHrdParameters(uint subLayerId)
        {
            this.subLayerId = subLayerId;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            this.bit_rate_value_minus1 = new uint[CpbCnt];
            this.cpb_size_value_minus1 = new uint[CpbCnt];
            this.cpb_size_du_value_minus1 = new uint[CpbCnt];
            this.bit_rate_du_value_minus1 = new uint[CpbCnt];
            this.cbr_flag = new byte[CpbCnt];
            for (i = 0; i < CpbCnt; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_rate_value_minus1[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.cpb_size_value_minus1[i]);

                if (sub_pic_hrd_params_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.cpb_size_du_value_minus1[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.bit_rate_du_value_minus1[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.cbr_flag[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;

            for (i = 0; i < CpbCnt; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.bit_rate_value_minus1[i]);
                size += stream.WriteUnsignedIntGolomb(this.cpb_size_value_minus1[i]);

                if (sub_pic_hrd_params_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.cpb_size_du_value_minus1[i]);
                    size += stream.WriteUnsignedIntGolomb(this.bit_rate_du_value_minus1[i]);
                }
                size += stream.WriteUnsignedInt(1, this.cbr_flag[i]);
            }

            return size;
        }

    }

    /*
  

vps_extension() {  
 if( vps_max_layers_minus1 > 0  &&  vps_base_layer_internal_flag )  
  profile_tier_level( 0, vps_max_sub_layers_minus1 )  
 splitting_flag u(1) 
 for( i = 0, NumScalabilityTypes = 0; i < 16; i++ ) {  
  scalability_mask_flag[ i ] u(1) 
  NumScalabilityTypes  +=  scalability_mask_flag[ i ]  
 }  
 for( j = 0; j < ( NumScalabilityTypes - splitting_flag ); j++ )  
  dimension_id_len_minus1[ j ] u(3) 
 vps_nuh_layer_id_present_flag u(1) 
 for( i = 1; i <= MaxLayersMinus1; i++ ) {  
  if( vps_nuh_layer_id_present_flag )   
   layer_id_in_nuh[ i ] u(6) 
  if( !splitting_flag )  
     for( j = 0; j < NumScalabilityTypes; j++ )  
    dimension_id[ i ][ j ] u(v) 
 }  
 view_id_len u(4) 
 if( view_id_len > 0 )  
  for( i = 0; i < NumViews; i++ )  
   view_id_val[ i ] u(v) 
 for( i = 1; i <= MaxLayersMinus1; i++ )  
  for( j = 0; j < i; j++ )  
   direct_dependency_flag[ i ][ j ] u(1) 
 if( NumIndependentLayers > 1 )   
  num_add_layer_sets ue(v) 
 for( i = 0; i < num_add_layer_sets; i++ )   
  for( j = 1; j < NumIndependentLayers; j++ )  
   highest_layer_idx_plus1[ i ][ j ] u(v) 
 vps_sub_layers_max_minus1_present_flag u(1) 
 if( vps_sub_layers_max_minus1_present_flag )  
  for( i = 0; i <= MaxLayersMinus1; i++ )  
   sub_layers_vps_max_minus1[ i ] u(3) 
 max_tid_ref_present_flag u(1) 
 if( max_tid_ref_present_flag )  
  for( i = 0; i < MaxLayersMinus1; i++ )  
   for( j = i + 1; j <= MaxLayersMinus1; j++ )  
    if( direct_dependency_flag[ j ][ i ] )  
     max_tid_il_ref_pics_plus1[ i ][ j ] u(3) 
 default_ref_layers_active_flag u(1) 
 vps_num_profile_tier_level_minus1 ue(v) 
 for( i = vps_base_layer_internal_flag != 0 ?  2 : 1; 
          i <= vps_num_profile_tier_level_minus1; i++ ) { 
 
  vps_profile_present_flag[ i ] u(1) 
  profile_tier_level( vps_profile_present_flag[ i ], vps_max_sub_layers_minus1 )  
 }  
 if( NumLayerSets > 1 ) {  
  num_add_olss ue(v) 
  default_output_layer_idc u(2) 
 }  
 NumOutputLayerSets = num_add_olss + NumLayerSets  
 for( i = 1; i < NumOutputLayerSets; i++ ) {  
  if( NumLayerSets > 2  &&  i >= NumLayerSets )  
   layer_set_idx_for_ols_minus1[ i ] u(v) 
  if( i > vps_num_layer_sets_minus1  ||  defaultOutputLayerIdc == 2 )  
   for( j = 0; j < NumLayersInIdList[ OlsIdxToLsIdx[ i ] ]; j++ )  
    output_layer_flag[ i ][ j ] u(1) 
  for( j = 0; j < NumLayersInIdList[ OlsIdxToLsIdx[ i ] ]; j++ )  
   if( NecessaryLayerFlag[ i ][ j ]  &&  vps_num_profile_tier_level_minus1 > 0 )  
    profile_tier_level_idx[ i ][ j ] u(v) 
  if( NumOutputLayersInOutputLayerSet[ i ] == 1 
   &&  NumDirectRefLayers[ OlsHighestOutputLayerId[ i ] ] > 0 ) 
 
   alt_output_layer_flag[ i ] u(1) 
    }  
 vps_num_rep_formats_minus1 ue(v) 
 for( i = 0; i <= vps_num_rep_formats_minus1; i++ )  
  rep_format()  
 if( vps_num_rep_formats_minus1 > 0 )  
  rep_format_idx_present_flag u(1) 
 if( rep_format_idx_present_flag )   
  for( i = vps_base_layer_internal_flag != 0 ?  1 : 0; i <= MaxLayersMinus1; i++ )  
   vps_rep_format_idx[ i ] u(v) 
 max_one_active_ref_layer_flag u(1) 
 vps_poc_lsb_aligned_flag u(1) 
 for( i = 1; i <= MaxLayersMinus1; i++ )  
  if( NumDirectRefLayers[ layer_id_in_nuh[ i ] ] == 0 )  
   poc_lsb_not_present_flag[ i ] u(1) 
 dpb_size()  
 direct_dep_type_len_minus2 ue(v) 
 direct_dependency_all_layers_flag u(1) 
 if( direct_dependency_all_layers_flag )  
  direct_dependency_all_layers_type u(v) 
 else {  
  for( i = vps_base_layer_internal_flag != 0 ?  1 : 2; i <= MaxLayersMinus1; i++ )  
   for( j = vps_base_layer_internal_flag != 0 ?  0 : 1; j < i; j++ )  
    if( direct_dependency_flag[ i ][ j ] )  
     direct_dependency_type[ i ][ j ] u(v) 
 }  
 vps_non_vui_extension_length ue(v) 
 for( i = 1; i <= vps_non_vui_extension_length; i++ )  
  vps_non_vui_extension_data_byte u(8) 
 vps_vui_present_flag u(1) 
 if( vps_vui_present_flag ) {  
  while( !byte_aligned() )  
   vps_vui_alignment_bit_equal_to_one u(1) 
  vps_vui()  
 }  
}
    */
    public class VpsExtension : IItuSerializable
    {
        private ProfileTierLevel profile_tier_level;
        public ProfileTierLevel ProfileTierLevel { get { return profile_tier_level; } set { profile_tier_level = value; } }
        private byte splitting_flag;
        public byte SplittingFlag { get { return splitting_flag; } set { splitting_flag = value; } }
        private byte[] scalability_mask_flag;
        public byte[] ScalabilityMaskFlag { get { return scalability_mask_flag; } set { scalability_mask_flag = value; } }
        private uint[] dimension_id_len_minus1;
        public uint[] DimensionIdLenMinus1 { get { return dimension_id_len_minus1; } set { dimension_id_len_minus1 = value; } }
        private byte vps_nuh_layer_id_present_flag;
        public byte VpsNuhLayerIdPresentFlag { get { return vps_nuh_layer_id_present_flag; } set { vps_nuh_layer_id_present_flag = value; } }
        private uint[] layer_id_in_nuh;
        public uint[] LayerIdInNuh { get { return layer_id_in_nuh; } set { layer_id_in_nuh = value; } }
        private uint[][] dimension_id;
        public uint[][] DimensionId { get { return dimension_id; } set { dimension_id = value; } }
        private uint view_id_len;
        public uint ViewIdLen { get { return view_id_len; } set { view_id_len = value; } }
        private uint[] view_id_val;
        public uint[] ViewIdVal { get { return view_id_val; } set { view_id_val = value; } }
        private byte[][] direct_dependency_flag;
        public byte[][] DirectDependencyFlag { get { return direct_dependency_flag; } set { direct_dependency_flag = value; } }
        private uint num_add_layer_sets;
        public uint NumAddLayerSets { get { return num_add_layer_sets; } set { num_add_layer_sets = value; } }
        private uint[][] highest_layer_idx_plus1;
        public uint[][] HighestLayerIdxPlus1 { get { return highest_layer_idx_plus1; } set { highest_layer_idx_plus1 = value; } }
        private byte vps_sub_layers_max_minus1_present_flag;
        public byte VpsSubLayersMaxMinus1PresentFlag { get { return vps_sub_layers_max_minus1_present_flag; } set { vps_sub_layers_max_minus1_present_flag = value; } }
        private uint[] sub_layers_vps_max_minus1;
        public uint[] SubLayersVpsMaxMinus1 { get { return sub_layers_vps_max_minus1; } set { sub_layers_vps_max_minus1 = value; } }
        private byte max_tid_ref_present_flag;
        public byte MaxTidRefPresentFlag { get { return max_tid_ref_present_flag; } set { max_tid_ref_present_flag = value; } }
        private uint[][] max_tid_il_ref_pics_plus1;
        public uint[][] MaxTidIlRefPicsPlus1 { get { return max_tid_il_ref_pics_plus1; } set { max_tid_il_ref_pics_plus1 = value; } }
        private byte default_ref_layers_active_flag;
        public byte DefaultRefLayersActiveFlag { get { return default_ref_layers_active_flag; } set { default_ref_layers_active_flag = value; } }
        private uint vps_num_profile_tier_level_minus1;
        public uint VpsNumProfileTierLevelMinus1 { get { return vps_num_profile_tier_level_minus1; } set { vps_num_profile_tier_level_minus1 = value; } }
        private byte[] vps_profile_present_flag;
        public byte[] VpsProfilePresentFlag { get { return vps_profile_present_flag; } set { vps_profile_present_flag = value; } }
        private ProfileTierLevel[] profile_tier_level0;
        public ProfileTierLevel[] ProfileTierLevel0 { get { return profile_tier_level0; } set { profile_tier_level0 = value; } }
        private uint num_add_olss;
        public uint NumAddOlss { get { return num_add_olss; } set { num_add_olss = value; } }
        private uint default_output_layer_idc;
        public uint DefaultOutputLayerIdc { get { return default_output_layer_idc; } set { default_output_layer_idc = value; } }
        private uint[] layer_set_idx_for_ols_minus1;
        public uint[] LayerSetIdxForOlsMinus1 { get { return layer_set_idx_for_ols_minus1; } set { layer_set_idx_for_ols_minus1 = value; } }
        private byte[][] output_layer_flag;
        public byte[][] OutputLayerFlag { get { return output_layer_flag; } set { output_layer_flag = value; } }
        private uint[][] profile_tier_level_idx;
        public uint[][] ProfileTierLevelIdx { get { return profile_tier_level_idx; } set { profile_tier_level_idx = value; } }
        private byte[] alt_output_layer_flag;
        public byte[] AltOutputLayerFlag { get { return alt_output_layer_flag; } set { alt_output_layer_flag = value; } }
        private uint vps_num_rep_formats_minus1;
        public uint VpsNumRepFormatsMinus1 { get { return vps_num_rep_formats_minus1; } set { vps_num_rep_formats_minus1 = value; } }
        private RepFormat[] rep_format;
        public RepFormat[] RepFormat { get { return rep_format; } set { rep_format = value; } }
        private byte rep_format_idx_present_flag;
        public byte RepFormatIdxPresentFlag { get { return rep_format_idx_present_flag; } set { rep_format_idx_present_flag = value; } }
        private uint[] vps_rep_format_idx;
        public uint[] VpsRepFormatIdx { get { return vps_rep_format_idx; } set { vps_rep_format_idx = value; } }
        private byte max_one_active_ref_layer_flag;
        public byte MaxOneActiveRefLayerFlag { get { return max_one_active_ref_layer_flag; } set { max_one_active_ref_layer_flag = value; } }
        private byte vps_poc_lsb_aligned_flag;
        public byte VpsPocLsbAlignedFlag { get { return vps_poc_lsb_aligned_flag; } set { vps_poc_lsb_aligned_flag = value; } }
        private byte[] poc_lsb_not_present_flag;
        public byte[] PocLsbNotPresentFlag { get { return poc_lsb_not_present_flag; } set { poc_lsb_not_present_flag = value; } }
        private DpbSize dpb_size;
        public DpbSize DpbSize { get { return dpb_size; } set { dpb_size = value; } }
        private uint direct_dep_type_len_minus2;
        public uint DirectDepTypeLenMinus2 { get { return direct_dep_type_len_minus2; } set { direct_dep_type_len_minus2 = value; } }
        private byte direct_dependency_all_layers_flag;
        public byte DirectDependencyAllLayersFlag { get { return direct_dependency_all_layers_flag; } set { direct_dependency_all_layers_flag = value; } }
        private uint direct_dependency_all_layers_type;
        public uint DirectDependencyAllLayersType { get { return direct_dependency_all_layers_type; } set { direct_dependency_all_layers_type = value; } }
        private uint[][] direct_dependency_type;
        public uint[][] DirectDependencyType { get { return direct_dependency_type; } set { direct_dependency_type = value; } }
        private uint vps_non_vui_extension_length;
        public uint VpsNonVuiExtensionLength { get { return vps_non_vui_extension_length; } set { vps_non_vui_extension_length = value; } }
        private uint[] vps_non_vui_extension_data_byte;
        public uint[] VpsNonVuiExtensionDataByte { get { return vps_non_vui_extension_data_byte; } set { vps_non_vui_extension_data_byte = value; } }
        private byte vps_vui_present_flag;
        public byte VpsVuiPresentFlag { get { return vps_vui_present_flag; } set { vps_vui_present_flag = value; } }
        private Dictionary<int, byte> vps_vui_alignment_bit_equal_to_one = new Dictionary<int, byte>();
        public Dictionary<int, byte> VpsVuiAlignmentBitEqualToOne { get { return vps_vui_alignment_bit_equal_to_one; } set { vps_vui_alignment_bit_equal_to_one = value; } }
        private VpsVui vps_vui;
        public VpsVui VpsVui { get { return vps_vui; } set { vps_vui = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VpsExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint NumScalabilityTypes = 0;
            uint j = 0;
            uint NumOutputLayerSets = 0;
            int whileIndex = -1;

            if (vps_max_layers_minus1 > 0 && vps_base_layer_internal_flag != 0)
            {
                this.profile_tier_level = new ProfileTierLevel(0, vps_max_sub_layers_minus1);
                size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.splitting_flag);

            this.scalability_mask_flag = new byte[16];
            for (i = 0, NumScalabilityTypes = 0; i < 16; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.scalability_mask_flag[i]);
                NumScalabilityTypes += scalability_mask_flag[i];
            }

            this.dimension_id_len_minus1 = new uint[(NumScalabilityTypes - splitting_flag)];
            for (j = 0; j < (NumScalabilityTypes - splitting_flag); j++)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.dimension_id_len_minus1[j]);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_nuh_layer_id_present_flag);

            this.layer_id_in_nuh = new uint[MaxLayersMinus1 + 1];
            this.dimension_id = new uint[MaxLayersMinus1 + 1][];
            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                if (vps_nuh_layer_id_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 6, out this.layer_id_in_nuh[i]);
                }

                if (splitting_flag == 0)
                {

                    this.dimension_id[i] = new uint[NumScalabilityTypes];
                    for (j = 0; j < NumScalabilityTypes; j++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, dimension_id, out this.dimension_id[i][j]);
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 4, out this.view_id_len);

            if (view_id_len > 0)
            {

                this.view_id_val = new uint[NumViews];
                for (i = 0; i < NumViews; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, view_id_val, out this.view_id_val[i]);
                }
            }

            this.direct_dependency_flag = new byte[MaxLayersMinus1 + 1][];
            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                this.direct_dependency_flag[i] = new byte[i];
                for (j = 0; j < i; j++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.direct_dependency_flag[i][j]);
                }
            }

            if (NumIndependentLayers > 1)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_add_layer_sets);
            }

            this.highest_layer_idx_plus1 = new uint[num_add_layer_sets][];
            for (i = 0; i < num_add_layer_sets; i++)
            {

                this.highest_layer_idx_plus1[i] = new uint[NumIndependentLayers];
                for (j = 1; j < NumIndependentLayers; j++)
                {
                    size += stream.ReadUnsignedIntVariable(size, highest_layer_idx_plus1, out this.highest_layer_idx_plus1[i][j]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_sub_layers_max_minus1_present_flag);

            if (vps_sub_layers_max_minus1_present_flag != 0)
            {

                this.sub_layers_vps_max_minus1 = new uint[MaxLayersMinus1 + 1];
                for (i = 0; i <= MaxLayersMinus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 3, out this.sub_layers_vps_max_minus1[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.max_tid_ref_present_flag);

            if (max_tid_ref_present_flag != 0)
            {

                this.max_tid_il_ref_pics_plus1 = new uint[MaxLayersMinus1 + 1][];
                for (i = 0; i < MaxLayersMinus1; i++)
                {

                    this.max_tid_il_ref_pics_plus1[i] = new uint[MaxLayersMinus1 + 1];
                    for (j = i + 1; j <= MaxLayersMinus1; j++)
                    {

                        if (direct_dependency_flag[j][i] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 3, out this.max_tid_il_ref_pics_plus1[i][j]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.default_ref_layers_active_flag);
            size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_profile_tier_level_minus1);

            this.vps_profile_present_flag = new byte[vps_num_profile_tier_level_minus1 + 1];
            this.profile_tier_level0 = new ProfileTierLevel[vps_num_profile_tier_level_minus1 + 1];
            for (i = vps_base_layer_internal_flag != 0 ? 2 : 1;
          i <= vps_num_profile_tier_level_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.vps_profile_present_flag[i]);
                this.profile_tier_level0[i] = new ProfileTierLevel(vps_profile_present_flag[i], vps_max_sub_layers_minus1);
                size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level0[i]);
            }

            if (NumLayerSets > 1)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_add_olss);
                size += stream.ReadUnsignedInt(size, 2, out this.default_output_layer_idc);
            }
            NumOutputLayerSets = num_add_olss + NumLayerSets;

            this.layer_set_idx_for_ols_minus1 = new uint[NumOutputLayerSets];
            this.output_layer_flag = new byte[NumOutputLayerSets][];
            this.profile_tier_level_idx = new uint[NumOutputLayerSets][];
            this.alt_output_layer_flag = new byte[NumOutputLayerSets];
            for (i = 1; i < NumOutputLayerSets; i++)
            {

                if (NumLayerSets > 2 && i >= NumLayerSets)
                {
                    size += stream.ReadUnsignedIntVariable(size, layer_set_idx_for_ols_minus1, out this.layer_set_idx_for_ols_minus1[i]);
                }

                if (i > vps_num_layer_sets_minus1 || defaultOutputLayerIdc == 2)
                {

                    this.output_layer_flag[i] = new byte[NumLayersInIdList[OlsIdxToLsIdx[i]]];
                    for (j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.output_layer_flag[i][j]);
                    }
                }

                this.profile_tier_level_idx[i] = new uint[NumLayersInIdList[OlsIdxToLsIdx[i]]];
                for (j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                {

                    if (NecessaryLayerFlag[i][j] != 0 && vps_num_profile_tier_level_minus1 > 0)
                    {
                        size += stream.ReadUnsignedIntVariable(size, profile_tier_level_idx, out this.profile_tier_level_idx[i][j]);
                    }
                }

                if (NumOutputLayersInOutputLayerSet[i] == 1
   && NumDirectRefLayers[OlsHighestOutputLayerId[i]] > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.alt_output_layer_flag[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_rep_formats_minus1);

            this.rep_format = new RepFormat[vps_num_rep_formats_minus1 + 1];
            for (i = 0; i <= vps_num_rep_formats_minus1; i++)
            {
                this.rep_format[i] = new RepFormat();
                size += stream.ReadClass<RepFormat>(size, context, this.rep_format[i]);
            }

            if (vps_num_rep_formats_minus1 > 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.rep_format_idx_present_flag);
            }

            if (rep_format_idx_present_flag != 0)
            {

                this.vps_rep_format_idx = new uint[MaxLayersMinus1 + 1];
                for (i = vps_base_layer_internal_flag != 0 ? 1 : 0; i <= MaxLayersMinus1; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, vps_rep_format_idx, out this.vps_rep_format_idx[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.max_one_active_ref_layer_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vps_poc_lsb_aligned_flag);

            this.poc_lsb_not_present_flag = new byte[MaxLayersMinus1 + 1];
            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                if (NumDirectRefLayers[layer_id_in_nuh[i]] == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.poc_lsb_not_present_flag[i]);
                }
            }
            this.dpb_size = new DpbSize();
            size += stream.ReadClass<DpbSize>(size, context, this.dpb_size);
            size += stream.ReadUnsignedIntGolomb(size, out this.direct_dep_type_len_minus2);
            size += stream.ReadUnsignedInt(size, 1, out this.direct_dependency_all_layers_flag);

            if (direct_dependency_all_layers_flag != 0)
            {
                size += stream.ReadUnsignedIntVariable(size, direct_dependency_all_layers_type, out this.direct_dependency_all_layers_type);
            }
            else
            {

                this.direct_dependency_type = new uint[MaxLayersMinus1 + 1][];
                for (i = vps_base_layer_internal_flag != 0 ? 1 : 2; i <= MaxLayersMinus1; i++)
                {

                    this.direct_dependency_type[i] = new uint[i];
                    for (j = vps_base_layer_internal_flag != 0 ? 0 : 1; j < i; j++)
                    {

                        if (direct_dependency_flag[i][j] != 0)
                        {
                            size += stream.ReadUnsignedIntVariable(size, direct_dependency_type, out this.direct_dependency_type[i][j]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.vps_non_vui_extension_length);

            this.vps_non_vui_extension_data_byte = new uint[vps_non_vui_extension_length];
            for (i = 1; i <= vps_non_vui_extension_length; i++)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.vps_non_vui_extension_data_byte[i]);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_vui_present_flag);

            if (vps_vui_present_flag != 0)
            {

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.vps_vui_alignment_bit_equal_to_one);
                }
                this.vps_vui = new VpsVui();
                size += stream.ReadClass<VpsVui>(size, context, this.vps_vui);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint NumScalabilityTypes = 0;
            uint j = 0;
            uint NumOutputLayerSets = 0;
            int whileIndex = -1;

            if (vps_max_layers_minus1 > 0 && vps_base_layer_internal_flag != 0)
            {
                size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level);
            }
            size += stream.WriteUnsignedInt(1, this.splitting_flag);

            for (i = 0, NumScalabilityTypes = 0; i < 16; i++)
            {
                size += stream.WriteUnsignedInt(1, this.scalability_mask_flag[i]);
                NumScalabilityTypes += scalability_mask_flag[i];
            }

            for (j = 0; j < (NumScalabilityTypes - splitting_flag); j++)
            {
                size += stream.WriteUnsignedInt(3, this.dimension_id_len_minus1[j]);
            }
            size += stream.WriteUnsignedInt(1, this.vps_nuh_layer_id_present_flag);

            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                if (vps_nuh_layer_id_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(6, this.layer_id_in_nuh[i]);
                }

                if (splitting_flag == 0)
                {

                    for (j = 0; j < NumScalabilityTypes; j++)
                    {
                        size += stream.WriteUnsignedIntVariable(dimension_id[i][j], this.dimension_id[i][j]);
                    }
                }
            }
            size += stream.WriteUnsignedInt(4, this.view_id_len);

            if (view_id_len > 0)
            {

                for (i = 0; i < NumViews; i++)
                {
                    size += stream.WriteUnsignedIntVariable(view_id_val[i], this.view_id_val[i]);
                }
            }

            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                for (j = 0; j < i; j++)
                {
                    size += stream.WriteUnsignedInt(1, this.direct_dependency_flag[i][j]);
                }
            }

            if (NumIndependentLayers > 1)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_add_layer_sets);
            }

            for (i = 0; i < num_add_layer_sets; i++)
            {

                for (j = 1; j < NumIndependentLayers; j++)
                {
                    size += stream.WriteUnsignedIntVariable(highest_layer_idx_plus1[i][j], this.highest_layer_idx_plus1[i][j]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.vps_sub_layers_max_minus1_present_flag);

            if (vps_sub_layers_max_minus1_present_flag != 0)
            {

                for (i = 0; i <= MaxLayersMinus1; i++)
                {
                    size += stream.WriteUnsignedInt(3, this.sub_layers_vps_max_minus1[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.max_tid_ref_present_flag);

            if (max_tid_ref_present_flag != 0)
            {

                for (i = 0; i < MaxLayersMinus1; i++)
                {

                    for (j = i + 1; j <= MaxLayersMinus1; j++)
                    {

                        if (direct_dependency_flag[j][i] != 0)
                        {
                            size += stream.WriteUnsignedInt(3, this.max_tid_il_ref_pics_plus1[i][j]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.default_ref_layers_active_flag);
            size += stream.WriteUnsignedIntGolomb(this.vps_num_profile_tier_level_minus1);

            for (i = vps_base_layer_internal_flag != 0 ? 2 : 1;
          i <= vps_num_profile_tier_level_minus1; i++)
            {
                size += stream.WriteUnsignedInt(1, this.vps_profile_present_flag[i]);
                size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level0[i]);
            }

            if (NumLayerSets > 1)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_add_olss);
                size += stream.WriteUnsignedInt(2, this.default_output_layer_idc);
            }
            NumOutputLayerSets = num_add_olss + NumLayerSets;

            for (i = 1; i < NumOutputLayerSets; i++)
            {

                if (NumLayerSets > 2 && i >= NumLayerSets)
                {
                    size += stream.WriteUnsignedIntVariable(layer_set_idx_for_ols_minus1[i], this.layer_set_idx_for_ols_minus1[i]);
                }

                if (i > vps_num_layer_sets_minus1 || defaultOutputLayerIdc == 2)
                {

                    for (j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.output_layer_flag[i][j]);
                    }
                }

                for (j = 0; j < NumLayersInIdList[OlsIdxToLsIdx[i]]; j++)
                {

                    if (NecessaryLayerFlag[i][j] != 0 && vps_num_profile_tier_level_minus1 > 0)
                    {
                        size += stream.WriteUnsignedIntVariable(profile_tier_level_idx[i][j], this.profile_tier_level_idx[i][j]);
                    }
                }

                if (NumOutputLayersInOutputLayerSet[i] == 1
   && NumDirectRefLayers[OlsHighestOutputLayerId[i]] > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.alt_output_layer_flag[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.vps_num_rep_formats_minus1);

            for (i = 0; i <= vps_num_rep_formats_minus1; i++)
            {
                size += stream.WriteClass<RepFormat>(context, this.rep_format[i]);
            }

            if (vps_num_rep_formats_minus1 > 0)
            {
                size += stream.WriteUnsignedInt(1, this.rep_format_idx_present_flag);
            }

            if (rep_format_idx_present_flag != 0)
            {

                for (i = vps_base_layer_internal_flag != 0 ? 1 : 0; i <= MaxLayersMinus1; i++)
                {
                    size += stream.WriteUnsignedIntVariable(vps_rep_format_idx[i], this.vps_rep_format_idx[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.max_one_active_ref_layer_flag);
            size += stream.WriteUnsignedInt(1, this.vps_poc_lsb_aligned_flag);

            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                if (NumDirectRefLayers[layer_id_in_nuh[i]] == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.poc_lsb_not_present_flag[i]);
                }
            }
            size += stream.WriteClass<DpbSize>(context, this.dpb_size);
            size += stream.WriteUnsignedIntGolomb(this.direct_dep_type_len_minus2);
            size += stream.WriteUnsignedInt(1, this.direct_dependency_all_layers_flag);

            if (direct_dependency_all_layers_flag != 0)
            {
                size += stream.WriteUnsignedIntVariable(direct_dependency_all_layers_type, this.direct_dependency_all_layers_type);
            }
            else
            {

                for (i = vps_base_layer_internal_flag != 0 ? 1 : 2; i <= MaxLayersMinus1; i++)
                {

                    for (j = vps_base_layer_internal_flag != 0 ? 0 : 1; j < i; j++)
                    {

                        if (direct_dependency_flag[i][j] != 0)
                        {
                            size += stream.WriteUnsignedIntVariable(direct_dependency_type[i][j], this.direct_dependency_type[i][j]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.vps_non_vui_extension_length);

            for (i = 1; i <= vps_non_vui_extension_length; i++)
            {
                size += stream.WriteUnsignedInt(8, this.vps_non_vui_extension_data_byte[i]);
            }
            size += stream.WriteUnsignedInt(1, this.vps_vui_present_flag);

            if (vps_vui_present_flag != 0)
            {

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.vps_vui_alignment_bit_equal_to_one);
                }
                size += stream.WriteClass<VpsVui>(context, this.vps_vui);
            }

            return size;
        }

    }

    /*
  

rep_format() {  
 pic_width_vps_in_luma_samples u(16) 
 pic_height_vps_in_luma_samples u(16) 
 chroma_and_bit_depth_vps_present_flag u(1) 
 if( chroma_and_bit_depth_vps_present_flag ) {  
  chroma_format_vps_idc u(2) 
  if( chroma_format_vps_idc == 3 )  
   separate_colour_plane_vps_flag u(1) 
  bit_depth_vps_luma_minus8 u(4) 
  bit_depth_vps_chroma_minus8 u(4) 
 }  
 conformance_window_vps_flag u(1) 
 if( conformance_window_vps_flag ) {  
  conf_win_vps_left_offset ue(v) 
  conf_win_vps_right_offset ue(v) 
  conf_win_vps_top_offset ue(v) 
  conf_win_vps_bottom_offset ue(v) 
 }  
}
    */
    public class RepFormat : IItuSerializable
    {
        private uint pic_width_vps_in_luma_samples;
        public uint PicWidthVpsInLumaSamples { get { return pic_width_vps_in_luma_samples; } set { pic_width_vps_in_luma_samples = value; } }
        private uint pic_height_vps_in_luma_samples;
        public uint PicHeightVpsInLumaSamples { get { return pic_height_vps_in_luma_samples; } set { pic_height_vps_in_luma_samples = value; } }
        private byte chroma_and_bit_depth_vps_present_flag;
        public byte ChromaAndBitDepthVpsPresentFlag { get { return chroma_and_bit_depth_vps_present_flag; } set { chroma_and_bit_depth_vps_present_flag = value; } }
        private uint chroma_format_vps_idc;
        public uint ChromaFormatVpsIdc { get { return chroma_format_vps_idc; } set { chroma_format_vps_idc = value; } }
        private byte separate_colour_plane_vps_flag;
        public byte SeparateColourPlaneVpsFlag { get { return separate_colour_plane_vps_flag; } set { separate_colour_plane_vps_flag = value; } }
        private uint bit_depth_vps_luma_minus8;
        public uint BitDepthVpsLumaMinus8 { get { return bit_depth_vps_luma_minus8; } set { bit_depth_vps_luma_minus8 = value; } }
        private uint bit_depth_vps_chroma_minus8;
        public uint BitDepthVpsChromaMinus8 { get { return bit_depth_vps_chroma_minus8; } set { bit_depth_vps_chroma_minus8 = value; } }
        private byte conformance_window_vps_flag;
        public byte ConformanceWindowVpsFlag { get { return conformance_window_vps_flag; } set { conformance_window_vps_flag = value; } }
        private uint conf_win_vps_left_offset;
        public uint ConfWinVpsLeftOffset { get { return conf_win_vps_left_offset; } set { conf_win_vps_left_offset = value; } }
        private uint conf_win_vps_right_offset;
        public uint ConfWinVpsRightOffset { get { return conf_win_vps_right_offset; } set { conf_win_vps_right_offset = value; } }
        private uint conf_win_vps_top_offset;
        public uint ConfWinVpsTopOffset { get { return conf_win_vps_top_offset; } set { conf_win_vps_top_offset = value; } }
        private uint conf_win_vps_bottom_offset;
        public uint ConfWinVpsBottomOffset { get { return conf_win_vps_bottom_offset; } set { conf_win_vps_bottom_offset = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public RepFormat()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 16, out this.pic_width_vps_in_luma_samples);
            size += stream.ReadUnsignedInt(size, 16, out this.pic_height_vps_in_luma_samples);
            size += stream.ReadUnsignedInt(size, 1, out this.chroma_and_bit_depth_vps_present_flag);

            if (chroma_and_bit_depth_vps_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.chroma_format_vps_idc);

                if (chroma_format_vps_idc == 3)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.separate_colour_plane_vps_flag);
                }
                size += stream.ReadUnsignedInt(size, 4, out this.bit_depth_vps_luma_minus8);
                size += stream.ReadUnsignedInt(size, 4, out this.bit_depth_vps_chroma_minus8);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.conformance_window_vps_flag);

            if (conformance_window_vps_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_vps_left_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_vps_right_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_vps_top_offset);
                size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_vps_bottom_offset);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(16, this.pic_width_vps_in_luma_samples);
            size += stream.WriteUnsignedInt(16, this.pic_height_vps_in_luma_samples);
            size += stream.WriteUnsignedInt(1, this.chroma_and_bit_depth_vps_present_flag);

            if (chroma_and_bit_depth_vps_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(2, this.chroma_format_vps_idc);

                if (chroma_format_vps_idc == 3)
                {
                    size += stream.WriteUnsignedInt(1, this.separate_colour_plane_vps_flag);
                }
                size += stream.WriteUnsignedInt(4, this.bit_depth_vps_luma_minus8);
                size += stream.WriteUnsignedInt(4, this.bit_depth_vps_chroma_minus8);
            }
            size += stream.WriteUnsignedInt(1, this.conformance_window_vps_flag);

            if (conformance_window_vps_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.conf_win_vps_left_offset);
                size += stream.WriteUnsignedIntGolomb(this.conf_win_vps_right_offset);
                size += stream.WriteUnsignedIntGolomb(this.conf_win_vps_top_offset);
                size += stream.WriteUnsignedIntGolomb(this.conf_win_vps_bottom_offset);
            }

            return size;
        }

    }

    /*
  

dpb_size() {  
 for( i = 1; i < NumOutputLayerSets; i++ ) {  
  currLsIdx = OlsIdxToLsIdx[ i ]  
  sub_layer_flag_info_present_flag[ i ] u(1) 
  for( j = 0; j <= MaxSubLayersInLayerSetMinus1[ currLsIdx ]; j++ ) {  
   if( j > 0  &&  sub_layer_flag_info_present_flag[ i ] )  
    sub_layer_dpb_info_present_flag[ i ][ j ] u(1) 
   if( sub_layer_dpb_info_present_flag[ i ][ j ] ) {  
    for( k = 0; k < NumLayersInIdList[ currLsIdx ]; k++ )  
     if( NecessaryLayerFlag[ i ][ k ]  &&  ( vps_base_layer_internal_flag  || 
      ( LayerSetLayerIdList[ currLsIdx ][ k ] != 0 ) ) ) 
 
      max_vps_dec_pic_buffering_minus1[ i ][ k ][ j ] ue(v) 
    max_vps_num_reorder_pics[ i ][ j ] ue(v) 
    max_vps_latency_increase_plus1[ i ][ j ] ue(v) 
   }  
  }  
 }  
}
    */
    public class DpbSize : IItuSerializable
    {
        private byte[] sub_layer_flag_info_present_flag;
        public byte[] SubLayerFlagInfoPresentFlag { get { return sub_layer_flag_info_present_flag; } set { sub_layer_flag_info_present_flag = value; } }
        private byte[][] sub_layer_dpb_info_present_flag;
        public byte[][] SubLayerDpbInfoPresentFlag { get { return sub_layer_dpb_info_present_flag; } set { sub_layer_dpb_info_present_flag = value; } }
        private uint[][][] max_vps_dec_pic_buffering_minus1;
        public uint[][][] MaxVpsDecPicBufferingMinus1 { get { return max_vps_dec_pic_buffering_minus1; } set { max_vps_dec_pic_buffering_minus1 = value; } }
        private uint[][] max_vps_num_reorder_pics;
        public uint[][] MaxVpsNumReorderPics { get { return max_vps_num_reorder_pics; } set { max_vps_num_reorder_pics = value; } }
        private uint[][] max_vps_latency_increase_plus1;
        public uint[][] MaxVpsLatencyIncreasePlus1 { get { return max_vps_latency_increase_plus1; } set { max_vps_latency_increase_plus1 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DpbSize()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint currLsIdx = 0;
            uint j = 0;
            uint k = 0;

            this.sub_layer_flag_info_present_flag = new byte[NumOutputLayerSets];
            this.sub_layer_dpb_info_present_flag = new byte[NumOutputLayerSets][];
            this.max_vps_dec_pic_buffering_minus1 = new uint[NumOutputLayerSets][][];
            this.max_vps_num_reorder_pics = new uint[NumOutputLayerSets][];
            this.max_vps_latency_increase_plus1 = new uint[NumOutputLayerSets][];
            for (i = 1; i < NumOutputLayerSets; i++)
            {
                currLsIdx = OlsIdxToLsIdx[i];
                size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_flag_info_present_flag[i]);

                this.sub_layer_dpb_info_present_flag[i] = new byte[MaxSubLayersInLayerSetMinus1 + 1[currLsIdx]];
                this.max_vps_dec_pic_buffering_minus1[i] = new uint[MaxSubLayersInLayerSetMinus1 + 1[currLsIdx]][];
                this.max_vps_num_reorder_pics[i] = new uint[MaxSubLayersInLayerSetMinus1 + 1[currLsIdx]];
                this.max_vps_latency_increase_plus1[i] = new uint[MaxSubLayersInLayerSetMinus1 + 1[currLsIdx]];
                for (j = 0; j <= MaxSubLayersInLayerSetMinus1[currLsIdx]; j++)
                {

                    if (j > 0 && sub_layer_flag_info_present_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.sub_layer_dpb_info_present_flag[i][j]);
                    }

                    if (sub_layer_dpb_info_present_flag[i][j] != 0)
                    {

                        this.max_vps_dec_pic_buffering_minus1[i][j] = new uint[NumLayersInIdList[currLsIdx]];
                        for (k = 0; k < NumLayersInIdList[currLsIdx]; k++)
                        {

                            if (NecessaryLayerFlag[i][k] != 0 && (vps_base_layer_internal_flag != 0 ||
      (LayerSetLayerIdList[currLsIdx][k] != 0)))
                            {
                                size += stream.ReadUnsignedIntGolomb(size, out this.max_vps_dec_pic_buffering_minus1[i][k][j]);
                            }
                        }
                        size += stream.ReadUnsignedIntGolomb(size, out this.max_vps_num_reorder_pics[i][j]);
                        size += stream.ReadUnsignedIntGolomb(size, out this.max_vps_latency_increase_plus1[i][j]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint currLsIdx = 0;
            uint j = 0;
            uint k = 0;

            for (i = 1; i < NumOutputLayerSets; i++)
            {
                currLsIdx = OlsIdxToLsIdx[i];
                size += stream.WriteUnsignedInt(1, this.sub_layer_flag_info_present_flag[i]);

                for (j = 0; j <= MaxSubLayersInLayerSetMinus1[currLsIdx]; j++)
                {

                    if (j > 0 && sub_layer_flag_info_present_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.sub_layer_dpb_info_present_flag[i][j]);
                    }

                    if (sub_layer_dpb_info_present_flag[i][j] != 0)
                    {

                        for (k = 0; k < NumLayersInIdList[currLsIdx]; k++)
                        {

                            if (NecessaryLayerFlag[i][k] != 0 && (vps_base_layer_internal_flag != 0 ||
      (LayerSetLayerIdList[currLsIdx][k] != 0)))
                            {
                                size += stream.WriteUnsignedIntGolomb(this.max_vps_dec_pic_buffering_minus1[i][k][j]);
                            }
                        }
                        size += stream.WriteUnsignedIntGolomb(this.max_vps_num_reorder_pics[i][j]);
                        size += stream.WriteUnsignedIntGolomb(this.max_vps_latency_increase_plus1[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
 

vps_vui() {  
 cross_layer_pic_type_aligned_flag u(1) 
 if( !cross_layer_pic_type_aligned_flag ) 
   cross_layer_irap_aligned_flag u(1) 
 if( cross_layer_irap_aligned_flag )  
  all_layers_idr_aligned_flag u(1) 
 bit_rate_present_vps_flag u(1) 
 pic_rate_present_vps_flag u(1) 
 if( bit_rate_present_vps_flag  ||  pic_rate_present_vps_flag )  
  for( i = vps_base_layer_internal_flag != 0 ?  0 : 1; i < NumLayerSets; i++ )  
   for( j = 0; j <= MaxSubLayersInLayerSetMinus1[ i ]; j++ ) {  
    if( bit_rate_present_vps_flag )  
     bit_rate_present_flag[ i ][ j ] u(1) 
    if( pic_rate_present_vps_flag )  
     pic_rate_present_flag[ i ][ j ] u(1) 
    if( bit_rate_present_flag[ i ][ j ] ) {  
     avg_bit_rate[ i ][ j ] u(16) 
     max_bit_rate[ i ][ j ] u(16) 
    }  
    if( pic_rate_present_flag[ i ][ j ] ) {  
     constant_pic_rate_idc[ i ][ j ] u(2) 
     avg_pic_rate[ i ][ j ] u(16) 
    }  
   }  
 video_signal_info_idx_present_flag u(1) 
 if( video_signal_info_idx_present_flag )  
  vps_num_video_signal_info_minus1 u(4) 
 for( i = 0; i <= vps_num_video_signal_info_minus1; i++ )  
  video_signal_info()  
 if( video_signal_info_idx_present_flag  &&  vps_num_video_signal_info_minus1 > 0 )  
  for( i = vps_base_layer_internal_flag != 0 ?  0 : 1; i <= MaxLayersMinus1; i++ )  
   vps_video_signal_info_idx[ i ] u(4) 
 tiles_not_in_use_flag u(1) 
 if( !tiles_not_in_use_flag ) {  
  for( i = vps_base_layer_internal_flag != 0 ?  0 : 1; i <= MaxLayersMinus1; i++ ) {  
   tiles_in_use_flag[ i ] u(1) 
   if( tiles_in_use_flag[ i ] )  
    loop_filter_not_across_tiles_flag[ i ] u(1) 
  }  
  for( i = vps_base_layer_internal_flag != 0 ?  1 : 2; i <= MaxLayersMinus1; i++ )  
   for( j = 0; j < NumDirectRefLayers[ layer_id_in_nuh[ i ] ]; j++ ) {  
    layerIdx = LayerIdxInVps[ IdDirectRefLayer[ layer_id_in_nuh[ i ] ][ j ] ]  
    if( tiles_in_use_flag[ i ]  &&  tiles_in_use_flag[ layerIdx ] )  
     tile_boundaries_aligned_flag[ i ][ j ] u(1) 
   }  
 }  
 wpp_not_in_use_flag u(1) 
 if( !wpp_not_in_use_flag )  
  for( i = vps_base_layer_internal_flag != 0 ?  0 : 1; i <= MaxLayersMinus1; i++ )  
   wpp_in_use_flag[ i ] u(1) 
 single_layer_for_non_irap_flag u(1)
  higher_layer_irap_skip_flag u(1) 
 ilp_restricted_ref_layers_flag u(1) 
 if( ilp_restricted_ref_layers_flag )  
  for( i = 1; i <= MaxLayersMinus1; i++ )  
   for( j = 0; j < NumDirectRefLayers[ layer_id_in_nuh[ i ] ]; j++ )  
    if( vps_base_layer_internal_flag  || 
        IdDirectRefLayer[ layer_id_in_nuh[ i ] ][ j ] > 0 ) { 
 
     min_spatial_segment_offset_plus1[ i ][ j ] ue(v) 
     if( min_spatial_segment_offset_plus1[ i ][ j ] > 0 ) {  
      ctu_based_offset_enabled_flag[ i ][ j ] u(1) 
      if( ctu_based_offset_enabled_flag[ i ][ j ] )  
       min_horizontal_ctu_offset_plus1[ i ][ j ] ue(v) 
     }  
    }  
 vps_vui_bsp_hrd_present_flag u(1) 
 if( vps_vui_bsp_hrd_present_flag )  
  vps_vui_bsp_hrd_params()  
 for( i = 1; i <= MaxLayersMinus1; i++ )  
  if( NumDirectRefLayers[ layer_id_in_nuh[ i ] ] == 0 )  
   base_layer_parameter_set_compatibility_flag[ i ] u(1) 
}
    */
    public class VpsVui : IItuSerializable
    {
        private byte cross_layer_pic_type_aligned_flag;
        public byte CrossLayerPicTypeAlignedFlag { get { return cross_layer_pic_type_aligned_flag; } set { cross_layer_pic_type_aligned_flag = value; } }
        private byte cross_layer_irap_aligned_flag;
        public byte CrossLayerIrapAlignedFlag { get { return cross_layer_irap_aligned_flag; } set { cross_layer_irap_aligned_flag = value; } }
        private byte all_layers_idr_aligned_flag;
        public byte AllLayersIdrAlignedFlag { get { return all_layers_idr_aligned_flag; } set { all_layers_idr_aligned_flag = value; } }
        private byte bit_rate_present_vps_flag;
        public byte BitRatePresentVpsFlag { get { return bit_rate_present_vps_flag; } set { bit_rate_present_vps_flag = value; } }
        private byte pic_rate_present_vps_flag;
        public byte PicRatePresentVpsFlag { get { return pic_rate_present_vps_flag; } set { pic_rate_present_vps_flag = value; } }
        private byte[][] bit_rate_present_flag;
        public byte[][] BitRatePresentFlag { get { return bit_rate_present_flag; } set { bit_rate_present_flag = value; } }
        private byte[][] pic_rate_present_flag;
        public byte[][] PicRatePresentFlag { get { return pic_rate_present_flag; } set { pic_rate_present_flag = value; } }
        private uint[][] avg_bit_rate;
        public uint[][] AvgBitRate { get { return avg_bit_rate; } set { avg_bit_rate = value; } }
        private uint[][] max_bit_rate;
        public uint[][] MaxBitRate { get { return max_bit_rate; } set { max_bit_rate = value; } }
        private uint[][] constant_pic_rate_idc;
        public uint[][] ConstantPicRateIdc { get { return constant_pic_rate_idc; } set { constant_pic_rate_idc = value; } }
        private uint[][] avg_pic_rate;
        public uint[][] AvgPicRate { get { return avg_pic_rate; } set { avg_pic_rate = value; } }
        private byte video_signal_info_idx_present_flag;
        public byte VideoSignalInfoIdxPresentFlag { get { return video_signal_info_idx_present_flag; } set { video_signal_info_idx_present_flag = value; } }
        private uint vps_num_video_signal_info_minus1;
        public uint VpsNumVideoSignalInfoMinus1 { get { return vps_num_video_signal_info_minus1; } set { vps_num_video_signal_info_minus1 = value; } }
        private VideoSignalInfo[] video_signal_info;
        public VideoSignalInfo[] VideoSignalInfo { get { return video_signal_info; } set { video_signal_info = value; } }
        private uint[] vps_video_signal_info_idx;
        public uint[] VpsVideoSignalInfoIdx { get { return vps_video_signal_info_idx; } set { vps_video_signal_info_idx = value; } }
        private byte tiles_not_in_use_flag;
        public byte TilesNotInUseFlag { get { return tiles_not_in_use_flag; } set { tiles_not_in_use_flag = value; } }
        private byte[] tiles_in_use_flag;
        public byte[] TilesInUseFlag { get { return tiles_in_use_flag; } set { tiles_in_use_flag = value; } }
        private byte[] loop_filter_not_across_tiles_flag;
        public byte[] LoopFilterNotAcrossTilesFlag { get { return loop_filter_not_across_tiles_flag; } set { loop_filter_not_across_tiles_flag = value; } }
        private byte[][] tile_boundaries_aligned_flag;
        public byte[][] TileBoundariesAlignedFlag { get { return tile_boundaries_aligned_flag; } set { tile_boundaries_aligned_flag = value; } }
        private byte wpp_not_in_use_flag;
        public byte WppNotInUseFlag { get { return wpp_not_in_use_flag; } set { wpp_not_in_use_flag = value; } }
        private byte[] wpp_in_use_flag;
        public byte[] WppInUseFlag { get { return wpp_in_use_flag; } set { wpp_in_use_flag = value; } }
        private byte single_layer_for_non_irap_flag;
        public byte SingleLayerForNonIrapFlag { get { return single_layer_for_non_irap_flag; } set { single_layer_for_non_irap_flag = value; } }
        private byte higher_layer_irap_skip_flag;
        public byte HigherLayerIrapSkipFlag { get { return higher_layer_irap_skip_flag; } set { higher_layer_irap_skip_flag = value; } }
        private byte ilp_restricted_ref_layers_flag;
        public byte IlpRestrictedRefLayersFlag { get { return ilp_restricted_ref_layers_flag; } set { ilp_restricted_ref_layers_flag = value; } }
        private uint[][] min_spatial_segment_offset_plus1;
        public uint[][] MinSpatialSegmentOffsetPlus1 { get { return min_spatial_segment_offset_plus1; } set { min_spatial_segment_offset_plus1 = value; } }
        private byte[][] ctu_based_offset_enabled_flag;
        public byte[][] CtuBasedOffsetEnabledFlag { get { return ctu_based_offset_enabled_flag; } set { ctu_based_offset_enabled_flag = value; } }
        private uint[][] min_horizontal_ctu_offset_plus1;
        public uint[][] MinHorizontalCtuOffsetPlus1 { get { return min_horizontal_ctu_offset_plus1; } set { min_horizontal_ctu_offset_plus1 = value; } }
        private byte vps_vui_bsp_hrd_present_flag;
        public byte VpsVuiBspHrdPresentFlag { get { return vps_vui_bsp_hrd_present_flag; } set { vps_vui_bsp_hrd_present_flag = value; } }
        private VpsVuiBspHrdParams vps_vui_bsp_hrd_params;
        public VpsVuiBspHrdParams VpsVuiBspHrdParams { get { return vps_vui_bsp_hrd_params; } set { vps_vui_bsp_hrd_params = value; } }
        private byte[] base_layer_parameter_set_compatibility_flag;
        public byte[] BaseLayerParameterSetCompatibilityFlag { get { return base_layer_parameter_set_compatibility_flag; } set { base_layer_parameter_set_compatibility_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VpsVui()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint layerIdx = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.cross_layer_pic_type_aligned_flag);

            if (cross_layer_pic_type_aligned_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.cross_layer_irap_aligned_flag);
            }

            if (cross_layer_irap_aligned_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.all_layers_idr_aligned_flag);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.bit_rate_present_vps_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pic_rate_present_vps_flag);

            if (bit_rate_present_vps_flag != 0 || pic_rate_present_vps_flag != 0)
            {

                this.bit_rate_present_flag = new byte[NumLayerSets][];
                this.pic_rate_present_flag = new byte[NumLayerSets][];
                this.avg_bit_rate = new uint[NumLayerSets][];
                this.max_bit_rate = new uint[NumLayerSets][];
                this.constant_pic_rate_idc = new uint[NumLayerSets][];
                this.avg_pic_rate = new uint[NumLayerSets][];
                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i < NumLayerSets; i++)
                {

                    this.bit_rate_present_flag[i] = new byte[MaxSubLayersInLayerSetMinus1[i] + 1];
                    this.pic_rate_present_flag[i] = new byte[MaxSubLayersInLayerSetMinus1[i] + 1];
                    this.avg_bit_rate[i] = new uint[MaxSubLayersInLayerSetMinus1[i] + 1];
                    this.max_bit_rate[i] = new uint[MaxSubLayersInLayerSetMinus1[i] + 1];
                    this.constant_pic_rate_idc[i] = new uint[MaxSubLayersInLayerSetMinus1[i] + 1];
                    this.avg_pic_rate[i] = new uint[MaxSubLayersInLayerSetMinus1[i] + 1];
                    for (j = 0; j <= MaxSubLayersInLayerSetMinus1[i]; j++)
                    {

                        if (bit_rate_present_vps_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.bit_rate_present_flag[i][j]);
                        }

                        if (pic_rate_present_vps_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.pic_rate_present_flag[i][j]);
                        }

                        if (bit_rate_present_flag[i][j] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 16, out this.avg_bit_rate[i][j]);
                            size += stream.ReadUnsignedInt(size, 16, out this.max_bit_rate[i][j]);
                        }

                        if (pic_rate_present_flag[i][j] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 2, out this.constant_pic_rate_idc[i][j]);
                            size += stream.ReadUnsignedInt(size, 16, out this.avg_pic_rate[i][j]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.video_signal_info_idx_present_flag);

            if (video_signal_info_idx_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 4, out this.vps_num_video_signal_info_minus1);
            }

            this.video_signal_info = new VideoSignalInfo[vps_num_video_signal_info_minus1 + 1];
            for (i = 0; i <= vps_num_video_signal_info_minus1; i++)
            {
                this.video_signal_info[i] = new VideoSignalInfo();
                size += stream.ReadClass<VideoSignalInfo>(size, context, this.video_signal_info[i]);
            }

            if (video_signal_info_idx_present_flag != 0 && vps_num_video_signal_info_minus1 > 0)
            {

                this.vps_video_signal_info_idx = new uint[MaxLayersMinus1 + 1];
                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i <= MaxLayersMinus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 4, out this.vps_video_signal_info_idx[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.tiles_not_in_use_flag);

            if (tiles_not_in_use_flag == 0)
            {

                this.tiles_in_use_flag = new byte[MaxLayersMinus1 + 1];
                this.loop_filter_not_across_tiles_flag = new byte[MaxLayersMinus1 + 1];
                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i <= MaxLayersMinus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.tiles_in_use_flag[i]);

                    if (tiles_in_use_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.loop_filter_not_across_tiles_flag[i]);
                    }
                }

                this.tile_boundaries_aligned_flag = new byte[MaxLayersMinus1 + 1][];
                for (i = vps_base_layer_internal_flag != 0 ? 1 : 2; i <= MaxLayersMinus1; i++)
                {

                    this.tile_boundaries_aligned_flag[i] = new byte[NumDirectRefLayers[layer_id_in_nuh[i]]];
                    for (j = 0; j < NumDirectRefLayers[layer_id_in_nuh[i]]; j++)
                    {
                        layerIdx = LayerIdxInVps[IdDirectRefLayer[layer_id_in_nuh[i]][j]];

                        if (tiles_in_use_flag[i] != 0 && tiles_in_use_flag[layerIdx] != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.tile_boundaries_aligned_flag[i][j]);
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.wpp_not_in_use_flag);

            if (wpp_not_in_use_flag == 0)
            {

                this.wpp_in_use_flag = new byte[MaxLayersMinus1 + 1];
                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i <= MaxLayersMinus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.wpp_in_use_flag[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.single_layer_for_non_irap_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.higher_layer_irap_skip_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.ilp_restricted_ref_layers_flag);

            if (ilp_restricted_ref_layers_flag != 0)
            {

                this.min_spatial_segment_offset_plus1 = new uint[MaxLayersMinus1 + 1][];
                this.ctu_based_offset_enabled_flag = new byte[MaxLayersMinus1 + 1][];
                this.min_horizontal_ctu_offset_plus1 = new uint[MaxLayersMinus1 + 1][];
                for (i = 1; i <= MaxLayersMinus1; i++)
                {

                    this.min_spatial_segment_offset_plus1[i] = new uint[NumDirectRefLayers[layer_id_in_nuh[i]]];
                    this.ctu_based_offset_enabled_flag[i] = new byte[NumDirectRefLayers[layer_id_in_nuh[i]]];
                    this.min_horizontal_ctu_offset_plus1[i] = new uint[NumDirectRefLayers[layer_id_in_nuh[i]]];
                    for (j = 0; j < NumDirectRefLayers[layer_id_in_nuh[i]]; j++)
                    {

                        if (vps_base_layer_internal_flag != 0 ||
        IdDirectRefLayer[layer_id_in_nuh[i]][j] > 0)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.min_spatial_segment_offset_plus1[i][j]);

                            if (min_spatial_segment_offset_plus1[i][j] > 0)
                            {
                                size += stream.ReadUnsignedInt(size, 1, out this.ctu_based_offset_enabled_flag[i][j]);

                                if (ctu_based_offset_enabled_flag[i][j] != 0)
                                {
                                    size += stream.ReadUnsignedIntGolomb(size, out this.min_horizontal_ctu_offset_plus1[i][j]);
                                }
                            }
                        }
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_vui_bsp_hrd_present_flag);

            if (vps_vui_bsp_hrd_present_flag != 0)
            {
                this.vps_vui_bsp_hrd_params = new VpsVuiBspHrdParams();
                size += stream.ReadClass<VpsVuiBspHrdParams>(size, context, this.vps_vui_bsp_hrd_params);
            }

            this.base_layer_parameter_set_compatibility_flag = new byte[MaxLayersMinus1 + 1];
            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                if (NumDirectRefLayers[layer_id_in_nuh[i]] == 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.base_layer_parameter_set_compatibility_flag[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint layerIdx = 0;
            size += stream.WriteUnsignedInt(1, this.cross_layer_pic_type_aligned_flag);

            if (cross_layer_pic_type_aligned_flag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.cross_layer_irap_aligned_flag);
            }

            if (cross_layer_irap_aligned_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.all_layers_idr_aligned_flag);
            }
            size += stream.WriteUnsignedInt(1, this.bit_rate_present_vps_flag);
            size += stream.WriteUnsignedInt(1, this.pic_rate_present_vps_flag);

            if (bit_rate_present_vps_flag != 0 || pic_rate_present_vps_flag != 0)
            {

                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i < NumLayerSets; i++)
                {

                    for (j = 0; j <= MaxSubLayersInLayerSetMinus1[i]; j++)
                    {

                        if (bit_rate_present_vps_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.bit_rate_present_flag[i][j]);
                        }

                        if (pic_rate_present_vps_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.pic_rate_present_flag[i][j]);
                        }

                        if (bit_rate_present_flag[i][j] != 0)
                        {
                            size += stream.WriteUnsignedInt(16, this.avg_bit_rate[i][j]);
                            size += stream.WriteUnsignedInt(16, this.max_bit_rate[i][j]);
                        }

                        if (pic_rate_present_flag[i][j] != 0)
                        {
                            size += stream.WriteUnsignedInt(2, this.constant_pic_rate_idc[i][j]);
                            size += stream.WriteUnsignedInt(16, this.avg_pic_rate[i][j]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.video_signal_info_idx_present_flag);

            if (video_signal_info_idx_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(4, this.vps_num_video_signal_info_minus1);
            }

            for (i = 0; i <= vps_num_video_signal_info_minus1; i++)
            {
                size += stream.WriteClass<VideoSignalInfo>(context, this.video_signal_info[i]);
            }

            if (video_signal_info_idx_present_flag != 0 && vps_num_video_signal_info_minus1 > 0)
            {

                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i <= MaxLayersMinus1; i++)
                {
                    size += stream.WriteUnsignedInt(4, this.vps_video_signal_info_idx[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.tiles_not_in_use_flag);

            if (tiles_not_in_use_flag == 0)
            {

                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i <= MaxLayersMinus1; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.tiles_in_use_flag[i]);

                    if (tiles_in_use_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.loop_filter_not_across_tiles_flag[i]);
                    }
                }

                for (i = vps_base_layer_internal_flag != 0 ? 1 : 2; i <= MaxLayersMinus1; i++)
                {

                    for (j = 0; j < NumDirectRefLayers[layer_id_in_nuh[i]]; j++)
                    {
                        layerIdx = LayerIdxInVps[IdDirectRefLayer[layer_id_in_nuh[i]][j]];

                        if (tiles_in_use_flag[i] != 0 && tiles_in_use_flag[layerIdx] != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.tile_boundaries_aligned_flag[i][j]);
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.wpp_not_in_use_flag);

            if (wpp_not_in_use_flag == 0)
            {

                for (i = vps_base_layer_internal_flag != 0 ? 0 : 1; i <= MaxLayersMinus1; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.wpp_in_use_flag[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.single_layer_for_non_irap_flag);
            size += stream.WriteUnsignedInt(1, this.higher_layer_irap_skip_flag);
            size += stream.WriteUnsignedInt(1, this.ilp_restricted_ref_layers_flag);

            if (ilp_restricted_ref_layers_flag != 0)
            {

                for (i = 1; i <= MaxLayersMinus1; i++)
                {

                    for (j = 0; j < NumDirectRefLayers[layer_id_in_nuh[i]]; j++)
                    {

                        if (vps_base_layer_internal_flag != 0 ||
        IdDirectRefLayer[layer_id_in_nuh[i]][j] > 0)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.min_spatial_segment_offset_plus1[i][j]);

                            if (min_spatial_segment_offset_plus1[i][j] > 0)
                            {
                                size += stream.WriteUnsignedInt(1, this.ctu_based_offset_enabled_flag[i][j]);

                                if (ctu_based_offset_enabled_flag[i][j] != 0)
                                {
                                    size += stream.WriteUnsignedIntGolomb(this.min_horizontal_ctu_offset_plus1[i][j]);
                                }
                            }
                        }
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.vps_vui_bsp_hrd_present_flag);

            if (vps_vui_bsp_hrd_present_flag != 0)
            {
                size += stream.WriteClass<VpsVuiBspHrdParams>(context, this.vps_vui_bsp_hrd_params);
            }

            for (i = 1; i <= MaxLayersMinus1; i++)
            {

                if (NumDirectRefLayers[layer_id_in_nuh[i]] == 0)
                {
                    size += stream.WriteUnsignedInt(1, this.base_layer_parameter_set_compatibility_flag[i]);
                }
            }

            return size;
        }

    }

    /*
  

video_signal_info() {  
 video_vps_format u(3) 
 video_full_range_vps_flag u(1) 
 colour_primaries_vps u(8) 
 transfer_characteristics_vps u(8) 
 matrix_coeffs_vps u(8) 
}
    */
    public class VideoSignalInfo : IItuSerializable
    {
        private uint video_vps_format;
        public uint VideoVpsFormat { get { return video_vps_format; } set { video_vps_format = value; } }
        private byte video_full_range_vps_flag;
        public byte VideoFullRangeVpsFlag { get { return video_full_range_vps_flag; } set { video_full_range_vps_flag = value; } }
        private uint colour_primaries_vps;
        public uint ColourPrimariesVps { get { return colour_primaries_vps; } set { colour_primaries_vps = value; } }
        private uint transfer_characteristics_vps;
        public uint TransferCharacteristicsVps { get { return transfer_characteristics_vps; } set { transfer_characteristics_vps = value; } }
        private uint matrix_coeffs_vps;
        public uint MatrixCoeffsVps { get { return matrix_coeffs_vps; } set { matrix_coeffs_vps = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VideoSignalInfo()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 3, out this.video_vps_format);
            size += stream.ReadUnsignedInt(size, 1, out this.video_full_range_vps_flag);
            size += stream.ReadUnsignedInt(size, 8, out this.colour_primaries_vps);
            size += stream.ReadUnsignedInt(size, 8, out this.transfer_characteristics_vps);
            size += stream.ReadUnsignedInt(size, 8, out this.matrix_coeffs_vps);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(3, this.video_vps_format);
            size += stream.WriteUnsignedInt(1, this.video_full_range_vps_flag);
            size += stream.WriteUnsignedInt(8, this.colour_primaries_vps);
            size += stream.WriteUnsignedInt(8, this.transfer_characteristics_vps);
            size += stream.WriteUnsignedInt(8, this.matrix_coeffs_vps);

            return size;
        }

    }

    /*
  

vps_vui_bsp_hrd_params() {  
 vps_num_add_hrd_params ue(v) 
 for( i = vps_num_hrd_parameters; i < vps_num_hrd_parameters + 
                  vps_num_add_hrd_params; i++ ) { 
 
  if( i > 0 )  
   cprms_add_present_flag[ i ] u(1) 
  num_sub_layer_hrd_minus1[ i ] ue(v) 
  hrd_parameters( cprms_add_present_flag[ i ], num_sub_layer_hrd_minus1[ i ] )  
 }  
 if( vps_num_hrd_parameters + vps_num_add_hrd_params > 0 )  
  for( h = 1; h < NumOutputLayerSets; h++ ) {  
   num_signalled_partitioning_schemes[ h ] ue(v) 
   for( j = 1; j < num_signalled_partitioning_schemes[ h ] + 1; j++ ) {  
    num_partitions_in_scheme_minus1[ h ][ j ] ue(v) 
    for( k = 0; k <= num_partitions_in_scheme_minus1[ h ][ j ]; k++ )  
     for( r = 0; r < NumLayersInIdList[ OlsIdxToLsIdx[ h ] ]; r++ )  
      layer_included_in_partition_flag[ h ][ j ][ k ][ r ] u(1) 
   }  
   for( i = 0; i < num_signalled_partitioning_schemes[ h ] + 1; i++ )  
    for( t = 0; t <= MaxSubLayersInLayerSetMinus1[ OlsIdxToLsIdx[ h ] ]; t++ ) {  
     num_bsp_schedules_minus1[ h ][ i ][ t ] ue(v) 
     for( j = 0; j <= num_bsp_schedules_minus1[ h ][ i ][ t ]; j++ )  
      for( k = 0; k <= num_partitions_in_scheme_minus1[ h ][ i ]; k++ ) {  
       if( vps_num_hrd_parameters + vps_num_add_hrd_params > 1 )  
        bsp_hrd_idx[ h ][ i ][ t ][ j ][ k ] u(v) 
       bsp_sched_idx[ h ][ i ][ t ][ j ][ k ] ue(v) 
      }  
    }  
  }  
}
    */
    public class VpsVuiBspHrdParams : IItuSerializable
    {
        private uint vps_num_add_hrd_params;
        public uint VpsNumAddHrdParams { get { return vps_num_add_hrd_params; } set { vps_num_add_hrd_params = value; } }
        private byte[] cprms_add_present_flag;
        public byte[] CprmsAddPresentFlag { get { return cprms_add_present_flag; } set { cprms_add_present_flag = value; } }
        private uint[] num_sub_layer_hrd_minus1;
        public uint[] NumSubLayerHrdMinus1 { get { return num_sub_layer_hrd_minus1; } set { num_sub_layer_hrd_minus1 = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private uint[] num_signalled_partitioning_schemes;
        public uint[] NumSignalledPartitioningSchemes { get { return num_signalled_partitioning_schemes; } set { num_signalled_partitioning_schemes = value; } }
        private uint[][] num_partitions_in_scheme_minus1;
        public uint[][] NumPartitionsInSchemeMinus1 { get { return num_partitions_in_scheme_minus1; } set { num_partitions_in_scheme_minus1 = value; } }
        private byte[][][][] layer_included_in_partition_flag;
        public byte[][][][] LayerIncludedInPartitionFlag { get { return layer_included_in_partition_flag; } set { layer_included_in_partition_flag = value; } }
        private uint[][][] num_bsp_schedules_minus1;
        public uint[][][] NumBspSchedulesMinus1 { get { return num_bsp_schedules_minus1; } set { num_bsp_schedules_minus1 = value; } }
        private uint[][][][][] bsp_hrd_idx;
        public uint[][][][][] BspHrdIdx { get { return bsp_hrd_idx; } set { bsp_hrd_idx = value; } }
        private uint[][][][][] bsp_sched_idx;
        public uint[][][][][] BspSchedIdx { get { return bsp_sched_idx; } set { bsp_sched_idx = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public VpsVuiBspHrdParams()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint h = 0;
            uint j = 0;
            uint k = 0;
            uint r = 0;
            uint t = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_add_hrd_params);

            this.cprms_add_present_flag = new byte[vps_num_hrd_parameters +
                  vps_num_add_hrd_params];
            this.num_sub_layer_hrd_minus1 = new uint[vps_num_hrd_parameters +
                  vps_num_add_hrd_params];
            this.hrd_parameters = new HrdParameters[vps_num_hrd_parameters +
                  vps_num_add_hrd_params];
            for (i = vps_num_hrd_parameters; i < vps_num_hrd_parameters +
                  vps_num_add_hrd_params; i++)
            {

                if (i > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.cprms_add_present_flag[i]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.num_sub_layer_hrd_minus1[i]);
                this.hrd_parameters[i] = new HrdParameters(cprms_add_present_flag[i], num_sub_layer_hrd_minus1[i]);
                size += stream.ReadClass<HrdParameters>(size, context, this.hrd_parameters[i]);
            }

            if (vps_num_hrd_parameters + vps_num_add_hrd_params > 0)
            {

                this.num_signalled_partitioning_schemes = new uint[NumOutputLayerSets];
                this.num_partitions_in_scheme_minus1 = new uint[NumOutputLayerSets][];
                this.layer_included_in_partition_flag = new byte[NumOutputLayerSets][][][];
                this.num_bsp_schedules_minus1 = new uint[NumOutputLayerSets][][];
                this.bsp_hrd_idx = new uint[NumOutputLayerSets][][][][];
                this.bsp_sched_idx = new uint[NumOutputLayerSets][][][][];
                for (h = 1; h < NumOutputLayerSets; h++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_signalled_partitioning_schemes[h]);

                    this.num_partitions_in_scheme_minus1[h] = new uint[num_signalled_partitioning_schemes[h] + 1];
                    this.layer_included_in_partition_flag[h] = new byte[num_signalled_partitioning_schemes[h] + 1][][];
                    for (j = 1; j < num_signalled_partitioning_schemes[h] + 1; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_partitions_in_scheme_minus1[h][j]);

                        this.layer_included_in_partition_flag[h][j] = new byte[num_partitions_in_scheme_minus1 + 1[h][j]][];
                        for (k = 0; k <= num_partitions_in_scheme_minus1[h][j]; k++)
                        {

                            this.layer_included_in_partition_flag[h][j][k] = new byte[NumLayersInIdList[OlsIdxToLsIdx[h]]];
                            for (r = 0; r < NumLayersInIdList[OlsIdxToLsIdx[h]]; r++)
                            {
                                size += stream.ReadUnsignedInt(size, 1, out this.layer_included_in_partition_flag[h][j][k][r]);
                            }
                        }
                    }

                    this.num_bsp_schedules_minus1[h] = new uint[num_signalled_partitioning_schemes[h] + 1][];
                    this.bsp_hrd_idx[h] = new uint[num_signalled_partitioning_schemes[h] + 1][][][];
                    this.bsp_sched_idx[h] = new uint[num_signalled_partitioning_schemes[h] + 1][][][];
                    for (i = 0; i < num_signalled_partitioning_schemes[h] + 1; i++)
                    {

                        this.num_bsp_schedules_minus1[h][i] = new uint[MaxSubLayersInLayerSetMinus1 + 1[OlsIdxToLsIdx[h]]];
                        this.bsp_hrd_idx[h][i] = new uint[MaxSubLayersInLayerSetMinus1 + 1[OlsIdxToLsIdx[h]]][][];
                        this.bsp_sched_idx[h][i] = new uint[MaxSubLayersInLayerSetMinus1 + 1[OlsIdxToLsIdx[h]]][][];
                        for (t = 0; t <= MaxSubLayersInLayerSetMinus1[OlsIdxToLsIdx[h]]; t++)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.num_bsp_schedules_minus1[h][i][t]);

                            this.bsp_hrd_idx[h][i][t] = new uint[num_bsp_schedules_minus1 + 1[h][i][t]][];
                            this.bsp_sched_idx[h][i][t] = new uint[num_bsp_schedules_minus1 + 1[h][i][t]][];
                            for (j = 0; j <= num_bsp_schedules_minus1[h][i][t]; j++)
                            {

                                this.bsp_hrd_idx[h][i][t][j] = new uint[num_partitions_in_scheme_minus1 + 1[h][i]];
                                this.bsp_sched_idx[h][i][t][j] = new uint[num_partitions_in_scheme_minus1 + 1[h][i]];
                                for (k = 0; k <= num_partitions_in_scheme_minus1[h][i]; k++)
                                {

                                    if (vps_num_hrd_parameters + vps_num_add_hrd_params > 1)
                                    {
                                        size += stream.ReadUnsignedIntVariable(size, bsp_hrd_idx, out this.bsp_hrd_idx[h][i][t][j][k]);
                                    }
                                    size += stream.ReadUnsignedIntGolomb(size, out this.bsp_sched_idx[h][i][t][j][k]);
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

            uint i = 0;
            uint h = 0;
            uint j = 0;
            uint k = 0;
            uint r = 0;
            uint t = 0;
            size += stream.WriteUnsignedIntGolomb(this.vps_num_add_hrd_params);

            for (i = vps_num_hrd_parameters; i < vps_num_hrd_parameters +
                  vps_num_add_hrd_params; i++)
            {

                if (i > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.cprms_add_present_flag[i]);
                }
                size += stream.WriteUnsignedIntGolomb(this.num_sub_layer_hrd_minus1[i]);
                size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
            }

            if (vps_num_hrd_parameters + vps_num_add_hrd_params > 0)
            {

                for (h = 1; h < NumOutputLayerSets; h++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_signalled_partitioning_schemes[h]);

                    for (j = 1; j < num_signalled_partitioning_schemes[h] + 1; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_partitions_in_scheme_minus1[h][j]);

                        for (k = 0; k <= num_partitions_in_scheme_minus1[h][j]; k++)
                        {

                            for (r = 0; r < NumLayersInIdList[OlsIdxToLsIdx[h]]; r++)
                            {
                                size += stream.WriteUnsignedInt(1, this.layer_included_in_partition_flag[h][j][k][r]);
                            }
                        }
                    }

                    for (i = 0; i < num_signalled_partitioning_schemes[h] + 1; i++)
                    {

                        for (t = 0; t <= MaxSubLayersInLayerSetMinus1[OlsIdxToLsIdx[h]]; t++)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.num_bsp_schedules_minus1[h][i][t]);

                            for (j = 0; j <= num_bsp_schedules_minus1[h][i][t]; j++)
                            {

                                for (k = 0; k <= num_partitions_in_scheme_minus1[h][i]; k++)
                                {

                                    if (vps_num_hrd_parameters + vps_num_add_hrd_params > 1)
                                    {
                                        size += stream.WriteUnsignedIntVariable(bsp_hrd_idx[h][i][t][j][k], this.bsp_hrd_idx[h][i][t][j][k]);
                                    }
                                    size += stream.WriteUnsignedIntGolomb(this.bsp_sched_idx[h][i][t][j][k]);
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
 

seq_parameter_set_rbsp() { 
 sps_video_parameter_set_id u(4) 
 if( nuh_layer_id == 0 )  
  sps_max_sub_layers_minus1 u(3) 
 else  
  sps_ext_or_max_sub_layers_minus1 u(3) 
 MultiLayerExtSpsFlag =  ( nuh_layer_id != 0  &&  sps_ext_or_max_sub_layers_minus1 == 7 ) 
 if( !MultiLayerExtSpsFlag ) {  
  sps_temporal_id_nesting_flag u(1) 
  profile_tier_level( 1, sps_max_sub_layers_minus1 )  
 }
  sps_seq_parameter_set_id ue(v) 
 if( MultiLayerExtSpsFlag ) {  
  update_rep_format_flag u(1) 
  if( update_rep_format_flag )  
   sps_rep_format_idx u(8) 
 } else {  
  chroma_format_idc ue(v) 
  if( chroma_format_idc == 3 )  
   separate_colour_plane_flag u(1) 
  pic_width_in_luma_samples ue(v) 
  pic_height_in_luma_samples ue(v) 
  conformance_window_flag u(1) 
  if( conformance_window_flag ) {  
   conf_win_left_offset ue(v) 
   conf_win_right_offset ue(v) 
   conf_win_top_offset ue(v) 
   conf_win_bottom_offset ue(v) 
  }  
  bit_depth_luma_minus8 ue(v) 
  bit_depth_chroma_minus8 ue(v) 
 }  
 log2_max_pic_order_cnt_lsb_minus4 ue(v) 
 if( !MultiLayerExtSpsFlag ) {  
  sps_sub_layer_ordering_info_present_flag u(1) 
  for( i = ( sps_sub_layer_ordering_info_present_flag != 0 ?  0 : sps_max_sub_layers_minus1 ); 
    i <= sps_max_sub_layers_minus1; i++ ) { 
 
   sps_max_dec_pic_buffering_minus1[ i ] ue(v) 
   sps_max_num_reorder_pics[ i ] ue(v) 
   sps_max_latency_increase_plus1[ i ] ue(v) 
  }  
 }  
 log2_min_luma_coding_block_size_minus3 ue(v) 
 log2_diff_max_min_luma_coding_block_size ue(v) 
 log2_min_luma_transform_block_size_minus2 ue(v) 
 log2_diff_max_min_luma_transform_block_size ue(v) 
 max_transform_hierarchy_depth_inter ue(v) 
 max_transform_hierarchy_depth_intra ue(v) 
 scaling_list_enabled_flag u(1) 
 if( scaling_list_enabled_flag ) {  
  if( MultiLayerExtSpsFlag )  
   sps_infer_scaling_list_flag u(1) 
  if( sps_infer_scaling_list_flag )  
   sps_scaling_list_ref_layer_id u(6) 
  else {  
   sps_scaling_list_data_present_flag u(1) 
   if( sps_scaling_list_data_present_flag )  
    scaling_list_data()  
  }  
 } 
  amp_enabled_flag u(1) 
 sample_adaptive_offset_enabled_flag u(1) 
 pcm_enabled_flag u(1) 
 if( pcm_enabled_flag ) {  
  pcm_sample_bit_depth_luma_minus1 u(4) 
  pcm_sample_bit_depth_chroma_minus1 u(4) 
  log2_min_pcm_luma_coding_block_size_minus3 ue(v) 
  log2_diff_max_min_pcm_luma_coding_block_size ue(v) 
  pcm_loop_filter_disabled_flag u(1) 
 }  
 num_short_term_ref_pic_sets ue(v) 
 for( i = 0; i < num_short_term_ref_pic_sets; i++ )  
  st_ref_pic_set( i )  
 long_term_ref_pics_present_flag u(1) 
 if( long_term_ref_pics_present_flag ) {  
  num_long_term_ref_pics_sps ue(v) 
  for( i = 0; i < num_long_term_ref_pics_sps; i++ ) {  
   lt_ref_pic_poc_lsb_sps[ i ] u(v) 
   used_by_curr_pic_lt_sps_flag[ i ] u(1) 
  }  
 }  
 sps_temporal_mvp_enabled_flag u(1) 
 strong_intra_smoothing_enabled_flag u(1) 
 vui_parameters_present_flag u(1) 
 if( vui_parameters_present_flag )  
  vui_parameters()  
 sps_extension_present_flag u(1) 
 if( sps_extension_present_flag ) {  
  sps_range_extension_flag u(1) 
  sps_multilayer_extension_flag u(1) 
  sps_3d_extension_flag u(1) 
  sps_scc_extension_flag u(1) 
  sps_extension_4bits u(5) 
 }  
 if( sps_range_extension_flag )  
  sps_range_extension()  
 if( sps_multilayer_extension_flag )  
  sps_multilayer_extension()  
 if( sps_3d_extension_flag )  
  sps_3d_extension()  /* specified in Annex I *//*  
 if( sps_scc_extension_flag )  
  sps_scc_extension()  
 if( sps_extension_4bits )  
  while( more_rbsp_data() )  
   sps_extension_data_flag u(1) 
 rbsp_trailing_bits()  
}
    */
    public class SeqParameterSetRbsp : IItuSerializable
    {
        private uint sps_video_parameter_set_id;
        public uint SpsVideoParameterSetId { get { return sps_video_parameter_set_id; } set { sps_video_parameter_set_id = value; } }
        private uint sps_max_sub_layers_minus1;
        public uint SpsMaxSubLayersMinus1 { get { return sps_max_sub_layers_minus1; } set { sps_max_sub_layers_minus1 = value; } }
        private uint sps_ext_or_max_sub_layers_minus1;
        public uint SpsExtOrMaxSubLayersMinus1 { get { return sps_ext_or_max_sub_layers_minus1; } set { sps_ext_or_max_sub_layers_minus1 = value; } }
        private byte sps_temporal_id_nesting_flag;
        public byte SpsTemporalIdNestingFlag { get { return sps_temporal_id_nesting_flag; } set { sps_temporal_id_nesting_flag = value; } }
        private ProfileTierLevel profile_tier_level;
        public ProfileTierLevel ProfileTierLevel { get { return profile_tier_level; } set { profile_tier_level = value; } }
        private uint sps_seq_parameter_set_id;
        public uint SpsSeqParameterSetId { get { return sps_seq_parameter_set_id; } set { sps_seq_parameter_set_id = value; } }
        private byte update_rep_format_flag;
        public byte UpdateRepFormatFlag { get { return update_rep_format_flag; } set { update_rep_format_flag = value; } }
        private uint sps_rep_format_idx;
        public uint SpsRepFormatIdx { get { return sps_rep_format_idx; } set { sps_rep_format_idx = value; } }
        private uint chroma_format_idc;
        public uint ChromaFormatIdc { get { return chroma_format_idc; } set { chroma_format_idc = value; } }
        private byte separate_colour_plane_flag;
        public byte SeparateColourPlaneFlag { get { return separate_colour_plane_flag; } set { separate_colour_plane_flag = value; } }
        private uint pic_width_in_luma_samples;
        public uint PicWidthInLumaSamples { get { return pic_width_in_luma_samples; } set { pic_width_in_luma_samples = value; } }
        private uint pic_height_in_luma_samples;
        public uint PicHeightInLumaSamples { get { return pic_height_in_luma_samples; } set { pic_height_in_luma_samples = value; } }
        private byte conformance_window_flag;
        public byte ConformanceWindowFlag { get { return conformance_window_flag; } set { conformance_window_flag = value; } }
        private uint conf_win_left_offset;
        public uint ConfWinLeftOffset { get { return conf_win_left_offset; } set { conf_win_left_offset = value; } }
        private uint conf_win_right_offset;
        public uint ConfWinRightOffset { get { return conf_win_right_offset; } set { conf_win_right_offset = value; } }
        private uint conf_win_top_offset;
        public uint ConfWinTopOffset { get { return conf_win_top_offset; } set { conf_win_top_offset = value; } }
        private uint conf_win_bottom_offset;
        public uint ConfWinBottomOffset { get { return conf_win_bottom_offset; } set { conf_win_bottom_offset = value; } }
        private uint bit_depth_luma_minus8;
        public uint BitDepthLumaMinus8 { get { return bit_depth_luma_minus8; } set { bit_depth_luma_minus8 = value; } }
        private uint bit_depth_chroma_minus8;
        public uint BitDepthChromaMinus8 { get { return bit_depth_chroma_minus8; } set { bit_depth_chroma_minus8 = value; } }
        private uint log2_max_pic_order_cnt_lsb_minus4;
        public uint Log2MaxPicOrderCntLsbMinus4 { get { return log2_max_pic_order_cnt_lsb_minus4; } set { log2_max_pic_order_cnt_lsb_minus4 = value; } }
        private byte sps_sub_layer_ordering_info_present_flag;
        public byte SpsSubLayerOrderingInfoPresentFlag { get { return sps_sub_layer_ordering_info_present_flag; } set { sps_sub_layer_ordering_info_present_flag = value; } }
        private uint[] sps_max_dec_pic_buffering_minus1;
        public uint[] SpsMaxDecPicBufferingMinus1 { get { return sps_max_dec_pic_buffering_minus1; } set { sps_max_dec_pic_buffering_minus1 = value; } }
        private uint[] sps_max_num_reorder_pics;
        public uint[] SpsMaxNumReorderPics { get { return sps_max_num_reorder_pics; } set { sps_max_num_reorder_pics = value; } }
        private uint[] sps_max_latency_increase_plus1;
        public uint[] SpsMaxLatencyIncreasePlus1 { get { return sps_max_latency_increase_plus1; } set { sps_max_latency_increase_plus1 = value; } }
        private uint log2_min_luma_coding_block_size_minus3;
        public uint Log2MinLumaCodingBlockSizeMinus3 { get { return log2_min_luma_coding_block_size_minus3; } set { log2_min_luma_coding_block_size_minus3 = value; } }
        private uint log2_diff_max_min_luma_coding_block_size;
        public uint Log2DiffMaxMinLumaCodingBlockSize { get { return log2_diff_max_min_luma_coding_block_size; } set { log2_diff_max_min_luma_coding_block_size = value; } }
        private uint log2_min_luma_transform_block_size_minus2;
        public uint Log2MinLumaTransformBlockSizeMinus2 { get { return log2_min_luma_transform_block_size_minus2; } set { log2_min_luma_transform_block_size_minus2 = value; } }
        private uint log2_diff_max_min_luma_transform_block_size;
        public uint Log2DiffMaxMinLumaTransformBlockSize { get { return log2_diff_max_min_luma_transform_block_size; } set { log2_diff_max_min_luma_transform_block_size = value; } }
        private uint max_transform_hierarchy_depth_inter;
        public uint MaxTransformHierarchyDepthInter { get { return max_transform_hierarchy_depth_inter; } set { max_transform_hierarchy_depth_inter = value; } }
        private uint max_transform_hierarchy_depth_intra;
        public uint MaxTransformHierarchyDepthIntra { get { return max_transform_hierarchy_depth_intra; } set { max_transform_hierarchy_depth_intra = value; } }
        private byte scaling_list_enabled_flag;
        public byte ScalingListEnabledFlag { get { return scaling_list_enabled_flag; } set { scaling_list_enabled_flag = value; } }
        private byte sps_infer_scaling_list_flag;
        public byte SpsInferScalingListFlag { get { return sps_infer_scaling_list_flag; } set { sps_infer_scaling_list_flag = value; } }
        private uint sps_scaling_list_ref_layer_id;
        public uint SpsScalingListRefLayerId { get { return sps_scaling_list_ref_layer_id; } set { sps_scaling_list_ref_layer_id = value; } }
        private byte sps_scaling_list_data_present_flag;
        public byte SpsScalingListDataPresentFlag { get { return sps_scaling_list_data_present_flag; } set { sps_scaling_list_data_present_flag = value; } }
        private ScalingListData scaling_list_data;
        public ScalingListData ScalingListData { get { return scaling_list_data; } set { scaling_list_data = value; } }
        private byte amp_enabled_flag;
        public byte AmpEnabledFlag { get { return amp_enabled_flag; } set { amp_enabled_flag = value; } }
        private byte sample_adaptive_offset_enabled_flag;
        public byte SampleAdaptiveOffsetEnabledFlag { get { return sample_adaptive_offset_enabled_flag; } set { sample_adaptive_offset_enabled_flag = value; } }
        private byte pcm_enabled_flag;
        public byte PcmEnabledFlag { get { return pcm_enabled_flag; } set { pcm_enabled_flag = value; } }
        private uint pcm_sample_bit_depth_luma_minus1;
        public uint PcmSampleBitDepthLumaMinus1 { get { return pcm_sample_bit_depth_luma_minus1; } set { pcm_sample_bit_depth_luma_minus1 = value; } }
        private uint pcm_sample_bit_depth_chroma_minus1;
        public uint PcmSampleBitDepthChromaMinus1 { get { return pcm_sample_bit_depth_chroma_minus1; } set { pcm_sample_bit_depth_chroma_minus1 = value; } }
        private uint log2_min_pcm_luma_coding_block_size_minus3;
        public uint Log2MinPcmLumaCodingBlockSizeMinus3 { get { return log2_min_pcm_luma_coding_block_size_minus3; } set { log2_min_pcm_luma_coding_block_size_minus3 = value; } }
        private uint log2_diff_max_min_pcm_luma_coding_block_size;
        public uint Log2DiffMaxMinPcmLumaCodingBlockSize { get { return log2_diff_max_min_pcm_luma_coding_block_size; } set { log2_diff_max_min_pcm_luma_coding_block_size = value; } }
        private byte pcm_loop_filter_disabled_flag;
        public byte PcmLoopFilterDisabledFlag { get { return pcm_loop_filter_disabled_flag; } set { pcm_loop_filter_disabled_flag = value; } }
        private uint num_short_term_ref_pic_sets;
        public uint NumShortTermRefPicSets { get { return num_short_term_ref_pic_sets; } set { num_short_term_ref_pic_sets = value; } }
        private StRefPicSet[] st_ref_pic_set;
        public StRefPicSet[] StRefPicSet { get { return st_ref_pic_set; } set { st_ref_pic_set = value; } }
        private byte long_term_ref_pics_present_flag;
        public byte LongTermRefPicsPresentFlag { get { return long_term_ref_pics_present_flag; } set { long_term_ref_pics_present_flag = value; } }
        private uint num_long_term_ref_pics_sps;
        public uint NumLongTermRefPicsSps { get { return num_long_term_ref_pics_sps; } set { num_long_term_ref_pics_sps = value; } }
        private uint[] lt_ref_pic_poc_lsb_sps;
        public uint[] LtRefPicPocLsbSps { get { return lt_ref_pic_poc_lsb_sps; } set { lt_ref_pic_poc_lsb_sps = value; } }
        private byte[] used_by_curr_pic_lt_sps_flag;
        public byte[] UsedByCurrPicLtSpsFlag { get { return used_by_curr_pic_lt_sps_flag; } set { used_by_curr_pic_lt_sps_flag = value; } }
        private byte sps_temporal_mvp_enabled_flag;
        public byte SpsTemporalMvpEnabledFlag { get { return sps_temporal_mvp_enabled_flag; } set { sps_temporal_mvp_enabled_flag = value; } }
        private byte strong_intra_smoothing_enabled_flag;
        public byte StrongIntraSmoothingEnabledFlag { get { return strong_intra_smoothing_enabled_flag; } set { strong_intra_smoothing_enabled_flag = value; } }
        private byte vui_parameters_present_flag;
        public byte VuiParametersPresentFlag { get { return vui_parameters_present_flag; } set { vui_parameters_present_flag = value; } }
        private VuiParameters vui_parameters;
        public VuiParameters VuiParameters { get { return vui_parameters; } set { vui_parameters = value; } }
        private byte sps_extension_present_flag;
        public byte SpsExtensionPresentFlag { get { return sps_extension_present_flag; } set { sps_extension_present_flag = value; } }
        private byte sps_range_extension_flag;
        public byte SpsRangeExtensionFlag { get { return sps_range_extension_flag; } set { sps_range_extension_flag = value; } }
        private byte sps_multilayer_extension_flag;
        public byte SpsMultilayerExtensionFlag { get { return sps_multilayer_extension_flag; } set { sps_multilayer_extension_flag = value; } }
        private byte sps_3d_extension_flag;
        public byte Sps3dExtensionFlag { get { return sps_3d_extension_flag; } set { sps_3d_extension_flag = value; } }
        private byte sps_scc_extension_flag;
        public byte SpsSccExtensionFlag { get { return sps_scc_extension_flag; } set { sps_scc_extension_flag = value; } }
        private uint sps_extension_4bits;
        public uint SpsExtension4bits { get { return sps_extension_4bits; } set { sps_extension_4bits = value; } }
        private SpsRangeExtension sps_range_extension;
        public SpsRangeExtension SpsRangeExtension { get { return sps_range_extension; } set { sps_range_extension = value; } }
        private SpsMultilayerExtension sps_multilayer_extension;
        public SpsMultilayerExtension SpsMultilayerExtension { get { return sps_multilayer_extension; } set { sps_multilayer_extension = value; } }
        private Sps3dExtension sps_3d_extension;
        public Sps3dExtension Sps3dExtension { get { return sps_3d_extension; } set { sps_3d_extension = value; } }
        private SpsSccExtension sps_scc_extension;
        public SpsSccExtension SpsSccExtension { get { return sps_scc_extension; } set { sps_scc_extension = value; } }
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

            uint MultiLayerExtSpsFlag = 0;
            uint i = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 4, out this.sps_video_parameter_set_id);

            if (nuh_layer_id == 0)
            {
                size += stream.ReadUnsignedInt(size, 3, out this.sps_max_sub_layers_minus1);
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 3, out this.sps_ext_or_max_sub_layers_minus1);
            }
            MultiLayerExtSpsFlag = (nuh_layer_id != 0 && sps_ext_or_max_sub_layers_minus1 == 7) ? (uint)1 : (uint)0;

            if (MultiLayerExtSpsFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_temporal_id_nesting_flag);
                this.profile_tier_level = new ProfileTierLevel(1, sps_max_sub_layers_minus1);
                size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.sps_seq_parameter_set_id);

            if (MultiLayerExtSpsFlag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.update_rep_format_flag);

                if (update_rep_format_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.sps_rep_format_idx);
                }
            }
            else
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.chroma_format_idc);

                if (chroma_format_idc == 3)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.separate_colour_plane_flag);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.pic_width_in_luma_samples);
                size += stream.ReadUnsignedIntGolomb(size, out this.pic_height_in_luma_samples);
                size += stream.ReadUnsignedInt(size, 1, out this.conformance_window_flag);

                if (conformance_window_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_left_offset);
                    size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_right_offset);
                    size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_top_offset);
                    size += stream.ReadUnsignedIntGolomb(size, out this.conf_win_bottom_offset);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_depth_luma_minus8);
                size += stream.ReadUnsignedIntGolomb(size, out this.bit_depth_chroma_minus8);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_max_pic_order_cnt_lsb_minus4);

            if (MultiLayerExtSpsFlag == 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_sub_layer_ordering_info_present_flag);

                this.sps_max_dec_pic_buffering_minus1 = new uint[sps_max_sub_layers_minus1 + 1];
                this.sps_max_num_reorder_pics = new uint[sps_max_sub_layers_minus1 + 1];
                this.sps_max_latency_increase_plus1 = new uint[sps_max_sub_layers_minus1 + 1];
                for (i = (sps_sub_layer_ordering_info_present_flag != 0 ? 0 : sps_max_sub_layers_minus1);
    i <= sps_max_sub_layers_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_dec_pic_buffering_minus1[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_num_reorder_pics[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.sps_max_latency_increase_plus1[i]);
                }
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_min_luma_coding_block_size_minus3);
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_diff_max_min_luma_coding_block_size);
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_min_luma_transform_block_size_minus2);
            size += stream.ReadUnsignedIntGolomb(size, out this.log2_diff_max_min_luma_transform_block_size);
            size += stream.ReadUnsignedIntGolomb(size, out this.max_transform_hierarchy_depth_inter);
            size += stream.ReadUnsignedIntGolomb(size, out this.max_transform_hierarchy_depth_intra);
            size += stream.ReadUnsignedInt(size, 1, out this.scaling_list_enabled_flag);

            if (scaling_list_enabled_flag != 0)
            {

                if (MultiLayerExtSpsFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_infer_scaling_list_flag);
                }

                if (sps_infer_scaling_list_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 6, out this.sps_scaling_list_ref_layer_id);
                }
                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sps_scaling_list_data_present_flag);

                    if (sps_scaling_list_data_present_flag != 0)
                    {
                        this.scaling_list_data = new ScalingListData();
                        size += stream.ReadClass<ScalingListData>(size, context, this.scaling_list_data);
                    }
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.amp_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.sample_adaptive_offset_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pcm_enabled_flag);

            if (pcm_enabled_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 4, out this.pcm_sample_bit_depth_luma_minus1);
                size += stream.ReadUnsignedInt(size, 4, out this.pcm_sample_bit_depth_chroma_minus1);
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_min_pcm_luma_coding_block_size_minus3);
                size += stream.ReadUnsignedIntGolomb(size, out this.log2_diff_max_min_pcm_luma_coding_block_size);
                size += stream.ReadUnsignedInt(size, 1, out this.pcm_loop_filter_disabled_flag);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_short_term_ref_pic_sets);

            this.st_ref_pic_set = new StRefPicSet[num_short_term_ref_pic_sets];
            for (i = 0; i < num_short_term_ref_pic_sets; i++)
            {
                this.st_ref_pic_set[i] = new StRefPicSet(i);
                size += stream.ReadClass<StRefPicSet>(size, context, this.st_ref_pic_set[i]);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.long_term_ref_pics_present_flag);

            if (long_term_ref_pics_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_long_term_ref_pics_sps);

                this.lt_ref_pic_poc_lsb_sps = new uint[num_long_term_ref_pics_sps];
                this.used_by_curr_pic_lt_sps_flag = new byte[num_long_term_ref_pics_sps];
                for (i = 0; i < num_long_term_ref_pics_sps; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, lt_ref_pic_poc_lsb_sps, out this.lt_ref_pic_poc_lsb_sps[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.used_by_curr_pic_lt_sps_flag[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_temporal_mvp_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.strong_intra_smoothing_enabled_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vui_parameters_present_flag);

            if (vui_parameters_present_flag != 0)
            {
                this.vui_parameters = new VuiParameters();
                size += stream.ReadClass<VuiParameters>(size, context, this.vui_parameters);
            }
            size += stream.ReadUnsignedInt(size, 1, out this.sps_extension_present_flag);

            if (sps_extension_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.sps_range_extension_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_multilayer_extension_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_3d_extension_flag);
                size += stream.ReadUnsignedInt(size, 1, out this.sps_scc_extension_flag);
                size += stream.ReadUnsignedInt(size, 5, out this.sps_extension_4bits);
            }

            if (sps_range_extension_flag != 0)
            {
                this.sps_range_extension = new SpsRangeExtension();
                size += stream.ReadClass<SpsRangeExtension>(size, context, this.sps_range_extension);
            }

            if (sps_multilayer_extension_flag != 0)
            {
                this.sps_multilayer_extension = new SpsMultilayerExtension();
                size += stream.ReadClass<SpsMultilayerExtension>(size, context, this.sps_multilayer_extension);
            }

            if (sps_3d_extension_flag != 0)
            {
                this.sps_3d_extension = new Sps3dExtension();
                size += stream.ReadClass<Sps3dExtension>(size, context, this.sps_3d_extension); // specified in Annex I 
            }

            if (sps_scc_extension_flag != 0)
            {
                this.sps_scc_extension = new SpsSccExtension();
                size += stream.ReadClass<SpsSccExtension>(size, context, this.sps_scc_extension);
            }

            if (sps_extension_4bits != 0)
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

            uint MultiLayerExtSpsFlag = 0;
            uint i = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(4, this.sps_video_parameter_set_id);

            if (nuh_layer_id == 0)
            {
                size += stream.WriteUnsignedInt(3, this.sps_max_sub_layers_minus1);
            }
            else
            {
                size += stream.WriteUnsignedInt(3, this.sps_ext_or_max_sub_layers_minus1);
            }
            MultiLayerExtSpsFlag = (nuh_layer_id != 0 && sps_ext_or_max_sub_layers_minus1 == 7) ? (uint)1 : (uint)0;

            if (MultiLayerExtSpsFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_temporal_id_nesting_flag);
                size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level);
            }
            size += stream.WriteUnsignedIntGolomb(this.sps_seq_parameter_set_id);

            if (MultiLayerExtSpsFlag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.update_rep_format_flag);

                if (update_rep_format_flag != 0)
                {
                    size += stream.WriteUnsignedInt(8, this.sps_rep_format_idx);
                }
            }
            else
            {
                size += stream.WriteUnsignedIntGolomb(this.chroma_format_idc);

                if (chroma_format_idc == 3)
                {
                    size += stream.WriteUnsignedInt(1, this.separate_colour_plane_flag);
                }
                size += stream.WriteUnsignedIntGolomb(this.pic_width_in_luma_samples);
                size += stream.WriteUnsignedIntGolomb(this.pic_height_in_luma_samples);
                size += stream.WriteUnsignedInt(1, this.conformance_window_flag);

                if (conformance_window_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.conf_win_left_offset);
                    size += stream.WriteUnsignedIntGolomb(this.conf_win_right_offset);
                    size += stream.WriteUnsignedIntGolomb(this.conf_win_top_offset);
                    size += stream.WriteUnsignedIntGolomb(this.conf_win_bottom_offset);
                }
                size += stream.WriteUnsignedIntGolomb(this.bit_depth_luma_minus8);
                size += stream.WriteUnsignedIntGolomb(this.bit_depth_chroma_minus8);
            }
            size += stream.WriteUnsignedIntGolomb(this.log2_max_pic_order_cnt_lsb_minus4);

            if (MultiLayerExtSpsFlag == 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_sub_layer_ordering_info_present_flag);

                for (i = (sps_sub_layer_ordering_info_present_flag != 0 ? 0 : sps_max_sub_layers_minus1);
    i <= sps_max_sub_layers_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.sps_max_dec_pic_buffering_minus1[i]);
                    size += stream.WriteUnsignedIntGolomb(this.sps_max_num_reorder_pics[i]);
                    size += stream.WriteUnsignedIntGolomb(this.sps_max_latency_increase_plus1[i]);
                }
            }
            size += stream.WriteUnsignedIntGolomb(this.log2_min_luma_coding_block_size_minus3);
            size += stream.WriteUnsignedIntGolomb(this.log2_diff_max_min_luma_coding_block_size);
            size += stream.WriteUnsignedIntGolomb(this.log2_min_luma_transform_block_size_minus2);
            size += stream.WriteUnsignedIntGolomb(this.log2_diff_max_min_luma_transform_block_size);
            size += stream.WriteUnsignedIntGolomb(this.max_transform_hierarchy_depth_inter);
            size += stream.WriteUnsignedIntGolomb(this.max_transform_hierarchy_depth_intra);
            size += stream.WriteUnsignedInt(1, this.scaling_list_enabled_flag);

            if (scaling_list_enabled_flag != 0)
            {

                if (MultiLayerExtSpsFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.sps_infer_scaling_list_flag);
                }

                if (sps_infer_scaling_list_flag != 0)
                {
                    size += stream.WriteUnsignedInt(6, this.sps_scaling_list_ref_layer_id);
                }
                else
                {
                    size += stream.WriteUnsignedInt(1, this.sps_scaling_list_data_present_flag);

                    if (sps_scaling_list_data_present_flag != 0)
                    {
                        size += stream.WriteClass<ScalingListData>(context, this.scaling_list_data);
                    }
                }
            }
            size += stream.WriteUnsignedInt(1, this.amp_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.sample_adaptive_offset_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.pcm_enabled_flag);

            if (pcm_enabled_flag != 0)
            {
                size += stream.WriteUnsignedInt(4, this.pcm_sample_bit_depth_luma_minus1);
                size += stream.WriteUnsignedInt(4, this.pcm_sample_bit_depth_chroma_minus1);
                size += stream.WriteUnsignedIntGolomb(this.log2_min_pcm_luma_coding_block_size_minus3);
                size += stream.WriteUnsignedIntGolomb(this.log2_diff_max_min_pcm_luma_coding_block_size);
                size += stream.WriteUnsignedInt(1, this.pcm_loop_filter_disabled_flag);
            }
            size += stream.WriteUnsignedIntGolomb(this.num_short_term_ref_pic_sets);

            for (i = 0; i < num_short_term_ref_pic_sets; i++)
            {
                size += stream.WriteClass<StRefPicSet>(context, this.st_ref_pic_set[i]);
            }
            size += stream.WriteUnsignedInt(1, this.long_term_ref_pics_present_flag);

            if (long_term_ref_pics_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_long_term_ref_pics_sps);

                for (i = 0; i < num_long_term_ref_pics_sps; i++)
                {
                    size += stream.WriteUnsignedIntVariable(lt_ref_pic_poc_lsb_sps[i], this.lt_ref_pic_poc_lsb_sps[i]);
                    size += stream.WriteUnsignedInt(1, this.used_by_curr_pic_lt_sps_flag[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.sps_temporal_mvp_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.strong_intra_smoothing_enabled_flag);
            size += stream.WriteUnsignedInt(1, this.vui_parameters_present_flag);

            if (vui_parameters_present_flag != 0)
            {
                size += stream.WriteClass<VuiParameters>(context, this.vui_parameters);
            }
            size += stream.WriteUnsignedInt(1, this.sps_extension_present_flag);

            if (sps_extension_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.sps_range_extension_flag);
                size += stream.WriteUnsignedInt(1, this.sps_multilayer_extension_flag);
                size += stream.WriteUnsignedInt(1, this.sps_3d_extension_flag);
                size += stream.WriteUnsignedInt(1, this.sps_scc_extension_flag);
                size += stream.WriteUnsignedInt(5, this.sps_extension_4bits);
            }

            if (sps_range_extension_flag != 0)
            {
                size += stream.WriteClass<SpsRangeExtension>(context, this.sps_range_extension);
            }

            if (sps_multilayer_extension_flag != 0)
            {
                size += stream.WriteClass<SpsMultilayerExtension>(context, this.sps_multilayer_extension);
            }

            if (sps_3d_extension_flag != 0)
            {
                size += stream.WriteClass<Sps3dExtension>(context, this.sps_3d_extension); // specified in Annex I 
            }

            if (sps_scc_extension_flag != 0)
            {
                size += stream.WriteClass<SpsSccExtension>(context, this.sps_scc_extension);
            }

            if (sps_extension_4bits != 0)
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
  

sps_multilayer_extension() { 
inter_view_mv_vert_constraint_flag  u(1) 
}
    */
    public class SpsMultilayerExtension : IItuSerializable
    {
        private byte inter_view_mv_vert_constraint_flag;
        public byte InterViewMvVertConstraintFlag { get { return inter_view_mv_vert_constraint_flag; } set { inter_view_mv_vert_constraint_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SpsMultilayerExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.inter_view_mv_vert_constraint_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.inter_view_mv_vert_constraint_flag);

            return size;
        }

    }

    /*


pps_multilayer_extension() {  
 poc_reset_info_present_flag u(1) 
 pps_infer_scaling_list_flag u(1) 
 if( pps_infer_scaling_list_flag )  
  pps_scaling_list_ref_layer_id u(6) 
 num_ref_loc_offsets ue(v) 
 for( i = 0; i < num_ref_loc_offsets; i++ ) {   
  ref_loc_offset_layer_id[ i ] u(6) 
  scaled_ref_layer_offset_present_flag[ i ] u(1) 
  if( scaled_ref_layer_offset_present_flag[ i ] ) {  
   scaled_ref_layer_left_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   scaled_ref_layer_top_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   scaled_ref_layer_right_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   scaled_ref_layer_bottom_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
  }  
  ref_region_offset_present_flag[ i ] u(1) 
  if( ref_region_offset_present_flag[ i ] ) {  
   ref_region_left_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   ref_region_top_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   ref_region_right_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
   ref_region_bottom_offset[ ref_loc_offset_layer_id[ i ] ] se(v) 
  }  
  resample_phase_set_present_flag[ i ] u(1) 
  if( resample_phase_set_present_flag[ i ] ) {  
   phase_hor_luma[ ref_loc_offset_layer_id[ i ] ] ue(v) 
   phase_ver_luma[ ref_loc_offset_layer_id[ i ] ] ue(v) 
   phase_hor_chroma_plus8[ ref_loc_offset_layer_id[ i ] ] ue(v) 
   phase_ver_chroma_plus8[ ref_loc_offset_layer_id[ i ] ] ue(v) 
  }  
 }  
 colour_mapping_enabled_flag u(1) 
 if( colour_mapping_enabled_flag )  
  colour_mapping_table()  
}
    */
    public class PpsMultilayerExtension : IItuSerializable
    {
        private byte poc_reset_info_present_flag;
        public byte PocResetInfoPresentFlag { get { return poc_reset_info_present_flag; } set { poc_reset_info_present_flag = value; } }
        private byte pps_infer_scaling_list_flag;
        public byte PpsInferScalingListFlag { get { return pps_infer_scaling_list_flag; } set { pps_infer_scaling_list_flag = value; } }
        private uint pps_scaling_list_ref_layer_id;
        public uint PpsScalingListRefLayerId { get { return pps_scaling_list_ref_layer_id; } set { pps_scaling_list_ref_layer_id = value; } }
        private uint num_ref_loc_offsets;
        public uint NumRefLocOffsets { get { return num_ref_loc_offsets; } set { num_ref_loc_offsets = value; } }
        private uint[] ref_loc_offset_layer_id;
        public uint[] RefLocOffsetLayerId { get { return ref_loc_offset_layer_id; } set { ref_loc_offset_layer_id = value; } }
        private byte[] scaled_ref_layer_offset_present_flag;
        public byte[] ScaledRefLayerOffsetPresentFlag { get { return scaled_ref_layer_offset_present_flag; } set { scaled_ref_layer_offset_present_flag = value; } }
        private int[] scaled_ref_layer_left_offset;
        public int[] ScaledRefLayerLeftOffset { get { return scaled_ref_layer_left_offset; } set { scaled_ref_layer_left_offset = value; } }
        private int[] scaled_ref_layer_top_offset;
        public int[] ScaledRefLayerTopOffset { get { return scaled_ref_layer_top_offset; } set { scaled_ref_layer_top_offset = value; } }
        private int[] scaled_ref_layer_right_offset;
        public int[] ScaledRefLayerRightOffset { get { return scaled_ref_layer_right_offset; } set { scaled_ref_layer_right_offset = value; } }
        private int[] scaled_ref_layer_bottom_offset;
        public int[] ScaledRefLayerBottomOffset { get { return scaled_ref_layer_bottom_offset; } set { scaled_ref_layer_bottom_offset = value; } }
        private byte[] ref_region_offset_present_flag;
        public byte[] RefRegionOffsetPresentFlag { get { return ref_region_offset_present_flag; } set { ref_region_offset_present_flag = value; } }
        private int[] ref_region_left_offset;
        public int[] RefRegionLeftOffset { get { return ref_region_left_offset; } set { ref_region_left_offset = value; } }
        private int[] ref_region_top_offset;
        public int[] RefRegionTopOffset { get { return ref_region_top_offset; } set { ref_region_top_offset = value; } }
        private int[] ref_region_right_offset;
        public int[] RefRegionRightOffset { get { return ref_region_right_offset; } set { ref_region_right_offset = value; } }
        private int[] ref_region_bottom_offset;
        public int[] RefRegionBottomOffset { get { return ref_region_bottom_offset; } set { ref_region_bottom_offset = value; } }
        private byte[] resample_phase_set_present_flag;
        public byte[] ResamplePhaseSetPresentFlag { get { return resample_phase_set_present_flag; } set { resample_phase_set_present_flag = value; } }
        private uint[] phase_hor_luma;
        public uint[] PhaseHorLuma { get { return phase_hor_luma; } set { phase_hor_luma = value; } }
        private uint[] phase_ver_luma;
        public uint[] PhaseVerLuma { get { return phase_ver_luma; } set { phase_ver_luma = value; } }
        private uint[] phase_hor_chroma_plus8;
        public uint[] PhaseHorChromaPlus8 { get { return phase_hor_chroma_plus8; } set { phase_hor_chroma_plus8 = value; } }
        private uint[] phase_ver_chroma_plus8;
        public uint[] PhaseVerChromaPlus8 { get { return phase_ver_chroma_plus8; } set { phase_ver_chroma_plus8 = value; } }
        private byte colour_mapping_enabled_flag;
        public byte ColourMappingEnabledFlag { get { return colour_mapping_enabled_flag; } set { colour_mapping_enabled_flag = value; } }
        private ColourMappingTable colour_mapping_table;
        public ColourMappingTable ColourMappingTable { get { return colour_mapping_table; } set { colour_mapping_table = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public PpsMultilayerExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.poc_reset_info_present_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.pps_infer_scaling_list_flag);

            if (pps_infer_scaling_list_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.pps_scaling_list_ref_layer_id);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_loc_offsets);

            this.ref_loc_offset_layer_id = new uint[num_ref_loc_offsets];
            this.scaled_ref_layer_offset_present_flag = new byte[num_ref_loc_offsets];
            this.scaled_ref_layer_left_offset = new int[num_ref_loc_offsets];
            this.scaled_ref_layer_top_offset = new int[num_ref_loc_offsets];
            this.scaled_ref_layer_right_offset = new int[num_ref_loc_offsets];
            this.scaled_ref_layer_bottom_offset = new int[num_ref_loc_offsets];
            this.ref_region_offset_present_flag = new byte[num_ref_loc_offsets];
            this.ref_region_left_offset = new int[num_ref_loc_offsets];
            this.ref_region_top_offset = new int[num_ref_loc_offsets];
            this.ref_region_right_offset = new int[num_ref_loc_offsets];
            this.ref_region_bottom_offset = new int[num_ref_loc_offsets];
            this.resample_phase_set_present_flag = new byte[num_ref_loc_offsets];
            this.phase_hor_luma = new uint[num_ref_loc_offsets];
            this.phase_ver_luma = new uint[num_ref_loc_offsets];
            this.phase_hor_chroma_plus8 = new uint[num_ref_loc_offsets];
            this.phase_ver_chroma_plus8 = new uint[num_ref_loc_offsets];
            for (i = 0; i < num_ref_loc_offsets; i++)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.ref_loc_offset_layer_id[i]);
                size += stream.ReadUnsignedInt(size, 1, out this.scaled_ref_layer_offset_present_flag[i]);

                if (scaled_ref_layer_offset_present_flag[i] != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.scaled_ref_layer_left_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.scaled_ref_layer_top_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.scaled_ref_layer_right_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.scaled_ref_layer_bottom_offset[ref_loc_offset_layer_id[i]]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.ref_region_offset_present_flag[i]);

                if (ref_region_offset_present_flag[i] != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.ref_region_left_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.ref_region_top_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.ref_region_right_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadSignedIntGolomb(size, out this.ref_region_bottom_offset[ref_loc_offset_layer_id[i]]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.resample_phase_set_present_flag[i]);

                if (resample_phase_set_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.phase_hor_luma[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.phase_ver_luma[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.phase_hor_chroma_plus8[ref_loc_offset_layer_id[i]]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.phase_ver_chroma_plus8[ref_loc_offset_layer_id[i]]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.colour_mapping_enabled_flag);

            if (colour_mapping_enabled_flag != 0)
            {
                this.colour_mapping_table = new ColourMappingTable();
                size += stream.ReadClass<ColourMappingTable>(size, context, this.colour_mapping_table);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(1, this.poc_reset_info_present_flag);
            size += stream.WriteUnsignedInt(1, this.pps_infer_scaling_list_flag);

            if (pps_infer_scaling_list_flag != 0)
            {
                size += stream.WriteUnsignedInt(6, this.pps_scaling_list_ref_layer_id);
            }
            size += stream.WriteUnsignedIntGolomb(this.num_ref_loc_offsets);

            for (i = 0; i < num_ref_loc_offsets; i++)
            {
                size += stream.WriteUnsignedInt(6, this.ref_loc_offset_layer_id[i]);
                size += stream.WriteUnsignedInt(1, this.scaled_ref_layer_offset_present_flag[i]);

                if (scaled_ref_layer_offset_present_flag[i] != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.scaled_ref_layer_left_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.scaled_ref_layer_top_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.scaled_ref_layer_right_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.scaled_ref_layer_bottom_offset[ref_loc_offset_layer_id[i]]);
                }
                size += stream.WriteUnsignedInt(1, this.ref_region_offset_present_flag[i]);

                if (ref_region_offset_present_flag[i] != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.ref_region_left_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.ref_region_top_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.ref_region_right_offset[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteSignedIntGolomb(this.ref_region_bottom_offset[ref_loc_offset_layer_id[i]]);
                }
                size += stream.WriteUnsignedInt(1, this.resample_phase_set_present_flag[i]);

                if (resample_phase_set_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.phase_hor_luma[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteUnsignedIntGolomb(this.phase_ver_luma[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteUnsignedIntGolomb(this.phase_hor_chroma_plus8[ref_loc_offset_layer_id[i]]);
                    size += stream.WriteUnsignedIntGolomb(this.phase_ver_chroma_plus8[ref_loc_offset_layer_id[i]]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.colour_mapping_enabled_flag);

            if (colour_mapping_enabled_flag != 0)
            {
                size += stream.WriteClass<ColourMappingTable>(context, this.colour_mapping_table);
            }

            return size;
        }

    }

    /*
 

colour_mapping_table() {  
 num_cm_ref_layers_minus1 ue(v) 
 for( i = 0; i <= num_cm_ref_layers_minus1; i++ )  
  cm_ref_layer_id[ i ] u(6) 
 cm_octant_depth u(2) 
 cm_y_part_num_log2 u(2) 
 luma_bit_depth_cm_input_minus8 ue(v) 
 chroma_bit_depth_cm_input_minus8 ue(v) 
 luma_bit_depth_cm_output_minus8 ue(v) 
 chroma_bit_depth_cm_output_minus8 ue(v) 
 cm_res_quant_bits u(2) 
 cm_delta_flc_bits_minus1 u(2) 
 if( cm_octant_depth == 1 ) {  
  cm_adapt_threshold_u_delta se(v) 
  cm_adapt_threshold_v_delta se(v) 
 }  
 colour_mapping_octants( 0, 0, 0, 0, 1  <<  cm_octant_depth )  
}
    */
    public class ColourMappingTable : IItuSerializable
    {
        private uint num_cm_ref_layers_minus1;
        public uint NumCmRefLayersMinus1 { get { return num_cm_ref_layers_minus1; } set { num_cm_ref_layers_minus1 = value; } }
        private uint[] cm_ref_layer_id;
        public uint[] CmRefLayerId { get { return cm_ref_layer_id; } set { cm_ref_layer_id = value; } }
        private uint cm_octant_depth;
        public uint CmOctantDepth { get { return cm_octant_depth; } set { cm_octant_depth = value; } }
        private uint cm_y_part_num_log2;
        public uint CmyPartNumLog2 { get { return cm_y_part_num_log2; } set { cm_y_part_num_log2 = value; } }
        private uint luma_bit_depth_cm_input_minus8;
        public uint LumaBitDepthCmInputMinus8 { get { return luma_bit_depth_cm_input_minus8; } set { luma_bit_depth_cm_input_minus8 = value; } }
        private uint chroma_bit_depth_cm_input_minus8;
        public uint ChromaBitDepthCmInputMinus8 { get { return chroma_bit_depth_cm_input_minus8; } set { chroma_bit_depth_cm_input_minus8 = value; } }
        private uint luma_bit_depth_cm_output_minus8;
        public uint LumaBitDepthCmOutputMinus8 { get { return luma_bit_depth_cm_output_minus8; } set { luma_bit_depth_cm_output_minus8 = value; } }
        private uint chroma_bit_depth_cm_output_minus8;
        public uint ChromaBitDepthCmOutputMinus8 { get { return chroma_bit_depth_cm_output_minus8; } set { chroma_bit_depth_cm_output_minus8 = value; } }
        private uint cm_res_quant_bits;
        public uint CmResQuantBits { get { return cm_res_quant_bits; } set { cm_res_quant_bits = value; } }
        private uint cm_delta_flc_bits_minus1;
        public uint CmDeltaFlcBitsMinus1 { get { return cm_delta_flc_bits_minus1; } set { cm_delta_flc_bits_minus1 = value; } }
        private int cm_adapt_threshold_u_delta;
        public int CmAdaptThresholduDelta { get { return cm_adapt_threshold_u_delta; } set { cm_adapt_threshold_u_delta = value; } }
        private int cm_adapt_threshold_v_delta;
        public int CmAdaptThresholdvDelta { get { return cm_adapt_threshold_v_delta; } set { cm_adapt_threshold_v_delta = value; } }
        private ColourMappingOctants colour_mapping_octants;
        public ColourMappingOctants ColourMappingOctants { get { return colour_mapping_octants; } set { colour_mapping_octants = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ColourMappingTable()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.num_cm_ref_layers_minus1);

            this.cm_ref_layer_id = new uint[num_cm_ref_layers_minus1 + 1];
            for (i = 0; i <= num_cm_ref_layers_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.cm_ref_layer_id[i]);
            }
            size += stream.ReadUnsignedInt(size, 2, out this.cm_octant_depth);
            size += stream.ReadUnsignedInt(size, 2, out this.cm_y_part_num_log2);
            size += stream.ReadUnsignedIntGolomb(size, out this.luma_bit_depth_cm_input_minus8);
            size += stream.ReadUnsignedIntGolomb(size, out this.chroma_bit_depth_cm_input_minus8);
            size += stream.ReadUnsignedIntGolomb(size, out this.luma_bit_depth_cm_output_minus8);
            size += stream.ReadUnsignedIntGolomb(size, out this.chroma_bit_depth_cm_output_minus8);
            size += stream.ReadUnsignedInt(size, 2, out this.cm_res_quant_bits);
            size += stream.ReadUnsignedInt(size, 2, out this.cm_delta_flc_bits_minus1);

            if (cm_octant_depth == 1)
            {
                size += stream.ReadSignedIntGolomb(size, out this.cm_adapt_threshold_u_delta);
                size += stream.ReadSignedIntGolomb(size, out this.cm_adapt_threshold_v_delta);
            }
            this.colour_mapping_octants = new ColourMappingOctants(0, 0, 0, 0, 1 << cm_octant_depth);
            size += stream.ReadClass<ColourMappingOctants>(size, context, this.colour_mapping_octants);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.num_cm_ref_layers_minus1);

            for (i = 0; i <= num_cm_ref_layers_minus1; i++)
            {
                size += stream.WriteUnsignedInt(6, this.cm_ref_layer_id[i]);
            }
            size += stream.WriteUnsignedInt(2, this.cm_octant_depth);
            size += stream.WriteUnsignedInt(2, this.cm_y_part_num_log2);
            size += stream.WriteUnsignedIntGolomb(this.luma_bit_depth_cm_input_minus8);
            size += stream.WriteUnsignedIntGolomb(this.chroma_bit_depth_cm_input_minus8);
            size += stream.WriteUnsignedIntGolomb(this.luma_bit_depth_cm_output_minus8);
            size += stream.WriteUnsignedIntGolomb(this.chroma_bit_depth_cm_output_minus8);
            size += stream.WriteUnsignedInt(2, this.cm_res_quant_bits);
            size += stream.WriteUnsignedInt(2, this.cm_delta_flc_bits_minus1);

            if (cm_octant_depth == 1)
            {
                size += stream.WriteSignedIntGolomb(this.cm_adapt_threshold_u_delta);
                size += stream.WriteSignedIntGolomb(this.cm_adapt_threshold_v_delta);
            }
            size += stream.WriteClass<ColourMappingOctants>(context, this.colour_mapping_octants);

            return size;
        }

    }

    /*
  

colour_mapping_octants( inpDepth, idxY, idxCb, idxCr, inpLength ) {   
 if( inpDepth < cm_octant_depth )  
  split_octant_flag u(1) 
 if( split_octant_flag )  
  for( k = 0; k < 2; k++ )  
   for( m = 0; m < 2; m++ )  
    for( n = 0; n < 2; n++ )  
     colour_mapping_octants( inpDepth + 1, idxY + PartNumY * k * inpLength / 2, 
      idxCb + m * inpLength / 2, idxCr + n * inpLength / 2, inpLength / 2 ) 
 
 else  
  for( i = 0; i < PartNumY; i++ ) {  
   idxShiftY = idxY + ( i << ( cm_octant_depth - inpDepth ) )  
   for( j = 0; j < 4; j++ ) {  
    coded_res_flag[ idxShiftY ][ idxCb ][ idxCr ][ j ] u(1) 
    if( coded_res_flag[ idxShiftY ][ idxCb ][ idxCr ][ j ] )  
     for( c = 0; c < 3; c++ ) {  
      res_coeff_q[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] ue(v) 
      res_coeff_r[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] u(v) 
      if( res_coeff_q[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ]  ||  
        res_coeff_r[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] ) 
 
       res_coeff_s[ idxShiftY ][ idxCb ][ idxCr ][ j ][ c ] u(1) 
     }  
   }  
  }  
}
    */
    public class ColourMappingOctants : IItuSerializable
    {
        private uint inpDepth;
        public uint InpDepth { get { return inpDepth; } set { inpDepth = value; } }
        private uint idxY;
        public uint IdxY { get { return idxY; } set { idxY = value; } }
        private uint idxCb;
        public uint IdxCb { get { return idxCb; } set { idxCb = value; } }
        private uint idxCr;
        public uint IdxCr { get { return idxCr; } set { idxCr = value; } }
        private uint inpLength;
        public uint InpLength { get { return inpLength; } set { inpLength = value; } }
        private byte split_octant_flag;
        public byte SplitOctantFlag { get { return split_octant_flag; } set { split_octant_flag = value; } }
        private ColourMappingOctants[][][] colour_mapping_octants;
        public ColourMappingOctants[][][] _ColourMappingOctants { get { return colour_mapping_octants; } set { colour_mapping_octants = value; } }
        private byte[][][][][] coded_res_flag;
        public byte[][][][][] CodedResFlag { get { return coded_res_flag; } set { coded_res_flag = value; } }
        private uint[][][][][][] res_coeff_q;
        public uint[][][][][][] ResCoeffq { get { return res_coeff_q; } set { res_coeff_q = value; } }
        private uint[][][][][][] res_coeff_r;
        public uint[][][][][][] ResCoeffr { get { return res_coeff_r; } set { res_coeff_r = value; } }
        private byte[][][][][][] res_coeff_s;
        public byte[][][][][][] ResCoeffs { get { return res_coeff_s; } set { res_coeff_s = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ColourMappingOctants(uint inpDepth, uint idxY, uint idxCb, uint idxCr, uint inpLength)
        {
            this.inpDepth = inpDepth;
            this.idxY = idxY;
            this.idxCb = idxCb;
            this.idxCr = idxCr;
            this.inpLength = inpLength;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;
            uint m = 0;
            uint n = 0;
            uint i = 0;
            uint idxShiftY = 0;
            uint j = 0;
            uint c = 0;

            if (inpDepth < cm_octant_depth)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.split_octant_flag);
            }

            if (split_octant_flag != 0)
            {

                this.colour_mapping_octants = new ColourMappingOctants[2][][];
                for (k = 0; k < 2; k++)
                {

                    this.colour_mapping_octants[k] = new ColourMappingOctants[2][];
                    for (m = 0; m < 2; m++)
                    {

                        this.colour_mapping_octants[k][m] = new ColourMappingOctants[2];
                        for (n = 0; n < 2; n++)
                        {
                            this.colour_mapping_octants[k][m][n] = new ColourMappingOctants(inpDepth + 1, idxY + PartNumY * k * inpLength / 2,
      idxCb + m * inpLength / 2, idxCr + n * inpLength / 2, inpLength / 2);
                            size += stream.ReadClass<ColourMappingOctants>(size, context, this.colour_mapping_octants[k][m][n]);
                        }
                    }
                }
            }
            else
            {

                this.coded_res_flag = new byte[PartNumY][];
                this.res_coeff_q = new uint[PartNumY][][];
                this.res_coeff_r = new uint[PartNumY][][];
                this.res_coeff_s = new byte[PartNumY][][];
                for (i = 0; i < PartNumY; i++)
                {
                    idxShiftY = idxY + (i << (int)(cm_octant_depth - inpDepth));

                    this.coded_res_flag[i] = new byte[4];
                    this.res_coeff_q[i] = new uint[4][];
                    this.res_coeff_r[i] = new uint[4][];
                    this.res_coeff_s[i] = new byte[4][];
                    for (j = 0; j < 4; j++)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.coded_res_flag[idxShiftY][idxCb][idxCr][j][i]);

                        if (coded_res_flag[idxShiftY][idxCb][idxCr][j] != 0)
                        {

                            this.res_coeff_q[i][j] = new uint[3];
                            this.res_coeff_r[i][j] = new uint[3];
                            this.res_coeff_s[i][j] = new byte[3];
                            for (c = 0; c < 3; c++)
                            {
                                size += stream.ReadUnsignedIntGolomb(size, out this.res_coeff_q[idxShiftY][idxCb][idxCr][j][c][i]);
                                size += stream.ReadUnsignedIntVariable(size, res_coeff_r, out this.res_coeff_r[idxShiftY][idxCb][idxCr][j][c][i]);

                                if (res_coeff_q[idxShiftY][idxCb][idxCr][j][c] != 0 ||
        res_coeff_r[idxShiftY][idxCb][idxCr][j][c] != 0)
                                {
                                    size += stream.ReadUnsignedInt(size, 1, out this.res_coeff_s[idxShiftY][idxCb][idxCr][j][c][i]);
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

            uint k = 0;
            uint m = 0;
            uint n = 0;
            uint i = 0;
            uint idxShiftY = 0;
            uint j = 0;
            uint c = 0;

            if (inpDepth < cm_octant_depth)
            {
                size += stream.WriteUnsignedInt(1, this.split_octant_flag);
            }

            if (split_octant_flag != 0)
            {

                for (k = 0; k < 2; k++)
                {

                    for (m = 0; m < 2; m++)
                    {

                        for (n = 0; n < 2; n++)
                        {
                            size += stream.WriteClass<ColourMappingOctants>(context, this.colour_mapping_octants[k][m][n]);
                        }
                    }
                }
            }
            else
            {

                for (i = 0; i < PartNumY; i++)
                {
                    idxShiftY = idxY + (i << (int)(cm_octant_depth - inpDepth));

                    for (j = 0; j < 4; j++)
                    {
                        size += stream.WriteUnsignedInt(1, this.coded_res_flag[idxShiftY][idxCb][idxCr][j][i]);

                        if (coded_res_flag[idxShiftY][idxCb][idxCr][j] != 0)
                        {

                            for (c = 0; c < 3; c++)
                            {
                                size += stream.WriteUnsignedIntGolomb(this.res_coeff_q[idxShiftY][idxCb][idxCr][j][c][i]);
                                size += stream.WriteUnsignedIntVariable(res_coeff_r[i][j][c], this.res_coeff_r[idxShiftY][idxCb][idxCr][j][c][i]);

                                if (res_coeff_q[idxShiftY][idxCb][idxCr][j][c] != 0 ||
        res_coeff_r[idxShiftY][idxCb][idxCr][j][c] != 0)
                                {
                                    size += stream.WriteUnsignedInt(1, this.res_coeff_s[idxShiftY][idxCb][idxCr][j][c][i]);
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
  

layers_not_present( payloadSize ) {  
 lnp_sei_active_vps_id u(4) 
 for( i = 0; i <= MaxLayersMinus1; i++ )   
  layer_not_present_flag[ i ] u(1) 
}
    */
    public class LayersNotPresent : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint lnp_sei_active_vps_id;
        public uint LnpSeiActiveVpsId { get { return lnp_sei_active_vps_id; } set { lnp_sei_active_vps_id = value; } }
        private byte[] layer_not_present_flag;
        public byte[] LayerNotPresentFlag { get { return layer_not_present_flag; } set { layer_not_present_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public LayersNotPresent(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 4, out this.lnp_sei_active_vps_id);

            this.layer_not_present_flag = new byte[MaxLayersMinus1 + 1];
            for (i = 0; i <= MaxLayersMinus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.layer_not_present_flag[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(4, this.lnp_sei_active_vps_id);

            for (i = 0; i <= MaxLayersMinus1; i++)
            {
                size += stream.WriteUnsignedInt(1, this.layer_not_present_flag[i]);
            }

            return size;
        }

    }

    /*
   

inter_layer_constrained_tile_sets( payloadSize ) {  
 il_all_tiles_exact_sample_value_match_flag u(1) 
 il_one_tile_per_tile_set_flag u(1) 
 if( !il_one_tile_per_tile_set_flag ) {  
  il_num_sets_in_message_minus1 ue(v) 
  if( il_num_sets_in_message_minus1 )  
   skipped_tile_set_present_flag u(1) 
  numSignificantSets = il_num_sets_in_message_minus1 - skipped_tile_set_present_flag + 1 
  for( i = 0; i < numSignificantSets; i++ ) {  
   ilcts_id[ i ] ue(v) 
   il_num_tile_rects_in_set_minus1[ i ] ue(v) 
   for( j = 0; j <= il_num_tile_rects_in_set_minus1[ i ]; j++ ) {  
    il_top_left_tile_idx[ i ][ j ] ue(v) 
    il_bottom_right_tile_idx[ i ][ j ] ue(v) 
   }  
   ilc_idc[ i ] u(2) 
   if ( !il_all_tiles_exact_sample_value_match_flag )  
    il_exact_sample_value_match_flag[ i ] u(1) 
  }  
 } else  
  all_tiles_ilc_idc u(2) 
}
    */
    public class InterLayerConstrainedTileSets : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte il_all_tiles_exact_sample_value_match_flag;
        public byte IlAllTilesExactSampleValueMatchFlag { get { return il_all_tiles_exact_sample_value_match_flag; } set { il_all_tiles_exact_sample_value_match_flag = value; } }
        private byte il_one_tile_per_tile_set_flag;
        public byte IlOneTilePerTileSetFlag { get { return il_one_tile_per_tile_set_flag; } set { il_one_tile_per_tile_set_flag = value; } }
        private uint il_num_sets_in_message_minus1;
        public uint IlNumSetsInMessageMinus1 { get { return il_num_sets_in_message_minus1; } set { il_num_sets_in_message_minus1 = value; } }
        private byte skipped_tile_set_present_flag;
        public byte SkippedTileSetPresentFlag { get { return skipped_tile_set_present_flag; } set { skipped_tile_set_present_flag = value; } }
        private uint[] ilcts_id;
        public uint[] IlctsId { get { return ilcts_id; } set { ilcts_id = value; } }
        private uint[] il_num_tile_rects_in_set_minus1;
        public uint[] IlNumTileRectsInSetMinus1 { get { return il_num_tile_rects_in_set_minus1; } set { il_num_tile_rects_in_set_minus1 = value; } }
        private uint[][] il_top_left_tile_idx;
        public uint[][] IlTopLeftTileIdx { get { return il_top_left_tile_idx; } set { il_top_left_tile_idx = value; } }
        private uint[][] il_bottom_right_tile_idx;
        public uint[][] IlBottomRightTileIdx { get { return il_bottom_right_tile_idx; } set { il_bottom_right_tile_idx = value; } }
        private uint[] ilc_idc;
        public uint[] IlcIdc { get { return ilc_idc; } set { ilc_idc = value; } }
        private byte[] il_exact_sample_value_match_flag;
        public byte[] IlExactSampleValueMatchFlag { get { return il_exact_sample_value_match_flag; } set { il_exact_sample_value_match_flag = value; } }
        private uint all_tiles_ilc_idc;
        public uint AllTilesIlcIdc { get { return all_tiles_ilc_idc; } set { all_tiles_ilc_idc = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public InterLayerConstrainedTileSets(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint numSignificantSets = 0;
            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.il_all_tiles_exact_sample_value_match_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.il_one_tile_per_tile_set_flag);

            if (il_one_tile_per_tile_set_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.il_num_sets_in_message_minus1);

                if (il_num_sets_in_message_minus1 != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.skipped_tile_set_present_flag);
                }
                numSignificantSets = il_num_sets_in_message_minus1 - skipped_tile_set_present_flag + 1;

                this.ilcts_id = new uint[numSignificantSets];
                this.il_num_tile_rects_in_set_minus1 = new uint[numSignificantSets];
                this.il_top_left_tile_idx = new uint[numSignificantSets][];
                this.il_bottom_right_tile_idx = new uint[numSignificantSets][];
                this.ilc_idc = new uint[numSignificantSets];
                this.il_exact_sample_value_match_flag = new byte[numSignificantSets];
                for (i = 0; i < numSignificantSets; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.ilcts_id[i]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.il_num_tile_rects_in_set_minus1[i]);

                    this.il_top_left_tile_idx[i] = new uint[il_num_tile_rects_in_set_minus1[i] + 1];
                    this.il_bottom_right_tile_idx[i] = new uint[il_num_tile_rects_in_set_minus1[i] + 1];
                    for (j = 0; j <= il_num_tile_rects_in_set_minus1[i]; j++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.il_top_left_tile_idx[i][j]);
                        size += stream.ReadUnsignedIntGolomb(size, out this.il_bottom_right_tile_idx[i][j]);
                    }
                    size += stream.ReadUnsignedInt(size, 2, out this.ilc_idc[i]);

                    if (il_all_tiles_exact_sample_value_match_flag == 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.il_exact_sample_value_match_flag[i]);
                    }
                }
            }
            else
            {
                size += stream.ReadUnsignedInt(size, 2, out this.all_tiles_ilc_idc);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint numSignificantSets = 0;
            uint i = 0;
            uint j = 0;
            size += stream.WriteUnsignedInt(1, this.il_all_tiles_exact_sample_value_match_flag);
            size += stream.WriteUnsignedInt(1, this.il_one_tile_per_tile_set_flag);

            if (il_one_tile_per_tile_set_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.il_num_sets_in_message_minus1);

                if (il_num_sets_in_message_minus1 != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.skipped_tile_set_present_flag);
                }
                numSignificantSets = il_num_sets_in_message_minus1 - skipped_tile_set_present_flag + 1;

                for (i = 0; i < numSignificantSets; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.ilcts_id[i]);
                    size += stream.WriteUnsignedIntGolomb(this.il_num_tile_rects_in_set_minus1[i]);

                    for (j = 0; j <= il_num_tile_rects_in_set_minus1[i]; j++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.il_top_left_tile_idx[i][j]);
                        size += stream.WriteUnsignedIntGolomb(this.il_bottom_right_tile_idx[i][j]);
                    }
                    size += stream.WriteUnsignedInt(2, this.ilc_idc[i]);

                    if (il_all_tiles_exact_sample_value_match_flag == 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.il_exact_sample_value_match_flag[i]);
                    }
                }
            }
            else
            {
                size += stream.WriteUnsignedInt(2, this.all_tiles_ilc_idc);
            }

            return size;
        }

    }

    /*
  

bsp_nesting( payloadSize ) {  
 sei_ols_idx ue(v) 
 sei_partitioning_scheme_idx ue(v) 
 bsp_idx ue(v) 
 num_seis_in_bsp_minus1 ue(v) 
 while( !byte_aligned() )  
  bsp_nesting_zero_bit /* equal to 0 *//* u(1) 
 for( i = 0; i <= num_seis_in_bsp_minus1; i++ )  
  sei_message()  
}
    */
    public class BspNesting : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sei_ols_idx;
        public uint SeiOlsIdx { get { return sei_ols_idx; } set { sei_ols_idx = value; } }
        private uint sei_partitioning_scheme_idx;
        public uint SeiPartitioningSchemeIdx { get { return sei_partitioning_scheme_idx; } set { sei_partitioning_scheme_idx = value; } }
        private uint bsp_idx;
        public uint BspIdx { get { return bsp_idx; } set { bsp_idx = value; } }
        private uint num_seis_in_bsp_minus1;
        public uint NumSeisInBspMinus1 { get { return num_seis_in_bsp_minus1; } set { num_seis_in_bsp_minus1 = value; } }
        private Dictionary<int, byte> bsp_nesting_zero_bit = new Dictionary<int, byte>();
        public Dictionary<int, byte> BspNestingZeroBit { get { return bsp_nesting_zero_bit; } set { bsp_nesting_zero_bit = value; } }
        private SeiMessage[] sei_message;
        public SeiMessage[] SeiMessage { get { return sei_message; } set { sei_message = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public BspNesting(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.sei_ols_idx);
            size += stream.ReadUnsignedIntGolomb(size, out this.sei_partitioning_scheme_idx);
            size += stream.ReadUnsignedIntGolomb(size, out this.bsp_idx);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_seis_in_bsp_minus1);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.ReadUnsignedInt(size, 1, whileIndex, this.bsp_nesting_zero_bit); // equal to 0 
            }

            this.sei_message = new SeiMessage[num_seis_in_bsp_minus1 + 1];
            for (i = 0; i <= num_seis_in_bsp_minus1; i++)
            {
                this.sei_message[i] = new SeiMessage();
                size += stream.ReadClass<SeiMessage>(size, context, this.sei_message[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            int whileIndex = -1;
            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.sei_ols_idx);
            size += stream.WriteUnsignedIntGolomb(this.sei_partitioning_scheme_idx);
            size += stream.WriteUnsignedIntGolomb(this.bsp_idx);
            size += stream.WriteUnsignedIntGolomb(this.num_seis_in_bsp_minus1);

            while (!stream.ByteAligned())
            {
                whileIndex++;

                size += stream.WriteUnsignedInt(1, whileIndex, this.bsp_nesting_zero_bit); // equal to 0 
            }

            for (i = 0; i <= num_seis_in_bsp_minus1; i++)
            {
                size += stream.WriteClass<SeiMessage>(context, this.sei_message[i]);
            }

            return size;
        }

    }

    /*
  

bsp_initial_arrival_time( payloadSize ) {  
 psIdx = sei_partitioning_scheme_idx  
 if( nalInitialArrivalDelayPresent )  
  for( i = 0; i < BspSchedCnt[ sei_ols_idx ][ psIdx ][ MaxTemporalId[ 0 ] ]; i++ )  
   nal_initial_arrival_delay[ i ] u(v) 
 if( vclInitialArrivalDelayPresent )  
  for( i = 0; i < BspSchedCnt[ sei_ols_idx ][ psIdx ][ MaxTemporalId[ 0 ] ]; i++ )  
   vcl_initial_arrival_delay[ i ] u(v) 
}
    */
    public class BspInitialArrivalTime : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint[] nal_initial_arrival_delay;
        public uint[] NalInitialArrivalDelay { get { return nal_initial_arrival_delay; } set { nal_initial_arrival_delay = value; } }
        private uint[] vcl_initial_arrival_delay;
        public uint[] VclInitialArrivalDelay { get { return vcl_initial_arrival_delay; } set { vcl_initial_arrival_delay = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public BspInitialArrivalTime(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint psIdx = 0;
            uint i = 0;
            psIdx = sei_partitioning_scheme_idx;

            if (nalInitialArrivalDelayPresent != 0)
            {

                this.nal_initial_arrival_delay = new uint[BspSchedCnt[sei_ols_idx][psIdx][MaxTemporalId[0]]];
                for (i = 0; i < BspSchedCnt[sei_ols_idx][psIdx][MaxTemporalId[0]]; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, nal_initial_arrival_delay, out this.nal_initial_arrival_delay[i]);
                }
            }

            if (vclInitialArrivalDelayPresent != 0)
            {

                this.vcl_initial_arrival_delay = new uint[BspSchedCnt[sei_ols_idx][psIdx][MaxTemporalId[0]]];
                for (i = 0; i < BspSchedCnt[sei_ols_idx][psIdx][MaxTemporalId[0]]; i++)
                {
                    size += stream.ReadUnsignedIntVariable(size, vcl_initial_arrival_delay, out this.vcl_initial_arrival_delay[i]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint psIdx = 0;
            uint i = 0;
            psIdx = sei_partitioning_scheme_idx;

            if (nalInitialArrivalDelayPresent != 0)
            {

                for (i = 0; i < BspSchedCnt[sei_ols_idx][psIdx][MaxTemporalId[0]]; i++)
                {
                    size += stream.WriteUnsignedIntVariable(nal_initial_arrival_delay[i], this.nal_initial_arrival_delay[i]);
                }
            }

            if (vclInitialArrivalDelayPresent != 0)
            {

                for (i = 0; i < BspSchedCnt[sei_ols_idx][psIdx][MaxTemporalId[0]]; i++)
                {
                    size += stream.WriteUnsignedIntVariable(vcl_initial_arrival_delay[i], this.vcl_initial_arrival_delay[i]);
                }
            }

            return size;
        }

    }

    /*
  

sub_bitstream_property( payloadSize ) {  
 sb_property_active_vps_id u(4) 
 num_additional_sub_streams_minus1 ue(v) 
 for( i = 0; i <= num_additional_sub_streams_minus1; i++ ) {  
  sub_bitstream_mode[ i ] u(2) 
  ols_idx_to_vps[ i ] ue(v) 
  highest_sublayer_id[ i ] u(3) 
  avg_sb_property_bit_rate[ i ] u(16) 
  max_sb_property_bit_rate[ i ] u(16) 
 }  
}
    */
    public class SubBitstreamProperty : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint sb_property_active_vps_id;
        public uint SbPropertyActiveVpsId { get { return sb_property_active_vps_id; } set { sb_property_active_vps_id = value; } }
        private uint num_additional_sub_streams_minus1;
        public uint NumAdditionalSubStreamsMinus1 { get { return num_additional_sub_streams_minus1; } set { num_additional_sub_streams_minus1 = value; } }
        private uint[] sub_bitstream_mode;
        public uint[] SubBitstreamMode { get { return sub_bitstream_mode; } set { sub_bitstream_mode = value; } }
        private uint[] ols_idx_to_vps;
        public uint[] OlsIdxToVps { get { return ols_idx_to_vps; } set { ols_idx_to_vps = value; } }
        private uint[] highest_sublayer_id;
        public uint[] HighestSublayerId { get { return highest_sublayer_id; } set { highest_sublayer_id = value; } }
        private uint[] avg_sb_property_bit_rate;
        public uint[] AvgSbPropertyBitRate { get { return avg_sb_property_bit_rate; } set { avg_sb_property_bit_rate = value; } }
        private uint[] max_sb_property_bit_rate;
        public uint[] MaxSbPropertyBitRate { get { return max_sb_property_bit_rate; } set { max_sb_property_bit_rate = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SubBitstreamProperty(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedInt(size, 4, out this.sb_property_active_vps_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.num_additional_sub_streams_minus1);

            this.sub_bitstream_mode = new uint[num_additional_sub_streams_minus1 + 1];
            this.ols_idx_to_vps = new uint[num_additional_sub_streams_minus1 + 1];
            this.highest_sublayer_id = new uint[num_additional_sub_streams_minus1 + 1];
            this.avg_sb_property_bit_rate = new uint[num_additional_sub_streams_minus1 + 1];
            this.max_sb_property_bit_rate = new uint[num_additional_sub_streams_minus1 + 1];
            for (i = 0; i <= num_additional_sub_streams_minus1; i++)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.sub_bitstream_mode[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.ols_idx_to_vps[i]);
                size += stream.ReadUnsignedInt(size, 3, out this.highest_sublayer_id[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.avg_sb_property_bit_rate[i]);
                size += stream.ReadUnsignedInt(size, 16, out this.max_sb_property_bit_rate[i]);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedInt(4, this.sb_property_active_vps_id);
            size += stream.WriteUnsignedIntGolomb(this.num_additional_sub_streams_minus1);

            for (i = 0; i <= num_additional_sub_streams_minus1; i++)
            {
                size += stream.WriteUnsignedInt(2, this.sub_bitstream_mode[i]);
                size += stream.WriteUnsignedIntGolomb(this.ols_idx_to_vps[i]);
                size += stream.WriteUnsignedInt(3, this.highest_sublayer_id[i]);
                size += stream.WriteUnsignedInt(16, this.avg_sb_property_bit_rate[i]);
                size += stream.WriteUnsignedInt(16, this.max_sb_property_bit_rate[i]);
            }

            return size;
        }

    }

    /*
 

alpha_channel_info( payloadSize ) { 
alpha_channel_cancel_flag u(1)  
if( !alpha_channel_cancel_flag ) {  

alpha_channel_use_idc u(3) 
alpha_channel_bit_depth_minus8 u(3) 
alpha_transparent_value u(v) 

alpha_opaque_value u(v)
alpha_channel_incr_flag u(1) 
alpha_channel_clip_flag u(1) 

if( alpha_channel_clip_flag )  
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
                size += stream.ReadUnsignedIntVariable(size, alpha_transparent_value, out this.alpha_transparent_value);
                size += stream.ReadUnsignedIntVariable(size, alpha_opaque_value, out this.alpha_opaque_value);
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
                size += stream.WriteUnsignedIntVariable(alpha_transparent_value, this.alpha_transparent_value);
                size += stream.WriteUnsignedIntVariable(alpha_opaque_value, this.alpha_opaque_value);
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
  

overlay_info( payloadSize ) {  
 overlay_info_cancel_flag u(1) 
 if( !overlay_info_cancel_flag ) {  
  overlay_content_aux_id_minus128 ue(v) 
  overlay_label_aux_id_minus128 ue(v) 
  overlay_alpha_aux_id_minus128 ue(v) 
  overlay_element_label_value_length_minus8 ue(v) 
  num_overlays_minus1 ue(v) 
  for( i = 0; i <= num_overlays_minus1; i++ ) {  
   overlay_idx[ i ] ue(v) 
   language_overlay_present_flag[ i ] u(1) 
   overlay_content_layer_id[ i ] u(6) 
   overlay_label_present_flag[ i ] u(1) 
   if( overlay_label_present_flag[ i ] )   
    overlay_label_layer_id[ i ] u(6) 
   overlay_alpha_present_flag[ i ] u(1) 
   if( overlay_alpha_present_flag[ i ] )  
    overlay_alpha_layer_id[ i ] u(6) 
   if( overlay_label_present_flag[ i ] ) {  
    num_overlay_elements_minus1[ i ] ue(v) 
    for( j = 0; j <= num_overlay_elements_minus1[ i ]; j++ ) {  
     overlay_element_label_min[ i ][ j ] u(v) 
     overlay_element_label_max[ i ][ j ] u(v) 
    }  
   }  
  }  
  while( !byte_aligned() )  
   overlay_zero_bit /* equal to 0 *//* f(1) 
  for( i = 0; i <= num_overlays_minus1; i++ ) {  
   if( language_overlay_present_flag[ i ] )  
    overlay_language[ i ] st(v) 
   overlay_name[ i ] st(v) 
   if( overlay_label_present_flag[ i ] )  
    for( j = 0; j <= num_overlay_elements_minus1[ i ]; j++ )  
     overlay_element_name[ i ][ j ] st(v) 
  }  
  overlay_info_persistence_flag u(1) 
 }  
}
    */
    public class OverlayInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte overlay_info_cancel_flag;
        public byte OverlayInfoCancelFlag { get { return overlay_info_cancel_flag; } set { overlay_info_cancel_flag = value; } }
        private uint overlay_content_aux_id_minus128;
        public uint OverlayContentAuxIdMinus128 { get { return overlay_content_aux_id_minus128; } set { overlay_content_aux_id_minus128 = value; } }
        private uint overlay_label_aux_id_minus128;
        public uint OverlayLabelAuxIdMinus128 { get { return overlay_label_aux_id_minus128; } set { overlay_label_aux_id_minus128 = value; } }
        private uint overlay_alpha_aux_id_minus128;
        public uint OverlayAlphaAuxIdMinus128 { get { return overlay_alpha_aux_id_minus128; } set { overlay_alpha_aux_id_minus128 = value; } }
        private uint overlay_element_label_value_length_minus8;
        public uint OverlayElementLabelValueLengthMinus8 { get { return overlay_element_label_value_length_minus8; } set { overlay_element_label_value_length_minus8 = value; } }
        private uint num_overlays_minus1;
        public uint NumOverlaysMinus1 { get { return num_overlays_minus1; } set { num_overlays_minus1 = value; } }
        private uint[] overlay_idx;
        public uint[] OverlayIdx { get { return overlay_idx; } set { overlay_idx = value; } }
        private byte[] language_overlay_present_flag;
        public byte[] LanguageOverlayPresentFlag { get { return language_overlay_present_flag; } set { language_overlay_present_flag = value; } }
        private uint[] overlay_content_layer_id;
        public uint[] OverlayContentLayerId { get { return overlay_content_layer_id; } set { overlay_content_layer_id = value; } }
        private byte[] overlay_label_present_flag;
        public byte[] OverlayLabelPresentFlag { get { return overlay_label_present_flag; } set { overlay_label_present_flag = value; } }
        private uint[] overlay_label_layer_id;
        public uint[] OverlayLabelLayerId { get { return overlay_label_layer_id; } set { overlay_label_layer_id = value; } }
        private byte[] overlay_alpha_present_flag;
        public byte[] OverlayAlphaPresentFlag { get { return overlay_alpha_present_flag; } set { overlay_alpha_present_flag = value; } }
        private uint[] overlay_alpha_layer_id;
        public uint[] OverlayAlphaLayerId { get { return overlay_alpha_layer_id; } set { overlay_alpha_layer_id = value; } }
        private uint[] num_overlay_elements_minus1;
        public uint[] NumOverlayElementsMinus1 { get { return num_overlay_elements_minus1; } set { num_overlay_elements_minus1 = value; } }
        private uint[][] overlay_element_label_min;
        public uint[][] OverlayElementLabelMin { get { return overlay_element_label_min; } set { overlay_element_label_min = value; } }
        private uint[][] overlay_element_label_max;
        public uint[][] OverlayElementLabelMax { get { return overlay_element_label_max; } set { overlay_element_label_max = value; } }
        private Dictionary<int, uint> overlay_zero_bit = new Dictionary<int, uint>();
        public Dictionary<int, uint> OverlayZeroBit { get { return overlay_zero_bit; } set { overlay_zero_bit = value; } }
        private byte[][] overlay_language;
        public byte[][] OverlayLanguage { get { return overlay_language; } set { overlay_language = value; } }
        private byte[][] overlay_name;
        public byte[][] OverlayName { get { return overlay_name; } set { overlay_name = value; } }
        private byte[][][] overlay_element_name;
        public byte[][][] OverlayElementName { get { return overlay_element_name; } set { overlay_element_name = value; } }
        private byte overlay_info_persistence_flag;
        public byte OverlayInfoPersistenceFlag { get { return overlay_info_persistence_flag; } set { overlay_info_persistence_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public OverlayInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 1, out this.overlay_info_cancel_flag);

            if (overlay_info_cancel_flag == 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.overlay_content_aux_id_minus128);
                size += stream.ReadUnsignedIntGolomb(size, out this.overlay_label_aux_id_minus128);
                size += stream.ReadUnsignedIntGolomb(size, out this.overlay_alpha_aux_id_minus128);
                size += stream.ReadUnsignedIntGolomb(size, out this.overlay_element_label_value_length_minus8);
                size += stream.ReadUnsignedIntGolomb(size, out this.num_overlays_minus1);

                this.overlay_idx = new uint[num_overlays_minus1 + 1];
                this.language_overlay_present_flag = new byte[num_overlays_minus1 + 1];
                this.overlay_content_layer_id = new uint[num_overlays_minus1 + 1];
                this.overlay_label_present_flag = new byte[num_overlays_minus1 + 1];
                this.overlay_label_layer_id = new uint[num_overlays_minus1 + 1];
                this.overlay_alpha_present_flag = new byte[num_overlays_minus1 + 1];
                this.overlay_alpha_layer_id = new uint[num_overlays_minus1 + 1];
                this.num_overlay_elements_minus1 = new uint[num_overlays_minus1 + 1];
                this.overlay_element_label_min = new uint[num_overlays_minus1 + 1][];
                this.overlay_element_label_max = new uint[num_overlays_minus1 + 1][];
                for (i = 0; i <= num_overlays_minus1; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.overlay_idx[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.language_overlay_present_flag[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.overlay_content_layer_id[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.overlay_label_present_flag[i]);

                    if (overlay_label_present_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 6, out this.overlay_label_layer_id[i]);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.overlay_alpha_present_flag[i]);

                    if (overlay_alpha_present_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 6, out this.overlay_alpha_layer_id[i]);
                    }

                    if (overlay_label_present_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_overlay_elements_minus1[i]);

                        this.overlay_element_label_min[i] = new uint[num_overlay_elements_minus1[i] + 1];
                        this.overlay_element_label_max[i] = new uint[num_overlay_elements_minus1[i] + 1];
                        for (j = 0; j <= num_overlay_elements_minus1[i]; j++)
                        {
                            size += stream.ReadUnsignedIntVariable(size, overlay_element_label_min, out this.overlay_element_label_min[i][j]);
                            size += stream.ReadUnsignedIntVariable(size, overlay_element_label_max, out this.overlay_element_label_max[i][j]);
                        }
                    }
                }

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadFixed(size, 1, whileIndex, this.overlay_zero_bit); // equal to 0 
                }

                this.overlay_language = new byte[num_overlays_minus1 + 1][];
                this.overlay_name = new byte[num_overlays_minus1 + 1][];
                this.overlay_element_name = new byte[num_overlays_minus1 + 1][][];
                for (i = 0; i <= num_overlays_minus1; i++)
                {

                    if (language_overlay_present_flag[i] != 0)
                    {
                        size += stream.ReadUtf8String(size, out this.overlay_language[i]);
                    }
                    size += stream.ReadUtf8String(size, out this.overlay_name[i]);

                    if (overlay_label_present_flag[i] != 0)
                    {

                        this.overlay_element_name[i] = new byte[num_overlay_elements_minus1[i] + 1][];
                        for (j = 0; j <= num_overlay_elements_minus1[i]; j++)
                        {
                            size += stream.ReadUtf8String(size, out this.overlay_element_name[i][j]);
                        }
                    }
                }
                size += stream.ReadUnsignedInt(size, 1, out this.overlay_info_persistence_flag);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(1, this.overlay_info_cancel_flag);

            if (overlay_info_cancel_flag == 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.overlay_content_aux_id_minus128);
                size += stream.WriteUnsignedIntGolomb(this.overlay_label_aux_id_minus128);
                size += stream.WriteUnsignedIntGolomb(this.overlay_alpha_aux_id_minus128);
                size += stream.WriteUnsignedIntGolomb(this.overlay_element_label_value_length_minus8);
                size += stream.WriteUnsignedIntGolomb(this.num_overlays_minus1);

                for (i = 0; i <= num_overlays_minus1; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.overlay_idx[i]);
                    size += stream.WriteUnsignedInt(1, this.language_overlay_present_flag[i]);
                    size += stream.WriteUnsignedInt(6, this.overlay_content_layer_id[i]);
                    size += stream.WriteUnsignedInt(1, this.overlay_label_present_flag[i]);

                    if (overlay_label_present_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(6, this.overlay_label_layer_id[i]);
                    }
                    size += stream.WriteUnsignedInt(1, this.overlay_alpha_present_flag[i]);

                    if (overlay_alpha_present_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(6, this.overlay_alpha_layer_id[i]);
                    }

                    if (overlay_label_present_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_overlay_elements_minus1[i]);

                        for (j = 0; j <= num_overlay_elements_minus1[i]; j++)
                        {
                            size += stream.WriteUnsignedIntVariable(overlay_element_label_min[i][j], this.overlay_element_label_min[i][j]);
                            size += stream.WriteUnsignedIntVariable(overlay_element_label_max[i][j], this.overlay_element_label_max[i][j]);
                        }
                    }
                }

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteFixed(1, whileIndex, this.overlay_zero_bit); // equal to 0 
                }

                for (i = 0; i <= num_overlays_minus1; i++)
                {

                    if (language_overlay_present_flag[i] != 0)
                    {
                        size += stream.WriteUtf8String(this.overlay_language[i]);
                    }
                    size += stream.WriteUtf8String(this.overlay_name[i]);

                    if (overlay_label_present_flag[i] != 0)
                    {

                        for (j = 0; j <= num_overlay_elements_minus1[i]; j++)
                        {
                            size += stream.WriteUtf8String(this.overlay_element_name[i][j]);
                        }
                    }
                }
                size += stream.WriteUnsignedInt(1, this.overlay_info_persistence_flag);
            }

            return size;
        }

    }

    /*
  

temporal_mv_prediction_constraints( payloadSize ) { 
prev_pics_not_used_flag  u(1) 
no_intra_layer_col_pic_flag u(1) 
}
    */
    public class TemporalMvPredictionConstraints : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte prev_pics_not_used_flag;
        public byte PrevPicsNotUsedFlag { get { return prev_pics_not_used_flag; } set { prev_pics_not_used_flag = value; } }
        private byte no_intra_layer_col_pic_flag;
        public byte NoIntraLayerColPicFlag { get { return no_intra_layer_col_pic_flag; } set { no_intra_layer_col_pic_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public TemporalMvPredictionConstraints(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.prev_pics_not_used_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.no_intra_layer_col_pic_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.prev_pics_not_used_flag);
            size += stream.WriteUnsignedInt(1, this.no_intra_layer_col_pic_flag);

            return size;
        }

    }

    /*
  

frame_field_info( payloadSize ) { 
ffinfo_pic_struct u(4)
ffinfo_source_scan_type u(2) 
ffinfo_duplicate_flag u(1) 
}
    */
    public class FrameFieldInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint ffinfo_pic_struct;
        public uint FfinfoPicStruct { get { return ffinfo_pic_struct; } set { ffinfo_pic_struct = value; } }
        private uint ffinfo_source_scan_type;
        public uint FfinfoSourceScanType { get { return ffinfo_source_scan_type; } set { ffinfo_source_scan_type = value; } }
        private byte ffinfo_duplicate_flag;
        public byte FfinfoDuplicateFlag { get { return ffinfo_duplicate_flag; } set { ffinfo_duplicate_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public FrameFieldInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 4, out this.ffinfo_pic_struct);
            size += stream.ReadUnsignedInt(size, 2, out this.ffinfo_source_scan_type);
            size += stream.ReadUnsignedInt(size, 1, out this.ffinfo_duplicate_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(4, this.ffinfo_pic_struct);
            size += stream.WriteUnsignedInt(2, this.ffinfo_source_scan_type);
            size += stream.WriteUnsignedInt(1, this.ffinfo_duplicate_flag);

            return size;
        }

    }

    /*
  


three_dimensional_reference_displays_info( payloadSize ) { 
prec_ref_display_width  ue(v) 
ref_viewing_distance_flag u(1) 


if( ref_viewing_distance_flag )  
prec_ref_viewing_dist ue(v) 
num_ref_displays_minus1 ue(v) 

for( i = 0; i <= num_ref_displays_minus1; i++ ) {  
left_view_id[ i ] ue(v) 
right_view_id[ i ] ue(v) 
exponent_ref_display_width[ i ] u(6) 
mantissa_ref_display_width[ i ] u(v) 
if( ref_viewing_distance_flag ) {  
exponent_ref_viewing_distance[ i ] u(6) 
mantissa_ref_viewing_distance[ i ] u(v) 
}  
additional_shift_present_flag[ i ] u(1) 
if( additional_shift_present_flag[ i ] )  
num_sample_shift_plus512[ i ] u(10) 
}  
three_dimensional_reference_displays_extension_flag u(1) 
}
    */
    public class ThreeDimensionalReferenceDisplaysInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint prec_ref_display_width;
        public uint PrecRefDisplayWidth { get { return prec_ref_display_width; } set { prec_ref_display_width = value; } }
        private byte ref_viewing_distance_flag;
        public byte RefViewingDistanceFlag { get { return ref_viewing_distance_flag; } set { ref_viewing_distance_flag = value; } }
        private uint prec_ref_viewing_dist;
        public uint PrecRefViewingDist { get { return prec_ref_viewing_dist; } set { prec_ref_viewing_dist = value; } }
        private uint num_ref_displays_minus1;
        public uint NumRefDisplaysMinus1 { get { return num_ref_displays_minus1; } set { num_ref_displays_minus1 = value; } }
        private uint[] left_view_id;
        public uint[] LeftViewId { get { return left_view_id; } set { left_view_id = value; } }
        private uint[] right_view_id;
        public uint[] RightViewId { get { return right_view_id; } set { right_view_id = value; } }
        private uint[] exponent_ref_display_width;
        public uint[] ExponentRefDisplayWidth { get { return exponent_ref_display_width; } set { exponent_ref_display_width = value; } }
        private uint[] mantissa_ref_display_width;
        public uint[] MantissaRefDisplayWidth { get { return mantissa_ref_display_width; } set { mantissa_ref_display_width = value; } }
        private uint[] exponent_ref_viewing_distance;
        public uint[] ExponentRefViewingDistance { get { return exponent_ref_viewing_distance; } set { exponent_ref_viewing_distance = value; } }
        private uint[] mantissa_ref_viewing_distance;
        public uint[] MantissaRefViewingDistance { get { return mantissa_ref_viewing_distance; } set { mantissa_ref_viewing_distance = value; } }
        private byte[] additional_shift_present_flag;
        public byte[] AdditionalShiftPresentFlag { get { return additional_shift_present_flag; } set { additional_shift_present_flag = value; } }
        private uint[] num_sample_shift_plus512;
        public uint[] NumSampleShiftPlus512 { get { return num_sample_shift_plus512; } set { num_sample_shift_plus512 = value; } }
        private byte three_dimensional_reference_displays_extension_flag;
        public byte ThreeDimensionalReferenceDisplaysExtensionFlag { get { return three_dimensional_reference_displays_extension_flag; } set { three_dimensional_reference_displays_extension_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public ThreeDimensionalReferenceDisplaysInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.prec_ref_display_width);
            size += stream.ReadUnsignedInt(size, 1, out this.ref_viewing_distance_flag);

            if (ref_viewing_distance_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_ref_viewing_dist);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_displays_minus1);

            this.left_view_id = new uint[num_ref_displays_minus1 + 1];
            this.right_view_id = new uint[num_ref_displays_minus1 + 1];
            this.exponent_ref_display_width = new uint[num_ref_displays_minus1 + 1];
            this.mantissa_ref_display_width = new uint[num_ref_displays_minus1 + 1];
            this.exponent_ref_viewing_distance = new uint[num_ref_displays_minus1 + 1];
            this.mantissa_ref_viewing_distance = new uint[num_ref_displays_minus1 + 1];
            this.additional_shift_present_flag = new byte[num_ref_displays_minus1 + 1];
            this.num_sample_shift_plus512 = new uint[num_ref_displays_minus1 + 1];
            for (i = 0; i <= num_ref_displays_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.left_view_id[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.right_view_id[i]);
                size += stream.ReadUnsignedInt(size, 6, out this.exponent_ref_display_width[i]);
                size += stream.ReadUnsignedIntVariable(size, mantissa_ref_display_width, out this.mantissa_ref_display_width[i]);

                if (ref_viewing_distance_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_ref_viewing_distance[i]);
                    size += stream.ReadUnsignedIntVariable(size, mantissa_ref_viewing_distance, out this.mantissa_ref_viewing_distance[i]);
                }
                size += stream.ReadUnsignedInt(size, 1, out this.additional_shift_present_flag[i]);

                if (additional_shift_present_flag[i] != 0)
                {
                    size += stream.ReadUnsignedInt(size, 10, out this.num_sample_shift_plus512[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.three_dimensional_reference_displays_extension_flag);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            size += stream.WriteUnsignedIntGolomb(this.prec_ref_display_width);
            size += stream.WriteUnsignedInt(1, this.ref_viewing_distance_flag);

            if (ref_viewing_distance_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.prec_ref_viewing_dist);
            }
            size += stream.WriteUnsignedIntGolomb(this.num_ref_displays_minus1);

            for (i = 0; i <= num_ref_displays_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.left_view_id[i]);
                size += stream.WriteUnsignedIntGolomb(this.right_view_id[i]);
                size += stream.WriteUnsignedInt(6, this.exponent_ref_display_width[i]);
                size += stream.WriteUnsignedIntVariable(mantissa_ref_display_width[i], this.mantissa_ref_display_width[i]);

                if (ref_viewing_distance_flag != 0)
                {
                    size += stream.WriteUnsignedInt(6, this.exponent_ref_viewing_distance[i]);
                    size += stream.WriteUnsignedIntVariable(mantissa_ref_viewing_distance[i], this.mantissa_ref_viewing_distance[i]);
                }
                size += stream.WriteUnsignedInt(1, this.additional_shift_present_flag[i]);

                if (additional_shift_present_flag[i] != 0)
                {
                    size += stream.WriteUnsignedInt(10, this.num_sample_shift_plus512[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.three_dimensional_reference_displays_extension_flag);

            return size;
        }

    }

    /*
  

depth_representation_info( payloadSize ) {  
 z_near_flag u(1) 
 z_far_flag u(1) 
 d_min_flag u(1) 
 d_max_flag u(1) 
 depth_representation_type ue(v) 
 if( d_min_flag  ||  d_max_flag )   
  disparity_ref_view_id ue(v) 
 if( z_near_flag )  
  depth_rep_info_element( ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen )  
 if( z_far_flag )  
  depth_rep_info_element( ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen )  
 if( d_min_flag )  
  depth_rep_info_element( DMinSign, DMinExp, DMinMantissa, DMinManLen )  
 if( d_max_flag )  
  depth_rep_info_element( DMaxSign, DMaxExp, DMaxMantissa, DMaxManLen )  
 if( depth_representation_type == 3 ) {  
  depth_nonlinear_representation_num_minus1 ue(v) 
  for( i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++ )  
   depth_nonlinear_representation_model[ i ] u(8)
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
                this.depth_rep_info_element = new DepthRepInfoElement(ZNearSign, ZNearExp, ZNearMantissa, ZNearManLen);
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element);
            }

            if (z_far_flag != 0)
            {
                this.depth_rep_info_element0 = new DepthRepInfoElement(ZFarSign, ZFarExp, ZFarMantissa, ZFarManLen);
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element0);
            }

            if (d_min_flag != 0)
            {
                this.depth_rep_info_element1 = new DepthRepInfoElement(DMinSign, DMinExp, DMinMantissa, DMinManLen);
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element1);
            }

            if (d_max_flag != 0)
            {
                this.depth_rep_info_element2 = new DepthRepInfoElement(DMaxSign, DMaxExp, DMaxMantissa, DMaxManLen);
                size += stream.ReadClass<DepthRepInfoElement>(size, context, this.depth_rep_info_element2);
            }

            if (depth_representation_type == 3)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.depth_nonlinear_representation_num_minus1);

                this.depth_nonlinear_representation_model = new uint[depth_nonlinear_representation_num_minus1 + 1 + 1];
                for (i = 1; i <= depth_nonlinear_representation_num_minus1 + 1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 8, out this.depth_nonlinear_representation_model[i]);
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
                    size += stream.WriteUnsignedInt(8, this.depth_nonlinear_representation_model[i]);
                }
            }

            return size;
        }

    }

    /*
 

depth_rep_info_element( OutSign, OutExp, OutMantissa, OutManLen ) {  
 da_sign_flag u(1) 
 da_exponent u(7) 
 da_mantissa_len_minus1 u(5) 
 da_mantissa u(v) 
}
    */
    public class DepthRepInfoElement : IItuSerializable
    {
        private uint outSign;
        public uint OutSign { get { return outSign; } set { outSign = value; } }
        private uint outExp;
        public uint OutExp { get { return outExp; } set { outExp = value; } }
        private uint outMantissa;
        public uint OutMantissa { get { return outMantissa; } set { outMantissa = value; } }
        private uint outManLen;
        public uint OutManLen { get { return outManLen; } set { outManLen = value; } }
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

        public DepthRepInfoElement(uint OutSign, uint OutExp, uint OutMantissa, uint OutManLen)
        {
            this.outSign = OutSign;
            this.outExp = OutExp;
            this.outMantissa = OutMantissa;
            this.outManLen = OutManLen;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 1, out this.da_sign_flag);
            size += stream.ReadUnsignedInt(size, 7, out this.da_exponent);
            size += stream.ReadUnsignedInt(size, 5, out this.da_mantissa_len_minus1);
            size += stream.ReadUnsignedIntVariable(size, da_mantissa, out this.da_mantissa);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(1, this.da_sign_flag);
            size += stream.WriteUnsignedInt(7, this.da_exponent);
            size += stream.WriteUnsignedInt(5, this.da_mantissa_len_minus1);
            size += stream.WriteUnsignedIntVariable(da_mantissa, this.da_mantissa);

            return size;
        }

    }

    /*


multiview_scene_info( payloadSize ) {  
 min_disparity se(v) 
 max_disparity_range ue(v) 
}
    */
    public class MultiviewSceneInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private int min_disparity;
        public int MinDisparity { get { return min_disparity; } set { min_disparity = value; } }
        private uint max_disparity_range;
        public uint MaxDisparityRange { get { return max_disparity_range; } set { max_disparity_range = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public MultiviewSceneInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadSignedIntGolomb(size, out this.min_disparity);
            size += stream.ReadUnsignedIntGolomb(size, out this.max_disparity_range);

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteSignedIntGolomb(this.min_disparity);
            size += stream.WriteUnsignedIntGolomb(this.max_disparity_range);

            return size;
        }

    }

    /*


multiview_acquisition_info( payloadSize ) {  
 intrinsic_param_flag u(1) 
 extrinsic_param_flag u(1) 
 if( intrinsic_param_flag ) {  
  intrinsic_params_equal_flag u(1) 
  prec_focal_length ue(v) 
  prec_principal_point ue(v) 
  prec_skew_factor ue(v) 
  for( i = 0; i <= intrinsic_params_equal_flag != 0 ?  0 : numViewsMinus1; i++ ) {  
   sign_focal_length_x[ i ] u(1) 
   exponent_focal_length_x[ i ] u(6) 
   mantissa_focal_length_x[ i ] u(v) 
   sign_focal_length_y[ i ] u(1) 
   exponent_focal_length_y[ i ] u(6) 
   mantissa_focal_length_y[ i ] u(v) 
   sign_principal_point_x[ i ] u(1) 
   exponent_principal_point_x[ i ] u(6) 
   mantissa_principal_point_x[ i ] u(v) 
   sign_principal_point_y[ i ] u(1) 
   exponent_principal_point_y[ i ] u(6) 
   mantissa_principal_point_y[ i ] u(v) 
   sign_skew_factor[ i ] u(1) 
   exponent_skew_factor[ i ] u(6) 
   mantissa_skew_factor[ i ] u(v) 
  }  
 }  
 if( extrinsic_param_flag ) {  
  prec_rotation_param ue(v) 
  prec_translation_param ue(v) 
  for( i = 0; i <= numViewsMinus1; i++ )  
   for( j = 0; j < 3; j++ ) { /* row *//*  
    for( k = 0; k < 3; k++ ) { /* column *//*  
     sign_r[ i ][ j ][ k ] u(1) 
     exponent_r[ i ][ j ][ k ] u(6) 
     mantissa_r[ i ][ j ][ k ] u(v) 
    }  
    sign_t[ i ][ j ] u(1) 
    exponent_t[ i ][ j ] u(6) 
    mantissa_t[ i ][ j ] u(v) 
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

            if (intrinsic_param_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_params_equal_flag);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_focal_length);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_principal_point);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_skew_factor);

                this.sign_focal_length_x = new byte[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.exponent_focal_length_x = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.mantissa_focal_length_x = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.sign_focal_length_y = new byte[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.exponent_focal_length_y = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.mantissa_focal_length_y = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.sign_principal_point_x = new byte[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.exponent_principal_point_x = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.mantissa_principal_point_x = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.sign_principal_point_y = new byte[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.exponent_principal_point_y = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.mantissa_principal_point_y = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.sign_skew_factor = new byte[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.exponent_skew_factor = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                this.mantissa_skew_factor = new uint[intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1 + 1];
                for (i = 0; i <= intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_focal_length_x[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_focal_length_x[i]);
                    size += stream.ReadUnsignedIntVariable(size, mantissa_focal_length_x, out this.mantissa_focal_length_x[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_focal_length_y[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_focal_length_y[i]);
                    size += stream.ReadUnsignedIntVariable(size, mantissa_focal_length_y, out this.mantissa_focal_length_y[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_principal_point_x[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_principal_point_x[i]);
                    size += stream.ReadUnsignedIntVariable(size, mantissa_principal_point_x, out this.mantissa_principal_point_x[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_principal_point_y[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_principal_point_y[i]);
                    size += stream.ReadUnsignedIntVariable(size, mantissa_principal_point_y, out this.mantissa_principal_point_y[i]);
                    size += stream.ReadUnsignedInt(size, 1, out this.sign_skew_factor[i]);
                    size += stream.ReadUnsignedInt(size, 6, out this.exponent_skew_factor[i]);
                    size += stream.ReadUnsignedIntVariable(size, mantissa_skew_factor, out this.mantissa_skew_factor[i]);
                }
            }

            if (extrinsic_param_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_rotation_param);
                size += stream.ReadUnsignedIntGolomb(size, out this.prec_translation_param);

                this.sign_r = new byte[numViewsMinus1 + 1][][];
                this.exponent_r = new uint[numViewsMinus1 + 1][][];
                this.mantissa_r = new uint[numViewsMinus1 + 1][][];
                this.sign_t = new byte[numViewsMinus1 + 1][];
                this.exponent_t = new uint[numViewsMinus1 + 1][];
                this.mantissa_t = new uint[numViewsMinus1 + 1][];
                for (i = 0; i <= numViewsMinus1; i++)
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
                            size += stream.ReadUnsignedIntVariable(size, mantissa_r, out this.mantissa_r[i][j][k]);
                        }
                        size += stream.ReadUnsignedInt(size, 1, out this.sign_t[i][j]);
                        size += stream.ReadUnsignedInt(size, 6, out this.exponent_t[i][j]);
                        size += stream.ReadUnsignedIntVariable(size, mantissa_t, out this.mantissa_t[i][j]);
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

            if (intrinsic_param_flag != 0)
            {
                size += stream.WriteUnsignedInt(1, this.intrinsic_params_equal_flag);
                size += stream.WriteUnsignedIntGolomb(this.prec_focal_length);
                size += stream.WriteUnsignedIntGolomb(this.prec_principal_point);
                size += stream.WriteUnsignedIntGolomb(this.prec_skew_factor);

                for (i = 0; i <= intrinsic_params_equal_flag != 0 ? 0 : numViewsMinus1; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.sign_focal_length_x[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_focal_length_x[i]);
                    size += stream.WriteUnsignedIntVariable(mantissa_focal_length_x[i], this.mantissa_focal_length_x[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_focal_length_y[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_focal_length_y[i]);
                    size += stream.WriteUnsignedIntVariable(mantissa_focal_length_y[i], this.mantissa_focal_length_y[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_principal_point_x[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_principal_point_x[i]);
                    size += stream.WriteUnsignedIntVariable(mantissa_principal_point_x[i], this.mantissa_principal_point_x[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_principal_point_y[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_principal_point_y[i]);
                    size += stream.WriteUnsignedIntVariable(mantissa_principal_point_y[i], this.mantissa_principal_point_y[i]);
                    size += stream.WriteUnsignedInt(1, this.sign_skew_factor[i]);
                    size += stream.WriteUnsignedInt(6, this.exponent_skew_factor[i]);
                    size += stream.WriteUnsignedIntVariable(mantissa_skew_factor[i], this.mantissa_skew_factor[i]);
                }
            }

            if (extrinsic_param_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.prec_rotation_param);
                size += stream.WriteUnsignedIntGolomb(this.prec_translation_param);

                for (i = 0; i <= numViewsMinus1; i++)
                {

                    for (j = 0; j < 3; j++)
                    {
                        /*  row  */


                        for (k = 0; k < 3; k++)
                        {
                            /*  column  */

                            size += stream.WriteUnsignedInt(1, this.sign_r[i][j][k]);
                            size += stream.WriteUnsignedInt(6, this.exponent_r[i][j][k]);
                            size += stream.WriteUnsignedIntVariable(mantissa_r[i][j][k], this.mantissa_r[i][j][k]);
                        }
                        size += stream.WriteUnsignedInt(1, this.sign_t[i][j]);
                        size += stream.WriteUnsignedInt(6, this.exponent_t[i][j]);
                        size += stream.WriteUnsignedIntVariable(mantissa_t[i][j], this.mantissa_t[i][j]);
                    }
                }
            }

            return size;
        }

    }

    /*
 

multiview_view_position( payloadSize ) { 
num_views_minus1  ue(v)
for( i = 0; i <= num_views_minus1; i++ )  
view_position[ i ] ue(v) 
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


video_parameter_set_rbsp() { 
vps_video_parameter_set_id  u(4) 
vps_base_layer_internal_flag u(1) 
vps_base_layer_available_flag u(1) 
vps_max_layers_minus1 u(6) 
vps_max_sub_layers_minus1 u(3) 
vps_temporal_id_nesting_flag u(1) 
vps_reserved_0xffff_16bits u(16) 
profile_tier_level( 1, vps_max_sub_layers_minus1 )  
vps_sub_layer_ordering_info_present_flag  u(1)
for( i = ( vps_sub_layer_ordering_info_present_flag != 0 ?  0 : vps_max_sub_layers_minus1 ); i<= vps_max_sub_layers_minus1; i++ ) { 
vps_max_dec_pic_buffering_minus1[ i ] ue(v) 
vps_max_num_reorder_pics[ i ] ue(v) 
vps_max_latency_increase_plus1[ i ] ue(v) 
}  
vps_max_layer_id u(6)
vps_num_layer_sets_minus1 ue(v) 
for( i = 1; i <= vps_num_layer_sets_minus1; i++ )  
for( j = 0; j <= vps_max_layer_id; j++ )  
layer_id_included_flag[ i ][ j ] u(1)
 vps_timing_info_present_flag u(1) 
 if( vps_timing_info_present_flag ) {  
  vps_num_units_in_tick u(32) 
  vps_time_scale u(32) 
  vps_poc_proportional_to_timing_flag u(1) 
  if( vps_poc_proportional_to_timing_flag )  
   vps_num_ticks_poc_diff_one_minus1 ue(v) 
  vps_num_hrd_parameters ue(v) 
  for( i = 0; i < vps_num_hrd_parameters; i++ ) {  
   hrd_layer_set_idx[ i ] ue(v) 
   if( i > 0 )  
    cprms_present_flag[ i ] u(1) 
   hrd_parameters( cprms_present_flag[ i ], vps_max_sub_layers_minus1 )  
  }  
 }  
 vps_extension_flag u(1) 
 if( vps_extension_flag ) {  
  while( !byte_aligned() )  
   vps_extension_alignment_bit_equal_to_one u(1) 
  vps_extension()  
  vps_extension2_flag u(1) 
  if( vps_extension2_flag ) {  
   vps_3d_extension_flag u(1) 
   if( vps_3d_extension_flag ) {  
    while( !byte_aligned() )  
     vps_3d_extension_alignment_bit_equal_to_one u(1) 
    vps_3d_extension()  
   }  
   vps_extension3_flag u(1) 
   if( vps_extension3_flag )  
    while( more_rbsp_data() )  
     vps_extension_data_flag u(1) 
  }  
 }  
 rbsp_trailing_bits()  
}
    */
    public class VideoParameterSetRbsp : IItuSerializable
    {
        private uint vps_video_parameter_set_id;
        public uint VpsVideoParameterSetId { get { return vps_video_parameter_set_id; } set { vps_video_parameter_set_id = value; } }
        private byte vps_base_layer_internal_flag;
        public byte VpsBaseLayerInternalFlag { get { return vps_base_layer_internal_flag; } set { vps_base_layer_internal_flag = value; } }
        private byte vps_base_layer_available_flag;
        public byte VpsBaseLayerAvailableFlag { get { return vps_base_layer_available_flag; } set { vps_base_layer_available_flag = value; } }
        private uint vps_max_layers_minus1;
        public uint VpsMaxLayersMinus1 { get { return vps_max_layers_minus1; } set { vps_max_layers_minus1 = value; } }
        private uint vps_max_sub_layers_minus1;
        public uint VpsMaxSubLayersMinus1 { get { return vps_max_sub_layers_minus1; } set { vps_max_sub_layers_minus1 = value; } }
        private byte vps_temporal_id_nesting_flag;
        public byte VpsTemporalIdNestingFlag { get { return vps_temporal_id_nesting_flag; } set { vps_temporal_id_nesting_flag = value; } }
        private uint vps_reserved_0xffff_16bits;
        public uint VpsReserved0xffff16bits { get { return vps_reserved_0xffff_16bits; } set { vps_reserved_0xffff_16bits = value; } }
        private ProfileTierLevel profile_tier_level;
        public ProfileTierLevel ProfileTierLevel { get { return profile_tier_level; } set { profile_tier_level = value; } }
        private byte vps_sub_layer_ordering_info_present_flag;
        public byte VpsSubLayerOrderingInfoPresentFlag { get { return vps_sub_layer_ordering_info_present_flag; } set { vps_sub_layer_ordering_info_present_flag = value; } }
        private uint[] vps_max_dec_pic_buffering_minus1;
        public uint[] VpsMaxDecPicBufferingMinus1 { get { return vps_max_dec_pic_buffering_minus1; } set { vps_max_dec_pic_buffering_minus1 = value; } }
        private uint[] vps_max_num_reorder_pics;
        public uint[] VpsMaxNumReorderPics { get { return vps_max_num_reorder_pics; } set { vps_max_num_reorder_pics = value; } }
        private uint[] vps_max_latency_increase_plus1;
        public uint[] VpsMaxLatencyIncreasePlus1 { get { return vps_max_latency_increase_plus1; } set { vps_max_latency_increase_plus1 = value; } }
        private uint vps_max_layer_id;
        public uint VpsMaxLayerId { get { return vps_max_layer_id; } set { vps_max_layer_id = value; } }
        private uint vps_num_layer_sets_minus1;
        public uint VpsNumLayerSetsMinus1 { get { return vps_num_layer_sets_minus1; } set { vps_num_layer_sets_minus1 = value; } }
        private byte[][] layer_id_included_flag;
        public byte[][] LayerIdIncludedFlag { get { return layer_id_included_flag; } set { layer_id_included_flag = value; } }
        private byte vps_timing_info_present_flag;
        public byte VpsTimingInfoPresentFlag { get { return vps_timing_info_present_flag; } set { vps_timing_info_present_flag = value; } }
        private uint vps_num_units_in_tick;
        public uint VpsNumUnitsInTick { get { return vps_num_units_in_tick; } set { vps_num_units_in_tick = value; } }
        private uint vps_time_scale;
        public uint VpsTimeScale { get { return vps_time_scale; } set { vps_time_scale = value; } }
        private byte vps_poc_proportional_to_timing_flag;
        public byte VpsPocProportionalToTimingFlag { get { return vps_poc_proportional_to_timing_flag; } set { vps_poc_proportional_to_timing_flag = value; } }
        private uint vps_num_ticks_poc_diff_one_minus1;
        public uint VpsNumTicksPocDiffOneMinus1 { get { return vps_num_ticks_poc_diff_one_minus1; } set { vps_num_ticks_poc_diff_one_minus1 = value; } }
        private uint vps_num_hrd_parameters;
        public uint VpsNumHrdParameters { get { return vps_num_hrd_parameters; } set { vps_num_hrd_parameters = value; } }
        private uint[] hrd_layer_set_idx;
        public uint[] HrdLayerSetIdx { get { return hrd_layer_set_idx; } set { hrd_layer_set_idx = value; } }
        private byte[] cprms_present_flag;
        public byte[] CprmsPresentFlag { get { return cprms_present_flag; } set { cprms_present_flag = value; } }
        private HrdParameters[] hrd_parameters;
        public HrdParameters[] HrdParameters { get { return hrd_parameters; } set { hrd_parameters = value; } }
        private byte vps_extension_flag;
        public byte VpsExtensionFlag { get { return vps_extension_flag; } set { vps_extension_flag = value; } }
        private Dictionary<int, byte> vps_extension_alignment_bit_equal_to_one = new Dictionary<int, byte>();
        public Dictionary<int, byte> VpsExtensionAlignmentBitEqualToOne { get { return vps_extension_alignment_bit_equal_to_one; } set { vps_extension_alignment_bit_equal_to_one = value; } }
        private VpsExtension vps_extension;
        public VpsExtension VpsExtension { get { return vps_extension; } set { vps_extension = value; } }
        private byte vps_extension2_flag;
        public byte VpsExtension2Flag { get { return vps_extension2_flag; } set { vps_extension2_flag = value; } }
        private byte vps_3d_extension_flag;
        public byte Vps3dExtensionFlag { get { return vps_3d_extension_flag; } set { vps_3d_extension_flag = value; } }
        private Dictionary<int, byte> vps_3d_extension_alignment_bit_equal_to_one = new Dictionary<int, byte>();
        public Dictionary<int, byte> Vps3dExtensionAlignmentBitEqualToOne { get { return vps_3d_extension_alignment_bit_equal_to_one; } set { vps_3d_extension_alignment_bit_equal_to_one = value; } }
        private Vps3dExtension vps_3d_extension;
        public Vps3dExtension Vps3dExtension { get { return vps_3d_extension; } set { vps_3d_extension = value; } }
        private byte vps_extension3_flag;
        public byte VpsExtension3Flag { get { return vps_extension3_flag; } set { vps_extension3_flag = value; } }
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
            size += stream.ReadUnsignedInt(size, 4, out this.vps_video_parameter_set_id);
            size += stream.ReadUnsignedInt(size, 1, out this.vps_base_layer_internal_flag);
            size += stream.ReadUnsignedInt(size, 1, out this.vps_base_layer_available_flag);
            size += stream.ReadUnsignedInt(size, 6, out this.vps_max_layers_minus1);
            size += stream.ReadUnsignedInt(size, 3, out this.vps_max_sub_layers_minus1);
            size += stream.ReadUnsignedInt(size, 1, out this.vps_temporal_id_nesting_flag);
            size += stream.ReadUnsignedInt(size, 16, out this.vps_reserved_0xffff_16bits);
            this.profile_tier_level = new ProfileTierLevel(1, vps_max_sub_layers_minus1);
            size += stream.ReadClass<ProfileTierLevel>(size, context, this.profile_tier_level);
            size += stream.ReadUnsignedInt(size, 1, out this.vps_sub_layer_ordering_info_present_flag);

            this.vps_max_dec_pic_buffering_minus1 = new uint[vps_max_sub_layers_minus1 + 1];
            this.vps_max_num_reorder_pics = new uint[vps_max_sub_layers_minus1 + 1];
            this.vps_max_latency_increase_plus1 = new uint[vps_max_sub_layers_minus1 + 1];
            for (i = (vps_sub_layer_ordering_info_present_flag != 0 ? 0 : vps_max_sub_layers_minus1); i <= vps_max_sub_layers_minus1; i++)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.vps_max_dec_pic_buffering_minus1[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.vps_max_num_reorder_pics[i]);
                size += stream.ReadUnsignedIntGolomb(size, out this.vps_max_latency_increase_plus1[i]);
            }
            size += stream.ReadUnsignedInt(size, 6, out this.vps_max_layer_id);
            size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_layer_sets_minus1);

            this.layer_id_included_flag = new byte[vps_num_layer_sets_minus1 + 1][];
            for (i = 1; i <= vps_num_layer_sets_minus1; i++)
            {

                this.layer_id_included_flag[i] = new byte[vps_max_layer_id];
                for (j = 0; j <= vps_max_layer_id; j++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.layer_id_included_flag[i][j]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_timing_info_present_flag);

            if (vps_timing_info_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 32, out this.vps_num_units_in_tick);
                size += stream.ReadUnsignedInt(size, 32, out this.vps_time_scale);
                size += stream.ReadUnsignedInt(size, 1, out this.vps_poc_proportional_to_timing_flag);

                if (vps_poc_proportional_to_timing_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_ticks_poc_diff_one_minus1);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.vps_num_hrd_parameters);

                this.hrd_layer_set_idx = new uint[vps_num_hrd_parameters];
                this.cprms_present_flag = new byte[vps_num_hrd_parameters];
                this.hrd_parameters = new HrdParameters[vps_num_hrd_parameters];
                for (i = 0; i < vps_num_hrd_parameters; i++)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.hrd_layer_set_idx[i]);

                    if (i > 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.cprms_present_flag[i]);
                    }
                    this.hrd_parameters[i] = new HrdParameters(cprms_present_flag[i], vps_max_sub_layers_minus1);
                    size += stream.ReadClass<HrdParameters>(size, context, this.hrd_parameters[i]);
                }
            }
            size += stream.ReadUnsignedInt(size, 1, out this.vps_extension_flag);

            if (vps_extension_flag != 0)
            {

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.vps_extension_alignment_bit_equal_to_one);
                }
                this.vps_extension = new VpsExtension();
                size += stream.ReadClass<VpsExtension>(size, context, this.vps_extension);
                size += stream.ReadUnsignedInt(size, 1, out this.vps_extension2_flag);

                if (vps_extension2_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.vps_3d_extension_flag);

                    if (vps_3d_extension_flag != 0)
                    {

                        while (!stream.ByteAligned())
                        {
                            whileIndex++;

                            size += stream.ReadUnsignedInt(size, 1, whileIndex, this.vps_3d_extension_alignment_bit_equal_to_one);
                        }
                        this.vps_3d_extension = new Vps3dExtension();
                        size += stream.ReadClass<Vps3dExtension>(size, context, this.vps_3d_extension);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.vps_extension3_flag);

                    if (vps_extension3_flag != 0)
                    {

                        while (stream.ReadMoreRbspData(this))
                        {
                            whileIndex++;

                            size += stream.ReadUnsignedInt(size, 1, whileIndex, this.vps_extension_data_flag);
                        }
                    }
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
            size += stream.WriteUnsignedInt(4, this.vps_video_parameter_set_id);
            size += stream.WriteUnsignedInt(1, this.vps_base_layer_internal_flag);
            size += stream.WriteUnsignedInt(1, this.vps_base_layer_available_flag);
            size += stream.WriteUnsignedInt(6, this.vps_max_layers_minus1);
            size += stream.WriteUnsignedInt(3, this.vps_max_sub_layers_minus1);
            size += stream.WriteUnsignedInt(1, this.vps_temporal_id_nesting_flag);
            size += stream.WriteUnsignedInt(16, this.vps_reserved_0xffff_16bits);
            size += stream.WriteClass<ProfileTierLevel>(context, this.profile_tier_level);
            size += stream.WriteUnsignedInt(1, this.vps_sub_layer_ordering_info_present_flag);

            for (i = (vps_sub_layer_ordering_info_present_flag != 0 ? 0 : vps_max_sub_layers_minus1); i <= vps_max_sub_layers_minus1; i++)
            {
                size += stream.WriteUnsignedIntGolomb(this.vps_max_dec_pic_buffering_minus1[i]);
                size += stream.WriteUnsignedIntGolomb(this.vps_max_num_reorder_pics[i]);
                size += stream.WriteUnsignedIntGolomb(this.vps_max_latency_increase_plus1[i]);
            }
            size += stream.WriteUnsignedInt(6, this.vps_max_layer_id);
            size += stream.WriteUnsignedIntGolomb(this.vps_num_layer_sets_minus1);

            for (i = 1; i <= vps_num_layer_sets_minus1; i++)
            {

                for (j = 0; j <= vps_max_layer_id; j++)
                {
                    size += stream.WriteUnsignedInt(1, this.layer_id_included_flag[i][j]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.vps_timing_info_present_flag);

            if (vps_timing_info_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(32, this.vps_num_units_in_tick);
                size += stream.WriteUnsignedInt(32, this.vps_time_scale);
                size += stream.WriteUnsignedInt(1, this.vps_poc_proportional_to_timing_flag);

                if (vps_poc_proportional_to_timing_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.vps_num_ticks_poc_diff_one_minus1);
                }
                size += stream.WriteUnsignedIntGolomb(this.vps_num_hrd_parameters);

                for (i = 0; i < vps_num_hrd_parameters; i++)
                {
                    size += stream.WriteUnsignedIntGolomb(this.hrd_layer_set_idx[i]);

                    if (i > 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.cprms_present_flag[i]);
                    }
                    size += stream.WriteClass<HrdParameters>(context, this.hrd_parameters[i]);
                }
            }
            size += stream.WriteUnsignedInt(1, this.vps_extension_flag);

            if (vps_extension_flag != 0)
            {

                while (!stream.ByteAligned())
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.vps_extension_alignment_bit_equal_to_one);
                }
                size += stream.WriteClass<VpsExtension>(context, this.vps_extension);
                size += stream.WriteUnsignedInt(1, this.vps_extension2_flag);

                if (vps_extension2_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.vps_3d_extension_flag);

                    if (vps_3d_extension_flag != 0)
                    {

                        while (!stream.ByteAligned())
                        {
                            whileIndex++;

                            size += stream.WriteUnsignedInt(1, whileIndex, this.vps_3d_extension_alignment_bit_equal_to_one);
                        }
                        size += stream.WriteClass<Vps3dExtension>(context, this.vps_3d_extension);
                    }
                    size += stream.WriteUnsignedInt(1, this.vps_extension3_flag);

                    if (vps_extension3_flag != 0)
                    {

                        while (stream.WriteMoreRbspData(this))
                        {
                            whileIndex++;

                            size += stream.WriteUnsignedInt(1, whileIndex, this.vps_extension_data_flag);
                        }
                    }
                }
            }
            size += stream.WriteClass<RbspTrailingBits>(context, this.rbsp_trailing_bits);

            return size;
        }

    }

    /*
 

vps_3d_extension() {  
 cp_precision ue(v) 
 for( n = 1; n < NumViews; n++ ) {  
  i = ViewOIdxList[ n ]  
  num_cp[ i ] u(6) 
  if( num_cp[ i ] > 0 ) {  
   cp_in_slice_segment_header_flag[ i ] u(1) 
   for( m = 0; m < num_cp[ i ]; m++ ) {  
    cp_ref_voi[ i ][ m ] ue(v) 
    if( !cp_in_slice_segment_header_flag[ i ] ) {  
     j = cp_ref_voi[ i ][ m ]  
     vps_cp_scale[ i ][ j ] se(v) 
     vps_cp_off[ i ][ j ] se(v) 
     vps_cp_inv_scale_plus_scale[ i ][ j ] se(v) 
     vps_cp_inv_off_plus_off[ i ][ j ] se(v) 
    }  
   }  
  }  
 }  
}
    */
    public class Vps3dExtension : IItuSerializable
    {
        private uint cp_precision;
        public uint CpPrecision { get { return cp_precision; } set { cp_precision = value; } }
        private uint[][] num_cp;
        public uint[][] NumCp { get { return num_cp; } set { num_cp = value; } }
        private byte[][] cp_in_slice_segment_header_flag;
        public byte[][] CpInSliceSegmentHeaderFlag { get { return cp_in_slice_segment_header_flag; } set { cp_in_slice_segment_header_flag = value; } }
        private uint[][][] cp_ref_voi;
        public uint[][][] CpRefVoi { get { return cp_ref_voi; } set { cp_ref_voi = value; } }
        private int[][][][] vps_cp_scale;
        public int[][][][] VpsCpScale { get { return vps_cp_scale; } set { vps_cp_scale = value; } }
        private int[][][][] vps_cp_off;
        public int[][][][] VpsCpOff { get { return vps_cp_off; } set { vps_cp_off = value; } }
        private int[][][][] vps_cp_inv_scale_plus_scale;
        public int[][][][] VpsCpInvScalePlusScale { get { return vps_cp_inv_scale_plus_scale; } set { vps_cp_inv_scale_plus_scale = value; } }
        private int[][][][] vps_cp_inv_off_plus_off;
        public int[][][][] VpsCpInvOffPlusOff { get { return vps_cp_inv_off_plus_off; } set { vps_cp_inv_off_plus_off = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public Vps3dExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint n = 0;
            uint i = 0;
            uint m = 0;
            uint j = 0;
            size += stream.ReadUnsignedIntGolomb(size, out this.cp_precision);

            this.num_cp = new uint[NumViews];
            this.cp_in_slice_segment_header_flag = new byte[NumViews];
            this.cp_ref_voi = new uint[NumViews][];
            this.vps_cp_scale = new int[NumViews][];
            this.vps_cp_off = new int[NumViews][];
            this.vps_cp_inv_scale_plus_scale = new int[NumViews][];
            this.vps_cp_inv_off_plus_off = new int[NumViews][];
            for (n = 1; n < NumViews; n++)
            {
                i = ViewOIdxList[n];
                size += stream.ReadUnsignedInt(size, 6, out this.num_cp[i][n]);

                if (num_cp[i] > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.cp_in_slice_segment_header_flag[i][n]);

                    this.cp_ref_voi[n] = new uint[num_cp[i]];
                    this.vps_cp_scale[n] = new int[num_cp[i]];
                    this.vps_cp_off[n] = new int[num_cp[i]];
                    this.vps_cp_inv_scale_plus_scale[n] = new int[num_cp[i]];
                    this.vps_cp_inv_off_plus_off[n] = new int[num_cp[i]];
                    for (m = 0; m < num_cp[i]; m++)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.cp_ref_voi[i][m][n]);

                        if (cp_in_slice_segment_header_flag[i] == 0)
                        {
                            j = cp_ref_voi[i][m];
                            size += stream.ReadSignedIntGolomb(size, out this.vps_cp_scale[i][j][n][m]);
                            size += stream.ReadSignedIntGolomb(size, out this.vps_cp_off[i][j][n][m]);
                            size += stream.ReadSignedIntGolomb(size, out this.vps_cp_inv_scale_plus_scale[i][j][n][m]);
                            size += stream.ReadSignedIntGolomb(size, out this.vps_cp_inv_off_plus_off[i][j][n][m]);
                        }
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint n = 0;
            uint i = 0;
            uint m = 0;
            uint j = 0;
            size += stream.WriteUnsignedIntGolomb(this.cp_precision);

            for (n = 1; n < NumViews; n++)
            {
                i = ViewOIdxList[n];
                size += stream.WriteUnsignedInt(6, this.num_cp[i][n]);

                if (num_cp[i] > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.cp_in_slice_segment_header_flag[i][n]);

                    for (m = 0; m < num_cp[i]; m++)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.cp_ref_voi[i][m][n]);

                        if (cp_in_slice_segment_header_flag[i] == 0)
                        {
                            j = cp_ref_voi[i][m];
                            size += stream.WriteSignedIntGolomb(this.vps_cp_scale[i][j][n][m]);
                            size += stream.WriteSignedIntGolomb(this.vps_cp_off[i][j][n][m]);
                            size += stream.WriteSignedIntGolomb(this.vps_cp_inv_scale_plus_scale[i][j][n][m]);
                            size += stream.WriteSignedIntGolomb(this.vps_cp_inv_off_plus_off[i][j][n][m]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

sps_3d_extension() {  
 for( d = 0; d <= 1; d++ ) {  
  iv_di_mc_enabled_flag[ d ] u(1) 
  iv_mv_scal_enabled_flag[ d ] u(1) 
  if( d == 0 ) {  
   log2_ivmc_sub_pb_size_minus3[ d ] ue(v) 
      iv_res_pred_enabled_flag[ d ] u(1) 
   depth_ref_enabled_flag[ d ] u(1) 
   vsp_mc_enabled_flag[ d ] u(1) 
   dbbp_enabled_flag[ d ] u(1) 
  } else {  
   tex_mc_enabled_flag[ d ] u(1) 
   log2_texmc_sub_pb_size_minus3[ d ] ue(v) 
   intra_contour_enabled_flag[ d ] u(1) 
   intra_dc_only_wedge_enabled_flag[ d ] u(1) 
   cqt_cu_part_pred_enabled_flag[ d ] u(1) 
   inter_dc_only_enabled_flag[ d ] u(1) 
   skip_intra_enabled_flag[ d ] u(1) 
  }  
 }  
}
    */
    public class Sps3dExtension : IItuSerializable
    {
        private byte[] iv_di_mc_enabled_flag;
        public byte[] IvDiMcEnabledFlag { get { return iv_di_mc_enabled_flag; } set { iv_di_mc_enabled_flag = value; } }
        private byte[] iv_mv_scal_enabled_flag;
        public byte[] IvMvScalEnabledFlag { get { return iv_mv_scal_enabled_flag; } set { iv_mv_scal_enabled_flag = value; } }
        private uint[] log2_ivmc_sub_pb_size_minus3;
        public uint[] Log2IvmcSubPbSizeMinus3 { get { return log2_ivmc_sub_pb_size_minus3; } set { log2_ivmc_sub_pb_size_minus3 = value; } }
        private byte[] iv_res_pred_enabled_flag;
        public byte[] IvResPredEnabledFlag { get { return iv_res_pred_enabled_flag; } set { iv_res_pred_enabled_flag = value; } }
        private byte[] depth_ref_enabled_flag;
        public byte[] DepthRefEnabledFlag { get { return depth_ref_enabled_flag; } set { depth_ref_enabled_flag = value; } }
        private byte[] vsp_mc_enabled_flag;
        public byte[] VspMcEnabledFlag { get { return vsp_mc_enabled_flag; } set { vsp_mc_enabled_flag = value; } }
        private byte[] dbbp_enabled_flag;
        public byte[] DbbpEnabledFlag { get { return dbbp_enabled_flag; } set { dbbp_enabled_flag = value; } }
        private byte[] tex_mc_enabled_flag;
        public byte[] TexMcEnabledFlag { get { return tex_mc_enabled_flag; } set { tex_mc_enabled_flag = value; } }
        private uint[] log2_texmc_sub_pb_size_minus3;
        public uint[] Log2TexmcSubPbSizeMinus3 { get { return log2_texmc_sub_pb_size_minus3; } set { log2_texmc_sub_pb_size_minus3 = value; } }
        private byte[] intra_contour_enabled_flag;
        public byte[] IntraContourEnabledFlag { get { return intra_contour_enabled_flag; } set { intra_contour_enabled_flag = value; } }
        private byte[] intra_dc_only_wedge_enabled_flag;
        public byte[] IntraDcOnlyWedgeEnabledFlag { get { return intra_dc_only_wedge_enabled_flag; } set { intra_dc_only_wedge_enabled_flag = value; } }
        private byte[] cqt_cu_part_pred_enabled_flag;
        public byte[] CqtCuPartPredEnabledFlag { get { return cqt_cu_part_pred_enabled_flag; } set { cqt_cu_part_pred_enabled_flag = value; } }
        private byte[] inter_dc_only_enabled_flag;
        public byte[] InterDcOnlyEnabledFlag { get { return inter_dc_only_enabled_flag; } set { inter_dc_only_enabled_flag = value; } }
        private byte[] skip_intra_enabled_flag;
        public byte[] SkipIntraEnabledFlag { get { return skip_intra_enabled_flag; } set { skip_intra_enabled_flag = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public Sps3dExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint d = 0;

            this.iv_di_mc_enabled_flag = new byte[1];
            this.iv_mv_scal_enabled_flag = new byte[1];
            this.log2_ivmc_sub_pb_size_minus3 = new uint[1];
            this.iv_res_pred_enabled_flag = new byte[1];
            this.depth_ref_enabled_flag = new byte[1];
            this.vsp_mc_enabled_flag = new byte[1];
            this.dbbp_enabled_flag = new byte[1];
            this.tex_mc_enabled_flag = new byte[1];
            this.log2_texmc_sub_pb_size_minus3 = new uint[1];
            this.intra_contour_enabled_flag = new byte[1];
            this.intra_dc_only_wedge_enabled_flag = new byte[1];
            this.cqt_cu_part_pred_enabled_flag = new byte[1];
            this.inter_dc_only_enabled_flag = new byte[1];
            this.skip_intra_enabled_flag = new byte[1];
            for (d = 0; d <= 1; d++)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.iv_di_mc_enabled_flag[d]);
                size += stream.ReadUnsignedInt(size, 1, out this.iv_mv_scal_enabled_flag[d]);

                if (d == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_ivmc_sub_pb_size_minus3[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.iv_res_pred_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.depth_ref_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.vsp_mc_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.dbbp_enabled_flag[d]);
                }
                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.tex_mc_enabled_flag[d]);
                    size += stream.ReadUnsignedIntGolomb(size, out this.log2_texmc_sub_pb_size_minus3[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.intra_contour_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.intra_dc_only_wedge_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.cqt_cu_part_pred_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.inter_dc_only_enabled_flag[d]);
                    size += stream.ReadUnsignedInt(size, 1, out this.skip_intra_enabled_flag[d]);
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint d = 0;

            for (d = 0; d <= 1; d++)
            {
                size += stream.WriteUnsignedInt(1, this.iv_di_mc_enabled_flag[d]);
                size += stream.WriteUnsignedInt(1, this.iv_mv_scal_enabled_flag[d]);

                if (d == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.log2_ivmc_sub_pb_size_minus3[d]);
                    size += stream.WriteUnsignedInt(1, this.iv_res_pred_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.depth_ref_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.vsp_mc_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.dbbp_enabled_flag[d]);
                }
                else
                {
                    size += stream.WriteUnsignedInt(1, this.tex_mc_enabled_flag[d]);
                    size += stream.WriteUnsignedIntGolomb(this.log2_texmc_sub_pb_size_minus3[d]);
                    size += stream.WriteUnsignedInt(1, this.intra_contour_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.intra_dc_only_wedge_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.cqt_cu_part_pred_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.inter_dc_only_enabled_flag[d]);
                    size += stream.WriteUnsignedInt(1, this.skip_intra_enabled_flag[d]);
                }
            }

            return size;
        }

    }

    /*
  

pps_3d_extension() {  
 dlts_present_flag u(1) 
 if( dlts_present_flag ) {  
  pps_depth_layers_minus1 u(6) 
  pps_bit_depth_for_depth_layers_minus8 u(4) 
  for( i = 0; i <= pps_depth_layers_minus1; i++ ) {  
   dlt_flag[ i ] u(1) 
   if( dlt_flag[ i ] ) {  
    dlt_pred_flag[ i ] u(1) 
    if( !dlt_pred_flag[ i ] )  
     dlt_val_flags_present_flag[ i ] u(1) 
    if( dlt_val_flags_present_flag[ i ] )  
     for( j = 0; j <= depthMaxValue; j++ )  
           dlt_value_flag[ i ][ j ] u(1) 
    else  
     delta_dlt()  
   }  
  }  
 }  
}
    */
    public class Pps3dExtension : IItuSerializable
    {
        private byte dlts_present_flag;
        public byte DltsPresentFlag { get { return dlts_present_flag; } set { dlts_present_flag = value; } }
        private uint pps_depth_layers_minus1;
        public uint PpsDepthLayersMinus1 { get { return pps_depth_layers_minus1; } set { pps_depth_layers_minus1 = value; } }
        private uint pps_bit_depth_for_depth_layers_minus8;
        public uint PpsBitDepthForDepthLayersMinus8 { get { return pps_bit_depth_for_depth_layers_minus8; } set { pps_bit_depth_for_depth_layers_minus8 = value; } }
        private byte[] dlt_flag;
        public byte[] DltFlag { get { return dlt_flag; } set { dlt_flag = value; } }
        private byte[] dlt_pred_flag;
        public byte[] DltPredFlag { get { return dlt_pred_flag; } set { dlt_pred_flag = value; } }
        private byte[] dlt_val_flags_present_flag;
        public byte[] DltValFlagsPresentFlag { get { return dlt_val_flags_present_flag; } set { dlt_val_flags_present_flag = value; } }
        private byte[][] dlt_value_flag;
        public byte[][] DltValueFlag { get { return dlt_value_flag; } set { dlt_value_flag = value; } }
        private DeltaDlt[] delta_dlt;
        public DeltaDlt[] DeltaDlt { get { return delta_dlt; } set { delta_dlt = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public Pps3dExtension()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.dlts_present_flag);

            if (dlts_present_flag != 0)
            {
                size += stream.ReadUnsignedInt(size, 6, out this.pps_depth_layers_minus1);
                size += stream.ReadUnsignedInt(size, 4, out this.pps_bit_depth_for_depth_layers_minus8);

                this.dlt_flag = new byte[pps_depth_layers_minus1 + 1];
                this.dlt_pred_flag = new byte[pps_depth_layers_minus1 + 1];
                this.dlt_val_flags_present_flag = new byte[pps_depth_layers_minus1 + 1];
                this.dlt_value_flag = new byte[pps_depth_layers_minus1 + 1][];
                this.delta_dlt = new DeltaDlt[pps_depth_layers_minus1 + 1];
                for (i = 0; i <= pps_depth_layers_minus1; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.dlt_flag[i]);

                    if (dlt_flag[i] != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.dlt_pred_flag[i]);

                        if (dlt_pred_flag[i] == 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.dlt_val_flags_present_flag[i]);
                        }

                        if (dlt_val_flags_present_flag[i] != 0)
                        {

                            this.dlt_value_flag[i] = new byte[depthMaxValue];
                            for (j = 0; j <= depthMaxValue; j++)
                            {
                                size += stream.ReadUnsignedInt(size, 1, out this.dlt_value_flag[i][j]);
                            }
                        }
                        else
                        {
                            this.delta_dlt[i] = new DeltaDlt();
                            size += stream.ReadClass<DeltaDlt>(size, context, this.delta_dlt[i]);
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
            size += stream.WriteUnsignedInt(1, this.dlts_present_flag);

            if (dlts_present_flag != 0)
            {
                size += stream.WriteUnsignedInt(6, this.pps_depth_layers_minus1);
                size += stream.WriteUnsignedInt(4, this.pps_bit_depth_for_depth_layers_minus8);

                for (i = 0; i <= pps_depth_layers_minus1; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.dlt_flag[i]);

                    if (dlt_flag[i] != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.dlt_pred_flag[i]);

                        if (dlt_pred_flag[i] == 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.dlt_val_flags_present_flag[i]);
                        }

                        if (dlt_val_flags_present_flag[i] != 0)
                        {

                            for (j = 0; j <= depthMaxValue; j++)
                            {
                                size += stream.WriteUnsignedInt(1, this.dlt_value_flag[i][j]);
                            }
                        }
                        else
                        {
                            size += stream.WriteClass<DeltaDlt>(context, this.delta_dlt[i]);
                        }
                    }
                }
            }

            return size;
        }

    }

    /*
  

delta_dlt() {  
 num_val_delta_dlt u(v) 
 if( num_val_delta_dlt > 0 ) {  
  if( num_val_delta_dlt > 1 )  
   max_diff u(v) 
  if( num_val_delta_dlt > 2  &&  max_diff > 0 )  
   min_diff_minus1 u(v) 
  delta_dlt_val0 u(v) 
  if( max_diff > ( min_diff_minus1 + 1 ) )  
   for( k = 1; k < num_val_delta_dlt; k++ )   
    delta_val_diff_minus_min[ k ] u(v) 
 }  
}
    */
    public class DeltaDlt : IItuSerializable
    {
        private uint num_val_delta_dlt;
        public uint NumValDeltaDlt { get { return num_val_delta_dlt; } set { num_val_delta_dlt = value; } }
        private uint max_diff;
        public uint MaxDiff { get { return max_diff; } set { max_diff = value; } }
        private uint min_diff_minus1;
        public uint MinDiffMinus1 { get { return min_diff_minus1; } set { min_diff_minus1 = value; } }
        private uint delta_dlt_val0;
        public uint DeltaDltVal0 { get { return delta_dlt_val0; } set { delta_dlt_val0 = value; } }
        private uint[] delta_val_diff_minus_min;
        public uint[] DeltaValDiffMinusMin { get { return delta_val_diff_minus_min; } set { delta_val_diff_minus_min = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public DeltaDlt()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;
            size += stream.ReadUnsignedIntVariable(size, num_val_delta_dlt, out this.num_val_delta_dlt);

            if (num_val_delta_dlt > 0)
            {

                if (num_val_delta_dlt > 1)
                {
                    size += stream.ReadUnsignedIntVariable(size, max_diff, out this.max_diff);
                }

                if (num_val_delta_dlt > 2 && max_diff > 0)
                {
                    size += stream.ReadUnsignedIntVariable(size, min_diff_minus1, out this.min_diff_minus1);
                }
                size += stream.ReadUnsignedIntVariable(size, delta_dlt_val0, out this.delta_dlt_val0);

                if (max_diff > (min_diff_minus1 + 1))
                {

                    this.delta_val_diff_minus_min = new uint[num_val_delta_dlt];
                    for (k = 1; k < num_val_delta_dlt; k++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, delta_val_diff_minus_min, out this.delta_val_diff_minus_min[k]);
                    }
                }
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint k = 0;
            size += stream.WriteUnsignedIntVariable(num_val_delta_dlt, this.num_val_delta_dlt);

            if (num_val_delta_dlt > 0)
            {

                if (num_val_delta_dlt > 1)
                {
                    size += stream.WriteUnsignedIntVariable(max_diff, this.max_diff);
                }

                if (num_val_delta_dlt > 2 && max_diff > 0)
                {
                    size += stream.WriteUnsignedIntVariable(min_diff_minus1, this.min_diff_minus1);
                }
                size += stream.WriteUnsignedIntVariable(delta_dlt_val0, this.delta_dlt_val0);

                if (max_diff > (min_diff_minus1 + 1))
                {

                    for (k = 1; k < num_val_delta_dlt; k++)
                    {
                        size += stream.WriteUnsignedIntVariable(delta_val_diff_minus_min[k], this.delta_val_diff_minus_min[k]);
                    }
                }
            }

            return size;
        }

    }

    /*
 

slice_segment_header() {  
 first_slice_segment_in_pic_flag u(1) 
 if( nal_unit_type >= BLA_W_LP  &&  nal_unit_type <= RSV_IRAP_VCL23 )  
  no_output_of_prior_pics_flag u(1) 
 slice_pic_parameter_set_id ue(v) 
 if( !first_slice_segment_in_pic_flag ) {  
  if( dependent_slice_segments_enabled_flag )  
   dependent_slice_segment_flag u(1) 
  slice_segment_address u(v) 
 }  
 if( !dependent_slice_segment_flag ) {  
  i = 0  
  if( num_extra_slice_header_bits > i ) {  
   i++  
   discardable_flag u(1) 
  }  
  if( num_extra_slice_header_bits > i ) {  
   i++  
   cross_layer_bla_flag u(1) 
  }  
  for(; i < num_extra_slice_header_bits; i++ )  
   slice_reserved_flag[ i ] u(1) 
  slice_type ue(v) 
  if( output_flag_present_flag )  
   pic_output_flag u(1) 
  if( separate_colour_plane_flag == 1 )  
   colour_plane_id u(2) 
  if( ( nuh_layer_id > 0  &&   
    !poc_lsb_not_present_flag[ LayerIdxInVps[ nuh_layer_id ] ] )  || 
    ( nal_unit_type != IDR_W_RADL  &&  nal_unit_type != IDR_N_LP ) ) 
 
   slice_pic_order_cnt_lsb u(v) 
  if( nal_unit_type != IDR_W_RADL  &&  nal_unit_type != IDR_N_LP ) {  
   short_term_ref_pic_set_sps_flag u(1) 
   if( !short_term_ref_pic_set_sps_flag )  
    st_ref_pic_set( num_short_term_ref_pic_sets )  
   else if( num_short_term_ref_pic_sets > 1 )  
    short_term_ref_pic_set_idx u(v) 
   if( long_term_ref_pics_present_flag ) { 
       if( num_long_term_ref_pics_sps > 0 )  
     num_long_term_sps ue(v) 
    num_long_term_pics ue(v) 
    for( i = 0; i < num_long_term_sps + num_long_term_pics; i++ ) {  
     if( i < num_long_term_sps ) {  
      if( num_long_term_ref_pics_sps > 1 )  
       lt_idx_sps[ i ] u(v) 
     } else {  
      poc_lsb_lt[ i ] u(v) 
      used_by_curr_pic_lt_flag[ i ] u(1) 
     }  
     delta_poc_msb_present_flag[ i ] u(1) 
     if( delta_poc_msb_present_flag[ i ] )  
      delta_poc_msb_cycle_lt[ i ] ue(v) 
    }  
   }  
   if( sps_temporal_mvp_enabled_flag )  
    slice_temporal_mvp_enabled_flag u(1) 
  }  
  if( nuh_layer_id > 0  &&  !default_ref_layers_active_flag  &&   
      NumRefListLayers[ nuh_layer_id ] > 0 ) {  
 
   inter_layer_pred_enabled_flag u(1) 
   if( inter_layer_pred_enabled_flag  &&  NumRefListLayers[ nuh_layer_id ] > 1) {  
    if( !max_one_active_ref_layer_flag )  
     num_inter_layer_ref_pics_minus1 u(v) 
    if( NumActiveRefLayerPics != NumRefListLayers[ nuh_layer_id ] )  
     for( i = 0; i < NumActiveRefLayerPics; i++ )   
      inter_layer_pred_layer_idc[ i ] u(v) 
   }  
  }  
  if( inCmpPredAvailFlag )  
   in_comp_pred_flag u(1) 
  if( sample_adaptive_offset_enabled_flag ) {  
   slice_sao_luma_flag u(1) 
   if( ChromaArrayType != 0 )  
    slice_sao_chroma_flag u(1) 
  }  
  if( slice_type == P  ||  slice_type == B ) {  
   num_ref_idx_active_override_flag u(1) 
   if( num_ref_idx_active_override_flag ) {  
    num_ref_idx_l0_active_minus1 ue(v) 
    if( slice_type == B )  
     num_ref_idx_l1_active_minus1 ue(v) 
   }  
   if( lists_modification_present_flag  &&  NumPicTotalCurr > 1 )  
    ref_pic_lists_modification()  
   if( slice_type == B )  
    mvd_l1_zero_flag u(1) 
   if( cabac_init_present_flag ) 
       cabac_init_flag u(1) 
   if( slice_temporal_mvp_enabled_flag ) {  
    if( slice_type == B )  
     collocated_from_l0_flag u(1) 
    if( ( collocated_from_l0_flag  &&  num_ref_idx_l0_active_minus1 > 0 )  || 
     ( !collocated_from_l0_flag  &&  num_ref_idx_l1_active_minus1 > 0 ) ) 
 
     collocated_ref_idx ue(v) 
   }  
   if( ( weighted_pred_flag  &&  slice_type == P )  || 
     ( weighted_bipred_flag  &&  slice_type == B ) ) 
 
    pred_weight_table()  
   else if( !DepthFlag  &&  NumRefListLayers[ nuh_layer_id ] > 0 ) {  
    slice_ic_enabled_flag u(1) 
    if( slice_ic_enabled_flag )  
     slice_ic_disabled_merge_zero_idx_flag u(1) 
   }  
   five_minus_max_num_merge_cand ue(v) 
  }  
  slice_qp_delta se(v) 
  if( pps_slice_chroma_qp_offsets_present_flag ) {  
   slice_cb_qp_offset se(v) 
   slice_cr_qp_offset se(v) 
  }  
  if( chroma_qp_offset_list_enabled_flag )  
   cu_chroma_qp_offset_enabled_flag u(1) 
  if( deblocking_filter_override_enabled_flag )  
   deblocking_filter_override_flag u(1) 
  if( deblocking_filter_override_flag ) {  
   slice_deblocking_filter_disabled_flag u(1) 
   if( !slice_deblocking_filter_disabled_flag ) {  
    slice_beta_offset_div2 se(v) 
    slice_tc_offset_div2 se(v) 
   }  
  }  
  if( pps_loop_filter_across_slices_enabled_flag  &&   
   ( slice_sao_luma_flag  ||  slice_sao_chroma_flag  || 
    !slice_deblocking_filter_disabled_flag ) ) 
 
   slice_loop_filter_across_slices_enabled_flag u(1) 
  if( cp_in_slice_segment_header_flag[ ViewIdx ] )  
   for( m = 0; m < num_cp[ ViewIdx ]; m++ ) {  
    j = cp_ref_voi[ ViewIdx ][ m ]  
    cp_scale[ j ] se(v) 
    cp_off[ j ] se(v) 
    cp_inv_scale_plus_scale[ j ] se(v) 
    cp_inv_off_plus_off[ j ] se(v) 
   }  
 }  
 if( tiles_enabled_flag  ||  entropy_coding_sync_enabled_flag ) {  
  num_entry_point_offsets ue(v) 
    if( num_entry_point_offsets > 0 ) {  
   offset_len_minus1 ue(v) 
   for( i = 0; i < num_entry_point_offsets; i++ )  
    entry_point_offset_minus1[ i ] u(v) 
  }  
 }  
 if( slice_segment_header_extension_present_flag ) {  
  slice_segment_header_extension_length ue(v) 
  if( poc_reset_info_present_flag )  
   poc_reset_idc u(2) 
  if( poc_reset_idc != 0 )  
   poc_reset_period_id u(6) 
  if( poc_reset_idc == 3 ) {  
   full_poc_reset_flag u(1) 
   poc_lsb_val u(v) 
  }  
  if( !PocMsbValRequiredFlag  &&  vps_poc_lsb_aligned_flag )  
   poc_msb_cycle_val_present_flag u(1) 
  if( poc_msb_cycle_val_present_flag )  
   poc_msb_cycle_val ue(v) 
  while( more_data_in_slice_segment_header_extension() )  
   slice_segment_header_extension_data_bit u(1) 
 }  
 byte_alignment()  
}
    */
    public class SliceSegmentHeader : IItuSerializable
    {
        private byte first_slice_segment_in_pic_flag;
        public byte FirstSliceSegmentInPicFlag { get { return first_slice_segment_in_pic_flag; } set { first_slice_segment_in_pic_flag = value; } }
        private byte no_output_of_prior_pics_flag;
        public byte NoOutputOfPriorPicsFlag { get { return no_output_of_prior_pics_flag; } set { no_output_of_prior_pics_flag = value; } }
        private uint slice_pic_parameter_set_id;
        public uint SlicePicParameterSetId { get { return slice_pic_parameter_set_id; } set { slice_pic_parameter_set_id = value; } }
        private byte dependent_slice_segment_flag;
        public byte DependentSliceSegmentFlag { get { return dependent_slice_segment_flag; } set { dependent_slice_segment_flag = value; } }
        private uint slice_segment_address;
        public uint SliceSegmentAddress { get { return slice_segment_address; } set { slice_segment_address = value; } }
        private byte discardable_flag;
        public byte DiscardableFlag { get { return discardable_flag; } set { discardable_flag = value; } }
        private byte cross_layer_bla_flag;
        public byte CrossLayerBlaFlag { get { return cross_layer_bla_flag; } set { cross_layer_bla_flag = value; } }
        private byte[][] slice_reserved_flag;
        public byte[][] SliceReservedFlag { get { return slice_reserved_flag; } set { slice_reserved_flag = value; } }
        private uint slice_type;
        public uint SliceType { get { return slice_type; } set { slice_type = value; } }
        private byte pic_output_flag;
        public byte PicOutputFlag { get { return pic_output_flag; } set { pic_output_flag = value; } }
        private uint colour_plane_id;
        public uint ColourPlaneId { get { return colour_plane_id; } set { colour_plane_id = value; } }
        private uint slice_pic_order_cnt_lsb;
        public uint SlicePicOrderCntLsb { get { return slice_pic_order_cnt_lsb; } set { slice_pic_order_cnt_lsb = value; } }
        private byte short_term_ref_pic_set_sps_flag;
        public byte ShortTermRefPicSetSpsFlag { get { return short_term_ref_pic_set_sps_flag; } set { short_term_ref_pic_set_sps_flag = value; } }
        private StRefPicSet st_ref_pic_set;
        public StRefPicSet StRefPicSet { get { return st_ref_pic_set; } set { st_ref_pic_set = value; } }
        private uint short_term_ref_pic_set_idx;
        public uint ShortTermRefPicSetIdx { get { return short_term_ref_pic_set_idx; } set { short_term_ref_pic_set_idx = value; } }
        private uint num_long_term_sps;
        public uint NumLongTermSps { get { return num_long_term_sps; } set { num_long_term_sps = value; } }
        private uint num_long_term_pics;
        public uint NumLongTermPics { get { return num_long_term_pics; } set { num_long_term_pics = value; } }
        private uint[] lt_idx_sps;
        public uint[] LtIdxSps { get { return lt_idx_sps; } set { lt_idx_sps = value; } }
        private uint[] poc_lsb_lt;
        public uint[] PocLsbLt { get { return poc_lsb_lt; } set { poc_lsb_lt = value; } }
        private byte[] used_by_curr_pic_lt_flag;
        public byte[] UsedByCurrPicLtFlag { get { return used_by_curr_pic_lt_flag; } set { used_by_curr_pic_lt_flag = value; } }
        private byte[] delta_poc_msb_present_flag;
        public byte[] DeltaPocMsbPresentFlag { get { return delta_poc_msb_present_flag; } set { delta_poc_msb_present_flag = value; } }
        private uint[] delta_poc_msb_cycle_lt;
        public uint[] DeltaPocMsbCycleLt { get { return delta_poc_msb_cycle_lt; } set { delta_poc_msb_cycle_lt = value; } }
        private byte slice_temporal_mvp_enabled_flag;
        public byte SliceTemporalMvpEnabledFlag { get { return slice_temporal_mvp_enabled_flag; } set { slice_temporal_mvp_enabled_flag = value; } }
        private byte inter_layer_pred_enabled_flag;
        public byte InterLayerPredEnabledFlag { get { return inter_layer_pred_enabled_flag; } set { inter_layer_pred_enabled_flag = value; } }
        private uint num_inter_layer_ref_pics_minus1;
        public uint NumInterLayerRefPicsMinus1 { get { return num_inter_layer_ref_pics_minus1; } set { num_inter_layer_ref_pics_minus1 = value; } }
        private uint[] inter_layer_pred_layer_idc;
        public uint[] InterLayerPredLayerIdc { get { return inter_layer_pred_layer_idc; } set { inter_layer_pred_layer_idc = value; } }
        private byte in_comp_pred_flag;
        public byte InCompPredFlag { get { return in_comp_pred_flag; } set { in_comp_pred_flag = value; } }
        private byte slice_sao_luma_flag;
        public byte SliceSaoLumaFlag { get { return slice_sao_luma_flag; } set { slice_sao_luma_flag = value; } }
        private byte slice_sao_chroma_flag;
        public byte SliceSaoChromaFlag { get { return slice_sao_chroma_flag; } set { slice_sao_chroma_flag = value; } }
        private byte num_ref_idx_active_override_flag;
        public byte NumRefIdxActiveOverrideFlag { get { return num_ref_idx_active_override_flag; } set { num_ref_idx_active_override_flag = value; } }
        private uint num_ref_idx_l0_active_minus1;
        public uint NumRefIdxL0ActiveMinus1 { get { return num_ref_idx_l0_active_minus1; } set { num_ref_idx_l0_active_minus1 = value; } }
        private uint num_ref_idx_l1_active_minus1;
        public uint NumRefIdxL1ActiveMinus1 { get { return num_ref_idx_l1_active_minus1; } set { num_ref_idx_l1_active_minus1 = value; } }
        private RefPicListsModification ref_pic_lists_modification;
        public RefPicListsModification RefPicListsModification { get { return ref_pic_lists_modification; } set { ref_pic_lists_modification = value; } }
        private byte mvd_l1_zero_flag;
        public byte MvdL1ZeroFlag { get { return mvd_l1_zero_flag; } set { mvd_l1_zero_flag = value; } }
        private byte cabac_init_flag;
        public byte CabacInitFlag { get { return cabac_init_flag; } set { cabac_init_flag = value; } }
        private byte collocated_from_l0_flag;
        public byte CollocatedFromL0Flag { get { return collocated_from_l0_flag; } set { collocated_from_l0_flag = value; } }
        private uint collocated_ref_idx;
        public uint CollocatedRefIdx { get { return collocated_ref_idx; } set { collocated_ref_idx = value; } }
        private PredWeightTable pred_weight_table;
        public PredWeightTable PredWeightTable { get { return pred_weight_table; } set { pred_weight_table = value; } }
        private byte slice_ic_enabled_flag;
        public byte SliceIcEnabledFlag { get { return slice_ic_enabled_flag; } set { slice_ic_enabled_flag = value; } }
        private byte slice_ic_disabled_merge_zero_idx_flag;
        public byte SliceIcDisabledMergeZeroIdxFlag { get { return slice_ic_disabled_merge_zero_idx_flag; } set { slice_ic_disabled_merge_zero_idx_flag = value; } }
        private uint five_minus_max_num_merge_cand;
        public uint FiveMinusMaxNumMergeCand { get { return five_minus_max_num_merge_cand; } set { five_minus_max_num_merge_cand = value; } }
        private int slice_qp_delta;
        public int SliceQpDelta { get { return slice_qp_delta; } set { slice_qp_delta = value; } }
        private int slice_cb_qp_offset;
        public int SliceCbQpOffset { get { return slice_cb_qp_offset; } set { slice_cb_qp_offset = value; } }
        private int slice_cr_qp_offset;
        public int SliceCrQpOffset { get { return slice_cr_qp_offset; } set { slice_cr_qp_offset = value; } }
        private byte cu_chroma_qp_offset_enabled_flag;
        public byte CuChromaQpOffsetEnabledFlag { get { return cu_chroma_qp_offset_enabled_flag; } set { cu_chroma_qp_offset_enabled_flag = value; } }
        private byte deblocking_filter_override_flag;
        public byte DeblockingFilterOverrideFlag { get { return deblocking_filter_override_flag; } set { deblocking_filter_override_flag = value; } }
        private byte slice_deblocking_filter_disabled_flag;
        public byte SliceDeblockingFilterDisabledFlag { get { return slice_deblocking_filter_disabled_flag; } set { slice_deblocking_filter_disabled_flag = value; } }
        private int slice_beta_offset_div2;
        public int SliceBetaOffsetDiv2 { get { return slice_beta_offset_div2; } set { slice_beta_offset_div2 = value; } }
        private int slice_tc_offset_div2;
        public int SliceTcOffsetDiv2 { get { return slice_tc_offset_div2; } set { slice_tc_offset_div2 = value; } }
        private byte slice_loop_filter_across_slices_enabled_flag;
        public byte SliceLoopFilterAcrossSlicesEnabledFlag { get { return slice_loop_filter_across_slices_enabled_flag; } set { slice_loop_filter_across_slices_enabled_flag = value; } }
        private int[][] cp_scale;
        public int[][] CpScale { get { return cp_scale; } set { cp_scale = value; } }
        private int[][] cp_off;
        public int[][] CpOff { get { return cp_off; } set { cp_off = value; } }
        private int[][] cp_inv_scale_plus_scale;
        public int[][] CpInvScalePlusScale { get { return cp_inv_scale_plus_scale; } set { cp_inv_scale_plus_scale = value; } }
        private int[][] cp_inv_off_plus_off;
        public int[][] CpInvOffPlusOff { get { return cp_inv_off_plus_off; } set { cp_inv_off_plus_off = value; } }
        private uint num_entry_point_offsets;
        public uint NumEntryPointOffsets { get { return num_entry_point_offsets; } set { num_entry_point_offsets = value; } }
        private uint offset_len_minus1;
        public uint OffsetLenMinus1 { get { return offset_len_minus1; } set { offset_len_minus1 = value; } }
        private uint[] entry_point_offset_minus1;
        public uint[] EntryPointOffsetMinus1 { get { return entry_point_offset_minus1; } set { entry_point_offset_minus1 = value; } }
        private uint slice_segment_header_extension_length;
        public uint SliceSegmentHeaderExtensionLength { get { return slice_segment_header_extension_length; } set { slice_segment_header_extension_length = value; } }
        private uint poc_reset_idc;
        public uint PocResetIdc { get { return poc_reset_idc; } set { poc_reset_idc = value; } }
        private uint poc_reset_period_id;
        public uint PocResetPeriodId { get { return poc_reset_period_id; } set { poc_reset_period_id = value; } }
        private byte full_poc_reset_flag;
        public byte FullPocResetFlag { get { return full_poc_reset_flag; } set { full_poc_reset_flag = value; } }
        private uint poc_lsb_val;
        public uint PocLsbVal { get { return poc_lsb_val; } set { poc_lsb_val = value; } }
        private byte poc_msb_cycle_val_present_flag;
        public byte PocMsbCycleValPresentFlag { get { return poc_msb_cycle_val_present_flag; } set { poc_msb_cycle_val_present_flag = value; } }
        private uint poc_msb_cycle_val;
        public uint PocMsbCycleVal { get { return poc_msb_cycle_val; } set { poc_msb_cycle_val = value; } }
        private Dictionary<int, byte> slice_segment_header_extension_data_bit = new Dictionary<int, byte>();
        public Dictionary<int, byte> SliceSegmentHeaderExtensionDataBit { get { return slice_segment_header_extension_data_bit; } set { slice_segment_header_extension_data_bit = value; } }
        private ByteAlignment byte_alignment;
        public ByteAlignment ByteAlignment { get { return byte_alignment; } set { byte_alignment = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public SliceSegmentHeader()
        {

        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint m = 0;
            uint j = 0;
            int whileIndex = -1;
            size += stream.ReadUnsignedInt(size, 1, out this.first_slice_segment_in_pic_flag);

            if (nal_unit_type >= H265NALTypes.BLA_W_LP && nal_unit_type <= H265NALTypes.RSV_IRAP_VCL23)
            {
                size += stream.ReadUnsignedInt(size, 1, out this.no_output_of_prior_pics_flag);
            }
            size += stream.ReadUnsignedIntGolomb(size, out this.slice_pic_parameter_set_id);

            if (first_slice_segment_in_pic_flag == 0)
            {

                if (dependent_slice_segments_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.dependent_slice_segment_flag);
                }
                size += stream.ReadUnsignedIntVariable(size, slice_segment_address, out this.slice_segment_address);
            }

            if (dependent_slice_segment_flag == 0)
            {
                i = 0;

                if (num_extra_slice_header_bits > i)
                {
                    i++;
                    size += stream.ReadUnsignedInt(size, 1, out this.discardable_flag);
                }

                if (num_extra_slice_header_bits > i)
                {
                    i++;
                    size += stream.ReadUnsignedInt(size, 1, out this.cross_layer_bla_flag);
                }

                this.slice_reserved_flag = new byte[num_extra_slice_header_bits];
                for (; i < num_extra_slice_header_bits; i++)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.slice_reserved_flag[i][]);
                }
                size += stream.ReadUnsignedIntGolomb(size, out this.slice_type);

                if (output_flag_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.pic_output_flag);
                }

                if (separate_colour_plane_flag == 1)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.colour_plane_id);
                }

                if ((nuh_layer_id > 0 &&
    poc_lsb_not_present_flag[LayerIdxInVps[nuh_layer_id]] == 0) ||
    (nal_unit_type != H265NALTypes.IDR_W_RADL && nal_unit_type != H265NALTypes.IDR_N_LP))
                {
                    size += stream.ReadUnsignedIntVariable(size, slice_pic_order_cnt_lsb, out this.slice_pic_order_cnt_lsb);
                }

                if (nal_unit_type != H265NALTypes.IDR_W_RADL && nal_unit_type != H265NALTypes.IDR_N_LP)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.short_term_ref_pic_set_sps_flag);

                    if (short_term_ref_pic_set_sps_flag == 0)
                    {
                        this.st_ref_pic_set = new StRefPicSet(num_short_term_ref_pic_sets);
                        size += stream.ReadClass<StRefPicSet>(size, context, this.st_ref_pic_set);
                    }
                    else if (num_short_term_ref_pic_sets > 1)
                    {
                        size += stream.ReadUnsignedIntVariable(size, short_term_ref_pic_set_idx, out this.short_term_ref_pic_set_idx);
                    }

                    if (long_term_ref_pics_present_flag != 0)
                    {

                        if (num_long_term_ref_pics_sps > 0)
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.num_long_term_sps);
                        }
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_long_term_pics);

                        this.lt_idx_sps = new uint[num_long_term_sps + num_long_term_pics];
                        this.poc_lsb_lt = new uint[num_long_term_sps + num_long_term_pics];
                        this.used_by_curr_pic_lt_flag = new byte[num_long_term_sps + num_long_term_pics];
                        this.delta_poc_msb_present_flag = new byte[num_long_term_sps + num_long_term_pics];
                        this.delta_poc_msb_cycle_lt = new uint[num_long_term_sps + num_long_term_pics];
                        for (i = 0; i < num_long_term_sps + num_long_term_pics; i++)
                        {

                            if (i < num_long_term_sps)
                            {

                                if (num_long_term_ref_pics_sps > 1)
                                {
                                    size += stream.ReadUnsignedIntVariable(size, lt_idx_sps, out this.lt_idx_sps[i]);
                                }
                            }
                            else
                            {
                                size += stream.ReadUnsignedIntVariable(size, poc_lsb_lt, out this.poc_lsb_lt[i]);
                                size += stream.ReadUnsignedInt(size, 1, out this.used_by_curr_pic_lt_flag[i]);
                            }
                            size += stream.ReadUnsignedInt(size, 1, out this.delta_poc_msb_present_flag[i]);

                            if (delta_poc_msb_present_flag[i] != 0)
                            {
                                size += stream.ReadUnsignedIntGolomb(size, out this.delta_poc_msb_cycle_lt[i]);
                            }
                        }
                    }

                    if (sps_temporal_mvp_enabled_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.slice_temporal_mvp_enabled_flag);
                    }
                }

                if (nuh_layer_id > 0 && default_ref_layers_active_flag == 0 &&
      NumRefListLayers[nuh_layer_id] > 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.inter_layer_pred_enabled_flag);

                    if (inter_layer_pred_enabled_flag != 0 && NumRefListLayers[nuh_layer_id] > 1)
                    {

                        if (max_one_active_ref_layer_flag == 0)
                        {
                            size += stream.ReadUnsignedIntVariable(size, num_inter_layer_ref_pics_minus1, out this.num_inter_layer_ref_pics_minus1);
                        }

                        if (NumActiveRefLayerPics != NumRefListLayers[nuh_layer_id])
                        {

                            this.inter_layer_pred_layer_idc = new uint[NumActiveRefLayerPics];
                            for (i = 0; i < NumActiveRefLayerPics; i++)
                            {
                                size += stream.ReadUnsignedIntVariable(size, inter_layer_pred_layer_idc, out this.inter_layer_pred_layer_idc[i]);
                            }
                        }
                    }
                }

                if (inCmpPredAvailFlag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.in_comp_pred_flag);
                }

                if (sample_adaptive_offset_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.slice_sao_luma_flag);

                    if (ChromaArrayType != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.slice_sao_chroma_flag);
                    }
                }

                if (H265FrameTypes.IsP(slice_type) || H265FrameTypes.IsB(slice_type))
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.num_ref_idx_active_override_flag);

                    if (num_ref_idx_active_override_flag != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_idx_l0_active_minus1);

                        if (H265FrameTypes.IsB(slice_type))
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.num_ref_idx_l1_active_minus1);
                        }
                    }

                    if (lists_modification_present_flag != 0 && NumPicTotalCurr > 1)
                    {
                        this.ref_pic_lists_modification = new RefPicListsModification();
                        size += stream.ReadClass<RefPicListsModification>(size, context, this.ref_pic_lists_modification);
                    }

                    if (H265FrameTypes.IsB(slice_type))
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.mvd_l1_zero_flag);
                    }

                    if (cabac_init_present_flag != 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.cabac_init_flag);
                    }

                    if (slice_temporal_mvp_enabled_flag != 0)
                    {

                        if (H265FrameTypes.IsB(slice_type))
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.collocated_from_l0_flag);
                        }

                        if ((collocated_from_l0_flag != 0 && num_ref_idx_l0_active_minus1 > 0) ||
     (!collocated_from_l0_flag != 0 != 0 && num_ref_idx_l1_active_minus1 > 0))
                        {
                            size += stream.ReadUnsignedIntGolomb(size, out this.collocated_ref_idx);
                        }
                    }

                    if ((weighted_pred_flag != 0 && H265FrameTypes.IsP(slice_type)) ||
     (weighted_bipred_flag != 0 && H265FrameTypes.IsB(slice_type)))
                    {
                        this.pred_weight_table = new PredWeightTable();
                        size += stream.ReadClass<PredWeightTable>(size, context, this.pred_weight_table);
                    }
                    else if (DepthFlag == 0 && NumRefListLayers[nuh_layer_id] > 0)
                    {
                        size += stream.ReadUnsignedInt(size, 1, out this.slice_ic_enabled_flag);

                        if (slice_ic_enabled_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.slice_ic_disabled_merge_zero_idx_flag);
                        }
                    }
                    size += stream.ReadUnsignedIntGolomb(size, out this.five_minus_max_num_merge_cand);
                }
                size += stream.ReadSignedIntGolomb(size, out this.slice_qp_delta);

                if (pps_slice_chroma_qp_offsets_present_flag != 0)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.slice_cb_qp_offset);
                    size += stream.ReadSignedIntGolomb(size, out this.slice_cr_qp_offset);
                }

                if (chroma_qp_offset_list_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.cu_chroma_qp_offset_enabled_flag);
                }

                if (deblocking_filter_override_enabled_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.deblocking_filter_override_flag);
                }

                if (deblocking_filter_override_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.slice_deblocking_filter_disabled_flag);

                    if (slice_deblocking_filter_disabled_flag == 0)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.slice_beta_offset_div2);
                        size += stream.ReadSignedIntGolomb(size, out this.slice_tc_offset_div2);
                    }
                }

                if (pps_loop_filter_across_slices_enabled_flag != 0 &&
   (slice_sao_luma_flag != 0 || slice_sao_chroma_flag != 0 ||
    slice_deblocking_filter_disabled_flag == 0))
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.slice_loop_filter_across_slices_enabled_flag);
                }

                if (cp_in_slice_segment_header_flag[ViewIdx] != 0)
                {

                    this.cp_scale = new int[num_cp[ViewIdx]];
                    this.cp_off = new int[num_cp[ViewIdx]];
                    this.cp_inv_scale_plus_scale = new int[num_cp[ViewIdx]];
                    this.cp_inv_off_plus_off = new int[num_cp[ViewIdx]];
                    for (m = 0; m < num_cp[ViewIdx]; m++)
                    {
                        j = cp_ref_voi[ViewIdx][m];
                        size += stream.ReadSignedIntGolomb(size, out this.cp_scale[j][m]);
                        size += stream.ReadSignedIntGolomb(size, out this.cp_off[j][m]);
                        size += stream.ReadSignedIntGolomb(size, out this.cp_inv_scale_plus_scale[j][m]);
                        size += stream.ReadSignedIntGolomb(size, out this.cp_inv_off_plus_off[j][m]);
                    }
                }
            }

            if (tiles_enabled_flag != 0 || entropy_coding_sync_enabled_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.num_entry_point_offsets);

                if (num_entry_point_offsets > 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.offset_len_minus1);

                    this.entry_point_offset_minus1 = new uint[num_entry_point_offsets];
                    for (i = 0; i < num_entry_point_offsets; i++)
                    {
                        size += stream.ReadUnsignedIntVariable(size, entry_point_offset_minus1, out this.entry_point_offset_minus1[i]);
                    }
                }
            }

            if (slice_segment_header_extension_present_flag != 0)
            {
                size += stream.ReadUnsignedIntGolomb(size, out this.slice_segment_header_extension_length);

                if (poc_reset_info_present_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 2, out this.poc_reset_idc);
                }

                if (poc_reset_idc != 0)
                {
                    size += stream.ReadUnsignedInt(size, 6, out this.poc_reset_period_id);
                }

                if (poc_reset_idc == 3)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.full_poc_reset_flag);
                    size += stream.ReadUnsignedIntVariable(size, poc_lsb_val, out this.poc_lsb_val);
                }

                if (PocMsbValRequiredFlag == 0 && vps_poc_lsb_aligned_flag != 0)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.poc_msb_cycle_val_present_flag);
                }

                if (poc_msb_cycle_val_present_flag != 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.poc_msb_cycle_val);
                }

                while (more_data_in_slice_segment_header_extension())
                {
                    whileIndex++;

                    size += stream.ReadUnsignedInt(size, 1, whileIndex, this.slice_segment_header_extension_data_bit);
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
            uint m = 0;
            uint j = 0;
            int whileIndex = -1;
            size += stream.WriteUnsignedInt(1, this.first_slice_segment_in_pic_flag);

            if (nal_unit_type >= H265NALTypes.BLA_W_LP && nal_unit_type <= H265NALTypes.RSV_IRAP_VCL23)
            {
                size += stream.WriteUnsignedInt(1, this.no_output_of_prior_pics_flag);
            }
            size += stream.WriteUnsignedIntGolomb(this.slice_pic_parameter_set_id);

            if (first_slice_segment_in_pic_flag == 0)
            {

                if (dependent_slice_segments_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.dependent_slice_segment_flag);
                }
                size += stream.WriteUnsignedIntVariable(slice_segment_address, this.slice_segment_address);
            }

            if (dependent_slice_segment_flag == 0)
            {
                i = 0;

                if (num_extra_slice_header_bits > i)
                {
                    i++;
                    size += stream.WriteUnsignedInt(1, this.discardable_flag);
                }

                if (num_extra_slice_header_bits > i)
                {
                    i++;
                    size += stream.WriteUnsignedInt(1, this.cross_layer_bla_flag);
                }

                for (; i < num_extra_slice_header_bits; i++)
                {
                    size += stream.WriteUnsignedInt(1, this.slice_reserved_flag[i][]);
                }
                size += stream.WriteUnsignedIntGolomb(this.slice_type);

                if (output_flag_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.pic_output_flag);
                }

                if (separate_colour_plane_flag == 1)
                {
                    size += stream.WriteUnsignedInt(2, this.colour_plane_id);
                }

                if ((nuh_layer_id > 0 &&
    poc_lsb_not_present_flag[LayerIdxInVps[nuh_layer_id]] == 0) ||
    (nal_unit_type != H265NALTypes.IDR_W_RADL && nal_unit_type != H265NALTypes.IDR_N_LP))
                {
                    size += stream.WriteUnsignedIntVariable(slice_pic_order_cnt_lsb, this.slice_pic_order_cnt_lsb);
                }

                if (nal_unit_type != H265NALTypes.IDR_W_RADL && nal_unit_type != H265NALTypes.IDR_N_LP)
                {
                    size += stream.WriteUnsignedInt(1, this.short_term_ref_pic_set_sps_flag);

                    if (short_term_ref_pic_set_sps_flag == 0)
                    {
                        size += stream.WriteClass<StRefPicSet>(context, this.st_ref_pic_set);
                    }
                    else if (num_short_term_ref_pic_sets > 1)
                    {
                        size += stream.WriteUnsignedIntVariable(short_term_ref_pic_set_idx, this.short_term_ref_pic_set_idx);
                    }

                    if (long_term_ref_pics_present_flag != 0)
                    {

                        if (num_long_term_ref_pics_sps > 0)
                        {
                            size += stream.WriteUnsignedIntGolomb(this.num_long_term_sps);
                        }
                        size += stream.WriteUnsignedIntGolomb(this.num_long_term_pics);

                        for (i = 0; i < num_long_term_sps + num_long_term_pics; i++)
                        {

                            if (i < num_long_term_sps)
                            {

                                if (num_long_term_ref_pics_sps > 1)
                                {
                                    size += stream.WriteUnsignedIntVariable(lt_idx_sps[i], this.lt_idx_sps[i]);
                                }
                            }
                            else
                            {
                                size += stream.WriteUnsignedIntVariable(poc_lsb_lt[i], this.poc_lsb_lt[i]);
                                size += stream.WriteUnsignedInt(1, this.used_by_curr_pic_lt_flag[i]);
                            }
                            size += stream.WriteUnsignedInt(1, this.delta_poc_msb_present_flag[i]);

                            if (delta_poc_msb_present_flag[i] != 0)
                            {
                                size += stream.WriteUnsignedIntGolomb(this.delta_poc_msb_cycle_lt[i]);
                            }
                        }
                    }

                    if (sps_temporal_mvp_enabled_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.slice_temporal_mvp_enabled_flag);
                    }
                }

                if (nuh_layer_id > 0 && default_ref_layers_active_flag == 0 &&
      NumRefListLayers[nuh_layer_id] > 0)
                {
                    size += stream.WriteUnsignedInt(1, this.inter_layer_pred_enabled_flag);

                    if (inter_layer_pred_enabled_flag != 0 && NumRefListLayers[nuh_layer_id] > 1)
                    {

                        if (max_one_active_ref_layer_flag == 0)
                        {
                            size += stream.WriteUnsignedIntVariable(num_inter_layer_ref_pics_minus1, this.num_inter_layer_ref_pics_minus1);
                        }

                        if (NumActiveRefLayerPics != NumRefListLayers[nuh_layer_id])
                        {

                            for (i = 0; i < NumActiveRefLayerPics; i++)
                            {
                                size += stream.WriteUnsignedIntVariable(inter_layer_pred_layer_idc[i], this.inter_layer_pred_layer_idc[i]);
                            }
                        }
                    }
                }

                if (inCmpPredAvailFlag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.in_comp_pred_flag);
                }

                if (sample_adaptive_offset_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.slice_sao_luma_flag);

                    if (ChromaArrayType != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.slice_sao_chroma_flag);
                    }
                }

                if (H265FrameTypes.IsP(slice_type) || H265FrameTypes.IsB(slice_type))
                {
                    size += stream.WriteUnsignedInt(1, this.num_ref_idx_active_override_flag);

                    if (num_ref_idx_active_override_flag != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.num_ref_idx_l0_active_minus1);

                        if (H265FrameTypes.IsB(slice_type))
                        {
                            size += stream.WriteUnsignedIntGolomb(this.num_ref_idx_l1_active_minus1);
                        }
                    }

                    if (lists_modification_present_flag != 0 && NumPicTotalCurr > 1)
                    {
                        size += stream.WriteClass<RefPicListsModification>(context, this.ref_pic_lists_modification);
                    }

                    if (H265FrameTypes.IsB(slice_type))
                    {
                        size += stream.WriteUnsignedInt(1, this.mvd_l1_zero_flag);
                    }

                    if (cabac_init_present_flag != 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.cabac_init_flag);
                    }

                    if (slice_temporal_mvp_enabled_flag != 0)
                    {

                        if (H265FrameTypes.IsB(slice_type))
                        {
                            size += stream.WriteUnsignedInt(1, this.collocated_from_l0_flag);
                        }

                        if ((collocated_from_l0_flag != 0 && num_ref_idx_l0_active_minus1 > 0) ||
     (!collocated_from_l0_flag != 0 != 0 && num_ref_idx_l1_active_minus1 > 0))
                        {
                            size += stream.WriteUnsignedIntGolomb(this.collocated_ref_idx);
                        }
                    }

                    if ((weighted_pred_flag != 0 && H265FrameTypes.IsP(slice_type)) ||
     (weighted_bipred_flag != 0 && H265FrameTypes.IsB(slice_type)))
                    {
                        size += stream.WriteClass<PredWeightTable>(context, this.pred_weight_table);
                    }
                    else if (DepthFlag == 0 && NumRefListLayers[nuh_layer_id] > 0)
                    {
                        size += stream.WriteUnsignedInt(1, this.slice_ic_enabled_flag);

                        if (slice_ic_enabled_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.slice_ic_disabled_merge_zero_idx_flag);
                        }
                    }
                    size += stream.WriteUnsignedIntGolomb(this.five_minus_max_num_merge_cand);
                }
                size += stream.WriteSignedIntGolomb(this.slice_qp_delta);

                if (pps_slice_chroma_qp_offsets_present_flag != 0)
                {
                    size += stream.WriteSignedIntGolomb(this.slice_cb_qp_offset);
                    size += stream.WriteSignedIntGolomb(this.slice_cr_qp_offset);
                }

                if (chroma_qp_offset_list_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.cu_chroma_qp_offset_enabled_flag);
                }

                if (deblocking_filter_override_enabled_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.deblocking_filter_override_flag);
                }

                if (deblocking_filter_override_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.slice_deblocking_filter_disabled_flag);

                    if (slice_deblocking_filter_disabled_flag == 0)
                    {
                        size += stream.WriteSignedIntGolomb(this.slice_beta_offset_div2);
                        size += stream.WriteSignedIntGolomb(this.slice_tc_offset_div2);
                    }
                }

                if (pps_loop_filter_across_slices_enabled_flag != 0 &&
   (slice_sao_luma_flag != 0 || slice_sao_chroma_flag != 0 ||
    slice_deblocking_filter_disabled_flag == 0))
                {
                    size += stream.WriteUnsignedInt(1, this.slice_loop_filter_across_slices_enabled_flag);
                }

                if (cp_in_slice_segment_header_flag[ViewIdx] != 0)
                {

                    for (m = 0; m < num_cp[ViewIdx]; m++)
                    {
                        j = cp_ref_voi[ViewIdx][m];
                        size += stream.WriteSignedIntGolomb(this.cp_scale[j][m]);
                        size += stream.WriteSignedIntGolomb(this.cp_off[j][m]);
                        size += stream.WriteSignedIntGolomb(this.cp_inv_scale_plus_scale[j][m]);
                        size += stream.WriteSignedIntGolomb(this.cp_inv_off_plus_off[j][m]);
                    }
                }
            }

            if (tiles_enabled_flag != 0 || entropy_coding_sync_enabled_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.num_entry_point_offsets);

                if (num_entry_point_offsets > 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.offset_len_minus1);

                    for (i = 0; i < num_entry_point_offsets; i++)
                    {
                        size += stream.WriteUnsignedIntVariable(entry_point_offset_minus1[i], this.entry_point_offset_minus1[i]);
                    }
                }
            }

            if (slice_segment_header_extension_present_flag != 0)
            {
                size += stream.WriteUnsignedIntGolomb(this.slice_segment_header_extension_length);

                if (poc_reset_info_present_flag != 0)
                {
                    size += stream.WriteUnsignedInt(2, this.poc_reset_idc);
                }

                if (poc_reset_idc != 0)
                {
                    size += stream.WriteUnsignedInt(6, this.poc_reset_period_id);
                }

                if (poc_reset_idc == 3)
                {
                    size += stream.WriteUnsignedInt(1, this.full_poc_reset_flag);
                    size += stream.WriteUnsignedIntVariable(poc_lsb_val, this.poc_lsb_val);
                }

                if (PocMsbValRequiredFlag == 0 && vps_poc_lsb_aligned_flag != 0)
                {
                    size += stream.WriteUnsignedInt(1, this.poc_msb_cycle_val_present_flag);
                }

                if (poc_msb_cycle_val_present_flag != 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.poc_msb_cycle_val);
                }

                while (more_data_in_slice_segment_header_extension())
                {
                    whileIndex++;

                    size += stream.WriteUnsignedInt(1, whileIndex, this.slice_segment_header_extension_data_bit);
                }
            }
            size += stream.WriteClass<ByteAlignment>(context, this.byte_alignment);

            return size;
        }

    }

    /*
  

alternative_depth_info( payloadSize ) {  
 alternative_depth_info_cancel_flag u(1) 
 if( alternative_depth_info_cancel_flag == 0 ) {  
  depth_type u(2) 
  if( depth_type == 0 ) {  
   num_constituent_views_gvd_minus1 ue(v) 
   depth_present_gvd_flag u(1) 
   z_gvd_flag u(1) 
   intrinsic_param_gvd_flag u(1) 
   rotation_gvd_flag u(1) 
   translation_gvd_flag u(1) 
   if( z_gvd_flag )   
    for( i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++ ) {  
     sign_gvd_z_near_flag[ i ] u(1) 
     exp_gvd_z_near[ i ] u(7) 
          man_len_gvd_z_near_minus1[ i ] u(5) 
     man_gvd_z_near[ i ] u(v) 
     sign_gvd_z_far_flag[ i ] u(1) 
     exp_gvd_z_far[ i ] u(7) 
     man_len_gvd_z_far_minus1[ i ] u(5) 
     man_gvd_z_far[ i ] u(v) 
    }  
   if( intrinsic_param_gvd_flag ) {  
    prec_gvd_focal_length ue(v) 
    prec_gvd_principal_point ue(v) 
   }  
   if( rotation_gvd_flag )  
    prec_gvd_rotation_param ue(v) 
   if( translation_gvd_flag )  
    prec_gvd_translation_param ue(v) 
   for( i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++ ) {  
    if( intrinsic_param_gvd_flag ) {  
     sign_gvd_focal_length_x[ i ] u(1) 
     exp_gvd_focal_length_x[ i ] u(6) 
     man_gvd_focal_length_x[ i ] u(v) 
     sign_gvd_focal_length_y[ i ] u(1) 
     exp_gvd_focal_length_y[ i ] u(6) 
     man_gvd_focal_length_y[ i ] u(v) 
     sign_gvd_principal_point_x[ i ] u(1) 
     exp_gvd_principal_point_x[ i ] u(6) 
     man_gvd_principal_point_x[ i ] u(v) 
     sign_gvd_principal_point_y[ i ] u(1) 
     exp_gvd_principal_point_y[ i ] u(6) 
     man_gvd_principal_point_y[ i ] u(v) 
    }  
    if( rotation_gvd_flag )   
     for( j = 0; j < 3; j++ ) /* row *//*  
      for( k = 0; k < 3; k++ ) { /* column *//*  
       sign_gvd_r[ i ][ j ][ k ] u(1) 
       exp_gvd_r[ i ][ j ][ k ] u(6) 
       man_gvd_r[ i ][ j ][ k ] u(v) 
      }  
    if( translation_gvd_flag ) {  
     sign_gvd_t_x[ i ] u(1) 
     exp_gvd_t_x[ i ] u(6) 
     man_gvd_t_x[ i ] u(v) 
    }  
   }  
  }  
  if( depth_type == 1 ) {  
   min_offset_x_int se(v) 
   min_offset_x_frac u(8) 
   max_offset_x_int se(v)
      max_offset_x_frac u(8) 
   offset_y_present_flag u(1) 
   if( offset_y_present_flag ){  
    min_offset_y_int se(v) 
    min_offset_y_frac u(8) 
    max_offset_y_int se(v) 
    max_offset_y_frac u(8) 
   }  
   warp_map_size_present_flag u(1) 
   if( warp_map_size_present_flag ) {  
    warp_map_width_minus2  ue(v) 
    warp_map_height_minus2 ue(v) 
   }  
  }  
 }  
}
    */
    public class AlternativeDepthInfo : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private byte alternative_depth_info_cancel_flag;
        public byte AlternativeDepthInfoCancelFlag { get { return alternative_depth_info_cancel_flag; } set { alternative_depth_info_cancel_flag = value; } }
        private uint depth_type;
        public uint DepthType { get { return depth_type; } set { depth_type = value; } }
        private uint num_constituent_views_gvd_minus1;
        public uint NumConstituentViewsGvdMinus1 { get { return num_constituent_views_gvd_minus1; } set { num_constituent_views_gvd_minus1 = value; } }
        private byte depth_present_gvd_flag;
        public byte DepthPresentGvdFlag { get { return depth_present_gvd_flag; } set { depth_present_gvd_flag = value; } }
        private byte z_gvd_flag;
        public byte zGvdFlag { get { return z_gvd_flag; } set { z_gvd_flag = value; } }
        private byte intrinsic_param_gvd_flag;
        public byte IntrinsicParamGvdFlag { get { return intrinsic_param_gvd_flag; } set { intrinsic_param_gvd_flag = value; } }
        private byte rotation_gvd_flag;
        public byte RotationGvdFlag { get { return rotation_gvd_flag; } set { rotation_gvd_flag = value; } }
        private byte translation_gvd_flag;
        public byte TranslationGvdFlag { get { return translation_gvd_flag; } set { translation_gvd_flag = value; } }
        private byte[] sign_gvd_z_near_flag;
        public byte[] SignGvdzNearFlag { get { return sign_gvd_z_near_flag; } set { sign_gvd_z_near_flag = value; } }
        private uint[] exp_gvd_z_near;
        public uint[] ExpGvdzNear { get { return exp_gvd_z_near; } set { exp_gvd_z_near = value; } }
        private uint[] man_len_gvd_z_near_minus1;
        public uint[] ManLenGvdzNearMinus1 { get { return man_len_gvd_z_near_minus1; } set { man_len_gvd_z_near_minus1 = value; } }
        private uint[] man_gvd_z_near;
        public uint[] ManGvdzNear { get { return man_gvd_z_near; } set { man_gvd_z_near = value; } }
        private byte[] sign_gvd_z_far_flag;
        public byte[] SignGvdzFarFlag { get { return sign_gvd_z_far_flag; } set { sign_gvd_z_far_flag = value; } }
        private uint[] exp_gvd_z_far;
        public uint[] ExpGvdzFar { get { return exp_gvd_z_far; } set { exp_gvd_z_far = value; } }
        private uint[] man_len_gvd_z_far_minus1;
        public uint[] ManLenGvdzFarMinus1 { get { return man_len_gvd_z_far_minus1; } set { man_len_gvd_z_far_minus1 = value; } }
        private uint[] man_gvd_z_far;
        public uint[] ManGvdzFar { get { return man_gvd_z_far; } set { man_gvd_z_far = value; } }
        private uint prec_gvd_focal_length;
        public uint PrecGvdFocalLength { get { return prec_gvd_focal_length; } set { prec_gvd_focal_length = value; } }
        private uint prec_gvd_principal_point;
        public uint PrecGvdPrincipalPoint { get { return prec_gvd_principal_point; } set { prec_gvd_principal_point = value; } }
        private uint prec_gvd_rotation_param;
        public uint PrecGvdRotationParam { get { return prec_gvd_rotation_param; } set { prec_gvd_rotation_param = value; } }
        private uint prec_gvd_translation_param;
        public uint PrecGvdTranslationParam { get { return prec_gvd_translation_param; } set { prec_gvd_translation_param = value; } }
        private byte[] sign_gvd_focal_length_x;
        public byte[] SignGvdFocalLengthx { get { return sign_gvd_focal_length_x; } set { sign_gvd_focal_length_x = value; } }
        private uint[] exp_gvd_focal_length_x;
        public uint[] ExpGvdFocalLengthx { get { return exp_gvd_focal_length_x; } set { exp_gvd_focal_length_x = value; } }
        private uint[] man_gvd_focal_length_x;
        public uint[] ManGvdFocalLengthx { get { return man_gvd_focal_length_x; } set { man_gvd_focal_length_x = value; } }
        private byte[] sign_gvd_focal_length_y;
        public byte[] SignGvdFocalLengthy { get { return sign_gvd_focal_length_y; } set { sign_gvd_focal_length_y = value; } }
        private uint[] exp_gvd_focal_length_y;
        public uint[] ExpGvdFocalLengthy { get { return exp_gvd_focal_length_y; } set { exp_gvd_focal_length_y = value; } }
        private uint[] man_gvd_focal_length_y;
        public uint[] ManGvdFocalLengthy { get { return man_gvd_focal_length_y; } set { man_gvd_focal_length_y = value; } }
        private byte[] sign_gvd_principal_point_x;
        public byte[] SignGvdPrincipalPointx { get { return sign_gvd_principal_point_x; } set { sign_gvd_principal_point_x = value; } }
        private uint[] exp_gvd_principal_point_x;
        public uint[] ExpGvdPrincipalPointx { get { return exp_gvd_principal_point_x; } set { exp_gvd_principal_point_x = value; } }
        private uint[] man_gvd_principal_point_x;
        public uint[] ManGvdPrincipalPointx { get { return man_gvd_principal_point_x; } set { man_gvd_principal_point_x = value; } }
        private byte[] sign_gvd_principal_point_y;
        public byte[] SignGvdPrincipalPointy { get { return sign_gvd_principal_point_y; } set { sign_gvd_principal_point_y = value; } }
        private uint[] exp_gvd_principal_point_y;
        public uint[] ExpGvdPrincipalPointy { get { return exp_gvd_principal_point_y; } set { exp_gvd_principal_point_y = value; } }
        private uint[] man_gvd_principal_point_y;
        public uint[] ManGvdPrincipalPointy { get { return man_gvd_principal_point_y; } set { man_gvd_principal_point_y = value; } }
        private byte[][][] sign_gvd_r;
        public byte[][][] SignGvdr { get { return sign_gvd_r; } set { sign_gvd_r = value; } }
        private uint[][][] exp_gvd_r;
        public uint[][][] ExpGvdr { get { return exp_gvd_r; } set { exp_gvd_r = value; } }
        private uint[][][] man_gvd_r;
        public uint[][][] ManGvdr { get { return man_gvd_r; } set { man_gvd_r = value; } }
        private byte[] sign_gvd_t_x;
        public byte[] SignGvdtx { get { return sign_gvd_t_x; } set { sign_gvd_t_x = value; } }
        private uint[] exp_gvd_t_x;
        public uint[] ExpGvdtx { get { return exp_gvd_t_x; } set { exp_gvd_t_x = value; } }
        private uint[] man_gvd_t_x;
        public uint[] ManGvdtx { get { return man_gvd_t_x; } set { man_gvd_t_x = value; } }
        private int min_offset_x_int;
        public int MinOffsetxInt { get { return min_offset_x_int; } set { min_offset_x_int = value; } }
        private uint min_offset_x_frac;
        public uint MinOffsetxFrac { get { return min_offset_x_frac; } set { min_offset_x_frac = value; } }
        private int max_offset_x_int;
        public int MaxOffsetxInt { get { return max_offset_x_int; } set { max_offset_x_int = value; } }
        private uint max_offset_x_frac;
        public uint MaxOffsetxFrac { get { return max_offset_x_frac; } set { max_offset_x_frac = value; } }
        private byte offset_y_present_flag;
        public byte OffsetyPresentFlag { get { return offset_y_present_flag; } set { offset_y_present_flag = value; } }
        private int min_offset_y_int;
        public int MinOffsetyInt { get { return min_offset_y_int; } set { min_offset_y_int = value; } }
        private uint min_offset_y_frac;
        public uint MinOffsetyFrac { get { return min_offset_y_frac; } set { min_offset_y_frac = value; } }
        private int max_offset_y_int;
        public int MaxOffsetyInt { get { return max_offset_y_int; } set { max_offset_y_int = value; } }
        private uint max_offset_y_frac;
        public uint MaxOffsetyFrac { get { return max_offset_y_frac; } set { max_offset_y_frac = value; } }
        private byte warp_map_size_present_flag;
        public byte WarpMapSizePresentFlag { get { return warp_map_size_present_flag; } set { warp_map_size_present_flag = value; } }
        private uint warp_map_width_minus2;
        public uint WarpMapWidthMinus2 { get { return warp_map_width_minus2; } set { warp_map_width_minus2 = value; } }
        private uint warp_map_height_minus2;
        public uint WarpMapHeightMinus2 { get { return warp_map_height_minus2; } set { warp_map_height_minus2 = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public AlternativeDepthInfo(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            uint i = 0;
            uint j = 0;
            uint k = 0;
            size += stream.ReadUnsignedInt(size, 1, out this.alternative_depth_info_cancel_flag);

            if (alternative_depth_info_cancel_flag == 0)
            {
                size += stream.ReadUnsignedInt(size, 2, out this.depth_type);

                if (depth_type == 0)
                {
                    size += stream.ReadUnsignedIntGolomb(size, out this.num_constituent_views_gvd_minus1);
                    size += stream.ReadUnsignedInt(size, 1, out this.depth_present_gvd_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.z_gvd_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.intrinsic_param_gvd_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.rotation_gvd_flag);
                    size += stream.ReadUnsignedInt(size, 1, out this.translation_gvd_flag);

                    if (z_gvd_flag != 0)
                    {

                        this.sign_gvd_z_near_flag = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.exp_gvd_z_near = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.man_len_gvd_z_near_minus1 = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.man_gvd_z_near = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.sign_gvd_z_far_flag = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.exp_gvd_z_far = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.man_len_gvd_z_far_minus1 = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                        this.man_gvd_z_far = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                        for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_z_near_flag[i]);
                            size += stream.ReadUnsignedInt(size, 7, out this.exp_gvd_z_near[i]);
                            size += stream.ReadUnsignedInt(size, 5, out this.man_len_gvd_z_near_minus1[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_z_near, out this.man_gvd_z_near[i]);
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_z_far_flag[i]);
                            size += stream.ReadUnsignedInt(size, 7, out this.exp_gvd_z_far[i]);
                            size += stream.ReadUnsignedInt(size, 5, out this.man_len_gvd_z_far_minus1[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_z_far, out this.man_gvd_z_far[i]);
                        }
                    }

                    if (intrinsic_param_gvd_flag != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_focal_length);
                        size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_principal_point);
                    }

                    if (rotation_gvd_flag != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_rotation_param);
                    }

                    if (translation_gvd_flag != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.prec_gvd_translation_param);
                    }

                    this.sign_gvd_focal_length_x = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_focal_length_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_focal_length_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.sign_gvd_focal_length_y = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_focal_length_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_focal_length_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.sign_gvd_principal_point_x = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_principal_point_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_principal_point_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.sign_gvd_principal_point_y = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_principal_point_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_principal_point_y = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.sign_gvd_r = new byte[num_constituent_views_gvd_minus1 + 1 + 1][][];
                    this.exp_gvd_r = new uint[num_constituent_views_gvd_minus1 + 1 + 1][][];
                    this.man_gvd_r = new uint[num_constituent_views_gvd_minus1 + 1 + 1][][];
                    this.sign_gvd_t_x = new byte[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.exp_gvd_t_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    this.man_gvd_t_x = new uint[num_constituent_views_gvd_minus1 + 1 + 1];
                    for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                    {

                        if (intrinsic_param_gvd_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_focal_length_x[i]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_focal_length_x[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_focal_length_x, out this.man_gvd_focal_length_x[i]);
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_focal_length_y[i]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_focal_length_y[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_focal_length_y, out this.man_gvd_focal_length_y[i]);
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_principal_point_x[i]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_principal_point_x[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_principal_point_x, out this.man_gvd_principal_point_x[i]);
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_principal_point_y[i]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_principal_point_y[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_principal_point_y, out this.man_gvd_principal_point_y[i]);
                        }

                        if (rotation_gvd_flag != 0)
                        {

                            this.sign_gvd_r[i] = new byte[3][];
                            this.exp_gvd_r[i] = new uint[3][];
                            this.man_gvd_r[i] = new uint[3][];
                            for (j = 0; j < 3; j++)
                            {

                                this.sign_gvd_r[i][j] = new byte[3];
                                this.exp_gvd_r[i][j] = new uint[3];
                                this.man_gvd_r[i][j] = new uint[3];
                                for (k = 0; k < 3; k++)
                                {
                                    /*  column  */

                                    size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_r[i][j][k]);
                                    size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_r[i][j][k]);
                                    size += stream.ReadUnsignedIntVariable(size, man_gvd_r, out this.man_gvd_r[i][j][k]);
                                }
                            }
                        }

                        if (translation_gvd_flag != 0)
                        {
                            size += stream.ReadUnsignedInt(size, 1, out this.sign_gvd_t_x[i]);
                            size += stream.ReadUnsignedInt(size, 6, out this.exp_gvd_t_x[i]);
                            size += stream.ReadUnsignedIntVariable(size, man_gvd_t_x, out this.man_gvd_t_x[i]);
                        }
                    }
                }

                if (depth_type == 1)
                {
                    size += stream.ReadSignedIntGolomb(size, out this.min_offset_x_int);
                    size += stream.ReadUnsignedInt(size, 8, out this.min_offset_x_frac);
                    size += stream.ReadSignedIntGolomb(size, out this.max_offset_x_int);
                    size += stream.ReadUnsignedInt(size, 8, out this.max_offset_x_frac);
                    size += stream.ReadUnsignedInt(size, 1, out this.offset_y_present_flag);

                    if (offset_y_present_flag != 0)
                    {
                        size += stream.ReadSignedIntGolomb(size, out this.min_offset_y_int);
                        size += stream.ReadUnsignedInt(size, 8, out this.min_offset_y_frac);
                        size += stream.ReadSignedIntGolomb(size, out this.max_offset_y_int);
                        size += stream.ReadUnsignedInt(size, 8, out this.max_offset_y_frac);
                    }
                    size += stream.ReadUnsignedInt(size, 1, out this.warp_map_size_present_flag);

                    if (warp_map_size_present_flag != 0)
                    {
                        size += stream.ReadUnsignedIntGolomb(size, out this.warp_map_width_minus2);
                        size += stream.ReadUnsignedIntGolomb(size, out this.warp_map_height_minus2);
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
            size += stream.WriteUnsignedInt(1, this.alternative_depth_info_cancel_flag);

            if (alternative_depth_info_cancel_flag == 0)
            {
                size += stream.WriteUnsignedInt(2, this.depth_type);

                if (depth_type == 0)
                {
                    size += stream.WriteUnsignedIntGolomb(this.num_constituent_views_gvd_minus1);
                    size += stream.WriteUnsignedInt(1, this.depth_present_gvd_flag);
                    size += stream.WriteUnsignedInt(1, this.z_gvd_flag);
                    size += stream.WriteUnsignedInt(1, this.intrinsic_param_gvd_flag);
                    size += stream.WriteUnsignedInt(1, this.rotation_gvd_flag);
                    size += stream.WriteUnsignedInt(1, this.translation_gvd_flag);

                    if (z_gvd_flag != 0)
                    {

                        for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                        {
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_z_near_flag[i]);
                            size += stream.WriteUnsignedInt(7, this.exp_gvd_z_near[i]);
                            size += stream.WriteUnsignedInt(5, this.man_len_gvd_z_near_minus1[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_z_near[i], this.man_gvd_z_near[i]);
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_z_far_flag[i]);
                            size += stream.WriteUnsignedInt(7, this.exp_gvd_z_far[i]);
                            size += stream.WriteUnsignedInt(5, this.man_len_gvd_z_far_minus1[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_z_far[i], this.man_gvd_z_far[i]);
                        }
                    }

                    if (intrinsic_param_gvd_flag != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.prec_gvd_focal_length);
                        size += stream.WriteUnsignedIntGolomb(this.prec_gvd_principal_point);
                    }

                    if (rotation_gvd_flag != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.prec_gvd_rotation_param);
                    }

                    if (translation_gvd_flag != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.prec_gvd_translation_param);
                    }

                    for (i = 0; i <= num_constituent_views_gvd_minus1 + 1; i++)
                    {

                        if (intrinsic_param_gvd_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_focal_length_x[i]);
                            size += stream.WriteUnsignedInt(6, this.exp_gvd_focal_length_x[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_focal_length_x[i], this.man_gvd_focal_length_x[i]);
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_focal_length_y[i]);
                            size += stream.WriteUnsignedInt(6, this.exp_gvd_focal_length_y[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_focal_length_y[i], this.man_gvd_focal_length_y[i]);
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_principal_point_x[i]);
                            size += stream.WriteUnsignedInt(6, this.exp_gvd_principal_point_x[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_principal_point_x[i], this.man_gvd_principal_point_x[i]);
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_principal_point_y[i]);
                            size += stream.WriteUnsignedInt(6, this.exp_gvd_principal_point_y[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_principal_point_y[i], this.man_gvd_principal_point_y[i]);
                        }

                        if (rotation_gvd_flag != 0)
                        {

                            for (j = 0; j < 3; j++)
                            {

                                for (k = 0; k < 3; k++)
                                {
                                    /*  column  */

                                    size += stream.WriteUnsignedInt(1, this.sign_gvd_r[i][j][k]);
                                    size += stream.WriteUnsignedInt(6, this.exp_gvd_r[i][j][k]);
                                    size += stream.WriteUnsignedIntVariable(man_gvd_r[i][j][k], this.man_gvd_r[i][j][k]);
                                }
                            }
                        }

                        if (translation_gvd_flag != 0)
                        {
                            size += stream.WriteUnsignedInt(1, this.sign_gvd_t_x[i]);
                            size += stream.WriteUnsignedInt(6, this.exp_gvd_t_x[i]);
                            size += stream.WriteUnsignedIntVariable(man_gvd_t_x[i], this.man_gvd_t_x[i]);
                        }
                    }
                }

                if (depth_type == 1)
                {
                    size += stream.WriteSignedIntGolomb(this.min_offset_x_int);
                    size += stream.WriteUnsignedInt(8, this.min_offset_x_frac);
                    size += stream.WriteSignedIntGolomb(this.max_offset_x_int);
                    size += stream.WriteUnsignedInt(8, this.max_offset_x_frac);
                    size += stream.WriteUnsignedInt(1, this.offset_y_present_flag);

                    if (offset_y_present_flag != 0)
                    {
                        size += stream.WriteSignedIntGolomb(this.min_offset_y_int);
                        size += stream.WriteUnsignedInt(8, this.min_offset_y_frac);
                        size += stream.WriteSignedIntGolomb(this.max_offset_y_int);
                        size += stream.WriteUnsignedInt(8, this.max_offset_y_frac);
                    }
                    size += stream.WriteUnsignedInt(1, this.warp_map_size_present_flag);

                    if (warp_map_size_present_flag != 0)
                    {
                        size += stream.WriteUnsignedIntGolomb(this.warp_map_width_minus2);
                        size += stream.WriteUnsignedIntGolomb(this.warp_map_height_minus2);
                    }
                }
            }

            return size;
        }

    }

    /*
  

green_metadata(payloadSize) {
    green_metadata_type 5 u(8)
    if (green_metadata_type == 0) {
        period_type 5 u(8)

        if (period_type == 2) {
            num_seconds 5 u(16)
        }
        else if (period_type == 3) {
            num_pictures 5 u(16)
        }

        percent_non_zero_macroblocks 5 u(8)
        percent_intra_coded_macroblocks 5 u(8)
        percent_six_tap_filtering 5 u(8)
        percent_alpha_point_deblocking_instance 5 u(8)
    }
    else if (green_metadata_type == 1) {
        xsd_metric_type 5 u(8)
        xsd_metric_value 5 u(16)
    }
}
    */
    public class GreenMetadata : IItuSerializable
    {
        private uint payloadSize;
        public uint PayloadSize { get { return payloadSize; } set { payloadSize = value; } }
        private uint green_metadata_type;
        public uint GreenMetadataType { get { return green_metadata_type; } set { green_metadata_type = value; } }
        private uint period_type;
        public uint PeriodType { get { return period_type; } set { period_type = value; } }
        private uint num_seconds;
        public uint NumSeconds { get { return num_seconds; } set { num_seconds = value; } }
        private uint num_pictures;
        public uint NumPictures { get { return num_pictures; } set { num_pictures = value; } }
        private uint percent_non_zero_macroblocks;
        public uint PercentNonZeroMacroblocks { get { return percent_non_zero_macroblocks; } set { percent_non_zero_macroblocks = value; } }
        private uint percent_intra_coded_macroblocks;
        public uint PercentIntraCodedMacroblocks { get { return percent_intra_coded_macroblocks; } set { percent_intra_coded_macroblocks = value; } }
        private uint percent_six_tap_filtering;
        public uint PercentSixTapFiltering { get { return percent_six_tap_filtering; } set { percent_six_tap_filtering = value; } }
        private uint percent_alpha_point_deblocking_instance;
        public uint PercentAlphaPointDeblockingInstance { get { return percent_alpha_point_deblocking_instance; } set { percent_alpha_point_deblocking_instance = value; } }
        private uint xsd_metric_type;
        public uint XsdMetricType { get { return xsd_metric_type; } set { xsd_metric_type = value; } }
        private uint xsd_metric_value;
        public uint XsdMetricValue { get { return xsd_metric_value; } set { xsd_metric_value = value; } }

        public int HasMoreRbspData { get; set; }
        public int[] ReadNextBits { get; set; }

        public GreenMetadata(uint payloadSize)
        {
            this.payloadSize = payloadSize;
        }

        public ulong Read(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadUnsignedInt(size, 8, out this.green_metadata_type);

            if (green_metadata_type == 0)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.period_type);

                if (period_type == 2)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.num_seconds);
                }
                else if (period_type == 3)
                {
                    size += stream.ReadUnsignedInt(size, 16, out this.num_pictures);
                }
                size += stream.ReadUnsignedInt(size, 8, out this.percent_non_zero_macroblocks);
                size += stream.ReadUnsignedInt(size, 8, out this.percent_intra_coded_macroblocks);
                size += stream.ReadUnsignedInt(size, 8, out this.percent_six_tap_filtering);
                size += stream.ReadUnsignedInt(size, 8, out this.percent_alpha_point_deblocking_instance);
            }
            else if (green_metadata_type == 1)
            {
                size += stream.ReadUnsignedInt(size, 8, out this.xsd_metric_type);
                size += stream.ReadUnsignedInt(size, 16, out this.xsd_metric_value);
            }

            return size;
        }

        public ulong Write(IItuContext context, ItuStream stream)
        {
            ulong size = 0;

            size += stream.WriteUnsignedInt(8, this.green_metadata_type);

            if (green_metadata_type == 0)
            {
                size += stream.WriteUnsignedInt(8, this.period_type);

                if (period_type == 2)
                {
                    size += stream.WriteUnsignedInt(16, this.num_seconds);
                }
                else if (period_type == 3)
                {
                    size += stream.WriteUnsignedInt(16, this.num_pictures);
                }
                size += stream.WriteUnsignedInt(8, this.percent_non_zero_macroblocks);
                size += stream.WriteUnsignedInt(8, this.percent_intra_coded_macroblocks);
                size += stream.WriteUnsignedInt(8, this.percent_six_tap_filtering);
                size += stream.WriteUnsignedInt(8, this.percent_alpha_point_deblocking_instance);
            }
            else if (green_metadata_type == 1)
            {
                size += stream.WriteUnsignedInt(8, this.xsd_metric_type);
                size += stream.WriteUnsignedInt(16, this.xsd_metric_value);
            }

            return size;
        }

    }

}
