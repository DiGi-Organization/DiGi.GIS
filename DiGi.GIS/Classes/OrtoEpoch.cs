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
    /// Represents one distinct observation moment at a location: every duplicate registration of the same image collapsed into one.
    /// </summary>
    public class OrtoEpoch : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(Date))]
        private readonly DateTimeOffset date;

        [JsonInclude, JsonPropertyName(nameof(DatePrecision))]
        private readonly OrtoDatePrecision datePrecision = OrtoDatePrecision.Undefined;

        [JsonInclude, JsonPropertyName(nameof(DownloadUrls))]
        private readonly List<string>? downloadUrls;

        [JsonInclude, JsonPropertyName(nameof(PixelSize))]
        private readonly double? pixelSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoEpoch"/> class.
        /// </summary>
        /// <param name="date">The acquisition date of the epoch.</param>
        /// <param name="datePrecision">The precision of <paramref name="date"/>.</param>
        /// <param name="pixelSize">The finest ground pixel size in metres among the sheets of the epoch.</param>
        /// <param name="downloadUrls">The download URLs of the canonical sheets the epoch was built from, one per duplicate group.</param>
        public OrtoEpoch(DateTimeOffset date, OrtoDatePrecision datePrecision, double? pixelSize, IEnumerable<string>? downloadUrls)
        {
            this.date = date;
            this.datePrecision = datePrecision;
            this.pixelSize = pixelSize;
            this.downloadUrls = downloadUrls == null ? null : new List<string>(downloadUrls);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoEpoch"/> class by copying an existing instance.
        /// </summary>
        /// <param name="ortoEpoch">The source epoch to copy from.</param>
        public OrtoEpoch(OrtoEpoch? ortoEpoch)
            : base(ortoEpoch)
        {
            if (ortoEpoch != null)
            {
                date = ortoEpoch.date;
                datePrecision = ortoEpoch.datePrecision;
                pixelSize = ortoEpoch.pixelSize;
                downloadUrls = ortoEpoch.downloadUrls == null ? null : new List<string>(ortoEpoch.downloadUrls);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoEpoch"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the epoch data.</param>
        public OrtoEpoch(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the acquisition date of the epoch.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset Date
        {
            get
            {
                return date;
            }
        }

        /// <summary>
        /// Gets the precision of <see cref="Date"/>.
        /// </summary>
        [JsonIgnore]
        public OrtoDatePrecision DatePrecision
        {
            get
            {
                return datePrecision;
            }
        }

        /// <summary>
        /// Gets a copy of the download URLs of the canonical sheets the epoch was built from.
        /// </summary>
        [JsonIgnore]
        public List<string>? DownloadUrls
        {
            get
            {
                return downloadUrls == null ? null : new List<string>(downloadUrls);
            }
        }

        /// <summary>
        /// Gets the finest ground pixel size in metres among the sheets of the epoch.
        /// </summary>
        [JsonIgnore]
        public double? PixelSize
        {
            get
            {
                return pixelSize;
            }
        }
    }
}
