using System;
using System.Collections.Generic;

namespace DiGi.GIS
{
    public static partial class Query
    {
        /// <summary>
        /// Returns Cohen's kappa between two classifications of the same items: κ = (p_o − p_e) / (1 − p_e).
        /// <para>The two sequences are paired by position. p_o is the observed agreement and p_e the agreement expected by chance from the two marginal distributions. When p_e is 1 (both sides use one and the same class for every item) the agreement is complete and 1 is returned.</para>
        /// </summary>
        /// <typeparam name="TEnum">The class enumeration.</typeparam>
        /// <param name="references">The first classification (reference or first annotator) of each item.</param>
        /// <param name="predictions">The second classification (prediction or second annotator) of each item, in the order of <paramref name="references"/>.</param>
        /// <returns>Kappa in [-1, 1], or null when either sequence is null or empty or their lengths differ.</returns>
        public static double? CohenKappa<TEnum>(this IEnumerable<TEnum>? references, IEnumerable<TEnum>? predictions) where TEnum : struct, Enum
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

            Dictionary<TEnum, int> counts_Reference = [];
            Dictionary<TEnum, int> counts_Prediction = [];

            int count = references_Temp.Count;
            int count_Agreement = 0;
            for (int i = 0; i < count; i++)
            {
                TEnum reference = references_Temp[i];
                TEnum prediction = predictions_Temp[i];

                if (equalityComparer.Equals(reference, prediction))
                {
                    count_Agreement++;
                }

                counts_Reference.TryGetValue(reference, out int count_Reference);
                counts_Reference[reference] = count_Reference + 1;

                counts_Prediction.TryGetValue(prediction, out int count_Prediction);
                counts_Prediction[prediction] = count_Prediction + 1;
            }

            double observedAgreement = (double)count_Agreement / count;

            double expectedAgreement = 0;
            foreach (KeyValuePair<TEnum, int> keyValuePair in counts_Reference)
            {
                if (counts_Prediction.TryGetValue(keyValuePair.Key, out int count_Prediction))
                {
                    expectedAgreement += ((double)keyValuePair.Value / count) * ((double)count_Prediction / count);
                }
            }

            if (1 - expectedAgreement <= 0)
            {
                return 1;
            }

            return (observedAgreement - expectedAgreement) / (1 - expectedAgreement);
        }
    }
}
