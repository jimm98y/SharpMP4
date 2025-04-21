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

    public static class H265Helpers
    {
        public static uint[] GetNumDirectRefLayers(IItuContext context)
        {
            throw new NotImplementedException();
        }

        public static uint GetCpbCnt(IItuContext context)
        {
            throw new NotImplementedException();
        }

        public static uint GetNumPicTotalCurr(IItuContext context)
        {
            throw new NotImplementedException();
            /*
            StRefPicSet stRpsIdx;
            if(((H265Context)context).SliceSegmentLayerRbsp.SliceSegmentHeader.ShortTermRefPicSetSpsFlag == 0)
            {
                stRpsIdx = ((H265Context)context).SliceSegmentLayerRbsp.SliceSegmentHeader.StRefPicSet;
            }
            else
            {
                stRpsIdx = ((H265Context)context).SeqParameterSetRbsp.StRefPicSet[short_term_ref_pic_set_idx];
            }

            if (inter_ref_pic_set_prediction_flag == 0)
            {
                NumNegativePics[stRpsIdx] = num_negative_pics;
            }
            else
            {
                int RefRpsIdx = stRpsIdx - (delta_idx_minus1 + 1);
                int deltaRps = (1 - 2 * delta_rps_sign) * (abs_delta_rps_minus1 + 1);

                int i = 0;
                for (int j = NumPositivePics[RefRpsIdx] - 1; j >= 0; j--)
                {
                    int dPoc = DeltaPocS1[RefRpsIdx][j] + deltaRps;
                    if (dPoc < 0 && use_delta_flag[NumNegativePics[RefRpsIdx] + j])
                    {
                        DeltaPocS0[stRpsIdx][i] = dPoc;
                        UsedByCurrPicS0[stRpsIdx][i++] = used_by_curr_pic_flag[NumNegativePics[RefRpsIdx] + j];
                    }
                }
                if (deltaRps < 0 && use_delta_flag[NumDeltaPocs[RefRpsIdx]])
                {
                    DeltaPocS0[stRpsIdx][i] = deltaRps;
                    UsedByCurrPicS0[stRpsIdx][i++] = used_by_curr_pic_flag[NumDeltaPocs[RefRpsIdx]];
                }
                for (int j = 0; j < NumNegativePics[RefRpsIdx]; j++)
                {
                    int dPoc = DeltaPocS0[RefRpsIdx][j] + deltaRps;
                    if (dPoc < 0 && use_delta_flag[j])
                    {
                        DeltaPocS0[stRpsIdx][i] = dPoc;
                        UsedByCurrPicS0[stRpsIdx][i++] = used_by_curr_pic_flag[j];
                    }
                }
                NumNegativePics[stRpsIdx] = i;
                i = 0;
                for (int j = NumNegativePics[RefRpsIdx] - 1; j >= 0; j--)
                {
                    int dPoc = DeltaPocS0[RefRpsIdx][j] + deltaRps;
                    if (dPoc > 0 && use_delta_flag[j])
                    {
                        DeltaPocS1[stRpsIdx][i] = dPoc;
                        UsedByCurrPicS1[stRpsIdx][i++] = used_by_curr_pic_flag[j];
                    }
                }
                if (deltaRps > 0 && use_delta_flag[NumDeltaPocs[RefRpsIdx]])
                {
                    DeltaPocS1[stRpsIdx][i] = deltaRps;
                    UsedByCurrPicS1[stRpsIdx][i++] = used_by_curr_pic_flag[NumDeltaPocs[RefRpsIdx]];
                }
                for (int j = 0; j < NumPositivePics[RefRpsIdx]; j++)
                {
                    int dPoc = DeltaPocS1[RefRpsIdx][j] + deltaRps;
                    if (dPoc > 0 && use_delta_flag[NumNegativePics[RefRpsIdx] + j])
                    {
                        DeltaPocS1[stRpsIdx][i] = dPoc;
                        UsedByCurrPicS1[stRpsIdx][i++] = used_by_curr_pic_flag[NumNegativePics[RefRpsIdx] + j];
                    }
                }
                NumPositivePics[stRpsIdx] = i;
            }

            uint NumPicTotalCurr = 0;
            for (int i = 0; i < NumNegativePics[CurrRpsIdx]; i++)
                if (UsedByCurrPicS0[CurrRpsIdx][i])
                    NumPicTotalCurr++;
            for (int i = 0; i < NumPositivePics[CurrRpsIdx]; i++)
                if (UsedByCurrPicS1[CurrRpsIdx][i])
                    NumPicTotalCurr++;
            for (int i = 0; i < num_long_term_sps + num_long_term_pics; i++)
                if (UsedByCurrPicLt[i])
                    NumPicTotalCurr++;
            if (pps_curr_pic_ref_enabled_flag)
                NumPicTotalCurr++;
            return NumPicTotalCurr;
            */
        }

        public static int[] GetMaxSubLayersInLayerSetMinus1(IItuContext context)
        {
            int[] LayerIdxInVps = new int[Math.Min(62, ((H265Context)context).VideoParameterSetRbsp.VpsMaxLayersMinus1) + 1];
            for (int i = 0; i <= Math.Min(62, ((H265Context)context).VideoParameterSetRbsp.VpsMaxLayersMinus1); i++)
            {
                LayerIdxInVps[((H265Context)context).VideoParameterSetAnnexfRbsp.VpsExtension.LayerIdInNuh[i]] = i;
            }

            int[] NumLayersInIdList = new int[((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1];
            NumLayersInIdList[0] = 1;
            int[][] MyLayerSetLayerIdList = new int[((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1][];
            MyLayerSetLayerIdList[0] = new int[((H265Context)context).VideoParameterSetRbsp.VpsMaxLayerId + 1];
            MyLayerSetLayerIdList[0][0] = 0;
            int n = 0;
            for (int i = 1; i <= ((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1; i++)
            {
                for (int m = 0; m <= ((H265Context)context).VideoParameterSetRbsp.VpsMaxLayerId; m++)
                {
                    MyLayerSetLayerIdList[i] = new int[((H265Context)context).VideoParameterSetRbsp.VpsMaxLayerId + 1];
                    if (((H265Context)context).VideoParameterSetRbsp.LayerIdIncludedFlag[i][m] != 0)
                        MyLayerSetLayerIdList[i][n++] = m;
                }
                NumLayersInIdList[i] = n;
            }

            int[] MaxSubLayersInLayerSetMinus1 = new int[(((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1 + ((H265Context)context).VideoParameterSetAnnexfRbsp.VpsExtension.NumAddLayerSets)];
            for (int i = 0; i < (((H265Context)context).VideoParameterSetRbsp.VpsNumLayerSetsMinus1 + 1 + ((H265Context)context).VideoParameterSetAnnexfRbsp.VpsExtension.NumAddLayerSets); i++)
            {
                int maxSlMinus1 = 0;
                for (int k = 0; k < NumLayersInIdList[i]; k++)
                {
                    int lId = MyLayerSetLayerIdList[i][k];
                    maxSlMinus1 = (int)Math.Max(maxSlMinus1, ((H265Context)context).VideoParameterSetAnnexfRbsp.VpsExtension.SubLayersVpsMaxMinus1[LayerIdxInVps[lId]]);
                }
                MaxSubLayersInLayerSetMinus1[i] = maxSlMinus1;
            }

            return MaxSubLayersInLayerSetMinus1;
        }
    }
}
