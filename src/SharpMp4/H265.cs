using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpMp4
{
    public class H265Track : TrackBase
    {
        public Dictionary<int, H265SpsNalUnit> Sps { get; set; } = new Dictionary<int, H265SpsNalUnit>();
        public Dictionary<int, H265PpsNalUnit> Pps { get; set; } = new Dictionary<int, H265PpsNalUnit>();
        public Dictionary<int, H265VpsNalUnit> Vps { get; set; } = new Dictionary<int, H265VpsNalUnit>();

        public H265Track(uint trackID, uint timescale) : base(trackID)
        {
            base.Handler = "vide";
            this.Timescale = timescale;
        }

        private H265NalSliceHeader _lastSliceHeader;
        private List<byte[]> _nalBuffer = new List<byte[]>();

        public override async Task ProcessSampleAsync(byte[] sample, uint duration)
        {
            var header = H265NalUnitHeader.ParseNALHeader(sample[0]);
            if (header.NalUnitType == H265NalUnitTypes.SPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed SPS: {ToHexString(sample)}");
                var sps = H265SpsNalUnit.Parse(sample);
                if (!Sps.ContainsKey(sps.SeqParameterSetId))
                {
                    Sps.Add(sps.SeqParameterSetId, sps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt SPS: {ToHexString(H265SpsNalUnit.Build(sps))}");
            }
            else if (header.NalUnitType == H265NalUnitTypes.PPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed PPS: {ToHexString(sample)}");
                var pps = H265PpsNalUnit.Parse(sample);
                if (!Pps.ContainsKey(pps.PicParameterSetId))
                {
                    Pps.Add(pps.PicParameterSetId, pps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt PPS: {ToHexString(H265PpsNalUnit.Build(pps))}");
            }
            else if (header.NalUnitType == H265NalUnitTypes.VPS)
            {
                if (Log.DebugEnabled) Log.Debug($"-Parsed VPS: {ToHexString(sample)}");
                var vps = H265VpsNalUnit.Parse(sample);
                if (!Vps.ContainsKey(vps.VpsParameterSetId))
                {
                    Vps.Add(vps.VpsParameterSetId, vps);
                }
                if (Log.DebugEnabled) Log.Debug($"Rebuilt VPS: {ToHexString(H265VpsNalUnit.Build(vps))}");
            }
            else
            {
                var sliceHeader = ReadSliceHeader(sample);

                _nalBuffer.Add(sample);

                if (IsNewSample(_lastSliceHeader, sliceHeader))
                {
                    int len = sample.Length;
                    IEnumerable<byte> result = new byte[0];

                    foreach (var nal in _nalBuffer)
                    {
                        // for each NAL, add 4 byte NAL size
                        byte[] size = new byte[] {
                            (byte)((len & 0xff000000) >> 24),
                            (byte)((len & 0xff0000) >> 16),
                            (byte)((len & 0xff00) >> 8),
                            (byte)(len & 0xff)
                        };
                        result = result.Concat(size).Concat(sample);
                    }

                    await base.ProcessSampleAsync(result.ToArray(), duration);
                    _nalBuffer.Clear();
                }

                _lastSliceHeader = sliceHeader;
            }
        }

        public static bool IsNewSample(H265NalSliceHeader oldHeader, H265NalSliceHeader newHeader)
        {
            throw new NotImplementedException();
        }

        private H265NalSliceHeader ReadSliceHeader(byte[] sample)
        {
            throw new NotImplementedException();
        }
    }

    public class H265VpsNalUnit
    {
        public int VpsParameterSetId { get; set; }

        public static byte[] Build(H265VpsNalUnit vps)
        {
            throw new NotImplementedException();
        }

        public static H265VpsNalUnit Parse(byte[] sample)
        {
            throw new NotImplementedException();
        }
    }

    public class H265SpsNalUnit
    {
        public int SeqParameterSetId { get; set; }

        public static byte[] Build(H265SpsNalUnit sps)
        {
            throw new NotImplementedException();
        }

        public static H265SpsNalUnit Parse(byte[] sample)
        {
            throw new NotImplementedException();
        }

        public (ushort Width, ushort Height) CalculateDimensions()
        {
            throw new NotImplementedException();
        }
    }

    public class H265PpsNalUnit
    {
        public int PicParameterSetId { get; set; }

        public static byte[] Build(H265PpsNalUnit pps)
        {
            throw new NotImplementedException();
        }

        public static H265PpsNalUnit Parse(byte[] sample)
        {
            throw new NotImplementedException();
        }
    }

    public class H265NalUnitHeader
    {
        public int NalUnitType { get; set; }

        public static H265NalUnitHeader ParseNALHeader(byte header)
        {
            throw new NotImplementedException();
        }
    }

    public class H265NalUnitTypes
    {
        public const int VPS = 32;
        public const int SPS = 33;
        public const int PPS = 34;
    }

    public class H265NalSliceHeader
    {
    }

    public class H265BoxBuilder
    {
        public static VisualSampleEntryBox CreateVisualSampleEntryBox(Mp4Box parent, H265Track h265Track)
        {
            throw new NotImplementedException();
        }
    }
}
