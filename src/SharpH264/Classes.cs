
using System;
using System.Collections.Generic;
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
