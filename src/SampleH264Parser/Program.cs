
// try parse SPS
using SharpH264;
using System;
using System.IO;
using System.Linq;

byte[] seiBinary = Convert.FromHexString("0600078B71B0000003004080");
using (ItuStream stream = new ItuStream(new MemoryStream(seiBinary)))
{
    NalUnit nu = new NalUnit((uint)seiBinary.Length);
    nu.Read(stream);

    SeiMessage sei = new SeiMessage();
    sei.Read(stream);

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
