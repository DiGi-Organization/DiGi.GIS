using DiGi.Core.Classes;
using DiGi.GIS.Enums;
using DiGi.GIS.Interfaces;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents one annotator's manual building shape label of one <see cref="BuildingShapeSampleItem"/>.
    /// <para>One label is kept per sample item and interpreter; labelling the item again replaces it. The interpreter and timestamp are stamped by the server from the session, never supplied by the client.</para>
    /// </summary>
    public class BuildingShapeLabel : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(SampleKey))]
        private readonly string? sampleKey;

        [JsonInclude, JsonPropertyName(nameof(CountyId))]
        private readonly int countyId;

        [JsonInclude, JsonPropertyName(nameof(Reference))]
        private readonly string? reference;

        [JsonInclude, JsonPropertyName(nameof(BuildingShape))]
        private readonly BuildingShape buildingShape = BuildingShape.Undefined;

        [JsonInclude, JsonPropertyName(nameof(Interpreter))]
        private readonly string? interpreter;

        [JsonInclude, JsonPropertyName(nameof(Note))]
        private readonly string? note;

        [JsonInclude, JsonPropertyName(nameof(DateTime))]
        private readonly DateTimeOffset dateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShapeLabel"/> class.
        /// </summary>
        /// <param name="sampleKey">The identity of the sample the labelled item belongs to.</param>
        /// <param name="countyId">The county id (not code) of the part holding the footprint.</param>
        /// <param name="reference">The building reference, unique only within <paramref name="countyId"/>.</param>
        /// <param name="buildingShape">The label; <see cref="BuildingShape.Undefined"/> means the annotator could not classify the footprint.</param>
        /// <param name="interpreter">Who recorded the label.</param>
        /// <param name="note">An optional free-text note.</param>
        /// <param name="dateTime">When the label was recorded (UTC).</param>
        public BuildingShapeLabel(string? sampleKey, int countyId, string? reference, BuildingShape buildingShape, string? interpreter, string? note, DateTimeOffset dateTime)
        {
            this.sampleKey = sampleKey;
            this.countyId = countyId;
            this.reference = reference;
            this.buildingShape = buildingShape;
            this.interpreter = interpreter;
            this.note = note;
            this.dateTime = dateTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShapeLabel"/> class by copying an existing instance.
        /// </summary>
        /// <param name="buildingShapeLabel">The source label to copy from.</param>
        public BuildingShapeLabel(BuildingShapeLabel? buildingShapeLabel)
            : base(buildingShapeLabel)
        {
            if (buildingShapeLabel != null)
            {
                sampleKey = buildingShapeLabel.sampleKey;
                countyId = buildingShapeLabel.countyId;
                reference = buildingShapeLabel.reference;
                buildingShape = buildingShapeLabel.buildingShape;
                interpreter = buildingShapeLabel.interpreter;
                note = buildingShapeLabel.note;
                dateTime = buildingShapeLabel.dateTime;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShapeLabel"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the label data.</param>
        public BuildingShapeLabel(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the identity of the sample the labelled item belongs to.
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
        /// Gets the label; <see cref="BuildingShape.Undefined"/> means the annotator could not classify the footprint.
        /// </summary>
        [JsonIgnore]
        public BuildingShape BuildingShape
        {
            get
            {
                return buildingShape;
            }
        }

        /// <summary>
        /// Gets who recorded the label.
        /// </summary>
        [JsonIgnore]
        public string? Interpreter
        {
            get
            {
                return interpreter;
            }
        }

        /// <summary>
        /// Gets the optional free-text note.
        /// </summary>
        [JsonIgnore]
        public string? Note
        {
            get
            {
                return note;
            }
        }

        /// <summary>
        /// Gets when the label was recorded (UTC).
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset DateTime
        {
            get
            {
                return dateTime;
            }
        }
    }
}
