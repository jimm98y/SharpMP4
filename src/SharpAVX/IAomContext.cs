namespace SharpAVX
{
    /// <summary>What reads itself from an AomStream: a unit of so many bytes.</summary>
    public interface IAomSerializable
    {
        void Read(AomStream stream, int size);
    }

    /// <summary>The state a codec's syntax is read into, unit by unit: an OBU of AV1 or AV2, a frame of VP9.</summary>
    public interface IAomContext : IAomSerializable
    {
    }

    /// <summary>The context of a stream of OBUs (AV1, AV2).</summary>
    public interface IAomObuContext : IAomContext
    {
        int SelectedOperatingPoint { get; set; }
        int ObuSizeLen { get; }
        byte[] LastObuFrameHeader { get; set; }
    }
}
