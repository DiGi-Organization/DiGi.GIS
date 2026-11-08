using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.Typology.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DiGi.GIS
{
    public static partial class Convert
    {
        /// <summary>
        /// Converts the specified typology into an array of strings, where each string represents a row
        /// in a tab-separated format containing the hierarchy path and associated metadata.
        /// </summary>
        /// <param name="typology">The typology instance to convert.</param>
        /// <returns>An array of tab-separated strings representing the typology structure, or an empty array if no data is found.</returns>
        public static string[]? ToSystem_Strings(this Typology.Classes.Typology? typology)
        {
            List<TypologyPath>? typologyPaths = Typology.Query.TypologyPaths(typology, true);
            if (typologyPaths is null || typologyPaths.Count == 0)
            {
                return [];
            }

            List<Typology.Classes.Typology> subTypologies = [];

            foreach (TypologyPath typologyPath in typologyPaths)
            {
                Typology.Classes.Typology? subTypology = Typology.Query.SubTypology(typology, typologyPath);

                if (subTypology?.References is null || subTypology.References.Count == 0)
                {
                    continue;
                }

                subTypologies.Add(subTypology);
            }

            if (subTypologies.Count == 0)
            {
                return [];
            }

            subTypologies.Sort();

            List<string[]> values = [];

            Typology.Classes.Typology? typology_Header = subTypologies.Find(x => x.TypologyPath?.GetTypologyPaths() is List<TypologyPath> typologyPaths_Parent);
            if (typology_Header is not null)
            {
                List<TypologyPath> typologyPaths_Parent = typology_Header.TypologyPath?.GetTypologyPaths()!;

                string[] values_Typology = new string[typologyPaths_Parent.Count + 2];

                for (int i = 0; i < typologyPaths_Parent.Count; i++)
                {
                    TypologyPath typologyPath_Parent = typologyPaths_Parent[i];

                    Typology.Classes.Typology? typology_Parent = Typology.Query.SubTypology(typology, typologyPath_Parent);
                    if (typology_Parent is null)
                    {
                        continue;
                    }

                    values_Typology[i] = typology_Parent.Description ?? "???";
                }

                values_Typology[values_Typology.Length - 2] = typology_Header.Description ?? "???";

                values_Typology[values_Typology.Length - 1] = "Count";

                values.Add(values_Typology);
            }

            foreach (Typology.Classes.Typology subTypology in subTypologies)
            {
                if (subTypology.TypologyPath?.GetTypologyPaths() is not List<TypologyPath> typologyPaths_Parent)
                {
                    continue;
                }

                string[] values_Typology = new string[typologyPaths_Parent.Count + 2];

                for (int i = 0; i < typologyPaths_Parent.Count; i++)
                {
                    TypologyPath typologyPath_Parent = typologyPaths_Parent[i];

                    Typology.Classes.Typology? typology_Parent = Typology.Query.SubTypology(typology, typologyPath_Parent);
                    if (typology_Parent is null)
                    {
                        continue;
                    }

                    values_Typology[i] = typology_Parent?.Name is null ? "???" : typology_Parent.Name;
                }

                values_Typology[values_Typology.Length - 2] = subTypology.Name is null ? "???" : subTypology.Name;

                values_Typology[values_Typology.Length - 1] = subTypology.References.Count.ToString(CultureInfo.CurrentCulture);

                values.Add(values_Typology);
            }

            return [.. values.ConvertAll(x => string.Join("\t", x))];
        }

        /// <summary>
        /// Converts orthophoto index rows to tab-separated lines, a header first: the sheet inventory table, one row per registration, with the probe date.
        /// <para>Numbers render with <see cref="CultureInfo.InvariantCulture"/> and dates as <c>yyyy-MM-dd</c> (probe date as ISO 8601 with offset), so a table written on any machine reads the same. Tabs and line breaks inside values are replaced by spaces.</para>
        /// </summary>
        /// <param name="ortoSheets">The rows; nulls are skipped.</param>
        /// <returns>The lines, or null when the rows are null.</returns>
        public static string[]? ToSystem_Strings(this IEnumerable<OrtoSheet?>? ortoSheets)
        {
            if (ortoSheets == null)
            {
                return null;
            }

            List<string> result =
            [
                string.Join("\t", "SheetId", "TypeName", "Year", "Date", "DatePrecision", "PixelSize", "ColorMode", "OrtoType", "Source", "CoordinateSystem", "ArchiveModule", "ApplicationNumber", "SheetFilled", "RegisterDate", "FileSize", "MinEasting", "MinNorthing", "MaxEasting", "MaxNorthing", "Usability", "UsabilityVersion", "Obstacles", "DownloadUrl", "ProbeDate")
            ];

            foreach (OrtoSheet? ortoSheet in ortoSheets)
            {
                if (ortoSheet == null)
                {
                    continue;
                }

                BoundingBox2D? boundingBox2D = ortoSheet.Footprint?.GetBoundingBox();

                result.Add(string.Join(
                    "\t",
                    Text(ortoSheet.SheetId),
                    Text(ortoSheet.TypeName),
                    ortoSheet.Year.ToString(CultureInfo.InvariantCulture),
                    Date(ortoSheet.Date),
                    ortoSheet.DatePrecision.ToString(),
                    Number(ortoSheet.PixelSize),
                    ortoSheet.ColorMode.ToString(),
                    ortoSheet.OrtoType.ToString(),
                    Text(ortoSheet.Source),
                    Text(ortoSheet.CoordinateSystem),
                    Text(ortoSheet.ArchiveModule),
                    Text(ortoSheet.ApplicationNumber),
                    ortoSheet.SheetFilled == null ? string.Empty : (ortoSheet.SheetFilled.Value ? "true" : "false"),
                    Date(ortoSheet.RegisterDate),
                    Number(ortoSheet.FileSize),
                    Number(boundingBox2D?.Min.X),
                    Number(boundingBox2D?.Min.Y),
                    Number(boundingBox2D?.Max.X),
                    Number(boundingBox2D?.Max.Y),
                    Number(Query.OrtoSheetUsability(ortoSheet)),
                    Constants.OrtoUsability.Version.ToString(CultureInfo.InvariantCulture),
                    Query.OrtoObstacles(ortoSheet).ToString().Replace(", ", "|"),
                    Text(ortoSheet.DownloadUrl),
                    ortoSheet.ProbeDate?.ToString("o", CultureInfo.InvariantCulture) ?? string.Empty));
            }

            return [.. result];

            static string Text(string? value)
            {
                return value == null ? string.Empty : value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
            }

            static string Date(DateTimeOffset? value)
            {
                return value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;
            }

            static string Number(double? value)
            {
                return value == null ? string.Empty : (value.Value == 0 ? 0 : value.Value).ToString("R", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Converts an orthophoto availability matrix to tab-separated lines, a header first: one row per cell and epoch, with the gap in days to the previous epoch of the cell.
        /// <para>The gap of a cell's first epoch is empty (left censored); <c>DaysToProbe</c> on its last epoch is the right-censored interval up to the probe date. Numbers render with <see cref="CultureInfo.InvariantCulture"/>.</para>
        /// </summary>
        /// <param name="ortoAvailabilityMatrix">The matrix.</param>
        /// <returns>The lines, or null when the matrix is null.</returns>
        public static string[]? ToSystem_Strings(this OrtoAvailabilityMatrix? ortoAvailabilityMatrix)
        {
            if (ortoAvailabilityMatrix == null)
            {
                return null;
            }

            List<string> result =
            [
                string.Join("\t", "LocationKey", "MinEasting", "MinNorthing", "CellSize", "CoverageFraction", "EpochIndex", "EpochCount", "Date", "DatePrecision", "PixelSize", "GapDays", "DaysToProbe", "ProbeDate")
            ];

            string cellSize = ortoAvailabilityMatrix.CellSize.ToString("R", CultureInfo.InvariantCulture);
            string coverageFraction = ortoAvailabilityMatrix.CoverageFraction.ToString("R", CultureInfo.InvariantCulture);

            foreach (OrtoAvailability ortoAvailability in ortoAvailabilityMatrix.OrtoAvailabilities ?? [])
            {
                List<OrtoEpoch> ortoEpochs = ortoAvailability.OrtoEpochs ?? [];
                List<int> gapDays = ortoAvailability.GapDays ?? [];
                BoundingBox2D? boundingBox2D = ortoAvailability.BoundingBox2D;
                DateTimeOffset? probeDate = ortoAvailability.ProbeDate;

                for (int i = 0; i < ortoEpochs.Count; i++)
                {
                    OrtoEpoch ortoEpoch = ortoEpochs[i];

                    string daysToProbe = i == ortoEpochs.Count - 1 && probeDate != null ? ((int)Math.Round((probeDate.Value - ortoEpoch.Date).TotalDays)).ToString(CultureInfo.InvariantCulture) : string.Empty;

                    result.Add(string.Join(
                        "\t",
                        ortoAvailability.LocationKey ?? string.Empty,
                        boundingBox2D?.Min.X.ToString("R", CultureInfo.InvariantCulture) ?? string.Empty,
                        boundingBox2D?.Min.Y.ToString("R", CultureInfo.InvariantCulture) ?? string.Empty,
                        cellSize,
                        coverageFraction,
                        i.ToString(CultureInfo.InvariantCulture),
                        ortoEpochs.Count.ToString(CultureInfo.InvariantCulture),
                        ortoEpoch.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ortoEpoch.DatePrecision.ToString(),
                        ortoEpoch.PixelSize?.ToString("R", CultureInfo.InvariantCulture) ?? string.Empty,
                        i == 0 || i - 1 >= gapDays.Count ? string.Empty : gapDays[i - 1].ToString(CultureInfo.InvariantCulture),
                        daysToProbe,
                        probeDate?.ToString("o", CultureInfo.InvariantCulture) ?? string.Empty));
                }
            }

            return [.. result];
        }
    }
}