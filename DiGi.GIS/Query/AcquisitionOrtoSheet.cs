using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the orthophoto index sheet whose photograph the time-enabled WMS shows at a location for a requested year: among the standard sheets whose footprint contains the location, the one with the latest date in or before that year.
        /// <para>For a year with no flight at the location the WMS answers the nearest earlier imagery, so the requested year alone is not the date of the photograph: in county 1465, 257 of 527 probed images fell in a year without a sheet at the point, and one image was served for up to 6 consecutive years (DiGi.GIS#17).</para>
        /// <para>Sheets on the same date are ordered by the finest <see cref="OrtoSheet.PixelSize"/>, then RGB before CIR, the composite the WMS serves. True orthophoto sheets are skipped: the time-enabled WMS does not serve them.</para>
        /// </summary>
        /// <param name="ortoSheets">The index sheets to search, for example the rows of the location's area; nulls and sheets without a footprint or date are ignored.</param>
        /// <param name="point2D">The location in EPSG:2180, easting in X.</param>
        /// <param name="year">The year to date against: for an image, the last consecutive requested year that returned it (<see cref="ContentRunEndYears"/>), not the first.</param>
        /// <returns>The sheet, or null when no sheet of that year or earlier contains the location.</returns>
        public static OrtoSheet? AcquisitionOrtoSheet(this IEnumerable<OrtoSheet?>? ortoSheets, Point2D? point2D, int year)
        {
            if (ortoSheets == null || point2D == null)
            {
                return null;
            }

            OrtoSheet? result = null;
            foreach (OrtoSheet? ortoSheet in ortoSheets)
            {
                if (ortoSheet?.Date is not DateTimeOffset date || ortoSheet.Footprint == null || ortoSheet.OrtoType == OrtoType.TrueOrtho)
                {
                    continue;
                }

                if (date.Year > year || !ortoSheet.Footprint.InRange(point2D))
                {
                    continue;
                }

                if (result == null || IsBetter(ortoSheet, result))
                {
                    result = ortoSheet;
                }
            }

            return result;

            static bool IsBetter(OrtoSheet ortoSheet, OrtoSheet ortoSheet_Current)
            {
                int comparison = ortoSheet.Date!.Value.CompareTo(ortoSheet_Current.Date!.Value);
                if (comparison != 0)
                {
                    return comparison > 0;
                }

                double pixelSize = ortoSheet.PixelSize ?? double.MaxValue;
                double pixelSize_Current = ortoSheet_Current.PixelSize ?? double.MaxValue;
                if (pixelSize != pixelSize_Current)
                {
                    return pixelSize < pixelSize_Current;
                }

                return ortoSheet.ColorMode == OrtoColorMode.RGB && ortoSheet_Current.ColorMode != OrtoColorMode.RGB;
            }
        }
    }
}
