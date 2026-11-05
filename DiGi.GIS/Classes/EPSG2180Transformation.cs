using DiGi.GIS.Constants;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NetTopologySuite.Geometries;
using System;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// An EPSG:2180 coordinate transformation that also carries the Polish domain bounding box, so a conversion can refuse a footprint that landed outside the country instead of returning plausible nonsense.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="ProjNet.CoordinateSystems.Transformations.CoordinateTransformation"/> cannot be constructed by this library - its only constructor is internal in ProjNET4GeoAPI 1.4.1 - so the handle the caller holds is this type.
    /// It is internal, which keeps the stateful projection engine off the public anemic surface, and immutable: built once, safe to share across the threads of one run.
    /// </para>
    /// <para>
    /// A transformation taken from ProjNet directly, rather than from <see cref="Create.EPSG2180TransformationFromEPSG4326"/> or <see cref="Create.EPSG2180TransformationFromEPSG3035"/>, still converts; it simply carries no domain, so only the finite-ordinate guard applies to it.
    /// </para>
    /// </remarks>
    internal sealed class EPSG2180Transformation : ICoordinateTransformation
    {
        private readonly ICoordinateSystem sourceCS;
        private readonly ICoordinateSystem targetCS;
        private readonly IMathTransform mathTransform;
        private readonly string name;
        private readonly string remarks;
        private readonly Envelope? domain;

        internal EPSG2180Transformation(ICoordinateSystem sourceCS, ICoordinateSystem targetCS, IMathTransform mathTransform, string name, string remarks, Envelope? domain)
        {
            this.sourceCS = sourceCS;
            this.targetCS = targetCS;
            this.mathTransform = mathTransform;
            this.name = name;
            this.remarks = remarks;
            this.domain = domain;
        }

        /// <summary>
        /// Gets the area of use, empty as ProjNet itself reports it for a transformation built from two coordinate systems.
        /// </summary>
        public string AreaOfUse => string.Empty;

        /// <summary>
        /// Gets the authority of the target coordinate system.
        /// </summary>
        public string Authority => "EPSG";

        /// <summary>
        /// Gets the code of the target coordinate system, EPSG:2180.
        /// </summary>
        public long AuthorityCode => 2180;

        /// <summary>
        /// Gets the mathematics of the conversion: <see cref="LambertAzimuthalEqualAreaMathTransform"/> for an EPSG:3035 source, the ProjNet transverse Mercator leg for an EPSG:4326 source.
        /// </summary>
        public IMathTransform MathTransform => mathTransform;

        /// <summary>
        /// Gets the name of the conversion.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the remarks stating what the conversion does and does not do, in particular that no datum shift is applied.
        /// </summary>
        public string Remarks => remarks;

        /// <summary>
        /// Gets the source coordinate system.
        /// </summary>
        public ICoordinateSystem SourceCS => sourceCS;

        /// <summary>
        /// Gets the target coordinate system, EPSG:2180.
        /// </summary>
        public ICoordinateSystem TargetCS => targetCS;

        /// <summary>
        /// Gets the operation type: both supported sources share the ETRS89 datum with EPSG:2180, so this is a conversion, not a transformation.
        /// </summary>
        public TransformType TransformType => TransformType.Conversion;

        /// <summary>
        /// Determines whether EPSG:2180 coordinates fall inside the Polish domain; a transformation without a domain accepts every finite coordinate pair.
        /// </summary>
        /// <param name="easting">The EPSG:2180 easting.</param>
        /// <param name="northing">The EPSG:2180 northing.</param>
        /// <returns><see langword="true"/> when the pair is inside the domain or no domain is known.</returns>
        internal bool IsWithinDomain(double easting, double northing)
        {
            if (domain == null)
            {
                return true;
            }

            return easting >= domain.MinX && easting <= domain.MaxX && northing >= domain.MinY && northing <= domain.MaxY;
        }

        /// <summary>
        /// Derives the EPSG:2180 bounding box of the Polish domain by projecting the nine sample points of the geographic window - its four corners, the midpoints of its four edges and its centre - and padding the result by <see cref="CoordinateSystem.EPSG2180DomainMargin"/>.
        /// The box is derived rather than hard-coded, so the geographic window stays the only place the domain is stated.
        /// </summary>
        /// <remarks>
        /// The window is stated in degrees, so it is always projected by the geographic to EPSG:2180 leg, whatever the source of the transformation the domain is attached to: handing it the composed leg of an EPSG:3035 source would project 52 as though it were 52 metres and produce a box no footprint can be inside. The domain is a property of how Poland sits in EPSG:2180, so both sources share it.
        /// </remarks>
        /// <returns>An <see cref="Envelope"/> in EPSG:2180 metres, or <see langword="null"/> when a sample point cannot be projected.</returns>
        internal static Envelope? CreateEPSG2180Domain()
        {
            IMathTransform geographicToEPSG2180 = Create.EPSG2180LegFactory()(true);
            double[] longitudes = [CoordinateSystem.PolandMinimumLongitude, (CoordinateSystem.PolandMinimumLongitude + CoordinateSystem.PolandMaximumLongitude) / 2.0, CoordinateSystem.PolandMaximumLongitude];
            double[] latitudes = [CoordinateSystem.PolandMinimumLatitude, (CoordinateSystem.PolandMinimumLatitude + CoordinateSystem.PolandMaximumLatitude) / 2.0, CoordinateSystem.PolandMaximumLatitude];

            double minimumEasting = double.MaxValue;
            double maximumEasting = double.MinValue;
            double minimumNorthing = double.MaxValue;
            double maximumNorthing = double.MinValue;

            for (int i = 0; i < longitudes.Length; i++)
            {
                for (int j = 0; j < latitudes.Length; j++)
                {
                    double[]? values = geographicToEPSG2180.Transform([longitudes[i], latitudes[j]]);
                    if (values == null || values.Length < 2 || double.IsNaN(values[0]) || double.IsInfinity(values[0]) || double.IsNaN(values[1]) || double.IsInfinity(values[1]))
                    {
                        return null;
                    }

                    minimumEasting = Math.Min(minimumEasting, values[0]);
                    maximumEasting = Math.Max(maximumEasting, values[0]);
                    minimumNorthing = Math.Min(minimumNorthing, values[1]);
                    maximumNorthing = Math.Max(maximumNorthing, values[1]);
                }
            }

            return new Envelope(minimumEasting - CoordinateSystem.EPSG2180DomainMargin, maximumEasting + CoordinateSystem.EPSG2180DomainMargin, minimumNorthing - CoordinateSystem.EPSG2180DomainMargin, maximumNorthing + CoordinateSystem.EPSG2180DomainMargin);
        }
    }
}
