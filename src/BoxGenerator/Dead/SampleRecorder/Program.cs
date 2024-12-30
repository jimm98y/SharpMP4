
using SharpMP4;

using (Stream fs = new BufferedStream(new FileStream("C:\\Users\\lukas\\Downloads\\big_buck_bunny_1080p_h264.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = Mp4.Create(fs);

    while (true)
    {
        var header = await mp4.Stream.ReadBoxHeaderAsync();
        var box = await mp4.Stream.ReadBoxAsync(header);
    }
}