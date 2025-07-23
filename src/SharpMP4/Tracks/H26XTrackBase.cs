using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Tracks
{
    public abstract class H26XTrackBase : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        public int NalLengthSize { get; set; } = 4;

        protected H26XTrackBase() : base()
        {
            DefaultSampleFlags = new SampleFlags() { SampleDependsOn = 1, SampleIsDifferenceSample = true };
            TimescaleFallback = 24000;
            FrameTickFallback = 1001;
        }

        public IEnumerable<byte[]> ParseSample(byte[] sample, int nalLengthSize)
        {
            ulong size = 0;
            long offsetInBytes = 0;

            if (Log.DebugEnabled) Log.Debug($"{nameof(H26XTrackBase)}: AU begin {sample.Length}");

            List<byte[]> naluList = new List<byte[]>();

            using (var markerStream = new IsoStream(new MemoryStream(sample)))
            {
                do
                {
                    uint nalUnitLength = 0;
                    size += markerStream.ReadVariableLengthSize((uint)nalLengthSize, out nalUnitLength);
                    offsetInBytes += nalLengthSize;

                    if (nalUnitLength > (sample.Length - offsetInBytes))
                    {
                        if (Log.ErrorEnabled) Log.Error($"{nameof(H26XTrackBase)}: Invalid NALU size: {nalUnitLength}");
                        nalUnitLength = (uint)(sample.Length - offsetInBytes);
                        size += nalUnitLength;
                        offsetInBytes += nalUnitLength;
                        break;
                    }

                    size += markerStream.ReadUInt8Array(size, (ulong)sample.Length, nalUnitLength, out byte[] sampleData);
                    offsetInBytes += sampleData.Length;
                    naluList.Add(sampleData);
                } while (offsetInBytes < sample.Length);

                if (offsetInBytes != sample.Length)
                    throw new Exception("Mismatch!");

                if (Log.DebugEnabled) Log.Debug($"{nameof(H26XTrackBase)}: AU end");

                return naluList;
            }
        }
    }
}
