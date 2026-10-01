using System;
using System.Collections.Generic;
using System.IO;

namespace SharpAVX
{
    /// <summary>
    /// VP9's descriptors (VP9 Bitstream &amp; Decoding Process Specification v0.7, 4.9) that AV1's do not have: s(n), and
    /// B(p) and L(n) of the Boolean coder (9.2), which VP8 codes with too. The Boolean decoder reads its bits through the
    /// stream, so get_position() counts them; the encoder is libvpx's (vpx_dsp/bitwriter), which the decoder is the inverse of.
    /// </summary>
    public partial class AomStream
    {
        #region s(n)

        // What a sign read with a magnitude of 0 is recorded as: "-0", which is 0, but written again is coded as it was
        private static readonly byte[] NegativeZero = { 1 };

        /// <summary>s(n) (4.9.2): an n bit magnitude, then a sign bit.</summary>
        public ulong ReadSignMagnitude(int count, out int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            long magnitude = ReadBits(count);
            int sign = magnitude == -1 ? -1 : ReadBit();
            if (sign == -1)
                throw new EndOfStreamException();
            value = sign != 0 ? -(int)magnitude : (int)magnitude;
            if (sign != 0 && magnitude == 0)
                _codedAs = NegativeZero;
            LogEnd(name, (ulong)count + 1, (long)value);
            Validate?.Invoke(name, value);
            return (ulong)count + 1;
        }

        public ulong WriteSignMagnitude(int count, int value, string name)
        {
            if (count > 32)
                throw new ArgumentOutOfRangeException(nameof(count));
            bool negativeZero = value == 0 && _pickedName == name && _picked.Value == 0 && _picked.Bytes != null;
            _pickedName = null;
            WriteBits(count, Math.Abs((long)value));
            WriteBits(1, value < 0 || negativeZero ? 1 : 0);
            LogEnd(name, (ulong)count + 1, (long)value);
            return (ulong)count + 1;
        }

        #endregion

        #region Boolean decoder (9.2)

        // BoolValue, BoolRange and BoolMaxBits (9.2.1)
        private int _boolValue;
        private int _boolRange;
        private long _boolMaxBits;

        /// <summary>
        /// init_bool( sz ) (9.2.1): the Boolean decoder started on the next sz bytes, and its marker read - a syntax element,
        /// which conformance has 0, and a stream that breaks it written again as it was.
        /// </summary>
        public void InitBool(int size)
        {
            if (size < 1)
                throw new InvalidDataException($"init_bool of {size} bytes: the Boolean decoder is given at least one.");
            long value = ReadBits(8);
            if (value == -1)
                throw new EndOfStreamException();
            _boolValue = (int)value;
            _boolRange = 255;
            _boolMaxBits = 8L * size - 8;
            int marker = DecodeBool(128);
            LogEnd("marker", 1, marker);
            Validate?.Invoke("marker", marker);
        }

        /// <summary>B(p) (4.9.3): read_bool( p ), a bool of probability p/256 of being 0.</summary>
        public ulong ReadBool(int probability, out int value, string name)
        {
            value = DecodeBool(probability);
            LogEnd(name, 1, value);
            Validate?.Invoke(name, value);
            return 1;
        }

        /// <summary>L(n) (4.9.4): read_literal( n ) (9.2.4), n bools of probability 128, the high first.</summary>
        public ulong ReadLiteral(int count, out int value, string name)
        {
            int x = 0;
            for (int i = 0; i < count; i++)
                x = 2 * x + DecodeBool(128);
            value = x;
            LogEnd(name, (ulong)count, value);
            Validate?.Invoke(name, value);
            return (ulong)count;
        }

        /// <summary>
        /// exit_bool( ) (9.2.3): the padding read, to the end of the sz bytes - a syntax element of BoolMaxBits, which
        /// conformance has 0, and a stream that breaks it written again as it was: past 64 bits, kept as its bytes.
        /// </summary>
        public void ExitBool()
        {
            long bits = _boolMaxBits;
            long value = 0;
            byte[] bytes = bits > 64 ? new byte[(bits + 7) / 8] : null;
            bool zero = true;
            for (long i = 0; i < bits; i++)
            {
                int bit = ReadBit();
                if (bit == -1)
                    throw new EndOfStreamException();
                zero &= bit == 0;
                if (bytes != null)
                    bytes[i >> 3] |= (byte)(bit << (7 - (int)(i & 7)));
                else
                    value = (value << 1) | (long)bit;
            }
            _boolMaxBits = 0;
            if (bytes != null && !zero)
            {
                _codedAs = bytes;
                value = 1;
            }
            LogEnd("padding", (ulong)bits, value);
            Validate?.Invoke("padding", value);
        }

