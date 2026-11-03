using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
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
        /// Asynchronously reads every sheet of one yearly orthophoto index typename, page by page.
        /// <para>Pages are requested one after another (never in parallel) with <c>STARTINDEX</c> advancing by <paramref name="pageSize"/>, until a page carries fewer features than that. A feature that does not convert is skipped (compare with <see cref="OrtoSheetCountAsync(HttpClient, string, BoundingBox2D, int, TimeSpan, CancellationToken)"/>) but still counts towards its page.</para>
        /// <para>Nothing is truncated silently. A page that still fails after the retries, or answers something other than a feature collection (a service exception arrives with HTTP 200), makes the whole call return null rather than the pages read so far. A page that starts with the same feature as the page before means the service ignored <c>STARTINDEX</c>, and throws <see cref="InvalidOperationException"/>, because carrying on would repeat the first page indefinitely.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send the requests.</param>
        /// <param name="typeName">The yearly typename (<see cref="OrtoSheetTypeName(int)"/>).</param>
        /// <param name="boundingBox2D">An optional filter box in EPSG:2180, easting in X.</param>
        /// <param name="pageSize">The number of features per page (<see cref="Constants.WebService.OrtoSheetPageSize"/>).</param>
        /// <param name="retryCount">The number of times a transient failure of a page is retried.</param>
        /// <param name="retryDelay">The delay before the first retry, doubling for each attempt after that.</param>
        /// <param name="probeDate">The moment recorded on every sheet as the time the index was read; null records the current time.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The sheets in service order, or null when the arguments are invalid or a page could not be read.</returns>
        /// <exception cref="InvalidOperationException">The service answered the same page twice.</exception>
        public static async Task<List<OrtoSheet>?> OrtoSheetsAsync(
            this HttpClient? httpClient,
            string? typeName,
            BoundingBox2D? boundingBox2D,
            int pageSize,
            int retryCount,
            TimeSpan retryDelay,
            DateTimeOffset? probeDate = null,
            CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(typeName) || pageSize <= 0)
            {
                return null;
            }

            DateTimeOffset probeDate_Temp = probeDate ?? DateTimeOffset.UtcNow;

            List<OrtoSheet> result = [];
            string? key_Previous = null;

            for (int startIndex = 0; ; startIndex += pageSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string? url = Url_OrtoSheets(typeName, startIndex, pageSize, boundingBox2D);

                byte[]? bytes = await ResponseBytesAsync(httpClient, url, retryCount, retryDelay, cancellationToken: cancellationToken).ConfigureAwait(false);
                int? count = bytes == null ? null : FeatureCount(bytes);
                if (bytes == null || count == null)
                {
                    return null;
                }

                List<OrtoSheet> ortoSheets = [];
                using (MemoryStream memoryStream = new(bytes))
                {
                    ortoSheets.AddRange(Convert.ToDiGi_OrtoSheets(memoryStream, probeDate_Temp));
                }

                if (ortoSheets.Count != 0)
                {
                    string key = ortoSheets[0].SheetId + "|" + ortoSheets[0].DownloadUrl;
                    if (key_Previous != null && key == key_Previous)
                    {
                        throw new InvalidOperationException(string.Format("The orthophoto index answered the page starting at {0} with the same first feature as the previous page ({1}); STARTINDEX is not being honoured.", startIndex, key));
                    }

                    key_Previous = key;
                    result.AddRange(ortoSheets);
                }

                // The end of the typename is decided by the features the page carried, not by the sheets that converted:
                // a page with one unreadable feature is still a full page.
                if (count.Value < pageSize)
                {
                    return result;
                }
            }

            // The number of features in a feature collection, or null when the document is not one.
            static int? FeatureCount(byte[] bytes)
            {
                try
                {
                    using MemoryStream memoryStream = new(bytes);
                    using XmlReader xmlReader = XmlReader.Create(memoryStream, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore });

                    if (xmlReader.MoveToContent() != XmlNodeType.Element || xmlReader.LocalName != "FeatureCollection")
                    {
                        return null;
                    }

                    int depth = xmlReader.Depth;
                    int result = 0;
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Depth == depth + 1 && (xmlReader.LocalName == "featureMember" || xmlReader.LocalName == "member"))
                        {
                            result++;
                        }
                    }

                    return result;
                }
                catch (XmlException)
                {
                    return null;
                }
            }
        }
    }
}
