using System;
using System.Security.Cryptography;

namespace SharpMP4.Encryption
{
    /// <summary>
    /// The ciphers of ISO/IEC 23001-7, clause 9, over a sample in place: AES-CTR for 'cenc' and 'cens', AES-CBC for 'cbc1'
    /// and 'cbcs', the second of each by a pattern of blocks - with a key of 128 bits, or of 256 as the draft of its
    /// Amendment 1 (ISO/IEC 23001-7:2023 DAM 1, MPEG w25903) allows. Both ways, so a sample written protected is one read
    /// back unprotected. Only AES-ECB is taken from the platform, every mode is built on it here.
    /// </summary>
    public static class CommonEncryption
    {
        private const int BlockSize = 16;

        /// <summary>Protects a sample in place.</summary>
        public static void Encrypt(string scheme, byte[] key, SampleEncryption encryption, byte[] buffer, int offset, int length) =>
            Transform(scheme, key, encryption, buffer, offset, length, encrypt: true);

        /// <summary>Recovers a protected sample in place.</summary>
        public static void Decrypt(string scheme, byte[] key, SampleEncryption encryption, byte[] buffer, int offset, int length) =>
            Transform(scheme, key, encryption, buffer, offset, length, encrypt: false);

        private static void Transform(string scheme, byte[] key, SampleEncryption encryption, byte[] buffer, int offset, int length, bool encrypt)
        {
            if (encryption == null || !encryption.IsProtected)
                return;
            if (key == null || (key.Length != 16 && key.Length != 32))
                throw new ArgumentException("The key is 16 bytes, or 32 for AES-256.", nameof(key));
            if (encryption.IV == null || (encryption.IV.Length != 8 && encryption.IV.Length != 16))
                throw new ArgumentException("The IV is 8 or 16 bytes.", nameof(encryption));

            bool counterMode = ProtectionSchemes.IsCounterMode(scheme);
            if (!counterMode && scheme != ProtectionSchemes.Cbc1 && scheme != ProtectionSchemes.Cbcs)
                throw new NotSupportedException($"Unsupported protection scheme: {scheme}");

            using var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            // CTR only ever encrypts the counter; CBC decrypts its blocks when recovering them
            using ICryptoTransform cipher = counterMode || encrypt ? aes.CreateEncryptor(key, null) : aes.CreateDecryptor(key, null);

            // a pattern of 0:0, as 'cbcs' audio has, protects every whole block
            bool pattern = ProtectionSchemes.IsPattern(scheme) && encryption.CryptByteBlock + encryption.SkipByteBlock > 0;
            int crypt = encryption.CryptByteBlock, cycle = encryption.CryptByteBlock + encryption.SkipByteBlock;

            var state = new CipherState(encryption.IV);
            int position = offset, end = offset + length;

            foreach (var (clear, protectedBytes) in Ranges(encryption.Subsamples, length))
            {
                position += clear;
                if (position + protectedBytes > end)
                    throw new ArgumentException("The subsamples run past the end of the sample.", nameof(encryption));

                if (scheme == ProtectionSchemes.Cbcs)
                    state.ResetChain(encryption.IV); // each range starts from the IV (9.6.1)

                if (!pattern && (scheme == ProtectionSchemes.Cenc || scheme == ProtectionSchemes.Piff))
                {
                    // every byte, the key stream running on from one range to the next
                    Counter(cipher, state, buffer, position, protectedBytes);
                }
                else
                {
                    // whole blocks only: a partial block at the end of a range is left clear
                    int blocks = protectedBytes / BlockSize;
                    for (int block = 0; block < blocks; block++)
                    {
                        if (pattern && block % cycle >= crypt)
                            continue;

                        int at = position + block * BlockSize;
                        if (counterMode)
                            Counter(cipher, state, buffer, at, BlockSize);
                        else if (encrypt)
                            CbcEncryptBlock(cipher, state, buffer, at);
                        else
                            CbcDecryptBlock(cipher, state, buffer, at);
                    }
                }

                position += protectedBytes;
            }
        }

        /// <summary>The sample's (clear, protected) runs: its subsamples, or all of it protected.</summary>
        private static (int Clear, int Protected)[] Ranges(EncryptionSubsample[] subsamples, int length)
        {
            if (subsamples == null || subsamples.Length == 0)
                return new[] { (0, length) };

            var ranges = new (int, int)[subsamples.Length];
            for (int i = 0; i < subsamples.Length; i++)
                ranges[i] = (subsamples[i].ClearBytes, checked((int)subsamples[i].ProtectedBytes));
            return ranges;
        }

        private sealed class CipherState
        {
            public readonly byte[] Counter = new byte[BlockSize];
            public readonly byte[] KeyStream = new byte[BlockSize];
            public int KeyStreamUsed = BlockSize;
            public readonly byte[] Chain = new byte[BlockSize];
            public readonly byte[] Block = new byte[BlockSize];

            public CipherState(byte[] iv)
            {
                // an 8 byte IV is the counter's upper half, its block count the lower (9.3)
                Buffer.BlockCopy(iv, 0, Counter, 0, iv.Length);
                ResetChain(iv);
            }

            public void ResetChain(byte[] iv)
            {
                Array.Clear(Chain, 0, BlockSize);
                Buffer.BlockCopy(iv, 0, Chain, 0, iv.Length);
            }

            /// <summary>The next counter: its lower 64 bits counted up, wrapping without carrying into the upper (9.3).</summary>
            public void Increment()
            {
                for (int i = BlockSize - 1; i >= 8; i--)
                {
                    if (++Counter[i] != 0)
                        break;
                }
            }
        }

        private static void Counter(ICryptoTransform cipher, CipherState state, byte[] buffer, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (state.KeyStreamUsed == BlockSize)
                {
                    cipher.TransformBlock(state.Counter, 0, BlockSize, state.KeyStream, 0);
                    state.Increment();
                    state.KeyStreamUsed = 0;
                }

                buffer[offset + i] ^= state.KeyStream[state.KeyStreamUsed++];
            }
        }

        private static void CbcEncryptBlock(ICryptoTransform cipher, CipherState state, byte[] buffer, int offset)
        {
            for (int i = 0; i < BlockSize; i++)
                state.Block[i] = (byte)(buffer[offset + i] ^ state.Chain[i]);
            cipher.TransformBlock(state.Block, 0, BlockSize, buffer, offset);
            Buffer.BlockCopy(buffer, offset, state.Chain, 0, BlockSize);
        }

        private static void CbcDecryptBlock(ICryptoTransform cipher, CipherState state, byte[] buffer, int offset)
        {
            cipher.TransformBlock(buffer, offset, BlockSize, state.Block, 0);
            for (int i = 0; i < BlockSize; i++)
            {
                byte ciphertext = buffer[offset + i];
                buffer[offset + i] = (byte)(state.Block[i] ^ state.Chain[i]);
                state.Chain[i] = ciphertext;
            }
        }
    }
}
