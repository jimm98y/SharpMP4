using SharpISOBMFF;
using SharpISOBMFF.Extensions;
using System.IO;
using System.Linq;

SharpISOBMFF.Log.SinkInfo = (o, e) => { };
SharpISOBMFF.Log.SinkDebug = (o, e) => { };

using (Stream inputFileStream = new BufferedStream(new FileStream("bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = new Container();
    mp4.Read(new IsoStream(inputFileStream));

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