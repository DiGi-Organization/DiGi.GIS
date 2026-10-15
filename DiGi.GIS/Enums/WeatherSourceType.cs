using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// Specifies the kind of weather data a stored solar radiation summary was calculated from.
    /// <para>The weather source describes the summary only: hourly shading factors depend on the sun position alone, so they are valid for any weather source.</para>
    /// </summary>
    public enum WeatherSourceType
    {
        /// <summary>
        /// The weather source is not known. A stored summary never carries this value; it is the default of a missing or unparsed member.
        /// </summary>
        [Description("Undefined")] Undefined = -1,

        /// <summary>
        /// A typical-year EnergyPlus weather file; the source id is the id of its <c>epw_file</c> row as an invariant integer string.
        /// </summary>
        [Description("EPW")] EPW = 0,

        /// <summary>
        /// Measured weather data from a live provider; the source id names the provider or station.
        /// </summary>
        [Description("Live")] Live = 1,
    }
}
