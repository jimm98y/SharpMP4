namespace SharpH266
{
    public static class H266Extensions
    {
        public static (uint Width, uint Height) CalculateDimensions(this SeqParameterSetRbsp sps)
        {
            ulong width = sps.SpsPicWidthMaxInLumaSamples;
            ulong height = sps.SpsPicHeightMaxInLumaSamples;
            return ((uint)width, (uint)height);
        }

        public static (uint Timescale, uint FrameTick) CalculateTimescale(this SeqParameterSetRbsp sps)
        {
            uint timescale = 0;
            uint frametick = 0;
            if (sps.GeneralTimingHrdParameters != null)
            {
                timescale = sps.GeneralTimingHrdParameters.TimeScale;
                frametick = sps.GeneralTimingHrdParameters.NumUnitsInTick;

                if (timescale == 0 || frametick == 0)
                {
                    timescale = 0;
                    frametick = 0;
                }
            }

            return (timescale, frametick);
        }
    }
}
