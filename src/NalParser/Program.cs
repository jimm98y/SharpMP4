
string path = "C:\\Temp\\HapRecording\\aaa.h264";
string fileName = Path.GetFileName(path);
var stream = File.Open(path, FileMode.Open);
string outDir = Path.GetDirectoryName(path) + "\\" + fileName.Substring(0, fileName.Length - 4);
Directory.CreateDirectory(outDir);
int nalIndex = -1;

int zeroByteCounter = 0;
MemoryStream ms = null;
while (true)
{ 
    int b = stream.ReadByte();
    if(b == -1)
    {
        break;
    }

    if (ms != null)
    {
        ms.WriteByte((byte)b);
    }

    if (b == 0)
        zeroByteCounter++;

    if(b == 1 && zeroByteCounter == 3)
    {
        // we have 0 0 0 1
        nalIndex++;

        var oldMs = ms;
        ms = new MemoryStream();

        if(oldMs != null)
        {
            var array = oldMs.ToArray();
            File.WriteAllBytes(outDir + "\\" + $"nal{nalIndex}.bin", array.Take(array.Length - 4).ToArray());
        }
    }

    if (b != 0)
        zeroByteCounter = 0;
}