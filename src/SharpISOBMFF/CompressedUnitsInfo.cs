using System;

namespace SharpISOBMFF
{
    /// <summary>
    /// What 'icef' (ISO/IEC 23001-17 generic compressed units) says of the units of an item, each compressed alone: where
    /// each is and its size, of fields whose sizes the two codes it starts with give - which the syntax the generator reads
    /// has no way of saying. Of the offset code 0, no offsets: each unit follows the one before it.
    /// </summary>
    public class CompressedUnitsInfo : IMp4Serializable
    {
        /// <summary>Of a class of no definition, none.</summary>
        public virtual string Syntax { get { return null; } }

        public virtual string DisplayName { get { return nameof(CompressedUnitsInfo); } }
        protected IMp4Serializable parent = null;
        public IMp4Serializable GetParent() { return parent; }
        public void SetParent(IMp4Serializable parent) { this.parent = parent; }
        public StreamMarker Padding { get; set; }

        /// <summary>The size of the offsets: 0 none, then 16, 24, 32 or 64 bits.</summary>
        public byte UnitOffsetCode { get; set; }

        /// <summary>The size of the sizes: 8, 16, 24, 32 or 64 bits.</summary>
        public byte UnitSizeCode { get; set; }

        /// <summary>The two bits after the codes, which are reserved, kept as they were.</summary>
        public byte Reserved { get; set; }

        /// <summary>Where each unit is; of the offset code 0, each where the one before it ends.</summary>
        public ulong[] UnitOffset { get; set; } = Array.Empty<ulong>();

        public ulong[] UnitSize { get; set; } = Array.Empty<ulong>();

        private static uint OffsetBits(byte code) => code switch
        {
            0 => 0,
            1 => 16,
            2 => 24,
            3 => 32,
            4 => 64,
            _ => throw new NotSupportedException($"The 'icef' unit offset code {code} is reserved."),
        };

        private static uint SizeBits(byte code) => code switch
        {
            0 => 8,
            1 => 16,
            2 => 24,
            3 => 32,
            4 => 64,
            _ => throw new NotSupportedException($"The 'icef' unit size code {code} is reserved."),
        };

        public ulong Read(IsoStream stream, ulong readSize)
        {
            ulong size = 0;
            size += stream.ReadBits(size, readSize, 3, out byte offsetCode, "unit_offset_code");
            size += stream.ReadBits(size, readSize, 3, out byte sizeCode, "unit_size_code");
            size += stream.ReadBits(size, readSize, 2, out byte reserved, "reserved");
            UnitOffsetCode = offsetCode;
            UnitSizeCode = sizeCode;
            Reserved = reserved;

            size += stream.ReadUInt32(size, readSize, out uint count, "num_compressed_units");
            uint offsetBits = OffsetBits(offsetCode), sizeBits = SizeBits(sizeCode);

            // each unit at least of its size field: a count more than the box holds is no allocation of it
            if ((ulong)count * sizeBits > readSize - size)
                throw new InvalidOperationException($"'icef' says {count} units, more than it has room for.");

            UnitOffset = new ulong[count];
            UnitSize = new ulong[count];
            ulong implied = 0;
            for (int i = 0; i < count; i++)
            {
                if (offsetBits == 0)
                {
                    UnitOffset[i] = implied;
                }
                else
                {
                    size += stream.ReadBits(size, readSize, offsetBits, out ulong offset, "unit_offset");
                    UnitOffset[i] = offset;
                }

                size += stream.ReadBits(size, readSize, sizeBits, out ulong unitSize, "unit_size");
                UnitSize[i] = unitSize;
                implied = UnitOffset[i] + unitSize;
            }
            return size;
        }

        public ulong Write(IsoStream stream)
        {
            ulong size = 0;
            size += stream.WriteBits(3, UnitOffsetCode, "unit_offset_code");
            size += stream.WriteBits(3, UnitSizeCode, "unit_size_code");
            size += stream.WriteBits(2, Reserved, "reserved");
            size += stream.WriteUInt32((uint)UnitSize.Length, "num_compressed_units");

            uint offsetBits = OffsetBits(UnitOffsetCode), sizeBits = SizeBits(UnitSizeCode);
            for (int i = 0; i < UnitSize.Length; i++)
            {
                if (offsetBits != 0)
                    size += stream.WriteBits(offsetBits, UnitOffset[i], "unit_offset");
                size += stream.WriteBits(sizeBits, UnitSize[i], "unit_size");
            }
            return size;
        }

        public ulong CalculateSize()
        {
            return 8 + 32 + (ulong)UnitSize.Length * (OffsetBits(UnitOffsetCode) + SizeBits(UnitSizeCode));
        }

        public override string ToString() { return BoxStringBuilder.Format(this); }

        public ulong Trace(BoxTracer tracer)
        {
            ulong size = 0;
            size += tracer.Field("unit_offset_code", 3, UnitOffsetCode);
            size += tracer.Field("unit_size_code", 3, UnitSizeCode);
            size += tracer.Field("reserved", 2, Reserved);
            size += tracer.Field("num_compressed_units", 32, (uint)UnitSize.Length);

            uint offsetBits = OffsetBits(UnitOffsetCode), sizeBits = SizeBits(UnitSizeCode);
            for (int i = 0; i < UnitSize.Length; i++)
            {
                if (offsetBits != 0)
                    size += tracer.Field($"unit_offset[{i}]", offsetBits, UnitOffset[i]);
                size += tracer.Field($"unit_size[{i}]", sizeBits, UnitSize[i]);
            }
            return size;
        }
    }
}
