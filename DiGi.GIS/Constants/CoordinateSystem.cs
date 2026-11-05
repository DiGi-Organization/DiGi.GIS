namespace DiGi.GIS.Constants
{
    /// <summary>
    /// Provides the coordinate system definitions and the projection parameters shared by the EPSG conversions of this library.
    /// </summary>
    public static class CoordinateSystem
    {
        /// <summary>
        /// The OGC WKT definition of EPSG:2180 (ETRS89 / Poland CS92). Single source of truth for both directions, so they cannot drift apart.
        /// </summary>
        public const string EPSG2180Wkt = "PROJCS[\"ETRS89 / Poland CS92\",GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4258\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",19],PARAMETER[\"scale_factor\",0.9993],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",-5300000],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],AUTHORITY[\"EPSG\",\"2180\"]]";

        /// <summary>
        /// The OGC WKT definition of EPSG:3035 (ETRS89 / LAEA Europe). It names the source system reported by Create.EPSG2180TransformationFromEPSG3035; it is never handed to ProjNet to build a transformation, because ProjNET4GeoAPI 1.4.1 registers no lambert_azimuthal_equal_area projection and CreateFromCoordinateSystems throws NotSupportedException for it.
        /// </summary>
        public const string EPSG3035Wkt = "PROJCS[\"ETRS89 / LAEA Europe\",GEOGCS[\"ETRS89\",DATUM[\"European_Terrestrial_Reference_System_1989\",SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],AUTHORITY[\"EPSG\",\"6258\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4258\"]],PROJECTION[\"Lambert_Azimuthal_Equal_Area\"],PARAMETER[\"latitude_of_center\",52],PARAMETER[\"longitude_of_center\",10],PARAMETER[\"false_easting\",4321000],PARAMETER[\"false_northing\",3210000],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Northing\",NORTH],AXIS[\"Easting\",EAST],AUTHORITY[\"EPSG\",\"3035\"]]";

        /// <summary>
        /// The semi-major axis of the GRS 1980 ellipsoid in metres.
        /// </summary>
        public const double SemiMajorAxis = 6378137.0;

        /// <summary>
        /// The inverse flattening of the GRS 1980 ellipsoid (EPSG:7019).
        /// </summary>
        public const double InverseFlattening = 298.257222101;

        /// <summary>
        /// The latitude of the EPSG:3035 projection centre in degrees.
        /// </summary>
        public const double LambertAzimuthalEqualAreaLatitudeOfOrigin = 52.0;

        /// <summary>
        /// The longitude of the EPSG:3035 projection centre in degrees.
        /// </summary>
        public const double LambertAzimuthalEqualAreaCentralMeridian = 10.0;

        /// <summary>
        /// The false easting of EPSG:3035 in metres.
        /// </summary>
        public const double LambertAzimuthalEqualAreaFalseEasting = 4321000.0;

        /// <summary>
        /// The false northing of EPSG:3035 in metres.
        /// </summary>
        public const double LambertAzimuthalEqualAreaFalseNorthing = 3210000.0;

        /// <summary>
        /// The western limit, in degrees east, of the geographic window bounding the Polish domain. Mainland Poland spans 14.12 E to 24.15 E and 49.00 N to 54.85 N; about a degree of slack keeps a footprint at the border inside the domain instead of clipping it.
        /// </summary>
        public const double PolandMinimumLongitude = 13.0;

        /// <summary>
        /// The eastern limit, in degrees east, of the geographic window bounding the Polish domain.
        /// </summary>
        public const double PolandMaximumLongitude = 25.5;

        /// <summary>
        /// The southern limit, in degrees north, of the geographic window bounding the Polish domain.
        /// </summary>
        public const double PolandMinimumLatitude = 48.5;

        /// <summary>
        /// The northern limit, in degrees north, of the geographic window bounding the Polish domain.
        /// </summary>
        public const double PolandMaximumLatitude = 55.5;

        /// <summary>
        /// The slack in metres added around the EPSG:2180 box derived from the Polish geographic window.
        /// </summary>
        public const double EPSG2180DomainMargin = 25000.0;
    }
}
