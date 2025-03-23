
// try parse SPS
using SharpH264;
using System;
using System.IO;
using System.Linq;

byte[] spsBinary = Convert.FromHexString("6742C00D9A7403C0113F2E0220000003002000000651E28554");
using (ItuStream stream = new ItuStream(new MemoryStream(spsBinary)))
{
    SeqParameterSetRbsp sps = new SeqParameterSetRbsp();
    sps.Read(stream);

    var ms = new MemoryStream();
    using (ItuStream wstream = new ItuStream(ms))
    {
        sps.Write(wstream);
        wstream.Flush();

        byte[] wbytes = ms.ToArray();
        if(!wbytes.SequenceEqual(spsBinary))
        {
            throw new Exception("Failed to write SPS");
        }
    }
}
