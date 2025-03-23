
// try parse SPS
using SharpH264;
using System;
using System.IO;
using System.Linq;

byte[] spsBinary = Convert.FromHexString("68EE019E2C");
using (ItuStream stream = new ItuStream(new MemoryStream(spsBinary)))
{
    NalUnit nu = new NalUnit((uint)spsBinary.Length);
    nu.Read(stream);

    PicParameterSetRbsp pps = new PicParameterSetRbsp();
    pps.Read(stream);

    var ms = new MemoryStream();
    using (ItuStream wstream = new ItuStream(ms))
    {
        nu.Write(wstream);
        pps.Write(wstream);

        byte[] wbytes = ms.ToArray();
        if(!wbytes.SequenceEqual(spsBinary))
        {
            throw new Exception("Failed to write PPS");
        }
    }
}
