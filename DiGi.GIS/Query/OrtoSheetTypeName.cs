using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the typename of the per-sheet orthophoto index layer for a year (<c>gugik:SkorowidzOrtofomapy2005</c>).
        /// <para>The index publishes 49 yearly typenames: 1957, 1958, 1961, 1965, 1967, 1969, 1971 to 1973, 1976, 1979, 1980, 1982, 1984, 1987, 1990, 1992 and every year from 1995. A year outside that list answers a service exception, not an empty collection.</para>
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>The typename.</returns>
        public static string OrtoSheetTypeName(int year)
        {
            return Constants.WebService.OrtoSheetTypeNamePrefix + year.ToString(CultureInfo.InvariantCulture);
        }
    }
}
