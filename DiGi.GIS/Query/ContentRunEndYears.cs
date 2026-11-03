using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets, for every requested year, the last year of the run of consecutive requested years that returned the same image.
        /// <para>This is the year an image is dated against (<see cref="AcquisitionOrtoSheet"/>), not the year it was first requested for. The time-enabled WMS can serve a photograph under a year before the index date of its sheet: in county 1465, 23 of 277 runs began before their sheet's year (for example requests 2006–2010 all answered the sheet filed under 2007, whose index date is a year placeholder; DiGi.GIS#17). Dating by the first year would give such an image the previous sheet's date, while the last year of the run still lies in or after the sheet's year.</para>
        /// <para>The content is compared by <see cref="ContentHash(byte[])"/>. A run is broken by a different image or by a year missing from the dictionary.</para>
        /// </summary>
        /// <param name="bytesDictionary">The images by requested year.</param>
        /// <returns>The last year of each year's run, or null when the input is null.</returns>
        public static Dictionary<int, int>? ContentRunEndYears(this IDictionary<int, byte[]>? bytesDictionary)
        {
            if (bytesDictionary == null)
            {
                return null;
            }

            List<int> years = [.. bytesDictionary.Keys.OrderBy(x => x)];
            List<string?> contentHashes = [.. years.Select(x => ContentHash(bytesDictionary[x]))];

            Dictionary<int, int> result = [];
            for (int i = years.Count - 1; i >= 0; i--)
            {
                bool isContinued = i + 1 < years.Count && years[i + 1] == years[i] + 1 && contentHashes[i] != null && contentHashes[i] == contentHashes[i + 1];
                result[years[i]] = isContinued ? result[years[i + 1]] : years[i];
            }

            return result;
        }
    }
}
