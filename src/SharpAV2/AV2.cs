namespace SharpAV2
{
    public interface IAomContext : IAomSerializable
    {
        int SelectedOperatingPoint { get; set; }
        int ObuSizeLen { get; }
        byte[] LastObuFrameHeader { get; set; }
    }

    public interface IAomSerializable
    {
        void Read(AomStream stream, int size);
    }

    public partial class AV2Context
    {

    }
}
