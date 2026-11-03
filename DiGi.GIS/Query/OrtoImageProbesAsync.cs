using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously probes one tile of a time-enabled orthophoto WMS at several requested times and classifies every response.
        /// <para><b>Order of classification</b> (mandatory): exception (<see cref="IsOrtoImage(byte[])"/> false), then blank (<paramref name="isBlank"/>), then the content hash (<see cref="ContentHash(byte[])"/>), recorded for images with content only. Every uniform white tile hashes to the same value, so hashing first would report all blank tiles as one duplicated epoch.</para>
        /// <para><b>Blanks are flaky.</b> One tile once answered blank and then real content minutes later, so a blank is asked for once more, after <paramref name="retryDelay"/>, before it is recorded as <see cref="OrtoImageProbeStatus.Blank"/>.</para>
        /// <para><b>Scale guard.</b> A tile coarser than half of <paramref name="maxScaleDenominator"/> is refused with <see cref="ArgumentOutOfRangeException"/> before anything is sent: above the service limit the answer is a uniform white image with HTTP 200, which would read as "no imagery".</para>
        /// <para>Decoding pixels needs an imaging library, which this netstandard2.0 assembly does not reference; <paramref name="isBlank"/> is supplied by the caller (for example from DiGi.GIS.Emgu.CV). Requests are sent one after another.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send the requests.</param>
        /// <param name="boundingBox2D">The tile in EPSG:2180, easting in X.</param>
        /// <param name="serviceUrl">The WMS endpoint.</param>
        /// <param name="layer">The layer of that endpoint.</param>
        /// <param name="times">The <c>TIME</c> values to probe.</param>
        /// <param name="width">The width of the image in pixels; the height follows the aspect ratio of the tile.</param>
        /// <param name="isBlank">Decides whether an image is of one uniform colour.</param>
        /// <param name="retryCount">The number of times a transient failure is retried.</param>
        /// <param name="retryDelay">The delay before the first retry, and before a blank is asked for again.</param>
        /// <param name="maxScaleDenominator">The <c>MaxScaleDenominator</c> of the service; tiles must stay at or below half of it.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>One probe per requested time, in the order given, or null when an argument is missing.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The tile is coarser than half of <paramref name="maxScaleDenominator"/>.</exception>
        public static async Task<List<OrtoImageProbe>?> OrtoImageProbesAsync(
            this HttpClient? httpClient,
            BoundingBox2D? boundingBox2D,
            string? serviceUrl,
            string? layer,
            IEnumerable<string>? times,
            int width,
            Func<byte[], bool>? isBlank,
            int retryCount,
            TimeSpan retryDelay,
            double maxScaleDenominator = Constants.WebService.OrtoMaxScaleDenominator,
            CancellationToken cancellationToken = default)
        {
            if (httpClient is null || boundingBox2D == null || string.IsNullOrWhiteSpace(serviceUrl) || string.IsNullOrWhiteSpace(layer) || times == null || isBlank == null || width <= 0 || boundingBox2D.Width <= 0 || boundingBox2D.Height <= 0)
            {
                return null;
            }

            double? scaleDenominator = OrtoImageScaleDenominator(boundingBox2D, width);
            if (scaleDenominator == null || scaleDenominator.Value > maxScaleDenominator / 2)
            {
                throw new ArgumentOutOfRangeException(nameof(width), string.Format(CultureInfo.InvariantCulture, "The tile scale denominator {0:F0} exceeds half of the service limit {1:F0}; the service would answer a blank image. Use a smaller tile or more pixels.", scaleDenominator, maxScaleDenominator));
            }

            int height = Math.Max(1, (int)Math.Round(width * boundingBox2D.Height / boundingBox2D.Width));

            List<OrtoImageProbe> result = [];
            foreach (string time in times)
            {
                string? url = Url_OrtoImage(boundingBox2D, serviceUrl, layer, time, width, height);

                OrtoImageProbeStatus ortoImageProbeStatus = OrtoImageProbeStatus.Undefined;
                string? contentHash = null;

                for (int attempt = 0; attempt < 2; attempt++)
                {
                    if (attempt != 0)
                    {
                        await Task.Delay(retryDelay < TimeSpan.Zero ? TimeSpan.Zero : retryDelay, cancellationToken).ConfigureAwait(false);
                    }

                    byte[]? bytes = await ResponseBytesAsync(httpClient, url, retryCount, retryDelay, cancellationToken: cancellationToken).ConfigureAwait(false);
                    if (bytes == null)
                    {
                        ortoImageProbeStatus = OrtoImageProbeStatus.Undefined;
                        break;
                    }

                    if (!IsOrtoImage(bytes))
                    {
                        ortoImageProbeStatus = OrtoImageProbeStatus.Exception;
                        break;
                    }

                    if (isBlank(bytes))
                    {
                        ortoImageProbeStatus = OrtoImageProbeStatus.Blank;
                        continue;
                    }

                    ortoImageProbeStatus = OrtoImageProbeStatus.Image;
                    contentHash = ContentHash(bytes);
                    break;
                }

                result.Add(new OrtoImageProbe(boundingBox2D, serviceUrl, layer, time, ortoImageProbeStatus, contentHash, DateTimeOffset.UtcNow));
            }

            return result;
        }
    }
}
