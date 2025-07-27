using SharpISOBMFF;
using SharpMP4.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMP4.Readers
{
    public class ImageReader
    {
        public Container Container { get; set; }
        public ITrack Track { get; set; }
        public MetaBox Meta { get; private set; }
        public MediaDataBox Mdat { get; private set; }
        public ItemLocationBox Iloc { get; private set; }
        public ImageSpatialExtentsProperty Ispe { get; private set; }
        public int ImageIndex { get; private set; }

        public void Parse(Container container)
        {
            if (container.Children.Count == 0)
                return;

            this.Container = container;

            var meta = container.Children.OfType<MetaBox>().Single();
            this.Meta = meta;

            var mdat = container.Children.OfType<MediaDataBox>().FirstOrDefault();
            this.Mdat = mdat;

            var iprp = meta.Children.OfType<ItemPropertiesBox>().Single();
            var ipco = iprp.Children.OfType<ItemPropertyContainerBox>().Single();
            var ipma = iprp.Children.OfType<ItemPropertyAssociationBox>().Single();
            var iref = meta.Children.OfType<ItemReferenceBox>().Single();
            var iinf = meta.Children.OfType<ItemInfoBox>().Single();

            var iloc = meta.Children.OfType<ItemLocationBox>().Single();
            Iloc = iloc;

            var primaryItem = meta.Children.OfType<PrimaryItemBox>().SingleOrDefault();
            uint primaryItemID = primaryItem.ItemID;

            int itemIndex = Array.IndexOf(ipma.ItemID, primaryItemID);
            var indexes = ipma.PropertyIndex[itemIndex];
            var propertyBoxes = ipco.Children.Where((x, idx) => indexes.Contains((ushort)(idx + 1))).ToArray();

            if (iinf.ItemInfos.Single(x => x.ItemID == primaryItemID).ItemType == IsoStream.FromFourCC("grid"))
            {
                // Apple stores HEIC files with a grid of images
                foreach (var tileID in iref.References.Single(x => x.FromItemID == primaryItemID).ToItemID)
                {
                    indexes = ipma.PropertyIndex[Array.IndexOf(ipma.ItemID, tileID)];
                    propertyBoxes = ipco.Children.Where((x, idx) => indexes.Contains((ushort)(idx + 1))).ToArray();
                    break; // let's hope all tiles share the same config
                }
            }

            // try to create a track for any of the property boxes
            foreach (var box in propertyBoxes)
            {
                try
                {
                    this.Track = TrackFactory.CreateTrack(0, box, 0, 0, IsoStream.FromFourCC(HandlerTypes.Video), HandlerNames.Video);
                    break;
                }
                catch (NotSupportedException)
                {
                    // ignore
                }
            }

            if(this.Track == null)
                throw new NotSupportedException("No supported track found in image file.");

            var ispe = propertyBoxes.OfType<ImageSpatialExtentsProperty>().SingleOrDefault();
            this.Ispe = ispe;
        }

        public ImageSample ReadSample()
        {
            if (this.Meta == null)
                throw new InvalidOperationException();

            return ReadImageSample();
        }

        private ImageSample ReadImageSample()
        {
            var ilocItemIndex = Math.Min(ImageIndex++, Iloc.BaseOffset.Length - 1);
            int baseOffset = IsoStream.GetInt(Iloc.BaseOffset[ilocItemIndex]);
            int extentOffset = IsoStream.GetInt(Iloc.ExtentOffset[ilocItemIndex][0]);
            int extentLength = IsoStream.GetInt(Iloc.ExtentLength[ilocItemIndex][0]);

            long startAddress = baseOffset + extentOffset;
            if (this.Mdat.Data.Stream.GetCurrentOffset() != startAddress)
            {
                this.Mdat.Data.Stream.SeekFromBeginning(startAddress);
            }

            ulong size = this.Mdat.Data.Stream.ReadBytes((ulong)extentLength, out byte[] extentData);
            return new ImageSample(extentData);
        }

        public IEnumerable<byte[]> ParseSample(byte[] sample)
        {
            return this.Track.ParseSample(sample);
        }
    }

    public class ImageSample
    {
        public byte[] Data { get; set; }
        public ImageSample(byte[] data)
        {
            this.Data = data;
        }
    }
}
