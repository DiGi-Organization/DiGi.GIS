using DiGi.Geometry.Planar.Classes;
using System;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Generates a WMS <c>GetFeatureInfo</c> URL asking the true orthophoto service which index sheets (<see cref="Constants.WebService.TrueOrthoIndexLayer"/>) cover a point.
        /// <para>The point is the centre of a 101 x 101 pixel image at 1 m per pixel, queried at its centre pixel. Pinned to <c>VERSION=1.1.0</c> with an easting-first <c>BBOX</c>, as <see cref="Url_OrtoImage(BoundingBox2D, string, string, string, int, int)"/>.</para>
        /// <para>Only <see cref="Constants.WebService.TrueOrthoInfoFormatHtml"/> carries the sheet attributes; <see cref="Constants.WebService.TrueOrthoInfoFormatGml"/> carries the sheet boxes only, in the same order (verified 2026-11-03).</para>
        /// </summary>
        /// <param name="point2D">The point in EPSG:2180, easting in X.</param>
        /// <param name="infoFormat">The <c>INFO_FORMAT</c> value.</param>
        /// <param name="featureCount">The maximum number of sheets to return.</param>
        /// <returns>The URL, or null when an argument is missing or <paramref name="featureCount"/> is not positive.</returns>
        public static string? Url_TrueOrthoSheets(this Point2D? point2D, string? infoFormat, int featureCount = 50)
        {
            if (point2D == null || string.IsNullOrWhiteSpace(infoFormat) || featureCount <= 0)
            {
                return null;
            }

            const double halfSize = 50.5;

            return string.Format(CultureInfo.InvariantCulture, "{0}?SERVICE=WMS&VERSION=1.1.0&REQUEST=GetFeatureInfo&LAYERS={1}&QUERY_LAYERS={1}&STYLES=&SRS=EPSG:2180&WIDTH=101&HEIGHT=101&X=50&Y=50&FEATURE_COUNT={2}&INFO_FORMAT={3}&BBOX={4},{5},{6},{7}", Constants.WebService.TrueOrtho, Constants.WebService.TrueOrthoIndexLayer, featureCount, Uri.EscapeDataString(infoFormat), (point2D.X - halfSize).ToString("R", CultureInfo.InvariantCulture), (point2D.Y - halfSize).ToString("R", CultureInfo.InvariantCulture), (point2D.X + halfSize).ToString("R", CultureInfo.InvariantCulture), (point2D.Y + halfSize).ToString("R", CultureInfo.InvariantCulture));
        }
    }
}
