using DiGi.Geometry.Planar.Classes;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Generates a WFS <c>GetFeature</c> URL with <c>RESULTTYPE=hits</c>, which answers only the number of features of a yearly orthophoto index typename (attribute <c>numberOfFeatures</c>).
        /// <para>The <c>BBOX</c> uses the same form as <see cref="Url_OrtoSheets(string, int, int, BoundingBox2D)"/> (<see cref="OrtoSheetBBox(BoundingBox2D)"/>).</para>
        /// </summary>
        /// <param name="typeName">The yearly typename (<see cref="OrtoSheetTypeName(int)"/>).</param>
        /// <param name="boundingBox2D">An optional filter box in EPSG:2180, easting in X.</param>
        /// <returns>The URL, or null when the typename is empty.</returns>
        public static string? Url_OrtoSheetCount(string? typeName, BoundingBox2D? boundingBox2D = null)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }

            string result = string.Format(CultureInfo.InvariantCulture, "{0}?SERVICE=WFS&VERSION=1.1.0&REQUEST=GetFeature&TYPENAME={1}&SRSNAME=EPSG:2180&RESULTTYPE=hits", Constants.WebService.OrtoSheetIndex, typeName);

            if (boundingBox2D != null)
            {
                result += "&BBOX=" + OrtoSheetBBox(boundingBox2D);
            }

            return result;
        }
    }
}
