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

using SharpH262;

namespace SharpMP4.Tests;

/// <summary>Tests against <see cref="H262Context"/>.</summary>
[TestClass]
public class H262ContextTests
{
    // picture_header( ): temporal_reference 0, picture_coding_type, vbv_delay 0xFFFF, no extra_bit_picture
    private static readonly byte[] IPicture = [0x00, 0x00, 0x01, 0x00, 0x00, 0x0F, 0xFF, 0xF8];
    private static readonly byte[] ForbiddenPicture = [0x00, 0x00, 0x01, 0x00, 0x00, 0x07, 0xFF, 0xF8];

    // slice( ) of vertical position 1: quantiser_scale_code, no slice_extension_flag, then its macroblocks
    private static readonly byte[] Slice = [0x00, 0x00, 0x01, 0x01, 0x08, 0x80, 0x00];
    private static readonly byte[] ForbiddenSlice = [0x00, 0x00, 0x01, 0x01, 0x00, 0x80, 0x00];

    private static object Read(byte[] unit, bool strict) =>
        new H262Context { Strict = strict }.ReadUnit(H262Context.StreamOf(unit, 0, unit.Length));

    /// <summary>
    /// A stream is read as far as it can be, but Strict rejects what no conforming one has: what ffmpeg rejects too, and
    /// a program stream's video with other streams' bytes in it has (FATE's t.mpg).
    /// </summary>
    [TestMethod]
    public void StrictRejectsAForbiddenPictureCodingTypeOrQuantiserScale()
    {
        Assert.AreEqual(1u, ((PictureHeader)Read(IPicture, strict: true)).PictureCodingType);
        Assert.AreEqual(0u, ((PictureHeader)Read(ForbiddenPicture, strict: false)).PictureCodingType);
        Assert.ThrowsExactly<InvalidDataException>(() => Read(ForbiddenPicture, strict: true));

        Assert.AreEqual(1u, ((SharpH262.Slice)Read(Slice, strict: true)).QuantiserScaleCode);
        Assert.AreEqual(0u, ((SharpH262.Slice)Read(ForbiddenSlice, strict: false)).QuantiserScaleCode);
        Assert.ThrowsExactly<InvalidDataException>(() => Read(ForbiddenSlice, strict: true));
    }

    /// <summary>A unit rejected is not kept: those after it are read against what was before it.</summary>
    [TestMethod]
    public void KeepsNoUnitStrictRejects()
    {
        var context = new H262Context { Strict = true };
        context.ReadUnit(H262Context.StreamOf(IPicture, 0, IPicture.Length));
        var kept = context.PictureHeader;

        Assert.ThrowsExactly<InvalidDataException>(() => context.ReadUnit(H262Context.StreamOf(ForbiddenPicture, 0, ForbiddenPicture.Length)));
        Assert.AreSame(kept, context.PictureHeader);
    }
}
