using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Distributes the population of a subdivision over its buildings in proportion to their occupancy areas.
        /// <para>Each building receives <c>floor(occupancyArea × occupancy / Σ occupancyArea)</c>, raised to one only when the population is at least the number of buildings. An overshoot is removed one resident at a time from the buildings with the highest occupancy; a remainder is added one resident at a time to buildings drawn uniformly over all buildings (indices 0 … n − 1, with replacement) by a pseudo-random generator seeded with the number of buildings, so the result is reproducible for a given order of <paramref name="occupancyAreas"/>. The sum of the result always equals <paramref name="occupancy"/>.</para>
        /// </summary>
        /// <param name="occupancyAreas">The occupancy areas of the buildings (see <see cref="OccupancyArea"/>); every value must be positive.</param>
        /// <param name="occupancy">The population of the subdivision.</param>
        /// <returns>The occupancy of each building in the order of <paramref name="occupancyAreas"/>, or an empty list when there are no areas or their sum is not positive.</returns>
        public static List<uint> Occupancies(this IReadOnlyList<double>? occupancyAreas, uint occupancy)
        {
            List<uint> result = [];
            if (occupancyAreas == null || occupancyAreas.Count == 0)
            {
                return result;
            }

            double totalArea = 0;
            foreach (double occupancyArea in occupancyAreas)
            {
                totalArea += occupancyArea;
            }

            if (!(totalArea > 0))
            {
                return result;
            }

            long remainingOccupancy = occupancy;
            double occupancyPerArea = remainingOccupancy / totalArea;
            bool canEnforceMin1 = remainingOccupancy >= occupancyAreas.Count;

            List<Tuple<int, uint>> tuples = [];
            for (int i = 0; i < occupancyAreas.Count; i++)
            {
                uint occupancy_Building = (uint)Math.Floor(occupancyAreas[i] * occupancyPerArea);
                if (canEnforceMin1 && occupancy_Building == 0)
                {
                    occupancy_Building = 1;
                }

                tuples.Add(new Tuple<int, uint>(i, occupancy_Building));
                remainingOccupancy -= occupancy_Building;
            }

            if (remainingOccupancy < 0)
            {
                tuples.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                for (int i = 0; i < tuples.Count && remainingOccupancy < 0; i++)
                {
                    if (tuples[i].Item2 > 0)
                    {
                        tuples[i] = new Tuple<int, uint>(tuples[i].Item1, tuples[i].Item2 - 1);
                        remainingOccupancy++;
                    }
                }
            }

            if (remainingOccupancy > 0)
            {
                Random random = new(tuples.Count);

                while (remainingOccupancy > 0)
                {
                    // Random.Next(int) excludes its bound, so every index 0 … Count − 1 can be drawn (DiGi.GIS#13).
                    int index = random.Next(tuples.Count);
                    tuples[index] = new Tuple<int, uint>(tuples[index].Item1, tuples[index].Item2 + 1);
                    remainingOccupancy--;
                }
            }

            uint[] occupancies = new uint[tuples.Count];
            foreach (Tuple<int, uint> tuple in tuples)
            {
                occupancies[tuple.Item1] = tuple.Item2;
            }

            result.AddRange(occupancies);
            return result;
        }
    }
}
