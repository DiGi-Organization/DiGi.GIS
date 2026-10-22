using DiGi.Core;
using DiGi.Core.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Interfaces;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents a PRG address point: a position in EPSG:2180 with the street, house number, locality and TERYT identifiers it was registered under.
    /// <para>It keeps the full PRG record, which <see cref="DiGi.Core.Classes.Address"/> cannot hold (no house number, no TERYT ids). Use <c>ToDiGi(this AddressPoint?)</c> for the lossy conversion to <see cref="DiGi.Core.Classes.Address"/>.</para>
    /// </summary>
    public class AddressPoint : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(HouseNumber))]
        private readonly string houseNumber = string.Empty;

        [JsonInclude, JsonPropertyName(nameof(LocalityName))]
        private readonly string? localityName;

        [JsonInclude, JsonPropertyName(nameof(Location))]
        private readonly Point2D? location;

        [JsonInclude, JsonPropertyName(nameof(PostalCode))]
        private readonly string? postalCode;

        [JsonInclude, JsonPropertyName(nameof(ReferenceId))]
        private readonly string? referenceId;

        [JsonInclude, JsonPropertyName(nameof(SIMC))]
        private readonly string? simc;

        [JsonInclude, JsonPropertyName(nameof(StreetName))]
        private readonly string? streetName;

        [JsonInclude, JsonPropertyName(nameof(ULIC))]
        private readonly string? ulic;

        [JsonInclude, JsonPropertyName(nameof(Version))]
        private readonly DateTimeOffset? version;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPoint"/> class.
        /// </summary>
        /// <param name="location">The position of the address point in EPSG:2180 (easting first).</param>
        /// <param name="streetName">The full street name, or null for a locality without streets.</param>
        /// <param name="ulic">The TERYT ULIC street identifier (a string, leading zeros are significant), or null.</param>
        /// <param name="houseNumber">The house number as free text (for example <c>6A</c>, <c>2/4</c>).</param>
        /// <param name="localityName">The locality name.</param>
        /// <param name="simc">The TERYT SIMC locality identifier (a string, leading zeros are significant).</param>
        /// <param name="postalCode">The postal code, or null.</param>
        /// <param name="referenceId">The PRG local identifier of the address point.</param>
        /// <param name="version">The PRG version of the address point.</param>
        public AddressPoint(Point2D? location, string? streetName, string? ulic, string? houseNumber, string? localityName, string? simc, string? postalCode, string? referenceId, DateTimeOffset? version)
        {
            this.location = location?.Clone<Point2D>();
            this.streetName = streetName;
            this.ulic = ulic;
            this.houseNumber = houseNumber ?? string.Empty;
            this.localityName = localityName;
            this.simc = simc;
            this.postalCode = postalCode;
            this.referenceId = referenceId;
            this.version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPoint"/> class by copying an existing instance.
        /// </summary>
        /// <param name="addressPoint">The source address point to copy from.</param>
        public AddressPoint(AddressPoint? addressPoint)
            : base(addressPoint)
        {
            if (addressPoint != null)
            {
                location = addressPoint.location?.Clone<Point2D>();
                streetName = addressPoint.streetName;
                ulic = addressPoint.ulic;
                houseNumber = addressPoint.houseNumber;
                localityName = addressPoint.localityName;
                simc = addressPoint.simc;
                postalCode = addressPoint.postalCode;
                referenceId = addressPoint.referenceId;
                version = addressPoint.version;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressPoint"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the address point data.</param>
        public AddressPoint(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the house number as free text.
        /// </summary>
        [JsonIgnore]
        public string HouseNumber
        {
            get
            {
                return houseNumber;
            }
        }

        /// <summary>
        /// Gets the locality name.
        /// </summary>
        [JsonIgnore]
        public string? LocalityName
        {
            get
            {
                return localityName;
            }
        }

        /// <summary>
        /// Gets a clone of the position in EPSG:2180 (easting first).
        /// </summary>
        [JsonIgnore]
        public Point2D? Location
        {
            get
            {
                return location?.Clone<Point2D>();
            }
        }

        /// <summary>
        /// Gets the postal code.
        /// </summary>
        [JsonIgnore]
        public string? PostalCode
        {
            get
            {
                return postalCode;
            }
        }

        /// <summary>
        /// Gets the PRG local identifier of the address point.
        /// </summary>
        [JsonIgnore]
        public string? ReferenceId
        {
            get
            {
                return referenceId;
            }
        }

        /// <summary>
        /// Gets the TERYT SIMC locality identifier.
        /// </summary>
        [JsonIgnore]
        public string? SIMC
        {
            get
            {
                return simc;
            }
        }

        /// <summary>
        /// Gets the full street name, or null for a locality without streets.
        /// </summary>
        [JsonIgnore]
        public string? StreetName
        {
            get
            {
                return streetName;
            }
        }

        /// <summary>
        /// Gets the TERYT ULIC street identifier.
        /// </summary>
        [JsonIgnore]
        public string? ULIC
        {
            get
            {
                return ulic;
            }
        }

        /// <summary>
        /// Gets the PRG version of the address point.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? Version
        {
            get
            {
                return version;
            }
        }
    }
}
