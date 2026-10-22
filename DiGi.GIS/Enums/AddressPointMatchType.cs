using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// Specifies how an address point was assigned to a building footprint.
    /// </summary>
    public enum AddressPointMatchType
    {
        /// <summary>
        /// The match type is not known. A created match never carries this value; it is the default of a missing or unparsed member.
        /// </summary>
        [Description("Undefined")] Undefined = -1,

        /// <summary>
        /// The point lies inside the footprint, or on its boundary within the tolerance.
        /// </summary>
        [Description("Inside")] Inside = 0,

        /// <summary>
        /// The point lies outside every footprint and was assigned to the nearest one within the maximum distance.
        /// </summary>
        [Description("Nearest")] Nearest = 1,

        /// <summary>
        /// No footprint lies within the maximum distance of the point, or the point has no location.
        /// </summary>
        [Description("Unmatched")] Unmatched = 2,
    }
}
