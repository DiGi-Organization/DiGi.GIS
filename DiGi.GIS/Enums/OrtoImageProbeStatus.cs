using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// The classification of one WMS <c>GetMap</c> response.
    /// <para>The order of the checks matters: exception first, blank second, content hash last. Every uniform white tile hashes to the same value, so hashing before the blank check reports all blank tiles in Poland as one duplicated epoch.</para>
    /// </summary>
    public enum OrtoImageProbeStatus
    {
        /// <summary>
        /// The request was not answered (transport failure or a non-success status after retries).
        /// </summary>
        [Description("Undefined")] Undefined = -1,

        /// <summary>
        /// The response is an image with content.
        /// </summary>
        [Description("Image")] Image = 0,

        /// <summary>
        /// The response is an image of one uniform colour: no data at that place and time, or a scale above the service limit.
        /// </summary>
        [Description("Blank")] Blank = 1,

        /// <summary>
        /// The response is an OGC service exception document rather than an image.
        /// </summary>
        [Description("Exception")] Exception = 2,
    }
}
