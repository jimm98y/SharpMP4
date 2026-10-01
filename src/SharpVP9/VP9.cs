using SharpAVX;
using System;
using System.IO;
using static SharpVP9.VP9Constants;

namespace SharpVP9
{
    /// <summary>
    /// A frame of VP9 (6.1), or the index of a superframe (B.2.1), as it was read, for writing it again: its size, its
    /// syntax elements as they were read, and the state they were read into. Changed through <see cref="Edit"/>, it is
    /// written with the changes.
    /// </summary>
    public sealed class VP9Unit
    {
        internal VP9Unit(int size, bool isSuperframeIndex, AomSyntaxRecord record, VP9Context read)
        {
            Size = size;
            IsSuperframeIndex = isSuperframeIndex;
            Record = record;
            Read = read;
        }

        /// <summary>The bytes of the frame (sz of frame), or of the superframe index.</summary>
        public int Size { get; set; }

        /// <summary>Whether it is the index of a superframe (B.2.1) rather than a frame.</summary>
        public bool IsSuperframeIndex { get; }

        /// <summary>Every occurrence of each syntax element, as it was read.</summary>
        public AomSyntaxRecord Record { get; }

        internal VP9Context Read { get; }

        /// <summary>The bytes of a unit that could not be read, which is written as it was; null for one that was read.</summary>
        public byte[] Unreadable { get; internal set; }

        /// <summary>The state to write the unit from, if it has been changed; null, it is written as it was read.</summary>
        public VP9Context Edited { get; private set; }

        /// <summary>
        /// The state the unit was read into, to change before it is written: the elements it gives other values are
        /// written with them. A change that makes the compressed header longer needs header_size_in_bytes, and the
        /// frame's size, too.
        /// </summary>
        public VP9Context Edit() => Edited ??= Read.Copy();
    }

    /// <summary>
    /// The state VP9's headers (VP9 Bitstream &amp; Decoding Process Specification v0.7) are read into, frame by frame: the
    /// uncompressed header, and the compressed header, which the Boolean coder codes; the tiles are taken as bytes, not
    /// decoded. The probability tables are not followed: they are adapted from the counts of the tiles decoded (8.4.2),
    /// and which bits the headers read does not depend on them - so what the compressed header updates them to is what
    /// its deltas make of the tables as they are here, not the probabilities a decoder has.
    /// </summary>
    public partial class VP9Context
    {
        private AomStream stream;

        /// <summary>True to keep, of each unit read, what writing it again takes: <see cref="LastUnit"/>.</summary>
        public bool RecordSyntax { get; set; }

        /// <summary>With <see cref="RecordSyntax"/>, the last unit read.</summary>
        public VP9Unit LastUnit { get; private set; }

        /// <summary>
        /// Reject what a conforming stream cannot contain - a fixed bit or byte of the wrong value, a Boolean decoder's
        /// marker or padding not 0 - rather than read on. Off by default: a stream is read as far as it can be.
        /// </summary>
        public bool Strict { get; set; }

        // True while a unit is written.
        private bool _writing;

        // Where the frame being read or written starts
        private int _frameStart;

        /// <summary>The bytes the last frame's uncompressed header took, to the end of its trailing bits.</summary>
        public int UncompressedHeaderSize { get; private set; }

        /// <summary>The bytes of the last frame's compressed header: header_size_in_bytes.</summary>
        public int CompressedHeaderSize => header_size_in_bytes;

        #region Reading and writing

        /// <summary>Reads a frame of sz bytes (6.1), and updates the references it refreshes (8.10).</summary>
        public void Read(AomStream stream, int size) => Read(stream, size, isSuperframeIndex: false);

        /// <summary>Reads the index of a superframe (B.2.1), of the size <see cref="SuperframeFrameSizes"/> leaves it.</summary>
        public void ReadSuperframeIndex(AomStream stream, int size) => Read(stream, size, isSuperframeIndex: true);

