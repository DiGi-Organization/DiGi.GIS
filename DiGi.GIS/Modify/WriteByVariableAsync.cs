using DiGi.BDL.Classes;
using DiGi.BDL.Enums;
using DiGi.Core.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiGi.GIS
{
    public static partial class Modify
    {
        /// <summary>
        /// Writes the BDL values of the given variables for the given statistical units to a statistical data collection file, fetching each variable for a whole unit level at once (DiGi-Organization/DiGi.GIS.PostgreSQL#149).
        /// <para>One <c>data/by-variable</c> request per page of a level - about 40 for the 4 000 gmina-level units - replaces one request per unit: the per-unit writer sent about 4 700 requests for one missing variable and was stopped by the BDL limits after 8, every later unit failing in silence.</para>
        /// <para>A variable is fetched for a level only when a unit of that level in <paramref name="statisticalUnits"/> lacks it (always with <paramref name="overrideExisting"/>), and - when some unit already holds the variable - only at a level some unit holds it at: the other levels are ones the variable is not published at. Levels go deepest first; within a level the variable the most units lack goes first, so a run the limits stop has filled the most collections (DiGi-Organization/DiGi.GIS.PostgreSQL#149: the census variables, missing from every gmina, came last in the given order and were never reached). A failed fetch is reported, not skipped in silence; after three failures in a row nothing more is requested - a blocked address or an exhausted quota fails every request - and the pairs not attempted are reported as failed too. Only the units given are written; the file is read once and saved once.</para>
        /// </summary>
        /// <param name="statisticalUnits">The statistical units to write, of any level; a statistical town is skipped.</param>
        /// <param name="path">The path to the statistical data collection file.</param>
        /// <param name="variables">The variables to write.</param>
        /// <param name="years">The years to retrieve.</param>
        /// <param name="clientId">The BDL API client identifier, or null for the anonymous limits.</param>
        /// <param name="overrideExisting">True to fetch a variable for a level even when every unit already holds it.</param>
        /// <param name="unitYearlyValuesDictionaryFunc">The fetch of one variable for one unit level, keyed by unit id, null on failure; null for <c>BDL.Create.UnitYearlyValuesDictionary</c>.</param>
        /// <param name="progress">Receives the number of variable and unit level pairs fetched so far, failed ones included.</param>
        /// <param name="cancellationToken">Checked before every fetch; a cancelled run writes nothing.</param>
        /// <returns>The number of collections written and the variable and unit level pairs that failed or were not attempted; null when the inputs are invalid or the file holds collections it cannot read (keyed in a former reference format - see <c>StorageFile.GetUndecodableEntryNames</c>), with nothing fetched.</returns>
        public static async Task<(int CollectionCount, List<(Variable Variable, int UnitLevel)> Failures)?> WriteByVariableAsync(
            this IEnumerable<StatisticalUnit>? statisticalUnits,
            string? path,
            IEnumerable<Variable>? variables,
            IEnumerable<int>? years,
            string? clientId = null,
            bool overrideExisting = false,
            Func<Variable, int, Task<Dictionary<string, UnitYearlyValues>?>>? unitYearlyValuesDictionaryFunc = null,
            IProgress<long>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (statisticalUnits == null || string.IsNullOrWhiteSpace(path) || variables == null || years == null)
            {
                return null;
            }

            List<int> years_Temp = [.. years];
            if (years_Temp.Count == 0)
            {
                return null;
            }

            unitYearlyValuesDictionaryFunc ??= (variable, unitLevel) => BDL.Create.UnitYearlyValuesDictionary(variable, unitLevel, years_Temp, clientId: clientId);

            // The units to write, each with its BDL level (the order of StatisticalUnitType) and its stored collection.
            Dictionary<string, (int UnitLevel, StatisticalDataCollection? StatisticalDataCollection)> units_ByCode = [];
            using (StatisticalDataCollectionFile statisticalDataCollectionFile = new(path))
            {
                statisticalDataCollectionFile.Open();

                // A collection keyed in a former reference format reads as missing: every unit would look empty, every
                // variable would be fetched again, and the save would leave the old entries beside the new ones.
                List<string>? undecodableEntryNames = statisticalDataCollectionFile.GetUndecodableEntryNames();
                if (undecodableEntryNames != null && undecodableEntryNames.Count != 0)
                {
                    return null;
                }

                foreach (StatisticalUnit statisticalUnit in statisticalUnits)
                {
                    if (statisticalUnit?.Code is not string code || statisticalUnit.GetStatisticalUnitType() is not StatisticalUnitType statisticalUnitType || statisticalUnitType == StatisticalUnitType.statistical_towns || units_ByCode.ContainsKey(code))
                    {
                        continue;
                    }

                    UniqueReference? uniqueReference = StatisticalDataCollectionFile.GetUniqueReference(code);
                    units_ByCode[code] = ((int)statisticalUnitType, uniqueReference is null ? null : statisticalDataCollectionFile.GetValue(uniqueReference));
                }
            }

            // A variable is fetched for a level a unit of which lacks it. Where some unit of any level already holds the
            // variable, only the levels holding it are asked: the others are levels the variable is not published at,
            // and asking them for every variable would fail hundreds of times. A variable no unit holds yet - a new
            // one - is asked at every level. The deepest level goes first, so the failures of the levels a new variable
            // is not published at cannot stop the run before the level it is published at.
            // Counted once, unit by unit: how many units of each level hold each variable reference (the variable id, as Query.Contains keys it).
            Dictionary<int, int> unitCounts_ByLevel = [];
            Dictionary<int, Dictionary<string, int>> heldCounts_ByLevel = [];
            foreach ((int unitLevel_Unit, StatisticalDataCollection? statisticalDataCollection_Unit) in units_ByCode.Values)
            {
                unitCounts_ByLevel[unitLevel_Unit] = unitCounts_ByLevel.TryGetValue(unitLevel_Unit, out int unitCount) ? unitCount + 1 : 1;
                if (!heldCounts_ByLevel.TryGetValue(unitLevel_Unit, out Dictionary<string, int>? heldCounts))
                {
                    heldCounts = [];
                    heldCounts_ByLevel[unitLevel_Unit] = heldCounts;
                }

                if (statisticalDataCollection_Unit is null)
                {
                    continue;
                }

                foreach (string reference in statisticalDataCollection_Unit.GetStatisticalDatas<Interfaces.IStatisticalData>().Select(x => x?.Reference).OfType<string>().Distinct())
                {
                    heldCounts[reference] = heldCounts.TryGetValue(reference, out int heldCount) ? heldCount + 1 : 1;
                }
            }

            // Within a level, the variable most units lack goes first: a run the BDL limits stop spends its requests
            // where they fill the most collections - a new variable missing from every unit before one a few units lack.
            List<(Variable Variable, int UnitLevel)> pairs = [];
            foreach (int unitLevel in unitCounts_ByLevel.Keys.OrderByDescending(x => x))
            {
                List<(Variable Variable, int MissingCount)> pairs_Level = [];
                foreach (Variable variable in variables.Distinct())
                {
                    string reference = ((int)variable).ToString(System.Globalization.CultureInfo.CurrentCulture);
                    int heldCount_Level = heldCounts_ByLevel[unitLevel].TryGetValue(reference, out int heldCount) ? heldCount : 0;
                    bool isHeld_AnyLevel = heldCounts_ByLevel.Values.Any(x => x.ContainsKey(reference));
                    if (isHeld_AnyLevel && heldCount_Level == 0)
                    {
                        continue;
                    }

                    if (overrideExisting || heldCount_Level < unitCounts_ByLevel[unitLevel])
                    {
                        pairs_Level.Add((variable, unitCounts_ByLevel[unitLevel] - heldCount_Level));
                    }
                }

                // OrderByDescending is stable: equally missing variables keep the order they were given in.
                pairs.AddRange(pairs_Level.OrderByDescending(x => x.MissingCount).Select(x => (x.Variable, unitLevel)));
            }

            List<(Variable Variable, int UnitLevel)> failures = [];
            Dictionary<string, StatisticalDataCollection> statisticalDataCollections_Changed = [];
            int failureCount_Consecutive = 0;
            long count = 0;
            foreach ((Variable variable, int unitLevel) in pairs)
            {
                if (failureCount_Consecutive >= 3)
                {
                    failures.Add((variable, unitLevel));
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();

                Dictionary<string, UnitYearlyValues>? unitYearlyValuesDictionary = await unitYearlyValuesDictionaryFunc(variable, unitLevel);
                progress?.Report(++count);
                if (unitYearlyValuesDictionary == null)
                {
                    failures.Add((variable, unitLevel));
                    failureCount_Consecutive++;
                    continue;
                }

                failureCount_Consecutive = 0;
                foreach (KeyValuePair<string, UnitYearlyValues> keyValuePair in unitYearlyValuesDictionary)
                {
                    if (!units_ByCode.TryGetValue(keyValuePair.Key, out (int UnitLevel, StatisticalDataCollection? StatisticalDataCollection) unit) || unit.UnitLevel != unitLevel)
                    {
                        continue;
                    }

                    if (statisticalDataCollections_Changed.TryGetValue(keyValuePair.Key, out StatisticalDataCollection? statisticalDataCollection))
                    {
                        statisticalDataCollection.AddRange(keyValuePair.Value);
                    }
                    else if (unit.StatisticalDataCollection is StatisticalDataCollection statisticalDataCollection_Stored)
                    {
                        statisticalDataCollection_Stored.AddRange(keyValuePair.Value);
                        statisticalDataCollections_Changed[keyValuePair.Key] = statisticalDataCollection_Stored;
                    }
                    else if (keyValuePair.Value.ToDiGi() is StatisticalDataCollection statisticalDataCollection_New)
                    {
                        statisticalDataCollections_Changed[keyValuePair.Key] = statisticalDataCollection_New;
                    }
                }
            }

            if (statisticalDataCollections_Changed.Count > 0)
            {
                using StatisticalDataCollectionFile statisticalDataCollectionFile = new(path);
                foreach (StatisticalDataCollection statisticalDataCollection in statisticalDataCollections_Changed.Values)
                {
                    statisticalDataCollectionFile.AddValue(statisticalDataCollection);
                }

                if (!statisticalDataCollectionFile.Save())
                {
                    return (0, [.. pairs]);
                }
            }

            return (statisticalDataCollections_Changed.Count, failures);
        }
    }
}
