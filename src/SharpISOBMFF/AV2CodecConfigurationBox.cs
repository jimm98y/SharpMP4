using System.Collections.Generic;

namespace SharpISOBMFF
{
    public partial class AV2CodecConfigurationBox
    {
        /// <summary>
        /// The OBUs the box carries. The draft's syntax (AV2 Codec ISO Media File Format Binding, 'av2C')
        /// reads config_obu as bytes to the end of the box, which keeps them as they were; its text has each
        /// config_obu a leb128() num_bytes_in_obu and the OBU that many bytes long (AV2 Annex B), which is
        /// how they are split here.
        /// </summary>
        public IReadOnlyList<byte[]> ConfigObus
        {
            get
            {
                var bytes = new List<byte>();
                if (config_obu != null)
                {
                    foreach (var part in config_obu)
                    {
                        if (part != null)
                            bytes.AddRange(part);
                    }
                }

                var obus = new List<byte[]>();
                int position = 0;
                while (position < bytes.Count)
                {
                    long length = 0;
                    int shift = 0;
                    while (position < bytes.Count)
                    {
                        byte b = bytes[position++];
                        length |= (long)(b & 0x7f) << shift;
                        shift += 7;
                        if ((b & 0x80) == 0)
                            break;
                    }

                    int count = (int)System.Math.Min(length, bytes.Count - position);
                    obus.Add(bytes.GetRange(position, count).ToArray());
                    position += count;
                }
                return obus;
            }
        }
    }
}
