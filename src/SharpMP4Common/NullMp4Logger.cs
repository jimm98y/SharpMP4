namespace SharpMP4.Common
{
    /// <summary>
    /// Implementation of <see cref="IMp4Logger"/> that does not log
    /// anywhere.
    /// </summary>
    public sealed class NullMp4Logger : IMp4Logger
    {
        public static readonly NullMp4Logger Instance = new NullMp4Logger();
        
        private NullMp4Logger() { }
        public void LogError(string error) { }
        public void LogWarning(string warning) { }
        public void LogInfo(string info) { }
        public void LogDebug(string debug) { }
        public void LogTrace(string trace) { }

        public bool IsErrorEnabled
        {
            get => false;
            set { }
        }

        public bool IsWarningEnabled
        {
            get => false;
            set { }
        }

        public bool IsInfoEnabled
        {
            get => false;
            set { }
        }

        public bool IsDebugEnabled
        {
            get => false;
            set { }
        }

        public bool IsTraceEnabled
        {
            get => false;
            set { }
        }
    }
}
