using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// The type of an orthophoto: standard (roofs offset radially against their ground footprint) or true orthophoto (roofs projected orthogonally).
    /// </summary>
    public enum OrtoType
    {
        /// <summary>
        /// The type was not determined. Rows of the per-sheet index are <see cref="Standard"/> and rows of the true orthophoto service are <see cref="TrueOrtho"/>; this marks a sheet from neither source.
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
