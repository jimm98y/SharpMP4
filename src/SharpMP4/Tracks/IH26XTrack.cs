namespace SharpMP4.Tracks
{
    public interface IH26XTrack
    {
        uint TimescaleOverride { get; set; }
        int FrameTickOverride { get; set; }
        uint TimescaleFallback { get; set; }
        int FrameTickFallback { get; set; }

        int NalLengthSize { get; set; }

        byte[][] GetVideoNALUs();
    }
}
