using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets, for every image of a stored (reduced) series, the last requested year of its run: the year before the next image's requested year, or <paramref name="lastYear"/> for the last image.
        /// <para>This is the stored-series counterpart of <see cref="ContentRunEndYears"/>. <see cref="OrtoDatas.Reduce"/> drops consecutive identical images and keeps the first requested year, so a stored image stood for every year up to the next stored one. A requested year that failed to download is indistinguishable from a repeat; its run is then over-long, which can only move the date to a later sheet within the gap.</para>
        /// </summary>
        /// <param name="ortoDatas">The stored series.</param>
        /// <param name="lastYear">The last year requested when the series was fetched (<c>OrtoDatasOptions.Years</c>).</param>
        /// <returns>The last year of each image's run by its requested <see cref="OrtoData.DateTime"/>, or null when the series is null.</returns>
        public static Dictionary<DateTime, int>? OrtoDataRunEndYears(this OrtoDatas? ortoDatas, int lastYear)
        {
            if (ortoDatas == null)
            {
                return null;
            }

            List<DateTime> dateTimes = [.. ortoDatas.Where(x => x != null).Select(x => x.DateTime).Distinct().OrderBy(x => x)];

            Dictionary<DateTime, int> result = [];
            for (int i = 0; i < dateTimes.Count; i++)
            {
                int year = i + 1 < dateTimes.Count ? dateTimes[i + 1].Year - 1 : lastYear;
                result[dateTimes[i]] = Math.Max(dateTimes[i].Year, year);
            }

            return result;
        }
    }
}
