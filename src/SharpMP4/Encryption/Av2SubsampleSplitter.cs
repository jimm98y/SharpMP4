using SharpAV2;
using SharpAVX;
using SharpISOBMFF;
using SharpMP4.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// Splits a sample of AV2's OBUs - each led by its leb128() size - into what is left in the clear and what is protected.
    /// The AV2 binding of ISO BMFF has no Common Encryption yet (AOMediaCodec/av2-isobmff#13): this protects the tiles as
    /// the AV1 binding does, one subsample to a tile, all else clear - the OBU sizes and headers, the frame headers and tile
    /// group headers, the tile sizes, and every OBU but a tile group's. A bridge frame, and an inactive BRU frame, have no
    /// tile data, and stay clear whole. 'cbcs' protects the whole of each tile, 'cenc' the whole blocks at its end.
    /// </summary>
    public sealed class Av2SubsampleSplitter : SubsampleSplitter
    {
        private readonly AV2Context _context = new AV2Context();

        public Av2SubsampleSplitter(AV2CodecConfigurationBox config, IMp4Logger logger) : base(logger)
        {
            // the sequence header, and the rest the samples depend on, which the samples do not repeat
            foreach (byte[] obu in config?.ConfigObus ?? Array.Empty<byte[]>())
                ReadObu(obu, 0, obu.Length);
        }

        public override bool Supports(string scheme) => scheme == ProtectionSchemes.Cenc || scheme == ProtectionSchemes.Cbcs;

        /// <summary>The subsamples of a sample: of each tile, the bytes before it clear and the tile protected.</summary>
        public override EncryptionSubsample[] Split(byte[] buffer, int offset, int length, bool wholeBlocks)
        {
            var subsamples = new List<EncryptionSubsample>();
            long clear = 0;
            int position = 0;

            for (int obu = 0; obu < length;)
            {
                int payload = obu;
                long size = ReadLeb128(buffer, offset, length, ref payload);
                if (size <= 0 || payload + size > length || !ReadObu(buffer, offset + payload, (int)size))
                    break;

                // the tiles, where they are in the OBU: from the sample's start
                for (int i = 0; i < _context.TileCount; i++)
                    AddTile(subsamples, ref clear, ref position, payload + (int)(_context.TileStart(i) / 8), _context.TileSize(i), wholeBlocks);
                obu = payload + (int)size;
            }

            // no run of none clear and none protected (9.5.1): an empty sample has no subsample at all
            clear += length - position;
            if (clear > 0)
                Add(subsamples, ref clear, 0);
            return subsamples.ToArray();
        }

        /// <summary>Reads an OBU into the context; false where it cannot be, the rest of the sample then left clear.</summary>
        private bool ReadObu(byte[] buffer, int offset, int size)
        {
            try
            {
                using (var stream = new AomStream(new MemoryStream(buffer, offset, size, writable: false)))
                    _context.Read(stream, size);
                return true;
            }
            catch (Exception ex)
            {
                if (Logger.IsWarningEnabled)
                    Logger.LogWarning($"An OBU could not be read, the rest of the sample is left clear: {ex.Message}");
                return false;
            }
        }

        private static long ReadLeb128(byte[] buffer, int offset, int length, ref int position)
        {
            long value = 0;
            for (int i = 0; i < 8 && position < length; i++)
            {
                byte b = buffer[offset + position++];
                value |= (long)(b & 0x7f) << (7 * i);
                if ((b & 0x80) == 0)
                    return value;
            }
            return -1;
        }
    }
}
