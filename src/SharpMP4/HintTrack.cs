using SharpISOBMFF;

namespace SharpMP4
{
    public class HintTrack : TrackBase
    {
        public override string HandlerName => "Bento4 Hint Handler\0"; // HandlerNames.Hint;

        public override string HandlerType => HandlerTypes.Hint;

        public override string Language { get; set; } = "eng";

        public HintTrack()
        {
        }

        public override Box CreateSampleEntryBox()
        {
            var rtpSampleEntry = new HintSampleEntry(IsoStream.FromFourCC("rtp ")); // TODO: investigate why it's not using RtpHintSampleEntry
            rtpSampleEntry.ReservedSampleEntry = new byte[6]; // TODO simplify API

            rtpSampleEntry.DataReferenceIndex = 1;
            return rtpSampleEntry;
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            // nothing to do
        }
    }
}
