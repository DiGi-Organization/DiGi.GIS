using DiGi.Core.Classes;
using DiGi.GIS.Enums;
using DiGi.GIS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents the stored solar radiation of one building: one <see cref="SurfaceSolarRadiation"/> per receiving surface, keyed by the solved building model, the neighbour radius and the solver version.
    /// <para>The key is <c>(CountyId, BuildingModelId, Radius, SolverVersion)</c>. The weather members (<see cref="WeatherSourceType"/>, <see cref="WeatherSourceId"/>, <see cref="WeatherPeriodStart"/>, <see cref="WeatherPeriodEnd"/>) describe the stored summary values only, not the key: the hourly shading factors are weather-independent, so a summary for another weather source is rebuilt from them without a new solve.</para>
    /// </summary>
    public class BuildingSolarRadiationResult : SerializableResult, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(BuildingModelId))]
        private readonly long buildingModelId = 0;

        [JsonInclude, JsonPropertyName(nameof(CountyId))]
        private readonly int countyId = 0;

        [JsonInclude, JsonPropertyName(nameof(CreatedAt))]
        private readonly DateTimeOffset? createdAt = null;

        [JsonInclude, JsonPropertyName(nameof(Radius))]
        private readonly double radius = 0;

        [JsonInclude, JsonPropertyName(nameof(Reference))]
        private readonly string? reference = null;

        [JsonInclude, JsonPropertyName(nameof(SolverVersion))]
        private readonly int solverVersion = 0;

        [JsonInclude, JsonPropertyName(nameof(SurfaceSolarRadiations))]
        private readonly List<SurfaceSolarRadiation>? surfaceSolarRadiations = null;

        [JsonInclude, JsonPropertyName(nameof(UserEmail))]
        private readonly string? userEmail = null;

        [JsonInclude, JsonPropertyName(nameof(WeatherPeriodEnd))]
        private readonly DateTimeOffset? weatherPeriodEnd = null;

        [JsonInclude, JsonPropertyName(nameof(WeatherPeriodStart))]
        private readonly DateTimeOffset? weatherPeriodStart = null;

        [JsonInclude, JsonPropertyName(nameof(WeatherSourceId))]
        private readonly string? weatherSourceId = null;

        [JsonInclude, JsonPropertyName(nameof(WeatherSourceType))]
        private readonly WeatherSourceType weatherSourceType = WeatherSourceType.Undefined;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingSolarRadiationResult"/> class.
        /// <para>The constructor assigns the values and clones the surfaces, filtering nulls; it does not validate them. The write endpoint validates a result before storing it.</para>
        /// </summary>
        /// <param name="reference">The <c>building_2d</c> reference of the building.</param>
        /// <param name="countyId">The id of the county part the building is filed under.</param>
        /// <param name="buildingModelId">The id of the solved building model; a regenerated model has a new id.</param>
        /// <param name="radius">The neighbour radius, in m, as rounded by the producer (0.1 m) so that it compares exactly.</param>
        /// <param name="solverVersion">The version of the solver, its options and the aggregation that produced the shading factors.</param>
        /// <param name="weatherSourceType">The kind of weather data the summary values were calculated from.</param>
        /// <param name="weatherSourceId">The id of the weather source, as an invariant string: the <c>epw_file</c> id for <see cref="WeatherSourceType.EPW"/>, the provider or station id for <see cref="WeatherSourceType.Live"/>.</param>
        /// <param name="weatherPeriodStart">The start of the weather period the summary covers, or null together with <paramref name="weatherPeriodEnd"/> for a typical year.</param>
        /// <param name="weatherPeriodEnd">The end of the weather period the summary covers, or null together with <paramref name="weatherPeriodStart"/> for a typical year.</param>
        /// <param name="userEmail">The email of the signed-in user who stored the result. The server sets it from the user's token, never from a client.</param>
        /// <param name="createdAt">When the result was stored.</param>
        /// <param name="surfaceSolarRadiations">The stored radiation of each receiving surface. Each item is cloned and null items are skipped.</param>
        public BuildingSolarRadiationResult(
            string? reference,
            int countyId,
            long buildingModelId,
            double radius,
            int solverVersion,
            WeatherSourceType weatherSourceType,
            string? weatherSourceId,
            DateTimeOffset? weatherPeriodStart,
            DateTimeOffset? weatherPeriodEnd,
            string? userEmail,
            DateTimeOffset? createdAt,
            IEnumerable<SurfaceSolarRadiation?>? surfaceSolarRadiations)
            : base()
        {
            this.reference = reference;
            this.countyId = countyId;
            this.buildingModelId = buildingModelId;
            this.radius = radius;
            this.solverVersion = solverVersion;
            this.weatherSourceType = weatherSourceType;
            this.weatherSourceId = weatherSourceId;
            this.weatherPeriodStart = weatherPeriodStart;
            this.weatherPeriodEnd = weatherPeriodEnd;
            this.userEmail = userEmail;
            this.createdAt = createdAt;

            if (surfaceSolarRadiations != null)
            {
                this.surfaceSolarRadiations = [];
                foreach (SurfaceSolarRadiation? surfaceSolarRadiation in surfaceSolarRadiations)
                {
                    if (Core.Query.Clone(surfaceSolarRadiation) is SurfaceSolarRadiation surfaceSolarRadiation_Temp)
                    {
                        this.surfaceSolarRadiations.Add(surfaceSolarRadiation_Temp);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingSolarRadiationResult"/> class by copying an existing instance.
        /// </summary>
        /// <param name="buildingSolarRadiationResult">The <see cref="BuildingSolarRadiationResult"/> to copy.</param>
        public BuildingSolarRadiationResult(BuildingSolarRadiationResult? buildingSolarRadiationResult)
            : base(buildingSolarRadiationResult)
        {
            if (buildingSolarRadiationResult != null)
            {
                reference = buildingSolarRadiationResult.reference;
                countyId = buildingSolarRadiationResult.countyId;
                buildingModelId = buildingSolarRadiationResult.buildingModelId;
                radius = buildingSolarRadiationResult.radius;
                solverVersion = buildingSolarRadiationResult.solverVersion;
                weatherSourceType = buildingSolarRadiationResult.weatherSourceType;
                weatherSourceId = buildingSolarRadiationResult.weatherSourceId;
                weatherPeriodStart = buildingSolarRadiationResult.weatherPeriodStart;
                weatherPeriodEnd = buildingSolarRadiationResult.weatherPeriodEnd;
                userEmail = buildingSolarRadiationResult.userEmail;
                createdAt = buildingSolarRadiationResult.createdAt;

                if (buildingSolarRadiationResult.surfaceSolarRadiations != null)
                {
                    surfaceSolarRadiations = [];
                    foreach (SurfaceSolarRadiation surfaceSolarRadiation in buildingSolarRadiationResult.surfaceSolarRadiations)
                    {
                        if (Core.Query.Clone(surfaceSolarRadiation) is SurfaceSolarRadiation surfaceSolarRadiation_Temp)
                        {
                            surfaceSolarRadiations.Add(surfaceSolarRadiation_Temp);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingSolarRadiationResult"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the building solar radiation data.</param>
        public BuildingSolarRadiationResult(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the id of the solved building model; a regenerated model has a new id, so its stored result no longer matches.
        /// </summary>
        [JsonIgnore]
        public long BuildingModelId
        {
            get
            {
                return buildingModelId;
            }
        }

        /// <summary>
        /// Gets the id of the county part the building is filed under.
        /// </summary>
        [JsonIgnore]
        public int CountyId
        {
            get
            {
                return countyId;
            }
        }

        /// <summary>
        /// Gets when the result was stored, or null.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? CreatedAt
        {
            get
            {
                return createdAt;
            }
        }

        /// <summary>
        /// Gets the neighbour radius, in m, as rounded by the producer (0.1 m) so that it compares exactly.
        /// </summary>
        [JsonIgnore]
        public double Radius
        {
            get
            {
                return radius;
            }
        }

        /// <summary>
        /// Gets the <c>building_2d</c> reference of the building.
        /// </summary>
        [JsonIgnore]
        public string? Reference
        {
            get
            {
                return reference;
            }
        }

        /// <summary>
        /// Gets the version of the solver, its options and the aggregation that produced the shading factors. A stored result with an older version no longer matches.
        /// </summary>
        [JsonIgnore]
        public int SolverVersion
        {
            get
            {
                return solverVersion;
            }
        }

        /// <summary>
        /// Gets clones of the stored radiation of each receiving surface, or null.
        /// </summary>
        [JsonIgnore]
        public List<SurfaceSolarRadiation>? SurfaceSolarRadiations
        {
            get
            {
                if (surfaceSolarRadiations is null)
                {
                    return null;
                }

                List<SurfaceSolarRadiation> result = [];
                foreach (SurfaceSolarRadiation surfaceSolarRadiation in surfaceSolarRadiations)
                {
                    if (Core.Query.Clone(surfaceSolarRadiation) is SurfaceSolarRadiation surfaceSolarRadiation_Temp)
                    {
                        result.Add(surfaceSolarRadiation_Temp);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the email of the signed-in user who stored the result, or null. The server sets it from the user's token, never from a client.
        /// </summary>
        [JsonIgnore]
        public string? UserEmail
        {
            get
            {
                return userEmail;
            }
        }

        /// <summary>
        /// Gets the end of the weather period the summary covers, or null (together with <see cref="WeatherPeriodStart"/>) for a typical year.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? WeatherPeriodEnd
        {
            get
            {
                return weatherPeriodEnd;
            }
        }

        /// <summary>
        /// Gets the start of the weather period the summary covers, or null (together with <see cref="WeatherPeriodEnd"/>) for a typical year.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? WeatherPeriodStart
        {
            get
            {
                return weatherPeriodStart;
            }
        }

        /// <summary>
        /// Gets the id of the weather source, as an invariant string: the <c>epw_file</c> id for <see cref="WeatherSourceType.EPW"/>, the provider or station id for <see cref="WeatherSourceType.Live"/>.
        /// </summary>
        [JsonIgnore]
        public string? WeatherSourceId
        {
            get
            {
                return weatherSourceId;
            }
        }

        /// <summary>
        /// Gets the kind of weather data the summary values were calculated from; <see cref="WeatherSourceType.Undefined"/> when the member is missing.
        /// </summary>
        [JsonIgnore]
        public WeatherSourceType WeatherSourceType
        {
            get
            {
                return weatherSourceType;
            }
        }
    }
}
