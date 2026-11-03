using DiGi.GIS.Classes;
using DiGi.GIS.Enums;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the obstacles known for an orthophoto sheet: those its index record implies, combined with any measured on a probed image.
        /// <para>From the record: <see cref="OrtoObstacle.SheetIncomplete"/> (not completely filled), <see cref="OrtoObstacle.DateYearPrecision"/> (year-only date) and <see cref="OrtoObstacle.TrueOrthoUnknown"/> (type not determined; never set for a sheet read by <see cref="Convert.ToDiGi_OrtoSheet(System.Xml.Linq.XElement, System.DateTimeOffset?)"/> or <see cref="TrueOrthoSheetsAsync(System.Net.Http.HttpClient, Geometry.Planar.Classes.Point2D, int, System.TimeSpan, System.DateTimeOffset?, System.Threading.CancellationToken)"/>). <see cref="OrtoObstacle.Unprobed"/> is set when <paramref name="measuredOrtoObstacle"/> is null.</para>
        /// </summary>
        /// <param name="ortoSheet">The sheet.</param>
        /// <param name="measuredOrtoObstacle">The obstacles measured on a probed image of the sheet, or null when it was not probed.</param>
        /// <returns>The combined obstacles, or <see cref="OrtoObstacle.None"/> when the sheet is null.</returns>
        public static OrtoObstacle OrtoObstacles(this OrtoSheet? ortoSheet, OrtoObstacle? measuredOrtoObstacle = null)
        {
            if (ortoSheet == null)
            {
                return OrtoObstacle.None;
            }

            OrtoObstacle result = measuredOrtoObstacle ?? OrtoObstacle.Unprobed;

            if (ortoSheet.SheetFilled == false)
            {
                result |= OrtoObstacle.SheetIncomplete;
            }

            if (ortoSheet.DatePrecision == OrtoDatePrecision.Year)
            {
                result |= OrtoObstacle.DateYearPrecision;
            }

            if (ortoSheet.OrtoType == OrtoType.Undefined)
            {
                result |= OrtoObstacle.TrueOrthoUnknown;
            }

            return result;
        }
    }
}
