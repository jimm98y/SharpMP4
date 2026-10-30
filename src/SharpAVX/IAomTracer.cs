namespace SharpAVX
{
    /// <summary>
    /// What an <see cref="AomStream"/> reads, as it does: each syntax element of where it ends, of its size and value, and
    /// each syntax structure around them - an OBU's header, a sequence header - of the syntax the specification gives it.
    /// Where is in bits of what the stream reads.
    /// </summary>
    public interface IAomTracer
    {
        /// <summary>A syntax structure begins, at the position given.</summary>
        void OnBeginClass(string name, string syntax, long position);

        /// <summary>A syntax structure that began ends, at the position given.</summary>
        void OnEndClass(string name, long position);

        /// <summary>A syntax element, ending at the position given, of the bits given, and its value.</summary>
        void OnField(string name, long end, ulong bits, object value);
    }
}
