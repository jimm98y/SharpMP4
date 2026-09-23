using System.Collections.Generic;

namespace SharpMP4
{
    /// <summary>
    /// Splits an Annex B byte stream - what an encoder hands out, and what a .264 or .265 file
    /// holds - into NAL units. The tracks here take NAL units without a start code in front of
    /// them, and refuse a sample that still carries one, so a caller with an elementary stream
    /// puts it through this first.
    /// </summary>
    public static class AnnexB
    {
        /// <summary>
        /// The NAL units in an Annex B buffer, each without its start code.
        /// </summary>
        /// <remarks>
        /// A NAL unit runs to the start of the next one's start code rather than to the start of
        /// its payload: ending it at the payload leaves the start code hanging off the end of the
        /// unit before, which puts three bytes that cannot appear in a NAL unit inside one. Where
        /// that unit is a parameter set, it goes on into the sample entry of whatever is written
        /// with it. Trailing zero bytes are dropped, which also takes care of the leading zero of
        /// a four byte start code.
        /// </remarks>
        public static IEnumerable<byte[]> ParseNalUnits(byte[] data)
        {
            if (data == null)
                yield break;

            var startCodes = new List<int>();
            var payloads = new List<int>();
            for (int i = 0; i + 3 < data.Length; i++)
            {
                if (data[i] == 0 && data[i + 1] == 0 && data[i + 2] == 1)
                {
                    startCodes.Add(i);
                    payloads.Add(i + 3);
                    i += 2;
                }
            }

            for (int i = 0; i < payloads.Count; i++)
            {
                int start = payloads[i];
                int end = i + 1 < payloads.Count ? startCodes[i + 1] : data.Length;

                while (end > start && data[end - 1] == 0)
                    end--;

                if (end <= start)
                    continue;

                var nalUnit = new byte[end - start];
                System.Buffer.BlockCopy(data, start, nalUnit, 0, nalUnit.Length);
                yield return nalUnit;
            }
        }
    }
}
