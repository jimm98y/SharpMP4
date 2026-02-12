using System;

namespace SharpH26X
{
    public static class MathEx
    {
        public static double Log2(double x)
        {
#if NET5_0_OR_GREATER
            return Math.Log2(x);
#else
            return Math.Log(x, 2);
#endif
        }

        public static int Clamp(int value, int min, int max)
        {
#if NET5_0_OR_GREATER
            return Math.Clamp(value, min, max);
#else
            int result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
#endif
        }
    }

    public static class ConvertEx
    {
        public static byte[] FromHexString(string hex)
        {
#if !NETCOREAPP
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
#else
            return Convert.FromHexString(hex);
#endif
        }

        public static string ToHexString(byte[] data)
        {
#if !NETCOREAPP
            string hexString = BitConverter.ToString(data);
            hexString = hexString.Replace("-", "");
            return hexString;
#else
            return Convert.ToHexString(data);
#endif
        }
    }
}
