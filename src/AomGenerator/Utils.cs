using System;
using System.Collections.Generic;
using System.Linq;

namespace AomGenerator
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

        public static string ToFirstLower(this string source)
        {
            if (string.IsNullOrEmpty(source) || source.Length < 2)
                return source;
            return string.Format("{0}{1}", char.ToLower(source[0]), source.Substring(1));
        }
    }

    public static class DictionaryExtensions
    {
#if NETSTANDARD
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }
#endif
    }
}
