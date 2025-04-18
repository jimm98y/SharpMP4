namespace SharpH264
{
    public interface IItuContext { }

    public class H264FrameTypes
    {
        // mod 5
        public const uint P = 0; // 5
        public const uint B = 1; // 6
        public const uint I = 2; // 7
        public const uint SP = 3; // 8
        public const uint SI = 4; // 9

        public static bool IsP(uint value) { return value % 5 == P; }
        public static bool IsB(uint value) { return value % 5 == B; }
        public static bool IsI(uint value) { return value % 5 == I; }
        public static bool IsSP(uint value) { return value % 5 == SP; }
        public static bool IsSI(uint value) { return value % 5 == SI; }
    }

    public class H264Constants
    {
        public const uint Extended_ISO = 255;
        public const uint Extended_SAR = 255;
    }

    public class H264NALTypes
    {
        public const uint UNSPECIFIED0 = 0;
        public const uint SLICE = 1;
        public const uint DPA = 2;
        public const uint DPB = 3;
        public const uint DPC = 4;
        public const uint IDR_SLICE = 5;
        public const uint SEI = 6;
        public const uint SPS = 7;
        public const uint PPS = 8;
        public const uint AUD = 9;
        public const uint END_OF_SEQUENCE = 10;
        public const uint END_OF_STREAM = 11;
        public const uint FILLER_DATA = 12;
        public const uint SPS_EXT = 13;
        public const uint PREFIX_NAL = 14;
        public const uint SUBSET_SPS = 15;
        public const uint DPS = 16;
        public const uint RESERVED0 = 17;
        public const uint RESERVED1 = 18;
        public const uint SLICE_NOPARTITIONING = 19;
        public const uint SLICE_EXT = 20;
        public const uint SLICE_EXT_VIEW_COMPONENT = 21;
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
}
