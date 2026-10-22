using DiGi.GIS.Classes;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Gets the occupancy area of a building: the footprint area multiplied by the number of storeys, with a building of zero storeys counted as one storey.
        /// </summary>
        /// <param name="building2D">The building to evaluate.</param>
        /// <returns>The occupancy area, or <c>null</c> when the building is not occupied (see <see cref="IsOccupied"/>), has no footprint, or its footprint area is not positive.</returns>
        public static double? OccupancyArea(this Building2D? building2D)
        {
            if (building2D == null || !building2D.IsOccupied())
            {
                return null;
            }

            if (building2D.PolygonalFace2D?.GetArea() is not double floorArea || !(floorArea > 0))
            {
                return null;
            }

            return floorArea * (building2D.Storeys is not > 0 ? 1 : building2D.Storeys);
        }
    }
}
