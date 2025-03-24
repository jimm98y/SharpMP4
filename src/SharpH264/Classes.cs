
using System;

namespace SharpH264
{
    public class FrameTypes
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

    public class H264Helpers
    {
        public static uint GetChromaArrayType()
        {
            // inferred from SPS
            /*
Depending on the value of separate_colour_plane_flag, the value of the variable ChromaArrayType is assigned as follows:
– If separate_colour_plane_flag is equal to 0, ChromaArrayType is set equal to chroma_format_idc.
– Otherwise (separate_colour_plane_flag is equal to 1), ChromaArrayType is set equal to 0.
             */
            throw new NotImplementedException();
        }

        public static uint GetIdrPicFlag()
        {
            throw new NotImplementedException();
        }

        public static uint GetValue(string field)
        {
            throw new NotImplementedException();
        }
        public static uint[] GetArray(string field)
        {
            throw new NotImplementedException();
        }

        public static uint[,] GetArray2(string field)
        {
            throw new NotImplementedException();
        }

        public static uint GetAllViewsPairedFlag()
        {
            /*
            The variable AllViewsPairedFlag is derived as follows:
AllViewsPairedFlag = 1 for( i = 1; i <= num_views_minus1; i++ ) AllViewsPairedFlag = ( AllViewsPairedFlag && depth_view_present_flag[ i ] && (J-9) texture_view_present_flag[ i ] )
             */
            throw new NotImplementedException();
        }
    }
}
