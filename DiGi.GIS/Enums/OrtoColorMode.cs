using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// The colour mode of an orthophoto sheet (index field <c>kolor</c>).
    /// </summary>
    public enum OrtoColorMode
    {
        /// <summary>
        /// The colour mode is missing or not recognised.
        /// </summary>
        [Description("Undefined")] Undefined = -1,

        /// <summary>
        /// Black and white (<c>B/W</c>).
        /// </summary>
        [Description("Black and White")] BlackAndWhite = 0,

        /// <summary>
        /// True colour (<c>RGB</c>).
        /// </summary>
        [Description("RGB")] RGB = 1,

        /// <summary>
        /// Colour infrared (<c>CIR</c>).
        /// </summary>
        [Description("CIR")] CIR = 2,
    }
}
