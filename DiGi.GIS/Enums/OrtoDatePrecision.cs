using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// The precision of an orthophoto acquisition date recorded in the sheet index.
    /// </summary>
    public enum OrtoDatePrecision
    {
        /// <summary>
        /// No date is known.
        /// </summary>
        [Description("Undefined")] Undefined = -1,

        /// <summary>
        /// The date is the real acquisition day.
        /// </summary>
        [Description("Day")] Day = 0,

        /// <summary>
        /// Only the acquisition month is known; the day is a placeholder.
        /// </summary>
        [Description("Month")] Month = 1,

        /// <summary>
        /// Only the acquisition year is known; the analog-era index stores a YYYY-01-01 placeholder.
        /// </summary>
        [Description("Year")] Year = 2,
    }
}
