using SharpH263;
using SharpISOBMFF;
using System;
using System.Collections.Generic;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H.263 track (ITU-T Rec. H.263): its samples pictures, each from its picture start code to the next; in an 's263' sample
    /// entry with a 'd263' (3GPP TS 26.244), whose profile and level the stream does not say - they are negotiated outside it
    /// (Annex X) - so are taken as the baseline profile and the lowest level of the picture's size, unless set.
    /// </summary>
    public class H263Track : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";

        private readonly H263Context _context = new H263Context();

        /// <summary>The vendor of the 'd263': four characters.</summary>
        public string Vendor { get; set; } = "smp4";

        /// <summary>The H.263 profile of the 'd263' (Annex X): 0, the baseline profile, unless set.</summary>
        public byte Profile { get; set; }

        /// <summary>The H.263 level of the 'd263' (Annex X): 0, the lowest the picture size fits, unless set.</summary>
        public byte Level { get; set; }

        private int _width, _height;
        private uint _pixelAspectH = 12, _pixelAspectV = 11;

        public H263Track()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 30000;
            FrameTickFallback = 1001;
        }

        public H263Track(uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;
        }

        /// <summary>A track of a 'd263', as a file has it: its profile and level kept, its size the entry's.</summary>
        public H263Track(Box config, uint timescale, int sampleDuration) : this(timescale, sampleDuration)
        {
            if (config is not H263SpecificBox d263)
                throw new ArgumentException($"Invalid H263SpecificBox: {config?.FourCC}");
            Vendor = IsoStream.ToFourCC(d263.Vendor);
            Profile = d263.Profile;
            Level = d263.Level;
            if (config.GetParent() is VisualSampleEntry entry)
            {
                _width = entry.Width;
                _height = entry.Height;
            }
        }

        /// <summary>
        /// A picture, from its picture start code to the next (<see cref="H263Context.Pictures"/>): a sample of its own, a sync
        /// sample where it is an I-picture. Its header is read for the picture's size and pixel aspect ratio.
        /// </summary>
        public override void ProcessSample(byte[] buffer, int offset, int length, out ArraySegment<byte> output, out bool isRandomAccessPoint)
        {
            output = default;
            isRandomAccessPoint = false;
            if (buffer == null || length == 0)
                return;

            output = new ArraySegment<byte>(buffer, offset, length);
            try
            {
                var picture = _context.ReadPicture(H263Context.StreamOf(buffer, offset, length, Logger));
                isRandomAccessPoint = _context.PictureType == H263PictureTypes.I;
                _width = _context.Width;
                _height = _context.Height;
                if (!_context.HasPlusPtype)
                {
                    // the standard source formats' samples are 12:11 (5.1.3)
                    (_pixelAspectH, _pixelAspectV) = (12, 11);
                }
                else if (picture.Ufep == 1 && picture.OpptypeSourceFormat == 6)
                {
                    (_pixelAspectH, _pixelAspectV) = picture.PixelAspectRatioCode switch
                    {
                        1 => (1u, 1u),
                        2 => (12u, 11u),
                        3 => (10u, 11u),
                        4 => (16u, 11u),
                        5 => (40u, 33u),
                        15 => (picture.ParWidth, picture.ParHeight),
                        _ => (1u, 1u),
                    };
                }
                else if (picture.Ufep == 1)
                {
                    (_pixelAspectH, _pixelAspectV) = (12, 11);
                }
            }
            catch (Exception ex)
            {
                // a picture whose header is not read - of Annex N's videomux, or Annex P's resampling - is still a sample
                if (Logger.IsWarningEnabled) Logger.LogWarning($"An H.263 picture header could not be read: {ex.Message}");
            }
        }

        /// <summary>The lowest level of Annex X (Table X.2) the picture's size fits: QCIF, CIF, 720x288, 720x576.</summary>
        private byte LevelOf()
        {
            if (_width <= 176 && _height <= 144)
                return 10;
            if (_width <= 352 && _height <= 288)
                return 20;
            if (_width <= 720 && _height <= 288)
                return 60;
            return 70;
        }

        public override Box CreateSampleEntryBox()
        {
            var entry = new VisualSampleEntry(IsoStream.FromFourCC("s263"));
            entry.Children = new List<Box>();
            entry.ReservedSampleEntry = new byte[6];
            entry.PreDefined0 = new uint[3];
            entry.DataReferenceIndex = 1;
            entry.Depth = 24;
            entry.FrameCount = 1;
            entry.Horizresolution = 72 << 16;
            entry.Vertresolution = 72 << 16;
            entry.Width = (ushort)_width;
            entry.Height = (ushort)_height;
            entry.Compressorname = BinaryUTF8String.GetBytes("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");

            var d263 = new H263SpecificBox
            {
                Vendor = IsoStream.FromFourCC((Vendor ?? "    ").PadRight(4).Substring(0, 4)),
                DecoderVersion = 0,
                Level = Level != 0 ? Level : LevelOf(),
                Profile = Profile,
            };
            d263.SetParent(entry);
            entry.Children.Add(d263);

            if (_pixelAspectH != _pixelAspectV && _pixelAspectH != 0 && _pixelAspectV != 0)
            {
                var pasp = new PixelAspectRatioBox { HSpacing = _pixelAspectH, VSpacing = _pixelAspectV };
                pasp.SetParent(entry);
                entry.Children.Add(pasp);
            }
            return entry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // the width at which the samples are shown square
            ulong width = _pixelAspectV == 0 ? (ulong)_width : (ulong)_width * _pixelAspectH / _pixelAspectV;
            tkhd.Width = (uint)width << 16;
            tkhd.Height = (uint)_height << 16;
        }

        public override ITrack Clone()
        {
            return CopySettingsTo(new H263Track(Timescale, DefaultSampleDuration)
            {
                Vendor = Vendor,
                Profile = Profile,
                Level = Level,
                _width = _width,
                _height = _height,
                _pixelAspectH = _pixelAspectH,
                _pixelAspectV = _pixelAspectV,
            });
        }
    }
}
