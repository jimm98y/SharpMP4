using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpISOBMFF
{
    /// <summary>
    /// What a box, a class or a descriptor is made of, as an <c>IItuTracer</c> is told of a NAL unit's: each field, of its
    /// position in bits from the start of what is traced, its name, its size in bits and its value - the value itself, a
    /// number, a string, an array, a <see cref="StreamMarker"/> - and each box, class and descriptor around its fields and
    /// the boxes in it. Each class's generated <see cref="IMp4Serializable.Trace"/> gives the fields its syntax has of the
    /// values it has, of the sizes <see cref="IMp4Serializable.CalculateSize"/> counts. Of itself it keeps nothing: what
    /// overrides its <c>On</c> methods - <see cref="BoxStringBuilder"/>, a line of each - makes of it what it needs.
    /// </summary>
    public class BoxTracer
    {
        /// <summary>The position, in bits, of the next field, from the start of what is traced.</summary>
        public ulong Position { get; private set; }

        /// <summary>
        /// A box, a class or a descriptor: it begins, then its fields - of a descriptor, its tag and size first, which are
        /// written before them - and what is after them that it was read with, then it ends. Its size.
        /// </summary>
        public ulong Append(IMp4Serializable value, string name = null)
        {
            ulong start = Position;
            ulong size = 0;
            OnBegin(name, value, start);

            // a box not entered: of the size it is written of, and nothing of what is in it - a box of a box of a million
            // entries is traced as quickly as one of none
            if (value is Box skipped && !Enter(skipped))
            {
                size = IsoStream.CalculateBoxSize(skipped) + 8 * (ulong)(skipped.Padding?.Length ?? 0);
                Position = start + size;
                OnEnd(value, start, size);
                return size;
            }

            try
            {
                if (value is Descriptor descriptor)
                {
                    ulong instance = (descriptor.CalculateSize() >> 3) + (ulong)(descriptor.Padding?.Length ?? 0);
                    size += Field("tag", 8, descriptor.Tag);
                    // of 7 bits a byte, each but the last of its top bit set: not the bits of the number
                    ulong sizeOfSize = 8 * IsoStream.CalculatePackedNumberLength(instance, descriptor.SizeOfSize >> 3);
                    OnSizeOfInstance(Position, sizeOfSize, instance);
                    Position += sizeOfSize;
                    size += sizeOfSize;
                }

                size += value.Trace(this);
                if (value.Padding != null)
                    size += Field("padding", 8 * (ulong)value.Padding.Length, value.Padding);
            }
            catch (Exception ex)
            {
                // a field it does not have, a class of no size - one made, not read, or set to what is not written: what
                // could not be traced, and the rest after it
                OnError(ex.Message, Position);
            }
            Position = start + size;
            OnEnd(value, start, size);
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
        /// A field of the size given, in bits: a number, a string, an array, by <see cref="OnField"/>; a class, a box, a
        /// descriptor - or an array of them - each by <see cref="Append"/>. Of none, null, nothing. Its size.
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
                    OnField(name, start, bits, value);
                    break;
            }
            Position = start + bits;
            return bits;
        }

        /// <summary>
        /// Whether to trace a box's fields and the boxes in it, once it has begun: of one not entered, only that it begins
        /// and ends, of its size. Every box is entered, unless overridden.
        /// </summary>
        protected virtual bool Enter(Box box) => true;

        /// <summary>A box, a class or a descriptor begins, at the position given, in bits: of what it is called, if anything.</summary>
        protected virtual void OnBegin(string name, IMp4Serializable value, ulong position)
        {
        }

        /// <summary>A box, a class or a descriptor that began at the position given ends, of the size given, in bits.</summary>
        protected virtual void OnEnd(IMp4Serializable value, ulong position, ulong bits)
        {
        }

        /// <summary>
        /// A field of a value that is not a box, a class nor a descriptor - a number, a flag, a string, an array, the bytes
        /// of a <see cref="StreamMarker"/> - at the position given, of the bits given.
        /// </summary>
        protected virtual void OnField(string name, ulong position, ulong bits, object value)
        {
        }

        /// <summary>
        /// A descriptor's size, of the bits given, 7 a byte, each byte but the last of its top bit set: not of the bits of
        /// the number.
        /// </summary>
        protected virtual void OnSizeOfInstance(ulong position, ulong bits, ulong sizeOfInstance)
        {
        }

        /// <summary>What could not be traced, at the position given, and the rest after it.</summary>
        protected virtual void OnError(string message, ulong position)
        {
        }
    }
}
