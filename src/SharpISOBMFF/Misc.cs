using System;
using System.Collections.Generic;

namespace SharpISOBMFF
{
    public static class HandlerTypes
    {
        public const string Video = "vide";
        public const string Sound = "soun";
    }

    public static class HandlerNames
    {
        public const string Video = "Video Handler\0";
        public const string Sound = "Sound Handler\0";
    }

    public class SampleFlags
    {
        public SampleFlags()
        { }

        public SampleFlags(
            byte reserved,
            byte isLeading,
            byte sampleDependsOn,
            byte sampleIsDependedOn,
            byte sampleHasRedundancy,
            byte samplePaddingValue,
            bool sampleIsDifferenceSample,
            int sampleDegradationPriority)
        {
            Reserved = reserved;
            IsLeading = isLeading;
            SampleDependsOn = sampleDependsOn;
            SampleIsDependedOn = sampleIsDependedOn;
            SampleHasRedundancy = sampleHasRedundancy;
            SamplePaddingValue = samplePaddingValue;
            SampleIsDifferenceSample = sampleIsDifferenceSample;
            SampleDegradationPriority = sampleDegradationPriority;
        }

        public byte Reserved { get; set; } = 0;
        public byte IsLeading { get; set; }
        public byte SampleDependsOn { get; set; }
        public byte SampleIsDependedOn { get; set; }
        public byte SampleHasRedundancy { get; set; }
        public byte SamplePaddingValue { get; set; }
        public bool SampleIsDifferenceSample { get; set; }
        public int SampleDegradationPriority { get; set; }

        public static SampleFlags Parse(uint sampleFlags)
        {
            byte reserved = (byte)((sampleFlags & 0xF0000000) >> 28);
            byte isLeading = (byte)((sampleFlags & 0x0C000000) >> 26);
            byte sampleDependsOn = (byte)((sampleFlags & 0x03000000) >> 24);
            byte sampleIsDependedOn = (byte)((sampleFlags & 0x00C00000) >> 22);
            byte sampleHasRedundancy = (byte)((sampleFlags & 0x00300000) >> 20);
            byte samplePaddingValue = (byte)((sampleFlags & 0x000e0000) >> 17);
            bool sampleIsDifferenceSample = (sampleFlags & 0x00010000) >> 16 > 0;
            int sampleDegradationPriority = (int)(sampleFlags & 0x0000ffff);

            return new SampleFlags(
                reserved,
                isLeading,
                sampleDependsOn,
                sampleIsDependedOn,
                sampleHasRedundancy,
                samplePaddingValue,
                sampleIsDifferenceSample,
                sampleDegradationPriority
                );
        }

        public static uint Build(SampleFlags sampleFlags)
        {
            uint ret = (uint)(
                   (sampleFlags.Reserved << 28) |
                   (sampleFlags.IsLeading << 26) |
                   (sampleFlags.SampleDependsOn << 24) |
                   (sampleFlags.SampleIsDependedOn << 22) |
                   (sampleFlags.SampleHasRedundancy << 20) |
                   (sampleFlags.SamplePaddingValue << 17) |
                   ((sampleFlags.SampleIsDifferenceSample ? 1 : 0) << 16) |
                   (sampleFlags.SampleDegradationPriority & 0x0000ffff));
            return ret;
        }

        public static implicit operator UInt32(SampleFlags sampleFlags)
        {
            return Build(sampleFlags);
        }
    }

    public static class AudioSpecificConfigDescriptor
    {
        public static readonly Dictionary<uint, uint> SamplingFrequencyMap = new Dictionary<uint, uint>()
        {
            {  0, 96000 },
            {  1, 88200 },
            {  2, 64000 },
            {  3, 48000 },
            {  4, 44100 },
            {  5, 32000 },
            {  6, 24000 },
            {  7, 22050 },
            {  8, 16000 },
            {  9, 12000 },
            { 10, 11025 },
            { 11,  8000 },
            { 96000,  0 },
            { 88200,  1 },
            { 64000,  2 },
            { 48000,  3 },
            { 44100,  4 },
            { 32000,  5 },
            { 24000,  6 },
            { 22050,  7 },
            { 16000,  8 },
            { 12000,  9 },
            { 11025, 10 },
            {  8000, 11 },
        };
    }
}
