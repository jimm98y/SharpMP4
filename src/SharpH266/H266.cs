using System;

namespace SharpH266
{
    public class H266NALTypes
    {          
        public const uint TRAIL_NUT = 0;                     // Coded slice of a trailing picture or subpicture
        public const uint STSA_NUT = 1;                      // Coded slice of an STSA picture or subpicture
        public const uint RADL_NUT = 2;                      // Coded slice of a random access picture or subpicture
        public const uint RASL_NUT = 3;                      // Coded slice of a random access picture or subpicture
        
        public const uint RSV_VCL_4 = 4;                     // Reserved for future use
        public const uint RSV_VCL_5 = 5;                     // Reserved for future use
        public const uint RSV_VCL_6 = 6;                     // Reserved for future use

        public const uint IDR_W_RADL = 7;                    // Coded slice of an IDR picture or subpicture
        public const uint IDR_N_LP = 8;                      // Coded slice of an IDR picture or subpicture
        public const uint CRA_NUT = 9;                       // Coded slice of a CRA picture or subpicture
        public const uint GDR_NUT = 10;                      // Coded slice of a GDR picture or subpicture
                                                             // 
        public const uint RSV_IRAP_11 = 11;                  // Reserved IRAP VCL NAL unit
        public const uint OPI_NUT = 12;                      // Operating Point Information
        public const uint DCI_NUT = 13;                      // Decoding Capability Information
        public const uint VPS_NUT = 14;                      // Video parameter set
        public const uint SPS_NUT = 15;                      // Sequence parameter set
        public const uint PPS_NUT = 16;                      // Picture parameter set
        public const uint PREFIX_APS_NUT = 17;               // Adaptation Parameter Set
        public const uint SUFFIX_APS_NUT = 18;               // Adaptation Parameter Set
        public const uint PH_NUT = 19;                       // Picture Header
        public const uint AUD_NUT = 20;                      // AU Delimiter
        public const uint EOS_NUT = 21;                      // End of Sequence
        public const uint EOB_NUT = 22;                      // End of Bitstream
        public const uint PREFIX_SEI_NUT = 23;               // Supplemental enhancement information
        public const uint SUFFIX_SEI_NUT = 24;               // Supplemental enhancement information
        public const uint FD_NUT = 25;                       // Filler Data

        public const uint RSV_NVCL_26 = 26;                  // Reserved non-VCL NAL unit types
        public const uint RSV_NVCL_27 = 27;                  // Reserved non-VCL NAL unit types

        public const uint UNSPEC_28 = 28;                    // Unspecified
        public const uint UNSPEC_29 = 29;                    // Unspecified
        public const uint UNSPEC_30 = 30;                    // Unspecified
        public const uint UNSPEC_31 = 31;                    // Unspecified
    }

    public partial class H266Context
    {
        public SeiPayload SeiPayload { get; set; }

        public void SetSeiPayload(SeiPayload payload)
        {
            if (SeiPayload == null)
            {
                SeiPayload = payload;
            }

            throw new NotImplementedException();
        }
    }
}
