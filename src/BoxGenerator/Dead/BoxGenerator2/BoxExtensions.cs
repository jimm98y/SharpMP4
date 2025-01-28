namespace SharpMP4
{
    public static class BoxHeaderExtensions
    {
        public static ulong GetBoxSizeInBits(this BoxHeader header)
        {
            if (header.Size == 1)
                return header.Largesize << 3;

            ulong size = (ulong)header.Size << 3;
            return size;
        }

        public static ulong GetHeaderSizeInBits(this BoxHeader header)
        {
            return header.CalculateSize();
        }
    }
}
