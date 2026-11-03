using DiGi.Geometry.Planar.Classes;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Computes the OGC scale denominator of a WMS tile: its ground resolution divided by the standard rendering pixel size (<see cref="Constants.WebService.StandardRenderingPixelSize"/>).
        /// <para>Above <see cref="Constants.WebService.OrtoMaxScaleDenominator"/> the orthophoto services answer a uniform white image with HTTP 200 and no exception, which reads as "no imagery here". A 4 km tile at 200 px (scale about 71 400) does exactly that.</para>
        /// </summary>
        /// <param name="boundingBox2D">The tile in EPSG:2180, easting in X.</param>
        /// <param name="width">The width of the image in pixels.</param>
        /// <returns>The scale denominator, or null when the box is missing or the width is not positive.</returns>
        public static double? OrtoImageScaleDenominator(this BoundingBox2D? boundingBox2D, int width)
        {
            if (boundingBox2D == null || width <= 0)
            {
                return null;
            }

            return boundingBox2D.Width / width / Constants.WebService.StandardRenderingPixelSize;
        }
    }
}
