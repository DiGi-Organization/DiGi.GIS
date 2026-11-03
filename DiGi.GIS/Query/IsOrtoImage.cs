namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Determines whether a WMS response body is an image (JPEG or PNG) rather than an OGC service exception document.
        /// <para>A service exception arrives with HTTP 200, so the status code says nothing. This is the first of the three classification steps (exception, blank, content hash) and reads only the leading signature bytes.</para>
        /// </summary>
        /// <param name="bytes">The response body.</param>
        /// <returns>True for a JPEG or PNG signature; otherwise, false.</returns>
        public static bool IsOrtoImage(this byte[]? bytes)
        {
            if (bytes == null || bytes.Length < 4)
            {
                return false;
            }

            if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            {
                return true;
            }

            return bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;
        }
    }
}
