namespace DiGi.GIS.Constants
{
    /// <summary>
    /// Provides the parameters of the dependency-free blank check on orthophoto JPEG tiles (<see cref="Query.IsOrtoJpegBlank(byte[])"/>).
    /// </summary>
    public static class OrtoBlank
    {
        /// <summary>
        /// The largest spread (maximum minus minimum, per colour component, on 0–255) of the 8 × 8 block means for which a tile still counts as one uniform colour.
        /// <para>The blank tile the GUGiK WMS answers decodes as exactly uniform white, so every block mean is the same and the tolerance only absorbs rounding of the DC quantisation. Same value as the pixel-based check in DiGi.GIS.Emgu.CV.</para>
        /// </summary>
        public const int Tolerance = 8;
    }
}
