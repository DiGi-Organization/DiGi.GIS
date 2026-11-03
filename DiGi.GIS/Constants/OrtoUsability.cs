namespace DiGi.GIS.Constants
{
    /// <summary>
    /// Provides the weights and factor values of the orthophoto usability rubric (<see cref="Query.OrtoSheetUsability(Classes.OrtoSheet, double?)"/>).
    /// <para>Any change to a value here changes every score, so <see cref="Version"/> must be bumped with it — tables scored under different versions are not comparable. The <c>OrtoSheetUsability</c> fact pins the values against the version and fails when one moves alone.</para>
    /// </summary>
    public static class OrtoUsability
    {
        /// <summary>
        /// The version of the rubric the values below define.
        /// </summary>
        public const int Version = 1;

        /// <summary>
        /// The weight of the ground resolution factor.
        /// </summary>
        public const double ResolutionWeight = 0.35;

        /// <summary>
        /// The weight of the orthophoto type factor.
        /// </summary>
        public const double OrtoTypeWeight = 0.15;

        /// <summary>
        /// The weight of the colour mode factor.
        /// </summary>
        public const double ColorModeWeight = 0.15;

        /// <summary>
        /// The weight of the source (analog or digital photography) factor.
        /// </summary>
        public const double SourceWeight = 0.10;

        /// <summary>
        /// The weight of the clarity factor (one minus the measured obstacle fraction).
        /// </summary>
        public const double ClarityWeight = 0.25;

        /// <summary>
        /// The upper score of a sheet the index marks as not completely filled (<c>czy_ark_wypelniony</c> other than <c>TAK</c>).
        /// </summary>
        public const double SheetIncompleteCap = 0.5;

        /// <summary>
        /// The ground resolutions in metres at the knots of the resolution factor, finest first; paired with <see cref="ResolutionValues"/> and interpolated linearly between knots.
        /// </summary>
        public static readonly double[] ResolutionKnots = [0.10, 0.25, 0.50, 1.00, 2.00];

        /// <summary>
        /// The resolution factor at each of <see cref="ResolutionKnots"/>. Below about 0.5 m small structures and roof edges stop being separable.
        /// </summary>
        public static readonly double[] ResolutionValues = [1.00, 0.90, 0.60, 0.35, 0.15];

        /// <summary>
        /// The orthophoto type factor of a true orthophoto (roofs projected orthogonally).
        /// </summary>
        public const double TrueOrthoValue = 1.00;

        /// <summary>
        /// The orthophoto type factor of a standard orthophoto (radial roof offset of several metres for a 10 m building).
        /// </summary>
        public const double StandardOrthoValue = 0.70;

        /// <summary>
        /// The orthophoto type factor when the type is not known.
        /// </summary>
        public const double UndefinedOrthoValue = 0.50;

        /// <summary>
        /// The colour mode factor of colour infrared imagery.
        /// </summary>
        public const double CIRValue = 1.00;

        /// <summary>
        /// The colour mode factor of true colour imagery.
        /// </summary>
        public const double RGBValue = 0.90;

        /// <summary>
        /// The colour mode factor of black and white imagery (texture only).
        /// </summary>
        public const double BlackAndWhiteValue = 0.50;

        /// <summary>
        /// The colour mode factor when the colour mode is not known.
        /// </summary>
        public const double UndefinedColorModeValue = 0.60;

        /// <summary>
        /// The source factor of digital photography.
        /// </summary>
        public const double DigitalSourceValue = 1.00;

        /// <summary>
        /// The source factor of scanned analog photography (scan artefacts, radiometric drift, year-precision date).
        /// </summary>
        public const double AnalogSourceValue = 0.60;

        /// <summary>
        /// The source factor when the source is not recognised.
        /// </summary>
        public const double UndefinedSourceValue = 0.80;

        /// <summary>
        /// The clarity prior subtracted for an unprobed sheet acquired in a leaf-on month (April to October). A prior, not a measurement.
        /// </summary>
        public const double LeafOnPrior = 0.05;

        /// <summary>
        /// The clarity prior subtracted for an unprobed sheet acquired in a snow-risk month (December to February). A prior, not a measurement.
        /// </summary>
        public const double SnowRiskPrior = 0.05;
    }
}
