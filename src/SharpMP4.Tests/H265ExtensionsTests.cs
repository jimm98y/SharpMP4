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

using SharpH265;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="H265Extensions"/>.</summary>
[TestClass]
public class H265ExtensionsTests
{
    /// <summary>
    /// 1080p is coded as 1920x1088, because the height is padded up to whole coding blocks, and
    /// the conformance window says to drop the 8 lines. Reporting the coded size put those lines
    /// on screen and made the picture the wrong shape.
    /// </summary>
    [TestMethod]
    public void AppliesTheConformanceWindowToTheCodedSize()
    {
        var sps = new SeqParameterSetRbsp
        {
            ChromaFormatIdc = 1,            // 4:2:0, so the offsets count chroma samples
            PicWidthInLumaSamples = 1920,
            PicHeightInLumaSamples = 1088,
            ConformanceWindowFlag = 1,
            ConfWinBottomOffset = 4,        // 4 chroma rows = 8 luma lines
        };

        Assert.AreEqual((1920u, 1080u), sps.CalculateDimensions());
    }

    /// <summary>Without a window the coded size is what is displayed.</summary>
    [TestMethod]
    public void ReportsTheCodedSizeWhenThereIsNoConformanceWindow()
    {
        var sps = new SeqParameterSetRbsp
        {
            ChromaFormatIdc = 1,
            PicWidthInLumaSamples = 1280,
            PicHeightInLumaSamples = 720,
            ConformanceWindowFlag = 0,
            ConfWinBottomOffset = 4,        // ignored while the flag is clear
        };

        Assert.AreEqual((1280u, 720u), sps.CalculateDimensions());
    }

    /// <summary>
    /// 4:4:4 has no chroma subsampling, so the same offset crops half as many lines as it does in
    /// 4:2:0; the units are chroma samples, not luma.
    /// </summary>
    [TestMethod]
    public void CountsTheOffsetsInChromaSamples()
    {
        var sps = new SeqParameterSetRbsp
        {
            ChromaFormatIdc = 3,            // 4:4:4, SubWidthC = SubHeightC = 1
            PicWidthInLumaSamples = 1920,
            PicHeightInLumaSamples = 1088,
            ConformanceWindowFlag = 1,
            ConfWinRightOffset = 8,
            ConfWinBottomOffset = 4,
        };

        Assert.AreEqual((1912u, 1084u), sps.CalculateDimensions());
    }
}
