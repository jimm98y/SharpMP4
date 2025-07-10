namespace SharpAV1
{
    public interface IAomContext { }

    public interface IAomSerializable
    {
        ulong Read(IAomContext context, AomStream stream);
        ulong Write(IAomContext context, AomStream stream);
    }

    public class AV1
    {

    }
}
