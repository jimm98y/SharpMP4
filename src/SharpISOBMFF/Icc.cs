using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpISOBMFF
{
    /// <summary>
    /// An ICC profile (ISO 15076-1, ICC.1:2010) of a 'colr' box of the colour type 'prof' or 'rICC'. Its header, its tag
    /// table and the elements of its tags are classes generated of their definitions (ICC-profile.json); what the
    /// definitions cannot say - that an element is where the tag table says, of the size it says, and that tags share
    /// elements - is said here. The profile is kept as the bytes it was read of and written as they were: its classes tell
    /// what the bytes are.
    /// </summary>
    public class ICC_profile : IMp4Serializable
    {
        public const int HeaderSize = 128;
        private const int TagEntrySize = 12;

        public string Syntax
        {
            get
            {
                return
@"class ICC_profile {
	ICCProfileHeader header;
	unsigned int(32) tag_count;
	ICCTagEntry tags[tag_count];
	ICCTagElement elements[];	// each at its tag's offset, of its tag's size; of tags of one offset, one
}";
            }
        }

        public string DisplayName { get { return nameof(ICC_profile); } }

        protected byte[] data;
        /// <summary>The bytes of the profile, as read; as written. Its classes are read of them when they are set.</summary>
        public byte[] Data { get { return data; } set { data = value; Describe(); } }

        protected StreamMarker padding = null;
        public StreamMarker Padding { get { return padding; } set { padding = value; } }

        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }

        /// <summary>The header; null of a profile shorter than its header and its tag count.</summary>
        public ICCProfileHeader Header { get; private set; }

        public uint TagCount { get; private set; }

        /// <summary>The entries of the tag table, as many as the profile has bytes of.</summary>
        public IReadOnlyList<ICCTagEntry> Tags { get; private set; } = Array.Empty<ICCTagEntry>();

        /// <summary>The elements of the tags, in the order they are in, each once; of a type of no class, its bytes alone.</summary>
        public IReadOnlyList<ICCTagElement> Elements { get; private set; } = Array.Empty<ICCTagElement>();

        public ulong Read(IsoStream stream, ulong readSize)
        {
            ulong boxSize = 0;
            boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize, out this.data, "data");
            Describe();
            return boxSize;
        }

        public ulong Write(IsoStream stream)
        {
            ulong boxSize = 0;
            boxSize += stream.WriteUInt8ArrayTillEnd(this.data, "data");
            return boxSize;
        }

        public ulong CalculateSize()
        {
            return (ulong)(data?.Length ?? 0) << 3;
        }

        public override string ToString() { return BoxStringBuilder.Format(this); }

        /// <summary>The classes of the bytes: the header, the tag table, and each element of the class of its type.</summary>
        private void Describe()
        {
            Header = null;
            TagCount = 0;
            Tags = Array.Empty<ICCTagEntry>();
            Elements = Array.Empty<ICCTagElement>();
            if (data == null || data.Length < HeaderSize + 4)
                return;

            var header = new ICCProfileHeader();
            header.Read(StreamOf(0, HeaderSize), HeaderSize * 8);
            Header = header;
            var count = StreamOf(HeaderSize, 4);
            count.ReadUInt32(0, 32, out uint tagCount, "tag_count");
            TagCount = tagCount;

            var tags = new List<ICCTagEntry>();
            for (long i = 0, at = HeaderSize + 4; i < tagCount && at + TagEntrySize <= data.Length; i++, at += TagEntrySize)
            {
                var tag = new ICCTagEntry();
                tag.Read(StreamOf((int)at, TagEntrySize), TagEntrySize * 8);
                tag.SetParent(this);
                tags.Add(tag);
            }
            Tags = tags;

            // in the order they are in, each once - tags of one offset share it - none in another
            int end = HeaderSize + 4 + tags.Count * TagEntrySize;
            var elements = new List<ICCTagElement>();
            foreach (var shared in tags
                .Where(t => t.Offset >= end && t.Size > 0 && (long)t.Offset + t.Size <= data.Length)
                .GroupBy(t => (t.Offset, t.Size))
                .OrderBy(g => g.Key.Offset))
            {
                if (shared.Key.Offset < end)
                    continue;
                elements.Add(new ICCTagElement(this, shared.ToArray(), (int)shared.Key.Offset, (int)shared.Key.Size));
                end = (int)(shared.Key.Offset + shared.Key.Size);
            }
            Elements = elements;
        }

        internal IsoStream StreamOf(int offset, int length)
        {
            return new IsoStream(new MemoryStream(data, offset, length, false));
        }

        public ulong Trace(BoxTracer tracer)
        {
            if (Header == null)
                return tracer.Field("data", (ulong)(data?.Length ?? 0) << 3, this.data);

            ulong size = 0;
            size += tracer.Field("header", HeaderSize * 8, Header);
            size += tracer.Field("tag_count", 32, TagCount);
            size += tracer.Field("tags", (ulong)Tags.Count * TagEntrySize * 8, Tags);

            int at = HeaderSize + 4 + Tags.Count * TagEntrySize;
            foreach (var element in Elements)
            {
                if (element.Offset > at)
                    size += tracer.Field(Unused(at, element.Offset), (ulong)(element.Offset - at) * 8, Slice(at, element.Offset - at));
                size += element.Trace(tracer);
                at = element.Offset + element.Length;
            }
            if (at < data.Length)
                size += tracer.Field(Unused(at, data.Length), (ulong)(data.Length - at) * 8, Slice(at, data.Length - at));
            return size;
        }

        // of fewer than 4 bytes of zeros, what aligns the next element to 4 bytes
        private string Unused(int from, int to)
        {
            bool zeros = true;
            for (int i = from; i < to; i++)
                zeros &= data[i] == 0;
            return zeros && to - from < 4 ? "alignment" : "unused";
        }

        internal byte[] Slice(int at, int length)
        {
            var slice = new byte[length];
            Buffer.BlockCopy(data, at, slice, 0, length);
            return slice;
        }
    }

    /// <summary>
    /// The element of one or more tags of an ICC profile, where their entries say: of a type of a class, that class read of
    /// its bytes (<see cref="Value"/>); of another, or of bytes the class cannot be read of, its bytes alone.
    /// </summary>
    public sealed class ICCTagElement
    {
        internal ICCTagElement(ICC_profile profile, IReadOnlyList<ICCTagEntry> tags, int offset, int length)
        {
            Profile = profile;
            Tags = tags;
            Offset = offset;
            Length = length;

            uint type = length >= 4 ? (uint)(profile.Data[offset] << 24 | profile.Data[offset + 1] << 16 | profile.Data[offset + 2] << 8 | profile.Data[offset + 3]) : 0;
            TypeSignature = type;
            var value = Create(IsoStream.ToFourCC(type));
            if (value == null)
                return;
            try
            {
                value.SetParent(profile);
                ulong read = value.Read(profile.StreamOf(offset, length), (ulong)length << 3);
                // of whole bytes, and as many as the class says it is: else its bytes alone
                if (read % 8 == 0 && read <= (ulong)length << 3 && value.CalculateSize() == read)
                {
                    Value = value;
                    ValueLength = (int)(read >> 3);
                }
            }
            catch (Exception)
            {
                // an element its class cannot be read of: its bytes
            }
        }

        public ICC_profile Profile { get; }

        /// <summary>The tags of the element: one, or those that share it.</summary>
        public IReadOnlyList<ICCTagEntry> Tags { get; }

        /// <summary>Where the element is, in bytes from the start of the profile.</summary>
        public int Offset { get; }
        public int Length { get; }

        public uint TypeSignature { get; }

        /// <summary>The element of its class; null of a type of none.</summary>
        public IMp4Serializable Value { get; }

        /// <summary>The bytes <see cref="Value"/> is read of: of the element, those after are of none.</summary>
        public int ValueLength { get; }

        public string Name { get { return string.Join(", ", Tags.Select(t => IsoStream.ToFourCC(t.TagSignature))); } }

        private static IMp4Serializable Create(string type)
        {
            switch (type)
            {
                case "XYZ ": return new ICCXYZType();
                case "curv": return new ICCCurveType();
                case "para": return new ICCParametricCurveType();
                case "sf32": return new ICCS15Fixed16ArrayType();
                case "text": return new ICCTextType();
                case "desc": return new ICCTextDescriptionType();
                case "mluc": return new ICCMultiLocalizedUnicodeType();
                case "sig ": return new ICCSignatureType();
                case "cicp": return new ICCCicpType();
                case "chrm": return new ICCChromaticityType();
                case "meas": return new ICCMeasurementType();
                case "view": return new ICCViewingConditionsType();
                default: return null;
            }
        }

        internal ulong Trace(BoxTracer tracer)
        {
            if (Value == null)
                return tracer.Field($"{Name} ('{IsoStream.ToFourCC(TypeSignature)}')", (ulong)Length << 3, Profile.Slice(Offset, Length));
            ulong size = tracer.Field(Name, (ulong)ValueLength << 3, Value);
            if (ValueLength < Length)
                size += tracer.Field($"{Name} unused", (ulong)(Length - ValueLength) << 3, Profile.Slice(Offset + ValueLength, Length - ValueLength));
            return size;
        }
    }
}
