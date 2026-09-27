using SharpAVX;
using System;

namespace SharpAV2
{
    public interface IAomContext : IAomSerializable
    {
        int SelectedOperatingPoint { get; set; }
        int ObuSizeLen { get; }
        byte[] LastObuFrameHeader { get; set; }
    }

    public interface IAomSerializable
    {
        void Read(AomStream stream, int size);
    }

    public partial class AV2Context
    {
        private AomStream stream;

        /// <summary>The operating point a decoder would select.</summary>
        public int SelectedOperatingPoint { get; set; } = 0;

        public byte[] LastObuFrameHeader { get; set; }

        /// <summary>The bits of the last OBU's obu_size.</summary>
        public int ObuSizeLen { get; private set; }

        /// <summary>
        /// An arithmetic coded element (L(n), S(), NS(n)): the tile data's, which SharpAV2 does not decode.
        /// The headers reach none; a syntax structure they share with the tile data has them on its other path.
        /// </summary>
        private ulong ReadArithmetic(out int value, string name)
        {
            throw new NotSupportedException($"{name} is arithmetic coded: SharpAV2 does not decode tile data.");
        }

        /// <summary>decode_4part( num ) (5.20.10.6): arithmetic coded, the tile data's.</summary>
        private int decode_4part(int num)
        {
            ReadArithmetic(out int value, "wiener_ns_base");
            return value;
        }

        /// <summary>Reads one OBU of sz bytes (AV2 5.3.1, open_bitstream_unit).</summary>
        /// <summary>The bit position just after the OBU being read.</summary>
        private long ObuEndPosition;

        public void Read(AomStream stream, int size)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            OpenBitstreamUnit(size);
        }
    }
}
