
// try parse SPS
using SharpH264;
using System;
using System.IO;
using System.Linq;

byte[] spsBinary = Convert.FromHexString("2742E01EA9181405FF2E00D418041ADB0AD7BDF010");
SeqParameterSetRbsp sps;
using (ItuStream stream = new ItuStream(new MemoryStream(spsBinary)))
{
    NalUnit nu = new NalUnit((uint)spsBinary.Length);
    nu.Read(stream);
    H264Helpers.SetNalUnit(nu);

    sps = new SeqParameterSetRbsp();
    H264Helpers.SetSeqParameterSet(sps);

    sps.Read(stream);

    var ms = new MemoryStream();
    using (ItuStream wstream = new ItuStream(ms))
    {
        nu.Write(wstream);
        sps.Write(wstream);

        byte[] wbytes = ms.ToArray();
        if (!wbytes.SequenceEqual(spsBinary))
        {
            throw new Exception("Failed to write SPS");
        }
    }
}

byte[] ppsBinary = Convert.FromHexString("28DE09C8");
PicParameterSetRbsp pps;
using (ItuStream stream = new ItuStream(new MemoryStream(ppsBinary)))
{
    NalUnit nu = new NalUnit((uint)ppsBinary.Length);
    nu.Read(stream);
    H264Helpers.SetNalUnit(nu);

    pps = new PicParameterSetRbsp();
    H264Helpers.SetPicParameterSet(pps);

    pps.Read(stream);

    var ms = new MemoryStream();
    using (ItuStream wstream = new ItuStream(ms))
    {
        nu.Write(wstream);
        pps.Write(wstream);

        byte[] wbytes = ms.ToArray();
        if (!wbytes.SequenceEqual(ppsBinary))
        {
            throw new Exception("Failed to write PPS");
        }
    }
}

byte[] seiBinary = Convert.FromHexString("0600078B71B0000003004080");
byte[] seiBinary2 = Convert.FromHexString("0605110387F44ECD0A4BDCA1943AC3D49B171F0080");
SeiRbsp sei;
using (ItuStream stream = new ItuStream(new MemoryStream(seiBinary)))
{
    ulong size = 0;
    NalUnit nu = new NalUnit((uint)seiBinary.Length);
    size += nu.Read(stream);
    H264Helpers.SetNalUnit(nu);

    sei = new SeiRbsp();
    H264Helpers.SetSei(sei);

    size += sei.Read(stream);

    var ms = new MemoryStream();
    using (ItuStream wstream = new ItuStream(ms))
    {
        nu.Write(wstream);
        sei.Write(wstream);

        byte[] wbytes = ms.ToArray();
        if(!wbytes.SequenceEqual(seiBinary))
        {
            throw new Exception("Failed to write SEI");
        }
    }
}
