using DiGi.Geometry.Planar.Classes;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the culture-invariant key of a location: the minimum corner of its box in EPSG:2180 metres, easting first, as <c>easting_northing</c>.
        /// <para>Rendered with <see cref="CultureInfo.InvariantCulture"/> and the round-trip format, with negative zero folded to zero, so the key a matrix is written under on one machine is the key it is looked up under on any other.</para>
        /// </summary>
        /// <param name="boundingBox2D">The location in EPSG:2180, easting in X.</param>
        /// <returns>The key, or null when the box is missing.</returns>
        public static string? OrtoLocationKey(this BoundingBox2D? boundingBox2D)
        {
            if (boundingBox2D == null)
            {
                return null;
            }

            return Format(boundingBox2D.Min.X) + "_" + Format(boundingBox2D.Min.Y);

            static string Format(double value)
            {
                return (value == 0 ? 0 : value).ToString("R", CultureInfo.InvariantCulture);
            }
        }
    }
}