        /// <summary>read_bool( p ) (9.2.2).</summary>
        private int DecodeBool(int probability)
        {
            int split = 1 + (((_boolRange - 1) * probability) >> 8);
            int bit;
            if (_boolValue < split)
            {
                _boolRange = split;
                bit = 0;
            }
            else
            {
                _boolRange -= split;
                _boolValue -= split;
                bit = 1;
            }

            while (_boolRange < 128)
            {
                // past BoolMaxBits, a 0, which conformance never needs
                int newBit = 0;
                if (_boolMaxBits > 0)
                {
                    newBit = ReadBit();
                    if (newBit == -1)
                        throw new EndOfStreamException();
                    _boolMaxBits--;
                }
                _boolRange <<= 1;
                _boolValue = (_boolValue << 1) + newBit;
            }

            return bit;
        }

        #endregion

        #region Boolean encoder (libvpx's vpx_writer)

        // lowvalue, range and count of vpx_writer; the bytes it has written, and how many it may
        private uint _lowValue;
        private uint _range;
        private int _count;
        private List<byte> _boolOutput;
        private int _boolSize;

        // The bools coded since init_bool, each as its probability and value: what an ending other than libvpx's is
        // checked against, where libvpx's does not fit
        private List<(byte Probability, byte Bit)> _coded;

        /// <summary>init_bool( sz ), written: the Boolean encoder started, for sz bytes, and its marker coded as it was read, 0 as libvpx codes it (vpx_start_encode).</summary>
        public void StartBool(int size)
        {
            _lowValue = 0;
            _range = 255;
            _count = -24;
            _boolOutput = new List<byte>();
            _boolSize = size;
            _coded = new List<(byte, byte)>();
            int marker = Pick("marker", 0, 0);
            EncodeBool(marker, 128);
            LogEnd("marker", 1, marker);
        }

        public ulong WriteBool(int probability, int value, string name)
        {
            EncodeBool(value, probability);
            LogEnd(name, 1, value);
            return 1;
        }

        public ulong WriteLiteral(int count, int value, string name)
        {
            for (int bit = count - 1; bit >= 0; bit--)
                EncodeBool((value >> bit) & 1, 128);
            LogEnd(name, (ulong)count, value);
            return (ulong)count;
        }

        /// <summary>
        /// exit_bool( ), written: the Boolean encoder ended as libvpx ends it (vpx_stop_encode) - 32 bools of 0, and a byte
        /// of 0 more where the last could be taken for a superframe marker - and padded with 0 to the sz bytes it was
        /// started for. Where that does not fit in sz, as the data of an encoder that ends it in fewer bytes may not, it is
        /// ended in the fewest bytes that read as the same bools.
        /// </summary>
        public void StopBool()
        {
            var coded = _coded;
            _coded = null;
            for (int i = 0; i < 32; i++)
                EncodeBool(0, 128);
            if ((_boolOutput[_boolOutput.Count - 1] & 0xe0) == 0xc0)
                _boolOutput.Add(0);

            var bytes = new byte[_boolSize];
            if (_boolOutput.Count <= _boolSize)
                _boolOutput.CopyTo(bytes);
            else if (!EndShortest(_boolOutput, coded, bytes))
                throw new InvalidOperationException($"The Boolean coded data takes more than the {_boolSize} bytes its size says.");
            _boolOutput = null;

            // the padding, from where the decoder stops reading, as it was read - of a stream that does not code it 0
            long read = 8 + DecodedBits(bytes, coded);
            long bits = 8L * bytes.Length - read;
            long padding = Pick("padding", 0L, 0L);
            byte[] paddingBytes = padding != 0 && _pickedName == "padding" ? _picked.Bytes : null;
            _pickedName = null;
            for (long i = 0; i < bits; i++)
            {
                int bit = paddingBytes != null ? (paddingBytes[i >> 3] >> (7 - (int)(i & 7))) & 1
                    : bits <= 64 ? (int)((padding >> (int)(bits - 1 - i)) & 1) : 0;
                long at = read + i;
                bytes[at >> 3] = (byte)((bytes[at >> 3] & ~(0x80 >> (int)(at & 7))) | (bit << (7 - (int)(at & 7))));
            }

            WriteBytes(bytes.Length * 8, bytes, "");
            _codedAs = paddingBytes;
            LogEnd("padding", (ulong)bits, padding);
        }

