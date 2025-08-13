using System;
using System.IO;
using System.Text;

namespace xpTURN.Common
{
    public class MD5Utils
    {
        public static string ComputeMD5Hash(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            return ComputeMD5Hash(bytes);
        }

        public static string ComputeMD5Hash(byte[] bytes)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hashBytes = md5.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static string ComputeMD5Hash(Stream stream)
        {
            var orgPosition = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);

            //
            try
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var hashBytes = md5.ComputeHash(stream);
                    return Convert.ToBase64String(hashBytes);
                }
            }
            finally
            {
                // Restore the original position of the stream
                stream.Seek(orgPosition, SeekOrigin.Begin);
            }
        }
    }
}