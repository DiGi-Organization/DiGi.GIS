using DiGi.Geometry.Planar.Classes;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Formats a box as the <c>BBOX</c> value of an orthophoto index WFS request: <c>minEasting,minNorthing,maxEasting,maxNorthing</c>, culture-invariant, without a CRS suffix.
        /// <para>Verified 2026-11-03: without a suffix the service reads easting first; with <c>,EPSG:2180</c> it reads northing first, and a box in the wrong order matches nothing without any error.</para>
        /// </summary>
        /// <param name="boundingBox2D">The box in EPSG:2180, easting in X.</param>
        /// <returns>The formatted value, or null when the box is missing.</returns>
        public static string? OrtoSheetBBox(this BoundingBox2D? boundingBox2D)
        {
            if (boundingBox2D == null)
            {
                return null;
            }

            return string.Join(",", boundingBox2D.Min.X.ToString("R", CultureInfo.InvariantCulture), boundingBox2D.Min.Y.ToString("R", CultureInfo.InvariantCulture), boundingBox2D.Max.X.ToString("R", CultureInfo.InvariantCulture), boundingBox2D.Max.Y.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
