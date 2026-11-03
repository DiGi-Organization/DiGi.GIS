using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Computes the SHA-256 of a byte array as lowercase hexadecimal.
        /// <para>Used to recognise one served orthophoto image behind several requested times. Hash only a response already classified as an image with content: every uniform blank tile of a given size has the same hash.</para>
        /// </summary>
        /// <param name="bytes">The bytes to hash.</param>
        /// <returns>The 64-character hash, or null when the array is null.</returns>
        public static string? ContentHash(this byte[]? bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            byte[] hash;
            using (SHA256 sHA256 = SHA256.Create())
            {
                hash = sHA256.ComputeHash(bytes);
            }

            StringBuilder stringBuilder = new(hash.Length * 2);
            foreach (byte @byte in hash)
            {
                stringBuilder.Append(@byte.ToString("x2", CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }
    }
}
