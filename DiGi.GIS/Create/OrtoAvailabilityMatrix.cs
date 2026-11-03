using DiGi.Geometry.Planar.Classes;
using DiGi.GIS.Classes;
using DiGi.GIS.Enums;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.Operation.Union;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiGi.GIS
{
    public static partial class Create
    {
        /// <summary>
        /// Builds an orthophoto availability matrix over a regular EPSG:2180 grid from index rows: per cell, the distinct epochs and the gaps in days between them.
        /// <para>The grid is aligned to multiples of <paramref name="cellSize"/> and spans the footprints. A date counts at a cell when the union of the footprints carrying that date covers at least <paramref name="coverageFraction"/> of the cell area — the union, because one 1:5 000 sheet (about 2 km) never covers a 5 km cell on its own while its campaign does; every 1998 cell came out empty under a per-sheet rule. Sheets of one block flown on different days do not add up, so a cell straddling them needs a smaller cell or a lower fraction. Cells no date covers are not emitted.</para>
        /// <para>Duplicates are removed before epochs are formed: the rows are grouped by <see cref="Query.OrtoSheetDuplicateGroups(IEnumerable{OrtoSheet?}?)"/> over the whole input, each group contributes its canonical row once, and rows with the same date at a cell form one epoch (its precision the best of theirs, its pixel size the finest). Rows without a date or footprint are ignored.</para>
        /// <para>The first epoch of a cell is left censored and the last is right censored at the probe date; gaps before 1995 measure index coverage, not necessarily the absence of imagery.</para>
        /// <para>The footprints are indexed in an NTS <see cref="STRtree{TItem}"/> built inside the call; nothing is cached between calls.</para>
        /// </summary>
        /// <param name="ortoSheets">The index rows; nulls are ignored.</param>
        /// <param name="cellSize">The grid cell size in metres.</param>
        /// <param name="coverageFraction">The fraction of a cell a footprint must cover, in (0, 1].</param>
        /// <param name="probeDate">The moment the index was read; null takes the latest <see cref="OrtoSheet.ProbeDate"/> of the rows.</param>
        /// <returns>The matrix, or null when the rows are null, the cell size is not positive or the fraction is outside (0, 1].</returns>
        public static OrtoAvailabilityMatrix? OrtoAvailabilityMatrix(this IEnumerable<OrtoSheet?>? ortoSheets, double cellSize = 5000, double coverageFraction = 0.9, DateTimeOffset? probeDate = null)
        {
            if (ortoSheets == null || double.IsNaN(cellSize) || cellSize <= 0 || double.IsNaN(coverageFraction) || coverageFraction <= 0 || coverageFraction > 1)
            {
                return null;
            }

            List<OrtoSheet> ortoSheets_Temp = [];
            foreach (OrtoSheet? ortoSheet in ortoSheets)
            {
                if (ortoSheet != null)
                {
                    ortoSheets_Temp.Add(ortoSheet);
                }
            }

            DateTimeOffset? probeDate_Temp = probeDate;
            if (probeDate_Temp == null)
            {
                foreach (OrtoSheet ortoSheet in ortoSheets_Temp)
                {
                    if (ortoSheet.ProbeDate is DateTimeOffset probeDate_Sheet && (probeDate_Temp == null || probeDate_Sheet > probeDate_Temp.Value))
                    {
                        probeDate_Temp = probeDate_Sheet;
                    }
                }
            }

            // One canonical row per duplicate group, decided over the whole input so a group split across cells stays one image.
            List<OrtoSheet> ortoSheets_Canonical = [];
            foreach (List<OrtoSheet> group in Query.OrtoSheetDuplicateGroups(ortoSheets_Temp) ?? [])
            {
                OrtoSheet? ortoSheet = group.Find(x => x.Date != null && x.Footprint != null);
                if (ortoSheet != null)
                {
                    ortoSheets_Canonical.Add(ortoSheet);
                }
            }

            List<OrtoSheet> ortoSheets_Indexed = [];
            List<Polygon> polygons = [];
            STRtree<int> sTRtree = new();

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (OrtoSheet ortoSheet in ortoSheets_Canonical)
            {
                Polygon2D? polygon2D = ortoSheet.Footprint;
                Polygon? polygon = Geometry.Planar.Convert.ToNTS_Polygon(polygon2D);
                if (polygon == null || polygon.IsEmpty || !polygon.IsValid)
                {
                    continue;
                }

                Envelope envelope = polygon.EnvelopeInternal;
                sTRtree.Insert(envelope, polygons.Count);
                polygons.Add(polygon);
                ortoSheets_Indexed.Add(ortoSheet);

                minX = Math.Min(minX, envelope.MinX);
                minY = Math.Min(minY, envelope.MinY);
                maxX = Math.Max(maxX, envelope.MaxX);
                maxY = Math.Max(maxY, envelope.MaxY);
            }

            List<OrtoAvailability> ortoAvailabilities = [];
            if (polygons.Count == 0)
            {
                return new OrtoAvailabilityMatrix(cellSize, coverageFraction, ortoAvailabilities, probeDate_Temp);
            }

            sTRtree.Build();

            GeometryFactory geometryFactory = new();
            double area_Cell = cellSize * cellSize;

            long indexX_Min = (long)Math.Floor(minX / cellSize);
            long indexX_Max = (long)Math.Ceiling(maxX / cellSize);
            long indexY_Min = (long)Math.Floor(minY / cellSize);
            long indexY_Max = (long)Math.Ceiling(maxY / cellSize);

            for (long indexX = indexX_Min; indexX < indexX_Max; indexX++)
            {
                for (long indexY = indexY_Min; indexY < indexY_Max; indexY++)
                {
                    double x = indexX * cellSize;
                    double y = indexY * cellSize;

                    Envelope envelope_Cell = new(x, x + cellSize, y, y + cellSize);
                    NetTopologySuite.Geometries.Geometry geometry_Cell = geometryFactory.ToGeometry(envelope_Cell);

                    // Per date, the sheets reaching into the cell and the parts of the cell they cover.
                    Dictionary<long, List<OrtoSheet>> dictionary = [];
                    Dictionary<long, List<NetTopologySuite.Geometries.Geometry>> dictionary_Geometries = [];
                    foreach (int index in sTRtree.Query(envelope_Cell))
                    {
                        NetTopologySuite.Geometries.Geometry geometry_Intersection = polygons[index].Intersection(geometry_Cell);
                        if (geometry_Intersection.IsEmpty || geometry_Intersection.Area <= 0)
                        {
                            continue;
                        }

                        OrtoSheet ortoSheet = ortoSheets_Indexed[index];
                        long ticks = ortoSheet.Date!.Value.UtcTicks;
                        if (!dictionary.TryGetValue(ticks, out List<OrtoSheet>? ortoSheets_Date))
                        {
                            ortoSheets_Date = [];
                            dictionary[ticks] = ortoSheets_Date;
                            dictionary_Geometries[ticks] = [];
                        }

                        ortoSheets_Date.Add(ortoSheet);
                        dictionary_Geometries[ticks].Add(geometry_Intersection);
                    }

                    foreach (KeyValuePair<long, List<NetTopologySuite.Geometries.Geometry>> keyValuePair in dictionary_Geometries)
                    {
                        double area = keyValuePair.Value.Count == 1 ? keyValuePair.Value[0].Area : UnaryUnionOp.Union(keyValuePair.Value).Area;
                        if (area / area_Cell < coverageFraction)
                        {
                            dictionary.Remove(keyValuePair.Key);
                        }
                    }

                    if (dictionary.Count == 0)
                    {
                        continue;
                    }

                    List<OrtoEpoch> ortoEpochs = [];
                    foreach (KeyValuePair<long, List<OrtoSheet>> keyValuePair in dictionary.OrderBy(x => x.Key))
                    {
                        List<OrtoSheet> ortoSheets_Date = keyValuePair.Value;

                        OrtoDatePrecision ortoDatePrecision = OrtoDatePrecision.Undefined;
                        double? pixelSize = null;
                        List<string> downloadUrls = [];
                        foreach (OrtoSheet ortoSheet in ortoSheets_Date)
                        {
                            if (ortoSheet.DatePrecision != OrtoDatePrecision.Undefined && (ortoDatePrecision == OrtoDatePrecision.Undefined || ortoSheet.DatePrecision < ortoDatePrecision))
                            {
                                ortoDatePrecision = ortoSheet.DatePrecision;
                            }

                            if (ortoSheet.PixelSize is double pixelSize_Sheet && (pixelSize == null || pixelSize_Sheet < pixelSize.Value))
                            {
                                pixelSize = pixelSize_Sheet;
                            }

                            if (!string.IsNullOrWhiteSpace(ortoSheet.DownloadUrl))
                            {
                                downloadUrls.Add(ortoSheet.DownloadUrl!);
                            }
                        }

                        downloadUrls.Sort(StringComparer.Ordinal);

                        ortoEpochs.Add(new OrtoEpoch(ortoSheets_Date[0].Date!.Value, ortoDatePrecision, pixelSize, downloadUrls));
                    }

                    List<int> gapDays = [];
                    for (int i = 1; i < ortoEpochs.Count; i++)
                    {
                        gapDays.Add((int)Math.Round((ortoEpochs[i].Date - ortoEpochs[i - 1].Date).TotalDays));
                    }

                    BoundingBox2D boundingBox2D = new(new Point2D(x, y), new Point2D(x + cellSize, y + cellSize));

                    ortoAvailabilities.Add(new OrtoAvailability(Query.OrtoLocationKey(boundingBox2D), boundingBox2D, ortoEpochs, gapDays, probeDate_Temp));
                }
            }

            return new OrtoAvailabilityMatrix(cellSize, coverageFraction, ortoAvailabilities, probeDate_Temp);
        }
    }
}
