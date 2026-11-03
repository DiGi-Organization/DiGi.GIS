using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Groups the images of one location into observations: images with the same content are one photograph, whatever year they were requested for.
        /// <para>The time-enabled WMS repeats the nearest earlier imagery for years with no flight, and <see cref="OrtoDatas.Reduce"/> removes only repeats in consecutive years, so the same photograph can still appear twice (A, B, A). The content is compared by <see cref="OrtoData.ContentHash"/>, or by <see cref="ContentHash(byte[])"/> of the bytes when the member is missing (rows stored before it existed).</para>
        /// <para>Within a group the images are ordered by <see cref="OrtoData.AcquisitionDate"/> (undated last), then by the requested <see cref="OrtoData.DateTime"/>; the first image dates the observation. The groups are ordered by their first images in the same way. Images without bytes are ignored.</para>
        /// </summary>
        /// <param name="ortoDatas">The images of one location.</param>
        /// <returns>The observations, or null when the input is null.</returns>
        public static List<List<OrtoData>>? OrtoObservations(this IEnumerable<OrtoData?>? ortoDatas)
        {
            if (ortoDatas == null)
            {
                return null;
            }

            Dictionary<string, List<OrtoData>> dictionary = [];
            foreach (OrtoData? ortoData in ortoDatas)
            {
                string? contentHash = ortoData?.ContentHash ?? ContentHash(ortoData?.Bytes);
                if (ortoData == null || contentHash == null || ortoData.Bytes == null || ortoData.Bytes.Length == 0)
                {
                    continue;
                }

                if (!dictionary.TryGetValue(contentHash, out List<OrtoData>? ortoDatas_Hash))
                {
                    ortoDatas_Hash = [];
                    dictionary[contentHash] = ortoDatas_Hash;
                }

                ortoDatas_Hash.Add(ortoData);
            }

            List<List<OrtoData>> result = [.. dictionary.Values.Select(x => x.OrderBy(y => y.AcquisitionDate ?? DateTimeOffset.MaxValue).ThenBy(y => y.DateTime).ToList())];

            return [.. result.OrderBy(x => x[0].AcquisitionDate ?? DateTimeOffset.MaxValue).ThenBy(x => x[0].DateTime)];
        }
    }
}
