using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// The type of an orthophoto: standard (roofs offset radially against their ground footprint) or true orthophoto (roofs projected orthogonally).
    /// </summary>
    public enum OrtoType
    {
        /// <summary>
        /// The type was not determined. The per-sheet index does not carry it, and the true orthophoto service offers no per-sheet feed.
        /// </summary>
        [Description("Undefined")] Undefined = -1,

        /// <summary>
        /// A standard orthophoto.
        /// </summary>
        [Description("Standard")] Standard = 0,

        /// <summary>
        /// A true orthophoto.
        /// </summary>
        [Description("True Ortho")] TrueOrtho = 1,
    }
}
