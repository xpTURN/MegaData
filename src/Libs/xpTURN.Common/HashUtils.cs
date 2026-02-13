using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace xpTURN.Common
{
    /// <summary>
    /// SHA-256 hash computation utilities. All methods return Base64-encoded hash strings.
    /// </summary>
    public class HashUtils
    {
        /// <summary>
        /// Computes the SHA-256 hash of a string (ASCII encoding). Null is treated as empty.
        /// </summary>
        /// <param name="text">The string to hash. Null is treated as empty.</param>
        /// <returns>Base64-encoded SHA-256 hash.</returns>
        public static string ComputeSHA256Hash(string text)
        {
            var safe = text ?? string.Empty;
            byte[] bytes = Encoding.ASCII.GetBytes(safe);
            return ComputeSHA256Hash(bytes);
        }

        /// <summary>
        /// Computes the SHA-256 hash of a byte array. Null is treated as empty.
        /// </summary>
        /// <param name="bytes">The byte array to hash. Null is treated as empty.</param>
        /// <returns>Base64-encoded SHA-256 hash.</returns>
        public static string ComputeSHA256Hash(byte[] bytes)
        {
            var safe = bytes ?? Array.Empty<byte>();
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(safe);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Computes the SHA-256 hash of the data in the given stream. The stream position is restored after hashing.
        /// </summary>
        /// <param name="stream">The stream to hash. Must be readable and seekable. Not null.</param>
        /// <returns>Base64-encoded SHA-256 hash.</returns>
        public static string ComputeSHA256Hash(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var orgPosition = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);

            try
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(stream);
                    return Convert.ToBase64String(hashBytes);
                }
            }
            finally
            {
                stream.Seek(orgPosition, SeekOrigin.Begin);
            }
        }
    }
}