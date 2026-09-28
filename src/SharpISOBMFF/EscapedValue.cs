namespace SharpISOBMFF
{
    /// <summary>
    /// escapedValue(nBits1, nBits2, nBits3) of ISO/IEC 23003-3, as FFmpeg reads it (libavcodec/aac/aacdec_usac.c,
    /// get_escaped_value): nBits1 bits; if all of them are ones, nBits2 more added to them; if those are all ones too,
    /// nBits3 more. Its parts are kept as read, so it writes back as it was; it is used as the number they add up to.
    /// </summary>
    public class EscapedValue : IMp4Serializable
    {
        public virtual string DisplayName { get { return nameof(EscapedValue); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }
        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

        public int NBits1 { get; }
        public int NBits2 { get; }
        public int NBits3 { get; }

        public uint Value1 { get; set; }
        public uint? Value2 { get; set; }
        public uint? Value3 { get; set; }

        /// <summary>The number its parts add up to.</summary>
        public uint Value { get { return Value1 + (Value2 ?? 0) + (Value3 ?? 0); } }

        public EscapedValue(int nBits1 = 0, int nBits2 = 0, int nBits3 = 0)
        {
            NBits1 = nBits1;
            NBits2 = nBits2;
            NBits3 = nBits3;
        }

        public static implicit operator uint(EscapedValue value) => value?.Value ?? 0;

        public virtual ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = stream.ReadBits(0, readSize, (uint)NBits1, out uint value1, "value1");
            Value1 = value1;
            if (value1 == (1u << NBits1) - 1)
            {
                boxSize += stream.ReadBits(boxSize, readSize, (uint)NBits2, out uint value2, "value2");
                Value2 = value2;
                if (NBits3 > 0 && value2 == (1u << NBits2) - 1)
                {
                    boxSize += stream.ReadBits(boxSize, readSize, (uint)NBits3, out uint value3, "value3");
                    Value3 = value3;
                }
            }
            return boxSize;
        }

        public virtual ulong Write(IsoStream stream)
        {
            ulong boxSize = stream.WriteBits((uint)NBits1, Value1, "value1");
            if (Value2 != null)
                boxSize += stream.WriteBits((uint)NBits2, Value2.Value, "value2");
            if (Value3 != null)
                boxSize += stream.WriteBits((uint)NBits3, Value3.Value, "value3");
            return boxSize;
        }

        public virtual ulong CalculateSize()
        {
            return (ulong)NBits1 + (Value2 != null ? (ulong)NBits2 : 0) + (Value3 != null ? (ulong)NBits3 : 0);
        }

        public override string ToString() => Value.ToString();
    }
}
