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

        /// <summary>The item read next, by its place in 'iloc'.</summary>
        public int ImageIndex { get; private set; }

        public TrackFactory TrackFactory { get; set; } = new();
        public IMp4Logger Logger { get; set; }

        // where the items are, of a stream that seeks and of one that does not alike
        private FileDataLocator _data;

        private static readonly uint Grid = IsoStream.FromFourCC("grid");
        private static readonly uint DerivedImage = IsoStream.FromFourCC("dimg");

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

            var propertyBoxes = PropertiesOf(ipco, ipma, primaryItemID);

            if (iinf.ItemInfos.FirstOrDefault(x => x.ItemID == primaryItemID)?.ItemType == Grid)
            {
                // Apple stores HEIC files with a grid of images, whose tiles the grid refers to ('dimg')
                var tileID = TilesOf(iref, primaryItemID).Cast<uint?>().FirstOrDefault();
                if (tileID != null)
                    propertyBoxes = PropertiesOf(ipco, ipma, tileID.Value); // let's hope all tiles share the same config
            }

            // try to create a track for any of the property boxes
            foreach (var box in propertyBoxes)
            {
                try
                {
                    this.Track = TrackFactory.CreateTrack(0, box, 0, 0, IsoStream.FromFourCC(HandlerTypes.Video), HandlerNames.Video, Logger);
                    break;
                }
                catch (NotSupportedException)
                {
                    // ignore
                }
            }

            if (this.Track == null)
                throw new NotSupportedException("No supported track found in image file.");
            else
                this.Track.Logger = this.Logger;

            var ispe = propertyBoxes.OfType<ImageSpatialExtentsProperty>().FirstOrDefault();
            this.Ispe = ispe;
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
