using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Scores how well an orthophoto sheet suits building recognition, on [0, 1], under rubric version <see cref="Constants.OrtoUsability.Version"/>.
        /// <para>The score is a weighted sum of five factors (weights and values in <see cref="Constants.OrtoUsability"/>): resolution (<see cref="OrtoSheet.PixelSize"/>, linear between knots), orthophoto type, colour mode, source (analog or digital) and clarity.</para>
        /// <para>Clarity is one minus <paramref name="obstacleFraction"/> when the sheet was probed. Without a probe it falls back to month priors — a leaf-on month (April to October) or a snow-risk month (December to February) costs a fixed amount — which are assumptions, not measurements; <see cref="OrtoObstacles(OrtoSheet, OrtoObstacle?)"/> then reports <see cref="OrtoObstacle.Unprobed"/>.</para>
        /// <para>A sheet the index marks as not completely filled is capped at <see cref="Constants.OrtoUsability.SheetIncompleteCap"/>. A year-precision date does not change the score. The scores are for relative sample selection, not radiometric claims.</para>
        /// </summary>
        /// <param name="ortoSheet">The sheet to score.</param>
        /// <param name="obstacleFraction">The measured fraction of the probed image lost to shadow, snow, haze or cloud, or null when the sheet was not probed.</param>
        /// <returns>The score, or null when the sheet is null.</returns>
        public static double? OrtoSheetUsability(this OrtoSheet? ortoSheet, double? obstacleFraction = null)
        {
            if (ortoSheet == null)
            {
                return null;
            }

            double[] resolutionKnots = Constants.OrtoUsability.ResolutionKnots;
            double[] resolutionValues = Constants.OrtoUsability.ResolutionValues;

            double resolution = resolutionValues[resolutionValues.Length - 1];
            if (ortoSheet.PixelSize is double pixelSize && !double.IsNaN(pixelSize))
            {
                if (pixelSize <= resolutionKnots[0])
                {
                    resolution = resolutionValues[0];
                }
                else
                {
                    for (int i = 1; i < resolutionKnots.Length; i++)
                    {
                        if (pixelSize <= resolutionKnots[i])
                        {
                            double fraction = (pixelSize - resolutionKnots[i - 1]) / (resolutionKnots[i] - resolutionKnots[i - 1]);
                            resolution = resolutionValues[i - 1] + (fraction * (resolutionValues[i] - resolutionValues[i - 1]));
                            break;
                        }
                    }
                }
            }

            double ortoType = ortoSheet.OrtoType switch
            {
                OrtoType.TrueOrtho => Constants.OrtoUsability.TrueOrthoValue,
                OrtoType.Standard => Constants.OrtoUsability.StandardOrthoValue,
                _ => Constants.OrtoUsability.UndefinedOrthoValue,
            };

            double colorMode = ortoSheet.ColorMode switch
            {
                OrtoColorMode.CIR => Constants.OrtoUsability.CIRValue,
                OrtoColorMode.RGB => Constants.OrtoUsability.RGBValue,
                OrtoColorMode.BlackAndWhite => Constants.OrtoUsability.BlackAndWhiteValue,
                _ => Constants.OrtoUsability.UndefinedColorModeValue,
            };

            double source = Constants.OrtoUsability.UndefinedSourceValue;
            if (ortoSheet.Source is string source_Text)
            {
                if (source_Text.IndexOf("cyfrow", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    source = Constants.OrtoUsability.DigitalSourceValue;
                }
                else if (source_Text.IndexOf("analog", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    source = Constants.OrtoUsability.AnalogSourceValue;
                }
            }

            double clarity;
            if (obstacleFraction is double obstacleFraction_Value)
            {
                clarity = 1 - Math.Max(0, Math.Min(1, obstacleFraction_Value));
            }
            else
            {
                clarity = 1;

                int? month = ortoSheet.DatePrecision == OrtoDatePrecision.Day || ortoSheet.DatePrecision == OrtoDatePrecision.Month ? ortoSheet.Date?.Month : null;
                if (month == null || (month >= 4 && month <= 10))
                {
                    clarity -= Constants.OrtoUsability.LeafOnPrior;
                }
                else if (month == 12 || month <= 2)
                {
                    clarity -= Constants.OrtoUsability.SnowRiskPrior;
                }
            }

            double result =
                (Constants.OrtoUsability.ResolutionWeight * resolution) +
                (Constants.OrtoUsability.OrtoTypeWeight * ortoType) +
                (Constants.OrtoUsability.ColorModeWeight * colorMode) +
                (Constants.OrtoUsability.SourceWeight * source) +
                (Constants.OrtoUsability.ClarityWeight * clarity);

            if (ortoSheet.SheetFilled == false)
            {
                result = Math.Min(result, Constants.OrtoUsability.SheetIncompleteCap);
            }

            return Math.Max(0, Math.Min(1, result));
        }
    }
}
