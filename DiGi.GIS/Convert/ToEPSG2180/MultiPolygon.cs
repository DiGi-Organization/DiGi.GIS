using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Reprojects every part of a multi polygon to EPSG:2180, keeping the part count and the ring count of every part.
        /// </summary>
        /// <param name="multiPolygon">The source multi polygon.</param>
        /// <param name="coordinateTransformation">A transformation from <see cref="Create.EPSG2180TransformationFromEPSG4326"/> or <see cref="Create.EPSG2180TransformationFromEPSG3035"/>.</param>
        /// <returns>The multi polygon in EPSG:2180, or <see langword="null"/> when any part cannot be reprojected.</returns>
        public static MultiPolygon? ToEPSG2180(this MultiPolygon? multiPolygon, ICoordinateTransformation? coordinateTransformation)
        {
            if (multiPolygon == null || multiPolygon.IsEmpty)
            {
                return null;
            }

            Polygon[] polygons_EPSG2180 = new Polygon[multiPolygon.NumGeometries];
            for (int i = 0; i < multiPolygon.NumGeometries; i++)
            {
                if (multiPolygon.GetGeometryN(i) is not Polygon polygon)
                {
                    return null;
                }

                if (ToEPSG2180(polygon, coordinateTransformation) is not Polygon polygon_EPSG2180)
                {
                    return null;
                }

                polygons_EPSG2180[i] = polygon_EPSG2180;
            }

            return multiPolygon.Factory.CreateMultiPolygon(polygons_EPSG2180);
        }
    }
}
