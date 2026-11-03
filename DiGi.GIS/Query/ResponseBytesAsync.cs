using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Asynchronously fetches the body of a GET request, retrying transient failures.
        /// <para>A non-success status is retried only when <see cref="IsTransient(HttpStatusCode)"/> says so; a transport failure or the client's own timeout is retried as well. The delay doubles after each attempt, and a <c>Retry-After</c> the server sends takes its place for the attempt that follows. Same policy as <see cref="ElevationAsync(HttpClient, Geometry.Planar.Classes.Point2D, int, TimeSpan, CancellationToken)"/>, except that HTTP 500 is not retried.</para>
        /// <para>The body is returned whatever it contains: the GUGiK services answer service exceptions with HTTP 200, so classifying the content is the caller's job.</para>
        /// </summary>
        /// <param name="httpClient">The HTTP client used to send the request.</param>
        /// <param name="url">The request URL.</param>
        /// <param name="retryCount">The number of times a transient failure is retried; zero sends the request once.</param>
        /// <param name="retryDelay">The delay before the first retry, doubling for each attempt after that.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The response body, or null when the arguments are missing or the request still fails after the retries.</returns>
        public static async Task<byte[]?> ResponseBytesAsync(this HttpClient? httpClient, string? url, int retryCount, TimeSpan retryDelay, CancellationToken cancellationToken = default)
        {
            if (httpClient is null || string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            TimeSpan delay = retryDelay < TimeSpan.Zero ? TimeSpan.Zero : retryDelay;

            for (int attempt = 0; ; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    using (HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false))
                    {
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            // ReadAsByteArrayAsync takes no cancellation token on netstandard2.0.
                            return await httpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        }

                        if (attempt >= retryCount || !httpResponseMessage.StatusCode.IsTransient())
                        {
                            return null;
                        }

                        if (httpResponseMessage.Headers.RetryAfter?.Delta is TimeSpan delta && delta > TimeSpan.Zero)
                        {
                            delay = delta;
                        }
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception)
                {
                    if (attempt >= retryCount)
                    {
                        return null;
                    }
                }

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

                if (delay <= TimeSpan.FromTicks(TimeSpan.MaxValue.Ticks / 2))
                {
                    delay = TimeSpan.FromTicks(delay.Ticks * 2);
                }
            }
        }
    }
}
