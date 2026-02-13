using System;
using System.Text;

namespace xpTURN.Common
{
    /// <summary>
    /// Crc32Helper provides functionality to compute CRC32 checksums for strings and byte sequences.
    /// It uses a precomputed table for efficient CRC calculation.
    /// poly = 0xEDB88320 (IEEE 802.3 standard method)
    /// </summary>
    public class Crc32Helper
    {
        private static readonly uint[] Table = new uint[256];

        static Crc32Helper()
        {
            const uint poly = 0xEDB88320u;
            for (uint i = 0; i < Table.Length; ++i)
            {
                uint crc = i;
                for (int j = 0; j < 8; ++j)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ poly;
                    else
                        crc >>= 1;
                }
                Table[i] = crc;
            }
        }

        /// <summary>
        /// Computes the CRC32 checksum of a string (UTF-8 encoded).
        /// </summary>
        /// <param name="value">The string to hash. Null is treated as empty.</param>
        /// <param name="ignoreCase">If true, the string is lowercased before hashing.</param>
        /// <returns>The CRC32 value as an unsigned 32-bit integer.</returns>
        public static uint ComputeUInt32(string value, bool ignoreCase = false)
        {
            var text = ignoreCase ? value?.ToLowerInvariant() ?? string.Empty : value ?? string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return ComputeCrc32(bytes.AsSpan());
        }

        /// <summary>
        /// Computes the CRC32 checksum of a string and returns it as a signed 32-bit integer (e.g. for use as GetHashCode).
        /// </summary>
        /// <param name="value">The string to hash. Null is treated as empty.</param>
        /// <param name="ignoreCase">If true, the string is lowercased before hashing.</param>
        /// <returns>The CRC32 value as a signed 32-bit integer (unchecked cast from uint).</returns>
        public static int ComputeInt32(string value, bool ignoreCase = false)
        {
            return unchecked((int)ComputeUInt32(value, ignoreCase));
        }

        /// <summary>
        /// Computes the CRC32 checksum of a byte array. No allocation when the caller already has a byte[] or span.
        /// </summary>
        /// <param name="bytes">The byte sequence to hash. Null is treated as empty.</param>
        /// <returns>The CRC32 value as an unsigned 32-bit integer.</returns>
        public static uint ComputeCrc32(byte[] bytes)
        {
            return ComputeCrc32((bytes ?? Array.Empty<byte>()).AsSpan());
        }

        /// <summary>
        /// Computes the CRC32 checksum of a byte span. Allocation-free for callers that have a span (e.g. from stackalloc or rented buffer).
        /// </summary>
        /// <param name="bytes">The byte sequence to hash.</param>
        /// <returns>The CRC32 value as an unsigned 32-bit integer.</returns>
        public static uint ComputeCrc32(ReadOnlySpan<byte> bytes)
        {
            uint crc = 0xFFFFFFFFu;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                crc = (crc >> 8) ^ Table[(crc ^ b) & 0xFF];
            }
            return ~crc;
        }
    }
}
