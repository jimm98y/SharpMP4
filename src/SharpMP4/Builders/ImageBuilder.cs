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
        /// The colour of the image, its 'colr': of 'nclx', or of an ICC profile; of its thumbnails too. Unless set, the
        /// colour description the stream has - AV1's color_config, HEVC's VUI - as 'nclx'; none where it has none.
        /// </summary>
        public ColourInformationBox Colour { get; set; }

        /// <summary>The shape of the image's pixels, its 'pasp': square unless set.</summary>
        public PixelAspectRatioBox PixelAspectRatio { get; set; }

        /// <summary>
        /// The crop of the image, its 'clap', of the image the pictures - or the grid - make: none unless set. It is applied
        /// first of the transformations, then <see cref="Rotation"/>, then <see cref="Mirror"/> (23008-12 6.5.10). Of the
        /// alpha too, which is of the image's size; not of the thumbnails, which are not.
        /// </summary>
        public CleanApertureBox CleanAperture { get; set; }

        /// <summary>
        /// The rotation of the image, its 'irot': its angle, in steps of 90 degrees counter-clockwise; none unless set. Of
        /// the alpha and the thumbnails too, which are shown as the image is.
        /// </summary>
        public ImageRotation Rotation { get; set; }

        /// <summary>
        /// The mirroring of the image, its 'imir': about its vertical axis, or its horizontal; none unless set. Of the alpha
        /// and the thumbnails too.
        /// </summary>
        public ImageMirror Mirror { get; set; }

        /// <summary>The logger; <see cref="DefaultMp4Logger"/> unless set.</summary>
        public IMp4Logger Logger { get; set; }
    }

    /// <summary>
    /// An image of a HEIF file - the primary image, its alpha, or a thumbnail of it - and what makes it: the coded pictures
    /// given, and the track that reads them.
    /// </summary>
    public interface IImagePlane
    {
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
        /// as an image, or the next tile; the configuration is of what <see cref="ProcessImageSample"/> was given, before
        /// it. A plane is given its pictures one way or the other: once given one stored as it is, what the track makes of
        /// units given after it is no picture of the plane.
        /// </summary>
        void ProcessRawImage(byte[] image);
    }

    /// <summary>
    /// Builders of HEIF images, which write a file of one image from its coded pictures - of one, or of the tiles of a grid
    /// - and of its alpha and thumbnails, of theirs. What the builder is given itself is of the primary image.
    /// </summary>
    public interface IImageBuilder : IImagePlane, IDisposable
    {
        /// <summary>The format of the file.</summary>
        ImageFormat Format { get; }

        /// <summary>
        /// The alpha of the image, an auxiliary image of its transparency, coded as a picture of one channel: of a grid, a
        /// grid of the same tiles. It is the file's once asked for, and is then to be given its pictures.
        /// </summary>
        IImagePlane Alpha { get; }

        /// <summary>A thumbnail of the image, of a picture of its own, smaller: one more each time it is called.</summary>
        IImagePlane AddThumbnail();

        /// <summary>Writes the file: of the pictures the tracks are still assembling too. It is called once.</summary>
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
    /// tiles, each is an item of its own, hidden, and the primary item a 'grid' of them, its description in the 'idat'. Its
    /// alpha is an auxiliary image of the same form ('auxl'), and each thumbnail an image of one picture ('thmb').
    /// </summary>
    public sealed class HeifImageBuilder : IImageBuilder
    {
        /// <summary>The type of an auxiliary image of alpha: of AV1's (AVIF 4.1), and of HEVC's (23008-12 Annex B.2).</summary>
        private const string AvifAlpha = "urn:mpeg:mpegB:cicp:systems:auxiliary:alpha";
        private const string HevcAlpha = "urn:mpeg:hevc:2015:auxid:1";

        private enum PlaneKind
        {
            Image,
            Alpha,
            Thumbnail,
        }

        /// <summary>An image of the file, and what makes it: its pictures, and the track that reads them.</summary>
        private sealed class Plane : IImagePlane
        {
            private readonly HeifImageBuilder _builder;

            public Plane(HeifImageBuilder builder, PlaneKind kind, int pictureCount, ITrack track)
            {
                _builder = builder;
                Kind = kind;
                PictureCount = pictureCount;
                Track = track;
            }

            public PlaneKind Kind { get; }

            /// <summary>How many pictures the image is of: of a grid, its tiles; else one.</summary>
            public int PictureCount { get; }

            public bool IsGrid => Kind != PlaneKind.Thumbnail && _builder.IsGrid;

            public ITrack Track { get; }

            public List<byte[]> Pictures { get; } = new List<byte[]>();

            // of the file being written: the items of the pictures, and of the grid, where it is one
            public uint FirstPictureID { get; set; }
            public uint GridID { get; set; }
            public uint ImageID => IsGrid ? GridID : FirstPictureID;
            public Box Config { get; set; }
            public uint PictureWidth { get; set; }
            public uint PictureHeight { get; set; }

            public string Name => Kind switch
            {
                PlaneKind.Alpha => "alpha",
                PlaneKind.Thumbnail => "thumbnail",
                _ => "image",
            };

            public void ProcessImageSample(byte[] sample)
            {
                _builder.ThrowIfFinished();
                Track.ProcessSample(sample, out var output, out _);
                if (output.Array != null && !_readsRawPictures && _builder.IsPicture(output))
                    AddPicture(output.ToArray());
            }

            public void ProcessAnnexBImageSample(byte[] sample)
            {
                _builder.ThrowIfFinished();
                foreach (var unit in Utils.NalUnits(Track, 0, new ArraySegment<byte>(sample)))
                    ProcessImageSample(unit.ToArray());
            }

            public void ProcessRawImage(byte[] image)
            {
                _builder.ThrowIfFinished();
                AddPicture((byte[])(image ?? throw new ArgumentNullException(nameof(image))).Clone());

                // Of AV1, the size of the picture is its frame header's: the sequence header has only the largest a picture
                // of it may be, which a thumbnail sharing the image's is smaller than. The picture's OBUs are read by the
                // track for it, what it gives back of them not a picture of the image - it is the one just added.
                if (_builder.Format == ImageFormat.Avif)
                {
                    _readsRawPictures = true;
                    try
                    {
                        foreach (var obu in Obus(image))
                            Track.ProcessSample(obu, out _, out _);
                    }
                    catch (Exception ex) when (ex is not OutOfMemoryException)
                    {
                        if (_builder.Logger.IsWarningEnabled)
                            _builder.Logger.LogWarning($"The {Name}'s picture could not be read for its size ({ex.GetType().Name}: {ex.Message}): of its sequence header's largest");
                    }
                }
            }

            // whether the track has read pictures stored as they are, which it gives back of no picture of the image
            private bool _readsRawPictures;

            /// <summary>The OBUs of an AV1 picture, each of its size field, as a sample has them (AV1-ISOBMFF 2.4).</summary>
            private static IEnumerable<byte[]> Obus(byte[] data)
            {
                int at = 0;
                while (at < data.Length)
                {
                    int start = at;
                    byte header = data[at];
                    at += (header & 0x4) != 0 ? 2 : 1;
                    if ((header & 0x2) == 0)
                    {
                        yield return Slice(data, start, data.Length);
                        yield break;
                    }

                    ulong size = 0;
                    for (int shift = 0; at < data.Length; shift += 7)
                    {
                        byte b = data[at++];
                        size |= (ulong)(b & 0x7F) << shift;
                        if ((b & 0x80) == 0)
                            break;
                    }
                    int end = (int)Math.Min((ulong)data.Length, (ulong)at + size);
                    yield return Slice(data, start, end);
                    at = end;
                }
            }

            private static byte[] Slice(byte[] data, int start, int end)
            {
                var slice = new byte[end - start];
                Array.Copy(data, start, slice, 0, slice.Length);
                return slice;
            }

            private void AddPicture(byte[] picture)
            {
                if (Pictures.Count == PictureCount)
                    throw new InvalidOperationException(IsGrid
                        ? $"The {Name}'s grid is of {PictureCount} tiles: a tile more was given."
                        : $"The {Name} is of one coded picture: a picture more was given." + (Kind == PlaneKind.Image ? " A grid's options make an image of tiles." : ""));
                Pictures.Add(picture);
            }

            /// <summary>The picture the track is still assembling; then whether the pictures are all there, and what they are of.</summary>
            public void Finish()
            {
                Track.ProcessSample(null, out var last, out _);
                if (last.Array != null && !_readsRawPictures && _builder.IsPicture(last))
                    AddPicture(last.ToArray());

                if (Pictures.Count != PictureCount)
                    throw new InvalidOperationException(IsGrid
                        ? $"The {Name}'s grid is of {PictureCount} tiles: {Pictures.Count} were given."
                        : $"The {Name} has no coded picture: none was given.");

                var entry = Track.CreateSampleEntryBox() as VisualSampleEntry
                    ?? throw new InvalidOperationException($"The {Name}'s track made no visual sample entry: give its parameter sets or sequence header first.");
                Config = entry.Children?.FirstOrDefault(x => x is HEVCConfigurationBox || x is AV1CodecConfigurationBox)
                    ?? throw new InvalidOperationException($"The {Name}'s track has no configuration: give its parameter sets or sequence header first.");
                PictureWidth = entry.Width;
                PictureHeight = entry.Height;

                // Of AV1, the size coded in the frame header, where one was read and is within the sequence header's
                // largest - of an image the size HEIF says (AVIF 2.2.1), where the entry's is the size it is shown at, its
                // render size; else the sequence header's largest.
                if (Track is AV1Track av1 && av1.SequenceHeaderObu is SharpAV1.AV1Context sequence)
                {
                    int maxWidth = sequence._MaxFrameWidthMinus1 + 1, maxHeight = sequence._MaxFrameHeightMinus1 + 1;
                    bool coded = av1.FrameSize is (int width, int height) && width <= maxWidth && height <= maxHeight;
                    PictureWidth = (uint)(coded ? av1.FrameSize.Value.Width : maxWidth);
                    PictureHeight = (uint)(coded ? av1.FrameSize.Value.Height : maxHeight);
                }
                if (PictureWidth == 0 || PictureHeight == 0)
                    throw new InvalidOperationException($"The {Name}'s track knows no size of it: give its parameter sets or sequence header first.");
            }
        }

        private readonly IMp4Output _output;
        private readonly ImageBuilderOptions _options;
        private readonly Plane _image;
        private readonly List<Plane> _thumbnails = new List<Plane>();
        private Plane _alpha;
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
            if (format != ImageFormat.Heic && format != ImageFormat.Avif)
                throw new ArgumentOutOfRangeException(nameof(format), format, "An image is of HEIC or AVIF.");

            Format = format;
            Logger = options.Logger ?? DefaultMp4Logger.Instance;
            _image = new Plane(this, PlaneKind.Image, PictureCountOfGrid, CreateTrack());
        }

        /// <inheritdoc/>
        public ImageFormat Format { get; }

        /// <inheritdoc/>
        public ITrack Track => _image.Track;

        public IMp4Logger Logger { get; }

        /// <inheritdoc/>
        public IImagePlane Alpha
        {
            get
            {
                if (_alpha == null)
                {
                    ThrowIfFinished();
                    _alpha = new Plane(this, PlaneKind.Alpha, PictureCountOfGrid, CreateTrack());
                }
                return _alpha;
            }
        }

        /// <inheritdoc/>
        public IImagePlane AddThumbnail()
        {
            ThrowIfFinished();
            var thumbnail = new Plane(this, PlaneKind.Thumbnail, 1, CreateTrack());
            _thumbnails.Add(thumbnail);
            return thumbnail;
        }

        private bool IsGrid => _options.GridColumns > 0;

        private int PictureCountOfGrid => IsGrid ? (int)(_options.GridColumns * _options.GridRows) : 1;

        private IEnumerable<Plane> Planes => new[] { _image, _alpha }.Where(x => x != null).Concat(_thumbnails);

        private ITrack CreateTrack()
        {
            ITrack track = Format == ImageFormat.Heic ? new H265Track() : new AV1Track();
            track.Logger = Logger;
            return track;
        }

        /// <inheritdoc/>
        public void ProcessImageSample(byte[] sample) => _image.ProcessImageSample(sample);

        /// <inheritdoc/>
        public void ProcessAnnexBImageSample(byte[] sample) => _image.ProcessAnnexBImageSample(sample);

        /// <inheritdoc/>
        public void ProcessRawImage(byte[] image) => _image.ProcessRawImage(image);

        /// <summary>
        /// Whether a unit a track gives back is a picture. AV1's gives back what it holds whatever it is - of a sequence
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
            _isFinalized = true;

            foreach (var plane in Planes)
                plane.Finish();

            var container = Build();

            var stream = _output.GetStream(0);
            container.Write(new IsoStream(stream));
            _output.Flush(stream, 0);
        }

        private Container Build()
        {
            // the size of the image the grid makes, of the image's tiles: its alpha's grid is of the same
            uint outputWidth = 0, outputHeight = 0;
            if (IsGrid)
            {
                outputWidth = _options.OutputWidth ?? _image.PictureWidth * _options.GridColumns;
                outputHeight = _options.OutputHeight ?? _image.PictureHeight * _options.GridRows;
                foreach (var plane in Planes.Where(x => x.IsGrid))
                {
                    if (outputWidth == 0 || outputHeight == 0 || outputWidth > plane.PictureWidth * _options.GridColumns || outputHeight > plane.PictureHeight * _options.GridRows)
                        throw new InvalidOperationException($"The output of the {plane.Name}'s grid, {outputWidth}x{outputHeight}, is not within its tiles, {_options.GridColumns}x{_options.GridRows} of {plane.PictureWidth}x{plane.PictureHeight}.");
                }
            }
            byte[] gridData = IsGrid ? GridDescription(outputWidth, outputHeight) : null;

            // The items: of each plane in turn its pictures - of a grid, tiles, hidden, as only the grid is to be shown - then
            // its grid; the image's, its alpha's, then its thumbnails'.
            uint itemType = IsoStream.FromFourCC(Format == ImageFormat.Heic ? "hvc1" : "av01");
            var items = new List<(uint ID, uint Type, bool Hidden, byte[] Data, bool InIdat)>();
            uint nextID = 1;
            foreach (var plane in Planes)
            {
                plane.FirstPictureID = nextID;
                foreach (var picture in plane.Pictures)
                    items.Add((nextID++, itemType, plane.IsGrid, picture, false));
                if (plane.IsGrid)
                {
                    plane.GridID = nextID;
                    items.Add((nextID++, IsoStream.FromFourCC("grid"), false, gridData, true));
                }
            }

            long mediaLength = items.Where(x => !x.InIdat).Sum(x => (long)x.Data.Length);
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

            Add(meta, new PrimaryItemBox { ItemID = _image.ImageID });

            var iinf = new ItemInfoBox { Children = new List<Box>(), EntryCount = (uint)items.Count };
            foreach (var item in items)
                Add(iinf, new ItemInfoEntry(2, item.Hidden ? 1u : 0u) { ItemID = item.ID, ItemType = item.Type, ItemName = new BinaryUTF8String("\0") });
            Add(meta, iinf);

            // The references: each grid derived from its tiles, row by row ('dimg'); the alpha an auxiliary image of the
            // image ('auxl'); each thumbnail one of it ('thmb').
            var references = new List<SingleItemTypeReferenceBox>();
            foreach (var plane in Planes.Where(x => x.IsGrid))
                references.Add(Reference("dimg", plane.GridID, Enumerable.Range((int)plane.FirstPictureID, plane.Pictures.Count).Select(x => (uint)x)));
            if (_alpha != null)
                references.Add(Reference("auxl", _alpha.ImageID, new[] { _image.ImageID }));
            foreach (var thumbnail in _thumbnails)
                references.Add(Reference("thmb", thumbnail.ImageID, new[] { _image.ImageID }));
            if (references.Count > 0)
            {
                var iref = new ItemReferenceBox { References = references.ToArray() };
                foreach (var reference in references)
                    reference.SetParent(iref);
                Add(meta, iref);
            }

            Add(meta, Properties(outputWidth, outputHeight));

            // where the items are: the pictures one after another in the 'mdat' (construction method 0, of offsets in the
            // file), the grids' descriptions in the 'idat' (1)
            int offsetSize = large ? 8 : 4;
            var iloc = new ItemLocationBox(1)
            {
                OffsetSize = (byte)offsetSize,
                LengthSize = (byte)offsetSize,
                BaseOffsetSize = 0,
                IndexSize = 0,
            };
            Add(meta, iloc);

            var idatItems = items.Where(x => x.InIdat).ToList();
            if (idatItems.Count > 0)
            {
                var idat = new MemoryStream();
                foreach (var item in idatItems)
                    idat.Write(item.Data, 0, item.Data.Length);
                Add(meta, new ItemDataBox { Data = new StreamMarker(0, idat.Length, new IsoStream(idat)) });
            }

            var media = new MemoryStream();
            foreach (var item in items.Where(x => !x.InIdat))
                media.Write(item.Data, 0, item.Data.Length);
            var mdat = new MediaDataBox { Data = new StreamMarker(0, mediaLength, new IsoStream(media)) };
            mdat.SetParent(container);
            container.Children.Add(mdat);

            // The offsets are of the file, after the 'ftyp' and the 'meta': the 'meta' is of the same size whatever they
            // are, of their size fixed, so it is sized of none first.
            FillLocations(iloc, items, offsetSize, 0);
            ulong mdatHeader = (IsoStream.CalculateBoxSize(mdat) >> 3) - (ulong)mediaLength;
            ulong mediaStart = ((ftyp.CalculateSize() + meta.CalculateSize()) >> 3) + mdatHeader;
            FillLocations(iloc, items, offsetSize, mediaStart);

            return container;
        }

        /// <summary>
        /// The properties, and the items each is of. Of each plane: the codec's configuration, which a decoder needs
        /// (essential), and the size of the pictures, of each picture; the bits of their channels and their colour where it
        /// is known, of the pictures and the grid alike; of the alpha, its type, alike; the grid's size, of the grid. Then of
        /// the image, and its alpha, the shape of its pixels, and its transformations, essential, as they are applied: crop,
        /// rotation, mirroring - of the thumbnails, of the image's size not, the rotation and mirroring alone. The
        /// descriptive properties are all before the transformative, as each item is to have them (23008-12 6.5.1).
        /// </summary>
        private ItemPropertiesBox Properties(uint outputWidth, uint outputHeight)
        {
            var properties = new List<(Box Property, bool Essential, List<uint> Items)>();
            void Of(Box property, bool essential, IEnumerable<uint> items)
            {
                if (property == null)
                    return;
                int at = properties.FindIndex(x => ReferenceEquals(x.Property, property));
                if (at >= 0)
                    properties[at].Items.AddRange(items);
                else
                    properties.Add((property, essential, items.ToList()));
            }

            foreach (var plane in Planes)
            {
                var pictures = Enumerable.Range((int)plane.FirstPictureID, plane.Pictures.Count).Select(x => (uint)x).ToList();
                var pixels = plane.IsGrid ? pictures.Append(plane.GridID).ToList() : pictures;

                Of(plane.Config, true, pictures);
                Of(new ImageSpatialExtentsProperty { ImageWidth = plane.PictureWidth, ImageHeight = plane.PictureHeight }, false, pictures);
                Of(PixelInformation(plane.Config), false, pixels);
                if (plane.Kind == PlaneKind.Alpha)
                {
                    string type = Format == ImageFormat.Avif ? AvifAlpha : HevcAlpha;
                    Of(new AuxiliaryTypeProperty { AuxType = new BinaryUTF8String(type + "\0"), AuxSubtype = Array.Empty<byte>() }, false, pixels);
                }
                else
                {
                    Of(_options.Colour ?? Colour(plane.Track), false, pixels);
                }
                if (plane.IsGrid)
                    Of(new ImageSpatialExtentsProperty { ImageWidth = outputWidth, ImageHeight = outputHeight }, false, new[] { plane.GridID });
            }

            var ofImage = new[] { _image, _alpha }.Where(x => x != null).Select(x => x.ImageID).ToList();
            var ofThumbnails = _thumbnails.Select(x => x.ImageID).ToList();
            Of(_options.PixelAspectRatio, false, ofImage);
            Of(_options.CleanAperture, true, ofImage);
            Of(_options.Rotation, true, ofImage.Concat(ofThumbnails));
            Of(_options.Mirror, true, ofImage.Concat(ofThumbnails));

            var ipco = new ItemPropertyContainerBox { Children = new List<Box>() };
            foreach (var property in properties)
                Add(ipco, property.Property);

            // of each item, its properties in the order they are in: the descriptive before the transformative
            var associations = properties
                .SelectMany((p, index) => p.Items.Select(item => (Item: item, p.Essential, Index: (ushort)(index + 1))))
                .GroupBy(x => x.Item)
                .OrderBy(x => x.Key)
                .Select(x => (Item: x.Key, Properties: x.OrderBy(p => p.Index).Select(p => (p.Essential, p.Index)).ToArray()))
                .ToList();

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
            return iprp;
        }

        private static SingleItemTypeReferenceBox Reference(string type, uint from, IEnumerable<uint> to)
        {
            var ids = to.Select(x => (ushort)x).ToArray();
            return new SingleItemTypeReferenceBox(IsoStream.FromFourCC(type))
            {
                FromItemID = (ushort)from,
                ReferenceCount = (ushort)ids.Length,
                ToItemID = ids,
            };
        }

        private static void FillLocations(ItemLocationBox iloc, List<(uint ID, uint Type, bool Hidden, byte[] Data, bool InIdat)> items, int offsetSize, ulong mediaStart)
        {
            int count = items.Count;
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

            ulong mediaOffset = mediaStart, idatOffset = 0;
            for (int i = 0; i < count; i++)
            {
                var item = items[i];
                ulong length = (ulong)item.Data.Length;

                iloc.ItemID[i] = item.ID;
                iloc.ConstructionMethod[i] = item.InIdat ? (byte)1 : (byte)0;
                iloc.BaseOffset[i] = Array.Empty<byte>();
                iloc.ExtentCount[i] = 1;
                iloc.ItemReferenceIndex[i] = new[] { Array.Empty<byte>() };
                iloc.ExtentOffset[i] = new[] { BigEndian(item.InIdat ? idatOffset : mediaOffset, offsetSize) };
                iloc.ExtentLength[i] = new[] { BigEndian(length, offsetSize) };

                if (item.InIdat)
                    idatOffset += length;
                else
                    mediaOffset += length;
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
        /// The colour description a track's stream has, as 'nclx': AV1's color_config, of its primaries, transfer and matrix
        /// where it describes them - unspecified, 2, where not - and its range; HEVC's VUI, the same. None where the stream
        /// says nothing a reader does not take of none - all unspecified, of limited range: a picture of full range without
        /// one is shown wrong.
        /// </summary>
        private static ColourInformationBox Colour(ITrack track)
        {
            (uint Primaries, uint Transfer, uint Matrix, bool FullRange)? description = null;
            if (track is AV1Track av1 && av1.SequenceHeaderObu is SharpAV1.AV1Context sequence)
            {
                bool described = sequence._ColorDescriptionPresentFlag != 0;
                if (described || sequence._ColorRange != 0)
                    description = described
                        ? ((uint)sequence._ColorPrimaries, (uint)sequence._TransferCharacteristics, (uint)sequence._MatrixCoefficients, sequence._ColorRange != 0)
                        : (2u, 2u, 2u, true);
            }
            else if (track is H265Track hevc && hevc.Sps.Values.FirstOrDefault() is SharpH265.SeqParameterSetRbsp sps
                && sps.VuiParametersPresentFlag != 0 && sps.VuiParameters is SharpH265.VuiParameters vui)
            {
                bool described = vui.ColourDescriptionPresentFlag != 0;
                if (described || vui.VideoFullRangeFlag != 0)
                    description = described
                        ? (vui.ColourPrimaries, vui.TransferCharacteristics, vui.MatrixCoeffs, vui.VideoFullRangeFlag != 0)
                        : (2u, 2u, 2u, true);
            }

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

        /// <summary>Disposes the tracks. Without <see cref="FinalizeImage"/> before it, nothing is written.</summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;
            foreach (var plane in Planes)
                plane.Track.Dispose();
            _isDisposed = true;
        }
    }
}
