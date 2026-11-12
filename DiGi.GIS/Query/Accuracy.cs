using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Returns the overall accuracy of a classification: the share of items whose prediction equals the reference.
        /// <para>The two sequences are paired by position.</para>
        /// </summary>
        /// <typeparam name="TEnum">The class enumeration.</typeparam>
        /// <param name="references">The reference (true) class of each item.</param>
        /// <param name="predictions">The predicted class of each item, in the order of <paramref name="references"/>.</param>
        /// <returns>The accuracy in [0, 1], or null when either sequence is null or empty or their lengths differ.</returns>
        public static double? Accuracy<TEnum>(this IEnumerable<TEnum>? references, IEnumerable<TEnum>? predictions) where TEnum : struct, Enum
        {
            if (references is null || predictions is null)
            {
                return null;
            }

            List<TEnum> references_Temp = [.. references];
            List<TEnum> predictions_Temp = [.. predictions];
            if (references_Temp.Count == 0 || references_Temp.Count != predictions_Temp.Count)
            {
                return null;
            }

            EqualityComparer<TEnum> equalityComparer = EqualityComparer<TEnum>.Default;

            int count_Agreement = 0;
            for (int i = 0; i < references_Temp.Count; i++)
            {
                if (equalityComparer.Equals(references_Temp[i], predictions_Temp[i]))
                {
                    count_Agreement++;
                }
            }

            return (double)count_Agreement / references_Temp.Count;
        }
    }
}
