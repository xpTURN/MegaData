using System;
using System.Text;

namespace xpTURN.Common
{
    // <summary>
    // Crc32Helper provides functionality to compute CRC32 checksums for strings.
    // It uses a precomputed table for efficient CRC calculation.
    // poly = 0xEDB88320 (IEEE 802.3 standard method)
    // </summary>
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
    
        public static uint ComputeUInt32(string value, bool ignoreCase = false)
        {
            if (ignoreCase)
                value = value?.ToLowerInvariant();

            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            return Compute(bytes);
        }

        public static int ComputeInt32(string value, bool ignoreCase = false)
        {
            return unchecked((int)ComputeUInt32(value, ignoreCase));
        }

        public static uint Compute(byte[] bytes)
        {
            uint crc = 0xFFFFFFFFu;
            foreach (byte b in bytes ?? Array.Empty<byte>())
            {
                crc = (crc >> 8) ^ Table[(crc ^ b) & 0xFF];
            }
            return ~crc;
        }
    }
}
