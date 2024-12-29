
using SharpMP4;

using (Stream fs = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = Mp4.Create(fs);
    var header = await mp4.Stream.ReadBoxHeaderAsync();
    var box = await mp4.Stream.ReadBoxAsync(header);

    var header2 = await mp4.Stream.ReadBoxHeaderAsync();
    var box2 = await mp4.Stream.ReadBoxAsync(header2);

    var header3 = await mp4.Stream.ReadBoxHeaderAsync();
    var box3 = await mp4.Stream.ReadBoxAsync(header3);
}