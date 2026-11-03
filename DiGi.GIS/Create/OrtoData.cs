using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the orthophoto data of one image requested from the time-enabled WMS, dated by the index sheet it shows.
        /// <para>The acquisition date is that of <see cref="Query.AcquisitionOrtoSheet"/> at the centre of <paramref name="boundingBox2D"/> for <paramref name="lastYear"/>, the last consecutive requested year that returned the same image (<see cref="Query.ContentRunEndYears"/>): the WMS can serve a photograph under a year before its sheet's index date. Without <paramref name="ortoSheets"/>, or when no sheet qualifies, the image is undated (<see cref="OrtoDatePrecision.Undefined"/>) and keeps only the requested year. The content hash is always set.</para>
        /// </summary>
        /// <param name="year">The year requested from the WMS.</param>
        /// <param name="bytes">The image.</param>
        /// <param name="scale">The scale factor of the image.</param>
        /// <param name="boundingBox2D">The requested area in EPSG:2180; its top-left corner is the image location.</param>
        /// <param name="ortoSheets">The orthophoto index sheets of the area, or null to leave the image undated.</param>
        /// <param name="lastYear">The last consecutive requested year that returned the same image; null means <paramref name="year"/>.</param>
        /// <returns>The orthophoto data, or null when the area is missing.</returns>
        public static OrtoData? OrtoData(int year, byte[]? bytes, double scale, BoundingBox2D? boundingBox2D, IEnumerable<OrtoSheet?>? ortoSheets = null, int? lastYear = null)
        {
            if (boundingBox2D == null)
            {
                return null;
            }

            OrtoSheet? ortoSheet = Query.AcquisitionOrtoSheet(ortoSheets, boundingBox2D.GetCentroid(), lastYear ?? year);

            return new Classes.OrtoData(
                new DateTime(year, 1, 1),
                bytes,
                scale,
                boundingBox2D.TopLeft,
                ortoSheet?.Date,
                ortoSheet == null ? OrtoDatePrecision.Undefined : ortoSheet.DatePrecision,
                ortoSheet?.SheetId,
                Query.ContentHash(bytes));
        }
    }
}
