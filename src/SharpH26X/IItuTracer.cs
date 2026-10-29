namespace SharpH26X
{
    /// <summary>
    /// What an <see cref="ItuStream"/> reads or writes, as it does: each syntax element of where it ends, of its size and
    /// value, and each class around them. Where is in bits of the NAL unit's bytes as they are, its emulation prevention
    /// bytes counted - an element of an emulation prevention byte in it begins 8 bits before its end and its size.
    /// </summary>
    public interface IItuTracer
    {
        /// <summary>A class begins, at the position given.</summary>
        void OnBeginClass(string name, IItuSerializable value, ulong position);

        /// <summary>A class that began ends, at the position given.</summary>
        void OnEndClass(string name, IItuSerializable value, ulong position);

        /// <summary>A syntax element, ending at the position given, of the bits given, and its value.</summary>
        void OnField(string name, ulong end, ulong bits, object value);
    }
}
