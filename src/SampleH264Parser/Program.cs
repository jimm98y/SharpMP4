
// try parse SPS
using SharpH264;
using System;
using System.IO;

byte[] spsBinary = Convert.FromHexString("674D40208D8D402802DFF80B7010101400000FA00001D4C3A18003D3F00004C4B4BBCB8D0C001E9F80002625A5DE5C28");
using (ItuStream stream = new ItuStream(new MemoryStream(spsBinary)))
{
    SeqParameterSetRbsp sps = new SeqParameterSetRbsp();
    sps.Read(stream);
}
