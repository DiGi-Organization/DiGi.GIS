using DiGi.GIS.Classes;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;

namespace DiGi.GIS
{
    public static partial class Create
    {
        /// <summary>
        /// Creates the transformation from EPSG:4326 (WGS 84, longitude in X) to EPSG:2180 (Poland CS92, easting in X).
        /// </summary>
        /// <remarks>
        /// Build it once and hold it: constructing a transformation builds the coordinate systems, parses the WKT and resolves the projection, which is the cost the <c>Convert.ToEPSG2180</c> overloads take it as a parameter to avoid paying per feature. Do not keep it in a static field - a run holds its own.
        /// No datum shift is applied, so EPSG:4326 coordinates are treated as ETRS89; in Poland the two differ by about a metre, and that difference is invisible to any check that runs both directions through the same library.
        /// The mathematics is a <see cref="DelegatingMathTransform"/>, not the ProjNet object itself, because calling <c>Inverse()</c> on a ProjNet leg corrupts its forward direction; see that class.
        /// </remarks>
        /// <returns>The transformation, or <see langword="null"/> when it could not be built.</returns>
        public static ICoordinateTransformation? EPSG2180TransformationFromEPSG4326()
        {
            try
            {
                DelegatingMathTransform mathTransform = new(EPSG2180LegFactory(), true);
                ICoordinateSystem coordinateSystem_EPSG4326 = EPSG4326GeographicCoordinateSystem();
                ICoordinateSystem coordinateSystem_EPSG2180 = EPSG2180CoordinateSystem();
                Envelope? domain = EPSG2180Transformation.CreateEPSG2180Domain();

                return new EPSG2180Transformation(coordinateSystem_EPSG4326, coordinateSystem_EPSG2180, mathTransform, "EPSG:4326 to EPSG:2180", "No datum shift is applied: EPSG:4326 coordinates are treated as ETRS89.", domain);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Builds a factory of the ProjNet leg between EPSG:4326 and EPSG:2180 in either direction.
        /// A factory, not a leg: ProjNET4GeoAPI 1.4.1 reverses the steps of a leg in place when asked for its inverse, so every direction needs a leg of its own.
        /// </summary>
        /// <returns>A function returning the source-to-target leg for <see langword="true"/> and the opposite leg for <see langword="false"/>.</returns>
        internal static Func<bool, IMathTransform> EPSG2180LegFactory()
        {
            return toEPSG2180 => toEPSG2180
                ? new CoordinateTransformationFactory().CreateFromCoordinateSystems(EPSG4326GeographicCoordinateSystem(), EPSG2180CoordinateSystem()).MathTransform
                : new CoordinateTransformationFactory().CreateFromCoordinateSystems(EPSG2180CoordinateSystem(), EPSG4326GeographicCoordinateSystem()).MathTransform;
        }

        internal static IGeographicCoordinateSystem EPSG4326GeographicCoordinateSystem()
        {
            return new CoordinateSystemFactory().CreateGeographicCoordinateSystem("WGS 84", AngularUnit.Degrees, HorizontalDatum.WGS84, PrimeMeridian.Greenwich, new AxisInfo("Longitude", AxisOrientationEnum.East), new AxisInfo("Latitude", AxisOrientationEnum.North));
        }

        internal static ICoordinateSystem EPSG2180CoordinateSystem()
        {
            return new CoordinateSystemFactory().CreateFromWkt(Constants.CoordinateSystem.EPSG2180Wkt);
        }
    }
}
