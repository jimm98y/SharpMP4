using System;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// The schemes of ISO/IEC 23001-7 (Common Encryption), by their 'schm' scheme type, and PIFF's, which is AES-CTR as 'cenc' is.
    /// </summary>
    public static class ProtectionSchemes
    {
        /// <summary>AES-CTR over every protected byte.</summary>
        public const string Cenc = "cenc";

        /// <summary>AES-CBC over the whole blocks of every protected range, one chain a sample.</summary>
        public const string Cbc1 = "cbc1";

        /// <summary>AES-CTR over a pattern of blocks of each protected range.</summary>
        public const string Cens = "cens";

        /// <summary>AES-CBC over a pattern of blocks of each protected range, each range starting from the IV.</summary>
        public const string Cbcs = "cbcs";

        /// <summary>Microsoft's PIFF: AES-CTR, as 'cenc'.</summary>
        public const string Piff = "piff";

        public static bool IsCounterMode(string scheme) => scheme == Cenc || scheme == Cens || scheme == Piff;

        public static bool IsPattern(string scheme) => scheme == Cens || scheme == Cbcs;
    }

    /// <summary>A run of a sample: so many bytes left in the clear, then so many protected.</summary>
    public readonly struct EncryptionSubsample
    {
        public EncryptionSubsample(int clearBytes, uint protectedBytes)
        {
            ClearBytes = clearBytes;
            ProtectedBytes = protectedBytes;
        }

        public int ClearBytes { get; }
        public uint ProtectedBytes { get; }

        public override string ToString() => $"{ClearBytes} clear, {ProtectedBytes} protected";
    }

    /// <summary>
    /// How one sample is protected: its key, its IV - the sample's own, or the constant IV of its track or sample
    /// group - and which of its bytes are protected, and in what pattern.
    /// </summary>
    public class SampleEncryption
    {
        /// <summary>False for a sample left in the clear, as samples of a 'seig' group with isProtected 0 are.</summary>
        public bool IsProtected { get; set; } = true;

        /// <summary>The 16 byte ID of the key the sample is protected with.</summary>
        public byte[] KeyId { get; set; }

        /// <summary>The IV: 8 or 16 bytes.</summary>
        public byte[] IV { get; set; }

        /// <summary>The sample's runs of clear and protected bytes, or null where all of it is protected.</summary>
        public EncryptionSubsample[] Subsamples { get; set; }

        /// <summary>Of a pattern scheme: so many 16 byte blocks protected, then <see cref="SkipByteBlock"/> left clear, over and over.</summary>
        public byte CryptByteBlock { get; set; }

        public byte SkipByteBlock { get; set; }

        public SampleEncryption Clone() => (SampleEncryption)MemberwiseClone();
    }
}
