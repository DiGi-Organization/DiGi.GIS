using DiGi.Core.Classes;
using DiGi.GIS.Enums;
using DiGi.GIS.Interfaces;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents the assignment of one <see cref="Classes.AddressPoint"/> to a <see cref="Building2D"/> footprint, or the report that it has none.
    /// <para>Created by <c>Create.AddressPointMatchResult</c>: one match per address point, so a building can receive any number of points while a point belongs to at most one building.</para>
    /// </summary>
    public class AddressPointMatch : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(AddressPoint))]
        private readonly AddressPoint? addressPoint;

        [JsonInclude, JsonPropertyName(nameof(AddressPointMatchType))]
        private readonly AddressPointMatchType addressPointMatchType = AddressPointMatchType.Undefined;

        [JsonInclude, JsonPropertyName(nameof(Distance))]
        private readonly double? distance;

        [JsonInclude, JsonPropertyName(nameof(Guid))]
        private readonly Guid? guid;

        [JsonInclude, JsonPropertyName(nameof(Reference))]
        private readonly string? reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPointMatch"/> class.
        /// </summary>
        /// <param name="addressPoint">The matched address point. It is cloned.</param>
        /// <param name="addressPointMatchType">How the point was assigned.</param>
        /// <param name="reference">The reference of the assigned building, or null when unmatched.</param>
        /// <param name="guid">The guid of the assigned building, or null when unmatched.</param>
        /// <param name="distance">The distance, in m, from the point to the assigned footprint (0 for <see cref="AddressPointMatchType.Inside"/>), or null when unmatched.</param>
        public AddressPointMatch(AddressPoint? addressPoint, AddressPointMatchType addressPointMatchType, string? reference, Guid? guid, double? distance)
        {
            this.addressPoint = Core.Query.Clone(addressPoint);
            this.addressPointMatchType = addressPointMatchType;
            this.reference = reference;
            this.guid = guid;
            this.distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPointMatch"/> class by copying an existing instance.
        /// </summary>
        /// <param name="addressPointMatch">The source match to copy from.</param>
        public AddressPointMatch(AddressPointMatch? addressPointMatch)
            : base(addressPointMatch)
        {
            if (addressPointMatch != null)
            {
                addressPoint = Core.Query.Clone(addressPointMatch.addressPoint);
                addressPointMatchType = addressPointMatch.addressPointMatchType;
                reference = addressPointMatch.reference;
                guid = addressPointMatch.guid;
                distance = addressPointMatch.distance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPointMatch"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the match data.</param>
        public AddressPointMatch(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets a clone of the matched address point.
        /// </summary>
        [JsonIgnore]
        public AddressPoint? AddressPoint
        {
            get
            {
                return Core.Query.Clone(addressPoint);
            }
        }

        /// <summary>
        /// Gets how the point was assigned; <see cref="AddressPointMatchType.Undefined"/> when the member is missing.
        /// </summary>
        [JsonIgnore]
        public AddressPointMatchType AddressPointMatchType
        {
            get
            {
                return addressPointMatchType;
            }
        }

        /// <summary>
        /// Gets the distance, in m, from the point to the assigned footprint (0 for <see cref="AddressPointMatchType.Inside"/>), or null when unmatched.
        /// </summary>
        [JsonIgnore]
        public double? Distance
        {
            get
            {
                return distance;
            }
        }

        /// <summary>
        /// Gets the guid of the assigned building, or null when unmatched.
        /// </summary>
        [JsonIgnore]
        public Guid? Guid
        {
            get
            {
                return guid;
            }
        }

        /// <summary>
        /// Gets the reference of the assigned building, or null when unmatched.
        /// </summary>
        [JsonIgnore]
        public string? Reference
        {
            get
            {
                return reference;
            }
        }
    }
}
