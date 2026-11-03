using DiGi.Core;
using DiGi.Core.Classes;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Planar.Interfaces;
using DiGi.GIS.Enums;
using DiGi.GIS.Interfaces;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents orthophoto data, containing the image bytes, timestamp, location, and scale factor.
    /// <para><see cref="DateTime"/> is the year requested from the time-enabled WMS, not the date of the photograph: for a year with no flight the service answers the nearest earlier imagery (DiGi.GIS#17). The photograph's date is <see cref="AcquisitionDate"/>, read from the orthophoto sheet index (<see cref="Query.AcquisitionOrtoSheet"/>), and identical images share <see cref="ContentHash"/>.</para>
    /// </summary>
    public class OrtoData : SerializableObject, IGISObject
    {
        [JsonInclude, JsonPropertyName(nameof(AcquisitionDate))]
        private readonly DateTimeOffset? acquisitionDate;

        // Initialized so a row stored before this member existed reads as Undefined, not as Day (0).
        [JsonInclude, JsonPropertyName(nameof(AcquisitionDatePrecision))]
        private readonly OrtoDatePrecision acquisitionDatePrecision = OrtoDatePrecision.Undefined;

        [JsonInclude, JsonPropertyName("Bytes")]
        private readonly byte[]? bytes;

        [JsonInclude, JsonPropertyName(nameof(ContentHash))]
        private readonly string? contentHash;

        [JsonInclude, JsonPropertyName("DateTime")]
        private readonly DateTime dateTime;

        [JsonInclude, JsonPropertyName("Location")]
        private readonly Point2D? location;

        [JsonInclude, JsonPropertyName("Scale")]
        private readonly double scale;

        [JsonInclude, JsonPropertyName(nameof(SheetId))]
        private readonly string? sheetId;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoData"/> class.
        /// </summary>
        /// <param name="dateTime">The year requested from the time-enabled WMS, as 1 January of that year.</param>
        /// <param name="bytes">The bitmap image stored as bytes.</param>
        /// <param name="scale">The scale factor of the image.</param>
        /// <param name="location">The top-left corner coordinates of the picture.</param>
        /// <param name="acquisitionDate">The date the photograph was taken, from the orthophoto sheet index, or null when no sheet was resolved.</param>
        /// <param name="acquisitionDatePrecision">The precision of <paramref name="acquisitionDate"/>; <see cref="OrtoDatePrecision.Undefined"/> when no sheet was resolved.</param>
        /// <param name="sheetId">The identifier of the index sheet the date was read from.</param>
        /// <param name="contentHash">The SHA-256 of <paramref name="bytes"/> (<see cref="Query.ContentHash(byte[])"/>).</param>
        public OrtoData(
            DateTime dateTime,
            byte[]? bytes,
            double scale,
            Point2D? location,
            DateTimeOffset? acquisitionDate = null,
            OrtoDatePrecision acquisitionDatePrecision = OrtoDatePrecision.Undefined,
            string? sheetId = null,
            string? contentHash = null)
        {
            this.dateTime = dateTime;
            this.bytes = bytes;
            this.scale = scale;
            this.location = location;
            this.acquisitionDate = acquisitionDate;
            this.acquisitionDatePrecision = acquisitionDatePrecision;
            this.sheetId = sheetId;
            this.contentHash = contentHash;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoData"/> class by copying an existing instance.
        /// </summary>
        /// <param name="ortoData">The source orthophoto data to copy from.</param>
        public OrtoData(OrtoData? ortoData)
        {
            if (ortoData != null)
            {
                dateTime = ortoData.dateTime;
                bytes = ortoData.bytes;
                scale = ortoData.scale;
                location = ortoData.location?.Clone<Point2D>();
                acquisitionDate = ortoData.acquisitionDate;
                acquisitionDatePrecision = ortoData.acquisitionDatePrecision;
                sheetId = ortoData.sheetId;
                contentHash = ortoData.contentHash;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoData"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing orthophoto data.</param>
        public OrtoData(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the date the photograph was taken, read from the orthophoto sheet index, or null when no sheet was resolved.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? AcquisitionDate
        {
            get
            {
                return acquisitionDate;
            }
        }

        /// <summary>
        /// Gets the precision of <see cref="AcquisitionDate"/>: <see cref="OrtoDatePrecision.Year"/> for an analog placeholder date, <see cref="OrtoDatePrecision.Undefined"/> when no sheet was resolved (an undated image, never an observation boundary).
        /// </summary>
        [JsonIgnore]
        public OrtoDatePrecision AcquisitionDatePrecision
        {
            get
            {
                return acquisitionDatePrecision;
            }
        }

        /// <summary>
        /// Bitmap image stored in bytes
        /// </summary>
        [JsonIgnore]
        public byte[]? Bytes
        {
            get
            {
                return bytes;
            }
        }

        /// <summary>
        /// Gets the SHA-256 of <see cref="Bytes"/>; identical images share it and are one observation (<see cref="Query.OrtoObservations"/>).
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
        /// Gets the year requested from the time-enabled WMS, as 1 January of that year. It is not the date of the photograph; see <see cref="AcquisitionDate"/>.
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
        }

        /// <summary>
        /// Top left corner of picture stored in bytes
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
        /// Gets the scale factor of the orthophoto.
        /// </summary>
        [JsonIgnore]
        public double Scale
        {
            get
            {
                return scale;
            }
        }

        /// <summary>
        /// Gets the identifier of the orthophoto index sheet <see cref="AcquisitionDate"/> was read from.
        /// </summary>
        [JsonIgnore]
        public string? SheetId
        {
            get
            {
                return sheetId;
            }
        }

        /// <summary>
        /// Transforms a point from orthophoto coordinates to world/GIS coordinates.
        /// </summary>
        /// <param name="point2D">The point in orthophoto coordinates.</param>
        /// <returns>The transformed point in world/GIS coordinates, or null if the input is null.</returns>
        public Point2D? FromOrto(Point2D? point2D)
        {
            if (point2D == null)
            {
                return null;
            }

            Vector2D vector2D = new(point2D.X, -point2D.Y);
            vector2D.Scale(1 / scale);

            return point2D.GetMoved(vector2D);
        }

        /// <summary>
        /// Transforms a polygonal face from orthophoto coordinates to world/GIS coordinates.
        /// </summary>
        /// <param name="polygonalFace2D">The polygonal face in orthophoto coordinates.</param>
        /// <returns>The transformed polygonal face in world/GIS coordinates, or null if the input is null.</returns>
        public PolygonalFace2D? FromOrto(PolygonalFace2D? polygonalFace2D)
        {
            if (polygonalFace2D == null)
            {
                return null;
            }

            return Geometry.Planar.Query.Transform(polygonalFace2D, x => FromOrto(x));
        }

        /// <summary>
        /// Transforms a polygonal object from orthophoto coordinates to world/GIS coordinates.
        /// </summary>
        /// <param name="polygonal2D">The polygonal object in orthophoto coordinates.</param>
        /// <returns>The transformed polygonal object in world/GIS coordinates, or null if the input is null.</returns>
        public IPolygonal2D? FromOrto(IPolygonal2D? polygonal2D)
        {
            if (polygonal2D == null)
            {
                return null;
            }

            return Geometry.Planar.Query.Transform(polygonal2D, x => FromOrto(x));
        }

        /// <summary>
        /// Calculates the bounding box of the orthophoto data based on the specified geometry context.
        /// </summary>
        /// <param name="geometryContext">The coordinate system context (Local or Global) for the bounding box.</param>
        /// <returns>A <see cref="BoundingBox2D"/> representing the area, or null if location is missing or context is undefined.</returns>
        public BoundingBox2D? GetBoundingBox(GeometryContext geometryContext)
        {
            if (location is null || geometryContext == GeometryContext.Undefined)
            {
                return null;
            }

            Size? size = GetSize(geometryContext);
            if (size is null)
            {
                return null;
            }

            if (geometryContext == GeometryContext.Local)
            {
                // Local: Top-Left is (0,0), Bottom-Right is (Width, -Height)
                return new BoundingBox2D(new Point2D(0, -size.Height), new Point2D(size.Width, 0));
            }

            // Global: Top-Left is 'location'.
            // Bottom-Right is location moved by (Width, -Height)
            Point2D minPoint = new(location.X, location.Y - size.Height);
            Point2D maxPoint = new(location.X + size.Width, location.Y);

            return new BoundingBox2D(minPoint, maxPoint);
        }

        /// <summary>
        /// Retrieves the dimensions of the orthophoto image, adjusted for the specified geometry context.
        /// </summary>
        /// <param name="geometryContext">The coordinate system context (Local or Global) to determine scaling.</param>
        /// <returns>A <see cref="Size"/> object containing width and height, or null if undefined or bytes are missing.</returns>
        public Size? GetSize(GeometryContext geometryContext)
        {
            if (geometryContext == GeometryContext.Undefined)
            {
                return null;
            }

            Size? size = Query.Size(bytes);
            if (size is null)
            {
                return null;
            }

            if (geometryContext == GeometryContext.Local)
            {
                return size;
            }

            return size.GetScaled(1 / scale);
        }

        /// <summary>
        /// Transforms a point from world/GIS coordinates to orthophoto coordinates.
        /// </summary>
        /// <param name="point2D">The point in world/GIS coordinates.</param>
        /// <returns>The transformed point in orthophoto coordinates, or null if the input is null.</returns>
        public Point2D? ToOrto(Point2D? point2D)
        {
            if (point2D == null)
            {
                return null;
            }

            Vector2D vector2D = new(location, point2D);

            vector2D.Scale(scale);

            return new Point2D(vector2D.X, -vector2D.Y);
        }

        /// <summary>
        /// Transforms a polygonal object from world/GIS coordinates to orthophoto coordinates.
        /// </summary>
        /// <param name="polygonal2D">The polygonal object in world/GIS coordinates.</param>
        /// <returns>The transformed polygonal object in orthophoto coordinates, or null if the input is null.</returns>
        public IPolygonal2D? ToOrto(IPolygonal2D? polygonal2D)
        {
            if (polygonal2D == null)
            {
                return null;
            }

            return Geometry.Planar.Query.Transform(polygonal2D, x => ToOrto(x));
        }

        /// <summary>
        /// Transforms a polygonal face from world/GIS coordinates to orthophoto coordinates.
        /// </summary>
        /// <param name="polygonalFace2D">The polygonal face in world/GIS coordinates.</param>
        /// <returns>The transformed polygonal face in orthophoto coordinates, or null if the input is null.</returns>
        public PolygonalFace2D? ToOrto(PolygonalFace2D? polygonalFace2D)
        {
            if (polygonalFace2D == null)
            {
                return null;
            }

            return Geometry.Planar.Query.Transform(polygonalFace2D, x => ToOrto(x));
        }
    }
}