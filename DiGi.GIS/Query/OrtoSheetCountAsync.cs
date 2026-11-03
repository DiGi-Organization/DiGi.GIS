using DiGi.Geometry.Planar.Classes;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously asks the orthophoto index WFS how many features a yearly typename holds (<c>RESULTTYPE=hits</c>, attribute <c>numberOfFeatures</c>).
        /// <para>Use it to verify a pull: <see cref="OrtoSheetsAsync(HttpClient, string, BoundingBox2D, int, int, TimeSpan, DateTimeOffset?, CancellationToken)"/> returning fewer sheets than this means features were lost.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send the request.</param>
        /// <param name="typeName">The yearly typename (<see cref="OrtoSheetTypeName(int)"/>).</param>
        /// <param name="boundingBox2D">An optional filter box in EPSG:2180, easting in X.</param>
        /// <param name="retryCount">The number of times a transient failure is retried.</param>
        /// <param name="retryDelay">The delay before the first retry, doubling for each attempt after that.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The feature count, or null when the request fails or the answer carries no count (a service exception, for example).</returns>
        public static async Task<int?> OrtoSheetCountAsync(this HttpClient? httpClient, string? typeName, BoundingBox2D? boundingBox2D, int retryCount, TimeSpan retryDelay, CancellationToken cancellationToken = default)
        {
            string? url = Url_OrtoSheetCount(typeName, boundingBox2D);

            byte[]? bytes = await ResponseBytesAsync(httpClient, url, retryCount, retryDelay, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (bytes == null)
            {
                return null;
            }

            try
            {
                using MemoryStream memoryStream = new(bytes);
                using XmlReader xmlReader = XmlReader.Create(memoryStream, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore });

                if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.LocalName == "FeatureCollection" && int.TryParse(xmlReader.GetAttribute("numberOfFeatures"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
                {
                    return result;
                }
            }
            catch (XmlException)
            {
            }

            return null;
        }
    }
}
