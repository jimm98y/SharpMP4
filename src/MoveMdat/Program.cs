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

using MoveMdat;
using SharpISOBMFF;
using SharpISOBMFF.Extensions;
using System.IO;
using System.Linq;

var logger = ConsoleWithoutInfoDebugLogger.Instance;

using (Stream inputFileStream = new BufferedStream(new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = new Container(logger);
    mp4.Read(new IsoStream(inputFileStream) { Logger = logger });

    var moov = mp4.Children.OfType<MovieBox>().Single(); // there should be a single moov
    var mdat = mp4.Children.OfType<MediaDataBox>().FirstOrDefault(); // there can be multiple mdat boxes

    int moovIndex = mp4.Children.IndexOf(moov);
    int mdatIndex = mp4.Children.IndexOf(mdat);

    long delta = 0;
    if (moovIndex > mdatIndex)
    {
        delta = (long)(moov.CalculateSize() >> 3);
    }

    if (mp4.Children.IndexOf(moov) != 1)
    {
        // move the moov to the 2nd position right after the ftyp box
        mp4.Children.Remove(moov);
        mp4.Children.Insert(1, moov);

        // if the moov was after mdat, adjust the offsets in stco or co64 boxes in all tracks
        moov.ModifyChunkOffsets(delta);
    }

    using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        mp4.Write(new IsoStream(output));
    }
}
