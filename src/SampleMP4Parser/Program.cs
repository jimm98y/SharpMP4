
using SharpMP4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;

string[] files = Directory.GetFiles("C:\\_videoFingerprints\\").ToArray();
//string[] files1 = DirSearch("\\\\HOME-DS\\video").Where(x => x.EndsWith(".mp4")).ToArray();
//string[] files1 = DirSearch("\\\\192.168.1.250\\misc").Where(x => x.EndsWith(".mp4")).ToArray();
//string[] files2 = DirSearch("\\\\192.168.1.250\\misc2").Where(x => x.EndsWith(".mp4")).ToArray();
//string[] files = files1.Concat(files2).ToArray();
//File.WriteAllLines("C:\\Temp\\testFiles3.txt", files1);
//string[] files = File.ReadAllLines("C:\\Temp\\errors.txt");
//string[] files = File.ReadAllLines("C:\\Temp\\testFiles5.txt");
//string[] files = [""];
//string[] files = ["C:\\Users\\lukas\\Downloads\\Arknights - Reimei Zensou - 01 [VVC_1080p_AAC].mp4"];
//string[] files = DirSearch("C:\\Git\\mp4parser").Where(x => x.EndsWith(".mp4")).ToArray();

HttpClient httpClient = new HttpClient();
foreach (var file in files)
{
    Debug.WriteLine($"----Reading: {file}");
    //using (Stream inputFileStream = await httpClient.GetStreamAsync(file))
    using (Stream inputFileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
    {
        var inputMp4 = new Mp4();
        inputMp4.Read(new IsoStream(new MyStream(inputFileStream)));
        
        // seek enabled
        //inputMp4.Read(new IsoStream(inputFileStream));

        using (var outputFileStream = new FileStream("test.mp4", FileMode.Create, FileAccess.ReadWrite))
        {
            //using (var fingerprintStream = new FileStream("C:\\_videoFingerprints\\" + System.IO.Path.GetFileName(file), FileMode.Create, FileAccess.ReadWrite))
            {
                inputMp4.Write(new IsoStream(outputFileStream));
                //inputMp4.Write(new IsoStream(fingerprintStream));

                outputFileStream.Flush();
                //fingerprintStream.Flush();

                outputFileStream.Seek(0, SeekOrigin.Begin);
                //inputFileStream.Seek(0, SeekOrigin.Begin);

                //using (Stream inputFileStream2 = await httpClient.GetStreamAsync(file))
                {
                    if (!AreStreamsEqual(inputFileStream, outputFileStream))
                    //if (!AreStreamsEqual(inputFileStream2, outputFileStream))
                    {
                        Debug.WriteLine($"Streams mismatch!!!!! {file}");
                        Console.WriteLine($"Streams mismatch!!!!! {file}");
                    }
                    else
                    {
                        Debug.WriteLine("------------------------ Streams are equal ------------------------");
                    }
                }
            }
        }
    }
}

bool AreStreamsEqual(Stream stream, Stream other)
{
    const int bufferSize = 2048;
    if (stream.CanSeek)
    {
        if (other.Length != stream.Length)
        {
            return false;
        }
    }

    int length = 0;
    byte[] buffer = new byte[bufferSize];
    byte[] otherBuffer = new byte[bufferSize];
    while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
    {
        other.ReadExactly(otherBuffer, 0, length);

        if (!otherBuffer.SequenceEqual(buffer))
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
                other.Seek(0, SeekOrigin.Begin);
            }
            return false;
        }
    }
    if (stream.CanSeek)
    {
        stream.Seek(0, SeekOrigin.Begin);
        other.Seek(0, SeekOrigin.Begin);
    }
    return true;
}

static string[] DirSearch(string sDir)
{
    List<string> files = new List<string>();
    try
    {
        foreach (string d in Directory.GetDirectories(sDir))
        {
            foreach (string f in Directory.GetFiles(d))
            {
                files.Add(f);
            }
            foreach(var ff in DirSearch(d))
            {
                files.Add(ff);
            }
        }

        foreach (string f in Directory.GetFiles(sDir))
        {
            files.Add(f);
        }
    }
    catch (System.Exception excpt)
    {
        Console.WriteLine(excpt.Message);
    }

    return files.ToArray();
}

class MyStream : Stream
{
    Stream inner;
    public MyStream(Stream inner)
    {
        this.inner = inner;
    }

    public override bool CanRead => inner.CanRead;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return inner.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}