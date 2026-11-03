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
    /// Represents one row of the GUGiK per-sheet orthophoto index: one sheet as registered under one yearly typename of <see cref="Constants.WebService.OrtoSheetIndex"/>.
    /// <para>A row is a registration, not an image: the same image can be registered more than once (each registration gets a fresh download file), and the same sheet identifier recurs for genuine re-acquisitions. <see cref="Query.OrtoSheetDuplicateGroups(System.Collections.Generic.IEnumerable{OrtoSheet?}?)"/> tells the two apart.</para>
    /// <para>Build from GML with <c>Convert.ToDiGi_OrtoSheet</c>, which classifies <see cref="DatePrecision"/>; this constructor stores what it is given.</para>
    /// </summary>
    public class OrtoSheet : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(ApplicationNumber))]
        private readonly string? applicationNumber;

        [JsonInclude, JsonPropertyName(nameof(ArchiveModule))]
        private readonly string? archiveModule;

        [JsonInclude, JsonPropertyName(nameof(ColorMode))]
        private readonly OrtoColorMode colorMode = OrtoColorMode.Undefined;

        [JsonInclude, JsonPropertyName(nameof(CoordinateSystem))]
        private readonly string? coordinateSystem;

        [JsonInclude, JsonPropertyName(nameof(Date))]
        private readonly DateTimeOffset? date;

        [JsonInclude, JsonPropertyName(nameof(DatePrecision))]
        private readonly OrtoDatePrecision datePrecision = OrtoDatePrecision.Undefined;

        [JsonInclude, JsonPropertyName(nameof(DownloadUrl))]
        private readonly string? downloadUrl;

        [JsonInclude, JsonPropertyName(nameof(FileSize))]
        private readonly double? fileSize;

        [JsonInclude, JsonPropertyName(nameof(Footprint))]
        private readonly Polygon2D? footprint;

        [JsonInclude, JsonPropertyName(nameof(OrtoType))]
        private readonly OrtoType ortoType = OrtoType.Undefined;

        [JsonInclude, JsonPropertyName(nameof(PixelSize))]
        private readonly double? pixelSize;

        [JsonInclude, JsonPropertyName(nameof(ProbeDate))]
        private readonly DateTimeOffset? probeDate;

        [JsonInclude, JsonPropertyName(nameof(RegisterDate))]
        private readonly DateTimeOffset? registerDate;

        [JsonInclude, JsonPropertyName(nameof(SheetFilled))]
        private readonly bool? sheetFilled;

        [JsonInclude, JsonPropertyName(nameof(SheetId))]
        private readonly string? sheetId;

        [JsonInclude, JsonPropertyName(nameof(Source))]
        private readonly string? source;

        [JsonInclude, JsonPropertyName(nameof(TypeName))]
        private readonly string? typeName;

        [JsonInclude, JsonPropertyName(nameof(Year))]
        private readonly int year;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoSheet"/> class.
        /// </summary>
        /// <param name="sheetId">The sheet identifier (<c>godlo</c>). An opaque grouping key: map sheet codes and TERYT-like ids both occur, and it is not unique within a typename.</param>
        /// <param name="typeName">The yearly typename the row was read from.</param>
        /// <param name="year">The year the row is filed under (<c>akt_rok</c>).</param>
        /// <param name="date">The acquisition date (<c>akt_data</c>), at precision <paramref name="datePrecision"/>.</param>
        /// <param name="datePrecision">The precision of <paramref name="date"/>.</param>
        /// <param name="pixelSize">The ground pixel size in metres (<c>piksel</c>).</param>
        /// <param name="colorMode">The colour mode (<c>kolor</c>).</param>
        /// <param name="source">The source of the imagery as published (<c>zrodlo_danych</c>).</param>
        /// <param name="coordinateSystem">The coordinate system of the sheet (<c>uklad_xy</c>).</param>
        /// <param name="archiveModule">The archive module (<c>modul_archiwizacji</c>).</param>
        /// <param name="applicationNumber">The PZGiK application number (<c>nr_zglosz</c>).</param>
        /// <param name="sheetFilled">Whether the sheet is completely filled with imagery (<c>czy_ark_wypelniony</c>).</param>
        /// <param name="downloadUrl">The download URL of the sheet file (<c>url_do_pobrania</c>).</param>
        /// <param name="registerDate">The PZGiK registration date (<c>dt_pzgik</c>); not an acquisition date.</param>
        /// <param name="fileSize">The file size in megabytes (<c>wlk_pliku_mb</c>).</param>
        /// <param name="footprint">The sheet footprint in EPSG:2180, easting in X.</param>
        /// <param name="ortoType">The orthophoto type.</param>
        /// <param name="probeDate">The moment the index was read.</param>
        public OrtoSheet(
            string? sheetId,
            string? typeName,
            int year,
            DateTimeOffset? date,
            OrtoDatePrecision datePrecision,
            double? pixelSize,
            OrtoColorMode colorMode,
            string? source,
            string? coordinateSystem,
            string? archiveModule,
            string? applicationNumber,
            bool? sheetFilled,
            string? downloadUrl,
            DateTimeOffset? registerDate,
            double? fileSize,
            Polygon2D? footprint,
            OrtoType ortoType,
            DateTimeOffset? probeDate)
        {
            this.sheetId = sheetId;
            this.typeName = typeName;
            this.year = year;
            this.date = date;
            this.datePrecision = datePrecision;
            this.pixelSize = pixelSize;
            this.colorMode = colorMode;
            this.source = source;
            this.coordinateSystem = coordinateSystem;
            this.archiveModule = archiveModule;
            this.applicationNumber = applicationNumber;
            this.sheetFilled = sheetFilled;
            this.downloadUrl = downloadUrl;
            this.registerDate = registerDate;
            this.fileSize = fileSize;
            this.footprint = Core.Query.Clone(footprint);
            this.ortoType = ortoType;
            this.probeDate = probeDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoSheet"/> class by copying an existing instance.
        /// </summary>
        /// <param name="ortoSheet">The source sheet to copy from.</param>
        public OrtoSheet(OrtoSheet? ortoSheet)
            : base(ortoSheet)
        {
            if (ortoSheet != null)
            {
                sheetId = ortoSheet.sheetId;
                typeName = ortoSheet.typeName;
                year = ortoSheet.year;
                date = ortoSheet.date;
                datePrecision = ortoSheet.datePrecision;
                pixelSize = ortoSheet.pixelSize;
                colorMode = ortoSheet.colorMode;
                source = ortoSheet.source;
                coordinateSystem = ortoSheet.coordinateSystem;
                archiveModule = ortoSheet.archiveModule;
                applicationNumber = ortoSheet.applicationNumber;
                sheetFilled = ortoSheet.sheetFilled;
                downloadUrl = ortoSheet.downloadUrl;
                registerDate = ortoSheet.registerDate;
                fileSize = ortoSheet.fileSize;
                footprint = Core.Query.Clone(ortoSheet.footprint);
                ortoType = ortoSheet.ortoType;
                probeDate = ortoSheet.probeDate;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoSheet"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the sheet data.</param>
        public OrtoSheet(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the PZGiK application number (<c>nr_zglosz</c>).
        /// </summary>
        [JsonIgnore]
        public string? ApplicationNumber
        {
            get
            {
                return applicationNumber;
            }
        }

        /// <summary>
        /// Gets the archive module (<c>modul_archiwizacji</c>).
        /// </summary>
        [JsonIgnore]
        public string? ArchiveModule
        {
            get
            {
                return archiveModule;
            }
        }

        /// <summary>
        /// Gets the colour mode (<c>kolor</c>).
        /// </summary>
        [JsonIgnore]
        public OrtoColorMode ColorMode
        {
            get
            {
                return colorMode;
            }
        }

        /// <summary>
        /// Gets the coordinate system of the sheet (<c>uklad_xy</c>).
        /// </summary>
        [JsonIgnore]
        public string? CoordinateSystem
        {
            get
            {
                return coordinateSystem;
            }
        }

        /// <summary>
        /// Gets the acquisition date (<c>akt_data</c>), at precision <see cref="DatePrecision"/>.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? Date
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
        /// Gets the download URL of the sheet file (<c>url_do_pobrania</c>).
        /// </summary>
        [JsonIgnore]
        public string? DownloadUrl
        {
            get
            {
                return downloadUrl;
            }
        }

        /// <summary>
        /// Gets the file size in megabytes (<c>wlk_pliku_mb</c>).
        /// </summary>
        [JsonIgnore]
        public double? FileSize
        {
            get
            {
                return fileSize;
            }
        }

        /// <summary>
        /// Gets a clone of the sheet footprint in EPSG:2180, easting in X.
        /// </summary>
        [JsonIgnore]
        public Polygon2D? Footprint
        {
            get
            {
                return Core.Query.Clone(footprint);
            }
        }

        /// <summary>
        /// Gets the orthophoto type.
        /// </summary>
        [JsonIgnore]
        public OrtoType OrtoType
        {
            get
            {
                return ortoType;
            }
        }

        /// <summary>
        /// Gets the ground pixel size in metres (<c>piksel</c>).
        /// </summary>
        [JsonIgnore]
        public double? PixelSize
        {
            get
            {
                return pixelSize;
            }
        }

        /// <summary>
        /// Gets the moment the index was read.
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
        /// Gets the PZGiK registration date (<c>dt_pzgik</c>). Not an acquisition date.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? RegisterDate
        {
            get
            {
                return registerDate;
            }
        }

        /// <summary>
        /// Gets whether the sheet is completely filled with imagery (<c>czy_ark_wypelniony</c>).
        /// </summary>
        [JsonIgnore]
        public bool? SheetFilled
        {
            get
            {
                return sheetFilled;
            }
        }

        /// <summary>
        /// Gets the sheet identifier (<c>godlo</c>). An opaque grouping key: never parse it, and never assume it is unique within a typename.
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
        /// Gets the source of the imagery as published (<c>zrodlo_danych</c>, for example <c>Zdj. analogowe</c> or <c>Zdj. cyfrowe</c>).
        /// </summary>
        [JsonIgnore]
        public string? Source
        {
            get
            {
                return source;
            }
        }

        /// <summary>
        /// Gets the yearly typename the row was read from.
        /// </summary>
        [JsonIgnore]
        public string? TypeName
        {
            get
            {
                return typeName;
            }
        }

        /// <summary>
        /// Gets the year the row is filed under (<c>akt_rok</c>).
        /// </summary>
        [JsonIgnore]
        public int Year
        {
            get
            {
                return year;
            }
        }
    }
}
