using DiGi.GIS.Enums;
using System;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Classifies the precision of an orthophoto acquisition date as published by GUGiK.
        /// <para>Analog-era rows carry a <c>YYYY-01-01</c> placeholder, so a 1 January date of an analog source (<paramref name="source"/> containing <c>analog</c>) is <see cref="OrtoDatePrecision.Year"/>; every other date is <see cref="OrtoDatePrecision.Day"/>. Both the index WFS and the true orthophoto WMS converters classify through this one rule.</para>
        /// </summary>
        /// <param name="date">The published acquisition date.</param>
        /// <param name="source">The published data source (<c>zrodlo_danych</c>, for example <c>Zdj. analogowe</c>).</param>
        /// <returns>The precision, or <see cref="OrtoDatePrecision.Undefined"/> when the date is null.</returns>
        public static OrtoDatePrecision OrtoSheetDatePrecision(this DateTimeOffset? date, string? source)
        {
            if (date == null)
            {
                return OrtoDatePrecision.Undefined;
            }

            bool analog = source != null && source.IndexOf("analog", StringComparison.OrdinalIgnoreCase) >= 0;

            return analog && date.Value.Month == 1 && date.Value.Day == 1 ? OrtoDatePrecision.Year : OrtoDatePrecision.Day;
        }
    }
}
