using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpISOBMFF
{
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
