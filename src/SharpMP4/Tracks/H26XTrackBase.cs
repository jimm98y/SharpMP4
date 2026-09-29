using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;
using SharpH26X;

namespace SharpMP4.Tracks
{
    public abstract class H26XTrackBase : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        public int NalLengthSize { get; set; } = 4;

        protected H26XTrackBase() : base()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 24000;
            FrameTickFallback = 1001;
        }

        private ItuStream _nalStream;

        /// <summary>
        /// The stream a NAL unit is read with: the track's one, reset to each unit, so reading a unit allocates no stream.
        /// It is the track's until the next call, so it is not kept.
        /// </summary>
        protected ItuStream NalStream(byte[] buffer, int offset, int length)
        {
            if (_nalStream == null)
                _nalStream = new ItuStream(buffer, offset, length, Logger);
            else
                _nalStream.Reset(buffer, offset, length);

            _nalStream.Logger = Logger;
            return _nalStream;
        }

        /// <summary>Whether the access unit being assembled holds a coded picture.</summary>
        protected bool SampleHasVcl { get; set; }

        /// <summary>
        /// Whether the access unit being assembled is a random access point - a sync sample (ISO/IEC 14496-15): each of its
        /// VCL NAL units one of a picture decoding can start at. See <see cref="AddVclNalUnit"/>.
        /// </summary>
        protected bool SampleHasIdr { get; set; }

        // The nuh_layer_id of the last VCL NAL unit: a picture of a layer no higher starts the next access unit.
        private int _lastVclLayerId = -1;

        /// <summary>
        /// Whether a VCL NAL unit that starts a picture starts a new access unit too: where it is of a layer no higher than
        /// the picture before it - as the base layer's picture, which starts an access unit of more than one layer.
        /// </summary>
        protected bool StartsAccessUnit(int layerId) => SampleHasVcl && layerId <= _lastVclLayerId;

        /// <summary>
        /// A VCL NAL unit of the access unit being assembled, once the one before it has been taken: the access unit is a
        /// random access point while each of its VCL NAL units is of one.
        /// </summary>
        protected void AddVclNalUnit(bool isRandomAccessPoint, int layerId = 0)
        {
            SampleHasIdr = SampleHasVcl ? SampleHasIdr && isRandomAccessPoint : isRandomAccessPoint;
            SampleHasVcl = true;
            _lastVclLayerId = layerId;
        }

        /// <summary>
        /// Adds a NAL unit to the access unit being assembled, with the length in front of it.
        /// The bytes are copied, so the caller is free to write over them.
        /// </summary>
        protected void AppendNalUnit(byte[] buffer, int offset, int length)
        {
            if (length > MaxNalUnitLength)
                throw new ArgumentOutOfRangeException(nameof(length),
                    $"a NAL unit of {length} bytes does not fit a {NalLengthSize} byte length");

            for (int i = NalLengthSize - 1; i >= 0; i--)
                AppendToSample((byte)(length >> (i * 8)));

            AppendToSample(buffer, offset, length);
        }

        // NAL units that came after the last VCL NAL unit of the access unit being assembled and that start the next one
        // where the VCL NAL unit after them is of a new one - access unit delimiters, prefix SEI, parameter sets kept in the
        // samples, picture headers - each with its length before it: which access unit they are of, only the VCL NAL unit
        // after them says (H.264 7.4.1.2.3, H.265 7.4.2.4.4 and Annex F, H.266 7.4.2.4.3). A prefix SEI of the base
        // layer between the base layer's picture and another layer's, or a prefix NAL unit before each slice of an MVC
        // base view, is of the access unit it is in.
        private byte[] _held = new byte[1024];
        private int _heldLength;

        /// <summary>
        /// A NAL unit that starts the next access unit if a new one comes next: held until the next VCL NAL unit says. Before
        /// the access unit has one, it is the access unit's.
        /// </summary>
        protected void HoldNalUnit(byte[] buffer, int offset, int length)
        {
            if (!SampleHasVcl)
            {
                AppendNalUnit(buffer, offset, length);
                return;
            }

            if (_heldLength + NalLengthSize + length > _held.Length)
                Array.Resize(ref _held, Math.Max(_held.Length * 2, _heldLength + NalLengthSize + length));
            for (int i = NalLengthSize - 1; i >= 0; i--)
                _held[_heldLength++] = (byte)(length >> (i * 8));
            System.Buffer.BlockCopy(buffer, offset, _held, _heldLength, length);
            _heldLength += length;
        }

        /// <summary>The NAL units held put in the access unit being assembled: of it, as what comes next says.</summary>
        protected void AttachHeldNalUnits()
        {
            if (_heldLength == 0)
                return;
            AppendToSample(_held, 0, _heldLength);
            _heldLength = 0;
        }

        /// <summary>
        /// A VCL NAL unit, before it is added: where it starts a new access unit, the one assembled is taken and the NAL
        /// units held start the new one; where not, they are of the one assembled.
        /// </summary>
        protected ArraySegment<byte> StartVclNalUnit(bool startsAccessUnit)
        {
            ArraySegment<byte> output = default;
            if (startsAccessUnit && SampleHasVcl)
                output = TakeSample();
            AttachHeldNalUnits();
            return output;
        }

        /// <summary>The last access unit, at the end of the stream: with what was held after it.</summary>
        protected ArraySegment<byte> FlushAccessUnit()
        {
            AttachHeldNalUnits();
            return HasSample && SampleHasVcl ? TakeSample() : default;
        }

        /// <summary>
        /// What the NAL units of a configuration record put in the sample being assembled - its declarative SEI messages -
        /// dropped: they are no sample's, and the samples bring their own.
        /// </summary>
        protected void DropConfigurationSample()
        {
            _heldLength = 0;
            if (HasSample)
                TakeSample();
        }

        /// <summary>The finished access unit, which also ends what was being said about it.</summary>
        protected new ArraySegment<byte> TakeSample()
        {
            SampleHasVcl = false;
            SampleHasIdr = false;
            return base.TakeSample();
        }

        private long MaxNalUnitLength => NalLengthSize >= 4 ? uint.MaxValue : (1L << (NalLengthSize * 8)) - 1;

        /// <summary>
        /// The NAL units of a sample, whichever way it carries them: each one behind its length,
        /// as a sample in a file holds them, or behind a start code, as an encoder hands them out
        /// and as a .264 or .265 file holds them.
        /// </summary>
        public override IEnumerable<ArraySegment<byte>> ParseSample(byte[] buffer, int offset, int length)
        {
            if (buffer != null && length >= 3 && buffer[offset] == 0 && buffer[offset + 1] == 0
                && (buffer[offset + 2] == 1 || (length >= 4 && buffer[offset + 2] == 0 && buffer[offset + 3] == 1)))
            {
                return ParseSample(new MemoryStream(buffer, offset, length));
            }

            return ParseLengthPrefixed(buffer, offset, length);
        }

        /// <summary>
        /// Reading an Annex B stream: the chunk in hand and how far through it the read has got,
        /// the NAL unit being built, and the zero bytes held back - which belong to a start code,
        /// or to the unit, and which of the two is only known once the byte after them arrives.
        /// </summary>
        private byte[] _readBuffer;
        private int _readAvailable;
        private int _readPosition;
        private byte[] _nalUnit = new byte[4096];
        private int _nalUnitLength;
        private int _zeros;
        private bool _startedNalUnit;
        private bool _parsing;

        /// <summary>
        /// The NAL units of an Annex B stream - an encoder's output, or a .264 or .265 file - one
        /// at a time as the stream is read. Nothing holds the stream and nothing holds a list of
        /// what has been read, so a stream of any size costs the read buffer and the largest NAL
        /// unit in it.
        /// </summary>
        /// <param name="bufferSize">How much of the stream is read at a time.</param>
        public IEnumerable<ArraySegment<byte>> ParseSample(Stream sample, int bufferSize = 64 * 1024)
        {
            if (sample == null)
                throw new ArgumentNullException(nameof(sample));
            if (bufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            // The reading state is the track's, so one stream is read at a time. Reading two at
            // once would have them write over each other's half finished NAL unit.
            if (_parsing)
                throw new InvalidOperationException("this track is already reading a stream");

            _parsing = true;
            try
            {
                if (_readBuffer == null || _readBuffer.Length != bufferSize)
                    _readBuffer = new byte[bufferSize];

                _readAvailable = 0;
                _readPosition = 0;
                _nalUnitLength = 0;
                _zeros = 0;
                _startedNalUnit = false;

                // A slice of the buffer the next NAL unit is read into: it is used before the
                // next one is asked for, or copied by whoever keeps it.
                while (ReadNalUnit(sample))
                    yield return new ArraySegment<byte>(_nalUnit, 0, _nalUnitLength);
            }
            finally
            {
                _parsing = false;
            }
        }

        /// <summary>
        /// Reads up to the start of the next NAL unit, leaving the one before it in
        /// <see cref="_nalUnit"/>. False once the stream holds no more.
        /// </summary>
        private bool ReadNalUnit(Stream stream)
        {
            _nalUnitLength = 0;

            while (true)
            {
                if (_readPosition == _readAvailable)
                {
                    _readAvailable = stream.Read(_readBuffer, 0, _readBuffer.Length);
                    _readPosition = 0;

                    if (_readAvailable <= 0)
                    {
                        // Whatever is in hand at the end of the stream is the last NAL unit. The
                        // zero bytes held back are trailing_zero_8bits and are not part of it.
                        _startedNalUnit = false;
                        return _nalUnitLength > 0;
                    }
                }

                while (_readPosition < _readAvailable)
                {
                    byte value = _readBuffer[_readPosition++];

                    if (value == 0)
                    {
                        _zeros++;
                        continue;
                    }

                    // 00 00 01, with any number of further zeros in front of it, starts a NAL
                    // unit. The sequence cannot occur inside one: an encoder puts an emulation
                    // prevention byte in the way.
                    if (value == 1 && _zeros >= 2)
                    {
                        _zeros = 0;

                        if (_startedNalUnit && _nalUnitLength > 0)
                            return true;

                        _startedNalUnit = true;
                        _nalUnitLength = 0;
                        continue;
                    }

                    if (_startedNalUnit)
                    {
                        // Zeros that turned out not to start anything belong to the unit.
                        while (_zeros > 0)
                        {
                            AppendToNalUnit(0);
                            _zeros--;
                        }

                        AppendToNalUnit(value);
                    }

                    _zeros = 0;
                }
            }
        }

        private void AppendToNalUnit(byte value)
        {
            if (_nalUnitLength == _nalUnit.Length)
                Array.Resize(ref _nalUnit, _nalUnit.Length * 2);

            _nalUnit[_nalUnitLength++] = value;
        }

        private IEnumerable<ArraySegment<byte>> ParseLengthPrefixed(byte[] buffer, int offset, int length)
        {
            if (this.Logger.IsDebugEnabled) this.Logger.LogDebug($"{nameof(H26XTrackBase)}: AU begin {length}");

            int position = 0;
            while (position + NalLengthSize <= length)
            {
                long nalUnitLength = 0;
                for (int i = 0; i < NalLengthSize; i++)
                    nalUnitLength = (nalUnitLength << 8) | buffer[offset + position + i];

                position += NalLengthSize;

                if (nalUnitLength > length - position)
                {
                    if (this.Logger.IsErrorEnabled) this.Logger.LogError($"{nameof(H26XTrackBase)}: Invalid NALU size: {nalUnitLength}");
                    nalUnitLength = length - position;
                }

                yield return new ArraySegment<byte>(buffer, offset + position, (int)nalUnitLength);
                position += (int)nalUnitLength;
            }

            if (position != length)
                throw new Exception("Mismatch!");

            if (this.Logger.IsDebugEnabled) this.Logger.LogDebug($"{nameof(H26XTrackBase)}: AU end");
        }

        /// <summary>
        /// Reads an Annex B byte stream - what an encoder hands out, and what a .264 or .265 file
        /// holds - as NAL units, for <see cref="ProcessAnnexB"/> to feed in one at a time.
        /// </summary>
        /// <remarks>
        /// The stream is read as it goes, a chunk at a time, and each NAL unit is built in a buffer
        /// that is reused for the next one. Nothing holds the stream, and nothing holds a list of
        /// what has been read, so an elementary stream of any size costs the read buffer plus the
        /// largest NAL unit in it.
        /// </remarks>
    }
}
