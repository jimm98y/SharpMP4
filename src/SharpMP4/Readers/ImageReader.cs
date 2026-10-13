using SharpISOBMFF;
using SharpMP4.Tracks;
using SharpMP4.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpMP4.Readers
{
    public class ImageSample
    {
        public byte[] Data { get; set; }

        public ImageSample(byte[] data)
        {
            this.Data = data;
        }
    }

    /// <summary>
    /// A grid of images (ISO/IEC 23008-12 6.6.2.3): tiles of one size, row by row, the image of the output size cropped from
    /// the top left of them.
    /// </summary>
    public class ImageGrid
    {
        public int Rows { get; }
        public int Columns { get; }
        public uint OutputWidth { get; }
        public uint OutputHeight { get; }

        public ImageGrid(int rows, int columns, uint outputWidth, uint outputHeight)
        {
            Rows = rows;
            Columns = columns;
            OutputWidth = outputWidth;
            OutputHeight = outputHeight;
        }

        /// <summary>
        /// Of the item's bytes: its version, its flags - whose lowest bit makes the sizes 32 bits, not 16 - the rows and the
        /// columns less one, then the output width and height.
        /// </summary>
        public static ImageGrid Parse(byte[] data)
        {
            if (data == null || data.Length < 8)
                throw new InvalidDataException("A grid's description is shorter than its fields.");
            if (data[0] != 0)
                throw new NotSupportedException($"A grid of version {data[0]} is not supported.");

            int size = (data[1] & 1) != 0 ? 4 : 2;
            if (data.Length < 4 + 2 * size)
                throw new InvalidDataException("A grid's description is shorter than its fields.");

            uint Read(int at)
            {
                uint value = 0;
                for (int i = 0; i < size; i++)
                    value = (value << 8) | data[at + i];
                return value;
            }
            return new ImageGrid(data[2] + 1, data[3] + 1, Read(4), Read(4 + size));
        }
    }

    /// <summary>
    /// Experimental reader for heif/heic/avif images.
    /// </summary>
    public class ImageReader : IDisposable
    {
        public Container Container { get; set; }
        public ITrack Track { get; set; }
        public MetaBox Meta { get; private set; }
        public MediaDataBox Mdat { get; private set; }
        public ItemLocationBox Iloc { get; private set; }

        /// <summary>The 'idat' of the 'meta', where items of construction method 1 are; null where it has none.</summary>
        public ItemDataBox Idat { get; private set; }
        public ImageSpatialExtentsProperty Ispe { get; private set; }

        /// <summary>The item the file shows: 'pitm's, or without one the first item there is.</summary>
        public uint PrimaryItemID { get; private set; }

        /// <summary>The groups of entities of the 'grpl': a stereo pair's ('ster'), alternatives ('altr'); none without one.</summary>
        public IReadOnlyList<EntityToGroupBox> Groups { get; private set; } = Array.Empty<EntityToGroupBox>();

        /// <summary>The item read next, by its place in 'iloc'.</summary>
        public int ImageIndex { get; private set; }

        public TrackFactory TrackFactory { get; set; } = new();
        public IMp4Logger Logger { get; set; }

        // where the items are, of a stream that seeks and of one that does not alike
        private FileDataLocator _data;

        // the configurations of the codecs of images: HEVC, VVC, AVC, AV1 and AV2
        private static readonly HashSet<string> CodecConfigurations = new HashSet<string> { "hvcC", "vvcC", "avcC", "av1C", "av2C" };

        private static readonly uint Grid = IsoStream.FromFourCC("grid");
        private static readonly uint DerivedImage = IsoStream.FromFourCC("dimg");

        private ItemPropertyContainerBox _ipco;
        private ItemPropertyAssociationBox _ipma;
        private ItemReferenceBox _iref;
        private ItemInfoBox _iinf;

        public ImageReader(IMp4Logger logger)
        {
            Logger = logger ?? DefaultMp4Logger.Instance;
        }

        public ImageReader()
            : this(DefaultMp4Logger.Instance)
        {
        }

        public void Parse(Container container)
        {
            if (container.Children.Count == 0)
                return;

            this.Container = container;

            var meta = container.Children.OfType<MetaBox>().SingleOrDefault()
                ?? throw new InvalidDataException("The file has no 'meta' box at its top level: it is not a HEIF image.");
            this.Meta = meta;

            var mdat = container.Children.OfType<MediaDataBox>().FirstOrDefault();
            this.Mdat = mdat;

            var iprp = meta.Children.OfType<ItemPropertiesBox>().Single();
            var ipco = iprp.Children.OfType<ItemPropertyContainerBox>().Single();
            var ipma = iprp.Children.OfType<ItemPropertyAssociationBox>().Single();
            var iref = meta.Children.OfType<ItemReferenceBox>().SingleOrDefault(); // optional: only items that refer to others have one
            var iinf = meta.Children.OfType<ItemInfoBox>().Single();

            var iloc = meta.Children.OfType<ItemLocationBox>().Single();
            Iloc = iloc;

            this.Idat = meta.Children.OfType<ItemDataBox>().SingleOrDefault();
            this._data = FileDataLocator.Of(container, this.Idat?.Data);

            // the primary item, as 'pitm' says; without one, the first item there is
            var primaryItem = meta.Children.OfType<PrimaryItemBox>().SingleOrDefault();
            uint primaryItemID = primaryItem?.ItemID ?? iinf.ItemInfos.FirstOrDefault()?.ItemID ?? iloc.ItemID?.FirstOrDefault() ?? 0;
            this.PrimaryItemID = primaryItemID;

            _ipco = ipco;
            _ipma = ipma;
            _iref = iref;
            _iinf = iinf;
            this.Groups = meta.Children.OfType<GroupsListBox>().SelectMany(x => x.Children.OfType<EntityToGroupBox>()).ToArray();

            var propertyBoxes = PropertiesOf(ipco, ipma, primaryItemID);

            if (iinf.ItemInfos.FirstOrDefault(x => x.ItemID == primaryItemID)?.ItemType == Grid)
            {
                // Apple stores HEIC files with a grid of images, whose tiles the grid refers to ('dimg')
                var tileID = TilesOf(iref, primaryItemID).Cast<uint?>().FirstOrDefault();
                if (tileID != null)
                    propertyBoxes = PropertiesOf(ipco, ipma, tileID.Value); // let's hope all tiles share the same config
            }

            this.Track = CreateTrack(propertyBoxes)
                ?? throw new NotSupportedException("No supported track found in image file.");

            var ispe = propertyBoxes.OfType<ImageSpatialExtentsProperty>().FirstOrDefault();
            this.Ispe = ispe;
        }

        /// <summary>
        /// The track of the codec an item is coded in, of its configuration among its properties: what parses its bytes into
        /// the units a decoder takes, and has its parameter sets. Null where it has no configuration a track is made of - a
        /// grid, which is of other items. The caller's to dispose.
        /// </summary>
        public ITrack CreateTrack(uint itemID)
        {
            if (_ipco == null)
                throw new InvalidOperationException("No 'meta' has been read: Parse a container that has one first.");
            return CreateTrack(PropertiesOf(_ipco, _ipma, itemID));
        }

        private ITrack CreateTrack(Box[] propertyBoxes)
        {
            // The track of the codec's configuration, whichever of the properties it is: tried first, as the factory makes a
            // track of any box - a pass-through one, of a box of no codec - and the first property is often the size or the
            // colour, as an AVIF's is. A pass-through track only where nothing else made a track.
            ITrack generic = null;
            ITrack found = null;
            foreach (var box in propertyBoxes.OrderBy(x => CodecConfigurations.Contains(IsoStream.ToFourCC(x.FourCC)) ? 0 : 1))
            {
                ITrack track;
                try
                {
                    track = TrackFactory.CreateTrack(0, box, 0, 0, IsoStream.FromFourCC(HandlerTypes.Video), HandlerNames.Video, Logger);
                }
                catch (NotSupportedException)
                {
                    continue;
                }

                if (track is GenericTrack && generic == null)
                {
                    generic = track;
                    continue;
                }
                if (track is GenericTrack)
                {
                    track.Dispose();
                    continue;
                }

                found = track;
                break;
            }

            if (found == null)
                found = generic;
            else
                generic?.Dispose();

            if (found != null)
                found.Logger = this.Logger;
            return found;
        }

        /// <summary>The type of an item, of its 'infe': 'hvc1', 'grid', 'Exif'; 0 where there is no such item.</summary>
        public uint ItemTypeOf(uint itemID) => _iinf?.ItemInfos.FirstOrDefault(x => x.ItemID == itemID)?.ItemType ?? 0;

        /// <summary>The properties 'ipma' associates an item with, in its order - that transformations are applied in.</summary>
        public IReadOnlyList<Box> PropertiesOf(uint itemID)
        {
            if (_ipco == null)
                throw new InvalidOperationException("No 'meta' has been read: Parse a container that has one first.");
            int itemIndex = _ipma.ItemID == null ? -1 : Array.IndexOf(_ipma.ItemID, itemID);
            if (itemIndex < 0)
                return Array.Empty<Box>();

            // in the order 'ipma' lists them, not that of the 'ipco'
            return _ipma.PropertyIndex[itemIndex]
                .Where(x => x > 0 && x <= _ipco.Children.Count)
                .Select(x => _ipco.Children[x - 1])
                .ToArray();
        }

        /// <summary>The items an item is derived from ('dimg'): a grid's tiles, in the order of the grid; none where it is not derived.</summary>
        public IReadOnlyList<uint> DerivedFrom(uint itemID) => TilesOf(_iref, itemID).ToArray();

        /// <summary>The bytes of an item, by its ID: see <see cref="ReadSample"/> for the items in the order of 'iloc'.</summary>
        public byte[] ReadItem(uint itemID)
        {
            if (this.Meta == null)
                throw new InvalidOperationException("No 'meta' has been read: Parse a container that has one first.");
            int index = Iloc.ItemID == null ? -1 : Array.IndexOf(Iloc.ItemID, itemID);
            if (index < 0)
                throw new InvalidDataException($"Item {itemID} is not in the 'iloc'.");
            return ReadItem(index);
        }

        /// <summary>The description of a grid item (ISO/IEC 23008-12 6.6.2.3): its rows and columns of tiles, and the size of its image.</summary>
        public ImageGrid ReadGrid(uint itemID)
        {
            if (ItemTypeOf(itemID) != Grid)
                throw new InvalidDataException($"Item {itemID} is not a grid.");
            return ImageGrid.Parse(ReadItem(itemID));
        }

        /// <summary>The properties 'ipma' associates an item with, none where it has none.</summary>
        private static Box[] PropertiesOf(ItemPropertyContainerBox ipco, ItemPropertyAssociationBox ipma, uint itemID)
        {
            int itemIndex = ipma.ItemID == null ? -1 : Array.IndexOf(ipma.ItemID, itemID);
            if (itemIndex < 0)
                return Array.Empty<Box>();

            var indexes = ipma.PropertyIndex[itemIndex];
            return ipco.Children.Where((x, idx) => indexes.Contains((ushort)(idx + 1))).ToArray();
        }

        /// <summary>The items an item is derived from ('dimg'), of an 'iref' of either version; none without an 'iref'.</summary>
        private static IEnumerable<uint> TilesOf(ItemReferenceBox iref, uint itemID)
        {
            if (iref == null)
                return Enumerable.Empty<uint>();

            var small = iref.References?.Where(x => x.FromItemID == itemID).OrderBy(x => x.FourCC == DerivedImage ? 0 : 1)
                .Select(x => (x.ToItemID ?? Array.Empty<ushort>()).Select(id => (uint)id));
            var large = iref.References0?.Where(x => x.FromItemID == itemID).OrderBy(x => x.FourCC == DerivedImage ? 0 : 1)
                .Select(x => (IEnumerable<uint>)(x.ToItemID ?? Array.Empty<uint>()));

            return (small ?? Enumerable.Empty<IEnumerable<uint>>()).Concat(large ?? Enumerable.Empty<IEnumerable<uint>>()).FirstOrDefault()
                ?? Enumerable.Empty<uint>();
        }

        /// <summary>The next item, in the order of 'iloc'; null after the last.</summary>
        public ImageSample ReadSample()
        {
            if (this.Meta == null)
                throw new InvalidOperationException("No 'meta' has been read: Parse a container that has one first.");

            return ReadImageSample();
        }

        private ImageSample ReadImageSample()
        {
            if (Iloc.ItemID == null || ImageIndex >= Iloc.ItemID.Length)
                return null;

            return new ImageSample(ReadItem(ImageIndex++));
        }

        /// <summary>
        /// The bytes of the item of a place in 'iloc' (ISO/IEC 14496-12 8.11.3): its extents, one after another, of the file
        /// (construction method 0) or of the 'idat' (1), each from the item's base offset.
        /// </summary>
        private byte[] ReadItem(int index)
        {
            byte constructionMethod = Iloc.ConstructionMethod != null && index < Iloc.ConstructionMethod.Length ? Iloc.ConstructionMethod[index] : (byte)0;
            if (constructionMethod > 1)
                throw new NotSupportedException($"Item {Iloc.ItemID[index]} is made of other items (construction method {constructionMethod}), which is not supported.");
            if (Iloc.DataReferenceIndex != null && index < Iloc.DataReferenceIndex.Length && Iloc.DataReferenceIndex[index] != 0)
                throw new NotSupportedException($"Item {Iloc.ItemID[index]} is in another file (data reference {Iloc.DataReferenceIndex[index]}), which is not supported.");

            long baseOffset = ToInt64(Iloc.BaseOffset[index]);
            var extents = new List<byte[]>();
            long total = 0;
            for (int j = 0; j < Iloc.ExtentCount[index]; j++)
            {
                long offset = baseOffset + ToInt64(Iloc.ExtentOffset[index][j]);
                long length = ToInt64(Iloc.ExtentLength[index][j]);

                IsoStream stream;
                long position;
                if (constructionMethod == 1)
                {
                    var idat = this.Idat?.Data ?? throw new InvalidDataException($"Item {Iloc.ItemID[index]} is in the 'idat', which the 'meta' does not have.");

                    // a length of 0 is all the data there is from the offset
                    if (length == 0)
                        length = idat.Length - offset;
                    if (offset < 0 || length < 0 || offset + length > idat.Length)
                        throw new InvalidDataException($"An extent of item {Iloc.ItemID[index]}, {length} bytes at {offset}, is past the end of the 'idat'.");
                    stream = idat.Stream;
                    position = idat.Position + offset;
                }
                else if (!_data.TryLocate(offset, length, out stream, out position, out long available))
                {
                    throw new InvalidDataException($"An extent of item {Iloc.ItemID[index]}, {length} bytes at {offset}, is in no 'mdat' that was read.");
                }
                else if (length == 0)
                {
                    length = available;
                }

                if (length > int.MaxValue || total + length > int.MaxValue)
                    throw new NotSupportedException($"Item {Iloc.ItemID[index]} is larger than an array holds.");

                if (stream.GetCurrentOffset() != position)
                    stream.SeekFromBeginning(position);

                stream.ReadBytes((ulong)length, out byte[] extent);
                extents.Add(extent);
                total += length;
            }

            if (extents.Count == 1)
                return extents[0];

            var data = new byte[total];
            int at = 0;
            foreach (var extent in extents)
            {
                Buffer.BlockCopy(extent, 0, data, at, extent.Length);
                at += extent.Length;
            }
            return data;
        }

        /// <summary>A field of 'iloc' of 0 to 8 bytes, most significant first.</summary>
        private static long ToInt64(byte[] bytes)
        {
            if (bytes == null)
                return 0;

            ulong value = 0;
            foreach (byte b in bytes)
                value = (value << 8) | b;

            if (value > long.MaxValue)
                throw new InvalidDataException($"An offset or length of 'iloc', {value}, is larger than a file can be.");
            return (long)value;
        }

        public IEnumerable<ArraySegment<byte>> ParseSample(byte[] sample)
        {
            return this.Track.ParseSample(sample);
        }

        /// <summary>Disposes the track the reader made. The container, and the stream it was read from, are the caller's.</summary>
        public void Dispose()
        {
            Track?.Dispose();
            Track = null;
        }
    }
}
