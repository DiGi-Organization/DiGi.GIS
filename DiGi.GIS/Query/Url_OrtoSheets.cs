using DiGi.Geometry.Planar.Classes;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Generates a WFS <c>GetFeature</c> URL for one page of a yearly orthophoto index typename.
        /// <para><c>VERSION=1.1.0</c> is mandatory: without it the service answers <c>MissingParameterValue</c>. Paging uses <c>MAXFEATURES</c> and <c>STARTINDEX</c>, both honoured by the service.</para>
        /// <para>The <c>BBOX</c> is written easting first and without a CRS suffix, which is how the service reads it. Appending <c>,EPSG:2180</c> flips the expected order to northing first (the official axis order of EPSG:2180, the same order the response <c>posList</c> uses), and an easting-first box then silently matches nothing.</para>
        /// </summary>
        /// <param name="typeName">The yearly typename (<see cref="OrtoSheetTypeName(int)"/>).</param>
        /// <param name="startIndex">The zero-based index of the first feature of the page.</param>
        /// <param name="count">The number of features of the page.</param>
        /// <param name="boundingBox2D">An optional filter box in EPSG:2180, easting in X.</param>
        /// <returns>The URL, or null when the typename is empty, the start index is negative or the count is not positive.</returns>
        public static string? Url_OrtoSheets(string? typeName, int startIndex, int count, BoundingBox2D? boundingBox2D = null)
        {
            if (string.IsNullOrWhiteSpace(typeName) || startIndex < 0 || count <= 0)
            {
                return null;
            }

            string result = string.Format(CultureInfo.InvariantCulture, "{0}?SERVICE=WFS&VERSION=1.1.0&REQUEST=GetFeature&TYPENAME={1}&SRSNAME=EPSG:2180&MAXFEATURES={2}&STARTINDEX={3}", Constants.WebService.OrtoSheetIndex, typeName, count, startIndex);

            if (boundingBox2D != null)
            {
                result += "&BBOX=" + OrtoSheetBBox(boundingBox2D);
            }

            return result;
        }
    }
}
