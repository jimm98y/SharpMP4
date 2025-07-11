namespace SharpAV1
{
    public interface IAomContext { }

    public interface IAomSerializable
    {
        ulong Read(IAomContext context, AomStream stream);
        ulong Write(IAomContext context, AomStream stream);
    }

    public class AV1RefFrames
    {
        public const int NONE = -1;
        public const int INTRA_FRAME = 0;
        public const int LAST_FRAME = 1;
        public const int LAST2_FRAME = 2;
        public const int LAST3_FRAME = 3;
        public const int GOLDEN_FRAME = 4;
        public const int BWDREF_FRAME = 5;
        public const int ALTREF2_FRAME = 6;
        public const int ALTREF_FRAME = 7;
    }

    public class AV1
    {
        
    }
}
