using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Returns the confusion matrix of a classification, keyed reference value then predicted value.
        /// <para>The two sequences are paired by position. Every value that occurs on either side becomes both a row and a column, and missing cells are present with a count of 0, so the matrix is square. Pairs are counted as given; filtering (for example of undefined labels) is left to the caller.</para>
        /// </summary>
        /// <typeparam name="TEnum">The class enumeration.</typeparam>
        /// <param name="references">The reference (true) class of each item.</param>
        /// <param name="predictions">The predicted class of each item, in the order of <paramref name="references"/>.</param>
        /// <returns>The counts keyed reference then prediction, or null when either sequence is null or their lengths differ.</returns>
        public static Dictionary<TEnum, Dictionary<TEnum, int>>? ConfusionMatrix<TEnum>(this IEnumerable<TEnum>? references, IEnumerable<TEnum>? predictions) where TEnum : struct, Enum
        {
            if (references is null || predictions is null)
            {
                return null;
            }

            List<TEnum> references_Temp = [.. references];
            List<TEnum> predictions_Temp = [.. predictions];
            if (references_Temp.Count != predictions_Temp.Count)
            {
                return null;
            }

            HashSet<TEnum> values = [.. references_Temp, .. predictions_Temp];

            Dictionary<TEnum, Dictionary<TEnum, int>> result = [];
            foreach (TEnum value_Reference in values)
            {
                Dictionary<TEnum, int> dictionary_Row = [];
                foreach (TEnum value_Prediction in values)
                {
                    dictionary_Row[value_Prediction] = 0;
                }

                result[value_Reference] = dictionary_Row;
            }

            for (int i = 0; i < references_Temp.Count; i++)
            {
                result[references_Temp[i]][predictions_Temp[i]]++;
            }

            return result;
        }
    }
}
