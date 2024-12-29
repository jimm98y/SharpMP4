
using SharpMP4;

using (Stream fs = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = Mp4.Create(fs);
    var header = await mp4.ReadNextBoxHeaderAsync();
    var box = await mp4.ReadNextBoxAsync(header);

    var header2 = await mp4.ReadNextBoxHeaderAsync();
    var box2 = await mp4.ReadNextBoxAsync(header2);

    var header3 = await mp4.ReadNextBoxHeaderAsync();
    var box3 = await mp4.ReadNextBoxAsync(header3);
}