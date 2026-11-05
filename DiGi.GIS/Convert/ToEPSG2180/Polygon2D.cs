using DiGi.Geometry.Planar.Classes;
using GeoAPI.CoordinateSystems.Transformations;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Reprojects a closed curve to EPSG:2180.
        /// </summary>
        /// <remarks>
        /// DiGi rings are stored open, with no repeated closing vertex, unlike a NetTopologySuite <see cref="NetTopologySuite.Geometries.LinearRing"/>. The reprojected points therefore go through <c>DiGi.Geometry.Planar.Create.Polygon2D</c>, which is where a repeated closing point is removed and a ring with fewer than three distinct points is refused; the constructor would store whatever it is handed.
        /// </remarks>
        /// <param name="polygon2D">The source closed curve.</param>
        /// <param name="coordinateTransformation">A transformation from <see cref="Create.EPSG2180TransformationFromEPSG4326"/> or <see cref="Create.EPSG2180TransformationFromEPSG3035"/>.</param>
        /// <returns>The closed curve in EPSG:2180 with the same vertices, or <see langword="null"/>.</returns>
        public static Polygon2D? ToEPSG2180(this Polygon2D? polygon2D, ICoordinateTransformation? coordinateTransformation)
        {
            if (polygon2D == null)
            {
                return null;
            }

            if (polygon2D.GetPoints(false) is not List<Point2D> point2Ds || point2Ds.Count < 3)
            {
                return null;
            }

            List<Point2D?> point2Ds_EPSG2180 = new(point2Ds.Count);
            for (int i = 0; i < point2Ds.Count; i++)
            {
                if (point2Ds[i] == null)
                {
                    return null;
                }

                if (TransformOrdinates(coordinateTransformation, point2Ds[i].X, point2Ds[i].Y) is not double[] values)
                {
                    return null;
                }

                point2Ds_EPSG2180.Add(new Point2D(values[0], values[1]));
            }

            return DiGi.Geometry.Planar.Create.Polygon2D(point2Ds_EPSG2180);
        }
    }
}
