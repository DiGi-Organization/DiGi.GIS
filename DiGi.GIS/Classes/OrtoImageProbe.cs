using DiGi.Core.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Enums;
using DiGi.GIS.Interfaces;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents one observation of a time-enabled orthophoto WMS: what the service served for one tile at one requested time.
    /// <para>The index (<see cref="OrtoSheet"/>) is the primary record; probes verify it. Two probes of one tile at different times with the same <see cref="ContentHash"/> are one served image, not two epochs.</para>
    /// </summary>
    public class OrtoImageProbe : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(BoundingBox2D))]
        private readonly BoundingBox2D? boundingBox2D;

        [JsonInclude, JsonPropertyName(nameof(ContentHash))]
        private readonly string? contentHash;

        [JsonInclude, JsonPropertyName(nameof(Layer))]
        private readonly string? layer;

        [JsonInclude, JsonPropertyName(nameof(OrtoImageProbeStatus))]
        private readonly OrtoImageProbeStatus ortoImageProbeStatus = OrtoImageProbeStatus.Undefined;

        [JsonInclude, JsonPropertyName(nameof(ProbeDate))]
        private readonly DateTimeOffset? probeDate;

        [JsonInclude, JsonPropertyName(nameof(ServiceUrl))]
        private readonly string? serviceUrl;

        [JsonInclude, JsonPropertyName(nameof(Time))]
        private readonly string? time;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoImageProbe"/> class.
        /// </summary>
        /// <param name="boundingBox2D">The probed tile in EPSG:2180, easting in X.</param>
        /// <param name="serviceUrl">The WMS endpoint that was asked.</param>
        /// <param name="layer">The WMS layer that was asked for.</param>
        /// <param name="time">The requested <c>TIME</c> value, as sent.</param>
        /// <param name="ortoImageProbeStatus">The classification of the response.</param>
        /// <param name="contentHash">The SHA-256 of the response bytes; set only for <see cref="OrtoImageProbeStatus.Image"/>.</param>
        /// <param name="probeDate">The moment the request was answered.</param>
        public OrtoImageProbe(BoundingBox2D? boundingBox2D, string? serviceUrl, string? layer, string? time, OrtoImageProbeStatus ortoImageProbeStatus, string? contentHash, DateTimeOffset? probeDate)
        {
            this.boundingBox2D = Core.Query.Clone(boundingBox2D);
            this.serviceUrl = serviceUrl;
            this.layer = layer;
            this.time = time;
            this.ortoImageProbeStatus = ortoImageProbeStatus;
            this.contentHash = contentHash;
            this.probeDate = probeDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoImageProbe"/> class by copying an existing instance.
        /// </summary>
        /// <param name="ortoImageProbe">The source probe to copy from.</param>
        public OrtoImageProbe(OrtoImageProbe? ortoImageProbe)
            : base(ortoImageProbe)
        {
            if (ortoImageProbe != null)
            {
                boundingBox2D = Core.Query.Clone(ortoImageProbe.boundingBox2D);
                serviceUrl = ortoImageProbe.serviceUrl;
                layer = ortoImageProbe.layer;
                time = ortoImageProbe.time;
                ortoImageProbeStatus = ortoImageProbe.ortoImageProbeStatus;
                contentHash = ortoImageProbe.contentHash;
                probeDate = ortoImageProbe.probeDate;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoImageProbe"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the probe data.</param>
        public OrtoImageProbe(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets a clone of the probed tile in EPSG:2180, easting in X.
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
        /// Gets the SHA-256 of the response bytes as lowercase hexadecimal; null unless <see cref="OrtoImageProbeStatus"/> is <see cref="OrtoImageProbeStatus.Image"/>.
        /// </summary>
        [JsonIgnore]
        public string? ContentHash
        {
            get
            {
                return contentHash;
            }
        }

        /// <summary>
        /// Gets the WMS layer that was asked for.
        /// </summary>
        [JsonIgnore]
        public string? Layer
        {
            get
            {
                return layer;
            }
        }

        /// <summary>
        /// Gets the classification of the response.
        /// </summary>
        [JsonIgnore]
        public OrtoImageProbeStatus OrtoImageProbeStatus
        {
            get
            {
                return ortoImageProbeStatus;
            }
        }

        /// <summary>
        /// Gets the moment the request was answered.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? ProbeDate
        {
            get
            {
                return probeDate;
            }
        }

        /// <summary>
        /// Gets the WMS endpoint that was asked.
        /// </summary>
        [JsonIgnore]
        public string? ServiceUrl
        {
            get
            {
                return serviceUrl;
            }
        }

        /// <summary>
        /// Gets the requested <c>TIME</c> value, as sent.
        /// </summary>
        [JsonIgnore]
        public string? Time
        {
            get
            {
                return time;
            }
        }
    }
}
