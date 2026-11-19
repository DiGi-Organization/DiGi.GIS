namespace DiGi.GIS.Constants
{
    /// <summary>
    /// Provides the constants that bound a predicted year built (<see cref="Enums.YearBuiltSource.Prediction"/>).
    /// </summary>
    public static class YearBuiltPrediction
    {
        /// <summary>
        /// The first year the orthophoto sequence of the year built predictor covers.
        /// <para>The predictor answers the first year it confidently sees a building, so a building already standing in this year is predicted as this year: a prediction at or before it means "built in this year or earlier", not "built in this year" (DiGi-Organization/DiGi.GIS.Analytical#16). DiGi.GIS.ML dates its detection columns from it, so it has to move with the sequence.</para>
        /// </summary>
        public const short FirstYear = 2008;
    }
}
