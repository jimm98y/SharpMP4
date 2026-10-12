using SharpISOBMFF;
using SharpMP4.Common;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Builders
{
    /// <summary>The format of a HEIF image a builder writes (ISO/IEC 23008-12): of the codec its items are of.</summary>
    public enum ImageFormat
    {
        /// <summary>HEIC: HEVC images ('hvc1' items), 'heic', of 'mif1'.</summary>
        Heic,

        /// <summary>AVIF: AV1 images ('av01' items), 'avif', of 'mif1' and 'miaf' (AV1 Image File Format).</summary>
        Avif,
    }

    /// <summary>What <see cref="ImageBuilder.Create"/> makes a builder with.</summary>
    public class ImageBuilderOptions
    {
        /// <summary>
        /// Of an image of tiles - a 'grid' item, as cameras write large images - the number of columns and rows of tiles,
        /// the tiles given row by row, each of the size of the first; 0, unless set, of an image of one coded picture.
        /// </summary>
        public uint GridColumns { get; set; }

        /// <summary>The number of rows of tiles of a grid. <see cref="GridColumns"/>.</summary>
        public uint GridRows { get; set; }

        /// <summary>
        /// Of a grid, the size of the image it makes, the tiles cropped to it at the right and the bottom; unless set, all
        /// of the tiles.
        /// </summary>
        public uint? OutputWidth { get; set; }

        /// <summary>The height of the image of a grid. <see cref="OutputWidth"/>.</summary>
        public uint? OutputHeight { get; set; }

        /// <summary>
        /// The colour of the image, its 'colr': of 'nclx', or of an ICC profile. Unless set, the colour description the
        /// stream has - AV1's color_config, HEVC's VUI - as 'nclx'; none where it has none.
        /// </summary>
        public ColourInformationBox Colour { get; set; }

        /// <summary>The shape of the image's pixels, its 'pasp': square unless set.</summary>
        public PixelAspectRatioBox PixelAspectRatio { get; set; }

        /// <summary>
        /// The crop of the image, its 'clap', of the image the pictures - or the grid - make: none unless set. It is applied
        /// first of the transformations, then <see cref="Rotation"/>, then <see cref="Mirror"/> (23008-12 6.5.10).
        /// </summary>
        public CleanApertureBox CleanAperture { get; set; }

        /// <summary>The rotation of the image, its 'irot': its angle, in steps of 90 degrees counter-clockwise; none unless set.</summary>
        public ImageRotation Rotation { get; set; }

        /// <summary>The mirroring of the image, its 'imir': about its vertical axis, or its horizontal; none unless set.</summary>
        public ImageMirror Mirror { get; set; }

        /// <summary>The logger; <see cref="DefaultMp4Logger"/> unless set.</summary>
        public IMp4Logger Logger { get; set; }
    }

    /// <summary>
    /// Builders of HEIF images, which write a file of one image from its coded pictures: of one, or of the tiles of a grid.
    /// </summary>
    public interface IImageBuilder : IDisposable
    {
        /// <summary>The format of the file.</summary>
        ImageFormat Format { get; }

        /// <summary>The track that reads the codec's units and makes the image's configuration of them: of HEVC or AV1.</summary>
        ITrack Track { get; }

        /// <summary>
        /// A unit of the image - an HEVC NAL unit, without its start code, or an AV1 OBU - processed by <see cref="Track"/>,
        /// as <see cref="IMp4Builder.ProcessTrackSample(uint, byte[], int, int)"/> processes video: parameter sets and
        /// sequence headers make the configuration, and each picture is an image, or the next tile of a grid.
        /// </summary>
        void ProcessImageSample(byte[] sample);

        /// <summary>
        /// HEVC NAL units of the Annex B byte stream, each behind its start code, as an encoder hands them out and a .265
        /// file holds them: the same, of each unit. Of HEIC only.
        /// </summary>
        /// <exception cref="NotSupportedException">The image is not of HEVC.</exception>
        void ProcessAnnexBImageSample(byte[] sample);

        /// <summary>
        /// A coded picture stored as it is - HEVC NAL units each behind its length, as an MP4 sample has them, or AV1 OBUs -
        /// as an image, or the next tile; the configuration is of what <see cref="ProcessImageSample"/> was given.
        /// </summary>
        void ProcessRawImage(byte[] image);

        /// <summary>Writes the file: of the picture the track is still assembling too. It is called once.</summary>
        void FinalizeImage();
    }

    /// <summary>
    /// Makes the builder of an image format, as <see cref="Readers.ImageReader"/> reads a file of either: one place to
    /// start writing, as <see cref="VideoBuilder"/> is of video.
    /// </summary>
    public static class ImageBuilder
    {
        /// <summary>A builder of the format, of the options given.</summary>
        /// <param name="output">Where the file is written. <see cref="IMp4Output"/>.</param>
        /// <param name="format">The format of the image: HEIC or AVIF.</param>
        /// <param name="options">Of a grid, its tiles and size; of none, an image of one coded picture.</param>
        /// <exception cref="ArgumentException">A grid of columns but no rows, or rows but no columns; or a size of no grid.</exception>
        public static IImageBuilder Create(IMp4Output output, ImageFormat format, ImageBuilderOptions options = null)
        {
            return new HeifImageBuilder(output, format, options ?? new ImageBuilderOptions());
        }
    }

    /// <summary>
    /// Writes a HEIF image of HEVC or AV1 (ISO/IEC 23008-12, MIAF): its 'ftyp', a 'meta' of the image's items, their
    /// properties and where they are, and an 'mdat' of their coded pictures. Of one picture, it is the primary item; of
    /// tiles, each is an item of its own, hidden, and the primary item a 'grid' of them, its description in the 'idat'.
    /// </summary>
    public sealed class HeifImageBuilder : IImageBuilder
    {
        private readonly IMp4Output _output;
        private readonly ImageBuilderOptions _options;
        private readonly List<byte[]> _images = new List<byte[]>();
        private bool _isFinalized;
        private bool _isDisposed;

        public HeifImageBuilder(IMp4Output output, ImageFormat format, ImageBuilderOptions options)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if ((options.GridColumns == 0) != (options.GridRows == 0))
                throw new ArgumentException($"A grid is of columns and rows: not {options.GridColumns} columns and {options.GridRows} rows.", nameof(options));
            if (options.GridColumns > 256 || options.GridRows > 256)
                throw new ArgumentException($"A grid is of 256 columns and rows at most: not {options.GridColumns} by {options.GridRows}.", nameof(options));
            if (!IsGrid && (options.OutputWidth.HasValue || options.OutputHeight.HasValue))
                throw new ArgumentException("The output size is of a grid: an image of one picture is of its size.", nameof(options));

            Format = format;
            Logger = options.Logger ?? DefaultMp4Logger.Instance;
            Track = format switch
            {
                ImageFormat.Heic => new H265Track(),
                ImageFormat.Avif => new AV1Track(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, "An image is of HEIC or AVIF."),
            };
            Track.Logger = Logger;
        }

        /// <inheritdoc/>
        public ImageFormat Format { get; }

        /// <inheritdoc/>
        public ITrack Track { get; }

        public IMp4Logger Logger { get; }

        private bool IsGrid => _options.GridColumns > 0;

        private int ImageCount => IsGrid ? (int)(_options.GridColumns * _options.GridRows) : 1;

        /// <inheritdoc/>
        public void ProcessImageSample(byte[] sample)
        {
            ThrowIfFinished();
            Track.ProcessSample(sample, out var output, out _);
            if (output.Array != null && IsPicture(output))
                AddImage(output.ToArray());
        }

        /// <summary>
        /// Whether a unit the track gives back is a picture. AV1's gives back what it holds whatever it is - of a sequence
        /// header alone, given for the configuration of pictures stored as they are, no picture: of no frame, frame header
        /// or tile group OBU (AV1 5.3.1).
        /// </summary>
        private bool IsPicture(ArraySegment<byte> unit)
        {
            if (Format != ImageFormat.Avif)
                return true;

            int at = unit.Offset, end = unit.Offset + unit.Count;
            byte[] data = unit.Array;
            while (at < end)
            {
                byte header = data[at];
                int type = (header >> 3) & 0xF;
                if (type == 3 || type == 4 || type == 6 || type == 7)
                    return true;

                at += (header & 0x4) != 0 ? 2 : 1;
                if ((header & 0x2) == 0)
                    return false; // the last OBU, of no size field: no picture before it

                ulong size = 0;
                for (int shift = 0; at < end; shift += 7)
                {
                    byte b = data[at++];
                    size |= (ulong)(b & 0x7F) << shift;
                    if ((b & 0x80) == 0)
                        break;
                }
                at += (int)Math.Min(size, (ulong)(end - at));
            }
            return false;
        }

        /// <inheritdoc/>
        public void ProcessAnnexBImageSample(byte[] sample)
        {
            ThrowIfFinished();
            foreach (var unit in Utils.NalUnits(Track, 0, new ArraySegment<byte>(sample)))
                ProcessImageSample(unit.ToArray());
        }

        /// <inheritdoc/>
        public void ProcessRawImage(byte[] image)
        {
            ThrowIfFinished();
            AddImage((byte[])(image ?? throw new ArgumentNullException(nameof(image))).Clone());
        }

        private void AddImage(byte[] image)
        {
            if (_images.Count == ImageCount)
                throw new InvalidOperationException(IsGrid
                    ? $"The grid is of {ImageCount} tiles: a tile more was given."
                    : "The image is of one coded picture: a picture more was given. A grid's options make an image of tiles.");
            _images.Add(image);
        }

        private void ThrowIfFinished()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(HeifImageBuilder));
            if (_isFinalized)
                throw new InvalidOperationException("The image was already finalized: FinalizeImage has written it, and it takes nothing more.");
        }

        /// <inheritdoc/>
        public void FinalizeImage()
        {
            ThrowIfFinished();

            // the picture the track is still assembling
            Track.ProcessSample(null, out var last, out _);
            if (last.Array != null && IsPicture(last))
                AddImage(last.ToArray());
            _isFinalized = true;

            if (_images.Count != ImageCount)
                throw new InvalidOperationException(IsGrid
                    ? $"The grid is of {ImageCount} tiles: {_images.Count} were given."
                    : "The image has no coded picture: none was given.");

            var entry = Track.CreateSampleEntryBox() as VisualSampleEntry
                ?? throw new InvalidOperationException("The track made no visual sample entry: give the image's parameter sets or sequence header first.");
            var config = entry.Children?.FirstOrDefault(x => x is HEVCConfigurationBox || x is AV1CodecConfigurationBox)
                ?? throw new InvalidOperationException("The track has no configuration: give the image's parameter sets or sequence header first.");
            uint tileWidth = entry.Width, tileHeight = entry.Height;
            if (tileWidth == 0 || tileHeight == 0)
                throw new InvalidOperationException("The track knows no size of the image: give its parameter sets or sequence header first.");

            var container = Build(config, tileWidth, tileHeight);

            var stream = _output.GetStream(0);
            container.Write(new IsoStream(stream));
            _output.Flush(stream, 0);
        }

        private Container Build(Box config, uint tileWidth, uint tileHeight)
        {
            uint itemType = IsoStream.FromFourCC(Format == ImageFormat.Heic ? "hvc1" : "av01");
            uint gridID = IsGrid ? (uint)_images.Count + 1 : 0;
            uint primaryID = IsGrid ? gridID : 1;

            uint outputWidth = _options.OutputWidth ?? tileWidth * _options.GridColumns;
            uint outputHeight = _options.OutputHeight ?? tileHeight * _options.GridRows;
            if (IsGrid && (outputWidth == 0 || outputHeight == 0 || outputWidth > tileWidth * _options.GridColumns || outputHeight > tileHeight * _options.GridRows))
                throw new InvalidOperationException($"The output of the grid, {outputWidth}x{outputHeight}, is not within its tiles, {_options.GridColumns}x{_options.GridRows} of {tileWidth}x{tileHeight}.");

            byte[] gridData = IsGrid ? GridDescription(outputWidth, outputHeight) : null;
            long mediaLength = _images.Sum(x => (long)x.Length);
            bool large = mediaLength > uint.MaxValue - 0x10000;

            var container = new Container();

            var ftyp = Format == ImageFormat.Heic
                ? FileType("heic", "mif1", "heic")
                : FileType("avif", "avif", "mif1", "miaf");
            ftyp.SetParent(container);
            container.Children.Add(ftyp);

            var meta = new MetaBox { HasFullBoxHeader = true, Children = new List<Box>() };
            meta.SetParent(container);
            container.Children.Add(meta);

            // what the 'meta' is of: a picture (23008-12 6.2)
            Add(meta, new HandlerBox
            {
                HandlerType = IsoStream.FromFourCC("pict"),
                Reserved = new uint[3],
                Name = new BinaryUTF8String("\0"),
            });

            Add(meta, new PrimaryItemBox { ItemID = primaryID });

            // the items: each picture - of a grid, a tile, hidden, as only the grid is to be shown - then the grid
            var iinf = new ItemInfoBox { Children = new List<Box>(), EntryCount = (uint)_images.Count + (IsGrid ? 1u : 0u) };
            for (int i = 0; i < _images.Count; i++)
                Add(iinf, new ItemInfoEntry(2, IsGrid ? 1u : 0u) { ItemID = (uint)i + 1, ItemType = itemType, ItemName = new BinaryUTF8String("\0") });
            if (IsGrid)
                Add(iinf, new ItemInfoEntry(2) { ItemID = gridID, ItemType = IsoStream.FromFourCC("grid"), ItemName = new BinaryUTF8String("\0") });
            Add(meta, iinf);

            // the grid is derived from its tiles, row by row ('dimg')
            if (IsGrid)
            {
                var dimg = new SingleItemTypeReferenceBox(IsoStream.FromFourCC("dimg"))
                {
                    FromItemID = (ushort)gridID,
                    ReferenceCount = (ushort)_images.Count,
                    ToItemID = Enumerable.Range(1, _images.Count).Select(x => (ushort)x).ToArray(),
                };
                var iref = new ItemReferenceBox { References = new[] { dimg } };
                dimg.SetParent(iref);
                Add(meta, iref);
            }

            // the properties: the codec's configuration, which a decoder needs (essential), the size of the pictures and of
            // the grid, and the bits of their channels; each picture of the first three, the grid of the last two
            // The properties, and the items each is of: the codec's configuration, which a decoder needs (essential), and the
            // size of the pictures, of each picture; the bits of their channels and their colour where it is known, of the
            // pictures and the grid alike; the grid's size, of the grid; then of the image - the picture, or the grid - the
            // shape of its pixels, and its transformations, essential, as they are applied: crop, rotation, mirroring
            // (23008-12 6.5.1, the descriptive properties before the transformative).
            var properties = new List<(Box Property, bool Essential, bool OfPictures, bool OfGrid)>
            {
                (config, true, true, false),
                (new ImageSpatialExtentsProperty { ImageWidth = tileWidth, ImageHeight = tileHeight }, false, true, false),
                (PixelInformation(config), false, true, true),
            };
            var colour = Colour();
            if (colour != null)
                properties.Add((colour, false, true, true));
            if (IsGrid)
                properties.Add((new ImageSpatialExtentsProperty { ImageWidth = outputWidth, ImageHeight = outputHeight }, false, false, true));

            foreach (var (property, essential) in new (Box, bool)[]
            {
                (_options.PixelAspectRatio, false),
                (_options.CleanAperture, true),
                (_options.Rotation, true),
                (_options.Mirror, true),
            })
            {
                if (property != null)
                    properties.Add((property, essential, !IsGrid, IsGrid));
            }

            var ipco = new ItemPropertyContainerBox { Children = new List<Box>() };
            foreach (var property in properties)
                Add(ipco, property.Property);

            (bool Essential, ushort Index)[] PropertiesOf(bool grid) => properties
                .Select((p, i) => (p, Index: (ushort)(i + 1)))
                .Where(x => grid ? x.p.OfGrid : x.p.OfPictures)
                .Select(x => (x.p.Essential, x.Index))
                .ToArray();

            var associations = new List<(uint Item, (bool Essential, ushort Index)[] Properties)>();
            for (int i = 0; i < _images.Count; i++)
                associations.Add(((uint)i + 1, PropertiesOf(grid: false)));
            if (IsGrid)
                associations.Add((gridID, PropertiesOf(grid: true)));

            var ipma = new ItemPropertyAssociationBox
            {
                EntryCount = (uint)associations.Count,
                ItemID = associations.Select(x => x.Item).ToArray(),
                AssociationCount = associations.Select(x => (byte)x.Properties.Length).ToArray(),
                Essential = associations.Select(x => x.Properties.Select(p => p.Essential).ToArray()).ToArray(),
                PropertyIndex = associations.Select(x => x.Properties.Select(p => p.Index).ToArray()).ToArray(),
            };

            var iprp = new ItemPropertiesBox { Children = new List<Box>() };
            Add(iprp, ipco);
            Add(iprp, ipma);
            Add(meta, iprp);

            // where the items are: the pictures one after another in the 'mdat' (construction method 0, of offsets in the
            // file), the grid's description in the 'idat' (1)
            int offsetSize = large ? 8 : 4;
            var iloc = new ItemLocationBox(1)
            {
                OffsetSize = (byte)offsetSize,
                LengthSize = (byte)offsetSize,
                BaseOffsetSize = 0,
                IndexSize = 0,
            };
            Add(meta, iloc);

            if (IsGrid)
            {
                var data = new MemoryStream(gridData);
                Add(meta, new ItemDataBox { Data = new StreamMarker(0, gridData.Length, new IsoStream(data)) });
            }

            var media = new MemoryStream();
            foreach (var image in _images)
                media.Write(image, 0, image.Length);
            var mdat = new MediaDataBox { Data = new StreamMarker(0, mediaLength, new IsoStream(media)) };
            mdat.SetParent(container);
            container.Children.Add(mdat);

            // The offsets are of the file, after the 'ftyp' and the 'meta': the 'meta' is of the same size whatever they
            // are, of their size fixed, so it is sized of none first.
            int gridLength = gridData?.Length ?? 0;
            FillLocations(iloc, gridID, gridLength, offsetSize, 0);
            ulong mdatHeader = (IsoStream.CalculateBoxSize(mdat) >> 3) - (ulong)mediaLength;
            ulong mediaStart = ((ftyp.CalculateSize() + meta.CalculateSize()) >> 3) + mdatHeader;
            FillLocations(iloc, gridID, gridLength, offsetSize, mediaStart);

            return container;
        }

        private void FillLocations(ItemLocationBox iloc, uint gridID, int gridLength, int offsetSize, ulong mediaStart)
        {
            int count = _images.Count + (IsGrid ? 1 : 0);
            iloc.ItemCount = (uint)count;
            iloc.ItemID = new uint[count];
            iloc.Reserved0 = new ushort[count];
            iloc.ConstructionMethod = new byte[count];
            iloc.DataReferenceIndex = new ushort[count];
            iloc.BaseOffset = new byte[count][];
            iloc.ExtentCount = new ushort[count];
            iloc.ItemReferenceIndex = new byte[count][][];
            iloc.ExtentOffset = new byte[count][][];
            iloc.ExtentLength = new byte[count][][];

            ulong offset = mediaStart;
            for (int i = 0; i < count; i++)
            {
                bool isGrid = IsGrid && i == _images.Count;
                ulong length = isGrid ? (ulong)gridLength : (ulong)_images[i].Length;

                iloc.ItemID[i] = isGrid ? gridID : (uint)i + 1;
                iloc.ConstructionMethod[i] = isGrid ? (byte)1 : (byte)0;
                iloc.BaseOffset[i] = Array.Empty<byte>();
                iloc.ExtentCount[i] = 1;
                iloc.ItemReferenceIndex[i] = new[] { Array.Empty<byte>() };
                iloc.ExtentOffset[i] = new[] { BigEndian(isGrid ? 0 : offset, offsetSize) };
                iloc.ExtentLength[i] = new[] { BigEndian(length, offsetSize) };

                if (!isGrid)
                    offset += length;
            }
        }

        // ImageGrid (23008-12 6.6.2.3.2): its version and flags - 32 bit sizes where a size needs them - the rows and columns
        // less one, and the size of the image
        private byte[] GridDescription(uint outputWidth, uint outputHeight)
        {
            bool large = outputWidth > 0xFFFF || outputHeight > 0xFFFF;
            int size = large ? 4 : 2;
            var data = new byte[4 + 2 * size];
            data[0] = 0;
            data[1] = large ? (byte)1 : (byte)0;
            data[2] = (byte)(_options.GridRows - 1);
            data[3] = (byte)(_options.GridColumns - 1);
            BigEndian(outputWidth, size).CopyTo(data, 4);
            BigEndian(outputHeight, size).CopyTo(data, 4 + size);
            return data;
        }

        /// <summary>
        /// The 'colr' of the options; else of the colour description the stream has, as 'nclx': AV1's color_config, of
        /// its primaries, transfer and matrix where it describes them - unspecified, 2, where not - and its range; HEVC's
        /// VUI, the same. None where the stream says neither, which a reader takes as limited range, of its defaults - a
        /// picture of full range without one is shown wrong.
        /// </summary>
        private ColourInformationBox Colour()
        {
            if (_options.Colour != null)
                return _options.Colour;

            (uint Primaries, uint Transfer, uint Matrix, bool FullRange)? description = null;
            if (Track is AV1Track av1 && av1.SequenceHeaderObu is SharpAV1.AV1Context sequence)
            {
                bool described = sequence._ColorDescriptionPresentFlag != 0;
                if (described || sequence._ColorRange != 0)
                    description = described
                        ? ((uint)sequence._ColorPrimaries, (uint)sequence._TransferCharacteristics, (uint)sequence._MatrixCoefficients, sequence._ColorRange != 0)
                        : (2u, 2u, 2u, true);
            }
            else if (Track is H265Track hevc && hevc.Sps.Values.FirstOrDefault() is SharpH265.SeqParameterSetRbsp sps
                && sps.VuiParametersPresentFlag != 0 && sps.VuiParameters is SharpH265.VuiParameters vui)
            {
                bool described = vui.ColourDescriptionPresentFlag != 0;
                if (described || vui.VideoFullRangeFlag != 0)
                    description = described
                        ? (vui.ColourPrimaries, vui.TransferCharacteristics, vui.MatrixCoeffs, vui.VideoFullRangeFlag != 0)
                        : (2u, 2u, 2u, true);
            }

            // what says nothing a reader does not take of none - all unspecified, of limited range - is not written
            if (description is not (uint primaries, uint transfer, uint matrix, bool fullRange)
                || (primaries == 2 && transfer == 2 && matrix == 2 && !fullRange))
                return null;

            return new ColourInformationBox
            {
                ColourType = IsoStream.FromFourCC("nclx"),
                ColourPrimaries = (ushort)primaries,
                TransferCharacteristics = (ushort)transfer,
                MatrixCoefficients = (ushort)matrix,
                FullRangeFlag = fullRange,
            };
        }

        /// <summary>The bits of each channel of the pictures, of their configuration: of a monochrome picture one, else three.</summary>
        private static PixelInformationProperty PixelInformation(Box config)
        {
            byte luma, chroma;
            bool monochrome;
            if (config is HEVCConfigurationBox hvcC)
            {
                var record = hvcC._HEVCConfig;
                luma = (byte)(record.BitDepthLumaMinus8 + 8);
                chroma = (byte)(record.BitDepthChromaMinus8 + 8);
                monochrome = record.ChromaFormat == 0;
            }
            else
            {
                var record = ((AV1CodecConfigurationBox)config).Av1Config;
                luma = chroma = record.TwelveBit ? (byte)12 : record.HighBitdepth ? (byte)10 : (byte)8;
                monochrome = record.Monochrome;
            }

            var bits = monochrome ? new[] { luma } : new[] { luma, chroma, chroma };
            return new PixelInformationProperty { NumChannels = (byte)bits.Length, BitsPerChannel = bits };
        }

        private static FileTypeBox FileType(string major, params string[] compatible) => new FileTypeBox
        {
            MajorBrand = IsoStream.FromFourCC(major),
            MinorVersion = 0,
            CompatibleBrands = compatible.Select(IsoStream.FromFourCC).ToArray(),
        };

        private static void Add(Box parent, Box child)
        {
            child.SetParent(parent);
            parent.Children.Add(child);
        }

        private static byte[] BigEndian(ulong value, int size)
        {
            var bytes = new byte[size];
            for (int i = 0; i < size; i++)
                bytes[i] = (byte)(value >> (8 * (size - 1 - i)));
            return bytes;
        }

        /// <summary>Disposes the track. Without <see cref="FinalizeImage"/> before it, nothing is written.</summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;
            Track.Dispose();
            _isDisposed = true;
        }
    }
}
