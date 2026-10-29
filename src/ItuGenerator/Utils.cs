using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ItuGenerator
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

    /// <summary>
    /// Replacements of the text of a definitions file however it is indented: each line of the text matched after any
    /// indentation, and the replacement indented from the level of the line it replaces, by its own braces and bodies.
    /// </summary>
    public static class IndentedText
    {
        /// <summary>A replacement the text must have the place of: where it has not, the text is not the one this was made for.</summary>
        public static string Fix(string definitions, string text, string replacement, string file)
        {
            var pattern = Pattern(text);
            if (!pattern.IsMatch(definitions))
                throw new InvalidOperationException($"{file} has no '{text}'");
            return Replace(definitions, pattern, replacement);
        }

        public static string Replace(string definitions, string text, string replacement) => Replace(definitions, Pattern(text), replacement);

        private static Regex Pattern(string text)
        {
            var lines = text.Split('\n');
            var pattern = new StringBuilder(@"(^[ \t]*)?" + Regex.Escape(lines[0]));
            for (int i = 1; i < lines.Length; i++)
            {
                // of a text to the end of its last line: not the indentation of the line after
                pattern.Append(i == lines.Length - 1 && lines[i].Length == 0 ? @"\n" : @"\n[ \t]*");
                pattern.Append(Regex.Escape(lines[i]));
            }
            return new Regex(pattern.ToString(), RegexOptions.Multiline);
        }

        private static string Replace(string definitions, Regex pattern, string replacement)
        {
            return pattern.Replace(definitions, m =>
            {
                int level = m.Groups[1].Value.Replace("\t", "    ").Length / 4;
                var lines = replacement.Split('\n');
                int body = 0;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (line.Length == 0)
                        continue;
                    if (line.StartsWith("}"))
                        level--;
                    lines[i] = new string(' ', 4 * Math.Max(0, level + body)) + line;
                    body = 0;
                    if (line.EndsWith("{"))
                        level++;
                    else if (Regex.IsMatch(line, @"^((if|while|for)\s*\(.*\)|else)$"))
                        body = 1;
                }
                string replaced = string.Join("\n", lines);
                // of a text from the middle of a line: of the line's indentation, as it was
                return m.Groups[1].Success ? replaced : replaced.TrimStart(' ');
            });
        }
    }
}
