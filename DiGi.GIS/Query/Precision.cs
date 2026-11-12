using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Returns the precision of one class: of the items predicted as <paramref name="value"/>, the share whose reference is also <paramref name="value"/>.
        /// <para>The two sequences are paired by position.</para>
        /// </summary>
        /// <typeparam name="TEnum">The class enumeration.</typeparam>
        /// <param name="references">The reference (true) class of each item.</param>
        /// <param name="predictions">The predicted class of each item, in the order of <paramref name="references"/>.</param>
        /// <param name="value">The class to measure.</param>
        /// <returns>The precision in [0, 1], or null when either sequence is null, their lengths differ, or nothing was predicted as <paramref name="value"/>.</returns>
        public static double? Precision<TEnum>(this IEnumerable<TEnum>? references, IEnumerable<TEnum>? predictions, TEnum value) where TEnum : struct, Enum
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

            EqualityComparer<TEnum> equalityComparer = EqualityComparer<TEnum>.Default;

            int count_Predicted = 0;
            int count_TruePositive = 0;
            for (int i = 0; i < predictions_Temp.Count; i++)
            {
                if (!equalityComparer.Equals(predictions_Temp[i], value))
                {
                    continue;
                }

                count_Predicted++;
                if (equalityComparer.Equals(references_Temp[i], value))
                {
                    count_TruePositive++;
                }
            }

            if (count_Predicted == 0)
            {
                return null;
            }

            return (double)count_TruePositive / count_Predicted;
        }
    }
}
