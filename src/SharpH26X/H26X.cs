namespace SharpH26X
{
    public interface IItuContext { }

    public interface IItuSerializable
    {
        ulong Read(IItuContext context, ItuStream stream);
        ulong Write(IItuContext context, ItuStream stream);
        int HasMoreRbspData { get; set; }
        int[] ReadNextBits { get; set; }

        /// <summary>The syntax its class is defined of, as the specification gives it; null of a class of no definition.</summary>
        string Syntax { get; }
    }
}
