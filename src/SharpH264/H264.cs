namespace SharpH264
{
    public class H264Constants
    {
        public const uint Extended_ISO = 255;
        public const uint Extended_SAR = 255;
    }

    public class H264FrameTypes
    {
        public const uint P = 0; // 5
        public const uint B = 1; // 6
        public const uint I = 2; // 7
        public const uint SP = 3; // 8
        public const uint SI = 4; // 9

        // mod 5
        public static bool IsP(uint value) { return value % 5 == P; }
        public static bool IsB(uint value) { return value % 5 == B; }
        public static bool IsI(uint value) { return value % 5 == I; }
        public static bool IsSP(uint value) { return value % 5 == SP; }
        public static bool IsSI(uint value) { return value % 5 == SI; }
    }

    public class H264NALTypes
    {
        public const uint UNSPECIFIED0 = 0;                 // Unspecified
        public const uint SLICE = 1;                        // Slice of non-IDR picture
        public const uint DPA = 2;                          // Slice data partition A 
        public const uint DPB = 3;                          // Slice data partition B
        public const uint DPC = 4;                          // Slice data partition C
        public const uint IDR_SLICE = 5;                    // Immediate decoder refresh (IDR) slice
        public const uint SEI = 6;                          // Supplemental enhancement information (SEI)
        public const uint SPS = 7;                          // Sequence parameter set
        public const uint PPS = 8;                          // Picture parameter set
        public const uint AUD = 9;                          // Access unit delimiter
        public const uint END_OF_SEQUENCE = 10;             // End of sequence
        public const uint END_OF_STREAM = 11;               // End of stream
        public const uint FILLER_DATA = 12;                 // Filler data
        public const uint SPS_EXT = 13;                     // SPS extension
        public const uint PREFIX_NAL = 14;                  // Prefix NAL unit
        public const uint SUBSET_SPS = 15;                  // Subset sequence parameter set
        public const uint DPS = 16;                         // Depth parameter set
        public const uint RESERVED0 = 17;
        public const uint RESERVED1 = 18;
        public const uint SLICE_NOPARTITIONING = 19;        // Slice of an auxiliary coded picture without partitioning
        public const uint SLICE_EXT = 20;                   // Slice extension
        public const uint SLICE_EXT_VIEW_COMPONENT = 21;    // Slice extension for depth view component or 3D-AVC texture view component
        public const uint RESERVED2 = 22;
        public const uint RESERVED3 = 23;
        public const uint UNSPECIFIED1 = 23;
        public const uint UNSPECIFIED2 = 24;
        public const uint UNSPECIFIED3 = 25;
        public const uint UNSPECIFIED4 = 26;
        public const uint UNSPECIFIED5 = 27;
        public const uint UNSPECIFIED6 = 28;
        public const uint UNSPECIFIED7 = 28;
        public const uint UNSPECIFIED8 = 30;
        public const uint UNSPECIFIED9 = 31;
    }

    public partial class H264Context
    {
        // TODO
    }
}
