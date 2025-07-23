using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMP4.Tracks
{
    public static class H26XTrackUtils
    {
        public static IEnumerable<byte[]> ParseSample(byte[] sample, int nalLengthSize)
        {
            ulong size = 0;
            long offsetInBytes = 0;

            if (SharpISOBMFF.Log.DebugEnabled) SharpISOBMFF.Log.Debug($"{nameof(H26XTrackUtils)}: AU begin {sample.Length}");

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
                        if (SharpISOBMFF.Log.ErrorEnabled) SharpISOBMFF.Log.Error($"{nameof(H26XTrackUtils)}: Invalid NALU size: {nalUnitLength}");
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

                if (SharpISOBMFF.Log.DebugEnabled) SharpISOBMFF.Log.Debug($"{nameof(H26XTrackUtils)}: AU end");

                return naluList;
            }
        }
    }
}
