using DiGi.Core.Classes;
using DiGi.GIS.Enums;
using DiGi.GIS.Interfaces;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents one footprint of a fixed, ordered building shape labelling sample, together with the <see cref="BuildingShapeSolver"/> result it was drawn under.
    /// <para>Every annotator labels the items of one sample in the same <see cref="Ordinal"/> order, which inter-annotator agreement (Cohen's kappa) requires. A <see cref="Reference"/> is unique only within its <see cref="CountyId"/>.</para>
    /// </summary>
    public class BuildingShapeSampleItem : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(SampleKey))]
        private readonly string? sampleKey;

        [JsonInclude, JsonPropertyName(nameof(Ordinal))]
        private readonly int ordinal;

        [JsonInclude, JsonPropertyName(nameof(CountyId))]
        private readonly int countyId;

        [JsonInclude, JsonPropertyName(nameof(Reference))]
        private readonly string? reference;

        [JsonInclude, JsonPropertyName(nameof(SolverShape))]
        private readonly BuildingShape solverShape = BuildingShape.Undefined;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShapeSampleItem"/> class.
        /// </summary>
        /// <param name="sampleKey">The identity of the sample the item belongs to.</param>
        /// <param name="ordinal">The zero-based position of the item in the labelling order of the sample.</param>
        /// <param name="countyId">The county id (not code) of the part holding the footprint.</param>
        /// <param name="reference">The building reference, unique only within <paramref name="countyId"/>.</param>
        /// <param name="solverShape">The <see cref="BuildingShapeSolver"/> result with default thresholds at sampling time.</param>
        public BuildingShapeSampleItem(string? sampleKey, int ordinal, int countyId, string? reference, BuildingShape solverShape)
        {
            this.sampleKey = sampleKey;
            this.ordinal = ordinal;
            this.countyId = countyId;
            this.reference = reference;
            this.solverShape = solverShape;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShapeSampleItem"/> class by copying an existing instance.
        /// </summary>
        /// <param name="buildingShapeSampleItem">The source item to copy from.</param>
        public BuildingShapeSampleItem(BuildingShapeSampleItem? buildingShapeSampleItem)
            : base(buildingShapeSampleItem)
        {
            if (buildingShapeSampleItem != null)
            {
                sampleKey = buildingShapeSampleItem.sampleKey;
                ordinal = buildingShapeSampleItem.ordinal;
                countyId = buildingShapeSampleItem.countyId;
                reference = buildingShapeSampleItem.reference;
                solverShape = buildingShapeSampleItem.solverShape;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShapeSampleItem"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the item data.</param>
        public BuildingShapeSampleItem(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the identity of the sample the item belongs to.
        /// </summary>
        [JsonIgnore]
        public string? SampleKey
        {
            get
            {
                return sampleKey;
            }
        }

        /// <summary>
        /// Gets the zero-based position of the item in the labelling order of the sample.
        /// </summary>
        [JsonIgnore]
        public int Ordinal
        {
            get
            {
                return ordinal;
            }
        }

        /// <summary>
        /// Gets the county id (not code) of the part holding the footprint.
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
        /// Gets the building reference, unique only within <see cref="CountyId"/>.
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
        /// Gets the <see cref="BuildingShapeSolver"/> result with default thresholds at sampling time. It is never shown to an annotator before labelling.
        /// </summary>
        [JsonIgnore]
        public BuildingShape SolverShape
        {
            get
            {
                return solverShape;
            }
        }
    }
}
