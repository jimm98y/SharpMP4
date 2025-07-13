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

    public class AV1ObuTypes
    {
        public const int OBU_RESERVED_0 = 0;
        public const int OBU_SEQUENCE_HEADER = 1;
        public const int OBU_TEMPORAL_DELIMITER = 2;
        public const int OBU_FRAME_HEADER = 3;
        public const int OBU_TILE_GROUP = 4;
        public const int OBU_METADATA = 5;
        public const int OBU_FRAME = 6;
        public const int OBU_REDUNDANT_FRAME_HEADER = 7;
        public const int OBU_TILE_LIST = 8;
        public const int OBU_RESERVED_9 = 9;
        public const int OBU_RESERVED_10 = 10;
        public const int OBU_RESERVED_11 = 11;
        public const int OBU_RESERVED_12 = 12;
        public const int OBU_RESERVED_13 = 13;
        public const int OBU_RESERVED_14 = 14;
        public const int OBU_PADDING = 15;
    }
}
