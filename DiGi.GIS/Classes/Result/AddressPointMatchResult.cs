using DiGi.Core.Classes;
using DiGi.GIS.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents the assignment of a set of PRG address points to building footprints: one <see cref="AddressPointMatch"/> per address point, in input order, with the parameters the assignment used.
    /// <para>Created by <c>Create.AddressPointMatchResult</c>. Unmatched points are kept as <see cref="Enums.AddressPointMatchType.Unmatched"/> matches, never dropped.</para>
    /// </summary>
    public class AddressPointMatchResult : SerializableResult, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(AddressPointMatches))]
        private readonly List<AddressPointMatch>? addressPointMatches = null;

        [JsonInclude, JsonPropertyName(nameof(MaxDistance))]
        private readonly double maxDistance = 0;

        [JsonInclude, JsonPropertyName(nameof(Tolerance))]
        private readonly double tolerance = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPointMatchResult"/> class.
        /// <para>The constructor assigns the values and clones the matches, filtering nulls; the matching itself is done by <c>Create.AddressPointMatchResult</c>.</para>
        /// </summary>
        /// <param name="maxDistance">The maximum distance, in m, at which a point outside every footprint was still assigned to the nearest one.</param>
        /// <param name="tolerance">The distance tolerance, in m, used for the inside and boundary test and for ties.</param>
        /// <param name="addressPointMatches">The matches, one per address point. Each item is cloned and null items are skipped.</param>
        public AddressPointMatchResult(double maxDistance, double tolerance, IEnumerable<AddressPointMatch?>? addressPointMatches)
            : base()
        {
            this.maxDistance = maxDistance;
            this.tolerance = tolerance;

            if (addressPointMatches != null)
            {
                this.addressPointMatches = [];
                foreach (AddressPointMatch? addressPointMatch in addressPointMatches)
                {
                    if (Core.Query.Clone(addressPointMatch) is AddressPointMatch addressPointMatch_Temp)
                    {
                        this.addressPointMatches.Add(addressPointMatch_Temp);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPointMatchResult"/> class by copying an existing instance.
        /// </summary>
        /// <param name="addressPointMatchResult">The <see cref="AddressPointMatchResult"/> to copy.</param>
        public AddressPointMatchResult(AddressPointMatchResult? addressPointMatchResult)
            : base(addressPointMatchResult)
        {
            if (addressPointMatchResult != null)
            {
                maxDistance = addressPointMatchResult.maxDistance;
                tolerance = addressPointMatchResult.tolerance;

                if (addressPointMatchResult.addressPointMatches != null)
                {
                    addressPointMatches = [];
                    foreach (AddressPointMatch addressPointMatch in addressPointMatchResult.addressPointMatches)
                    {
                        if (Core.Query.Clone(addressPointMatch) is AddressPointMatch addressPointMatch_Temp)
                        {
                            addressPointMatches.Add(addressPointMatch_Temp);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPointMatchResult"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the result data.</param>
        public AddressPointMatchResult(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets clones of the matches, one per address point in input order, or null.
        /// </summary>
        [JsonIgnore]
        public List<AddressPointMatch>? AddressPointMatches
        {
            get
            {
                if (addressPointMatches is null)
                {
                    return null;
                }

                List<AddressPointMatch> result = [];
                foreach (AddressPointMatch addressPointMatch in addressPointMatches)
                {
                    if (Core.Query.Clone(addressPointMatch) is AddressPointMatch addressPointMatch_Temp)
                    {
                        result.Add(addressPointMatch_Temp);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the maximum distance, in m, at which a point outside every footprint was still assigned to the nearest one.
        /// </summary>
        [JsonIgnore]
        public double MaxDistance
        {
            get
            {
                return maxDistance;
            }
        }

        /// <summary>
        /// Gets the distance tolerance, in m, used for the inside and boundary test and for ties.
        /// </summary>
        [JsonIgnore]
        public double Tolerance
        {
            get
            {
                return tolerance;
            }
        }
    }
}
