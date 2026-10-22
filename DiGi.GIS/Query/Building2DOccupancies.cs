using DiGi.GIS.Classes;
using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Distributes the population of a subdivision over its occupied buildings in proportion to their occupancy areas (see <see cref="OccupancyArea"/> and <see cref="Occupancies(IReadOnlyList{double}, uint)"/>).
        /// <para>Buildings without an occupancy area are left out. The others are ordered by <see cref="GISGuidObject2D.Reference"/> (ordinal, a missing reference first) and then by <see cref="Core.Classes.GuidObject.Guid"/> before the distribution, so the result depends only on the buildings and not on the order in which they are passed or stored.</para>
        /// </summary>
        /// <param name="building2Ds">The buildings of the subdivision, in any order.</param>
        /// <param name="occupancy">The population of the subdivision.</param>
        /// <returns>One tuple per occupied building — the building, its occupancy area and its occupancy — in the distribution order, or an empty list when no building has an occupancy area.</returns>
        public static List<Tuple<Building2D, double, uint>> Building2DOccupancies(this IEnumerable<Building2D?>? building2Ds, uint occupancy)
        {
            List<Tuple<Building2D, double, uint>> result = [];
            if (building2Ds == null)
            {
                return result;
            }

            List<Tuple<Building2D, double>> tuples = [];
            foreach (Building2D? building2D in building2Ds)
            {
                if (building2D?.OccupancyArea() is double occupancyArea)
                {
                    tuples.Add(new Tuple<Building2D, double>(building2D, occupancyArea));
                }
            }

            tuples.Sort((x, y) =>
            {
                int comparison = string.CompareOrdinal(x.Item1.Reference, y.Item1.Reference);
                return comparison != 0 ? comparison : x.Item1.Guid.CompareTo(y.Item1.Guid);
            });

            List<uint> occupancies = tuples.ConvertAll(x => x.Item2).Occupancies(occupancy);
            if (occupancies.Count != tuples.Count)
            {
                return result;
            }

            for (int i = 0; i < tuples.Count; i++)
            {
                result.Add(new Tuple<Building2D, double, uint>(tuples[i].Item1, tuples[i].Item2, occupancies[i]));
            }

            return result;
        }
    }
}
