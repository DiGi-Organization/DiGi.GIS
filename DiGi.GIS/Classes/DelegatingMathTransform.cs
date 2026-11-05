using GeoAPI.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems.Transformations;
using System;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Hands a ProjNet mathematics object to a caller without handing over the object ProjNet damages when its direction is reversed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ProjNET4GeoAPI 1.4.1 <c>ConcatenatedTransform.Inverse()</c> reverses the list of steps inside the instance it is called on instead of copying it, so the forward direction of a held transformation stops working afterwards and returns nonsense without reporting anything: on this machine, transforming the Warsaw pair through a leg that had once been passed to <see cref="Inverse"/> produced (12.335876698571296, 47.6742301256117) in place of (630388.5747769627, 489307.7984585427).
    /// A conversion holds its transformation for a whole run, so that is silent corruption of every feature converted after the first call to <see cref="Inverse"/>.
    /// </para>
    /// <para>
    /// This wrapper therefore keeps the direction of a leg fixed and builds a fresh, correctly oriented leg whenever the opposite direction is asked for, which is why the Create methods hand over a factory rather than a leg. <see cref="Invert"/> is refused outright: it would reverse the wrapped leg in place for the same reason.
    /// </para>
    /// </remarks>
    internal sealed class DelegatingMathTransform : MathTransform
    {
        private readonly IMathTransform mathTransform;
        private readonly Func<bool, IMathTransform> legFactory;
        private readonly bool toTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingMathTransform"/> class, building the leg for the requested direction once.
        /// </summary>
        /// <param name="legFactory">Builds a fresh ProjNet leg; <see langword="true"/> asks for the source-to-target direction, <see langword="false"/> for the opposite one.</param>
        /// <param name="toTarget">The direction of this instance.</param>
        internal DelegatingMathTransform(Func<bool, IMathTransform> legFactory, bool toTarget)
        {
            this.legFactory = legFactory;
            this.toTarget = toTarget;
            mathTransform = legFactory(toTarget);
        }

        /// <summary>
        /// Gets the number of source ordinates of the wrapped leg.
        /// </summary>
        public override int DimSource => mathTransform.DimSource;

        /// <summary>
        /// Gets the number of target ordinates of the wrapped leg.
        /// </summary>
        public override int DimTarget => mathTransform.DimTarget;

        /// <summary>
        /// Gets the well-known text of the wrapped leg.
        /// </summary>
        public override string WKT => mathTransform.WKT;

        /// <summary>
        /// Gets the XML form of the wrapped leg.
        /// </summary>
        public override string XML => mathTransform.XML;

        /// <summary>
        /// Converts one coordinate pair through the wrapped leg.
        /// </summary>
        /// <param name="point">The source ordinates.</param>
        /// <returns>The converted ordinates.</returns>
        public override double[] Transform(double[] point)
        {
            return mathTransform.Transform(point);
        }

        /// <summary>
        /// Returns a new instance over a freshly built leg in the opposite direction, leaving this instance and the leg it wraps untouched.
        /// </summary>
        /// <returns>The opposite direction.</returns>
        public override IMathTransform Inverse()
        {
            return new DelegatingMathTransform(legFactory, !toTarget);
        }

        /// <summary>
        /// Not supported: reversing in place is exactly what corrupts a held ProjNet leg.
        /// </summary>
        public override void Invert()
        {
            throw new NotSupportedException("DelegatingMathTransform is immutable; use Inverse() to obtain the opposite direction.");
        }
    }
}
