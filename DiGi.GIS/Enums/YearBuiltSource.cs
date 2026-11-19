using System.ComponentModel;

namespace DiGi.GIS.Enums
{
    /// <summary>
    /// Specifies the source from which the year built information was obtained.
    /// </summary>
    public enum YearBuiltSource
    {
        /// <summary>
        /// Indicates that the year built was provided by a user.
        /// </summary>
        [Description("User")] User,

        /// <summary>
        /// Indicates that the year built was generated via a prediction model.
        /// </summary>
        [Description("Prediction")] Prediction,

        /// <summary>
        /// Indicates that the year built was obtained from another source.
        /// </summary>
        [Description("Other")] Other,

        /// <summary>
        /// Indicates that the year built is a stated fallback standing in for a year no source gives, such as a prediction of <see cref="Constants.YearBuiltPrediction.FirstYear"/>, which only bounds the year (DiGi-Organization/DiGi.GIS.Analytical#16).
        /// </summary>
        [Description("Fallback")] Fallback,

        /// <summary>
        /// Indicates that the year built is the census year built of the building's area (<see cref="Query.CensusYearBuilt"/>), standing in for a year no source gives, such as a prediction of <see cref="Constants.YearBuiltPrediction.FirstYear"/> (DiGi-Organization/DiGi.GIS.Analytical#17).
        /// </summary>
        [Description("Census")] Census,
    }
}