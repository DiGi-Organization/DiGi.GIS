using DiGi.GIS.Classes;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using System;

namespace DiGi.GIS
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the transformation from EPSG:3035 (ETRS89 / LAEA Europe, easting in X) to EPSG:2180 (Poland CS92, easting in X).
        /// </summary>
        /// <remarks>
        /// ProjNET4GeoAPI 1.4.1 implements no Lambert Azimuthal Equal Area projection, so this is not a ProjNet transformation: the LAEA leg is computed by <see cref="LambertAzimuthalEqualAreaMathTransform"/> and its geographic output is handed to the ProjNet geographic to EPSG:2180 leg, which is the same leg <see cref="EPSG2180TransformationFromEPSG4326"/> uses. Both coordinate systems are ETRS89, so no datum shift is involved in either leg.
        /// The geographic to EPSG:2180 leg is rebuilt whenever a direction is reversed, so taking <see cref="GeoAPI.CoordinateSystems.Transformations.IMathTransform.Inverse"/> leaves this transformation usable.
        /// </remarks>
        /// <returns>The transformation, or <see langword="null"/> when it could not be built.</returns>
        public static ICoordinateTransformation? EPSG2180TransformationFromEPSG3035()
        {
            try
            {
                LambertAzimuthalEqualAreaMathTransform mathTransform = new(EPSG2180LegFactory(), false);
                ICoordinateSystem coordinateSystem_EPSG3035 = new CoordinateSystemFactory().CreateFromWkt(Constants.CoordinateSystem.EPSG3035Wkt);
                ICoordinateSystem coordinateSystem_EPSG2180 = EPSG2180CoordinateSystem();
                Envelope? domain = EPSG2180Transformation.CreateEPSG2180Domain();

                return new EPSG2180Transformation(coordinateSystem_EPSG3035, coordinateSystem_EPSG2180, mathTransform, "EPSG:3035 to EPSG:2180", "Single datum (ETRS89); the Lambert Azimuthal Equal Area leg is computed by this library because ProjNET4GeoAPI 1.4.1 does not implement it.", domain);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
