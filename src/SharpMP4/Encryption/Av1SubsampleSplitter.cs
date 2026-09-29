using SharpAV1;
using SharpAVX;
using SharpISOBMFF;
using SharpMP4.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// Splits a sample of AV1's OBUs into what is left in the clear and what is protected, as the AV1 binding of ISO BMFF
    /// says (AV1 Codec ISO Media File Format Binding, 4.2): of each tile the tile data is protected, one subsample to a
    /// tile, and all else stays clear - the OBU headers and sizes, the temporal delimiters, sequence headers, frame headers,
    /// metadata and padding, and the tile group headers and tile sizes. 'cbcs' protects the whole of each tile; 'cenc' the
    /// whole blocks at its end, a tile of less than a block left clear. The binding allows only these two.
    /// </summary>
    public sealed class Av1SubsampleSplitter : SubsampleSplitter
    {
        // every layer read, so that the tiles of each are found and protected
        private readonly AV1Context _context = new AV1Context { AllLayers = 1 };

        public Av1SubsampleSplitter(AV1CodecConfigurationRecord config, IMp4Logger logger) : base(logger)
        {
            // the sequence header the frame headers after it depend on
            var configObus = config?.ConfigOBUs;
            if (configObus != null && configObus.Length > 0)
                Read(configObus, 0, configObus.Length, null);
        }

        public override bool Supports(string scheme) => scheme == ProtectionSchemes.Cenc || scheme == ProtectionSchemes.Cbcs;

        /// <summary>The subsamples of a sample: of each tile, the bytes before it clear and the tile protected.</summary>
        public override EncryptionSubsample[] Split(byte[] buffer, int offset, int length, bool wholeBlocks)
        {
            var subsamples = new List<EncryptionSubsample>();
            long clear = 0;
            int position = 0;

            Read(buffer, offset, length, (start, size) => AddTile(subsamples, ref clear, ref position, start, size, wholeBlocks));

            // no run of none clear and none protected (9.5.1): an empty sample has no subsample at all
            clear += length - position;
            if (clear > 0)
                Add(subsamples, ref clear, 0);
            return subsamples.ToArray();
        }

        /// <summary>
        /// Reads the OBUs of a sample into the context, giving each tile's start - in bytes from the sample's - and size
        /// to <paramref name="tile"/>. Where an OBU cannot be read, the rest is left as it is: clear.
        /// </summary>
        private void Read(byte[] buffer, int offset, int length, Action<int, int> tile)
        {
            using (var stream = new AomStream(new MemoryStream(buffer, offset, length, writable: false)))
            {
                int position = 0;
                while (position < length)
                {
                    try
                    {
                        _context.Read(stream, length - position);
                    }
                    catch (Exception ex)
                    {
                        if (Logger.IsWarningEnabled)
                            Logger.LogWarning($"An OBU could not be read, the rest of the sample is left clear: {ex.Message}");
                        return;
                    }

                    int headerBytes = 1 + (_context._ObuExtensionFlag != 0 ? 1 : 0) + (_context.ObuSizeLen >> 3);
                    int obuBytes = _context._ObuHasSizeField != 0 ? headerBytes + _context._ObuSize : length - position;

                    int type = _context._ObuType;
                    if (type == AV1ObuTypes.OBU_FRAME_HEADER || type == AV1ObuTypes.OBU_FRAME)
                    {
                        // what a redundant frame header after it repeats
                        _context.LastObuFrameHeader = new byte[_context._ObuSize];
                        Buffer.BlockCopy(buffer, offset + position + headerBytes, _context.LastObuFrameHeader, 0, _context._ObuSize);
                    }

                    if (tile != null && (type == AV1ObuTypes.OBU_TILE_GROUP || type == AV1ObuTypes.OBU_FRAME))
                    {
                        for (int i = 0; i < _context.TileCount; i++)
                            tile((int)(_context.TileStart(i) / 8), _context.TileSize(i));
                    }

                    if (obuBytes <= 0)
                        return;
                    position += obuBytes;
                }
            }
        }
    }
}
