using DiGi.Core.Classes;
using DiGi.GIS.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.GIS.Classes
{
    /// <summary>
    /// Represents an orthophoto availability matrix over a regular EPSG:2180 grid, together with the options it was built with, so a stored table is self-describing.
    /// <para>Build with <see cref="Create.OrtoAvailabilityMatrix(System.Collections.Generic.IEnumerable{OrtoSheet?}?, double, double, DateTimeOffset?)"/>; this constructor stores what it is given.</para>
    /// </summary>
    public class OrtoAvailabilityMatrix : SerializableObject, IGISSerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(CellSize))]
        private readonly double cellSize;

        [JsonInclude, JsonPropertyName(nameof(CoverageFraction))]
        private readonly double coverageFraction;

        [JsonInclude, JsonPropertyName(nameof(OrtoAvailabilities))]
        private readonly List<OrtoAvailability>? ortoAvailabilities;

        [JsonInclude, JsonPropertyName(nameof(ProbeDate))]
        private readonly DateTimeOffset? probeDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoAvailabilityMatrix"/> class.
        /// </summary>
        /// <param name="cellSize">The grid cell size in metres.</param>
        /// <param name="coverageFraction">The fraction of a cell the union of the footprints of one date must cover for that date to count at that cell.</param>
        /// <param name="ortoAvailabilities">The cells of the matrix.</param>
        /// <param name="probeDate">The moment the index was read.</param>
        public OrtoAvailabilityMatrix(double cellSize, double coverageFraction, IEnumerable<OrtoAvailability>? ortoAvailabilities, DateTimeOffset? probeDate)
        {
            this.cellSize = cellSize;
            this.coverageFraction = coverageFraction;
            this.ortoAvailabilities = Clone(ortoAvailabilities);
            this.probeDate = probeDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoAvailabilityMatrix"/> class by copying an existing instance.
        /// </summary>
        /// <param name="ortoAvailabilityMatrix">The source matrix to copy from.</param>
        public OrtoAvailabilityMatrix(OrtoAvailabilityMatrix? ortoAvailabilityMatrix)
            : base(ortoAvailabilityMatrix)
        {
            if (ortoAvailabilityMatrix != null)
            {
                cellSize = ortoAvailabilityMatrix.cellSize;
                coverageFraction = ortoAvailabilityMatrix.coverageFraction;
                ortoAvailabilities = Clone(ortoAvailabilityMatrix.ortoAvailabilities);
                probeDate = ortoAvailabilityMatrix.probeDate;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrtoAvailabilityMatrix"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the matrix data.</param>
        public OrtoAvailabilityMatrix(JsonObject? jsonObject)
            : base(jsonObject)
        {
        }

        /// <summary>
        /// Gets the grid cell size in metres.
        /// </summary>
        [JsonIgnore]
        public double CellSize
        {
            get
            {
                return cellSize;
            }
        }

        /// <summary>
        /// Gets the fraction of a cell the union of the footprints of one date must cover for that date to count at that cell.
        /// </summary>
        [JsonIgnore]
        public double CoverageFraction
        {
            get
            {
                return coverageFraction;
            }
        }

        /// <summary>
        /// Gets clones of the cells of the matrix.
        /// </summary>
        [JsonIgnore]
        public List<OrtoAvailability>? OrtoAvailabilities
        {
            get
            {
                return Clone(ortoAvailabilities);
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

        private static List<OrtoAvailability>? Clone(IEnumerable<OrtoAvailability?>? ortoAvailabilities)
        {
            if (ortoAvailabilities == null)
            {
                return null;
            }

            List<OrtoAvailability> result = [];
            foreach (OrtoAvailability? ortoAvailability in ortoAvailabilities)
            {
                if (Core.Query.Clone(ortoAvailability) is OrtoAvailability ortoAvailability_Temp)
                {
                    result.Add(ortoAvailability_Temp);
                }
            }

            return result;
        }
    }
}
