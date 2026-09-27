using SharpAVX;
using System;
using System.IO;

namespace SharpAV1
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

    /// <summary>
    /// An OBU as it was read, for writing it again: its size, its syntax elements as they were read, and
    /// the state they were read into. Changed through <see cref="Edit"/>, it is written with the changes.
    /// </summary>
    public sealed class AV1Obu
    {
        internal AV1Obu(int size, AomSyntaxRecord record, AV1Context read)
        {
            Size = size;
            Record = record;
            Read = read;
        }

        /// <summary>The bytes of the OBU, its header and obu_size among them (sz of open_bitstream_unit).</summary>
        public int Size { get; set; }

        /// <summary>Every occurrence of each syntax element, as it was read.</summary>
        public AomSyntaxRecord Record { get; }

        internal AV1Context Read { get; }

        /// <summary>
        /// The bytes of an OBU that could not be read - to the end of what it was read from - and so is
        /// written as it was; null for one that was read.
        /// </summary>
        public byte[] Unreadable { get; internal set; }

        /// <summary>The state to write the OBU from, if it has been changed; null, it is written as it was read.</summary>
        public AV1Context Edited { get; private set; }

        /// <summary>
        /// The state the OBU was read into, to change before it is written: the elements it gives other
        /// values are written with them. A change that makes the OBU longer or shorter needs obu_size too.
        /// </summary>
        public AV1Context Edit() => Edited ??= Read.Copy();

        /// <summary>
        /// The OBU without its record: written from the state it was read into, as an encoder writes from
        /// its own - what that state does not say, such as the tile data, it cannot write.
        /// </summary>
        public AV1Obu FromState() => new AV1Obu(Size, new AomSyntaxRecord(), Read) { Edited = Edited };
    }

    public partial class AV1Context
    {
        private AomStream stream;

        /// <summary>True to keep, of each OBU read, what writing it again takes: <see cref="LastObu"/>.</summary>
        public bool RecordSyntax { get; set; }

        /// <summary>With <see cref="RecordSyntax"/>, the last OBU read.</summary>
        public AV1Obu LastObu { get; private set; }

        // True while an OBU is written.
        private bool _writing;

        /// <summary>
        /// Default selected operating point.
        /// </summary>
        public int SelectedOperatingPoint { get; set; } = 0;

        /// <summary>
        /// 1 to read the OBUs of every layer, rather than drop those outside the operating point
        /// selected as a decoder does (7.1) - to look at a scalable stream whole.
        /// </summary>
        public int AllLayers { get; set; } = 0;

        /// <summary>
        /// Reject what a conforming stream cannot contain - a fixed bit of the wrong value, a
        /// profile or tile index out of its range, a reference that is not there, a redundant
        /// frame header that differs from the one it repeats - as ffmpeg's reader does, rather
        /// than read on. Off by default: a stream is read as far as it can be.
        /// </summary>
        public bool Strict { get; set; }

        private int obu_size_len = 0;
        private int prevFrame;

        public byte[] LastObuFrameHeader { get; set; }
        /// <summary>
        /// The bits of the last OBU's obu_size - none when it coded no size. It kept the size of
        /// the last OBU that did, and a caller working out where an OBU without one ends, or where
        /// its payload starts, was that many bytes out.
        /// </summary>
        public int ObuSizeLen { get { return obu_has_size_field != 0 ? obu_size_len : 0; } }

        public int[] RefMiCols { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefMiRows { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefFrameId { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefFrameHeight { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefFrameType { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefRenderWidth { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefRenderHeight { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] RefUpscaledWidth { get; set; } = new int[AV1Constants.NUM_REF_FRAMES];
        public int[] Remap_Lr_Type { get; set; } = new int[] { AV1FrameRestorationType.RESTORE_NONE, AV1FrameRestorationType.RESTORE_SWITCHABLE, AV1FrameRestorationType.RESTORE_WIENER, AV1FrameRestorationType.RESTORE_SGRPROJ };
        public int[] Ref_Frame_List = { AV1RefFrames.LAST2_FRAME, AV1RefFrames.LAST3_FRAME, AV1RefFrames.BWDREF_FRAME, AV1RefFrames.ALTREF2_FRAME, AV1RefFrames.ALTREF_FRAME };
        // The segmentation features' widths, ranges and signs (5.9.14). They were left all zero,
        // so no feature value was ever read, and everything after one out of step.
        public int[] Segmentation_Feature_Bits { get; set; } = [8, 6, 6, 6, 6, 3, 0, 0];
        public int[] Segmentation_Feature_Max { get; set; } = [255, AV1Constants.MAX_LOOP_FILTER, AV1Constants.MAX_LOOP_FILTER, AV1Constants.MAX_LOOP_FILTER, AV1Constants.MAX_LOOP_FILTER, 7, 0, 0];
        public int[] Segmentation_Feature_Signed { get; set; } = [1, 1, 1, 1, 1, 0, 0, 0];

        public int[][] PrevSegmentIds { get; set; } = new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] };
        public int[][][] SavedSegmentIds { get; set; } = new int[AV1Constants.NUM_REF_FRAMES][][] {
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] },
            new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] }
        };

        public int[][] PrevGmParams { get; set; } = new int[8][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] };
        public int[][][] SavedGmParams { get; set; } = new int[AV1Constants.NUM_REF_FRAMES][][] {
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] },
            new int[AV1RefFrames.ALTREF_FRAME + 1][] { new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6], new int[6] }
        };

        public int[][][] SavedFeatureEnabled { get; set; } = new int[AV1Constants.NUM_REF_FRAMES][][] {
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] }
            };
        public int[][][] SavedFeatureData { get; set; } = new int[AV1Constants.NUM_REF_FRAMES][][] {
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] },
            new int[AV1Constants.MAX_SEGMENTS][] { new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX], new int[AV1Constants.SEG_LVL_MAX] }
            };
        
        public int[][] saved_loop_filter_ref_deltas = new int[AV1Constants.NUM_REF_FRAMES][] { new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8], new int[8] };
        public int[][] saved_loop_filter_mode_deltas = new int[AV1Constants.NUM_REF_FRAMES][] { new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2], new int[2] };
        public void Read(AomStream stream, int size)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (Strict)
                stream.Validate = CheckValue;
            foundRefs = 0;
            var record = RecordSyntax ? new AomSyntaxRecord() : null;
            stream.Record = record;
            var input = stream.Bitstream.BaseStream;
            long start = record != null && input.CanSeek ? input.Position : -1;
            try
            {
                OpenBitstreamUnit(size);
            }
            catch when (start >= 0)
            {
                // Kept as its bytes, and the state reading it left: what a writer takes to follow on
                input.Position = start;
                var bytes = new byte[size];
                int read = 0, count;
                while (read < size && (count = input.Read(bytes, read, size - read)) > 0)
                    read += count;
                Array.Resize(ref bytes, read);
                LastObu = new AV1Obu(size, record, Copy()) { Unreadable = bytes };
                throw;
            }
            finally
            {
                stream.Record = null;
            }
            if (record != null)
                LastObu = new AV1Obu(size, record, Copy());
        }

        /// <summary>
        /// Writes an OBU read with <see cref="RecordSyntax"/>: as it was read, but for the changes made to
        /// it (<see cref="AV1Obu.Edit"/>). This context must have written the OBUs before it, as another
        /// read them - its state is a decoder's, which the syntax depends on - and been given, as the
        /// reader is, the last frame header's bytes (<see cref="LastObuFrameHeader"/>).
        /// </summary>
        public void Write(AomStream stream, AV1Obu obu)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (obu == null)
                throw new ArgumentNullException(nameof(obu));
            stream.WriteLimit = stream.GetPosition() + (long)obu.Size * 8;
            stream.Source = obu.Record;
            _original = obu.Read;
            _edited = obu.Edited;
            if (obu.Unreadable != null)
            {
                // As it was, and on from where reading it was left
                stream.WriteBytes(obu.Unreadable.Length * 8, obu.Unreadable, "unreadable_obu");
                LoadContext(obu.Read.SaveContext());
                _original = null;
                _edited = null;
                stream.WriteLimit = -1;
                return;
            }
            _writing = true;
            try
            {
                WriteOpenBitstreamUnit(obu.Size);
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

        /// <summary>A copy of the syntax elements and variables: what writing an OBU again takes of the state.</summary>
        internal AV1Context Copy()
        {
            var copy = new AV1Context { SelectedOperatingPoint = SelectedOperatingPoint, AllLayers = AllLayers, Strict = Strict };
            copy.LoadContext(SaveContext(), copy: false);
            copy.ItutT35Payload = ItutT35Payload;
            copy.UnknownMetadataPayload = UnknownMetadataPayload;
            return copy;
        }

        /// <summary>
        /// The rest of the OBU, which the syntax does not read: tile data, or an OBU dropped. Skipped as it
        /// is read; recorded, it is kept, and written as it was.
        /// </summary>
        private void RestOfObu(string name)
        {
            long bits = (long)obu_size * 8 - (stream.GetPosition() - startPosition);
            if (bits <= 0)
                return;

            // The bits to the next byte, then the bytes
            int lead = (int)Math.Min(bits, (8 - stream.GetPosition() % 8) % 8);
            int bytes = (int)((bits - lead) / 8);
            if (_writing)
            {
                if (lead > 0)
                    stream.WriteFixed(lead, stream.Pick(name + "_bits", 0, 0), name + "_bits");
                byte[] rest = stream.Pick(name, (byte[])null, null)
                    ?? throw new InvalidOperationException($"The {name} was not recorded: SharpAV1 writes it as it was read.");
                stream.WriteBytes(bytes * 8, rest, name);
            }
            else if (stream.Record != null)
            {
                if (lead > 0)
                    stream.ReadFixed(lead, out _, name + "_bits");
                stream.ReadBytes(bytes * 8, out _, name);
            }
            else
            {
                stream.Skip(bits);
            }
        }

        /// <summary>
        /// Which reference slots a frame has been stored in: what a frame that refers to one needs.
        /// Not RefValid, which an error resilient frame expecting other order hints clears too.
        /// Marked as soon as the frame header says which slots the frame refreshes, as ffmpeg
        /// marks them: in a broken stream, the tile groups after it may never come.
        /// </summary>
        private readonly bool[] refFilled = new bool[AV1Constants.NUM_REF_FRAMES];

        // The found_refs read in the OBU: the i of the loop they are read in, which is its own.
        private int foundRefs;

        /// <summary>The checks of <see cref="Strict"/> on the elements they concern.</summary>
        private void CheckValue(string name, long value)
        {
            bool valid = name switch
            {
                "obu_forbidden_bit" or "zero_bit" => value == 0,
                "trailing_one_bit" => value == 1,
                "seq_profile" => value <= 2,
                // leb128() values are at most 2^32 - 1 (4.10.5).
                "obu_size" or "metadata_type" => value <= uint.MaxValue,
                "tg_start" => value < NumTiles,
                "tg_end" => value >= tg_start && value < NumTiles,
                // Read in the loop over i, once for each i up to the one found: the reference it names is
                // ref_frame_idx[ i ], i being how many were read before it in the OBU.
                "found_ref" => value == 0 || refFilled[ref_frame_idx[foundRefs]],
                "frame_to_show_map_idx" => refFilled[value],
                _ => true,
            };

            if (!valid)
                throw new InvalidDataException($"{name} of {value} is not allowed here.");
            if (name == "found_ref")
                foundRefs++;

            // refresh_frame_flags is allFrames, not coded, for a switch frame and a shown key frame,
            // and a key frame shown again refreshes every slot too (7.21).
            if (name == "refresh_frame_flags")
                MarkFilled(value);
            else if (name == "show_frame" && (frame_type == AV1FrameTypes.SWITCH_FRAME || (value == 1 && frame_type == AV1FrameTypes.KEY_FRAME)))
                MarkFilled(0xFF);
            else if (name == "frame_to_show_map_idx" && RefFrameType[value] == AV1FrameTypes.KEY_FRAME)
                MarkFilled(0xFF);
        }

        private void MarkFilled(long flags)
        {
            for (int slot = 0; slot < AV1Constants.NUM_REF_FRAMES; slot++)
                if (((flags >> slot) & 1) != 0)
                    refFilled[slot] = true;
        }

        private static int Min(int x, int y) => x <= y ? x : y;

        private static int Max(int x, int y) => x >= y ? x : y;

        /// <summary>get_position( ): the bit position in the OBU's stream.</summary>
        private int get_position() => stream.GetPosition();

        public static int Clip3(int low, int high, int value)
        {
            return MathEx.Clamp(value, low, high);
        }

        /// <summary>
        /// get_qindex (7.12.2): the quantizer index a segment is coded with. The frame header asks
        /// for it to decide which segments are lossless, and a lossless frame codes no loop filter
        /// or CDEF parameters - so a stub that never returned 0 read those that were not there.
        /// </summary>
        private int get_qindex(int ignoreDeltaQ, int segmentId)
        {
            // CurrentQIndex only moves while tiles are decoded; in the headers it is base_q_idx.
            int currentQIndex = base_q_idx;

            if (segmentation_enabled != 0 && FeatureEnabled[segmentId][AV1Constants.SEG_LVL_ALT_Q] != 0)
            {
                int data = FeatureData[segmentId][AV1Constants.SEG_LVL_ALT_Q];
                int qindex = base_q_idx + data;
                if (ignoreDeltaQ == 0 && delta_q_present == 1)
                    qindex = currentQIndex + data;

                return Clip3(0, 255, qindex);
            }

            if (ignoreDeltaQ == 0 && delta_q_present == 1)
                return currentQIndex;

            return base_q_idx;
        }

        /// <summary>
        /// drop_obu( ): an OBU outside the operating point, which a decoder leaves unread. Recorded, it is
        /// kept whole, and written as it was.
        /// </summary>
        private void drop_obu()
        {
            if (_writing || stream.Record != null)
                RestOfObu("dropped_obu");
        }

        /// <summary>
        /// set_frame_refs (7.8). The loops count with variables of their own: they shared the
        /// class's i, which the find_* helpers left at NUM_REF_FRAMES, so only LAST2_FRAME was
        /// looked for among the forward references and the rest took the earliest frame - and
        /// skip_mode_present was read where a decoder does not.
        /// </summary>
        private void reset_grain_params() 
        {
            apply_grain = 0;
            grain_seed = 0;
            update_grain = 0;
            film_grain_params_ref_idx = 0;
            num_y_points = 0;
            for (int i = 0; i < point_y_value.Length; i++)
            {
                point_y_value[i] = 0;
                point_y_scaling[i] = 0;
            }
            chroma_scaling_from_luma = 0;
            num_cb_points = 0;
            for (int i = 0; i < point_cb_value.Length; i++)
            {
                point_cb_value[i] = 0;
                point_cb_scaling[i] = 0;
            }
            num_cr_points = 0;
            for (int i = 0; i < point_cr_value.Length; i++)
            {
                point_cr_value[i] = 0;
                point_cr_scaling[i] = 0;
            }
            grain_scaling_minus_8 = 0;
            ar_coeff_lag = 0;
            for (int i = 0; i < 24; i++)
            {
                ar_coeffs_y_plus_128[i] = 0;
            }
            for (int i = 0; i < 25; i++)
            {
                ar_coeffs_cb_plus_128[i] = 0;
                ar_coeffs_cr_plus_128[i] = 0;
            }
            ar_coeff_shift_minus_6 = 0;
            grain_scale_shift = 0;
            cb_mult = 0;
            cb_luma_mult = 0;
            cb_offset = 0;
            cr_mult = 0;
            cr_luma_mult = 0;
            cr_offset = 0;
            overlap_flag = 0;
            clip_to_restricted_range = 0;
        }

        private void load_grain_params(int p) 
        {
            /* load_grain_params(idx) is a function call that indicates that all the syntax elements read in film_grain_params should be
            set equal to the values stored in an area of memory indexed by idx. */
        }

        private void setup_past_independence() 
        {
            for (int r = AV1RefFrames.LAST_FRAME; r <= AV1RefFrames.ALTREF_FRAME; r++)
            {
                for (int ri = 0; ri <= 5; ri++)
                {
                    PrevGmParams[r][ri] = ((ri % 3 == 2) ? (1 << AV1Constants.WARPEDMODEL_PREC_BITS) : 0);
                }
            }
        }
        private void load_cdfs(int value) 
        {
            /* load_cdfs( ctx ) is a function call that indicates that the CDF tables are loaded from frame context number ctx in the
            range 0 to (NUM_REF_FRAMES - 1). When this function is invoked, a copy of each CDF array mentioned in the
            semantics for init_coeff_cdfs and init_non_coeff_cdfs is loaded from an area of memory indexed by ctx. (The memory
            contents of these frame contexts have been initialized by previous calls to save_cdfs). Once the CDF arrays have been
            loaded, the last entry in each array, representing the symbol count for that context, is set to 0. */
        }

        private void load_previous() 
        {
            prevFrame = ref_frame_idx[primary_ref_frame];

            // A copy: taking the saved array itself, setup_past_independence() in a later frame
            // wrote its defaults over what that reference frame had saved.
            for (int r = AV1RefFrames.LAST_FRAME; r <= AV1RefFrames.ALTREF_FRAME; r++)
                Array.Copy(SavedGmParams[prevFrame][r], PrevGmParams[r], 6);
            load_loop_filter_params(prevFrame);
            load_segmentation_params(prevFrame);
        }

        private void load_segmentation_params(int i)
        {
            /*
            load_segmentation_params( i ) is a function call that indicates that the values of FeatureEnabled[ j ][ k ] and FeatureData[ j ][ k ] 
            for j = 0 .. MAX_SEGMENTS-1, for k = 0 .. SEG_LVL_MAX-1 should be loaded from an area of memory indexed by i.
             */
            for (int j = 0; j < AV1Constants.MAX_SEGMENTS; j++)
            {
                for (int k = 0; k < AV1Constants.SEG_LVL_MAX; k++)
                {
                    FeatureEnabled[j][k] = SavedFeatureEnabled[i][j][k];
                    FeatureData[j][k] = SavedFeatureData[i][j][k];
                }
            }
        }

        private void save_segmentation_params(int i)
        {
            for (int j = 0; j < AV1Constants.MAX_SEGMENTS; j++)
            {
                for (int k = 0; k < AV1Constants.SEG_LVL_MAX; k++)
                {
                    SavedFeatureEnabled[i][j][k] = FeatureEnabled[j][k];
                    SavedFeatureData[i][j][k] = FeatureData[j][k];
                }
            }
        }

        private void load_loop_filter_params(int i)
        {
            /*
            load_loop_filter_params( i ) is a function call that indicates that the values of loop_filter_ref_deltas[ j ] for j = 0 ..
            TOTAL_REFS_PER_FRAME-1, and the values of loop_filter_mode_deltas[ j ] for j = 0 .. 1 should be loaded from an area
            of memory indexed by i.
            */
            for (int j = 0; j < AV1Constants.TOTAL_REFS_PER_FRAME; j++)
            {
                loop_filter_ref_deltas[j] = saved_loop_filter_ref_deltas[i][j];
            }
            for (int j = 0; j <= 1; j++)
            {
                loop_filter_mode_deltas[j] = saved_loop_filter_mode_deltas[i][j];
            }
        }

        private void save_loop_filter_params(int i)
        {
            for (int j = 0; j < AV1Constants.TOTAL_REFS_PER_FRAME; j++)
            {
                saved_loop_filter_ref_deltas[i][j] = loop_filter_ref_deltas[j]; 
            }
            for (int j = 0; j <= 1; j++)
            {
                saved_loop_filter_mode_deltas[i][j] = loop_filter_mode_deltas[j];
            }
        }

        private void motion_field_estimation() 
        {
            /* nothing - needed for the decoding */
        }

        private void init_coeff_cdfs() 
        {
            /* nothing - needed for the decoding */ 
        }

        private void init_non_coeff_cdfs()
        {
            /* nothing - needed for the decoding */
        }

        /// <summary>
        /// The ITU-T T.35 payload of the last metadata OBU that had one - HDR10+, for one. Set on the state
        /// of an OBU being changed (<see cref="AV1Obu.Edit"/>), it is the payload written.
        /// </summary>
        public byte[] ItutT35Payload { get; set; }

        /// <summary>
        /// itu_t_t35_payload_bytes: the rest of the OBU, up to its trailing bits. The syntax gives
        /// no length, so it is where the trailing one bit is - the last byte that is not zero,
        /// 0x80, the payload being whole bytes. Read as nothing, as it was, the payload was taken
        /// for trailing bits, and those ran on past the end of the OBU.
        /// </summary>
        private void ItutT35PayloadBytes()
        {
            if (_writing)
            {
                ItutT35Payload = WritePayload("itu_t_t35_payload_bytes", _original?.ItutT35Payload, _edited?.ItutT35Payload);
                return;
            }
            stream.ReadBytes(PayloadBytesBeforeTrailingBits() * 8, out byte[] payload, "itu_t_t35_payload_bytes");
            ItutT35Payload = payload;
        }

        /// <summary>A payload that runs to the trailing bits: as it was read, or as it was changed to.</summary>
        private byte[] WritePayload(string name, byte[] original, byte[] edited)
        {
            byte[] payload = stream.Pick(name, original, _edited != null ? edited : original) ?? Array.Empty<byte>();
            stream.WriteBytes(payload.Length * 8, payload, name);
            return payload;
        }

        /// <summary>The payload of the last metadata OBU of a type the syntax does not know; set, as ItutT35Payload is.</summary>
        public byte[] UnknownMetadataPayload { get; set; }

        private void MetadataUnknownPayload()
        {
            if (_writing)
            {
                UnknownMetadataPayload = WritePayload("payload", _original?.UnknownMetadataPayload, _edited?.UnknownMetadataPayload);
                return;
            }
            stream.ReadBytes(PayloadBytesBeforeTrailingBits() * 8, out byte[] payload, "payload");
            UnknownMetadataPayload = payload;
        }

        /// <summary>
        /// How many whole bytes the OBU has left before its trailing bits: up to the last byte that
        /// is not zero, which - what comes before being whole bytes - is the 0x80 its trailing one
        /// bit starts. Worked out by reading ahead, without reading on. Written, only padding_obu asks:
        /// as many bytes as it was read with.
        /// </summary>
        private int PayloadBytesBeforeTrailingBits()
        {
            if (_writing)
                return stream.Source?["obu_padding_byte"].Count ?? 0;

            long remainingBits = (long)obu_size * 8 - (stream.GetPosition() - startPosition);
            var baseStream = stream.Bitstream.BaseStream;
            if (remainingBits <= 0 || stream.GetPosition() % 8 != 0 || !baseStream.CanSeek)
                return 0;

            // Byte aligned, so the underlying stream is at the next byte to read.
            var rest = new byte[remainingBits / 8];
            long position = baseStream.Position;
            int read = 0;
            while (read < rest.Length)
            {
                int count = baseStream.Read(rest, read, rest.Length - read);
                if (count <= 0)
                    break;
                read += count;
            }
            baseStream.Position = position;

            int trailing = read > 0 ? Array.FindLastIndex(rest, read - 1, read, b => b != 0) : -1;
            return Math.Max(0, trailing);
        }

        /// <summary>
        /// load_previous_segment_ids (7.20): the segment map of the frame predicted from. The map
        /// comes of decoding tiles, which a header parser does not, so there is none to load. This
        /// used to copy one the size of the current frame from arrays sized for none, and to
        /// overwrite the size recorded for the reference frame with the current one's.
        /// </summary>
        private void load_previous_segment_ids()
        {
            prevFrame = ref_frame_idx[primary_ref_frame];
        }

        /// <summary>
        /// Whether this frame's references were refreshed when its header ended, not to be again.
        /// </summary>
        private bool referencesRefreshed;

        /// <summary>How many bits the last frame header took: what a redundant copy of it takes.</summary>
        private int lastFrameHeaderBits;

        /// <summary>Whether a redundant frame header is being read again from the one it copies.</summary>
        private bool readingCopy;

        /// <summary>
        /// Where the frame header ends. With <see cref="Strict"/>, the references are refreshed
        /// here, as ffmpeg's reader refreshes them: a frame it rejects past its header - a trailing
        /// bit, a tile group - has refreshed them all the same, and the frames after it are read
        /// against those. The spec refreshes them in decode_frame_wrapup(), once the frame is
        /// decoded; in a conforming stream both come to the same.
        /// </summary>
        private void FrameHeaderDone()
        {
            if (readingCopy)
                return;

            lastFrameHeaderBits = stream.GetPosition() - startPosition;
            if (Strict && show_existing_frame == 0)
            {
                RefreshReferences();
                referencesRefreshed = true;
            }
        }

        private void decode_frame_wrapup()
        {
            // A key frame shown again is loaded first (7.21), and every reference then refreshed
            // from it. Without the loading, every slot took the order hint, size and parameters of
            // whatever frame came before - and the frames after read skip_mode_present, which
            // depends on the references' order hints, or did not, by those.
            if (show_existing_frame == 1 && frame_type == AV1FrameTypes.KEY_FRAME)
            {
                int shown = frame_to_show_map_idx;
                current_frame_id = RefFrameId[shown];
                UpscaledWidth = RefUpscaledWidth[shown];
                FrameHeight = RefFrameHeight[shown];
                RenderWidth = RefRenderWidth[shown];
                RenderHeight = RefRenderHeight[shown];
                OrderHint = RefOrderHint[shown];
                for (int ri = AV1RefFrames.LAST_FRAME; ri <= AV1RefFrames.ALTREF_FRAME; ri++)
                    for (int k = 0; k < 6; k++)
                        gm_params[ri][k] = SavedGmParams[shown][ri][k];
                load_loop_filter_params(shown);
                load_segmentation_params(shown);
            }

            if (referencesRefreshed)
                referencesRefreshed = false;
            else
                RefreshReferences();
        }

        private void RefreshReferences()
        {
            for (int i = 0; i < AV1Constants.NUM_REF_FRAMES; i++)
            {
                if (((refresh_frame_flags >> i) & 1) == 1)
                {
                    RefValid[i] = 1;
                    RefFrameId[i] = current_frame_id;
                    RefUpscaledWidth[i] = UpscaledWidth;
                    RefFrameHeight[i] = FrameHeight;
                    RefRenderWidth[i] = RenderWidth;
                    RefRenderHeight[i] = RenderHeight;
                    RefFrameType[i] = frame_type;
                    
                    // Every reference, ALTREF_FRAME too (7.20): stopping short of it, a frame
                    // predicting ALTREF_FRAME's global motion read against zeros, not what the
                    // frame it refers to had.
                    for (int ri = AV1RefFrames.LAST_FRAME; ri <= AV1RefFrames.ALTREF_FRAME; ri++)
                    {
                        for (int j = 0; j <= 5; j++)
                        {
                            SavedGmParams[i][ri][j] = gm_params[ri][j];
                        }
                    }

                    save_loop_filter_params(i);
                    save_segmentation_params(i);

                    RefOrderHint[i] = OrderHint;
                }
            }            
        }
        
        private int choose_operating_point()
        {
            return SelectedOperatingPoint;
        }

        private void skip_obu() => RestOfObu("obu_rest");

        private void frame_header_copy()
        {
            if (LastObuFrameHeader == null)
                throw new InvalidDataException("A redundant frame header with no frame header before it.");

            // With Strict, the references were refreshed when the header ended: read again against
            // them, the header would not take the bits it took. The copy is the header's bits.
            int bits = lastFrameHeaderBits;
            bool writing = _writing;
            if (!Strict)
            {
                using var aomStream = new AomStream(new MemoryStream(LastObuFrameHeader));
                var oldStream = this.stream;
                var oldSeenFrameHeader = SeenFrameHeader;
                var oldObuType = _ObuType;
                SeenFrameHeader = 0;
                this.stream = aomStream;
                readingCopy = true;
                _writing = false;
                try
                {
                    FrameHeaderObu();
                }
                finally
                {
                    _writing = writing;
                    readingCopy = false;
                    SeenFrameHeader = oldSeenFrameHeader;
                    _ObuType = oldObuType;
                    this.stream = oldStream;
                }
                bits = aomStream.GetPosition();
            }

            // The copy is in this OBU too, as many bits as the header it copies: read over them.
            // Left where they were, they were read as the OBU's trailing bits.
            byte[] copy = [];
            int rest = 0;
            if (writing)
            {
                // The header it repeats, as recorded: or, where it was not, the one this context wrote
                byte[] header = LastObuFrameHeader ?? [];
                byte[] headerBytes = new byte[Math.Min(bits / 8, header.Length)];
                Array.Copy(header, headerBytes, headerBytes.Length);
                int headerRest = bits % 8 > 0 && bits / 8 < header.Length ? header[bits / 8] >> (8 - bits % 8) : 0;
                if (bits >= 8)
                    this.stream.WriteBytes(bits / 8 * 8, copy = this.stream.Pick("frame_header_copy", headerBytes, headerBytes), "frame_header_copy");
                if (bits % 8 > 0)
                    this.stream.WriteFixed(bits % 8, rest = this.stream.Pick("frame_header_copy", headerRest, headerRest), "frame_header_copy");
                return;
            }
            if (bits >= 8)
                this.stream.ReadBytes(bits / 8 * 8, out copy, "frame_header_copy");
            if (bits % 8 > 0)
                this.stream.ReadFixed(bits % 8, out rest, "frame_header_copy");

            // A redundant frame header is a copy of the one it repeats (6.8.1).
            if (Strict && (!StartsWith(LastObuFrameHeader, copy) ||
                (bits % 8 > 0 && rest != LastObuFrameHeader[bits / 8] >> (8 - bits % 8))))
                throw new InvalidDataException("A redundant frame header that differs from the frame header it repeats.");
        }

        /// <summary>Whether data starts with prefix; spans are not in net481's framework.</summary>
        private static bool StartsWith(byte[] data, byte[] prefix)
        {
            if (data.Length < prefix.Length)
                return false;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (data[i] != prefix[i])
                    return false;
            }
            return true;
        }
    }

    public static class AV1RefFrames
    {
        public const int NONE = -1;
        public const int INTRA_FRAME = 0;
        public const int LAST_FRAME = 1;
        public const int LAST2_FRAME = 2;
        public const int LAST3_FRAME = 3;
        public const int GOLDEN_FRAME = 4;
        public const int BWDREF_FRAME = 5;
        public const int ALTREF2_FRAME = 6;
        public const int ALTREF_FRAME = 7;
    }

    public static class AV1ObuTypes
    {
        public const int OBU_RESERVED_0 = 0;
        public const int OBU_SEQUENCE_HEADER = 1;
        public const int OBU_TEMPORAL_DELIMITER = 2;
        public const int OBU_FRAME_HEADER = 3;
        public const int OBU_TILE_GROUP = 4;
        public const int OBU_METADATA = 5;
        public const int OBU_FRAME = 6;
        public const int OBU_REDUNDANT_FRAME_HEADER = 7;
        public const int OBU_TILE_LIST = 8;
        public const int OBU_RESERVED_9 = 9;
        public const int OBU_RESERVED_10 = 10;
        public const int OBU_RESERVED_11 = 11;
        public const int OBU_RESERVED_12 = 12;
        public const int OBU_RESERVED_13 = 13;
        public const int OBU_RESERVED_14 = 14;
        public const int OBU_PADDING = 15;
    }

    public static class AV1Constants
    {
        public const int REFS_PER_FRAME = 7;                // Number of reference frames that can be used for inter prediction
        public const int TOTAL_REFS_PER_FRAME = 8;          // Number of reference frame types (including intra type)
        public const int BLOCK_SIZE_GROUPS = 4;             // Number of contexts when decoding y_mode
        public const int BLOCK_SIZES = 22;                  // Number of different block sizes used
        public const int BLOCK_INVALID = 22;                // Sentinel value to mark partition choices that are not allowed
        public const int MAX_SB_SIZE = 128;                 // Maximum size of a superblock in luma samples
        public const int MI_SIZE = 4;                       // Smallest size of a mode info block in luma samples
        public const int MI_SIZE_LOG2 = 2;                  // Base 2 logarithm of smallest size of a mode info block
        public const int MAX_TILE_WIDTH = 4096;             // Maximum width of a tile in units of luma samples
        public const int MAX_TILE_AREA = 4096 * 2304;       // Maximum area of a tile in units of luma samples
        public const int MAX_TILE_ROWS = 64;                // Maximum number of tile rows
        public const int MAX_TILE_COLS = 64;                // Maximum number of tile columns
        public const int INTRABC_DELAY_PIXELS = 256;        // Number of horizontal luma samples before intra block copy can be used
        public const int INTRABC_DELAY_SB64 = 4;            // Number of 64 by 64 blocks before intra block copy can be used
        public const int NUM_REF_FRAMES = 8;                // Number of frames that can be stored for future reference
        public const int IS_INTER_CONTEXTS = 4;             // Number of contexts for is_inter
        public const int REF_CONTEXTS = 3;                  // Number of contexts for single_ref , comp_ref , comp_bwdref , uni_comp_ref , uni_comp_ref_p1 and uni_comp_ref_p2
        public const int MAX_SEGMENTS = 8;                  // Number of segments allowed in segmentation map
        public const int SEGMENT_ID_CONTEXTS = 3;           // Number of contexts for segment_id
        public const int SEG_LVL_ALT_Q = 0;                 // Index for quantizer segment feature
        public const int SEG_LVL_ALT_LF_Y_V = 1;            // Index for vertical luma loop filter segment feature
        public const int SEG_LVL_REF_FRAME = 5;             // Index for reference frame segment feature
        public const int SEG_LVL_SKIP = 6;                  // Index for skip segment feature
        public const int SEG_LVL_GLOBALMV = 7;              // Index for global mv feature
        public const int SEG_LVL_MAX = 8;                   // Number of segment features
        public const int PLANE_TYPES = 2;                   // Number of different plane types(luma or chroma)
        public const int TX_SIZE_CONTEXTS = 3;              // Number of contexts for transform size
        public const int INTERP_FILTERS = 3;                // Number of values for interp_filter
        public const int INTERP_FILTER_CONTEXTS = 16;       // Number of contexts for interp_filter
        public const int SKIP_MODE_CONTEXTS = 3;            // Number of contexts for decoding skip_mode
        public const int SKIP_CONTEXTS = 3;                 // Number of contexts for decoding skip
        public const int PARTITION_CONTEXTS = 4;            // Number of contexts when decoding partition
        public const int TX_SIZES = 5;                      // Number of square transform sizes
        public const int TX_SIZES_ALL = 19;                 // Number of transform sizes(including non-square sizes)
        public const int TX_MODES = 3;                      // Number of values for tx_mode
        public const int DCT_DCT = 0;                       // Inverse transform rows with DCT and columns with DCT
        public const int ADST_DCT = 1;                      // Inverse transform rows with DCT and columns with ADST
        public const int DCT_ADST = 2;                      // Inverse transform rows with ADST and columns with DCT
        public const int ADST_ADST = 3;                     // Inverse transform rows with ADST and columns with ADST
        public const int FLIPADST_DCT = 4;                  // Inverse transform rows with DCT and columns with FLIPADST
        public const int DCT_FLIPADST = 5;                  // Inverse transform rows with FLIPADST and columns with DCT
        public const int FLIPADST_FLIPADST = 6;             // Inverse transform rows with FLIPADST and columns with FLIPADST
        public const int ADST_FLIPADST = 7;                 // Inverse transform rows with FLIPADST and columns with ADST
        public const int FLIPADST_ADST = 8;                 // Inverse transform rows with ADST and columns with FLIPADST
        public const int IDTX = 9;                          // Inverse transform rows with identity and columns with identity
        public const int V_DCT = 10;                        // Inverse transform rows with identity and columns with DCT
        public const int H_DCT = 11;                        // Inverse transform rows with DCT and columns with identity
        public const int V_ADST = 12;                       // Inverse transform rows with identity and columns with ADST
        public const int H_ADST = 13;                       // Inverse transform rows with ADST and columns with identity
        public const int V_FLIPADST = 14;                   // Inverse transform rows with identity and columns with FLIPADST
        public const int H_FLIPADST = 15;                   // Inverse transform rows with FLIPADST and columns with identity
        public const int TX_TYPES = 16;                     // Number of inverse transform types
        public const int MB_MODE_COUNT = 17;                // Number of values for YMode
        public const int INTRA_MODES = 13;                  // Number of values for y_mode
        public const int UV_INTRA_MODES_CFL_NOT_ALLOWED = 13; // Number of values for uv_mode when chroma from luma is not allowed
        public const int UV_INTRA_MODES_CFL_ALLOWED = 14;   // Number of values for uv_mode when chroma from luma is allowed
        public const int COMPOUND_MODES = 8;                // Number of values for compound_mode
        public const int COMPOUND_MODE_CONTEXTS = 8;        // Number of contexts for compound_mode
        public const int COMP_NEWMV_CTXS = 5;               // Number of new mv values used when constructing context for compound_mode
        public const int NEW_MV_CONTEXTS = 6;               // Number of contexts for new_mv
        public const int ZERO_MV_CONTEXTS = 2;              // Number of contexts for zero_mv
        public const int REF_MV_CONTEXTS = 6;               // Number of contexts for ref_mv
        public const int DRL_MODE_CONTEXTS = 3;             // Number of contexts for drl_mode
        public const int MV_CONTEXTS = 2;                   // Number of contexts for decoding motion vectors including one for intra block copy
        public const int MV_INTRABC_CONTEXT = 1;            // Motion vector context used for intra block copy
        public const int MV_JOINTS = 4;                     // Number of values for mv_joint
        public const int MV_CLASSES = 11;                   // Number of values for mv_class
        public const int CLASS0_SIZE = 2;                   // Number of values for mv_class0_bit
        public const int MV_OFFSET_BITS = 10;               // Maximum number of bits for decoding motion vectors
        public const int MAX_LOOP_FILTER = 63;              // Maximum value used for loop filtering
        public const int REF_SCALE_SHIFT = 14;              // Number of bits of precision when scaling reference frames
        public const int SUBPEL_BITS = 4;                   // Number of bits of precision when choosing an inter prediction filter kernel
        public const int SUBPEL_MASK = 15;                  // ( 1 << SUBPEL_BITS ) - 1
        public const int SCALE_SUBPEL_BITS = 10;            // Number of bits of precision when computing inter prediction locations
        public const int MV_BORDER = 128;                   // Value used when clipping motion vectors
        public const int PALETTE_COLOR_CONTEXTS = 5;        // Number of values for color contexts
        public const int PALETTE_MAX_COLOR_CONTEXT_HASH = 8; // Number of mappings between color context hash and color context
        public const int PALETTE_BLOCK_SIZE_CONTEXTS = 7; // Number of values for palette block size
        public const int PALETTE_Y_MODE_CONTEXTS = 3; // Number of values for palette Y plane mode contexts
        public const int PALETTE_UV_MODE_CONTEXTS = 2; // Number of values for palette U and V plane mode contexts
        public const int PALETTE_SIZES = 7; // Number of values for palette_size
        public const int PALETTE_COLORS = 8; // Number of values for palette_color
        public const int PALETTE_NUM_NEIGHBORS = 3; // Number of neighbors considered within palette computation
        public const int DELTA_Q_SMALL = 3; // Value indicating alternative encoding of quantizer index delta values
        public const int DELTA_LF_SMALL = 3; // Value indicating alternative encoding of loop filter delta values
        public const int QM_TOTAL_SIZE = 3344; // Number of values in the quantizer matrix
        public const int MAX_ANGLE_DELTA = 3; // Maximum magnitude of AngleDeltaY and AngleDeltaUV
        public const int DIRECTIONAL_MODES = 8; // Number of directional intra modes
        public const int ANGLE_STEP = 3; // Number of degrees of step per unit increase in AngleDeltaY or AngleDeltaUV.
        public const int TX_SET_TYPES_INTRA = 3; // Number of intra transform set types
        public const int TX_SET_TYPES_INTER = 4; // Number of inter transform set types
        public const int WARPEDMODEL_PREC_BITS = 16; // Internal precision of warped motion models
        public const int IDENTITY = 0; // Warp model is just an identity transform
        public const int TRANSLATION = 1; // Warp model is a pure translation
        public const int ROTZOOM = 2; // Warp model is a rotation + symmetric zoom + translation
        public const int AFFINE = 3; // Warp model is a general affine transform
        public const int GM_ABS_TRANS_BITS = 12; // Number of bits encoded for translational components of global motion models, if part of a ROTZOOM or AFFINE model
        public const int GM_ABS_TRANS_ONLY_BITS = 9; // Number of bits encoded for translational components of global motion models, if part of a TRANSLATION model
        public const int GM_ABS_ALPHA_BITS = 12; // Number of bits encoded for non-translational components of global motion models
        public const int DIV_LUT_PREC_BITS = 14; // Number of fractional bits of entries in divisor lookup table
        public const int DIV_LUT_BITS = 8; // Number of fractional bits for lookup in divisor lookup table
        public const int DIV_LUT_NUM = 257; // Number of entries in divisor lookup table
        public const int MOTION_MODES = 3; // Number of values for motion modes
        public const int SIMPLE = 0; // Use translation or global motion compensation
        public const int OBMC = 1; // Use overlapped block motion compensation
        public const int LOCALWARP = 2; // Use local warp motion compensation
        public const int LEAST_SQUARES_SAMPLES_MAX = 8; // Largest number of samples used when computing a local warp
        public const int LS_MV_MAX = 256; // Largest motion vector difference to include in local warp computation
        public const int WARPEDMODEL_TRANS_CLAMP = 1 << 23; // Clamping value used for translation components of warp
        public const int WARPEDMODEL_NONDIAGAFFINE_CLAMP = 1 << 13; // Clamping value used for matrix components of warp
        public const int WARPEDPIXEL_PREC_SHIFTS = 1 << 6; // Number of phases used in warped filtering
        public const int WARPEDDIFF_PREC_BITS = 10; // Number of extra bits of precision in warped filtering
        public const int GM_ALPHA_PREC_BITS = 15; // Number of fractional bits for sending non translational warp model coefficients
        public const int GM_TRANS_PREC_BITS = 6; // Number of fractional bits for sending translational warp model coefficients
        public const int GM_TRANS_ONLY_PREC_BITS = 3; // Number of fractional bits used for pure translational warps
        public const int INTERINTRA_MODES = 4; // Number of inter intra modes
        public const int MASK_MASTER_SIZE = 64; // Size of MasterMask array
        public const int SEGMENT_ID_PREDICTED_CONTEXTS = 3; // Number of contexts for segment_id_predicted
        //public const int IS_INTER_CONTEXTS = 4; // Number of contexts for is_inter
        //public const int SKIP_CONTEXTS = 3; // Number of contexts for skip
        public const int FWD_REFS = 4; // Number of syntax elements for forward reference frames
        public const int BWD_REFS = 3; // Number of syntax elements for backward reference frames
        public const int SINGLE_REFS = 7; // Number of syntax elements for single reference frames
        public const int UNIDIR_COMP_REFS = 4; // Number of syntax elements for unidirectional compound reference frames
        public const int COMPOUND_TYPES = 2; // Number of values for compound_type
        public const int CFL_JOINT_SIGNS = 8; // Number of values for cfl_alpha_signs
        public const int CFL_ALPHABET_SIZE = 16; // Number of values for cfl_alpha_u and cfl_alpha_v
        public const int COMP_INTER_CONTEXTS = 5; // Number of contexts for comp_mode
        public const int COMP_REF_TYPE_CONTEXTS = 5; // Number of contexts for comp_ref_type
        public const int CFL_ALPHA_CONTEXTS = 6; // Number of contexts for cfl_alpha_u and cfl_alpha_v
        public const int INTRA_MODE_CONTEXTS = 5; // Number of each of left and above contexts for intra_frame_y_mode
        public const int COMP_GROUP_IDX_CONTEXTS = 6; // Number of contexts for comp_group_idx
        public const int COMPOUND_IDX_CONTEXTS = 6; // Number of contexts for compound_idx
        public const int INTRA_EDGE_KERNELS = 3; // Number of filter kernels for the intra edge filter
        public const int INTRA_EDGE_TAPS = 5; // Number of kernel taps for the intra edge filter
        public const int FRAME_LF_COUNT = 4; // Number of loop filter strength values
        public const int MAX_VARTX_DEPTH = 2; // Maximum depth for variable transform trees
        public const int TXFM_PARTITION_CONTEXTS = 21; // Number of contexts for txfm_split
        public const int REF_CAT_LEVEL = 640; // Bonus weight for close motion vectors
        public const int MAX_REF_MV_STACK_SIZE = 8; // Maximum number of motion vectors in the stack
        public const int MFMV_STACK_SIZE = 3; // Stack size for motion field motion vectors
        public const int MAX_TX_DEPTH = 2; // Maximum times the transform can be split
        public const int WEDGE_TYPES = 16; // Number of directions for the wedge mask process
        public const int FILTER_BITS = 7; // Number of bits used in Wiener filter coefficients
        public const int WIENER_COEFFS = 3; // Number of Wiener filter coefficients to read
        public const int SGRPROJ_PARAMS_BITS = 4; // Number of bits needed to specify self guided filter set
        public const int SGRPROJ_PRJ_SUBEXP_K = 4; // Controls how self guided deltas are read
        public const int SGRPROJ_PRJ_BITS = 7; // Precision bits during self guided restoration
        public const int SGRPROJ_RST_BITS = 4; // Restoration precision bits generated higher than source before projection
        public const int SGRPROJ_MTABLE_BITS = 20; // Precision of mtable division table
        public const int SGRPROJ_RECIP_BITS = 12; // Precision of division by n table
        public const int SGRPROJ_SGR_BITS = 8; // Internal precision bits for core selfguided_restoration
        public const int EC_PROB_SHIFT = 6; // Number of bits to reduce CDF precision during arithmetic coding
        public const int EC_MIN_PROB = 4; // Minimum probability assigned to each symbol during arithmetic coding
        public const int SELECT_SCREEN_CONTENT_TOOLS = 2; // Value that indicates the allow_screen_content_tools syntax element is coded
        public const int SELECT_INTEGER_MV = 2; // Value that indicates the force_integer_mv syntax element is coded
        public const int RESTORATION_TILESIZE_MAX = 256; // Maximum size of a loop restoration tile
        public const int MAX_FRAME_DISTANCE = 31; // Maximum distance when computing weighted prediction
        public const int MAX_OFFSET_WIDTH = 8; // Maximum horizontal offset of a projected motion vector
        public const int MAX_OFFSET_HEIGHT = 0; // Maximum vertical offset of a projected motion vector
        public const int WARP_PARAM_REDUCE_BITS = 6; // Rounding bitwidth for the parameters to the shear process
        public const int NUM_BASE_LEVELS = 2; // Number of quantizer base levels
        public const int COEFF_BASE_RANGE = 12; // The quantizer range above NUM_BASE_LEVELS above which the Exp-Golomb coding process is activated
        public const int BR_CDF_SIZE = 4; // Number of values for coeff_br
        public const int SIG_COEF_CONTEXTS_EOB = 4; // Number of contexts for coeff_base_eob
        public const int SIG_COEF_CONTEXTS_2D = 26; // Context offset for coeff_base for horizontal-only or vertical-only transforms.
        public const int SIG_COEF_CONTEXTS = 42; // Number of contexts for coeff_base
        public const int SIG_REF_DIFF_OFFSET_NUM = 5; // Maximum number of context samples to be used in determining the context index for coeff_base and coeff_base_eob.
        public const int SUPERRES_NUM = 8; // Numerator for upscaling ratio
        public const int SUPERRES_DENOM_MIN = 9; // Smallest denominator for upscaling ratio
        public const int SUPERRES_DENOM_BITS = 3; // Number of bits sent to specify denominator of upscaling ratio
        public const int SUPERRES_FILTER_BITS = 6; // Number of bits of fractional precision for upscaling filter selection 
        public const int SUPERRES_FILTER_SHIFTS = 1 << SUPERRES_FILTER_BITS; // Number of phases of upscaling filters
        public const int SUPERRES_FILTER_TAPS = 8; // Number of taps of upscaling filters
        public const int SUPERRES_FILTER_OFFSET = 3; // Sample offset for upscaling filters
        public const int SUPERRES_SCALE_BITS = 14; // Number of fractional bits for computing position in upscaling
        public const int SUPERRES_SCALE_MASK = (1 << 14) - 1; // Mask for computing position in upscaling
        public const int SUPERRES_EXTRA_BITS = 8; // Difference in precision between SUPERRES_SCALE_BITS and SUPERRES_FILTER_BITS
        public const int TXB_SKIP_CONTEXTS = 13; // Number of contexts for all_zero
        public const int EOB_COEF_CONTEXTS = 9; // Number of contexts for eob_extra
        public const int DC_SIGN_CONTEXTS = 3; // Number of contexts for dc_sign
        public const int LEVEL_CONTEXTS = 21; // Number of contexts for coeff_br
        public const int TX_CLASS_2D = 0; // Transform class for transform types performing non-identity transforms in both directions
        public const int TX_CLASS_HORIZ = 1; // Transform class for transforms performing only a horizontal non-identity transform
        public const int TX_CLASS_VERT = 2; // Transform class for transforms performing only a vertical non-identity transform
        public const int REFMVS_LIMIT = (1 << 12) - 1; // Largest reference MV component that can be saved
        public const int INTRA_FILTER_SCALE_BITS = 4; // Scaling shift for intra filtering process
        public const int INTRA_FILTER_MODES = 5; // Number of types of intra filtering
        public const int COEFF_CDF_Q_CTXS = 4; // Number of selectable context types for the coeff() syntax structure
        public const int PRIMARY_REF_NONE = 7; // Value of primary_ref_frame indicating that there is no primary reference frame
        public const int BUFFER_POOL_MAX_SIZE = 10; // Number of frames in buffer pool
    }

    public static class AV1ColorPrimaries
    {
        public const int CP_BT_709 = 1;
        public const int CP_UNSPECIFIED = 2;
        public const int CP_BT_470_M = 4;
        public const int CP_BT_470_B_G = 5;
        public const int CP_BT_601 = 6;
        public const int CP_SMPTE_240 = 7;
        public const int CP_GENERIC_FILM = 8;
        public const int CP_BT_2020 = 9;
        public const int CP_XYZ = 10;
        public const int CP_SMPTE_431 = 11;
        public const int CP_SMPTE_432 = 12;
        public const int CP_EBU_3213 = 22;
    }

    public static class AV1TransferCharacteristics
    {
        public const int TC_RESERVED_0 = 0;
        public const int TC_BT_709 = 1;
        public const int TC_UNSPECIFIED = 2;
        public const int TC_RESERVED_3 = 3;
        public const int TC_BT_470_M = 4;
        public const int TC_BT_470_B_G = 5;
        public const int TC_BT_601 = 6;
        public const int TC_SMPTE_240 = 7;
        public const int TC_LINEAR = 8;
        public const int TC_LOG_100 = 9;
        public const int TC_LOG_100_SQRT10 = 10;
        public const int TC_IEC_61966 = 11;
        public const int TC_BT_1361 = 12;
        public const int TC_SRGB = 13;
        public const int TC_BT_2020_10_BIT = 14;
        public const int TC_BT_2020_12_BIT = 15;
        public const int TC_SMPTE_2084 = 16;
        public const int TC_SMPTE_428 = 17;
        public const int TC_HLG = 18;
    }

    public static class AV1MatrixCoefficients
    {
        public const int MC_IDENTITY = 0;
        public const int MC_BT_709 = 1;
        public const int MC_UNSPECIFIED = 2;
        public const int MC_RESERVED_3 = 3;
        public const int MC_FCC = 4;
        public const int MC_BT_470_B_G = 5;
        public const int MC_BT_601 = 6;
        public const int MC_SMPTE_240 = 7;
        public const int MC_SMPTE_YCGCO = 8;
        public const int MC_BT_2020_NCL = 9;
        public const int MC_BT_2020_CL = 10;
        public const int MC_SMPTE_2085 = 11;
        public const int MC_CHROMAT_NCL = 12;
        public const int MC_CHROMAT_CL = 13;
        public const int MC_ICTCP = 14;
    }

    public static class AV1ChromaSamplePosition
    {
        public const int CSP_UNKNOWN = 0;
        public const int CSP_VERTICAL = 1;
        public const int CSP_COLOCATED = 2;
        public const int CSP_RESERVED = 3;
    }

    public static class AV1FrameTypes
    {
        public const int KEY_FRAME = 0;
        public const int INTER_FRAME = 1;
        public const int INTRA_ONLY_FRAME = 2;
        public const int SWITCH_FRAME = 3;
    }

    public static class AV1MetadataType
    {
        public const int METADATA_TYPE_HDR_CLL = 1;
        public const int METADATA_TYPE_HDR_MDCV = 2;
        public const int METADATA_TYPE_SCALABILITY = 3;
        public const int METADATA_TYPE_ITUT_T35 = 4;
        public const int METADATA_TYPE_TIMECODE = 5;
    }

    public static class AV1FrameRestorationType
    {
        public const int RESTORE_NONE = 0;
        public const int RESTORE_SWITCHABLE = 3;
        public const int RESTORE_WIENER = 1;
        public const int RESTORE_SGRPROJ = 2;
    }

    public static class AV1ScalabilityModeIdc
    {
        public const int SCALABILITY_L1T2 = 0;
        public const int SCALABILITY_L1T3 = 1;
        public const int SCALABILITY_L2T1 = 2;
        public const int SCALABILITY_L2T2 = 3;
        public const int SCALABILITY_L2T3 = 4;
        public const int SCALABILITY_S2T1 = 5;
        public const int SCALABILITY_S2T2 = 6;
        public const int SCALABILITY_S2T3 = 7;
        public const int SCALABILITY_L2T1h = 8;
        public const int SCALABILITY_L2T2h = 9;
        public const int SCALABILITY_L2T3h = 10;
        public const int SCALABILITY_S2T1h = 11;
        public const int SCALABILITY_S2T2h = 12;
        public const int SCALABILITY_S2T3h = 13;
        public const int SCALABILITY_SS = 14;
        public const int SCALABILITY_L3T1 = 15;
        public const int SCALABILITY_L3T2 = 16;
        public const int SCALABILITY_L3T3 = 17;
        public const int SCALABILITY_S3T1 = 18;
        public const int SCALABILITY_S3T2 = 19;
        public const int SCALABILITY_S3T3 = 20;
        public const int SCALABILITY_L3T2_KEY = 21;
        public const int SCALABILITY_L3T3_KEY = 22;
        public const int SCALABILITY_L4T5_KEY = 23;
        public const int SCALABILITY_L4T7_KEY = 24;
        public const int SCALABILITY_L3T2_KEY_SHIFT = 25;
        public const int SCALABILITY_L3T3_KEY_SHIFT = 26;
        public const int SCALABILITY_L4T5_KEY_SHIFT = 27;
        public const int SCALABILITY_L4T7_KEY_SHIFT = 28;
    }

    public static class AV1TxModes
    {
        public const int ONLY_4X4 = 0;
        public const int TX_MODE_LARGEST = 1;
        public const int TX_MODE_SELECT = 2;
    }

    public static class AV1InterpolationFilter
    {
        public const int EIGHTTAP = 0;
        public const int EIGHTTAP_SMOOTH = 1;
        public const int EIGHTTAP_SHARP = 2;
        public const int BILINEAR = 3;
        public const int SWITCHABLE = 4;
    }
}
