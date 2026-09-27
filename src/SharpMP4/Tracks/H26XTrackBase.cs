using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>Whether the access unit being assembled holds a coded picture.</summary>
        protected bool SampleHasVcl { get; set; }

        /// <summary>Whether the access unit being assembled starts at a random access point.</summary>
        protected bool SampleHasIdr { get; set; }

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
