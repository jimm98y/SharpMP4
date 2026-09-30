using SharpAVX;
using SharpMP4.Common;
using SharpVP9;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// Splits a sample of VP9 into what is left in the clear and what is protected, as the VP Codec ISO Media File Format
    /// Binding says (Common Encryption): of each frame - of a superframe too, one subsample to a frame - its uncompressed
    /// header is clear, and the rest is protected, the compressed header with the tiles; the superframe index is clear.
    /// So that the counter of each frame can be worked out, the protected bytes of every frame are whole blocks, of every
    /// scheme: the bytes left over put in the clear before them, as Shaka Packager does. A frame shown again is its header
    /// alone, and clear.
    /// </summary>
    public sealed class Vp9SubsampleSplitter : SubsampleSplitter
    {
        // the headers of a frame depend on those before it: the sizes of the frames it refers to
        private readonly VP9Context _context = new VP9Context();

        public Vp9SubsampleSplitter(IMp4Logger logger) : base(logger)
        {
        }

        /// <summary>The subsamples of a sample. The protected bytes are whole blocks whatever <paramref name="wholeBlocks"/> says.</summary>
        public override EncryptionSubsample[] Split(byte[] buffer, int offset, int length, bool wholeBlocks)
        {
            var subsamples = new List<EncryptionSubsample>();
            long clear = 0;
            int position = 0;

            int[] sizes = VP9Context.SuperframeFrameSizes(buffer, offset, length) ?? new[] { length };
            int start = 0;
            foreach (int size in sizes)
            {
                if (size <= 0 || size > length - start)
                    break;

                int header;
                try
                {
                    using (var stream = new AomStream(new MemoryStream(buffer, offset + start, size, writable: false)))
                        _context.Read(stream, size);
                    // a frame shown again has no compressed header, nor tiles: all of it is its header
                    header = _context.CompressedHeaderSize > 0 ? _context.UncompressedHeaderSize : size;
                }
                catch (Exception ex)
                {
                    if (Logger.IsWarningEnabled)
                        Logger.LogWarning($"A VP9 frame could not be read, the rest of the sample is left clear: {ex.Message}");
                    break;
                }

                AddTile(subsamples, ref clear, ref position, start + header, size - header, wholeBlocks: true);
                start += size;
            }

            // the superframe index, and what could not be read, clear; no run of none clear and none protected (9.5.1)
            clear += length - position;
            if (clear > 0)
                Add(subsamples, ref clear, 0);
            return subsamples.ToArray();
        }
    }
}
