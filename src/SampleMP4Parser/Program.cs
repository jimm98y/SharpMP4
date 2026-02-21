using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

string[] files = ["bunny.mp4"];

foreach (var file in files)
{
    Debug.WriteLine($"Reading: {file}");
    using (Stream inputFileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
    {
        var inputMp4 = new Container();
        inputMp4.Read(new IsoStream(new MyStream(inputFileStream)));
        
        using (var outputFileStream = new FileStream("test.mp4", FileMode.Create, FileAccess.ReadWrite))
        {
            inputMp4.Write(new IsoStream(outputFileStream));
            outputFileStream.Flush();
            outputFileStream.Seek(0, SeekOrigin.Begin);

            if (!AreStreamsEqual(inputFileStream, outputFileStream))
            {
                Debug.WriteLine($"Streams mismatch!!! {file}");
                Console.WriteLine($"Streams mismatch!!! {file}");
            }
            else
            {
                Debug.WriteLine("- Streams are equal -");
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
