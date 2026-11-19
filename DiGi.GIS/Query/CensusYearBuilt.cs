using DiGi.BDL.Enums;
using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Returns the census year built of an area: the median construction year of its inhabited dwellings built up to <paramref name="lastYear"/>, weighted by dwelling count (DiGi-Organization/DiGi.GIS#23).
        /// <para>The counts are the NSP 2021 dwellings by construction period: every <see cref="Variable"/> with a <c>BDL.Query.YearBuiltRange</c>, read for <paramref name="year"/>. A missing, negative or zero count is skipped. A period starting after <paramref name="lastYear"/> is dropped; a period running past it keeps the share of its count that falls up to <paramref name="lastYear"/>, the count taken as uniform over the period's years.</para>
        /// <para>The median is interpolated linearly within the period holding half the weight and rounded down. A median that falls in the period open towards the past ("before 1918") has no width to interpolate in and is that period's last year; a median exactly on a period boundary is the last year of the earlier period.</para>
        /// <para>It stands in for a predicted year built that only bounds the year (DiGi-Organization/DiGi.GIS.Analytical#17); the default <paramref name="lastYear"/> is <see cref="Constants.YearBuiltPrediction.FirstYear"/>, because the predictor dates every building it saw appear after it.</para>
        /// </summary>
        /// <param name="statisticalDataCollection">The statistical data of the area's statistical unit.</param>
        /// <param name="lastYear">The last construction year counted.</param>
        /// <param name="year">The year the census counts are read for.</param>
        /// <returns>The census year built, or null when the collection is null or holds no positive count up to <paramref name="lastYear"/>.</returns>
        public static short? CensusYearBuilt(this StatisticalDataCollection? statisticalDataCollection, short lastYear = Constants.YearBuiltPrediction.FirstYear, short year = 2021)
        {
            if (statisticalDataCollection is null)
            {
                return null;
            }

            List<(short? FirstYear, short LastYear, double Count)> periods = [];
            foreach (Variable variable in Enum.GetValues(typeof(Variable)))
            {
                BDL.Classes.YearBuiltRange? yearBuiltRange = BDL.Query.YearBuiltRange(variable);
                if (yearBuiltRange is null || yearBuiltRange.FirstYear > lastYear)
                {
                    continue;
                }

                StatisticalYearlyDoubleData? statisticalYearlyDoubleData = StatisticalData<StatisticalYearlyDoubleData>(statisticalDataCollection, variable);
                if (statisticalYearlyDoubleData is null || !statisticalYearlyDoubleData.TryGetValue(year, out double count) || double.IsNaN(count) || count <= 0)
                {
                    continue;
                }

                short lastYear_Period = yearBuiltRange.LastYear;
                if (lastYear_Period > lastYear)
                {
                    if (yearBuiltRange.FirstYear is short firstYear_Period)
                    {
                        count *= (double)(lastYear - firstYear_Period + 1) / (lastYear_Period - firstYear_Period + 1);
                    }

                    lastYear_Period = lastYear;
                }

                periods.Add((yearBuiltRange.FirstYear, lastYear_Period, count));
            }

            double total = periods.Sum(x => x.Count);
            if (periods.Count == 0 || total <= 0)
            {
                return null;
            }

            periods = [.. periods.OrderBy(x => x.LastYear)];

            double half = total / 2;
            double cumulative = 0;
            foreach ((short? firstYear, short lastYear_Period, double count) in periods)
            {
                if (cumulative + count < half)
                {
                    cumulative += count;
                    continue;
                }

                if (firstYear is not short firstYear_Period)
                {
                    return lastYear_Period;
                }

                double median = firstYear_Period + ((half - cumulative) / count * (lastYear_Period - firstYear_Period + 1));
                return (short)System.Math.Min(lastYear_Period, System.Math.Max(firstYear_Period, System.Math.Floor(median)));
            }

            return periods[periods.Count - 1].LastYear;
        }
    }
}
