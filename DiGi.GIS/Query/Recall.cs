using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Returns the recall of one class: of the items whose reference is <paramref name="value"/>, the share also predicted as <paramref name="value"/>.
        /// <para>The two sequences are paired by position.</para>
        /// </summary>
        /// <typeparam name="TEnum">The class enumeration.</typeparam>
        /// <param name="references">The reference (true) class of each item.</param>
        /// <param name="predictions">The predicted class of each item, in the order of <paramref name="references"/>.</param>
        /// <param name="value">The class to measure.</param>
        /// <returns>The recall in [0, 1], or null when either sequence is null, their lengths differ, or no reference is <paramref name="value"/>.</returns>
        public static double? Recall<TEnum>(this IEnumerable<TEnum>? references, IEnumerable<TEnum>? predictions, TEnum value) where TEnum : struct, Enum
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

            int count_Reference = 0;
            int count_TruePositive = 0;
            for (int i = 0; i < references_Temp.Count; i++)
            {
                if (!equalityComparer.Equals(references_Temp[i], value))
                {
                    continue;
                }

                count_Reference++;
                if (equalityComparer.Equals(predictions_Temp[i], value))
                {
                    count_TruePositive++;
                }
            }

            if (count_Reference == 0)
            {
                return null;
            }

            return (double)count_TruePositive / count_Reference;
        }
    }
}
