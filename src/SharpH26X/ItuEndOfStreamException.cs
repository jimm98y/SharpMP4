using System;

namespace SharpH26X
{
    /// <summary>
    /// Raised when a bitstream cannot be parsed: it ended earlier than the syntax requires, or it
    /// declares a structure that could not fit into what is left of it.
    /// </summary>
    /// <remarks>
    /// Parsing input from elsewhere is expected to fail sometimes, so failures are reported with a
    /// type the caller can catch. Letting NullReferenceException or OverflowException escape leaves
    /// callers unable to tell a malformed file from a defect in the library.
    /// </remarks>
    public class ItuEndOfStreamException : Exception
    {
        public ItuEndOfStreamException(string message) : base(message)
        {
        }

        public ItuEndOfStreamException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
