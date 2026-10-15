using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Quantizes shading factors into one byte each, as <see cref="Classes.SurfaceSolarRadiation.ShadingFactors"/> stores them.
        /// <para>Each value is clamped to [0, 1] and mapped to <c>round(f · 255)</c>, rounding a midpoint away from zero, so the step is 1/255 and the round-trip error through <see cref="ToSystem_Doubles(byte[])"/> is at most 0.5/255. NaN maps to 0.</para>
        /// </summary>
        /// <param name="shadingFactors">The shading factors, 0 (unshaded) to 1 (fully shaded).</param>
        /// <returns>One byte per shading factor, in the same order, or null when <paramref name="shadingFactors"/> is null.</returns>
        public static byte[]? ToSystem_Bytes(this IEnumerable<double>? shadingFactors)
        {
            if (shadingFactors is null)
            {
                return null;
            }

            List<byte> bytes = [];
            foreach (double shadingFactor in shadingFactors)
            {
                if (double.IsNaN(shadingFactor) || shadingFactor <= 0)
                {
                    bytes.Add(0);
                    continue;
                }

                if (shadingFactor >= 1)
                {
                    bytes.Add(byte.MaxValue);
                    continue;
                }

                bytes.Add((byte)Math.Round(shadingFactor * byte.MaxValue, MidpointRounding.AwayFromZero));
            }

            return [.. bytes];
        }
    }
}
