using System;
using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// Conditions that reduce how well an orthophoto sheet shows buildings, or that limit what is known about it.
    /// </summary>
    [Flags]
    public enum OrtoObstacle
    {
        /// <summary>
        /// No obstacle is known.
        /// </summary>
        [Description("None")] None = 0,

        /// <summary>
        /// No image of the sheet was probed; the clarity factor uses month priors instead of measurements.
        /// </summary>
        [Description("Unprobed")] Unprobed = 1,

        /// <summary>
        /// A significant fraction of the probed image is in deep shadow.
        /// </summary>
        [Description("Shadow")] Shadow = 2,

        /// <summary>
        /// A significant fraction of the probed image is snow covered (RGB heuristic; no SWIR band, so no NDSI).
        /// </summary>
        [Description("Snow")] Snow = 4,

        /// <summary>
        /// The probed image is hazy or foggy (low local contrast).
        /// </summary>
        [Description("Haze")] Haze = 8,

        /// <summary>
        /// A significant fraction of the probed image is cloud covered.
        /// </summary>
        [Description("Cloud")] Cloud = 16,

        /// <summary>
        /// The index marks the sheet as not completely filled with imagery.
        /// </summary>
        [Description("Sheet Incomplete")] SheetIncomplete = 32,

        /// <summary>
        /// The acquisition date is known to the year only.
        /// </summary>
        [Description("Date Year Precision")] DateYearPrecision = 64,

        /// <summary>
        /// Whether the sheet is a true orthophoto was not determined.
        /// </summary>
        [Description("True Ortho Unknown")] TrueOrthoUnknown = 128,
    }
}
