using DiGi.Geometry.Planar.Classes;
using System;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Generates a WMS <c>GetMap</c> URL for one tile of a time-enabled orthophoto service.
        /// <para>Pinned to <c>VERSION=1.1.0</c> with an easting-first <c>BBOX</c>, the form <see cref="Url_OrtoData(BoundingBox2D, int, int, int)"/> already uses. WMS 1.3.0 reads EPSG:2180 northing first; do not switch versions without re-probing.</para>
        /// <para>The URL is built whatever the scale. Whether the tile is fine enough for the service to answer with imagery is for <see cref="OrtoImageScaleDenominator(BoundingBox2D, int)"/> to check.</para>
        /// </summary>
        /// <param name="boundingBox2D">The tile in EPSG:2180, easting in X.</param>
        /// <param name="serviceUrl">The WMS endpoint (<see cref="Constants.WebService.StandardResolutionTime"/> or <see cref="Constants.WebService.HighResolutionTime"/>).</param>
        /// <param name="layer">The layer, which differs per service (<see cref="Constants.WebService.StandardResolutionTimeLayer"/>, <see cref="Constants.WebService.HighResolutionTimeLayer"/>).</param>
        /// <param name="time">The <c>TIME</c> value: a year, a month (<c>yyyy-MM</c>) or a day (<c>yyyy-MM-dd</c>).</param>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <returns>The URL, or null when an argument is missing or a size is not positive.</returns>
        public static string? Url_OrtoImage(this BoundingBox2D? boundingBox2D, string? serviceUrl, string? layer, string? time, int width, int height)
        {
            if (boundingBox2D == null || string.IsNullOrWhiteSpace(serviceUrl) || string.IsNullOrWhiteSpace(layer) || string.IsNullOrWhiteSpace(time) || width <= 0 || height <= 0)
            {
                return null;
            }

            Point2D min = boundingBox2D.Min;
            Point2D max = boundingBox2D.Max;

            return string.Format(CultureInfo.InvariantCulture, "{0}?SERVICE=WMS&VERSION=1.1.0&REQUEST=GetMap&LAYERS={1}&STYLES=&FORMAT=image%2Fjpeg&EXCEPTIONS=application%2Fvnd.ogc.se_xml&TIME={2}&SRS=EPSG:2180&WIDTH={3}&HEIGHT={4}&BBOX={5},{6},{7},{8}", serviceUrl, layer, Uri.EscapeDataString(time), width, height, min.X.ToString("R", CultureInfo.InvariantCulture), min.Y.ToString("R", CultureInfo.InvariantCulture), max.X.ToString("R", CultureInfo.InvariantCulture), max.Y.ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
