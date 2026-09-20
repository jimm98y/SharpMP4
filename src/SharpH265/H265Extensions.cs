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

            // The coded size is padded up to whole coding blocks and the conformance window says
            // how much of that padding to drop. 1920x1080 is coded as 1920x1088 with 8 lines
            // cropped, so ignoring the window puts the padding on screen.
            if (sps.ConformanceWindowFlag != 0)
            {
                ulong chromaArrayType = sps.SeparateColourPlaneFlag == 0 ? sps.ChromaFormatIdc : 0;

                // 7.4.3.2.1: SubWidthC and SubHeightC follow from the chroma format.
                ulong subWidthC = chromaArrayType == 1 || chromaArrayType == 2 ? 2ul : 1ul;
                ulong subHeightC = chromaArrayType == 1 ? 2ul : 1ul;

                width -= subWidthC * (sps.ConfWinLeftOffset + sps.ConfWinRightOffset);
                height -= subHeightC * (sps.ConfWinTopOffset + sps.ConfWinBottomOffset);
            }

            return ((uint)width, (uint)height);
        }
    }
}
