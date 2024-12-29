
using SharpMP4;

using (Stream fs = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var mp4 = Mp4.Create(fs);
    var box = await mp4.ReadNextBoxHeader();
}