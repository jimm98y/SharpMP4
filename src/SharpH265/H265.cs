using SharpH26X;

namespace SharpH265
{
    public class H265Constants
    {
        public const uint EXTENDED_ISO = 255;
        public const uint EXTENDED_SAR = 255;
    }

    public class H265FrameTypes
    {
        public const uint P = 0;
        public const uint B = 1;
        public const uint I = 2;

        public static bool IsP(uint value) { return value == P; }
        public static bool IsB(uint value) { return value == B; }
        public static bool IsI(uint value) { return value == I; }
    }

    public class H265NALTypes
    {
        public const uint TRAIL_N = 0;                 // Coded slice segment of a non-TSA, non-STSA trailing picture
        public const uint TRAIL_R = 1;                 // Coded slice segment of a non-TSA, non-STSA trailing picture
        public const uint TSA_N = 2;                   // Coded slice segment of a TSA picture
        public const uint TSA_R = 3;                   // Coded slice segment of a TSA picture
        public const uint STSA_N = 4;                  // Coded slice segment of a STSA picture
        public const uint STSA_R = 5;                  // Coded slice segment of a STSA picture
        public const uint RADL_N = 6;                  // Coded slice segment of a RADL picture
        public const uint RADL_R = 7;                  // Coded slice segment of a RADL picture
        public const uint RASL_N = 8;                  // Coded slice segment of a RASL picture
        public const uint RASL_R = 9;                  // Coded slice segment of a RASL picture

        public const uint RSV_VCL_N10 = 10;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R11 = 11;            // Reserved non-IRAP sub-layer reference VCL NAL unit types
        public const uint RSV_VCL_N12 = 12;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R13 = 13;            // Reserved non-IRAP sub-layer reference VCL NAL unit types
        public const uint RSV_VCL_N14 = 14;            // Reserved non-IRAP SLNR VCL NAL unit types
        public const uint RSV_VCL_R15 = 15;            // Reserved non-IRAP sub-layer reference VCL NAL unit types

        public const uint BLA_W_LP = 16;               // Coded slice segment of a BLA picture
        public const uint BLA_W_RADL = 17;             // Coded slice segment of a BLA picture
        public const uint BLA_N_LP = 18;               // Coded slice segment of a BLA picture
        public const uint IDR_W_RADL = 19;             // Coded slice segment of an IDR picture
        public const uint IDR_N_LP = 20;               // Coded slice segment of an IDR picture
        public const uint CRA_NUT = 21;                // Coded slice segment of a CRA picture

        public const uint RSV_IRAP_VCL22 = 22;         // Reserved IRAP VCL NAL unit types
        public const uint RSV_IRAP_VCL23 = 23;         // Reserved IRAP VCL NAL unit types
        public const uint RSV_VCL24 = 24;              // Reserved VCL NAL unit types
        public const uint RSV_VCL25 = 25;              // Reserved VCL NAL unit types
        public const uint RSV_VCL26 = 26;              // Reserved VCL NAL unit types
        public const uint RSV_VCL27 = 27;              // Reserved VCL NAL unit types
        public const uint RSV_VCL28 = 28;              // Reserved VCL NAL unit types
        public const uint RSV_VCL29 = 29;              // Reserved VCL NAL unit types
        public const uint RSV_VCL30 = 30;              // Reserved VCL NAL unit types
        public const uint RSV_VCL31 = 31;              // Reserved VCL NAL unit types

        public const uint VPS_NUT = 32;                // Video parameter set
        public const uint SPS_NUT = 33;                // Sequence parameter set
        public const uint PPS_NUT = 34;                // Picture parameter set
        public const uint AUD_NUT = 35;                // Access unit delimiter
        public const uint EOS_NUT = 36;                // End of sequence
        public const uint EOB_NUT = 37;                // End of stream
        public const uint FD_NUT = 38;                 // Filler data
        public const uint PREFIX_SEI_NUT = 39;         // Prefix SEI
        public const uint SUFFIX_SEI_NUT = 40;         // Suffix SEI

