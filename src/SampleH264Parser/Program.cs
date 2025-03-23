
// try parse SPS
using SharpH264;
using System;
using System.IO;
using System.Linq;

byte[] spsBinary = Convert.FromHexString("674D40208D8D402802DFF80B7010101400000FA00001D4C3A18003D3F00004C4B4BBCB8D0C001E9F80002625A5DE5C28");
using (ItuStream stream = new ItuStream(new MemoryStream(spsBinary)))
{
    NalUnit nu = new NalUnit((uint)spsBinary.Length);
    nu.Read(stream);

    SeqParameterSetRbsp sps = new SeqParameterSetRbsp();
    sps.Read(stream);

    var ms = new MemoryStream();
    using (ItuStream wstream = new ItuStream(ms))
    {
        nu.Write(wstream);
        sps.Write(wstream);

        byte[] wbytes = ms.ToArray();
        if(!wbytes.SequenceEqual(spsBinary))
        {
            throw new Exception("Failed to write SPS");
        }
    }
}
