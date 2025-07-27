namespace SharpH265
{
    public static class H265Extensions
    {
        public static (uint Timescale, uint FrameTick) CalculateTimescale(this SeqParameterSetRbsp sps)
        {
            uint timescale = 0;
            uint frametick = 0;
            var vui = sps.VuiParameters;
            if (vui != null && vui.VuiTimingInfoPresentFlag != 0)
            {
                timescale = vui.VuiTimeScale;
                frametick = vui.VuiNumUnitsInTick;

                if (timescale == 0 || frametick == 0)
                {
                    timescale = 0;
                    frametick = 0;
                }
            }

            return (timescale, frametick);
        }

        public static (uint Width, uint Height) CalculateDimensions(this SeqParameterSetRbsp sps)
        {
            ulong width = sps.PicWidthInLumaSamples;
            ulong height = sps.PicHeightInLumaSamples;
            return ((uint)width, (uint)height);
        }
    }
}
