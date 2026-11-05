using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Spatial.Classes;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts from EPSG:2180 to EPSG:4326 Coordinate System
        /// </summary>
        /// <param name="point2D">Point2D in EPSG:2180 Coordinate System</param>
        /// <returns>Point3D in EPSG:4326 Coordinate System</returns>
        public static Point3D? ToEPSG4326(this Point2D? point2D)
        {
            if (point2D is null)
            {
                return null;
            }

            CoordinateSystemFactory coordinateSystemFactory = new();
            ICoordinateSystem coordinateSystem_EPSG2180 = coordinateSystemFactory.CreateFromWkt(Constants.CoordinateSystem.EPSG2180Wkt);

            IGeographicCoordinateSystem geographicCoordinateSystem_EPSG4326 = coordinateSystemFactory.CreateGeographicCoordinateSystem("WGS 84", AngularUnit.Degrees, HorizontalDatum.WGS84, PrimeMeridian.Greenwich, new AxisInfo("Longitude", AxisOrientationEnum.East), new AxisInfo("Latitude", AxisOrientationEnum.North));

            CoordinateTransformationFactory coordinateTransformationFactory = new();
            ICoordinateTransformation coordinateTransformation = coordinateTransformationFactory.CreateFromCoordinateSystems(coordinateSystem_EPSG2180, geographicCoordinateSystem_EPSG4326);

            double[] values = coordinateTransformation.MathTransform.Transform([point2D.X, point2D.Y]);
            if (values == null)
            {
                return null;
            }

            return new Point3D(values[0], values[1], values[2]);
        }
    }
}