        /// <summary>
        /// The coded value, libvpx's bytes, rounded up to the fewest bytes - the rest 0 - that the decoder reads as the same
        /// bools: any value in the interval the last bool left does, and libvpx's is its low end, with 32 bools to spare.
        /// </summary>
        private static bool EndShortest(List<byte> output, List<(byte Probability, byte Bit)> coded, byte[] bytes)
        {
            for (int n = 1; n <= bytes.Length && n <= output.Count; n++)
            {
                Array.Clear(bytes, 0, bytes.Length);
                output.CopyTo(0, bytes, 0, n);
                bool below = false;
                for (int i = n; i < output.Count; i++)
                    below |= output[i] != 0;
                if (below)
                {
                    // up by one in the last byte kept, carried
                    int at = n - 1;
                    while (at >= 0 && bytes[at] == 0xff)
                        bytes[at--] = 0;
                    if (at < 0)
                        continue;
                    bytes[at]++;
                }
                if (DecodedBits(bytes, coded) >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The bits the Boolean decoder (9.2) reads of data - all of it the decoder's - after its first byte, to give these
        /// bools, marker and all; -1 if it gives others.
        /// </summary>
        private static long DecodedBits(byte[] data, List<(byte Probability, byte Bit)> coded)
        {
            int position = 0;
            long maxBits = 8L * data.Length - 8;
            int value = data[0], range = 255;
            position = 8;
            foreach (var (probability, expected) in coded)
            {
                int split = 1 + (((range - 1) * probability) >> 8);
                int bit;
                if (value < split)
                {
                    range = split;
                    bit = 0;
                }
                else
                {
                    range -= split;
                    value -= split;
                    bit = 1;
                }
                if (bit != expected)
                    return -1;

                while (range < 128)
                {
                    int newBit = 0;
                    if (maxBits > 0)
                    {
                        newBit = (data[position >> 3] >> (7 - (position & 7))) & 1;
                        position++;
                        maxBits--;
                    }
                    range <<= 1;
                    value = (value << 1) + newBit;
                }
            }
            return position - 8;
        }

        /// <summary>vpx_write: a bool of probability p/256 of being 0.</summary>
        private void EncodeBool(int bit, int probability)
        {
            _coded?.Add(((byte)probability, (byte)bit));
            uint split = 1 + (((_range - 1) * (uint)probability) >> 8);
            uint range = split;
            uint lowValue = _lowValue;
            int count = _count;
            if (bit != 0)
            {
                lowValue += split;
                range = _range - split;
            }

            int shift = Norm(range);
            range <<= shift;
            count += shift;
            if (count >= 0)
            {
                int offset = shift - count;
                if (((lowValue << (offset - 1)) & 0x80000000) != 0)
                {
                    // the carry, into the bytes written
                    int x = _boolOutput.Count - 1;
                    while (x >= 0 && _boolOutput[x] == 0xff)
                    {
                        _boolOutput[x] = 0;
                        x--;
                    }
                    _boolOutput[x]++;
                }

                _boolOutput.Add((byte)((lowValue >> (24 - offset)) & 0xff));
                lowValue <<= offset;
                shift = count;
                lowValue &= 0xffffff;
                count -= 8;
            }

            lowValue <<= shift;
            _count = count;
            _lowValue = lowValue;
            _range = range;
        }

        /// <summary>vpx_norm: the shift that brings a range of 1 to 255 to 128 or more.</summary>
        private static int Norm(uint range)
        {
            int shift = 0;
            while (range < 128)
            {
                range <<= 1;
                shift++;
            }
            return shift;
        }

        #endregion
    }
}
