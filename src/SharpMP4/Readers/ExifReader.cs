using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpMP4.Readers
{
    /// <summary>
    /// The tags of Exif (CIPA DC-008) - of an Exif item of a HEIF image (ISO/IEC 23008-12 A.2.1): an offset to the TIFF
    /// header, then TIFF (TIFF 6.0) - its 0th IFD, the Exif, GPS and interoperability IFDs it points to, and the 1st IFD,
    /// of the thumbnail. Each tag keyed by its name, of its IFD; its value as it is: ASCII with its terminating zero, a
    /// rational as numerator/denominator.
    /// </summary>
    internal static class ExifReader
    {
        public static void Read(byte[] item, Box box, List<MetadataTag> tags)
        {
            if (item == null || item.Length < 4)
                return;
            // exif_tiff_header_offset: from after it, where the TIFF header is - after "Exif\0\0", as JPEG's APP1 has it
            long tiff = 4 + ((long)item[0] << 24 | (long)item[1] << 16 | (long)item[2] << 8 | item[3]);
            if (tiff + 8 > item.Length)
                return;
            bool little;
            if (item[tiff] == 'I' && item[tiff + 1] == 'I')
                little = true;
            else if (item[tiff] == 'M' && item[tiff + 1] == 'M')
                little = false;
            else
                return;

            var reader = new Tiff(item, (int)tiff, little);
            if (reader.UInt16(2) != 42)
                return;

            // each IFD once, whatever points to it: a loop of offsets reads no IFD twice
            var visited = new HashSet<long>();
            uint next = reader.ReadIfd(reader.UInt32(4), "IFD0", box, tags, visited);
            if (next != 0)
                reader.ReadIfd(next, "IFD1", box, tags, visited);
        }

        private sealed class Tiff
        {
            private readonly byte[] _bytes;
            private readonly int _start;
            private readonly bool _little;

            public Tiff(byte[] bytes, int start, bool little)
            {
                _bytes = bytes;
                _start = start;
                _little = little;
            }

            private bool Has(long offset, long length) => offset >= 0 && length >= 0 && _start + offset + length <= _bytes.Length;

            public ushort UInt16(long offset)
            {
                int at = (int)(_start + offset);
                return _little ? (ushort)(_bytes[at] | _bytes[at + 1] << 8) : (ushort)(_bytes[at] << 8 | _bytes[at + 1]);
            }

            public uint UInt32(long offset)
            {
                int at = (int)(_start + offset);
                return _little
                    ? (uint)(_bytes[at] | _bytes[at + 1] << 8 | _bytes[at + 2] << 16 | _bytes[at + 3] << 24)
                    : (uint)(_bytes[at] << 24 | _bytes[at + 1] << 16 | _bytes[at + 2] << 8 | _bytes[at + 3]);
            }

            /// <summary>The tags of an IFD, and of the IFDs it points to; the offset of the IFD after it, 0 of none.</summary>
            public uint ReadIfd(uint offset, string ifd, Box box, List<MetadataTag> tags, HashSet<long> visited)
            {
                if (offset == 0 || !visited.Add(offset) || !Has(offset, 2))
                    return 0;
                int count = UInt16(offset);
                if (!Has(offset + 2, count * 12L + 4))
                    return 0;
                for (int i = 0; i < count; i++)
                {
                    long entry = offset + 2 + i * 12L;
                    ushort tag = UInt16(entry);
                    ushort type = UInt16(entry + 2);
                    uint values = UInt32(entry + 4);

                    string sub = tag switch { 0x8769 => "Exif", 0x8825 => "GPS", 0xA005 => "Interop", _ => null };
                    if (sub != null)
                    {
                        ReadIfd(UInt32(entry + 8), sub, box, tags, visited);
                        continue;
                    }

                    int size = SizeOf(type);
                    long length = size * (long)values;
                    // of 4 bytes or fewer, the value itself; else where it is
                    long at = length <= 4 ? entry + 8 : UInt32(entry + 8);
                    object value = size == 0 || !Has(at, length) ? null : Value(type, values, at);
                    tags.Add(new MetadataTag
                    {
                        Family = MetadataFamily.Exif,
                        Namespace = ifd,
                        Key = NameOf(ifd, tag),
                        Value = value,
                        Box = box,
                    });
                }
                return UInt32(offset + 2 + count * 12L);
            }

            private static int SizeOf(ushort type) => type switch
            {
                1 or 2 or 6 or 7 => 1,  // BYTE, ASCII, SBYTE, UNDEFINED
                3 or 8 => 2,            // SHORT, SSHORT
                4 or 9 or 11 => 4,      // LONG, SLONG, FLOAT
                5 or 10 or 12 => 8,     // RATIONAL, SRATIONAL, DOUBLE
                _ => 0,
            };

            private object Value(ushort type, uint count, long at)
            {
                switch (type)
                {
                    case 2:
                        // ASCII as it is, its terminating zero too
                        return Encoding.ASCII.GetString(_bytes, (int)(_start + at), (int)count);
                    case 7:
                    {
                        var bytes = new byte[count];
                        Buffer.BlockCopy(_bytes, (int)(_start + at), bytes, 0, (int)count);
                        // of printable ASCII - ExifVersion's "0232" - as text
                        bool text = count > 0 && Array.TrueForAll(bytes, b => b >= 0x20 && b < 0x7F);
                        return text ? Encoding.ASCII.GetString(bytes) : bytes;
                    }
                    case 5:
                    case 10:
                    {
                        var rationals = new string[count];
                        for (int i = 0; i < count; i++)
                        {
                            uint numerator = UInt32(at + 8L * i), denominator = UInt32(at + 8L * i + 4);
                            rationals[i] = type == 5
                                ? $"{numerator}/{denominator}"
                                : $"{(int)numerator}/{(int)denominator}";
                        }
                        return count == 1 ? rationals[0] : rationals;
                    }
                    default:
                    {
                        var numbers = new long[count];
                        for (int i = 0; i < count; i++)
                        {
                            numbers[i] = type switch
                            {
                                1 => _bytes[_start + at + i],
                                6 => (sbyte)_bytes[_start + at + i],
                                3 => UInt16(at + 2L * i),
                                8 => (short)UInt16(at + 2L * i),
                                4 => UInt32(at + 4L * i),
                                9 => (int)UInt32(at + 4L * i),
                                _ => 0,
                            };
                        }
                        if (type is 11 or 12)
                            return Floats(type, count, at);
                        return count == 1 ? numbers[0] : numbers;
                    }
                }
            }

            private object Floats(ushort type, uint count, long at)
            {
                var values = new double[count];
                for (int i = 0; i < count; i++)
                {
                    if (type == 11)
                    {
                        values[i] = BitConverterSingle(UInt32(at + 4L * i));
                    }
                    else
                    {
                        ulong high = UInt32(at + 8L * i + (_little ? 4 : 0)), low = UInt32(at + 8L * i + (_little ? 0 : 4));
                        values[i] = BitConverter.Int64BitsToDouble((long)(high << 32 | low));
                    }
                }
                return count == 1 ? values[0] : string.Join(" ", Array.ConvertAll(values, v => v.ToString(CultureInfo.InvariantCulture)));
            }

            private static float BitConverterSingle(uint bits) => BitConverter.ToSingle(BitConverter.GetBytes(bits), 0);
        }

        /// <summary>The name of a tag, of its IFD (CIPA DC-008, TIFF 6.0); of one not known, its number.</summary>
        private static string NameOf(string ifd, ushort tag)
        {
            if (ifd == "GPS")
            {
                switch (tag)
                {
                    case 0x0000: return "GPSVersionID";
                    case 0x0001: return "GPSLatitudeRef";
                    case 0x0002: return "GPSLatitude";
                    case 0x0003: return "GPSLongitudeRef";
                    case 0x0004: return "GPSLongitude";
                    case 0x0005: return "GPSAltitudeRef";
                    case 0x0006: return "GPSAltitude";
                    case 0x0007: return "GPSTimeStamp";
                    case 0x000C: return "GPSSpeedRef";
                    case 0x000D: return "GPSSpeed";
                    case 0x0010: return "GPSImgDirectionRef";
                    case 0x0011: return "GPSImgDirection";
                    case 0x0012: return "GPSMapDatum";
                    case 0x001B: return "GPSProcessingMethod";
                    case 0x001D: return "GPSDateStamp";
                }
                return $"0x{tag:X4}";
            }
            if (ifd == "Interop")
            {
                switch (tag)
                {
                    case 0x0001: return "InteroperabilityIndex";
                    case 0x0002: return "InteroperabilityVersion";
                }
                return $"0x{tag:X4}";
            }
            switch (tag)
            {
                case 0x0100: return "ImageWidth";
                case 0x0101: return "ImageLength";
                case 0x0102: return "BitsPerSample";
                case 0x0103: return "Compression";
                case 0x0106: return "PhotometricInterpretation";
                case 0x010E: return "ImageDescription";
                case 0x010F: return "Make";
                case 0x0110: return "Model";
                case 0x0112: return "Orientation";
                case 0x0115: return "SamplesPerPixel";
                case 0x011A: return "XResolution";
                case 0x011B: return "YResolution";
                case 0x0128: return "ResolutionUnit";
                case 0x0131: return "Software";
                case 0x0132: return "DateTime";
                case 0x013B: return "Artist";
                case 0x0201: return "JPEGInterchangeFormat";
                case 0x0202: return "JPEGInterchangeFormatLength";
                case 0x0213: return "YCbCrPositioning";
                case 0x8298: return "Copyright";
                case 0x829A: return "ExposureTime";
                case 0x829D: return "FNumber";
                case 0x8822: return "ExposureProgram";
                case 0x8827: return "PhotographicSensitivity";
                case 0x8830: return "SensitivityType";
                case 0x9000: return "ExifVersion";
                case 0x9003: return "DateTimeOriginal";
                case 0x9004: return "DateTimeDigitized";
                case 0x9010: return "OffsetTime";
                case 0x9011: return "OffsetTimeOriginal";
                case 0x9012: return "OffsetTimeDigitized";
                case 0x9101: return "ComponentsConfiguration";
                case 0x9201: return "ShutterSpeedValue";
                case 0x9202: return "ApertureValue";
                case 0x9203: return "BrightnessValue";
                case 0x9204: return "ExposureBiasValue";
                case 0x9205: return "MaxApertureValue";
                case 0x9206: return "SubjectDistance";
                case 0x9207: return "MeteringMode";
                case 0x9208: return "LightSource";
                case 0x9209: return "Flash";
                case 0x920A: return "FocalLength";
                case 0x9214: return "SubjectArea";
                case 0x927C: return "MakerNote";
                case 0x9286: return "UserComment";
                case 0x9290: return "SubSecTime";
                case 0x9291: return "SubSecTimeOriginal";
                case 0x9292: return "SubSecTimeDigitized";
                case 0xA000: return "FlashpixVersion";
                case 0xA001: return "ColorSpace";
                case 0xA002: return "PixelXDimension";
                case 0xA003: return "PixelYDimension";
                case 0xA20E: return "FocalPlaneXResolution";
                case 0xA20F: return "FocalPlaneYResolution";
                case 0xA210: return "FocalPlaneResolutionUnit";
                case 0xA217: return "SensingMethod";
                case 0xA300: return "FileSource";
                case 0xA301: return "SceneType";
                case 0xA401: return "CustomRendered";
                case 0xA402: return "ExposureMode";
                case 0xA403: return "WhiteBalance";
                case 0xA404: return "DigitalZoomRatio";
                case 0xA405: return "FocalLengthIn35mmFilm";
                case 0xA406: return "SceneCaptureType";
                case 0xA408: return "Contrast";
                case 0xA409: return "Saturation";
                case 0xA40A: return "Sharpness";
                case 0xA40C: return "SubjectDistanceRange";
                case 0xA420: return "ImageUniqueID";
                case 0xA430: return "CameraOwnerName";
                case 0xA431: return "BodySerialNumber";
                case 0xA432: return "LensSpecification";
                case 0xA433: return "LensMake";
                case 0xA434: return "LensModel";
                case 0xA435: return "LensSerialNumber";
            }
            return $"0x{tag:X4}";
        }
    }
}
