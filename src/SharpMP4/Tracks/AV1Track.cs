using SharpISOBMFF;
using SharpAV1;
using System;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// AV1 Track.
    /// </summary>
    /// <remarks>
    /// https://aomedia.org/specifications/av1/
    /// </remarks>
    public class AV1Track : TrackBase
    {
        public const string BRAND = "av01";

        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "eng";

        public AV1Track()
        {

        }

        public AV1Track(Box sampleEntry, uint timescale, int sampleDuration)
        {
            throw new NotImplementedException();
        }

        public override Box CreateSampleEntryBox()
        {
            throw new NotImplementedException();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            throw new NotImplementedException();
        }
    }
}
