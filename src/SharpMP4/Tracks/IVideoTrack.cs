namespace SharpMP4.Tracks
{
    public interface IVideoTrack
    {
        uint TimescaleOverride { get; set; }
        int FrameTickOverride { get; set; }
        uint TimescaleFallback { get; set; }
        int FrameTickFallback { get; set; }
        byte[][] GetVideoUnits();
    }
}
