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
