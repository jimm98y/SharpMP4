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
