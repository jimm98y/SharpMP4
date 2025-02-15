using System;
using System.Linq;

namespace BoxGenerator
{
    public static class StringExtensions
    {
        public static string ToPropertyCase(this string source)
        {
            if (source.Contains('_'))
            {
                var parts = source
                    .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                return string.Join("", parts.Select(ToCapital));
            }
            else
            {
                return ToCapital(source);
            }
        }

        public static string ToCapital(this string source)
        {
            if (string.IsNullOrEmpty(source) || source.Length < 2)
                return source;
            return string.Format("{0}{1}", char.ToUpper(source[0]), source.Substring(1));
        }
    }
}