        public const uint RSV_NVCL41 = 41;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL42 = 42;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL43 = 43;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL44 = 44;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL45 = 45;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL46 = 46;             // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL47 = 47;             // Reserved non-VCL NAL unit types
        public const uint UNSPEC48 = 48;               // Unspecified NAL unit types
        public const uint UNSPEC49 = 49;               // Unspecified NAL unit types
        public const uint UNSPEC50 = 50;               // Unspecified NAL unit types
        public const uint UNSPEC51 = 51;               // Unspecified NAL unit types
        public const uint UNSPEC52 = 52;               // Unspecified NAL unit types
        public const uint UNSPEC53 = 53;               // Unspecified NAL unit types
        public const uint UNSPEC54 = 54;               // Unspecified NAL unit types
        public const uint UNSPEC55 = 55;               // Unspecified NAL unit types
        public const uint UNSPEC56 = 56;               // Unspecified NAL unit types
        public const uint UNSPEC57 = 57;               // Unspecified NAL unit types
        public const uint UNSPEC58 = 58;               // Unspecified NAL unit types
        public const uint UNSPEC59 = 59;               // Unspecified NAL unit types
        public const uint UNSPEC60 = 60;               // Unspecified NAL unit types
        public const uint UNSPEC61 = 61;               // Unspecified NAL unit types
        public const uint UNSPEC62 = 62;               // Unspecified NAL unit types
        public const uint UNSPEC63 = 63;               // Unspecified NAL unit types
    }

    public partial class H265Context
    {
        public uint NumLayerSets { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint FirstAddLayerSetIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint LastAddLayerSetIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumNegativePics { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumPositivePics { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] UsedByCurrPicS0 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] UsedByCurrPicS1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] UsedByCurrPicLt { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint CurrRpsIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }


