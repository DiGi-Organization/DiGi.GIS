using DiGi.GIS.Classes;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the distinct years in which an orthophoto was observed at a location, in ascending order — the evidence-based alternative to the fixed <see cref="Classes.OrtoDatasOptions.Years"/> default range.
        /// <para>Years come from deduplicated epochs, so a year in which the index only re-registered an earlier image does not appear. The list reflects the index, not what the time-enabled WMS serves; the two can drift.</para>
        /// </summary>
        /// <param name="ortoAvailability">The matrix cell of the location.</param>
        /// <returns>The years, or null when the cell is null.</returns>
        public static List<int>? AvailableOrtoYears(this OrtoAvailability? ortoAvailability)
        {
            if (ortoAvailability == null)
            {
                return null;
            }

            SortedSet<int> years = [];
            foreach (OrtoEpoch ortoEpoch in ortoAvailability.OrtoEpochs ?? [])
            {
                years.Add(ortoEpoch.Date.Year);
            }

            return [.. years];
        }
    }
}
