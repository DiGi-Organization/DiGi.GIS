using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously reads the true orthophoto sheets covering a point from the true orthophoto WMS (<see cref="Constants.WebService.TrueOrtho"/>), as <see cref="OrtoSheet"/> objects of type <see cref="Enums.OrtoType.TrueOrtho"/>.
        /// <para>True orthophotos are a product of their own and are not rows of the orthophoto index WFS, which has no true orthophoto feed (its WFS answers <c>Unauthorized</c>). Coverage is limited to about a dozen cities (2026-11-03), so most points return an empty list.</para>
        /// <para>Two requests are sent one after the other: the HTML answer for the attributes, then the GML answer for the footprints (<see cref="Convert.ToDiGi_OrtoSheets(string, string, DateTimeOffset?)"/> pairs them). A failed GML request leaves the footprints null rather than failing the call.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send the requests.</param>
        /// <param name="point2D">The point in EPSG:2180, easting in X.</param>
        /// <param name="retryCount">The number of times a transient failure is retried.</param>
        /// <param name="retryDelay">The delay before the first retry, doubling for each attempt after that.</param>
        /// <param name="probeDate">The moment stamped on every sheet; the current UTC time when null.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The sheets covering the point (empty when there is none), or null when an argument is missing or the HTML request fails.</returns>
        public static async Task<List<OrtoSheet>?> TrueOrthoSheetsAsync(this HttpClient? httpClient, Point2D? point2D, int retryCount, TimeSpan retryDelay, DateTimeOffset? probeDate = null, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || point2D == null)
            {
                return null;
            }

            byte[]? bytes_Html = await ResponseBytesAsync(httpClient, Url_TrueOrthoSheets(point2D, Constants.WebService.TrueOrthoInfoFormatHtml), retryCount, retryDelay, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (bytes_Html == null)
            {
                return null;
            }

            byte[]? bytes_Gml = await ResponseBytesAsync(httpClient, Url_TrueOrthoSheets(point2D, Constants.WebService.TrueOrthoInfoFormatGml), retryCount, retryDelay, cancellationToken: cancellationToken).ConfigureAwait(false);

            string html = Encoding.UTF8.GetString(bytes_Html);
            string? gml = bytes_Gml == null ? null : Encoding.UTF8.GetString(bytes_Gml);

            return Convert.ToDiGi_OrtoSheets(html, gml, probeDate ?? DateTimeOffset.UtcNow);
        }
    }
}
