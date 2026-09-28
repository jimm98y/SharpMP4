namespace SharpISOBMFF
{
    public partial class AudioSpecificConfig
    {
        /// <summary>
        /// An explicit SBR or PS config (ISO/IEC 14496-3 1.6.2.1, Table 1.15) codes audioObjectType twice: first
        /// the signalled type, 5 (SBR) or 29 (PS), then the core type, which <see cref="AudioObjectType"/> is.
        /// This is the first; null where there is only one.
        /// </summary>
        protected GetAudioObjectType signalledAudioObjectType;
        public GetAudioObjectType SignalledAudioObjectType { get { return signalledAudioObjectType; } set { signalledAudioObjectType = value; } }

        /// <summary>
        /// What follows the last field the syntax reads: zero bytes a writer padded with, or the config of an
        /// object type it does not define (USAC's UsacConfig, ISO/IEC 23003-3). Written back as it was.
        /// </summary>
        protected RemainingBits remainder;
        public RemainingBits Remainder { get { return remainder; } set { remainder = value; } }
    }
}
