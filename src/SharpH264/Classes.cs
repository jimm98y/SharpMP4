
using System;
using System.Linq;

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

    public class H264Context
    {
        public NalUnit NalHeader { get; set; }
        public SeqParameterSetRbsp Sps { get; set; }
        public PicParameterSetRbsp Pps { get; set; }
        public SubsetSeqParameterSetRbsp SubsetSps { get; set; }
        public SeqParameterSetExtensionRbsp SpsExtension { get; set; }
        public DepthParameterSetRbsp Dps { get; set; }
        public AccessUnitDelimiterRbsp Au { get; set; }
        public SeiRbsp Sei { get; set; }
    }

    public class H264Helpers
    {
        public static uint[,] GetArray2(string field)
        {
            switch(field)
            {
                case "ZNearSign":
                    throw new NotImplementedException();
                case "ZNearExp":
                    throw new NotImplementedException();
                case "ZNearMantissa":
                    throw new NotImplementedException();
                case "ZNearManLen":
                    throw new NotImplementedException();
                case "ZFarSign":
                    throw new NotImplementedException();
                case "ZFarExp":
                    throw new NotImplementedException();
                case "ZFarMantissa":
                    throw new NotImplementedException();
                case "ZFarManLen":
                    throw new NotImplementedException();
                case "DMinSign":
                    throw new NotImplementedException();
                case "DMinExp":
                    throw new NotImplementedException();
                case "DMinMantissa":
                    throw new NotImplementedException();
                case "DMinManLen":
                    throw new NotImplementedException();
                case "DMaxSign":
                    throw new NotImplementedException();
                case "DMaxExp":
                    throw new NotImplementedException();
                case "DMaxMantissa":
                    throw new NotImplementedException();
                case "DMaxManLen":
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
