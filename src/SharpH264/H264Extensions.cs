namespace SharpH264
{
    public static class H264Extensions
    {
        public static (uint Width, uint Height) CalculateDimensions(this SeqParameterSetRbsp sps)
        {
            var spsData = sps.SeqParameterSetData;
            ulong width = (spsData.PicWidthInMbsMinus1 + 1) * 16;
            ulong mult = 2;
            if (spsData.FrameMbsOnlyFlag != 0)
            {
                mult = 1;
            }
            ulong height = 16 * (spsData.PicHeightInMapUnitsMinus1 + 1) * mult;
            if (spsData.FrameCroppingFlag != 0)
            {
                ulong chromaFormat = spsData.ChromaFormatIdc;
                ulong chromaArrayType = 0;
                if (spsData.SeparateColourPlaneFlag == 0)
                {
                    chromaArrayType = chromaFormat;
                }

                ulong cropUnitX = 1;
                ulong cropUnitY = mult;
                if (chromaArrayType != 0)
                {
                    uint subWidth = 2;
                    uint subHeight = 1;
                    if (chromaFormat == 3)
                        subWidth = 1;
                    if (chromaFormat == 1)
                        subHeight = 2;

                    cropUnitX = subWidth;
                    cropUnitY = subHeight * mult;
                }

                width -= cropUnitX * (spsData.FrameCropLeftOffset + spsData.FrameCropRightOffset);
                height -= cropUnitY * (spsData.FrameCropTopOffset + spsData.FrameCropBottomOffset);
            }
            return ((uint)width, (uint)height);
        }

        public static (uint Timescale, uint FrameTick) CalculateTimescale(this SeqParameterSetRbsp sps)
        {
            uint timescale = 0;
            uint frametick = 0;
            var vui = sps.SeqParameterSetData.VuiParameters;
            if (vui != null && vui.TimingInfoPresentFlag != 0)
            {
                // MaxFPS = Ceil( time_scale / ( 2 * num_units_in_tick ) )
                timescale = vui.TimeScale;
                frametick = vui.NumUnitsInTick;

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
