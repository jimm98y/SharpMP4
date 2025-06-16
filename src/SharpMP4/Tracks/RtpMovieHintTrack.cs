using SharpISOBMFF;

namespace SharpMP4.Tracks
{
    public class RtpMovieHintTrack : TrackBase
    {
        public override string HandlerName => HandlerNames.Hint;

        public override string HandlerType => HandlerTypes.Hint;

        public override string Language { get; set; } = "eng";

        public RtpMovieHintTrack()
        {
        }

        public override Box CreateSampleEntryBox()
        {
            var rtpSampleEntry = new RtpMovieHintInformation(); 
            rtpSampleEntry.Sdptext = new byte[4] { 0, 0, 0, 1 };
            rtpSampleEntry.Descriptionformat = 0;
            return rtpSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // nothing to do
        }
    }
}