        private void Read(AomStream stream, int size, bool isSuperframeIndex)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (Strict)
                stream.Validate = CheckValue;
            UncompressedHeaderSize = 0;
            var record = RecordSyntax ? new AomSyntaxRecord() : null;
            stream.Record = record;
            // read so that it can be read again: the bytes read of it kept, a stream that cannot seek as well
            SharpMP4.Common.Bitstream.PeekState? start = record != null ? stream.Bitstream.BeginPeek() : null;
            try
            {
                if (isSuperframeIndex)
                {
                    SuperframeIndex();
                }
                else
                {
                    _frameStart = get_position();
                    Frame(size);
                    UpdateReferences();
                }
            }
            catch when (start != null)
            {
                // Kept as its bytes, and the state reading it left: what a writer takes to follow on
                stream.Record = null;
                stream.Bitstream.EndPeek(start.Value);
                start = null;
                stream.Bitstream.BitsPosition = (stream.Bitstream.BitsPosition + 7) & ~7L;
                var bytes = new byte[size];
                Array.Resize(ref bytes, stream.Bitstream.ReadAvailableBytes(bytes, 0, size));
                LastUnit = new VP9Unit(size, isSuperframeIndex, record, Copy()) { Unreadable = bytes };
                throw;
            }
            finally
            {
                if (start != null)
                    stream.Bitstream.AcceptPeek(start.Value);
                stream.Record = null;
                stream.Validate = null;
            }
            if (record != null)
                LastUnit = new VP9Unit(size, isSuperframeIndex, record, Copy());
        }

