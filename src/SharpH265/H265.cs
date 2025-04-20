namespace SharpH265
{
    public class H265FrameTypes
    {
        public const uint P = 0; 
        public const uint B = 1; 
        public const uint I = 2; 

        public static bool IsP(uint value) { return value == P; }
        public static bool IsB(uint value) { return value == B; }
        public static bool IsI(uint value) { return value == I; }
    }

    public class H265Constants
    {
        public const uint EXTENDED_ISO = 255;
        public const uint EXTENDED_SAR = 255;
    }
}
