#if NETFRAMEWORK
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace SharpMP4.Tests;

/// <summary>
/// What the tests use of .NET that .NET Framework does not have and Meziantou.Polyfill does not add: members of the
/// framework's own types, so the tests read the same on every target.
/// </summary>
internal static class FrameworkPolyfills
{
    extension(ProcessStartInfo start)
    {
        /// <summary>
        /// The arguments, each added to <see cref="ProcessStartInfo.Arguments"/> quoted as the program's
        /// CommandLineToArgvW reads it back, as .NET does with its own ArgumentList.
        /// </summary>
        public Collection<string> ArgumentList => new ArgumentAppender(start);
    }

    extension(GC)
    {
        /// <summary>
        /// What the application domain has allocated, as the thread's own count is not there to read. The tests do not run
        /// in parallel, so what the others allocate meanwhile is the test host's little.
        /// </summary>
        public static long GetAllocatedBytesForCurrentThread()
        {
            AppDomain.MonitoringIsEnabled = true;
            return AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
        }
    }

    private sealed class ArgumentAppender(ProcessStartInfo start) : Collection<string>
    {
        protected override void InsertItem(int index, string item)
        {
            if (index != Count)
                throw new NotSupportedException("arguments are only appended");
            base.InsertItem(index, item);
            start.Arguments = string.IsNullOrEmpty(start.Arguments) ? Quote(item) : start.Arguments + " " + Quote(item);
        }

        protected override void SetItem(int index, string item) => throw new NotSupportedException("arguments are only appended");

        protected override void RemoveItem(int index) => throw new NotSupportedException("arguments are only appended");

        protected override void ClearItems() => throw new NotSupportedException("arguments are only appended");

        /// <summary>Quoted when it has a space, a tab or a quote, or is empty; backslashes before a quote doubled.</summary>
        private static string Quote(string argument)
        {
            if (argument.Length > 0 && argument.IndexOfAny([' ', '\t', '\n', '\v', '"']) < 0)
                return argument;

            var quoted = new StringBuilder("\"");
            for (int i = 0; i < argument.Length; i++)
            {
                int backslashes = 0;
                while (i < argument.Length && argument[i] == '\\')
                {
                    backslashes++;
                    i++;
                }

                if (i == argument.Length)
                {
                    quoted.Append('\\', backslashes * 2); // before the closing quote
                    break;
                }

                if (argument[i] == '"')
                    quoted.Append('\\', backslashes * 2 + 1).Append('"');
                else
                    quoted.Append('\\', backslashes).Append(argument[i]);
            }
            return quoted.Append('"').ToString();
        }
    }
}
#endif
