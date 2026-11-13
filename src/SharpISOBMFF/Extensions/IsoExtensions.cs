// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using System.IO;
using System.Linq;

namespace SharpISOBMFF.Extensions
{
    public static class IMp4SerializableExtensions
    {
        public static byte[] ToBytes(this IMp4Serializable box)
        {
            var ms = new MemoryStream();
            using (var isoStream = new IsoStream(ms))
            {
                box.Write(isoStream);
                return ms.ToArray();
            }
        }
    }

    public static class ChunkOffsetBoxExtensions
    {
        public static void ModifyChunkOffsets(this ChunkOffsetBox stco, int delta)
        {
            for (int i = 0; i < stco.ChunkOffset.Length; i++)
            {
                stco.ChunkOffset[i] = (uint)((int)stco.ChunkOffset[i] + delta);
            }
        }
    }

    public static class ChunkLargeOffsetBoxExtensions
    {
        public static void ModifyChunkOffsets(this ChunkLargeOffsetBox co64, long delta)
        {
            for (int i = 0; i < co64.ChunkOffset.Length; i++)
            {
                co64.ChunkOffset[i] = (ulong)((long)co64.ChunkOffset[i] + delta);
            }
        }
    }

    public static class MovieHeaderBoxExtensions
    {
        public static void ModifyChunkOffsets(this MovieBox moov, long delta)
        {
            if (delta == 0)
                return;

            foreach (var track in moov.Children.OfType<TrackBox>())
            {
                var mdia = track.Children.OfType<MediaBox>().Single();
                var minf = mdia.Children.OfType<MediaInformationBox>().Single();
                var stbl = minf.Children.OfType<SampleTableBox>().Single();
                var stco = stbl.Children.OfType<ChunkOffsetBox>().SingleOrDefault();
                var co64 = stbl.Children.OfType<ChunkLargeOffsetBox>().SingleOrDefault();

                if (stco != null)
                {
                    stco.ModifyChunkOffsets((int)delta);
                }
                else if (co64 != null)
                {
                    co64.ModifyChunkOffsets(delta);
                }
            }
        }
    }
}