        public uint NumPicTotalCurr { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        // VPS
        public uint[] LayerIdxInVps { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] NumLayersInIdList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] LayerSetLayerIdList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] MaxSubLayersInLayerSetMinus1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        // Slice Header
        public int TemporalId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int DepthFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int ViewIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] curCmpLIds { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] inCmpRefViewIdcs { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] RefPicLayerId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] IdRefListLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] CpPresentFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] ViewCompLayerPresentFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] ViewCompLayerId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] ScalabilityId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] DependencyId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] AuxId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumRefListLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint numCurCmpLIds { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint cpAvailableFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint allRefCmpLayersAvailFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint inCmpPredAvailFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint MaxLayersMinus1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] ViewOrderIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] DepthLayerFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] IdDirectRefLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] DependencyFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[] layerIdInListFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] IdRefLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] IdPredictedLayer { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public int[][] TreePartitionLayerIdList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumDirectRefLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumRefLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumPredictedLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] MaxTemporalId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NumViews { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] ViewOIdxList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NumIndependentLayers { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumLayersInTreePartition { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NumActiveRefLayerPics { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }


        // TODO
        public uint[][][] BspSchedCnt { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[][] NecessaryLayerFlag { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] RefPicList1 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumOutputLayersInOutputLayerSet { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] OlsHighestOutputLayerId { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint VclInitialArrivalDelayPresent { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint NalInitialArrivalDelayPresent { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint CurrPic { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint RefRpsIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] RefPicList0 { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] OlsIdxToLsIdx { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint[] NumDeltaPocs { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint PicSizeInCtbsY { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public uint CpbCnt { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public uint PicOrderCnt(uint picID)
        {
            throw new NotImplementedException();
        }

        public uint PicLayerId(uint picID)
        {
            throw new NotImplementedException();
        }

        public void OnVpsMaxLayersMinus1()
        {
            var vps_max_layers_minus1 = VideoParameterSetRbsp.VpsMaxLayersMinus1;
            MaxLayersMinus1 = Math.Min(62, vps_max_layers_minus1);
        }

        public void OnNuhTemporalIdPlus1()
        {
            var nuh_temporal_id_plus1 = NalHeader.NalUnitHeader.NuhTemporalIdPlus1;
            TemporalId = (int)nuh_temporal_id_plus1 - 1;
        }

        public void OnDimensionId() // F-3
        {
            var dimension_id = VideoParameterSetRbsp.VpsExtension.DimensionId;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;
            var scalability_mask_flag = VideoParameterSetRbsp.VpsExtension?.ScalabilityMaskFlag;

            NumViews = 1;
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                uint lId = layer_id_in_nuh[i];
                for (int smIdx = 0, j = 0; smIdx < 16; smIdx++)
                {
                    if (scalability_mask_flag[smIdx] != 0)
                        ScalabilityId[i][smIdx] = (int)dimension_id[i][j++];
                    else
                        ScalabilityId[i][smIdx] = 0;
                }
                DepthLayerFlag[lId] = ScalabilityId[i][0];
                ViewOrderIdx[lId] = ScalabilityId[i][1];
                DependencyId[lId] = ScalabilityId[i][2];
                AuxId[lId] = ScalabilityId[i][3];
                if (i > 0)
                {
                    uint newViewFlag = 1;
                    for (int j = 0; j < i; j++)
                        if (ViewOrderIdx[lId] == ViewOrderIdx[layer_id_in_nuh[j]])
                            newViewFlag = 0;
                    NumViews += newViewFlag;
                }
            }
        }

        public void OnNumAddLayerSets()
        {
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var num_add_layer_sets = VideoParameterSetRbsp.VpsExtension.NumAddLayerSets;

            NumLayerSets = vps_num_layer_sets_minus1 + 1 + num_add_layer_sets; // F-7
            if(num_add_layer_sets > 0)
            {
                // F-8
                FirstAddLayerSetIdx = vps_num_layer_sets_minus1 + 1;
                LastAddLayerSetIdx = FirstAddLayerSetIdx + num_add_layer_sets - 1;
            }
        }

        public void OnHighestLayerIdxPlus1(uint i) // F-9
        {
            var vps_num_layer_sets_minus1 = VideoParameterSetRbsp.VpsNumLayerSetsMinus1;
            var highest_layer_idx_plus1 = VideoParameterSetRbsp.VpsExtension.HighestLayerIdxPlus1;

            int layerNum = 0;
            uint lsIdx = vps_num_layer_sets_minus1 + 1 + i;
            for (int treeIdx = 1; treeIdx < NumIndependentLayers; treeIdx++)
                for (int layerCnt = 0; layerCnt < highest_layer_idx_plus1[i][treeIdx]; layerCnt++)
                    LayerSetLayerIdList[lsIdx][layerNum++] = TreePartitionLayerIdList[treeIdx][layerCnt];
            NumLayersInIdList[lsIdx] = layerNum;
        }

        public void OnDirectDependencyType() 
        {
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;

            // I-7
            int idx = 0;
            ViewOIdxList[idx++] = 0;
            for (int i = 1; i <= MaxLayersMinus1; i++)
            {
                uint lId = layer_id_in_nuh[i];
                int newViewFlag = 1;
                for (int j = 0; j < i; j++)
                    if (ViewOrderIdx[layer_id_in_nuh[i]] == ViewOrderIdx[layer_id_in_nuh[j]])
                        newViewFlag = 0;
                if (newViewFlag != 0)
                    ViewOIdxList[idx++] = (uint)ViewOrderIdx[lId];
            }

            // I-8
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                uint iNuhLId = layer_id_in_nuh[i];
                NumRefListLayers[iNuhLId] = 0;
                for (int j = 0; j < NumDirectRefLayers[iNuhLId]; j++)
                {
                    int jNuhLId = IdDirectRefLayer[iNuhLId][j];
                    if (DepthLayerFlag[iNuhLId] == DepthLayerFlag[jNuhLId])
                        IdRefListLayer[iNuhLId][NumRefListLayers[iNuhLId]++] = jNuhLId;
                }
            }

            // I-9
            for (int depFlag = 0; depFlag <= 1; depFlag++)
            {
                for (int i = 0; i < NumViews; i++)
                {
                    uint iViewOIdx = ViewOIdxList[i];
                    int layerId = -1;
                    for (int j = 0; j <= MaxLayersMinus1; j++)
                    {
                        int jNuhLId = (int)layer_id_in_nuh[j];
                        if (DepthLayerFlag[jNuhLId] == depFlag && ViewOrderIdx[jNuhLId] == iViewOIdx && DependencyId[jNuhLId] == 0 && AuxId[jNuhLId] == 0)
                            layerId = jNuhLId;

                    }
                    ViewCompLayerPresentFlag[iViewOIdx][depFlag] = (layerId != -1) ? 1u : 0u;
                    ViewCompLayerId[iViewOIdx][depFlag] = layerId;
                }
            }            
        }

        public void OnDirectDependencyFlag()
        {
            var direct_dependency_flag = VideoParameterSetRbsp.VpsExtension.DirectDependencyFlag;
            var layer_id_in_nuh = VideoParameterSetRbsp.VpsExtension.LayerIdInNuh;

            uint k = 0;
            
            // F-4
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                for (int j = 0; j <= MaxLayersMinus1; j++)
                {
                    DependencyFlag[i][j] = direct_dependency_flag[i][j];
                    for (k = 0; k < i; k++)
                        if (direct_dependency_flag[i][k] != 0 && DependencyFlag[k][j] != 0)
                            DependencyFlag[i][j] = 1;
                }
            }

            // F-5
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                int iNuhLId = (int)layer_id_in_nuh[i];
                int j = 0, d = 0, r = 0, p = 0;
                for (j = 0, d = 0, r = 0, p = 0; j <= MaxLayersMinus1; j++)
                {
                    int jNuhLid = (int)layer_id_in_nuh[j];
                    if (direct_dependency_flag[i][j] != 0)
                        IdDirectRefLayer[iNuhLId][d++] = jNuhLid;
                    if (DependencyFlag[i][j] != 0)
                        IdRefLayer[iNuhLId][r++] = jNuhLid;
                    if (DependencyFlag[j][i] != 0)
                        IdPredictedLayer[iNuhLId][p++] = jNuhLid;
                }
                NumDirectRefLayers[iNuhLId] = (uint)d;
                NumRefLayers[iNuhLId] = (uint)r;
                NumPredictedLayers[iNuhLId] = (uint)p;
            }

            k = 0;

            // F-6
            for (int i = 0; i <= 63; i++)
                layerIdInListFlag[i] = 0;
            for (int i = 0; i <= MaxLayersMinus1; i++)
            {
                int iNuhLId = (int)layer_id_in_nuh[i];
                if (NumDirectRefLayers[iNuhLId] == 0)
                {
                    uint h = 0;
                    TreePartitionLayerIdList[k][0] = iNuhLId;
                    for (int j = 0; j < NumPredictedLayers[iNuhLId]; j++)
                    {
                        int predLId = IdPredictedLayer[iNuhLId][j];
                        if (layerIdInListFlag[predLId] == 0)
                        {
                            TreePartitionLayerIdList[k][h++] = predLId;
                            layerIdInListFlag[predLId] = 1;
                        }
                    }
                    NumLayersInTreePartition[k++] = h;
                }
            }
            NumIndependentLayers = k;
        }

        public void OnInterLayerPredLayerIdc()
        {
            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;
            var inter_layer_pred_layer_idc = SliceSegmentLayerRbsp.SliceSegmentHeader.InterLayerPredLayerIdc;

            // RefPicLayerId
            for (int i = 0; i < NumActiveRefLayerPics; i++)
            {
                RefPicLayerId[i] = IdRefListLayer[nuh_layer_id][inter_layer_pred_layer_idc[i]];
            }
        }

        public void OnSliceHeaderRbsp()
        {
            var nuh_layer_id = NalHeader.NalUnitHeader.NuhLayerId;

            var direct_dependency_flag = VideoParameterSetRbsp.VpsExtension.DirectDependencyFlag;
            var sub_layers_vps_max_minus1 = VideoParameterSetRbsp.VpsExtension.SubLayersVpsMaxMinus1;
            var max_tid_il_ref_pics_plus1 = VideoParameterSetRbsp.VpsExtension.MaxTidIlRefPicsPlus1;

            var vsp_mc_enabled_flag = SeqParameterSetRbsp.Sps3dExtension.VspMcEnabledFlag;
            var dbbp_enabled_flag = SeqParameterSetRbsp.Sps3dExtension.DbbpEnabledFlag;
            var depth_ref_enabled_flag = SeqParameterSetRbsp.Sps3dExtension.DepthRefEnabledFlag;
            var intra_contour_enabled_flag = SeqParameterSetRbsp.Sps3dExtension.IntraContourEnabledFlag;
            var cqt_cu_part_pred_enabled_flag = SeqParameterSetRbsp.Sps3dExtension.CqtCuPartPredEnabledFlag;
            var tex_mc_enabled_flag = SeqParameterSetRbsp.Sps3dExtension.TexMcEnabledFlag;

            DepthFlag = DepthLayerFlag[nuh_layer_id];
            ViewIdx = ViewOrderIdx[nuh_layer_id];

            curCmpLIds = DepthFlag != 0 ? new int[] { (int)nuh_layer_id } : RefPicLayerId;
            numCurCmpLIds = DepthFlag != 0 ? 1 : NumActiveRefLayerPics;

            cpAvailableFlag = 1;
            allRefCmpLayersAvailFlag = 1;
            for (int i = 0; i < numCurCmpLIds; i++)
            {
                inCmpRefViewIdcs[i] = ViewOrderIdx[curCmpLIds[i]];
                if (CpPresentFlag[ViewIdx][inCmpRefViewIdcs[i]] == 0)
                {
                    cpAvailableFlag = 0;
                }

                int refCmpCurLIdAvailFlag = 0;

                if (ViewCompLayerPresentFlag[inCmpRefViewIdcs[i]][DepthFlag != 0 ? 0 : 1] == 1)
                {
                    uint j = LayerIdxInVps[ViewCompLayerId[inCmpRefViewIdcs[i]][DepthFlag != 0 ? 0 : 1]];

                    if (direct_dependency_flag[LayerIdxInVps[nuh_layer_id]][j] == 1 &&
                        sub_layers_vps_max_minus1[j] >= TemporalId &&
                        (TemporalId == 0 || max_tid_il_ref_pics_plus1[j][LayerIdxInVps[nuh_layer_id]] > TemporalId))
                    {
                        refCmpCurLIdAvailFlag = 1;
                    }
                }

                if (refCmpCurLIdAvailFlag == 0)
                {
                    allRefCmpLayersAvailFlag = 0;
                }
            }

            if (allRefCmpLayersAvailFlag == 0)
            {
                inCmpPredAvailFlag = 0;
            }
            else
            {
                if (DepthFlag == 0)
                    inCmpPredAvailFlag = (vsp_mc_enabled_flag[DepthFlag] != 0 || dbbp_enabled_flag[DepthFlag] != 0 || depth_ref_enabled_flag[DepthFlag] != 0) ? 1u : 0u; // I-17
                else
                    inCmpPredAvailFlag = (intra_contour_enabled_flag[DepthFlag] != 0 || cqt_cu_part_pred_enabled_flag[DepthFlag] != 0 || tex_mc_enabled_flag[DepthFlag] != 0) ? 1u : 0u; // I-18
            }
        }

        public void OnCpRefVoi() // I-12
        {
            var num_cp = VideoParameterSetRbsp.Vps3dExtension.NumCp;
            var cp_ref_voi = VideoParameterSetRbsp.Vps3dExtension.CpRefVoi;

            for (int n = 1; n < NumViews; n++)
            {
                uint i = ViewOIdxList[n];
                for (int m = 0; m < num_cp[i]; m++)
                    CpPresentFlag[i][cp_ref_voi[i][m]] = 1;
            }
        }

        public void OnListEntryL0() // F-56, replacing 7-57
        {
            var pps_curr_pic_ref_enabled_flag = PicParameterSetRbsp.PpsSccExtension.PpsCurrPicRefEnabledFlag;
            var num_long_term_sps = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermSps;
            var num_long_term_pics = SliceSegmentLayerRbsp.SliceSegmentHeader.NumLongTermPics;
            var nal_unit_type = NalHeader.NalUnitHeader.NalUnitType;

            NumPicTotalCurr = 0;
            if (nal_unit_type != H265NALTypes.IDR_W_RADL && nal_unit_type != H265NALTypes.IDR_N_LP)
            {
                for (int i = 0; i < NumNegativePics[CurrRpsIdx]; i++)
                    if (UsedByCurrPicS0[CurrRpsIdx][i] != 0)
                        NumPicTotalCurr++;
                for (int i = 0; i < NumPositivePics[CurrRpsIdx]; i++)
                    if (UsedByCurrPicS1[CurrRpsIdx][i] != 0)
                        NumPicTotalCurr++;
                for (int i = 0; i < num_long_term_sps + num_long_term_pics; i++)
                    if (UsedByCurrPicLt[i] != 0)
                        NumPicTotalCurr++;
}
            if (pps_curr_pic_ref_enabled_flag != 0)
                NumPicTotalCurr++;
            NumPicTotalCurr += NumActiveRefLayerPics;
        }
    }
}
