using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpMp4
{
    /// <summary>
    /// Opus Track. 
    /// </summary>
    /// <remarks>https://opus-codec.org/docs/opus_in_isobmff.html</remarks>
    public class OpusTrack : TrackBase
    {
        public override string HdlrName => HdlrNames.Sound;

        public override string HdlrType => HdlrTypes.Sound;

        /// <summary>
        /// Ctor.
        /// </summary>
        public OpusTrack()
        { }

        public override Mp4Box CreateSampleEntryBox(Mp4Box parent)
        {
            throw new NotImplementedException();
        }

        public override void FillTkhdBox(TkhdBox tkhd)
        {
            throw new NotImplementedException();
        }
    }

    public class OpusSpecificBox : Mp4Box
    {
        public const string TYPE = "dOps";

        public OpusSpecificBox(uint size, string type, Mp4Box parent) : base(size, type, parent)
        { }

        public static async Task<Mp4Box> ParseAsync(uint size, string type, Mp4Box parent, Stream stream)
        {
            throw new NotImplementedException();
        }

        public static Task<uint> BuildAsync(Mp4Box box, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
