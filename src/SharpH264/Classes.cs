
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class MbTypes
    {
        public const uint I_NxN = 0;
        public const uint I_16x16_0_0_0 = 1;
        public const uint I_16x16_1_0_0 = 2;
        public const uint I_16x16_2_0_0 = 3;
        public const uint I_16x16_3_0_0 = 4;
        public const uint I_16x16_0_1_0 = 5;
        public const uint I_16x16_1_1_0 = 6;
        public const uint I_16x16_2_1_0 = 7;
        public const uint I_16x16_3_1_0 = 8;
        public const uint I_16x16_0_2_0 = 9;
        public const uint I_16x16_1_2_0 = 10;
        public const uint I_16x16_2_2_0 = 11;
        public const uint I_16x16_3_2_0 = 12;
        public const uint I_16x16_0_0_1 = 13;
        public const uint I_16x16_1_0_1 = 14;
        public const uint I_16x16_2_0_1 = 15;
        public const uint I_16x16_3_0_1 = 16;
        public const uint I_16x16_0_1_1 = 17;
        public const uint I_16x16_1_1_1 = 18;
        public const uint I_16x16_2_1_1 = 19;
        public const uint I_16x16_3_1_1 = 20;
        public const uint I_16x16_0_2_1 = 21;
        public const uint I_16x16_1_2_1 = 22;
        public const uint I_16x16_2_2_1 = 23;
        public const uint I_16x16_3_2_1 = 24;
        public const uint I_PCM = 25;

        public const uint P_L0_16x16 = 0;
        public const uint P_L0_L0_16x8 = 1;
        public const uint P_L0_L0_8x16 = 2;
        public const uint P_8x8 = 3;
        public const uint P_8x8ref0 = 4;

        public const uint B_Direct_16x16 = 0;
        public const uint B_L0_16x16 = 1;
        public const uint B_L1_16x16 = 2;
        public const uint B_Bi_16x16 = 3;
        public const uint B_L0_L0_16x8 = 4;
        public const uint B_L0_L0_8x16 = 5;
        public const uint B_L1_L1_16x8 = 6;
        public const uint B_L1_L1_8x16 = 7;
        public const uint B_L0_L1_16x8 = 8;
        public const uint B_L0_L1_8x16 = 9;
        public const uint B_L1_L0_16x8 = 10;
        public const uint B_L1_L0_8x16 = 11;
        public const uint B_L0_Bi_16x8 = 12;
        public const uint B_L0_Bi_8x16 = 13;
        public const uint B_L1_Bi_16x8 = 14;
        public const uint B_L1_Bi_8x16 = 15;
        public const uint B_Bi_L0_16x8 = 16;
        public const uint B_Bi_L0_8x16 = 17;
        public const uint B_Bi_L1_16x8 = 18;
        public const uint B_Bi_L1_8x16 = 19;
        public const uint B_Bi_Bi_16x8 = 20;
        public const uint B_Bi_Bi_8x16 = 21;
        public const uint B_8x8 = 22;

        public const uint B_Direct_8x8 = 0;

        public static MbPartPredModes MbPartPredMode(uint mb_type, uint part)
        {
            throw new NotImplementedException();
        }

        public static MbPartPredModes SubMbPredMode(uint mb_type)
        {
            throw new NotImplementedException();
        }
    }

    public enum MbPartPredModes
    {
        NA,
        Direct,

        Intra_4x4,
        Intra_8x8,
        Intra_16x16,

        Pred_L0,
        Pred_L1,
        BiPred
    }

    public static class ScalingLists
    {
        public static uint[] Default_4x4_Intra = new uint[] { 6, 13, 13, 20, 20, 20, 28, 28, 28, 28, 32, 32, 32, 37, 37, 42 };
        public static uint[] Default_4x4_Inter = new uint[] { 10, 14, 14, 20, 20, 20, 24, 24, 24, 24, 27, 27, 27, 30, 30, 34 };
        
        public static uint[] Default_8x8_Intra = new uint[] { 6,10,10,13,11,13,16,16,16,16,18,18,18,18,18,23,23,23,23,23,23,25,25,25,25,25,25,25,27,27,27,27,27,27,27,27,29,29,29,29,29,29,29,31,31,31,31,31,31,33,33,33,33,33,36,36,36,36,38,38,38,40,40,42 };
        public static uint[] Default_8x8_Inter = new uint[] { 9,13,13,15,13,15,17,17,17,17,19,19,19,19,19,21,21,21,21,21,21,22,22,22,22,22,22,22,24,24,24,24,24,24,24,24,25,25,25,25,25,25,25,27,27,27,27,27,27,28,28,28,28,28,30,30,30,30,32,32,32,33,33,35 };
    }

    public class ResidualBlock : IItuSerializable
    {
        public ulong Read(ItuStream stream)
        {
            return 0;
        }

        public ulong Write(ItuStream stream)
        {
            return 0;
        }

        public ulong CalculateSize(ItuStream stream)
        {
            return 0;
        }
    }

    public class GreenMetadata : IItuSerializable
    {
        public ulong Read(ItuStream stream)
        {
            throw new NotImplementedException();
        }

        public ulong Write(ItuStream stream)
        {
            throw new NotImplementedException();
        }

        public ulong CalculateSize(ItuStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
