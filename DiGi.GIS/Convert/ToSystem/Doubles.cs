namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Decodes quantized shading factors, as stored in <see cref="Classes.SurfaceSolarRadiation.ShadingFactors"/>, back into values from 0 to 1.
        /// <para>Each byte <c>b</c> maps to <c>b / 255</c>, the inverse of <see cref="ToSystem_Bytes(System.Collections.Generic.IEnumerable{double})"/>.</para>
        /// </summary>
        /// <param name="bytes">The quantized shading factors.</param>
        /// <returns>One shading factor per byte, in the same order, or null when <paramref name="bytes"/> is null.</returns>
        public static double[]? ToSystem_Doubles(this byte[]? bytes)
        {
            if (bytes is null)
            {
                return null;
            }

            double[] result = new double[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                result[i] = bytes[i] / (double)byte.MaxValue;
            }

            return result;
        }
    }
}
