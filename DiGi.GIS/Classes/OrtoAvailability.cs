using DiGi.Core.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents one cell of an orthophoto availability matrix: the distinct epochs observed at one location and the gaps between them.
    /// <para>The first epoch is left censored (nothing earlier is known, which is not the same as nothing earlier existing) and the last is right censored at <see cref="ProbeDate"/>.</para>
    /// <para>Build with <see cref="Create.OrtoAvailabilityMatrix(System.Collections.Generic.IEnumerable{OrtoSheet?}?, double, double, DateTimeOffset?)"/>, which deduplicates the sheets and computes the gaps; this constructor stores what it is given.</para>
    /// </summary>
    public class OrtoAvailability : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(BoundingBox2D))]
        private readonly BoundingBox2D? boundingBox2D;

        [JsonInclude, JsonPropertyName(nameof(GapDays))]
        private readonly List<int>? gapDays;

        [JsonInclude, JsonPropertyName(nameof(LocationKey))]
        private readonly string? locationKey;

        [JsonInclude, JsonPropertyName(nameof(OrtoEpochs))]
        private readonly List<OrtoEpoch>? ortoEpochs;

        [JsonInclude, JsonPropertyName(nameof(ProbeDate))]
        private readonly DateTimeOffset? probeDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoAvailability"/> class.
        /// </summary>
        /// <param name="locationKey">The culture-invariant key of the location (<see cref="Query.OrtoLocationKey(BoundingBox2D?)"/>).</param>
        /// <param name="boundingBox2D">The location in EPSG:2180, easting in X.</param>
        /// <param name="ortoEpochs">The distinct epochs in date order.</param>
        /// <param name="gapDays">The whole days between each pair of consecutive epochs; one element fewer than <paramref name="ortoEpochs"/>.</param>
        /// <param name="probeDate">The moment the index was read, at which the last epoch is right censored.</param>
        public OrtoAvailability(string? locationKey, BoundingBox2D? boundingBox2D, IEnumerable<OrtoEpoch>? ortoEpochs, IEnumerable<int>? gapDays, DateTimeOffset? probeDate)
        {
            this.locationKey = locationKey;
            this.boundingBox2D = Core.Query.Clone(boundingBox2D);
            this.ortoEpochs = Clone(ortoEpochs);
            this.gapDays = gapDays == null ? null : new List<int>(gapDays);
            this.probeDate = probeDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoAvailability"/> class by copying an existing instance.
        /// </summary>
        /// <param name="ortoAvailability">The source cell to copy from.</param>
        public OrtoAvailability(OrtoAvailability? ortoAvailability)
            : base(ortoAvailability)
        {
            if (ortoAvailability != null)
            {
                locationKey = ortoAvailability.locationKey;
                boundingBox2D = Core.Query.Clone(ortoAvailability.boundingBox2D);
                ortoEpochs = Clone(ortoAvailability.ortoEpochs);
                gapDays = ortoAvailability.gapDays == null ? null : new List<int>(ortoAvailability.gapDays);
                probeDate = ortoAvailability.probeDate;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoAvailability"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the cell data.</param>
        public OrtoAvailability(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets a clone of the location in EPSG:2180, easting in X.
        /// </summary>
        [JsonIgnore]
        public BoundingBox2D? BoundingBox2D
        {
            get
            {
                return Core.Query.Clone(boundingBox2D);
            }
        }

        /// <summary>
        /// Gets a copy of the whole days between each pair of consecutive epochs.
        /// </summary>
        [JsonIgnore]
        public List<int>? GapDays
        {
            get
            {
                return gapDays == null ? null : new List<int>(gapDays);
            }
        }

        /// <summary>
        /// Gets the culture-invariant key of the location.
        /// </summary>
        [JsonIgnore]
        public string? LocationKey
        {
            get
            {
                return locationKey;
            }
        }

        /// <summary>
        /// Gets clones of the distinct epochs in date order.
        /// </summary>
        [JsonIgnore]
        public List<OrtoEpoch>? OrtoEpochs
        {
            get
            {
                return Clone(ortoEpochs);
            }
        }

        /// <summary>
        /// Gets the moment the index was read, at which the last epoch is right censored.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? ProbeDate
        {
            get
            {
                return probeDate;
            }
        }

        private static List<OrtoEpoch>? Clone(IEnumerable<OrtoEpoch?>? ortoEpochs)
        {
            if (ortoEpochs == null)
            {
                return null;
            }

            List<OrtoEpoch> result = [];
            foreach (OrtoEpoch? ortoEpoch in ortoEpochs)
            {
                if (Core.Query.Clone(ortoEpoch) is OrtoEpoch ortoEpoch_Temp)
                {
                    result.Add(ortoEpoch_Temp);
                }
            }

            return result;
        }
    }
}
