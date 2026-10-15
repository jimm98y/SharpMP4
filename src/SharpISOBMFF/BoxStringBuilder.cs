using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpISOBMFF
{
    /// <summary>
    /// What the ToString of a box, a class or a descriptor is made of, as FFmpeg's trace_headers traces a bitstream's
    /// headers: a line of each field, of its position in bits, its name, its bits and its value -
    /// <code>
    /// 64            version                                             00000000 = 0
    /// </code>
    /// - each box, class and descriptor of a line of its own before its fields, which are indented a level deeper, as are
    /// the boxes in it. Of the fields as they are now, read or set since: each class's generated ToString(BoxStringBuilder)
    /// gives the fields its syntax has of the values it has, of the sizes CalculateSize counts. Of the large runs of bytes
    /// - a <see cref="StreamMarker"/>, a 'mdat''s data - "...", which are not read for it.
    /// </summary>
    public class BoxStringBuilder
    {
        // FFmpeg's ff_cbs_trace_syntax_element: the bits end at the 62nd character after the position, unless the name is
        // too long for them
        private const int NameColumns = 60;
        private const string Indentation = "  ";

        private readonly StringBuilder text = new StringBuilder();
        private int level = 0;

        /// <summary>The position, in bits, of the next field, from the start of what is described.</summary>
        public ulong Position { get; private set; }

        /// <summary>A box, a class or a descriptor, and every one in it, a line each of their fields.</summary>
        public static string Format(IMp4Serializable value)
        {
            var builder = new BoxStringBuilder();
            builder.Append(value);
            return builder.ToString();
        }

        public override string ToString() => text.ToString();

        /// <summary>
        /// A box, a class or a descriptor: a line of what it is, then its fields a level deeper - of a descriptor, its tag
        /// and size first, which are written before them - and what is after them that it was read with. Its size.
        /// </summary>
        public ulong Append(IMp4Serializable value, string name = null)
        {
            ulong start = Position;
            ulong size = 0;
            Title(name, value);
            level++;
            try
            {
                if (value is Descriptor descriptor)
                {
                    ulong instance = (descriptor.CalculateSize() >> 3) + (ulong)(descriptor.Padding?.Length ?? 0);
                    size += Field("tag", 8, descriptor.Tag);
                    // of 7 bits a byte, each but the last of its top bit set: not the bits of the number
                    ulong sizeOfSize = 8 * IsoStream.CalculatePackedNumberLength(instance, descriptor.SizeOfSize >> 3);
                    Line("sizeOfInstance", "", instance.ToString(CultureInfo.InvariantCulture));
                    Position += sizeOfSize;
                    size += sizeOfSize;
                }

                size += value.ToString(this);
                if (value.Padding != null)
                    size += Field("padding", 8 * (ulong)value.Padding.Length, value.Padding);
            }
            catch (Exception ex)
            {
                // a field it does not have, a class of no size - one made, not read, or set to what is not written: what
                // could not be described, and the rest after it
                Line("error", "", ex.Message);
            }
            level--;
            Position = start + size;
            return size;
        }

        /// <summary>The boxes in a box, each and its fields: their size.</summary>
        public ulong Children(IHasBoxChildren box)
        {
            ulong size = 0;
            if (box.Children == null)
                return size;
            foreach (var child in box.Children)
                size += Append(child);
            return size;
        }

        /// <summary>The descriptors in a descriptor, each and its fields: their size.</summary>
        public ulong Descriptors(Descriptor descriptor)
        {
            ulong size = 0;
            if (descriptor.Children == null)
                return size;
            foreach (var child in descriptor.Children)
                size += Append(child);
            return size;
        }

        /// <summary>
        /// A field of the size given, in bits: a number, a string, an array a line; a class, a box, a descriptor - or an
        /// array of them - each of its own lines, by <see cref="Append"/>. Of none, null, nothing. Its size.
        /// </summary>
        public ulong Field(string name, ulong bits, object value)
        {
            ulong start = Position;
            switch (value)
            {
                case null:
                    break;
                // a run of no bits - SLConfigDescriptor's time stamps of a length of 0 - is not written, nor traced
                case ICollection empty when bits == 0 && empty.Count == 0:
                    break;
                case IMp4Serializable serializable:
                    Append(serializable, name);
                    break;
                case IEnumerable<IMp4Serializable> items:
                    int i = 0;
                    foreach (var item in items)
                    {
                        if (item != null)
                            Append(item, $"{name}[{i}]");
                        i++;
                    }
                    break;
                default:
                    Line(name, BitsOf(bits, value), Value(bits, value));
                    break;
            }
            Position = start + bits;
            return bits;
        }

        private void Title(string name, IMp4Serializable value)
        {
            text.Append(' ', 12);
            AppendIndentation();
            if (!string.IsNullOrEmpty(name) && name != value.DisplayName)
                text.Append(name).Append(": ");
            if (value is Box box)
                text.Append('\'').Append(IsoStream.EscapeString(IsoStream.ToFourCC(box.FourCC))).Append("' ");
            text.Append(value.DisplayName).AppendLine();
        }

        private void Line(string name, string bits, string value)
        {
            int length = Indentation.Length * level + name.Length;
            int pad = length + bits.Length > NameColumns ? bits.Length + 2 : NameColumns + 1 - length;
            text.Append(Position.ToString(CultureInfo.InvariantCulture).PadRight(10)).Append("  ");
            AppendIndentation();
            text.Append(name).Append(bits.PadLeft(pad)).Append(" = ").Append(value).AppendLine();
        }

        private void AppendIndentation()
        {
            for (int i = 0; i < level; i++)
                text.Append(Indentation);
        }

        /// <summary>The bits of a number or a flag, as written, as FFmpeg shows them: of any other value, none.</summary>
        private static string BitsOf(ulong bits, object value)
        {
            if (bits == 0 || bits > 64)
                return "";
            ulong number;
            switch (value)
            {
                case bool b: number = b ? 1UL : 0UL; break;
                case byte b: number = b; break;
                case sbyte b: number = unchecked((ulong)b); break;
                case ushort b: number = b; break;
                case short b: number = unchecked((ulong)b); break;
                case uint b: number = b; break;
                case int b: number = unchecked((ulong)b); break;
                case ulong b: number = b; break;
                case long b: number = unchecked((ulong)b); break;
                default: return "";
            }
            var chars = new char[bits];
            for (int i = 0; i < (int)bits; i++)
                chars[i] = ((number >> ((int)bits - 1 - i)) & 1) != 0 ? '1' : '0';
            return new string(chars);
        }

        /// <summary>A value as it is shown: a flag 0 or 1, a number of 32 bits that is a four-character code with it.</summary>
        private static string Value(ulong bits, object value)
        {
            switch (value)
            {
                case null:
                    return "null";
                case bool b:
                    return b ? "1" : "0";
                case StreamMarker:
                    return "...";
                case string s:
                    return Quoted(s);
                case BinaryUTF8String s:
                    return Quoted(s.ToString());
                case MultiLanguageString s:
                    return $"{s.Language}: {Quoted(s.Value.ToString())}";
                case byte[] bytes:
                    return Hex(bytes);
                case uint fourCC when bits == 32 && IsFourCC(fourCC):
                    return $"{fourCC} ('{IsoStream.ToFourCC(fourCC)}')";
                case IFormattable formattable:
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                case IEnumerable items:
                    var sb = new StringBuilder("[");
                    bool first = true;
                    foreach (var item in items)
                    {
                        if (!first)
                            sb.Append(", ");
                        sb.Append(Value(item is uint ? 32UL : 0UL, item));
                        first = false;
                    }
                    return sb.Append(']').ToString();
                default:
                    return value.ToString();
            }
        }

        private static string Quoted(string s) => "\"" + IsoStream.EscapeString(s) + "\"";

        private static string Hex(byte[] bytes)
        {
            if (bytes.Length == 0)
                return "[]";
            var sb = new StringBuilder("0x", 2 + 2 * bytes.Length);
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        // of letters, digits and spaces, and '©', which Apple's metadata keys start with: a number a four-character code
        // is not, by chance, but once in hundreds
        private static bool IsFourCC(uint value)
        {
            for (int shift = 24; shift >= 0; shift -= 8)
            {
                char c = (char)((value >> shift) & 0xFF);
                if (!(c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == ' ' || c == '©'))
                    return false;
            }
            return true;
        }
    }
}