        /// <summary>
        /// Writes a unit read with <see cref="RecordSyntax"/>: as it was read, but for the changes made to it
        /// (<see cref="VP9Unit.Edit"/>). This context must have written the frames before it, as another read them: its
        /// state is a decoder's, which the syntax depends on.
        /// </summary>
        public void Write(AomStream stream, VP9Unit unit)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            stream.WriteLimit = stream.GetPosition() + (long)unit.Size * 8;
            stream.Source = unit.Record;
            _original = unit.Read;
            _edited = unit.Edited;
            if (unit.Unreadable != null)
            {
                // As it was, and on from where reading it was left
                stream.WriteBytes(unit.Unreadable.Length * 8, unit.Unreadable, "unreadable_unit");
                LoadContext(unit.Read.SaveContext());
                _original = null;
                _edited = null;
                stream.WriteLimit = -1;
                return;
            }
            _writing = true;
            try
            {
                if (unit.IsSuperframeIndex)
                {
                    WriteSuperframeIndex();
                }
                else
                {
                    _frameStart = get_position();
                    WriteFrame(unit.Size);
                    UpdateReferences();
                }
            }
            finally
            {
                _writing = false;
                _original = null;
                _edited = null;
                stream.Source = null;
                stream.WriteLimit = -1;
            }
        }

        /// <summary>A copy of the state: what the context holds, but none of what it reads or writes.</summary>
        internal VP9Context Copy()
        {
            var copy = new VP9Context { Strict = Strict };
            copy.LoadContext(SaveContext(), copy: false);
            return copy;
        }

        #endregion

        #region Superframes (Annex B)

        /// <summary>
        /// The sizes of the frames of a chunk that is a superframe (B.4), or null if it is not: its last byte is a
        /// superframe_marker, and the first byte of the index that byte says the chunk ends in matches it. The index is
        /// the rest of the chunk.
        /// </summary>
        public static int[] SuperframeFrameSizes(byte[] chunk, int offset, int count)
        {
            if (chunk == null || count < 1)
                return null;
            byte last = chunk[offset + count - 1];
            if ((last & 0xe0) != 0xc0)
                return null;

            int szBytes = ((last >> 3) & 0x3) + 1;
            int numFrames = (last & 0x7) + 1;
            int szIndex = 2 + numFrames * szBytes;
            if (szIndex > count || chunk[offset + count - szIndex] != last)
                return null;

            // frame_sizes[ i ]: SzBytes bytes each, little-endian
            var sizes = new int[numFrames];
            int at = offset + count - szIndex + 1;
            for (int i = 0; i < numFrames; i++)
            {
                int size = 0;
                for (int b = 0; b < szBytes; b++)
                    size |= chunk[at++] << (8 * b);
                sizes[i] = size;
            }
            return sizes;
        }

        /// <summary>SzBytes (B.3).</summary>
        private int SzBytes => bytes_per_framesize_minus_1 + 1;

        /// <summary>NumFrames (B.3).</summary>
        private int NumFrames => frames_in_superframe_minus_1 + 1;

        #endregion

        #region Processes

        private static int Min(int x, int y) => x <= y ? x : y;

        private static int Abs(int x) => x < 0 ? -x : x;

        /// <summary>get_position( ): the bit position in the stream.</summary>
        private int get_position() => stream.GetPosition();

        /// <summary>init_bool( sz ) (9.2.1): the Boolean decoder, or encoder, started, after the uncompressed header.</summary>
        private void init_bool(int sz)
        {
            UncompressedHeaderSize = (get_position() - _frameStart) / 8;
            if (_writing)
                stream.StartBool(sz);
            else
                stream.InitBool(sz);
        }

        /// <summary>exit_bool( ) (9.2.3): the compressed header's Boolean coding ended.</summary>
        private void exit_bool()
        {
            if (_writing)
                stream.StopBool();
            else
                stream.ExitBool();
        }

        /// <summary>
        /// decode_tiles( sz ) (6.4): the tiles are not decoded, but taken as they are - skipped, or recorded and written
        /// back as they were read.
        /// </summary>
        private void decode_tiles(int sz)
        {
            if (sz < 0)
                throw new InvalidDataException($"The headers pass the end of the frame by {-sz} bytes.");
            if (sz == 0)
                return;
            if (_writing)
            {
                byte[] tiles = stream.Pick("tile_data", (byte[])null, null)
                    ?? throw new InvalidOperationException("The tile_data was not recorded: SharpVP9 writes it as it was read.");
                stream.WriteBytes(sz * 8, tiles, "tile_data");
            }
            else if (stream.Record != null)
            {
                stream.ReadBytes(sz * 8, out _, "tile_data");
            }
            else
            {
                stream.Skip((long)sz * 8);
            }
        }

        // The probability tables are not followed (see the class): loading, saving and adapting them, and the counts
        // adapting takes, are left undone.
        private void load_probs(int ctx) { }
        private void load_probs2(int ctx) { }
        private void save_probs(int ctx) { }
        private void clear_counts() { }
        private void refresh_probs() { }

        /// <summary>
        /// setup_past_independence (7.2): the frame decoded without the frames before it. The segmentation, the loop
        /// filter's deltas and the sign biases are reset; the probability tables, which are not followed, and the
        /// segmentation map, the tiles', are not.
        /// </summary>
        private void setup_past_independence()
        {
            for (int i = 0; i < MAX_SEGMENTS; i++)
            {
                for (int j = 0; j < SEG_LVL_MAX; j++)
                {
                    FeatureData[i][j] = 0;
                    FeatureEnabled[i][j] = 0;
                }
            }
            segmentation_abs_or_delta_update = 0;
            loop_filter_delta_enabled = 1;
            loop_filter_ref_deltas[INTRA_FRAME] = 1;
            loop_filter_ref_deltas[LAST_FRAME] = 0;
            loop_filter_ref_deltas[GOLDEN_FRAME] = -1;
            loop_filter_ref_deltas[ALTREF_FRAME] = -1;
            for (int i = 0; i < MAX_MODE_LF_DELTAS; i++)
                loop_filter_mode_deltas[i] = 0;
            for (int i = 0; i < MAX_REF_FRAMES; i++)
                ref_frame_sign_bias[i] = 0;
        }

        /// <summary>
        /// The reference frame update process (8.10), as far as the headers go: the size, subsampling and bit depth of the
        /// frame, into each slot refresh_frame_flags names - what frame_size_with_refs of a frame after it reads.
        /// </summary>
        private void UpdateReferences()
        {
            for (int i = 0; i < NUM_REF_FRAMES; i++)
            {
                if (((refresh_frame_flags >> i) & 1) == 0)
                    continue;
                RefFrameWidth[i] = FrameWidth;
                RefFrameHeight[i] = FrameHeight;
                RefSubsamplingX[i] = subsampling_x;
                RefSubsamplingY[i] = subsampling_y;
                RefBitDepth[i] = BitDepth;
            }
        }

        public int[] RefFrameWidth { get; set; } = new int[NUM_REF_FRAMES];
        public int[] RefFrameHeight { get; set; } = new int[NUM_REF_FRAMES];
        public int[] RefSubsamplingX { get; set; } = new int[NUM_REF_FRAMES];
        public int[] RefSubsamplingY { get; set; } = new int[NUM_REF_FRAMES];
        public int[] RefBitDepth { get; set; } = new int[NUM_REF_FRAMES];

        private sealed partial class ContextState
        {
            public int[] RefFrameWidth;
            public int[] RefFrameHeight;
            public int[] RefSubsamplingX;
            public int[] RefSubsamplingY;
            public int[] RefBitDepth;
        }

        partial void SaveContextExtra(ContextState state)
        {
            state.RefFrameWidth = (int[])RefFrameWidth.Clone();
            state.RefFrameHeight = (int[])RefFrameHeight.Clone();
            state.RefSubsamplingX = (int[])RefSubsamplingX.Clone();
            state.RefSubsamplingY = (int[])RefSubsamplingY.Clone();
            state.RefBitDepth = (int[])RefBitDepth.Clone();
        }

        partial void LoadContextExtra(ContextState state)
        {
            RefFrameWidth = (int[])state.RefFrameWidth.Clone();
            RefFrameHeight = (int[])state.RefFrameHeight.Clone();
            RefSubsamplingX = (int[])state.RefSubsamplingX.Clone();
            RefSubsamplingY = (int[])state.RefSubsamplingY.Clone();
            RefBitDepth = (int[])state.RefBitDepth.Clone();
        }

        /// <summary>The checks of <see cref="Strict"/> on the elements they concern.</summary>
        private void CheckValue(string name, long value)
        {
            bool valid = name switch
            {
                "frame_marker" => value == 2,
                "frame_sync_byte_0" => value == 0x49,
                "frame_sync_byte_1" => value == 0x83,
                "frame_sync_byte_2" => value == 0x42,
                "reserved_zero" or "zero_bit" or "padding_bit" or "marker" or "padding" => value == 0,
                "superframe_marker" => value == 6,
                _ => true,
            };
            if (!valid)
                throw new InvalidDataException($"{name} of {value} is not allowed here.");
        }

        #endregion

        #region Tables

        /// <summary>literal_to_type (6.2.7).</summary>
        private static readonly int[] literal_to_type = { EIGHTTAP_SMOOTH, EIGHTTAP, EIGHTTAP_SHARP, BILINEAR };

        /// <summary>segmentation_feature_bits (6.2.11).</summary>
        private static readonly int[] segmentation_feature_bits = { 8, 6, 2, 0 };

        /// <summary>segmentation_feature_signed (6.2.11).</summary>
        private static readonly int[] segmentation_feature_signed = { 1, 1, 0, 0 };

        /// <summary>tx_mode_to_biggest_tx_size (10.2).</summary>
        private static readonly int[] tx_mode_to_biggest_tx_size = { TX_4X4, TX_8X8, TX_16X16, TX_32X32, TX_32X32 };

        /// <summary>inv_map_table (6.3.5).</summary>
        private static readonly int[] inv_map_table =
        {
            7, 20, 33, 46, 59, 72, 85, 98, 111, 124, 137, 150, 163, 176, 189,
            202, 215, 228, 241, 254, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11,
            12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25, 26, 27,
            28, 29, 30, 31, 32, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43,
            44, 45, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 60,
            61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 73, 74, 75, 76,
            77, 78, 79, 80, 81, 82, 83, 84, 86, 87, 88, 89, 90, 91, 92,
            93, 94, 95, 96, 97, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108,
            109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 125,
            126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 138, 139, 140, 141,
            142, 143, 144, 145, 146, 147, 148, 149, 151, 152, 153, 154, 155, 156, 157,
            158, 159, 160, 161, 162, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173,
            174, 175, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 190,
            191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 203, 204, 205, 206,
            207, 208, 209, 210, 211, 212, 213, 214, 216, 217, 218, 219, 220, 221, 222,
            223, 224, 225, 226, 227, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238,
            239, 240, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 253,
        };

        #endregion
    }
}
