using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Reprojects a polygon to EPSG:2180 using a transformation the caller built once and holds.
        /// </summary>
        /// <remarks>
        /// The footprint is the unit of failure: one non-finite vertex, a ring shorter than three distinct vertices, or a vertex that lands outside the Polish domain voids the whole polygon, because a ring with some vertices reprojected is worse than no ring - it still counts as a feature downstream. Rings are rebuilt through the factory of the input geometry, so precision model and SRID survive, and every ring is closed by copying the transformed first vertex.
        /// </remarks>
        /// <param name="polygon">The source polygon.</param>
        /// <param name="coordinateTransformation">A transformation from <see cref="Create.EPSG2180TransformationFromEPSG4326"/> or <see cref="Create.EPSG2180TransformationFromEPSG3035"/>.</param>
        /// <returns>The polygon in EPSG:2180 with the same number of rings, or <see langword="null"/>.</returns>
        public static Polygon? ToEPSG2180(this Polygon? polygon, ICoordinateTransformation? coordinateTransformation)
        {
            if (polygon == null || polygon.IsEmpty)
            {
                return null;
            }

            if (TransformRing(polygon.ExteriorRing, coordinateTransformation) is not Coordinate[] exteriorCoordinates)
            {
                return null;
            }

            if (polygon.Factory.CreateLinearRing(exteriorCoordinates) is not LinearRing linearRing_ExteriorRing)
            {
                return null;
            }

            LineString[] interiorRings = polygon.InteriorRings;
            if (interiorRings == null || interiorRings.Length == 0)
            {
                return polygon.Factory.CreatePolygon(linearRing_ExteriorRing, null);
            }

            List<LinearRing> linearRings_InteriorRings = [];
            foreach (LineString lineString in interiorRings)
            {
                if (TransformRing(lineString, coordinateTransformation) is not Coordinate[] coordinates)
                {
                    return null;
                }

                if (polygon.Factory.CreateLinearRing(coordinates) is not LinearRing linearRing)
                {
                    return null;
                }

                linearRings_InteriorRings.Add(linearRing);
            }

            return polygon.Factory.CreatePolygon(linearRing_ExteriorRing, [.. linearRings_InteriorRings]);
        }
    }
}
