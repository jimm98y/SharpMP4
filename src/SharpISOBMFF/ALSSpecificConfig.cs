namespace SharpISOBMFF
{
    public partial class ALSSpecificConfig
    {
        /// <summary>
        /// The bits of each chan_pos (ISO/IEC 14496-3 11.4.1, Table 11.1): ChBits = ceil[log2(channels+1)],
        /// channels being the number of channels less one.
        /// </summary>
        private static int ChBits(ushort channels)
        {
            int count = channels + 1, bits = 0;
            while ((1 << bits) < count)
                bits++;
            return bits == 0 ? 1 : bits;
        }
    }
}
