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
                ReadConfigObu(obu);
        }

        public override bool Supports(string scheme) => scheme == ProtectionSchemes.Cenc || scheme == ProtectionSchemes.Cbcs;

        /// <summary>The subsamples of a sample: of each tile, the bytes before it clear and the tile protected.</summary>
        public override EncryptionSubsample[] Split(byte[] buffer, int offset, int length, bool wholeBlocks)
        {
            var subsamples = new List<EncryptionSubsample>();
            long clear = 0;
            int position = 0;

            // one stream over the sample: each OBU's size, then the OBU, the tiles' positions counted from the sample's start
            using (var stream = new AomStream(new MemoryStream(buffer, offset, length, writable: false)))
            {
                while (stream.GetPosition() < (long)length * 8)
                {
                    int size = AV2Context.ReadObuSize(stream, (long)length * 8);
                    if (size < 0)
                        break;

                    // an OBU that cannot be read is left clear: the next is read from where it starts
                    var error = _context.ReadWhole(stream, size);
                    if (error != null)
                    {
                        if (Logger.IsWarningEnabled)
                            Logger.LogWarning($"An OBU could not be read, it is left clear: {error.Message}");
                        continue;
                    }
                    for (int i = 0; i < _context.TileCount; i++)
                        AddTile(subsamples, ref clear, ref position, (int)(_context.TileStart(i) / 8), _context.TileSize(i), wholeBlocks);
                }
            }

            // no run of none clear and none protected (9.5.1): an empty sample has no subsample at all
            clear += length - position;
            if (clear > 0)
                Add(subsamples, ref clear, 0);
            return subsamples.ToArray();
        }

        /// <summary>Reads an OBU of the 'av2C' - not led by its size - into the context.</summary>
        private void ReadConfigObu(byte[] obu)
        {
            try
            {
                using (var stream = new AomStream(new MemoryStream(obu, writable: false)))
                    _context.Read(stream, obu.Length);
            }
            catch (Exception ex)
            {
                if (Logger.IsWarningEnabled)
                    Logger.LogWarning($"An OBU of the 'av2C' could not be read: {ex.Message}");
            }
        }
    }
}
