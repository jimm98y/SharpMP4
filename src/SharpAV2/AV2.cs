using SharpAVX;
using System;
using System.IO;

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

    /// <summary>
    /// An OBU as it was read, for writing it again: its size, its syntax elements as they were read, and
    /// the state they were read into. Changed through <see cref="Edit"/>, it is written with the changes.
    /// </summary>
    public sealed class AV2Obu
    {
        internal AV2Obu(int size, AomSyntaxRecord record, AV2Context read)
        {
            Size = size;
            Record = record;
            Read = read;
        }

        /// <summary>The bytes of the OBU after its obu_size (sz of open_bitstream_unit).</summary>
        public int Size { get; set; }

        /// <summary>Every occurrence of each syntax element, as it was read.</summary>
        public AomSyntaxRecord Record { get; }

        internal AV2Context Read { get; }

        /// <summary>
        /// The bytes of an OBU that could not be read - to the end of what it was read from - and so is
        /// written as it was; null for one that was read.
        /// </summary>
        public byte[] Unreadable { get; internal set; }

        /// <summary>The state to write the OBU from, if it has been changed; null, it is written as it was read.</summary>
        public AV2Context Edited { get; private set; }

        /// <summary>
        /// The state the OBU was read into, to change before it is written: the elements it gives other
        /// values are written with them. A change that makes the OBU longer or shorter needs its Size too.
        /// </summary>
        public AV2Context Edit() => Edited ??= Read.Copy();

        /// <summary>
        /// The OBU without its record: written from the state it was read into, as an encoder writes from
        /// its own - what that state does not say, such as the tile data, it cannot write.
        /// </summary>
        public AV2Obu FromState() => new AV2Obu(Size, new AomSyntaxRecord(), Read) { Edited = Edited };
    }

    public partial class AV2Context
    {
        private AomStream stream;

        /// <summary>The operating point a decoder would select.</summary>
        public int SelectedOperatingPoint { get; set; } = 0;

        public byte[] LastObuFrameHeader { get; set; }

        /// <summary>The bits of the last OBU's obu_size.</summary>
        public int ObuSizeLen { get; private set; }

        /// <summary>True to keep, of each OBU read, what writing it again takes: <see cref="LastObu"/>.</summary>
        public bool RecordSyntax { get; set; }

        /// <summary>With <see cref="RecordSyntax"/>, the last OBU read.</summary>
        public AV2Obu LastObu { get; private set; }

        // True while an OBU is written.
        private bool _writing;

        /// <summary>
        /// An arithmetic coded element (L(n), S(), NS(n)): the tile data's, which SharpAV2 does not decode.
        /// The headers reach none; a syntax structure they share with the tile data has them on its other path.
        /// </summary>
        private ulong ReadArithmetic(out int value, string name)
        {
            throw new NotSupportedException($"{name} is arithmetic coded: SharpAV2 does not decode tile data.");
        }

        private ulong WriteArithmetic(int value, string name)
        {
            throw new NotSupportedException($"{name} is arithmetic coded: SharpAV2 does not encode tile data.");
        }

        /// <summary>decode_4part( num ) (5.20.10.6): arithmetic coded, the tile data's.</summary>
        private int decode_4part(int num)
        {
            ReadArithmetic(out int value, "wiener_ns_base");
            return value;
        }

        /// <summary>The bit position just after the OBU being read or written.</summary>
        private long ObuEndPosition;

        /// <summary>Reads one OBU of sz bytes (AV2 5.3.1, open_bitstream_unit).</summary>
        public void Read(AomStream stream, int size)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            ObuEndPosition = stream.GetPosition() + (long)size * 8;
            _paddingLength = -1;
            TileCount = 0;
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
                LastObu = new AV2Obu(size, record, Copy()) { Unreadable = bytes };
                throw;
            }
            finally
            {
                stream.Record = null;
            }
            if (record != null)
                LastObu = new AV2Obu(size, record, Copy());
        }

        /// <summary>
        /// Writes an OBU read with <see cref="RecordSyntax"/>, after its obu_size: as it was read, but for
        /// the changes made to it (<see cref="AV2Obu.Edit"/>). This context must have written the OBUs
        /// before it, as another read them: its state is a decoder's, which the syntax depends on.
        /// </summary>
        public void Write(AomStream stream, AV2Obu obu)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (obu == null)
                throw new ArgumentNullException(nameof(obu));
            ObuEndPosition = stream.GetPosition() + (long)obu.Size * 8;
            _paddingLength = -1;
            TileCount = 0;
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

        /// <summary>A copy of the state: what the context holds, but none of what it reads or writes.</summary>
        internal AV2Context Copy()
        {
            var copy = new AV2Context { SelectedOperatingPoint = SelectedOperatingPoint };
            copy.LoadContext(SaveContext(), copy: false);
            return copy;
        }
    }
}
