using DiGi.GIS.Constants;
using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems.Transformations;
using System;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// The Lambert Azimuthal Equal Area leg of the EPSG:3035 to EPSG:2180 conversion: it converts ETRS89-LAEA coordinates to geographic coordinates on the ellipsoid, then hands them to the ProjNet geographic to EPSG:2180 leg it wraps.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exists because ProjNET4GeoAPI 1.4.1 registers no lambert_azimuthal_equal_area projection, so <see cref="CoordinateTransformationFactory.CreateFromCoordinateSystems"/> throws <see cref="NotSupportedException"/> for EPSG:3035 even though the WKT of that coordinate system parses. The package is pinned at 1.4.1 across eight HintPath-consuming projects, so replacing it is a deployment change rather than a detail of this conversion, and registering a projection into the process-wide ProjectionsRegistry from a library is not acceptable either.
    /// </para>
    /// <para>
    /// The formulation is EPSG Guidance Note 7-2 method 9820 (Lambert Azimuthal Equal Area, ellipsoidal) in the oblique aspect with an azimuth of the central meridian of zero, as PROJ implements it: the ellipsoid is mapped onto the authalic sphere of radius a * sqrt(q_p / 2), the azimuthal formulae run there, and dd = cos(phi_0) / (sqrt(1 - e^2 sin^2 phi_0) * r_q * cos(beta_0)) restores unit scale at the projection centre.
    /// Where PROJ inverts the authalic latitude with a truncated series, this evaluates sin(beta) = q(phi) / q_p directly and inverts it by Newton iteration on dq/dphi = 2 (1 - e^2) cos(phi) / (1 - e^2 sin^2 phi)^2, which is exact to double precision.
    /// </para>
    /// <para>
    /// The sign convention for a latitude of origin south of the equator is not implemented: EPSG:3035 uses 52 N. Axes follow the DiGi convention - easting in index 0, northing in index 1 - not the northing-first order of the EPSG:3035 definition.
    /// A rejected coordinate pair yields an empty ordinate array rather than an exception, which is what lets the conversion overloads fail one footprint closed; the inherited TransformList therefore reports rejected points as empty arrays, and the inherited coordinate-based helpers see an array too short to read.
    /// </para>
    /// </remarks>
    internal sealed class LambertAzimuthalEqualAreaMathTransform : MathTransform
    {
        private const double DomainEpsilon = 1e-10;
        private const int MaximumAuthalicIterations = 12;
        private const double AuthalicTolerance = 1e-13;
        private const double AuthalicResidualTolerance = 1e-9;

        private readonly IMathTransform mathTransform;
        private readonly Func<bool, IMathTransform> legFactory;
        private readonly bool toLambertAzimuthalEqualArea;

        private readonly double semiMajorAxis;
        private readonly double eccentricity;
        private readonly double eccentricitySquared;
        private readonly double oneMinusEccentricitySquared;
        private readonly double maximumQ;
        private readonly double authalicRadius;
        private readonly double axisScale;
        private readonly double eastingScale;
        private readonly double northingScale;
        private readonly double sineOriginAuthalicLatitude;
        private readonly double cosineOriginAuthalicLatitude;
        private readonly double centralMeridianRadians;
        private readonly double latitudeOfOriginRadians;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambertAzimuthalEqualAreaMathTransform"/> class over a geographic to EPSG:2180 leg, or over its inverse.
        /// The ellipsoid constants are computed here because an instance is built once per run by the Create factory, never per feature.
        /// </summary>
        /// <param name="legFactory">Builds a fresh ProjNet leg on demand, asked by the direction of that leg: <see langword="true"/> for the geographic to EPSG:2180 leg, <see langword="false"/> for the leg that returns geographic ordinates from EPSG:2180 ones. A factory rather than a leg, because ProjNet's own <see cref="MathTransform.Inverse"/> reverses the steps of the instance it is called on and would corrupt a leg shared by both directions.</param>
        /// <param name="toLambertAzimuthalEqualArea">When <see langword="false"/>, the instance converts EPSG:3035 to EPSG:2180; when <see langword="true"/>, EPSG:2180 to EPSG:3035.</param>
        internal LambertAzimuthalEqualAreaMathTransform(Func<bool, IMathTransform> legFactory, bool toLambertAzimuthalEqualArea)
        {
            this.legFactory = legFactory;
            this.toLambertAzimuthalEqualArea = toLambertAzimuthalEqualArea;

            // The factory is asked by direction of the ProjNet leg, not by direction of this class: converting EPSG:3035 to EPSG:2180 ends in the leg that produces EPSG:2180 ordinates, and converting back starts in the leg that produces geographic ones.
            mathTransform = legFactory(!toLambertAzimuthalEqualArea);

            semiMajorAxis = CoordinateSystem.SemiMajorAxis;
            double inverseFlattening = 1.0 / CoordinateSystem.InverseFlattening;
            eccentricitySquared = 2.0 * inverseFlattening - inverseFlattening * inverseFlattening;
            eccentricity = Math.Sqrt(eccentricitySquared);
            oneMinusEccentricitySquared = 1.0 - eccentricitySquared;

            maximumQ = Q(1.0);
            authalicRadius = Math.Sqrt(0.5 * maximumQ);

            latitudeOfOriginRadians = DegreesToRadians(CoordinateSystem.LambertAzimuthalEqualAreaLatitudeOfOrigin);
            centralMeridianRadians = DegreesToRadians(CoordinateSystem.LambertAzimuthalEqualAreaCentralMeridian);

            double sineOriginLatitude = Math.Sin(latitudeOfOriginRadians);
            double originAuthalicLatitude = Math.Asin(Q(sineOriginLatitude) / maximumQ);
            sineOriginAuthalicLatitude = Math.Sin(originAuthalicLatitude);
            cosineOriginAuthalicLatitude = Math.Cos(originAuthalicLatitude);

            axisScale = Math.Cos(latitudeOfOriginRadians) / (Math.Sqrt(1.0 - eccentricitySquared * sineOriginLatitude * sineOriginLatitude) * authalicRadius * cosineOriginAuthalicLatitude);
            eastingScale = authalicRadius * axisScale;
            northingScale = authalicRadius / axisScale;
        }

        /// <summary>
        /// Gets the number of source ordinates, always two.
        /// </summary>
        public override int DimSource => 2;

        /// <summary>
        /// Gets the number of target ordinates, always two.
        /// </summary>
        public override int DimTarget => 2;

        /// <summary>
        /// Gets an empty string: this leg is not expressible in WKT, because the projection it implements is exactly the one ProjNET4GeoAPI 1.4.1 cannot describe.
        /// </summary>
        public override string WKT => string.Empty;

        /// <summary>
        /// Gets an empty string, for the reason given for <see cref="WKT"/>.
        /// </summary>
        public override string XML => string.Empty;

        /// <summary>
        /// Converts one coordinate pair, yielding an empty array when a value is not finite or the point lies outside the projection domain.
        /// </summary>
        /// <param name="point">The source ordinates, easting in index 0 and northing in index 1.</param>
        /// <returns>The converted easting and northing, or an empty array.</returns>
        public override double[] Transform(double[] point)
        {
            if (point == null || point.Length < 2)
            {
                return [];
            }

            if (!toLambertAzimuthalEqualArea)
            {
                if (!TryToGeographic(point[0], point[1], out double longitude, out double latitude))
                {
                    return [];
                }

                return mathTransform.Transform([longitude, latitude]);
            }

            double[]? geographicValues = mathTransform.Transform([point[0], point[1]]);
            if (geographicValues == null || geographicValues.Length < 2)
            {
                return [];
            }

            if (!TryToLambertAzimuthalEqualArea(geographicValues[0], geographicValues[1], out double easting, out double northing))
            {
                return [];
            }

            return [easting, northing];
        }

        /// <summary>
        /// Returns the opposite direction: EPSG:2180 to EPSG:3035 when this instance converts EPSG:3035 to EPSG:2180, and the reverse otherwise.
        /// The new instance takes a freshly built ProjNet leg, so this one keeps working.
        /// </summary>
        /// <returns>A new instance over its own leg.</returns>
        public override IMathTransform Inverse()
        {
            return new LambertAzimuthalEqualAreaMathTransform(legFactory, !toLambertAzimuthalEqualArea);
        }

        /// <summary>
        /// Not supported: this instance is immutable, so take the opposite direction from <see cref="Inverse"/> instead.
        /// </summary>
        public override void Invert()
        {
            throw new NotSupportedException("LambertAzimuthalEqualAreaMathTransform is immutable; use Inverse() to obtain the opposite direction.");
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        // q(phi) of the ellipsoid. The authalic latitude of phi is asin(q(phi) / q_p), with q(90 degrees) = q_p.
        private double Q(double sineLatitude)
        {
            return oneMinusEccentricitySquared * (sineLatitude / (1.0 - eccentricitySquared * sineLatitude * sineLatitude) - 0.5 / eccentricity * Math.Log((1.0 - eccentricity * sineLatitude) / (1.0 + eccentricity * sineLatitude)));
        }

        private bool TryToGeographic(double easting, double northing, out double longitude, out double latitude)
        {
            longitude = double.NaN;
            latitude = double.NaN;

            if (!IsFinite(easting) || !IsFinite(northing))
            {
                return false;
            }

            double x = (easting - CoordinateSystem.LambertAzimuthalEqualAreaFalseEasting) / semiMajorAxis / axisScale;
            double y = (northing - CoordinateSystem.LambertAzimuthalEqualAreaFalseNorthing) / semiMajorAxis * axisScale;

            double radius = Math.Sqrt(x * x + y * y);
            if (radius < DomainEpsilon)
            {
                longitude = CoordinateSystem.LambertAzimuthalEqualAreaCentralMeridian;
                latitude = CoordinateSystem.LambertAzimuthalEqualAreaLatitudeOfOrigin;
                return true;
            }

            double asinArgument = 0.5 * radius / authalicRadius;
            if (asinArgument > 1.0)
            {
                return false;
            }

            double sphericalDistance = 2.0 * Math.Asin(asinArgument);
            double cosineSphericalDistance = Math.Cos(sphericalDistance);
            double sineSphericalDistance = Math.Sin(sphericalDistance);

            double sineAuthalicLatitude = cosineSphericalDistance * sineOriginAuthalicLatitude + y * sineSphericalDistance * cosineOriginAuthalicLatitude / radius;
            if (sineAuthalicLatitude < -1.0 || sineAuthalicLatitude > 1.0)
            {
                return false;
            }

            double yComponent = radius * cosineOriginAuthalicLatitude * cosineSphericalDistance - y * sineOriginAuthalicLatitude * sineSphericalDistance;
            if (!TryLatitude(Math.Asin(sineAuthalicLatitude), out double latitudeRadians))
            {
                return false;
            }

            longitude = RadiansToDegrees(centralMeridianRadians + Math.Atan2(x * sineSphericalDistance, yComponent));
            latitude = RadiansToDegrees(latitudeRadians);
            return true;
        }

        private bool TryToLambertAzimuthalEqualArea(double longitude, double latitude, out double easting, out double northing)
        {
            easting = double.NaN;
            northing = double.NaN;

            if (!IsFinite(longitude) || !IsFinite(latitude))
            {
                return false;
            }

            double longitudeFromCentralMeridian = DegreesToRadians(longitude) - centralMeridianRadians;
            double latitudeRadians = DegreesToRadians(latitude);
            if (!TryAuthalicLatitude(Math.Sin(latitudeRadians), out double sineAuthalicLatitude, out double cosineAuthalicLatitude))
            {
                return false;
            }

            double cosineLongitude = Math.Cos(longitudeFromCentralMeridian);
            double denominator = 1.0 + sineOriginAuthalicLatitude * sineAuthalicLatitude + cosineOriginAuthalicLatitude * cosineAuthalicLatitude * cosineLongitude;
            if (denominator < DomainEpsilon)
            {
                return false;
            }

            double scale = Math.Sqrt(2.0 / denominator);
            double x = eastingScale * scale * cosineAuthalicLatitude * Math.Sin(longitudeFromCentralMeridian);
            double y = northingScale * scale * (cosineOriginAuthalicLatitude * sineAuthalicLatitude - sineOriginAuthalicLatitude * cosineAuthalicLatitude * cosineLongitude);

            easting = x * semiMajorAxis + CoordinateSystem.LambertAzimuthalEqualAreaFalseEasting;
            northing = y * semiMajorAxis + CoordinateSystem.LambertAzimuthalEqualAreaFalseNorthing;
            return IsFinite(easting) && IsFinite(northing);
        }

        private bool TryAuthalicLatitude(double sineLatitude, out double sineAuthalicLatitude, out double cosineAuthalicLatitude)
        {
            sineAuthalicLatitude = double.NaN;
            cosineAuthalicLatitude = double.NaN;

            if (sineLatitude < -1.0 || sineLatitude > 1.0)
            {
                return false;
            }

            sineAuthalicLatitude = Q(sineLatitude) / maximumQ;
            if (sineAuthalicLatitude < -1.0 || sineAuthalicLatitude > 1.0)
            {
                return false;
            }

            cosineAuthalicLatitude = Math.Sqrt(Math.Max(0.0, 1.0 - sineAuthalicLatitude * sineAuthalicLatitude));
            return true;
        }

        // Inverts sin(authalic latitude) = q(phi) / q_p by Newton iteration, starting from the authalic latitude itself.
        // Accepting on the residual rather than only on the step keeps a slow convergence from rejecting a valid point.
        private bool TryLatitude(double authalicLatitude, out double latitude)
        {
            latitude = double.NaN;

            double target = maximumQ * Math.Sin(authalicLatitude);
            double current = authalicLatitude;
            for (int i = 0; i < MaximumAuthalicIterations; i++)
            {
                double sineCurrent = Math.Sin(current);
                if (sineCurrent <= -1.0 || sineCurrent >= 1.0)
                {
                    return false;
                }

                double denominator = 1.0 - eccentricitySquared * sineCurrent * sineCurrent;
                double derivative = 2.0 * oneMinusEccentricitySquared * Math.Cos(current) / (denominator * denominator);
                if (!IsFinite(derivative) || Math.Abs(derivative) < 1e-12)
                {
                    return false;
                }

                double delta = (Q(sineCurrent) - target) / derivative;
                current -= delta;
                if (Math.Abs(delta) < AuthalicTolerance)
                {
                    latitude = current;
                    return true;
                }
            }

            double residual = Math.Abs(Q(Math.Sin(current)) - target);
            if (residual > AuthalicResidualTolerance)
            {
                return false;
            }

            latitude = current;
            return true;
        }
    }
}
