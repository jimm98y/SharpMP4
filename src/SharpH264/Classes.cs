namespace SharpH264
{
    public interface IItuContext { }

    public interface IItuSerializable
    {
        ulong Read(IItuContext context, ItuStream stream);
        ulong Write(IItuContext context, ItuStream stream);
        int HasMoreRbspData { get; set; }
        int[] ReadNextBits { get; set; }
    }
}
