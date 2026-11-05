using DiGi.GIS.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts a point from geographic coordinates (EPSG:4326, longitude in X) to EPSG:2180 (easting in X).
        /// </summary>
        /// <remarks>
        /// This convenience overload builds the transformation on every call. A loop over many coordinates or footprints takes an <see cref="ICoordinateTransformation"/> from <see cref="Create.EPSG2180TransformationFromEPSG4326"/> and uses the polygonal overloads in this folder instead.
        /// </remarks>
        /// <param name="point3D">The geographic point.</param>
        /// <returns>The EPSG:2180 point, or <see langword="null"/> when the point is null, an ordinate is not finite, or the result falls outside the Polish domain.</returns>
        public static Point2D? ToEPSG2180(this Point3D? point3D)
        {
            if (point3D == null)
            {
                return null;
            }

            if (TransformOrdinates(Create.EPSG2180TransformationFromEPSG4326(), point3D.X, point3D.Y) is not double[] values)
            {
                return null;
            }

            return new Point2D(values[0], values[1]);
        }

        // Shared internals of the EPSG:2180 overloads in this folder. They are private to Convert, so they are not public members and do not get a file of their own.

        // One vertex: null rather than a partial answer, so a caller can never carry a half-reprojected ring.
        // A transformation that is not one of ours carries no domain, so only the finite guard applies to it.
        private static double[]? TransformOrdinates(ICoordinateTransformation? coordinateTransformation, double sourceX, double sourceY)
        {
            if (coordinateTransformation?.MathTransform is not IMathTransform mathTransform)
            {
                return null;
            }

            if (!IsFinite(sourceX) || !IsFinite(sourceY))
            {
                return null;
            }

            double[]? values = mathTransform.Transform([sourceX, sourceY]);
            if (values == null || values.Length < 2 || !IsFinite(values[0]) || !IsFinite(values[1]))
            {
                return null;
            }

            if (coordinateTransformation is EPSG2180Transformation epsg2180Transformation && !epsg2180Transformation.IsWithinDomain(values[0], values[1]))
            {
                return null;
            }

            return values;
        }

        // A ring for NetTopologySuite, which requires the closing vertex to repeat the first one: the closing pair is copied from the transformed first vertex rather than transformed again, so closure is exact and no vertex is projected twice.
        private static Coordinate[]? TransformRing(LineString? lineString, ICoordinateTransformation? coordinateTransformation)
        {
            if (lineString == null || lineString.IsEmpty)
            {
                return null;
            }

            Coordinate[] coordinates = lineString.Coordinates;
            if (coordinates == null || coordinates.Length < 4)
            {
                return null;
            }

            int vertexCount = coordinates.Length;
            if (coordinates[0].X == coordinates[vertexCount - 1].X && coordinates[0].Y == coordinates[vertexCount - 1].Y)
            {
                vertexCount--;
            }

            if (vertexCount < 3)
            {
                return null;
            }

            List<Coordinate> result = new(vertexCount + 1);
            for (int i = 0; i < vertexCount; i++)
            {
                if (TransformOrdinates(coordinateTransformation, coordinates[i].X, coordinates[i].Y) is not double[] values)
                {
                    return null;
                }

                result.Add(new Coordinate(values[0], values[1]));
            }

            result.Add(new Coordinate(result[0].X, result[0].Y));
            return [.. result];
        }

        // double.IsFinite is not available on netstandard2.0.
        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
